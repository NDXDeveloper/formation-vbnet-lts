' ============================================================================
'  Section 3.5 : Modules, espaces de noms et classes partielles
'  Description : Les espaces de noms de la section : blocs imbriqués et forme
'                pointée équivalente. Le RootNamespace du projet étant
'                « Contoso.Ventes », l'espace réel de DepotFactures est
'                Contoso.Ventes.Donnees — le piège vérifié dans Program.vb.
'  Fichier source : 05-modules-namespaces.md
' ============================================================================

' Projet dont le Root Namespace vaut « Contoso.Ventes ».
Namespace Donnees                  ' espace réel : Contoso.Ventes.Donnees
    Public Class DepotFactures
    End Class
End Namespace

Namespace Facturation
    Public Class Facture
    End Class

    Namespace Calculs              ' imbriqué -> Facturation.Calculs
        Public Module Taxes
            Public Function MontantTVA(ht As Decimal) As Decimal
                Return ht * Utilitaires.TauxTVA
            End Function
        End Module
    End Namespace
End Namespace

' Forme pointée, strictement équivalente au bloc imbriqué :
Namespace Facturation.Calculs
    Public Module Arrondis
        Public Function ArrondirCentimes(montant As Decimal) As Decimal
            Return Math.Round(montant, 2)
        End Function
    End Module
End Namespace
