' ============================================================================
'  Section 8.1 : Consommer des API REST
'  Description : Le POCO de la section, correspondant à la forme du JSON.
'                <JsonPropertyName> mappe un nom JSON « en_stock » sur la
'                propriété EnStock (casse Pascal de VB).
'  Fichier source : 01-consommer-api-rest.md
' ============================================================================

Imports System.Text.Json.Serialization

Public Class Produit
    Public Property Id As Integer
    Public Property Nom As String
    Public Property Prix As Decimal

    <JsonPropertyName("en_stock")>
    Public Property EnStock As Boolean
End Class
