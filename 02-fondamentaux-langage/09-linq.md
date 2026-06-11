🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.9 LINQ — un point fort de VB.NET ⭐

> LINQ (*Language Integrated Query*) permet d'interroger des données — collections en mémoire, bases de
> données, XML — directement dans le langage, avec une syntaxe quasi déclarative. C'est un domaine où
> VB.NET brille : sa **syntaxe de requête** est particulièrement riche et lisible, plus complète que
> celle de C# sur certains points (notamment la requête `Aggregate`, qui n'a pas d'équivalent en
> syntaxe de requête C#).

L'essentiel des opérateurs vit dans l'espace `System.Linq` (les mots-clés de requête sont, eux, intégrés
au langage).

```vb
Imports System.Linq
```

---

## Qu'est-ce que LINQ ?

LINQ offre une **grammaire de requête unifiée** quelle que soit la source, grâce à des *fournisseurs* :

- **LINQ to Objects** : sur toute séquence en mémoire (`IEnumerable(Of T)`) — l'objet de cette section.
- **LINQ to Entities** : sur une base de données via EF Core, traduit en SQL (→ [module 7](../07-acces-donnees/02-ef-core-10.md)).
- **LINQ to XML** : sur des documents XML.

Deux écritures équivalentes coexistent : la **syntaxe de requête** (`From…Where…Select`) et la **syntaxe
par méthodes** (méthodes d'extension `.Where(…).Select(…)`). On peut les mélanger librement.

Les exemples ci-dessous s'appuient sur une petite classe :

```vb
Public Class Produit
    Public Property Nom As String
    Public Property Prix As Decimal
    Public Property Categorie As String
End Class

Dim produits As New List(Of Produit) From {
    New Produit With {.Nom = "Clavier", .Prix = 45D, .Categorie = "Périphérique"},
    New Produit With {.Nom = "Écran", .Prix = 220D, .Categorie = "Affichage"},
    New Produit With {.Nom = "Souris", .Prix = 25D, .Categorie = "Périphérique"}
}
```

---

## LINQ to Objects — syntaxe de requête

La syntaxe de requête se lit presque comme une phrase :

```vb
Dim abordables = From p In produits
                 Where p.Prix < 50D
                 Order By p.Prix
                 Select p.Nom
```

`abordables` est un `IEnumerable(Of String)`. La clause `Select` réalise la **projection** : elle peut
renvoyer une propriété, plusieurs (via un **type anonyme**), ou un nouvel objet.

```vb
Dim synthese = From p In produits
               Where p.Categorie = "Périphérique"
               Select New With {Key p.Nom, p.Prix}
```

> En VB, un type anonyme se crée avec `New With {…}` ; le modificateur **`Key`** marque les propriétés qui
> participent à l'égalité et au hachage (et les rend en lecture seule) — une finesse propre à VB.

### Pourquoi LINQ est un point fort de VB ⭐

La syntaxe de requête VB propose des **mots-clés** que C# n'offre pas en syntaxe de requête (où il faut
basculer en méthodes) : `Distinct`, `Skip`, `Take`, `Skip While`, `Take While`, et surtout
**`Aggregate`** :

```vb
' Agrégation directement en syntaxe de requête (spécifique à VB)
Dim total = Aggregate p In produits Into Sum(p.Prix)
Dim moyenne = Aggregate p In produits Into Average(p.Prix)

' Pagination et dédoublonnage comme clauses de requête
Dim categories = From p In produits Select p.Categorie Distinct
Dim page2 = From p In produits Order By p.Nom Skip 10 Take 10
```

Le **regroupement** s'exprime aussi très naturellement, avec calcul d'agrégats par groupe :

```vb
Dim parCategorie = From p In produits
                   Group p By p.Categorie Into Nombre = Count(), PrixMoyen = Average(p.Prix)
' Chaque résultat expose : .Categorie, .Nombre, .PrixMoyen
```

---

## Syntaxe par méthodes (méthodes d'extension)

La même requête s'écrit par enchaînement de méthodes, avec des **lambdas** VB (`Function(…) …`, voir
[section 3.6](../03-poo/06-evenements-delegues.md)) :

```vb
Dim abordables = produits.
    Where(Function(p) p.Prix < 50D).
    OrderBy(Function(p) p.Prix).
    Select(Function(p) p.Nom)
```

> La continuation de ligne est **implicite** après le point `.` en fin de ligne — pas besoin de `_`.

Les opérateurs standard couvrent tous les besoins : `Where`, `Select`, `SelectMany` (aplatir),
`OrderBy` / `OrderByDescending` / `ThenBy`, `GroupBy`, `Join`, `Distinct`, `Union` / `Intersect` /
`Except`, `Skip` / `Take` / `SkipWhile` / `TakeWhile`, `First` / `FirstOrDefault` / `Single` / `Last`,
`Any` / `All` / `Contains`, `Count` / `Sum` / `Average` / `Min` / `Max` / `Aggregate`, et les
matérialisations `ToList` / `ToArray` / `ToDictionary` / `ToHashSet`.

---

