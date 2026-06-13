' ============================================================================
'  Section 2.11 : Portée, visibilité et modificateurs d'accès
'  Description : Exemple complet reprenant les extraits de la section :
'                  · portée de bloc vs portée de procédure (la variable du
'                    bloc For n'existe plus après Next) ;
'                  · les six modificateurs d'accès via CompteBancaire
'                    (Private, Protected Friend, Public) et une dérivée ;
'                  · l'asymétrie des défauts Structure / Class (Defauts.vb),
'                    avec l'erreur BC30389 vérifiée pour le champ privé ;
'                  · InternalsVisibleTo (montré en commentaire : il suppose
'                    un second assembly de tests, voir module 13).
'  Fichier source : 11-portee-visibilite.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoPortee()
        DemoModificateurs()
        DemoDefauts()
    End Sub

    ' ---- Portée de bloc vs portée de procédure -------------------------------
    Private Sub DemoPortee()
        Console.WriteLine("== Portée (bloc vs procédure) ==")

        Dim total As Integer = 0           ' portée : toute la procédure
        For i As Integer = 1 To 3          ' 'i' : portée limitée à la boucle
            Dim carre As Integer = i * i   ' 'carre' : portée limitée au bloc For
            total += carre
        Next
        ' Console.WriteLine(carre)   ' ERREUR : 'carre' est hors de portée ici
        Console.WriteLine($"total = {total} (1 + 4 + 9 — 'carre' et 'i' n'existent plus ici)")
    End Sub

    ' ---- Modificateurs d'accès ---------------------------------------------------
    Private Sub DemoModificateurs()
        Console.WriteLine()
        Console.WriteLine("== Modificateurs d'accès (Friend = internal de C#) ==")

        Dim compte As New CompteBancaire()
        compte.Crediter(100D)                       ' Public
        Console.WriteLine($"compte.Solde = {compte.Solde} (propriété publique en lecture seule)")
        ' compte._solde = 1000000D                  ' ERREUR : '_solde' est Private

        ' Protected Friend : accessible ici car NOUS SOMMES dans le même assembly.
        compte.TauxInterne = 0.03D
        Console.WriteLine($"compte.TauxInterne = {compte.TauxInterne} (Protected Friend, même assembly)")

        Dim epargne As New CompteEpargne()
        epargne.Crediter(1000D)
        epargne.TauxInterne = 0.05D
        epargne.AppliquerInterets()                 ' utilise les membres Protected
        Console.WriteLine($"epargne.Solde après intérêts = {epargne.Solde}")

        ' InternalsVisibleTo (rend les membres Friend visibles au projet de tests) :
        ' <Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("MaBibliotheque.Tests")>
    End Sub

    ' ---- Défauts d'accès : l'asymétrie Structure / Class ----------------------------
    Private Sub DemoDefauts()
        Console.WriteLine()
        Console.WriteLine("== Défauts d'accès : asymétrie Structure / Class ==")

        Dim ps As PointStruct
        ps.X = 5                       ' compile : champ Dim PUBLIC par défaut (Structure)
        Console.WriteLine($"PointStruct : ps.X = {ps.X} (champ Dim public par défaut)")

        Dim pc As New PointClasse()
        ' pc.Y = 5                     ' ERREUR BC30389 (vérifiée) :
        '                              ' « 'Y' n'est pas accessible dans ce contexte,
        '                              '   car il est 'Private'. »
        Console.WriteLine($"PointClasse : pc.LireY() = {pc.LireY()} (champ Dim privé par défaut)")
        Console.WriteLine("-> bonne pratique : écrire TOUJOURS le modificateur explicitement.")
    End Sub

End Module
