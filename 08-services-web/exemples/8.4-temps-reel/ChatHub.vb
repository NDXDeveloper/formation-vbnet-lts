' ============================================================================
'  Section 8.4 : Communication temps réel — SignalR (hub typé)
'  Description : Le hub côté serveur. Variante FORTEMENT TYPÉE (recommandée) :
'                au lieu de noms de méthodes en chaînes, le hub hérite de
'                Hub(Of IChatClient) et appelle des méthodes du client vérifiées
'                à la compilation (Clients.All.RecevoirMessage(...)). Un hub est
'                une simple classe : terrain naturel de VB.NET.
'  Fichier source : 04-temps-reel.md
' ============================================================================

Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.SignalR

' Décrit les méthodes que le SERVEUR peut appeler sur le CLIENT.
Public Interface IChatClient
    Function RecevoirMessage(utilisateur As String, message As String) As Task
End Interface

Public Class ChatHub
    Inherits Hub(Of IChatClient)

    ' Méthode appelable par les clients (via InvokeAsync/SendAsync).
    Public Async Function EnvoyerMessage(utilisateur As String, message As String) As Task
        ' Appel typé, vérifié à la compilation, diffusé à tous les clients.
        Await Clients.All.RecevoirMessage(utilisateur, message)
    End Function
End Class
