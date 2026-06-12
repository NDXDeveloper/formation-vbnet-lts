🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 13. Tests et qualité du code

> **Partie 6 — Qualité, performance et exploitation**
> Prouver qu'un code VB.NET **fonctionne** — et qu'il **continuera** de fonctionner après
> chaque modification.

---

## Vue d'ensemble

Le module 12 visait à *réagir* aux erreurs, les *comprendre* et les *tracer*. Ce chapitre prend
le problème par l'autre bout : **empêcher les erreurs d'arriver**, et surtout **les empêcher de
revenir**. Car le vrai danger n'est pas le bug qu'on découvre une fois — c'est celui qu'on
réintroduit silencieusement trois mois plus tard, en touchant à autre chose.

Tester, ce n'est pas une formalité d'« assurance qualité » réservée aux grandes équipes. C'est ce
qui transforme un code qui *compile* en un code dans lequel on peut **modifier, refactorer et
livrer en confiance**. Un test qui passe au vert après un changement, c'est une preuve immédiate
que l'on n'a rien cassé.

Ce chapitre couvre toute la chaîne de la qualité : tests unitaires, *mocking* et TDD, tests
d'intégration, analyse statique, couverture de code, génération de tests par IA, et notions de
*benchmark*.

---

## VB.NET et les tests : un terrain pleinement supporté

C'est, comme le module 12, un domaine où le **gel du langage VB ne coûte rien** : les frameworks
de test, les bibliothèques de *mocking*, les analyseurs et les outils de couverture sont tous des
**bibliothèques NuGet** et de l'**outillage** que l'on consomme — pas de la nouvelle syntaxe.

Mieux : VB.NET y est un **citoyen de première classe au sens littéral**.

> 🟢 **Les projets de test font partie des modèles de projet VB.NET.** Sur la poignée de modèles
> disponibles en VB.NET (cinq familles : Console, bibliothèque de classes, Windows Forms, WPF…),
> les **projets de test** en sont une à part entière. On crée donc un projet de test **xUnit**,
> **NUnit** ou **MSTest** directement en VB.NET, sans détour.

Deux réserves honnêtes, propres au langage, que les sous-sections détailleront :

- ⚠️ **StyleCop est spécifique à C#.** Les analyseurs StyleCop imposent un style de code **C#** et
  ne s'appliquent pas à VB.NET. Côté VB, le style se gouverne via `.editorconfig` et les
  analyseurs qui prennent en charge VB, complétés par **SonarQube** (qui dispose de règles
  VB.NET). → détaillé en [13.4](04-analyse-statique.md).
- 🤖 **Les assistants IA génèrent du C# par défaut.** Pour la génération de tests assistée
  ([13.5](05-couverture-tests-ia.md)), il faut **toujours préciser « VB.NET »** et **valider** le
  résultat — le biais C# des modèles est ici aussi à l'œuvre (cf. [module 17](../17-developpement-ia/README.md)).

---

## Pourquoi c'est décisif pour VB.NET en particulier

Au-delà des bonnes pratiques universelles, les tests jouent un rôle **stratégique** dans les
scénarios qui sont au cœur de VB.NET.

> 🔗 **Les tests sont le filet de sécurité de la migration et de la modernisation.** Une grande
> partie du travail en VB.NET consiste à **maintenir et faire évoluer du legacy** (VB6,
> .NET Framework) ou à **moderniser** du code existant. Or on ne refactore pas sereinement sans
> garde-fou : une suite de **tests de non-régression** est précisément ce qui permet de migrer
> ou réorganiser sans tout casser. C'est ce qui relie ce chapitre aux modules
> [11.6 (moderniser)](../11-migration-legacy/06-moderniser.md),
> [11.7 (gestion des risques)](../11-migration-legacy/07-gestion-risques.md) et
> [17.3 (migration assistée par IA)](../17-developpement-ia/03-migration-legacy-ia.md).

Avant de toucher à du code ancien, on l'**enveloppe de tests** qui décrivent son comportement
actuel ; on modifie ensuite en gardant ces tests au vert. Le test n'est plus un luxe : c'est
l'outil qui rend le changement possible.

---

## Objectifs du chapitre

À l'issue de ce chapitre, vous saurez :

- Distinguer les **niveaux de tests** (unitaires, intégration) et le rôle de chacun.
- Écrire des **tests unitaires en VB.NET** avec un framework moderne (xUnit, NUnit ou MSTest) et
  situer la nouvelle plateforme **Microsoft.Testing.Platform** de .NET 10.
- Isoler le code testé grâce au **mocking** (Moq, NSubstitute) et pratiquer le cycle **TDD**.
- Mettre en place des **tests d'intégration** réalistes (base de données réelle, conteneurs).
- Exploiter l'**analyse statique** pour la qualité — en connaissant les limites VB de certains
  outils.
- Mesurer la **couverture de code** et tirer parti de l'**IA** pour générer des tests, en
  validant systématiquement.
- Disposer de **notions de micro-benchmark** pour mesurer les performances avec rigueur.

---

## Contenu du chapitre

