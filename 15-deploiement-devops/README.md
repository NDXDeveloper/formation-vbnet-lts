🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 🚀 15. Déploiement et DevOps

**Livrer une application VB.NET : du poste de bureau au cloud, en passant par l'automatisation**
.NET 10 LTS · Visual Studio 2026 · VB.NET 16.9

---

## 🎯 De quoi parle ce module

Écrire le code n'est que la moitié du travail : encore faut-il le **livrer** — l'empaqueter, le
distribuer, l'automatiser, et le faire tourner de façon fiable et reproductible. C'est l'objet du
**déploiement** et des pratiques **DevOps**.

Ce domaine occupe une place **particulière** dans le paysage VB.NET, et il vaut la peine de la
cadrer honnêtement d'emblée :

- D'un côté, il touche au **scénario phare** du langage : le **déploiement d'applications de
  bureau** (Windows Forms ⭐, WPF). C'est un terrain **historique et toujours pertinent** de VB,
  pour lequel l'outillage est mûr et complet (ClickOnce, MSIX, Microsoft Store).
- De l'autre, l'**essentiel de l'outillage DevOps** — la CLI .NET, MSBuild, les pipelines CI/CD,
  Docker, les SDK cloud — est **agnostique du langage**. Un pipeline qui exécute `dotnet build`,
  `dotnet test` puis `dotnet publish` traite un projet **VB.NET exactement comme un projet C#** ;
  un `Dockerfile` fondé sur l'image du SDK .NET construit l'un comme l'autre.

Autrement dit : le déploiement est, pour l'essentiel, une affaire de **runtime et d'outillage**,
**pas** de langage de surface. VB.NET y est donc **pleinement à sa place** — à quelques exceptions
clairement signalées.

---

## ⚠️ Le périmètre, sans détour

Quelques zones relèvent de la **frontière VB/C#** déjà rencontrée dans le cours, et ce module les
indique honnêtement :

- **Azure Functions** ne dispose **pas de support VB officiel** → on l'écrit en **C#** (→ 15.5,
  et **[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**).
- **Native AOT** n'est **pas pris en charge** en pratique pour VB (→ **[Annexe B.4](../annexes/frontiere-vbnet-csharp/README.md)**) :
  les modes de packaging **framework-dependent**, **self-contained** et **fichier unique** restent
  disponibles, mais la compilation native anticipée est une voie C#.
- **L'élagage (*trimming*)** fonctionne sur l'IL, mais s'accommode mal du code **fortement
  réflexif** : la **liaison tardive** (`Option Strict Off`) et certains usages de l'espace `My`
  peuvent provoquer des avertissements ou des dysfonctionnements à l'exécution (→ 15.6).
- **La conteneurisation** vise les **services** : Web API (en VB, **par contrôleurs**,
  → **[module 8.2](../08-services-web/02-web-api-controllers.md)**) et services de fond — en
  gardant à l'esprit qu'il n'existe **pas de modèle de projet « Worker » en VB**, à câbler à la
  main via le Generic Host (→ **[module 4.8](../04-async/08-background-services.md)**).

Rien de tout cela n'entrave le cœur de cible — le **déploiement desktop** — qui demeure entièrement
réalisable et recommandé en VB.NET.

---

## 📋 Ce que vous saurez faire à l'issue de ce module

- **Empaqueter et distribuer** une application de bureau VB.NET : **ClickOnce**, **MSIX**, et
  publication sur le **Microsoft Store**.
- **Choisir** entre déploiement **dépendant du framework** et **autonome** (*self-contained*),
  et recourir au **fichier unique** quand c'est pertinent.
- **Automatiser** la chaîne build → test → publication avec **GitHub Actions**, **Azure DevOps**
  ou **GitLab CI**.
- **Conteneuriser** une Web API ou un service de fond VB.NET avec **Docker** et les **images
  conteneur .NET 10**.
