' ============================================================================
'  Section 13.1 : Tests unitaires — NUnit (VB.NET)
'  Description : ⚠️ Piège n°1 de NUnit en VB.NET : « Is » est l'opérateur de
'                comparaison de références du langage. On NE PEUT PAS écrire
'                Is.EqualTo(...) comme en C# : il faut échapper la classe de
'                contraintes en [Is].EqualTo(...). Les autres classes (Throws,
'                Has, Does) ne sont pas des mots-clés et s'utilisent normalement.
'  Fichier source : 01-tests-unitaires.md
' ============================================================================

Imports System
Imports System.Threading.Tasks
Imports Calculatrice
Imports NUnit.Framework

<TestFixture>
Public Class CalculatriceTests

    <Test>
    Public Sub Additionner_DeuxNombres_RetourneLaSomme()
        Dim calc As New Calculatrice()
        Dim resultat = calc.Additionner(2, 3)
        Assert.That(resultat, [Is].EqualTo(5))        ' [Is] échappé : obligatoire en VB
    End Sub

    <TestCase(2, 3, 5)>
    <TestCase(-1, 1, 0)>
    <TestCase(0, 0, 0)>
    Public Sub Additionner_PlusieursCas(a As Integer, b As Integer, attendu As Integer)
        Assert.That(New Calculatrice().Additionner(a, b), [Is].EqualTo(attendu))
    End Sub

    <Test>
    Public Sub Diviser_ParZero_LeveUneException()
        Dim calc As New Calculatrice()
        Assert.Throws(Of DivideByZeroException)(Sub() calc.Diviser(10, 0))   ' Throws : pas un mot-clé
    End Sub

    <Test>
    Public Async Function Charger_Async_RetourneLesDonnees() As Task
        Dim produits = Await New ServiceCatalogue().ChargerAsync()
        Assert.That(produits, [Is].Not.Empty)
        Assert.That(produits.Count, [Is].EqualTo(2))
    End Function

End Class
