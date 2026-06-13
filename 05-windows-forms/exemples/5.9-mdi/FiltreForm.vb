' ============================================================================
'  Section 5.9 : Applications MDI et multi-formulaires
'  Description : Communication DÉCOUPLÉE entre formulaires (extrait de la
'                section) : ce formulaire non modal EXPOSE un événement
'                FiltreApplique (avec un EventArgs dédié) auquel le parent
'                s'abonne — sans accès direct aux contrôles de l'autre.
'  Fichier source : 09-mdi.md
' ============================================================================

Public Class FiltreForm

    Public Event FiltreApplique As EventHandler(Of FiltreEventArgs)

    Private Sub btnAppliquer_Click(sender As Object, e As EventArgs) Handles btnAppliquer.Click
        RaiseEvent FiltreApplique(Me, New FiltreEventArgs(txtTerme.Text))
    End Sub

End Class

Public Class FiltreEventArgs
    Inherits EventArgs

    Public ReadOnly Property Terme As String

    Public Sub New(terme As String)
        Me.Terme = terme
    End Sub
End Class
