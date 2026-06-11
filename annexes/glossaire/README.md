🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe F — Glossaire et acronymes

Terminologie .NET, *patterns* et architectures, acronymes courants, et vocabulaire de l'IA et du développement
assisté.

Les définitions sont données avec un **angle VB.NET**. Beaucoup de termes proviennent de l'écosystème .NET élargi
que VB **consomme** (souvent décrit en C# dans la documentation) ; les renvois pointent vers le module ou
l'annexe qui approfondit le sujet. Indicateurs : ⭐ propre à VB, 🔗 hybride VB/C#, ⚠️ limite côté VB, 🤖 IA.

---

## F.1 — Terminologie .NET

**ADO.NET** — Couche d'accès aux données « bas niveau » de .NET (connexions, commandes paramétrées,
`DataReader`, `DataSet`). Base sous-jacente d'EF Core (module 7.1).

**Assembly (assembly)** — Unité de déploiement compilée (`.dll` ou `.exe`) contenant du code IL et des
métadonnées. Une fois compilée, une assembly C# est consommable depuis VB sans friction — fondement de
l'interopérabilité (module 9.3, [Annexe B](../frontiere-vbnet-csharp/README.md) 🔗).

**Asynchronie (async/await)** — Modèle d'exécution non bloquant fondé sur `Task`/`Task(Of T)` et les mots-clés
`Async`/`Await` ; essentiel pour une UI réactive et les E/S (module 4).

**BCL (Base Class Library)** — Bibliothèque de classes de base de .NET (collections, E/S, LINQ, etc.), commune à
tous les langages .NET.

**`BackgroundService`** — Classe de base pour un service de fond hébergé, via le *Generic Host*. ⚠️ Pas de modèle
de projet *Worker* en VB → à câbler à la main (module 4.8).

**Boxing / unboxing** — Conversion d'un type valeur en `Object` (*boxing*, avec allocation sur le tas) et
l'opération inverse (*unboxing*). Coûteux dans les boucles chaudes ; les **génériques** l'évitent (modules 2.8, 14.2).

**CLR (Common Language Runtime)** — Moteur d'exécution de .NET : compilation JIT, ramasse-miettes, sûreté de
type. Partagé par VB et C#, c'est lui qui rend l'interopérabilité transparente.

**CLS (Common Language Specification)** — Sous-ensemble de règles garantissant l'interopérabilité entre langages
.NET. Respecter la CLS rend une bibliothèque consommable proprement depuis VB comme depuis C#.

**Code-behind** — Fichier de code associé à une vue (formulaire WinForms, fenêtre WPF/XAML) qui en gère la
logique. ⚠️ Anti-pattern courant : y noyer la logique métier (Annexe C.3).

**CTS (Common Type System)** — Système de types unifié de .NET, partagé par tous les langages — d'où l'identité
des types entre VB et C# ([Annexe A](../correspondance-vbnet-csharp/README.md)).

**`DbContext` / `DbSet`** — Pivots d'EF Core : le `DbContext` représente la session de base de données, chaque
`DbSet(Of T)` une table interrogeable en LINQ (module 7.2).

**Délégué (delegate)** — Type représentant une référence à une méthode ; base des événements et des lambdas. Un
point fort idiomatique de VB via `Handles`/`WithEvents` (module 3.6 ⭐).

**EF Core (Entity Framework Core)** — ORM officiel de .NET ; mappe objets et tables, génère du SQL à partir de
LINQ to Entities (module 7.2).

**Espace `My`** ⭐ — Raccourci propre à VB donnant un accès simplifié à l'application, la machine, les paramètres
et les ressources (`My.Settings`, `My.Computer`…). ⚠️ Support **partiel** sur .NET moderne (correct en WinForms,
limité en WPF, membres web supprimés — module 2.12).

**Framework-dependent vs self-contained** — Deux modes de déploiement : *framework-dependent* suppose un runtime
.NET installé ; *self-contained* embarque le runtime dans le livrable (module 15.1).

**Garbage Collector (GC, ramasse-miettes)** — Gestion automatique de la mémoire managée, par générations. ⭐ En
VB, utilisez `Using`/`IDisposable` pour une libération **déterministe** des ressources non managées (module 14.3).

**Generic Host** — Infrastructure d'hébergement (`Microsoft.Extensions.Hosting`) fournissant DI, configuration,
journalisation et cycle de vie ; sert aux services de fond (module 4.8).

**Génériques (generics)** — Types et méthodes paramétrés par un type (`List(Of T)`, `Of T`), garantissant la
sûreté de type sans *boxing* (modules 2.8, 2.10).

