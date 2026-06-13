' ============================================================================
'  Section 5.9 : Applications MDI et multi-formulaires
'  Description : Une fenêtre « document » MDI. Elle se rattache au conteneur
'                via MdiParent (réglé par le parent). Un compteur statique
'                numérote les documents ouverts.
'  Fichier source : 09-mdi.md
' ============================================================================

Public Class DocumentForm

    Private Shared _compteur As Integer = 0

    Public Sub New()
        InitializeComponent()
        _compteur += 1
        Me.Text = $"Document {_compteur}"
    End Sub

End Class
