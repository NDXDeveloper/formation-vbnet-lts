🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 8.5 ⚠️ gRPC et GraphQL — outillage orienté C#, à déléguer

> **Module 8 — Consommer et exposer des services** · gRPC · GraphQL · la frontière VB / C# → [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)

gRPC et GraphQL sont deux approches puissantes de communication entre services. Leur point commun, du point de vue de VB.NET, est décisif : **leur outillage .NET est bâti autour de la génération de code C#**. Cette section explique *pourquoi*, précise ce qui reste **consommable depuis VB**, et montre comment **déléguer le reste à C#**. Le sujet figure en bonne place à l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md) (§ B.5).

---

## 🎯 Le point commun : une génération de code orientée C#

| Technologie | Mécanisme de génération | Cible |
|-------------|-------------------------|-------|
| **gRPC** | `Grpc.Tools` compile les fichiers `.proto` (via `protoc` + plugin gRPC) | **C#** uniquement |
| **GraphQL — client typé** | StrawberryShake, via des **générateurs de source** | **C#** uniquement |
| **GraphQL — serveur** | HotChocolate, types et attributs **C#** (+ générateurs de source) | **C#** centré |

C'est la racine du problème : ces outils n'émettent pas — et ne savent pas consommer — de VB. La nuance, on va le voir, est plus favorable pour GraphQL que pour gRPC.

---

## 🔴 gRPC — à déléguer (client compris)

### En bref

gRPC est un framework RPC haute performance sur **HTTP/2** et **Protocol Buffers** (*protobuf*). Il est **contract-first** : on décrit services et messages dans des fichiers `.proto`, à partir desquels le code est généré.

### Pourquoi c'est hors périmètre VB

La génération depuis `.proto` est assurée par le paquet **`Grpc.Tools`**, qui invoque le compilateur protobuf et le **plugin C#** — il produit exclusivement du **C#**. Tenter de l'utiliser dans un projet VB échoue d'ailleurs explicitement dès le build (vérifié sur .NET 10) : *« proto compilation is only supported by default in a C# project (extension .csproj) »*.

Point essentiel : **le serveur et le client requièrent tous deux cette génération**. Le *stub* client (`...Client`) est lui aussi du C# généré, tout comme les classes de messages *protobuf*. Il n'y a donc **pas de chemin pratique en VB pur**, même pour appeler un service gRPC.

### Comment l'utiliser quand même : isoler en C#

La solution officielle est limpide : **placer le `.proto` et la génération dans un projet C#**, puis le référencer depuis VB. Une fois compilés, les types générés sont de simples types .NET, parfaitement consommables en VB.

```xml
<!-- Bibliothèque C# : Catalogue.Grpc.Client.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Grpc.Net.Client" Version="2.*" />
    <PackageReference Include="Google.Protobuf" Version="3.*" />
    <PackageReference Include="Grpc.Tools" Version="2.*" PrivateAssets="All" />
  </ItemGroup>
  <ItemGroup>
    <!-- Génère uniquement le client gRPC (C#) à partir du .proto -->
    <Protobuf Include="Protos\catalogue.proto" GrpcServices="Client" />
  </ItemGroup>
</Project>
```

```vb
' Projet VB : référence la bibliothèque C# ci-dessus
Imports Grpc.Net.Client
Imports Catalogue.Grpc          ' espace de noms généré dans la bibliothèque C#

Dim canal = GrpcChannel.ForAddress("https://localhost:5001")
Dim client = New CatalogueService.CatalogueServiceClient(canal)

Dim reponse = Await client.ObtenirProduitAsync(New ProduitRequete With {.Id = 42})
Console.WriteLine(reponse.Nom)
```

Le métier reste en VB ; seule la **brique gRPC** (proto + code généré) vit en C# — c'est la stratégie hybride du module 10 appliquée telle quelle.

### 💡 L'échappatoire pragmatique : le *JSON transcoding*

gRPC propose le **JSON transcoding** : en l'activant côté serveur, le même service gRPC est aussi exposé comme une **API REST/JSON**. Un client VB peut alors le consommer comme **n'importe quelle API REST** (§ 8.1), avec un simple `HttpClient` et **sans aucun outillage gRPC**. Quand on ne maîtrise pas le client mais qu'on contrôle (ou peut influencer) le serveur, c'est souvent la voie la plus simple en VB.

---

## 🟠 GraphQL — consommable en VB, outillage typé en C#

### En bref

GraphQL est un langage de requête pour API : le client demande **exactement** les données dont il a besoin via un schéma, sur un **endpoint unique**, évitant le sur- ou sous-chargement typique de REST.

### La bonne nouvelle : consommer GraphQL ne demande pas de C#

Contrairement à gRPC, **une requête GraphQL est, sur le réseau, un simple `POST` HTTP avec un corps JSON** (`{ "query": "…", "variables": { … } }`) et une réponse JSON (`{ "data": … , "errors": … }`). On peut donc consommer **n'importe quelle API GraphQL depuis VB** avec les outils du § 8.1 — `HttpClient` et `System.Text.Json` — sans aucune dépendance spécifique :

