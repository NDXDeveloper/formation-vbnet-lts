' ============================================================================
'  Section 18.4 : Migration hybride VB -> C# — démonstration du piège « ^ »
'  Description : La console VB appelle la bibliothèque C# migrée et compare trois
'                valeurs : le « ^ » de VB (puissance, 125), l'équivalent C# correct
'                (Math.Pow, 125) et ce qu'une conversion littérale « a ^ b » donnerait
'                en C# (XOR, 6). L'interop VB↔C# est transparente (même IL) ; le
'                point clé est que « ça compile » ne garantit pas l'équivalence.
'  Fichier source : 04-migrer-vers-csharp.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports CalculsCsharp

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 18.4 Migration hybride VB -> C# : le piège de l'opérateur ^ ===")
        Console.WriteLine()

        Dim a As Integer = 5, b As Integer = 3

        ' En VB, « ^ » est la PUISSANCE.
        Dim puissanceVb As Double = a ^ b

        ' Appels à la bibliothèque C# « migrée » (référence de projet, interop transparente).
        Dim puissanceCs As Double = OperateursMigres.Puissance(a, b)   ' Math.Pow : équivalent CORRECT
        Dim xorCs As Integer = OperateursMigres.XorBinaire(a, b)       ' « a ^ b » en C# = XOR

        Console.WriteLine($"VB   : {a} ^ {b} (puissance)          = {puissanceVb}")
        Console.WriteLine($"C#   : Puissance({a},{b}) = Math.Pow   = {puissanceCs}  (équivalent CORRECT)")
        Console.WriteLine($"C#   : {a} ^ {b} = XOR binaire         = {xorCs}  (conversion LITTÉRALE -> FAUX)")
        Console.WriteLine()

        Dim equivalenceOk As Boolean = (puissanceVb = puissanceCs)
        Dim litteraleDiverge As Boolean = (CInt(puissanceVb) <> xorCs)
        Console.WriteLine($"Équivalence correcte (VB ^  =  C# Math.Pow) : {equivalenceOk}")
        Console.WriteLine($"Conversion littérale fausse (125 <> 6)       : {litteraleDiverge}")
    End Sub
End Module
