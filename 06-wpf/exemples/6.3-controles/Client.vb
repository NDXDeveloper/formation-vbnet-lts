' ============================================================================
'  Section 6.3 : Contrôles et contrôles de données
'  Description : Type métier affiché par le DataGrid et la ListView, et
'                Produit pour la ListView/GridView.
'  Fichier source : 03-controles.md
' ============================================================================

Public Class Client
    Public Property Nom As String
    Public Property Ville As String
    Public Property EstActif As Boolean
End Class

Public Class Produit
    Public Property Reference As String
    Public Property Designation As String
    Public Property Prix As Decimal
End Class
