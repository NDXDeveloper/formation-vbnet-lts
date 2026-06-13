' ============================================================================
'  Section 2.1 : Structure d'un programme ; Option Strict / Explicit / Infer / Compare
'  Description : Démonstration de la portée FICHIER des directives Option :
'                la directive « Option Compare Text » ci-dessous surcharge,
'                pour ce seul fichier, le réglage Binary du projet — les
'                comparaisons de chaînes y deviennent insensibles à la casse
'                (et tiennent compte de la culture).
'  Fichier source : 01-structure-options.md
' ============================================================================

Option Compare Text

Namespace MonApplication

    Public Module ComparaisonsTexte

        ''' <summary>
        ''' Compare deux chaînes avec l'opérateur « = » de CE fichier,
        ''' régi par Option Compare Text : "Apple" = "apple" renvoie True.
        ''' </summary>
        Public Function EgalEnModeText(a As String, b As String) As Boolean
            Return a = b
        End Function

    End Module

End Namespace
