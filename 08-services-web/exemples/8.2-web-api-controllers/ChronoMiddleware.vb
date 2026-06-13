' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Middleware personnalisé (classe exposant InvokeAsync). Mesure
'                la durée de chaque requête et la journalise ; ses dépendances
'                « par requête » (ici ILogger) sont injectées en paramètres
'                d'InvokeAsync. Branché via app.UseMiddleware(Of ChronoMiddleware).
'                Ajoute aussi un en-tête X-Duree-ms, observable côté client.
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System.Diagnostics
Imports System.Threading.Tasks
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
        ' L'en-tête doit être posé AVANT que la réponse ne commence à être envoyée.
        contexte.Response.OnStarting(
            Function()
                chrono.Stop()
                contexte.Response.Headers("X-Duree-ms") = chrono.ElapsedMilliseconds.ToString()
                Return Task.CompletedTask
            End Function)

        Await _suivant(contexte)                 ' passe la main au middleware suivant

        logger.LogInformation("{Methode} {Chemin} en {Duree} ms",
            contexte.Request.Method, contexte.Request.Path, chrono.ElapsedMilliseconds)
    End Function
End Class
