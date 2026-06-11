🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 4. Programmation asynchrone et parallèle

L'asynchronie n'est pas un sujet « avancé » réservé aux experts : c'est une compétence de base pour toute application VB.NET moderne. Une interface Windows Forms ou WPF qui se fige pendant un appel réseau, une lecture de fichier ou une requête en base de données offre une expérience inacceptable, et un service qui traite ses requêtes en série s'effondre sous la charge. L'asynchronie répond précisément à ces deux problèmes : elle libère le thread d'interface pendant les temps d'attente et permet à un serveur de servir bien plus de requêtes avec les mêmes ressources.

**Bonne nouvelle pour VB.NET** : `Async`/`Await` sont des fonctionnalités de **première classe du langage**, strictement équivalentes à celles de C#. Contrairement à d'autres domaines où VB.NET montre ses limites (voir l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)), l'asynchronie relève essentiellement de la *consommation* d'API du runtime et du framework — exactement le terrain où VB.NET est pleinement pertinent. La quasi-totalité de ce module est donc directement applicable et recommandée en VB.NET ✅.

---

## VB.NET et l'asynchronie en 2026

Le modèle asynchrone de .NET — le *Task-based Asynchronous Pattern* (TAP), fondé sur `Task` et `Task(Of T)` — est stable depuis plus de dix ans et pleinement disponible en VB.NET (les mots-clés `Async` et `Await` existent depuis VB 11, bien avant le gel du langage à la version 16.9). Le compilateur réécrit une fonction asynchrone en machine à états, exactement comme en C# : il n'y a **aucune pénalité ni différence de comportement** entre les deux langages.

Une fonction asynchrone se déclare simplement, et `Await` se comporte comme en C# :

```vb
Async Function ChargerProfilAsync(id As Integer) As Task(Of Profil)
    Using client As New HttpClient()
        Dim json = Await client.GetStringAsync($"https://api.exemple.com/profils/{id}")
        Return JsonSerializer.Deserialize(Of Profil)(json)
    End Using
End Function
```

Comme le langage est figé, **aucune nouvelle syntaxe asynchrone ne sera ajoutée à VB.NET** ; en revanche, le modèle profite *gratuitement* des améliorations du runtime de .NET 10 (JIT, PGO, réduction des allocations) sans la moindre modification de code — voir le [module 14.6](../14-performance/06-apports-net10.md) 🆕.

Deux nuances méritent toutefois d'être signalées dès maintenant, car elles distinguent réellement VB.NET de C# :

> ⚠️ **Flux asynchrones (`IAsyncEnumerable`)** — VB.NET sait *consommer* un flux asynchrone, mais ne dispose **pas** de la syntaxe `Await For Each` de C# (`await foreach`). La consommation passe par une itération manuelle de l'énumérateur asynchrone (`GetAsyncEnumerator` / `MoveNextAsync`). VB ne peut pas non plus *produire* de flux : il n'existe pas d'itérateur asynchrone (`Async Iterator`). On reste donc côté consommation — voir la section 4.6.

> ⚠️ **Pas de modèle de projet « Worker »** — Le SDK .NET ne fournit pas de template *Worker Service* en VB.NET. Pour héberger des traitements de fond (`BackgroundService`), on câble le *Generic Host* **à la main** dans un projet Console — voir la section 4.8.

En dehors de ces deux points, le contenu de ce module s'écrit en VB.NET sans réserve.

---

## Asynchronie ≠ parallélisme

Le module traite deux notions liées, mais qu'il ne faut surtout pas confondre :

- L'**asynchronie** consiste à ne pas bloquer un thread pendant une *attente* (E/S réseau, disque, base de données). Elle améliore la **réactivité** (l'UI reste fluide) et la **montée en charge** (un serveur sert plus de requêtes), le plus souvent *sans* créer de threads supplémentaires.
- Le **parallélisme** consiste à exécuter plusieurs calculs *simultanément* sur plusieurs cœurs pour réduire le temps total d'un travail intensif en **CPU**. Il améliore le **débit**, au prix de threads réellement occupés en parallèle.

