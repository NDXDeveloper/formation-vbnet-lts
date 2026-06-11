🔝 Retour au [Sommaire](/SOMMAIRE.md)


# 4.5 Parallélisme pragmatique (`Parallel.For`/`ForEach`, PLINQ)

Jusqu'ici, ce module traitait de l'asynchronie d'**E/S** : ne pas bloquer un thread pendant qu'on attend une ressource. Le **parallélisme** est l'autre forme de concurrence : exécuter plusieurs **calculs** simultanément sur plusieurs cœurs, pour réduire le temps total d'un travail intensif en processeur (voir la distinction posée en [4.1](01-pourquoi-async.md)). Le mot « **pragmatique** » du titre est volontaire : le parallélisme n'est ni gratuit, ni systématiquement plus rapide. Cette section présente les outils — `Parallel.For`/`ForEach`, PLINQ — et surtout *quand* ils valent la peine.

---

## Parallélisme de données vs de tâches

Deux familles, déjà entrevues :

- Le **parallélisme de tâches** exécute des opérations **différentes** de front : on l'a vu en [4.2](02-async-await.md) avec `Task.Run` + `Task.WhenAll`.
- Le **parallélisme de données** applique la **même** opération à de **nombreux éléments** en parallèle. C'est le sujet ici : `Parallel.For`, `Parallel.ForEach` et PLINQ.

---

## `Parallel.For` et `Parallel.ForEach`

Ces méthodes répartissent automatiquement les itérations sur les threads du pool. Le corps de la boucle s'exécute donc **simultanément** sur plusieurs threads.

```vb
' Calcul lourd et indépendant sur chaque élément
Parallel.For(0, points.Length,
             Sub(i)
                 resultats(i) = CalculerTrajectoire(points(i))   ' calcul intensif
             End Sub)
```

```vb
' Même principe sur une collection
Parallel.ForEach(documents,
                 Sub(doc)
                     Indexer(doc)
                 End Sub)
```

L'exemple ci-dessus est correct **précisément parce que** chaque itération écrit dans un emplacement distinct (`resultats(i)`) : deux threads ne touchent jamais la même case. Dès qu'un état est *partagé*, tout change — voir plus bas.

---

## Contrôler le parallélisme : `ParallelOptions`

`ParallelOptions` règle deux choses essentielles : le **degré de parallélisme** maximal et l'**annulation** (le `CancellationToken` de [4.4](04-annulation-timeout.md)).

```vb
Dim options As New ParallelOptions With {
    .MaxDegreeOfParallelism = Environment.ProcessorCount,
    .CancellationToken = ct
}

Parallel.For(0, points.Length, options,
             Sub(i)
                 resultats(i) = CalculerTrajectoire(points(i))
             End Sub)
' Si ct est annulé, Parallel.For lève OperationCanceledException.
```

Limiter `MaxDegreeOfParallelism` est utile pour ne pas saturer la machine (par exemple si le traitement sollicite déjà fortement le disque, ou pour laisser des cœurs au reste de l'application).

---

## Interrompre une boucle : `ParallelLoopState`

Pour sortir avant la fin, on demande l'état de boucle dans le corps et on appelle `Break()` ou `Stop()` :

```vb
Parallel.For(0, elements.Length,
             Sub(i, etat)
                 If EstLaCible(elements(i)) Then etat.Stop()
                 Traiter(elements(i))
             End Sub)
```

- `Stop()` arrête **au plus vite**, sans garantie sur les itérations déjà lancées.
- `Break()` empêche le démarrage des itérations d'**indice supérieur**, mais laisse finir les inférieures (utile quand l'ordre a un sens).

`Parallel.For` renvoie un `ParallelLoopResult` indiquant si la boucle s'est terminée complètement.

---

## PLINQ (`AsParallel`)

PLINQ parallélise une requête LINQ de façon déclarative : il suffit d'insérer `AsParallel()` dans la chaîne.

```vb
Dim premiers = nombres.AsParallel().Where(Function(n) EstPremier(n)).ToList()
```

