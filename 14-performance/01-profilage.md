🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 14.1 — Profilage

**Mesurer avant d'optimiser : les outils de Visual Studio 2026 et la trousse `dotnet-*`**

---

## 🎯 Pourquoi profiler

L'optimisation guidée par l'intuition est, presque toujours, une perte de temps. Le code
que l'on *croit* lent l'est rarement, et le vrai goulot d'étranglement se cache souvent là
où on ne l'attend pas — une requête mal indexée, un appel réseau synchrone, une allocation
dans une boucle chaude. **Profiler, c'est remplacer la conjecture par la mesure.**

Un cycle d'optimisation sain tient en trois temps :

1. **Mesurer** une charge représentative pour localiser objectivement le coût (CPU,
   mémoire, attente).
2. **Corriger** le point identifié — un seul changement à la fois.
3. **Re-mesurer** pour confirmer le gain (et vérifier qu'on n'a pas déplacé le problème
   ailleurs).

Le profilage répond à deux grandes familles de questions, qui appellent des outils
différents :

- **« Où passe le temps CPU ? »** → profilage *temporel* (échantillonnage ou instrumentation).
- **« Où va la mémoire ? »** → profilage *mémoire* (allocations, rétention, comportement du GC).

> 💡 **Une note VB.NET, rassurante.** Tous les outils présentés ici opèrent au niveau du
> **runtime** (EventPipe, EventCounters, SOS), pas du langage de surface. Ils sont
> **totalement agnostiques** : il n'y a rien de spécifique à installer pour VB.NET, et les
> noms de méthodes s'affichent simplement avec la syntaxe VB. Une application VB.NET se
> profile **exactement** comme une application C#.

---

## 🧪 La bonne méthode de mesure

Quelques règles conditionnent la fiabilité de toute mesure. Les ignorer produit des chiffres
trompeurs.

- **Profiler en configuration `Release`, jamais `Debug`.** En `Debug`, le JIT désactive ses
  optimisations et ajoute des vérifications : les chiffres n'ont aucun rapport avec la réalité
  de production.
- **Tenir compte du préchauffage (*warm-up*).** Avec la compilation par paliers (*tiered
  compilation*) et l'optimisation guidée par le profil dynamique (Dynamic PGO), les
  **premières** exécutions d'une méthode passent par un code non encore optimisé. Les tout
  premiers échantillons sont donc plus lents que le régime stabilisé : on laisse l'application
  « chauffer » avant de conclure.
- **Utiliser une charge représentative.** Un jeu de données jouet ne révèle pas les coûts qui
  n'apparaissent qu'à l'échelle réelle.
- **Connaître l'effet observateur.** Toute mesure a un coût. L'**instrumentation** (qui compte
  chaque appel) est précise mais intrusive ; l'**échantillonnage** (qui photographie la pile à
  intervalle régulier) est moins précis mais bien plus léger — souvent le bon compromis.
- **Établir une *baseline*.** Sans point de référence avant modification, « plus rapide » n'est
  qu'une impression. Pour quantifier finement un micro-changement, on s'appuie sur
  **[BenchmarkDotNet](../13-tests-qualite/06-benchmarkdotnet.md)** (module 13.6).

---

## 🛠️ Deux familles d'outils

| Famille | Outils | Quand |
|---------|--------|-------|
| **Intégrés à l'IDE** | Outils de diagnostic et Profileur de performances de **Visual Studio 2026** | En développement, sur le poste, avec une UI riche et le code source sous les yeux. |
| **En ligne de commande** | `dotnet-counters`, `dotnet-trace`, `dotnet-dump`, `dotnet-gcdump` | Multiplateforme, scriptable, **utilisable en production** (faible surcharge, sans débogueur attaché). |

Les deux familles sont complémentaires : on diagnostique souvent un incident de production
avec les outils `dotnet-*`, puis on rejoue et on creuse le scénario sur le poste dans Visual
Studio.

---

## 🧰 Visual Studio 2026

### Les Outils de diagnostic (pendant le débogage)

Lors d'une session de débogage (F5), la fenêtre **Outils de diagnostic** affiche en direct
l'utilisation **CPU**, la consommation **mémoire** et la chronologie des **événements**. C'est
la première fenêtre à regarder pour un aperçu immédiat, sans configuration. Elle permet aussi
de prendre des **instantanés mémoire** (*snapshots*) à différents moments et de comparer le
nombre d'objets entre deux points — la base de la chasse aux fuites.

### Le Profileur de performances (Alt+F2)

Lancé **sans débogueur** (donc avec des mesures plus représentatives), via le menu
*Déboguer ▸ Profileur de performances*, il regroupe plusieurs outils ciblés :

- **Utilisation du processeur (CPU Usage)** — profil par échantillonnage : quelles méthodes
  consomment le temps CPU, avec l'arbre des appels et la vue *Hot Path*.
- **Utilisation de la mémoire (Memory Usage)** — instantanés du tas managé, comparables entre
  eux pour repérer ce qui grossit.
