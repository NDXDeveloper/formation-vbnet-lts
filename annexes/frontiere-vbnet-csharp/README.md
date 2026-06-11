🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe B — Frontière VB.NET / C# : ce qu'on délègue à C# et pourquoi ⭐ 🔗

L'annexe de référence sur le **périmètre**. Pour chaque sujet situé *hors* du terrain de VB.NET en 2026, elle
donne deux choses : la **raison** (pourquoi ce n'est pas faisable — ou pas raisonnable — en VB), et la manière
de **continuer à en bénéficier depuis VB.NET** via une bibliothèque C#.

Ce document est le pendant « négatif » du reste de la formation : là où les autres modules disent *ce que VB
fait bien*, celui-ci délimite *ce qu'on confie à C#* — sans drame, et sans jamais perdre l'accès au résultat.

---

## B.0 — Le principe : une frontière d'**écriture**, pas d'**usage**

Rappel de la stratégie officielle Microsoft : VB.NET est un langage **stabilisé**, en *consumption-only*, qui
n'est **pas étendu à de nouveaux workloads** (voir modules 1.6 et 18). Concrètement, la frontière passe à un
endroit précis et libérateur :

> **VB.NET ne peut pas toujours *écrire* certaines constructions modernes, mais il peut presque toujours en
> *consommer* le résultat.**

C'est possible parce que VB et C# partagent **le même runtime (CLR), les mêmes types et le même format
d'assembly**. Une fois compilée en DLL, une brique C# est, pour VB, une bibliothèque .NET ordinaire : ses
classes, interfaces, méthodes `Async`, génériques et événements traversent la frontière de façon transparente.

Le **patron récurrent** de toute cette annexe est donc unique (et c'est exactement la stratégie hybride du
[module 10](../../10-hybride-vbnet-csharp/README.md) 🔗) :

1. **Isoler** la fonctionnalité « hors VB » dans une **bibliothèque C#**.
2. Lui faire **exposer une surface .NET propre** (interfaces, classes simples, méthodes `async`) — sans exiger
   de l'appelant qu'il manipule les constructions C#-only.
3. **Référencer et consommer** cette bibliothèque depuis VB.NET.

```vb
' Côté VB : on consomme une brique C# comme n'importe quelle bibliothèque .NET
Imports MaSolution.Coeur ' bibliothèque écrite en C#

Dim service As ITraitement = New ServiceRapide()       ' classe C#
Dim resultat = Await service.AnalyserAsync(donnees)     ' async traverse sans friction
```

> 💡 Comme vous écrirez (et lirez) du C# pour ces briques, gardez l'[Annexe A — Correspondance syntaxique](../correspondance-vbnet-csharp/README.md)
> sous la main, et appuyez-vous sur l'IA ([module 17](../../17-developpement-ia/README.md) 🤖) pour produire le
> C# que VB ne sait pas écrire.

---

## B.1 — Blazor (front-end web)

**Pourquoi hors VB.** Blazor repose sur **Razor** : les fichiers `.razor` sont compilés en classes **C#**, et
l'ensemble du modèle de composants, du *render tree* et de l'outillage est conçu pour C#. Il n'existe **aucun
chemin Razor → VB**. Le front-end web n'est tout simplement pas un terrain VB.

**Comment en bénéficier depuis VB.** Deux angles utiles :

- **Logique métier partagée.** Placez votre **domaine, vos services et votre accès aux données** dans une
  bibliothèque de classes **VB.NET**, et faites-la référencer par le projet Blazor (C#) via l'injection de
  dépendances. L'UI reste en C#, le cœur métier reste en VB.
- **Blazor Hybrid sur le bureau.** Une application **WinForms ou WPF en VB** (terrain de prédilection de VB)
  peut héberger le contrôle `BlazorWebView` et afficher des composants Razor. L'application hôte est en VB ;
  les composants eux-mêmes vivent dans une *Razor Class Library* en C#.

> Pour les vues web côté serveur (Razor Pages, vues MVC), même conclusion : **C# uniquement** (voir module 8.3).
> En VB, la voie web réaliste est la **Web API par contrôleurs** ([module 8.2](../../08-services-web/02-web-api-controllers.md) ✅).

---

## B.2 — .NET MAUI et WinUI 3 (UI « modernes » multiplateforme et Windows)

**Pourquoi hors VB.** Il n'existe **pas de modèle de projet MAUI en VB**. Le *code-behind* XAML, les
gestionnaires et les *handlers* sont en **C#**, et le *workload* MAUI cible C#. L'UI multiplateforme n'est pas
un scénario VB.

**Même verdict pour WinUI 3** (Windows App SDK), la pile d'UI Windows « moderne » au design Fluent : les
modèles de projet et toute la chaîne XAML ciblent **C# et C++ uniquement** — pas de chemin VB. Pour un
développeur VB de bureau, ce n'est pas un deuil : **WinForms ⭐ et WPF restent pleinement pris en charge et
investis sur .NET 10** (module 5.2 — mode sombre, formulaires asynchrones…) ; c'est la voie desktop VB.

**Comment en bénéficier depuis VB.** Le seul partage réaliste est **horizontal** : extraire les **modèles, la
validation, les services et l'accès aux données** dans une bibliothèque .NET **VB**, référencée par le projet
de tête **C# MAUI ou WinUI 3**. L'UI est en C#, le cœur réutilisable peut rester en VB.

> En pratique, un nouveau projet mobile (MAUI) ou Fluent (WinUI 3) se fait **entièrement en C#**. L'intérêt de
> la frontière ici est surtout de **réutiliser un existant VB** (règles métier, calculs LOB) sans le réécrire.

---

## B.3 — Minimal APIs

**Pourquoi (essentiellement) hors VB.** Les Minimal APIs sont pensées autour des *top-level statements*
(`Program.cs` sans `Main` explicite), que **VB ne possède pas**, et il n'existe **aucun modèle de projet VB**.
On *peut* appeler `WebApplication.CreateBuilder(...)` depuis un `Sub Main`, mais la syntaxe des points de
terminaison (lambdas, chaînage) y est nettement plus lourde — l'esprit « minimal » s'y perd.

**Comment en bénéficier depuis VB — ou plutôt, l'alternative recommandée.** Dans la quasi-totalité
des cas, **VB n'a pas besoin des Minimal APIs** : la **Web API par contrôleurs** est pleinement prise en charge et
recommandée ([module 8.2](../../08-services-web/02-web-api-controllers.md) ✅). Si un point de terminaison
*minimal* est réellement souhaité, écrivez l'hôte et les endpoints dans un **petit projet C#** et placez les
*handlers*/services dans des **bibliothèques VB**.

```vb
' Possible mais NON recommandé : Minimal API "à la main" en VB, via Sub Main
Imports Microsoft.AspNetCore.Builder

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)
        Dim app = builder.Build()
        app.MapGet("/", Function() "Bonjour")
        app.Run()
    End Sub
End Module
```

---

## B.4 — Native AOT

**Pourquoi hors VB en pratique.** La compilation **Native AOT** exige une compatibilité totale avec l'élagage
(*trimming*) et la compilation anticipée. Le runtime VB (`Microsoft.VisualBasic`) s'appuie sur des mécanismes
(infrastructure de *late binding*, aides basées sur la réflexion) **incompatibles** avec ces contraintes, et la
stratégie « *no new workloads* » de Microsoft ne fait pas de l'AOT une cible pour VB. **VB ne peut donc pas être
le point d'entrée d'une publication Native AOT.**

**Comment en bénéficier depuis VB — ou pourquoi c'est rarement nécessaire.**

- Si un composant précis a *réellement* besoin d'AOT (démarrage quasi instantané, empreinte minimale, outil CLI
  ou conteneur ultra-léger), **écrivez ce composant en C#** et publiez-le en AOT, séparément.
- Mais surtout : sur le **terrain naturel de VB** (applications de bureau WinForms/WPF, bibliothèques, LOB),
  l'AOT n'est généralement **pas un objectif**. Les modes de déploiement adaptés y sont le *framework-dependent*,
  le *self-contained* ou ReadyToRun ([module 15.1](../../15-deploiement-devops/01-packaging-desktop.md)). Ne
  « cherchez » pas l'AOT en VB.

---

## B.5 — gRPC et GraphQL

**Pourquoi orientés C#.**

- **gRPC** : la génération de code à partir des fichiers `.proto` (outillage *Grpc.Tools* / protobuf) **émet du
  C#**, et les modèles de service gRPC d'ASP.NET Core sont en C#. Il n'existe **pas de cible de génération VB**.
- **GraphQL** : la bibliothèque dominante de l'écosystème .NET (*HotChocolate*) est **C#-first**, exploite
  intensément les *source generators* et des fonctionnalités propres à C#, que ce soit en *code-first* ou
  *schema-first*.

**Comment en bénéficier depuis VB.** Distinguez **consommer** et **exposer** :

- **Consommer une API GraphQL** = HTTP + JSON. C'est **pleinement du périmètre VB** : `HttpClient` +
  `System.Text.Json` ([module 8.1](../../08-services-web/01-consommer-api-rest.md)).
- **Consommer un service gRPC** : faites **générer le client C#** dans une bibliothèque C# (à partir du `.proto`),
  puis **référencez-la depuis VB** — le client généré est une simple classe .NET.
- **Exposer** un service gRPC ou GraphQL : projet **C#** ; la logique des *resolvers*/services peut, elle,
  vivre dans des **bibliothèques VB** appelées par le projet C#.

---

## B.6 — Source generators (en tant qu'auteur)

**Pourquoi à écrire en C#.** Un *source generator* est un composant Roslyn (`IIncrementalGenerator`) : en
pratique, il s'**écrit en C#** (les API, l'écosystème et tous les exemples sont C#). Plus important pour le
quotidien : les **générateurs courants émettent du C# et ciblent des projets C#**, si bien qu'un **projet VB ne
peut pas les utiliser comme cible** — notamment :

- `[GeneratedRegex]` (regex compilées à la génération),
- le générateur source de `System.Text.Json` (`[JsonSerializable]`),
- `[LoggerMessage]` (journalisation hautes performances),
- `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]`) — **C# uniquement** (voir module 6.6).

