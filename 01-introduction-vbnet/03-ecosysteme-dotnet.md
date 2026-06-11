🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 1.3 L'écosystème .NET

Avant d'écrire la moindre ligne de VB.NET, il faut comprendre **sur quoi** ce code va
s'exécuter et **avec quels outils** on le produit. Le mot « .NET » recouvre en réalité
plusieurs choses que l'on confond souvent : une plateforme d'exécution (le *runtime*),
un kit de développement (le *SDK*), un gestionnaire de paquets (*NuGet*) et une façon
d'organiser le code (la *solution* et ses *projets*).

Cette mise au point est d'autant plus importante en VB.NET qu'un piège récurrent —
surtout quand on maintient du legacy — consiste à confondre le **.NET Framework**
(historique, Windows uniquement) et le **.NET moderne** (multiplateforme, dont .NET 10
fait partie). Les deux portent « .NET » dans leur nom, mais ce ne sont pas la même
plateforme, et tout ce qui tourne sur l'un ne tourne pas forcément sur l'autre.

---

## .NET Framework (Windows, legacy) vs .NET moderne (.NET 10)

### Deux lignées, un seul nom

Il existe historiquement **deux lignées** de .NET :

- Le **.NET Framework** est la plateforme historique, apparue en 2002 (v1.0) et arrivée
  à son dernier palier avec la **4.8.1** (2022). Elle fonctionne **uniquement sous
  Windows**, s'installe au niveau de la machine (elle est en grande partie intégrée à
  Windows) et c'est sur elle que des millions de lignes de VB.NET ont été écrites. Elle
  reste **supportée** (son cycle de vie est lié à celui de Windows), mais elle est en
  **mode maintenance** : pas de nouvelle version majeure, pas de nouvelles
  fonctionnalités notables. La 4.8.1 est le terminus.

- Le **.NET moderne** est né en 2016 sous le nom *.NET Core* : une réécriture
  **multiplateforme** (Windows, Linux, macOS), **open source** et modulaire. À partir de
  **.NET 5 (2020)**, Microsoft a abandonné le suffixe « Core » et le numéro 4 (pour
  éviter toute confusion avec le .NET Framework 4.x) : la plateforme s'appelle désormais
  simplement **.NET**. La cadence est annuelle, et l'on en est aujourd'hui à
  **.NET 10 (2025)**.

C'est cette seconde lignée — celle qui évolue — que l'on désigne par « .NET moderne » ou
parfois « .NET (Core) » dans le reste de cette formation.

### Ce qui les distingue concrètement

| | .NET Framework 4.8.1 | .NET moderne (.NET 10) |
|---|---|---|
| **Plateformes** | Windows uniquement | Windows, Linux, macOS |
| **Open source** | Non (en grande partie fermé) | Oui (dépôt `dotnet/runtime`) |
| **Déploiement** | Installé au niveau machine, dans Windows | Par application (*framework-dependent* ou *self-contained*) |
| **Versions côte à côte** | Difficile (une seule version active) | Oui (plusieurs runtimes/SDK cohabitent) |
| **Performance** | Figée | Améliorée à chaque version |
| **Évolution** | Maintenance (figée à 4.8.1) | Active, une version par an |
| **État en 2026** | Legacy / maintenance | Recommandé pour tout nouveau projet |

### Et VB.NET dans tout ça ?

Le **langage** VB.NET fonctionne sur les deux lignées : on peut écrire du VB.NET qui
cible le .NET Framework 4.8.1 *ou* le .NET 10. Mais les **charges de travail**
(*workloads*) disponibles ne sont pas les mêmes :

- **Windows Forms** et **WPF** ⭐ existent sur les deux lignées, mais c'est la version
  **.NET moderne** qui est activement développée (mode sombre intégré, gains de
  performance, formulaires async…). Pour tout nouveau développement de bureau, on cible
  **.NET 10**. (→ modules 5 et 6)
- Les **bibliothèques de classes** : ciblez .NET 10 — ou **`.NET Standard 2.0`** si vous
  devez partager du code avec du legacy .NET Framework (cas de coexistence, → 11.5).
- **ASP.NET Web Forms** (legacy VB) ⚠️ n'existe **que** sur .NET Framework. Il n'y a
  **aucun chemin de migration** vers le .NET moderne : c'est le point de divergence le
  plus douloureux pour les équipes VB qui maintiennent du web ancien. (→ 11.4)