- **Suivi des allocations d'objets .NET (.NET Object Allocation Tracking)** — chaque allocation
  et sa pile d'appel : idéal pour traquer les allocations cachées qui nourrissent le GC.
- **Instrumentation** — comptage exact des appels et chronométrage précis (plus intrusif que
  l'échantillonnage).
- **.NET Async** — visualise les chaînes `Async`/`Await`, les temps d'attente et les points où
  le flux asynchrone se bloque (→ **[module 4](../04-async/README.md)**).
- **Base de données** et **E/S de fichiers** — durée et fréquence des requêtes et des accès
  disque, souvent les vrais coupables dans une application LOB.

> 🆕 Dans la lignée de son positionnement « AI-native », Visual Studio 2026 peut **assister la
> lecture** des rapports de profilage (résumer un chemin chaud, suggérer des pistes). Cette aide
> reste un confort : la décision et la validation par re-mesure restent à votre charge.

---

## 💻 La trousse `dotnet-*` (multiplateforme, production)

Ces quatre outils en ligne de commande s'installent comme des outils .NET globaux :

```bash
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-dump
dotnet tool install --global dotnet-gcdump
```

La plupart s'attachent à un processus en cours par son **nom** (`-n MonApp`) ou son **PID**
(`-p 1234`). Pour lister les processus .NET visibles :

```bash
dotnet-counters ps
```

### `dotnet-counters` — la surveillance en direct

Le premier réflexe pour répondre à *« est-ce que quelque chose ne va pas, et globalement
quoi ? »*. Il affiche en temps réel des **métriques** issues du runtime (et de bibliothèques
comme ASP.NET Core) : utilisation CPU, taille du tas par génération, **taux d'allocation**,
nombre de GC, exceptions levées, état du pool de threads, etc. Sa surcharge est **très faible**,
ce qui le rend sûr en production.

```bash
# Surveillance en direct des compteurs du runtime
dotnet-counters monitor -n MonApp --counters System.Runtime

# Collecte vers un fichier pour analyse ultérieure
dotnet-counters collect -n MonApp --format csv -o compteurs.csv
```

Ces métriques proviennent de l'API `System.Diagnostics.Metrics`. Vous pouvez en **exposer de
personnalisées** depuis votre code VB.NET — un point qui relève pleinement du périmètre
« consommation » du langage et rejoint l'**[observabilité](../12-exceptions-debogage/04-observabilite.md)**
(module 12.4) :

```vb
Imports System.Diagnostics.Metrics

Public Module Telemetrie
    Private ReadOnly Compteur As New Meter("MonApp.Metier", "1.0.0")

    Public ReadOnly CommandesTraitees As Counter(Of Long) =
        Compteur.CreateCounter(Of Long)("commandes-traitees")
End Module

' Ailleurs, à chaque commande traitée :
'   Telemetrie.CommandesTraitees.Add(1)
```

Que l'on observe ensuite en direct :

```bash
dotnet-counters monitor -n MonApp --counters MonApp.Metier
```

### `dotnet-trace` — le profil CPU détaillé sans débogueur

Quand `dotnet-counters` a confirmé un problème CPU, `dotnet-trace` répond à *« où, précisément,
le temps part-il ? »*. Il collecte une trace par **échantillonnage** (et des événements
EventPipe) sans débogueur attaché, avec une surcharge maîtrisée.

```bash
# Profil CPU par défaut (échantillonnage)
dotnet-trace collect -n MonApp --profile cpu-sampling

# Profils orientés mémoire/GC
dotnet-trace collect -n MonApp --profile gc-verbose

# Conversion vers un format lisible dans un visualiseur de flammes
dotnet-trace convert trace.nettrace --format speedscope
```

Le fichier `.nettrace` produit s'ouvre dans Visual Studio, ou se convertit (format
*speedscope* / *chromium*) pour une lecture en **graphe de flammes** (*flame graph*).

### `dotnet-gcdump` — l'analyse du tas managé (fuites mémoire)

Pour répondre à *« qu'est-ce qui occupe la mémoire managée, et qu'est-ce qui empêche sa
libération ? »*. Il capture un **instantané du tas managé** (`.gcdump`) : nombre d'objets par
type, tailles, et chaînes de rétention. Il est **bien plus léger** qu'un dump complet (il
déclenche un GC et provoque une courte pause, mais ne fige pas durablement le processus).

```bash
dotnet-gcdump collect -n MonApp
```

La technique classique de détection de fuite : prendre **deux instantanés** à quelques minutes
d'intervalle sous charge stable, puis **comparer** les types dont le nombre d'instances ne
cesse de croître. Le `.gcdump` s'ouvre dans Visual Studio (Hub de diagnostic) ou PerfView.

### `dotnet-dump` — l'image mémoire complète (blocages, plantages)

L'outil le plus puissant et le plus lourd. Il capture une **image mémoire complète** du
processus et fournit un analyseur en ligne de commande façon SOS. On le réserve aux *« pourquoi
ça se fige / pourquoi ça a planté ? »* : blocages (*deadlocks*), interblocages de threads, état
complet au moment d'un crash.

