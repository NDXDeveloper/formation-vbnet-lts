🔝 Retour au [Sommaire](/SOMMAIRE.md)


# 4.6 Consommer `IAsyncEnumerable` / flux asynchrones ; `ValueTask` (notions)

Le titre de cette section emploie un verbe précis : **consommer**. C'est tout l'enjeu pour VB.NET. Les flux asynchrones et le type `ValueTask` sont des notions que VB sait **utiliser** sans réserve — mais qu'il ne sait pas (ou très mal) **produire**. Cette section montre comment les consommer correctement, et où se situent exactement les limites du langage.

---

## Qu'est-ce qu'un flux asynchrone ?

Un flux asynchrone (`IAsyncEnumerable(Of T)`) est une séquence dont les éléments sont produits **de façon asynchrone, un à un**. Là où un `Task(Of List(Of T))` attend que *toute* la collection soit prête avant de la livrer en bloc, un `IAsyncEnumerable(Of T)` permet de **traiter chaque élément dès qu'il arrive**, sans charger l'ensemble en mémoire.

Les cas d'usage typiques : résultats paginés d'une API, requête de base de données diffusée ligne par ligne, lecture d'un gros fichier, flux temps réel. Côté VB, vous en consommerez surtout depuis des bibliothèques : `AsAsyncEnumerable()` d'EF Core, les canaux (`Channel`), les réponses en flux de gRPC ou de `HttpClient`.

Les interfaces sous-jacentes sont minimales :

```vb
' (forme des interfaces du framework, pour comprendre la consommation)
Interface IAsyncEnumerable(Of Out T)
    Function GetAsyncEnumerator(ct As CancellationToken) As IAsyncEnumerator(Of T)
End Interface

Interface IAsyncEnumerator(Of Out T)   ' : IAsyncDisposable
    Function MoveNextAsync() As ValueTask(Of Boolean)
    ReadOnly Property Current As T
    Function DisposeAsync() As ValueTask
End Interface
```

On note d'emblée que `MoveNextAsync` et `DisposeAsync` renvoient des **`ValueTask`** — autrement dit, **consommer un flux asynchrone, c'est déjà consommer du `ValueTask`** (sujet repris plus bas).

---

## Consommer : une ligne en C#, à la main en VB ⚠️

En C#, on parcourt un flux asynchrone avec `await foreach` — une seule ligne. **VB ne dispose pas de cette syntaxe** : il n'existe pas d'`Await For Each`. Il faut donc dérouler manuellement l'énumérateur : obtenir l'énumérateur, boucler tant que `MoveNextAsync` renvoie `True`, lire `Current`.

```vb
Dim enumerateur = flux.GetAsyncEnumerator(ct)
Do While Await enumerateur.MoveNextAsync()
    Dim element = enumerateur.Current
    Traiter(element)
Loop
```

Cette boucle fonctionne — mais elle est **incomplète** : elle ne libère pas l'énumérateur. Et c'est là que VB se complique sérieusement.

---

## La libération de l'énumérateur : le vrai point dur ⚠️

Un énumérateur asynchrone implémente `IAsyncDisposable` : il faut donc appeler `Await enumerateur.DisposeAsync()` pour le libérer proprement. En C#, `await foreach` s'en charge automatiquement (et, à défaut, on emploie `await using`). En VB, **deux limites se cumulent** :

- VB **n'autorise pas `Await` dans un bloc `Finally`** (ni dans un `Catch`) — c'est une règle explicite du langage. On ne peut donc pas placer `Await enumerateur.DisposeAsync()` dans le `Finally` qui garantirait la libération.
- VB **ne possède pas d'`Await Using`** : il n'existe aucune syntaxe de libération asynchrone (le `Using` de VB n'appelle que le `Dispose()` *synchrone*).

Le motif habituel « acquérir, puis libérer automatiquement en sortie de bloc » — celui du `Using` — est donc inaccessible ici. La consommation **correcte et sûre face aux exceptions** passe par un contournement : capturer l'éventuelle exception dans le `Catch` (sans l'attendre — on se contente de la *mémoriser*), libérer l'énumérateur **hors** du `Try` (où `Await` est de nouveau autorisé), puis relancer en préservant la pile d'origine.

