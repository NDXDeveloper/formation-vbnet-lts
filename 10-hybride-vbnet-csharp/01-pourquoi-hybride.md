🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 10.1 Pourquoi l'hybride : la réponse pragmatique au gel du langage

> **Ni abandonner VB, ni ignorer le reste de .NET : pourquoi l'architecture hybride est la réponse raisonnée — et non un compromis par défaut — au statut figé du langage.**

Le README de ce chapitre a posé la thèse en quelques lignes. Cette section en fait la **démonstration**, en partant honnêtement du problème, en examinant les réponses possibles, et en montrant pourquoi l'hybride est celle qui maximise la valeur au moindre coût et au moindre risque.

---

## Le constat : un langage figé, un écosystème qui avance

Le point de départ a été établi dès le module 1 : depuis 2020, Visual Basic .NET est un langage **stabilisé**. Microsoft le maintient — sécurité, compatibilité du *runtime*, expérience Visual Studio — mais **n'y ajoute plus de nouvelle syntaxe**. Le langage est figé à la version **16.9**.

La conséquence pratique se mesure dans le temps. **C# continue, lui, de s'enrichir à chaque version** : *records*, filtrage de motifs (*pattern matching*) toujours plus expressif, constructeurs primaires, expressions de collection, améliorations autour de `Span` et des `ref struct`, et ainsi de suite. VB, lui, n'évolue plus sur ce terrain. L'écart **se creuse** donc mécaniquement, version après version — non par négligence, mais par **choix stratégique assumé** de Microsoft.

Il faut le dire sans détour : au **niveau du langage**, VB.NET prend progressivement du retard sur C#. C'est le problème réel que ce chapitre cherche à résoudre — pas à nier.

---

## Trois réponses possibles — et pourquoi une seule tient

Face à ce constat, trois attitudes sont concevables. Deux d'entre elles ne résistent pas à l'examen.

| Réponse | L'idée | Pourquoi elle ne suffit pas |
|---------|--------|------------------------------|
| **Tout réécrire en C#** | Migrer l'existant sans attendre | Coûteux et risqué ; jette du code **éprouvé** et l'**expertise** de l'équipe ; rarement justifié à l'échelle globale (→ modules 11 et 18) |
| **Rester en VB pur** | Se passer des nouveautés | Viable pour certaines applications, mais **coupe** des gains de performance, de fonctionnalités et de briques d'écosystème réellement utiles ; peut devenir une **vraie contrainte** |
| **Attendre que VB rattrape C#** | Patienter | **Contraire à la stratégie explicite** : cela n'arrivera pas. Attendre n'est pas un plan |

