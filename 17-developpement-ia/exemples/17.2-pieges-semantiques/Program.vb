' ============================================================================
'  Section 17.2 / 17.7 : Pièges sémantiques des opérateurs
'  Description : Met en évidence, à l'exécution, les écarts entre une traduction
'                littérale du C# et la sémantique réelle de VB. Ce sont les pièges
'                « ça compile mais c'est faux » : aucun message du compilateur, mais
'                un résultat différent. C'est pourquoi le module 17 insiste : la
'                sémantique, c'est l'affaire des TESTS, pas du compilateur.
'  Fichier source : 02-prompting-vbnet.md (et 07-limites-pieges.md)
' ============================================================================

Option Strict On
Option Explicit On

Imports System

Module Program

    ' Sonde : incrémente un compteur quand elle est évaluée — sert à prouver
    ' (ou réfuter) le court-circuit des opérateurs logiques.
    Private _evaluationsDroite As Integer

    Private Function Sonde(valeur As Boolean) As Boolean
        _evaluationsDroite += 1
        Return valeur
    End Function

    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 17.2/17.7 Pièges sémantiques des opérateurs VB ===")
        Console.WriteLine()

        ' (1) ^ : PUISSANCE en VB — c'est un XOR en C#. Le XOR de VB s'écrit « Xor ».
        Dim puissance As Double = 5 ^ 3      ' 5 puissance 3 = 125  (en C#, 5 ^ 3 = 6, un XOR)
        Dim ouExclusif As Integer = 5 Xor 3  ' XOR réel en VB = 6   (ce que C# écrit « 5 ^ 3 »)
        Console.WriteLine($"[^]    5 ^ 3   = {puissance} (PUISSANCE)   |   5 Xor 3 = {ouExclusif} (XOR, = C# « 5 ^ 3 »)")

        ' (2) \ vs / : division ENTIÈRE vs FLOTTANTE.
        '     En C#, « 7 / 2 » entre deux entiers vaut 3 ; le « / » de VB vaut TOUJOURS 3,5.
        Dim entiere As Integer = 7 \ 2       ' 3
        Dim flottante As Double = 7 / 2      ' 3,5  (séparateur décimal fr-FR = virgule)
        Console.WriteLine($"[\ /]  7 \ 2   = {entiere} (entière, = C# « int/int »)   |   7 / 2 = {flottante} (flottante)")

        ' (3) Mod : reste (le « % » du C#).
        Console.WriteLine($"[Mod]  7 Mod 3 = {7 Mod 3}")
        Console.WriteLine()

        ' (4) Court-circuit : AndAlso/OrElse court-circuitent, And/Or NON.
        _evaluationsDroite = 0
        Dim avecAndAlso As Boolean = (False AndAlso Sonde(True))
        Console.WriteLine($"[AndAlso] False AndAlso Sonde() : opérande droite évaluée {_evaluationsDroite} fois (court-circuit)")

        _evaluationsDroite = 0
        Dim avecAnd As Boolean = (False And Sonde(True))
        Console.WriteLine($"[And]     False And Sonde()     : opérande droite évaluée {_evaluationsDroite} fois (PAS de court-circuit)")

        ' Conséquence concrète : le court-circuit protège d'une division par zéro.
        Dim x As Integer = 0
        Dim sur As Boolean = (x <> 0 AndAlso (10 \ x) > 1)   ' sûr : (10 \ x) n'est jamais évalué
        Console.WriteLine($"[AndAlso] (x=0) x<>0 AndAlso (10\x)>1 = {sur} (sûr, court-circuité)")

        Dim leveException As Boolean = False
        Try
            Dim danger As Boolean = (x <> 0 And (10 \ x) > 1)   ' And évalue les DEUX -> division par zéro
        Catch ex As DivideByZeroException
            leveException = True
        End Try
        Console.WriteLine($"[And]     (x=0) x<>0 And (10\x)>1 -> DivideByZeroException = {leveException}")
    End Sub

End Module
