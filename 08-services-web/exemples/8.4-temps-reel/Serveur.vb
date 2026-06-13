' ============================================================================
'  Section 8.4 : Communication temps réel — serveur in-process
'  Description : Construit l'application ASP.NET Core qui expose les trois
'                canaux : le hub SignalR (MapHub), un endpoint WebSocket BRUT
'                d'écho (UseWebSockets + AcceptWebSocketAsync, depuis un
'                gestionnaire VB) et un flux Server-Sent Events (text/event-
'                stream). Sert de « serveur distant » aux clients du même
'                programme, pour que tout soit vérifiable hors ligne.
'  Fichier source : 04-temps-reel.md
' ============================================================================

Imports System
Imports System.Net.WebSockets
Imports System.Text.Json
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging

Public Module Serveur

    Public Function Construire(url As String) As WebApplication
        Dim builder = WebApplication.CreateBuilder()
        builder.WebHost.UseUrls(url)
        builder.Logging.SetMinimumLevel(LogLevel.Warning)   ' console propre
        builder.Services.AddSignalR()

        Dim app = builder.Build()
        app.UseWebSockets()
        app.MapHub(Of ChatHub)("/chat")
        app.Map("/ws", AddressOf GestionnaireWebSocket)
        app.MapGet("/sse", AddressOf GestionnaireSse)
        Return app
    End Function

    ' --- Endpoint WebSocket brut : écho de chaque trame reçue ---
    Friend Async Function GestionnaireWebSocket(contexte As HttpContext) As Task
        If Not contexte.WebSockets.IsWebSocketRequest Then
            contexte.Response.StatusCode = 400
            Return
        End If

        Using socket = Await contexte.WebSockets.AcceptWebSocketAsync()
            Dim tampon(4095) As Byte
            Dim segment As New ArraySegment(Of Byte)(tampon)
            Dim resultat = Await socket.ReceiveAsync(segment, CancellationToken.None)
            While Not resultat.CloseStatus.HasValue
                Await socket.SendAsync(New ArraySegment(Of Byte)(tampon, 0, resultat.Count),
                                       resultat.MessageType, resultat.EndOfMessage, CancellationToken.None)
                resultat = Await socket.ReceiveAsync(segment, CancellationToken.None)
            End While
            Await socket.CloseAsync(resultat.CloseStatus.Value, resultat.CloseStatusDescription, CancellationToken.None)
        End Using
    End Function

    ' --- Endpoint Server-Sent Events : émet 3 notifications puis se termine ---
    Friend Async Function GestionnaireSse(contexte As HttpContext) As Task
        contexte.Response.ContentType = "text/event-stream"
        For i = 1 To 3
            Dim notif As New Notification With {.Titre = $"Notification {i}", .Numero = i}
            Dim charge = JsonSerializer.Serialize(notif)
            Await contexte.Response.WriteAsync("data: " & charge & vbLf & vbLf)
            Await contexte.Response.Body.FlushAsync()
        Next
    End Function

End Module
