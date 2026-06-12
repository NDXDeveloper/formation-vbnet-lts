🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 12.3 — Journalisation (`Microsoft.Extensions.Logging`, Serilog, *structured logging*)

> **Module 12 — Exceptions, débogage et journalisation**
> Savoir ce que fait l'application *en production*, là où le débogueur ne peut plus aller.

---

## Pourquoi journaliser

Le débogueur (12.2) suppose que vous êtes là, devant l'écran, pendant que le problème survient.
Mais en production, vous n'y êtes pas : l'application tourne chez le client, sur un serveur,
parfois la nuit. La **journalisation** est ce qui laisse une trace exploitable de ce qui s'est
passé — hier comme en ce moment même.

C'est aussi l'aboutissement naturel de l'instrumentation légère évoquée en fin de 12.2 : quand
un `Debug.WriteLine` ponctuel devient un enregistrement permanent, structuré, avec des niveaux
et des destinations, on ne fait plus du débogage mais de la journalisation.

> 🟢 **Entièrement dans le périmètre de VB.NET.** Toute la pile de journalisation moderne
> (`Microsoft.Extensions.Logging`, Serilog…) est constituée de **bibliothèques** que l'on
> **consomme** via leurs API. C'est exactement le scénario *consumption-only* recommandé pour
> VB.NET : aucune limitation par rapport à C#. Une seule réserve, signalée plus bas, concerne le
> **générateur de source** `[LoggerMessage]`, réservé à C#.

---

## Journalisation vs `Console.WriteLine` / `Debug.WriteLine`

| | `Console.WriteLine` / `Debug.WriteLine` | Journalisation |
|---|---|---|
| **Niveaux** | aucun | Trace → Critical |
| **Destinations** | une seule, figée | console, fichier, base, service… simultanément |
| **Filtrage** | aucun | par niveau et par catégorie, configurable |
| **Structure** | texte plat | propriétés **requêtables** (logging structuré) |
| **Production** | inadapté | conçu pour ça |

`Debug.WriteLine` reste utile pour un diagnostic *temporaire* en développement (il disparaît en
`Release`). Pour tout le reste, on journalise.

---

## `Microsoft.Extensions.Logging` : l'abstraction standard

C'est **l'abstraction de journalisation officielle de .NET**. Le code écrit contre son interface
`ILogger` ne dépend d'aucune implémentation particulière : on peut brancher la console, des
fichiers, Serilog, Application Insights… sans toucher au code métier.

### Les briques

- **`ILogger` / `ILogger(Of T)`** : l'interface que votre code utilise pour journaliser. La
  variante générique `ILogger(Of T)` associe automatiquement la **catégorie** au nom complet du
  type `T`.
- **`ILoggerFactory`** : crée des loggers et orchestre les fournisseurs.
- **`ILoggerProvider`** : une destination (console, fichier, etc.).

```vb
Imports Microsoft.Extensions.Logging

Public Class ServiceCommandes
    Private ReadOnly _logger As ILogger(Of ServiceCommandes)

    Public Sub New(logger As ILogger(Of ServiceCommandes))
        _logger = logger   ' injecté par le conteneur (voir plus bas)
    End Sub

    Public Sub Traiter(commandeId As Integer)
        _logger.LogInformation("Traitement de la commande {CommandeId}", commandeId)
    End Sub
End Class
```

### Les niveaux de log

| Niveau | Quand l'utiliser |
|--------|------------------|
| `Trace` | Détail extrême, à des fins de mise au point. **Jamais** en production (peut contenir des données sensibles). |
| `Debug` | Information de diagnostic utile au développement. |
| `Information` | Déroulement normal de l'application (« commande traitée »). |
| `Warning` | Situation anormale mais non bloquante (tentative en échec, repli). |
| `Error` | Échec d'une opération courante — souvent associé à une exception. |
| `Critical` | Défaillance grave (panne, corruption) exigeant une attention immédiate. |
| `None` | Désactive la journalisation. |

### Les méthodes et les fournisseurs

À chaque niveau correspond une méthode d'extension : `LogTrace`, `LogDebug`, `LogInformation`,
`LogWarning`, `LogError`, `LogCritical`. Les fournisseurs intégrés couvrent la console
(`AddConsole`), la fenêtre de sortie du débogueur (`AddDebug`), le **journal d'événements
Windows** (`AddEventLog`), `EventSource`, etc. ; d'innombrables fournisseurs tiers complètent la
liste.

> 💡 **Pour une bibliothèque** (un scénario phare de VB.NET), ne dépendez que du paquet
> `Microsoft.Extensions.Logging.Abstractions` : il contient `ILogger` sans tirer toute
> l'infrastructure. La bibliothèque journalise « à l'aveugle » ; c'est l'application hôte qui
> décide *où* vont les logs.

