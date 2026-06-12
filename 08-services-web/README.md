🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 🌐 8. Consommer et exposer des services

> **Partie 4 — Services, web et temps réel (périmètre réaliste)**

Une application moderne vit rarement seule. Elle interroge des API tierces, expose ses propres services à d'autres applications et échange parfois des données en temps réel. Ce module traite de cette communication entre applications — un domaine où **VB.NET occupe une place précise qu'il faut comprendre avant d'écrire la moindre ligne**.

La bonne nouvelle : tout ce qui relève de la **consommation de services** est pleinement du ressort de VB.NET. Un `HttpClient` est une bibliothèque .NET comme une autre ; appeler une API REST, désérialiser du JSON ou se connecter à un hub SignalR se fait exactement comme en C#. De même, **exposer une Web API par contrôleurs** — le scénario serveur le plus courant — fonctionne parfaitement en VB.NET. ✅

La nuance, fidèle à l'esprit de cette formation : le « web moderne » côté présentation (Blazor, Razor, MVC avec vues) et certains scénarios de création de services (Minimal APIs idiomatiques, gRPC et GraphQL) relèvent du **territoire C#**. Ce module vous apprend à exploiter pleinement ce qui marche, et à reconnaître honnêtement ce qu'il vaut mieux déléguer — sans jamais faire semblant. ⚠️ 🔗

---

## 🎯 Objectifs du module

À l'issue de ce module, vous saurez :

- situer **où se trouve VB.NET** dans l'écosystème des services .NET 10, et pourquoi ;
- **consommer des API REST** de façon robuste et résiliente (`HttpClient`, `IHttpClientFactory`, `System.Text.Json`, Polly) ;
- **exposer une Web API ASP.NET Core** en VB.NET par contrôleurs : routes, liaison de modèle, injection de dépendances, middleware, documentation OpenAPI et sécurité ;
- **identifier les limites web** de VB.NET et décider en connaissance de cause quand passer la main à C# ;
- mettre en place une **communication temps réel** réaliste (SignalR côté client et hub, WebSockets) ;
- comprendre **pourquoi gRPC et GraphQL** se délèguent, tout en sachant les consommer.

---

## 🧭 Le périmètre web de VB.NET, en un coup d'œil

Avant d'entrer dans le détail, voici la carte du terrain. Elle conditionne tout le module.

| Scénario | Statut en VB.NET | Approche recommandée |
|----------|------------------|----------------------|
| Consommer des API REST (`HttpClient`) | ✅ Pleinement pris en charge | Directement en VB.NET → § 8.1 |
| Exposer une Web API **par contrôleurs** | ✅ Recommandé | `ControllerBase` en VB.NET → § 8.2 |
| **Minimal APIs** | ⚠️ Possible mais peu idiomatique (pas de modèle de projet, pas de *top-level statements*) | Contrôleurs, ou C# → § 8.3 · [Annexe B](../annexes/frontiere-vbnet-csharp/README.md) |
| **Razor Pages / vues MVC / Blazor** | ⚠️ C# uniquement (*Razor génère du C#*) | Front séparé ou réécriture en C# → § 8.3 · [Annexe B](../annexes/frontiere-vbnet-csharp/README.md) |
| SignalR (client **et** hub) | ✅ Réaliste | En VB.NET → § 8.4 |
| WebSockets, Server-Sent Events (consommation) | ✅ Notions | `ClientWebSocket` en VB.NET → § 8.4 |
| **gRPC / GraphQL** (création de service) | ⚠️ Outillage orienté C# | Déléguer à C# → § 8.5 · [Annexe B](../annexes/frontiere-vbnet-csharp/README.md) |

> **À retenir :** la frontière n'est pas « web = interdit en VB ». Elle est plus fine. **Consommer** des services et **exposer une API par contrôleurs** sont des scénarios de premier rang en VB.NET ; ce sont surtout le **rendu de pages web** et la **création de services à fort outillage** qui basculent côté C#.

---

## 🆕 Ce que .NET 10 apporte à ce module

Plusieurs nouveautés .NET 10 bénéficient directement à VB.NET, sans rien changer à la façon d'écrire le code :

- **OpenAPI 3.1** désormais généré nativement (documentation d'API toujours à jour) ;
- **limitation de débit** (*rate limiting*) intégrée au pipeline ASP.NET Core ;
- prise en charge affinée de **`Problem Details`** pour des réponses d'erreur normalisées ;
- améliorations de résilience et de `System.Text.Json` côté consommation.

Ces apports sont détaillés là où ils comptent, principalement en § 8.1 (consommation) et § 8.2 (exposition).

---

## 📦 Prérequis

Ce module suppose acquis :

- la **programmation asynchrone** (module 4) — `Async`/`Await`, `Task`, annulation : indispensable, *tout* ici est asynchrone ;
- l'**accès aux données** (module 7) — c'est généralement ce qui se trouve derrière un service ;
- les **collections, LINQ et `System.Text.Json`** (module 2) ;
- les **bases de la POO** et des interfaces (module 3), utiles pour l'injection de dépendances.

---

## 📚 Plan du module

| § | Section | Repère |
|---|---------|--------|
| 8.1 | [Consommer des API REST](01-consommer-api-rest.md) — `HttpClient`, `IHttpClientFactory`, `System.Text.Json`, résilience avec Polly | Cœur consommation |
| 8.2 | [Exposer une Web API ASP.NET Core en VB.NET (par contrôleurs)](02-web-api-controllers.md) | ✅ Réaliste |
| 8.3 | [Limites du web en VB.NET](03-limites-web-vbnet.md) | ⚠️ Frontière |
| 8.4 | [Communication temps réel](04-temps-reel.md) — SignalR, WebSockets, SSE | Notions + réaliste |
| 8.5 | [gRPC et GraphQL](05-grpc-graphql.md) | ⚠️ À déléguer |

---

## 🔗 Lien avec la stratégie hybride

Ce module est une application directe de la **stratégie hybride VB.NET / C#** (module 10). Le réflexe à acquérir : garder en VB.NET la logique métier, la consommation de services et l'exposition d'API par contrôleurs — et **isoler dans une bibliothèque C#** ce qui exige Minimal APIs, gRPC, GraphQL ou un front web moderne, pour le **consommer** ensuite depuis VB.NET. Pour le détail de chaque sujet hors périmètre, l'annexe de référence reste **[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)**.

> 💡 La même logique de « consommation » se prolonge au **module 17.9**, où l'on intègre des services d'IA (`Microsoft.Extensions.AI`, Azure OpenAI) dans une application VB.NET : là encore, de simples bibliothèques .NET, pleinement dans le périmètre de VB.NET.

---

⬅️ [Partie 3 — Données et persistance](../07-acces-donnees/README.md) · ➡️ [8.1 — Consommer des API REST](01-consommer-api-rest.md)

⏭️ [Consommer des API REST (HttpClient, IHttpClientFactory, System.Text.Json, résilience avec Polly)](/08-services-web/01-consommer-api-rest.md)
