' ============================================================================
'  Section 4.8 : Services en arrière-plan : Generic Host et BackgroundService
'  Description : Le travail périodique du cours avec PeriodicTimer (.NET 6+),
'                cadence FIXE. WaitForNextTickAsync renvoie un
'                ValueTask(Of Boolean) — que VB consomme sans difficulté
'                (Await direct). Limité à 3 tics pour la démo.
'  Fichier source : 08-background-services.md
' ============================================================================

Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Public Class ServicePeriodique
    Inherits BackgroundService

    Private ReadOnly _journal As ILogger(Of ServicePeriodique)
    Private _tics As Integer = 0

    Public Sub New(journal As ILogger(Of ServicePeriodique))
        _journal = journal
    End Sub

    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        Using minuteur As New PeriodicTimer(TimeSpan.FromMilliseconds(100))
            Try
                Do While Await minuteur.WaitForNextTickAsync(stoppingToken)
                    _tics += 1
                    _journal.LogInformation("PeriodicTimer : tic n° {Tic}", _tics)
                    If _tics >= 3 Then Exit Do      ' borne la démo
                Loop
            Catch ex As OperationCanceledException
            End Try
        End Using
    End Function
End Class
