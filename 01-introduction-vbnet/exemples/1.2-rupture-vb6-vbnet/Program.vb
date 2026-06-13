' ============================================================================
'  Section 1.2 : De Visual Basic 6 à VB.NET (histoire et héritage legacy)
'  Description : Exemple ajouté à la formation — il rend concrètes, côté
'                VB.NET, les ruptures VB6 → VB.NET décrites dans la section :
'                  1. le type Integer passe de 16 bits (VB6) à 32 bits ;
'                  2. les tableaux deviennent indexés à partir de zéro ;
'                  3. le type Variant disparaît au profit d'Object ;
'                  4. la « vraie orientation objet » : héritage
'                     d'implémentation et polymorphisme (absents de VB6).
'  Fichier source : 02-histoire-evolution.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        ' Affichage correct des caractères accentués dans la console Windows.
        Console.OutputEncoding = System.Text.Encoding.UTF8

        ' ---- 1. Integer : 16 bits en VB6, 32 bits en VB.NET -----------------
        ' En VB6, Integer était un entier 16 bits (max 32 767) ; en VB.NET,
        ' Integer correspond à System.Int32 (32 bits). L'équivalent de
        ' l'ancien Integer 16 bits s'appelle désormais Short.
        Console.WriteLine("== Tailles des types entiers ==")
        Console.WriteLine($"Short.MaxValue   = {Short.MaxValue,20}  (16 bits — l'« Integer » de VB6)")
        Console.WriteLine($"Integer.MaxValue = {Integer.MaxValue,20}  (32 bits — l'Integer de VB.NET)")
        Console.WriteLine($"Long.MaxValue    = {Long.MaxValue,20}  (64 bits)")

        ' ---- 2. Tableaux indexés à partir de zéro ---------------------------
        ' En VB6, « Dim t(5) » avec Option Base 1 donnait des indices 1 à 5.
        ' En VB.NET, tout tableau commence à l'indice 0.
        Console.WriteLine()
        Console.WriteLine("== Tableaux indexés à partir de zéro ==")
        Dim jours() As String = {"lundi", "mardi", "mercredi", "jeudi", "vendredi"}
        Console.WriteLine($"jours(0) = {jours(0)}  (premier élément : indice 0, pas 1)")
        Console.WriteLine($"jours({jours.Length - 1}) = {jours(jours.Length - 1)}  (dernier élément : Length - 1)")

        ' ---- 3. Le Variant disparaît au profit d'Object ---------------------
        ' Object est le type racine de .NET : il peut référencer n'importe
        ' quelle valeur, mais reste fortement typé à l'exécution.
        Console.WriteLine()
        Console.WriteLine("== Object remplace Variant ==")
        Dim valeur As Object = 42
        Console.WriteLine($"valeur contient un {valeur.GetType().Name} : {valeur}")
        valeur = "quarante-deux"
        Console.WriteLine($"valeur contient maintenant un {valeur.GetType().Name} : {valeur}")

        ' ---- 4. Héritage d'implémentation et polymorphisme ------------------
        ' VB6 proposait classes et interfaces, mais pas d'héritage
        ' d'implémentation. VB.NET est un langage orienté objet complet :
        ' MustInherit / Inherits / Overrides — voir Animaux.vb.
        Console.WriteLine()
        Console.WriteLine("== Héritage d'implémentation (impossible en VB6) ==")
        Dim animaux As New List(Of Animal) From {
            New Chien("Rex"),
            New Chat("Mistigri")
        }
        For Each animal In animaux
            Console.WriteLine(animal.SePresenter())
        Next
    End Sub

End Module
