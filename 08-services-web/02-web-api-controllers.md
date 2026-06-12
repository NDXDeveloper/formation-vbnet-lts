🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 8.2 Exposer une Web API ASP.NET Core en VB.NET (par contrôleurs) ✅

> **Module 8 — Consommer et exposer des services** · Contrôleurs, routes, liaison de modèle, DI, middleware, OpenAPI 3.1, JWT, `Problem Details`, versioning, *rate limiting*

Exposer une Web API est le **scénario web réellement recommandé en VB.NET** ✅ — à une condition : passer par des **contrôleurs**, pas par les Minimal APIs. Un contrôleur est une classe ; ce modèle objet est parfaitement idiomatique en VB.NET. Les Minimal APIs, à l'inverse, reposent sur les *top-level statements* et un style très orienté lambdas que VB ne sait pas exprimer confortablement (voir § 8.3 et [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

Le reste — routes, injection de dépendances, middleware, sécurité, documentation — fonctionne en VB.NET exactement comme le décrit la documentation ASP.NET Core, avec la syntaxe VB. Cette section couvre la chaîne complète, en signalant honnêtement les quelques frictions propres à VB.

---

## 🧱 Pourquoi les contrôleurs en VB.NET

| Approche | Convient à VB.NET ? | Raison |
|----------|---------------------|--------|
| **Contrôleurs** (`ControllerBase`) | ✅ Oui, recommandé | Modèle à base de classes, naturel en VB |
| Minimal APIs | ⚠️ Possible mais peu idiomatique | Suppose *top-level statements* (absents en VB) et un style lambdas contraint |
| Razor Pages / MVC avec vues / Blazor | ❌ Non | *Razor génère du C#* |

Le choix est donc tranché : en VB.NET, une Web API se construit **avec des contrôleurs**.

---

## 🛠️ Créer le projet (sans modèle) ⚠️

Première friction : **il n'existe pas de modèle de projet ASP.NET Core en VB.NET**. La méthode fiable consiste à partir d'un projet console VB, puis à le transformer en projet web.

```bash
dotnet new console -lang VB -n MonApi
```

Ouvrez ensuite `MonApi.vbproj` et basculez-le sur le **SDK Web** :

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>MonApi</RootNamespace>
    <!-- Le fichier XML est généré, mais voir la note OpenAPI plus bas (§ documentation) -->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
    <PackageReference Include="Scalar.AspNetCore" Version="2.*" />
  </ItemGroup>

</Project>
```

Le SDK `Microsoft.NET.Sdk.Web` référence implicitement le framework partagé ASP.NET Core : aucune dépendance supplémentaire n'est nécessaire pour les contrôleurs.

Seconde friction : **VB ne dispose pas des *top-level statements***. Le point d'entrée est donc un `Module` avec un `Sub Main` explicite — là où le `Program.cs` de C# tient en quelques lignes sans cérémonie. (Et rappelez-vous : VB n'accepte **ni** `Async Main`, **ni même** un `Function Main(...) As Task` — BC30737, vérifié ; si du code asynchrone est nécessaire au démarrage, on délègue à un `MainAsync` comme en [8.1](01-consommer-api-rest.md). Pour une Web API, `app.Run()` étant bloquant, le `Sub Main` suffit.)

```vb
' Program.vb
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()        ' active le pipeline des contrôleurs

        Dim app = builder.Build()

        app.UseHttpsRedirection()
        app.MapControllers()                     ' associe les routes aux actions

        app.Run()
    End Sub
End Module
```

> 💡 Le *scaffolding* de Visual Studio (clic droit → « Ajouter un contrôleur ») est orienté C# et n'est pas disponible pour VB. Ce n'est pas un obstacle : un contrôleur n'est qu'une classe héritant de `ControllerBase`, qu'on écrit à la main.

Les sections suivantes **enrichissent** ce squelette : chaque préoccupation (OpenAPI, sécurité, versioning…) s'ajoute à `builder.Services` et/ou au pipeline.

---

## 🎯 Contrôleurs, routes et actions

Un contrôleur d'API hérite de `ControllerBase` (et non de `Controller`, réservé aux vues). On l'annote de `<ApiController>` — qui active des conventions pratiques (liaison automatique, réponses `400` automatiques en cas de modèle invalide) — et de `<Route>` pour le préfixe d'URL.

```vb
Imports Microsoft.AspNetCore.Mvc

