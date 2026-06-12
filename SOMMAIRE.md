# 📚 Sommaire — Formation Visual Basic .NET (.NET 10 LTS)
**Apprendre VB.NET là où il est réellement pertinent — du débutant au professionnel**  
**Juin 2026** · .NET 10 LTS · Visual Studio 2026

---

## 🧭 À lire avant de commencer — la philosophie de cette formation

Visual Basic .NET est, depuis 2020, un **langage stabilisé** : Microsoft le maintient
(sécurité, compatibilité runtime, expérience Visual Studio) mais **n'ajoute plus de
nouvelles fonctionnalités de langage**. La stratégie officielle est claire :

> *« Visual Basic adoptera généralement une approche de consommation seule et évitera
> toute nouvelle syntaxe. Visual Basic ne sera pas étendu à de nouveaux workloads. Nous
> continuerons à investir dans l'expérience Visual Studio et l'interopérabilité avec C#,
> en particulier dans les scénarios cœur de Visual Basic comme **Windows Forms** et les
> **bibliothèques**. »*
> — [Stratégie du langage Visual Basic, Microsoft Learn](https://learn.microsoft.com/dotnet/visual-basic/getting-started/strategy)

Cette formation en tient compte. Elle **n'enseigne pas en VB.NET ce qui se code mieux
en C# aujourd'hui**. À la place :

- ✅ Elle approfondit ce que VB.NET fait **réellement bien** : applications de bureau
  (Windows Forms, WPF), bibliothèques, accès aux données, interopérabilité COM/Office,
  maintenance et migration de legacy, Web API par contrôleurs.
- 🔗 Elle fait de l'**architecture hybride VB.NET / C#** une compétence centrale : déléguer
  les briques modernes (perf, features récentes) à des bibliothèques C# **consommées**
  depuis VB.NET.
- ⚠️ Elle signale honnêtement les sujets **hors périmètre VB** (Blazor, MAUI, Minimal APIs,
  Native AOT, gRPC, microservices…) sans prétendre les enseigner en VB — voir
  **[Annexe B — Frontière VB.NET / C#](annexes/frontiere-vbnet-csharp/README.md)**.
- 🤖 Elle intègre le **développement assisté par IA**, indispensable en VB.NET (les modèles
  sont surtout entraînés sur du C#).

> **Repère de périmètre :** sur la grosse vingtaine de modèles de projet du SDK .NET 10, **douze
> seulement** existent en VB.NET — cinq familles : Console, bibliothèque de classes, Windows
> Forms, WPF et projets de test. C'est exactement le terrain que couvre cette formation.

---

## 🎯 Parcours de formation

| Parcours | Modules | Durée estimée |
|----------|---------|---------------|
| **Débutant** | 1-2, 5, 7, 12 | 5-7 jours |
| **Développeur Desktop** (cœur VB) ⭐ | 1-7, 9, 12-13, 15-16 | 10-12 jours |
| **Développeur Données / LOB** | 1-3, 7-8, 12-13, 16 | 8-10 jours |
| **Maintenance & Migration Legacy** | 1-3, 7, 9-11, 17 | 7-9 jours |
| **Web API / Services** (réaliste) | 1-4, 7-8, 12-13, 15-16 | 9-11 jours |
| **Architecte / Hybride VB-C#** 🔗 | 1-4, 9-11, 14-16, 18 + Annexe B | 12-15 jours |
| **IA-First** 🤖 | 1-2, 7, 17, 18 | 3-4 jours |
| **Formation complète** | 1-18 + annexes | 22-28 jours |

---

## **Partie 1 — Comprendre VB.NET en 2026 (cadrage & langage)**

### 1. [Introduction : VB.NET et l'écosystème .NET 10](01-introduction-vbnet/README.md)
- 1.1 [Qu'est-ce que VB.NET et à quoi il sert réellement en 2026](01-introduction-vbnet/01-quest-ce-que-vbnet.md)
- 1.2 [De Visual Basic 6 à VB.NET (histoire et héritage legacy)](01-introduction-vbnet/02-histoire-evolution.md)
- 1.3 [L'écosystème .NET](01-introduction-vbnet/03-ecosysteme-dotnet.md)
    - .NET Framework (Windows, legacy) vs .NET moderne (.NET 10)
    - .NET 10 LTS : apports clés, support jusqu'au 14 novembre 2028
    - Runtime, SDK, NuGet, structure d'une solution
- 1.4 [Installation et outils](01-introduction-vbnet/04-installation-outils.md)
    - Visual Studio 2026 (UX Fluent, IDE « AI-native ») 🆕
    - VS Code et .NET CLI — ⚠️ en VS Code, pas de C# Dev Kit pour VB.NET (édition de base, sans IntelliSense riche ni débogage intégré)
- 1.5 [Premier projet pas à pas (Console et Windows Forms)](01-introduction-vbnet/05-premier-projet.md)
- 1.6 [**VB.NET en 2026 : positionnement honnête**](01-introduction-vbnet/06-positionnement-2026.md) ⭐ 🔄
    - La stratégie officielle Microsoft : langage « stabilisé », *consumption-only*, aucune nouvelle syntaxe
    - Langage figé à **VB 16.9** : ce que cela implique concrètement
    - Scénarios cœur (Windows Forms, bibliothèques) vs workloads non supportés
    - Lecture du périmètre réel : une poignée de modèles de projet VB (douze, en cinq familles)
    - [VB.NET vs C# : quand choisir quoi (grille de décision)](01-introduction-vbnet/06.1-vbnet-vs-csharp.md)
    - [La stratégie hybride VB.NET / C# en une page](01-introduction-vbnet/06.2-strategie-hybride.md) 🔗
    - [L'ère de l'IA : pourquoi c'est décisif pour VB.NET (le biais C# des modèles)](01-introduction-vbnet/06.3-ere-ia.md) 🤖

### 2. [Fondamentaux du langage](02-fondamentaux-langage/README.md)
- 2.1 [Structure d'un programme ; `Option Strict` / `Explicit` / `Infer` / `Compare`](02-fondamentaux-langage/01-structure-options.md)
- 2.2 [Types de données et variables](02-fondamentaux-langage/02-types-variables.md)
    - Types valeur vs référence, types primitifs
    - Types nullables de valeur — `Nullable(Of T)` / `T?` (≠ *nullable reference types* de C#) ⚠️
    - Inférence de type, constantes et énumérations
- 2.3 [Opérateurs et expressions (`&`, `Mod`, `Like`, `Is`/`IsNot`, `AndAlso`/`OrElse`)](02-fondamentaux-langage/03-operateurs.md)
- 2.4 [Structures conditionnelles](02-fondamentaux-langage/04-conditions.md)
    - `If…Then…Else`, opérateur `If()` ternaire
    - `Select Case`
    - `TypeOf…Is` et tests de type (filtrage de motifs VB — plus limité qu'en C#) ⚠️
- 2.5 [Boucles et itérations (`For…Next`, `For Each`, `Do…Loop`, `While`)](02-fondamentaux-langage/05-boucles.md)
- 2.6 [Chaînes et manipulation de texte (`String`, `StringBuilder`, interpolation `$"…"`)](02-fondamentaux-langage/06-chaines.md)
- 2.7 [Dates, nombres, formatage et culture](02-fondamentaux-langage/07-dates-nombres-culture.md)
- 2.8 [Tableaux et collections](02-fondamentaux-langage/08-tableaux-collections.md)
    - Tableaux uni- et multidimensionnels
    - Génériques : `List(Of T)`, `Dictionary(Of TKey, TValue)`
    - Collections spécialisées (`ObservableCollection`, collections concurrentes)
- 2.9 [LINQ — un point fort de VB.NET](02-fondamentaux-langage/09-linq.md) ⭐
    - LINQ to Objects ; syntaxe requête (`From…Where…Select`) vs méthodes d'extension
    - LINQ to Entities (→ module 7)
- 2.10 [Génériques avancés (contraintes, méthodes génériques, variance)](02-fondamentaux-langage/10-generiques-avances.md)
- 2.11 [Portée, visibilité et modificateurs d'accès (`Public`/`Private`/`Protected`/`Friend`)](02-fondamentaux-langage/11-portee-visibilite.md)
- 2.12 [L'espace `My` — un raccourci propre à VB.NET](02-fondamentaux-langage/12-espace-my.md) ⭐ ⚠️
    - `My.Application`, `My.Computer`, `My.Settings`, `My.Resources`, `My.User`
    - ⚠️ Support partiel sur .NET moderne : correct en Windows Forms, limité/absent en WPF, membres web (`My.Request`, `My.WebServices`) supprimés
    - Quand l'utiliser (productivité) vs ses limites (couplage, testabilité)

### 3. [Programmation orientée objet](03-poo/README.md)
- 3.1 [Classes et objets (propriétés auto et complètes, constructeurs, `Me`)](03-poo/01-classes-objets.md)
- 3.2 [Structures (`Structure`) et tuples](03-poo/02-structures-tuples.md)
- 3.3 [Héritage et polymorphisme](03-poo/03-heritage-polymorphisme.md)
    - `Inherits`, `Overridable`/`Overrides`, `MustInherit`/`MustOverride`, `NotInheritable`, `Shadows`
- 3.4 [Interfaces (`Implements`, interfaces génériques et multiples)](03-poo/04-interfaces.md)
- 3.5 [Modules, espaces de noms et classes partielles](03-poo/05-modules-namespaces.md)
- 3.6 [**Événements et délégués — un point fort idiomatique de VB.NET**](03-poo/06-evenements-delegues.md) ⭐
    - `Event` / `RaiseEvent` / `Handles` / `WithEvents` (l'idiome VB)
    - `EventHandler(Of T)`, `AddHandler`/`RemoveHandler`
    - Délégués, lambdas (`Function`/`Sub`), expressions
    - Pattern Observer idiomatique
- 3.7 [Types immuables et records](03-poo/07-immuabilite-records.md) ⚠️ 🔗
    - Consommer des *records* C# depuis VB.NET (VB ne **déclare** pas de `record`)
    - Approcher l'immuabilité en VB (`ReadOnly`, propriétés en lecture seule)
- 3.8 [Réflexion et attributs (`System.Reflection`, attributs personnalisés)](03-poo/08-reflexion-attributs.md)

### 4. [Programmation asynchrone et parallèle](04-async/README.md)
- 4.1 [Pourquoi l'asynchronie (UI réactive, opérations d'E/S)](04-async/01-pourquoi-async.md)
- 4.2 [`Async`/`Await` (`Task`, `Task(Of T)`)](04-async/02-async-await.md)
- 4.3 [Gestion des exceptions asynchrones](04-async/03-exceptions-async.md)
- 4.4 [Annulation et timeout (`CancellationToken`)](04-async/04-annulation-timeout.md)
- 4.5 [Parallélisme pragmatique (`Parallel.For`/`ForEach`, PLINQ)](04-async/05-parallelisme.md)
- 4.6 [Consommer `IAsyncEnumerable` / flux asynchrones ; `ValueTask` (notions)](04-async/06-async-streams.md)
- 4.7 [Synchronisation et thread-safety (`SyncLock`, `Interlocked`, `SemaphoreSlim`)](04-async/07-synchronisation.md)
- 4.8 [Services en arrière-plan : Generic Host et `BackgroundService`](04-async/08-background-services.md)
    - `Microsoft.Extensions.Hosting`, `IHostedService` / `BackgroundService`
    - ⚠️ Pas de modèle de projet « Worker » en VB.NET → à câbler à la main via le Generic Host
    - Services Windows, tâches planifiées et traitements de fond

---

## **Partie 2 — Applications de bureau (le cœur de VB.NET)**

### 5. [Windows Forms — le scénario phare](05-windows-forms/README.md) ⭐ 🔄
- 5.1 [Introduction, architecture et le Concepteur (Designer)](05-windows-forms/01-introduction-designer.md)
- 5.2 [Windows Forms sur .NET 10 (modernisation : mode sombre intégré, formulaires async `ShowAsync`/`ShowDialogAsync`, presse-papiers sécurisé)](05-windows-forms/02-winforms-net10.md) 🆕
- 5.3 [Contrôles fondamentaux (`Form`, `Button`, `TextBox`, conteneurs, boîtes de dialogue)](05-windows-forms/03-controles-fondamentaux.md)
- 5.4 [Contrôles avancés (`DataGridView`, `TreeView`/`ListView`, `MenuStrip`/`ToolStrip`/`StatusStrip`)](05-windows-forms/04-controles-avances.md)
- 5.5 [Contrôles personnalisés et `UserControl`](05-windows-forms/05-controles-personnalises.md)
- 5.6 [Gestion des événements (souris, clavier, cycle de vie du formulaire)](05-windows-forms/06-evenements.md)
- 5.7 [Validation (`ErrorProvider`, `DataAnnotations`, règles personnalisées)](05-windows-forms/07-validation.md)
- 5.8 [Liaison de données WinForms (`BindingSource`, `BindingList`, liaison à une BDD)](05-windows-forms/08-data-binding.md)
- 5.9 [Applications MDI et multi-formulaires](05-windows-forms/09-mdi.md)
- 5.10 [Préférences et paramètres utilisateur (`My.Settings`)](05-windows-forms/10-preferences.md)
- 5.11 [Internationalisation (i18n/l10n, ressources `.resx`)](05-windows-forms/11-internationalisation.md)
- 5.12 [Nouveautés Windows Forms .NET 10 (presse-papiers JSON, `Form.FormScreenCaptureMode` anti-capture, éditeurs de designer portés depuis .NET Framework)](05-windows-forms/12-nouveautes-net10.md) 🆕
- 5.13 Déploiement → voir [module 15](15-deploiement-devops/README.md)

### 6. [WPF (Windows Presentation Foundation)](06-wpf/README.md)
- 6.1 [Introduction ; WPF vs Windows Forms (lequel choisir)](06-wpf/01-introduction-wpf-vs-winforms.md)
- 6.2 [XAML fondamentaux et systèmes de layout (`Grid`, `StackPanel`, `DockPanel`)](06-wpf/02-xaml-layout.md)
- 6.3 [Contrôles et contrôles de données (`DataGrid`, `ListView`)](06-wpf/03-controles.md)
- 6.4 [Liaison de données (`OneWay`/`TwoWay`, `INotifyPropertyChanged`, `ObservableCollection`, convertisseurs, validation)](06-wpf/04-data-binding.md)
- 6.5 [Styles, ressources, templates et triggers](06-wpf/05-styles-templates.md)
- 6.6 [Architecture MVVM](06-wpf/06-mvvm.md)
    - Principes ; `ICommand` / `RelayCommand` (implémentation manuelle en VB)
    - `CommunityToolkit.Mvvm` — les *source generators* (`[ObservableProperty]`, `[RelayCommand]`) sont **C# uniquement** ; en VB, utiliser directement les classes de base `ObservableObject` / `RelayCommand` (sans générateurs) 🔗
- 6.7 [Animations et multimédia (notions)](06-wpf/07-animations-multimedia.md)
- 6.8 [Thèmes et Fluent Design (.NET 10)](06-wpf/08-fluent-design-net10.md) 🆕
- 6.9 [Performance WPF (virtualisation, binding, rendu)](06-wpf/09-performance.md)

---

## **Partie 3 — Données et persistance**

### 7. [Accès aux données](07-acces-donnees/README.md) ⭐ 🔄
- 7.1 [ADO.NET (connexions, commandes paramétrées, `DataReader`, `DataSet`/`DataTable`, transactions)](07-acces-donnees/01-adonet.md)
- 7.2 [Entity Framework Core 10](07-acces-donnees/02-ef-core-10.md) 🆕
    - Installation, `DbContext`/`DbSet`
    - Code-First et migrations
    - Database-First / *scaffolding* (utile pour le legacy)
    - LINQ to Entities, relations, chargement (lazy / eager / explicit)
    - Requêtes asynchrones, pagination, tri, filtrage dynamique, concurrence
    - Nouveautés EF Core 10 (`LeftJoin` natif, *named query filters*, colonnes JSON, types complexes) 🆕
- 7.3 [Fournisseurs de bases de données](07-acces-donnees/03-fournisseurs.md)
    - SQL Server / LocalDB · SQLite (applications locales) · MySQL / MariaDB · PostgreSQL (Npgsql)
- 7.4 [Autres stockages (notions) : NoSQL (MongoDB, Cosmos DB), Redis (cache)](07-acces-donnees/04-autres-stockages.md)
- 7.5 [Sérialisation (`System.Text.Json`, XML, CSV)](07-acces-donnees/05-serialisation.md)
- 7.6 [Fichiers, flux (`Stream`) et E/S](07-acces-donnees/06-fichiers-io.md)
    - `System.IO` : `File`, `Directory`, `Path` ; lecture/écriture synchrone et asynchrone
    - Flux : `Stream`, `FileStream`, `MemoryStream` ; `StreamReader`/`StreamWriter`, `BinaryReader`/`BinaryWriter`
    - Flux bufferisés, de compression (`GZipStream`, `BrotliStream`) et de chiffrement (`CryptoStream`)

---

## **Partie 4 — Services, web et temps réel (périmètre réaliste)**

### 8. [Consommer et exposer des services](08-services-web/README.md)
- 8.1 [Consommer des API REST (`HttpClient`, `IHttpClientFactory`, `System.Text.Json`, résilience avec Polly)](08-services-web/01-consommer-api-rest.md)
- 8.2 [Exposer une Web API ASP.NET Core en VB.NET (par contrôleurs)](08-services-web/02-web-api-controllers.md) ✅
    - Contrôleurs (`ControllerBase`), routes, actions, liaison de modèle
    - Injection de dépendances, middleware, pipeline de requêtes
    - Documentation OpenAPI / Swagger (OpenAPI 3.1 dans .NET 10) 🆕
    - Authentification (JWT, OAuth 2.0 / OpenID Connect) — consommation
    - `Problem Details`, versioning d'API, rate limiting intégré
- 8.3 [⚠️ Limites du web en VB.NET](08-services-web/03-limites-web-vbnet.md) → [Annexe B](annexes/frontiere-vbnet-csharp/README.md)
    - **Minimal APIs** : possibles mais **sans modèle de projet** et syntaxe contrainte (pas de *top-level statements*) → préférer les contrôleurs, ou C#
    - **Razor Pages / vues MVC / Blazor** : **C# uniquement**
- 8.4 [Communication temps réel](08-services-web/04-temps-reel.md)
    - SignalR (client VB.NET ; hub réaliste)
    - WebSockets (`ClientWebSocket`), Server-Sent Events (consommation) — notions
- 8.5 [gRPC et GraphQL ⚠️ — outillage orienté C#, à déléguer](08-services-web/05-grpc-graphql.md) → [Annexe B](annexes/frontiere-vbnet-csharp/README.md)

---

## **Partie 5 — Interopérabilité, hybride et migration (les forces de VB.NET)**

### 9. [Interopérabilité](09-interoperabilite/README.md) 🔗 ⭐
- 9.1 [P/Invoke (appel d'API natives Windows, marshaling, callbacks)](09-interoperabilite/01-pinvoke.md)
- 9.2 [COM et automation Office (Excel, Word, Outlook) — force historique de VB](09-interoperabilite/02-com-office.md) ⭐
    - Utilisation de composants COM, RCW / CCW
    - *Early binding* vs *late binding* (`Option Strict Off`)
- 9.3 [Interopérabilité entre langages .NET (VB ↔ C#, F#)](09-interoperabilite/03-interop-langages.md)
- 9.4 [WebView2 (intégrer du web moderne dans une application de bureau)](09-interoperabilite/04-webview2.md)

### 10. [Architecture hybride VB.NET / C# — la stratégie 2026](10-hybride-vbnet-csharp/README.md) 🔗 ⭐ 🆕
- 10.1 [Pourquoi l'hybride : la réponse pragmatique au gel du langage](10-hybride-vbnet-csharp/01-pourquoi-hybride.md)
- 10.2 [Quand déléguer à C# (perf/`Span`, records, source generators, Minimal APIs, Native AOT, Blazor/MAUI)](10-hybride-vbnet-csharp/02-quand-deleguer.md)
- 10.3 [Isoler les fonctionnalités avancées dans des bibliothèques C#](10-hybride-vbnet-csharp/03-isoler-en-csharp.md)
- 10.4 [Consommer ces bibliothèques de façon transparente depuis VB.NET](10-hybride-vbnet-csharp/04-consommer-depuis-vbnet.md)
- 10.5 [Atelier : cœur en C# (performance / fonctionnalités), UI et métier en VB.NET](10-hybride-vbnet-csharp/05-atelier-core-csharp-ui-vbnet.md)
- 10.6 [Gérer une solution mixte (NuGet, projets, build, tests)](10-hybride-vbnet-csharp/06-solution-mixte.md)

### 11. [Migration et maintenance du code legacy](11-migration-legacy/README.md) ⭐
- 11.1 [Évaluer l'existant ; stratégies (incrémentale vs *big-bang*)](11-migration-legacy/01-evaluer-strategies.md)
- 11.2 [VB6 → VB.NET (outils de conversion, pièges, APIs obsolètes)](11-migration-legacy/02-vb6-vers-vbnet.md)
- 11.3 [.NET Framework 4.x → .NET 10 (analyse de dépendances, APIs retirées, `appsettings`, breaking changes .NET 10)](11-migration-legacy/03-framework-vers-net10.md) 🆕
- 11.4 [ASP.NET **Web Forms** (legacy VB) : maintenance et stratégie de sortie](11-migration-legacy/04-web-forms-legacy.md) ⚠️
    - ⚠️ **Aucun chemin de migration vers .NET moderne** : Web Forms n'existe pas hors .NET Framework
    - Options : rester sur .NET Framework (supporté avec Windows) ou réécrire (MVC/Blazor en C#, ou Web API VB + front séparé)
- 11.5 [Coexistence .NET Framework / .NET moderne, `.NET Standard`](11-migration-legacy/05-coexistence.md)
- 11.6 [Moderniser (async, LINQ, EF Core, injection de dépendances, testabilité)](11-migration-legacy/06-moderniser.md)
- 11.7 [Gestion des risques (sauvegarde, tests de non-régression, *rollback*)](11-migration-legacy/07-gestion-risques.md)
- 11.8 Migration assistée par IA → voir [module 17](17-developpement-ia/README.md) 🤖

---

## **Partie 6 — Qualité, performance et exploitation**

### 12. [Exceptions, débogage et journalisation](12-exceptions-debogage/README.md)
- 12.1 [`Try`/`Catch`/`Finally`, filtres `When`, hiérarchie, exceptions personnalisées](12-exceptions-debogage/01-exceptions.md)
- 12.2 [Débogage (points d'arrêt, espions, Hot Reload, débogage asynchrone) ; outils de VS 2026](12-exceptions-debogage/02-debogage.md) 🆕
- 12.3 [Journalisation (`Microsoft.Extensions.Logging`, Serilog, *structured logging*)](12-exceptions-debogage/03-journalisation.md)
- 12.4 [Observabilité (notions : OpenTelemetry, *health checks*, métriques)](12-exceptions-debogage/04-observabilite.md)

### 13. [Tests et qualité du code](13-tests-qualite/README.md)
- 13.1 [Tests unitaires (xUnit, NUnit, MSTest ; `Microsoft.Testing.Platform` dans .NET 10)](13-tests-qualite/01-tests-unitaires.md) 🆕
- 13.2 [*Mocking* (Moq, NSubstitute) et TDD](13-tests-qualite/02-mocking-tdd.md)
- 13.3 [Tests d'intégration (`WebApplicationFactory`, Testcontainers, tests avec base de données)](13-tests-qualite/03-tests-integration.md)
- 13.4 [Analyse statique (analyseurs Roslyn, SonarQube, StyleCop)](13-tests-qualite/04-analyse-statique.md)
- 13.5 [Couverture de code ; génération de tests par IA](13-tests-qualite/05-couverture-tests-ia.md) 🤖
- 13.6 [BenchmarkDotNet (notions)](13-tests-qualite/06-benchmarkdotnet.md)

### 14. [Performance et gestion de la mémoire](14-performance/README.md)
- 14.1 [Profilage (VS Profiler, `dotnet-counters`/`trace`/`dump`/`gcdump`)](14-performance/01-profilage.md)
- 14.2 [Types valeur vs référence, allocations](14-performance/02-types-allocations.md)
- 14.3 [Garbage Collector (générations, modes, tuning, `IDisposable`/`Using`, finaliseurs)](14-performance/03-gc.md)
- 14.4 [`Span(Of T)` / `Memory(Of T)` (consommation), pooling (`ArrayPool`, `ObjectPool`)](14-performance/04-span-pooling.md)
- 14.5 [Bonnes pratiques de performance en VB.NET](14-performance/05-bonnes-pratiques.md)
- 14.6 [Ce que .NET 10 apporte gratuitement (JIT/PGO/SIMD/dévirtualisation, sans changer le code)](14-performance/06-apports-net10.md) 🆕

### 15. [Déploiement et DevOps](15-deploiement-devops/README.md)
- 15.1 [Packaging desktop : ClickOnce, MSIX, *framework-dependent* vs *self-contained*, fichier unique](15-deploiement-devops/01-packaging-desktop.md)
- 15.2 [Distribution via le Microsoft Store](15-deploiement-devops/02-microsoft-store.md)
- 15.3 [CI/CD (GitHub Actions, Azure DevOps, GitLab CI)](15-deploiement-devops/03-cicd.md)
- 15.4 [Conteneurisation (Docker pour Web API / Worker ; images conteneur .NET 10)](15-deploiement-devops/04-docker.md) 🆕
- 15.5 [Cloud (essentiels, consommés via SDK) : Azure App Service, Blob Storage, Key Vault ; ⚠️ Azure Functions sans support VB officiel → C#](15-deploiement-devops/05-cloud-essentiels.md)
- 15.6 [Outils de build .NET 10 (élagage NuGet, améliorations MSBuild)](15-deploiement-devops/06-outils-build-net10.md) 🆕

### 16. [Sécurité des applications](16-securite/README.md)
- 16.1 [Authentification et autorisation (OAuth 2.0 / OIDC, JWT, Microsoft Entra ID)](16-securite/01-auth.md)
- 16.2 [Cryptographie (hachage, chiffrement, gestion des secrets, Key Vault)](16-securite/02-cryptographie.md)
- 16.3 [OWASP Top 10 pour .NET (injection SQL et paramétrage, validation, encodage de sortie)](16-securite/03-owasp.md)
- 16.4 [Dépendances et vulnérabilités (scan NuGet, SAST/DAST en CI/CD)](16-securite/04-dependances-vulnerabilites.md)
- 16.5 [Checklist de sécurité VB.NET](16-securite/05-checklist.md)

---

## **Partie 7 — IA et avenir**

### 17. [Développer en VB.NET avec l'IA (l'ère Copilot)](17-developpement-ia/README.md) 🤖 🆕 ⭐
- 17.1 [Coder en 2026 avec l'IA : pourquoi c'est crucial pour VB.NET (le biais C# des modèles)](17-developpement-ia/01-pourquoi-ia-vbnet.md)
- 17.2 [Prompting efficace pour obtenir du code VB.NET](17-developpement-ia/02-prompting-vbnet.md)
    - Toujours préciser « Visual Basic .NET / VB.NET » et la version .NET cible ; éviter l'ambiguïté avec VB6
    - Convertir et corriger le code C# généré vers VB.NET
    - Modèles de prompts prêts à l'emploi
- 17.3 [Migrer du legacy avec l'IA (VB6, .NET Framework)](17-developpement-ia/03-migration-legacy-ia.md)
- 17.4 [Générer des tests, mocks et documentation XML](17-developpement-ia/04-generer-tests-doc.md)
- 17.5 [Déboguer et optimiser avec l'IA (expliquer des erreurs, analyser des *stack traces*)](17-developpement-ia/05-debugger-optimiser.md)
- 17.6 [Workflow IA-first : Copilot dans Visual Studio 2026, VS Code, agents](17-developpement-ia/06-workflow-ia-first.md)
- 17.7 [Limites et pièges (hallucinations de syntaxe C# en VB, validation systématique)](17-developpement-ia/07-limites-pieges.md)
- 17.8 [Cas concrets (migration VB6→VB.NET, API REST, WPF MVVM)](17-developpement-ia/08-cas-concrets.md)
- 17.9 [Intégrer l'IA dans vos applications VB.NET (consommation d'API)](17-developpement-ia/09-consommer-ia.md) ✅
    - `Microsoft.Extensions.AI` (abstraction) ; SDK Azure OpenAI / OpenAI
    - Chat, embeddings, *function calling*, RAG basique
    - De simples bibliothèques .NET → pleinement dans le périmètre « consommation » de VB.NET

### 18. [Stratégie, feuille de route et ressources](18-strategie-roadmap/README.md)
- 18.1 [La stratégie officielle Microsoft pour VB.NET (rappel et lecture)](18-strategie-roadmap/01-strategie-microsoft.md)
- 18.2 [Support long terme et feuille de route .NET (cycles LTS / STS)](18-strategie-roadmap/02-roadmap-dotnet.md)
- 18.3 [Cas d'usage optimaux pour VB.NET : quand rester, quand migrer vers C#](18-strategie-roadmap/03-cas-usage-quand-migrer.md)
- 18.4 [Migrer vers C# si nécessaire (outils de conversion, coût/bénéfice, code hybride)](18-strategie-roadmap/04-migrer-vers-csharp.md)
- 18.5 [Communauté, documentation, livres, formation continue, outils tiers](18-strategie-roadmap/05-ressources-communaute.md)

---

## **Annexes**

### A. [Correspondance syntaxique VB.NET ↔ C# (aide-mémoire)](annexes/correspondance-vbnet-csharp/README.md)
Tableaux de conversion : mots-clés, types, structures, événements, LINQ, async — indispensable pour
lire le C# (omniprésent dans la doc et les réponses d'IA) et le transposer en VB.NET.

### B. [**Frontière VB.NET / C# : ce qu'on délègue à C# et pourquoi**](annexes/frontiere-vbnet-csharp/README.md) ⭐ 🔗
L'annexe de référence sur le périmètre. Pour chaque sujet *hors VB* : la raison, et comment le
**consommer depuis VB.NET** via une bibliothèque C#.
- B.1 Blazor (web front-end) — *Razor génère du C#*
- B.2 .NET MAUI et WinUI 3 (UI « modernes » multiplateforme et Windows) — *pas de modèle VB, code UI en C#*
- B.3 Minimal APIs — *sans modèle, syntaxe contrainte*
- B.4 Native AOT — *non pris en charge en pratique pour VB*
- B.5 gRPC et GraphQL — *outillage orienté C#*
- B.6 Source generators (auteur) — *à écrire en C#*
- B.7 Records, `init`, types `Span`-first — *consommables, non déclarables en VB*
- B.8 Microservices / Dapr / Kubernetes — *écosystème majoritairement C#*

### C. [Bonnes pratiques de codage VB.NET](annexes/bonnes-pratiques/README.md)
Conventions de nommage, organisation du code et des projets, commentaires et documentation,
gestion des erreurs et journalisation, et bonnes pratiques avec les assistants IA. 🤖

### D. [Raccourcis et astuces Visual Studio 2026](annexes/visual-studio-2026/README.md) 🆕
Raccourcis essentiels pour VB.NET, *snippets* personnalisés, extensions recommandées, extensions IA.

### E. [Guide de migration vers .NET 10 LTS](annexes/migration-net10/README.md) 🆕
Stratégies depuis .NET 6 / 8 / Framework, checklist complète, breaking changes .NET 10, considérations
spécifiques à VB.NET.

### F. [Glossaire et acronymes](annexes/glossaire/README.md)
Terminologie .NET, patterns et architectures, acronymes courants, vocabulaire IA et développement assisté.

### G. [FAQ et dépannage](annexes/faq-depannage/README.md)
Erreurs courantes et solutions, problèmes de performance, problèmes de déploiement, compatibilité des
versions .NET, pièges fréquents avec l'IA et VB.NET. 🤖

### H. [Versions .NET et cycle de support](annexes/versions-reference/README.md)

| Version | Type | Sortie | Fin de support | État (juin 2026) |
|---------|------|--------|----------------|------------------|
| **.NET 10** | **LTS** | Novembre 2025 | **14 novembre 2028** | ✅ **Recommandée** |
| .NET 9 | STS | Novembre 2024 | 10 novembre 2026 | ⚠️ Support bientôt terminé |
| .NET 8 | LTS | Novembre 2023 | 10 novembre 2026 | ⚠️ Support bientôt terminé |
| .NET 11 | STS | Prévue nov. 2026 | ~nov. 2028 | 🔮 À venir |
| .NET Framework 4.8.1 | — | 2022 | Lié au cycle de Windows | 🔧 Legacy / maintenance |

**Visual Studio 2026** : disponibilité générale le 11 novembre 2025 (.NET Conf 2025) — support complet
de .NET 10 et de C# 14.  
**Langage VB.NET** : figé à la version **16.9** (langage stabilisé, *consumption-only*).

---

## ✅ Ce que cette formation garantit

- **Honnête** : on enseigne ce que VB.NET fait réellement en 2026, pas ce qu'il « pourrait » faire.
- **Recentrée** : applications de bureau (WinForms ⭐, WPF), bibliothèques, données, interop, migration.
- **À jour** : .NET 10 LTS et Visual Studio 2026, avec les nouveautés réellement accessibles en VB.NET.
- **Hybride** 🔗 : l'architecture VB.NET / C# comme compétence centrale (module 10 et Annexe B).
- **IA-first** 🤖 : un module complet sur le développement assisté, indispensable en VB.NET.
- **Pragmatique** : maintenance de legacy, migration VB6 / .NET Framework, déploiement desktop réel.

**Durée estimée** : ~30-40 h de lecture théorique ; 22-28 jours pour le parcours complet en pratiquant les exemples  
**Public** : développeurs VB.NET débutants à confirmés, équipes de maintenance et de migration legacy  
**Prérequis** : bases en programmation recommandées

---

## 🏷️ Légende des indicateurs

- 🆕 **Nouveau** : spécifique à .NET 10 LTS ou Visual Studio 2026
- 🔄 **Mis à jour** : contenu actualisé pour .NET 10
- ⭐ **Cœur VB.NET** : scénario phare ou point fort idiomatique du langage
- ✅ **Réaliste en VB.NET** : scénario pris en charge et recommandé en VB.NET
- ⚠️ **Limite VB.NET** : sujet à réaliser plutôt en C# (voir [Annexe B](annexes/frontiere-vbnet-csharp/README.md))
- 🔗 **Hybride** : interopérabilité / architecture VB.NET ↔ C#
- 🤖 **IA** : développement assisté par intelligence artificielle

---

**Juin 2026**  
**.NET** : 10 LTS (novembre 2025, support jusqu'en novembre 2028)  
**Visual Studio** : 2026  
**Langage** : VB.NET 16.9 (stabilisé)  
**Licence** : Creative Commons BY-NC-SA 4.0
