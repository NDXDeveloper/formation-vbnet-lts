🔝 Retour au [Sommaire](/SOMMAIRE.md)


# 4.8 Services en arrière-plan : Generic Host et `BackgroundService`

Beaucoup d'applications ont besoin d'un travail qui tourne **en continu**, en arrière-plan : interroger une file de messages, scruter un dossier, envoyer des notifications, exécuter une tâche toutes les dix minutes. .NET fournit pour cela une infrastructure complète — le **Generic Host** — et une classe de base, `BackgroundService`, qui couvre le cas courant. Tout cela s'utilise pleinement en VB.NET ; la seule friction, signalée dans l'[introduction du module](README.md), est **l'absence de modèle de projet « Worker »** : on câble l'hôte à la main.

---

## Le Generic Host (`Microsoft.Extensions.Hosting`)

Le *Generic Host* assemble en un seul objet (`IHost`) les briques transverses d'une application : **injection de dépendances**, **configuration**, **journalisation**, **gestion du cycle de vie** et **arrêt propre**. On le construit avec un *builder*, on enregistre ses services, puis on le lance.

```vb
Dim builder = Host.CreateApplicationBuilder(args)
' builder.Services.Add... (enregistrement des dépendances)
Dim hote = builder.Build()
hote.Run()
```

> ⚠️ **Piège VB — ne nommez pas la variable `host`.** Tout le code C# du monde écrit `var host = builder.Build();` après `Host.CreateApplicationBuilder(...)`. Transposé tel quel en VB, **cela ne compile pas** : VB étant **insensible à la casse**, la variable locale `host` masque la classe `Host` dans tout le bloc, et l'appel `Host.CreateApplicationBuilder` échoue avec l'erreur BC32000 (« référence à la variable locale avant sa déclaration »). On nomme donc la variable autrement (`hote`, `app`…) — ou l'on qualifie la classe (`Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(...)`). C'est un cas d'école du « biais C# » des assistants IA.

---

## `IHostedService` et `BackgroundService`

Un service hébergé implémente `IHostedService`, dont les deux méthodes — `StartAsync(ct)` et `StopAsync(ct)` — sont appelées au démarrage et à l'arrêt de l'hôte. C'est l'interface de bas niveau, utile quand on a besoin d'un contrôle fin du démarrage et de l'arrêt.

Pour le cas le plus fréquent — une boucle qui tourne jusqu'à l'arrêt —, on préfère la classe de base **`BackgroundService`**, dont on redéfinit la seule méthode `ExecuteAsync`. Son paramètre `stoppingToken` est un `CancellationToken` (voir [4.4](04-annulation-timeout.md)) **signalé à l'arrêt** de l'application : c'est par lui qu'on quitte la boucle proprement.

```vb
Imports System.Threading
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Public Class ServiceDeNettoyage
    Inherits BackgroundService

    Private ReadOnly _journal As ILogger(Of ServiceDeNettoyage)

    Public Sub New(journal As ILogger(Of ServiceDeNettoyage))
        _journal = journal                          ' injection par constructeur
    End Sub

    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        Try
            Do While Not stoppingToken.IsCancellationRequested
                _journal.LogInformation("Nettoyage à {Heure}", DateTimeOffset.Now)
                Await NettoyerAsync(stoppingToken)
                Await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken)
            Loop
        Catch ex As OperationCanceledException
            ' Arrêt demandé : sortie normale
        End Try
    End Function

    Private Async Function NettoyerAsync(ct As CancellationToken) As Task
        ' ... travail de fond ...
    End Function
End Class
```

On l'enregistre auprès du conteneur via `AddHostedService` :

```vb
builder.Services.AddHostedService(Of ServiceDeNettoyage)()
```

---

## Pas de modèle « Worker » en VB.NET — le câblage manuel ⚠️

En C#, le modèle de projet **Worker Service** (`dotnet new worker`) génère un projet prêt à l'emploi, avec l'hôte et un `Worker` déjà câblés. **Ce modèle n'existe pas en VB.NET** : le SDK ne propose qu'une poignée de modèles VB (Console, bibliothèque de classes, Windows Forms, WPF, tests) — *Worker* n'en fait pas partie.

La conséquence est simple, et sans gravité : on crée un projet **Console** classique et on câble le *Generic Host* à la main.

```vb
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.DependencyInjection

Module Program
    Sub Main(args As String())
        Dim builder = Host.CreateApplicationBuilder(args)

        ' Enregistrement des dépendances et du service de fond
        builder.Services.AddHostedService(Of ServiceDeNettoyage)()

        Dim hote = builder.Build()   ' « hote », pas « host » : voir le piège plus haut
        hote.Run()                   ' bloque jusqu'à l'arrêt
    End Sub
End Module
```

> **À noter** — La limite « pas d'async Main » de VB ([4.2](02-async-await.md)) **ne pose aucun problème ici** : `host.Run()` est une méthode **synchrone** qui bloque jusqu'à l'arrêt de l'application. Un `Sub Main` ordinaire suffit, sans le moindre `Await`. (Si l'on tenait à `host.RunAsync()`, on le pontèrait avec `GetAwaiter().GetResult()`, comme vu en 4.2 — mais c'est inutile.)

---

## Injection de dépendances : attention aux portées

Un `BackgroundService` est enregistré comme **singleton** : il vit aussi longtemps que l'hôte. On **ne peut donc pas** y injecter directement un service à portée *scoped* (par exemple un `DbContext` EF Core), au risque de le conserver indéfiniment. Le motif correct consiste à **créer une portée** à chaque cycle de travail :