**Comment en bénéficier depuis VB.**

- **Écrire un générateur** → projet **C#**.
- **Profiter des fonctionnalités générées** → isolez le code généré dans une **bibliothèque C#** qui expose un
  résultat « propre » à VB. Par exemple : effectuer la **sérialisation JSON hautes performances** dans une
  bibliothèque C# (avec le contexte source-généré) et n'exposer à VB que des méthodes `Serialize`/`Deserialize`.
- **Replis natifs côté VB** quand la perf n'est pas critique : utiliser `Regex` à l'exécution plutôt que
  `[GeneratedRegex]`, ou — pour MVVM en WPF — dériver directement des classes de base `ObservableObject` /
  `RelayCommand` **sans** générateurs (module 6.6 🔗).

---

## B.7 — Records, `init`, types `Span`-first

**Pourquoi non *déclarables* en VB (mais *consommables*).** Ce sont des constructions que VB sait utiliser une
fois compilées, mais pas exprimer dans sa propre syntaxe :

- **`record` / `record struct`** : se déclarent uniquement en **C#**. VB en consomme une instance
  (constructeur positionnel, égalité de valeur, `ToString`…), mais ne possède **ni le mot-clé `record`,
  ni l'expression `with`, ni la déconstruction** (on reconstruit l'objet, ou on lit ses propriétés).
