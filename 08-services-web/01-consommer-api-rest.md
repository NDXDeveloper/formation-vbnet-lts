🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 8.1 Consommer des API REST

> **Module 8 — Consommer et exposer des services** · `HttpClient`, `IHttpClientFactory`, `System.Text.Json`, résilience avec Polly

Consommer une API REST est le **scénario web le plus confortable en VB.NET — sans le moindre astérisque**. Un client HTTP, un sérialiseur JSON et une bibliothèque de résilience sont de simples briques .NET ; on les utilise en VB.NET exactement comme en C#, avec les mêmes types et les mêmes méthodes. C'est l'illustration parfaite du périmètre « consommation » au cœur de la stratégie 2026 : tout ce qui se *consomme* est pleinement à votre portée.

Cette section couvre la chaîne complète d'un appel REST robuste : créer correctement le client (`IHttpClientFactory`), envoyer la requête (`HttpClient`), lire et mapper la réponse (`System.Text.Json`), et survivre aux pannes réseau (Polly).

---

## 🌐 `HttpClient` : le client HTTP de .NET

`HttpClient` (espace de noms `System.Net.Http`) est la classe centrale pour émettre des requêtes HTTP. Toutes ses opérations réseau sont **asynchrones** — d'où l'importance des prérequis du module 4.

### Un premier appel `GET`

```vb
Imports System.Net.Http

Public Class CatalogueService
    Private ReadOnly _client As HttpClient

    Public Sub New(client As HttpClient)
        _client = client
    End Sub

    Public Async Function ObtenirJsonBrutAsync() As Task(Of String)
        Dim reponse = Await _client.GetAsync("https://api.exemple.com/produits")
        reponse.EnsureSuccessStatusCode()                 ' lève si code <> 2xx
        Return Await reponse.Content.ReadAsStringAsync()
    End Function
End Class
```

Quelques points clés :

- `GetAsync` renvoie un `HttpResponseMessage` : en-têtes, code d'état et corps (`Content`).
- `EnsureSuccessStatusCode()` lève une `HttpRequestException` si le code n'est pas un succès (`2xx`). Pratique, mais à manier selon les cas (voir la gestion d'erreurs plus bas).
- Le corps se lit via `Content`, ici sous forme de chaîne. En pratique, on le désérialise directement (section suivante).

### ⚠️ Le piège du cycle de vie de `HttpClient`

`HttpClient` est conçu pour être **réutilisé**, pas instancié à chaque appel. Le réflexe naïf suivant est un anti-pattern :

```vb
' ❌ À NE PAS FAIRE : un nouveau client par appel
Public Async Function MauvaiseMethodeAsync() As Task(Of String)
    Using client As New HttpClient()                      ' épuisement de sockets garanti
        Return Await client.GetStringAsync("https://api.exemple.com/produits")
    End Using
End Function
```

Deux problèmes bien connus :

- **Épuisement des sockets** : chaque `HttpClient` jetable laisse des connexions en état `TIME_WAIT`. Sous charge, le système finit par manquer de ports.
- **DNS périmé** : un `HttpClient` gardé indéfiniment en variable `Shared`, à l'inverse, ne réagit pas aux changements de DNS.

La réponse moderne à ce dilemme n'est ni « un par appel » ni « un singleton éternel », mais **`IHttpClientFactory`**, qui gère un pool de handlers avec rotation. C'est l'objet de la section dédiée plus bas.

---

## 📦 `System.Text.Json` : sérialiser et désérialiser

`System.Text.Json` (espace de noms `System.Text.Json`) est le sérialiseur JSON intégré à .NET, performant et sans dépendance externe.

### Désérialisation de base

Définissez une classe correspondant à la forme du JSON :

```vb
Imports System.Text.Json
Imports System.Text.Json.Serialization

Public Class Produit
    Public Property Id As Integer
    Public Property Nom As String
    Public Property Prix As Decimal

    <JsonPropertyName("en_stock")>
    Public Property EnStock As Boolean
End Class
```

Puis désérialisez :

```vb
Dim json = Await reponse.Content.ReadAsStringAsync()
Dim produit = JsonSerializer.Deserialize(Of Produit)(json)
```

### Personnaliser le mapping

