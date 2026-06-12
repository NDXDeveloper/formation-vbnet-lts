🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 16.1 Authentification et autorisation

L'authentification et l'autorisation forment la **première ligne de défense** de toute application : elles déterminent qui peut entrer, et ce qu'il a le droit de faire une fois entré. C'est aussi le domaine où l'écosystème .NET impose de s'appuyer sur des **standards éprouvés** plutôt que d'improviser — improviser, ici, c'est ouvrir une faille.

Bonne nouvelle, dans la lignée du [README du module](README.md) : tout ce qui suit relève du **périmètre de consommation** dans lequel VB.NET est pleinement à l'aise. Les protocoles (OAuth 2.0, OpenID Connect), les jetons (JWT) et les bibliothèques (Microsoft Entra ID, MSAL — *Microsoft Authentication Library*) sont les mêmes qu'en C#, et s'utilisent à l'identique depuis VB.NET.

## Deux notions à ne jamais confondre

| | Question posée | Anglais | Réponse type |
|---|---|---|---|
| **Authentification** | *Qui êtes-vous ?* | AuthN | « Cet utilisateur est bien Alice. » |
| **Autorisation** | *Qu'avez-vous le droit de faire ?* | AuthZ | « Alice peut lire les commandes, mais pas les supprimer. » |

L'authentification **précède toujours** l'autorisation : on ne peut décider des droits d'une personne qu'après avoir établi son identité. Confondre les deux — par exemple croire qu'un utilisateur authentifié est, de ce fait, autorisé à tout — est une erreur de conception classique et dangereuse.

## OAuth 2.0 et OpenID Connect : les standards modernes

### OAuth 2.0 — l'autorisation déléguée

OAuth 2.0 est un protocole d'**autorisation déléguée**. Il répond à un besoin précis : permettre à une application d'accéder à des ressources **au nom d'un utilisateur**, sans jamais manipuler son mot de passe. Quand une application vous demande « se connecter avec Microsoft » et que vous accordez l'accès, c'est OAuth 2.0 qui orchestre l'échange.

