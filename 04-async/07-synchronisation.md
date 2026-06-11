🔝 Retour au [Sommaire](/SOMMAIRE.md)


# 4.7 Synchronisation et thread-safety (`SyncLock`, `Interlocked`, `SemaphoreSlim`)

La section 4.5 a désigné la **sécurité des threads** comme le « danger n°1 » du parallélisme, en renvoyant ici pour les outils. Les voici. Dès que plusieurs threads (boucle parallèle, `Task.Run`, opérations asynchrones concurrentes, services de fond) accèdent au même **état mutable**, il faut coordonner ces accès. VB.NET dispose pour cela de trois instruments principaux : `SyncLock` (exclusion mutuelle), `Interlocked` (opérations atomiques) et `SemaphoreSlim` (limitation de concurrence et verrou *asynchrone*). Et une règle d'or au-dessus de tout : **éviter l'état partagé** quand c'est possible.

---

## Le problème : l'état mutable partagé

Rappel de [4.5](05-parallelisme.md) : une opération aussi banale que `compteur += 1` n'est **pas atomique**. C'est une suite lire → modifier → écrire, et deux threads peuvent lire la même valeur avant de la réécrire — une incrémentation est alors perdue. Le résultat est faux et non reproductible (*race condition*).

À cela s'ajoute un second piège, plus discret : la **visibilité mémoire**. Sans synchronisation, rien ne garantit qu'un thread voie immédiatement la valeur écrite par un autre (caches processeur, réordonnancement). Les primitives ci-dessous règlent ces deux problèmes à la fois.

---

## `SyncLock` — l'exclusion mutuelle

`SyncLock` est l'équivalent VB du `lock` de C# : il garantit qu'**un seul thread à la fois** exécute la région protégée. On verrouille toujours sur un objet **privé et dédié**.

```vb
Private ReadOnly _verrou As New Object()
Private _solde As Decimal

Public Sub Crediter(montant As Decimal)
    SyncLock _verrou
        _solde += montant        ' un seul thread à la fois ici
    End SyncLock
End Sub
```

Quelques règles :

