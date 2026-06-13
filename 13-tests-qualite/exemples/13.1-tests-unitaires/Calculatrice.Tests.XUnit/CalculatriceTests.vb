' ============================================================================
'  Section 13.1 : Tests unitaires — xUnit (VB.NET)
'  Description : Démontre la structure Arrange-Act-Assert, le test paramétré
'                (<Theory>/<InlineData>), le test asynchrone (Async Function As
'                Task) et le test d'exception (Assert.Throws + lambda Sub).
'  Fichier source : 01-tests-unitaires.md
' ============================================================================

Imports System
Imports System.Threading.Tasks
Imports Calculatrice
Imports Xunit

Public Class CalculatriceTests

    <Fact>
    Public Sub Additionner_DeuxNombres_RetourneLaSomme()
        ' Arrange
        Dim calc As New Calculatrice()
        ' Act
        Dim resultat = calc.Additionner(2, 3)
        ' Assert
        Assert.Equal(5, resultat)
    End Sub

    <Theory>
    <InlineData(2, 3, 5)>
    <InlineData(-1, 1, 0)>
    <InlineData(0, 0, 0)>
    Public Sub Additionner_PlusieursCas_RetourneLaSomme(a As Integer, b As Integer, attendu As Integer)
        Assert.Equal(attendu, New Calculatrice().Additionner(a, b))
    End Sub

    <Fact>
    Public Sub Diviser_ParZero_LeveUneException()
        Dim calc As New Calculatrice()
        Assert.Throws(Of DivideByZeroException)(Sub() calc.Diviser(10, 0))
    End Sub

    <Fact>
    Public Async Function Charger_Async_RetourneLesDonnees() As Task
        Dim service As New ServiceCatalogue()
        Dim produits = Await service.ChargerAsync()
        Assert.NotEmpty(produits)
        Assert.Equal(2, produits.Count)
    End Function

End Class
