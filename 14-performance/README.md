🔝 Retour au [Sommaire](/SOMMAIRE.md)

# ⚡ 14. Performance et gestion de la mémoire

**Mesurer avant d'optimiser, comprendre où va la mémoire, laisser le runtime travailler**
.NET 10 LTS · Visual Studio 2026 · VB.NET 16.9

---

## 🎯 De quoi parle ce module

La performance d'une application .NET se joue d'abord au niveau du **runtime** (le CLR),
pas au niveau du langage de surface. VB.NET et C# compilent vers le même IL, s'exécutent
sur le même CoreCLR, et partagent le même ramasse-miettes (GC), le même compilateur
just-in-time (JIT) et les mêmes optimisations. **À code équivalent, les performances sont
équivalentes** : il n'existe pas de « pénalité VB.NET » à l'exécution.

Ce module se concentre donc sur ce qui compte réellement et qui est, pour l'essentiel,
**indépendant du langage** : comprendre le coût des allocations, mesurer plutôt que
deviner, maîtriser le cycle de vie de la mémoire, et exploiter ce que le runtime offre
gratuitement. C'est un terrain où VB.NET est pleinement à sa place.

À une réserve près, clairement signalée : les primitives bas niveau les plus récentes —
`Span(Of T)`, `stackalloc`, SIMD écrit à la main — ne se **consomment** depuis VB.NET que
de façon encadrée (en expressions, via `Memory(Of T)` — précisé en 14.4), et s'**écrivent**
en C#. VB.NET ne déclare pas de *ref struct* et n'a pas de mot-clé `stackalloc` : pour les
chemins critiques (« hot paths ») fortement vectorisés ou sans allocation, la stratégie
hybride du cours s'applique ici aussi — on isole ces briques dans une bibliothèque C# que
l'on consomme depuis VB.NET (→ **[module 10 — Architecture hybride](../10-hybride-vbnet-csharp/README.md)** 🔗).

---

## 🧭 La hiérarchie honnête des leviers de performance

En VB.NET — comme en C# — l'optimisation prématurée et exotique est rarement la bonne
porte d'entrée. Les gains les plus importants viennent presque toujours, dans l'ordre :

1. **Le bon algorithme et la bonne structure de données.** Passer d'un parcours en O(n²)
   à O(n), ou d'une `List` parcourue linéairement à un `Dictionary`, dépasse de loin tout
   micro-réglage. C'est le levier numéro un, et il est entièrement à la portée de VB.NET.
2. **L'asynchronie correcte pour les E/S.** Ne pas bloquer de threads sur des opérations
   réseau, disque ou base de données (→ **[module 4 — Async](../04-async/README.md)**).
3. **La réduction des allocations inutiles.** Moins d'objets éphémères sur le tas, c'est
   moins de travail pour le GC et de meilleures performances soutenues.
4. **La mesure ciblée.** On profile pour trouver le vrai goulot d'étranglement, on optimise
   ce point précis, puis on re-mesure pour confirmer le gain.
5. **Les gains « gratuits » du runtime.** Recompiler vers .NET 10 suffit souvent à accélérer
   le code sans le modifier (voir 14.6).

Ce n'est qu'**ensuite**, et seulement sur les chemins critiques identifiés par la mesure,
que l'on envisage les primitives sans allocation (`Span`, *pooling*) — et, si elles se prêtent
mal à VB.NET, leur délégation à une bibliothèque C#.

> **Règle d'or du module :** *« Measure, don't guess. »* On ne devine pas la performance,
> on la mesure. Une intuition non vérifiée par un profileur ou un benchmark n'est qu'une
> hypothèse.

---

## 📋 Ce que vous saurez faire à l'issue de ce module

- **Profiler** une application VB.NET avec les outils de Visual Studio 2026 et les outils
  en ligne de commande (`dotnet-counters`, `dotnet-trace`, `dotnet-dump`, `dotnet-gcdump`)
  pour localiser objectivement les goulots d'étranglement CPU et mémoire.
- **Distinguer** types valeur et types référence, comprendre la pile (*stack*) et le tas
  (*heap*), et reconnaître les allocations cachées qui pèsent sur le GC.
