' ============================================================================
'  Section 8.4 : Communication temps réel — orchestration
'  Description : Point d'entrée. Démarre le serveur in-process (StartAsync, non
'                bloquant — contrairement à app.Run), puis enchaîne les trois
'                démonstrations clientes, et arrête proprement. NB : Await étant
'                interdit dans un Finally (BC36943, le piège même du § 8.4), on
'                arrête le serveur via le motif « capturer, libérer, relancer ».
'  Fichier source : 04-temps-reel.md
' ============================================================================

Imports System
Imports System.Runtime.ExceptionServices
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Hosting

Module Program

    Function Main(args As String()) As Integer
        Return MainAsync().GetAwaiter().GetResult()
    End Function

    Async Function MainAsync() As Task(Of Integer)
        Console.OutputEncoding = Text.Encoding.UTF8
        Dim url = "http://127.0.0.1:5182"

        Console.WriteLine("=== 8.4 Communication temps réel (SignalR / WebSocket / SSE) ===")
        Console.WriteLine()

        Dim app = Serveur.Construire(url)
        Await app.StartAsync()
        Console.WriteLine($"Serveur in-process démarré sur {url}")
        Console.WriteLine()

        Dim capture As ExceptionDispatchInfo = Nothing
        Try
            Console.WriteLine("[SignalR] hub typé sur /chat")
            Await DemoSignalR.ExecuterAsync(url)
            Console.WriteLine()

            Console.WriteLine("[WebSocket] écho brut sur /ws")
            Await DemoWebSockets.ExecuterAsync(url)
            Console.WriteLine()

            Console.WriteLine("[SSE] flux sur /sse via SseParser (énumérateur manuel)")
            Await DemoSse.ExecuterAsync(url)
        Catch ex As Exception
            capture = ExceptionDispatchInfo.Capture(ex)
        End Try

        Await app.StopAsync()       ' hors du Finally : Await autorisé
        capture?.Throw()

        Console.WriteLine()
        Console.WriteLine("Terminé.")
        Return 0
    End Function

End Module