- **Consommer** les services cloud essentiels via leurs **SDK** (App Service, Blob Storage,
  Key Vault), en sachant ce qui se délègue à C#.
- **Tirer parti** des **outils de build de .NET 10** (élagage des paquets NuGet, améliorations
  MSBuild) pour des livrables plus sobres.

---

## 🗂️ Plan du module

| Section | Sujet | En bref |
|---------|-------|---------|
| **15.1** | [Packaging desktop](01-packaging-desktop.md) | ClickOnce, MSIX, *framework-dependent* vs *self-contained*, fichier unique — le cœur du déploiement VB ⭐. |
| **15.2** | [Microsoft Store](02-microsoft-store.md) | Distribuer une application de bureau via la boutique Windows. |
| **15.3** | [CI/CD](03-cicd.md) | GitHub Actions, Azure DevOps, GitLab CI : automatiser build, test et publication. |
| **15.4** | [Conteneurisation](04-docker.md) 🆕 | Docker pour Web API / service de fond ; images conteneur .NET 10. |
| **15.5** | [Cloud (essentiels via SDK)](05-cloud-essentiels.md) | App Service, Blob Storage, Key Vault ; ⚠️ Azure Functions → C#. |
| **15.6** | [Outils de build .NET 10](06-outils-build-net10.md) 🆕 | Élagage NuGet, améliorations MSBuild : des livrables plus légers. |

La progression va du **plus proche du cœur VB** (le déploiement desktop, 15.1–15.2) vers
l'**automatisation** (15.3) et les **scénarios serveur/cloud** (15.4–15.5), pour finir par
l'**outillage de build** transversal (15.6).

---

## 🔌 Prérequis et liens avec le reste du cours

Ce module s'appuie sur :

- **[Module 5 — Windows Forms](../05-windows-forms/README.md)** ⭐ et
  **[Module 6 — WPF](../06-wpf/README.md)** : les applications de bureau que l'on empaquette ici.
- **[Module 8 — Services web](../08-services-web/README.md)** : la Web API (par contrôleurs) que
  l'on conteneurise (15.4).
- **[Module 1.4 — Installation et outils](../01-introduction-vbnet/04-installation-outils.md)** :
  la CLI .NET et MSBuild, socle de toute la chaîne de build.

Il prolonge et alimente :

- **[Module 13 — Tests et qualité](../13-tests-qualite/README.md)** : les tests qu'exécute le
  pipeline CI/CD (15.3).
- **[Module 16 — Sécurité](../16-securite/README.md)** : la gestion des **secrets** et de
  **Key Vault** lors du déploiement cloud (15.5).
- **[Module 11.3 — Framework → .NET 10](../11-migration-legacy/03-framework-vers-net10.md)** et
  l'**[Annexe E — Guide de migration vers .NET 10](../annexes/migration-net10/README.md)** :
  pour les projets que l'on modernise avant de redéployer.

---

## 🧭 Le bon réflexe DevOps en VB.NET

Une idée directrice traverse ce module : **traiter VB.NET comme un citoyen .NET de plein droit**
dans la chaîne de livraison. La quasi-totalité des recettes (workflows CI/CD, `Dockerfile`,
commandes `dotnet publish`, consommation de SDK cloud) se transposent **telles quelles** depuis le
monde C# — il suffit le plus souvent de **remplacer le langage du projet**, pas la démarche. Quand
une exception existe (Azure Functions, Native AOT, *trimming* réflexif), elle est **isolée et
documentée**, et la **stratégie hybride** (→ **[module 10](../10-hybride-vbnet-csharp/README.md)** 🔗)
fournit la porte de sortie le cas échéant.

Pour le **cœur de cible** — livrer une application de bureau VB.NET à ses utilisateurs —, tout est
là, complet et moderne.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Packaging desktop : ClickOnce, MSIX, *framework-dependent* vs *self-contained*, fichier unique](/15-deploiement-devops/01-packaging-desktop.md)