- Verrouiller sur un objet **privé** réservé à cet usage — **jamais** sur `Me`, sur `GetType()`, ni sur une chaîne (du code extérieur pourrait verrouiller le même objet et provoquer un interblocage ; les chaînes peuvent en plus être *internées*).
- Garder la **section critique aussi courte que possible** : on ne fait que le strict nécessaire sous le verrou.
- Ne jamais appeler de **code inconnu** (rappel d'événement, callback) en tenant le verrou : risque d'interblocage.

---

## `SyncLock` et asynchronie : pourquoi `SemaphoreSlim`

Point capital pour ce module : **on ne peut pas placer un `Await` à l'intérieur d'un `SyncLock`** — le langage l'interdit (tout comme C# interdit `await` dans un `lock`). Ce n'est donc pas une particularité de VB, mais une conséquence de la nature même du verrou.

La raison : `SyncLock` s'appuie sur `Monitor`, dont le verrou est **lié au thread** qui l'a pris (c'est ce thread, et lui seul, qui doit le relâcher). Or un `Await` peut faire reprendre la suite **sur un autre thread** : le verrou ne pourrait plus être relâché correctement. D'où l'interdiction.

Conséquence pratique :

> `SyncLock` ne sert qu'aux sections critiques **synchrones**. Pour protéger une section critique qui contient un `Await`, on utilise **`SemaphoreSlim`** (voir plus bas), seul verrou compatible avec l'asynchronie.

---

## `Interlocked` — les opérations atomiques

Pour de simples opérations sur **une seule variable** (compteur, accumulation, échange), inutile de poser un verrou : `Interlocked` les rend atomiques **sans blocage**, ce qui est nettement plus rapide.

```vb
Private _compteur As Integer

Public Sub Incrementer()
    Interlocked.Increment(_compteur)     ' atomique, lock-free
End Sub
```

Les méthodes utiles : `Increment`, `Decrement`, `Add` (déjà croisée en [4.5](05-parallelisme.md) pour fusionner des sous-totaux), `Exchange` (affecter en récupérant l'ancienne valeur) et `CompareExchange` (affecter **seulement si** la valeur actuelle est celle attendue — la brique des algorithmes sans verrou). Dès qu'il s'agit d'un compteur ou d'un drapeau, on préfère `Interlocked` à un `SyncLock`.

---

## `SemaphoreSlim` — limiter la concurrence et verrouiller en async

Un `SemaphoreSlim` autorise **jusqu'à N** threads ou tâches à entrer dans une région. Deux usages essentiels.

**1. Le verrou asynchrone.** Un `SemaphoreSlim(1, 1)` n'autorise qu'**un** occupant : c'est l'exclusion mutuelle — mais, contrairement à `SyncLock`, elle **fonctionne avec `Await`**. Le motif : attendre le jeton (`WaitAsync`, *avant* le `Try`), puis libérer dans le `Finally` (`Release` est **synchrone**, donc autorisé en `Finally`).

```vb
Private ReadOnly _verrouAsync As New SemaphoreSlim(1, 1)

Public Async Function MettreAJourAsync(ct As CancellationToken) As Task
    Await _verrouAsync.WaitAsync(ct)          ' attente du jeton (hors Try)
    Try
        Dim donnees = Await LireAsync(ct)     ' on PEUT attendre ici
        Await EcrireAsync(Transformer(donnees), ct)
    Finally
        _verrouAsync.Release()                ' synchrone → permis dans Finally
    End Try
End Function
```

C'est précisément ce que `SyncLock` ne sait pas faire. Si le sémaphore est compatible avec l'asynchronie, c'est qu'il n'est **pas lié à un thread** : n'importe quel thread peut appeler `Release`.

**2. La limitation de débit (*throttling*).** Un `SemaphoreSlim(N)` borne le nombre d'opérations **simultanées** — typiquement pour ne pas saturer un service distant :

```vb
' Au plus 5 téléchargements en parallèle
Private ReadOnly _limiteur As New SemaphoreSlim(5)

Public Async Function TelechargerAsync(url As String, ct As CancellationToken) As Task(Of Byte())
    Await _limiteur.WaitAsync(ct)
    Try
        Return Await _client.GetByteArrayAsync(url, ct)
    Finally
        _limiteur.Release()
    End Try
End Function
```

Appelée pour de nombreuses URL à la fois, cette méthode n'en laisse passer que cinq de front. (C'est une alternative à `Parallel.ForEachAsync` de [4.5](05-parallelisme.md), sans la contrainte du délégué `ValueTask`.)

---

## Choisir la bonne primitive

| Besoin | Outil |
|--------|-------|
| Compteur, drapeau, accumulation simple | `Interlocked` |
| Section critique **synchrone** | `SyncLock` |
| Section critique **asynchrone** (avec `Await`) | `SemaphoreSlim(1, 1)` |
| Limiter à **N** opérations concurrentes | `SemaphoreSlim(N)` |
| Collection partagée | une **collection concurrente** (voir ci-dessous) |

---

## Mieux : éviter le verrou

Le code le plus sûr est celui qui n'a **rien à verrouiller**. Avant de poser un `SyncLock`, on se demande si l'on peut :

- utiliser une **collection concurrente** — `ConcurrentDictionary(Of K, V)`, `ConcurrentQueue(Of T)`, `ConcurrentBag(Of T)`, `BlockingCollection(Of T)` — qui gère la concurrence en interne, sans verrou explicite ;

```vb
Private ReadOnly _cache As New ConcurrentDictionary(Of String, Profil)()
Dim p = _cache.GetOrAdd(cle, Function(k) ChargerProfil(k))
```

- privilégier des **données immuables** (lues sans risque par plusieurs threads), ou un modèle par **passage de messages** (chaque donnée n'appartient qu'à un thread à la fois).

Moins il y a d'état mutable partagé, moins il y a de bugs de concurrence.

---

## Pièges et bonnes pratiques

- **Sections critiques courtes** ; ne jamais y appeler de code inconnu.
- **Ordre de verrouillage cohérent** entre tous les threads : prendre les verrous toujours dans le même ordre évite les interblocages (le cas classique : un thread tient A et attend B pendant qu'un autre tient B et attend A).
- Ne jamais verrouiller sur `Me`, un type ou une chaîne.
- Ne pas **bloquer** sur de l'asynchrone (`.Result`/`.Wait()`) — rappel des sections précédentes.
- **Différence VB** : VB n'a **pas** de mot-clé `volatile` (contrairement à C#). Pour des accès volatils, on passe par les méthodes de la classe `System.Threading.Volatile` (`Volatile.Read`, `Volatile.Write`). Dans la plupart des cas, les primitives ci-dessus suffisent et dispensent d'y recourir.

---

## En résumé

- Tout **état mutable partagé** entre threads doit être protégé — sans quoi : *race conditions* et problèmes de visibilité mémoire.
- `SyncLock` (équivalent de `lock`) assure l'exclusion mutuelle **synchrone** ; on verrouille sur un objet **privé dédié**, en gardant la section courte.
- **On ne peut pas `Await` dans un `SyncLock`** (ni dans un `lock` C#) : pour une section critique asynchrone, c'est **`SemaphoreSlim(1, 1)`** (`WaitAsync` / `Release`).
- `Interlocked` rend atomiques les opérations sur une variable (compteurs, `Add`, `CompareExchange`) — sans verrou.
- `SemaphoreSlim(N)` **limite la concurrence** à N opérations simultanées.
- **Mieux vaut éviter le verrou** : collections concurrentes, données immuables, passage de messages.

Nous savons désormais exécuter du travail concurrent **et** le synchroniser sans risque. Reste à voir comment **héberger** du travail de fond durable — services, traitements planifiés — via le *Generic Host*, en notant au passage une absence côté VB déjà signalée dans l'introduction du module.


⏭️ [Services en arrière-plan : Generic Host et BackgroundService](/04-async/08-background-services.md)