> 💡 **Règle simple pour 2026.** Tout **nouveau** projet VB.NET cible le **.NET moderne
> (.NET 10 LTS)**. Le .NET Framework ne sert plus qu'à **maintenir** l'existant qui ne
> peut pas (ou ne doit pas) migrer.

---

## .NET 10 LTS : apports clés, support jusqu'au 14 novembre 2028

### LTS ou STS : pourquoi viser .NET 10

Microsoft publie **une version de .NET chaque année, en novembre**, selon deux régimes
de support :

- **LTS** (*Long Term Support*) : supportée **3 ans**. Ce sont les versions paires de la
  lignée moderne : .NET 6, **8**, **10**…
- **STS** (*Standard Term Support*) : supportée **24 mois** (depuis .NET 9 — c'était
  18 mois auparavant). Ce sont les versions intermédiaires : .NET 7, 9, 11…

Concrètement, une version **LTS sort une année sur deux**. **.NET 10 est une LTS**,
sortie le **11 novembre 2025** et supportée jusqu'au **14 novembre 2028**. Pour des
applications de production — et c'est typiquement le profil VB.NET (logiciels métier,
applications de bureau d'entreprise) — on privilégie une LTS, pour sa stabilité et sa
longue fenêtre de support.

Le panorama complet des versions et de leur cycle de support figure en **Annexe H**.
Retenez ici l'essentiel : en juin 2026, **.NET 10 est la version recommandée** ; .NET 8
(LTS) et .NET 9 (STS) arrivent tous deux en fin de support le **10 novembre 2026**.

### Ce que .NET 10 apporte — et qui profite à VB.NET

C'est ici qu'intervient une distinction **capitale** pour un développeur VB.NET. Le
**langage** Visual Basic est figé à la version **16.9** (langage stabilisé,
*consumption-only*) : **.NET 10 n'apporte donc aucune nouvelle syntaxe à VB**. En
revanche, une application VB.NET qui passe sur .NET 10 bénéficie **gratuitement**,
sans changer une ligne de code, des améliorations apportées « sous le capot ».

On peut classer ces apports en quatre familles :

**1. Runtime et performance — « gratuit », sans toucher au code** ⭐
Le compilateur JIT de .NET 10 améliore l'*inlining*, la dévirtualisation des méthodes et
les allocations sur la pile ; il ajoute le support d'instructions CPU récentes (AVX10.2),
améliore la génération de code pour les arguments de type structure et optimise mieux les
boucles. Résultat : moins d'allocations et un coût CPU réduit sur les chemins chauds —
votre code VB existant tourne **plus vite** une fois recompilé pour .NET 10. (→ 14.6)

**2. Bibliothèques (BCL) — consommables directement depuis VB** ✅
De nouvelles API arrivent en cryptographie (algorithmes **post-quantiques** ML-KEM,
ML-DSA, SLH-DSA), en sérialisation JSON (`System.Text.Json` : interdiction des propriétés
dupliquées, mode strict, support de `PipeReader`), en compression ZIP **asynchrone**, avec
une nouvelle classe `WebSocketStream`, ainsi que des ajouts en globalisation, numérique,
collections et diagnostics. Toutes ces API sont de « simples » types .NET : elles entrent
**pleinement dans le périmètre de consommation** de VB.NET.

**3. Frameworks applicatifs alignés**
La même livraison met à jour les piles applicatives. Pour un développeur VB, les plus
pertinentes sont :

- **Windows Forms** 🆕 : mode sombre intégré, formulaires asynchrones, presse-papiers
  JSON, anti-capture d'écran — du cœur VB. (→ module 5)
- **WPF** 🆕 : styles Fluent étendus à davantage de contrôles, optimisations de rendu.
  (→ module 6)
- **EF Core 10** 🆕 : `LeftJoin` natif, *named query filters*, colonnes JSON, types
  complexes. (→ 7.2)
- **ASP.NET Core 10** : OpenAPI 3.1, améliorations des Minimal API et de Blazor.
  ⚠️ Côté VB, on reste sur la **Web API par contrôleurs** ; Blazor et Razor demeurent du
  C#. (→ module 8 et Annexe B)

