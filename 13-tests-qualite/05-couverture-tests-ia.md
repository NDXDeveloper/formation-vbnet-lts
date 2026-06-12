🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 13.5 — Couverture de code ; génération de tests par IA 🤖

> **Module 13 — Tests et qualité du code**
> Mesurer ce que vos tests couvrent réellement — et accélérer leur écriture, avec lucidité.

---

Cette section réunit deux thèmes : **mesurer** la couverture des tests, et **les écrire plus vite**
grâce à l'IA. Le second est marqué 🤖 à dessein : c'est précisément là que le **biais C# des
modèles et de l'outillage** — fil rouge de cette formation — se manifeste le plus concrètement
pour VB.NET.

---

# Partie A — La couverture de code

## Qu'est-ce que la couverture ?

La **couverture de code** mesure la **proportion du code exécutée par les tests**. C'est une
**métrique de diagnostic** : elle ne dit pas si les tests sont bons, mais elle révèle ce qui
**n'est pas testé du tout** — son vrai intérêt.

## Les types de couverture

| Type | Ce qu'il mesure |
|------|-----------------|
| **Couverture de lignes** | Quelles lignes ont été exécutées. |
| **Couverture de branches** | Quelles **issues de décision** ont été empruntées (le `True` *et* le `False` d'un `If`). |
| **Couverture de méthodes** | Quelles méthodes ont été appelées au moins une fois. |

> 💡 **La couverture de branches est plus parlante que celle de lignes.** Une ligne peut être
> « couverte » alors qu'une branche logique qu'elle contient ne l'est pas — par exemple un
> `If a AndAlso b` dont seul un cas de court-circuit a été exercé. Privilégiez la couverture de
> branches comme signal.

## Les outils en .NET

Ce sont des outils et des bibliothèques — donc **utilisables tels quels en VB.NET** :

- **Coverlet** — la bibliothèque de couverture multiplateforme de référence. Avec `dotnet test` :

  ```
  dotnet test --collect:"XPlat Code Coverage"
  ```

  produit un rapport au format **Cobertura** (XML).
- **Microsoft.Testing.Platform** (.NET 10) — propose la couverture via son **extension** dédiée
  (`--coverage`), dans la continuité de la nouvelle plateforme vue en [13.1](01-tests-unitaires.md).
- **Visual Studio 2026** — couverture intégrée à l'Explorateur de tests (édition Enterprise).
- **ReportGenerator** — convertit les données brutes (Cobertura, etc.) en **rapport HTML**
  lisible, avec le détail ligne par ligne et fichier par fichier.

## ⚠️ La couverture n'est pas la qualité

C'est le point le plus important de cette partie, et le plus souvent mal compris.

> ⚠️ **Une couverture élevée ne prouve pas que le code est correct.** On peut exécuter 100 % des
> lignes **sans rien vérifier de pertinent** : un test qui appelle une méthode mais n'affirme
> presque rien « couvre » le code tout en ne le testant pas vraiment. La couverture mesure ce qui
> est *exécuté*, pas ce qui est *vérifié*.

Deux conséquences pratiques :

- **N'érigez pas un pourcentage en objectif.** Dès qu'une mesure devient une cible, elle cesse
  d'être un bon indicateur (loi de Goodhart) : viser « 90 % » pousse à écrire des tests triviaux
  qui gonflent le chiffre sans valeur. Utilisez plutôt la couverture pour **repérer les zones
  critiques non testées**.
- **Pour mesurer la *qualité* des tests** (et non leur quantité), la **mutation testing**
  introduit des bugs artificiels dans le code et vérifie que les tests les détectent. C'est un
  meilleur juge — mais son outillage .NET (comme Stryker.NET) est **orienté C#**, nouvelle
  illustration du biais d'écosystème.

---

# Partie B — La génération de tests par IA 🤖

## La promesse

Les assistants IA (GitHub Copilot et consorts) accélèrent la partie **mécanique** de l'écriture
des tests : générer la structure **Arrange-Act-Assert**, proposer des cas limites, produire des
jeux de données, échafauder rapidement une première salve de tests. Sur du code répétitif, le gain
de temps est réel.

## ⚠️ La réalité VB.NET : l'outillage dédié cible C#

C'est le cœur du 🤖 de cette section, et il faut le dire sans détour.

Visual Studio 2026 propose un outillage de test assisté de premier plan — l'agent **`@test`** et
la capacité **« GitHub Copilot testing for .NET »** — qui génère des tests **adaptés au framework
et aux conventions du projet** (xUnit, NUnit, MSTest), construit et exécute les tests, et calcule
même la couverture.

> ⚠️ **Mais cet outillage dédié vise C#.** D'après la documentation Microsoft, les points d'entrée
> de génération de tests (menu contextuel, suggestions) routent automatiquement vers l'agent
> `@test` **lorsque le focus de l'IDE est sur du code C#** ; **pour les projets non-C# — donc
> VB.NET — ces options basculent sur un prompt Copilot *générique***. Autrement dit, le flux
> spécialisé (conscience du framework, respect des conventions, exécution intégrée) n'est pas au
> rendez-vous en VB : on retombe sur l'assistant générique.

Conséquence : un développeur VB.NET **n'est pas privé** d'aide IA pour ses tests — il peut tout à
fait demander des tests via Copilot Chat — mais il doit **fournir et vérifier manuellement** ce
que l'agent `@test` apporte automatiquement en C# (le bon framework, les bonnes conventions, une
syntaxe correcte). C'est exactement le **biais C#** que cette formation documente de bout en bout
(cf. [module 17](../17-developpement-ia/README.md) et [1.4](../01-introduction-vbnet/04-installation-outils.md)).

