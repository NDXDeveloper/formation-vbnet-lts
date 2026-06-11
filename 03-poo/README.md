🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3. Programmation orientée objet

Visual Basic .NET est un langage **entièrement orienté objet**. Contrairement à son ancêtre Visual Basic 6 — qui n'offrait qu'une orientation objet partielle (pas d'héritage d'implémentation, notamment) — VB.NET repose intégralement sur le système de types unifié de .NET : **tout dérive de `System.Object`**, des types valeur les plus simples (`Integer`, `Boolean`) aux classes les plus élaborées de vos applications.

Ce chapitre constitue le socle conceptuel de toute la suite de la formation. Les scénarios cœur de VB.NET en 2026 — Windows Forms, WPF, bibliothèques de classes — sont **structurés autour d'objets** : un formulaire est une classe, un contrôle est un objet, une fenêtre WPF expose des propriétés liables, et une bibliothèque réutilisable n'est rien d'autre qu'un ensemble cohérent de types publics. Maîtriser la POO, c'est donc maîtriser la matière première de l'écosystème.

La POO en VB.NET s'articule autour des quatre piliers classiques :

- **Encapsulation** — regrouper données et comportements, et contrôler leur visibilité (`Public`, `Private`, `Protected`, `Friend`).
- **Héritage** — réutiliser et spécialiser du code existant via `Inherits`.
- **Polymorphisme** — substituer des comportements via `Overridable` / `Overrides` et les interfaces.
- **Abstraction** — exposer des contrats (`Interface`, classes `MustInherit`) sans dépendre des implémentations concrètes.

---

## 🎯 Objectifs du chapitre

À l'issue de ce chapitre, vous saurez :

- Concevoir des classes propres (propriétés auto et complètes, constructeurs, `Me`) et choisir à bon escient entre `Class` et `Structure`.
- Mettre en œuvre l'héritage et le polymorphisme avec les mots-clés idiomatiques de VB.NET.
- Définir et implémenter des interfaces, y compris génériques et multiples.
- Organiser votre code avec les modules, les espaces de noms et les classes partielles.
- Exploiter le **modèle d'événements déclaratif de VB.NET** (`WithEvents` / `Handles` / `RaiseEvent`) — l'un de ses véritables points forts.
- Comprendre l'immuabilité en VB.NET et savoir **consommer** des *records* écrits en C#.
- Inspecter et étendre vos types à l'exécution via la réflexion et les attributs.

---

## Pourquoi la POO reste pleinement pertinente en VB.NET (2026)

Le langage VB.NET est **figé à la version 16.9** : Microsoft le maintient mais n'y ajoute plus de syntaxe (stratégie *consumption-only*). On pourrait croire que ce gel pénalise la POO — il n'en est rien.

La quasi-totalité du modèle objet de VB.NET était **déjà complète et stable** bien avant ce gel. Héritage, interfaces, polymorphisme, génériques, événements, réflexion : tout cela fonctionne aujourd'hui exactement comme dans n'importe quel langage .NET moderne, et continuera de fonctionner sur .NET 10 LTS et au-delà. Le gel concerne surtout les **nouveautés syntaxiques** apparues côté C# (`record` déclarés, *pattern matching* avancé, membres `init`…), pas les fondations objet.

Deux conséquences pratiques traversent ce chapitre :

1. **Là où VB.NET brille, on l'exploite à fond.** Le modèle d'événements `WithEvents` / `Handles` (§ 3.6 ⭐) est plus déclaratif et souvent plus lisible que son équivalent C#. C'est un idiome historique de VB qui demeure un atout réel pour les applications de bureau pilotées par les événements.

2. **Là où VB.NET ne déclare pas, il consomme.** VB ne possède pas le mot-clé `record` (§ 3.7 ⚠️ 🔗). Mais vu de l'extérieur, un *record* n'est qu'une classe .NET ordinaire : on peut donc parfaitement **consommer depuis VB.NET** un type `record` défini dans une bibliothèque C#, conformément à la stratégie hybride de la formation (module 10). Pour l'immuabilité « maison », VB s'appuie sur `ReadOnly` et les propriétés en lecture seule.