Point fondamental souvent mal compris : **OAuth 2.0 n'est pas un protocole d'authentification**. Il gère des autorisations (l'accès à des ressources), pas l'identité de l'utilisateur. Pour authentifier, on lui ajoute une couche : OpenID Connect.

### OpenID Connect — la couche d'authentification

OpenID Connect (OIDC) est une **surcouche d'OAuth 2.0** qui standardise l'authentification. Là où OAuth 2.0 délivre un droit d'accès, OIDC délivre en plus une **preuve d'identité** sous la forme d'un *jeton d'identité* (ID token). C'est OIDC qui rend possible le « Se connecter avec… » de façon sûre et interopérable.

En résumé : **OAuth 2.0 autorise, OpenID Connect authentifie.** Dans une application moderne, on utilise presque toujours les deux ensemble.

### Les acteurs et les flux

OAuth 2.0 / OIDC fait intervenir quatre rôles :

- **Le propriétaire de la ressource** (*resource owner*) : l'utilisateur.
- **Le client** : votre application (de bureau, web ou API) qui demande l'accès.
- **Le serveur d'autorisation** (*authorization server*) : l'autorité qui authentifie et délivre les jetons (par ex. Microsoft Entra ID).
- **Le serveur de ressources** (*resource server*) : l'API protégée qui vérifie les jetons.

Le protocole définit plusieurs **flux** (*grant types*). Le choix dépend du type de client :

| Flux | Scénario type | Recommandation |
|------|---------------|----------------|
| **Authorization Code + PKCE** | Application interactive avec un utilisateur (bureau, web, mobile, SPA) | ✅ Le choix par défaut |
| **Client Credentials** | Service à service, démon, tâche de fond — **sans utilisateur** | ✅ Pour les traitements automatiques |
| **Device Code** | Appareils sans navigateur (IoT, terminal) | ✅ Cas particuliers |
| Implicit / Resource Owner Password (ROPC) | (hérités) | ⚠️ À éviter — obsolètes et peu sûrs |

Pour une application de bureau VB.NET (Windows Forms, WPF) qui authentifie un utilisateur, le flux à retenir est **Authorization Code avec PKCE** (*Proof Key for Code Exchange*) — heureusement géré de bout en bout par MSAL (voir plus bas), sans que vous ayez à l'implémenter à la main.

### Pourquoi ne jamais réinventer ces protocoles

Ces standards encapsulent des années de retours d'expérience sur les attaques (interception de jetons, rejeu, hameçonnage du consentement…). Réimplémenter « sa » connexion à la main, c'est réintroduire des failles que ces protocoles ont précisément éliminées. **La règle est simple : on consomme une bibliothèque éprouvée, on n'écrit pas son propre protocole d'authentification.**

## Les jetons JWT (JSON Web Token)

Le JWT est le format de jeton omniprésent dans OAuth 2.0 / OIDC. Le comprendre est indispensable pour sécuriser correctement une API.

### Anatomie d'un jeton

Un JWT est une chaîne de trois parties séparées par des points, chacune encodée en Base64URL :

```
eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9 . eyJzdWIiOiI...payload... . SflKxw...signature
└──────── en-tête (header) ────────┘   └─── charge utile (payload) ───┘   └─ signature ─┘
```

- **L'en-tête** indique l'algorithme de signature (par ex. `RS256`) et le type.
- **La charge utile** contient les *revendications* (*claims*) : des informations sur l'utilisateur et le jeton. Quelques revendications standard :

```json
{
  "iss": "https://login.microsoftonline.com/{tenant}/v2.0",  // émetteur (issuer)
  "aud": "api://mon-api",                                     // destinataire (audience)
  "sub": "a1b2c3d4-...",                                      // identifiant de l'utilisateur (subject)
  "exp": 1718200000,                                          // date d'expiration
  "roles": ["Administrateur"],                                // rôles applicatifs
  "scp": "Commandes.Read Commandes.Write"                     // périmètres (scopes) accordés
}
```

- **La signature** garantit que le jeton n'a pas été altéré et qu'il provient bien de l'autorité attendue.

> ⚠️ **Un JWT est signé, pas chiffré.** Sa charge utile est lisible par quiconque possède le jeton (un simple décodage Base64URL suffit). On n'y place donc **jamais** de donnée sensible (mot de passe, secret, donnée personnelle non nécessaire).

### Trois jetons, trois rôles

| Jeton | Délivré par | Rôle |
|-------|-------------|------|
| **Jeton d'accès** (*access token*) | OAuth 2.0 | Présenté à l'API pour prouver le **droit d'accès**. C'est lui que l'on envoie dans l'en-tête `Authorization: Bearer …`. |
| **Jeton d'identité** (*ID token*) | OpenID Connect | Prouve au **client** l'**identité** de l'utilisateur authentifié. Ne sert pas à appeler une API. |
| **Jeton de rafraîchissement** (*refresh token*) | OAuth 2.0 | Permet d'obtenir un nouveau jeton d'accès sans réauthentifier l'utilisateur, lorsque le précédent a expiré. À conserver de façon sécurisée. |

### Valider un jeton, c'est *tout* valider

Côté API, accepter un JWT sans le valider complètement revient à ne pas avoir de sécurité du tout. Une validation correcte vérifie au minimum :

- **la signature** (le jeton vient bien de l'autorité de confiance) ;
- **l'émetteur** (`iss`) et **l'audience** (`aud`) — le jeton a été émis par votre autorité **et destiné à votre API** ;
- **l'expiration** (`exp`) — le jeton n'est pas périmé.

La bonne nouvelle : avec l'intergiciel d'authentification d'ASP.NET Core, ces validations sont **réalisées automatiquement** dès lors que la configuration est correcte (voir la mise en œuvre ci-dessous). Vous n'écrivez pas le code de vérification cryptographique : vous le **configurez**.

## Microsoft Entra ID

**Microsoft Entra ID** (anciennement *Azure Active Directory*) est le fournisseur d'identité (IdP) cloud de Microsoft. Il joue le rôle de **serveur d'autorisation** OAuth 2.0 / OIDC : il authentifie les utilisateurs et délivre les jetons. C'est la solution de prédilection pour les applications d'entreprise de l'écosystème .NET / Microsoft 365.

### Les concepts à connaître

- **Locataire** (*tenant*) : l'instance d'annuaire de votre organisation.
- **Inscription d'application** (*app registration*) : la déclaration de votre application auprès d'Entra ID, qui lui attribue un **identifiant client** (*client ID*) et définit ce qu'elle a le droit de demander.
- **Périmètres** (*scopes*) : les autorisations fines exposées par une API (par ex. `Commandes.Read`).
- **Rôles applicatifs** (*app roles*) : des rôles que l'on attribue aux utilisateurs ou aux services (par ex. `Administrateur`), reflétés dans le jeton.

### Deux familles de bibliothèques

Microsoft fournit deux bibliothèques complémentaires, à ne pas confondre :

| Bibliothèque | Pour quel projet | Rôle |
|--------------|------------------|------|
| **Microsoft.Identity.Web** | Web API et applications web (**côté serveur**) | Protéger une API : valider les jetons entrants, intégrer Entra ID à ASP.NET Core. |
| **MSAL.NET** (`Microsoft.Identity.Client`) | Applications **clientes** (bureau WinForms/WPF, console) | Acquérir des jetons : authentifier l'utilisateur et obtenir un jeton d'accès pour appeler une API. |

Le couple typique d'une solution VB.NET est donc : une **application de bureau** qui utilise **MSAL.NET** pour se connecter, appelant une **Web API** protégée par **Microsoft.Identity.Web**.

### Et les comptes locaux ? ASP.NET Core Identity

Tout ce qui précède **délègue** l'identité à un fournisseur (Entra ID). Pour une application qui gère **ses propres comptes** (inscription, connexion, mots de passe, verrouillage), ASP.NET Core fournit **ASP.NET Core Identity** — le successeur moderne de *Forms Authentication* et d'*ASP.NET Membership* du monde .NET Framework (voir [module 11 (§11.4)](../11-migration-legacy/04-web-forms-legacy.md)). Ses modèles de projet et son interface générée sont orientés C#, mais la bibliothèque se **consomme** depuis VB.NET comme n'importe quel paquet — son `PasswordHasher(Of TUser)` est d'ailleurs la voie recommandée pour hacher des mots de passe ([16.2](02-cryptographie.md)). Cela dit, dès qu'un annuaire existe, **déléguer à un IdP** reste l'option la plus sûre : ne pas stocker de mots de passe du tout, c'est autant de responsabilité en moins.

## Mise en œuvre en VB.NET

> **Note de périmètre.** Tout le code ci-dessous est de la **consommation pure** : il fonctionne en VB.NET exactement comme en C#, sans qu'aucune brique ne doive être déléguée à une bibliothèque C#. La seule particularité concerne la **structure d'amorçage** d'une Web API en VB.NET : faute de modèle de projet Web et d'*instructions de premier niveau*, on encapsule la configuration dans un `Sub Main` (voir [module 8](../08-services-web/README.md) pour la mise en place du projet).

### Protéger une Web API (côté serveur)

**Approche générique (compatible avec n'importe quel fournisseur OIDC).** On configure l'authentification par jeton porteur (*JWT Bearer*) en indiquant l'autorité et l'audience attendues :

```vb
Imports Microsoft.AspNetCore.Authentication.JwtBearer

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        builder.Services _
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) _
            .AddJwtBearer(Sub(options)
                              ' L'autorité expose ses clés de signature via les métadonnées
                              ' OIDC : le handler les récupère et les renouvelle tout seul.
                              options.Authority = "https://login.microsoftonline.com/{tenant-id}/v2.0"
                              options.Audience = "api://{api-client-id}"
                          End Sub)

        builder.Services.AddAuthorization()
        builder.Services.AddControllers()

        Dim app = builder.Build()

        ' L'ORDRE EST IMPORTANT : authentifier AVANT d'autoriser.
        app.UseAuthentication()
        app.UseAuthorization()
        app.MapControllers()

        app.Run()
    End Sub
End Module
```

Avec cette configuration, le framework valide **automatiquement** la signature, l'émetteur, l'audience et l'expiration de chaque jeton reçu. Pour un contrôle plus fin, on peut renseigner explicitement les paramètres de validation (type `TokenValidationParameters`, espace de noms `Microsoft.IdentityModel.Tokens`) :

```vb
options.TokenValidationParameters = New TokenValidationParameters With {
    .ValidateIssuer = True,
    .ValidateAudience = True,
    .ValidateLifetime = True,
    .ValidateIssuerSigningKey = True,
    .ClockSkew = TimeSpan.FromSeconds(30)   ' tolérance d'horloge réduite
}
```

**Approche spécifique à Entra ID.** Microsoft.Identity.Web simplifie encore la configuration pour les locataires Entra. On déporte les paramètres dans `appsettings.json` :

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "{tenant-id}",
    "ClientId": "{api-client-id}",
    "Audience": "api://{api-client-id}"
  }
}
```

…puis :

```vb
Imports Microsoft.Identity.Web

builder.Services _
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) _
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
```

> 🔒 Ces valeurs de configuration ne sont pas des secrets, mais **un secret client ne doit jamais figurer dans `appsettings.json` ni dans le code source**. La gestion sécurisée des secrets est traitée en [16.2](02-cryptographie.md).

### Authentifier un client de bureau avec MSAL.NET ⭐

Voici le scénario le plus idiomatique côté VB.NET : une application Windows Forms ou WPF qui authentifie l'utilisateur auprès d'Entra ID, puis appelle l'API protégée ci-dessus. On isole la logique d'authentification dans un service dédié :

```vb
Imports Microsoft.Identity.Client

