' ============================================================================
'  Section 8.4 : Communication temps réel — Server-Sent Events (notions)
'  Description : Consommation d'un flux SSE avec SseParser (System.Net.
'                ServerSentEvents, inclus au framework sur net10.0).
'                ⚠️ Double friction VB illustrée ici :
'                  1) pas d'« Await For Each » -> parcours via énumérateur
'                     asynchrone MANUEL (GetAsyncEnumerator + MoveNextAsync) ;
'                  2) Await interdit dans un Finally (BC36943) -> motif
'                     « capturer, libérer, relancer » : DisposeAsync APRÈS le
'                     Try, puis relance éventuelle via ExceptionDispatchInfo
'                     (cf. module 4.6).
'  Fichier source : 04-temps-reel.md
' ============================================================================

Imports System
Imports System.Net.Http
Imports System.Net.ServerSentEvents
Imports System.Runtime.ExceptionServices
Imports System.Text.Json
Imports System.Threading.Tasks

Public Module DemoSse
    Public Async Function ExecuterAsync(baseUrl As String) As Task
        Using client As New HttpClient()
            ' Le Stream SSE obtenu via HttpClient (en text/event-stream).
            Dim flux = Await client.GetStreamAsync($"{baseUrl}/sse")

            Dim parseur = SseParser.Create(flux)
            Dim enumerateur = parseur.EnumerateAsync().GetAsyncEnumerator()
            Dim capture As ExceptionDispatchInfo = Nothing
            Dim total = 0

            Try
                While Await enumerateur.MoveNextAsync()
                    Dim evenement = enumerateur.Current
                    ' evenement.Data est une chaîne ; on la désérialise (JSON).
                    Dim notif = JsonSerializer.Deserialize(Of Notification)(evenement.Data)
                    Console.WriteLine($"  SSE reçu : {notif.Titre} (n°{notif.Numero})")
                    total += 1
                End While
            Catch ex As Exception
                capture = ExceptionDispatchInfo.Capture(ex)
            End Try

            Await enumerateur.DisposeAsync()   ' hors du Finally : Await autorisé
            capture?.Throw()                   ' relance éventuelle, pile préservée

            Console.WriteLine($"  -> {total} évènement(s) SSE consommé(s)")
        End Using
    End Function
End Module
