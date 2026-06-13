' ============================================================================
'  Section 9.4 : WebView2 — fenêtre hôte (WinForms)
'  Description : Le formulaire portant le contrôle WebView2. Initialise
'                CoreWebView2 en ASYNCHRONE (le piège n°1 : tout accès avant que
'                EnsureCoreWebView2Async ait rendu la main échoue), expose un
'                objet hôte, branche la messagerie, mappe un dossier local sur un
'                hôte virtuel, navigue, puis — à la réception du message — lit le
'                titre par ExecuteScriptAsync. En mode auto-test (DEMO_AUTOCLOSE=1),
'                journalise chaque étape dans %TEMP% et ferme la fenêtre.
'  Fichier source : 04-webview2.md
' ============================================================================

Imports System
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms

Public Class FenetrePrincipale
    Inherits Form

    Private ReadOnly _webView As New WebView2()
    Private ReadOnly _autotest As Boolean = (Environment.GetEnvironmentVariable("DEMO_AUTOCLOSE") = "1")
    Private ReadOnly _journal As String = Path.Combine(Path.GetTempPath(), "webview2-autotest.log")

    Public Sub New()
        Text = "9.4 WebView2 — VB.NET"
        Width = 900
        Height = 600
        _webView.Dock = DockStyle.Fill
        Controls.Add(_webView)

        AddHandler Load, AddressOf AuChargement

        ' Filet de sécurité en auto-test : fermer même si WebView2 ne répond pas.
        If _autotest Then
            Dim chien As New Timer() With {.Interval = 20000}
            AddHandler chien.Tick, Sub()
                                       chien.Stop()
                                       Journaliser("TIMEOUT : pas de message reçu sous 20 s")
                                       Close()
                                   End Sub
            chien.Start()
        End If
    End Sub

    Private Async Sub AuChargement(sender As Object, e As EventArgs)
        Try
            ' OBLIGATOIRE avant tout usage : sans cela, CoreWebView2 vaut Nothing.
            Await _webView.EnsureCoreWebView2Async()
            Journaliser("EnsureCoreWebView2Async OK ; version=" & _webView.CoreWebView2.Environment.BrowserVersionString)

            ' Objet hôte : exposer du .NET à JavaScript (via COM -> <ComVisible>).
            _webView.CoreWebView2.AddHostObjectToScript("pont", New Pont())

            ' JS -> .NET : messagerie.
            AddHandler _webView.CoreWebView2.WebMessageReceived, AddressOf AuMessage

            ' Contenu local servi via un nom d'hôte virtuel (préférable à file://).
            Dim dossier = Path.Combine(AppContext.BaseDirectory, "WebAssets")
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "appassets", dossier, CoreWebView2HostResourceAccessKind.Allow)

            _webView.CoreWebView2.Navigate("https://appassets/index.html")
        Catch ex As Exception
            Journaliser($"ERREUR init : {ex.GetType().Name} : {ex.Message}")
            FermerSiAutotest()
        End Try
    End Sub

    Private Async Sub AuMessage(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)
        ' 2) JS -> .NET : message reçu de la page.
        Dim brut = e.TryGetWebMessageAsString()
        Journaliser("WebMessageReceived = " & brut)

        ' 1) .NET -> JS : exécuter du script, résultat sérialisé en JSON (titre entre guillemets).
        Dim titreJson = Await _webView.CoreWebView2.ExecuteScriptAsync("document.title")
        Journaliser("ExecuteScriptAsync(document.title) = " & titreJson)

        FermerSiAutotest()
    End Sub

    Private Sub Journaliser(ligne As String)
        Try
            File.AppendAllText(_journal, ligne & Environment.NewLine)
        Catch
        End Try
        Console.WriteLine(ligne)
    End Sub

    Private Sub FermerSiAutotest()
        If Not _autotest Then Return
        ' Laisser la boucle de messages se vider, puis fermer.
        Dim t As New Timer() With {.Interval = 400}
        AddHandler t.Tick, Sub()
                               t.Stop()
                               Close()
                           End Sub
        t.Start()
    End Sub
End Class
