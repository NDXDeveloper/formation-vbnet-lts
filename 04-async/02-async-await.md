🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 4.2 `Async`/`Await` (`Task`, `Task(Of T)`)

La section précédente a posé le *pourquoi*. Voici le *comment*. La programmation asynchrone en VB.NET repose sur trois briques, partagées trait pour trait avec C# — `Async` et `Await` ont d'ailleurs été conçus en même temps que leur équivalent C#, dès VB 11 (2012) :

- le modificateur **`Async`**, qui autorise une méthode à contenir des `Await` ;
- l'opérateur **`Await`**, qui suspend l'exécution sans bloquer le thread ;
- le type **`Task`** (ou **`Task(Of T)`**), qui représente l'opération en cours.

---

## Anatomie d'une méthode asynchrone

Une méthode asynchrone se reconnaît à trois éléments : le modificateur `Async`, un type de retour `Task` ou `Task(Of T)`, et au moins un `Await` dans le corps. Le suffixe **`Async`** dans le nom est une convention universelle (pas une obligation du langage) qui signale d'un coup d'œil qu'on manipule une `Task`.

```vb
' Renvoie une valeur (un String) : type de retour Task(Of String)
Async Function ChargerTexteAsync(url As String) As Task(Of String)
    Using client As New HttpClient()
        Return Await client.GetStringAsync(url)
    End Using
End Function

' Ne renvoie aucune valeur : type de retour Task
Async Function SauvegarderAsync(chemin As String, donnees As Byte()) As Task
    Await File.WriteAllBytesAsync(chemin, donnees)
End Function
```

Notez le retour : dans une `Async Function … As Task(Of String)`, on écrit `Return <unString>` — **la valeur brute**, pas une `Task`. Le compilateur se charge de l'emballer dans la `Task`. (Sans `Async`, ce serait l'inverse : on renverrait une `Task` directement, voir plus bas.)

> **À noter** — Un `Await` ne peut apparaître que dans une méthode ou une lambda marquée `Async`. Et une méthode `Async` *sans aucun* `Await` compile, mais avec un avertissement : elle s'exécute en réalité de façon synchrone.

---

## `Task` et `Task(Of T)` : que représentent-ils ?

Une `Task` n'est pas le résultat d'une opération : c'est une **promesse** que l'opération finira, accompagnée de son état. Pensez-y comme à un reçu de pressing — il ne contient pas vos vêtements, mais vous permet de les récupérer une fois prêts.

- `Task` représente une opération qui se **terminera** sans produire de valeur.
- `Task(Of T)` représente une opération qui se terminera en **produisant un `T`**.

Une `Task` traverse plusieurs états : en cours, **terminée avec succès**, **terminée en erreur** (*faulted* — voir [4.3](03-exceptions-async.md)) ou **annulée** (*canceled* — voir [4.4](04-annulation-timeout.md)). `Await` connaît ces états et réagit en conséquence : il restitue la valeur en cas de succès, et relance l'exception en cas d'erreur.

---

## `Await` : suspendre sans bloquer

C'est l'opérateur central. Face à une `Task` :

- si elle est **déjà terminée**, l'exécution se poursuit immédiatement avec son résultat ;
- si elle est **encore en cours**, `Await` **rend la main à l'appelant** — sans bloquer le thread — et planifie la reprise du code pour le moment où la tâche s'achèvera.

`Await` sur une `Task(Of T)` **déballe** le résultat : l'expression `Await maTache` a le type `T`. `Await` sur une `Task` (sans valeur) s'emploie comme une instruction.

```vb
Dim texte As String = Await ChargerTexteAsync(url)   ' Await Task(Of String) → String
Await SauvegarderAsync(chemin, octets)               ' Await Task → instruction
```

La différence fondamentale avec `.Result` ou `.Wait()` : `Await` **ne bloque pas** le thread pendant l'attente. C'est tout l'intérêt (voir [4.1](01-pourquoi-async.md)).

---

## Ce que fait le compilateur

Vous écrivez du code qui *ressemble* à du code synchrone, mais le compilateur le réécrit en **machine à états**. Concrètement, la méthode s'exécute normalement jusqu'au **premier `Await` portant sur une tâche non terminée** ; à cet instant, elle **retourne une `Task` à son appelant** et s'interrompt. Quand la tâche attendue s'achève, la suite (la *continuation*) reprend là où elle s'était arrêtée.

L'implication pratique : appeler une méthode asynchrone ne « lance pas un thread ». Tant qu'on n'a pas atteint un `Await` sur une opération réellement en attente, tout s'exécute sur le thread appelant. On ne consomme un mécanisme de reprise que là où c'est nécessaire.

