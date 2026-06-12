🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.6 Workflow IA-first : Copilot dans Visual Studio 2026, VS Code, agents

Les sections précédentes ont donné les techniques — *prompting* ([17.2](02-prompting-vbnet.md)), migration ([17.3](03-migration-legacy-ia.md)), tests et documentation ([17.4](04-generer-tests-doc.md)), débogage et optimisation ([17.5](05-debugger-optimiser.md)). Cette section les assemble en un **workflow quotidien** et cartographie où l'IA s'intègre. Le message propre à VB.NET est simple : en 2026, *« IA-first »* est la posture par défaut, mais pour VB.NET, le workflow doit être **configuré délibérément**, parce que chaque outil penche vers le C#. Et le choix de l'environnement y pèse plus lourd qu'en C#.

> **Périmètre et volatilité.** Les raccourcis, extensions et astuces de Visual Studio 2026 sont détaillés dans l'[Annexe D](../annexes/visual-studio-2026/README.md). L'outillage IA évolue vite : pour l'état exact des fonctionnalités, référez-vous à l'Annexe D et à la documentation officielle. On s'attache ici aux **principes** et aux **contraintes VB**, qui, eux, sont stables.

---

## Les modes de l'assistant : le vocabulaire de base

Pour raisonner sur un workflow, il faut un modèle mental des modes d'usage :

