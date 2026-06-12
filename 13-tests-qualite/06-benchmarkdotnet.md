🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 13.6 — BenchmarkDotNet (notions)

> **Module 13 — Tests et qualité du code**
> Mesurer les performances avec **méthode**, plutôt qu'au chronomètre approximatif.

---

> 📐 **Section « notions ».** Cette dernière section du chapitre est une **ouverture** : comprendre
> ce qu'est un micro-benchmark, pourquoi le faire correctement est délicat, et quand y recourir.
> Elle prépare le [module 14 (performance)](../14-performance/README.md), qui approfondit le sujet.

---

## Le problème : « mesurer au chronomètre » est trompeur

Le réflexe naturel, pour comparer deux façons d'écrire un calcul, est d'entourer une boucle d'un
`Stopwatch` :

```vb
' ❌ Mesure naïve — résultats peu fiables
Dim sw = Stopwatch.StartNew()
For i = 1 To 1000000
    MaMethode()
Next
sw.Stop()
Console.WriteLine(sw.ElapsedMilliseconds)
```

Cette mesure est **rarement fiable**, pour plusieurs raisons :

- **Chauffe du JIT** : les premières exécutions paient la compilation à la volée (*JIT*) ; sans
  période de chauffe, on mesure surtout la compilation.
- **Interférence du ramasse-miettes** (*GC*) : une collecte survenue pendant la mesure fausse le
  résultat.
- **Bruit de mesure** : charge système, fréquence du processeur, ordre d'exécution… introduisent
  une variabilité importante.
- **Élimination de code mort** : si le résultat n'est pas utilisé, le compilateur peut **supprimer**
  le calcul — on mesure alors… rien.

## Qu'est-ce que BenchmarkDotNet ?

**BenchmarkDotNet** est la bibliothèque de **micro-benchmark** de référence en .NET. Elle prend en
charge tout ce que la mesure naïve néglige : elle exécute le code **de nombreuses fois**, dans des
**processus isolés**, avec une phase de **chauffe**, puis réalise une **analyse statistique** des
résultats (moyenne, écart-type, valeurs aberrantes écartées). On obtient ainsi des chiffres
**reproductibles** et exploitables — y compris la consommation **mémoire**.

C'est une **bibliothèque** : pleinement utilisable depuis VB.NET, comme tout l'outillage de ce
chapitre.

## L'anatomie d'un benchmark

On écrit une classe dont les méthodes à comparer portent l'attribut `<Benchmark>`, et l'on lance
l'exécution avec `BenchmarkRunner.Run(Of T)()`. Exemple classique : concaténation de chaînes par
`&` contre `StringBuilder`.

```vb
Imports BenchmarkDotNet.Attributes
Imports BenchmarkDotNet.Running
Imports System.Text

<MemoryDiagnoser>   ' mesure aussi les allocations mémoire
Public Class ConcatenationBenchmarks

    <Params(100, 10000)>          ' le benchmark sera exécuté pour chaque valeur
    Public Property NombreItems As Integer

    <Benchmark(Baseline:=True)>   ' la référence à laquelle comparer
    Public Function AvecConcatenation() As String
        Dim resultat = ""
        For i = 1 To NombreItems
            resultat &= i.ToString()
        Next
        Return resultat           ' on RENVOIE le résultat (anti-élimination de code mort)
    End Function

    <Benchmark>
    Public Function AvecStringBuilder() As String
        Dim sb As New StringBuilder()
        For i = 1 To NombreItems
            sb.Append(i)
        Next
        Return sb.ToString()
    End Function
End Class

Module Program
    Sub Main()
        BenchmarkRunner.Run(Of ConcatenationBenchmarks)()
    End Sub
End Module
```

Quelques éléments utiles : `<Params(…)>` exécute le benchmark pour plusieurs jeux de valeurs,
`<Baseline:=True>` désigne la référence pour le calcul de ratio, `<MemoryDiagnoser>` ajoute les
colonnes d'allocation, et `<GlobalSetup>` / `<GlobalCleanup>` permettent de préparer/libérer un
contexte hors mesure.

> 💡 **Renvoyez toujours un résultat** depuis une méthode de benchmark (ou consommez-le) : c'est
> ce qui empêche le compilateur d'« optimiser » le calcul en le supprimant.

## Lire les résultats

BenchmarkDotNet produit un **tableau de synthèse**. Schématiquement :

| Method | NombreItems | Mean | Error | StdDev | Ratio | Allocated |
|--------|------------:|-----:|------:|-------:|------:|----------:|
| AvecConcatenation | 10000 | … | … | … | 1.00 | … |
| AvecStringBuilder | 10000 | … | … | … | (bien moindre) | … |

Comment l'interpréter :

