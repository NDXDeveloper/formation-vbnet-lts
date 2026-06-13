' ============================================================================
'  Section 13.4 : Analyse statique — code à diagnostics
'  Description : Code déclenchant volontairement deux diagnostics que les
'                analyseurs Roslyn lèvent EN VB.NET :
'                  • CA1822 : Doubler n'utilise pas l'état d'instance → « peut
'                    être marqué Shared » (règle de qualité/perf) ;
'                  • IDE0051 : MethodeInutilisee est un membre privé jamais
'                    référencé (règle de style).
'                Configurées en « warning » par .editorconfig, elles s'affichent
'                au build sans le faire échouer.
'  Fichier source : 04-analyse-statique.md
' ============================================================================

Public Class AvecDiagnostic

    ' CA1822 : ne touche aucun champ d'instance → l'analyseur suggère « Shared ».
    Public Function Doubler(x As Integer) As Integer
        Return x * 2
    End Function

    ' IDE0051 : membre privé jamais utilisé.
    Private Sub MethodeInutilisee()
    End Sub

End Class
