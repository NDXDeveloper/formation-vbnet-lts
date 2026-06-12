🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.5 Déboguer et optimiser avec l'IA (expliquer des erreurs, analyser des *stack traces*)

Le débogage et l'optimisation sont deux activités où l'IA offre une aide réelle — et où chacune se heurte à une **limite dure, liée à un outil que l'IA ne peut pas remplacer** : le débogueur pour le débogage, le profileur pour l'optimisation. Cette section traite de la conduite de l'assistant pour ces deux tâches.

> **Périmètre.** La méthodologie et les outils de débogage relèvent du [module 12](../12-exceptions-debogage/README.md) (notamment [12.2](../12-exceptions-debogage/02-debogage.md)). Le profilage, le GC et les techniques d'optimisation relèvent du [module 14](../14-performance/README.md). Les règles de *prompting* sont en [17.2](02-prompting-vbnet.md). On se concentre ici sur ce que l'IA fait bien et sur ce qui induit en erreur.

> **Le principe commun.** L'IA **ne voit pas votre exécution**. Elle raisonne sur un instantané statique : elle n'observe ni l'état réel à l'exécution, ni vos données, ni votre environnement, ni le *timing*. Ses diagnostics sont des **hypothèses, jamais des observations**. L'outil — débogueur ou profileur — reste l'arbitre.

---

## Déboguer avec l'IA

### Expliquer des erreurs et des exceptions

C'est l'usage le plus rentable et le moins risqué. L'assistant décode remarquablement les messages obscurs — erreurs du compilateur VB (les codes `BCxxxx`) comme exceptions à l'exécution — en langage clair, avec les causes probables et des pistes de correction. Face à une erreur inconnue, c'est souvent plus rapide qu'une recherche.

Point VB.NET : collez le message **exact** et le code concerné, et précisez « VB.NET » pour que les corrections reviennent en VB et non en C#.

### Analyser des *stack traces*

Une *stack trace* est un artefact structuré, terrain idéal pour l'IA : elle identifie la trame qui échoue le plus probablement, explique la chaîne d'appels et indique où chercher en priorité. Donnez-lui la trace **complète** et le code autour des trames du haut — plus le contexte est riche, meilleure est l'hypothèse. Elle est particulièrement efficace sur les exceptions courantes (`NullReferenceException`, `InvalidCastException`…) et pour **réduire l'espace de recherche**.

### Générer des hypothèses de cause

« Voici le symptôme, qu'est-ce qui pourrait le provoquer ? » : l'IA énumère bien les causes candidates. Servez-vous-en pour **élargir** vos hypothèses, puis testez chacune au débogueur.

### La limite cardinale : l'IA n'exécute pas votre code

Tout découle de ce fait. Le diagnostic du modèle est une conjecture sur un code figé, pas un constat. Et le danger propre au débogage est l'**assurance trompeuse** : une explication fluide, plausible mais fausse, vous envoie sur une mauvaise piste — précisément là où vous êtes déjà dans l'incertitude, donc là où le coût d'une fausse piste est le plus élevé.

Le **débogueur reste la source de vérité** : points d'arrêt, espions, exécution pas à pas, Hot Reload ([12.2](../12-exceptions-debogage/02-debogage.md)). L'IA suggère *où* regarder ; le débogueur confirme ce qui *se passe réellement*. L'assistant complète l'outil, il ne le remplace pas.

Les cas où l'IA est la plus faible sont ceux qui exigent une observation qu'elle ne peut pas faire : *heisenbugs*, concurrence et problèmes de *timing*, bugs spécifiques à un environnement, état asynchrone (le débogage async relève de 12.2). Et quelques **pièges VB.NET** à surveiller dans ses diagnostics : elle peut proposer des corrections ou des APIs C#, confondre VB6 et VB.NET (`On Error` vs `Try/Catch`), ou mal lire un opérateur VB à l'origine même du bug (le `^` qui est une puissance, la division entière `\` — voir [17.2](02-prompting-vbnet.md)), voire ignorer un problème de liaison tardive sous `Option Strict Off`.

### Posture

L'IA est un **générateur d'hypothèses et un explicateur** ; le débogueur est l'**arbitre** ; vous décidez. Utilisez l'assistant pour vous débloquer et pour *comprendre*, jamais pour *conclure* sans vérification.

---

## Optimiser avec l'IA

### Là où l'IA aide

L'assistant repère bien les inefficacités manifestes : allocations dans une boucle, concaténations de chaînes à remplacer par `StringBuilder`, matérialisations LINQ inutiles ou requêtes N+1, *boxing* de types valeur, appels synchrones sur des opérations qui devraient être asynchrones. Il explique pourquoi un code est lent et propose des alternatives idiomatiques plus rapides. Il aide aussi à écrire un banc d'essai BenchmarkDotNet (cf. [module 14](../14-performance/README.md)).

### La limite cardinale : mesurer d'abord