---

## Le logging structuré : le concept clé

C'est l'idée la plus importante de cette section. Un log structuré n'est pas une chaîne de texte,
c'est un **message accompagné de propriétés nommées**. On l'obtient avec un **modèle de message**
contenant des espaces réservés `{…}` :

```vb
_logger.LogInformation("Commande {CommandeId} traitée en {DureeMs} ms", commandeId, duree)
```

Ici, le système de journalisation n'enregistre pas seulement la phrase finale : il conserve
`CommandeId` et `DureeMs` comme **propriétés exploitables**. Dans un outil comme Seq, Elastic ou
Application Insights, on peut alors **filtrer** (`CommandeId = 4271`), **agréger** (durée moyenne)
ou **alerter** — ce qu'un texte plat ne permet jamais.

### ⚠️ Le piège n°1 en VB.NET : ne pas utiliser l'interpolation `$"…"`

Le réflexe naturel en VB.NET est d'écrire une chaîne interpolée. **C'est précisément ce qu'il ne
faut pas faire dans un message de log :**

```vb
' ❌ À PROSCRIRE : on perd toute la structure, et la chaîne est TOUJOURS construite
_logger.LogInformation($"Commande {commandeId} traitée en {duree} ms")
```

```vb
' ✅ Modèle de message avec espaces réservés nommés
_logger.LogInformation("Commande {CommandeId} traitée en {DureeMs} ms", commandeId, duree)
```

La version interpolée a deux défauts : elle réduit le log à un **texte plat** (plus de propriétés
`CommandeId`/`DureeMs` requêtables), et elle **construit la chaîne même si le niveau est
désactivé** (coût inutile). La version à modèle, elle, ne formate la chaîne que si nécessaire et
préserve la structure.

> 💡 **Deux points à retenir :**
> - Les espaces réservés sont associés aux arguments **par position**, pas par nom : l'ordre
>   compte (`{CommandeId}` ↔ 1ᵉʳ argument, `{DureeMs}` ↔ 2ᵉ).
> - L'analyseur **CA2254** vous avertit lorsqu'un modèle de message n'est pas une constante
>   (typiquement quand vous glissez une interpolation). Écoutez-le.

### Journaliser une exception

Reprenons le fil de la [12.1](01-exceptions.md) : passez l'exception en **premier argument**,
pour que la *stack trace* soit capturée de façon structurée — ne la noyez pas dans le modèle.

```vb
Try
    Traiter(commandeId)
Catch ex As Exception
    _logger.LogError(ex, "Échec du traitement de la commande {CommandeId}", commandeId)
    Throw   ' on relance en préservant la pile (cf. 12.1)
End Try
```

---

## Obtenir et configurer un logger

### En Web API ou application à hôte générique

Quand l'application utilise le **Generic Host** (Web API, et services de fond câblés à la main —
cf. [4.8](../04-async/08-background-services.md)), la journalisation est déjà branchée : il suffit
de **demander un `ILogger(Of T)` dans le constructeur**, le conteneur d'injection s'occupe du
reste. La configuration se fait alors le plus souvent dans `appsettings.json`.

### ⚠️ En Windows Forms : pas d'hôte par défaut

Le scénario phare de VB.NET mérite une mise au point. Une application **Windows Forms** ne dispose
**pas** d'un Generic Host ni d'un conteneur d'injection par défaut. Deux options :

1. **Créer une `LoggerFactory` à la main** (le plus simple) :

```vb
Imports Microsoft.Extensions.Logging

' Au démarrage de l'application, conservée pour toute sa durée de vie
Dim fabrique = LoggerFactory.Create(
    Sub(builder)
        builder.AddDebug()
        builder.AddConsole()
        builder.SetMinimumLevel(LogLevel.Information)
    End Sub)

Dim logger = fabrique.CreateLogger(Of MainForm)()
logger.LogInformation("Application démarrée")
```

> ⚠️ **Ne nommez pas la variable `loggerFactory`.** VB étant insensible à la casse, ce nom
> entre en **collision avec la classe `LoggerFactory`** elle-même (erreur BC30980) — le piège
> déjà rencontré avec `host`/`Host` ([4.8](../04-async/08-background-services.md)) et
> `aes`/`Aes` ([7.6](../07-acces-donnees/06-fichiers-io.md)). Le `var loggerFactory = …` des
> exemples C# ne se transpose donc pas tel quel : on choisit un autre nom (`fabrique`).

