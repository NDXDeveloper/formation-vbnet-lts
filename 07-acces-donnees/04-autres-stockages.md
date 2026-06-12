🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 7.4 Autres stockages (notions)

> **Module 7 — Accès aux données** · Section 4
> Au-delà du relationnel · NoSQL documentaire (MongoDB, Cosmos DB) · Cache en mémoire (Redis)

---

## 🧭 Introduction : au-delà du relationnel

Les bases relationnelles (§ 7.1 à 7.3) couvrent l'immense majorité des besoins d'une application métier VB.NET. Mais certains scénarios appellent d'autres modèles : des données **semi-structurées** sans schéma fixe, un besoin de **scalabilité horizontale** massive, ou simplement une **mise en cache** pour soulager la base principale.

Cette section est un **panorama de notions** : l'objectif est de savoir **ce qui existe, à quoi ça sert, et quand l'envisager** — pas de fournir un tutoriel exhaustif de chaque technologie.

> ### 🔑 Le point rassurant pour VB.NET
> Toutes ces technologies s'utilisent depuis .NET via des **bibliothèques (SDK) standard** : pilote MongoDB, SDK Azure Cosmos DB, client Redis. Les consommer depuis VB.NET, c'est simplement **appeler des API .NET** — aucune limitation propre au langage. On est ici en plein dans le périmètre **« consommation »** où VB.NET est parfaitement à l'aise. Même les fournisseurs EF Core pour MongoDB et Cosmos DB fonctionnent en VB (avec les mêmes considérations de transposition C# → VB vues en § 7.2).

**Quand sortir du relationnel ?** Quelques signaux : des documents aux formes très variables, un volume et un débit qui exigent une distribution mondiale, des données par nature hiérarchiques (JSON imbriqué), ou un besoin de réponses en **sub-milliseconde** (cache). À l'inverse, dès qu'on a besoin de **jointures riches**, d'un **schéma fort** et de **transactions ACID** entre plusieurs entités, le relationnel reste le meilleur choix.

---

## 🍃 1. NoSQL : les bases documentaires

### Le principe

Une base **documentaire** stocke des **documents** (typiquement du JSON, ou du BSON binaire pour MongoDB) plutôt que des lignes dans des tables. Chaque document est autonome et peut contenir des structures imbriquées. Le schéma est **flexible** : deux documents d'une même collection peuvent avoir des champs différents. On gagne en souplesse et en scalabilité, on renonce aux jointures relationnelles et au schéma rigide.

### MongoDB

La base documentaire open source la plus répandue. Deux façons de l'utiliser depuis VB.NET.

**Le pilote natif** (`MongoDB.Driver`) — l'approche la plus mature et complète :

```vb
Imports MongoDB.Driver
Imports MongoDB.Bson

Public Class Produit
    Public Property Id As ObjectId
    <BsonElement("libelle")>
    Public Property Libelle As String
    Public Property Prix As Decimal
    Public Property Tags As List(Of String)
End Class

' Connexion et accès à une collection
Dim client As New MongoClient("mongodb://localhost:27017")
Dim base = client.GetDatabase("boutique")
Dim produits = base.GetCollection(Of Produit)("produits")

' Insertion
Await produits.InsertOneAsync(New Produit With {
    .Libelle = "Clavier",
    .Prix = 49.9D,
    .Tags = New List(Of String) From {"périphérique", "bureau"}
})

' Requête (LINQ via le pilote)
Dim chers = Await produits.Find(Function(p) p.Prix > 30D).ToListAsync()
```

**Le fournisseur EF Core** (`MongoDB.EntityFrameworkCore`) — pour rester dans le paradigme `DbContext` / LINQ d'EF Core :

```vb
Public Class BoutiqueMongoContext
    Inherits DbContext

    Public Property Produits As DbSet(Of Produit)

    Public Sub New(options As DbContextOptions(Of BoutiqueMongoContext))
        MyBase.New(options)
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
        modelBuilder.Entity(Of Produit)().ToCollection("produits")
    End Sub
End Class

' Configuration
Dim client As New MongoClient("mongodb://localhost:27017")
options.UseMongoDB(client, "boutique")
```

Le fournisseur EF Core pour MongoDB est **officiel et compatible EF Core 10** (paquet en version 10.0). Réserve honnête : il prend en charge un **sous-ensemble** des fonctionnalités d'EF Core (il est plus récent que les fournisseurs relationnels). Pour des scénarios avancés, le **pilote natif** et son propre fournisseur LINQ restent souvent plus complets.

### Azure Cosmos DB

La base **multi-modèle distribuée mondialement** de Microsoft, pensée pour la haute disponibilité et l'échelle planétaire. Elle expose plusieurs **API** (NoSQL/Core, MongoDB, Cassandra, Gremlin, Table), de sorte qu'on peut souvent réutiliser des compétences existantes.

```vb
' SDK natif : Microsoft.Azure.Cosmos  →  CosmosClient
' Fournisseur EF Core : Microsoft.EntityFrameworkCore.Cosmos (officiel Microsoft)

options.UseCosmos(
    accountEndpoint:="https://moncompte.documents.azure.com:443/",
    accountKey:="********",
    databaseName:="Boutique")
```

