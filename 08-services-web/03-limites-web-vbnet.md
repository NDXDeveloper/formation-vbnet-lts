🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 8.3 ⚠️ Limites du web en VB.NET

> **Module 8 — Consommer et exposer des services** · Minimal APIs · Razor Pages / vues MVC / Blazor · la frontière VB / C# → [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)

Le § 8.2 a montré ce que VB.NET fait réellement bien sur le web : consommer des services et exposer une Web API **par contrôleurs**. Cette section trace honnêtement la limite inverse : ce qui, sur le web, est **à contre-emploi** ou tout simplement **impossible** en VB.NET — et, surtout, *pourquoi*, pour pouvoir décider sans tâtonner.

La règle de lecture est constante dans toute la formation : une limite n'est pas un échec, c'est un signal qu'il faut **déléguer la brique concernée à C#** et la **consommer** depuis VB (stratégie hybride, module 10). L'annexe de référence pour chaque sujet hors périmètre reste l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md).

---

## 🗺️ La carte de la frontière web

Ce tableau consolide tout le module : pour chaque scénario, son statut en VB.NET, la raison, et l'orientation.

| Scénario web | En VB.NET | Pourquoi | Où aller |
|--------------|-----------|----------|----------|
| Consommer des API REST | ✅ Pleinement | Bibliothèques .NET pures | § 8.1 |
| Web API **par contrôleurs** | ✅ Recommandé | Modèle à base de classes, idiomatique | § 8.2 |
| **Minimal APIs** | ⚠️ Possible, mais à contre-emploi | Pas de modèle, pas de *top-level statements*, générateur AOT en C# | Contrôleurs (§ 8.2) ou C# |
| **Razor Pages / vues MVC** | ❌ C# uniquement | *Razor compile vers du C#* | Front séparé ou C# |
| **Blazor** (Server / WebAssembly) | ❌ C# uniquement | Composants *Razor* → C# | Front séparé ou C# |
| SignalR (client **et** hub) | ✅ Réaliste | Classes + client .NET | § 8.4 |
| **gRPC / GraphQL** (création) | ⚠️ Outillage C# | Génération de code orientée C# | § 8.5 · [Annexe B](../annexes/frontiere-vbnet-csharp/README.md) |

Deux cas méritent une explication détaillée : les **Minimal APIs** (possibles mais déconseillées) et la famille **Razor** (réellement hors de portée). C'est l'objet des deux sections qui suivent.

---

## 🔸 Minimal APIs : ça marche, mais à contre-emploi

### Oui, techniquement, ça fonctionne

Tout ce qui est .NET est accessible à VB.NET — Minimal APIs comprises. En partant d'un projet console VB basculé sur le SDK Web (comme au § 8.2), et en plaçant le code dans le `Sub Main`, on obtient une Minimal API fonctionnelle :

```vb
' Program.vb — une Minimal API en VB.NET : fonctionnelle
Imports Microsoft.AspNetCore.Builder

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)
        Dim app = builder.Build()

        app.MapGet("/", Function() "Bonjour depuis VB.NET !")

        app.Run()
    End Sub
End Module
```

Le problème n'est donc pas la compilation — c'est l'**ergonomie** et l'**outillage**.

### Mais on perd ce qui en fait l'intérêt

L'argument des Minimal APIs est la **concision** : en C#, une API tient en quelques lignes au niveau du fichier, sans cérémonie. Or, en VB, deux contraintes neutralisent précisément cette concision :

- **Pas de modèle de projet** : il faut monter le projet à la main (console → SDK `Microsoft.NET.Sdk.Web`).
- **Pas de *top-level statements* ni d'*implicit usings*** : tout vit dans `Module Program` / `Sub Main`, et chaque espace de noms doit être importé explicitement.

Autrement dit, la version VB réintroduit exactement la « cérémonie » que les Minimal APIs cherchaient à supprimer. Le bénéfice s'évapore.

### Et on perd l'optimisation moderne

Plus structurel : le **Request Delegate Generator** — le générateur de source qui, pour les Minimal APIs, produit la liaison des paramètres à la compilation, de meilleurs diagnostics et la compatibilité **Native AOT** — est un générateur de source **C#**. Il ne s'exécute pas dans un projet VB. 🔗

Conséquence : une Minimal API en VB retombe sur la liaison **par réflexion à l'exécution**, sans génération à la compilation et **sans Native AOT**. On hérite des inconvénients sans le principal atout.

### Verdict

| Besoin | Choix en VB.NET |
|--------|-----------------|
| Une API HTTP « normale » | **Contrôleurs** (§ 8.2) — idiomatique et complet |
| Des caractéristiques propres aux Minimal APIs (Native AOT, empreinte minimale, écosystème minimal-API) | **C#** — la brique se délègue ([Annexe B](../annexes/frontiere-vbnet-csharp/README.md)) |