On affine avec quelques opérateurs dédiés :

```vb
Dim resultats = source.AsParallel() _
                      .WithDegreeOfParallelism(4) _
                      .WithCancellation(ct) _
                      .Where(Function(x) Filtrer(x)) _
                      .Select(Function(x) Transformer(x)) _
                      .ToList()
```

À connaître : `AsOrdered()` préserve l'ordre des résultats (à un coût non négligeable), `AsSequential()` repasse en séquentiel pour la suite de la chaîne, et `ForAll(...)` effectue une itération parallèle avec effet de bord (sans collecter de résultat) — à n'utiliser qu'avec un récepteur thread-safe :

```vb
nombres.AsParallel().ForAll(Sub(n) sacConcurrent.Add(Transformer(n)))
```

PLINQ n'est avantageux que si le travail par élément est **suffisamment lourd** pour amortir le surcoût de partitionnement et de fusion. Pour des opérations triviales, un LINQ séquentiel est plus rapide.

---

## Le danger n°1 : la sécurité des threads

C'est l'écueil central du parallélisme — et il est **identique en VB et en C#**. Comme le corps s'exécute sur plusieurs threads à la fois, tout **état mutable partagé** devient une **course critique** (*race condition*) :

```vb
' FAUX — plusieurs threads modifient 'somme' simultanément
Dim somme As Long = 0
Parallel.For(0, valeurs.Length,
             Sub(i)
                 somme += valeurs(i)        ' lecture-modification-écriture non atomique → résultat erroné
             End Sub)
```

`somme += x` n'est pas une opération atomique : deux threads peuvent lire la même valeur avant de l'écrire, et une addition est perdue. Le résultat est faux *et* non déterministe. Trois remèdes :

**Agrégation locale par thread** — c'est la solution idiomatique pour cumuler un résultat. La surcharge `Parallel.For` avec `localInit`/`localFinally` donne à chaque thread son propre sous-total, fusionné une seule fois à la fin :

```vb
Dim somme As Long = 0
Parallel.For(0, valeurs.Length,
             Function() 0L,                                       ' init : sous-total local
             Function(i, etat, sousTotal) sousTotal + valeurs(i), ' accumulation sans verrou
             Sub(sousTotal) Interlocked.Add(somme, sousTotal))    ' fusion atomique
```

**Collections concurrentes** — `ConcurrentBag(Of T)`, `ConcurrentDictionary(Of K, V)`, etc., conçues pour les accès simultanés.

**Opérations atomiques** — `Interlocked.Add`, `Interlocked.Increment`… pour de simples compteurs.

> Les primitives de synchronisation (`SyncLock`, `Interlocked`, `SemaphoreSlim`) et les collections thread-safe font l'objet de la section [4.7](07-synchronisation.md). Retenez ici la règle : **un corps de boucle parallèle ne doit jamais écrire dans un état partagé sans protection.**

---

## Les exceptions dans une boucle parallèle