---

## Enchaîner et composer des tâches

**En séquence**, chaque `Await` attend que le précédent soit terminé — comme du code synchrone, mais sans blocage :

```vb
Dim utilisateur = Await ChargerUtilisateurAsync(id)
Dim commandes = Await ChargerCommandesAsync(utilisateur.Id)   ' attend le premier
```

**En parallèle**, si deux opérations sont indépendantes, on les *démarre* d'abord, puis on les attend ensemble avec `Task.WhenAll`. Les deux appels réseau progressent alors simultanément :

```vb
Dim tMeteo = ChargerMeteoAsync(ville)        ' démarre…
Dim tTrafic = ChargerTraficAsync(ville)      ' …démarre aussi, sans attendre le premier

Dim resultats = Await Task.WhenAll(tMeteo, tTrafic)   ' resultats() As String
Dim meteo = resultats(0)
Dim trafic = resultats(1)
```

`Task.WhenAny`, lui, restitue la **première** tâche terminée — utile pour un délai d'expiration ou une course entre plusieurs sources :

```vb
Dim premiere = Await Task.WhenAny(tSourceA, tSourceB)   ' la Task terminée en premier
Dim valeur = Await premiere
```

> **À noter** — Le comportement de `Task.WhenAll` face aux exceptions (quand plusieurs tâches échouent) est traité en [4.3](03-exceptions-async.md).

---

## `Task.Run` : décharger un calcul

`Await` ne convient qu'aux opérations d'**E/S**. Pour un travail **intensif en CPU** que l'on veut sortir du thread d'interface, on le **décharge** sur un thread du pool avec `Task.Run`, puis on l'attend :

```vb
' Calcul lourd exécuté sur un thread d'arrière-plan ; l'UI reste réactive.
Dim image = Await Task.Run(Function() RedimensionnerImage(source, 4000, 3000))
```

Particularité VB.NET : pour passer une méthode existante (et non une lambda) à `Task.Run`, on utilise **`AddressOf`** (VB ne convertit pas implicitement un nom de méthode en délégué) :

```vb
Dim resultat = Await Task.Run(AddressOf CalculerStatistiques)
```

Rappel du [4.1](01-pourquoi-async.md) : on n'enveloppe **jamais** une opération d'E/S dans `Task.Run`. Si une variante `…Async` existe, on l'appelle directement. Le vrai parallélisme de données (plusieurs éléments traités de front) relève de la section [4.5](05-parallelisme.md).

---

## « Async de bout en bout »

L'asynchronie se propage **vers le haut** de la pile d'appels : une méthode asynchrone doit être *attendue* par un appelant lui-même asynchrone, et ainsi de suite. On évite à tout prix de **bloquer au milieu de la chaîne** avec `.Result` ou `.Wait()` — outre le gaspillage, cela peut provoquer un **interblocage** sur le thread d'interface (la continuation a besoin de ce thread… que `.Result` immobilise).

La règle « *async all the way* » se heurte toutefois à une question : où s'arrête la chaîne ? Au **point d'entrée** du programme — et c'est là qu'apparaît une spécificité VB.NET.

### Démarrer depuis `Main` : une limite propre à VB.NET ⚠️

En C#, on peut depuis la version 7.1 écrire `static async Task Main()` et utiliser `await` directement dans le point d'entrée. **VB.NET ne dispose pas de cette possibilité** : il n'existe pas d'`Async Function Main() As Task`. C'est un cas d'école du « biais C# » : la documentation et les assistants IA proposeront volontiers un `Main` asynchrone… qui ne compile pas en VB.

Le contournement consiste à garder un `Sub Main()` **synchrone** qui amorce une fonction asynchrone et l'attend une seule fois, au sommet, via `GetAwaiter().GetResult()` :

```vb
Module Program
    Sub Main(args As String())
        ' Unique point de blocage, au sommet de la chaîne.
        MainAsync(args).GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync(args As String()) As Task
        Dim texte = Await ChargerTexteAsync("https://exemple.com")
        Console.WriteLine(texte)
    End Function
End Module
```

On préfère `GetAwaiter().GetResult()` à `.Wait()` ou `.Result` car il **relance l'exception d'origine** telle quelle, au lieu de l'envelopper dans une `AggregateException` (voir [4.3](03-exceptions-async.md)).

> Cette contrainte ne concerne en pratique que les applications **Console**. En Windows Forms et WPF, l'amorçage est assuré par le framework (`Application.Run`), et les gestionnaires d'événements sont des `Async Sub` : le pont entre synchrone et asynchrone est déjà en place.

---