Le JSON réel ne respecte pas toujours la casse Pascal de VB.NET. Deux leviers :

- l'attribut `<JsonPropertyName("…")>` pour un nom précis (voir `EnStock` ci-dessus) ;
- un objet `JsonSerializerOptions` pour une règle globale.

```vb
' ⚠️ Réutilisez la MÊME instance d'options (elle met en cache les métadonnées de type).
Private Shared ReadOnly _options As New JsonSerializerOptions With {
    .PropertyNameCaseInsensitive = True,
    .PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
}

Dim produit = JsonSerializer.Deserialize(Of Produit)(json, _options)
```

> 💡 **Piège de performance** : recréer un `JsonSerializerOptions` à chaque appel annule le cache de métadonnées interne et dégrade nettement les performances. Déclarez-le une fois (`Shared ReadOnly`) et réutilisez-le.

### ⚠️ Le mode « source generator » est réservé à C#

`System.Text.Json` propose un mode haute performance fondé sur un **générateur de source** (`JsonSerializerContext` annoté de `[JsonSerializable]`), utile pour l'AOT et le *trimming*. Ce générateur **émet du code C#** et **cible les projets C#** — il ne fonctionne pas dans un projet VB.NET. 🔗

Conséquence concrète et sans gravité : en VB.NET, on utilise le **sérialiseur par réflexion** (le mode par défaut), qui est pleinement pris en charge et largement suffisant pour consommer des API REST. Si un besoin spécifique de JSON *source-generated* (AOT, démarrage à froid critique) apparaissait, ce serait un candidat type à **isoler dans une bibliothèque C#** — voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md). Pour la consommation REST courante, cette nuance n'a aucun impact.

### Les extensions `System.Net.Http.Json` (raccourcis)

Plutôt que de lire la chaîne puis de désérialiser en deux temps, l'espace de noms `System.Net.Http.Json` ajoute des méthodes d'extension qui font les deux d'un coup :

```vb
Imports System.Net.Http.Json

' GET + désérialisation
Dim produits = Await _client.GetFromJsonAsync(Of List(Of Produit))("/api/produits")

' POST d'un objet sérialisé en JSON, puis lecture de la réponse
Dim nouveau As New Produit With {.Nom = "Clavier", .Prix = 49.9D}
Dim reponse = Await _client.PostAsJsonAsync("/api/produits", nouveau)
reponse.EnsureSuccessStatusCode()
Dim cree = Await reponse.Content.ReadFromJsonAsync(Of Produit)()
```

Ces extensions sont à privilégier : moins de code, et elles diffusent le flux directement dans le désérialiseur (sans matérialiser toute la chaîne en mémoire).

---

## 🏭 `IHttpClientFactory` : la bonne façon de créer des clients

`IHttpClientFactory` (paquet `Microsoft.Extensions.Http`) résout proprement le problème de cycle de vie évoqué plus haut : elle gère un **pool de handlers** réutilisés et renouvelés périodiquement, ce qui évite à la fois l'épuisement des sockets et le DNS périmé.

### Mise en place de l'injection de dépendances

`IHttpClientFactory` s'inscrit dans le conteneur d'injection de dépendances. Dans une application de bureau ou console, on configure ce conteneur via l'**hôte générique** (`Microsoft.Extensions.Hosting`, cf. module 4.8) ; dans une Web API, il est déjà présent (cf. module 8.2).

```vb
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Module Program
    ' Rappel VB : pas d'Async Main (BC30737, module 4.2) — on délègue à MainAsync
    Sub Main(args As String())
        MainAsync(args).GetAwaiter().GetResult()
    End Sub

    Async Function MainAsync(args As String()) As Task
        Dim builder = Host.CreateApplicationBuilder(args)

        ' Enregistrement d'un client (voir les variantes ci-dessous)
        builder.Services.AddHttpClient(Of CatalogueClient)(Sub(client)
                client.BaseAddress = New Uri("https://api.exemple.com/")
                client.Timeout = TimeSpan.FromSeconds(30)
            End Sub)

        Dim app = builder.Build()

        Dim catalogue = app.Services.GetRequiredService(Of CatalogueClient)()
        Dim produits = Await catalogue.ObtenirProduitsAsync()
        Console.WriteLine($"{produits.Count} produit(s) reçu(s).")
    End Function
End Module
```