```vb
Private ReadOnly _fournisseur As IServiceProvider   ' injecté par constructeur

Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
    Try
        Do While Not stoppingToken.IsCancellationRequested
            Using portee = _fournisseur.CreateScope()
                Dim contexte = portee.ServiceProvider.GetRequiredService(Of AppDbContext)()
                Await TraiterAsync(contexte, stoppingToken)
            End Using                                  ' portée libérée à chaque tour
            Await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken)
        Loop
    Catch ex As OperationCanceledException
    End Try
End Function
```

---

## Travaux périodiques

La boucle `Await Task.Delay(intervalle, stoppingToken)` ci-dessus est l'approche la plus simple, mais l'intervalle ne tient pas compte de la **durée du travail** (dérive progressive). Depuis .NET 6, `PeriodicTimer` cadence sur un rythme **fixe** :

```vb
Using timer As New PeriodicTimer(TimeSpan.FromMinutes(5))
    Try
        Do While Await timer.WaitForNextTickAsync(stoppingToken)
            Await FaireLeTravailAsync(stoppingToken)
        Loop
    Catch ex As OperationCanceledException
    End Try
End Using
```

`WaitForNextTickAsync` renvoie un `ValueTask(Of Boolean)` — on l'**attend** sans difficulté (VB consomme `ValueTask`, voir [4.6](06-async-streams.md)). Pour de la planification de type *cron*, on se tourne vers des bibliothèques dédiées comme **Quartz.NET** ou **Hangfire** : ce ne sont que des paquets .NET, donc pleinement utilisables depuis VB (consommation).

---

## Exécuter en tant que service Windows

Le **même code** d'hôte peut tourner comme application Console (en développement) ou comme **service Windows** (en production). Il suffit du paquet `Microsoft.Extensions.Hosting.WindowsServices` et d'une ligne :

```vb
Dim builder = Host.CreateApplicationBuilder(args)
builder.Services.AddWindowsService()                       ' intégration au gestionnaire de services
builder.Services.AddHostedService(Of ServiceDeNettoyage)()
builder.Build().Run()
```

On installe ensuite le binaire publié comme service (`sc create`, ou `New-Service` en PowerShell). Là encore, rien de spécifique à VB : ce sont des appels de bibliothèque.

---

## Arrêt propre (*graceful shutdown*)

Lorsqu'on interrompt l'application (Ctrl+C, `SIGTERM`, arrêt du service Windows), l'hôte **signale le `stoppingToken`** et laisse aux services un délai pour se terminer (le *shutdown timeout*). C'est tout l'intérêt de l'annulation coopérative de [4.4](04-annulation-timeout.md) : un service bien écrit surveille `stoppingToken`, finit son cycle en cours et libère ses ressources, plutôt que d'être coupé net.

---

## En résumé

- Le **Generic Host** (`Microsoft.Extensions.Hosting`) fournit DI, configuration, journalisation, cycle de vie et arrêt propre.
- On implémente du travail de fond via `IHostedService` (`StartAsync`/`StopAsync`) ou, plus simplement, en héritant de **`BackgroundService`** (`ExecuteAsync(stoppingToken)`), enregistré par `AddHostedService`.
- **Limite VB** ⚠️ : pas de modèle « Worker » — on câble le *Generic Host* à la main dans un projet **Console**. La méthode **synchrone** `host.Run()` rend l'absence d'async Main sans conséquence.
- Pour les services *scoped* (ex. `DbContext`), créer une **portée** par cycle (`CreateScope`).
- Travaux périodiques : boucle `Task.Delay`, ou `PeriodicTimer` (.NET 6+) pour un rythme fixe ; *cron* via Quartz.NET / Hangfire.
- Le même hôte tourne en **service Windows** avec `AddWindowsService()`.

---

## Conclusion du module 4

Ce module a couvert l'ensemble de la concurrence telle qu'elle se pratique réellement en VB.NET : **pourquoi** l'asynchronie ([4.1](01-pourquoi-async.md)), sa **mécanique** ([4.2](02-async-await.md)), ses **exceptions** ([4.3](03-exceptions-async.md)) et son **annulation** ([4.4](04-annulation-timeout.md)) ; le **parallélisme** CPU ([4.5](05-parallelisme.md)) ; la **consommation** de flux asynchrones ([4.6](06-async-streams.md)) ; la **synchronisation** ([4.7](07-synchronisation.md)) ; et enfin l'**hébergement** de travail de fond (cette section).

Un fil rouge s'en dégage, fidèle au positionnement de la formation : **côté consommation, VB.NET fait tout** — `Async`/`Await`, `Task`, annulation, parallélisme, sémaphores, hôtes, services Windows. Les limites rencontrées concernent uniquement la **production** de certaines constructions modernes, là où il faut déléguer à C# ou contourner :

- pas d'**`Async Main`** ([4.2](02-async-await.md)) — pont via `GetAwaiter().GetResult()` ;
- pas de méthode **`Async … As ValueTask`** ([4.5](05-parallelisme.md), [4.6](06-async-streams.md)) — on enveloppe une `Task` ;
- pas d'**`Await For Each`**, pas d'**`Await` en `Finally`**, pas d'**itérateur asynchrone** ([4.6](06-async-streams.md)) — consommation manuelle, production en C# ;
- pas de **modèle « Worker »** (cette section) — câblage manuel du *Generic Host*.

Aucune de ces limites n'empêche d'écrire des applications concurrentes robustes en VB.NET — elles invitent simplement à connaître la **frontière VB / C#** (voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)) et à câbler proprement ce que le langage ne génère pas.


⏭️ [Windows Forms — le scénario phare](/05-windows-forms/README.md)
