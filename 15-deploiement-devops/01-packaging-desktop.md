🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 15.1 — Packaging desktop

**Livrer une application de bureau VB.NET : comment empaqueter le runtime, et comment l'installer chez l'utilisateur**

---

## 🎯 Deux décisions, pas une

Déployer une application de bureau VB.NET (Windows Forms ⭐, WPF) suppose de répondre à **deux
questions distinctes**, souvent confondues :

1. **Comment empaqueter le runtime ?** — l'application embarque-t-elle .NET, ou compte-t-elle sur
   un runtime déjà présent ? C'est le choix **framework-dependent vs self-contained**, auquel
   s'ajoute l'option **fichier unique**.
2. **Comment l'installer et la mettre à jour chez l'utilisateur ?** — c'est le choix du
   **mécanisme de distribution** : **ClickOnce** ou **MSIX**.

Bonne nouvelle pour le cœur de cible VB.NET : **les deux décisions sont pleinement outillées et
identiques à C#**. Le packaging est une affaire de **build et d'outillage**, pas de langage — on
suit les mêmes recettes, on cible simplement un projet VB.

---

## 📦 Décision 1 — le modèle de publication

### Framework-dependent (dépendant du framework)

Le livrable contient **votre application et ses dépendances tierces**, mais **pas** le runtime
.NET : celui-ci doit être **installé sur la machine cible**. C'est le modèle de publication dominant depuis les débuts de .NET.

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

- ✅ **Livrable léger** ; **mise à jour du runtime indépendante** de l'application (le mécanisme de
  *roll-forward* sélectionne automatiquement le dernier correctif de la version majeure installée).
- ❌ Exige un **runtime .NET présent** sur le poste — maîtrisable en entreprise, moins garanti
  pour le grand public.

### Self-contained (autonome)

Le livrable embarque **l'application, ses dépendances *et* le runtime .NET** : le poste cible
**n'a besoin d'aucun .NET préinstallé**. Il faut alors **spécifier un identifiant de runtime (RID)**.

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

- ✅ **Aucune dépendance** sur le poste ; **version du runtime figée** et donc parfaitement
  reproductible.
- ❌ **Livrable volumineux** (il inclut le runtime et les bibliothèques de base) ; la mise à jour de .NET impose de **republier une nouvelle version** de l'application.

### Comparaison

| Critère | Framework-dependent | Self-contained |
|---------|---------------------|----------------|
| **Runtime .NET sur la cible** | **Requis** (installé) | **Inclus** dans le livrable |
| **Taille du livrable** | Petite | Grande (runtime + BCL) |
| **RID obligatoire** | Non (portable possible) | **Oui** |
| **Mise à jour du runtime** | Indépendante (*roll-forward*) | Via une nouvelle version de l'app |
| **Reproductibilité** | Dépend du poste | Totale |
| **Cas typique** | Parc Windows avec .NET déployé | Poste sans .NET garanti / version figée |

> ℹ️ **Le RID (*Runtime Identifier*).** Il désigne la plateforme cible : `win-x64`, `win-x86`,
> `win-arm64`. Le `TargetFramework` d'une application de bureau est `net10.0-windows`. On le fixe
> dans le `.vbproj` ou en argument de `dotnet publish`.

```xml
<!-- .vbproj d'une application Windows Forms -->
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <UseWindowsForms>true</UseWindowsForms>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <SelfContained>false</SelfContained>
</PropertyGroup>
```

---

## 🗜️ Le fichier unique (*single-file*)

Indépendamment du modèle, on peut **regrouper tout le livrable en un seul exécutable** — pratique
à copier et à distribuer. Le fichier unique est compatible avec les deux modèles de publication, framework-dependent comme self-contained.

```bash
# Fichier unique, dépendant du framework
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

```xml
<PropertyGroup>
  <PublishSingleFile>true</PublishSingleFile>
