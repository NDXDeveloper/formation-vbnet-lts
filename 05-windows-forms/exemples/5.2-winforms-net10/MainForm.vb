' ============================================================================
'  Section 5.2 : Windows Forms sur .NET 10 (modernisation)
'  Description : Démonstration des trois modernisations de la section :
'                  · MODE SOMBRE : activé via ApplicationEvents.vb
'                    (ApplyApplicationDefaults) ; le formulaire affiche le
'                    mode retenu (Application.ColorMode) et celui du système ;
'                  · FORMULAIRES ASYNC : ShowDialogAsync attendu sans figer
'                    l'interface (gestionnaire Async Sub) ;
'                  · PRESSE-PAPIERS SÉCURISÉ : Clipboard.SetDataAsJson /
'                    TryGetData(Of Client) — round-trip JSON vérifié dans le
'                    journal d'auto-test.
'  Fichier source : 02-winforms-net10.md
' ============================================================================

Imports System.Windows.Forms

Public Class MainForm

    Private Const JOURNAL As String = "5.2-winforms-net10-autotest.log"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Mode sombre : on AFFICHE le mode retenu (réglé dans ApplicationEvents.vb)
        lblColorMode.Text = $"Application.ColorMode = {Application.ColorMode}" & Environment.NewLine &
                            $"Application.SystemColorMode = {Application.SystemColorMode}"
        ' FlatStyle.System rend mieux en mode sombre (cf. section)
        btnModifier.FlatStyle = FlatStyle.System

        VerifierPressePapiers()
        AutoFermeture.Activer(Me)
    End Sub

    ' ---- Formulaires asynchrones : ShowDialogAsync ----
    Private Async Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click
        Using dlg As New ClientEditForm()
            Dim resultat As DialogResult = Await dlg.ShowDialogAsync(Me)
            lblResultatDialogue.Text = $"ShowDialogAsync a renvoyé : {resultat}"
        End Using
    End Sub

    ' ---- Presse-papiers sécurisé : round-trip JSON vérifié ----
    Private Sub VerifierPressePapiers()
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        Dim original As New Client With {.Nom = "Dupont", .Ville = "Rouen"}
        Dim ok As Boolean = False
        Dim recupere As Client = Nothing
        Try
            Clipboard.SetDataAsJson("MonApp.Client", original)
            If Clipboard.ContainsData("MonApp.Client") AndAlso
               Clipboard.TryGetData("MonApp.Client", recupere) Then
                ok = recupere IsNot Nothing AndAlso
                     recupere.Nom = original.Nom AndAlso
                     recupere.Ville = original.Ville
            End If
        Catch ex As Exception
            AutoFermeture.Journaliser(JOURNAL, $"Presse-papiers indisponible : {ex.GetType().Name}")
        End Try

        If recupere IsNot Nothing Then
            lblPressePapiers.Text = $"Relu du presse-papiers (JSON) : {recupere.Nom} — {recupere.Ville}"
        End If
        AutoFermeture.Journaliser(JOURNAL, $"Clipboard round-trip JSON Client : {ok}")
        AutoFermeture.Journaliser(JOURNAL, $"  Nom relu = {recupere?.Nom} ; Ville relue = {recupere?.Ville}")
    End Sub

End Class
