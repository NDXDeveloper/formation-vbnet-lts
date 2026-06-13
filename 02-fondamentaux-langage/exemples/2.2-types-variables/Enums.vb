' ============================================================================
'  Section 2.2 : Types de données et variables
'  Description : Les trois énumérations de la section — valeurs
'                auto-incrémentées (JourSemaine), type sous-jacent et valeurs
'                explicites (CodeHttp), et indicateurs combinables <Flags>
'                (Permissions). Les attributs s'écrivent entre chevrons <...>
'                en VB (et non entre crochets [...] comme en C#).
'  Fichier source : 02-types-variables.md
' ============================================================================

''' <summary>Valeurs auto-incrémentées à partir de 0.</summary>
Public Enum JourSemaine
    Lundi      ' 0
    Mardi      ' 1
    Mercredi   ' 2
    Jeudi      ' 3
    Vendredi   ' 4
    Samedi     ' 5
    Dimanche   ' 6
End Enum

''' <summary>Type sous-jacent imposé et valeurs explicites.</summary>
Public Enum CodeHttp As Integer
    Ok = 200
    NonTrouve = 404
    ErreurServeur = 500
End Enum

''' <summary>Jeu d'indicateurs combinables par bits (puissances de deux).</summary>
<Flags>
Public Enum Permissions
    Aucune = 0
    Lecture = 1
    Ecriture = 2
    Execution = 4
End Enum
