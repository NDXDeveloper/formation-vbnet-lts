Option Strict Off
' ============================================================================
'  Section 14.5 : Bonnes pratiques — liaison tardive (late binding), ISOLÉE
'  Description : La liaison tardive (appel de membre sur un Object, résolu à
'                l'exécution par réflexion) est BEAUCOUP plus lente que la
'                liaison anticipée — et repoussse les erreurs de type à
'                l'exécution. Elle EXIGE Option Strict Off : on le limite donc à
'                CE SEUL fichier (en tête), le reste du projet restant Strict On.
'  Fichier source : 05-bonnes-pratiques.md
' ============================================================================

Public Module LiaisonTardive
    ' service est un Object : Traiter est résolu À L'EXÉCUTION (liaison tardive).
    Public Function AppelTardif(service As Object, x As Integer) As Integer
        Return service.Traiter(x)
    End Function
End Module
