' ============================================================================
'  Section 15.3 : CI/CD — tests de la logique métier
'  Description : Tests xUnit vérifiés à chaque exécution du pipeline.
'  Fichier source : 03-cicd.md
' ============================================================================

Imports Calculs
Imports Xunit

Public Class CalculateurTvaTests
    <Theory>
    <InlineData(100, 0.2, 120)>
    <InlineData(50, 0.1, 55)>
    <InlineData(0, 0.2, 0)>
    Public Sub Ttc_CalculeLeMontantAttendu(ht As Double, taux As Double, attendu As Double)
        Dim calc As New CalculateurTva()
        Assert.Equal(CDec(attendu), calc.Ttc(CDec(ht), CDec(taux)))
    End Sub
End Class