## Requête ou méthodes : laquelle choisir ?

Les deux formes sont **équivalentes** (la syntaxe de requête est compilée en appels de méthodes). En
pratique :

- la **syntaxe de requête** est plus lisible pour les requêtes à plusieurs clauses (jointures,
  regroupements, `Let`, tri) — et, en VB, elle couvre davantage d'opérateurs ;
- la **syntaxe par méthodes** reste nécessaire pour les opérateurs **terminaux** sans mot-clé (`Count`,
  `Sum`, `First`, `ToList`…). On termine alors souvent une requête par un appel de méthode :

```vb
Dim nombre = (From p In produits Where p.Prix > 50D).Count()
Dim liste = (From p In produits Order By p.Prix Select p.Nom).ToList()
```

---

## Exécution différée vs immédiate ⚠️

C'est un point essentiel. La plupart des opérateurs (`Where`, `Select`, `OrderBy`, `Group By`…) sont à
**exécution différée** : la requête n'est **pas exécutée à sa définition**, mais à chaque **parcours**
(`For Each`, `ToList`, `Count`…).

```vb
Dim chers = From p In produits Where p.Prix > 100D Select p   ' défini, pas exécuté

produits.Add(New Produit With {.Nom = "Station", .Prix = 900D})

For Each p In chers   ' exécuté MAINTENANT → inclut "Station"
    Console.WriteLine(p.Nom)
Next
```

Deux conséquences :

- une requête différée **reflète les changements** de la source survenus entre sa définition et son
  parcours ;
- la **re-parcourir** la **ré-exécute** (coût répété). Pour figer un instantané, matérialisez avec
  **`ToList`** / `ToArray`.

Les opérateurs **immédiats** (`ToList`, `ToArray`, `Count`, `Sum`, `First`, `Any`…) exécutent, eux, la
requête sur-le-champ.

---

## LINQ to Entities (→ module 7)

Le même savoir-faire LINQ s'applique aux bases de données via **EF Core**, mais avec une différence
majeure : la requête porte sur un **`IQueryable(Of T)`** (et non `IEnumerable(Of T)`). Au lieu d'exécuter
des délégués en mémoire, le fournisseur construit un **arbre d'expression** qu'il **traduit en SQL**,
exécuté dans la base lors du parcours.

```vb
' Aperçu (détaillé au module 7.2) :
Dim chers = From p In contexte.Produits
            Where p.Prix > 100D
            Order By p.Nom
            Select p          ' traduit en SQL, exécuté côté base
```

Trois points à retenir, approfondis au [module 7.2](../07-acces-donnees/02-ef-core-10.md) :

- **`IQueryable` → SQL** : tant qu'on reste sur `IQueryable`, les opérateurs s'ajoutent à la requête SQL.
  Dès qu'on matérialise (`ToList`, `AsEnumerable`), la suite s'exécute **en mémoire**.
- **Limites de traduction** : toutes les méthodes .NET ne sont pas traduisibles en SQL. Une opération non
  traduisible doit venir **après** matérialisation.
- **Variantes asynchrones** : EF Core fournit `ToListAsync`, `FirstOrDefaultAsync`, etc. (en lien avec le
  [module 4](../04-async/README.md)).

---

## Et l'IA dans tout ça ? 🤖

LINQ est globalement bien traité par les assistants, à quelques nuances près :

- **Lambdas** : en VB, on écrit **`Function(p) p.Prix < 50`** (ou `Sub(…)`), **jamais `p => …`** (la
  flèche `=>` de C# n'existe pas en VB).
- **Génériques** : `IEnumerable<T>` → **`IEnumerable(Of T)`**.
- **Syntaxe de requête** : un assistant entraîné sur C# tend à tout convertir en **syntaxe par
  méthodes**, perdant la lisibilité — et parfois la concision — de la requête VB (notamment la requête
  `Aggregate`, sans équivalent en syntaxe de requête C#). Demandez explicitement la **syntaxe de requête
  VB** quand elle est plus claire.

Bon point : les **noms des opérateurs** (`Where`, `Select`, `OrderBy`…) sont identiques dans les deux
langages. La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- LINQ interroge toute source via des fournisseurs (**to Objects**, **to Entities**, to XML), en
  **syntaxe de requête** ou **par méthodes** (équivalentes).
- La **syntaxe de requête VB** est un point fort ⭐ : mots-clés `Distinct`, `Skip`, `Take`,
  `Group By … Into`, et la requête **`Aggregate`** propre à VB.
- En syntaxe par méthodes, les lambdas s'écrivent **`Function(…) …`** (pas `=>`).
- **Exécution différée** par défaut (re-parcours = ré-exécution) ; matérialisez avec `ToList` pour figer.
- **LINQ to Entities** réutilise ces compétences sur `IQueryable`, **traduit en SQL** — avec ses limites
  de traduction (→ [module 7](../07-acces-donnees/02-ef-core-10.md)).

---

⏭️ [Génériques avancés (contraintes, méthodes génériques, variance)](/02-fondamentaux-langage/10-generiques-avances.md)
