🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.4 Générer des tests, mocks et documentation XML

Écrire des tests, configurer des doubles de test et documenter le code sont trois tâches **répétitives et reconnaissables** — exactement le genre de travail où l'IA brille, et exactement le genre de discipline que les développeurs négligent faute de temps. L'assistant lève cet obstacle. Mais ces trois tâches n'ont pas le même niveau de risque, et c'est tout l'objet de cette section : savoir jusqu'où s'appuyer sur l'IA pour chacune.

> **Périmètre.** Les frameworks de test, le *mocking* et la couverture relèvent du [module 13](../13-tests-qualite/README.md) (dont [13.5](../13-tests-qualite/05-couverture-tests-ia.md), qui traite spécifiquement de la couverture et de la génération de tests par IA). Les règles de *prompting* sont en [17.2](02-prompting-vbnet.md). On ne les reprend pas ; on se concentre sur la conduite de l'IA pour ces trois artefacts.

> **Le gradient de confiance.** Les trois tâches ne se valent pas face à l'IA. La **documentation XML** est la plus sûre (un commentaire faux coûte peu). Les **mocks** sont à risque modéré (surtout des questions de syntaxe VB et de conception). Les **tests** sont les plus délicats — non par difficulté technique, mais à cause d'un piège de fond : l'IA peut valider vos bugs.

---

## Le biais C# s'applique aussi au code de test

Un rappel utile avant tout : la documentation et les exemples des frameworks de test (xUnit, NUnit, MSTest) comme des bibliothèques de *mocking* (Moq, NSubstitute) sont massivement en **C#**. Le biais décrit en [17.1](01-pourquoi-ia-vbnet.md) s'applique donc pleinement au code de test : par défaut, le modèle produit des tests en C#, ou en VB teinté de C#.

Deux points VB.NET reviennent constamment et méritent d'être exigés dans chaque *prompt* : les **attributs s'écrivent entre chevrons** (`<Fact>`, `<Test>`, `<TestMethod>`…), pas entre crochets façon C# (`[Fact]`) ; et les **lambdas utilisent `Function` / `Sub`**, jamais la flèche `=>` du C#. Les règles de [17.2](02-prompting-vbnet.md) s'appliquent sans changement.

---

## Générer des tests unitaires

### Là où l'IA excelle

C'est un gain de productivité réel. L'assistant est excellent pour **énumérer les cas** — chemin nominal, valeurs limites, bornes, entrées invalides, chemins d'exception — et pour produire le *boilerplate* Arrange-Act-Assert, le nommage des tests et les tests paramétrés (`<Theory>` / `<InlineData>` en xUnit, `<TestCase>` en NUnit). Il couvre vite et bien tout ce qui est évident.

### Le danger cardinal : tester le code, pas l'intention

C'est le piège le plus important de toute la section, et il est subtil. Si vous demandez à l'IA de lire votre **implémentation** pour en écrire les tests, elle produit des assertions qui décrivent ce que le code *fait* — y compris ses bugs. Elle valide ce *qui est*, pas ce *qui devrait être*. Une fonction boguée se retrouve ainsi couverte de tests verts qui figent l'erreur.

Le remède est une affaire de méthode : **fournir à l'IA la spécification — le comportement attendu — dans le *prompt***, et non lui faire déduire l'intention à partir du code. Puis relire chaque assertion en se posant une seule question : *encode-t-elle une exigence, ou se contente-t-elle de refléter l'implémentation ?* L'**oracle** — ce qui est correct — vous appartient ; l'IA ne peut pas inventer vos règles métier.

### Les autres pièges

- **La couverture trompeuse.** Un pourcentage élevé obtenu par des tests générés ne signifie pas des tests utiles. La couverture est un outil, pas un objectif (cf. [13.5](../13-tests-qualite/05-couverture-tests-ia.md)).
- **Les tests triviaux, redondants ou tautologiques**, qui gonflent la suite sans rien vérifier — ou les assertions portées sur le *mock* plutôt que sur le comportement.
- **Les cas limites métier** que l'IA ignore faute de connaître votre domaine.

### La nuance migration

Attention à ne pas confondre deux intentions opposées. En **migration** ([17.3](03-migration-legacy-ia.md)), les tests de **caractérisation** capturent volontairement le comportement *actuel* comme référence : là, refléter l'implémentation existante est précisément le but. En développement neuf ou en correction de bug, c'est l'inverse : on teste le comportement *attendu*. Le même outil sert deux objectifs distincts — sachez lequel vous poursuivez.

### Posture

L'IA rédige le premier jet ; vous détenez l'oracle et vérifiez l'intention contre l'implémentation. Le cycle TDD ([13.2](../13-tests-qualite/02-mocking-tdd.md)) reste piloté par l'humain ; l'IA en accélère les parties mécaniques.

---

## Générer des mocks

La conception du *mocking* et le choix des bibliothèques relèvent de [13.2](../13-tests-qualite/02-mocking-tdd.md). Côté IA, l'assistant est efficace sur le *boilerplate* : configurer des valeurs de retour, définir des séquences, vérifier des interactions.

