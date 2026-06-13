' ============================================================================
'  Section 10.5 : Atelier — type de résultat renvoyé à l'UI (VB)
'  Description : Le résultat de l'import, agrégé par le métier et consommé par
'                l'UI. Réunit les transactions valides, les rejetées, les
'                totaux par catégorie et le total général. Les Transaction qu'il
'                porte viennent du cœur C# (type remonté de façon transitive).
'  Fichier source : 05-atelier-core-csharp-ui-vbnet.md
' ============================================================================

Imports System.Collections.Generic
Imports Importateur.Coeur

Namespace Importateur.Metier

    Public Class ResultatImport
        Public Property Transactions As IReadOnlyList(Of Transaction)
        Public Property Rejetees As IReadOnlyList(Of Transaction)
        Public Property TotauxParCategorie As IReadOnlyDictionary(Of String, Decimal)
        Public Property TotalGeneral As Decimal
    End Class

End Namespace