2. **Mettre en place un Generic Host manuellement** pour bénéficier de l'injection de dépendances
   et de la configuration (`appsettings.json`), puis résoudre les formulaires et services depuis
   le conteneur. Plus structurant pour une application LOB ambitieuse.

> 💡 N'oubliez pas de **libérer** la `LoggerFactory` (`Dispose`) à la fermeture de l'application,
> afin que les fournisseurs vident bien leurs tampons (*flush*).

### Configuration via `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "MonApp.Services": "Debug"
    }
  }
}
```

Chaque clé est une **catégorie** (souvent un espace de noms) et fixe le niveau **minimal** à
journaliser pour cette branche. Ici : tout en `Information`, sauf le framework Microsoft réduit
au `Warning`, et `MonApp.Services` détaillé jusqu'au `Debug`.

---

## Filtrage par niveau et catégorie

Le filtrage évite à la fois le bruit et le coût. Le principe : pour une **catégorie** donnée,
seuls les messages d'un niveau **supérieur ou égal** au minimum configuré sont émis. On règle
ainsi finement la verbosité branche par branche — verbeux sur *votre* code, silencieux sur le
framework — sans recompiler.

---

## Les portées (`BeginScope`) : enrichir le contexte

Une **portée** attache automatiquement des propriétés à *tous* les logs émis pendant son
existence. Idéal pour rattacher un identifiant de requête, d'utilisateur ou de commande à une
série d'opérations, sans le répéter à chaque appel.

```vb
Using _logger.BeginScope("Commande {CommandeId}", commandeId)
    _logger.LogInformation("Début du traitement")
    ValiderStock()
    EnregistrerPaiement()
    _logger.LogInformation("Traitement terminé")   ' tous ces logs portent CommandeId
End Using
```

> ⚠️ Les portées ne sont visibles que si le fournisseur les prend en charge et qu'elles sont
> activées (par exemple `IncludeScopes = True` sur le fournisseur Console). Serilog, lui, les
> intègre naturellement via son `LogContext`.

---

## Performance

La journalisation est conçue pour être économe, **à condition de bien l'utiliser**.

### Garder les arguments coûteux derrière `IsEnabled`

Si le calcul d'un argument est lui-même onéreux, vérifiez d'abord que le niveau est actif :

```vb
If _logger.IsEnabled(LogLevel.Debug) Then
    _logger.LogDebug("État détaillé : {Etat}", CalculerEtatCouteux())
End If
```

### ⚠️ Le générateur de source `[LoggerMessage]` est réservé à C#

Pour la journalisation à très haute fréquence, .NET propose une **journalisation source-générée**
au moyen de méthodes partielles décorées de `[LoggerMessage]` : le code optimal est produit à la
compilation, sans allocation ni *boxing*.

> ⚠️ **Ce générateur de source n'est disponible qu'en C#** — c'est l'un des sujets que cette
> formation signale comme frontière C# (au même titre que les *source generators* de
> `CommunityToolkit.Mvvm`, cf. [6.6](../06-wpf/06-mvvm.md) et l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).
> En VB.NET, on obtient un résultat équivalent avec l'**API d'exécution** `LoggerMessage.Define`,
> qui préexiste au générateur et fonctionne parfaitement :

```vb
Imports Microsoft.Extensions.Logging
Imports System.Runtime.CompilerServices

Public Module LogsCommandes
    ' Délégué défini une seule fois, réutilisé à chaque appel
    Private ReadOnly _commandeTraitee As Action(Of ILogger, Integer, Exception) =
        LoggerMessage.Define(Of Integer)(
            LogLevel.Information,
            New EventId(1, NameOf(CommandeTraitee)),
            "Commande {CommandeId} traitée avec succès")

    <Extension>
    Public Sub CommandeTraitee(logger As ILogger, commandeId As Integer)
        _commandeTraitee(logger, commandeId, Nothing)
    End Sub
End Module
```

> 🔗 **Stratégie hybride :** si vous écrivez réellement du code de journalisation à très haute
> fréquence et que vous tenez aux méthodes `[LoggerMessage]` source-générées, isolez ce point
> dans une **petite bibliothèque C#** consommée depuis VB.NET (cf. [module 10](../10-hybride-vbnet-csharp/README.md)).
> Pour l'immense majorité des applications, `LoggerMessage.Define` — voire un simple
> `LogInformation` à modèle — suffit amplement.

---

## Serilog : journalisation structurée riche

`Microsoft.Extensions.Logging` est l'**abstraction** ; il lui faut une **implémentation** pour
décider où et comment écrire. **Serilog** est l'un des choix les plus répandus, pensé pour le
logging structuré de bout en bout. Il apporte deux notions clés :

- les **puits** (*sinks*) : les destinations (console, fichier, **Seq**, base de données,
  Elasticsearch, Application Insights…), que l'on peut **cumuler** ;
- les **enrichisseurs** (*enrichers*) : ils ajoutent automatiquement des propriétés à chaque log
  (nom de la machine, identifiant de thread, contexte applicatif…).

### Configuration en VB.NET

Serilog se configure par une **chaîne fluide**. En VB.NET, utilisez la continuation de ligne
explicite `_` pour enchaîner les appels :

```vb
Imports Serilog

