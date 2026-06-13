# 💻 Exemples du module 8 — Consommer et exposer des services

Cinq exemples **complets, compilés et exécutés**, un par section (le README du module
n'a pas de code). Ce module trace la **frontière web de VB.NET** : certains exemples sont
donc des **serveurs réels** (Web API, temps réel) et le dernier est une **solution hybride
VB + C#** — exactement la stratégie que le cours recommande pour l'outillage orienté C#.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

---

## 🔧 Partis pris pour rendre les exemples exécutables hors ligne

Le cours décrit des scénarios qui supposent un service distant, un fournisseur d'identité,
ou un cluster. Pour que **tout tourne sur un poste isolé**, et **sans rien sacrifier du sujet
enseigné**, ces exemples appliquent les adaptations suivantes — toutes signalées dans les
en-têtes de fichiers :

- **8.1** embarque un petit **serveur local** (`System.Net.HttpListener`) qui tient lieu
  d'« API distante » à consommer ; le sujet (client typé + résilience Polly) est intact.
- **8.4** **héberge** in-process le hub SignalR, l'endpoint WebSocket et le flux SSE, puis
  les **consomme** dans le même programme — sorties directement vérifiables.
- **8.2** remplace l'autorité JWT externe (Entra ID…) par une **clé symétrique locale** et un
  émetteur de jeton **de test** (`/dev/jeton`) : la chaîne « 401 → jeton → 204 » devient
  testable hors ligne. L'API ne fait que **valider** le jeton, comme dans le cours.
- **8.5** est une **solution hybride** : la brique gRPC (génération `.proto`) et le serveur
  GraphQL (HotChocolate) vivent dans des projets **C#** ; le projet **VB** les **consomme** —
  c'est précisément la démonstration de la section.

C'est le même esprit que le module 7 (SQLite à la place de SQL Server) : on substitue
l'**infrastructure**, jamais le **concept**.

---

## ▶️ Comment compiler et lancer

Trois familles d'exemples, trois modes de lancement :

### Applications console (8.1, 8.4) — sortie directement vérifiable
```bash
cd 8.1-consommer-api-rest        # ou 8.4-temps-reel
dotnet run                       # ou : ouvrir le .vbproj dans VS 2026, puis F5
```

### Serveurs web (8.2, 8.4 héberge, 8.3) — à interroger
Dans **Visual Studio 2026**, ouvrir le `.vbproj` et **F5** (le profil `launchSettings.json`
fixe l'URL et l'environnement *Development*).

> ⚠️ **En ligne de commande** (SDK 10.0.301, ce poste), `dotnet run` **n'applique pas**
> `launchSettings.json` (ni `--launch-profile`) : il démarre en *Production* sur le port 5000.
> Pour reproduire l'environnement validé, on passe les variables explicitement (elles, sont
> bien prises en compte) :
> ```powershell
> $env:ASPNETCORE_ENVIRONMENT = 'Development'
> $env:ASPNETCORE_URLS = 'http://localhost:5180'   # 5181 pour 8.3
> dotnet run
> ```
> On interroge ensuite l'API avec `Invoke-RestMethod` / `curl` (voir chaque section).

### Solution hybride (8.5) — serveur C# + client VB
```bash
# 1) démarrer le serveur de services (C#), il écoute sur 5185 (gRPC) et 5186 (REST+GraphQL)
cd 8.5-grpc-graphql/ServeurServices && dotnet run
# 2) dans un autre terminal, lancer le client VB
cd 8.5-grpc-graphql/ClientVb && dotnet run
```

