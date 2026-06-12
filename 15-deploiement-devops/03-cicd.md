🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 15.3 — CI/CD

**Automatiser build, test et livraison — un terrain où VB.NET est un citoyen .NET de plein droit**

---

## 🎯 Intégration et livraison continues

L'intégration continue (**CI**) et la livraison/déploiement continus (**CD**) consistent à
**automatiser** la chaîne qui mène du code au livrable :

- **CI** : chaque *commit* déclenche **build + tests**, pour **détecter les régressions au plus
  tôt** plutôt qu'à la veille d'une livraison.
- **CD** : les builds validés sont **livrés** (artefacts) ou **déployés** automatiquement, de façon
  **reproductible**.

Le point décisif pour ce cours : **la CI/CD est agnostique du langage**. Un pipeline qui exécute
`dotnet restore`, `dotnet build`, `dotnet test` puis `dotnet publish` traite un projet **VB.NET
exactement comme un projet C#**. Les recettes du monde C# se **transposent telles quelles** — il
suffit de pointer vers le `.vbproj` ou la solution. La **CLI .NET** est le **dénominateur commun**.

---

## 🔧 Le pipeline .NET universel

Quelle que soit la plateforme (GitHub, Azure DevOps, GitLab), un pipeline .NET enchaîne les mêmes
étapes, via les mêmes commandes :

```bash
dotnet restore                          # 1. Restaurer les paquets NuGet
dotnet build -c Release --no-restore    # 2. Compiler en Release
dotnet test  -c Release --no-build      # 3. Exécuter les tests (→ module 13)
dotnet publish -c Release ...           # 4. Produire le livrable (→ 15.1)
# 5. Empaqueter / déployer : ClickOnce, MSIX, Store, Docker, cloud
```

Cette **colonne vertébrale** est identique d'une plateforme à l'autre : seule change la **syntaxe
du fichier de pipeline** (YAML) et le vocabulaire des tâches.

---

## 🟣 L'angle VB.NET : une seule réserve, le runner Windows

Les pipelines sont **identiques à C#**, à **une nuance pratique** près, héritée de la nature des
projets :

- **Applications de bureau** (Windows Forms ⭐, WPF) ciblant `net10.0-windows` : elles référencent
  le framework de bureau Windows et exigent en pratique un **runner Windows** (`windows-latest`,
  pool Windows, runner Windows). Par défaut, leur build échoue sur Linux (erreur `NETSDK1100`) ;
  la propriété `EnableWindowsTargeting` permet certes une **compilation croisée**, mais ni
  l'exécution des tests UI, ni la signature, ni l'empaquetage (ClickOnce/MSIX) — le runner Windows
  reste la voie normale.
- **Bibliothèques, Web API (par contrôleurs), console** multiplateformes : elles se compilent
  **aussi bien sur Linux**, plus rapide et moins coûteux.

C'est la **seule** spécificité réelle à retenir pour la CI/CD en VB.NET. Tout le reste se règle
comme en C#.

---

## 🐙 GitHub Actions

Des workflows **YAML** dans `.github/workflows/`, déclenchés sur *push* ou *pull request*. L'action
`actions/setup-dotnet` installe le SDK voulu.

```yaml
# .github/workflows/ci.yml
name: CI
on:
  push:
    branches: [ main ]
  pull_request:

jobs:
  build:
    runs-on: windows-latest          # requis pour WinForms/WPF ; ubuntu-latest suffit pour libs/Web API
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
          cache: true                # mise en cache NuGet intégrée
      - run: dotnet restore
      - run: dotnet build -c Release --no-restore
      - run: dotnet test  -c Release --no-build
```

Idéal pour les projets hébergés sur GitHub ; écosystème d'actions très riche.

---

## 🔷 Azure DevOps Pipelines

Pipelines **YAML** (`azure-pipelines.yml`), avec les tâches `UseDotNet@2` (installation du SDK) et
`DotNetCoreCLI@2` (commandes `dotnet`). Intégration étroite avec l'écosystème Azure/Microsoft.

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include: [ main ]

pool:
  vmImage: 'windows-latest'          # 'ubuntu-latest' pour les projets multiplateformes

steps:
  - task: UseDotNet@2
    inputs:
      packageType: 'sdk'
      version: '10.0.x'
  - task: DotNetCoreCLI@2
    inputs:
      command: 'build'
      arguments: '-c Release'
  - task: DotNetCoreCLI@2
    inputs:
      command: 'test'
      arguments: '-c Release'