Si des itérations échouent, leurs exceptions sont regroupées dans une **`AggregateException`** (le même type qu'en [4.3](03-exceptions-async.md)) :

```vb
Try
    Parallel.For(0, n, Sub(i) PeutEchouer(i))
Catch ex As AggregateException
    For Each interne In ex.InnerExceptions
        _journal.Erreur("Itération en échec", interne)
    Next
End Try
```

---

## Concurrence d'E/S : `Parallel.ForEachAsync` (depuis .NET 6)

`Parallel.For`/`ForEach` sont faits pour le **CPU**. Pour lancer de nombreuses opérations **asynchrones d'E/S** avec une concurrence *bornée* — typiquement « appeler une API pour 1 000 éléments, mais 10 à la fois » —, l'outil adapté est `Parallel.ForEachAsync`, disponible depuis .NET 6.

Mais ici surgit une **vraie limite de VB.NET** ⚠️. Le corps attendu par `Parallel.ForEachAsync` est un délégué renvoyant un **`ValueTask`** — or **VB ne sait pas écrire de méthode ou de lambda `Async` renvoyant un `ValueTask`** : le compilateur la rejette (erreur *BC36945* — le modificateur `Async` n'est admis que sur un `Sub` ou une `Function` renvoyant `Task`/`Task(Of T)`). On ne peut donc **pas** passer de lambda `Async` directe comme on le ferait en C#.

Le contournement est simple : on écrit une méthode `Async … As Task` classique, et on l'**enveloppe** dans un `ValueTask` au moyen d'une lambda *non* asynchrone :

```vb
Dim options As New ParallelOptions With {.MaxDegreeOfParallelism = 10}

Await Parallel.ForEachAsync(urls, options,
    Function(url, ct) New ValueTask(TraiterUrlAsync(url, ct)))   ' lambda NON-async

Private Async Function TraiterUrlAsync(url As String,
                                       ct As CancellationToken) As Task
    Dim contenu = Await _client.GetStringAsync(url, ct)
    Await EnregistrerAsync(contenu, ct)
End Function
```

La lambda renvoie `New ValueTask(uneTask)` : le constructeur de `ValueTask` emballe la `Task` produite par la méthode asynchrone. (La consommation de `ValueTask` en général est abordée en [4.6](06-async-streams.md).) C'est un cas d'école du « biais C# » : la documentation et les assistants IA proposeront une lambda `Async`… qui ne compile pas en VB.

---

## Quand le parallélisme aide vraiment

Le cœur du « pragmatique ». Paralléliser n'est rentable que si le gain dépasse le **surcoût** (partitionnement du travail, coordination des threads, fusion des résultats). Quelques repères :

- **Travail par élément substantiel** : si chaque itération est un calcul coûteux, paralléliser paie. Pour des opérations triviales (incrémenter, copier), le surcoût dépasse le gain — le **séquentiel** est plus rapide.
- **Éléments indépendants** : pas de dépendance d'un élément à l'autre, et idéalement pas d'état partagé.
- **Beaucoup d'éléments** : sur une poignée d'items, l'ordonnancement coûte plus qu'il ne rapporte.
- **Travail CPU, pas E/S** : on **n'utilise jamais** `Parallel.For` pour de l'E/S — cela monopolise des threads à ne rien faire. Pour de l'E/S concurrente, c'est `Task.WhenAll` ou `Parallel.ForEachAsync`.
- **Mesurer** : l'intuition trompe. On vérifie le gain réel (Benchmark.NET, voir [13.6](../13-tests-qualite/06-benchmarkdotnet.md)).

Enfin, garder à l'esprit la **loi d'Amdahl** : l'accélération est plafonnée par la part **séquentielle** du programme. Huit cœurs ne donnent presque jamais un facteur huit — la portion non parallélisable (et le surcoût de coordination) limite le résultat.

---

## En résumé

- Le parallélisme vise le travail **CPU** : `Parallel.For`/`ForEach` répartissent les itérations, PLINQ parallélise une requête LINQ.
- `ParallelOptions` règle `MaxDegreeOfParallelism` et l'annulation ; `ParallelLoopState` (`Break`/`Stop`) interrompt la boucle.
- **Danger n°1** : la **sécurité des threads** — jamais d'écriture dans un état partagé sans protection ; agrégation locale, collections concurrentes ou `Interlocked` (voir [4.7](07-synchronisation.md)).
- Les exceptions des itérations sont regroupées dans une `AggregateException`.
- **Limite VB** ⚠️ : pas de lambda `Async` pour `Parallel.ForEachAsync` (VB n'écrit pas de `Async … As ValueTask`) — on enveloppe une `Async … As Task` dans `New ValueTask(...)`.
- **Pragmatique** : ne paralléliser que du travail CPU substantiel, indépendant et nombreux ; jamais pour de l'E/S ; toujours **mesurer**.

Reste un dernier pan de la concurrence à couvrir côté *consommation* : recevoir des données au fil de l'eau avec les flux asynchrones, et le type `ValueTask` croisé à l'instant.


⏭️ [Consommer IAsyncEnumerable / flux asynchrones ; ValueTask (notions)](/04-async/06-async-streams.md)