**`Handles` / `WithEvents`** ⭐ — Idiome **déclaratif** de VB pour s'abonner à un événement (sans `AddHandler`
explicite) ; sans équivalent direct en C# (module 3.6).

**Hot Reload** — Application des modifications de code sans redémarrer l'application en cours de débogage ;
précieux pour ajuster une UI WinForms/WPF (module 12.2, [Annexe D](../visual-studio-2026/README.md)).

**`IDisposable` / `Using`** — Contrat de libération déterministe des ressources ; `Using … End Using` garantit
l'appel à `Dispose` (modules 14.3, Annexe C).

**IL / CIL (Intermediate Language)** — Bytecode intermédiaire produit par la compilation de VB ou C#, puis
compilé en code natif par le JIT à l'exécution.

**IntelliSense** — Complétion **syntaxique** de l'IDE (membres, paramètres). À distinguer de Copilot, qui génère
du code porteur de logique ([Annexe D § D.5](../visual-studio-2026/README.md) 🤖).

**JIT (Just-In-Time)** — Compilation du code IL en code natif **au moment de l'exécution**. .NET 10 en améliore
l'*inlining* et la dévirtualisation « gratuitement » (module 14.6).

**LINQ (Language Integrated Query)** ⭐ — Langage de requête intégré (syntaxe `From…Where…Select` ou méthodes
d'extension) ; un point fort de VB (module 2.9, [Annexe A § A.11](../correspondance-vbnet-csharp/README.md)).

**Code managé / non managé** — Le code *managé* s'exécute sous le contrôle du CLR (mémoire, sécurité, types) ; le
code *non managé* (API Windows natives, COM) s'appelle via P/Invoke et l'interop (module 9).

**Marshaling** — Conversion des données entre code managé et non managé lors des appels d'interopérabilité
(module 9.1).

**MSBuild** — Moteur de build de .NET, piloté par les fichiers projet (`.vbproj`). .NET 10 en apporte des
améliorations (module 15.6).

**.NET (moderne) / .NET Framework / .NET Standard** — *.NET moderne* (.NET 10) est multiplateforme et activement
développé ; *.NET Framework* (4.x) est l'ancienne pile Windows en maintenance ; *.NET Standard* est une
spécification d'API permettant la coexistence des deux (modules 1.3, 11.5).

**NuGet** — Gestionnaire de paquets de l'écosystème .NET ; les dépendances se déclarent en `PackageReference`
dans le projet.

**Types nullables (nullable)** — En VB, `Nullable(Of T)` / `T?` désigne un **type valeur nullable**. ⚠️ À ne pas
confondre avec les *nullable reference types* (NRT) de C#, absents de VB (module 2.2).

**P/Invoke (Platform Invoke)** — Mécanisme d'appel des fonctions d'API natives Windows depuis du code managé
(module 9.1).

**PGO (Profile-Guided Optimization)** — Optimisation du JIT guidée par des profils d'exécution réels ; un gain de
.NET 10 obtenu sans changer le code (module 14.6).

**Propriété par défaut (`Default Property`)** ⭐ — Équivalent VB de l'**indexeur** C# (`this[…]`) : permet
d'écrire `objet(index)` sans nommer la propriété. Exige au moins un paramètre requis
([Annexe A § A.8](../correspondance-vbnet-csharp/README.md)).

**ReadyToRun (R2R)** — Pré-compilation partielle en natif pour accélérer le démarrage, sans les contraintes de
l'AOT (module 15.1).

**Records (record)** 🔗 — Types à **égalité de valeur** générée. ⚠️ **Déclarables uniquement en C#** ;
VB les **consomme** mais ne possède ni `record` ni `with` (module 3.7, [Annexe B.7](../frontiere-vbnet-csharp/README.md)).

**Réflexion (reflection)** — Inspection et manipulation des types et métadonnées à l'exécution
(`System.Reflection`) ; base de nombreux frameworks (module 3.8).

**Roslyn** — Le compilateur .NET (C# et VB) exposé sous forme d'API ; sous-tend les analyseurs et les *source
generators*.

**Runtime** — Environnement d'exécution (le CLR plus les bibliothèques) qui fait tourner une application .NET.

**SDK (.NET SDK)** — Ensemble d'outils de build et de ligne de commande (`dotnet`) pour créer, compiler et
publier des projets .NET.

**Projet SDK-style** — Format de fichier projet moderne, concis (`.vbproj` allégé), cible de la migration depuis
les anciens projets (module 11.3, [Annexe E](../migration-net10/README.md)).

**Sérialisation (serialization)** — Conversion d'objets en un format transportable/stockable et inverse :
`System.Text.Json`, XML, CSV (module 7.5).

**SignalR** — Bibliothèque de communication temps réel ; consommable en client VB, hub réalisable (module 8.4).

**SLNX** — Nouveau format de solution **XML** de Visual Studio 2026, à l'analyse plus rapide et interopérable avec
les versions antérieures ([Annexe D](../visual-studio-2026/README.md)).

**Source generator** 🔗 — Composant Roslyn qui **génère du code à la compilation**. ⚠️ Écrit en C#, et les
générateurs courants ciblent C# (module 6.6, [Annexe B.6](../frontiere-vbnet-csharp/README.md)).

**`Span(Of T)` / `Memory(Of T)`** 🔗 — Vues sur de la mémoire contiguë évitant les allocations. ⚠️ **Consommables**
en VB dans des cas limités ; les API *Span-first* s'écrivent mieux en C# (module 14.4, [Annexe B.7](../frontiere-vbnet-csharp/README.md)).

**TFM (Target Framework Moniker)** — Identifiant du framework cible dans le projet, p. ex. `net10.0` ou
`net10.0-windows` ([Annexe E](../migration-net10/README.md)).

**Windows Forms (WinForms)** ⭐ — Framework d'UI de bureau, **scénario phare de VB** ; modernisé sur .NET 10
(mode sombre intégré, formulaires asynchrones `ShowAsync`, presse-papiers sécurisé) (module 5).

**WinUI 3 (Windows App SDK)** — Pile d'UI Windows « moderne » au design Fluent. ⚠️ **C# / C++ uniquement**
(pas de chemin VB) : le desktop VB reste WinForms ⭐ / WPF ([Annexe B § B.2](../frontiere-vbnet-csharp/README.md)).

**WPF (Windows Presentation Foundation)** — Framework d'UI de bureau fondé sur XAML, le binding et MVVM (module 6).

**XAML** — Langage de balisage XML décrivant les interfaces WPF (et autres). En WPF, le code-behind reste en VB
(module 6.2).

---

## F.2 — *Patterns* et architectures

**Architecture en couches (layered)** — Séparation UI / logique métier / accès aux données ; structure de base
d'une application maintenable (Annexe C.3).

**Architecture hybride VB.NET / C#** 🔗 ⭐ — Stratégie centrale de la formation : déléguer les briques modernes à
des bibliothèques **C#** et les **consommer depuis VB** (module 10, [Annexe B](../frontiere-vbnet-csharp/README.md)).

**Code-First / Database-First** — Deux approches EF Core : *Code-First* part des classes (avec migrations) ;
*Database-First* génère le modèle depuis une base existante — utile pour le legacy (module 7.2).

**CQRS (Command Query Responsibility Segregation)** — Séparation des opérations de lecture et d'écriture ; pattern
avancé pour les systèmes à forte charge.

**DTO (Data Transfer Object)** — Objet simple servant à transporter des données entre couches ou via le réseau,
sans logique métier.

**Injection de dépendances (DI) / Inversion de contrôle (IoC)** — Fournir à un composant ses dépendances de
l'extérieur plutôt que de les créer en interne ; améliore testabilité et découplage (modules 5.2, 8.2).

**Liaison de données (data binding)** — Synchronisation automatique entre l'UI et un modèle de données
(`BindingSource` en WinForms ; `INotifyPropertyChanged`/binding en WPF — modules 5.8, 6.4).

**MVC (Model-View-Controller)** — Pattern d'architecture web séparant données, vue et contrôleur ; la voie web
réaliste en VB passe par les **contrôleurs** d'API (module 8.2 ✅).

**MVVM (Model-View-ViewModel)** — Pattern d'UI de WPF découplant la vue de la logique de présentation. ⚠️ Les
*source generators* de `CommunityToolkit.Mvvm` sont C#-only ; en VB, on dérive directement d'`ObservableObject` /
`RelayCommand` (module 6.6 🔗).

**Observateur (Observer)** — Pattern où des abonnés réagissent aux notifications d'un sujet ; en VB, idiomatique
via `Event`/`RaiseEvent`/`Handles` (module 3.6 ⭐).

**Pattern Dispose (`IDisposable`)** — Convention de libération déterministe des ressources, utilisée avec `Using`
(module 14.3).

**Référentiel (Repository) / Unité de travail (Unit of Work)** — Abstraction de l'accès aux données : le
*Repository* encapsule les requêtes d'une entité, l'*Unit of Work* regroupe les changements en une transaction.

**Séparation des responsabilités (separation of concerns)** — Principe consistant à isoler chaque préoccupation
(UI, métier, données) dans des unités distinctes.

**SOLID** — Cinq principes de conception OO : responsabilité unique (SRP), ouvert/fermé (OCP), substitution de
Liskov (LSP), ségrégation des interfaces (ISP), inversion des dépendances (DIP).

**TAP (Task-based Asynchronous Pattern)** — Le modèle asynchrone standard de .NET, fondé sur `Async`/`Await` et
`Task` (module 4.2).

---

## F.3 — Acronymes courants

| Acronyme | Signification | Note |
|----------|---------------|------|
| ADO | ActiveX Data Objects (.NET) | Accès données bas niveau (7.1) |
| AOT | Ahead-Of-Time | ⚠️ Non pris en charge en pratique pour VB (B.4) |
| API | Application Programming Interface | |
| BCL | Base Class Library | |
| CI/CD | Continuous Integration / Continuous Delivery | Module 15.3 |
| CIL / MSIL | Common Intermediate Language | Bytecode .NET |
| CLR | Common Language Runtime | Moteur d'exécution |
| CLS | Common Language Specification | Interop inter-langages |
| COM | Component Object Model | Interop Office/Windows (9.2) |
| CRUD | Create, Read, Update, Delete | |
| CTS | Common Type System | |
| DI | Dependency Injection | Injection de dépendances |
| DTO | Data Transfer Object | |
| EF | Entity Framework (Core) | ORM (7.2) |
| GC | Garbage Collector | Ramasse-miettes (14.3) |
| gRPC | gRPC Remote Procedure Calls | ⚠️ Outillage orienté C# (B.5) |
| IDE | Integrated Development Environment | Visual Studio 2026 |
| IL | Intermediate Language | Voir CIL |
| IoC | Inversion of Control | Inversion de contrôle |
| JIT | Just-In-Time (compilation) | |
| JSON | JavaScript Object Notation | `System.Text.Json` |
| JWT | JSON Web Token | Authentification (16.1) |
| LINQ | Language Integrated Query | ⭐ Point fort VB (2.9) |
| LTS | Long-Term Support | 3 ans ; .NET 10 jusqu'en nov. 2028 |
| MAUI | Multi-platform App UI | ⚠️ Pas de modèle VB (B.2) |
| MVC | Model-View-Controller | |
| MVVM | Model-View-ViewModel | UI WPF (6.6) |
| NRT | Nullable Reference Types | ⚠️ C# uniquement (2.2) |
| OIDC | OpenID Connect | Authentification (16.1) |
| OOP / POO | Object-Oriented Programming / Prog. Orientée Objet | Module 3 |
| ORM | Object-Relational Mapping | EF Core |
| OWASP | Open Worldwide Application Security Project | Sécurité (16.3) |
| PGO | Profile-Guided Optimization | Gain .NET 10 (14.6) |
| R2R | ReadyToRun | Démarrage accéléré |
| REST | Representational State Transfer | API REST (8.1-8.2) |
| SDK | Software Development Kit | `dotnet` |
| SIMD | Single Instruction, Multiple Data | Gain JIT .NET 10 (14.6) |
| SOLID | 5 principes OO (SRP/OCP/LSP/ISP/DIP) | Voir F.2 |
| STS | Standard-Term Support | 2 ans (.NET 9 → nov. 2026) |
| TAP | Task-based Asynchronous Pattern | async/await (4.2) |
| TFM | Target Framework Moniker | `net10.0` (E) |
| UI | User Interface | |
| WCF | Windows Communication Foundation | ⚠️ Côté serveur, absent du moderne (E.2) |
| WPF | Windows Presentation Foundation | Module 6 |
| XAML | eXtensible Application Markup Language | UI WPF (6.2) |
| XML | eXtensible Markup Language | |

---

## F.4 — Vocabulaire IA et développement assisté 🤖

Termes utiles aux deux facettes IA de la formation : **développer en VB.NET avec l'IA** (module 17) et
**consommer des API d'IA depuis VB.NET** (module 17.9).

**Agent (IA / coding agent)** — Système IA capable d'enchaîner des actions de façon autonome (analyser, modifier,
tester). Ex. l'agent `@modernize-dotnet`, qui prend en charge VB ([Annexe E § E.3](../migration-net10/README.md)).

**Appel de fonctions / d'outils (function calling)** — Capacité d'un LLM à invoquer des fonctions définies par le
développeur. Consommable depuis VB via `Microsoft.Extensions.AI` (module 17.9 ✅).

**Biais C# (des modèles)** ⭐ 🤖 — Concept clé de la formation : les modèles étant surtout entraînés sur du C#,
la **qualité des suggestions en VB est moindre** et le code généré tend vers des tournures C# (modules 1.6.3, 17.1).

**« C#-ism »** ⭐ — Terme de la formation désignant une **construction de syntaxe C# glissée par erreur dans du code
VB** généré (`;`, `{}`, `record`, `await foreach`…). À traquer systématiquement (Annexe C.6, [Annexe A](../correspondance-vbnet-csharp/README.md)).

**Complétion de code vs chat** — Deux modes de Copilot : la *complétion* suggère du code en ligne ; le *chat*
offre une interface conversationnelle (explications, génération, conversion C#→VB).

**Embedding (plongement / vecteur)** — Représentation numérique (vecteur) d'un texte permettant la recherche
sémantique ; brique d'un système RAG (module 17.9).

**Fenêtre de contexte (context window)** — Quantité maximale de texte (mesurée en *tokens*) qu'un modèle peut
prendre en compte en une fois.

**Fine-tuning (affinage)** — Spécialisation d'un modèle pré-entraîné sur des données ciblées.

**Grounding (ancrage)** — Fait d'appuyer les réponses d'un modèle sur des sources factuelles fournies, pour en
réduire les erreurs (principe du RAG).

**Hallucination** — Production par le modèle d'une sortie plausible mais **fausse**. En VB, se manifeste souvent
par une **syntaxe C# inventée comme étant du VB** (module 17.7).

**Inférence (inference)** — Exécution d'un modèle entraîné pour produire une réponse à partir d'une entrée.

**LLM (Large Language Model)** — Grand modèle de langage entraîné sur de vastes corpus de texte (et de code).

**MCP (Model Context Protocol)** — Protocole standardisant la connexion d'un assistant IA à des outils et des
sources de données externes.

**`Microsoft.Extensions.AI`** ✅ — Abstraction .NET unifiant l'accès aux fournisseurs d'IA (chat, *embeddings*) ;
de simples bibliothèques .NET, donc **pleinement dans le périmètre « consommation » de VB** (module 17.9).

**Prompt / Prompt engineering** — Le texte d'instruction fourni au modèle / l'art de le formuler. ⭐
En VB, **toujours préciser « VB.NET » et la version .NET cible** (modules 17.2, Annexe C.6).

**RAG (Retrieval-Augmented Generation)** — Augmentation d'un LLM par des informations **récupérées** (souvent via
*embeddings*) avant génération ; réalisable en VB par consommation d'API (module 17.9).

**Skill (compétence Copilot)** — Capacité spécialisée qu'un agent Copilot découvre et utilise (p. ex. les 30+
compétences de modernisation de `@modernize-dotnet` — [Annexe E](../migration-net10/README.md)).

