' ============================================================================
'  Section 5.1 : Introduction, architecture et le Concepteur (Designer)
'  Description : VOTRE code du formulaire — gestionnaires d'événements et
'                logique, exactement comme l'épure de la section. La classe
'                n'indique PAS « Inherits System.Windows.Forms.Form » : c'est
'                la partie générée (Form1.Designer.vb) qui le déclare. Le
'                « Handles btnValider.Click » fonctionne parce que btnValider
'                est déclaré « Friend WithEvents » dans le Designer.
'  Fichier source : 01-introduction-designer.md
' ============================================================================

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Code exécuté au chargement du formulaire
        lblInfo.Text = "Form1 = Form1.vb (votre code) + Form1.Designer.vb (Concepteur)," & Environment.NewLine &
                       "reliés par InitializeComponent et WithEvents/Handles."
        AutoFermeture.Activer(Me)   ' smoke test automatique si DEMO_AUTOCLOSE=1
    End Sub

    Private Sub btnValider_Click(sender As Object, e As EventArgs) Handles btnValider.Click
        MessageBox.Show("Bonjour !")
    End Sub

End Class
