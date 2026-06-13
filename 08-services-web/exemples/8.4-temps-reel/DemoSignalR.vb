' ============================================================================
'  Section 8.4 : Communication temps réel — client SignalR (VB.NET)
'  Description : Le client SignalR. On construit la connexion avec
'                HubConnectionBuilder().WithUrl(...).WithAutomaticReconnect(),
'                on enregistre un gestionnaire de réception avec On(Of …), on
'                suit le cycle de vie via l'évènement Reconnecting, puis on
'                StartAsync et InvokeAsync. Les lambdas VB (Sub/Function)
'                servent de gestionnaires sans difficulté. Le client reçoit sa
'                propre diffusion (Clients.All) : on l'attend pour vérifier.
'  Fichier source : 04-temps-reel.md
' ============================================================================

Imports System
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.SignalR.Client

Public Module DemoSignalR
    Public Async Function ExecuterAsync(baseUrl As String) As Task
        Dim connexion = New HubConnectionBuilder() _
            .WithUrl($"{baseUrl}/chat") _
            .WithAutomaticReconnect() _
            .Build()

        ' Réception : un gestionnaire par message poussé par le hub.
        Dim recu As New TaskCompletionSource(Of String)(TaskCreationOptions.RunContinuationsAsynchronously)
        connexion.On(Of String, String)("RecevoirMessage",
            Sub(utilisateur, message)
                recu.TrySetResult($"{utilisateur} : {message}")
            End Sub)

        ' Suivi du cycle de vie (reconnexion automatique active).
        AddHandler connexion.Reconnecting,
            Function(erreur As Exception)
                Console.WriteLine("  Reconnexion en cours…")
                Return Task.CompletedTask
            End Function

        Await connexion.StartAsync()
        Console.WriteLine($"  Connecté (état : {connexion.State})")

        ' Envoi vers le hub.
        Await connexion.InvokeAsync("EnvoyerMessage", "Alice", "Bonjour à tous")

        ' Attente de la diffusion (timeout de sécurité).
        Dim gagnant = Await Task.WhenAny(recu.Task, Task.Delay(5000))
        If gagnant Is recu.Task Then
            Console.WriteLine($"  Message diffusé reçu : {recu.Task.Result}")
        Else
            Console.WriteLine("  (timeout : aucune diffusion reçue)")
        End If

        Await connexion.DisposeAsync()
    End Function
End Module
