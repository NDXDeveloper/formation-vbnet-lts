' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Formulaire de démarrage du projet « cadre applicatif ».
'                Même idiome Handles que dans la variante à démarrage
'                explicite — seule la façon de démarrer l'application change.
'  Fichier source : 05-premier-projet.md
' ============================================================================

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MessageBox.Show("Bonjour depuis le cadre applicatif VB !")
    End Sub

End Class
