# 💻 Exemples du module 4 — Programmation asynchrone et parallèle

Huit projets **complets, compilés et exécutés** (un par section ; le README du module n'a
pas de code). Chaque projet reprend **tous les extraits** de sa section, assemblés en
programme exécutable ; chaque fichier source porte un en-tête **section concernée /
description / fichier du cours**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · runtime 10.0.x · Windows 11 (culture machine fr-FR).

> 💡 **Compilation / exécution** — projets console :
> ```bash
> cd <dossier-de-l-exemple>
> dotnet run          # ou : ouvrir le .vbproj / .sln dans VS 2026, puis F5
> ```
> Le 4.6 est une **solution** (`dotnet build FluxAsynchrones.sln` puis
> `dotnet run --project AppVB`). Le 4.8 restaure un paquet NuGet
> (`Microsoft.Extensions.Hosting`) à la première compilation (réseau requis une fois).
> Tous les projets activent `Option Strict On`.

> ⚠️ **Sorties dépendantes du temps** — l'asynchronie et le parallélisme produisent des
> **durées** (ms) qui varient d'une machine à l'autre, et un **ordre d'entrelacement** des
> messages qui peut différer. Les exemples sont conçus pour que **les valeurs vérifiées**
> (sommes, comptages, ordres logiques, exactitude) restent **déterministes** ; seuls les
> chiffres de durée et l'ordre d'affichage des lignes concurrentes peuvent bouger.

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Limites VB démontrées |
|---|---|---|
| `README.md` (module) | — (aucun code) | |
| `01-pourquoi-async.md` | [`4.1-pourquoi-async`](#41-pourquoi-async) | |
| `02-async-await.md` | [`4.2-async-await`](#42-async-await) | pas d'`Async Main` |
| `03-exceptions-async.md` | [`4.3-exceptions-async`](#43-exceptions-async) | `Await` interdit dans `Catch`/`Finally` |
| `04-annulation-timeout.md` | [`4.4-annulation-timeout`](#44-annulation-timeout) | |
| `05-parallelisme.md` | [`4.5-parallelisme`](#45-parallelisme) | pas de lambda `Async … As ValueTask` |
| `06-async-streams.md` | [`4.6-flux-asynchrones`](#46-flux-asynchrones) (solution VB + C#) | pas d'`Await For Each`, pas d'itérateur async |
| `07-synchronisation.md` | [`4.7-synchronisation`](#47-synchronisation) | `Await` interdit dans `SyncLock` |
| `08-background-services.md` | [`4.8-background-services`](#48-background-services) | pas de modèle « Worker » ; piège `host` |

### Limites VB.NET vérifiées au compilateur

Les cinq « biais C# » signalés par le module ont été **réellement provoqués** dans un projet
témoin et donnent les erreurs suivantes (citées dans les exemples) :

| Construction C# transposée naïvement | Erreur VB obtenue |
|---|---|
| `Async Function Main() As Task` | **BC30737** (aucun `Main` de signature appropriée) |
| `Await` dans un `Catch` / `Finally` / `SyncLock` | **BC36943** |
| `Async Function … As ValueTask` (ou lambda) | **BC36945** |
| `Async Iterator Function … As IAsyncEnumerable` | **BC36936** (`Async` + `Iterator` incompatibles) |
| `Dim host = builder.Build()` (après `Host.Create…`) | **BC32000** (la variable `host` masque la classe `Host`) |

---

## 4.1-pourquoi-async

- **Section** : 4.1 — Pourquoi l'asynchronie · **Fichier** : `01-pourquoi-async.md`
- **Description** : les concepts du cours (illustrés en WinForms) transposés en console :
  un appel **bloquant** (`Thread.Sleep`) qui immobilise le thread, vs un appel
  **asynchrone** (`Task.Delay`) pendant lequel un autre travail progresse (« réactivité ») ;
  les E/S asynchrones réelles (`File.WriteAllTextAsync`/`ReadAllTextAsync`) ; le tableau
  E/S (*I/O-bound*) vs calcul (*CPU-bound*).
- **Sortie attendue** (vérifiée, valeurs stables) :
  ```text
  Le thread n'a RIEN pu faire pendant ~400 ms (une UI serait restée figée).
  Résultat reçu : « 3 résultats pour « VB.NET » »
  Pendant l'attente, le programme a exécuté N battements (il est resté réactif).
  Réactif pendant l'attente : True
  File.ReadAllTextAsync -> « Contenu écrit par File.WriteAllTextAsync. »
  ```
- **Comportement** : `Réactif pendant l'attente : True` est garanti ; le nombre de
  battements (~7) et la durée (~400 ms) varient légèrement.

## 4.2-async-await

- **Section** : 4.2 — `Async`/`Await` · **Fichier** : `02-async-await.md`
- **Description** : le **pont** `Sub Main` synchrone + `MainAsync().GetAwaiter().GetResult()`
  (⚠️ pas d'`Async Main` en VB) ; `Async Function … As Task(Of T)` (on `Return` la valeur)
  et `As Task` (fichier réel) ; **séquence vs `Task.WhenAll`** (chronométré : WhenAll plus
  rapide) ; `Task.WhenAny` (course) ; `Task.Run` + **`AddressOf`** pour une méthode nommée ;
  relais d'une `Task` sans `Async`, `Task.FromResult` / `Task.CompletedTask` ;
  `ConfigureAwait(False)`.
- **Sortie attendue** (vérifiée, extraits stables) :
  ```text
  ChargerTexteAsync -> « contenu de document-1 »
  Résultats WhenAll : Météo Paris : 21 °C | Trafic Paris : fluide
  Séquence ≈ 500 ms ; WhenAll ≈ 305 ms
  WhenAll plus rapide que la séquence : True
  Première réponse : « miroir rapide »
  Somme des carrés 1..1000 = 333833500
  CalculerStatistiques (via AddressOf) = 50,5
  Task.FromResult : 42
  ```
- **Comportement** : `WhenAll plus rapide … : True` garanti (≈ max des deux délais vs
  leur somme) ; `333833500` et `50,5` sont exacts.

## 4.3-exceptions-async

- **Section** : 4.3 — Gestion des exceptions asynchrones · **Fichier** : `03-exceptions-async.md`
- **Description** : `Try`/`Catch` autour d'`Await` ; `Finally`/`Using` exécutés malgré
  l'échec ; ⚠️ le **contournement** « capturer / `Await` après le bloc » car `Await` est
  interdit dans `Catch` (BC36943) ; les **filtres `When`** ⭐ (404 = cas métier, 500
  relancé) ; l'exception **synchrone** d'une méthode `Async` qui sort à l'`Await` (pas à
  l'appel) ; `Task.WhenAll` (première exception relancée, toutes dans `InnerExceptions`) ;
  `.Wait()` → `AggregateException` vs `GetAwaiter().GetResult()` → exception d'origine ;
  le **piège `Async Sub`** (`Try`/`Catch` obligatoire à l'intérieur).
- **Sortie attendue** (vérifiée) :
  ```text
  Catch -> InvalidOperationException : Échec réseau simulé.
  Using -> RessourceTracee.Dispose() appelée
  JournaliserAsync (hors Catch) -> Échec réseau simulé.
  Statut 404 -> résultat : Nothing (absence, pas une erreur)
  Statut 500 -> relancée puis attrapée ici : InternalServerError
  L'appel ValiderEtChargerAsync(-1) n'a PAS levé d'exception.
  L'Await, lui, relance : ArgumentException (« id »)
  Await relance la PREMIÈRE : « défaillance B »
  toutes.Exception.InnerExceptions en contient : 2
  .Wait()                  -> AggregateException
  GetAwaiter().GetResult() -> InvalidOperationException (« erreur d'origine »)
  Async Sub -> échec géré À L'INTÉRIEUR : Échec réseau simulé.
  ```
- **Comportement** : « défaillance B » est la première relancée car son délai est inférieur ;
  l'ordre des deux `InnerExceptions` peut varier, mais leur **nombre (2)** est garanti.

## 4.4-annulation-timeout

- **Section** : 4.4 — Annulation et timeout · **Fichier** : `04-annulation-timeout.md`
- **Description** : le modèle coopératif (CTS « bouton Annuler ») ;
  `ThrowIfCancellationRequested` (points de contrôle) ; **état `Canceled` vs `Faulted`**
  vérifié ; timeout via `CancellationTokenSource(TimeSpan)` → `TimeoutException` ;
  `CreateLinkedTokenSource` + filtre `When` (distingue timeout et annulation utilisateur,
  deux scénarios) ; **`Task.WaitAsync`** (.NET 6+) qui borne l'**attente** mais laisse
  l'opération **continuer** (piège vérifié).
- **Sortie attendue** (vérifiée) :
  ```text
  Opération annulée.
  Annulé au point de contrôle après 2 fichiers traités.
  ThrowIfCancellationRequested -> IsCanceled = True, IsFaulted = False
  Exception ordinaire          -> IsCanceled = False, IsFaulted = True
  Converti en TimeoutException : « Le chargement a dépassé le délai. »
  Scénario A -> TimeoutException : Délai de 300 ms dépassé.
  Scénario B -> TaskCanceledException : annulation utilisateur (pas un timeout)
  TimeoutException après 300 ms ; opération terminée ? False
  …et s'est achevée quand même : opération terminée ? True
  ```
- **Comportement** : tous les états (`Canceled`/`Faulted`) et booléens sont déterministes ;
  la dernière ligne prouve que `WaitAsync` n'interrompt pas l'opération sous-jacente.

## 4.5-parallelisme

- **Section** : 4.5 — Parallélisme pragmatique · **Fichier** : `05-parallelisme.md`
- **Description** : `Parallel.For`/`ForEach` (écriture dans des cases distinctes) ;
  `ParallelOptions` ; `ParallelLoopState.Stop` + `ParallelLoopResult` ; **PLINQ**
  (`AsParallel`, `WithDegreeOfParallelism`, `ForAll`) ; ⚠️ la **race condition**
  (`somme += …` → résultat **faux**) puis le remède **agrégation locale**
  (`localInit`/`localFinally` + `Interlocked.Add` → résultat **exact**) ;
  `AggregateException` ; ⚠️ **`Parallel.ForEachAsync`** avec la lambda **non-async** +
  `New ValueTask(…)` (limite VB).
- **Sortie attendue** (vérifiée — valeurs déterministes) :
  ```text
  Carrés : 0, 1, 4, 9, 16, 25, 36, 49
  Cible trouvée : 42 ; boucle complétée : False
  Nombre de premiers entre 2 et 100000 : 9592
  Somme « somme += » (non protégée) : <variable> -> exacte ? False
  Somme par agrégation locale : 1000000 -> exacte ? True
  AggregateException : 3 itération(s) en échec
  Concurrence maximale observée : 5 (≤ 5 attendu : True)
  ```
- **Comportement** : le **comptage de premiers (9592)**, la **somme correcte (1000000)**
  et la **concurrence bornée (≤ 5)** sont garantis ; la somme « non protégée » est **faux
  par conception** (sa valeur exacte varie — c'est précisément la démonstration).

## 4.6-flux-asynchrones

- **Section** : 4.6 — Flux asynchrones ; `ValueTask` · **Fichier** : `06-async-streams.md`
- **Description** : **solution hybride** `FluxAsynchrones.sln` :
  - `FluxCsharp` (C#) **produit** des `IAsyncEnumerable` via un itérateur asynchrone
    (`async`/`yield return`) — **impossible en VB** (BC36936) ;
  - `AppVB` (VB) **consomme** : ⚠️ pas d'`Await For Each` → déroulé manuel
    (`GetAsyncEnumerator` → `Do While Await MoveNextAsync()` → `Current`) ; le **motif de
    libération sûre** (capture / `DisposeAsync` **hors** `Try` / relance par
    `ExceptionDispatchInfo`) sur un flux qui **échoue au 3ᵉ élément** ; matérialisation ;
    `ValueTask` (attendu une fois, `.AsTask()` sinon) ; `IAsyncDisposable` en VB
    (`ValueTask.CompletedTask` / `New ValueTask(uneTask)`).
- **Compiler / exécuter** :
  ```bash
  cd 4.6-flux-asynchrones
  dotnet build FluxAsynchrones.sln
  dotnet run --project AppVB
  ```
- **Sortie attendue** (vérifiée) :
  ```text
  Mesures reçues une à une : 10, 20, 30, 40, 50
  Reçues avant l'échec : 10, 20
  Exception relancée (pile préservée) : Capteur en panne au 3e élément.
  Collection matérialisée : [10, 20, 30, 40, 50] (somme = 150)
  Premier MoveNextAsync (ValueTask attendu une fois) : True ; Current = 10
  RessourceSimple : libération synchrone (ValueTask.CompletedTask)
  ConnexionAsync : fermeture asynchrone terminée
  ```
- **Comportement** : le flux en échec livre bien `10, 20` **avant** l'exception, qui est
  relancée avec son message d'origine — preuve que la libération a eu lieu et que la pile
  est préservée.

## 4.7-synchronisation

- **Section** : 4.7 — Synchronisation et thread-safety · **Fichier** : `07-synchronisation.md`
- **Description** : ⚠️ le **problème** (`compteur += 1` → **faux**) ; **`SyncLock`** (objet
  privé dédié → **exact**) ; **`Interlocked.Increment`** (exact, lock-free) ;
  **`SemaphoreSlim(1, 1)`** comme **verrou asynchrone** (section critique contenant un
  `Await` — ce que `SyncLock` interdit) ; **`SemaphoreSlim(5)`** pour le **throttling**
  (pic mesuré ≤ 5) ; **`ConcurrentDictionary.GetOrAdd`** pour éviter le verrou.
- **Sortie attendue** (vérifiée — sauf le compteur « faux ») :
  ```text
  Attendu : 100000 ; obtenu : <variable> -> exact ? False     (race condition)
  Attendu : 100000 ; obtenu : 100000 -> exact ? True          (SyncLock)
  Attendu : 100000 ; obtenu : 100000 -> exact ? True          (Interlocked)
  Attendu : 50 ; obtenu : 50 -> exact ? True                  (SemaphoreSlim verrou async)
  pic de concurrence : 5 (≤ 5 : True)                          (throttling)
  Clés distinctes dans le cache : 10 (attendu : 10)
  cache(3) = 9 (= 3 * 3)
  ```
- **Comportement** : les trois compteurs protégés donnent **exactement** 100000, le verrou
  async **exactement** 50, et le pic de concurrence reste **≤ 5** ; seule la première ligne
  (volontairement non protégée) est fausse et variable.

## 4.8-background-services

- **Section** : 4.8 — Generic Host et `BackgroundService` · **Fichier** : `08-background-services.md`
- **Description** : ⚠️ pas de modèle « Worker » en VB → **câblage manuel** du *Generic
  Host* dans une **console** (`Host.CreateApplicationBuilder` → `AddHostedService` →
  `Build` → `Run`) ; ⚠️ le **piège `host`** (variable nommée **`hote`**, jamais `host`,
  sous peine de **BC32000**) ; `host.Run()` **synchrone** (l'absence d'`Async Main` est
  sans conséquence) ; trois `BackgroundService` : boucle `stoppingToken`
  (`ServiceDeNettoyage`), **`PeriodicTimer`** (`ServicePeriodique`), et **portée par
  cycle** `CreateScope` pour un service *scoped* (`ServiceAvecPortee`, instances
  distinctes) ; **arrêt propre**.
- **Compiler / exécuter** :
  ```bash
  cd 4.8-background-services
  dotnet run     # restaure Microsoft.Extensions.Hosting au 1er build
  ```
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  info: Microsoft.Hosting.Lifetime[0] Application started. Press Ctrl+C to shut down.
  info: …ServiceDeNettoyage[0] Cycle de nettoyage n° 1
  info: …ServiceAvecPortee[0] Cycle 1 : UniteDeTravail 97b0c3f7
  info: …ServicePeriodique[0] PeriodicTimer : tic n° 1
  …
  info: …ServiceAvecPortee[0] Instances scoped toutes distinctes : True
  info: Microsoft.Hosting.Lifetime[0] Application is shutting down...
  Hôte arrêté proprement (graceful shutdown).
  ```
- **Comportement** : les trois services s'exécutent **en concurrence** ; l'application
  s'arrête dès qu'un service appelle `StopApplication()` (après 3 cycles). Ce qui est
  **garanti** : les identifiants des `UniteDeTravail` sont **distincts** d'un cycle à
  l'autre (« Instances scoped toutes distinctes : True ») et l'arrêt est **propre**.
  L'ordre exact des lignes et le nombre de cycles atteints par chaque service avant l'arrêt
  peuvent varier d'une exécution à l'autre (services concurrents).

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build`. Le 4.8 retéléchargera `Microsoft.Extensions.Hosting` depuis le cache NuGet.

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

> Le 4.1 et le 4.2 écrivent, à l'exécution, un fichier temporaire dans `%TEMP%`
> (`pourquoi-async-demo.txt`, `async-await-demo.bin`) qu'ils suppriment eux-mêmes.

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