En VB.NET, pour exposer une API, le réflexe est **contrôleurs**, sans hésitation.

---

## 🔸 Razor Pages, vues MVC, Blazor : C# uniquement

Ici, la limite est nette : **aucune de ces technologies n'est disponible en VB.NET**. Ce n'est pas une question d'ergonomie, mais d'architecture.

### Pourquoi : Razor compile vers du C#

Razor Pages, les vues MVC (`.cshtml`) et les composants Blazor (`.razor`) reposent tous sur le **moteur Razor**, qui transforme le balisage en **code C#** lors de la compilation. Or, **ASP.NET Core ne propose pas de Razor en VB** : il n'existe pas de `.vbhtml`. Tout ce qui passe par Razor est donc, par construction, du C#.

> 📜 *Note historique* : un Razor VB (`.vbhtml`) a existé, mais uniquement à l'époque d'ASP.NET Web Pages / MVC 5 sur **.NET Framework**. Il n'a jamais été porté sur ASP.NET Core. (La maintenance d'ASP.NET **Web Forms** legacy en VB est, elle, traitée au module 11.4 — c'est un problème différent : *aucun* chemin vers .NET moderne.)

Cela couvre :

- le **rendu HTML côté serveur** : Razor Pages et vues MVC ;
- l'**UI Blazor** sous toutes ses formes : Blazor Server et Blazor WebAssembly.

### Ce que ça implique concrètement

Une application VB.NET ne peut ni rendre de vues Razor, ni héberger de composants Blazor écrits en VB. Dès qu'un projet a besoin d'une **interface web rendue côté serveur** ou d'un **front applicatif Blazor**, cette couche de présentation **sera en C#**.

### Les deux voies réalistes

| Voie | Principe | Quand |
|------|----------|-------|
| **A — API VB + front séparé** | API par contrôleurs en VB (§ 8.2), et un front indépendant : SPA en JS/TS, ou un front Razor/Blazor en C# | Séparation claire back/front ; le métier reste pleinement en VB |
| **B — Hybride** | UI en C# (Razor/Blazor) consommant une **bibliothèque métier en VB** | On veut une UI web moderne tout en capitalisant sur du code VB existant |

Dans les deux cas, le métier, l'accès aux données et la logique restent là où VB.NET excelle ; seule la **présentation web** bascule côté C#.

---

## 🔗 Les autres frontières (rappel)

Cette section a traité le rendu web et les Minimal APIs. Le module aborde encore **gRPC et GraphQL** (création de services à fort outillage, orientée C#) au § 8.5. Pour le **catalogue complet** des sujets hors périmètre VB — .NET MAUI et WinUI 3, Native AOT, écriture de générateurs de source, microservices / Dapr / Kubernetes — la référence unique est l'**[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)**, qui explique pour chacun *la raison* et *comment le consommer depuis VB.NET*.

---

## 🧭 Le bon réflexe : isoler en C#, consommer depuis VB

Face à une limite web, la démarche est toujours la même :

1. **Garder en VB.NET** ce qu'il fait bien : logique métier, accès aux données, API par contrôleurs, consommation de services.
2. **Isoler la brique hors périmètre** (front Razor/Blazor, endpoint AOT, service gRPC…) dans un **projet ou une bibliothèque C#**.
3. **Composer / consommer** cette brique depuis VB.NET, via une solution mixte.

C'est exactement la stratégie hybride détaillée au **module 10**, dont ce module 8 est une application directe.

---

## 📌 À retenir

- **Minimal APIs** : *possibles* en VB (tout est .NET), mais **à contre-emploi** — l'absence de modèle et de *top-level statements* annule leur concision, et le générateur AOT étant en C#, on perd Native AOT. → Préférer les **contrôleurs** (§ 8.2), ou C#.
- **Razor Pages, vues MVC, Blazor** : **C# uniquement**, sans exception — *Razor compile vers du C#* et il n'existe pas de Razor VB dans ASP.NET Core.
- La présentation web moderne se traite donc soit en **front séparé** (API VB + SPA/C#), soit en **hybride** (UI C# + métier VB).
- Une limite n'est jamais un cul-de-sac : on **isole en C#** et on **consomme depuis VB** — voir le module 10 et l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md).

---

⬅️ [8.2 — Exposer une Web API ASP.NET Core en VB.NET](02-web-api-controllers.md) · ➡️ [8.4 — Communication temps réel](04-temps-reel.md)

⏭️ [Communication temps réel](/08-services-web/04-temps-reel.md)
