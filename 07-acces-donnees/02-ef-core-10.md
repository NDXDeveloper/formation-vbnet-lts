🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 7.2 Entity Framework Core 10 🆕

> **Module 7 — Accès aux données** · Section 2
> L'ORM moderne de .NET · `DbContext` / `DbSet`, Code-First & migrations, Database-First, LINQ to Entities, relations, chargement, asynchrone, concurrence, et les nouveautés EF Core 10

---

## 🧭 Introduction : pourquoi un ORM ?

Là où ADO.NET (§ 7.1) vous fait écrire le SQL à la main et transformer vous-même chaque ligne en objet, **Entity Framework Core** (EF Core) est un **ORM** (*Object-Relational Mapper*) : vous décrivez vos données sous forme de **classes .NET**, et EF Core se charge de générer le SQL, d'exécuter les requêtes et de matérialiser les résultats en objets. Vous gagnez en productivité, en lisibilité et en sécurité (les requêtes sont paramétrées d'office).

EF Core **n'élimine pas** ADO.NET : il s'appuie dessus. Tout ce que vous avez vu en § 7.1 (connexions, commandes, transactions, pool) continue d'opérer « sous le capot ». Et pour les requêtes les plus exigeantes, on peut toujours redescendre au SQL brut depuis EF Core.

EF Core 10 est la version livrée avec **.NET 10 LTS** (novembre 2025), et elle est **pleinement utilisable en VB.NET** — c'est une bibliothèque .NET ordinaire, au cœur du périmètre « consommation » du langage.

> ### ⚠️ Une réalité à garder en tête
> La documentation officielle d'EF Core et la quasi-totalité des exemples en ligne sont **en C#**. Vous devrez donc régulièrement **transposer** ces exemples en VB.NET. Cette section le fait pour vous, mais c'est une compétence à cultiver — et un terrain où l'IA (module 17) aide beaucoup, à condition de **toujours valider** sa sortie. Quelques pièges de transposition récurrents sont signalés au fil du texte.

---

## ⚙️ 1. Installation

EF Core se compose d'un **paquet cœur** et d'un **paquet fournisseur** par SGBD. Pour SQL Server :

```
Microsoft.EntityFrameworkCore.SqlServer   ' le fournisseur (entraîne le cœur)
Microsoft.EntityFrameworkCore.Design      ' nécessaire aux migrations et au scaffolding
Microsoft.EntityFrameworkCore.Tools       ' commandes de la console PMC (Add-Migration…)
```

Pour les outils en ligne de commande (multiplateforme), on installe aussi l'outil global :

```
dotnet tool install --global dotnet-ef
```

Selon le SGBD, on remplace le fournisseur :

| SGBD | Paquet fournisseur EF Core |
|---|---|
| SQL Server / Azure SQL | `Microsoft.EntityFrameworkCore.SqlServer` |
| SQLite | `Microsoft.EntityFrameworkCore.Sqlite` |
| PostgreSQL | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| MySQL / MariaDB | `Pomelo.EntityFrameworkCore.MySql` |

---

## 🧱 2. Le `DbContext` et les `DbSet`

Deux briques fondamentales structurent tout projet EF Core :

- les **entités** : de simples classes qui représentent vos tables ;
- le **`DbContext`** : la session de travail avec la base, qui expose un **`DbSet(Of T)`** par entité.

Définissons un petit modèle métier (un client et ses commandes) :

```vb
Public Class Client
    Public Property Id As Integer
    Public Property Nom As String = ""
    Public Property Email As String
    Public Property DateInscription As Date

    ' Propriété de navigation : les commandes de ce client (relation 1-N)
    Public Property Commandes As ICollection(Of Commande) = New List(Of Commande)
End Class

Public Class Commande
    Public Property Id As Integer
    Public Property Reference As String = ""
    Public Property Montant As Decimal
    Public Property DateCommande As Date

    ' Clé étrangère (convention : <Navigation>Id) + navigation inverse
    Public Property ClientId As Integer
    Public Property Client As Client
End Class
```

Le `DbContext` :

```vb
Imports Microsoft.EntityFrameworkCore

Public Class BoutiqueContext
    Inherits DbContext

    Public Property Clients As DbSet(Of Client)
    Public Property Commandes As DbSet(Of Commande)

    Protected Overrides Sub OnConfiguring(options As DbContextOptionsBuilder)
        options.UseSqlServer(
            "Server=(localdb)\MSSQLLocalDB;Database=Boutique;Trusted_Connection=True;TrustServerCertificate=True;")
    End Sub
End Class
```

