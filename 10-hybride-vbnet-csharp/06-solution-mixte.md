🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 10.6 Gérer une solution mixte (NuGet, projets, build, tests)

> **La plomberie de l'hybride : organiser, lier, compiler et tester une solution qui réunit des projets VB.NET et C#.**

Les sections 10.3 et 10.4 ont traité le **code** de la frontière. Celle-ci, qui clôt le chapitre, en traite la **mécanique de projet** : comment structurer une solution à deux langages, gérer ses dépendances, la compiler et la tester. Bonne nouvelle d'emblée : l'outillage .NET est **agnostique du langage**, et Visual Studio gère une solution mixte sans aménagement particulier.

---

## Le principe : la frontière est au niveau du *projet*

Un point structurant à poser d'abord : **on ne mélange pas VB et C# au sein d'un même projet**. Le fichier de projet (`.vbproj` ou `.csproj`) détermine le langage de **tous** ses fichiers. La frontière hybride passe donc toujours **entre projets**, jamais à l'intérieur de l'un d'eux.

En revanche, une **même solution** (`.sln`) réunit sans difficulté des projets des deux langages. MSBuild les compile uniformément, et le *runtime* les fait cohabiter (chapitre 9). C'est exactement ce qu'illustrait l'atelier (10.5) : trois projets, deux langages, une solution.

---

## Organiser les projets

On structure la solution **par responsabilité**, avec une **direction de dépendance** claire et sans cycle. Une disposition courante :

```
ImportateurTransactions/
├─ ImportateurTransactions.sln
├─ Directory.Build.props          ← propriétés MSBuild partagées
├─ Directory.Packages.props       ← versions NuGet centralisées
├─ src/
│  ├─ Importateur.Coeur/    (C#)   ← performance + fonctionnalités
│  ├─ Importateur.Metier/   (VB)   ← règles + agrégation
│  └─ Importateur.UI/       (VB)   ← interface de bureau
└─ tests/
   ├─ Importateur.Coeur.Tests/    (C# ou VB)
   └─ Importateur.Metier.Tests/   (VB)
```

Quelques principes :

- **Une responsabilité par projet** ; les bibliothèques C# restent focalisées (10.3).
- **Une direction de dépendance unique** (`UI → Metier → Coeur`), sans cycle.
- **Une convention de nommage** cohérente (`Societe.Produit.Couche`).
- Séparer **`src/`** et **`tests/`** par des dossiers de solution.

---

## Lier les projets : trois voies

| Voie | Mécanisme | Quand l'utiliser |
|------|-----------|------------------|
| **Référence de projet** | `ProjectReference` | Projets **co-développés** dans la même solution (cas hybride courant) |
| **Paquet NuGet** | `PackageReference` | Bibliothèque **versionnée / partagée** entre solutions, ou tierce |
| **Référence de DLL** | `Reference` | Assembly compilée fournie **sans source ni paquet** |

Une **référence de projet** depuis un projet VB vers un projet C# n'a rien de particulier — l'extension trahit le changement de langage, mais MSBuild s'en moque :

```xml
<!-- Dans Importateur.Metier.vbproj -->
<ItemGroup>
  <ProjectReference Include="..\Importateur.Coeur\Importateur.Coeur.csproj" />
</ItemGroup>
```

Si le cœur devient un paquet réutilisable, on le consomme par `PackageReference` :

```xml
<ItemGroup>
  <PackageReference Include="Societe.Importateur.Coeur" Version="1.2.0" />
</ItemGroup>
```

En pratique : **référence de projet** pendant le développement actif des deux côtés ; **NuGet** dès que la bibliothèque est partagée entre plusieurs solutions ou distribuée.

---

## Cibles et compatibilité

Les projets doivent cibler des **TFM compatibles**. Le plus simple : tout en `net10.0`. Deux nuances utiles :

- **Réutilisation large / legacy** : une bibliothèque C# peut cibler **`netstandard2.0`** pour être consommable **à la fois** par une application VB .NET Framework et par une application VB sur .NET moderne. C'est précieux pendant une **coexistence** .NET Framework / .NET moderne (module 11.5). Attention : une application VB .NET Framework **ne peut pas** consommer une bibliothèque `net10.0` — seulement `netstandard2.0` (ou une cible Framework).
- **Version de langage indépendante par projet** : le projet C# peut activer le **dernier C#** (`<LangVersion>latest</LangVersion>`) tandis que le projet VB reste, par nature, à VB **16.9**. Chaque projet **gouverne son propre langage** — c'est précisément ce qui permet à C# d'apporter les nouveautés que VB consomme.

---

## Centraliser la configuration

Deux fichiers à la racine de la solution évitent la duplication entre projets — quel que soit leur langage.

**`Directory.Build.props`** applique des propriétés MSBuild **partagées** à tous les projets :

```xml
<!-- Appliqué à TOUS les projets (VB et C#) -->
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Version>1.0.0</Version>
    <Company>Societe</Company>
  </PropertyGroup>

  <!-- ⚠️ Options propres au langage : à conditionner dans un fichier partagé -->
  <PropertyGroup Condition="'$(MSBuildProjectExtension)' == '.csproj'">
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <PropertyGroup Condition="'$(MSBuildProjectExtension)' == '.vbproj'">
    <OptionStrict>On</OptionStrict>
    <OptionExplicit>On</OptionExplicit>
  </PropertyGroup>
</Project>
```

