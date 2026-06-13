' ============================================================================
'  Section 15.4 : Conteneurisation — le service de fond (BackgroundService)
'  Description : Un BackgroundService hébergé par le Generic Host. Pour la
'                démonstration, il effectue quelques cycles puis demande l'arrêt
'                de l'hôte (StopApplication), afin de se terminer proprement.
'                En conteneur, il tournerait indéfiniment.
'  Fichier source : 04-docker.md
' ============================================================================

Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Public Class Travailleur
    Inherits BackgroundService

    Private ReadOnly _logger As ILogger(Of Travailleur)
    Private ReadOnly _cycleDeVie As IHostApplicationLifetime

    Public Sub New(logger As ILogger(Of Travailleur), cycleDeVie As IHostApplicationLifetime)
        _logger = logger
        _cycleDeVie = cycleDeVie
    End Sub

    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        For i = 1 To 3
            If stoppingToken.IsCancellationRequested Then Exit For
            _logger.LogInformation("Cycle de travail {Numero}", i)
            Await Task.Delay(200, stoppingToken)
        Next
        _logger.LogInformation("Travail terminé.")
        _cycleDeVie.StopApplication()   ' arrête l'hôte (démo) ; en conteneur, on bouclerait
    End Function
End Class
