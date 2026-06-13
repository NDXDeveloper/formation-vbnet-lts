' ============================================================================
'  Section 13.1 : Tests unitaires — MSTest (VB.NET)
'  Description : <TestClass>/<TestMethod>, paramétrage par <DataRow> (l'ancien
'                <DataTestMethod> est obsolète — MSTEST0044), test d'exception
'                (Assert.ThrowsException) et test asynchrone. Exécuté via la
'                plateforme Microsoft.Testing.Platform (cf. le .vbproj).
'  Fichier source : 01-tests-unitaires.md
' ============================================================================

Imports System
Imports System.Threading.Tasks
Imports Calculatrice
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class CalculatriceTests

    <TestMethod>
    Public Sub Additionner_DeuxNombres_RetourneLaSomme()
        Dim calc As New Calculatrice()
        Assert.AreEqual(5, calc.Additionner(2, 3))   ' AreEqual(attendu, obtenu)
    End Sub

    <TestMethod>
    <DataRow(2, 3, 5)>
    <DataRow(-1, 1, 0)>
    <DataRow(0, 0, 0)>
    Public Sub Additionner_PlusieursCas(a As Integer, b As Integer, attendu As Integer)
        Assert.AreEqual(attendu, New Calculatrice().Additionner(a, b))
    End Sub

    <TestMethod>
    Public Sub Diviser_ParZero_LeveUneException()
        Dim calc As New Calculatrice()
        Assert.ThrowsException(Of DivideByZeroException)(Sub() calc.Diviser(10, 0))
    End Sub

    <TestMethod>
    Public Async Function Charger_Async_RetourneLesDonnees() As Task
        Dim produits = Await New ServiceCatalogue().ChargerAsync()
        Assert.AreEqual(2, produits.Count)
    End Function

End Class
