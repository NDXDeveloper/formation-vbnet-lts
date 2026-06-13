' ============================================================================
'  Section 11.7 : Gestion des risques — comportement hérité (à préserver)
'  Description : Deux comportements « hérités » au résultat surprenant, mais que
'                la migration doit reproduire À L'IDENTIQUE :
'                  • Arrondir : CInt applique l'arrondi BANQUIER (au pair le plus
'                    proche) — 0,5→0, 1,5→2, 2,5→2… (et non l'arrondi « scolaire ») ;
'                  • FormaterCode : ancienne chaîne de longueur fixe — complétée
'                    OU TRONQUÉE à 5 caractères.
'                Les tests de caractérisation verrouillent ces comportements.
'  Fichier source : 07-gestion-risques.md
' ============================================================================

Public Class CalculateurLegacy

    ' CInt = arrondi banquier (round half to even). Comportement hérité à préserver.
    Public Function Arrondir(valeur As Double) As Integer
        Return CInt(valeur)
    End Function

    ' Ancienne sémantique « String * 5 » : complète à droite, ou tronque à 5.
    Public Function FormaterCode(code As String) As String
        Return (code & "     ").Substring(0, 5)
    End Function

End Class
