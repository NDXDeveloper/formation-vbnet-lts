' ============================================================================
'  Section 12.2 : Débogage — attribut <DebuggerDisplay>
'  Description : Personnalise l'affichage de l'objet dans les fenêtres d'espion.
'                Motif recommandé : une PROPRIÉTÉ PRIVÉE qui compose la chaîne,
'                référencée avec le suffixe « ,nq » (no quotes). Au lieu de
'                « Debogage.Commande », le débogueur affichera
'                « Commande 4271 — 1250 (Expédiée) ».
'  Fichier source : 02-debogage.md
' ============================================================================

Imports System.Diagnostics

<DebuggerDisplay("{AffichageDebogueur,nq}")>
Public Class Commande
    Public Property Id As Integer
    Public Property Montant As Decimal
    Public Property Statut As String

    ' Propriété privée dédiée à l'affichage (le débogueur l'évalue).
    Private ReadOnly Property AffichageDebogueur As String
        Get
            Return $"Commande {Id} — {Montant} ({Statut})"
        End Get
    End Property

    ' Exposé pour la VÉRIFICATION hors débogueur (mêmes données que ,nq).
    Friend Function TexteAffichageDebogueur() As String
        Return AffichageDebogueur
    End Function
End Class