Public Class ServiceAuthentification
    Private ReadOnly _app As IPublicClientApplication
    Private ReadOnly _scopes As String() = {"api://{api-client-id}/access_as_user"}

    Public Sub New()
        _app = PublicClientApplicationBuilder _
            .Create("{client-id-du-bureau}") _
            .WithAuthority(AzureCloudInstance.AzurePublic, "{tenant-id}") _
            .WithRedirectUri("http://localhost") _
            .Build()
    End Sub

    Public Async Function ObtenirJetonAccesAsync() As Task(Of String)
        Dim comptes = Await _app.GetAccountsAsync()
        Dim resultat As AuthenticationResult = Nothing
        Dim interactionRequise = False

        Try
            ' 1. On tente d'abord un jeton silencieux (depuis le cache).
            resultat = Await _app.AcquireTokenSilent(_scopes, comptes.FirstOrDefault()) _
                .ExecuteAsync()
        Catch ex As MsalUiRequiredException
            ' 2. Aucun jeton valide en cache : on le note, sans plus —
            '    Await est interdit dans un bloc Catch en VB.
            interactionRequise = True
        End Try

        If interactionRequise Then
            ' 3. Authentification interactive (ouverture du navigateur,
            '    flux Authorization Code + PKCE géré par MSAL).
            resultat = Await _app.AcquireTokenInteractive(_scopes) _
                .ExecuteAsync()
        End If

        Return resultat.AccessToken
    End Function