```vb
Imports System.Runtime.ExceptionServices
' ...

Dim enumerateur = flux.GetAsyncEnumerator(ct)
Dim capture As ExceptionDispatchInfo = Nothing
Try
    Do While Await enumerateur.MoveNextAsync()
        Traiter(enumerateur.Current)
    Loop
Catch ex As Exception
    capture = ExceptionDispatchInfo.Capture(ex)   ' on mémorise (pas d'Await ici)
End Try

Await enumerateur.DisposeAsync()                   ' Await autorisé : hors Try/Catch/Finally
capture?.Throw()                                   ' relance en conservant la pile
```

On utilise `ExceptionDispatchInfo.Capture(...).Throw()` plutôt qu'un simple `Throw capture` pour ne **pas écraser la pile d'appels** d'origine. Ce que C# règle en un mot-clé (`await foreach`) demande, en VB, cette plomberie manuelle. C'est le coût réel de l'absence des fonctionnalités de flux asynchrones dans le langage.

---

## Options pragmatiques

Avant d'écrire la boucle ci-dessus, deux questions valent d'être posées :

**Ai-je vraiment besoin de diffuser ?** Si le traitement n'a pas besoin d'être incrémental, inutile de consommer un flux : matérialisez la collection d'un coup. Côté EF Core, par exemple, `Await requete.ToListAsync(ct)` renvoie directement un `List(Of T)` — pas de flux, pas de plomberie. Ne consommez un `IAsyncEnumerable` que lorsque traiter les éléments **au fil de l'eau** apporte un gain réel (mémoire, latence du premier élément).

**Une bibliothèque peut-elle simplifier ?** Le paquet NuGet `System.Linq.Async` ajoute des opérateurs façon LINQ sur `IAsyncEnumerable` (dont `ToListAsync`, des helpers d'itération, etc.) et gère la libération en interne. Pour un corps de traitement **synchrone**, il peut réduire la consommation à un seul appel ; pour un corps lui-même asynchrone, les mêmes contraintes VB sur les délégués `ValueTask` (voir plus bas) peuvent toutefois resurgir. À évaluer selon le besoin.

---

## VB ne *produit* pas de flux asynchrones ⚠️ 🔗

Symétriquement, **VB ne peut pas écrire de flux asynchrone**. Produire un `IAsyncEnumerable` suppose un **itérateur asynchrone** — la combinaison d'`Await` et de `Yield` dans une même méthode (`async`/`yield return` en C#). VB **n'a pas d'itérateur asynchrone** : on ne peut pas marquer `Async` une `Iterator Function` (erreur BC36936 — « les modificateurs `Async` et `Iterator` ne peuvent pas être utilisés en même temps »). (C'est la même racine que l'impossibilité d'écrire des méthodes `Async … As ValueTask`, vue en [4.5](05-parallelisme.md).)

