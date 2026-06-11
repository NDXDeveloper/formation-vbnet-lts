🔝 Retour au [Sommaire](/SOMMAIRE.md)


# 4.3 Gestion des exceptions asynchrones

La bonne nouvelle d'abord : en asynchrone, on attrape les erreurs avec un `Try`/`Catch` ordinaire autour d'un `Await`, exactement comme en synchrone. Les subtilités se nichent ailleurs — dans le **moment** où l'exception surgit, dans le cas particulier des `Async Sub`, dans les **échecs multiples** de `Task.WhenAll`, et dans l'`AggregateException` qui emballe parfois l'erreur d'origine. Cette section fait le tour de ces points. VB.NET y dispose même d'un atout que C# n'a obtenu que tardivement : les **filtres `When`**.

---

## Le principe : l'exception voyage dans la `Task`

Quand une méthode asynchrone lève une exception, celle-ci n'est pas propagée immédiatement : elle est **capturée et stockée dans la `Task`**, qui passe à l'état *faulted* (en erreur). L'exception n'est **relancée qu'au moment de l'`Await`**.

C'est précisément ce qui rend la gestion d'erreurs naturelle : puisque `Await` relance l'exception, il suffit de l'entourer d'un `Try`/`Catch`.

```vb
Try
    Dim texte = Await ChargerTexteAsync(url)
    Traiter(texte)
Catch ex As HttpRequestException
    _journal.Erreur("Échec réseau", ex)
End Try
```

---

## `Try`/`Catch`/`Finally` autour d'`Await`

Tout fonctionne comme en synchrone : plusieurs `Catch` typés, un `Finally`, et la possibilité de relancer avec `Throw`. Point important : le bloc `Finally` s'exécute **après** que l'opération attendue s'est achevée (ou a échoué), et un `Using` contenant un `Await` libère bien sa ressource même si l'opération échoue.

```vb
_indicateur.Visible = True
Try
    Using fichier = File.OpenRead(chemin)          ' Using = Try/Finally implicite
        Dim donnees = Await LireToutAsync(fichier)  ' même en cas d'échec ici…
        Return Analyser(donnees)
    End Using                                        ' …le fichier est bien fermé
Catch ex As IOException
    _journal.Erreur("Lecture impossible", ex)
    Return Nothing
Finally
    _indicateur.Visible = False                      ' toujours exécuté
End Try
```

> ⚠️ **`Await` ne peut pas figurer *à l'intérieur* d'un `Catch` ou d'un `Finally` en VB.NET** (erreur BC36943 — l'interdiction vaut aussi dans un `SyncLock`, voir [4.7](07-synchronisation.md)). C# a levé cette restriction avec C# 6 (2015), mais pas VB, gelé avant : le réflexe courant « journaliser en asynchrone depuis le `Catch` » (`await LogAsync(ex)`), omniprésent dans le code C# et les réponses d'IA, **ne compile pas** en VB. Le contournement consiste à **capturer** l'exception, puis à faire l'`Await` *après* le bloc :
>
> ```vb
> Dim erreur As Exception = Nothing
> Try
>     Await OperationAsync()
> Catch ex As Exception
>     erreur = ex                      ' on capture — aucun Await ici
> End Try
>
> If erreur IsNot Nothing Then
>     Await JournaliserAsync(erreur)   ' l'Await se fait HORS du Catch
> End If
> ```

---

## Les filtres `When` — un atout idiomatique de VB.NET ⭐

VB.NET prend en charge les **filtres d'exception** depuis ses tout débuts (2002) ; C# ne les a obtenus qu'avec C# 6 (2015). La syntaxe ajoute une condition `When` à un `Catch` : le bloc n'attrape l'exception **que si la condition est vraie**.

```vb
Try
    Dim reponse = Await _client.GetAsync(url)
    reponse.EnsureSuccessStatusCode()
    Return Await reponse.Content.ReadAsStringAsync()

Catch ex As HttpRequestException When ex.StatusCode = HttpStatusCode.NotFound
    ' 404 : cas métier attendu, pas une vraie erreur → on renvoie une absence
    Return Nothing

Catch ex As HttpRequestException
    ' tout autre échec HTTP : on journalise et on relance
    _journal.Erreur("Erreur HTTP", ex)
    Throw
End Try
```