Deux frictions méritent attention. D'abord la **syntaxe VB** : Moq et NSubstitute reposent fortement sur des lambdas et sont documentés en C# ; le modèle émet volontiers la flèche `=>` au lieu de `Function(x) …` / `Sub(x) …`. Certaines constructions de Moq sont en outre plus laborieuses en VB (configuration des méthodes `Sub`/void, certains *matchers*). Soyez explicite sur la syntaxe VB et vérifiez le résultat.

Ensuite la **conception**, où le risque dépasse l'IA mais que celle-ci amplifie : sommé de simuler, le modèle *sur-mocke* généreusement. Trop de doubles produisent des tests fragiles qui valident le *mock* plutôt que le code. C'est à vous de décider ce qui mérite vraiment un double de test.

---

## Générer la documentation XML

C'est la tâche la plus sûre du gradient, et celle où l'on peut s'appuyer le plus largement sur l'IA. Produire `<summary>`, `<param>`, `<returns>`, `<remarks>` et `<exception>` à partir d'une signature est au cœur des compétences du modèle, et le coût d'un commentaire erroné reste faible : on le lit.

Le point VB.NET à ne pas manquer : les commentaires de documentation s'écrivent avec **`'''` (trois apostrophes)**, et non `///` comme en C#. Le modèle se trompe régulièrement de marqueur — précisez-le.

« Sûre » ne veut pas dire « sans relecture » pour autant. Un `<summary>` faussement assuré induit en erreur chaque lecteur futur, et une documentation périmée est pire qu'absente. La bonne pratique : l'IA rédige le bloc complet, vous vérifiez l'**exactitude sémantique** (elle peut mal décrire le rôle d'un paramètre) et vous maintenez la doc synchronisée avec le code.

---

## Gabarits de prompts

Ces gabarits prolongent ceux de [17.2](02-prompting-vbnet.md). Remplacez les `{…}` ; ils se copient tels quels.

**1. Générer des tests à partir d'une spécification** (et non du seul code) :

```text
En Visual Basic .NET (.NET 10, framework {xUnit | NUnit | MSTest}, Option Strict On), écris des tests
unitaires pour la méthode ci-dessous.
- Teste le COMPORTEMENT ATTENDU décrit dans la spécification, pas seulement ce que fait le code.
- Couvre : cas nominal, valeurs limites, entrées invalides, exceptions attendues.
- Syntaxe VB : attributs entre chevrons (<Fact>, <Test>…), lambdas avec Function/Sub, aucune syntaxe C#.
- Nomme chaque test d'après ce qu'il vérifie.

Spécification (ce que la méthode DOIT faire) :
{règles et cas limites attendus}

Méthode :
{colle la méthode}
```

**2. Compléter la couverture / cas limites manquants** :

```text
Voici une méthode VB.NET et ses tests existants ({framework}). Identifie les cas NON couverts (limites,
erreurs, branches) et ajoute uniquement les tests manquants, en VB.NET idiomatique. Ne duplique pas les
tests existants ; n'écris aucun test tautologique.

Méthode :
{code}

Tests existants :
{code}
```

**3. Configurer un double de test en VB** :

```text
En Visual Basic .NET (.NET 10, {Moq | NSubstitute}, Option Strict On), écris la configuration du double
de test pour {l'interface / la dépendance}, afin de tester {le scénario}.
- Lambdas VB (Function/Sub), pas de flèche C# (=>).
- Configure les valeurs de retour nécessaires et vérifie les interactions attendues.
- Ne simule QUE cette dépendance ; ne sur-mocke pas.

Interface à simuler :
{colle l'interface}
```

**4. Générer la documentation XML** :

```text
Ajoute la documentation XML VB.NET (commentaires ''') à ce membre : <summary>, <param>, <returns>,
<exception> et <remarks> si utile. Utilise ''' (trois apostrophes), PAS /// (syntaxe C#).
Décris fidèlement le comportement réel ; si un paramètre ou un cas n'est pas clair, signale-le au lieu
d'inventer.

Membre :
{colle le code}
```

---

## En résumé

Suivez le gradient de confiance. **Documentation XML** : appuyez-vous largement sur l'IA, en vérifiant l'exactitude et le marqueur `'''`. **Mocks** : laissez l'IA produire le *boilerplate*, en surveillant la syntaxe de lambda VB et la tentation du sur-*mocking*. **Tests** : l'usage le plus discipliné — fournissez toujours l'oracle, et relisez chaque assertion pour qu'elle teste l'intention, jamais l'implémentation.

Le tableau complet de la couverture et de la qualité est dans le [module 13](../13-tests-qualite/README.md) ; le catalogue des pièges propres à l'IA, en [17.7](07-limites-pieges.md) ; et le débogage assisté, en [17.5](05-debugger-optimiser.md).

⏭️ [Déboguer et optimiser avec l'IA (expliquer des erreurs, analyser des *stack traces*)](/17-developpement-ia/05-debugger-optimiser.md)