End Class
```

> ⚠️ **Le détour par `interactionRequise` n'est pas une coquetterie.** Les exemples C# de la documentation MSAL placent l'appel interactif directement dans le bloc `Catch` — en VB.NET, c'est impossible : `Await` est **interdit dans `Catch` et `Finally`** (erreur `BC36943`, voir [module 4 (§4.3)](../04-async/03-exceptions-async.md)). On note donc la décision dans le `Catch`, et l'on relance l'acquisition **après** le `Try`. Ce piège de transposition C# → VB est récurrent dès qu'une logique de repli asynchrone se déclenche sur exception.

L'appel à l'API consiste alors simplement à placer le jeton dans l'en-tête `Authorization` :

```vb
Imports System.Net.Http
Imports System.Net.Http.Headers

Dim jeton = Await _serviceAuth.ObtenirJetonAccesAsync()
_httpClient.DefaultRequestHeaders.Authorization =
    New AuthenticationHeaderValue("Bearer", jeton)

Dim reponse = Await _httpClient.GetAsync("https://mon-api/api/commandes")
```

Tout le protocole (PKCE, échange de code, mise en cache et rafraîchissement des jetons) est pris en charge par MSAL : vous n'écrivez que la logique métier.

## L'autorisation en pratique

Une fois l'utilisateur authentifié, ASP.NET Core offre plusieurs niveaux de contrôle d'accès, du plus simple au plus expressif.

### Exiger une authentification

L'attribut `<Authorize>` exige simplement un utilisateur authentifié :

```vb
Imports Microsoft.AspNetCore.Authorization
Imports Microsoft.AspNetCore.Mvc

<Authorize>
<ApiController>
<Route("api/[controller]")>
Public Class CommandesController
    Inherits ControllerBase

    <HttpGet>
    Public Function ObtenirCommandes() As IActionResult
        ' Accessible à tout utilisateur authentifié.
        Return Ok("Liste des commandes.")
    End Function
End Class
```

### Par rôle (RBAC)

On restreint une action à un ou plusieurs rôles applicatifs — le contrôle d'accès par rôles (*Role-Based Access Control*, RBAC) :

```vb
<HttpDelete("{id}")>
<Authorize(Roles:="Administrateur")>
Public Function SupprimerCommande(id As Integer) As IActionResult
    ' Réservé aux porteurs du rôle « Administrateur ».
    Return NoContent()
