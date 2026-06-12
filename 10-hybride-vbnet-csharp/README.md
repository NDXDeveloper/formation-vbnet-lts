🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 10. Architecture hybride VB.NET / C# — la stratégie 2026 🔗 ⭐ 🆕

> **Garder VB.NET là où il excelle, déléguer à C# ce qui lui échappe — et faire des deux une seule solution cohérente.**

Ce chapitre est le **prolongement naturel du chapitre 9** et, sans doute, le plus stratégique de toute la formation. La section **9.3** a établi le *mécanisme* : VB, C# et F# partageant le même *runtime*, un projet VB.NET consomme une bibliothèque C# sans la moindre friction. Le présent chapitre transforme ce mécanisme en **architecture** : une façon réfléchie et durable de structurer une application réelle autour de cette complémentarité.

---

## La thèse du chapitre, en quelques lignes

Rappelons le point de départ, posé dès le module 1 : Visual Basic .NET est un langage **stabilisé**. Microsoft le maintient (sécurité, *runtime*, expérience Visual Studio) mais **n'y ajoute plus de nouvelle syntaxe** ; sa stratégie officielle parle d'une approche de **consommation seule** (*consumption-only*). Vu sous un certain angle, c'est une impasse. Vu autrement — celui de ce chapitre — c'est une **invitation à l'architecture** :

> **VB.NET n'a pas besoin de nouvelle syntaxe s'il peut *consommer* tout ce que le .NET moderne produit.**

Dès lors, le gel du langage **cesse de faire mal** pour tout ce que l'on délègue. La réponse pragmatique — argumentée en détail en **10.1** — tient en une phrase :

```
┌──────────────────────────────────────────────────────┐
│  Application VB.NET                                  │
│  • UI (Windows Forms ⭐ / WPF)                        │   ← productivité, idiomes VB ;
│  • logique métier, règles, LINQ, événements          │     le gel du langage n'y nuit pas
│                                                      │
│            │  consomme (référence .NET)              │
│            ▼                                         │
│  Bibliothèque(s) C#                                  │
│  • perf / Span, records, générateurs de source       │   ← fonctionnalités modernes,
│  • Minimal API, Native AOT, front Blazor / MAUI      │     isolées et encapsulées
└──────────────────────────────────────────────────────┘
```

**Cœur en C# (fonctionnalités et performance), UI et métier en VB.NET.** C'est l'expression *architecturale* de la doctrine officielle : VB **consomme**, C# **produit** ce qui relève des nouveautés du langage.

---

## Pourquoi c'est techniquement solide

Cette architecture ne repose sur aucune astuce fragile : elle s'appuie sur les fondations démontrées au chapitre 9.

- Tous les langages compilent vers le **même IL** et s'exécutent sur le **même CLR**, avec un **système de types unifié** (CTS) — la frontière VB ↔ C# n'est qu'une **référence d'assembly ordinaire** (section 9.3).
- Le **CLS** garantit qu'une API publique conçue correctement est consommable depuis VB sans surprise.
- C'est, du reste, ce que tout développeur VB.NET fait déjà chaque jour en installant des paquets **NuGet** majoritairement écrits en C#. L'hybride ne fait que **systématiser et internaliser** cette pratique avec *vos propres* bibliothèques.

---

## Une bonne architecture, pas un pis-aller

Il serait réducteur de voir l'hybride comme une rustine imposée par les circonstances. C'est, en réalité, un cas particulier de **séparation des responsabilités** — une architecture en couches — dont la **frontière coïncide avec les forces de chaque langage** :

- VB.NET pour ce qu'il fait remarquablement : interfaces de bureau, code métier lisible, événements idiomatiques, accès aux données ;
- C# pour ce qui relève des évolutions récentes du langage et de la plateforme.

Cette approche **pérennise l'investissement VB** : votre code d'interface et de métier reste en place, et le jour où une fonctionnalité .NET récente devient nécessaire, on ajoute une bibliothèque C# **sans rien réécrire**. Le gel du langage, au lieu d'être une limite subie, devient une **manière de travailler propre et stable**.

