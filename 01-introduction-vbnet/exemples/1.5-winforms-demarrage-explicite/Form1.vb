' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Le gestionnaire d'événement exact montré dans la section :
'                la clause Handles relie la méthode Button1_Click à
'                l'événement Click du bouton — l'idiome VB de gestion des
'                événements (équivalent du « += » de C#, détaillé en 3.6).
'                La classe est partielle : l'autre moitié (les contrôles)
'                vit dans Form1.Designer.vb.
'  Fichier source : 05-premier-projet.md
' ============================================================================

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MessageBox.Show("Bonjour depuis VB.NET !")
    End Sub
End Class