Log.Logger = New LoggerConfiguration() _
    .MinimumLevel.Information() _
    .Enrich.FromLogContext() _
    .Enrich.WithMachineName() _
    .WriteTo.Console() _
    .WriteTo.File("logs/app-.log", rollingInterval:=RollingInterval.Day) _
    .CreateLogger()

' Utilisation directe de l'API statique
Log.Information("Application {App} démarrée", "MonApp")
```

Le puits fichier ci-dessus effectue une **rotation quotidienne** (`rollingInterval:=RollingInterval.Day`) :
un fichier par jour, sans intervention.

### Brancher Serilog derrière `ILogger`

Le meilleur des deux mondes : votre code reste écrit contre l'abstraction standard `ILogger`, et
Serilog fait le travail en coulisses. On relie les deux via `Serilog.Extensions.Logging` :

```vb
Dim factory = LoggerFactory.Create(Sub(builder) builder.AddSerilog())
Dim logger = factory.CreateLogger(Of MainForm)()
logger.LogInformation("Pris en charge par Serilog, via ILogger")
```

> 💡 Dans une application à hôte générique, l'intégration est encore plus directe
> (`AddSerilog()` sur la collection de services). Pensez à appeler `Log.CloseAndFlush()` à
> l'arrêt pour garantir l'écriture des derniers messages.

---

## Bonnes pratiques

- **Logging structuré, pas de concaténation.** Toujours un modèle de message avec espaces
  réservés ; jamais d'interpolation `$"…"` ni de `&` dans le message.
- **Le bon niveau, avec discipline.** `Information` pour le flux normal, `Warning` pour
  l'anormal-non-bloquant, `Error` pour les échecs (avec l'exception), `Critical` pour les
  pannes. Réservez `Trace`/`Debug` au développement.
- **⚠️ Jamais de données sensibles dans les logs.** Pas de mots de passe, jetons, numéros de
  carte complets, ni de données personnelles non nécessaires : c'est à la fois un risque de
  **sécurité** (voir [module 16](../16-securite/README.md)) et un enjeu **RGPD**.
- **Journalisez à la frontière, une seule fois.** Évitez le « *log-and-throw* » à chaque niveau,
  qui multiplie les entrées pour une même erreur. Capturez le contexte là où il est le plus
  riche, journalisez-y, puis laissez remonter.
- **Passez l'exception en premier argument** de `LogError`/`LogCritical`, pas seulement
  `ex.Message`.
- **Dépendez de l'abstraction.** Dans vos bibliothèques, `ILogger` (paquet *Abstractions*)
  uniquement ; l'hôte choisit l'implémentation.
- **Pensez à la corrélation.** Un identifiant de requête/transaction (via `BeginScope`) permet
  de relier tous les logs d'une même opération — indispensable dès que le volume grossit.
- **Videz les tampons à l'arrêt** (`Dispose` de la factory, `Log.CloseAndFlush()` pour Serilog).

---

## Vers l'observabilité

La journalisation structurée n'est que l'un des trois piliers de l'**observabilité** — avec les
**métriques** et les **traces distribuées**. Dès qu'une application dépasse le poste isolé
(plusieurs services, appels en chaîne, montée en charge), savoir *ce qui se passe* suppose de
corréler ces trois signaux. C'est l'objet de la section suivante.

---

## À retenir

Journaliser, ce n'est pas afficher du texte : c'est produire des **enregistrements structurés**,
filtrables et exploitables, qui survivent à l'instant du bug. En VB.NET, toute la pile moderne
est accessible sans réserve — `Microsoft.Extensions.Logging` comme abstraction, Serilog comme
implémentation riche — à deux nuances près, propres au langage : on évite l'**interpolation**
dans les messages (au profit des modèles à espaces réservés), et le **générateur de source**
`[LoggerMessage]` cède la place à `LoggerMessage.Define`. Tout le reste est identique à C#.

➡️ Section suivante : **[12.4 — Observabilité (notions)](04-observabilite.md)**, pour étendre la
journalisation aux métriques et aux traces distribuées.

⏭️ [Observabilité (notions : OpenTelemetry, *health checks*, métriques)](/12-exceptions-debogage/04-observabilite.md)
