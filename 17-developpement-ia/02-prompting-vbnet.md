🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.2 Prompting efficace pour obtenir du code VB.NET

La section [17.1](01-pourquoi-ia-vbnet.md) a établi le *pourquoi* : le biais C# des modèles est structurel, et on ne le corrigera pas en attendant un meilleur modèle. On le neutralise par la **méthode** — et la première méthode, la plus rentable, est la qualité du *prompt*. Cette section donne les règles de formulation, la procédure de conversion du C# vers VB.NET, et une série de gabarits prêts à copier.

> **Principe directeur.** Le modèle comble chaque zone d'ambiguïté par du C#. Bien *prompter* pour VB.NET, c'est donc **rendre explicite tout ce qu'un défaut C# déciderait à votre place** : le langage, la version, le type de projet, les options de compilation, les idiomes attendus. Moins vous laissez d'implicite, moins le modèle improvise.

---

## Les cinq règles de formulation

### Règle n°1 — Nommer le langage sans ambiguïté

Écrivez toujours **« Visual Basic .NET »** ou **« VB.NET »**, jamais « Visual Basic » seul, et jamais « Basic ». Pour un modèle, « Visual Basic » recouvre aussi bien le **VB6** historique que le VB.NET moderne — et il n'est pas rare qu'il réponde alors avec des réflexes d'une autre époque : `MsgBox` au lieu de `MessageBox.Show`, `Variant`, `Set`, `Dim` sans type, APIs et concepteur de formulaires VB6.

Nommer explicitement « VB.NET » ancre le langage moderne. On renforce utilement en précisant « pas VB6 » et en mentionnant des constructions que VB6 ne connaît pas (async/await, LINQ, génériques) : ce sont autant de signaux que vous attendez du VB.NET récent.

### Règle n°2 — Préciser la version .NET cible et le contexte du projet

Indiquez la cible **.NET 10 (LTS)** et le fait que le langage VB est **figé à la version 16.9**, en mode *consumption-only*. C'est le meilleur antidote aux *fonctionnalités fantômes* : sans cette précision, le modèle prête volontiers à VB des nouveautés tirées des dernières versions de C#, en supposant une parité qui n'existe plus.

Donnez aussi le **type de projet** (Windows Forms, WPF, bibliothèque de classes, console, Web API par contrôleurs), exigez `Option Strict On` et `Option Explicit On`, et mentionnez les paquets NuGet en jeu si c'est pertinent. Plus le modèle connaît vos contraintes, moins il a de latitude pour dériver vers le C#.

### Règle n°3 — Exiger les conventions et idiomes VB

Demandez explicitement le style idiomatique : `Handles` / `WithEvents` pour les événements, blocs `Using` pour les ressources, commentaires de documentation au format `'''`, conventions de nommage VB. Précisez « compatible `Option Strict On` » pour bannir la liaison tardive et imposer les conversions explicites. Vous pouvez aussi lister les anti-patterns à éviter, mais le plus efficace reste de **décrire le résultat voulu en termes VB**.

### Règle n°4 — Ancrer avec un exemple (l'effet *few-shot*)

C'est le levier le plus sous-utilisé et le plus puissant. Collez un **court extrait représentatif de votre code VB existant** — un en-tête de classe, un gestionnaire d'événement, votre style de commentaire. Le modèle imite ce qu'on lui montre : un ou deux exemples concrets l'orientent vers votre époque et votre style bien plus sûrement qu'un paragraphe de consignes. C'est aussi le moyen le plus fiable de le maintenir *en VB* du début à la fin d'une réponse longue.

### Règle n°5 — Demander une seule chose, vérifiable

Cadrez serré : une unité focalisée que vous pouvez compiler et tester. Les demandes vastes et floues maximisent la dérive ; il vaut mieux itérer. Demandez enfin au modèle d'**expliciter ses hypothèses** et de **signaler tout point dont il n'est pas sûr qu'il soit valide en VB** — cela transforme une affirmation trompeuse en question que vous pouvez trancher.

---

## Convertir et corriger du C# vers VB.NET

Malgré un bon *prompt*, vous aurez en permanence du C# à traduire : celui que le modèle produit quand même, et surtout celui que vous trouvez ailleurs — documentation officielle, réponses de forum, exemples de bibliothèques. Savoir piloter cette conversion est une compétence quotidienne.

Le modèle est **fiable sur la traduction mécanique** du C# direct : appels à la bibliothèque de classes, LINQ, async/await, code de *consommation* en général. C'est précisément là qu'il se trompe le moins, et c'est une bonne nouvelle car c'est le terrain de VB.NET.

En revanche, certaines zones exigent une **relecture humaine systématique**, car le C# n'y a pas de correspondance directe en VB (le catalogue exhaustif est en [17.7](07-limites-pieges.md) ; l'aide-mémoire de transposition, en [Annexe A](../annexes/correspondance-vbnet-csharp/README.md)) :

