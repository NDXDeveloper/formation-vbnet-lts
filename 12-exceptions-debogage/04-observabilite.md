🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 12.4 — Observabilité (notions : OpenTelemetry, *health checks*, métriques)

> **Module 12 — Exceptions, débogage et journalisation**
> Une ouverture : étendre la journalisation aux métriques et aux traces, pour comprendre une
> application *distribuée et en mouvement*.

---

## De la journalisation à l'observabilité

La journalisation (12.3) répond à « *que s'est-il passé ?* ». Mais dès qu'une application se
compose de plusieurs services qui s'appellent en cascade, sous charge, d'autres questions
surgissent : *combien de requêtes par seconde ? où le temps est-il passé dans cette requête
précise ? le service est-il en bonne santé en ce moment ?* L'**observabilité** est la capacité à
répondre à ces questions à partir des signaux émis par le système.

> 📐 **Section « notions ».** Ce sujet est présenté comme une **ouverture**, pas comme un
> approfondissement. L'objectif est de comprendre le vocabulaire et de savoir *quand* ces outils
> deviennent pertinents — pas de construire une plateforme d'observabilité complète.

---

## Observabilité vs surveillance (*monitoring*)

On confond souvent les deux :

- La **surveillance** répond à des questions **connues d'avance** : « le CPU dépasse-t-il 80 % ? »,
  « le disque est-il plein ? ». On instrumente ce qu'on sait vouloir surveiller.
- L'**observabilité** vise à répondre à des questions **qu'on ne s'était pas posées** : explorer,
  après coup, *pourquoi* telle requête a été lente, sans avoir anticipé précisément ce
  scénario. C'est la différence entre « *known unknowns* » et « *unknown unknowns* ».

En pratique, l'observabilité repose sur trois types de signaux complémentaires.

---

## Les trois piliers : logs, métriques, traces

| Pilier | Question à laquelle il répond | Primitive .NET | Exemple |
|--------|-------------------------------|----------------|---------|
| **Journaux** (*logs*) | Que s'est-il passé *précisément* ? | `ILogger` (cf. [12.3](03-journalisation.md)) | « Commande 4271 en échec : stock insuffisant » |
| **Métriques** (*metrics*) | Combien ? À quelle vitesse ? Quelle tendance ? | `System.Diagnostics.Metrics` | « 120 commandes/min ; latence p95 = 80 ms » |
| **Traces** (*traces*) | Par où est passée cette requête, et où a-t-elle ralenti ? | `System.Diagnostics.Activity` | « Requête X : API → BDD (60 ms) → paiement (200 ms) » |

L'idée clé : ces trois signaux se **corrèlent**. Une métrique révèle une anomalie (« la latence
explose »), une trace montre *où* (« le service paiement »), et les journaux disent *pourquoi*
(« délai d'attente sur la base »).

---

## ⚠️ Le bon périmètre pour VB.NET

Avant d'aller plus loin, une mise au point honnête, fidèle à l'esprit de cette formation.

> ⚠️ **Pour une application de bureau VB.NET typique (Windows Forms, WPF), la journalisation de
> la 12.3 suffit le plus souvent.** Les traces distribuées et les *health checks* sont conçus
> pour les systèmes **répartis** : plusieurs services, appels réseau en chaîne, exécution
> serveur. Leur plein intérêt apparaît dans le scénario **Web API / services** — que VB.NET sait
> faire par contrôleurs (cf. [module 8](../08-services-web/README.md)).

Deux nuances complètent ce constat :

- **Tout est consommable depuis VB.NET.** Les primitives d'instrumentation (`Meter`,
  `ActivitySource`) et OpenTelemetry sont des **bibliothèques** : pleinement dans le périmètre
  *consumption-only* de VB.NET. Vous *pouvez* instrumenter une application VB, y compris de
  bureau (par exemple pour des métriques de performance ou d'usage).
- 🔗 **Mais l'habitat naturel de l'observabilité penche C#.** Microservices, conteneurs,
  Kubernetes, orchestration cloud-native… cet écosystème est majoritairement C# (cf.
  [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)). Côté VB, on **consomme** ces outils
  là où ils sont justifiés, sans chercher à reconstruire cet écosystème.

Autrement dit : connaissez ces notions, appliquez-les quand votre application le justifie
(typiquement une Web API), et n'en faites pas un dogme pour un simple outil de bureau.

---

## OpenTelemetry : le standard ouvert