- **Comprendre et régler** le ramasse-miettes : générations, modes de GC (*Workstation* /
  *Server*, concurrent / *background*), et libération déterministe des ressources via
  `IDisposable`, `Using` et les finaliseurs.
- **Consommer** `Span(Of T)` et `Memory(Of T)` depuis VB.NET, et réduire la pression mémoire
  par le *pooling* (`ArrayPool(Of T)`, `ObjectPool(Of T)`).
- **Appliquer** des bonnes pratiques de performance idiomatiques et réalistes en VB.NET,
  en sachant **où s'arrête le langage** et où la délégation à C# devient pertinente.
- **Tirer parti** des améliorations « gratuites » de .NET 10 (JIT, Dynamic PGO, SIMD dans
  la BCL, dévirtualisation) — des gains obtenus sans changer une ligne de code.

---

## 🗂️ Plan du module

Le module progresse de la **mesure** vers l'**action**, puis vers les **gains offerts par
la plateforme** :

| Section | Sujet | En bref |
|---------|-------|---------|
| **14.1** | [Profilage](01-profilage.md) | Les outils pour mesurer avant d'optimiser : VS Profiler, `dotnet-counters`/`trace`/`dump`/`gcdump`. |
| **14.2** | [Types valeur vs référence, allocations](02-types-allocations.md) | Pile et tas, coût des allocations, allocations cachées à débusquer. |
| **14.3** | [Garbage Collector](03-gc.md) | Générations, modes, *tuning*, et nettoyage déterministe (`IDisposable` / `Using`, finaliseurs). |
| **14.4** | [`Span(Of T)` / `Memory(Of T)`, pooling](04-span-pooling.md) | Réduire les allocations ; consommer `Span` depuis VB ; `ArrayPool` et `ObjectPool`. |
| **14.5** | [Bonnes pratiques VB.NET](05-bonnes-pratiques.md) | Habitudes de performance pragmatiques, et la frontière langage VB / C#. |
| **14.6** | [Ce que .NET 10 apporte gratuitement](06-apports-net10.md) 🆕 | JIT, PGO, SIMD, dévirtualisation : des gains sans toucher au code. |

L'ordre est volontaire : on **mesure** (14.1) avant de comprendre la **mécanique mémoire**
(14.2–14.3), puis on agit avec les **outils de réduction d'allocation** (14.4), on
**consolide** par les bonnes pratiques (14.5), et l'on termine par les **gains offerts par
la plateforme** (14.6) — souvent le meilleur rapport effort/résultat.

---

## 🔌 Prérequis et liens avec le reste du cours

Ce module suppose acquis :

- **[Module 2 — Fondamentaux](../02-fondamentaux-langage/README.md)** : types valeur vs
  référence, génériques et collections (les bases que ce module approfondit côté coût).
- **[Module 4 — Async](../04-async/README.md)** : l'asynchronie reste le premier levier de
  réactivité pour les applications d'E/S et d'interface.
- **[Module 13.6 — BenchmarkDotNet (notions)](../13-tests-qualite/06-benchmarkdotnet.md)** :
  pour quantifier rigoureusement un gain de performance en micro-benchmark.

Il éclaire et alimente :

- **[Module 10 — Architecture hybride VB.NET / C#](../10-hybride-vbnet-csharp/README.md)** 🔗 :
  la décision de déléguer un chemin critique `Span`-first / SIMD à une bibliothèque C#.
- **[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)** (B.7 :
  records, `init`, types `Span`-first — *consommables, non déclarables en VB*).

---

## ⚠️ Périmètre VB.NET pour ce module

- ✅ **Pleinement en VB.NET** : profilage, analyse des allocations, réglage du GC, gestion
  déterministe des ressources (`Using`), *pooling*, **consommation** de `Span`/`Memory`, et
  bénéfice intégral des optimisations runtime de .NET 10.
- ⚠️ **À déléguer à C# si nécessaire** (sur chemins critiques identifiés par la mesure) :
  l'**écriture** de code `Span`-first intensif, `stackalloc`, *ref structs* maison et boucles
  SIMD vectorisées à la main. VB.NET ne propose pas ces constructions d'auteur — il les
  *utilise*, ce qui suffit dans l'immense majorité des cas (→ module 10, annexe B).

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Profilage (VS Profiler, dotnet-counters/trace/dump/gcdump)](/14-performance/01-profilage.md)
