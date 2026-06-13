' ============================================================================
'  Section 11.5 : Coexistence — logique métier partagée (.NET Standard 2.0)
'  Description : Règle de gestion pure, sans dépendance de plateforme : un seul
'                assembly que l'ancien (.NET Framework) et le nouveau (.NET 10)
'                référencent tous deux. Évite la duplication de la logique métier
'                pendant la transition.
'  Fichier source : 05-coexistence.md
' ============================================================================

Imports System

Public Class CalculateurRemise

    ' Remise : 10 % au-delà de 100 €, 5 % au-delà de 50 €, sinon aucune.
    Public Function Calculer(montant As Decimal) As Decimal
        If montant > 100D Then Return Math.Round(montant * 0.1D, 2)
        If montant > 50D Then Return Math.Round(montant * 0.05D, 2)
        Return 0D
    End Function

End Class
