' ============================================================================
'  Section 5.2 : Windows Forms sur .NET 10 (modernisation)
'  Description : Le type métier sérialisable en JSON de la section, utilisé
'                par le presse-papiers sécurisé (Clipboard.SetDataAsJson /
'                TryGetData(Of T)).
'  Fichier source : 02-winforms-net10.md
' ============================================================================

Public Class Client
    Public Property Nom As String
    Public Property Ville As String
End Class