### Clients typés (recommandé) ⭐

Le **client typé** est l'approche la plus idiomatique : une classe dédiée reçoit un `HttpClient` par injection et encapsule les appels d'un service. Le code métier ne manipule jamais d'URL nues.

```vb
Imports System.Net.Http.Json

Public Class CatalogueClient
    Private ReadOnly _http As HttpClient

    Public Sub New(http As HttpClient)         ' HttpClient fourni par la fabrique
        _http = http
    End Sub

    Public Async Function ObtenirProduitsAsync() As Task(Of IReadOnlyList(Of Produit))
        Dim produits = Await _http.GetFromJsonAsync(Of List(Of Produit))("api/produits")
        Return If(produits, New List(Of Produit)())
    End Function

    Public Async Function ObtenirParIdAsync(id As Integer) As Task(Of Produit)
        Return Await _http.GetFromJsonAsync(Of Produit)($"api/produits/{id}")
    End Function
End Class
```

L'enregistrement `AddHttpClient(Of CatalogueClient)(…)` (vu plus haut) suffit : la fabrique injecte un `HttpClient` correctement configuré et géré.

### Clients nommés

Quand une classe doit dialoguer avec plusieurs API, ou pour des appels ponctuels, on déclare des clients **nommés** et on les obtient via la fabrique :

```vb
builder.Services.AddHttpClient("github", Sub(client)
        client.BaseAddress = New Uri("https://api.github.com/")
        client.DefaultRequestHeaders.Add("User-Agent", "MonApplication")
    End Sub)
```

```vb
Public Class DepotService
    Private ReadOnly _fabrique As IHttpClientFactory

    Public Sub New(fabrique As IHttpClientFactory)
        _fabrique = fabrique
    End Sub

    Public Async Function ListerDepotsAsync() As Task(Of String)
        Dim client = _fabrique.CreateClient("github")   ' client préconfiguré
        Return Await client.GetStringAsync("users/dotnet/repos")
    End Function
End Class
```

> ⚠️ Un client obtenu via `CreateClient` ou injecté dans un client typé **ne doit pas être placé dans un `Using`** : sa durée de vie est gérée par la fabrique. Ne le disposez pas vous-même.

---

## 🛡️ Résilience avec Polly

Le réseau échoue : coupures passagères, services momentanément surchargés, latences. Une consommation d'API sérieuse doit **encaisser ces défaillances transitoires** plutôt que de propager la première erreur venue. C'est le rôle de **Polly**, intégré à .NET via le paquet `Microsoft.Extensions.Http.Resilience`.

### La voie simple : `AddStandardResilienceHandler` ⭐

La façon la plus directe — et la plus idiomatique en VB.NET — consiste à ajouter le **pipeline de résilience standard** au client. En une ligne, on obtient une combinaison éprouvée : limitation de débit, timeout total, réessais avec backoff, disjoncteur (*circuit breaker*) et timeout par tentative.

```vb
Imports Microsoft.Extensions.Http.Resilience

builder.Services.AddHttpClient(Of CatalogueClient)(Sub(client)
        client.BaseAddress = New Uri("https://api.exemple.com/")
    End Sub).AddStandardResilienceHandler()
```

On peut ajuster les valeurs par défaut via les options :

```vb
.AddStandardResilienceHandler(Sub(options)
        options.Retry.MaxRetryAttempts = 5
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10)
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60)
    End Sub)
```

> ⚠️ **Interaction avec `HttpClient.Timeout`** : ce dernier s'applique à l'appel *complet*, réessais compris, et peut donc couper une séquence de retries. Lorsque vous utilisez le handler de résilience, laissez la gestion du temps au pipeline et neutralisez le timeout global : `client.Timeout = Timeout.InfiniteTimeSpan`.

### Personnaliser le pipeline

Pour un contrôle fin, `AddResilienceHandler` permet de composer les stratégies à la main. L'avantage en VB.NET : ces stratégies se configurent via des **objets d'options à propriétés**, sans lambdas génériques verbeuses.

