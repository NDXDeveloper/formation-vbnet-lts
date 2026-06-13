' ============================================================================
'  Section 1.6.2 : La stratégie hybride VB.NET / C# en une page
'  Description : L'étape 3 du « Comment ça marche » de la section : côté VB,
'                on appelle l'API publique de la bibliothèque C# comme
'                n'importe quel type .NET — y compris les records et les
'                propriétés init-only, qui se consomment sans difficulté
'                alors qu'ils ne se déclarent pas en VB.
'  Fichier source : 06.2-strategie-hybride.md
'  Compilation    : dotnet build HybrideDemo.sln (à la racine de l'exemple)
'  Exécution      : dotnet run --project AppVB
' ============================================================================

Imports System
Imports ModerneLib

Module Program

    Sub Main(args As String())
        ' Affichage correct des caractères accentués dans la console Windows.
        Console.OutputEncoding = System.Text.Encoding.UTF8

        ' ---- 1. Consommer un record C# depuis VB.NET ------------------------
        ' Instanciation par le constructeur positionnel, comme un type ordinaire.
        Dim p1 As New Produit("Café moulu", 4D)
        Dim p2 As New Produit("Café moulu", 4D)

        Console.WriteLine("== Un record C# consommé depuis VB.NET ==")
        Console.WriteLine($"p1 : {p1}")                ' ToString() généré par le record
        Console.WriteLine($"p2 : {p2}")
        Console.WriteLine($"Égalité par valeur  (p1 = p2)  : {p1 = p2}")
        Console.WriteLine($"Références distinctes (p1 Is p2) : {p1 Is p2}")

        ' Un record est immuable : l'affectation ne compile pas.
        ' La ligne ci-dessous, décommentée, provoque l'erreur BC37311 (vérifiée) :
        ' « La propriété d'initialisation uniquement 'Nom' peut uniquement être
        '   affectée par un initialiseur de membre d'objet, ou sur 'Me',
        '   'MyClass' ou 'MyBase' dans un constructeur d'instance. »
        'p1.Nom = "Autre nom"

        ' ---- 2. Consommer des propriétés init-only (capacité VB 16.9) -------
        ' L'initialiseur With {...} est le seul endroit où l'affectation
        ' de ces propriétés est autorisée.
        Dim article As New ArticleStock With {.Nom = "Thé vert", .Quantite = 12}

        Console.WriteLine()
        Console.WriteLine("== Propriétés init-only consommées depuis VB ==")
        Console.WriteLine($"Article : {article.Nom}, quantité : {article.Quantite}")

        ' Hors initialiseur, même erreur BC37311 à la compilation (vérifiée) :
        'article.Quantite = 99
    End Sub

End Module
