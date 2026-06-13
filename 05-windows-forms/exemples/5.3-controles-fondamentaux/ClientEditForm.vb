' ============================================================================
'  Section 5.3 : Contrôles fondamentaux
'  Description : Boîte de dialogue modale personnalisée, exactement selon la
'                section : la donnée saisie est exposée en PROPRIÉTÉ publique
'                (Client), marquée <DesignerSerializationVisibility(Hidden)>
'                pour satisfaire l'analyseur WFO1000 (.NET 9+). Les boutons
'                portent un DialogResult, et AcceptButton/CancelButton sont
'                réglés dans le Designer.
'  Fichier source : 03-controles-fondamentaux.md
' ============================================================================

Imports System.ComponentModel

Public Class ClientEditForm

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Client As Client

    Private Sub ClientEditForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Client IsNot Nothing Then
            txtNom.Text = Client.Nom
            txtVille.Text = Client.Ville
        End If
    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        ' On reporte la saisie dans la propriété exposée, lue par l'appelant.
        Client = New Client With {.Nom = txtNom.Text, .Ville = txtVille.Text}
    End Sub

End Class