```vb
Imports System.Net.Http.Json
Imports System.Text.Json

' Corps de la requête GraphQL (objet JSON anonyme : clés "query" et "variables")
Dim requete = New With {
    .query = "query($id: ID!) { produit(id: $id) { nom prix } }",
    .variables = New With {.id = "42"}
}

Dim reponse = Await client.PostAsJsonAsync("https://exemple.com/graphql", requete)
reponse.EnsureSuccessStatusCode()

' Réponse GraphQL : { "data": { "produit": { … } }, "errors": [ … ] }
Using doc = Await JsonDocument.ParseAsync(Await reponse.Content.ReadAsStreamAsync())
    Dim produit = doc.RootElement.GetProperty("data").GetProperty("produit")
    Console.WriteLine(produit.GetProperty("nom").GetString())
End Using
```

On perd le confort du typage fort et de l'outillage, mais **le scénario de consommation fonctionne nativement en VB**.

### Ce qui est orienté C# : le serveur et le client *typé*

- **Serveur — HotChocolate** : le framework GraphQL .NET de référence définit le schéma à partir de **types et attributs C#** (`AddGraphQLServer().AddQueryType(Of Query)()`, classes `Query` annotées) et s'appuie sur des générateurs de source. Écrire un serveur GraphQL en VB n'est pas réaliste → **à déléguer à C#**.
- **Client typé — StrawberryShake** : il génère un client fortement typé à partir du schéma et des requêtes `.graphql`, via des **générateurs de source C#** (paquet `StrawberryShake.CodeGeneration.CSharp.Analyzers`). Le code est injecté dans la compilation par Roslyn — donc **C# uniquement**, inutilisable en VB.

### Le choix en VB.NET

| Besoin | Voie en VB.NET |
|--------|----------------|
| Simplement **consommer** une API GraphQL | **`HttpClient` brut** en VB (§ 8.1) — direct |
| Vouloir un **client fortement typé** | StrawberryShake en **C#**, consommé depuis VB |
| **Exposer** un serveur GraphQL | HotChocolate en **C#** (UI/métier éventuellement en VB) |

---

## 🧭 Récapitulatif : que faire en VB.NET

| Technologie | Côté serveur | Côté client / consommation |
|-------------|--------------|----------------------------|
| **gRPC** | ❌ C# (`Grpc.Tools` génère du C#) | ❌ *Stub* client en C# → isoler en bibliothèque C#<br>💡 *ou* **JSON transcoding** → consommé comme du REST en VB (§ 8.1) |
| **GraphQL** | ❌ C# (HotChocolate : types et générateurs C#) | ✅ **`HttpClient` brut** en VB (POST + JSON, § 8.1)<br>❌ Client *typé* (StrawberryShake : générateurs de source C#) |

La différence clé : pour **gRPC**, même appeler le service suppose du code C# généré (sauf JSON transcoding) ; pour **GraphQL**, la **consommation reste pleinement en VB**, seuls le serveur et le client typé basculent en C#.

---

## 🔗 Le réflexe hybride

La démarche est, une fois encore, celle du module 10 : **isoler la brique gRPC/GraphQL dans un projet C#** (service, *stub* client généré, ou serveur HotChocolate), puis la **référencer et la consommer depuis VB.NET**. Le détail de chaque sujet hors périmètre — et la justification — figure à l'**[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)** (§ B.5).

---

## 📌 À retenir

- **gRPC** est **entièrement orienté C#** : `Grpc.Tools` ne génère que du C#, et **le client comme le serveur** en dépendent. → L'isoler dans une **bibliothèque C#** consommée depuis VB ; ou, si le serveur l'autorise, le consommer en **REST via JSON transcoding** (§ 8.1).
- **GraphQL** est plus nuancé : **consommer** une API GraphQL se fait nativement en VB avec **`HttpClient` + JSON** (§ 8.1). En revanche, le **serveur** (HotChocolate) et le **client fortement typé** (StrawberryShake, générateurs de source) sont **du C#**.
- Dans les deux cas, la règle reste : VB.NET garde le métier, **C# porte l'outillage**, et l'on compose via la **stratégie hybride** (module 10, [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

---

## 🏁 Bilan du module 8

Ce module a tracé une frontière nette et honnête pour les services en VB.NET :

- ✅ **Consommer des services** (REST, § 8.1) et **exposer une Web API par contrôleurs** (§ 8.2) : pleinement du ressort de VB.NET.
- ✅ **SignalR** (§ 8.4) : temps réel réaliste, hub *et* client.
- ⚠️ **Minimal APIs** (§ 8.3) : possibles mais à contre-emploi → contrôleurs.
- ❌ **Razor / Blazor** (§ 8.3) et **gRPC / GraphQL côté création** (§ 8.5) : C#, à déléguer — tout en restant **consommables** depuis VB (GraphQL en HTTP brut ; gRPC via une bibliothèque C# ou le JSON transcoding).

Le fil conducteur : **consommer est toujours possible en VB ; créer les briques à fort outillage se délègue à C#**.

---

⬅️ [8.4 — Communication temps réel](04-temps-reel.md) · 🏠 [Module 8 — Sommaire](README.md) · ➡️ [Module 9 — Interopérabilité](../09-interoperabilite/README.md)

⏭️ [Interopérabilité](/09-interoperabilite/README.md)
