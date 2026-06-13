' ============================================================================
'  Section 17.1 : Démonstration de la classe immuable
'  Description : Vérifie le comportement « record-like » : égalité par valeur,
'                hash cohérent entre instances égales, immuabilité (la copie
'                « Avec » ne modifie pas l'original).
'  Fichier source : 01-pourquoi-ia-vbnet.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 17.1 Classe immuable VB (équivalent record C#) ===")
        Console.WriteLine()

        Dim a As New Personne("Alice", 30)
        Dim b As New Personne("Alice", 30)
        Dim c As New Personne("Bob", 25)

        Console.WriteLine($"a = {a}")
        Console.WriteLine($"a = b (par valeur) = {a.Equals(b)} ; même hash = {a.GetHashCode() = b.GetHashCode()}")
        Console.WriteLine($"a = c (par valeur) = {a.Equals(c)}")

        ' Immuabilité : « Avec » renvoie une NOUVELLE instance, a reste inchangé.
        Dim d As Personne = a.Avec(age:=31)
        Console.WriteLine($"d = a.Avec(age:=31) -> d = {d}")
        Console.WriteLine($"a inchangé après la copie = {a}")
    End Sub
End Module
