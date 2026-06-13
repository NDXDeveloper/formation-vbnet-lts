' ============================================================================
'  Section 17.9 : vérification de la plomberie (chat + RAG)
'  Description : Trois tests déterministes : (1) AssistantClient achemine la question
'                au client et renvoie sa réponse ; (2) le RAG récupère le document
'                pertinent par similarité ; (3) le RAG injecte bien le contexte
'                récupéré dans le prompt envoyé au modèle.
'  Fichier source : 09-consommer-ia.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.Extensions.AI
Imports Xunit

Public Class TestsIntegration

    <Fact>
    Public Async Function AssistantClient_AcheminLaQuestion_EtRendLaReponse() As Task
        ' Le faux renvoie une réponse calculée à partir du dernier message utilisateur.
        Dim faux As New ClientChatFaux(Function(messages) "Réponse à : " & messages.Last(Function(m) m.Role = ChatRole.User).Text)
        Dim assistant As New AssistantClient(faux)

        Dim reponse = Await assistant.PoserAsync("Bonjour")

        Assert.Equal("Réponse à : Bonjour", reponse)
    End Function

    <Fact>
    Public Sub Rag_Recupere_LeDocumentPertinent()
        Dim connaissances As New BaseConnaissancesRag()
        connaissances.Indexer("Le chat dort sur le canapé du salon.")
        connaissances.Indexer("La voiture roule vite sur l'autoroute.")

        Dim document = connaissances.Recuperer("Où dort le chat ?")

        Assert.Equal(2, connaissances.Nombre)
        Assert.Contains("chat", document)
    End Sub

    <Fact>
    Public Async Function Rag_InjecteLeContexteRecupere_DansLePrompt() As Task
        Dim connaissances As New BaseConnaissancesRag()
        connaissances.Indexer("Le chat dort sur le canapé du salon.")
        connaissances.Indexer("La voiture roule vite sur l'autoroute.")

        ' Le faux renvoie le message SYSTÈME : prouve que le contexte récupéré y est injecté.
        Dim faux As New ClientChatFaux(Function(messages) messages.First(Function(m) m.Role = ChatRole.System).Text)
        Dim assistant As New AssistantRag(connaissances, faux)

        Dim reponse = Await assistant.RepondreAsync("Où dort le chat ?")

        Assert.Contains("canapé", reponse)
    End Function

End Class
