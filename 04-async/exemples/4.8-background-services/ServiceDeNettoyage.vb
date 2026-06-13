' ============================================================================
'  Section 4.8 : Services en arrière-plan : Generic Host et BackgroundService
'  Description : Le BackgroundService exact du cours (boucle pilotée par
'                stoppingToken, journalisation injectée par constructeur,
'                Task.Delay annulable, OperationCanceledException = arrêt
'                normal). Adaptation pour la démo : après 3 cycles, le service
'                demande l'arrêt de l'application (StopApplication) afin que
'                l'exemple se TERMINE et reste vérifiable (un vrai service
'                tournerait indéfiniment).
'  Fichier source : 08-background-services.md
' ============================================================================

Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Public Class ServiceDeNettoyage
    Inherits BackgroundService

    Private ReadOnly _journal As ILogger(Of ServiceDeNettoyage)
    Private ReadOnly _dureeDeVie As IHostApplicationLifetime
    Private _cycles As Integer = 0

    Public Sub New(journal As ILogger(Of ServiceDeNettoyage),
                   dureeDeVie As IHostApplicationLifetime)
        _journal = journal                          ' injection par constructeur
        _dureeDeVie = dureeDeVie
    End Sub

    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        Try
            Do While Not stoppingToken.IsCancellationRequested
                _cycles += 1
                _journal.LogInformation("Cycle de nettoyage n° {Cycle}", _cycles)
                Await NettoyerAsync(stoppingToken)

                If _cycles >= 3 Then
                    _journal.LogInformation("3 cycles effectués : demande d'arrêt de l'application.")
                    _dureeDeVie.StopApplication()    ' termine la démo proprement
                    Return
                End If

                Await Task.Delay(TimeSpan.FromMilliseconds(150), stoppingToken)
            Loop
        Catch ex As OperationCanceledException
            ' Arrêt demandé : sortie normale
        End Try
    End Function

    Private Async Function NettoyerAsync(ct As CancellationToken) As Task
        Await Task.Delay(50, ct)                    ' simule un travail de fond
    End Function
End Class