C'est la première règle de la performance, et l'IA l'enfreint par défaut. Ses suggestions visent des points chauds **théoriques**, pas *votre* véritable goulot d'étranglement — qu'elle ignore, faute de savoir où votre application passe son temps. **Optimiser sans profiler, c'est optimiser au mauvais endroit.**

Le profileur (VS Profiler, `dotnet-counters` / `trace` / `dump` / `gcdump` — [14.1](../14-performance/01-profilage.md)) vous donne le point chaud réel. Apportez **cette mesure** à l'IA, pas une intuition. À l'inverse, sans point chaud confirmé, l'assistant verse volontiers dans la **micro-optimisation prématurée** : des retouches qui ne changent rien au temps total et nuisent à la lisibilité. Ce sont du bruit.

### Les pièges spécifiques

- **.NET 10 fait déjà beaucoup gratuitement.** Le JIT, le PGO, le SIMD et la dévirtualisation optimisent sans changer le code ([14.6](../14-performance/06-apports-net10.md)). L'IA, qui peut l'ignorer, propose parfois des optimisations manuelles que le runtime gère déjà — voire devenues contre-productives.
- **Des suggestions réservées au C#.** L'écriture de code *Span-first* est hors de portée de VB : le compilateur ne permet pas de **déclarer** une variable ou un paramètre `Span(Of T)` — seules les expressions éphémères passent, tandis que `Memory(Of T)` est pleinement utilisable ([14.4](../14-performance/04-span-pooling.md)) ; certaines optimisations relèvent franchement du C# et de la stratégie hybride ([module 10](../10-hybride-vbnet-csharp/README.md)). L'assistant peut proposer ce que VB ne fait pas proprement.
- **Le risque de correction.** Une « optimisation » peut changer le comportement. Un code *plus rapide mais faux* ne vaut rien : il faut **retester** (cf. [17.4](04-generer-tests-doc.md)).

### Posture

Mesurer → apporter le point chaud réel à l'IA → la laisser proposer → mesurer le gain au banc d'essai → revérifier la justesse. Le **profileur est l'arbitre**.

---

## Gabarits de prompts

Ces gabarits prolongent ceux de [17.2](02-prompting-vbnet.md). Remplacez les `{…}` ; ils se copient tels quels.

**1. Expliquer une erreur / une exception** :

```text
En Visual Basic .NET (.NET 10), explique cette erreur : cause probable, signification concrète, pistes
de correction. Si plusieurs causes sont possibles, liste-les par probabilité. Propose les corrections
en VB.NET, pas en C#.

Message d'erreur (exact) :
{colle le message — BCxxxx ou l'exception}

Code concerné :
{colle le code pertinent}
```

**2. Analyser une *stack trace*** :

```text
Analyse cette stack trace (.NET 10, application VB.NET). Identifie la trame qui échoue le plus
probablement, explique la chaîne d'appels et indique où chercher en priorité. Formule ton diagnostic
en HYPOTHÈSES à vérifier au débogueur, pas en certitudes.

Stack trace :
{colle la stack trace complète}

Code autour des trames du haut :
{colle le code}
```

**3. Générer des hypothèses de cause** :

```text
Bug en VB.NET (.NET 10). Symptôme : {ce qui se passe vs ce qui est attendu}.
Contexte : {type de projet, déclencheur, données}.
Énumère les causes possibles par ordre de probabilité ; pour chacune, indique l'observation à faire
au débogueur (point d'arrêt, valeur à inspecter) qui la confirmerait ou l'écarterait.

Code :
{colle le code}
```

**4. Optimiser un point chaud confirmé par le profileur** :

```text
Le profileur (VS Profiler / dotnet-trace) indique que ce code VB.NET (.NET 10) est un point chaud :
{colle la mesure — % CPU, allocations, temps}.
Propose des optimisations en VB.NET idiomatique, en préservant strictement le comportement.
- Tiens compte de ce que .NET 10 optimise déjà (JIT/PGO/SIMD) : ne réécris pas ce que le runtime gère.
- Pour chaque proposition, indique le gain attendu et le risque de régression à retester.
- Si une optimisation relève plutôt du C# (Span-first), signale-le au lieu de forcer en VB.

Code :
{colle le code}
```

---

## En résumé

Deux activités, deux arbitres. Pour le **débogage**, l'IA explique et formule des hypothèses avec brio, mais le **débogueur** observe ce qui se passe vraiment — ne laissez jamais *« ça paraît juste »* tenir lieu de vérification. Pour l'**optimisation**, l'IA suggère des améliorations théoriques, mais le **profileur** désigne le point chaud réel — mesurez d'abord, puis retestez la justesse de chaque changement.

La méthodologie complète est dans les modules [12](../12-exceptions-debogage/README.md) (débogage) et [14](../14-performance/README.md) (performance) ; le catalogue des pièges propres à l'IA, en [17.7](07-limites-pieges.md) ; et les erreurs courantes du couple IA / VB.NET, dans l'[Annexe G](../annexes/faq-depannage/README.md).

⏭️ [Workflow IA-first : Copilot dans Visual Studio 2026, VS Code, agents](/17-developpement-ia/06-workflow-ia-first.md)
