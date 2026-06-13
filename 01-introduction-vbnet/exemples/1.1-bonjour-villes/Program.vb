' ============================================================================
'  Section 1.1 : Qu'est-ce que VB.NET et à quoi il sert réellement en 2026
'  Description : Exemple complet construit autour de l'aperçu du langage donné
'                dans la section — déclaration avec Dim, générique
'                List(Of String), initialiseur de collection From {...},
'                boucle For Each et interpolation de chaînes $"..." — puis
'                démonstration que VB.NET accède à la même bibliothèque de
'                classes (BCL) et au même runtime .NET que C#.
'  Fichier source : 01-quest-ce-que-vbnet.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        ' Affichage correct des caractères accentués dans la console Windows.
        Console.OutputEncoding = System.Text.Encoding.UTF8

        ' ---- 1. L'aperçu exact de la section 1.1 ---------------------------
        ' « Des mots en toutes lettres, pas d'accolades ni de points-virgules :
        '   le code se lit presque comme une phrase. »
        Dim villes As New List(Of String) From {"Paris", "Lyon", "Marseille"}

        For Each ville In villes
            Console.WriteLine($"Bonjour {ville} !")
        Next

        ' ---- 2. La même bibliothèque de classes (BCL) que C# ---------------
        ' « Tout ce que C# fait avec System.*, VB.NET le fait aussi. »
        villes.Sort()
        Console.WriteLine()
        Console.WriteLine("Villes triées : " & String.Join(", ", villes))
        Console.WriteLine($"Math.Max(10, 25) = {Math.Max(10, 25)}")

        ' ---- 3. Le même runtime que C# --------------------------------------
        ' Le code VB est compilé en IL et exécuté par le CLR — ici, .NET 10.
        Console.WriteLine($"Version du runtime .NET : {Environment.Version}")
    End Sub

End Module
