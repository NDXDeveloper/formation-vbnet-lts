' ============================================================================
'  Section 4.8 : Services en arrière-plan : Generic Host et BackgroundService
'  Description : Le motif « portée par cycle » du cours : un BackgroundService
'                est un SINGLETON ; pour consommer un service SCOPED (ex.
'                DbContext EF Core), il faut créer une portée à chaque cycle
'                (CreateScope). On le démontre ici avec un service scoped
'                porteur d'un identifiant unique : une nouvelle instance est
'                résolue à chaque tour (Ids distincts).
'  Fichier source : 08-background-services.md
' ============================================================================

Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

''' <summary>Service à portée « scoped » : tient lieu de DbContext de démonstration.</summary>
Public Class UniteDeTravail
    Public ReadOnly Property Id As Guid = Guid.NewGuid()
End Class

Public Class ServiceAvecPortee
    Inherits BackgroundService

    Private ReadOnly _fournisseur As IServiceProvider
    Private ReadOnly _journal As ILogger(Of ServiceAvecPortee)
    Private ReadOnly _dureeDeVie As IHostApplicationLifetime
    Private _cycles As Integer = 0

    Public Sub New(fournisseur As IServiceProvider,
                   journal As ILogger(Of ServiceAvecPortee),
                   dureeDeVie As IHostApplicationLifetime)
        _fournisseur = fournisseur
        _journal = journal
        _dureeDeVie = dureeDeVie
    End Sub

    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        Dim idsVus As New List(Of Guid)
        Try
            Do While Not stoppingToken.IsCancellationRequested
                Using portee = _fournisseur.CreateScope()
                    Dim unite = portee.ServiceProvider.GetRequiredService(Of UniteDeTravail)()
                    idsVus.Add(unite.Id)
                    _journal.LogInformation("Cycle {Cycle} : UniteDeTravail {Id}", _cycles + 1, unite.Id.ToString().Substring(0, 8))
                End Using                              ' portée libérée à chaque tour

                _cycles += 1
                If _cycles >= 3 Then
                    Dim tousDistincts = idsVus.Distinct().Count() = idsVus.Count
                    _journal.LogInformation("Instances scoped toutes distinctes : {Distinctes}", tousDistincts)
                    _dureeDeVie.StopApplication()
                    Return
                End If
                Await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken)
            Loop
        Catch ex As OperationCanceledException
        End Try
    End Function
End Class
