' ============================================================================
'  Section 2.11 : Portée, visibilité et modificateurs d'accès
'  Description : La classe exacte de la section : champ Private (interne au
'                type), Protected Friend (dérivés OU même assembly), surface
'                publique minimale (propriété en lecture seule + méthode),
'                et méthode Protected Overridable pour les classes dérivées.
'  Fichier source : 11-portee-visibilite.md
' ============================================================================

Public Class CompteBancaire
    Private _solde As Decimal               ' interne au type
    Protected Friend TauxInterne As Decimal ' dérivés OU même assembly

    Public ReadOnly Property Solde As Decimal   ' surface publique (le contrat)
        Get
            Return _solde
        End Get
    End Property

    Public Sub Crediter(montant As Decimal)
        _solde += montant
    End Sub

    Protected Overridable Sub Journaliser(message As String)
        ' accessible aux classes dérivées
    End Sub
End Class

''' <summary>Dérivée : illustre l'accès aux membres Protected.</summary>
Public Class CompteEpargne
    Inherits CompteBancaire

    Public Sub AppliquerInterets()
        ' TauxInterne (Protected Friend) et Journaliser (Protected)
        ' sont accessibles depuis la classe dérivée.
        Crediter(Solde * TauxInterne)
        Journaliser("Intérêts appliqués.")
    End Sub
End Class
