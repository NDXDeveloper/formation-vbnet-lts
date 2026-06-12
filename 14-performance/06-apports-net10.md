🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 14.6 — Ce que .NET 10 apporte gratuitement 🆕

**JIT, PGO, SIMD, dévirtualisation : des gains de performance sans changer une ligne de code**

---

## 🎯 La meilleure nouvelle pour VB.NET

Voici, sans doute, le chapitre le plus encourageant du module — et il l'est tout
particulièrement pour VB.NET. Le langage est **figé** (→ **[module 1.6](../01-introduction-vbnet/06-positionnement-2026.md)**),
mais les améliorations de performance de .NET 10 se situent **sous** le langage : dans le
**runtime**, le compilateur **JIT** et le **ramasse-miettes**. Elles sont donc **agnostiques du
langage** et s'appliquent **intégralement** à VB.NET.

Autrement dit : une application VB.NET reciblée vers .NET 10 reçoit **exactement les mêmes**
accélérations qu'une application C#. Le gel du langage vous prive de nouvelle **syntaxe**, pas de
**vitesse**. Microsoft présente d'ailleurs .NET 10 comme sa version la plus performante à ce jour,
et l'essentiel de ces gains s'obtient **gratuitement** : il suffit de **recompiler**.

---

## 🆓 Pourquoi c'est « gratuit »

Le code VB.NET et C# compile vers le **même IL**, exécuté sur le **même runtime**. Quand ce
runtime améliore la façon dont il **traduit l'IL en code machine** (le JIT), **dispose les blocs
de code**, **alloue la mémoire** ou **collecte les déchets**, votre code en profite **sans
modification**. Le seul geste requis :

- **Recibler le projet vers `net10.0`** (propriété `TargetFramework`) et **recompiler**. C'est une
  modification d'**une ligne**, qui donne accès aux nouvelles API, aligne build et tests sur le
  runtime, et garantit le bénéfice complet des optimisations.

L'**optimisation guidée par le profil dynamique** (Dynamic PGO) et la **compilation par paliers**
sont par ailleurs **actives par défaut** : une partie des gains s'opère même **à l'exécution**,
sans réglage.

---

## 🚀 Le catalogue des gains automatiques

### JIT : inlining et dévirtualisation

Le compilateur JIT génère du meilleur code machine. .NET 10 inline davantage de méthodes, y compris certaines devenues éligibles à la dévirtualisation,
et introduit un cadre d'**analyse interprocédurale** (IPA). Surtout, le gain phare de la campagne
de « dé-abstraction » : le JIT peut désormais dévirtualiser et inliner les méthodes d'interface des tableaux.
Concrètement, parcourir un tableau via `IEnumerable(Of T)` ou `IList(Of T)` — autrefois pénalisé
par l'abstraction — devient bien plus rapide.

Ce point a une conséquence directe pour un **point fort de VB.NET**, **LINQ** (→ **[2.9](../02-fondamentaux-langage/09-linq.md)**) :
une grande partie des bibliothèques, dont LINQ, reposait sur l'idée qu'itérer via l'indexeur est plus rapide qu'itérer via IEnumerable —
un écart que .NET 10 réduit, accélérant LINQ sans rien changer à vos requêtes.

### Analyse d'échappement : moins d'allocations sur le tas

C'est peut-être le gain le plus élégant au regard de tout ce module. Une analyse d'échappement améliorée permet désormais d'allouer sur la pile des objets qui restaient sur le tas, notamment des énumérateurs dans certains scénarios,
ainsi que de petits **tableaux de types valeur** qui ne « s'échappent » pas de la méthode.

Souvenez-vous des sections 14.2 et 14.3 : chaque objet sur le tas est du travail pour le GC. Ici,
le JIT **élimine automatiquement** une partie de ces allocations — exactement le *churn* Gen 0 que
l'on cherchait à réduire à la main. Une part du travail d'optimisation d'hier devient **gratuite**
aujourd'hui.

### Structures : promotion physique en registres

Le JIT peut placer les membres d'une structure dans des registres plutôt qu'en mémoire (promotion physique), éliminant des accès mémoire, en particulier lors du passage d'une structure en argument.
.NET 10 améliore le cas où plusieurs membres partagent un même registre. Le bon usage des
**structures** (14.2) s'en trouve encore plus rentable.

### Disposition du code et boucles

.NET 10 réorganise le code en blocs de base selon une nouvelle approche, remplaçant l'ancien parcours en ordre post-fixe inverse (RPO),
ce qui densifie les chemins chauds et raccourcit les branchements. Côté boucles, l'inversion de boucle est améliorée, aux côtés de nouvelles stratégies d'allocation sur la pile et de changements de write-barrier ;
le déroulage et le **hissage des vérifications de borne** hors des boucles réduisent les
opérations redondantes.

### SIMD et accélération matérielle

.NET 10 ajoute de nouveaux chemins d'accélération matérielle, dont AVX10.2 et le SVE d'Arm64, avec une vectorisation améliorée.
Sur matériel compatible, les méthodes de la BCL **vectorisées en interne** (recherche, comparaison,
encodage, crypto) en bénéficient automatiquement. Des mesures rapportent des gains notables sur ARM64 — opérations cryptographiques, sérialisation JSON, manipulation de chaînes — avec, de surcroît, un avantage de coût des instances ARM64
(un argument à considérer pour les déploiements cloud, → **[module 15.5](../15-deploiement-devops/05-cloud-essentiels.md)**).

### Ramasse-miettes

