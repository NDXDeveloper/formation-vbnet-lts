' ============================================================================
'  Section 1.3 : L'écosystème .NET — Structure d'une solution (solution mixte)
'  Description : Projet de test (xUnit, VB.NET) de la solution « MaSolution ».
'                Vérifie la logique métier VB et, au passage, la consommation
'                de la brique C# (BaremeTva, record LigneFacture) depuis VB.
'                Lancement : dotnet test (à la racine de la solution).
'  Fichier source : 03-ecosysteme-dotnet.md
' ============================================================================

Imports System.Collections.Generic
Imports MonApp.Core
Imports MonApp.Metier
Imports Xunit

Public Class CalculateurFactureTests

    Private ReadOnly _calculateur As New CalculateurFacture()
    Private ReadOnly _lignes As List(Of LigneFacture) = FactureDemo.Lignes()

    <Fact>
    Public Sub TotalHt_de_la_facture_de_demo_vaut_71_euros()
        Assert.Equal(71D, _calculateur.TotalHt(_lignes))
    End Sub

    <Fact>
    Public Sub TotalTva_de_la_facture_de_demo_vaut_7_98_euros()
        Assert.Equal(7.98D, _calculateur.TotalTva(_lignes))
    End Sub

    <Fact>
    Public Sub TotalTtc_de_la_facture_de_demo_vaut_78_98_euros()
        Assert.Equal(78.98D, _calculateur.TotalTtc(_lignes))
    End Sub

    <Fact>
    Public Sub Le_bareme_Csharp_se_consomme_depuis_VB()
        ' BaremeTva est une classe statique C# (switch expression) ;
        ' l'appel depuis VB est strictement identique à un appel VB.
        Assert.Equal(0.2D, BaremeTva.Taux(CategorieTva.TauxNormal))
        Assert.Equal(0.055D, BaremeTva.Taux(CategorieTva.TauxReduit))
    End Sub

    <Fact>
    Public Sub Le_record_Csharp_a_une_egalite_par_valeur()
        ' Deux records C# construits avec les mêmes valeurs sont égaux,
        ' même s'il s'agit de deux instances distinctes.
        Dim a As New LigneFacture("Café", 10D, 1, CategorieTva.TauxReduit)
        Dim b As New LigneFacture("Café", 10D, 1, CategorieTva.TauxReduit)
        Assert.Equal(a, b)
        Assert.False(a Is b)
    End Sub

End Class
