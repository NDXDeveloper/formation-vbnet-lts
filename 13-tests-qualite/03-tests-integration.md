🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 13.3 — Tests d'intégration (`WebApplicationFactory`, Testcontainers, tests avec base de données)

> **Module 13 — Tests et qualité du code**
> Vérifier que les composants fonctionnent **ensemble**, contre une infrastructure réelle.

---

## Tests unitaires vs tests d'intégration

Les sections précédentes isolaient une unité de code en remplaçant ses dépendances par des
doublures ([13.2](02-mocking-tdd.md)). Un **test d'intégration** fait l'inverse : il vérifie que
plusieurs composants — votre code **et** une infrastructure réelle (base de données, pipeline
HTTP, système de fichiers…) — fonctionnent **correctement ensemble**.

Les deux sont complémentaires, et l'on ne doit pas tout faire au même niveau. La **pyramide des
tests** résume l'équilibre visé :

- **Beaucoup** de tests **unitaires** à la base : rapides, isolés, exécutés en continu.
- **Un nombre ciblé** de tests d'**intégration** au milieu : plus lents, sur les **coutures**
  importantes (accès aux données, API…).
- **Très peu** de tests de **bout en bout** (UI, scénarios complets) au sommet : lents et
  fragiles, réservés aux parcours critiques.

> 💡 Le piège classique est la pyramide « inversée » : trop de tests lents de haut niveau, trop
> peu de tests unitaires. On vise l'inverse — une **large base** de tests unitaires, complétée par
> des tests d'intégration là où le risque réel se situe.

---

## Ce que vérifie un test d'intégration

Concrètement, un test d'intégration s'attaque aux **points de jonction** que les tests unitaires,
par construction, n'exercent pas :

- la traduction réelle d'une requête **EF Core** en SQL exécuté sur une **vraie base** ;
- le **pipeline HTTP complet** d'une Web API (routage, liaison de modèle, middleware,
  sérialisation, codes de statut) ;
- l'interaction avec un service externe, un cache, une file de messages…

Voyons les deux outils phares pour cela en .NET.

---

## Tester une Web API : `WebApplicationFactory`

ASP.NET Core fournit un **hôte de test en mémoire** via `WebApplicationFactory(Of TEntryPoint)`
(paquet `Microsoft.AspNetCore.Mvc.Testing`). Il **démarre toute l'application** en cours de
processus — injection de dépendances, middleware, routage, contrôleurs — et expose un `HttpClient`
qui adresse de **vraies requêtes HTTP** à ce serveur. On teste ainsi la Web API « pour de vrai »,
sans la déployer ni ouvrir de port réseau.

Comme VB.NET réalise les Web API **par contrôleurs** (cf. [8.2](../08-services-web/02-web-api-controllers.md)),
ce mécanisme s'applique directement :

```vb
Imports Microsoft.AspNetCore.Mvc.Testing
Imports System.Net
Imports Xunit

Public Class CommandesApiTests
    Implements IClassFixture(Of WebApplicationFactory(Of Program))

    Private ReadOnly _factory As WebApplicationFactory(Of Program)

    Public Sub New(factory As WebApplicationFactory(Of Program))
        _factory = factory
    End Sub

    <Fact>
    Public Async Function GetCommandes_RetourneSucces() As Task
        Dim client = _factory.CreateClient()
        Dim reponse = Await client.GetAsync("/api/commandes")
        reponse.EnsureSuccessStatusCode()
        Assert.Equal(HttpStatusCode.OK, reponse.StatusCode)
    End Function
End Class
```

`IClassFixture` partage la même fabrique entre les tests de la classe ; `CreateClient()` fournit
un client connecté au serveur en mémoire.

### 🟢 Un point où VB.NET évite un piège de C#

`WebApplicationFactory(Of TEntryPoint)` a besoin d'un **type de point d'entrée** accessible depuis
le projet de test. En C#, les *top-level statements* génèrent une classe `Program` **interne**,
d'où l'astuce bien connue d'ajouter `public partial class Program { }` (ou `InternalsVisibleTo`).

> 🟢 **En VB.NET, ce piège n'existe pas.** Faute de *top-level statements*, vous écrivez **toujours
> un point d'entrée explicite** — il suffit de le déclarer **`Public`** pour que le projet de test
> puisse le référencer. La « limite » de VB (pas de *top-level statements*, cf.
> [8.3](../08-services-web/03-limites-web-vbnet.md)) devient ici un petit avantage.

```vb
' Point d'entrée explicite, rendu accessible au projet de test
Public Class Program
    Public Shared Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)
        ' … configuration des services, contrôleurs …
        Dim app = builder.Build()
        ' … middleware, routes …
        app.Run()
    End Sub
End Class
```

### Personnaliser la fabrique

Pour un test d'intégration réaliste, on veut souvent **remplacer** une dépendance — typiquement,
faire pointer le `DbContext` vers une base de **test** plutôt que la base de production. On dérive
alors la fabrique et on redéfinit `ConfigureWebHost` :

```vb
Public Class FabriqueDeTest
    Inherits WebApplicationFactory(Of Program)

    Public Property ChaineConnexion As String

    Protected Overrides Sub ConfigureWebHost(builder As IWebHostBuilder)
        builder.ConfigureServices(
            Sub(services)
                ' Retirer l'enregistrement réel du DbContext,
                ' puis le réenregistrer vers la base de test (ChaineConnexion).
            End Sub)
    End Sub
End Class
```

---

## Tester contre une vraie base de données

### ⚠️ Le piège du fournisseur « InMemory »

La tentation est grande de remplacer la base par le fournisseur **EF Core InMemory** pour aller
vite. C'est trompeur :

