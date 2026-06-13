# 💻 Exemples du module 14 — Performance et gestion de la mémoire

La performance se joue au niveau du **runtime** (CLR/JIT/GC), pas du langage de surface : VB.NET
et C# compilent vers le même IL, **à code équivalent les performances sont identiques**. Ce
module est donc pleinement à la portée de VB.NET — à une réserve près, clairement signalée :
l'**écriture** de code `Span`-first intensif relève de C# (VB le **consomme**). Les sections
porteuses de code sont reconstruites ici en exemples **complets, compilés et exécutés**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

> 🔬 **Méthode.** Les allocations sont **mesurées** avec `GC.GetAllocatedBytesForCurrentThread()`
> (octets alloués sur le thread), avec préchauffage JIT préalable. Les valeurs exactes dépendent
> du runtime ; ce sont les **ordres de grandeur et comparaisons** qui comptent (et sont stables).

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Vérifié |
|---|---|---|
| **14.1** Profilage | [`14.1-profilage`](#141-profilage) | compteur `commandes-traitees` émis ; outils CLI documentés |
| **14.2** Types/allocations | [`14.2-types-allocations`](#142-types-allocations) | boxing 320 Ko vs 40 Ko ; énumérateur IEnumerable 40 Ko vs 0 |
| **14.3** GC | [`14.3-gc`](#143-gc) | Using/Dispose, pattern complet, promotion gen 0→1, IsServerGC=True |
| **14.4** Span/pooling | [`14.4-span-pooling`](#144-span-pooling) | AsSpan **0 octet** vs Substring 3,2 Mo ; ArrayPool/ObjectPool |
| **14.5** Bonnes pratiques | [`14.5-bonnes-pratiques`](#145-bonnes-pratiques) | Ordinal 0 vs ToLower 5,6 Mo ; pré-dim 40 Ko vs 131 Ko |
| **14.6** Apports .NET 10 | *(pas de projet)* | gains « gratuits » (JIT/PGO/SIMD/GC) → recibler `net10.0` |

---

## ▶️ Comment compiler et lancer

```bash
cd 14.1-profilage         && dotnet run -c Release
cd 14.2-types-allocations && dotnet run -c Release
cd 14.3-gc                && dotnet run -c Release
cd 14.4-span-pooling      && dotnet run -c Release
cd 14.5-bonnes-pratiques  && dotnet run -c Release
```

> Profiler/mesurer **en `Release`** (en `Debug`, le JIT n'optimise pas — chiffres trompeurs).

---

## 14.1-profilage

- **Section** : 14.1 · **Fichier** : `01-profilage.md`
- **Description** : la part **code** de la section — exposer une **métrique personnalisée**
  (`System.Diagnostics.Metrics` : `Meter` + `Counter`) observable en direct par
  `dotnet-counters`. Le gros de 14.1 (VS Profiler, `dotnet-counters`/`trace`/`dump`/`gcdump`) est
  de l'**outillage CLI**, agnostique du langage — documenté ici, non scriptable comme « exemple ».
- **Sortie attendue** (vérifiée) :
  ```text
  Compteur : nom=commandes-traitees ; meter=MonApp.Metier ; type=Counter`1
  5 commandes traitées — compteur incrémenté (émission sans erreur).
  ```
- **Comportement vérifié** : le compteur est créé et émis sans erreur. Observation live :
  `dotnet-counters monitor -n Profilage --counters MonApp.Metier`.

## 14.2-types-allocations

- **Section** : 14.2 · **Fichier** : `02-types-allocations.md`
- **Description** : sémantique de copie (`Structure` indépendante vs `Class` partagée),
  **boxing**/unboxing, et **mesures** : `ArrayList` (boxing) vs `List(Of Integer)`, énumérateur
  `List` (struct) vs via `IEnumerable` (boxé).
- **Sortie attendue** (vérifiée) :
  ```text
  [Copie] Structure : a.X=1, b.X=99 → indépendants   |   Class : x=y=99 → partagé
  [Allocations, N=10000]  ArrayList=320 056 octets ; List(Of Integer)=40 056 octets  (≈ 8×)
  [Tuples] ValueTuple IsValueType=True ; Tuple IsValueType=False
  [Énumérateur] For Each List=0 octet ; via IEnumerable=40 000 octets (énumérateur boxé)
  ```
- **Comportement vérifié** : le boxing alloue **~8×** plus que les génériques ; itérer via
  `IEnumerable(Of T)` **boxe** l'énumérateur (1000 parcours → 40 Ko) là où `List(Of T)` n'alloue
  **rien**.

## 14.3-gc

- **Section** : 14.3 · **Fichier** : `03-gc.md`
- **Description** : libération **déterministe** — `Using`→`Dispose`, **pattern Dispose complet**
  (avec `Finalize` en VB via `Overrides Sub Finalize`, et `GC.SuppressFinalize`),
  **`IAsyncDisposable`** libéré « à la main » (pas de `Await Using` en VB ; motif
  capturer/libérer/relancer) — et les **générations** (promotion d'un survivant, comptage des
  collectes). Le mode **Server GC** est activé par MSBuild et vérifié au runtime.
- **Sortie attendue** (vérifiée ; `*` = dépend de la machine) :
  ```text
  [Using] EstLibere=True
  [Dispose complet] EstLibere=True ; appels Dispose(disposing)=2 (idempotent)
  [IAsyncDisposable] EstLibere=True
  [Générations] gen initiale=0 ; après GC.Collect=1 ; promu=True
                collectes Gen 0 déclenchées = 31 (> 0)        *
  [Mode GC] IsServerGC=True ; LatencyMode=Interactive
  ```
- **Comportement vérifié** : `Dispose` garanti par `Using` ; le double `Dispose` est **idempotent** ;
  l'objet survivant est **promu** (0→1) ; le réglage `<ServerGarbageCollection>true>` prend effet
  (`IsServerGC=True`).
- **⚠️ Frontière VB rencontrée** : `Async` ne peut **pas** retourner `ValueTask` (**BC36945**) — on
  enveloppe une méthode `Async`/`Task` dans `New ValueTask(...)` pour implémenter
  `IAsyncDisposable.DisposeAsync`.

## 14.4-span-pooling

- **Section** : 14.4 · **Fichier** : `04-span-pooling.md`
- **Description** : `Span` consommé **uniquement en expression** (`Integer.Parse(s.AsSpan(...))`,
  `AsSpan().IndexOf`, `flux.Read(buf.AsSpan())`) ; **mesure** `Substring` (alloue) vs `AsSpan`
  (0 allocation) ; `Memory(Of T)` (déclarable, asynchrone) + `.Span(i)` ; **pooling**
  (`ArrayPool`, `ObjectPool`).
- **Sortie attendue** (vérifiée) :
  ```text
  [Span en expression] Integer.Parse(AsSpan(8,4))=1234 ; AsSpan().IndexOf(",")=3
  [Allocations, 100000 parses]  via Substring=3 200 000 octets ; via AsSpan=0 octet
  [Memory(Of Byte)] ReadAsync → lus=3 ; premier octet=7
  [ArrayPool] Rent(256) → Length=256 ; emprunte(0)=42
  [ObjectPool] message=Bonjour VB.NET
  ```
- **Comportement vérifié** : le *slicing* via `AsSpan` n'alloue **rien** (0 octet sur 100 000
  *parses*) là où `Substring` alloue 3,2 Mo. `Memory(Of T)` traverse l'`Await`.
- **⚠️ Frontière VB documentée** : **déclarer** une variable/paramètre/champ `Span`/`ReadOnlySpan`
  ne compile pas (**BC30668**) ; seules les **expressions éphémères** sont permises (la déclaration
  interdite est laissée en commentaire dans le code).

## 14.5-bonnes-pratiques

- **Section** : 14.5 · **Fichier** : `05-bonnes-pratiques.md`
- **Description** : réflexes distinctifs — **`Option Strict On`** (liaison **anticipée** via
  interface vs **tardive** via `Object`, isolée dans un fichier `Option Strict Off`), comparaisons
  **`Ordinal`** (0 allocation, indépendant de la culture) vs `ToLower` (alloue), et
  **pré-dimensionnement** des collections.
- **Sortie attendue** (vérifiée) :
  ```text
  [Liaison] anticipée=42 ; tardive=42 ; identiques=True
  [Comparaison] ToLower=True ; OrdinalIgnoreCase=True   |   ToLower=5 600 000 octets ; Ordinal=0 octet
  [Pré-dimensionnement] sans capacité=131 400 octets ; avec capacité=40 056 octets
  ```
- **Comportement vérifié** : liaison tardive et anticipée donnent le **même** résultat (la tardive
  étant résolue à l'exécution, plus lente) ; `OrdinalIgnoreCase` n'alloue **rien** vs `ToLower`
  (5,6 Mo) ; pré-dimensionner divise par ~3 les allocations (évite les réallocations par doublement).

## 14.6 — Apports .NET 10 (pas de projet)

- **Section** : 14.6 · **Fichier** : `06-apports-net10.md`
- Les gains de .NET 10 (JIT/inlining/dévirtualisation, analyse d'échappement, SIMD/AVX10.2, GC plus
  sobre, LINQ plus rapide) vivent **sous le langage** : **agnostiques**, ils profitent à VB.NET
  **sans changer le code**. Le seul geste : **recibler `net10.0` et recompiler** — ce que **tous**
  les exemples ci-dessus font déjà. Pas d'exemple autonome : ces gains se **mesurent** (avant/après)
  plutôt qu'ils ne s'« écrivent » (cf. 14.1 et BenchmarkDotNet, module 13.6).

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier `dotnet run`
(le paquet Microsoft.Extensions.ObjectPool de 14.4 est restauré depuis le cache).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