```vb
Imports Microsoft.Extensions.Http.Resilience
Imports Polly

builder.Services.AddHttpClient(Of CatalogueClient)(Sub(client)
        client.BaseAddress = New Uri("https://api.exemple.com/")
    End Sub).AddResilienceHandler("catalogue", Sub(pipeline, contexte)
        pipeline.AddRetry(New HttpRetryStrategyOptions With {
            .MaxRetryAttempts = 3,
            .Delay = TimeSpan.FromSeconds(1),
            .BackoffType = DelayBackoffType.Exponential,
            .UseJitter = True
        })
        pipeline.AddCircuitBreaker(New HttpCircuitBreakerStrategyOptions With {
            .FailureRatio = 0.5,
            .SamplingDuration = TimeSpan.FromSeconds(10),
            .MinimumThroughput = 8,
            .BreakDuration = TimeSpan.FromSeconds(30)
        })
        pipeline.AddTimeout(TimeSpan.FromSeconds(10))
    End Sub)
```

Les options `Http*StrategyOptions` préconfigurent la détection des erreurs HTTP transitoires (codes `5xx`, `408`, `HttpRequestException`), ce qui évite d'écrire soi-même la logique de « quoi réessayer ».

### Hors injection de dépendances : Polly autonome

Sans hôte ni fabrique, on construit directement un `ResiliencePipeline` puis on l'exécute. Cette **API fluide très générique** de Polly v8 (lambdas renvoyant des `ValueTask(Of T)`) est plus lourde à écrire en VB qu'en C# : dès que l'application utilise `IHttpClientFactory`, **l'intégration via `AddStandardResilienceHandler` reste la voie la plus simple et la plus naturelle en VB.NET**. Si une orchestration de résilience complexe et autonome devenait nécessaire, c'est un candidat raisonnable à isoler en C# (cf. [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

### Les stratégies clés

| Stratégie | Rôle | Cas d'usage typique |
|-----------|------|---------------------|
| **Retry** (avec backoff + *jitter*) | Réessayer les échecs transitoires | Coupures réseau passagères, `5xx`, `408` |
| **Circuit Breaker** | Cesser d'appeler un service en panne | Échecs répétés → ne pas aggraver la situation |
| **Timeout** | Borner la durée d'un appel | Service lent ou bloqué |
| **Rate Limiter** | Limiter le débit sortant | Respecter les quotas d'une API |
| **Hedging** | Lancer des appels en parallèle | Réduire la latence de queue (*tail latency*) |

> 💡 Le *jitter* (aléa ajouté au délai de réessai) évite que de nombreux clients réessaient tous au même instant et ne provoquent une « tempête de retries ». Activez-le (`UseJitter = True`).

---

## 🔐 Authentification et en-têtes

La plupart des API exigent un en-tête d'authentification. Pour un cas ponctuel, on le pose directement :

```vb
Imports System.Net.Http.Headers

client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", jeton)
```

Pour injecter automatiquement le jeton sur **chaque** requête d'un client, on écrit un `DelegatingHandler` — un maillon du pipeline de traitement HTTP :

```vb
Imports System.Net.Http.Headers

Public Class AuthHandler
    Inherits DelegatingHandler

    Private ReadOnly _fournisseur As IFournisseurJeton

    Public Sub New(fournisseur As IFournisseurJeton)
        _fournisseur = fournisseur
    End Sub

    Protected Overrides Async Function SendAsync(
            requete As HttpRequestMessage,
            cancellationToken As CancellationToken) As Task(Of HttpResponseMessage)

        Dim jeton = Await _fournisseur.ObtenirAsync(cancellationToken)
        requete.Headers.Authorization = New AuthenticationHeaderValue("Bearer", jeton)
        Return Await MyBase.SendAsync(requete, cancellationToken)
    End Function
End Class
```

On le branche sur le client lors de l'enregistrement :

```vb
builder.Services.AddTransient(Of AuthHandler)()
builder.Services.AddHttpClient(Of CatalogueClient)(Sub(client)
        client.BaseAddress = New Uri("https://api.exemple.com/")
    End Sub).AddHttpMessageHandler(Of AuthHandler)()
```

Le détail des protocoles (OAuth 2.0 / OpenID Connect, JWT, Microsoft Entra ID) est traité au **module 16.1**.

---

## 🧯 Gérer les erreurs proprement

