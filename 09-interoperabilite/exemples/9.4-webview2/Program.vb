' ============================================================================
'  Section 9.4 : WebView2 — point d'entrée (WinForms)
'  Description : Démarre l'application Windows Forms. Réinitialise le journal
'                d'auto-test puis lance la boucle de messages sur la fenêtre
'                hôte. <STAThread> est requis pour WinForms (et WebView2).
'  Fichier source : 04-webview2.md
' ============================================================================

Imports System
Imports System.IO
Imports System.Windows.Forms

Module Program
    <STAThread>
    Sub Main()
        ' Repartir d'un journal propre pour l'auto-test.
        Try
            File.Delete(Path.Combine(Path.GetTempPath(), "webview2-autotest.log"))
        Catch
        End Try

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New FenetrePrincipale())
    End Sub
End Module