- **Mean** : temps moyen d'exécution. **Error** et **StdDev** disent la **fiabilité** de la mesure
  (plus c'est faible, plus c'est stable).
- **Ratio** : performance **relative** à la ligne de base (un ratio de 0,1 signifie « dix fois plus
  rapide que la référence »).
- **Allocated** : mémoire allouée par opération — souvent aussi décisif que le temps, car les
  allocations alimentent le travail du GC.

Sur cet exemple, on retrouve un résultat connu : la concaténation par `&` réalloue à chaque
itération, là où `StringBuilder` reste quasi constant — l'écart se creuse nettement à mesure que
`NombreItems` augmente.

## Micro-benchmark ≠ profilage ≠ test de charge

Distinction essentielle pour ne pas se tromper d'outil :

- **BenchmarkDotNet** sert à **comparer finement de petites portions de code** entre elles
  (micro-benchmark).
- Le **profilage** (VS Profiler, `dotnet-trace`…) sert à trouver **où** une application réelle
  passe son temps — c'est l'objet du [module 14.1](../14-performance/01-profilage.md).
- Le **test de charge** mesure le comportement **sous trafic** (montée en charge, débit) — encore
  autre chose.

L'ordre logique est d'abord **profiler** pour identifier le vrai goulet d'étranglement, **puis**
benchmarker les solutions candidates à cet endroit précis.

## Bonnes pratiques et pièges

- **Exécutez en configuration `Release`.** BenchmarkDotNet refuse (à juste titre) de produire des
  résultats sérieux en `Debug`.
- **Benchmarkez des scénarios réalistes** : des entrées représentatives, pas des cas
  artificiellement favorables.
- **Évitez l'optimisation prématurée.** « *L'optimisation prématurée est la racine de tous les
  maux* » (Knuth) : ne micro-optimisez pas un code qui n'est pas un point chaud. Mesurez pour
  **décider sur des données**, pas pour optimiser par réflexe.

> 🤖 **Un mot sur l'IA.** L'agent **`@profiler`** de Visual Studio 2026 peut même **générer et
> optimiser des benchmarks BenchmarkDotNet** et comparer les métriques avant/après. Comme les
> autres agents de VS 2026, il est toutefois **orienté C#** : côté VB.NET, vérifiez la prise en
> charge et validez le code produit (cf. [13.5](05-couverture-tests-ia.md) et le
> [module 17](../17-developpement-ia/README.md)).

---

## À retenir

Mesurer une performance « à la main » avec un `Stopwatch` est trompeur : chauffe du JIT,
ramasse-miettes, bruit et élimination de code mort faussent le résultat. **BenchmarkDotNet** fait
ce travail correctement — exécutions multiples en processus isolés, chauffe, statistiques, mesure
des allocations — et s'utilise sans réserve en VB.NET. On l'emploie pour des **micro-benchmarks**
ciblés (comparer deux implémentations), **après** avoir profilé pour localiser le vrai point chaud,
et **sans tomber dans l'optimisation prématurée**.

---

## ✅ Conclusion du chapitre 13

Ce chapitre a déroulé la **chaîne de la qualité** en VB.NET :

- **[13.1 — Tests unitaires](01-tests-unitaires.md)** : le socle (xUnit, NUnit, MSTest ; la
  plateforme MTP de .NET 10). 🆕
- **[13.2 — Mocking et TDD](02-mocking-tdd.md)** : isoler l'unité testée et laisser les tests
  guider la conception.
- **[13.3 — Tests d'intégration](03-tests-integration.md)** : vérifier les composants ensemble,
  contre une vraie infrastructure.
- **[13.4 — Analyse statique](04-analyse-statique.md)** : détecter des problèmes sans exécuter le
  code.
- **[13.5 — Couverture et IA](05-couverture-tests-ia.md)** : mesurer ce que les tests couvrent, et
  générer des tests avec discernement. 🤖
- **13.6 — BenchmarkDotNet** : mesurer les performances avec méthode.

Le fil rouge rejoint celui du module 12 : tous ces outils sont des **bibliothèques** et de
**l'outillage** — donc **pleinement accessibles en VB.NET**, qui dispose même de **modèles de
projet de test natifs**. Les seules réserves sont ponctuelles et clairement identifiées, et toutes
relèvent du **biais C# de l'écosystème** : **StyleCop** et **Roslynator** (C# uniquement), le flux
de génération de tests **`@test`** (C#, repli sur Copilot générique en VB), et les agents IA en
général. Partout ailleurs, VB.NET teste et se mesure exactement comme C#.

➡️ Chapitre suivant : **[14. Performance et gestion de la mémoire](../14-performance/README.md)** —
du micro-benchmark à l'optimisation réelle, en passant par le profilage et le GC.

⏭️ [Performance et gestion de la mémoire](/14-performance/README.md)
