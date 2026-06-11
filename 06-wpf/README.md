🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6. WPF (Windows Presentation Foundation)

**Concevoir des interfaces de bureau riches, vectorielles et pilotées par les données**

Windows Presentation Foundation (WPF) est le second grand framework d'interface de bureau de l'écosystème .NET, aux côtés de Windows Forms (module 5). Introduit en 2006 avec .NET Framework 3.0, il repose sur une approche radicalement différente : une interface décrite de façon **déclarative en XAML**, un moteur de rendu **vectoriel** accéléré par DirectX, et surtout un **système de liaison de données** parmi les plus puissants de toutes les plateformes d'UI. Sur .NET 10, WPF est pleinement pris en charge et modernisé (thème Fluent, mode sombre).

Là où Windows Forms enveloppe les contrôles Win32 historiques, WPF sépare nettement l'apparence de la logique : on compose l'interface en XAML, on la stylise via des ressources et des *templates* réutilisables, et on la relie aux données par *binding* — au point qu'une vue WPF bien conçue ne contient quasiment aucun code-behind. C'est cette philosophie qui rend naturelle l'architecture **MVVM**, abordée en 6.6.

---

## 🧭 WPF dans l'univers VB.NET — positionnement honnête

Bonne nouvelle d'emblée : **WPF est un scénario de premier rang en VB.NET**. C'est l'une des cinq familles de modèles de projet disponibles pour le langage (Console, bibliothèque de classes, Windows Forms, **WPF**, projets de test). Contrairement à Blazor, MAUI ou WinUI 3 — dont le code d'interface est *C# uniquement* (voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)) —, **on peut écrire une application WPF complète en VB.NET**, du XAML au *view-model*.

Cela dit, restons lucides sur deux points :

- **Windows Forms demeure le scénario phare ⭐ de VB.NET.** C'est l'outil le plus répandu pour les applications de bureau VB, le plus rapide à prendre en main et le mieux servi par l'espace `My`. WPF se justifie lorsqu'on a besoin de ses forces propres : interfaces hautement personnalisées, indépendance de résolution, *data binding* riche et MVVM, styles et animations avancés. Le choix entre les deux est l'objet de la section [6.1](01-introduction-wpf-vs-winforms.md).
- **L'outillage MVVM moderne est partiellement hors de portée 🔗.** Les *source generators* de `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]`) sont **C# uniquement**. En VB.NET, on utilise directement les classes de base de la bibliothèque (`ObservableObject`, `RelayCommand`), ce qui fonctionne parfaitement mais reste un peu plus verbeux. Ce point est détaillé en [6.6](06-mvvm.md).

En résumé : WPF en VB.NET, **oui** ✅ — avec une UI déclarative et un *binding* qui compensent largement l'absence de quelques raccourcis de génération de code.

---

## 🎯 Objectifs du chapitre

À l'issue de ce chapitre, vous saurez :

- choisir en connaissance de cause entre WPF et Windows Forms selon le projet ;
- structurer une interface en **XAML** à l'aide des principaux systèmes de disposition ;
- mettre en œuvre le **data binding** WPF (`OneWay`/`TwoWay`, `INotifyPropertyChanged`, `ObservableCollection`, convertisseurs, validation) ;
- styliser et « thématiser » une application via ressources, styles, *templates* et *triggers* ;
- appliquer l'architecture **MVVM** en VB.NET, y compris l'implémentation des commandes ;
- exploiter le thème **Fluent / mode sombre** de .NET 10 ;
- diagnostiquer et corriger les principaux écueils de performance.

---

## 📑 Plan du chapitre

- **6.1 — [Introduction ; WPF vs Windows Forms (lequel choisir)](01-introduction-wpf-vs-winforms.md)**  
  Architecture de WPF, différences fondamentales avec WinForms, et grille de décision.
- **6.2 — [XAML fondamentaux et systèmes de layout](02-xaml-layout.md)**  
  Syntaxe XAML, arbres visuel et logique, dispositions `Grid`, `StackPanel`, `DockPanel`.
