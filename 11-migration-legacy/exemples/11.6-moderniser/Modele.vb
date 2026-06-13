' ============================================================================
'  Section 11.6 : Moderniser — modèle de démonstration
'  Description : Petit modèle Client utilisé pour la démonstration LINQ
'                (filtrage des clients actifs).
'  Fichier source : 06-moderniser.md
' ============================================================================

Public Class Client
    Public ReadOnly Property Nom As String
    Public ReadOnly Property EstActif As Boolean

    Public Sub New(nom As String, estActif As Boolean)
        _Nom = nom
        _EstActif = estActif
    End Sub
End Class