**OpenTelemetry (OTel)** est le standard **ouvert et indépendant du fournisseur** pour produire
et exporter de la télémétrie (traces, métriques, journaux). Son intérêt : instrumenter **une
seule fois**, puis exporter vers **n'importe quelle destination** (Prometheus, Grafana, Jaeger,
Azure Monitor / Application Insights, Seq…) sans réécrire le code.

Un point d'architecture essentiel en .NET :

> 💡 **En .NET, on instrumente avec les primitives de la BCL, et OpenTelemetry collecte et
> exporte.** Les API natives de diagnostic — `Activity`/`ActivitySource` pour les traces,
> `Meter` pour les métriques, `ILogger` pour les journaux — ont été **alignées sur les concepts
> d'OpenTelemetry** (une `Activity` *est* un *span*). OpenTelemetry .NET se contente d'écouter
> ces sources et de les router vers les exporteurs. Conséquence pratique : **votre code VB.NET
> reste écrit contre la BCL standard** ; OTel se branche au-dessus, en configuration.

---

## Les métriques (`System.Diagnostics.Metrics`)

Une **métrique** est une mesure numérique agrégée dans le temps. Les principaux types :

- **Compteur** (*counter*) : une valeur qui ne fait qu'**augmenter** (nombre de commandes,
  d'erreurs…).
- **Compteur ajustable** (*up-down counter*) : peut monter et descendre (files d'attente,
  connexions actives…).
- **Histogramme** (*histogram*) : une **distribution** de valeurs (utile pour les latences et
  leurs percentiles p50/p95/p99).
- **Jauge observable** (*observable gauge*) : une valeur lue **à la demande** (mémoire utilisée,
  taille d'un cache…).

En VB.NET, on instrumente via la classe `Meter` :

```vb
Imports System.Diagnostics.Metrics

Public Class MetriquesCommandes
    Private Shared ReadOnly _meter As New Meter("MonApp.Commandes")

    Private Shared ReadOnly _commandesTraitees As Counter(Of Long) =
        _meter.CreateCounter(Of Long)(
            "commandes.traitees", "commande", "Nombre de commandes traitées")

    Public Shared Sub CommandeTraitee()
        _commandesTraitees.Add(1)
    End Sub
End Class
```

> 💡 Même sans OpenTelemetry, ces métriques sont consultables **en direct** avec l'outil
> `dotnet-counters` (cf. [14.1 — Profilage](../14-performance/01-profilage.md)). L'agent
> **`@profiler`** de Visual Studio 2026 (cf. [12.2](02-debogage.md)) capture lui aussi temps CPU
> et mémoire pendant le débogage.

---

## Les traces distribuées (`Activity` / `ActivitySource`)

Une **trace** suit une opération **de bout en bout**, même lorsqu'elle traverse plusieurs
services. Elle se compose de **spans** (en .NET, des `Activity`) imbriqués : chaque span
représente une étape, avec sa durée et ses attributs, reliés par un **identifiant de trace**
propagé d'un service à l'autre (standard *W3C Trace Context*).

```vb
Imports System.Diagnostics

Public Class ServiceCommandes
    Private Shared ReadOnly _source As New ActivitySource("MonApp.Commandes")

    Public Sub Traiter(commandeId As Integer)
        Using activite = _source.StartActivity("TraiterCommande")
            activite?.SetTag("commande.id", commandeId)
            ValiderStock(commandeId)
            EnregistrerPaiement(commandeId)
        End Using
    End Sub
End Class
```

Deux subtilités VB à noter :

- `StartActivity` renvoie `Nothing` si **aucun écouteur** n'est abonné à la source : d'où l'usage
  systématique de l'opérateur conditionnel `activite?.` pour éviter une `NullReferenceException`.
- `Activity` implémente `IDisposable` : le bloc `Using` **clôt automatiquement** le span (et
  mesure sa durée) en sortie.

C'est ce mécanisme qui permet, dans une architecture distribuée, de répondre à « *cette requête
a mis 2 secondes — où, exactement ?* » en visualisant la chaîne complète des appels.

---

## Les *health checks*

Un **contrôle de santé** (*health check*) est un point de terminaison qui répond à une question
simple : « *l'application est-elle en état de fonctionner ?* ». Il est interrogé par un
équilibreur de charge, un orchestrateur (Docker, Kubernetes) ou une supervision, qui décide d'y
router du trafic — ou de redémarrer l'instance.

On distingue classiquement :

- la **vivacité** (*liveness*) : « le processus est-il vivant ? » (sinon, on le redémarre) ;
- la **disponibilité** (*readiness*) : « est-il prêt à recevoir des requêtes ? » (dépendances
  initialisées, base accessible…).

Côté ASP.NET Core (donc en Web API VB.NET par contrôleurs), la mise en place est directe via
`Microsoft.Extensions.Diagnostics.HealthChecks` :

```vb
' Enregistrement
builder.Services.AddHealthChecks()

' Exposition d'un point de terminaison
app.MapHealthChecks("/health")
```

On y ajoute ensuite des contrôles personnalisés (base de données joignable, service tiers
disponible…), chacun renvoyant un état `Healthy`, `Degraded` ou `Unhealthy`.

> ⚠️ Les *health checks* sont un concept **web / service** : ils n'ont pas de sens pour une
> application de bureau. C'est un signal de plus que l'observabilité s'épanouit côté serveur.

---

## Câbler OpenTelemetry (notions)

Dans une application à **hôte générique** (Web API), brancher OpenTelemetry consiste à déclarer
les **sources** à écouter et les **exporteurs** vers lesquels router. En VB.NET, on utilise la
continuation de ligne `_` pour la chaîne fluide :

```vb
builder.Services.AddOpenTelemetry() _
    .WithMetrics(Sub(m) m.AddMeter("MonApp.Commandes").AddOtlpExporter()) _
    .WithTracing(Sub(t) t.AddSource("MonApp.Commandes").AddOtlpExporter())
```

L'exporteur **OTLP** (*OpenTelemetry Protocol*) envoie les données vers n'importe quel backend
compatible. À partir de là, les métriques et traces définies plus haut alimentent
automatiquement votre outil d'observabilité, sans modifier le code métier.

> 🔗 **L'écosystème de réception penche C#/cloud.** Les backends courants (Prometheus + Grafana
> pour les métriques, Jaeger pour les traces, Application Insights côté Azure) et les outils
> cloud-native d'orchestration sont majoritairement pilotés en C#. Votre rôle, côté VB.NET, se
> limite à **émettre** la télémétrie (ce qui est pleinement supporté) et à **consommer** ces
> services là où ils sont justifiés — voir l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md).