En pratique : on **consomme** des flux fournis par des bibliothèques, mais dès qu'il faut **produire** un flux asynchrone, on écrit la brique en **C#** et on l'expose à VB (architecture hybride — voir [module 10](../10-hybride-vbnet-csharp/README.md) et [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)). Implémenter `IAsyncEnumerable` à la main en VB (en codant soi-même l'énumérateur) reste théoriquement possible, mais l'effort est tel que c'est rarement la bonne option.

---

## `ValueTask` (notions)

`ValueTask` et `ValueTask(Of T)` sont des alternatives **structures** à `Task`. Là où `Task` est une classe (donc une allocation sur le tas à chaque appel), `ValueTask` évite cette allocation lorsque le résultat est **souvent disponible immédiatement** (valeur en cache, chemin synchrone). C'est une optimisation pour les API à très haut débit.

Du point de vue du **consommateur**, on l'attend comme une `Task` :

```vb
Dim disponible = Await enumerateur.MoveNextAsync()   ' Await ValueTask(Of Boolean) → Boolean
```

Mais un `ValueTask` impose des **règles de consommation** plus strictes :

- on ne l'attend **qu'une seule fois** ; il ne faut pas l'attendre deux fois, ni accéder à son résultat avant qu'il soit terminé ;
- si l'on a besoin de l'attendre plusieurs fois, de le passer à `Task.WhenAll`/`WhenAny`, ou de le conserver, on le convertit d'abord en `Task` avec `.AsTask()`.

Quand faut-il s'en soucier ? **Rarement.** La recommandation officielle est de renvoyer `Task`/`Task(Of T)` par défaut, et de ne recourir à `ValueTask` que si un profilage le justifie. Pour un développeur VB, `ValueTask` est donc surtout quelque chose que l'on **attend** en provenance d'API performantes.

---

## `ValueTask` et VB : consommer oui, produire non ⚠️

Rappel du [4.5](05-parallelisme.md), à sa place ici : **VB ne peut pas écrire de méthode `Async … As ValueTask`** (erreur *BC36945*). Cela a une conséquence concrète dès qu'on implémente une interface dont un membre renvoie un `ValueTask` — typiquement `IAsyncDisposable.DisposeAsync`. On ne peut pas marquer la méthode `Async` ; on renvoie alors un `ValueTask` **à la main**, soit déjà terminé, soit enveloppant une `Task` :

```vb
' Libération synchrone, emballée dans un ValueTask déjà terminé
Public Function DisposeAsync() As ValueTask Implements IAsyncDisposable.DisposeAsync
    _ressource.Dispose()
    Return ValueTask.CompletedTask
End Function
```

```vb
' Libération réellement asynchrone : on enveloppe une Async … As Task
Public Function DisposeAsync() As ValueTask Implements IAsyncDisposable.DisposeAsync
    Return New ValueTask(FermerConnexionAsync())
End Function

Private Async Function FermerConnexionAsync() As Task
    Await _connexion.CloseAsync()
End Function
```

C'est exactement le même contournement que pour `Parallel.ForEachAsync` ([4.5](05-parallelisme.md)) : VB **consomme** `ValueTask` librement, mais pour en **produire** un depuis du code asynchrone, il enveloppe une `Async … As Task`.

---

## En résumé

- Un flux asynchrone (`IAsyncEnumerable(Of T)`) livre ses éléments **un à un**, de façon asynchrone ; on le consomme depuis des bibliothèques (EF Core, canaux, gRPC…).
- **VB n'a pas d'`Await For Each`** ⚠️ : on déroule l'énumérateur à la main (`GetAsyncEnumerator` → `Do While Await MoveNextAsync()` → `Current`).
- **La libération se complique** ⚠️ : pas d'`Await` en `Finally`, pas d'`Await Using` — la consommation sûre exige un motif capture / `DisposeAsync` hors `Try` / relance via `ExceptionDispatchInfo`.
- **VB ne produit pas de flux asynchrones** ⚠️ 🔗 (pas d'itérateur asynchrone) : on écrit les producteurs en **C#**.
- `ValueTask` (notions) : optimisation structure vs `Task`, à **attendre une seule fois** ; convertir via `.AsTask()` si besoin ; par défaut, on préfère `Task`.
- **VB consomme `ValueTask` mais ne l'écrit pas** ⚠️ (BC36945) : pour implémenter `DisposeAsync`, on renvoie un `ValueTask` à la main (`ValueTask.CompletedTask` ou `New ValueTask(uneTask)`).

Cette section a montré une concurrence largement *subie* (consommation). La suivante revient à une concurrence *maîtrisée* : protéger un état partagé entre plusieurs threads avec les primitives de synchronisation.


⏭️ [Synchronisation et thread-safety (SyncLock, Interlocked, SemaphoreSlim)](/04-async/07-synchronisation.md)