Le GC en arrière-plan est optimisé pour réduire la fragmentation par compaction, et des changements de write-barrier diminuent l'usage mémoire et les pauses.
S'y ajoute **DATAS**, l'adaptation dynamique de la taille du tas Server par défaut depuis .NET 9 et
reconduite (→ **[14.3](03-gc.md)**) : une empreinte mémoire plus sobre, sans configuration.

### LINQ, encore

Au-delà de la dévirtualisation des tableaux, les méthodes Order() et OrderDescending() utilisent désormais un tri optimisé qui réduit les allocations et les comparaisons,
et le code généré pour les motifs courants (`Where`, `Select`, `Aggregate`) bénéficie de la
Dynamic PGO. LINQ, déjà recommandé en VB.NET pour la clarté, devient **plus rapide gratuitement**.

### Et tout ce qui passe par la BCL

Rappel de la **[14.4](04-span-pooling.md)** : la bibliothèque de base est largement réécrite autour
de `Span`. Chaque appel à une API standard (chaînes, *parsing*, formatage, E/S, JSON) capte ces
optimisations **sans la moindre ligne de `Span` de votre part**.

---

## ⚖️ Ce que VB.NET ne reçoit pas — et pourquoi cela ne change rien à la performance

Soyons honnêtes : .NET 10 s'accompagne de **C# 14**, dont VB ne bénéficie pas. C# 14 introduit notamment les propriétés adossées à un champ, de nouvelles conversions de span, l'opérateur d'affectation null-conditionnel et la prise en charge des interfaces sur les ref struct.

Mais il faut distinguer deux choses :

- Ces ajouts sont des **fonctionnalités de langage** (syntaxe, expressivité) — **pas** des
  optimisations de runtime. **Toutes** les accélérations du catalogue ci-dessus opèrent **sous** le
  langage et s'appliquent **pleinement** à VB.NET.
- Certaines de ces fonctionnalités C# 14 servent justement à **écrire** du code bas niveau (les
  interfaces sur *ref struct*, les nouvelles conversions de span). C'est exactement la **frontière
  de la 14.4** : si vous devez **rédiger** un *hot path* `Span`-intensif, c'est l'affaire d'une
  **bibliothèque C#** consommée depuis VB (→ **[module 10](../10-hybride-vbnet-csharp/README.md)** 🔗).
  La performance **à l'exécution**, elle, vous est acquise dans les deux langages.

En clair : **le langage figé coûte de la syntaxe, jamais de la vitesse.**

---

## 🧭 Comment en profiter concrètement

1. **Recibler vers `net10.0`** dans le `.vbproj` et **recompiler** — la modification d'une ligne
   qui débloque l'essentiel des gains et l'accès aux nouvelles API.
2. **Laisser les défauts faire leur travail** : Dynamic PGO et compilation par paliers sont déjà
   actives.
3. **Envisager ARM64** si le déploiement s'y prête (gains de calcul et coût réduit).
4. **Mesurer son gain réel.** Ces améliorations sont **dépendantes de la charge** : un code riche
   en allocations, en LINQ, en structures ou tournant sur ARM64 peut beaucoup gagner ; un autre,
   modérément. On ne **suppose** pas un pourcentage — on le **vérifie** avec un avant/après
   (→ **[14.1](01-profilage.md)**, et **[BenchmarkDotNet](../13-tests-qualite/06-benchmarkdotnet.md)**).

Pour le passage proprement dit depuis une version antérieure (analyse de dépendances, *breaking
changes*, checklist), voir **[module 11.3](../11-migration-legacy/03-framework-vers-net10.md)** et
l'**[Annexe E — Guide de migration vers .NET 10](../annexes/migration-net10/README.md)**.

---

## 🔁 En résumé

- Les gains de performance de .NET 10 vivent dans le **runtime / JIT / GC** : ils sont **agnostiques
  du langage** et profitent **intégralement** à VB.NET, langage figé compris.
- Au menu, **sans changer le code** : améliorations d'inlining, de dévirtualisation de méthodes et d'allocations sur la pile, support d'AVX10.2, meilleur code pour les arguments de structure et inversion de boucle améliorée —
  auxquels s'ajoutent une analyse d'échappement plus fine (moins d'allocations), une meilleure
  disposition du code, un GC plus sobre, et un LINQ plus rapide.
- Le seul geste : **recibler `net10.0`, recompiler, mesurer**. Dynamic PGO et compilation par
  paliers font le reste à l'exécution.
- Le gel de VB ne coûte que de la **syntaxe** (C# 14), jamais de la **vitesse** : les *hot paths*
  bas niveau à **écrire** restent l'affaire de C# (14.4), la performance à l'**exécution** est
  partagée.

---

## ✅ Conclusion du module 14

Le fil de ce module forme une boucle cohérente : **mesurer** avant tout (14.1), comprendre **où va
la mémoire** (14.2) et **comment le GC la gère** (14.3), **réduire les allocations** sur les
chemins critiques (14.4), en faire des **réflexes** disciplinés (14.5), et enfin **laisser la
plateforme** offrir le reste (14.6).

La leçon d'ensemble, fidèle au positionnement honnête de la formation : en VB.NET, la performance
ne tient ni à des astuces de bas niveau ni à un changement de langage, mais à de **bons choix de
conception**, à la **discipline des allocations**, et à un **runtime moderne** qui travaille pour
vous. Là où le langage cède la main — le bas niveau intensif — la **stratégie hybride** prend le
relais. Pour le reste, VB.NET sur .NET 10 est un socle rapide, stable et pleinement d'actualité.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Déploiement et DevOps](/15-deploiement-devops/README.md)
