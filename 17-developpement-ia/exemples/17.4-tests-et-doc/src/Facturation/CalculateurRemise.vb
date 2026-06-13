' ============================================================================
'  Section 17.4 : Documentation XML (''' VB) + code à tester
'  Description : Le barème de remise est la SPÉCIFICATION : 0 % sous 10 unités,
'                5 % de 10 à 49, 10 % à partir de 50. La documentation XML est
'                rédigée avec ''' (trois apostrophes) — JAMAIS /// (syntaxe C# que
'                l'IA propose par défaut). Les commentaires décrivent le
'                comportement réel ; les tests vérifient cette même intention.
'  Fichier source : 04-generer-tests-doc.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System

''' <summary>Calcule des montants de commande en appliquant un barème de remise par quantité.</summary>
''' <remarks>Barème : 0 % pour moins de 10 unités, 5 % de 10 à 49, 10 % à partir de 50.</remarks>
Public Class CalculateurRemise

    ''' <summary>Renvoie le taux de remise applicable pour une quantité donnée.</summary>
    ''' <param name="quantite">Nombre d'unités commandées.</param>
    ''' <returns>Le taux sous forme décimale : <c>0</c>, <c>0,05</c> ou <c>0,10</c>.</returns>
    Public Function TauxRemise(quantite As Integer) As Decimal
        If quantite >= 50 Then Return 0.1D
        If quantite >= 10 Then Return 0.05D
        Return 0D
    End Function

    ''' <summary>Calcule le montant total d'une ligne de commande, remise comprise.</summary>
    ''' <param name="quantite">Nombre d'unités (doit être positif ou nul).</param>
    ''' <param name="prixUnitaire">Prix d'une unité (doit être positif ou nul).</param>
    ''' <returns><c>quantite × prixUnitaire × (1 − taux)</c>, arrondi au comportement de <see cref="Decimal"/>.</returns>
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Levée si <paramref name="quantite"/> ou <paramref name="prixUnitaire"/> est négatif.
    ''' </exception>
    Public Function CalculerTotal(quantite As Integer, prixUnitaire As Decimal) As Decimal
        If quantite < 0 Then Throw New ArgumentOutOfRangeException(NameOf(quantite))
        If prixUnitaire < 0D Then Throw New ArgumentOutOfRangeException(NameOf(prixUnitaire))
        Dim taux As Decimal = TauxRemise(quantite)
        Return quantite * prixUnitaire * (1D - taux)
    End Function

End Class
