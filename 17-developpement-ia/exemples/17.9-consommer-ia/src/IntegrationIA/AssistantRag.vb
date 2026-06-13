' ============================================================================
'  Section 17.9 : RAG basique — injecter le contexte récupéré dans le chat
'  Description : Enchaîne les briques : on RÉCUPÈRE le passage pertinent dans la base
'                vectorisée, puis on l'INJECTE dans le message système avant d'appeler
'                le IChatClient. Tous les composants se consomment depuis VB ; ici le
'                client de chat est le faux déterministe, mais le code d'intégration
'                serait identique avec un vrai client (Azure OpenAI / OpenAI).
'  Fichier source : 09-consommer-ia.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Microsoft.Extensions.AI

''' <summary>Assistant RAG : récupère un contexte puis interroge le modèle de chat.</summary>
Public Class AssistantRag

    Private ReadOnly _connaissances As BaseConnaissancesRag
    Private ReadOnly _chat As IChatClient

    Public Sub New(connaissances As BaseConnaissancesRag, chat As IChatClient)
        _connaissances = connaissances
        _chat = chat
    End Sub

    ''' <summary>Récupère le passage pertinent, l'injecte en contexte, et renvoie la réponse.</summary>
    Public Async Function RepondreAsync(question As String) As Task(Of String)
        Dim contexte As String = _connaissances.Recuperer(question)
        Dim consigne As String = $"Réponds uniquement à partir de ce contexte : {contexte}"

        Dim messages As New List(Of ChatMessage) From {
            New ChatMessage(ChatRole.System, consigne),
            New ChatMessage(ChatRole.User, question)
        }
        Dim reponse = Await _chat.GetResponseAsync(messages)
        Return reponse.Text
    End Function

End Class