- **Complétion en ligne** (suggestions « fantômes ») : l'assistant complète au fil de la frappe. C'est le mode le plus présent — et celui où le biais C# est le plus **insidieux**, car il oriente discrètement vers des tournures C# sans que vous le demandiez.
- **Chat conversationnel** (et chat en ligne sur une sélection) : poser une question, expliquer, générer, refactoriser. Souvent via des commandes slash (`/explain`, `/fix`, `/tests`, `/doc`) et des références de contexte (un fichier, l'espace de travail).
- **Mode agent / agents** : l'IA planifie et exécute des changements **multi-fichiers** de façon autonome, peut lancer la compilation et les tests, et itère vers un objectif. Le plus puissant — et le plus risqué.
- **Instructions personnalisées** : une configuration persistante (au niveau du dépôt ou de l'IDE) qui oriente le chat et les agents sans qu'on ait à se répéter. C'est, pour VB.NET, le levier le plus important.

---

## Configurer le workflow pour VB.NET — le geste décisif

Puisque tous les modes penchent vers le C# par défaut, le premier réflexe d'une équipe VB est de **fixer une fois pour toutes des instructions persistantes**, plutôt que de répéter « VB.NET, .NET 10, Option Strict On » à chaque *prompt* (à rapprocher du gabarit d'instructions persistantes de [17.2](02-prompting-vbnet.md)). L'idéal est un **fichier d'instructions au niveau du dépôt**, versionné et partagé : il voyage avec le projet et s'applique au chat, aux agents et à la revue de code — mais **pas à la complétion en ligne**, qui ne lit pas ce fichier. Chez GitHub Copilot, ce fichier s'appelle `.github/copilot-instructions.md` (reconnu par Visual Studio 2026 comme par VS Code) :

```text
# Instructions du dépôt pour l'assistant IA
- Tout le code de ce dépôt est en Visual Basic .NET (jamais VB6, jamais C# sauf demande explicite).
- Cible : .NET 10 (LTS). Langage VB 16.9 (stabilisé). Option Strict On et Option Explicit On.
- N'utilise aucune construction absente de VB (record déclaré, top-level statements, switch
  expression, constructeur primaire, expression de collection).
- Idiomes attendus : Handles/WithEvents, blocs Using, attributs entre chevrons, documentation '''.
- Signale toute fonctionnalité sans équivalent VB au lieu de l'inventer.
```

Deux compléments à cette configuration : **ancrer par le contexte du dépôt** — c'est lui, et non le fichier d'instructions, qui gouverne la **complétion** : plus le projet contient de vrai code VB, plus elle reste en VB (l'effet *few-shot* de [17.2](02-prompting-vbnet.md), à l'échelle de la base de code) ; et activer **`Option Strict On` sur tout le projet**, pour que le compilateur rattrape ce qui passe malgré tout.

---

## Copilot dans Visual Studio 2026 — le foyer recommandé pour VB.NET

Visual Studio 2026 est un IDE *« AI-native »*, et c'est le **foyer recommandé** pour VB.NET assisté par IA. On y trouve réunis ce que VB.NET exige et ce que l'IA apporte : le **concepteur** complet (Windows Forms ⭐, WPF), un **IntelliSense riche**, le **débogueur intégré** — et Copilot profondément intégré (complétion, chat, mode agent, instructions personnalisées).

C'est précisément cette réunion qui compte : le vébéiste y obtient à la fois l'outillage de langage mûr *et* les fonctionnalités d'IA. Pour les raccourcis et les extensions IA, voir l'[Annexe D](../annexes/visual-studio-2026/README.md).

---

## Copilot dans VS Code — un chemin dégradé pour VB.NET

Un point d'honnêteté important, et propre à VB.NET (cf. [1.4](../01-introduction-vbnet/04-installation-outils.md)) : **VS Code ne dispose pas du C# Dev Kit pour VB.NET**. L'édition y reste **basique** — sans IntelliSense riche, sans débogage intégré pour VB. Copilot fonctionne bien dans VS Code, mais l'**expérience VB autour de lui est dégradée** : peu de services de langage sur lesquels l'IA puisse s'appuyer, et une boucle de validation (compilateur, débogueur) affaiblie.

La conséquence est claire : pour VB.NET, **VS Code est un recours**, pas l'atelier principal — utile pour une retouche rapide, pour les parties non-VB d'un dépôt mixte, ou là où Visual Studio n'est pas disponible. La valeur de Copilot y est plafonnée par la minceur de l'outillage VB. Cette asymétrie n'existe pas en C# (VS Code + C# Dev Kit est excellent) : c'est une considération spécifiquement VB.

---

## Les agents — puissance et risque amplifiés

Le mode agent laisse l'IA planifier et exécuter seule des changements multi-fichiers, lancer *builds* et tests, et itérer. C'est réellement puissant sur le travail de masse de VB.NET : échafaudage, refactorisations répétitives, passes de migration ([17.3](03-migration-legacy-ia.md)), génération de tests à l'échelle d'un projet ([17.4](04-generer-tests-doc.md)).

Mais l'amplification joue dans les deux sens. Un agent qui agit en autonomie **sur un modèle biaisé vers le C#** peut **propager rapidement des tournures C# ou une hypothèse fausse sur de nombreux fichiers** — et une prémisse erronée se compose à mesure qu'il avance. Les risques de dérive sémantique et d'assurance trompeuse ([17.1](01-pourquoi-ia-vbnet.md), [17.5](05-debugger-optimiser.md)) **croissent avec l'autonomie**.

D'où des garde-fous plus stricts que pour le chat :

- **Tenir l'agent en laisse** : des tâches petites et bien cadrées, et la relecture de **chaque diff** avant acceptation.
- **Le compilateur et les tests comme garde-fou de réalité** : sur un dépôt avec `Option Strict On` et une vraie suite de tests ([17.4](04-generer-tests-doc.md)), l'agent s'auto-corrige bien mieux. Sans filet de tests, un agent autonome est dangereux — particulièrement en migration ([module 11](../11-migration-legacy/README.md)).
- **Le contrôle de version, non négociable** : branche dédiée, petits *commits*, *rollback* facile, pour qu'une exécution d'agent reste réversible.
- **Les instructions persistantes VB s'appliquent aussi aux agents** — et y comptent davantage, puisque vous n'êtes pas dans la boucle à chaque ligne.

La posture : l'agent est un **multiplicateur de force que l'on supervise**, pas un substitut au jugement. La règle de tout le module — *« l'IA propose, le développeur valide »* — devient ici : *« l'IA exécute sous supervision, le développeur relit le diff »*.

---

## Assembler la journée IA-first — la boucle

Le workflow qui relie les sections 17.2 à 17.5 :

1. **Configurer une fois** : instructions persistantes VB + `Option Strict On` + filet de tests.
2. **Écrire / générer** : complétion et chat pour le code neuf, toujours relus ([17.2](02-prompting-vbnet.md)).
3. **Convertir** : le C# de la documentation ou des forums → VB via le chat ([17.2](02-prompting-vbnet.md)).
4. **Tester et documenter** : générer les tests (en fournissant l'oracle) et la documentation XML ([17.4](04-generer-tests-doc.md)).
5. **Déboguer et optimiser** : expliquer les erreurs, analyser les *stack traces*, optimiser les points chauds confirmés — le débogueur et le profileur restant les arbitres ([17.5](05-debugger-optimiser.md)).
6. **Travail de masse** : confier une tâche cadrée à un agent, relire le diff, laisser compilateur et tests faire barrage.
7. **En continu** : le développeur garde l'autorité ; les outils (compilateur, débogueur, profileur, tests) tiennent l'IA honnête.

---

## En résumé

Un workflow *IA-first* pour VB.NET tient en trois décisions : **configurer délibérément** (instructions persistantes VB, `Option Strict On`, filet de tests), choisir le **bon foyer** (Visual Studio 2026, car l'expérience VB de VS Code est trop mince), et **superviser les agents** (plus d'autonomie appelle plus de garde-fous). L'outillage avance vite — l'[Annexe D](../annexes/visual-studio-2026/README.md) donne l'état des fonctionnalités de VS 2026 ; le catalogue des pièges est en [17.7](07-limites-pieges.md) ; et les cas de bout en bout, en [17.8](08-cas-concrets.md).

⏭️ [Limites et pièges (hallucinations de syntaxe C# en VB, validation systématique)](/17-developpement-ia/07-limites-pieges.md)
