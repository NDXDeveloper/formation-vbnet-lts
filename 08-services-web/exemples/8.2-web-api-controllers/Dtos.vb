' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Les DTO d'entrée, validés par DataAnnotations. Grâce à
'                <ApiController>, un modèle invalide déclenche AUTOMATIQUEMENT
'                une réponse 400 au format Problem Details — sans code en plus.
'                MiseAJourProduit suit le même modèle que CreationProduit.
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System.ComponentModel.DataAnnotations

Public Class CreationProduit
    <Required>
    <StringLength(100, MinimumLength:=2)>
    Public Property Nom As String

    <Range(0.0, 100000.0)>
    Public Property Prix As Decimal

    Public Property EnStock As Boolean
End Class

Public Class MiseAJourProduit
    <Required>
    <StringLength(100, MinimumLength:=2)>
    Public Property Nom As String

    <Range(0.0, 100000.0)>
    Public Property Prix As Decimal

    Public Property EnStock As Boolean
End Class