> ⚠️ Certaines propriétés sont **spécifiques au langage** : `Nullable`, `ImplicitUsings`, `LangVersion` relèvent de C# ; VB utilise `OptionStrict`, `OptionExplicit`, `OptionInfer`, `OptionCompare`. Dans un fichier partagé, on les **conditionne** par l'extension du projet (comme ci-dessus), sinon on les laisse dans chaque projet.

**`Directory.Packages.props`** centralise les **versions NuGet** (*Central Package Management*) :

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="xunit" Version="2.9.0" />
    <PackageVersion Include="Moq" Version="4.20.0" />
  </ItemGroup>
</Project>
```

Les projets référencent alors les paquets **sans numéro de version** (`<PackageReference Include="xunit" />`), la version venant du fichier central. Cela vaut indistinctement pour les projets VB et C#.

---

## Construire

Le build est **agnostique du langage**. Les commandes opèrent sur **toute la solution**, quel que soit le mélange :

```
dotnet restore
dotnet build
dotnet test
```

L'**ordre de compilation** découle des dépendances de projet : `Coeur` est compilé avant `Metier`, lui-même avant `UI`. Côté **CI/CD** (module 15.3), une solution mixte ne demande **aucun traitement particulier** : un pipeline standard `restore → build → test` la prend en charge telle quelle.

---

## Tester

Les **projets de test peuvent être écrits dans l'un ou l'autre langage** — ce sont de simples assemblies .NET référençant le code à tester. En pratique, on teste souvent chaque couche dans **son** langage, par lisibilité, mais rien ne l'impose.

La **frontière facilite la testabilité**. Pour tester le métier VB **en isolant** le cœur C#, on fait dépendre le métier d'une **interface** exposée par le cœur (10.3), que l'on **simule** (*mock*) dans le test :

```vb
' Test VB du métier : le cœur C# est remplacé par un mock de son interface
Dim faux = New Mock(Of IAnalyseurCsv)()
faux.Setup(Function(a) a.AnalyserAsync(It.IsAny(Of IO.Stream)(), It.IsAny(Of CancellationToken)())) _
    .ReturnsAsync(transactionsDeTest)

Dim service As New ServiceImport(faux.Object)
' ... vérifier les règles et l'agrégation ...
```

Par rapport à l'atelier (10.5), cela suppose deux ajustements : le cœur **expose une interface** `IAnalyseurCsv` (implémentée par `AnalyseurCsv`), et `ServiceImport` **la reçoit par constructeur** au lieu d'instancier l'analyseur lui-même — l'injection de dépendances (module 4.8) mise au service des tests (module 13.2).

Le cœur C#, lui, est testé **à part**, sur ses propres entrées. Frameworks (xUnit, NUnit, MSTest) et bibliothèques de *mocking* (Moq, NSubstitute) fonctionnent avec les deux langages — détails au **module 13**, qui couvre aussi le **`Microsoft.Testing.Platform`** de .NET 10. 🆕

---

## Versionner le cœur partagé

Dès que le cœur C# est distribué en **paquet NuGet**, on le **versionne** en SemVer, et sa **surface publique est le contrat** (10.3) :

- changement **interne** (perf, refactor) → version **corrective** ;
- ajout **non cassant** → version **mineure** ;
- modification **cassante** de la surface → version **majeure**.

Pendant le développement actif des deux côtés, la **référence de projet** évite le cycle « empaqueter / publier / mettre à jour » et reste plus fluide.

---

## ⚠️ Pièges et points pratiques

- **La frontière est le projet** : ne pas chercher à mélanger les langages dans un même `.csproj`/`.vbproj`.
- **Propriétés spécifiques au langage** : conditionner `Nullable`, `LangVersion` (C#) et `Option*` (VB) dans un fichier partagé.
- **Analyseurs souvent orientés C#** : certains analyseurs Roslyn et StyleCop ne s'appliquent qu'à C# ; on les configure **par langage** (module 13.4).
- **IDE** : Visual Studio 2026 gère pleinement les solutions mixtes. **VS Code reste limité pour VB** (pas de C# Dev Kit pour VB — module 1.4) : pour une solution hybride confortable, **VS 2026** est le choix naturel.

---

## En résumé

- La **frontière passe entre projets** : un langage par projet, mais **une seule solution** réunit VB et C#.
- **Organiser par responsabilité**, direction de dépendance unique (`UI → Metier → Coeur`), `src/` et `tests/` séparés.
- **Lier** par **référence de projet** (co-développement), **NuGet** (partage/distribution) ou **DLL** (binaire opaque).
- **Compatibilité** : cibler des TFM compatibles ; `netstandard2.0` pour une bibliothèque consommable par le *legacy* ; **chaque projet gouverne sa version de langage** (C# récent, VB 16.9).
- **Centraliser** : `Directory.Build.props` (propriétés MSBuild, options conditionnées par langage) et `Directory.Packages.props` (versions NuGet).
- **Build et tests agnostiques** : `dotnet build`/`test` sur toute la solution ; la frontière facilite le *mocking* via interface (module 13).

---

> 🏁 **Fin du chapitre 10.** Avec le chapitre 9 (le *mécanisme* de l'interopérabilité) et ce chapitre 10 (l'*architecture* hybride), la **stratégie 2026** est complète : VB.NET consomme pleinement le .NET moderne sans renoncer à ses forces. La suite, le **module 11 — Migration et maintenance du legacy**, prolonge ces idées vers la coexistence et la modernisation de l'existant ; l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)** en reste le catalogue de référence.

⏭️ [Migration et maintenance du code legacy](/11-migration-legacy/README.md)
