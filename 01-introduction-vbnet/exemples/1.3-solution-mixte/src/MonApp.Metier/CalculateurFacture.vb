' ============================================================================
'  Section 1.3 : L'écosystème .NET — Structure d'une solution (solution mixte)
'  Description : Logique métier en VB.NET. Elle consomme les types du projet
'                C# MonApp.Core (le record LigneFacture, le barème de TVA)
'                exactement comme des types .NET ordinaires : VB et C#
'                compilent vers le même IL et partagent la même BCL.
'  Fichier source : 03-ecosysteme-dotnet.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports MonApp.Core

''' <summary>Calcule les totaux d'une facture (HT, TVA, TTC).</summary>
Public Class CalculateurFacture

    ''' <summary>Somme des totaux hors taxes des lignes.</summary>
    Public Function TotalHt(lignes As IEnumerable(Of LigneFacture)) As Decimal
        Return lignes.Sum(Function(l) l.TotalHt)
    End Function

    ''' <summary>Somme des TVA par ligne, au taux propre à chaque catégorie.</summary>
    Public Function TotalTva(lignes As IEnumerable(Of LigneFacture)) As Decimal
        Return lignes.Sum(Function(l) l.TotalHt * BaremeTva.Taux(l.Categorie))
    End Function

    ''' <summary>Total toutes taxes comprises.</summary>
    Public Function TotalTtc(lignes As IEnumerable(Of LigneFacture)) As Decimal
        Return TotalHt(lignes) + TotalTva(lignes)
    End Function

End Class