---

## 🗺️ Plan du chapitre

| Section | Sujet | Repère |
|---------|-------|--------|
| **3.1** | Classes et objets — propriétés (auto et complètes), constructeurs, `Me` | Fondation |
| **3.2** | Structures (`Structure`) et tuples — types valeur et regroupements légers | |
| **3.3** | Héritage et polymorphisme — `Inherits`, `Overrides`, `MustInherit`, `NotInheritable`, `Shadows` | |
| **3.4** | Interfaces — `Implements`, interfaces génériques et multiples | |
| **3.5** | Modules, espaces de noms et classes partielles — organisation du code | |
| **3.6** | **Événements et délégués** — l'idiome `WithEvents` / `Handles` / `RaiseEvent` | ⭐ Cœur VB |
| **3.7** | Types immuables et *records* — consommer du C#, approcher l'immuabilité | ⚠️ 🔗 |
| **3.8** | Réflexion et attributs — `System.Reflection`, attributs personnalisés | |

La progression va du plus concret (une classe, un objet) au plus avancé (réflexion et métaprogrammation). Les sections 3.1 à 3.5 posent les briques universelles de la POO ; la section 3.6 met en lumière une spécificité idiomatique forte de VB.NET ; les sections 3.7 et 3.8 ouvrent sur des sujets plus pointus, dont un point de frontière VB/C# important.

---

## Spécificités VB.NET à garder en tête

Plusieurs constructions de ce chapitre n'ont pas d'équivalent strict — ou s'écrivent différemment — en C#. Les connaître évite bien des confusions, en particulier lorsqu'on lit du code C# (omniprésent dans la documentation et les réponses d'IA) pour le transposer en VB.NET :

- **`Me`, `MyBase`, `MyClass`** — `Me` correspond à `this` ; `MyBase` à `base` ; `MyClass` (sans équivalent direct en C#) appelle l'implémentation définie dans la classe courante comme si le membre n'était pas surchargeable.
- **`Module`** — un conteneur propre à VB.NET (§ 3.5), à mi-chemin entre la classe statique de C# et un espace de noms implicite : ses membres sont accessibles sans qualification.
- **Mots-clés d'héritage en toutes lettres** — `Inherits`, `Implements`, `Overridable` / `Overrides`, `MustInherit` / `MustOverride`, `NotInheritable` remplacent respectivement les `:`, `virtual`, `override`, `abstract` et `sealed` de C#.
- **Syntaxe des attributs** — VB.NET utilise des chevrons `<Attribut>` là où C# emploie des crochets `[Attribute]` (§ 3.8).
- **Pas de `record` déclaré** — VB consomme les *records* C# mais ne les définit pas (§ 3.7).

Pour une correspondance exhaustive, reportez-vous à l'**[Annexe A — Correspondance syntaxique VB.NET ↔ C#](../annexes/correspondance-vbnet-csharp/README.md)**.

> 🤖 **Astuce IA** — Les modèles de langage étant majoritairement entraînés sur du C#, ils génèrent fréquemment des constructions POO en syntaxe C# (`public class`, `: BaseClass`, `[Attribute]`). Précisez toujours « **Visual Basic .NET** » dans vos requêtes et vérifiez systématiquement la transposition des mots-clés ci-dessus. Voir le **[module 17](../17-developpement-ia/README.md)**.

---

## Prérequis

Ce chapitre suppose acquis :

- Les **fondamentaux du langage** (module 2) : types, variables, opérateurs, structures de contrôle, collections et génériques de base.
- La notion de **type valeur vs type référence** (§ 2.2), revisitée ici à travers `Class` et `Structure`.
- Idéalement, la lecture du **positionnement 2026** (§ 1.6) pour bien saisir le contexte « langage stabilisé » évoqué plus haut.

---

Place aux fondations : commençons par la brique élémentaire de tout programme orienté objet — la **classe** et l'**objet**.

⏭️ [Classes et objets (propriétés auto et complètes, constructeurs, Me)](/03-poo/01-classes-objets.md)
