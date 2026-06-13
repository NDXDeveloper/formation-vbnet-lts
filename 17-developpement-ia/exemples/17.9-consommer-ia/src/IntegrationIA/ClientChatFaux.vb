' ============================================================================
'  Section 17.9 : Faux IChatClient déterministe (test de la plomberie)
'  Description : La section le rappelle : un appel de modèle est NON DÉTERMINISTE,
'                on ne peut donc pas asserter une sortie exacte. On teste la
'                PLOMBERIE de l'intégration avec un faux client dont la réponse est
'                calculée, de façon déterministe, à partir des messages reçus.
'                Implémente IChatClient ; seul GetResponseAsync est utile ici
'                (le flux en streaming n'est pas requis pour la démonstration).
'  Fichier source : 09-consommer-ia.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Extensions.AI

''' <summary>Implémentation de test de IChatClient : réponse déterministe, sans réseau.</summary>
Public NotInheritable Class ClientChatFaux
    Implements IChatClient

    Private ReadOnly _reponse As Func(Of IReadOnlyList(Of ChatMessage), String)

    ''' <summary>Crée le faux client ; <paramref name="reponse"/> calcule le texte à renvoyer.</summary>
    Public Sub New(reponse As Func(Of IReadOnlyList(Of ChatMessage), String))
        _reponse = reponse
    End Sub

    Public Function GetResponseAsync(messages As IEnumerable(Of ChatMessage),
                                     Optional options As ChatOptions = Nothing,
                                     Optional cancellationToken As CancellationToken = Nothing) As Task(Of ChatResponse) _
                                     Implements IChatClient.GetResponseAsync
        Dim texte As String = _reponse(messages.ToList())
        Return Task.FromResult(New ChatResponse(New ChatMessage(ChatRole.Assistant, texte)))
    End Function

    Public Function GetStreamingResponseAsync(messages As IEnumerable(Of ChatMessage),
                                              Optional options As ChatOptions = Nothing,
                                              Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of ChatResponseUpdate) _
                                              Implements IChatClient.GetStreamingResponseAsync
        Throw New NotSupportedException("Démonstration : seul GetResponseAsync est implémenté.")
    End Function

    Public Function GetService(serviceType As Type, Optional serviceKey As Object = Nothing) As Object _
                               Implements IChatClient.GetService
        Return Nothing
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub

End Class
