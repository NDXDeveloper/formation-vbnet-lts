' ============================================================================
'  Section 17.4 : Tests vérifiant l'intention (la spécification du barème)
'  Description : Chaque test encode une EXIGENCE du barème (valeurs aux bornes
'                9/10/49/50, montant remisé, entrées invalides → exception), et non
'                un simple reflet de l'implémentation. Syntaxe VB : <Theory>/
'                <InlineData> entre chevrons, et Sub() (pas =>) pour Assert.Throws.
'  Fichier source : 04-generer-tests-doc.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports Xunit

Public Class TestsRemise

    Private ReadOnly _calc As New CalculateurRemise()

    ' Bornes du barème : 0 % < 10 ; 5 % de 10 à 49 ; 10 % >= 50.
    <Theory>
    <InlineData(0, 0.0)>
    <InlineData(9, 0.0)>
    <InlineData(10, 0.05)>
    <InlineData(49, 0.05)>
    <InlineData(50, 0.1)>
    <InlineData(1000, 0.1)>
    Public Sub TauxRemise_SuitLeBareme(quantite As Integer, tauxAttendu As Double)
        Assert.Equal(CDec(tauxAttendu), _calc.TauxRemise(quantite))
    End Sub

    <Fact>
    Public Sub CalculerTotal_AppliqueLaRemise()
        ' 50 unités à 2,00 → 100,00 puis −10 % = 90,00.
        Assert.Equal(90D, _calc.CalculerTotal(50, 2D))
    End Sub

    <Fact>
    Public Sub CalculerTotal_SansRemise_SousLeSeuil()
        ' 9 unités à 2,00 → 18,00 (aucune remise).
        Assert.Equal(18D, _calc.CalculerTotal(9, 2D))
    End Sub

    <Fact>
    Public Sub CalculerTotal_QuantiteNegative_Leve()
        Assert.Throws(Of ArgumentOutOfRangeException)(Sub() _calc.CalculerTotal(-1, 2D))
    End Sub

    <Fact>
    Public Sub CalculerTotal_PrixNegatif_Leve()
        Assert.Throws(Of ArgumentOutOfRangeException)(Sub() _calc.CalculerTotal(10, -2D))
    End Sub

End Class
