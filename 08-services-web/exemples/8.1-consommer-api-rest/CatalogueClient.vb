' ============================================================================
'  Section 8.1 : Consommer des API REST
'  Description : Le CLIENT TYPÉ de la section : une classe dédiée reçoit un
'                HttpClient par injection (fourni par IHttpClientFactory) et
'                encapsule les appels. Emploie les extensions
'                System.Net.Http.Json (GetFromJsonAsync / PostAsJsonAsync) et
'                gère explicitement le 404 (absence légitime -> Nothing).
'                Le HttpClient injecté n'est PAS placé dans un Using : sa durée
'                de vie est gérée par la fabrique.
'  Fichier source : 01-consommer-api-rest.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Json
Imports System.Text.Json
Imports System.Text.Json.Serialization

Public Class CatalogueClient
    Private ReadOnly _http As HttpClient

    ' Options réutilisées (Shared ReadOnly) : conserve le cache de métadonnées.
    Private Shared ReadOnly _options As New JsonSerializerOptions With {
        .PropertyNameCaseInsensitive = True,
        .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    }

    Public Sub New(http As HttpClient)
        _http = http
    End Sub

    Public Async Function ObtenirProduitsAsync() As Task(Of IReadOnlyList(Of Produit))
        Dim produits = Await _http.GetFromJsonAsync(Of List(Of Produit))("api/produits", _options)
        Return If(produits, New List(Of Produit)())
    End Function

    Public Async Function ObtenirParIdAsync(id As Integer) As Task(Of Produit)
        Dim reponse = Await _http.GetAsync($"api/produits/{id}")
        If reponse.IsSuccessStatusCode Then
            Return Await reponse.Content.ReadFromJsonAsync(Of Produit)(_options)
        ElseIf reponse.StatusCode = HttpStatusCode.NotFound Then
            Return Nothing                                   ' absence légitime
        Else
            reponse.EnsureSuccessStatusCode()
            Return Nothing
        End If
    End Function

    Public Async Function CreerAsync(nouveau As Produit) As Task(Of Produit)
        Dim reponse = Await _http.PostAsJsonAsync("api/produits", nouveau, _options)
        reponse.EnsureSuccessStatusCode()
        Return Await reponse.Content.ReadFromJsonAsync(Of Produit)(_options)
    End Function
End Class