<ApiController>
<Route("api/[controller]")>
Public Class ProduitsController
    Inherits ControllerBase

    Private ReadOnly _service As IProduitService

    Public Sub New(service As IProduitService)        ' injecté par le conteneur
        _service = service
    End Sub

    <HttpGet>
    Public Async Function ListerAsync() As Task(Of ActionResult(Of IEnumerable(Of Produit)))
        Return Ok(Await _service.ListerAsync())
    End Function

    <HttpGet("{id:int}", Name:="ObtenirProduit")>
    Public Async Function ObtenirAsync(id As Integer) As Task(Of ActionResult(Of Produit))
        Dim produit = Await _service.ObtenirAsync(id)
        If produit Is Nothing Then Return NotFound()
        Return Ok(produit)
    End Function

    <HttpPost>
    Public Async Function CreerAsync(<FromBody> nouveau As CreationProduit) As Task(Of ActionResult(Of Produit))
        Dim cree = Await _service.CreerAsync(nouveau)
        ' 201 Created + en-tête Location pointant vers la ressource créée
        Return CreatedAtRoute("ObtenirProduit", New With {.id = cree.Id}, cree)
    End Function

    <HttpPut("{id:int}")>
    Public Async Function MettreAJourAsync(id As Integer, <FromBody> maj As MiseAJourProduit) As Task(Of IActionResult)
        If Not Await _service.MettreAJourAsync(id, maj) Then Return NotFound()
        Return NoContent()
    End Function

    <HttpDelete("{id:int}")>
    Public Async Function SupprimerAsync(id As Integer) As Task(Of IActionResult)
        Await _service.SupprimerAsync(id)
        Return NoContent()
    End Function
End Class
```

> 📌 **Pour monter l'exemple de bout en bout** : la classe `Produit` est le POCO déjà écrit au
> § [8.1](01-consommer-api-rest.md) ; `MiseAJourProduit` se déclare sur le même modèle que
> `CreationProduit` (plus bas) ; et `ProduitService` — l'implémentation de `IProduitService`
> enregistrée à la section suivante — est une simple classe `Implements IProduitService`, dont le
> corps dépend de votre source de données (module [7](../07-acces-donnees/README.md)).

Points à retenir :

- `ActionResult(Of T)` permet de renvoyer soit un objet typé (`Ok(produit)`), soit un autre code d'état (`NotFound()`), tout en informant OpenAPI du type retourné.
- Le **routage par attributs** (`<HttpGet("{id:int}")>`) est explicite et lisible. La contrainte `:int` filtre le segment d'URL.
- `CreatedAtRoute` renvoie un `201` avec l'en-tête `Location`. On utilise ici une **route nommée** (`Name:="ObtenirProduit"`) plutôt que `CreatedAtAction(NameOf(...))` : par défaut, MVC retire le suffixe `Async` du nom d'action, ce qui ferait échouer une résolution par nom de méthode. La route nommée évite ce piège.

### Liaison de modèle (*model binding*)

ASP.NET Core remplit automatiquement les paramètres d'action à partir de la requête. Les attributs précisent la source quand c'est ambigu :

| Attribut | Source |
|----------|--------|
| `<FromRoute>` | Segment d'URL (`/produits/42`) |
| `<FromQuery>` | Chaîne de requête (`?page=2`) |
| `<FromBody>` | Corps JSON (un seul par action) |
| `<FromHeader>` | En-tête HTTP |
| `<FromServices>` | Conteneur d'injection de dépendances |

La **validation** s'appuie sur les `DataAnnotations`. Grâce à `<ApiController>`, un modèle invalide déclenche automatiquement une réponse `400` au format `Problem Details` — sans code supplémentaire.

```vb
Imports System.ComponentModel.DataAnnotations

Public Class CreationProduit
    <Required>
    <StringLength(100, MinimumLength:=2)>
    Public Property Nom As String

    <Range(0.0, 100000.0)>
    Public Property Prix As Decimal
End Class
```

---

## 🧩 Injection de dépendances

Le conteneur d'injection est intégré. On enregistre les services au démarrage, et le constructeur du contrôleur les reçoit automatiquement (comme `IProduitService` ci-dessus).

```vb
' Dans Program.vb, avant builder.Build()
builder.Services.AddScoped(Of IProduitService, ProduitService)()
builder.Services.AddSingleton(Of IHorloge, HorlogeSysteme)()
```

```vb
Public Interface IProduitService
    Function ListerAsync() As Task(Of IReadOnlyList(Of Produit))
    Function ObtenirAsync(id As Integer) As Task(Of Produit)
    Function CreerAsync(nouveau As CreationProduit) As Task(Of Produit)
    Function MettreAJourAsync(id As Integer, maj As MiseAJourProduit) As Task(Of Boolean)
    Function SupprimerAsync(id As Integer) As Task
