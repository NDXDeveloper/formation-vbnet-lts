' ============================================================================
'  Section 1.3 : L'écosystème .NET — Structure d'une solution (solution mixte)
'  Description : Facture de démonstration partagée par l'interface (MonApp.UI)
'                et les tests (MonApp.Tests). Notez l'instanciation de records
'                C# depuis VB.NET, via leur constructeur ordinaire.
'  Fichier source : 03-ecosysteme-dotnet.md
' ============================================================================

Imports System.Collections.Generic
Imports MonApp.Core

''' <summary>Jeu de données de démonstration.</summary>
Public Module FactureDemo

    ''' <summary>
    ''' Trois lignes aux trois taux de TVA :
    '''   36,00 € HT à 5,5 %  +  25,00 € HT à 20 %  +  10,00 € HT à 10 %
    '''   → HT = 71,00 € ; TVA = 7,98 € ; TTC = 78,98 €.
    ''' </summary>
    Public Function Lignes() As List(Of LigneFacture)
        Return New List(Of LigneFacture) From {
            New LigneFacture("Café en grains 1 kg", 18D, 2, CategorieTva.TauxReduit),
            New LigneFacture("Cafetière à piston", 25D, 1, CategorieTva.TauxNormal),
            New LigneFacture("Livraison express", 10D, 1, CategorieTva.TauxIntermediaire)
        }
    End Function

End Module