End Function
```

### Par politique (*policy-based*) — l'approche recommandée

Le contrôle par rôle est rigide. L'approche moderne d'ASP.NET Core consiste à définir des **politiques** nommées, qui combinent rôles, revendications et périmètres de façon centralisée :

```vb
builder.Services.AddAuthorization(Sub(options)
    options.AddPolicy("AdminUniquement",
        Sub(policy) policy.RequireRole("Administrateur"))

    options.AddPolicy("MajeurVerifie",
        Sub(policy) policy.RequireClaim("age_verifie", "true"))

    options.AddPolicy("LectureCommandes",
        Sub(policy) policy.RequireAssertion(
            Function(contexte)
                Dim scp = contexte.User.FindFirst("scp")?.Value
                Return scp IsNot Nothing AndAlso
                       scp.Split(" "c).Contains("Commandes.Read")
            End Function))
End Sub)
```

> ⚠️ **Pourquoi pas `policy.RequireClaim("scp", "Commandes.Read")` ?** Parce que `RequireClaim` teste l'**égalité exacte** de la valeur du claim — or, dans un jeton Entra ID, `scp` est **une seule revendication** contenant tous les périmètres **séparés par des espaces** (revoyez la charge utile d'exemple plus haut : `"scp": "Commandes.Read Commandes.Write"`). La politique fondée sur `RequireClaim` refuserait donc l'accès à un jeton parfaitement valide. D'où le `RequireAssertion`, qui découpe la valeur avant de chercher le périmètre. Avec **Microsoft.Identity.Web**, l'extension `policy.RequireScope("Commandes.Read")` et l'attribut `<RequiredScope(...)>` encapsulent précisément cette logique.

…que l'on applique ensuite par leur nom :

```vb
<HttpGet>
<Authorize(Policy:="LectureCommandes")>
Public Function ObtenirCommandes() As IActionResult
    Return Ok("Liste des commandes — visible avec le périmètre Commandes.Read.")
End Function
```

L'avantage est la **centralisation** : la règle d'accès est définie une fois, modifiable à un seul endroit, et lisible par son intention (`"MajeurVerifie"`) plutôt que par sa mécanique.

### Lire l'identité de l'utilisateur

Dans un contrôleur, la propriété `User` donne accès aux revendications du jeton validé :

```vb
Imports System.Security.Claims

<HttpGet("moi")>
Public Function ObtenirMonProfil() As IActionResult
    Dim identifiant = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
    Dim nom = User.Identity?.Name
    Dim estAdmin = User.IsInRole("Administrateur")

    Return Ok(New With {identifiant, nom, estAdmin})
End Function
```

## Bonnes pratiques et pièges à éviter

- **Toujours en HTTPS.** Un jeton circulant en clair est un jeton volé. Le chiffrement du transport est non négociable.
- **Ne jamais réimplémenter l'authentification.** On s'appuie sur OAuth 2.0 / OIDC et sur des bibliothèques éprouvées (Entra ID, MSAL), jamais sur un mécanisme maison.
- **Valider intégralement les jetons** côté API : signature, émetteur, audience, expiration. Laissez le framework le faire, mais vérifiez que la configuration est correcte.
- **Jetons d'accès de courte durée**, complétés par des jetons de rafraîchissement — pour limiter la fenêtre d'exploitation d'un jeton compromis.
- **Aucun secret dans le code ni dans `appsettings.json`.** Les secrets clients vont dans un coffre (voir [16.2](02-cryptographie.md)).
- **Principe du moindre privilège.** N'accordez que les périmètres et rôles strictement nécessaires ; une application qui demande trop de droits est une cible plus dangereuse.
- **Ne pas confondre authentification et autorisation.** Un utilisateur authentifié n'est pas, pour autant, autorisé à tout.

---

L'authentification garantit l'identité, l'autorisation borne les actions — mais les données qu'échangent vos applications doivent aussi être **protégées en elles-mêmes**, qu'elles transitent sur le réseau ou reposent en base. C'est l'objet de la section suivante.

**Suite : [16.2 — Cryptographie »](02-cryptographie.md)**

⏭️ [Cryptographie (hachage, chiffrement, gestion des secrets, Key Vault)](/16-securite/02-cryptographie.md)