> ### ⚠️ Pièges de transposition C# → VB ici
> - **`required`** (mot-clé C# pour rendre une propriété obligatoire) **n'existe pas en VB.NET**. On l'omet, en gérant l'initialisation autrement (valeur par défaut, constructeur, ou contrainte EF `IsRequired`).
> - **`record`** ne se **déclare** pas en VB. Pour une entité EF, ce n'est pas gênant : on utilise une `Class` (voir § 3.7 pour consommer des *records* C#).

**En application réelle (ASP.NET Core, Generic Host), on préfère l'injection de dépendances** plutôt que `OnConfiguring`. Le contexte reçoit alors ses options par le constructeur :

```vb
' Dans la classe BoutiqueContext :
Public Sub New(options As DbContextOptions(Of BoutiqueContext))
    MyBase.New(options)
End Sub

' À l'enregistrement des services :
services.AddDbContext(Of BoutiqueContext)(
    Sub(options) options.UseSqlServer(chaineConnexion))
```

Le `DbContext` **implémente `IDisposable`** : on l'entoure d'un `Using` (ou on le laisse gérer par le conteneur d'injection). Il n'est **pas thread-safe** : une instance = une unité de travail, de courte durée.

---

## 🏗️ 3. Code-First et migrations

L'approche **Code-First** consiste à définir le modèle en code, puis à laisser EF Core générer (et faire évoluer) le schéma de base. La configuration passe par trois mécanismes complémentaires :

**Les conventions** — EF déduit beaucoup automatiquement : une propriété `Id` devient la clé primaire, `ClientId` devient une clé étrangère vers `Client`, etc.

**Les annotations de données** — des attributs sur les propriétés :

```vb
Imports System.ComponentModel.DataAnnotations

Public Class Client
    <Key>
    Public Property Id As Integer

    <Required, MaxLength(100)>
    Public Property Nom As String = ""

    <MaxLength(255)>
    Public Property Email As String
    ' …
End Class
```

**L'API Fluent** — dans `OnModelCreating`, pour les configurations plus fines (la méthode recommandée pour les cas complexes) :

```vb
Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
    modelBuilder.Entity(Of Client)(
        Sub(entity)
            entity.HasKey(Function(c) c.Id)
            entity.Property(Function(c) c.Nom).IsRequired().HasMaxLength(100)
            entity.HasIndex(Function(c) c.Email).IsUnique()
        End Sub)

    modelBuilder.Entity(Of Commande)(
        Sub(entity)
            entity.Property(Function(c) c.Montant).HasPrecision(18, 2)
            entity.HasOne(Function(c) c.Client) _
                  .WithMany(Function(cl) cl.Commandes) _
                  .HasForeignKey(Function(c) c.ClientId)
        End Sub)
End Sub
```

*Note de syntaxe VB : les lambdas d'expression s'écrivent `Function(c) c.Id` ; les lambdas multi-instructions `Sub(entity) … End Sub` ; et les chaînes Fluent sur plusieurs lignes utilisent la continuation `_`.*

Le modèle prêt, les **migrations** créent et font évoluer le schéma. En console du gestionnaire de paquets (PMC) :

```
Add-Migration CreationInitiale
Update-Database
```

…ou en ligne de commande (CLI) :

```
dotnet ef migrations add CreationInitiale
dotnet ef database update
```

> ### ⚠️ VB.NET et migrations : la génération de code est C# uniquement (vérifié sur EF Core 10)
> Dans un projet VB, `Add-Migration` / `dotnet ef migrations add` échoue avec un message sans ambiguïté :
> *« The project language 'VB' isn't supported by the built-in IMigrationsCodeGenerator service »*. EF Core ne
> sait générer les fichiers de migration **qu'en C#** ; le paquet communautaire `EntityFrameworkCore.VisualBasic`,
> qui comblait ce manque, s'arrête à **EF Core 8**. Sur .NET 10, deux voies pragmatiques :
>
> - **la voie hybride 🔗 (recommandée)** — placer le `DbContext` et ses migrations dans une **bibliothèque C#**
>   référencée par l'application VB (les entités peuvent rester dans un projet VB, que la bibliothèque C#
>   référence). C'est l'option que suggère le message d'erreur lui-même… et exactement l'architecture du
>   [module 10](../10-hybride-vbnet-csharp/README.md). Seule la **génération** est concernée : l'**application**
>   des migrations (`Update-Database`, `dotnet ef database update`, `contexte.Database.Migrate()`) fonctionne
>   ensuite quel que soit le langage de l'application ;
> - **sans migrations** — pour une petite application locale, `EnsureCreated()` crée le schéma d'un coup
>   (sans historique d'évolution) ; au-delà, on gère le schéma par **scripts SQL versionnés**.

Chaque migration est une classe avec une méthode `Up` (appliquer) et `Down` (annuler), versionnée dans votre dépôt. `Update-Database` applique les migrations en attente ; on peut aussi générer un **script SQL** (`Script-Migration` / `dotnet ef migrations script`) à faire valider avant un déploiement en production.

---

## 🔧 4. Database-First : le *scaffolding* (précieux pour le legacy) ⭐

Quand la base **existe déjà** — cas extrêmement fréquent en maintenance d'applications VB.NET — on procède en sens inverse : le **reverse engineering** (ou *scaffolding*) génère automatiquement les entités et le `DbContext` à partir du schéma existant. Les commandes ci-dessous se lancent **dans le projet qui recevra le code généré** — en pratique une bibliothèque C#, comme l'explique l'encadré qui suit.

En PMC :

```
Scaffold-DbContext "Server=(localdb)\MSSQLLocalDB;Database=Boutique;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context BoutiqueContext
```

En CLI :

```
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=Boutique;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context BoutiqueContext
```

Quelques options utiles : `-Tables` / `--table` pour ne cibler que certaines tables, `-DataAnnotations` pour utiliser les attributs plutôt que l'API Fluent, `-Force` / `--force` pour réécrire.

> ### ⚠️ Le *scaffolding* ne génère pas de VB (vérifié sur EF Core 10)
> Lancé dans un **projet VB**, le scaffolding échoue : *« The project language 'VB' isn't supported by the
> built-in IModelCodeGenerator service »*. Comme pour les migrations, la **génération de code** d'EF Core est
> **C# uniquement** — le paquet communautaire `EntityFrameworkCore.VisualBasic` s'arrête à EF Core 8, et
> l'extension **EF Core Power Tools** (sélection graphique des tables, options avancées) cible elle aussi les
> projets **C#**. Les voies réalistes sur .NET 10 :
>
> - **scaffolder dans une bibliothèque C#** dédiée à l'accès aux données, consommée depuis l'application VB —
>   l'architecture hybride 🔗 du [module 10](../10-hybride-vbnet-csharp/README.md), encore elle ; EF Core
>   Power Tools devient alors pleinement utilisable sur cette bibliothèque ;
> - pour un schéma modeste, **écrire les entités VB à la main** (ce sont de simples classes — celles du § 2)
>   et laisser les conventions et l'API Fluent faire le mapping ;
> - recourir à un outil commercial qui génère nativement du VB (Entity Developer, de Devart).

Le code généré — dans la bibliothèque C#, donc — est un **point de départ** : on l'ajuste (noms de navigations, types) et, à partir de là, on peut basculer en Code-First pour faire évoluer le schéma par migrations.

---

## 🔍 5. Interroger : LINQ to Entities

C'est ici qu'EF Core brille en VB.NET, car **LINQ** est intégré à la syntaxe du langage (voir § 2.9). Une requête LINQ sur un `DbSet` est **traduite en SQL** et exécutée côté base.

VB.NET propose les deux syntaxes. La **syntaxe méthode** :

```vb
Using contexte As New BoutiqueContext()
    Dim recents = Await contexte.Clients _
        .Where(Function(c) c.DateInscription >= New Date(2025, 1, 1)) _
        .OrderBy(Function(c) c.Nom) _
        .ToListAsync()
End Using
```

…et la **syntaxe requête**, souvent très lisible :

```vb
Dim requete = From c In contexte.Clients
              Where c.DateInscription >= New Date(2025, 1, 1)
              Order By c.Nom
              Select c

Dim recents = Await requete.ToListAsync()
```

Deux notions essentielles :

**L'exécution différée.** Une requête LINQ n'est **pas** exécutée à sa définition, mais seulement à son **énumération** (`ToListAsync`, `FirstAsync`, une boucle `For Each`…). Tant qu'on ne l'énumère pas, on ne fait que **composer** la requête — propriété centrale qu'on exploite pour le filtrage dynamique (§ 8).

> ### ⚠️ Évaluation côté serveur vs côté client
> Depuis EF Core 3, si une partie de votre requête **ne peut pas être traduite en SQL**, EF lève une **exception** plutôt que de rapatrier silencieusement les données pour les filtrer en mémoire (ce qui pouvait écrouler les performances). C'est une protection : si vous tombez dessus, simplifiez l'expression ou matérialisez explicitement (`ToList`) avant la partie non traduisible.

### Et quand LINQ ne suffit pas : redescendre au SQL brut

L'introduction le promettait : EF Core permet de **revenir ponctuellement au SQL** pour une requête très spécifique — sans renoncer à la sécurité, car la chaîne interpolée est convertie en **commande paramétrée** (les valeurs ne sont jamais concaténées dans le SQL, fidèle au fil rouge du § 7.1) :

```vb
' Requête SQL brute, paramétrée automatiquement via l'interpolation
Dim clients = Await contexte.Clients _
    .FromSqlInterpolated($"SELECT * FROM Clients WHERE Nom = {nomSaisi}") _
    .ToListAsync()

' Commande sans résultat (UPDATE, DELETE…), paramétrée de la même façon
Dim lignes = Await contexte.Database _
    .ExecuteSqlAsync($"UPDATE Clients SET EstActif = 1 WHERE Nom = {nomSaisi}")
```

> ⚠️ **Piège de transposition C# → VB, vérifié sur .NET 10 :** les exemples C# utilisent `FromSql($"…")` —
> en VB, cet appel échoue avec une **ambiguïté de surcharge** (`BC30521` : « aucun 'FromSql' accessible n'est
> plus spécifique »), la résolution VB départageant mal les surcharges entre `DbSet` et `IQueryable`.
> Utilisez son synonyme historique **`FromSqlInterpolated`**, strictement équivalent et sans ambiguïté
> (`ExecuteSqlAsync`, lui, s'emploie directement). Et pour du SQL dont les paramètres sont déjà construits,
> `FromSqlRaw` existe — à réserver aux cas où **aucune** valeur ne vient de l'utilisateur.

Enfin, pour **voir le SQL** qu'EF Core génère à partir de votre LINQ — réflexe de diagnostic essentiel (et la réponse au « problème N+1 » de l'[Annexe G](../annexes/faq-depannage/README.md)) :

```vb
' Ponctuellement : le SQL d'une requête précise
Dim requete = contexte.Clients.Where(Function(c) c.EstActif)
Console.WriteLine(requete.ToQueryString())

' Globalement : journaliser tout le SQL émis par le contexte
options.UseSqlServer(chaineConnexion) _
       .LogTo(AddressOf Console.WriteLine, LogLevel.Information)   ' Imports Microsoft.Extensions.Logging
```

---

## 🔗 6. Les relations

EF Core modélise les relations via les **propriétés de navigation** et les **clés étrangères**. Les trois cardinalités :

- **Un-à-plusieurs** (le cas le plus courant) — un `Client` a plusieurs `Commande`. C'est la relation configurée plus haut (`HasOne(...).WithMany(...).HasForeignKey(...)`). La convention seule (`ClientId` + navigation `Client`) suffit souvent.
- **Plusieurs-à-plusieurs** — par exemple `Commande` ↔ `Produit`. Depuis EF Core 5, on déclare simplement une collection de chaque côté (*skip navigations*) et EF crée et gère la **table de jonction** automatiquement.
- **Un-à-un** — par exemple `Client` ↔ `Adresse`, avec une clé étrangère unique d'un côté.

```vb
' Plusieurs-à-plusieurs : EF crée la table de jonction tout seul
Public Class Commande
    ' …
    Public Property Produits As ICollection(Of Produit) = New List(Of Produit)
End Class

Public Class Produit
    Public Property Id As Integer
    Public Property Libelle As String = ""
    Public Property Commandes As ICollection(Of Commande) = New List(Of Commande)
End Class
```

---

## 📥 7. Le chargement des données liées : *eager*, *lazy*, *explicit*

Quand on charge un `Client`, ses `Commandes` sont-elles chargées en même temps ? Trois stratégies existent.

**Chargement anticipé (*eager*) — recommandé par défaut.** On demande explicitement les données liées avec `Include` (et `ThenInclude` pour les niveaux imbriqués). Tout part en une (ou peu de) requête(s) maîtrisée(s) :

```vb
Dim commandes = Await contexte.Commandes _
    .Include(Function(c) c.Client) _
    .ToListAsync()

' Imbriqué : clients → leurs commandes → les produits de chaque commande
Dim clients = Await contexte.Clients _
    .Include(Function(c) c.Commandes) _
        .ThenInclude(Function(cmd) cmd.Produits) _
    .ToListAsync()
```

**Chargement différé (*lazy*).** Les données liées se chargent **automatiquement** au premier accès à la propriété de navigation. Pratique, mais à manier avec prudence. Il faut :

1. installer le paquet `Microsoft.EntityFrameworkCore.Proxies` et activer `options.UseLazyLoadingProxies()` ;
2. **déclarer les propriétés de navigation `Overridable`** — c'est le point VB.NET à ne pas manquer.

```vb
' Configuration
options.UseLazyLoadingProxies() _
       .UseSqlServer(chaineConnexion)

' Entité : la navigation DOIT être Overridable
Public Class Client
    ' …
    Public Overridable Property Commandes As ICollection(Of Commande) = New List(Of Commande)
End Class
```

> ### ⚠️ Deux pièges du *lazy loading*
> - **`Overridable` (VB) = `virtual` (C#).** Le proxy de lazy loading est une sous-classe générée qui **redéfinit** la navigation ; sans `Overridable`, le chargement différé ne s'active tout simplement pas. C'est l'erreur n°1 quand on transpose un exemple C# (`virtual`) sans adapter le mot-clé.
> - **Le problème « N+1 ».** Accéder à une navigation *lazy* dans une boucle déclenche **une requête SQL par élément** — désastreux sur de gros volumes. En cas de doute, préférez le chargement anticipé (`Include`).

**Chargement explicite.** On garde le contrôle total et on charge à la demande, après coup :

```vb
Dim client = Await contexte.Clients.FirstAsync()

' Charger explicitement une collection
Await contexte.Entry(client) _
    .Collection(Function(c) c.Commandes) _
    .LoadAsync()

' …ou une référence unique
Await contexte.Entry(uneCommande) _
    .Reference(Function(c) c.Client) _
    .LoadAsync()
```

---

## ⏩ 8. Asynchrone, pagination, tri et filtrage dynamique

**Les requêtes asynchrones** sont la norme en EF Core, surtout en UI ou en serveur (voir module 4). À chaque opération terminale correspond une variante `…Async` : `ToListAsync`, `FirstOrDefaultAsync`, `SingleOrDefaultAsync`, `AnyAsync`, `CountAsync`, `SumAsync`, `FindAsync`, et bien sûr **`SaveChangesAsync`** pour persister les modifications.

**La pagination** s'exprime avec `Skip` et `Take`, précédés d'un **tri déterministe** (sans `OrderBy`, l'ordre des pages n'est pas garanti) :

```vb
Public Async Function ObtenirPageAsync(numeroPage As Integer, taillePage As Integer) As Task(Of List(Of Client))
    Using contexte As New BoutiqueContext()
        Return Await contexte.Clients _
            .OrderBy(Function(c) c.Nom) _
            .Skip((numeroPage - 1) * taillePage) _
            .Take(taillePage) _
            .ToListAsync()
    End Using
End Function
```

**Le filtrage dynamique** est une application directe de l'exécution différée : on **compose** la requête conditionnellement sur un `IQueryable`, et le SQL n'est généré qu'avec les seuls critères réellement fournis.

```vb
Public Async Function RechercherCommandesAsync(nomClient As String,
                                               montantMin As Decimal?) As Task(Of List(Of Commande))
    Using contexte As New BoutiqueContext()
        ' Requête de base, NON encore exécutée
        Dim requete As IQueryable(Of Commande) = contexte.Commandes

        If Not String.IsNullOrWhiteSpace(nomClient) Then
            requete = requete.Where(Function(c) c.Client.Nom.Contains(nomClient))
        End If

        If montantMin.HasValue Then
            requete = requete.Where(Function(c) c.Montant >= montantMin.Value)
        End If

        ' Le SQL n'est construit et exécuté qu'ICI, avec les filtres retenus
        Return Await requete _
            .OrderByDescending(Function(c) c.DateCommande) _
            .ToListAsync()
    End Using
End Function
```

C'est l'un des grands atouts de LINQ + EF Core : un code de recherche lisible qui produit une requête SQL **optimale**, ajustée aux critères réels.

---

## 🔒 9. La concurrence (optimiste)

Que se passe-t-il si **deux utilisateurs modifient le même enregistrement** en même temps ? Par défaut, EF Core pratique la **concurrence optimiste** : il ne pose pas de verrou, mais **détecte** le conflit au moment d'enregistrer.

On désigne un **jeton de concurrence**. Le plus simple sous SQL Server : une colonne `rowversion`, signalée par `<Timestamp>` :

```vb
Public Class Commande
    ' …
    <Timestamp>
    Public Property Version As Byte()
End Class
```

EF inclut alors ce jeton dans la clause `WHERE` de l'`UPDATE`. Si la ligne a changé entre-temps, **zéro ligne n'est affectée** et EF lève une exception, qu'on traite :

```vb
' Rappel VB : Await est interdit dans un Catch (BC36943, voir § 7.1 et module 4.3) —
' on CAPTURE donc le conflit, et on le traite après le Try.
Dim conflit As DbUpdateConcurrencyException = Nothing
Try
    Await contexte.SaveChangesAsync()
Catch ex As DbUpdateConcurrencyException
    ' Un autre utilisateur a modifié l'enregistrement entre la lecture et l'écriture.
    conflit = ex
End Try

If conflit IsNot Nothing Then
    ' Stratégies possibles :
    '   - recharger les valeurs de la base et redemander à l'utilisateur ;
    '   - fusionner les modifications ;
    '   - faire primer une version ("le dernier l'emporte").
    Dim entree = conflit.Entries.Single()
    Dim valeursBase = Await entree.GetDatabaseValuesAsync()
    ' … logique de résolution selon le métier
End If
```

La concurrence **optimiste** convient à la majorité des applications (pas de verrou maintenu, donc meilleure scalabilité). La concurrence **pessimiste** (verrous explicites) reste réservée à des cas très particuliers.

---

## 🆕 10. Les nouveautés d'EF Core 10

Quatre apports majeurs accompagnent .NET 10. Les exemples officiels étant en C#, ils sont transposés ici en VB.NET.

### `LeftJoin` et `RightJoin` natifs

Pendant des années, une simple jointure externe gauche imposait en LINQ une combinaison verbeuse de `GroupJoin` + `SelectMany` + `DefaultIfEmpty`. EF Core 10 introduit les opérateurs **`LeftJoin`** et **`RightJoin`**, traduits directement en `LEFT JOIN` / `RIGHT JOIN` SQL :

```vb
Dim resultat = Await contexte.Commandes _
    .LeftJoin(
        contexte.Clients,
        Function(cmd) cmd.ClientId,
        Function(cl) cl.Id,
        Function(cmd, cl) New With {
            cmd.Reference,
            .NomClient = If(cl IsNot Nothing, cl.Nom, "(client inconnu)")
        }) _
    .ToListAsync()
```

Ce sont des opérateurs LINQ de **.NET 10** (donc valables aussi en LINQ to Objects), qu'EF Core sait traduire. Le résultat est immédiatement plus lisible — un bénéfice pour toute requête impliquant des relations optionnelles.

### *Named query filters* (filtres globaux nommés)

Jusqu'ici, EF Core n'autorisait **qu'un seul** filtre de requête global par entité. EF Core 10 permet d'en définir **plusieurs, nommés**, et de les **désactiver sélectivement** — idéal pour combiner, par exemple, suppression logique et multi-locataire (*multi-tenancy*) :

```vb
' Dans OnModelCreating :
modelBuilder.Entity(Of Client)() _
    .HasQueryFilter("NonSupprime", Function(c) Not c.EstSupprime)

modelBuilder.Entity(Of Client)() _
    .HasQueryFilter("ClientActif", Function(c) c.EstActif)

' À l'usage : ignorer UN filtre précis, en conservant l'autre
Dim tousYComprisInactifs = Await contexte.Clients _
    .IgnoreQueryFilters({"ClientActif"}) _
    .ToListAsync()
```

### Colonnes JSON et types complexes vers JSON

EF Core 10 permet de mapper un **type complexe** directement vers une **colonne JSON**, avec requêtage des propriétés imbriquées **traduit côté serveur** et indexation possible sur un chemin JSON :

```vb
' Un type complexe : une valeur sans identité propre (ici une structure)
Public Structure Profil
    Public Property Bio As String
    Public Property SiteWeb As String
    Public Property Theme As String
End Structure

Public Class Client
    ' …
    Public Property Profil As Profil
End Class

' Mapping en colonne JSON (OnModelCreating)
modelBuilder.Entity(Of Client)(
    Sub(entity)
        entity.ComplexProperty(Function(c) c.Profil, Sub(p) p.ToJson())
        entity.HasIndex(Function(c) c.Profil.SiteWeb)   ' index sur un chemin JSON
    End Sub)

' Requête sur une propriété imbriquée du document JSON
Dim clientsFr = Await contexte.Clients _
    .Where(Function(c) c.Profil.SiteWeb.EndsWith(".fr")) _
    .ToListAsync()
```

L'intérêt : plus de sérialisation manuelle, de meilleures performances grâce à l'indexation JSON native, et un modèle qui évolue **sans migration de schéma** pour les données semi-structurées.

### Types complexes consolidés

Les **types complexes** (introduits en EF Core 8) modélisent des objets-valeurs **sans identité ni clé propre** — par opposition aux entités. EF Core 10 étend leur prise en charge : ils acceptent désormais les **structures (`Structure`)** comme ci-dessus, se mappent en JSON, et deviennent l'option recommandée pour les objets-valeurs (adresses, coordonnées, profils…).

*Pour aller plus loin :* EF Core 10 apporte aussi `ExecuteUpdate` sur les propriétés JSON (mise à jour en masse sans charger les entités) et, avec SQL Server 2025 / Azure SQL, la prise en charge des nouveaux types natifs `json` et `vector` (recherche par similarité d'*embeddings*).

---

## 🧭 Synthèse : quand utiliser quoi ?

| Situation | Choix recommandé |
|---|---|
| Nouveau projet, schéma piloté par le code | **Code-First** + migrations (§ 3) |
| Base existante (legacy) | **Database-First** / *scaffolding* (§ 4) |
| Charger des données liées de façon maîtrisée | Chargement **anticipé** `Include` (§ 7) |
| Recherche multi-critères optionnels | **Filtrage dynamique** sur `IQueryable` (§ 8) |
| Édition concurrente d'un même enregistrement | **Concurrence optimiste** (`rowversion`) (§ 9) |
| Données semi-structurées (profil, métadonnées) | **Colonne JSON** via type complexe (§ 10) |

---

## ✅ À retenir

- **EF Core s'appuie sur ADO.NET** : il apporte productivité, lisibilité et sécurité (requêtes paramétrées d'office), sans rien retirer de la puissance sous-jacente.
- **Pleinement utilisable en VB.NET**, mais avec une doc et un outillage **C#-centriques** : la transposition est une compétence à cultiver (et l'IA aide — module 17).
- **Deux portes d'entrée** : Code-First + migrations pour le neuf ; *scaffolding* pour le legacy — en sachant que la **génération de code** (migrations comme scaffolding) est **C# uniquement** : on la confie à une bibliothèque C# dédiée (module 10 🔗).
- **LINQ to Entities + exécution différée** : composez vos requêtes, EF génère le SQL — le filtrage dynamique en est la plus belle illustration.
- **Chargement** : privilégiez l'anticipé (`Include`) ; pour le différé, n'oubliez pas que les navigations doivent être **`Overridable`** (le piège VB classique) et méfiez-vous du « N+1 ».
- **Asynchrone partout** (`…Async`, `SaveChangesAsync`) et **concurrence optimiste** (`rowversion` + `DbUpdateConcurrencyException`).
- **EF Core 10** : `LeftJoin`/`RightJoin` natifs, filtres globaux **nommés**, **colonnes JSON** via types complexes (structs compris).

---

> **Prochaine étape →** [§ 7.3 — Fournisseurs de bases de données](03-fournisseurs.md) : SQL Server, SQLite, MySQL/MariaDB et PostgreSQL — comment brancher EF Core (et ADO.NET) sur chacun, et ce qui change concrètement d'un fournisseur à l'autre.

⏭️ [Fournisseurs de bases de données](/07-acces-donnees/03-fournisseurs.md)