L'erreur la plus fréquente est de mélanger les deux : envelopper un appel réseau dans un `Task.Run` ne le rend pas « plus asynchrone » (on bloque simplement un thread d'arrière-plan au lieu du thread courant), et `Await` n'accélère jamais un calcul purement mathématique. Les sections 4.1 et 4.5 clarifient ces choix.

---

## Objectifs du module

À l'issue de ce module, vous serez en mesure de :

- comprendre **quand et pourquoi** recourir à l'asynchronie plutôt qu'au parallélisme ;
- écrire et composer des méthodes `Async`/`Await` renvoyant `Task` et `Task(Of T)`, et savoir pourquoi `Async Sub` doit rester réservé aux gestionnaires d'événements ;
- propager et **gérer correctement les exceptions** dans un contexte asynchrone ;
- mettre en place une **annulation coopérative** et des délais d'expiration via `CancellationToken` ;
- exploiter à bon escient le **parallélisme de données** (`Parallel.For`/`ForEach`, PLINQ) ;
- **consommer** des flux asynchrones (`IAsyncEnumerable`) et comprendre l'intérêt de `ValueTask` ;
- protéger l'**état partagé** entre threads (`SyncLock`, `Interlocked`, `SemaphoreSlim`) ;
- héberger des **traitements de fond** à l'aide du *Generic Host* et de `BackgroundService`.

---

## Plan du module

| Section | Sujet | En bref |
|---------|-------|---------|
| [4.1](01-pourquoi-async.md) | Pourquoi l'asynchronie | UI réactive, opérations d'E/S, et les idées fausses les plus courantes |
| [4.2](02-async-await.md) | `Async`/`Await` | Le cœur du module : `Task`, `Task(Of T)`, composition (`WhenAll`/`WhenAny`) |
| [4.3](03-exceptions-async.md) | Exceptions asynchrones | Propagation, `Try`/`Catch` autour de `Await`, cas de l'`AggregateException` |
| [4.4](04-annulation-timeout.md) | Annulation et timeout | `CancellationToken`, `CancellationTokenSource`, annulation coopérative |
| [4.5](05-parallelisme.md) | Parallélisme pragmatique | `Parallel.For`/`ForEach`, PLINQ : puissance et pièges |
| [4.6](06-async-streams.md) | Flux asynchrones | Consommer `IAsyncEnumerable` ⚠️ ; `ValueTask` (notions) |
| [4.7](07-synchronisation.md) | Synchronisation & thread-safety | `SyncLock`, `Interlocked`, `SemaphoreSlim` |
| [4.8](08-background-services.md) | Services en arrière-plan | *Generic Host*, `BackgroundService` ⚠️, services Windows |

---

## Prérequis

- **Module 2 — Fondamentaux du langage** : lambdas (`Function`/`Sub`), génériques et `List(Of T)`.
- **Module 3 — Programmation orientée objet** : délégués et événements, utiles pour comprendre les *continuations* et le retour sur le thread d'origine.
- Une **notion de ce qu'est un thread** est un plus, mais n'est pas indispensable : le modèle `Async`/`Await` permet justement d'écrire du code concurrent sans manipuler directement les threads.

---

## Pour aller plus loin

Une fois ce module acquis, l'asynchronie irrigue la majeure partie de la formation :

- **Interface réactive** : les formulaires asynchrones `ShowAsync`/`ShowDialogAsync` de Windows Forms sur .NET 10 ([module 5.2](../05-windows-forms/02-winforms-net10.md)) 🆕.
- **Données** : requêtes EF Core asynchrones, pagination et concurrence ([module 7.2](../07-acces-donnees/02-ef-core-10.md)).
- **Services** : consommation d'API REST avec `HttpClient` et résilience Polly ([module 8.1](../08-services-web/01-consommer-api-rest.md)).
- **Performance** : `Span(Of T)`, *pooling* et allocations ([module 14.4](../14-performance/04-span-pooling.md)), et les gains runtime offerts par .NET 10 ([module 14.6](../14-performance/06-apports-net10.md)) 🆕.

---

⏭️ [Pourquoi l'asynchronie (UI réactive, opérations d'E/S)](/04-async/01-pourquoi-async.md)