---

## À retenir

L'observabilité étend la journalisation à deux autres signaux — les **métriques** (combien, à
quelle vitesse, quelle tendance) et les **traces** (par où, et où ça ralentit) — pour comprendre
un système réparti. En .NET, on l'instrumente avec des primitives **natives** (`Meter`,
`ActivitySource`) qu'OpenTelemetry collecte et exporte vers le backend de son choix ; tout cela
est **émis sans difficulté depuis VB.NET**. La vraie question n'est pas « *VB ou C# ?* » mais
« *en ai-je besoin ?* » : pour une application de bureau, la journalisation suffit généralement ;
les traces et les *health checks* prennent tout leur sens dès qu'on passe à une **Web API** ou à
des **services**.

---

## ✅ Conclusion du chapitre 12

Ce chapitre a déroulé le **trépied de la fiabilité** :

- **[12.1 — Exceptions](01-exceptions.md)** : réagir proprement aux erreurs (`Try`/`Catch`/`Finally`,
  filtres `When`, exceptions personnalisées).
- **[12.2 — Débogage](02-debogage.md)** : comprendre les bugs avec l'outillage de Visual Studio
  2026, plutôt que deviner.
- **[12.3 — Journalisation](03-journalisation.md)** : garder une trace structurée et exploitable
  de ce qui se passe en production.
- **12.4 — Observabilité** : étendre cette trace aux métriques et aux traces pour les systèmes
  distribués.

Le fil rouge : sur tous ces sujets, **VB.NET n'est pas pénalisé par le gel du langage**, car ils
relèvent du runtime, de la BCL, de bibliothèques NuGet et de l'IDE — jamais d'une nouvelle
syntaxe. Les seules réserves rencontrées sont ponctuelles et bien identifiées : le générateur de
source `[LoggerMessage]` (réservé à C#, contourné par `LoggerMessage.Define`), et les **agents IA
de débogage** de VS 2026 (pensés d'abord pour C#).

➡️ Chapitre suivant : **[13. Tests et qualité du code](../13-tests-qualite/README.md)** — car un
code fiable se prouve aussi par les tests, prolongement naturel de la Partie 6 « Qualité,
performance et exploitation ».

⏭️ [Tests et qualité du code](/13-tests-qualite/README.md)