**4. SDK et outillage** 🆕
Le SDK .NET 10 embarque **C# 14** et **F# 10** — mais **aucune évolution du langage VB**
(stabilisé, → 1.1). Il introduit la commande `dnx` (exécution directe d'outils .NET),
la possibilité d'exécuter un fichier C# sans projet (*file-based apps*), des améliorations
MSBuild et l'élagage des dépendances NuGet. Côté IDE, **Visual Studio 2026** (disponibilité
générale le **11 novembre 2025**) apporte une nouvelle interface Fluent, une intégration
poussée de Copilot et un chargement nettement plus rapide des grosses solutions.

> ⚠️ **Le point à retenir pour VB.NET**
> .NET 10 n'ajoute **aucune syntaxe** au langage VB (figé à 16.9). Mais vos applications
> VB.NET profitent **gratuitement** des gains de runtime, des nouvelles API de
> bibliothèques et de l'outillage modernisé. Les nouveautés **de langage** (C# 14) et les
> *workloads* modernes (Blazor, MAUI, Minimal APIs, Native AOT) se **consomment** depuis
> VB via des bibliothèques C# — c'est la stratégie hybride du **module 10**.
> (→ Annexe B)

---

## Runtime, SDK, NuGet, structure d'une solution

Ces quatre briques forment le socle technique sur lequel reposent tous les modules
suivants. Les voici, du plus bas niveau (l'exécution) au plus haut (l'organisation du
travail).

### Le runtime : ce qui exécute votre code

VB.NET ne se compile pas directement en code machine. Le compilateur produit un **code
intermédiaire** (IL, *Intermediate Language*) accompagné de métadonnées, empaqueté dans
des **assemblies** (`.dll` ou `.exe`). Au lancement, le **runtime .NET** prend le relais.
Sur le .NET moderne, ce runtime s'appelle **CoreCLR** et assure notamment :

- la compilation **JIT** (*Just-In-Time*) de l'IL en code machine natif au moment de
  l'exécution — c'est précisément là qu'agissent les optimisations de .NET 10 évoquées
  plus haut ;
- la gestion automatique de la mémoire via le **Garbage Collector (GC)** (→ module 14) ;
- les services transverses : gestion des exceptions, sécurité de type, chargement des
  assemblies.

Le runtime est distribué sous forme de **runtimes partagés** que l'on choisit selon le
type d'application :

- le **.NET Runtime** : le socle (console, bibliothèques) ;
- le **.NET Desktop Runtime** : ajoute **Windows Forms** et **WPF** — indispensable pour
  le cœur VB ;
- l'**ASP.NET Core Runtime** : ajoute l'hébergement web.

Côté machine cliente, deux modèles de déploiement existent (détaillés au module 15) :
le mode **framework-dependent** (l'application suppose le runtime déjà installé : paquet
léger) et le mode **self-contained** (le runtime est embarqué : application autonome,
plus volumineuse).

### Le SDK : ce qui sert à développer

Le **runtime** suffit pour *exécuter* une application ; le **SDK** (*Software Development
Kit*) est nécessaire pour la *construire*. Le SDK .NET regroupe :

- les **compilateurs** VB, C# et F# (pour VB, c'est le compilateur **Roslyn** VB) ;
- **MSBuild**, le moteur de *build* ;
- la **CLI `dotnet`**, l'outil en ligne de commande ;
- les **modèles de projet** (*templates*) et NuGet, intégré.

La CLI `dotnet` est le couteau suisse du développeur :

```bash
dotnet new console -lang VB      # nouveau projet console VB.NET
dotnet new winforms -lang VB     # nouveau projet Windows Forms VB.NET
dotnet build                     # compile la solution
dotnet run                       # compile et exécute
dotnet test                      # exécute les tests
dotnet add package <Paquet>      # ajoute une dépendance NuGet
```

> ⚠️ **Rappel de périmètre.** Parmi la grosse vingtaine de modèles de **projet**
> `dotnet new` du SDK, **douze seulement** existent en VB — cinq familles : console,
> bibliothèque de classes, Windows Forms, WPF et projets de test. C'est exactement le
> terrain couvert par cette formation. (→ 1.6)

Un fichier optionnel, **`global.json`**, permet d'**épingler** une version précise du SDK
pour une solution donnée — utile en équipe et en CI pour garantir des *builds*
reproductibles. À noter : plusieurs versions du SDK **et** du runtime cohabitent sans
conflit sur une même machine (installations *côte à côte*).

### NuGet : le gestionnaire de paquets

