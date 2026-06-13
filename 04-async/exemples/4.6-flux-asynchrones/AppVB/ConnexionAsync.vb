' ============================================================================
'  Section 4.6 : Consommer IAsyncEnumerable / flux asynchrones ; ValueTask
'  Description : Les deux formes EXACTES du cours pour implémenter
'                IAsyncDisposable.DisposeAsync en VB — qui renvoie un
'                ValueTask alors que VB ne sait pas écrire « Async ... As
'                ValueTask » (BC36945). On renvoie donc le ValueTask À LA
'                MAIN : soit déjà terminé (ValueTask.CompletedTask), soit en
'                enveloppant une Async...As Task (New ValueTask(...)).
'  Fichier source : 06-async-streams.md
' ============================================================================

Imports System
Imports System.Threading.Tasks

''' <summary>Libération SYNCHRONE emballée dans un ValueTask déjà terminé.</summary>
Public Class RessourceSimple
    Implements IAsyncDisposable

    Public Function DisposeAsync() As ValueTask Implements IAsyncDisposable.DisposeAsync
        Console.WriteLine("  RessourceSimple : libération synchrone (ValueTask.CompletedTask)")
        Return ValueTask.CompletedTask
    End Function
End Class

''' <summary>Libération réellement ASYNCHRONE : on enveloppe une Async...As Task.</summary>
Public Class ConnexionAsync
    Implements IAsyncDisposable

    Public Function DisposeAsync() As ValueTask Implements IAsyncDisposable.DisposeAsync
        Return New ValueTask(FermerConnexionAsync())
    End Function

    Private Async Function FermerConnexionAsync() As Task
        Await Task.Delay(20)                ' simule une fermeture réseau asynchrone
        Console.WriteLine("  ConnexionAsync : fermeture asynchrone terminée")
    End Function
End Class
