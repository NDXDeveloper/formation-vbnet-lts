🔝 Retour au [Sommaire](/SOMMAIRE.md)


# 4.4 Annulation et timeout (`CancellationToken`)

Une opération asynchrone longue — un téléchargement, un traitement par lots, une requête — doit pouvoir être **interrompue** : parce que l'utilisateur a cliqué sur « Annuler », parce qu'un délai a expiré, ou parce que l'application se ferme. .NET propose pour cela un mécanisme unifié, fondé sur deux types complémentaires : `CancellationTokenSource` (qui **demande** l'annulation) et `CancellationToken` (qui la **observe**).

---

## Un modèle coopératif

Point essentiel à comprendre d'emblée : en .NET, **on n'interrompt pas une tâche de force**. Tuer un thread en plein travail (l'ancien `Thread.Abort`) laisserait l'application dans un état incohérent, et n'existe d'ailleurs plus sur .NET moderne. L'annulation est **coopérative** : celui qui demande l'annulation lève un drapeau, et le code en cours d'exécution **vérifie ce drapeau** et s'arrête proprement de lui-même.

Les deux rôles se répartissent ainsi :

- Le **`CancellationTokenSource`** (CTS) est la télécommande : on appelle `Cancel()` dessus pour signaler l'annulation.
- Le **`CancellationToken`** est le capteur : on le passe à l'opération, qui l'interroge ou le transmet plus bas.

---

## `CancellationTokenSource` et `CancellationToken`

Le `CancellationTokenSource` produit un token via sa propriété `Token`, et déclenche l'annulation via `Cancel()`. Comme il peut détenir un minuteur interne, il est **`IDisposable`** : on le libère avec `Using` ou un `Dispose()` explicite. Voici le schéma classique d'un bouton « Annuler » :

```vb
Private _cts As CancellationTokenSource

Private Async Sub BtnDemarrer_Click(sender As Object, e As EventArgs) Handles BtnDemarrer.Click
    _cts = New CancellationTokenSource()
    Try
        Dim resultat = Await LongTraitementAsync(_cts.Token)
        AfficherResultat(resultat)
    Catch ex As OperationCanceledException
        _statut.Text = "Opération annulée."
    Finally
        _cts.Dispose()
        _cts = Nothing
    End Try
End Sub

Private Sub BtnAnnuler_Click(sender As Object, e As EventArgs) Handles BtnAnnuler.Click
    _cts?.Cancel()
End Sub
```

---

## Propager le token

Par convention, le `CancellationToken` est le **dernier paramètre** d'une méthode asynchrone, souvent facultatif. En VB, on lui donne la valeur par défaut `Nothing` : pour un type structure comme `CancellationToken`, cela équivaut à `CancellationToken.None`, c'est-à-dire « pas d'annulation ».

```vb
Async Function ChargerAsync(url As String,
                            Optional ct As CancellationToken = Nothing) As Task(Of String)
    Using client As New HttpClient()
        Return Await client.GetStringAsync(url, ct)   ' on TRANSMET le token
    End Using
End Function
```

La règle d'or est de **faire suivre le token** à toutes les méthodes du framework qui en acceptent un — et beaucoup en acceptent : `HttpClient.GetAsync(url, ct)`, `Task.Delay(delai, ct)`, `Stream.ReadAsync(tampon, ct)`, ou encore `ToListAsync(ct)` côté EF Core. Sans cette transmission, l'annulation ne « descend » pas jusqu'à l'opération bloquante et reste sans effet.

---

## Répondre à l'annulation

Trois moyens, selon le contexte :

`ThrowIfCancellationRequested()` lève une `OperationCanceledException` si l'annulation est demandée. C'est l'outil de choix dans une **boucle**, à placer à un point sûr (entre deux unités de travail) :

```vb
Async Function TraiterFichiersAsync(fichiers As IEnumerable(Of String),
                                    ct As CancellationToken) As Task
    For Each fichier In fichiers
        ct.ThrowIfCancellationRequested()                 ' point de contrôle
        Dim contenu = Await File.ReadAllTextAsync(fichier, ct)
        Await TraiterAsync(contenu, ct)
    Next
End Function
```

`IsCancellationRequested` est un simple booléen, utile quand on veut **terminer proprement** (libérer des ressources, sauvegarder un état partiel) avant de s'arrêter, plutôt que de lever une exception sèche.

`Register(...)` enregistre un rappel exécuté **au moment** de l'annulation. Il sert à faire coopérer une opération qui n'accepte pas de token, en l'interrompant par un autre moyen :

```vb
Using inscription = ct.Register(Sub() connexion.Abort())
    Await connexion.RecevoirAsync()
End Using
```

---

## L'annulation n'est pas une erreur

Comme annoncé en [4.3](03-exceptions-async.md), une opération annulée relance une **`OperationCanceledException`** (ou sa dérivée `TaskCanceledException`) à l'`Await`. Ce n'est pas un échec ordinaire : on l'attrape **séparément**, pour ne pas la journaliser comme une vraie erreur.

```vb
Try
    Await LongTraitementAsync(ct)
Catch ex As OperationCanceledException
    ' Annulation attendue : information, pas erreur
    _journal.Info("Traitement annulé par l'utilisateur.")
Catch ex As Exception
    ' Véritable défaillance
    _journal.Erreur("Échec du traitement", ex)
End Try
```

> **À noter** — Utiliser `ct.ThrowIfCancellationRequested()` (plutôt que de lever soi-même une `OperationCanceledException`) garantit que la tâche se termine bien dans l'état **annulé** (*Canceled*), et non *en erreur* (*Faulted*). La distinction compte pour `Task.WhenAll` et les continuations, qui traitent différemment les deux états.

---

## Le timeout

Un timeout n'est qu'une annulation **déclenchée par le temps**. Plusieurs approches, de la plus simple à la plus complète.

**Annulation automatique après un délai** — le constructeur `CancellationTokenSource(TimeSpan)` (ou la méthode `CancelAfter`) programme l'annulation :

```vb
Using cts As New CancellationTokenSource(TimeSpan.FromSeconds(10))
    Try
        Return Await ChargerAsync(url, cts.Token)
    Catch ex As OperationCanceledException
        Throw New TimeoutException("Le chargement a dépassé 10 secondes.")
    End Try
End Using
```

**Combiner timeout et annulation utilisateur** — quand l'opération peut être interrompue *soit* par un délai, *soit* par l'utilisateur, on fusionne les deux sources avec `CreateLinkedTokenSource` : le token lié est annulé dès que **l'une** des sources l'est. Un filtre `When` (voir [4.3](03-exceptions-async.md)) permet alors de distinguer les deux causes :

```vb
Async Function ChargerAvecDelaiAsync(url As String,
                                     delai As TimeSpan,
                                     tokenUtilisateur As CancellationToken) As Task(Of String)
    Using ctsDelai As New CancellationTokenSource(delai)
        Using ctsLie = CancellationTokenSource.CreateLinkedTokenSource(
                            tokenUtilisateur, ctsDelai.Token)
            Try
                Return Await ChargerAsync(url, ctsLie.Token)
            Catch ex As OperationCanceledException When ctsDelai.IsCancellationRequested
                ' Seul le minuteur a déclenché → c'est un timeout, pas l'utilisateur
                Throw New TimeoutException($"Délai de {delai.TotalSeconds:N0}s dépassé.")
            End Try
        End Using
    End Using
End Function
```

**Appliquer un délai à n'importe quelle tâche** — depuis .NET 6, `Task.WaitAsync` impose un timeout (ou un token) à une tâche existante, **même si l'opération sous-jacente n'accepte aucun token** :

```vb
Try
    Dim resultat = Await OperationSansTokenAsync().WaitAsync(TimeSpan.FromSeconds(5))
Catch ex As TimeoutException
    ' La tâche n'a pas répondu dans les 5 secondes
End Try
```

> **Piège** — `WaitAsync` cesse seulement d'**attendre** : l'opération d'origine, elle, **continue de tourner** en arrière-plan (on a juste arrêté de la guetter). Pour réellement *stopper* le travail, il faut un `CancellationToken` que l'opération honore. `WaitAsync` est un garde-fou, pas une annulation.

---

## Annulation et parallélisme

L'annulation s'étend aux traitements parallèles : `Parallel.For`/`ForEach` et PLINQ acceptent un token via leurs options (`ParallelOptions.CancellationToken`). Le détail relève de la section suivante.

---

## Bonnes pratiques

- **Accepter et transmettre** un `CancellationToken` dans toute API asynchrone publique ; le faire suivre à chaque appel qui en accepte un. Ne passez pas `CancellationToken.None` quand vous détenez un vrai token à propager.
- **Libérer** le `CancellationTokenSource` (`Using` ou `Dispose`) — il peut détenir un minuteur.
- **Attraper l'`OperationCanceledException` à part** : c'est une information, pas une erreur ; ne la journalisez pas comme une défaillance.
- En interface, **renouveler le CTS** à chaque opération (un nouveau pour chaque clic), et désactiver/réactiver les boutons en conséquence.
- `CancellationToken.None` (ou `Nothing` en VB) signifie explicitement « aucune annulation ».

---

## En résumé

- L'annulation .NET est **coopérative** : `CancellationTokenSource` demande, `CancellationToken` observe, le code vérifie et s'arrête.
- On **transmet** le token (dernier paramètre, souvent `Optional … = Nothing`) à toutes les méthodes qui en acceptent un.
- On **répond** via `ThrowIfCancellationRequested()` (boucles), `IsCancellationRequested` (arrêt propre) ou `Register(...)` (API non coopérative).
- L'annulation se manifeste par une `OperationCanceledException`, qu'on attrape **séparément** des vraies erreurs.
- Le **timeout** est une annulation par le temps : `CancellationTokenSource(TimeSpan)`, `CreateLinkedTokenSource` pour combiner les causes, `Task.WaitAsync` (depuis .NET 6) pour borner n'importe quelle tâche — sans pour autant l'interrompre.

Nous avons fait le tour de l'asynchronie d'E/S : sa raison d'être, sa mécanique, ses erreurs, son annulation. Place à l'autre forme de concurrence — exécuter plusieurs **calculs** de front pour exploiter tous les cœurs du processeur.


⏭️ [Parallélisme pragmatique (Parallel.For/ForEach, PLINQ)](/04-async/05-parallelisme.md)
