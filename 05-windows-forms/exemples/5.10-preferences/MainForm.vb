' ============================================================================
'  Section 5.10 : Préférences et paramètres utilisateur (My.Settings)
'  Description : Reprend tous les extraits de la section :
'                  · lecture/écriture FORTEMENT TYPÉES de My.Settings ;
'                  · le motif Upgrade() (gardé par le booléen MettreAJour)
'                    pour reprendre les valeurs après un changement de version ;
'                  · un COMPTEUR DE LANCEMENTS de portée User, incrémenté et
'                    persisté par My.Settings.Save() — il survit aux
'                    relancements (vérifié dans le journal d'auto-test) ;
'                  · enregistrement à la fermeture (FormClosing).
'  Fichier source : 10-preferences.md
' ============================================================================

Imports System.Windows.Forms

Public Class MainForm

    Private Const JOURNAL As String = "5.10-preferences-autotest.log"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Motif Upgrade() : reprend les valeurs de la version précédente, une seule fois.
        If My.Settings.MettreAJour Then
            My.Settings.Upgrade()
            My.Settings.MettreAJour = False
            My.Settings.Save()
        End If

        ' Compteur de lancements (portée User), persisté.
        My.Settings.CompteurLancements += 1
        My.Settings.Save()

        lblCompteur.Text = $"Nombre de lancements : {My.Settings.CompteurLancements}"
        lblDossier.Text = $"Dernier dossier : {If(String.IsNullOrEmpty(My.Settings.DernierDossier), "(aucun)", My.Settings.DernierDossier)}"
        chkModeSombre.Checked = My.Settings.ModeSombre

        AutoFermeture.Journaliser(JOURNAL, $"Lancement -> CompteurLancements = {My.Settings.CompteurLancements}")
        AutoFermeture.Activer(Me)
    End Sub

    ' Lecture/écriture fortement typée : choisir un dossier et le mémoriser.
    Private Sub btnChoisirDossier_Click(sender As Object, e As EventArgs) Handles btnChoisirDossier.Click
        Using dlg As New FolderBrowserDialog()
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                My.Settings.DernierDossier = dlg.SelectedPath
                lblDossier.Text = $"Dernier dossier : {My.Settings.DernierDossier}"
            End If
        End Using
    End Sub

    Private Sub chkModeSombre_CheckedChanged(sender As Object, e As EventArgs) Handles chkModeSombre.CheckedChanged
        My.Settings.ModeSombre = chkModeSombre.Checked
    End Sub

    ' Enregistrement explicite à la fermeture (la section le recommande).
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        My.Settings.Save()
    End Sub

End Class
