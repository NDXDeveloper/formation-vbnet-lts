' ============================================================================
'  Section 14.2 : Types valeur vs référence — types de démonstration
'  Description : Point (Structure = type valeur) et Boite (Class = type
'                référence) servent à montrer la différence de sémantique de
'                copie : copier une structure copie la valeur (indépendance) ;
'                copier une classe copie la référence (partage de l'objet).
'  Fichier source : 02-types-allocations.md
' ============================================================================

' Type VALEUR : la donnée elle-même, copiée par valeur.
Public Structure Point
    Public X As Integer
    Public Y As Integer
End Structure

' Type RÉFÉRENCE : une référence vers la donnée sur le tas.
Public Class Boite
    Public Valeur As Integer
End Class