Toutes les valeurs affichées ci-dessous ont été **observées à l'exécution**.

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Type | Paquets NuGet (versions validées) |
|---|---|---|---|
| `README.md` (module) | — (aucun code) | | |
| `01-consommer-api-rest.md` | [`8.1-consommer-api-rest`](#81-consommer-api-rest) | Console VB | Microsoft.Extensions.Hosting 10.0.0 · Microsoft.Extensions.Http.Resilience 10.0.0 |
| `02-web-api-controllers.md` | [`8.2-web-api-controllers`](#82-web-api-controllers) | Web API VB | Microsoft.AspNetCore.OpenApi 10.0.0 · Scalar.AspNetCore 2.16.3 · …JwtBearer 10.0.0 · Asp.Versioning.Mvc(.ApiExplorer) 8.1.1 |
| `03-limites-web-vbnet.md` | [`8.3-minimal-api-vb`](#83-minimal-api-vb) | Minimal API VB | — (SDK Web seul) |
| `04-temps-reel.md` | [`8.4-temps-reel`](#84-temps-reel) | Console VB (héberge) | Microsoft.AspNetCore.SignalR.Client 10.0.0 · *(SseParser : framework)* |
| `05-grpc-graphql.md` | [`8.5-grpc-graphql`](#85-grpc-graphql) | Hybride VB + C# | Grpc.AspNetCore 2.80.0 · …Grpc.JsonTranscoding 10.0.0 · HotChocolate.AspNetCore 16.1.3 · Grpc.Tools 2.81.1 · Google.Protobuf 3.35.1 |

---

## 8.1-consommer-api-rest

- **Section** : 8.1 — Consommer des API REST · **Fichier** : `01-consommer-api-rest.md`
- **Description** : un **client typé** (`CatalogueClient`) reçoit un `HttpClient` par injection
  via `IHttpClientFactory` (`AddHttpClient(Of CatalogueClient)`), avec **résilience Polly**
  (`AddStandardResilienceHandler` : *retries*, disjoncteur, *timeouts*). Appels via
  `GetFromJsonAsync` / `PostAsJsonAsync`, `JsonSerializerOptions` **`Shared ReadOnly`**, POCO
  avec `<JsonPropertyName("en_stock")>`, et **gestion explicite du 404** (→ `Nothing`). Un
  serveur `HttpListener` local sert le JSON.
- **Sortie attendue** (vérifiée) :
  ```text
  GET /api/produits -> 3 produit(s) reçu(s) :
    #1 Clavier — 49,90 € — en stock : True
    #2 Écran — 220,00 € — en stock : True
    #3 Souris — 25,00 € — en stock : False
  GET /api/produits/2 -> Écran (220,00 €)
  GET /api/produits/999 -> introuvable (404 géré)
  POST /api/produits -> créé avec id 4 : Webcam
  GET /api/produits (après POST) -> 4 produit(s)
  ```
- **Comportement vérifié** : la désérialisation mappe `en_stock` → `EnStock` ; le 404 est traité
  comme une absence légitime ; le POST renvoie la ressource créée (id 4) et la liste s'allonge.
  Les journaux Polly/HttpClient (niveau *Information*) sont réduits à *Warning* pour une sortie
  nette.

## 8.2-web-api-controllers

- **Section** : 8.2 — Exposer une Web API par contrôleurs · **Fichier** : `02-web-api-controllers.md`
- **Description** : Web API **complète**. Pas de modèle VB → console basculé sur
  `Microsoft.NET.Sdk.Web` + `Sub Main` (pas d'`Async Main`). `ProduitsController : ControllerBase`
  (`<ApiController>`), CRUD avec `ActionResult(Of T)`, **`CreatedAtRoute`** (route **nommée** pour
  éviter le piège du suffixe `Async`), validation **DataAnnotations** (→ 400 automatique),
  **`Problem Details`** (409 métier), **injection de dépendances** (`IProduitService`),
  **middleware** `ChronoMiddleware` (en-tête `X-Duree-ms`), **OpenAPI 3.1** + **Scalar** (document
  personnalisé par *transformer*, les commentaires XML étant C#-only), **versioning**
  (`Asp.Versioning`, v1/v2), **rate limiting** intégré, et **JWT Bearer** (`<Authorize>` par rôle).
- **Lancer puis vérifier** (cf. section *Serveurs web* ci-dessus ; URL `http://localhost:5180`).
- **Sortie attendue** (vérifiée, extraits — codes HTTP et corps) :
  ```text
  GET  /openapi/v1.json      -> 200 ; openapi=3.1.1 ; info.title="API Catalogue"
  GET  /api/v1/produits      -> 200 ; en-têtes X-Duree-ms=… ; api-supported-versions=1.0, 2.0 ; 3 produits
  GET  /api/v2/produits      -> 200 ; liste triée par prix décroissant (variante v2)
  GET  /api/v1/produits/2    -> 200 ; {"id":2,"nom":"Écran",...}
  GET  /api/v1/produits/999  -> 404
  POST /api/v1/produits      -> 201 ; Location: http://localhost:5180/api/v1.0/Produits/4
  POST (nom="X")             -> 400 ; application/problem+json : "One or more validation errors occurred."
  POST (nom="Clavier")       -> 409 ; application/problem+json : title="Conflit de ressource",
                                       detail="Un produit nommé « Clavier » existe déjà."
  PUT  /api/v1/produits/3    -> 204
  DELETE /api/v1/produits/3 (sans jeton)              -> 401
  DELETE /api/v1/produits/3 (jeton rôle Administrateur)-> 204
  DELETE /api/v1/produits/2 (jeton rôle insuffisant)  -> 403
  GET  /api/v1/produits/ping x5 (policy stricte 3/min) -> 200,200,200,429,429
  ```
- **Comportement vérifié** : OpenAPI **3.1.1** généré avec titre/description du *transformer* ;
  versioning (en-tête `api-supported-versions`, v2 distincte) ; `X-Duree-ms` posé par le middleware ;
  validation et conflit renvoyés en **`Problem Details`** (RFC 9457, guillemets français préservés) ;
  autorisation par rôle (401/403/204) ; **rate limiting** (429 au 4ᵉ appel).
- **Note** : `app.UseHttpsRedirection()` n'est appelé **qu'hors** *Development* (l'exemple tourne en
  HTTP local pour un test sans certificat) ; `/dev/jeton` est un **émetteur de test**, hors cours.

## 8.3-minimal-api-vb

- **Section** : 8.3 — Limites du web en VB.NET · **Fichier** : `03-limites-web-vbnet.md`
- **Description** : la seule partie **compilable** de la section : une **Minimal API en VB**. Elle
  **fonctionne** (tout est .NET) — l'exemple le prouve — mais « à contre-emploi » : projet monté à
  la main, tout dans `Module`/`Sub Main`, `Imports` explicites, liaison de paramètres **par
  réflexion** (pas de Request Delegate Generator en VB → pas de Native AOT). Routes : texte, JSON,
  paramètres de route (`/somme/{a}/{b}`), POST avec corps. (Razor/Blazor : **C# uniquement**, sans
  code.)
- **Sortie attendue** (vérifiée, URL `http://localhost:5181`) :
  ```text
  GET  /            -> Bonjour depuis VB.NET !
  GET  /info        -> {"message":"Minimal API en VB.NET","langage":"VB.NET","aot":false}
  GET  /somme/7/35  -> 42
  POST /echo {texte} -> {"texte":"coucou"}
  GET  /inexistant  -> 404
  ```
- **Comportement vérifié** : la liaison de `a`/`b` depuis l'URL fonctionne (42) ; le corps JSON est
  lié (`/echo`) — le tout par réflexion, conformément au verdict de la section.

## 8.4-temps-reel

- **Section** : 8.4 — Communication temps réel · **Fichier** : `04-temps-reel.md`
- **Description** : un seul programme **héberge et consomme** les trois canaux. **SignalR** : hub
  **fortement typé** (`Hub(Of IChatClient)`), client `HubConnectionBuilder().WithUrl(...)
  .WithAutomaticReconnect()`, `On(Of …)`, `InvokeAsync`, évènement `Reconnecting`. **WebSocket** :
  `ClientWebSocket` brut (connect/send/receive/close) contre un endpoint d'écho serveur
  (`UseWebSockets` + `AcceptWebSocketAsync`). **SSE** : consommation via `SseParser`
  (`System.Net.ServerSentEvents`, **inclus au framework**) avec **énumérateur asynchrone manuel**
  (pas d'`Await For Each`) et motif **« capturer, libérer, relancer »** (`Await` interdit en
  `Finally`, BC36943). L'orchestration applique elle-même ce motif pour arrêter le serveur.
- **Sortie attendue** (vérifiée) :
  ```text
  [SignalR] hub typé sur /chat
    Connecté (état : Connected)
    Message diffusé reçu : Alice : Bonjour à tous
  [WebSocket] écho brut sur /ws
    Envoyé : ping
    Reçu (écho) : ping
  [SSE] flux sur /sse via SseParser (énumérateur manuel)
    SSE reçu : Notification 1 (n°1)
    SSE reçu : Notification 2 (n°2)
    SSE reçu : Notification 3 (n°3)
    -> 3 évènement(s) SSE consommé(s)
  ```
- **Comportement vérifié** : le client SignalR reçoit sa propre diffusion (`Clients.All`) ; le
  WebSocket renvoie l'écho ; les 3 évènements SSE sont consommés par l'énumérateur manuel.
  Confirme aussi que `SseParser` est **dans le framework** sur `net10.0` (build sans paquet).

## 8.5-grpc-graphql

- **Section** : 8.5 — gRPC et GraphQL · **Fichier** : `05-grpc-graphql.md`
- **Description** : **solution hybride** (3 projets). `Catalogue.Grpc` (**C#**) génère le **client
  gRPC** depuis `catalogue.proto` (`Grpc.Tools`) — la génération **échoue en `.vbproj`** (vérifié :
  *« proto compilation is only supported by default in a C# project »*). `ServeurServices` (**C#**)
  héberge le service gRPC avec **JSON transcoding** *et* un serveur **GraphQL HotChocolate**.
  `ClientVb` (**VB**) consomme les **trois voies** : gRPC **natif** (via la lib C# référencée),
  gRPC en **REST** (transcoding, simple `HttpClient` comme § 8.1), et **GraphQL** en HTTP brut
  (`PostAsJsonAsync` + `JsonDocument`).
- **Sortie attendue** (vérifiée, client VB) :
  ```text
  [gRPC natif] via la bibliothèque cliente C# (Catalogue.Grpc)
    gRPC natif : #42 Écran 27 pouces — 220,00 €
  [gRPC -> REST] JSON transcoding, consommé comme une API REST (§ 8.1)
    REST transcodé : #42 Écran 27 pouces — 220,00 €
  [GraphQL] HttpClient + JSON brut (POST query/variables)
    GraphQL : Écran 27 pouces — 220,00 €
  ```
- **Comportement vérifié** : VB appelle un service gRPC **uniquement** via une bibliothèque cliente
  **C#** (la stratégie « isoler en C# ») ; le **même** service est consommable en **REST** sans
  aucun outillage gRPC (transcoding) ; **GraphQL** se consomme nativement en VB. Le serveur écoute
  en **HTTP/2 (5185, gRPC)** et **HTTP/1.1 (5186, REST + GraphQL)**.
- **Notes honnêtes** : le `.proto` du serveur importe deux fichiers *well-known* Google
  (`google/api/http.proto`, `annotations.proto`) **vendus localement** sous `Protos/google/api/`,
  comme l'indique la doc Microsoft du transcoding. La requête GraphQL utilise `$id: String!` (et non
  `ID!` comme l'illustration du cours) car le schéma HotChocolate type l'argument depuis un `string`
  C#. gRPC en clair (h2c) : le client active
  `System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport`.

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build` (les paquets NuGet sont restaurés depuis le cache, y compris le code gRPC généré
et le schéma HotChocolate).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
