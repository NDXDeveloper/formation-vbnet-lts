🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 8.4 Communication temps réel

> **Module 8 — Consommer et exposer des services** · SignalR (hub + client VB.NET) · WebSockets (`ClientWebSocket`) · Server-Sent Events — notions

La communication temps réel consiste à **pousser** des données vers les clients sans qu'ils aient à interroger le serveur en boucle. .NET offre trois outils pour cela, et VB.NET se place très bien sur ce terrain : **SignalR est pleinement réaliste** (le hub côté serveur *et* le client), tandis que **WebSockets** et **Server-Sent Events** sont présentés ici comme des **notions**, centrées sur la consommation.

---

## 🧭 Trois options, un choix

| Technologie | Direction | Niveau | Idéal pour | En VB.NET |
|-------------|-----------|--------|------------|-----------|
| **SignalR** | Bidirectionnel | Élevé (abstraction) | Chat, collaboration, tableaux de bord, notifications | ✅ Hub **et** client |
| **WebSockets** (`ClientWebSocket`) | Bidirectionnel | Bas (brut) | Contrôle fin, dialoguer avec un serveur WebSocket non-SignalR | ✅ Consommation (notions) |
| **Server-Sent Events** | Unidirectionnel (serveur → client) | Bas / moyen | Flux de notifications, *feeds*, jetons d'IA en *streaming* | ✅ Consommation (notions) |

Règle pratique : dans une application .NET, **SignalR est le choix par défaut** pour le temps réel — il masque le transport et gère la reconnexion. On descend vers WebSockets ou SSE bruts quand on a une raison précise (contrôle bas niveau, ou un serveur tiers imposant le protocole).

---

## 🔵 SignalR — l'option de référence (réaliste en VB.NET) ✅

SignalR est la bibliothèque temps réel d'ASP.NET Core. Elle gère pour vous le **transport** (WebSockets, puis repli sur Server-Sent Events ou *long-polling*), les **groupes** de clients et la **reconnexion**. Deux raisons en font un excellent choix en VB.NET : le **hub** est une simple classe (idiomatique en VB), et le **client** est une bibliothèque .NET (pure consommation).

### Le hub (côté serveur) — une simple classe

Un hub hérite de `Hub` et expose des méthodes que les clients appellent ; il pousse vers les clients via `Clients`.

```vb
Imports Microsoft.AspNetCore.SignalR

Public Class ChatHub
    Inherits Hub

    ' Méthode appelable par les clients
    Public Async Function EnvoyerMessage(utilisateur As String, message As String) As Task
        ' Diffuse à tous les clients connectés
        Await Clients.All.SendAsync("RecevoirMessage", utilisateur, message)
    End Function

    Public Overrides Async Function OnConnectedAsync() As Task
        Await Clients.All.SendAsync("Systeme", $"{Context.ConnectionId} a rejoint.")
        Await MyBase.OnConnectedAsync()
    End Function
End Class
```

Le hub fait partie du framework partagé ASP.NET Core : aucun paquet supplémentaire n'est nécessaire (avec le SDK `Microsoft.NET.Sdk.Web` du § 8.2). On l'enregistre et on le route dans `Program.vb` :

```vb
builder.Services.AddSignalR()
' …
app.MapHub(Of ChatHub)("/chat")
```

#### Variante typée (recommandée)

Plutôt que des noms de méthodes en chaînes de caractères, un **hub fortement typé** s'appuie sur une interface décrivant les méthodes du client :

```vb
Public Interface IChatClient
    Function RecevoirMessage(utilisateur As String, message As String) As Task
End Interface

Public Class ChatHub
    Inherits Hub(Of IChatClient)

    Public Async Function EnvoyerMessage(utilisateur As String, message As String) As Task
        ' Appel typé, vérifié à la compilation
        Await Clients.All.RecevoirMessage(utilisateur, message)
    End Function
End Class
```

### Le client VB.NET

Côté client (application console, WinForms, WPF…), on installe le paquet `Microsoft.AspNetCore.SignalR.Client`, puis on construit une connexion :