- **Accesseurs `init`** : déclarés en C# uniquement. Côté consommation en revanche, bonne nouvelle
  vérifiée sur .NET 10 : VB **règle les propriétés `init` via son initialiseur `With { }`** — c'est
  précisément la capacité de consommation apportée par VB 16.9 (module 3.7) — et le compilateur les
  verrouille ensuite (toute réassignation est refusée, erreur BC37311).
- **Types `Span`-first (`Span(Of T)`, `ref struct`)** : VB **consomme** `Span(Of T)` dans des scénarios
  **limités** (usage local, passage en paramètre — module 14.4), mais ne peut **pas déclarer de `ref struct`** et
  reste contraint (pas d'usage en `Async`, itérateurs, lambdas). Les API **`Span`-first** orientées performance
  ne s'écrivent donc pas confortablement en VB.

**Comment en bénéficier depuis VB.** Déclarez ces types et API dans une **bibliothèque C#**, et consommez-les
depuis VB en respectant deux règles :

- pour les propriétés `init`, utiliser au choix le **constructeur** ou l'initialiseur **`With { … }`**
  (les deux fonctionnent — module 3.7) ; seule l'expression `with` de copie reste hors de portée :
  prévoir des méthodes de copie « `WithXxx` » côté C# si la copie non destructive est fréquente ;
- pour les API `Span`-first critiques, exposer une **surface VB-amicale** (surcharges acceptant des tableaux,
  méthodes renvoyant un résultat déjà matérialisé) plutôt que d'imposer la manipulation de `Span`.

---

## B.8 — Microservices / Dapr / Kubernetes

**Pourquoi « majoritairement C# ».** Ici, la frontière tient moins à une **limite de langage**
qu'à la **gravité de l'écosystème**. Un microservice peut techniquement être une **Web API VB par contrôleurs**
conteneurisée (c'est pris en charge — module 15.4). Mais tout l'outillage *cloud-native* autour est **C#-first** :

- **pas de modèle de projet *Worker*** en VB (module 4.8 — à câbler à la main via le Generic Host) ;
- **Azure Functions sans support VB officiel** → C# (module 15.5) ;
- les SDK **Dapr**, l'orchestration **.NET Aspire** et la quasi-totalité des exemples/templates supposent C#.

**Comment en bénéficier depuis VB — la posture pragmatique.**

- **Kubernetes est agnostique au langage** : il exécute des **conteneurs**, et un conteneur peut très bien
  embarquer une application **VB** (Web API par contrôleurs). De ce côté-là, VB passe la frontière sans souci.
- En revanche, **Workers de fond, Functions, services fortement Dapr et orchestration Aspire** gagnent à être
  écrits en **C#**.
- **Architecture hybride réaliste** : VB pour les **applications LOB/bureau** et les **bibliothèques de domaine**
  réutilisables ; C# pour la **couche cloud-native / orchestration**. Les deux partageant le même CLR, les
  bibliothèques VB sont consommées sans friction par les services C#.

---

## B.9 — Synthèse : statut et approche recommandée

| Sujet | Statut en VB.NET | Approche recommandée |
|-------|------------------|----------------------|
| **Blazor** (front web) | Non | UI en C# (Razor) ; **logique métier en bibliothèque VB** injectée. Hôte Blazor Hybrid possible en WinForms/WPF VB |
| **.NET MAUI** | Non | UI en C# ; **réutiliser le domaine VB** via bibliothèque partagée |
| **WinUI 3** (Windows App SDK) | Non (C#/C++ uniquement) | Le desktop VB reste **WinForms ⭐ / WPF** (pleinement supportés .NET 10) ; sinon UI C#, domaine VB |
| **Minimal APIs** | Possible mais déconseillé | Préférer la **Web API par contrôleurs en VB** ✅ ; sinon hôte C# |
| **Native AOT** | Non (en pratique) | Composant à publier en AOT → **C#** ; inutile sur le terrain bureau de VB |
| **gRPC** | Consommation seulement | **Client généré en C#**, référencé depuis VB ; service exposé en C# |
| **GraphQL** | Consommation = oui (HTTP/JSON ✅) | **Consommer** en VB ; **exposer** en C# |
| **Source generators** (auteur) | Non | Générateur en **C#** ; isoler le code généré, exposer un résultat propre à VB |
| **Records / `init` / `Span`-first** | Consommables, non déclarables | **Déclarer en C#** ; consommer en VB (`init` via constructeur **ou** `With { }`, surface VB-amicale pour `Span`) |
| **Microservices / Dapr / Aspire** | Web API conteneurisée = oui ; reste = C# | **VB pour LOB/domaine**, **C# pour le cloud-native** ; K8s exécute le conteneur VB sans souci |

---

### Voir aussi

- Module 10 — [Architecture hybride VB.NET / C#](../../10-hybride-vbnet-csharp/README.md) ⭐ 🔗 (la mise en œuvre concrète de tout ce qui précède)
- Module 1.6 — [VB.NET en 2026 : positionnement honnête](../../01-introduction-vbnet/06-positionnement-2026.md) (la stratégie « consumption-only »)
- Module 8 — [Consommer et exposer des services](../../08-services-web/README.md) (la voie web réaliste : contrôleurs ✅)
- Module 17 — [Développer en VB.NET avec l'IA](../../17-developpement-ia/README.md) 🤖 (produire les briques C# que VB ne sait pas écrire)
- [Annexe A — Correspondance syntaxique VB.NET ↔ C#](../correspondance-vbnet-csharp/README.md) (lire et transposer le C# de ces briques)

---

**Juin 2026** · .NET 10 LTS · VB.NET 16.9 (stabilisé) · C# 14

⏭️ [Bonnes pratiques de codage VB.NET](/annexes/bonnes-pratiques/README.md)