> ⚠️ **Le fournisseur InMemory d'EF Core n'est pas une base de données.** Il ne génère pas de SQL,
> n'applique pas les contraintes (clés, unicité, relations) et traduit les requêtes différemment
> d'un vrai moteur. Un test « vert » sur InMemory peut donc masquer un bug qui n'apparaîtra qu'en
> production. Microsoft le déconseille pour tester le comportement réel des requêtes.

Deux options sérieuses :

- **SQLite en mode mémoire** — un *vrai* moteur SQL, léger, suffisant pour beaucoup de cas
  (compromis intermédiaire).
- **Testcontainers** — la base **cible réelle** (SQL Server, PostgreSQL…), dans un conteneur
  jetable. C'est l'approche la plus fidèle, détaillée ci-dessous.

### Testcontainers : une vraie base, jetable

**Testcontainers** lance une **vraie base de données dans un conteneur Docker**, le temps des
tests, puis la détruit. On obtient ainsi une base réelle, **isolée** et **reproductible**, sans
installation manuelle ni état persistant entre les exécutions. C'est une **bibliothèque** .NET
(modules `Testcontainers.MsSql`, `Testcontainers.PostgreSql`, etc.) — donc pleinement consommable
depuis VB.NET.

```vb
Imports Testcontainers.MsSql

' Construire et démarrer un conteneur SQL Server
Dim conteneur = New MsSqlBuilder().Build()
Await conteneur.StartAsync()

' Récupérer la chaîne de connexion du conteneur, pour configurer EF Core / ADO.NET
Dim chaineConnexion = conteneur.GetConnectionString()
' … exécuter migrations + tests contre cette base bien réelle …

' Libérer le conteneur à la fin
Await conteneur.DisposeAsync()
```

En pratique, le conteneur est **démarré une fois pour la classe de tests** (via le mécanisme de
cycle de vie asynchrone du framework — `IAsyncLifetime` en xUnit, par exemple) et **libéré** à la
fin.

> ⚠️ **Prérequis : Docker** doit être disponible sur la machine de développement **et** sur l'agent
> de CI. C'est le prix de la fidélité — largement justifié pour les tests d'accès aux données.

---

## Combiner les deux : Web API réelle + base réelle

Le test d'intégration le plus complet associe les deux outils : la **fabrique** démarre
l'application, et on lui injecte la **chaîne de connexion du conteneur** Testcontainers. On
vérifie alors un parcours de bout en bout — requête HTTP → contrôleur → EF Core → vraie base —
dans des conditions très proches de la production.

L'enchaînement, conceptuellement : démarrer le conteneur de base → appliquer les migrations →
construire la `FabriqueDeTest` avec la chaîne de connexion du conteneur → exécuter les requêtes
HTTP via le client → tout détruire à la fin.

---

## Isolation et données de test

Des tests d'intégration **fiables** exigent un état **maîtrisé** : chaque test doit partir d'une
base propre et prévisible, sinon l'ordre d'exécution influence les résultats.

- **Réinitialiser entre les tests.** Une bibliothèque comme **Respawn** remet efficacement la
  base à un état vierge entre les tests (en vidant les données), ce qui permet de partager un
  même conteneur sans fuite d'état d'un test à l'autre.
- **Semer des données connues** (*seeding*) au début de chaque test, pour des assertions
  déterministes.
- **Éviter les dépendances entre tests** : chacun doit pouvoir s'exécuter **seul**.

---

## Séparer tests unitaires et tests d'intégration

Les tests d'intégration sont **plus lents** et requièrent une infrastructure (Docker). Il est sain
de les **distinguer** des tests unitaires, pour exécuter ces derniers en continu et réserver les
premiers à des moments choisis (avant un commit, en CI).

On les **catégorise** à l'aide d'attributs, selon le framework :

| Framework | Attribut de catégorie |
|-----------|-----------------------|
| xUnit | `<Trait("Category", "Integration")>` |
| NUnit | `<Category("Integration")>` |
| MSTest | `<TestCategory("Integration")>` |

On filtre ensuite à l'exécution (par exemple `dotnet test --filter`), pour ne lancer que les tests
voulus. Côté CI, on dédie souvent une étape distincte aux tests d'intégration (avec un *runner*
disposant de Docker).

---

## VB.NET : tout est consommable

Aucun frein côté VB.NET sur ce terrain : `WebApplicationFactory` (sur une Web API VB par
contrôleurs), **Testcontainers** et les bibliothèques d'isolation sont des **bibliothèques** et de
l'**outillage** — exactement le périmètre *consumption-only* du langage. Mieux, on l'a vu, l'absence
de *top-level statements* en VB **évite** le piège du `Program` interne propre à C#.

---

## À retenir

Les tests d'intégration vérifient ce que les tests unitaires ne peuvent pas : que votre code
fonctionne **avec** son infrastructure réelle. Pour une Web API VB.NET, **`WebApplicationFactory`**
exécute tout le pipeline en mémoire et adresse de vraies requêtes HTTP ; pour l'accès aux données,
on évite le trompeur fournisseur **InMemory** au profit de **Testcontainers**, qui fournit une
**vraie base jetable** (au prix d'une dépendance à Docker). On veille à l'**isolation** (état
propre entre tests) et l'on **sépare** ces tests, plus lents, des tests unitaires. Le tout est
pleinement accessible en VB.NET — qui évite même au passage un piège de configuration bien connu
de C#.

➡️ Section suivante : **[13.4 — Analyse statique](04-analyse-statique.md)**, pour détecter des
problèmes **sans même exécuter** le code.

⏭️ [Analyse statique (analyseurs Roslyn, SonarQube, StyleCop)](/13-tests-qualite/04-analyse-statique.md)
