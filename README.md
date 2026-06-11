# 💻 Formation Visual Basic .NET avec .NET 10 LTS

![License](https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-blue.svg)
![.NET Version](https://img.shields.io/badge/.NET-10%20LTS-purple.svg)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2026-blueviolet.svg)
![VB](https://img.shields.io/badge/VB.NET-16.9%20(stabilisé)-informational.svg)
![Modules](https://img.shields.io/badge/Modules-18-green.svg)
![Language](https://img.shields.io/badge/Langue-Français-blue.svg)
![Mise à jour](https://img.shields.io/badge/Mise%20à%20jour-Juin%202026-brightgreen.svg)

**Un guide progressif pour comprendre, maintenir et faire évoluer des applications VB.NET avec .NET 10 LTS — recentré sur ce que VB.NET fait réellement bien en 2026.**

<p align="center">
  <img src="https://upload.wikimedia.org/wikipedia/commons/4/40/VB.NET_Logo.svg" alt="VB.NET Logo" width="200"/>
</p>

---

## 📖 Table des matières

- [À propos](#-à-propos)
- [Points forts](#-points-forts)
- [Pour qui ?](#-pour-qui-)
- [Contenu](#-contenu-de-la-formation)
- [Démarrage](#-démarrage-rapide)
- [Structure](#-structure-du-projet)
- [Parcours](#-parcours-dapprentissage-suggéré)
- [Ressources](#-ressources-officielles)
- [FAQ](#-faq)
- [Licence](#-licence)
- [Contact](#-contact)

---

## 📋 À propos

Formation centrée sur **ce que Visual Basic .NET fait réellement bien en 2026** : applications de
bureau (Windows Forms, WPF), bibliothèques, accès aux données, interopérabilité COM/Office,
maintenance et migration de code *legacy*, et Web API par contrôleurs — le tout avec une
**architecture hybride VB.NET / C#** assumée et un volet **développement assisté par IA**.

Depuis 2020, VB.NET est un **langage stabilisé** (*feature complete*) : Microsoft le maintient et le
supporte sur .NET 10 LTS, mais **n'ajoute plus de nouvelles fonctionnalités de langage** et ne
l'étend pas à de nouveaux *workloads*. Cette formation en tient compte : elle **n'enseigne pas en
VB.NET ce qui se code aujourd'hui en C#** (Blazor, MAUI, Minimal APIs, Native AOT, microservices…),
mais explique honnêtement la frontière et comment **déléguer ces briques à C# pour les consommer
depuis VB.NET**.

> Cette formation ne cherche pas à promouvoir VB.NET face à d'autres langages, mais à fournir une
> ressource sérieuse et à jour pour des applications existantes qui doivent continuer à évoluer.
> Elle n'est pas parfaite ; l'objectif est de vous faire gagner du temps.

**✨ Ce que vous y trouverez :**

- 📚 **18 modules recentrés** — du langage aux applications de bureau, aux données et à la migration
- 🆕 **.NET 10 LTS** — version de référence (novembre 2025, support jusqu'en novembre 2028)
- 🧭 **Positionnement honnête** — VB stabilisé (16.9), périmètre réel, frontière VB.NET / C# explicite
- 🔗 **Architecture hybride VB.NET + C#** — déléguer à C# ce que VB ne fait pas, et le consommer depuis VB
- 🖥️ **Cœur VB.NET** — Windows Forms, WPF, bibliothèques, ADO.NET / EF Core 10, interop COM/Office
- 🔧 **Maintenance & migration** — VB6 → VB.NET, .NET Framework → .NET 10, ASP.NET Web Forms (impasse)
- 🤖 **Développement assisté par IA** — un module complet (GitHub Copilot, ChatGPT, Claude)
- 📖 **8 annexes** — correspondance VB/C#, frontière VB/C#, bonnes pratiques, migration, références
- 🇫🇷 **En français** — parce que c'est plus accessible

**Durée estimée :** ~30-40 h de lecture théorique • 22-28 jours pour le parcours complet avec ateliers  
**Niveau :** débutant à confirmé

---

## ✨ Points forts

- ✅ **Honnête** — on enseigne ce que VB.NET fait réellement en 2026, pas ce qu'il « pourrait » faire
- ✅ **Recentrée** — applications de bureau (WinForms ⭐, WPF), bibliothèques, données, interop, migration
- ✅ **À jour** — .NET 10 LTS et Visual Studio 2026, avec les nouveautés réellement accessibles en VB.NET
- ✅ **Transparente** — documentation claire de la frontière VB.NET vs C# (Annexe B)
- 🔗 **Hybride** — l'architecture VB.NET / C# comme compétence centrale (Module 10)
- 🤖 **IA-first** — un module complet sur le développement assisté, indispensable en VB.NET
- 🛠️ **Pragmatique** — maintenance de *legacy*, migration VB6 / .NET Framework, déploiement desktop réel
- 🧩 **Stratégique** — stratégie Microsoft, roadmap .NET, quand rester en VB et quand migrer vers C#

---

## 🎯 Pour qui ?

Cette formation s'adresse à différents profils. Choisissez votre parcours :

| 👤 Profil | 📚 Modules recommandés | ⏱️ Durée estimée |
|-----------|------------------------|------------------|
| **Débutant** | 1-2, 5, 7, 12 | 5-7 jours |
| **Développeur Desktop** (cœur VB) ⭐ | 1-7, 9, 12-13, 15-16 | 10-12 jours |
| **Développeur Données / LOB** | 1-3, 7-8, 12-13, 16 | 8-10 jours |
| **Maintenance & Migration Legacy** | 1-3, 7, 9-11, 17 | 7-9 jours |
| **Web API / Services** (réaliste) | 1-4, 7-8, 12-13, 15-16 | 9-11 jours |
| **Architecte / Hybride VB-C#** 🔗 | 1-4, 9-11, 14-16, 18 + Annexe B | 12-15 jours |
| **IA-First** 🤖 | 1-2, 7, 17, 18 | 3-4 jours |
| **Formation complète** | 1-18 + annexes | 22-28 jours |

---

## 📚 Contenu de la formation

### Les 7 parties

| # | Partie | Modules | Niveau | Sujets clés |
|---|--------|---------|--------|-------------|
| **1** | Comprendre VB.NET en 2026 (cadrage & langage) | 1-4 | 🌱 Débutant | Positionnement 2026, types, POO, événements (`WithEvents`), async |
| **2** | Applications de bureau | 5-6 | 🌿 Intermédiaire | Windows Forms ⭐, WPF, XAML, MVVM, data binding |
| **3** | Données et persistance | 7 | 🌿 Intermédiaire | ADO.NET, EF Core 10, fournisseurs, flux (`Stream`), sérialisation |
| **4** | Services, web et temps réel | 8 | 🌿 Intermédiaire | Consommer des API REST, Web API par contrôleurs, SignalR |
| **5** | Interopérabilité, hybride et migration | 9-11 | 🌳 Avancé | P/Invoke, COM/Office, **hybride VB/C#** 🔗, migration VB6 / Framework |
| **6** | Qualité, performance et exploitation | 12-16 | 🌳 Avancé | Exceptions, tests, performance, déploiement desktop, sécurité |
| **7** | IA et avenir | 17-18 | 🌳 Avancé | **Développement assisté par IA** 🤖, stratégie, roadmap, ressources |

### Les modules

1. Introduction : VB.NET et l'écosystème .NET 10
2. Fondamentaux du langage
3. Programmation orientée objet
4. Programmation asynchrone et parallèle
5. **Windows Forms — le scénario phare** ⭐
6. WPF (Windows Presentation Foundation)
7. Accès aux données (ADO.NET, EF Core 10)
8. Consommer et exposer des services (Web API par contrôleurs)
9. Interopérabilité (P/Invoke, COM/Office) 🔗
10. **Architecture hybride VB.NET / C#** 🔗
11. Migration et maintenance du code legacy
12. Exceptions, débogage et journalisation
13. Tests et qualité du code
14. Performance et gestion de la mémoire
15. Déploiement et DevOps
16. Sécurité des applications
17. **Développer en VB.NET avec l'IA (l'ère Copilot)** 🤖
18. Stratégie, feuille de route et ressources

### 🔥 Nouveautés .NET 10 LTS couvertes (et réellement accessibles en VB.NET)

- 📊 **Entity Framework Core 10** — `LeftJoin` natif, *named query filters*, colonnes JSON, types complexes
- 🌐 **ASP.NET Core** (Web API par contrôleurs) — OpenAPI 3.1, rate limiting intégré, *Problem Details*
- ⚡ **Performance** — gains JIT / PGO / SIMD obtenus **gratuitement** via le runtime, sans changer le code
- 🎨 **UI desktop** — WPF Fluent Design & thèmes ; WinForms (presse-papiers JSON, mode sombre intégré, formulaires async, `Form.ScreenCaptureMode`)
- ⚠️ **Délégué à C#** — Blazor, MAUI, Minimal APIs, Native AOT → voir **Annexe B « Frontière VB.NET / C# »**

### 📎 Les 8 annexes

- **A.** Correspondance syntaxique VB.NET ↔ C# (aide-mémoire)
- **B.** **Frontière VB.NET / C#** : ce qu'on délègue à C# et pourquoi 🔗
- **C.** Bonnes pratiques de codage VB.NET (+ avec l'IA 🤖)
- **D.** Raccourcis et astuces Visual Studio 2026 🆕
- **E.** Guide de migration vers .NET 10 LTS 🆕
- **F.** Glossaire et acronymes
- **G.** FAQ et dépannage (+ pièges IA / VB.NET 🤖)
- **H.** Versions .NET et cycle de support

📖 **Sommaire détaillé** → [SOMMAIRE.md](/SOMMAIRE.md)

---

## 🚀 Démarrage rapide

### Installation .NET 10 et Visual Studio 2026

```powershell
# Vérifier la version .NET installée
dotnet --version

# Installer .NET 10 LTS
# Windows : https://dotnet.microsoft.com/download/dotnet/10.0
# macOS   : brew install dotnet   (la formule principale installe .NET 10)
# Linux   : https://learn.microsoft.com/dotnet/core/install/linux

# Installer Visual Studio 2026 Community (gratuit)
# https://visualstudio.microsoft.com/downloads/
# ✅ Sélectionner la charge de travail : ".NET desktop development"
```

### Créer votre premier projet VB.NET

```powershell
# Application console
dotnet new console -lang VB -n MonPremierProjet
cd MonPremierProjet
dotnet run

# Windows Forms (scénario cœur de VB.NET)
dotnet new winforms -lang VB -n MonAppWinForms

# Bibliothèque de classes (réutilisable depuis VB et C#)
dotnet new classlib -lang VB -n MaBibliotheque

# ⚠️ Pas de modèle « webapi » en VB : la Web API par contrôleurs
#    (l'approche réaliste en VB.NET) se met en place à la main — voir module 8.

# ✅ Vous êtes prêt !
```

> 💡 Sur la grosse vingtaine de modèles de projet du SDK .NET 10, **douze seulement** existent en
> VB.NET — cinq familles : Console, bibliothèque de classes, Windows Forms, WPF et projets de
> test. C'est exactement le terrain couvert par cette formation.

### Cloner cette formation

```bash
git clone https://github.com/NDXDeveloper/formation-vbnet-lts.git
cd formation-vbnet-lts
```

---

## 📁 Structure du projet

```
formation-vbnet-lts/
│
├── 📄 README.md                      # Ce fichier
├── 📄 SOMMAIRE.md                    # Table des matières complète (source de vérité)
├── 📄 LICENSE                        # Licence CC BY-NC-SA 4.0
│
├── 📂 01-introduction-vbnet/         # Positionnement 2026, écosystème .NET 10
├── 📂 02-fondamentaux-langage/       # Types, LINQ, espace My…
├── 📂 03-poo/                        # Classes, héritage, événements (WithEvents)
├── 📂 04-async/                      # async/await, TPL, flux asynchrones
├── 📂 05-windows-forms/              # ⭐ Le scénario phare
├── 📂 06-wpf/                        # XAML, MVVM, Fluent
├── 📂 07-acces-donnees/              # ADO.NET, EF Core 10, flux (Stream)
├── 📂 08-services-web/               # Consommer / exposer des Web API (contrôleurs)
├── 📂 09-interoperabilite/           # 🔗 P/Invoke, COM/Office, WebView2
├── 📂 10-hybride-vbnet-csharp/       # 🔗 Architecture hybride VB.NET / C#
├── 📂 11-migration-legacy/           # VB6 → VB.NET, Framework → .NET 10, Web Forms
├── 📂 12-exceptions-debogage/        # Exceptions, débogage, journalisation
├── 📂 13-tests-qualite/              # xUnit/NUnit/MSTest, Moq, analyseurs
├── 📂 14-performance/                # Profilage, GC, Span, apports .NET 10
├── 📂 15-deploiement-devops/         # ClickOnce/MSIX, CI/CD, Docker
├── 📂 16-securite/                   # Auth, cryptographie, OWASP
├── 📂 17-developpement-ia/           # 🤖 Développer avec l'IA (l'ère Copilot)
├── 📂 18-strategie-roadmap/          # Stratégie Microsoft, roadmap, ressources
│
└── 📂 annexes/
    ├── correspondance-vbnet-csharp/  # A. Aide-mémoire VB ↔ C#
    ├── frontiere-vbnet-csharp/       # B. 🔗 Frontière VB/C#
    ├── bonnes-pratiques/             # C.
    ├── visual-studio-2026/           # D.
    ├── migration-net10/              # E.
    ├── glossaire/                    # F.
    ├── faq-depannage/                # G.
    └── versions-reference/           # H.
```

> Chaque dossier contient un `README.md` (sommaire du module) et un fichier `.md` par section,
> reliés entre eux par une navigation 🔝 Sommaire / ⏭️ section suivante.

---

## 🗓️ Parcours d'apprentissage suggéré

```
🌱 DÉBUTANT
│
├─ Partie 1 : Comprendre VB.NET en 2026 (cadrage & langage)
└─ Partie 2 : Applications de bureau (Windows Forms, WPF)
   │
   ▼
🌿 INTERMÉDIAIRE
│
├─ Partie 3 : Données et persistance (ADO.NET, EF Core 10)
└─ Partie 4 : Services, web et temps réel (Web API par contrôleurs)
   │
   ▼
🌳 AVANCÉ
│
├─ Partie 5 : Interopérabilité, hybride VB/C# et migration
├─ Partie 6 : Qualité, performance et exploitation
└─ Partie 7 : IA et avenir (développement assisté, stratégie, roadmap)

🎓 Total : ~30-40 h de lecture (22-28 jours avec ateliers, à 30 min-1 h/jour)
```

**🎯 Parcours Express (3-4 jours)** pour les pressés :
- Module 1 — Introduction et positionnement 2026
- Module 5 — Windows Forms
- Module 7 — Accès aux données
- Module 10 — Architecture hybride VB.NET / C#
- Module 17 — Développer avec l'IA

---

## 🔗 Ressources officielles

| Ressource | Lien |
|-----------|------|
| 📖 Documentation .NET | [learn.microsoft.com/dotnet](https://learn.microsoft.com/dotnet) |
| 📖 Référence du langage VB.NET | [learn.microsoft.com/dotnet/visual-basic](https://learn.microsoft.com/dotnet/visual-basic) |
| 🧭 Stratégie du langage Visual Basic | [learn.microsoft.com/dotnet/visual-basic/getting-started/strategy](https://learn.microsoft.com/dotnet/visual-basic/getting-started/strategy) |
| 🏠 .NET Foundation | [dotnetfoundation.org](https://dotnetfoundation.org) |
| 🤖 GitHub Copilot | [github.com/features/copilot](https://github.com/features/copilot) |
| ⚙️ Visual Studio 2026 | [visualstudio.microsoft.com](https://visualstudio.microsoft.com) |
| 📥 Télécharger .NET | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| 💬 Forum Q&A | [learn.microsoft.com/answers](https://learn.microsoft.com/answers) |

---

## ❓ FAQ

**Q : VB.NET est-il toujours supporté en 2026 ?**
> Oui — mais c'est un **langage stabilisé** (*feature complete*). Il est maintenu et pleinement supporté
> sur **.NET 10 LTS** (support jusqu'en novembre 2028), sans **nouvelles fonctionnalités de langage**
> (figé à VB 16.9). Détails au Module 18 et en Annexe B.

**Q : Quelles sont les limitations de VB.NET vs C# ?**
> L'**Annexe B « Frontière VB.NET / C# »** les liste (Blazor, MAUI, Minimal APIs, Native AOT, records,
> source generators…). La solution pragmatique est l'**architecture hybride** du **Module 10**.

**Q : Puis-je mixer VB.NET et C# dans un même projet ?**
> Oui, et c'est même recommandé pour les briques modernes : **Module 10** explique comment isoler les
> features avancées dans des bibliothèques C# et les consommer de façon transparente depuis VB.NET.

**Q : Et le web (Blazor, MAUI, Minimal APIs) en VB.NET ?**
> Ce sont des technologies **C# uniquement** (ou sans support VB réaliste). En VB.NET, on reste sur les
> **Web API par contrôleurs** (Module 8) ; le reste se délègue à C# (Annexe B).

**Q : J'ai du code legacy (VB6, .NET Framework, Web Forms). Par où commencer ?**
> Module 11. ⚠️ Cas particulier : **ASP.NET Web Forms n'a pas de chemin vers .NET moderne** — on reste
> sur .NET Framework ou on réécrit (MVC/Blazor en C#, ou Web API VB + front séparé).

**Q : La formation couvre-t-elle le développement avec l'IA ?**
> Oui — **Module 17** complet : prompting efficace pour du VB.NET (et non du C# converti), migration
> assistée, génération de tests, et les pièges (les modèles sont surtout entraînés sur du C#).

**Q : Cette formation remplace-t-elle la documentation officielle ?**
> Non, elle la complète. C'est un guide d'apprentissage progressif, pas une référence exhaustive.

**Q : Puis-je utiliser ce contenu pour enseigner ?**
> Oui, sous licence CC BY-NC-SA 4.0 — attribution requise, usage non commercial, partage identique.

---

## 📝 Licence

Ce projet est sous licence **Creative Commons Attribution - Pas d'Utilisation Commerciale - Partage dans les Mêmes Conditions 4.0 International (CC BY-NC-SA 4.0)**.

✅ **Vous pouvez :**
- Partager — copier et redistribuer le matériel
- Adapter — remixer, transformer et créer à partir du matériel

⚠️ **Selon les conditions suivantes :**
- **Attribution** — vous devez créditer l'œuvre originale
- **Pas d'Utilisation Commerciale** — pas d'usage à des fins commerciales
- **Partage dans les Mêmes Conditions** — toute redistribution sous la même licence

📄 Voir le fichier [LICENSE](/LICENSE) pour les détails complets.

**Attribution suggérée :**
```
Formation VB.NET avec .NET 10 LTS par Nicolas DEOUX
https://github.com/NDXDeveloper/formation-vbnet-lts
Licence CC BY-NC-SA 4.0
```

---

## 👨‍💻 Contact

**Nicolas DEOUX**
- 📧 [NDXDev@gmail.com](mailto:NDXDev@gmail.com)
- 💼 [LinkedIn](https://www.linkedin.com/in/nicolas-deoux-ab295980/)
- 🐙 [GitHub](https://github.com/NDXDeveloper)

---

## 🙏 Remerciements

Merci à :
- **Microsoft** et la **.NET Foundation** pour .NET et Visual Studio
- La **communauté VB.NET** qui continue de faire vivre le langage
- Les contributeurs **open source** qui rendent ces outils accessibles
- **Anthropic**, **OpenAI** et les créateurs d'outils IA qui transforment le développement
- **Vous** qui prenez le temps d'apprendre avec cette formation

---

<div align="center">

## 💻 Bon apprentissage avec VB.NET ! 💻

*Cette formation est un travail en cours. Elle n'est pas parfaite, mais j'espère sincèrement qu'elle vous sera utile dans votre parcours d'apprentissage.*

**[📖 Consulter le sommaire complet →](/SOMMAIRE.md)**

[![Star on GitHub](https://img.shields.io/github/stars/NDXDeveloper/formation-vbnet-lts?style=social)](https://github.com/NDXDeveloper/formation-vbnet-lts)
[![Follow](https://img.shields.io/github/followers/NDXDeveloper?style=social)](https://github.com/NDXDeveloper)

**[⬆ Retour en haut](#-formation-visual-basic-net-avec-net-10-lts)**

*Dernière mise à jour : Juin 2026*

</div>