`EnsureSuccessStatusCode()` convient pour un échec « tout ou rien ». Mais distinguer un `404` (ressource absente) d'une vraie panne demande souvent un traitement explicite :

```vb
Imports System.Net

Public Async Function ObtenirParIdAsync(id As Integer, token As CancellationToken) As Task(Of Produit)
    Dim reponse = Await _http.GetAsync($"api/produits/{id}", token)

    If reponse.IsSuccessStatusCode Then
        Return Await reponse.Content.ReadFromJsonAsync(Of Produit)(cancellationToken:=token)
    ElseIf reponse.StatusCode = HttpStatusCode.NotFound Then
        Return Nothing                                   ' absence légitime, pas une erreur
    Else
        Dim corps = Await reponse.Content.ReadAsStringAsync(token)
        Throw New ApplicationException($"Échec ({CInt(reponse.StatusCode)}) : {corps}")
    End If
End Function
```

Les deux exceptions à connaître :

- **`HttpRequestException`** : échec réseau, DNS, TLS, ou code d'erreur via `EnsureSuccessStatusCode`.
- **`TaskCanceledException`** (souvent avec une `TimeoutException` interne) : timeout du client ou annulation par le `CancellationToken`.

> 💡 Propagez toujours un `CancellationToken` jusqu'à `HttpClient` : c'est ce qui rend une application de bureau réactive (annulation par l'utilisateur) et un service correctement borné dans le temps.

---

## 🆕 Ce que .NET 10 apporte

Côté consommation, .NET 10 améliore l'existant sans changer votre code :

- **`System.Text.Json`** : optimisations de performance et raffinements de l'API de sérialisation (mode réflexion compris, donc directement profitable en VB.NET).
- **Résilience** : valeurs par défaut affinées du pipeline standard et meilleure intégration avec `IHttpClientFactory`.
- Les nouveautés plus visibles (**OpenAPI 3.1**, `Problem Details`, *rate limiting* intégré) concernent surtout le **côté serveur** et sont traitées au module 8.2.

---

## ✅ Bonnes pratiques et pièges

| À faire | À éviter |
|---------|----------|
| Utiliser `IHttpClientFactory` (clients typés ou nommés) | `New HttpClient()` à chaque appel → épuisement de sockets |
| Réutiliser une instance `JsonSerializerOptions` (`Shared ReadOnly`) | Recréer les options à chaque appel → perte du cache de métadonnées |
| Préférer les extensions `GetFromJsonAsync` / `PostAsJsonAsync` | Lire la chaîne puis désérialiser en deux temps inutilement |
| Propager un `CancellationToken` de bout en bout | Des appels non annulables qui figent l'UI |
| Gérer explicitement les codes d'état pertinents | Supposer que toute réponse reçue est exploitable |
| Déléguer la résilience à `AddStandardResilienceHandler` | Réimplémenter retries et disjoncteur à la main |
| Mettre `Timeout.InfiniteTimeSpan` avec le handler de résilience | Laisser `HttpClient.Timeout` couper les réessais |
| Rester sur la sérialisation par réflexion (mode VB) | Tenter le générateur de source JSON (C# uniquement) |

---

## 🧭 À retenir

- **La consommation d'API REST est 100 % du ressort de VB.NET** : `HttpClient`, `System.Text.Json` et Polly sont de simples bibliothèques .NET, sans frontière de langage.
- **`IHttpClientFactory` + clients typés** est le modèle de référence : il règle le cycle de vie du client et structure proprement le code d'accès.
- En VB.NET, on utilise **`System.Text.Json` en mode réflexion** ; le mode *source generator*, propre à C#, n'est pas nécessaire ici.
- La résilience se branche en une ligne avec **`AddStandardResilienceHandler`** ; le pipeline se personnalise via des options à propriétés, confortables en VB.
- Les rares frictions (générateur JSON, API fluide autonome de Polly) sont mineures et, le cas échéant, se traitent via la **stratégie hybride** — jamais un obstacle pour consommer une API.

---

⬅️ [8. Consommer et exposer des services](README.md) · ➡️ [8.2 — Exposer une Web API ASP.NET Core en VB.NET](02-web-api-controllers.md)

⏭️ [Exposer une Web API ASP.NET Core en VB.NET (par contrôleurs)](/08-services-web/02-web-api-controllers.md)