```

Souvent le choix par défaut des organisations déjà investies dans Azure (déploiement App Service,
Key Vault, → 15.5).

---

## 🦊 GitLab CI

Un fichier `.gitlab-ci.yml` décrivant des **stages** et des **jobs**, exécutés par des runners —
typiquement à partir de l'**image Docker du SDK .NET**.

```yaml
# .gitlab-ci.yml
stages: [ build, test ]

build:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:10.0
  script:
    - dotnet restore
    - dotnet build -c Release --no-restore

test:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:10.0
  script:
    - dotnet test -c Release
```

> ⚠️ L'image SDK ci-dessus est **Linux** : parfaite pour bibliothèques, Web API et console, mais
> **inadaptée** à une application **WinForms/WPF**, qui requiert un **runner Windows**.

---

## 📦 Automatiser les livrables desktop et leur distribution

Au-delà de build/test, le pipeline produit et diffuse les livrables de la 15.1/15.2 — toujours sur
un **runner Windows** pour le desktop :

- **ClickOnce** : la publication en ligne de commande passe par **MSBuild** et un **profil de
  publication** (créé au préalable dans Visual Studio, → 15.1).

  ```bash
  msbuild MonApp.vbproj /target:Publish /p:PublishProfile=ProfilClickOnce /p:Configuration=Release
  ```

- **MSIX** : on construit le package puis on le **signe** (`signtool`) avec un **certificat
  conservé dans le coffre à secrets** de la plateforme — jamais en clair dans le dépôt.
- **Microsoft Store** : rappel de la 15.2 — Visual Studio 2026 ayant **retiré la soumission
  automatique depuis l'IDE**, l'automatisation passe par l'**API de soumission du Microsoft Store**.
- **Docker** : build et *push* de l'image conteneur pour une Web API ou un service de fond
  (→ **[15.4](04-docker.md)**).

---

## 🧩 Préoccupations transverses

Quelques éléments communs à toute CI/CD .NET :

- **Mise en cache NuGet** : accélère nettement les builds (cache intégré à `setup-dotnet`, ou cache
  du dossier `~/.nuget/packages`).
- **Secrets** : certificats de signature, clés d'API, identifiants de déploiement vivent dans le
  **coffre à secrets** de la plateforme (GitHub Secrets, groupes de variables Azure DevOps liés à
  **Key Vault**, variables CI/CD GitLab masquées). À traiter sérieusement (→ **[module 16](../16-securite/README.md)**
  et **[15.5, Key Vault](05-cloud-essentiels.md)**).
- **Artefacts** : les livrables (binaire publié, package MSIX, image) sont **publiés** par le
  pipeline pour être déployés ensuite.
- **Déclencheurs** : *push*, *pull request*, **tags/releases**, planification.
- **Builds matriciels** : compiler plusieurs **RID** ou configurations en parallèle.
- **Portes qualité** : analyseurs Roslyn et **SonarQube** (→ **[13.4](../13-tests-qualite/04-analyse-statique.md)**),
  **couverture de tests** (→ **[13.5](../13-tests-qualite/05-couverture-tests-ia.md)**) — un *build*
  peut **échouer** si la qualité régresse.

---

## 🏗️ Bonne pratique : *build once, deploy many*

Un principe directeur : **construire et publier une seule fois**, puis **promouvoir le même
artefact** d'un environnement à l'autre (dev → préproduction → production), avec d'éventuelles
**approbations** entre les étapes. Recompiler à chaque environnement introduit le risque de livrer
un binaire **différent** de celui qui a été testé. L'artefact validé en CI est celui qui part en
production.

---

## 🔁 En résumé

- La **CI/CD est agnostique du langage** : VB.NET y est un **citoyen .NET de plein droit**, et les
  pipelines C# se **transposent tels quels** (CLI .NET comme dénominateur commun).
- Le **pipeline universel** enchaîne `restore → build → test → publish → packaging/déploiement`,
  identique sur **GitHub Actions**, **Azure DevOps** et **GitLab CI** ; seule la **syntaxe YAML**
  diffère.
- **Seule réserve VB** : les applications **WinForms/WPF** (`net10.0-windows`) exigent un **runner
  Windows** ; les projets multiplateformes se compilent aussi sur **Linux**.
- L'**automatisation des livrables** suit la 15.1/15.2 : ClickOnce via **MSBuild + profil**, MSIX
  **signé** (secrets), Store via **API de soumission**, conteneurs (→ 15.4).
- **Secrets dans un coffre**, **caching** pour la vitesse, **portes qualité**, et le réflexe
  **construire une fois, déployer partout**.

La conteneurisation — image, Web API et service de fond — fait l'objet de la
**[15.4 — Conteneurisation](04-docker.md)**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Conteneurisation (Docker pour Web API / Worker ; images conteneur .NET 10)](/15-deploiement-devops/04-docker.md)