Deux notions structurantes à connaître avant de se lancer :

- **Les Request Units (RU)** : Cosmos facture et limite le débit en « unités de requête ». Le **dimensionnement des RU** est un sujet central de coût et de performance.
- **La clé de partition** : un bon choix de clé de partition conditionne la scalabilité et la performance. C'est une décision de conception **difficile à modifier ensuite** — à réfléchir en amont.

> ### ⚠️ Verrouillage fournisseur (*vendor lock-in*)
> Cosmos DB est un service **propriétaire Azure**. MongoDB, à l'inverse, fonctionne partout (multi-cloud, sur site). Ce critère d'indépendance pèse souvent dans le choix entre les deux.

---

## ⚡ 2. Redis : le cache en mémoire

### Le principe

**Redis** est un magasin **clé-valeur en mémoire**, extrêmement rapide (réponses en sub-milliseconde). Son usage le plus courant est le **cache** : on y range des résultats coûteux à calculer ou à requêter, pour soulager la base principale et accélérer l'application.

### Le client natif

`StackExchange.Redis` est le client .NET de référence.

```vb
Imports StackExchange.Redis

' La connexion est COÛTEUSE à créer : on la conserve et on la réutilise (singleton)
Dim connexion = ConnectionMultiplexer.Connect("localhost:6379")
Dim cache = connexion.GetDatabase()

' Écrire avec une expiration (TTL) de 5 minutes
Await cache.StringSetAsync("client:42", jsonClient, TimeSpan.FromMinutes(5))

' Lire
Dim valeur = Await cache.StringGetAsync("client:42")
If valeur.HasValue Then
    ' Cache hit : utiliser la valeur sans toucher la base
End If
```

### L'abstraction `IDistributedCache`

Dans une application qui utilise l'injection de dépendances, on préfère souvent l'abstraction standard `IDistributedCache` (paquet `Microsoft.Extensions.Caching.StackExchangeRedis`), qui découple le code du client concret :

```vb
services.AddStackExchangeRedisCache(
    Sub(opts) opts.Configuration = "localhost:6379")

' Puis, via l'IDistributedCache injecté :
Await cache.SetStringAsync("cle", valeur,
    New DistributedCacheEntryOptions With {
        .AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    })
Dim resultat = Await cache.GetStringAsync("cle")
```

### Au-delà du cache

Redis sert aussi de magasin de **sessions**, de support à des **verrous distribués**, à de la **limitation de débit** (*rate limiting*), à des **classements** (*sorted sets*) ou à de la messagerie **publish/subscribe**.

> ### Le motif *cache-aside*
> Le schéma de cache le plus répandu : on **regarde d'abord dans le cache** ; en cas d'absence (*cache miss*), on lit la **base**, on **remplit le cache** (avec un TTL), puis on renvoie la valeur. Les lectures suivantes sont alors servies depuis Redis.
>
> ⚠️ **Redis est volatil par défaut.** C'est un cache, **pas** un magasin primaire : n'y conservez jamais l'unique copie d'une donnée critique sans persistance configurée et stratégie d'invalidation maîtrisée.

---

## 🧭 Synthèse : quel stockage pour quel besoin ?

| Besoin | Piste |
|---|---|
| Données métier structurées, jointures, transactions | **Relationnel** (§ 7.1–7.3) — le défaut |
| Documents souples, schéma variable, JSON imbriqué | **MongoDB** |
| Échelle mondiale, multi-modèle, sur Azure | **Cosmos DB** |
| Accélérer les lectures, soulager la base | **Redis** (cache) |
| Sessions, verrous distribués, classements, pub/sub | **Redis** |

---

## ✅ À retenir

- **Le relationnel reste le défaut** ; on sort du relationnel pour des raisons précises : schéma souple, échelle massive, ou cache.
- **MongoDB** : base documentaire open source, accessible via le **pilote natif** (complet) ou un **fournisseur EF Core officiel** compatible EF Core 10 (sous-ensemble de fonctionnalités).
- **Cosmos DB** : base distribuée multi-modèle d'Azure ; penser **RU** et **clé de partition** en amont, et garder à l'esprit le **verrouillage fournisseur**.
- **Redis** : cache clé-valeur en mémoire (`StackExchange.Redis` ou `IDistributedCache`) ; motif *cache-aside*, mais **jamais comme magasin primaire** d'une donnée critique.
- **Pour VB.NET, aucun obstacle** : ce ne sont que des bibliothèques .NET à **consommer** — le terrain de confort du langage.

---

> **Prochaine étape →** [§ 7.5 — Sérialisation](05-serialisation.md) : `System.Text.Json`, XML et CSV — comment transformer vos objets en formats d'échange et de stockage, un besoin transversal à toutes les approches vues jusqu'ici.

⏭️ [Sérialisation (System.Text.Json, XML, CSV)](/07-acces-donnees/05-serialisation.md)
