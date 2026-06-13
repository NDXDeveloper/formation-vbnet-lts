' ============================================================================
'  Section 15.3 : CI/CD — code testé par le pipeline
'  Description : Une logique métier simple (calcul TTC) que la phase « test » du
'                pipeline vérifie à chaque commit. Rien de spécifique à VB : le
'                pipeline exécute dotnet test exactement comme pour du C#.
'  Fichier source : 03-cicd.md
' ============================================================================

Imports System

Public Class CalculateurTva
    ' Montant TTC = HT × (1 + taux), arrondi au centime.
    Public Function Ttc(montantHt As Decimal, taux As Decimal) As Decimal
        Return Math.Round(montantHt * (1D + taux), 2)
    End Function
End Class