End Interface
```

Les trois durées de vie usuelles : `AddSingleton` (une instance pour l'application), `AddScoped` (une par requête HTTP — le cas typique d'un service métier ou d'un `DbContext`), `AddTransient` (une par résolution).

---

## 🔁 Middleware et pipeline de requêtes

Chaque requête traverse un **pipeline** de composants (*middleware*) dans l'ordre où ils sont déclarés. Cet ordre est déterminant : l'authentification doit précéder l'autorisation, qui doit précéder l'exécution des contrôleurs.

```vb
Dim app = builder.Build()

app.UseExceptionHandler()        ' capture les exceptions → Problem Details
app.UseHttpsRedirection()
app.UseRateLimiter()             ' limitation de débit (voir plus bas)
app.UseAuthentication()          ' qui es-tu ?
app.UseAuthorization()           ' as-tu le droit ?
app.MapControllers()             ' exécute l'action correspondante

app.Run()
```

Un middleware personnalisé est une classe exposant `InvokeAsync` ; ses dépendances par requête sont injectées en paramètres :

```vb
Imports System.Diagnostics
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.Logging

Public Class ChronoMiddleware
    Private ReadOnly _suivant As RequestDelegate

    Public Sub New(suivant As RequestDelegate)
        _suivant = suivant
    End Sub

    Public Async Function InvokeAsync(contexte As HttpContext,
                                      logger As ILogger(Of ChronoMiddleware)) As Task
        Dim chrono = Stopwatch.StartNew()
        Await _suivant(contexte)                 ' passe la main au middleware suivant
        chrono.Stop()
        logger.LogInformation("{Methode} {Chemin} en {Duree} ms",
            contexte.Request.Method, contexte.Request.Path, chrono.ElapsedMilliseconds)
    End Function
End Class
```

On le branche dans le pipeline avec `app.UseMiddleware(Of ChronoMiddleware)()`.

---

## 📖 Documentation OpenAPI / Swagger (OpenAPI 3.1 dans .NET 10) 🆕

Depuis .NET 9, ASP.NET Core génère la documentation OpenAPI **nativement** via le paquet `Microsoft.AspNetCore.OpenApi`, qui a remplacé Swashbuckle dans le modèle par défaut. Dans **.NET 10**, ce générateur produit des documents **OpenAPI 3.1 par défaut** (au lieu de 3.0), s'appuie sur le schéma JSON 2020-12, expose des *transformers* pour personnaliser le document, et reste compatible Native AOT.

```vb
' Services
builder.Services.AddOpenApi()

' Pipeline (en développement)
If app.Environment.IsDevelopment() Then
    app.MapOpenApi()                 ' expose /openapi/v1.json (OpenAPI 3.1)
    app.MapScalarApiReference()      ' interface Scalar interactive sur /scalar
