🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5. Windows Forms — le scénario phare ⭐ 🔄

Avec ce module, nous entrons dans la **Partie 2 — Applications de bureau**, c'est-à-dire dans le cœur même de Visual Basic .NET. S'il fallait ne retenir qu'un seul domaine où VB.NET reste aujourd'hui pleinement pertinent, productif et soutenu, ce serait celui-ci : la création d'applications de bureau Windows avec **Windows Forms** (souvent abrégé *WinForms*).

Ce n'est ni de la nostalgie, ni un choix par défaut. C'est la conséquence directe de la stratégie de langage posée au module 1.

> La position officielle de Microsoft est de continuer à investir dans « les scénarios cœur de Visual Basic comme **Windows Forms** et les **bibliothèques** ». Développer une application WinForms en VB.NET, c'est donc se placer exactement là où le langage est documenté, outillé et pérenne — et non sur un terrain en sursis.

## Pourquoi Windows Forms est le scénario phare

Plusieurs raisons font de WinForms le terrain de prédilection de VB.NET :

- **Le développement visuel (RAD).** Le Concepteur (*Designer*) de Visual Studio permet de construire une interface par glisser-déposer, avec une boucle « dessiner → double-cliquer → coder l'événement » d'une productivité immédiate. C'est l'expérience qui a fait le succès historique de Visual Basic, et elle reste excellente.
- **Un modèle événementiel idiomatique.** Les mots-clés `Handles` et `WithEvents`, vus au module [3.6](../03-poo/06-evenements-delegues.md), rendent le câblage des événements particulièrement lisible en VB.NET — souvent plus qu'en C#.
- **Une liaison de données mature.** Pour les applications de gestion (*LOB*, *Line of Business*) — formulaires de saisie, grilles, états — l'écosystème `BindingSource` / `BindingList` est éprouvé et productif.
- **L'espace `My` bien pris en charge.** À la différence de WPF, l'espace `My` (module [2.12](../02-fondamentaux-langage/12-espace-my.md)) fonctionne correctement en WinForms : `My.Settings`, `My.Resources` et `My.Computer` y sont des raccourcis appréciables.

## Ce que .NET 10 change pour Windows Forms

Loin d'être figée, la plateforme Windows Forms a été **modernisée** sur .NET moderne, et .NET 10 LTS apporte des nouveautés concrètes :

- un **mode sombre intégré** ;
- des **formulaires asynchrones** (`ShowAsync` / `ShowDialogAsync`) ;
- un **presse-papiers sécurisé** et un échange de données au format JSON ;
- la propriété **`Form.FormScreenCaptureMode`** pour restreindre la capture d'écran ;
- le portage d'éditeurs du Concepteur hérités de .NET Framework.

Ces apports sont détaillés dans les sections [5.2](02-winforms-net10.md) et [5.12](12-nouveautes-net10.md). Retenez dès maintenant l'essentiel : WinForms n'est pas seulement maintenu, il continue d'évoluer.

> ⚠️ **Note de périmètre.** Windows Forms est une technologie **Windows uniquement**. Pour une interface multiplateforme, ce n'est pas le bon outil (voir l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)). Pour une alternative de bureau plus riche graphiquement, voir le module [6 — WPF](../06-wpf/README.md).

## Objectifs du module

À l'issue de ce module, vous serez capable de :

- concevoir une interface avec le Concepteur et en comprendre l'architecture sous-jacente ;
- mettre en œuvre les contrôles fondamentaux et avancés (dont `DataGridView`, `TreeView`, `MenuStrip`…) ;
- créer vos propres contrôles réutilisables (`UserControl`) ;
- gérer le cycle de vie d'un formulaire et ses événements (souris, clavier) ;
- valider les saisies utilisateur et lier vos formulaires à des données ;
- structurer une application multi-formulaires ou MDI ;
- gérer les préférences et l'internationalisation, et tirer parti des nouveautés .NET 10.

## Prérequis

Ce module s'appuie sur les fondations posées en Partie 1. Avant de commencer, il est recommandé de maîtriser :

- les **fondamentaux du langage** (modules 1 et 2), en particulier les collections et l'espace `My` ([2.12](../02-fondamentaux-langage/12-espace-my.md)) ;
- la **programmation orientée objet** (module 3), tout spécialement les **événements et délégués** ([3.6](../03-poo/06-evenements-delegues.md)), au cœur du modèle WinForms ;
- les bases de l'**asynchronisme** (module 4) pour garder une interface réactive (`Async` / `Await`).

## Plan du module

- 5.1 — [Introduction, architecture et le Concepteur (*Designer*)](01-introduction-designer.md)
- 5.2 — [Windows Forms sur .NET 10](02-winforms-net10.md) 🆕 — *mode sombre, formulaires async, presse-papiers sécurisé*
- 5.3 — [Contrôles fondamentaux](03-controles-fondamentaux.md) — *`Form`, `Button`, `TextBox`, conteneurs, boîtes de dialogue*
- 5.4 — [Contrôles avancés](04-controles-avances.md) — *`DataGridView`, `TreeView`/`ListView`, `MenuStrip`/`ToolStrip`/`StatusStrip`*
- 5.5 — [Contrôles personnalisés et `UserControl`](05-controles-personnalises.md)
- 5.6 — [Gestion des événements](06-evenements.md) — *souris, clavier, cycle de vie du formulaire*
- 5.7 — [Validation](07-validation.md) — *`ErrorProvider`, `DataAnnotations`, règles personnalisées*
- 5.8 — [Liaison de données WinForms](08-data-binding.md) — *`BindingSource`, `BindingList`, liaison à une BDD*
- 5.9 — [Applications MDI et multi-formulaires](09-mdi.md)
- 5.10 — [Préférences et paramètres utilisateur](10-preferences.md) — *`My.Settings`*
- 5.11 — [Internationalisation](11-internationalisation.md) — *i18n/l10n, ressources `.resx`*
- 5.12 — [Nouveautés Windows Forms .NET 10](12-nouveautes-net10.md) 🆕
- 5.13 — Déploiement → voir le module [15 — Déploiement et DevOps](../15-deploiement-devops/README.md)

---

Commençons par les fondations : l'architecture d'une application Windows Forms et l'outil central de ce module, le Concepteur de Visual Studio → [5.1 — Introduction, architecture et le Concepteur](01-introduction-designer.md).

⏭️ [Introduction, architecture et le Concepteur (Designer)](/05-windows-forms/01-introduction-designer.md)
