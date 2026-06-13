' ============================================================================
'  Section 8.4 : Communication temps réel — WebSockets (notions)
'  Description : Le client WebSocket BAS NIVEAU (System.Net.WebSockets.
'                ClientWebSocket) : on gère soi-même trames et tampons.
'                Connexion, envoi d'un texte (« ping »), réception de l'écho,
'                fermeture propre. API identique à C#. Au-delà de l'échange de
'                trames simples, SignalR évite de réimplémenter reconnexion,
'                découpage et replis de transport.
'  Fichier source : 04-temps-reel.md
' ============================================================================

Imports System
Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Public Module DemoWebSockets
    Public Async Function ExecuterAsync(baseUrl As String) As Task
        Dim uriWs As New Uri(baseUrl.Replace("http://", "ws://") & "/ws")

        Using socket As New ClientWebSocket()
            Await socket.ConnectAsync(uriWs, CancellationToken.None)
            Console.WriteLine($"  Connecté : {uriWs}")

            ' Envoi d'un message texte.
            Dim donnees = Encoding.UTF8.GetBytes("ping")
            Await socket.SendAsync(donnees, WebSocketMessageType.Text,
                                   endOfMessage:=True, CancellationToken.None)
            Console.WriteLine("  Envoyé : ping")

            ' Réception d'un message (l'écho du serveur).
            Dim tampon(4095) As Byte
            Dim resultat = Await socket.ReceiveAsync(tampon, CancellationToken.None)
            Dim message = Encoding.UTF8.GetString(tampon, 0, resultat.Count)
            Console.WriteLine($"  Reçu (écho) : {message}")

            ' Fermeture propre.
            Await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Terminé", CancellationToken.None)
        End Using
    End Function
End Module
