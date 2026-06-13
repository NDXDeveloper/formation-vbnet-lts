' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Le POCO exposé par l'API — le même qu'en 8.1. C'est le type
'                renvoyé par les actions (ActionResult(Of Produit)).
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Public Class Produit
    Public Property Id As Integer
    Public Property Nom As String
    Public Property Prix As Decimal
    Public Property EnStock As Boolean
End Class