Au-delà de la lisibilité, le filtre présente un avantage technique réel : la condition `When` est évaluée **avant** le déroulement de la pile (*stack unwinding*). Si elle est fausse, c'est comme si ce `Catch` n'existait pas — la pile d'appels reste intacte pour les `Catch` suivants et pour le diagnostic, ce qu'un `Catch` classique avec un `If … Then Throw` à l'intérieur ne permet pas.

> **À noter** — Ici, `HttpRequestException.StatusCode` est de type `HttpStatusCode?` (nullable). En VB, si la valeur est `Nothing`, la comparaison vaut `False` dans une condition `When` : le filtre échoue simplement et l'on passe au `Catch` suivant. Comportement correct et sans surprise.

---

## Quand l'exception surgit-elle vraiment ?

Dans une méthode marquée `Async`, **même les exceptions de la partie synchrone** (avant le premier `Await`) sont placées dans la `Task` : elles ne sont **pas** levées à l'appel, mais à l'`Await`.

```vb
Async Function ValiderEtChargerAsync(id As Integer) As Task(Of Profil)
    If id <= 0 Then
        Throw New ArgumentException("Identifiant invalide", NameOf(id))  ' (1)
    End If
    Return Await _depot.LireAsync(id)
End Function
```

L'`ArgumentException` (1) **ne se déclenche pas** quand on écrit `ValiderEtChargerAsync(-1)` : elle attend l'`Await` de la `Task` renvoyée. C'est le pendant du point soulevé en [4.2](02-async-await.md) : une méthode `Function … As Task` **sans `Async`** qui lèverait une exception dans son corps, elle, échouerait **dès l'appel** (synchrone), avant même de fournir une `Task`. À garder en tête si vous voulez une validation « fail-fast » : il faut alors une méthode d'entrée synchrone qui valide, puis délègue à la version asynchrone.

---

## Plusieurs échecs : `Task.WhenAll`

`Task.WhenAll` passe en erreur dès qu'**une** des tâches échoue. Mais attention : l'`Await` ne relance que **la première** exception. Si plusieurs tâches ont échoué et que vous voulez les connaître toutes, il faut inspecter la propriété `Exception` de la tâche agrégée (une `AggregateException`).

```vb
Dim toutes = Task.WhenAll(t1, t2, t3)
Try
    Await toutes
Catch ex As Exception
    ' ex = la PREMIÈRE exception uniquement.
    ' Pour toutes les obtenir :
    For Each interne In toutes.Exception.InnerExceptions
        _journal.Erreur("Tâche en échec", interne)
    Next
End Try
```

Tant que vous n'avez besoin que de réagir à « au moins un échec », le `Catch` simple suffit. Le parcours de `InnerExceptions` ne sert que si chaque échec doit être traité individuellement.

---

## `AggregateException` et le déballage

La bibliothèque de tâches (TPL) regroupe les exceptions dans une **`AggregateException`**. Selon la façon dont on consomme la tâche, on reçoit l'exception d'origine… ou son emballage :

| Façon de consommer | Exception reçue |
|--------------------|-----------------|
| `Await maTache` | l'exception **d'origine**, déballée (la première) |
| `maTache.GetAwaiter().GetResult()` | l'exception **d'origine**, déballée |
| `maTache.Result` / `maTache.Wait()` | une **`AggregateException`** (emballée) |
| `maTache.Exception` | une **`AggregateException`** (contient *toutes* les internes) |

C'est la raison concrète, annoncée en [4.2](02-async-await.md), de préférer `Await` (ou `GetAwaiter().GetResult()` au point d'entrée) à `.Result`/`.Wait()` : on récupère directement l'exception utile, du bon type, sans avoir à fouiller dans une `AggregateException`.