- **6.3 — [Contrôles et contrôles de données](03-controles.md)**  
  Contrôles courants et présentation de collections avec `DataGrid` et `ListView`.
- **6.4 — [Liaison de données](04-data-binding.md)**  
  Le cœur de WPF : modes de *binding*, `INotifyPropertyChanged`, `ObservableCollection`, convertisseurs et validation.
- **6.5 — [Styles, ressources, templates et triggers](05-styles-templates.md)**  
  Séparer le fond de la forme : dictionnaires de ressources, styles, *control templates* et déclencheurs.
- **6.6 — [Architecture MVVM](06-mvvm.md)** 🔗  
  Principes du patron, `ICommand` / `RelayCommand`, et l'usage de `CommunityToolkit.Mvvm` côté VB (sans générateurs).
- **6.7 — [Animations et multimédia (notions)](07-animations-multimedia.md)**  
  Storyboards, animations déclaratives et lecture de médias — vue d'ensemble.
- **6.8 — [Thèmes et Fluent Design (.NET 10)](08-fluent-design-net10.md)** 🆕  
  Thème Fluent intégré, modes clair / sombre / système et couleurs d'accentuation.
- **6.9 — [Performance WPF](09-performance.md)**  
  Virtualisation des listes, bonnes pratiques de *binding* et coût du rendu.

---

## 🧩 Prérequis

Ce chapitre suppose acquis :

- les **fondamentaux du langage** (modules [1](../01-introduction-vbnet/README.md) et [2](../02-fondamentaux-langage/README.md)) et la **POO** (module [3](../03-poo/README.md)) ;
- en particulier les **événements et délégués** ([3.6](../03-poo/06-evenements-delegues.md)) — essentiels pour comprendre les commandes et le *binding* ;
- la **programmation asynchrone** (module [4](../04-async/README.md)), indispensable pour garder une UI réactive ;
- une familiarité avec **Windows Forms** (module [5](../05-windows-forms/README.md)) est utile pour la comparaison, sans être obligatoire.

> 💡 Le XAML et la documentation WPF regorgent d'exemples **en C#**. Gardez l'[Annexe A — Correspondance VB.NET ↔ C#](../annexes/correspondance-vbnet-csharp/README.md) à portée de main pour les transposer, et appuyez-vous sur l'IA (module [17](../17-developpement-ia/README.md)) en pensant à toujours préciser « Visual Basic .NET ».

---

## ⚠️ Points clés à garder en tête

- **WPF est Windows uniquement.** Comme WinForms, il ne cible ni Linux ni macOS. Pour du multiplateforme, le terrain bascule hors VB (voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).
- **Une courbe d'apprentissage réelle.** XAML, *binding*, ressources et MVVM forment un ensemble cohérent mais plus exigeant au départ que le glisser-déposer de WinForms. L'investissement paie sur les interfaces complexes et durables.
- **MVVM en VB : les générateurs de code en moins, pas le patron.** On écrit propriétés et commandes un peu plus explicitement ; l'architecture, elle, reste pleinement accessible.
- **.NET 10 modernise WPF sans réécriture.** Thème Fluent et mode sombre s'ajoutent à l'existant ; le code WPF historique continue de fonctionner.

---

## 🧭 Quand suivre ce chapitre

WPF s'adresse avant tout au **parcours Développeur Desktop** ⭐. Si votre objectif est de livrer rapidement des applications de gestion classiques, Windows Forms (module 5) suffira souvent ; passez à WPF dès que vous visez des interfaces sur mesure, fortement liées aux données ou destinées à durer. Pour le déploiement de vos applications de bureau, rendez-vous au [module 15](../15-deploiement-devops/README.md).

⏭️ [Introduction ; WPF vs Windows Forms (lequel choisir)](/06-wpf/01-introduction-wpf-vs-winforms.md)
