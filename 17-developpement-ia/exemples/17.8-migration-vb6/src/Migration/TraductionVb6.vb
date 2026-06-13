' ============================================================================
'  Section 17.8 (Cas 1) : deux traductions d'un « On Error Resume Next »
'  Description : Le VB6 d'origine additionne des lignes ; une ligne illisible est
'                IGNORÉE et le calcul CONTINUE (Resume Next reprend à l'instruction
'                suivante) :
'
'                    On Error Resume Next
'                    For i = 0 To UBound(lignes)
'                        total = total + CDbl(lignes(i))   ' illisible -> ignorée
'                        n = n + 1
'                    Next
'
'                MoyenneFausse  : UN SEUL Try/Catch autour de la boucle -> la 1re
'                                 ligne illisible fait SORTIR de la boucle ; les
'                                 lignes suivantes sont PERDUES. (Change le résultat.)
'                MoyenneCorrecte: Try/Catch PAR ITÉRATION -> la ligne fautive est
'                                 ignorée et la boucle CONTINUE (équivalent fidèle).
'  Fichier source : 08-cas-concrets.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System

''' <summary>Calcule la moyenne de lignes numériques en ignorant les lignes illisibles.</summary>
Public Module TraductionVb6

    ''' <summary>❌ Traduction FAUSSE : un seul Try/Catch abandonne la boucle à la 1re erreur.</summary>
    Public Function MoyenneFausse(lignes As String()) As Double
        Dim total As Double = 0
        Dim n As Integer = 0
        Try
            For Each ligne In lignes
                total += CDbl(ligne)   ' une ligne illisible lève -> on quitte toute la boucle
                n += 1
            Next
        Catch
            ' avale l'erreur, mais on a déjà perdu le reste des lignes
        End Try
        Return If(n = 0, 0, total / n)
    End Function

    ''' <summary>✅ Traduction CORRECTE : Try/Catch par itération = Resume Next (ignore et continue).</summary>
    Public Function MoyenneCorrecte(lignes As String()) As Double
        Dim total As Double = 0
        Dim n As Integer = 0
        For Each ligne In lignes
            Try
                total += CDbl(ligne)
                n += 1
            Catch
                ' ligne illisible : ignorée, on passe à la suivante (équivalent de Resume Next)
            End Try
        Next
        Return If(n = 0, 0, total / n)
    End Function

End Module