```bash
# Capture
dotnet-dump collect -n MonApp

# Analyse interactive
dotnet-dump analyze core_20260612_103245
```

Dans l'analyseur, quelques commandes SOS essentielles :

```text
> clrthreads        ' liste les threads managés
> clrstack          ' pile d'appels managée du thread courant
> dumpheap -stat    ' synthèse du tas : objets par type et taille totale
> gcroot <adresse>  ' qui maintient en vie l'objet à cette adresse
> syncblk           ' verrous détenus — utile pour diagnostiquer un deadlock
```

Comme il **fige le processus** le temps de la capture, on l'emploie avec discernement en
production. Pour automatiser la capture **au moment d'un crash**, le runtime expose des
variables d'environnement :

```bash
export DOTNET_DbgEnableMiniDump=1      # générer un dump à la levée d'une exception fatale
export DOTNET_DbgMiniDumpType=2        # 2 = Heap (inclut le tas managé)
```

> ℹ️ **Notion connexe — `dotnet-monitor`.** Pour la production conteneurisée, `dotnet-monitor`
> expose traces, dumps et métriques **à la demande** via une API HTTP et des déclencheurs, sans
> ouvrir d'accès interactif au conteneur. À garder en tête pour les déploiements Docker/Kubernetes
> (→ **[module 15.4](../15-deploiement-devops/04-docker.md)**).

---

## 📊 Quel outil pour quel besoin

| Outil | Mesure quoi | Surcharge | Sortie | Question type |
|-------|-------------|-----------|--------|---------------|
| **`dotnet-counters`** | Métriques live (CPU, GC, taux d'alloc, threads, exceptions) | Très faible | Console / CSV / JSON | « Quelque chose ne va pas — quoi, globalement ? » |
| **`dotnet-trace`** | Échantillonnage CPU + événements | Faible à modérée | `.nettrace` → *speedscope* | « Où, précisément, part le temps CPU ? » |
| **`dotnet-gcdump`** | Instantané du tas managé | Faible (brève pause GC) | `.gcdump` | « Qu'est-ce qui occupe / retient la mémoire ? » |
| **`dotnet-dump`** | Image mémoire complète du processus | Élevée (fige le processus) | Dump natif | « Pourquoi ça se fige / ça plante ? » |

### Du symptôme à l'outil

- **Application lente, cause inconnue** → `dotnet-counters` pour une vue d'ensemble, puis
  `dotnet-trace` (ou *CPU Usage* dans Visual Studio) pour cibler le chemin chaud.
- **Mémoire qui grimpe sans redescendre (fuite suspectée)** → deux `dotnet-gcdump` comparés
  (ou *Memory Usage* / suivi des allocations dans Visual Studio).
- **Application figée / interblocage** → `dotnet-dump` (`clrthreads`, `clrstack`, `syncblk`),
  ou les *piles parallèles* de Visual Studio.
- **Plantage en production** → capture automatique de dump (variables `DOTNET_DbgEnableMiniDump`)
  puis `dotnet-dump analyze`.
- **GC trop fréquents / pression gen-0** → suivi des allocations .NET dans Visual Studio, ou
  `dotnet-trace --profile gc-verbose`.
- **Latences `Async`/`Await`** → l'outil **.NET Async** de Visual Studio.

---

## ⚠️ Pièges fréquents

- **Profiler en `Debug`.** Première cause de chiffres faux : on profile toujours en `Release`.
- **Oublier le préchauffage.** Conclure sur les premiers échantillons, avant que le JIT à
  paliers n'ait optimisé le code, mène à des décisions erronées.
- **Confondre symptôme et cause.** Un pic CPU peut n'être que la conséquence d'un GC déclenché
  par un excès d'allocations : le vrai levier est alors la réduction d'allocations (→ 14.2),
  pas l'optimisation du code « chaud » apparent.
- **Charge non représentative.** Un goulot d'étranglement qui n'existe qu'à l'échelle réelle
  restera invisible sur un jeu d'essai minuscule.
- **Optimiser sans baseline ni re-mesure.** Sans avant/après chiffré, on ne sait pas si l'on a
  amélioré, dégradé, ou simplement déplacé le problème.

---

## 🔁 En résumé

Le profilage est le **point de départ obligé** de tout travail de performance : on mesure, on
cible, on corrige, on re-mesure. Visual Studio 2026 offre l'expérience la plus riche sur le
poste de développement ; la trousse `dotnet-*` couvre la production et le multiplateforme avec
une faible surcharge. Tous ces outils sont **identiques en VB.NET et en C#**, puisqu'ils
travaillent au niveau du runtime.

Armé de mesures fiables, on peut aborder la **mécanique** sous-jacente — et c'est l'objet des
sections suivantes : le coût des **allocations** et la distinction valeur/référence
(**[14.2](02-types-allocations.md)**), puis le fonctionnement et le réglage du **ramasse-miettes**
(**[14.3](03-gc.md)**).

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Types valeur vs référence, allocations](/14-performance/02-types-allocations.md)
