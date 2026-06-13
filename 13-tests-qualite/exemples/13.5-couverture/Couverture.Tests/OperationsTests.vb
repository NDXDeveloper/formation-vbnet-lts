' ============================================================================
'  Section 13.5 : Couverture de code — tests (couverture partielle volontaire)
'  Description : On ne teste QUE Doubler. La couverture mesurée par Coverlet
'                révélera que Tripler n'est pas exercée — l'usage pertinent de
'                la métrique (repérer le non-testé, pas viser un pourcentage).
'  Fichier source : 05-couverture-tests-ia.md
' ============================================================================

Imports Couverture
Imports Xunit

Public Class OperationsTests
    <Fact>
    Public Sub Doubler_RetourneLeDouble()
        Assert.Equal(10, New Operations().Doubler(5))
    End Sub
End Class
