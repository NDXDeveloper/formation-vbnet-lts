' ============================================================================
'  Section 17.9 : Chat — consommation de IChatClient
'  Description : Reprise du code de la section : une dépendance IChatClient injectée,
'                une fonction Async, une liste de ChatMessage typés par rôle, un Await.
'                Du VB.NET ordinaire — la consommation est la zone de confort de VB.
'  Fichier source : 09-consommer-ia.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Microsoft.Extensions.AI

''' <summary>Assistant conversationnel s'appuyant sur l'abstraction IChatClient.</summary>
Public Class AssistantClient

    Private ReadOnly _chat As IChatClient

    Public Sub New(chat As IChatClient)
        _chat = chat
    End Sub

    ''' <summary>Pose une question et renvoie la réponse textuelle du modèle.</summary>
    Public Async Function PoserAsync(question As String) As Task(Of String)
        Dim messages As New List(Of ChatMessage) From {
            New ChatMessage(ChatRole.System, "Tu réponds en français, de façon concise."),
            New ChatMessage(ChatRole.User, question)
        }
        Dim reponse = Await _chat.GetResponseAsync(messages)
        Return reponse.Text
    End Function

End Class