| Section | Sujet | Indicateurs |
|---------|-------|-------------|
| **13.1** | [Tests unitaires (xUnit, NUnit, MSTest ; `Microsoft.Testing.Platform` dans .NET 10)](01-tests-unitaires.md) | 🆕 |
| **13.2** | [*Mocking* (Moq, NSubstitute) et TDD](02-mocking-tdd.md) | |
| **13.3** | [Tests d'intégration (`WebApplicationFactory`, Testcontainers, tests avec base de données)](03-tests-integration.md) | |
| **13.4** | [Analyse statique (analyseurs Roslyn, SonarQube, StyleCop)](04-analyse-statique.md) | ⚠️ |
| **13.5** | [Couverture de code ; génération de tests par IA](05-couverture-tests-ia.md) | 🤖 |
| **13.6** | [BenchmarkDotNet (notions)](06-benchmarkdotnet.md) | |

**13.1 — Tests unitaires.** Le socle. Écrire des tests rapides et isolés qui vérifient une unité
de code, avec les frameworks de référence (xUnit, NUnit, MSTest), la structure **AAA**
(*Arrange-Act-Assert*) et le nommage. Présente aussi la nouvelle plateforme
**Microsoft.Testing.Platform** introduite avec .NET 10. 🆕

**13.2 — Mocking et TDD.** Tester une unité sans dépendre de ses voisines : remplacer les
dépendances par des **doublures** (avec Moq ou NSubstitute), et adopter le cycle TDD
*rouge → vert → refactor* pour laisser les tests guider la conception.

**13.3 — Tests d'intégration.** Vérifier que les composants fonctionnent **ensemble**, contre une
infrastructure réelle : `WebApplicationFactory` pour une Web API VB.NET, **Testcontainers** pour
lancer une vraie base de données en conteneur le temps du test.

**13.4 — Analyse statique.** Détecter des problèmes **sans exécuter** le code, grâce aux
analyseurs Roslyn et à **SonarQube**. Cette section précise aussi la frontière côté VB
(StyleCop étant spécifique à C#). ⚠️

**13.5 — Couverture et IA.** Mesurer **quelle part du code** est réellement exercée par les
tests, et **générer des tests avec l'IA** — avec la rigueur indispensable côté VB.NET (préciser
le langage, valider le code produit). 🤖

**13.6 — BenchmarkDotNet (notions).** Mesurer les performances avec méthode plutôt qu'au
chronomètre approximatif : une introduction aux **micro-benchmarks**, en ouverture vers le
[module 14 (performance)](../14-performance/README.md).

---

## Prérequis et liens avec les autres modules

**Prérequis recommandés :**

- **[Module 3 — Programmation orientée objet](../03-poo/README.md)** : interfaces et injection de
  dépendances facilitent grandement la testabilité (et le *mocking*).
- **[Module 4 — Programmation asynchrone](../04-async/README.md)** : tester du code `Async`/`Await`
  demande de connaître les notions du chapitre 4.

**Ce chapitre prolonge ou complète :**

- **[Module 12 — Exceptions, débogage et journalisation](../12-exceptions-debogage/README.md)** :
  un code bien **instrumenté** est un code plus facile à tester et à diagnostiquer — la relation
  va dans les deux sens.
- **[Module 11 — Migration et maintenance du legacy](../11-migration-legacy/README.md)** : les
  tests de non-régression sont le **filet de sécurité** de toute migration.
- **[Module 14 — Performance](../14-performance/README.md)** : BenchmarkDotNet et le profilage
  prolongent la mesure de la qualité vers la performance.
- **[Module 10 — Architecture hybride VB.NET / C#](../10-hybride-vbnet-csharp/README.md)** 🔗 :
  dans une solution mixte, les tests peuvent être écrits en VB **ou** en C#, et couvrir les deux.
- **[Annexe C — Bonnes pratiques de codage](../annexes/bonnes-pratiques/README.md)** pour les
  conventions de qualité au quotidien.

> 🤖 **Côté IA :** la génération de tests, de *mocks* et de documentation assistée fait l'objet de
> [17.4 — Générer des tests, mocks et documentation XML](../17-developpement-ia/04-generer-tests-doc.md).
> Comme toujours en VB.NET, précisez le langage dans vos prompts et relisez le résultat.

---

## En résumé

La qualité ne se rajoute pas à la fin : elle s'**outille** dès le départ, par des tests qui
prouvent que le code fait ce qu'on attend et le signalent dès qu'il cesse de le faire. Sur ce
terrain, VB.NET n'a **rien à envier à C#** — au point d'offrir des modèles de projet de test
natifs — et y trouve même un intérêt particulier : les tests sont ce qui rend la **migration** et
la **modernisation** du legacy possibles sans casse. Les seules réserves sont ponctuelles et
clairement identifiées : **StyleCop** ne vise que C#, et l'**IA** génère du C# par défaut. Tout le
reste est à votre disposition, à l'identique.

➡️ Commencez par **[13.1 — Tests unitaires](01-tests-unitaires.md)** 🆕.

⏭️ [Tests unitaires (xUnit, NUnit, MSTest ; Microsoft.Testing.Platform dans .NET 10)](/13-tests-qualite/01-tests-unitaires.md)