**System prompt / instructions** — Consignes de cadrage données en amont à un modèle, fixant son rôle et ses
contraintes.

**Température (temperature)** — Paramètre réglant l'aléa des réponses d'un modèle : basse = déterministe, haute =
créative.

**Token / tokenisation** — Unité de texte (mot ou fragment) traitée par un modèle ; sert à mesurer le contexte et
le coût.

**Validation (du code généré)** ⭐ — **Règle d'or** de l'usage de l'IA en VB : tout code généré se **compile et se
relit** avant adoption (module 17.7, Annexe C.6).

**Base vectorielle (vector database)** — Stockage optimisé pour la recherche de similarité entre *embeddings* ;
support fréquent d'un système RAG.

---

### Voir aussi

- [Annexe A — Correspondance syntaxique VB.NET ↔ C#](../correspondance-vbnet-csharp/README.md) (les termes de syntaxe en regard)
- [Annexe B — Frontière VB.NET / C#](../frontiere-vbnet-csharp/README.md) 🔗 (records, Span, source generators, AOT…)
- [Annexe C — Bonnes pratiques](../bonnes-pratiques/README.md) (dont les règles d'usage de l'IA)
- [Annexe H — Versions et cycle de support](../versions-reference/README.md) (LTS/STS, dates)
- Module 17 — [Développer en VB.NET avec l'IA](../../17-developpement-ia/README.md) 🤖

---

**Juin 2026** · .NET 10 LTS · VB.NET 16.9 (stabilisé) · Visual Studio 2026

⏭️ [FAQ et dépannage](/annexes/faq-depannage/README.md)