```vb
Imports Microsoft.AspNetCore.SignalR.Client

Dim connexion = New HubConnectionBuilder() _
    .WithUrl("https://localhost:5001/chat") _
    .WithAutomaticReconnect() _
    .Build()

' Réception : on enregistre un gestionnaire pour chaque message poussé par le hub
connexion.On(Of String, String)("RecevoirMessage",
    Sub(utilisateur, message)
        Console.WriteLine($"{utilisateur} : {message}")
    End Sub)

' Suivi du cycle de vie (la reconnexion automatique est active)
AddHandler connexion.Reconnecting, Function(erreur)
        Console.WriteLine("Reconnexion en cours…")
        Return Task.CompletedTask
    End Function

' Démarrage de la connexion
Await connexion.StartAsync()

' Envoi vers le hub
Await connexion.InvokeAsync("EnvoyerMessage", "Alice", "Bonjour à tous")
```

Tout fonctionne en VB comme dans la documentation : `On(...)` pour recevoir, `InvokeAsync` / `SendAsync` pour appeler, et les événements `Reconnecting` / `Reconnected` / `Closed` pour le cycle de vie. Les lambdas VB (`Sub(...)`, `Function(...)`) servent de gestionnaires sans difficulté.

> 💡 SignalR illustre parfaitement le périmètre VB.NET : la partie serveur est une **classe** (terrain naturel de VB), la partie client une **bibliothèque consommée**. Aucune des deux ne sort du cœur de VB.NET.

---

## 🟢 WebSockets (`ClientWebSocket`) — notions

Quand on a besoin d'un canal **bidirectionnel bas niveau** — par exemple pour dialoguer avec un serveur WebSocket qui n'utilise pas SignalR — `System.Net.WebSockets.ClientWebSocket` est le client brut. On gère soi-même les trames et les tampons.

```vb
Imports System.Net.WebSockets
Imports System.Text

Dim socket As New ClientWebSocket()
Await socket.ConnectAsync(New Uri("wss://exemple.com/flux"), CancellationToken.None)

' Envoi d'un message texte
Dim donnees = Encoding.UTF8.GetBytes("ping")
Await socket.SendAsync(donnees, WebSocketMessageType.Text,
                       endOfMessage:=True, CancellationToken.None)

' Réception d'un message
Dim tampon(4095) As Byte
Dim resultat = Await socket.ReceiveAsync(tampon, CancellationToken.None)
Dim message = Encoding.UTF8.GetString(tampon, 0, resultat.Count)

' Fermeture propre
Await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Terminé", CancellationToken.None)
```

Côté serveur, ASP.NET Core sait aussi accepter des WebSockets bruts (`app.UseWebSockets()` puis `HttpContext.WebSockets.AcceptWebSocketAsync()`), utilisable depuis un contrôleur VB. Mais dès que le besoin dépasse l'échange de trames simples, **SignalR évite de réimplémenter à la main la reconnexion, le découpage des messages et les replis de transport**.

---

## 🟡 Server-Sent Events — notions (consommation)

Les **Server-Sent Events** (SSE) sont un flux **unidirectionnel** (serveur → client) de texte ou de JSON, transporté sur une simple requête HTTP longue (`text/event-stream`). C'est idéal pour des **notifications**, des *feeds* en quasi temps réel, ou le *streaming* de jetons d'un modèle d'IA — sans la complexité d'un canal bidirectionnel.