**NuGet** est le gestionnaire de paquets officiel de .NET. Un paquet (`.nupkg`) regroupe
des assemblies, leurs dépendances et des métadonnées ; il est publié sur **nuget.org**
(ou sur un flux privé). C'est le mécanisme par lequel on ajoute des **bibliothèques
tierces** — et, en VB.NET, **par lequel on consomme les briques C#** de la stratégie
hybride 🔗 (module 10) ainsi que les SDK cloud et IA (→ 17.9).

Les dépendances se déclarent dans le fichier projet sous forme de **`PackageReference`**
(style SDK moderne), puis sont **restaurées** (`dotnet restore`, exécuté automatiquement
au *build*) dans un cache local. Quelques paquets omniprésents, même en VB :
`Microsoft.Extensions.*` (injection de dépendances, journalisation, *hosting*,
configuration), `Microsoft.EntityFrameworkCore`, `CommunityToolkit.Mvvm`, `Serilog`,
`Polly`. Côté bonnes pratiques (→ 16.4) : épingler les versions, surveiller les
vulnérabilités (scan NuGet) et privilégier les sources de confiance.

### Structure d'une solution

Une **solution** regroupe un ou plusieurs **projets** et sert d'unité de travail dans
Visual Studio. Elle est décrite par un fichier **`.sln`** — ou par le **format XML plus
récent `.slnx`** 🆕, plus lisible et plus facile à fusionner sous Git.

Un projet VB.NET est, lui, décrit par un fichier **`.vbproj`** au **format SDK-style** :
concis et lisible, par opposition aux anciens `.vbproj` verbeux du .NET Framework. Voici
le squelette typique d'un projet console moderne :

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>MonApp</RootNamespace>
    <OptionStrict>On</OptionStrict>
    <OptionExplicit>On</OptionExplicit>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
  </ItemGroup>
</Project>
```

Quelques points à relever :

- `TargetFramework` indique la version ciblée (ici `net10.0` ; ce serait
  **`net10.0-windows`** pour un projet Windows Forms ou WPF, qui dépendent de Windows,
  avec en plus `<UseWindowsForms>true</UseWindowsForms>` ou `<UseWPF>true</UseWPF>`).
- En SDK-style, **les fichiers `.vb` du dossier sont inclus automatiquement** : nul besoin
  de les énumérer.
- Les options de compilation (`OptionStrict`, `OptionExplicit`…) peuvent se définir ici,
  au niveau du projet (→ 2.1).

Une solution s'organise typiquement ainsi — l'exemple illustre au passage la **solution
mixte** VB/C# du module 10.6 :

```text
MaSolution.sln
├── src/
│   ├── MonApp.UI/            (VB.NET — Windows Forms / WPF)
│   │   └── MonApp.UI.vbproj
│   ├── MonApp.Metier/        (VB.NET — bibliothèque de classes)
│   │   └── MonApp.Metier.vbproj
│   └── MonApp.Core/          (C# — briques performantes / modernes) 🔗
│       └── MonApp.Core.csproj
└── tests/
    └── MonApp.Tests/         (projet de test)
        └── MonApp.Tests.vbproj
```

Isoler dans un projet **C#** les fonctionnalités hors périmètre VB, puis le référencer
depuis les projets VB, est exactement la traduction concrète de la stratégie hybride.
On trouve enfin, à la racine d'une solution, plusieurs fichiers utiles : `global.json`
(épingle le SDK), `Directory.Build.props` et `Directory.Packages.props` (propriétés et
versions de paquets communes à tous les projets — *Central Package Management*),
`nuget.config` (sources de paquets), sans oublier `.gitignore` et `README.md`.

### Récapitulatif des briques

| Brique | Rôle | Nécessaire pour… |
|---|---|---|
| **Runtime** | Exécuter le code (JIT, GC, services) | faire **tourner** une application |
| **SDK** | Compiler et outiller (`dotnet`, MSBuild, compilateurs) | **développer** |
| **NuGet** | Distribuer et consommer des bibliothèques | **ajouter des dépendances** (et consommer du C# 🔗) |
| **Solution / Projet** | Organiser et structurer le code | **organiser le travail** |

Ce socle posé, la section suivante passe à la pratique : installer ces briques et choisir
ses outils de développement.

⏭️ [Installation et outils](/01-introduction-vbnet/04-installation-outils.md)