---

## Ce que recouvre concrètement la délégation

Les sujets que l'on confie typiquement à une bibliothèque C# (détaillés en **10.2** et catalogués dans l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**) :

- **performance fine** : `Span(Of T)`, parsing sans allocation, traitements intensifs ;
- **records**, `init`, types immuables avancés ;
- **générateurs de source** (en tant qu'auteur) ;
- **Minimal APIs** ;
- **Native AOT** ;
- **front-ends modernes** : Blazor, .NET MAUI, WinUI 3 ;
- outillage orienté C# : **gRPC**, **GraphQL**.

---

## ⚠️ Quand — et quand ne pas — recourir à l'hybride

L'hybride est un **outil**, pas une obligation. Pour une application de bureau ou de gestion (LOB) entièrement en VB.NET, sans besoin de fonctionnalités de pointe, on peut très bien n'avoir **jamais** à écrire la moindre bibliothèque C#. La question du *quand* — traitée en **10.2** — est donc centrale.

Et la démarche a un coût à ne pas masquer : **deux langages** dans la solution, une **solution mixte** à gérer (build, tests, NuGet — section 10.6), une **charge cognitive** et des **compétences d'équipe** supplémentaires, ainsi qu'une **frontière à concevoir « VB-friendly »** (constructeurs explicites, pas de `ref struct` dans la surface publique, conformité CLS — rappels de la 9.3). On adopte l'hybride **lorsque la délégation est réellement justifiée**, pas par principe.

---

## Objectifs du chapitre

À l'issue de ce chapitre, vous serez en mesure de :

- **comprendre et expliquer** pourquoi l'architecture hybride est la réponse pragmatique au gel du langage ;
- **décider** quels éléments déléguer à C# et lesquels conserver en VB.NET ;
- **isoler** proprement une fonctionnalité avancée dans une bibliothèque C# au contrat bien défini ;
- **consommer** cette bibliothèque de façon transparente depuis VB.NET ;
- **structurer** une solution mixte cohérente (projets, NuGet, build, tests).

---

## Prérequis

- Le **chapitre 9**, en particulier la section **9.3** (interopérabilité entre langages) — socle direct de ce chapitre.
- Une capacité à **lire du C#** : l'**[Annexe A — Correspondance syntaxique VB.NET ↔ C#](../annexes/correspondance-vbnet-csharp/README.md)** est ici une compagne précieuse.
- Des **notions sur les fonctionnalités déléguées** : records (module 3.7), `Span`/`Memory` (module 14.4), async (module 4), selon les besoins.

---

## Plan du chapitre

- **10.1 — Pourquoi l'hybride** : la réponse pragmatique au gel du langage. L'argumentaire complet de la thèse posée ci-dessus.
- **10.2 — Quand déléguer à C#** : les critères de décision (perf/`Span`, records, générateurs de source, Minimal APIs, Native AOT, Blazor/MAUI).
- **10.3 — Isoler les fonctionnalités avancées** dans des bibliothèques C# : concevoir un contrat clair et encapsulé.
- **10.4 — Consommer ces bibliothèques** de façon transparente depuis VB.NET : la frontière vue côté appelant.
- **10.5 — Atelier** : cœur en C# (performance / fonctionnalités), UI et métier en VB.NET — la mise en pratique de bout en bout.
- **10.6 — Gérer une solution mixte** : NuGet, organisation des projets, build et tests d'une solution à deux langages.

---

> 🔗 **À lire en regard** : le **chapitre 9** (le mécanisme), l'**[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)** (le catalogue de ce qu'on délègue) et l'**[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** (lire et transposer le C#).

⏭️ [Pourquoi l'hybride : la réponse pragmatique au gel du langage](/10-hybride-vbnet-csharp/01-pourquoi-hybride.md)