La réécriture intégrale traite parfois un cas particulier (un projet où la cible C# s'impose — voir le module 18), mais ce n'est pas une réponse *générale* : elle sacrifie un capital existant et fonctionnel. Le repli sur VB pur enferme l'application à mesure que l'écart se creuse. Quant à l'attente, elle revient à miser sur un revirement que Microsoft a explicitement écarté.

**Aucune des deux postures extrêmes n'est satisfaisante** — ni la loyauté dogmatique à VB qui ignorerait le reste du monde, ni l'abandon réflexe qui réécrirait tout. La voie médiane n'est pas ici un pis-aller : c'est, précisément, l'**optimum**.

---

## Le retournement décisif : consommer n'est pas écrire

Tout le raisonnement repose sur une distinction que le chapitre 9 a rendue concrète :

> **Le gel du langage porte sur ce que l'on *écrit* en VB. Il ne porte pas sur ce que l'on *consomme*.**

Un *record* C#, une API fondée sur `Span`, un type produit par un générateur de source : **une fois compilés**, ce ne sont que des **types .NET ordinaires**. VB les consomme sans difficulté (section 9.3). Dès qu'une fonctionnalité peut être **encapsulée derrière une frontière d'assembly** et appelée, le gel **cesse de la concerner**.

Cela change la nature même du problème. Le gel n'est pas un **mur** derrière lequel VB serait bloqué ; c'est une **ligne qui ne compte qu'au point de rédaction**. Tout ce qui se trouve de l'autre côté reste accessible — il suffit de le faire écrire en C#, puis de le consommer.

### Un exemple concret

Supposons que votre application VB.NET doive analyser de **gros fichiers CSV** avec un minimum d'allocations. En VB pur, vous ne pouvez pas écrire le code `Span`-first le plus efficace : la manipulation fine des `ref struct` y est limitée (section 9.3). Vous voilà, en apparence, prisonnier du gel.

Mais il suffit d'une **petite bibliothèque C#** — quelques dizaines de lignes — exposant une méthode au contrat trivial :

```vb
' Côté C# : public IEnumerable<Enregistrement> ParseLignes(Stream flux)
Dim enregistrements = analyseur.ParseLignes(flux)   ' une simple référence suffit
For Each e In enregistrements
    ' ... logique métier en VB.NET ...
Next
```

La performance vit en C#, derrière une frontière ; la logique métier reste en VB, là où elle est lisible et productive. **À aucun moment le gel du langage ne vous a gêné** : il ne portait que sur ce que vous auriez écrit *en VB*.

---

## VB.NET reste pertinent pour l'essentiel du travail

L'argument précédent neutralise le gel pour ce qu'on délègue. Encore faut-il rappeler **tout ce pour quoi VB.NET demeure pleinement adapté** — et c'est considérable.

Une grande part du travail réel d'une application n'a **aucun besoin** des fonctionnalités de langage les plus récentes : les **interfaces de bureau** (Windows Forms ⭐, WPF), la **logique métier**, les **règles de gestion**, l'**accès aux données**, l'**orchestration**, la **gestion d'événements**, les **requêtes LINQ**. VB fait tout cela **bien** et de façon **idiomatique**.

Les nouveautés syntaxiques de C# apportent souvent de la **concision** ou de l'**expressivité** sur des motifs précis (records pour des DTO, expressions de collection, filtrage de motifs) — précieux, mais ce n'est **pas là** que se concentre l'essentiel de la valeur d'une application de gestion ou de bureau.

Il faut même renverser la perspective : la **stabilité** de VB est, en soi, un **atout**. Pas de remous, pas de changement de langage cassant, un code écrit aujourd'hui qui continuera de compiler demain. Pour des applications **à longue durée de vie** et **à forte maintenance** — le terrain de prédilection de VB —, c'est une qualité, pas un défaut. C'est d'ailleurs ce que protège la doctrine officielle, qui cible explicitement les **scénarios cœur** de VB (Windows Forms, bibliothèques) et l'expérience Visual Studio.

---

## Une architecture saine : isoler la volatilité

Vu sous l'angle de l'ingénierie, l'hybride n'est pas une rustine : c'est un cas classique de **séparation des responsabilités**, dont la frontière coïncide avec les forces de chaque langage.

Mieux : il **isole la volatilité**. Les fonctionnalités récentes du langage et de la plateforme **bougent vite** — elles vivent, dans cette architecture, dans des **bibliothèques C#** clairement délimitées. La couche d'interface et de métier en VB.NET, elle, **reste stable**. C'est exactement le principe que l'on applique lorsqu'on isole une dépendance changeante derrière une interface : ici, la « chose changeante » est *l'ensemble des nouveautés du langage moderne*, cantonné côté C#.

Cette logique n'a, du reste, rien d'exotique. Les grandes bases de code .NET sont **déjà polyglottes** dans les faits ; l'hybride ne fait que rendre cette réalité **intentionnelle et maîtrisée**.

---

## L'hybride *est* la stratégie officielle

Un point souvent négligé : cette architecture ne **contredit pas** Microsoft, elle en **applique** la stratégie. La doctrine officielle ne se contente pas de figer le langage ; elle indique aussi où investir — l'**interopérabilité avec C#**, en particulier dans les scénarios cœur que sont **Windows Forms** et les **bibliothèques**.

> *« Nous continuerons à investir dans l'expérience Visual Studio et l'interopérabilité avec C#, en particulier dans les scénarios cœur de Visual Basic comme Windows Forms et les bibliothèques. »*
> — *Stratégie du langage Visual Basic*, Microsoft Learn (citée en tête du [Sommaire](/SOMMAIRE.md))

L'architecture hybride est la **traduction architecturale** de cette phrase. Faire écrire les briques modernes en C# et les consommer depuis une application VB, c'est **faire exactement ce que la stratégie suggère** — pas la contourner.

---

## 🤖 La synergie avec l'IA

L'ère de l'IA renforce encore la pertinence de cette approche. Travailler en hybride, c'est **lire et transposer du C# en permanence** — or les modèles d'IA sont **majoritairement entraînés sur du C#** (module 1.6.3). Les deux se répondent :

- l'IA est particulièrement à l'aise pour **produire les briques C#** que l'on souhaite isoler ;
- elle excelle à **traduire** un exemple C# trouvé dans la documentation vers le VB qui le consommera.

La frontière VB/C# de l'hybride et l'assistance par IA se **renforcent mutuellement** — un thème approfondi au **module 17**.

---

## ⚠️ À quelles conditions l'hybride tient ses promesses

Honnêteté oblige : l'hybride n'est ni gratuit ni universellement nécessaire. Ses bénéfices ne se matérialisent que sous certaines **conditions** :

- **N'y recourir que lorsque la délégation est justifiée.** Pour une application de bureau ou de gestion entièrement en VB, sans besoin de fonctionnalités de pointe, on peut très bien n'écrire **jamais** la moindre ligne de C#. La question du *quand* est l'objet de la **section 10.2**.
- **Assumer le coût d'une solution mixte** : deux langages, une charge cognitive et des compétences d'équipe supplémentaires, un build et des tests à organiser (**section 10.6**).
- **Concevoir une frontière « VB-friendly »** : des constructeurs explicites (VB règle certes les propriétés `init` via `With { }`, mais le constructeur reste la voie la plus directe), pas de `ref struct` dans la surface publique consommée par VB, pas de membres distingués par la seule casse, conformité **CLS** (rappels de la section 9.3).

Ces points ne sont pas des arguments **contre** l'hybride : ce sont les **conditions** de sa réussite. Bien menée, l'architecture transforme le gel du langage en une manière de travailler **propre, stable et durable**.

---

## Pourquoi « pragmatique »

Le titre de cette section n'emploie pas le mot par hasard. L'hybride est **pragmatique** parce qu'il refuse les deux postures de principe :

- ni la **loyauté dogmatique** à VB, qui ignorerait l'écosystème et s'enfermerait ;
- ni l'**abandon réflexe**, qui réécrirait tout en C# au prix d'un capital existant.

Il **maximise la valeur** en gardant ce qui fonctionne, en ajoutant ce qui est nécessaire, **au plus bas coût et au plus faible risque**. C'est moins une concession qu'une **décision d'ingénierie lucide**.

---

## En résumé

- Le gel du langage est **réel** : VB.NET est figé à la 16.9 tandis que C# continue d'évoluer ; l'écart se creuse au niveau du **langage**.
- Les deux réponses extrêmes échouent : **tout réécrire** (coûteux, risqué) et **rester en VB pur** (s'enfermer) ; **attendre** n'est pas un plan.
- L'insight central : **consommer n'est pas écrire**. Le gel ne porte que sur ce qu'on rédige en VB ; tout ce qu'on **encapsule en C# et consomme** échappe au gel (chapitre 9).
- VB reste **pleinement pertinent** pour l'essentiel du travail (UI, métier, données, LINQ, événements), et sa **stabilité** est un atout pour le code à longue durée de vie.
- L'hybride est une **architecture saine** (séparation des responsabilités, isolation de la volatilité) et l'**expression de la stratégie officielle** de Microsoft (interopérabilité avec C#).
- Il a un **coût** et des **conditions** (délégation justifiée, solution mixte, frontière VB-friendly) — non des objections, mais les clés de sa réussite.

> 🔗 **Suite logique** : la section **10.2 — Quand déléguer à C#** passe de la justification aux **critères de décision** concrets.

⏭️ [Quand déléguer à C# (perf/Span, records, source generators, Minimal APIs, Native AOT, Blazor/MAUI)](/10-hybride-vbnet-csharp/02-quand-deleguer.md)