End If
```

Le paquet natif génère le **document**, mais n'embarque **pas d'interface visuelle**. On ajoute donc une UI séparée : ici **Scalar** (`Scalar.AspNetCore`, ajouté au `.vbproj` plus haut). Swashbuckle reste disponible en option si l'on préfère l'interface Swagger classique.

### ⚠️ Friction VB : les commentaires XML n'alimentent pas le document

En .NET 10, l'enrichissement du document OpenAPI à partir des **commentaires de documentation XML** (résumés, descriptions) est assuré par un **générateur de source C#** exécuté à la compilation. Comme tous les générateurs de source qui émettent du C#, **il ne s'exécute pas dans un projet VB.NET**. Vos commentaires `'''<summary>` ne se retrouveront donc pas dans le document. 🔗

La **génération du document lui-même fonctionne parfaitement** en VB ; c'est seulement cet enrichissement « gratuit » par commentaires qui manque. Deux parades, toutes deux indépendantes du langage :

1. **Décrire via des attributs de métadonnées** sur les actions : `<ProducesResponseType>`, `<Produces>`, `<Tags>`, et les attributs `<EndpointSummary>` / `<EndpointDescription>`.

```vb
<HttpGet("{id:int}", Name:="ObtenirProduit")>
<EndpointSummary("Récupère un produit par son identifiant")>
<ProducesResponseType(GetType(Produit), StatusCodes.Status200OK)>
<ProducesResponseType(StatusCodes.Status404NotFound)>
Public Async Function ObtenirAsync(id As Integer) As Task(Of ActionResult(Of Produit))
    ' ...
End Function
```

2. **Personnaliser le document par *transformer*** (exécuté à l'exécution, donc valable en VB) — par exemple pour le titre et la description globale :

```vb
builder.Services.AddOpenApi(Sub(options)
        options.AddDocumentTransformer(Function(document, contexte, jeton)
            document.Info = New OpenApiInfo With {
                .Title = "API Catalogue",
                .Version = "v1",
                .Description = "Gestion du catalogue de produits."
            }
            Return Task.CompletedTask
        End Function)
    End Sub)
```

*(Le type `OpenApiInfo` provient de `Microsoft.OpenApi` ; l'espace de noms exact dépend de la version du paquet `Microsoft.OpenApi`, passé en v2 avec .NET 10.)*

---

## 🔐 Authentification (JWT, OAuth 2.0 / OpenID Connect) — consommation

Une Web API **consomme** des jetons émis par un fournisseur d'identité (Microsoft Entra ID, Auth0, Keycloak…) : elle ne les fabrique pas, elle les **valide**. C'est le rôle de l'authentification **JWT Bearer** (paquet `Microsoft.AspNetCore.Authentication.JwtBearer`).

```vb
Imports Microsoft.AspNetCore.Authentication.JwtBearer
Imports Microsoft.IdentityModel.Tokens

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) _
    .AddJwtBearer(Sub(options)
        options.Authority = "https://login.microsoftonline.com/{tenant}/v2.0"
        options.Audience = "api://mon-api"
        options.TokenValidationParameters = New TokenValidationParameters With {
            .ValidateIssuer = True,
            .ValidateAudience = True,
            .ValidateLifetime = True
        }
    End Sub)

builder.Services.AddAuthorization()
```

On protège ensuite un contrôleur ou une action avec `<Authorize>`, en restreignant éventuellement par rôle ou par politique :

```vb
<Authorize(Roles:="Administrateur")>
<HttpDelete("{id:int}")>
Public Async Function SupprimerAsync(id As Integer) As Task(Of IActionResult)
    ' Accessible uniquement avec un jeton valide portant le rôle "Administrateur"
    Await _service.SupprimerAsync(id)
    Return NoContent()
End Function
```

> ⚠️ Dans le pipeline, `UseAuthentication()` doit précéder `UseAuthorization()`. Le détail des protocoles (flux OAuth 2.0 / OpenID Connect, configuration Entra ID, gestion des *scopes*) est traité au **module 16.1**.

---

## 🧯 `Problem Details` (RFC 9457)

`Problem Details` normalise les réponses d'erreur dans un format JSON standard (champs `type`, `title`, `status`, `detail`, `instance`). On l'active globalement :

```vb
builder.Services.AddProblemDetails()
```

Couplé à `app.UseExceptionHandler()`, toute exception non gérée produit alors une réponse `Problem Details` propre, sans fuite de détails internes. Et, grâce à `<ApiController>`, les **erreurs de validation** sont déjà renvoyées dans ce format.

Dans une action, on renvoie une erreur métier structurée avec `Problem(...)` :

```vb
<HttpPost>
Public Async Function CreerAsync(<FromBody> nouveau As CreationProduit) As Task(Of ActionResult(Of Produit))
    If Await _service.ExisteAsync(nouveau.Nom) Then
        Return Problem(
            detail:=$"Un produit nommé « {nouveau.Nom} » existe déjà.",
            statusCode:=StatusCodes.Status409Conflict,
            title:="Conflit de ressource")
    End If

    Dim cree = Await _service.CreerAsync(nouveau)
    Return CreatedAtRoute("ObtenirProduit", New With {.id = cree.Id}, cree)
End Function
```

---

## 🔢 Versioning d'API

Pour faire évoluer une API sans casser les clients existants, on la **versionne**. Le paquet de référence est **`Asp.Versioning.Mvc`** (avec `Asp.Versioning.Mvc.ApiExplorer` pour l'intégration OpenAPI) ; l'ancien `Microsoft.AspNetCore.Mvc.Versioning` est **déprécié**.

```vb
Imports Asp.Versioning

builder.Services.AddApiVersioning(Sub(options)
        options.DefaultApiVersion = New ApiVersion(1, 0)
        options.AssumeDefaultVersionWhenUnspecified = True
        options.ReportApiVersions = True          ' annonce les versions via en-têtes
    End Sub).AddMvc().AddApiExplorer(Sub(options)
        options.GroupNameFormat = "'v'VVV"
        options.SubstituteApiVersionInUrl = True
    End Sub)
```

On déclare la version sur le contrôleur, et on l'intègre à la route :

```vb
<ApiController>
<ApiVersion("1.0")>
<ApiVersion("2.0")>
<Route("api/v{version:apiVersion}/[controller]")>
Public Class ProduitsController
    Inherits ControllerBase

    <HttpGet>
    <MapToApiVersion("2.0")>
    Public Async Function ListerV2Async() As Task(Of ActionResult(Of IEnumerable(Of Produit)))
        ' Variante réservée à la version 2.0
        Return Ok(Await _service.ListerEnrichiAsync())
    End Function
End Class
```

Les stratégies courantes : segment d'URL (`/api/v2/...`, le plus explicite), paramètre de requête, ou en-tête HTTP.

---

## 🚦 Rate limiting intégré

Depuis .NET 7, la **limitation de débit** est intégrée à ASP.NET Core, sans paquet tiers. On définit une ou plusieurs politiques (fenêtre fixe, fenêtre glissante, jeton-seau, concurrence), puis on les active.

```vb
Imports System.Threading.RateLimiting          ' les options des limiteurs
Imports Microsoft.AspNetCore.RateLimiting      ' AddFixedWindowLimiter & co (extensions)

builder.Services.AddRateLimiter(Sub(options)
        options.AddFixedWindowLimiter("fixe", Sub(limiteur)
            limiteur.PermitLimit = 100              ' 100 requêtes…
            limiteur.Window = TimeSpan.FromMinutes(1) ' …par minute
            limiteur.QueueLimit = 0
        End Sub)
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests
    End Sub)
```

Côté pipeline, `app.UseRateLimiter()` ; côté contrôleur, on applique la politique par attribut :

```vb
Imports Microsoft.AspNetCore.RateLimiting

<EnableRateLimiting("fixe")>
<ApiController>
<Route("api/[controller]")>
Public Class ProduitsController
    Inherits ControllerBase
    ' Toutes les actions de ce contrôleur sont limitées par la politique "fixe".
    ' <DisableRateLimiting> exempte une action particulière si besoin.
End Class
```

---

## 🆕 Récapitulatif des apports .NET 10 pour ce module

- **OpenAPI 3.1 par défaut** via le générateur natif `Microsoft.AspNetCore.OpenApi`, avec génération possible à la compilation et transformers améliorés.
- **`Problem Details`** mieux intégré au traitement des erreurs et à la validation.
- **Limitation de débit** affinée dans le pipeline.
- Documentation OpenAPI enrichie par **commentaires XML**… mais via un générateur de source C# — d'où la parade par attributs et transformers en VB (voir plus haut).

---

## ⚠️ Les frictions VB à connaître

| Sujet | État en VB.NET | Parade |
|-------|----------------|--------|
| Modèle de projet Web API | ❌ Inexistant | Partir d'un console VB → SDK `Microsoft.NET.Sdk.Web` |
| Point d'entrée | ⚠️ Pas de *top-level statements* (ni de `Main` retournant `Task`) | `Module Program` + `Sub Main`, délégant à un `MainAsync` au besoin |
| *Scaffolding* de contrôleurs (VS) | ⚠️ Orienté C# | Écrire les contrôleurs à la main (simples classes) |
| Commentaires XML → OpenAPI | ⚠️ Générateur de source C# | Attributs (`ProducesResponseType`, `EndpointSummary`) + transformers |
| Minimal APIs | ⚠️ Peu idiomatique | Utiliser des contrôleurs |

Aucune de ces frictions n'est bloquante : ce sont des ajustements de mise en route, pas des limites de capacité. Le **fond** — contrôleurs, DI, middleware, sécurité, versioning, *rate limiting* — est pleinement disponible en VB.NET.

---

## 🧭 À retenir

- En VB.NET, une Web API **se construit avec des contrôleurs** (`ControllerBase`) : c'est le scénario web réaliste et recommandé ✅.
- Il faut **monter le projet à la main** (pas de modèle, pas de *top-level statements*), puis tout le reste suit la documentation ASP.NET Core en syntaxe VB.
- **OpenAPI 3.1** est généré nativement (.NET 10) ; en VB, on enrichit le document par **attributs et transformers**, le générateur de commentaires XML étant réservé à C#.
- L'**authentification JWT** valide des jetons émis par un fournisseur d'identité (consommation) ; `Problem Details`, versioning (`Asp.Versioning.Mvc`) et *rate limiting* intégré complètent une API de production.
- Les rares frictions VB se contournent simplement ; le cas échéant, une brique très spécifique (front web, génération de source) se délègue à C# selon la **stratégie hybride** (module 10, [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

---

⬅️ [8.1 — Consommer des API REST](01-consommer-api-rest.md) · ➡️ [8.3 — Limites du web en VB.NET](03-limites-web-vbnet.md)

⏭️ [⚠️ Limites du web en VB.NET](/08-services-web/03-limites-web-vbnet.md)
