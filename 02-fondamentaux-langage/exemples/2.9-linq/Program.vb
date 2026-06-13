' ============================================================================
'  Section 2.9 : LINQ — un point fort de VB.NET
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · la syntaxe de requête (From...Where...Order By...Select)
'                    et la projection en type anonyme (New With {Key ...}) ;
'                  · les mots-clés de requête PROPRES À VB : Aggregate
'                    (sans équivalent en syntaxe de requête C#), Distinct,
'                    Skip / Take ;
'                  · le regroupement Group By ... Into avec agrégats ;
'                  · la même requête en syntaxe par méthodes (lambdas
'                    Function(...), continuation implicite après le point) ;
'                  · les opérateurs terminaux (.Count(), .ToList()) ;
'                  · l'exécution DIFFÉRÉE : une requête reflète les
'                    changements de la source jusqu'à sa matérialisation.
'  Fichier source : 09-linq.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        ' Les données exactes de la section.
        Dim produits As New List(Of Produit) From {
            New Produit With {.Nom = "Clavier", .Prix = 45D, .Categorie = "Périphérique"},
            New Produit With {.Nom = "Écran", .Prix = 220D, .Categorie = "Affichage"},
            New Produit With {.Nom = "Souris", .Prix = 25D, .Categorie = "Périphérique"}
        }

        DemoSyntaxeRequete(produits)
        DemoMotsClesVB(produits)
        DemoGroupBy(produits)
        DemoSyntaxeMethodes(produits)
        DemoOperateursTerminaux(produits)
        DemoExecutionDifferee(produits)
    End Sub

    ' ---- Syntaxe de requête ---------------------------------------------------
    Private Sub DemoSyntaxeRequete(produits As List(Of Produit))
        Console.WriteLine("== Syntaxe de requête ==")

        Dim abordables = From p In produits
                         Where p.Prix < 50D
                         Order By p.Prix
                         Select p.Nom

        Console.WriteLine($"Moins de 50 € (tri par prix) : {String.Join(", ", abordables)}")

        ' Projection en type anonyme — Key marque les propriétés d'égalité (propre à VB)
        Dim synthese = From p In produits
                       Where p.Categorie = "Périphérique"
                       Select New With {Key p.Nom, p.Prix}

        For Each element In synthese
            Console.WriteLine($"  type anonyme : {element}")
        Next
    End Sub

    ' ---- Les mots-clés de requête propres à VB ⭐ -------------------------------
    Private Sub DemoMotsClesVB(produits As List(Of Produit))
        Console.WriteLine()
        Console.WriteLine("== Mots-clés de requête propres à VB (Aggregate, Distinct, Skip/Take) ==")

        ' Agrégation directement en syntaxe de requête (spécifique à VB)
        Dim total = Aggregate p In produits Into Sum(p.Prix)
        Dim moyenne = Aggregate p In produits Into Average(p.Prix)
        Console.WriteLine($"Aggregate ... Into Sum     : {total}")
        Console.WriteLine($"Aggregate ... Into Average : {moyenne:F2}")

        ' Dédoublonnage et pagination comme clauses de requête
        Dim categories = From p In produits Select p.Categorie Distinct
        Console.WriteLine($"Catégories distinctes : {String.Join(", ", categories)}")

        ' (le cours pagine avec Skip 10 Take 10 ; sur 3 éléments : Skip 1 Take 1)
        Dim page = From p In produits Order By p.Nom Skip 1 Take 1
        Console.WriteLine($"Order By Nom, Skip 1 Take 1 : {(From p In page Select p.Nom).Single()}")
    End Sub

    ' ---- Group By ... Into -----------------------------------------------------------
    Private Sub DemoGroupBy(produits As List(Of Produit))
        Console.WriteLine()
        Console.WriteLine("== Group By ... Into (agrégats par groupe) ==")

        Dim parCategorie = From p In produits
                           Group p By p.Categorie Into Nombre = Count(), PrixMoyen = Average(p.Prix)

        For Each groupe In parCategorie
            Console.WriteLine($"  {groupe.Categorie} : {groupe.Nombre} produit(s), prix moyen {groupe.PrixMoyen:F2}")
        Next
    End Sub

    ' ---- Syntaxe par méthodes ------------------------------------------------------------
    Private Sub DemoSyntaxeMethodes(produits As List(Of Produit))
        Console.WriteLine()
        Console.WriteLine("== Syntaxe par méthodes (équivalente) ==")

        Dim abordables = produits.
            Where(Function(p) p.Prix < 50D).
            OrderBy(Function(p) p.Prix).
            Select(Function(p) p.Nom)

        Console.WriteLine($"Where/OrderBy/Select : {String.Join(", ", abordables)}")
        ' (continuation implicite après le point en fin de ligne — pas de '_')
    End Sub

    ' ---- Opérateurs terminaux ---------------------------------------------------------------
    Private Sub DemoOperateursTerminaux(produits As List(Of Produit))
        Console.WriteLine()
        Console.WriteLine("== Opérateurs terminaux (mélange requête + méthode) ==")

        Dim nombre = (From p In produits Where p.Prix > 50D).Count()
        Dim liste = (From p In produits Order By p.Prix Select p.Nom).ToList()

        Console.WriteLine($"Count des produits > 50 € : {nombre}")
        Console.WriteLine($"ToList (tri par prix)     : {String.Join(", ", liste)}")
    End Sub

    ' ---- Exécution différée ⚠️ -----------------------------------------------------------------
    Private Sub DemoExecutionDifferee(produits As List(Of Produit))
        Console.WriteLine()
        Console.WriteLine("== Exécution différée ==")

        Dim chers = From p In produits Where p.Prix > 100D Select p   ' défini, PAS exécuté

        produits.Add(New Produit With {.Nom = "Station", .Prix = 900D, .Categorie = "Affichage"})

        Console.Write("Parcours après l'ajout de Station : ")
        Dim noms As New List(Of String)
        For Each p In chers   ' exécuté MAINTENANT -> inclut "Station"
            noms.Add(p.Nom)
        Next
        Console.WriteLine(String.Join(", ", noms))

        ' Pour figer un instantané : matérialiser avec ToList
        Dim fige = chers.ToList()
        produits.Add(New Produit With {.Nom = "Serveur", .Prix = 2000D, .Categorie = "Affichage"})
        Console.WriteLine($"Instantané ToList (après ajout de Serveur) : {String.Join(", ", fige.Select(Function(p) p.Nom))}")
        Console.WriteLine($"La requête différée, elle, voit Serveur : {String.Join(", ", chers.Select(Function(p) p.Nom))}")
    End Sub

End Module