- **Records, `init`, syntaxe positionnelle** → doivent devenir des classes écrites à la main ; le modèle invente parfois un `Record` *déclaré*, qui n'existe pas en VB.
- **`switch` expressions et filtrage de motifs avancé** → le `Select Case` et le `TypeOf…Is` de VB sont plus limités ; les motifs C# complexes ne se traduisent pas un pour un.
- **Top-level statements** → VB exige un `Module` et un `Sub Main` explicites.
- **Types nullables** → le `?` n'a pas le même sens : les *nullable reference types* de C# (`string?`) ≠ les types nullables de **valeur** de VB (`Nullable(Of T)` / `T?`), et VB n'a pas de NRT (cf. [2.2](../02-fondamentaux-langage/02-types-variables.md)). Le modèle confond régulièrement les deux.
- **Opérateurs — le piège classique** : `^` est la **puissance** en VB, mais le **XOR binaire** en C# (XOR s'écrit `Xor` en VB) ; la division entière est `\` en VB (le `/` renvoie toujours un flottant), et `%` devient `Mod` ; `&&` / `||` deviennent `AndAlso` / `OrElse`. Une traduction littérale de ces opérateurs change silencieusement le résultat.
- **Sensibilité à la casse** : C# distingue les identifiants par la casse, VB non — deux membres C# qui ne diffèrent que par la casse entrent en collision en VB.
- **Divers** : `out` / `ref` (→ `ByRef`, éventuellement `<Out>`), itérateurs `yield` (→ `Iterator` / `Yield`), membres à corps d'expression (`=>`), chaînes brutes (VB a `$"…"` mais pas de *raw string literals*).

La procédure gagnante combine modèle et compilateur : **traduire → coller dans Visual Studio → laisser `Option Strict On` et le compilateur signaler les erreurs → renvoyer ces erreurs au modèle pour correction → recommencer**. Le compilateur est l'arbitre de vérité. Attention toutefois : *« ça compile »* ne veut pas dire *« c'est correct »* — les différences d'opérateurs (`^`) et de gestion du `null` produisent du code qui compile mais se comporte autrement. La validation finale passe par les tests.

---

## Modèles de prompts prêts à l'emploi

Voici une série de gabarits réutilisables. Remplacez les `{…}` par votre contexte ; ils sont conçus pour être copiés tels quels.

**1. Préambule de génération** — à préfixer à toute demande de code :

```text
Tu es un assistant expert en Visual Basic .NET (VB.NET), pas en VB6.
Contraintes impératives :
- Langage : Visual Basic .NET, version de langage 16.9 (stabilisée, "consumption-only").
- Cible : .NET 10 (LTS). Type de projet : {Windows Forms | WPF | bibliothèque | console | Web API par contrôleurs}.
- Option Strict On et Option Explicit On : pas de liaison tardive, conversions explicites.
- N'utilise AUCUNE construction absente de VB : pas de record déclaré, pas de top-level
  statements, pas de switch expression, pas de constructeur primaire, pas d'expression de collection.
- Style idiomatique VB : Handles/WithEvents pour les événements, blocs Using, doc avec '''.
- Si une fonctionnalité demandée n'a pas d'équivalent direct en VB, signale-le au lieu d'inventer.

Demande : {décris précisément le besoin, le contexte et les types en jeu}.
```

**2. Conversion C# → VB.NET** :

```text
Convertis le code C# suivant en Visual Basic .NET idiomatique, ciblant .NET 10, Option Strict On.
- Conserve le comportement à l'identique.
- Remplace tout élément C# sans équivalent (record, init, switch expression, top-level…) par
  l'écriture VB correcte (classe écrite à la main, Select Case, Module/Sub Main, etc.).
- Pièges à vérifier : opérateur ^ (puissance en VB, pas XOR), division entière \, Mod, AndAlso/OrElse,
  sensibilité à la casse, types nullables (valeur en VB ≠ référence en C#).
- À la fin, liste chaque point sans équivalent direct et le choix que tu as fait.

Code C# :
{colle ici le code C#}
```

**3. Correction à partir des erreurs du compilateur** :

```text
Le code VB.NET suivant ne compile pas sous .NET 10 / Option Strict On.

Code :
{colle le code}

Erreurs du compilateur (Visual Studio) :
{colle les messages BCxxxx avec les numéros de ligne}

Corrige uniquement le nécessaire, en VB.NET idiomatique, sans introduire de syntaxe C#.
Explique brièvement chaque correction.
```

**4. Gestionnaire d'événement / contrôle Windows Forms** :

```text
En Visual Basic .NET (.NET 10, Windows Forms, Option Strict On), écris le gestionnaire
d'événement {Button1.Click | Form_Load | …} qui {comportement attendu}.
Utilise l'idiome VB avec Handles (et WithEvents si nécessaire), gère les erreurs avec Try/Catch,
et n'emploie aucune syntaxe C#.
Contexte disponible : {contrôles, champs et propriétés concernés}.
```

**5. Instructions persistantes** — à régler **une seule fois** dans les réglages de l'assistant (Copilot, chat), pour éviter de répéter le préambule à chaque message :

```text
Réponds toujours en Visual Basic .NET (jamais VB6, jamais C# sauf demande explicite).
Cible .NET 10, langage VB 16.9, Option Strict On. N'utilise aucune construction VB inexistante
et signale les cas sans équivalent direct. Privilégie les idiomes VB (Handles/WithEvents, Using,
''' pour la documentation).
```

---

## En résumé

Un bon *prompt* VB.NET tient en trois réflexes : **nommer** le langage et la cible sans ambiguïté, **ancrer** la réponse avec un exemple de votre code, et **valider** systématiquement par le compilateur puis les tests. La conversion du C# omniprésent suit la même discipline, en surveillant les quelques zones — records, `null`, opérateurs `^` et `\`, casse — où la traduction littérale trahit l'intention.

Ces techniques s'intègrent dans un flux de travail plus large : leur mise en place dans Visual Studio 2026 et la logique des agents sont traitées en [17.6](06-workflow-ia-first.md), tandis que les cas concrets de bout en bout (migration, API REST, WPF MVVM) le sont en [17.8](08-cas-concrets.md).

⏭️ [Migrer du legacy avec l'IA (VB6, .NET Framework)](/17-developpement-ia/03-migration-legacy-ia.md)