Depuis .NET 9 (et à maturité en .NET 10), la BCL fournit `System.Net.ServerSentEvents` avec le type **`SseParser`** pour consommer un flux SSE — **inclus dans le framework** sur `net10.0`, sans paquet à ajouter (vérifié ; le paquet NuGet du même nom n'existe que pour les anciennes cibles).

### ⚠️ La friction VB : pas de `Await For Each`

`SseParser.Create(flux).EnumerateAsync()` renvoie un `IAsyncEnumerable(Of SseItem(Of String))`. En C#, on le parcourt avec `await foreach`. **VB ne dispose pas de cette syntaxe** : il faut consommer le flux asynchrone via l'**énumérateur asynchrone manuel** (cf. module 4.6 — VB *consomme* `IAsyncEnumerable`, mais sans sucre syntaxique). Et attention au second piège du même mécanisme : `Await` étant interdit dans un `Finally` (BC36943), la libération asynchrone de l'énumérateur se fait **après** le `Try`, selon le motif « capturer, libérer, relancer » du module 4.6 :

```vb
Imports System.Net.ServerSentEvents
Imports System.Runtime.ExceptionServices
Imports System.Text.Json

' flux : le Stream SSE obtenu via HttpClient (en text/event-stream)
Dim flux = Await client.GetStreamAsync("https://exemple.com/sse")

Dim parseur = SseParser.Create(flux)
Dim enumerateur = parseur.EnumerateAsync().GetAsyncEnumerator()
Dim capture As ExceptionDispatchInfo = Nothing
Try
    While Await enumerateur.MoveNextAsync()
        Dim evenement = enumerateur.Current
        ' evenement.Data est une chaîne ; on la désérialise si c'est du JSON
        Dim notif = JsonSerializer.Deserialize(Of Notification)(evenement.Data)
        Console.WriteLine(notif.Titre)
    End While
Catch ex As Exception
    capture = ExceptionDispatchInfo.Capture(ex)
End Try

Await enumerateur.DisposeAsync()   ' hors du Finally : Await autorisé
capture?.Throw()                   ' relance éventuelle, pile préservée
```

Deux remarques importantes :

- **Pas (encore) de client SSE « haut niveau »** : .NET fournit le *parseur*, mais aucun client à la `EventSource` gérant la **reconnexion** automatiquement. Cette logique (réessais, reprise sur `Last-Event-ID`) reste à votre charge, au-dessus de `HttpClient` + `SseParser`.
- **Côté serveur**, .NET 10 facilite aussi l'**émission** de SSE (via `TypedResults`/`Results` et `SseFormatter`), exposable depuis un contrôleur VB — mais le périmètre de cette section reste la **consommation**.

---

## 🆕 Ce que .NET 10 apporte

- **SSE** : le parseur `System.Net.ServerSentEvents` est arrivé à maturité (consommation), et le côté serveur s'est doté de `SseFormatter` et de résultats SSE typés.
- **SignalR** reste stable et pleinement utilisable en VB.NET (hub et client), sans changement de modèle.

---

## ⚠️ La friction VB à retenir

| Sujet | État en VB.NET | À savoir |
|-------|----------------|----------|
| SignalR (hub + client) | ✅ Réaliste | Hub = classe, client = bibliothèque ; rien hors périmètre |
| `ClientWebSocket` | ✅ Consommation | API bas niveau, identique à C# |
| Consommer SSE (`SseParser`) | ✅ Consommation | **Pas de `Await For Each`** ni d'`Await` dans un `Finally` → énumérateur manuel + motif « capturer, libérer, relancer » (module 4.6) |
| Reconnexion SSE | ⚠️ À implémenter | Aucun client `EventSource` haut niveau dans .NET |

Aucune limite de fond ici : les trois technologies sont consommables en VB.NET. Les seules véritables aspérités sont les deux jumelles du flux asynchrone — pas de `Await For Each`, et pas d'`Await` dans un `Finally` (BC36943) — toutes deux réglées par le même motif : parcours manuel d'énumérateur, puis « capturer, libérer, relancer » (module 4.6). Un réflexe à acquérir pour tout flux asynchrone.

---

## 🧭 À retenir

- Pour le temps réel en .NET, **SignalR est le choix par défaut** et il est **pleinement réaliste en VB.NET** : le hub est une classe, le client une bibliothèque consommée, avec transport et reconnexion gérés.
- **WebSockets** (`ClientWebSocket`) offrent un canal bidirectionnel **bas niveau**, utile pour un contrôle fin ou un serveur non-SignalR — sinon, préférer SignalR.
- **Server-Sent Events** conviennent au *push* **unidirectionnel** (notifications, *feeds*, jetons d'IA) ; on les consomme avec `SseParser` (.NET 9/10).
- Les frictions VB notables sont les deux pièges jumeaux du flux asynchrone — pas de `Await For Each`, pas d'`Await` dans un `Finally` — réglés par l'**énumérateur manuel** et le motif « capturer, libérer, relancer » (module 4.6) ; la **reconnexion SSE**, elle, reste à coder à la main.

---

⬅️ [8.3 — Limites du web en VB.NET](03-limites-web-vbnet.md) · ➡️ [8.5 — gRPC et GraphQL](05-grpc-graphql.md)

⏭️ [gRPC et GraphQL ⚠️ — outillage orienté C#, à déléguer](/08-services-web/05-grpc-graphql.md)