Quand on doit malgré tout manipuler une `AggregateException`, deux méthodes aident : `Flatten()` aplatit les agrégations imbriquées, et `Handle(...)` permet de traiter sélectivement certaines exceptions internes. On distingue aussi `InnerException` (la première) de `InnerExceptions` (la collection complète).

---

## Le piège des `Async Sub`

C'est l'écueil le plus important en pratique. Un `Async Sub` ne renvoie **pas** de `Task` : son exception **n'a nulle part où aller**. Elle est relancée sur le contexte de synchronisation et, faute d'appelant pour l'intercepter, peut **faire planter l'application**.

```vb
' DANGEREUX : si OperationRisqueeAsync échoue, personne ne peut attraper l'exception.
Private Async Sub TraiterEnFond()
    Await OperationRisqueeAsync()
End Sub
```

> **Piège** — Comme un `Async Sub` ne peut pas être attendu, **aucun appelant ne peut intercepter ses exceptions**. La règle est donc : toujours encapsuler le corps d'un `Async Sub` dans un `Try`/`Catch`. Et comme `Async Sub` est réservé aux **gestionnaires d'événements** (voir [4.2](02-async-await.md)), c'est là, à l'intérieur du gestionnaire, qu'il faut gérer l'erreur.

```vb
Private Async Sub BtnEnvoyer_Click(sender As Object, e As EventArgs) Handles BtnEnvoyer.Click
    Try
        Await EnvoyerAsync()
        MessageBox.Show("Envoi réussi.")
    Catch ex As Exception
        MessageBox.Show($"Échec de l'envoi : {ex.Message}")
    End Try
End Sub
```

---

## Exceptions non observées (« lancer et oublier »)

Si une `Task` en erreur n'est **jamais attendue ni observée**, son exception devient *unobserved*. Sur .NET moderne, cela **ne fait pas planter** le processus par défaut (ce comportement a changé depuis .NET Framework 4.5), mais l'erreur disparaît silencieusement — un bug qui passe inaperçu est souvent pire qu'un bug bruyant.

L'événement global `TaskScheduler.UnobservedTaskException` permet de capter ces cas au moment de la finalisation, mais ce n'est qu'un filet de sécurité de dernier recours. La bonne pratique : ne pas lancer de tâche « et l'oublier » sans, au minimum, brancher une continuation qui en journalise le résultat — ou tout simplement l'attendre.

---

## Annulation : une erreur à part

L'annulation d'une tâche se manifeste par une `OperationCanceledException` (ou sa dérivée `TaskCanceledException`) relancée à l'`Await`. Ce n'est pas un échec « ordinaire » : on la distingue généralement des vraies erreurs pour ne pas la journaliser comme telle. La mécanique d'annulation, et la façon de traiter cette exception proprement, font l'objet de la section suivante.

---

## En résumé

- Une exception dans une méthode asynchrone est stockée dans la `Task` et **relancée à l'`Await`** : un `Try`/`Catch` ordinaire suffit.
- ⚠️ **`Await` est interdit *dans* les blocs `Catch`/`Finally`** en VB (BC36943, contrairement à C# 6+) : capturer l'exception, puis attendre **après** le bloc.
- **VB en avance** ⭐ : les filtres `Catch … When` (depuis 2002) filtrent sans dérouler la pile — idéaux pour distinguer cas métier et vraies erreurs.
- Dans une méthode `Async`, même les exceptions synchrones sortent à l'`Await`, pas à l'appel.
- `Task.WhenAll` ne relance que la **première** exception ; les autres sont dans `task.Exception.InnerExceptions`.
- `Await` **déballe** l'exception ; `.Result`/`.Wait()` la livrent emballée dans une `AggregateException` — préférez `Await`.
- **Piège majeur** : un `Async Sub` ne transmet pas ses exceptions — `Try`/`Catch` **obligatoire** à l'intérieur (gestionnaires d'événements).
- Ne « lancez-oubliez » jamais une tâche sans en observer le résultat.

Reste à voir comment **interrompre** proprement une opération asynchrone, et d'où vient exactement l'`OperationCanceledException` évoquée ci-dessus.


⏭️ [Annulation et timeout (CancellationToken)](/04-async/04-annulation-timeout.md)
