🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 1. Introduction : VB.NET et l'écosystème .NET 10

Bienvenue dans cette formation. Ce premier module pose les **fondations** — et, plus
important encore, il **cadre les attentes**. Avant d'apprendre à écrire du VB.NET, encore
faut-il savoir ce qu'est VB.NET en 2026, ce qu'il fait réellement bien, et où s'arrête
honnêtement son périmètre.

C'est le parti pris assumé de cette formation : **ne pas vendre de rêve**. Visual Basic
.NET est, depuis 2020, un **langage stabilisé** : Microsoft le maintient (sécurité,
compatibilité *runtime*, expérience Visual Studio) mais **n'y ajoute plus de nouvelle
syntaxe**. Loin d'être une mauvaise nouvelle, c'est une information **structurante** :
elle indique précisément où investir son énergie — applications de bureau, bibliothèques,
accès aux données, interopérabilité, maintenance de legacy — et ce qu'il vaut mieux
**déléguer à C#**.

Ce module installe ce socle : un peu d'histoire pour comprendre l'héritage, une carte de
l'écosystème .NET pour ne plus confondre les plateformes, l'outillage pour démarrer, un
premier projet pas à pas — et une section pivot (la **1.6**) entièrement consacrée au
positionnement honnête de VB.NET en 2026.

> 💡 **Le fil rouge de toute la formation.** On enseigne ce que VB.NET fait **réellement**
> en 2026, pas ce qu'il « pourrait » faire. Quand un sujet se code mieux en C# aujourd'hui,
> on le dit — et on montre comment le **consommer depuis VB.NET** plutôt que de le
> contourner maladroitement. (→ Annexe B)

## 🎯 Objectifs du module

À l'issue de ce module, vous serez capable de :

- expliquer **ce qu'est VB.NET en 2026** et à quoi il sert vraiment — autant que ce pour
  quoi il n'est plus le meilleur choix ;
- situer le langage dans son **héritage**, de Visual Basic 6 à aujourd'hui, et comprendre
  pourquoi tant de code legacy subsiste encore ;
- distinguer le **.NET Framework** (legacy, Windows uniquement) du **.NET moderne**
  (.NET 10 LTS), et savoir lequel cibler ;
- identifier les **briques de l'écosystème** — *runtime*, SDK, NuGet, solution — et ce que
  .NET 10 LTS apporte, y compris « gratuitement » à VB.NET ;
- **installer** un environnement de développement opérationnel (Visual Studio 2026,
  VS Code, CLI `dotnet`) ;
- **créer et exécuter** vos premiers projets (Console puis Windows Forms) ;
- adopter d'emblée la bonne **posture stratégique** : savoir *quand rester en VB.NET*,
  *quand déléguer à C#*, et pourquoi l'IA est devenue décisive pour ce langage.

## 📋 Prérequis

- Des **bases en programmation** (variables, conditions, boucles) sont recommandées, mais
  pas indispensables.
- **Aucune** connaissance préalable de VB.NET ou de .NET n'est requise.
- Un **poste Windows** est conseillé : le cœur de VB.NET (Windows Forms, WPF) y est
  pleinement pris en charge. Le développement reste possible sous Linux/macOS pour la
  console et les bibliothèques, avec quelques limites d'outillage signalées en **1.4**.

## 🧭 Contenu du module

Les six sections s'enchaînent du « pourquoi » vers le « comment », pour se conclure sur la
stratégie :

- **1.1 — [Qu'est-ce que VB.NET et à quoi il sert réellement en 2026](01-quest-ce-que-vbnet.md)**
  La définition sans détour : ses domaines de pertinence réels, et ceux qu'il a cédés à C#.
- **1.2 — [De Visual Basic 6 à VB.NET](02-histoire-evolution.md)**
  L'héritage et la rupture VB6 → VB.NET, pour mesurer l'ampleur du parc legacy encore en
  service.
- **1.3 — [L'écosystème .NET](03-ecosysteme-dotnet.md)**
  .NET Framework (legacy, Windows) vs .NET moderne (.NET 10 LTS) ; *runtime*, SDK, NuGet
  et structure d'une solution.
- **1.4 — [Installation et outils](04-installation-outils.md)** 🆕
  Visual Studio 2026 (IDE « AI-native »), VS Code et la CLI — avec les limites d'outillage
  propres à VB (pas de C# Dev Kit pour VB.NET dans VS Code).
- **1.5 — [Premier projet pas à pas](05-premier-projet.md)**
  Créer, comprendre et exécuter une première application **Console**, puis une application
  **Windows Forms**.
- **1.6 — [VB.NET en 2026 : positionnement honnête](06-positionnement-2026.md)** ⭐ 🔄
  La **section pivot** du module : la stratégie officielle Microsoft (langage
  *consumption-only*), le langage **figé à VB 16.9**, les scénarios cœur vs les *workloads*
  non pris en charge, et trois mises au point décisives — la
  [grille de décision VB/C#](06.1-vbnet-vs-csharp.md), la
  [stratégie hybride en une page](06.2-strategie-hybride.md) 🔗 et
  [l'ère de l'IA](06.3-ere-ia.md) 🤖.

## 👥 Pour quel parcours ?

Ce module est le **tronc commun de tous les parcours**. Quel que soit votre objectif —
débutant, développeur desktop ⭐, données/LOB, maintenance et migration legacy, Web API,
architecte hybride 🔗 ou IA-first 🤖 —, vous commencez ici. C'est le seul module qu'aucun
parcours ne saute : il fixe le vocabulaire, les repères de périmètre et la posture
stratégique sur lesquels tout le reste s'appuie.

## ⏱️ Durée estimée

Comptez environ **2 à 4 heures de lecture** pour l'ensemble du module — davantage si vous
installez l'environnement et reproduisez les premiers projets en suivant la section 1.5.

---

Une fois ce socle en place, vous aborderez les **fondamentaux du langage** (module 2) sur
des bases saines. Mais commençons par la question la plus directe — et la plus honnête :
**qu'est-ce que VB.NET, et à quoi sert-il réellement en 2026 ?** (→ 1.1)

⏭️ [Qu'est-ce que VB.NET et à quoi il sert réellement en 2026](/01-introduction-vbnet/01-quest-ce-que-vbnet.md)