## Le contexte de reprise et `ConfigureAwait`

Par défaut, dans une application de bureau, le code situé après un `Await` reprend **sur le thread d'interface** (le *SynchronizationContext* capturé). C'est ce confort qui permet de mettre à jour les contrôles juste après un `Await`, sans `Invoke`.

Ce comportement a un coût (un retour planifié sur le thread d'interface) et n'a aucun intérêt dans du **code de bibliothèque** qui ne touche pas à l'interface. On le désactive alors avec `ConfigureAwait(False)`, qui autorise la reprise sur n'importe quel thread :

```vb
' Dans une bibliothèque : inutile de revenir sur le thread d'interface.
Dim json = Await client.GetStringAsync(url).ConfigureAwait(False)
```

Repères :

- **Code d'interface** (gestionnaires d'événements, où l'on manipule les contrôles au retour) : on **garde** le comportement par défaut.
- **Bibliothèques et code sans UI** : on emploie systématiquement `ConfigureAwait(False)` — meilleure performance et moindre risque d'interblocage.
- En **ASP.NET Core**, il n'existe pas de `SynchronizationContext` : la question ne se pose quasiment pas. C'est sur le **bureau** qu'elle compte.

---

## Renvoyer une `Task` sans `Async`

Toute méthode asynchrone n'a pas besoin du couple `Async`/`Await`. Si vous vous contentez de **transmettre** la `Task` d'un autre appel sans rien faire après, renvoyez-la directement : on économise une machine à états.

```vb
' Pas d'Async/Await : on relaie simplement la Task du dépôt.
Public Function ChargerProfilAsync(id As Integer) As Task(Of Profil)
    Return _depot.LireAsync(id)
End Function
```

De même, pour renvoyer une valeur **déjà disponible** depuis une signature asynchrone (typiquement en implémentant une interface asynchrone), on utilise des tâches préfabriquées — toujours **sans** `Async` :

```vb
Public Function ObtenirParDefautAsync() As Task(Of Integer)
    Return Task.FromResult(42)        ' Task(Of T) déjà terminée
End Function

Public Function RienAFaireAsync() As Task
    Return Task.CompletedTask         ' Task déjà terminée
End Function
```

> **À noter** — Transmettre la `Task` directement (sans `Await`) modifie subtilement la propagation des exceptions et le moment où s'exécutent les blocs `Using`/`Try` : à réserver aux relais purs, sans logique après l'appel.

---

## `Async Sub` : à réserver aux événements

`Async Sub` produit une opération **« lancer et oublier »** : l'appelant ne reçoit pas de `Task`, donc il ne peut ni l'attendre, ni récupérer son résultat, **ni intercepter ses exceptions** — une exception non gérée dans un `Async Sub` remonte directement au runtime et peut faire planter le processus (détaillé en [4.3](03-exceptions-async.md)).

Son **seul** usage légitime est le **gestionnaire d'événements**, car la signature d'un événement impose un `Sub` :

```vb
Private Async Sub BtnCharger_Click(sender As Object, e As EventArgs) Handles BtnCharger.Click
    BtnCharger.Enabled = False
    Try
        TxtResultat.Text = Await ChargerTexteAsync(url)
    Finally
        BtnCharger.Enabled = True
    End Try
End Sub
```

Partout ailleurs, une méthode asynchrone renvoie `Task` ou `Task(Of T)`.

---

## En résumé

- Trois briques : `Async` (autorise `Await`), `Await` (suspend sans bloquer), `Task`/`Task(Of T)` (l'opération en cours).
- Dans une `Async Function … As Task(Of T)`, on renvoie **la valeur** ; le compilateur l'emballe dans la `Task`.
- On **enchaîne** avec des `Await` successifs, on **parallélise** des opérations indépendantes avec `Task.WhenAll`/`WhenAny`, on **décharge** un calcul CPU avec `Task.Run` (avec `AddressOf` pour une méthode nommée).
- L'asynchronie se propage de bout en bout ; on ne bloque pas avec `.Result`/`.Wait()`.
- **Spécificité VB.NET** : pas d'`Async Main` ⚠️ — en Console, on amorce via `Sub Main()` + `MainAsync().GetAwaiter().GetResult()`.
- `ConfigureAwait(False)` dans les bibliothèques ; comportement par défaut dans le code d'interface.
- `Async Sub` uniquement pour les gestionnaires d'événements.

Reste un sujet qui change de nature en asynchrone : la gestion des erreurs. Comment une exception se propage-t-elle à travers une `Task`, et où la rattraper ?


⏭️ [Gestion des exceptions asynchrones](/04-async/03-exceptions-async.md)
