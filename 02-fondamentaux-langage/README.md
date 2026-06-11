🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2. Fondamentaux du langage

> Le socle stable de VB.NET : la grammaire, les types et les constructions que vous
> retrouverez dans **chaque** application — et qui, le langage étant figé, ne changeront plus.

---

## Introduction

Ce module couvre les briques de base du langage : la façon dont un programme est structuré,
les types de données, les opérateurs, les structures de contrôle, les chaînes, les collections,
LINQ, les génériques et la portée des membres. C'est le chapitre le plus *fondamental* de la
formation — celui sur lequel reposent tous les suivants, des Windows Forms à l'accès aux données.

En 2026, apprendre les fondamentaux de VB.NET présente un avantage particulier : **le langage est
stabilisé** (figé à la version 16.9, approche *consumption-only*, voir le
[module 1.6](../01-introduction-vbnet/06-positionnement-2026.md)). Il n'y a pas de nouvelle syntaxe
à courir après, pas de *churn* à absorber. Ce que vous lisez ici reste valide pour toute la durée de
vos projets. Cette stabilité n'est pas une limite quand il s'agit des bases : c'est une garantie.

Les fondamentaux sont aussi le terrain où VB.NET et C# se ressemblent le plus. Même CLR, même
bibliothèque de classes (BCL), mêmes sémantiques de types : seule la syntaxe diffère. C'est donc la
zone où la **conversion C# → VB** — et l'assistance par IA — est la plus fiable. Quelques constructions
restent néanmoins **idiomatiques de VB.NET** et méritent une attention particulière : la syntaxe de
requête **LINQ** (section 2.9) et l'**espace `My`** (section 2.12). Le module signale aussi, sans détour,
les points où VB est plus limité que C# (filtrage de motifs, types référence nullables).

## Objectifs du module

À l'issue de ce module, vous saurez :

- structurer un programme VB.NET et choisir les bons réglages `Option` (en particulier pourquoi
  `Option Strict On` doit être un réflexe) ;
- manipuler correctement types valeur et référence, types nullables, constantes et énumérations ;
- maîtriser les opérateurs propres à VB (`&`, `Mod`, `Like`, `Is`/`IsNot`, `AndAlso`/`OrElse`) et les
  structures de contrôle (conditions, boucles) ;
- traiter chaînes, dates et nombres en tenant compte de la **culture** (un piège classique) ;
- utiliser tableaux, collections génériques et **LINQ** de façon idiomatique ;
- comprendre les génériques avancés, la portée et les modificateurs d'accès ;
- savoir quand l'espace `My` fait gagner du temps — et où sont ses limites sur .NET moderne.

## Un détail qui change tout : `Option Strict On`

Pour des raisons historiques (héritage de VB6), VB.NET autorise par défaut des conversions de types
implicites et une liaison tardive permissive. Activer **`Option Strict On`** rétablit une vérification
de types stricte à la compilation et élimine une large classe d'erreurs silencieuses. C'est, de loin,
la décision de configuration la plus importante de tout le module — la formation l'adopte par défaut
dans l'ensemble de ses exemples. La section 2.1 explique pourquoi, et comment l'imposer au niveau du
projet plutôt que fichier par fichier.

## Contenu du module