## Le réflexe en VB.NET : prompter et valider

Faute de flux dédié, deux disciplines deviennent indispensables.

**1. Prompter explicitement.** Toujours préciser dans la demande :

- le langage : **« Visual Basic .NET / VB.NET »** (sans quoi le modèle produit du C# par défaut) ;
- le **framework** de test visé (xUnit, NUnit ou MSTest) ;
- les conventions attendues (nommage, bibliothèque de *mocking*…).

**2. Valider systématiquement.** Le code généré pour VB doit être relu avec une vigilance
particulière aux **fuites de syntaxe C#** :

- la confusion classique de NUnit : le modèle oublie souvent que **`Is` est un mot-clé VB** et
  écrit `Is.EqualTo(...)` au lieu de **`[Is].EqualTo(...)`** (cf. [13.1](01-tests-unitaires.md)) ;
- des `var`, des points-virgules, une syntaxe de lambda erronée, des interpolations mal formées…

Vérifiez d'abord que **ça compile**, puis — surtout — que les tests **vérifient quelque chose de
sensé**.

## Les limites universelles de l'IA pour les tests

Au-delà du langage, des limites s'appliquent à **tous** les tests générés par IA — y compris en C# :

> ⚠️ **Un test généré décrit le comportement *actuel* du code, pas son comportement *correct*.**
> S'il y a déjà un bug, le test généré « passe au vert » en figeant ce bug : il valide la
> **cohérence avec l'existant**, pas la conformité à la spécification. L'IA ne remplace donc pas
> votre jugement sur *ce qui devrait* se passer.

Trois autres pièges :

- **Assertions faibles ou absentes** → de la **fausse couverture** (le lien avec la partie A est
  direct : du code « couvert » mais pas réellement vérifié).
- **Tests tautologiques** qui ne font que recopier l'implémentation, fragiles au moindre
  refactoring.
- **Sur-mocking**, contre lequel mettait en garde [13.2](02-mocking-tdd.md).

La règle qui en découle : l'IA accélère le **comment** (échafauder, écrire) ; le **quoi** (que
tester) et le **est-ce pertinent ?** (le test a-t-il du sens) restent **humains**.

## Bons usages de l'IA pour les tests

Employée avec discernement, l'IA est précieuse pour :

- **échafauder** la structure d'une classe de tests et ses doublures (un point de départ que
  *vous* affinez) ;
- **suggérer des cas limites** souvent oubliés (valeurs frontières, `Nothing`, collections vides) ;
- **générer des jeux de données** de test ;
- **convertir** des exemples de tests C# (omniprésents) vers VB.NET ;
- **produire de la documentation XML** (cf. [17.4](../17-developpement-ia/04-generer-tests-doc.md)).

---

## À retenir

La **couverture de code** indique ce que vos tests exécutent — utile surtout pour repérer les
**zones non testées** — mais elle ne mesure **pas** leur qualité : 100 % de couverture avec des
assertions creuses est une illusion de sécurité (n'en faites pas une cible). Les outils (Coverlet,
extension de couverture de MTP, ReportGenerator) s'utilisent sans souci en VB.NET. Côté
**génération par IA** 🤖, l'outillage **dédié** de VS 2026 (agent `@test`) **vise C#** : en VB.NET,
on retombe sur l'assistant générique, ce qui impose deux disciplines — **prompter explicitement**
(« VB.NET », framework, conventions) et **valider rigoureusement** (compilation, le piège `[Is]`,
et surtout la pertinence des assertions). Et quel que soit le langage, l'IA fige le comportement
*existant*, bugs compris : le jugement sur *ce qu'il faut tester* reste le vôtre.

➡️ Section suivante : **[13.6 — BenchmarkDotNet (notions)](06-benchmarkdotnet.md)**, pour mesurer les
performances avec méthode.

⏭️ [BenchmarkDotNet (notions)](/13-tests-qualite/06-benchmarkdotnet.md)