</PropertyGroup>
```

Quelques nuances à connaître :

- **Taille.** Un fichier unique **self-contained** est **volumineux**, puisqu'il inclut le runtime et les bibliothèques du framework.
- **Démarrage.** Depuis .NET 5, les bibliothèques **managées** sont chargées **directement depuis
  le *bundle***, sans extraction sur disque. Seules les bibliothèques **natives** incluses sont
  **extraites au premier lancement** (sous `%TEMP%\.net` sur Windows), ce qui peut le ralentir ;
  le livrable est par ailleurs **spécifique à une plateforme**.
- **Bibliothèques natives.** Pour les inclure dans l'auto-extraction, on ajoute
  `-p:IncludeNativeLibrariesForSelfExtract=true`.
- **Combinaisons.** Le fichier unique se combine avec **ReadyToRun** (R2R) et le ***trimming***.
  R2R précompile l'IL en code natif : livrable plus gros, mais **démarrage à froid plus rapide** —
  intéressant pour une application de bureau réactive.

---

## ✂️ Le *trimming* — et son piège VB.NET ⚠️

Le *trimming* (élagage / *tree shaking*) supprime par analyse statique le code inutilisé pour
réduire la taille. **Deux limites majeures** le concernent en VB :

- **Réservé au self-contained.** Activer `PublishTrimmed=true` sur un livrable framework-dependent échoue (erreur `NETSDK1102` : l'optimisation de taille n'est pas prise en charge pour cette configuration de publication).
- **Hostile à la réflexion.** Le *trimming* repose sur une analyse statique : du code qui charge des types **dynamiquement par réflexion** peut voir ces types **supprimés** à tort, provoquant des erreurs **à l'exécution**.

> ⚠️ **Pourquoi c'est sensible en VB.NET.** La **liaison tardive**
> (`Option Strict Off`, → **[14.5](../14-performance/05-bonnes-pratiques.md)**) et certains usages de l'espace **`My`**
> sont **fortement réflexifs** — précisément ce que le *trimming* tolère mal. Un livrable VB
> *trimmé* qui en abuse peut **planter** une fois déployé. **Règle** : si l'on *trimme*, on
> **teste l'application complète** (suite d'intégration → **[module 13.3](../13-tests-qualite/03-tests-integration.md)**)
> avant toute mise en production. À ne pas confondre avec l'**élagage des paquets NuGet** — un
> mécanisme distinct et, lui, sans risque — dont la distinction est détaillée en **[15.6](06-outils-build-net10.md)**.

> 🚫 **Et Native AOT ?** La compilation native anticipée (binaire natif sans JIT, démarrage très
> rapide) impose des **restrictions** — pas de génération de code à l'exécution, réflexion limitée,
> paquets NuGet partiellement compatibles. Elle **n'est pas prise en charge en pratique pour VB**
> (→ **[Annexe B.4](../annexes/frontiere-vbnet-csharp/README.md)**) : ce besoin précis est une voie
> **C#**.

---

## 🚚 Décision 2 — le mécanisme de distribution

Une fois le livrable produit, reste à le **porter chez l'utilisateur** et à gérer ses **mises à
jour**. Deux mécanismes natifs, tous deux **disponibles pour VB.NET**.

### ClickOnce — le classique, simple et auto-actualisé

ClickOnce déploie des applications **Windows Forms, WPF ou console** depuis une **page web, un partage réseau ou un média**, sans privilèges administrateur.
Ses atouts historiques en font l'outil de prédilection des applications **métier (LOB) internes** :

- **Mise à jour automatique** : l'application peut **vérifier et installer** les nouvelles versions, avec une option de **mise à jour obligatoire** pour aligner tout le parc.
- **Modes en ligne / hors ligne** ; **pas de droits admin** requis.
- **Signature obligatoire** : les manifestes ClickOnce doivent être **signés cryptographiquement**.

Côté outillage, depuis Visual Studio 2019 (16.8), on publie des applications de bureau **.NET Core 3.1 / .NET 5+** en ClickOnce via **l'outil Publier** (clic droit sur le projet ▸ *Publier*). ClickOnce s'utilise **indifféremment en C# ou en Visual Basic .NET**.
Pour l'automatisation, la publication en ligne de commande **via MSBuild requiert un profil de publication** préalablement créé dans Visual Studio (→ **[15.3, CI/CD](03-cicd.md)**).

- ✅ Mise en place **rapide**, auto-update **intégrée**, idéale en **intranet**.
- ❌ **Windows uniquement** ; isolation limitée par rapport à MSIX ; signature à gérer.

### MSIX — le format moderne, conteneurisé

MSIX est le **format de package Windows moderne** (`.msix` / `.msixbundle`). Il installe
l'application dans un **conteneur léger**, ce qui garantit une **installation propre** et une
**désinstallation nette** (sans résidus dans le registre ou le système de fichiers). Ses
caractéristiques :

- **Mise à jour automatique** et **gestion de version** intégrées ; **pas de droits admin**.
- **Signature obligatoire** (certificat de confiance).
- **Compatible Microsoft Store** (→ **[15.2](02-microsoft-store.md)**) et bien adapté aux **parcs
  d'entreprise gérés** (Intune/MDM).
- Création via le **projet de package d'application Windows** (*Windows Application Packaging
  Project*), le **MSIX packagé en projet unique**, ou l'outil **MSIX Packaging Tool** ; il
  empaquette la **sortie publiée** de votre application VB.

- ✅ Installation/désinstallation **propres**, **Store**, **gouvernance** d'entreprise.
- ❌ **Windows 10/11** ; certificat et chaîne de confiance à mettre en place.

### ClickOnce vs MSIX

| Critère | **ClickOnce** | **MSIX** |
|---------|---------------|----------|
| **Maturité** | Classique, éprouvé | Moderne |
| **Installation** | Lien web / partage / média | Package signé `.msix` |
| **Mise à jour auto** | ✅ intégrée | ✅ intégrée |
| **Droits admin** | Non requis | Non requis |
| **Isolation / conteneur** | Non | ✅ (install propre, désinstallation nette) |
| **Microsoft Store** | Non | ✅ (→ 15.2) |
| **Parcs gérés (Intune/MDM)** | Limité | ✅ |
| **Signature** | Requise | Requise |
| **Idéal pour** | Apps LOB **internes**, auto-update simple | Distribution **moderne**, Store, entreprise |

---

## 🧭 Guide de décision

En combinant les deux décisions, quelques profils types se dégagent :

- **Application LOB interne**, parc Windows avec .NET déployé, mises à jour fréquentes →
  **framework-dependent + ClickOnce**. Léger, simple, auto-actualisé.
- **Application grand public**, poste sans .NET garanti → **self-contained** (éventuellement
  **fichier unique**) **+ MSIX** (ou Microsoft Store, → 15.2).
- **Application d'entreprise gérée** (Intune/MDM) → **MSIX**, pour la gouvernance et l'installation
  propre.
- **Distribution la plus simple possible** (un seul fichier à copier) → **self-contained
  single-file** — *trimmé* avec **prudence** si l'application recourt à la liaison tardive ou à `My`.

> 💡 Le bon réflexe : choisir le **modèle de publication** selon la **cible** (présence ou non de
> .NET, contrainte de taille), puis le **mécanisme de distribution** selon le **public** (interne
> vs grand public, Store, parc géré). Les deux choix sont **indépendants** et se composent.

---

## 🔁 En résumé

- **Deux décisions distinctes** : *empaqueter le runtime* (framework-dependent vs self-contained,
  + fichier unique) et *distribuer* (ClickOnce vs MSIX). Toutes deux **pleinement disponibles en
  VB.NET**, à l'identique de C#.
- **Framework-dependent** = léger mais exige .NET sur la cible ; **self-contained** = autonome mais
  volumineux (RID requis). Le **fichier unique** se pose sur l'un ou l'autre, au prix d'un démarrage
  à froid plus lent.
- **Le *trimming*** ne vaut que pour le self-contained et **s'accommode mal de la réflexion** —
  vigilance avec la **liaison tardive** et `My` ; **Native AOT** reste hors VB.
- **ClickOnce** : simple, auto-actualisé, idéal en **intranet** ; **MSIX** : moderne, conteneurisé,
  **Store** et **parcs gérés**.

La distribution via la boutique Windows fait l'objet de la **[15.2 — Microsoft Store](02-microsoft-store.md)**,
et l'automatisation de toute cette chaîne, de la **[15.3 — CI/CD](03-cicd.md)**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Distribution via le Microsoft Store](/15-deploiement-devops/02-microsoft-store.md)