- **2.1 — [Structure d'un programme ; `Option Strict` / `Explicit` / `Infer` / `Compare`](01-structure-options.md)**
  L'anatomie d'un fichier `.vb`, les procédures (`Sub`/`Function` et leurs paramètres) et les quatre
  directives `Option` qui gouvernent la rigueur du compilateur. `Option Compare` conditionne notamment
  le comportement des comparaisons de chaînes et de l'opérateur `Like`.
- **2.2 — [Types de données et variables](02-types-variables.md)**
  Types valeur vs référence, primitifs, inférence de type, constantes et énumérations. On y clarifie
  un point souvent confondu : les **types nullables de valeur** (`Nullable(Of T)` / `T?`) n'ont rien à
  voir avec les *nullable reference types* de C#, absents de VB. ⚠️
- **2.3 — [Opérateurs et expressions](03-operateurs.md)**
  Les opérateurs idiomatiques de VB : concaténation `&`, modulo `Mod`, correspondance de motifs `Like`,
  égalité de référence `Is`/`IsNot`, et opérateurs logiques à court-circuit `AndAlso`/`OrElse`.
- **2.4 — [Structures conditionnelles](04-conditions.md)**
  `If…Then…Else`, opérateur ternaire `If()`, `Select Case`, et tests de type `TypeOf…Is`. On y note
  honnêtement que le **filtrage de motifs de VB est plus limité que celui de C#**. ⚠️
- **2.5 — [Boucles et itérations](05-boucles.md)**
  `For…Next`, `For Each`, `Do…Loop` et `While` : quand utiliser quoi.
- **2.6 — [Chaînes et manipulation de texte](06-chaines.md)**
  Immuabilité de `String`, performance avec `StringBuilder`, et interpolation `$"…"`.
- **2.7 — [Dates, nombres, formatage et culture](07-dates-nombres-culture.md)**
  Formatage et analyse en tenant compte de la `CultureInfo` — l'une des sources de bugs les plus
  fréquentes en production.
- **2.8 — [Tableaux et collections](08-tableaux-collections.md)**
  Tableaux uni- et multidimensionnels, génériques (`List(Of T)`, `Dictionary(Of TKey, TValue)`) et
  collections spécialisées (`ObservableCollection`, collections concurrentes).
- **2.9 — [LINQ — un point fort de VB.NET](09-linq.md)** ⭐
  LINQ to Objects, syntaxe requête (`From…Where…Select`) et méthodes d'extension. La syntaxe de requête
  de VB est particulièrement expressive — un atout réel du langage.
- **2.10 — [Génériques avancés](10-generiques-avances.md)**
  Contraintes, méthodes génériques et variance (covariance/contravariance avec `In`/`Out`).
- **2.11 — [Portée, visibilité et modificateurs d'accès](11-portee-visibilite.md)**
  `Public`, `Private`, `Protected` et `Friend` (l'équivalent VB de l'`internal` de C#).
- **2.12 — [L'espace `My` — un raccourci propre à VB.NET](12-espace-my.md)** ⭐ ⚠️
  `My.Application`, `My.Computer`, `My.Settings`, `My.Resources`, `My.User` : un gain de productivité
  réel, mais au **support partiel sur .NET moderne** (correct en Windows Forms, limité en WPF, membres
  web supprimés). À utiliser en connaissance de cause.

## Points forts et limites à garder en tête

Deux sections de ce module mettent en avant des **forces idiomatiques** de VB.NET : LINQ (2.9), dont la
syntaxe de requête est plus lisible que son équivalent C# dans bien des cas, et l'espace `My` (2.12), qui
n'a pas d'équivalent direct ailleurs.

À l'inverse, le module signale honnêtement trois **limites** par rapport à C#, conformément à l'esprit de
la formation : le filtrage de motifs reste rudimentaire (2.4), VB ne dispose pas des *nullable reference
types* (2.2), et le support de l'espace `My` est inégal sur .NET moderne (2.12). Ces points ne sont pas
des défauts à masquer mais des frontières à connaître — et, le cas échéant, à franchir via l'architecture
hybride (voir [module 10](../10-hybride-vbnet-csharp/README.md) et
[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

## Et l'IA dans tout ça ? 🤖

Les fondamentaux constituent la zone où les assistants IA sont les **plus fiables** : ces constructions
existent à l'identique en C#, abondamment représenté dans les données d'entraînement, et la traduction
est mécanique. C'est donc un excellent terrain pour pratiquer le workflow « générer en C#, convertir en
VB.NET » présenté au [module 17](../17-developpement-ia/README.md).

Restez toutefois vigilant sur les constructions **idiomatiques** : un modèle peut proposer un `??` au lieu
de `If(x, y)`, un `switch` à la place de `Select Case`, ou ignorer la syntaxe de requête LINQ et l'espace
`My`. La règle d'or — toujours préciser « **Visual Basic .NET** » et la version .NET cible — s'applique dès
ce chapitre.

## Prérequis

- Avoir parcouru le [module 1 — Introduction](../01-introduction-vbnet/README.md), notamment le
  [positionnement de VB.NET en 2026](../01-introduction-vbnet/06-positionnement-2026.md) et l'installation
  des outils.
- Des bases en programmation (variables, fonctions, logique conditionnelle) sont recommandées.

---

⏭️ [Structure d'un programme ; Option Strict / Explicit / Infer / Compare](/02-fondamentaux-langage/01-structure-options.md)
