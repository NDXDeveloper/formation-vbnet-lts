' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Le contrôleur d'API. Hérite de ControllerBase, annoté
'                <ApiController> (conventions : liaison auto, 400 auto sur
'                modèle invalide). Démontre : routage par attributs avec
'                contrainte :int, ActionResult(Of T), CreatedAtRoute (route
'                NOMMÉE pour éviter le piège du suffixe Async), Problem Details
'                (409), versioning (v1/v2 via MapToApiVersion), rate limiting
'                par attribut (policy stricte sur /ping), et <Authorize> par
'                rôle sur DELETE.
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Asp.Versioning
Imports Microsoft.AspNetCore.Authorization
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.RateLimiting

<ApiController>
<ApiVersion("1.0")>
<ApiVersion("2.0")>
<Route("api/v{version:apiVersion}/[controller]")>
<EnableRateLimiting("fixe")>
Public Class ProduitsController
    Inherits ControllerBase

    Private ReadOnly _service As IProduitService

    Public Sub New(service As IProduitService)        ' injecté par le conteneur
        _service = service
    End Sub

    ' --- v1 : liste simple ---
    <HttpGet>
    <MapToApiVersion("1.0")>
    <EndpointSummary("Liste les produits (v1)")>
    <ProducesResponseType(GetType(IEnumerable(Of Produit)), StatusCodes.Status200OK)>
    Public Async Function ListerAsync() As Task(Of ActionResult(Of IEnumerable(Of Produit)))
        Return Ok(Await _service.ListerAsync())
    End Function

    ' --- v2 : liste enrichie (variante réservée à la version 2.0) ---
    <HttpGet>
    <MapToApiVersion("2.0")>
    <EndpointSummary("Liste les produits, triée par prix (v2)")>
    Public Async Function ListerV2Async() As Task(Of ActionResult(Of IEnumerable(Of Produit)))
        Return Ok(Await _service.ListerEnrichiAsync())
    End Function

    <HttpGet("{id:int}", Name:="ObtenirProduit")>
    <EndpointSummary("Récupère un produit par son identifiant")>
    <ProducesResponseType(GetType(Produit), StatusCodes.Status200OK)>
    <ProducesResponseType(StatusCodes.Status404NotFound)>
    Public Async Function ObtenirAsync(id As Integer) As Task(Of ActionResult(Of Produit))
        Dim produit = Await _service.ObtenirAsync(id)
        If produit Is Nothing Then Return NotFound()
        Return Ok(produit)
    End Function

    <HttpPost>
    <ProducesResponseType(GetType(Produit), StatusCodes.Status201Created)>
    <ProducesResponseType(StatusCodes.Status400BadRequest)>
    <ProducesResponseType(StatusCodes.Status409Conflict)>
    Public Async Function CreerAsync(<FromBody> nouveau As CreationProduit) As Task(Of ActionResult(Of Produit))
        If Await _service.ExisteAsync(nouveau.Nom) Then
            ' Erreur métier structurée au format Problem Details (RFC 9457).
            Return Problem(
                detail:=$"Un produit nommé « {nouveau.Nom} » existe déjà.",
                statusCode:=StatusCodes.Status409Conflict,
                title:="Conflit de ressource")
        End If

        Dim cree = Await _service.CreerAsync(nouveau)
        ' 201 Created + en-tête Location ; route NOMMÉE (évite le retrait du suffixe Async).
        Return CreatedAtRoute("ObtenirProduit", New With {.id = cree.Id, .version = "1.0"}, cree)
    End Function

    <HttpPut("{id:int}")>
    <ProducesResponseType(StatusCodes.Status204NoContent)>
    <ProducesResponseType(StatusCodes.Status404NotFound)>
    Public Async Function MettreAJourAsync(id As Integer, <FromBody> maj As MiseAJourProduit) As Task(Of IActionResult)
        If Not Await _service.MettreAJourAsync(id, maj) Then Return NotFound()
        Return NoContent()
    End Function

    ' Protégé : nécessite un jeton valide portant le rôle "Administrateur".
    <Authorize(Roles:="Administrateur")>
    <HttpDelete("{id:int}")>
    <ProducesResponseType(StatusCodes.Status204NoContent)>
    <ProducesResponseType(StatusCodes.Status401Unauthorized)>
    <ProducesResponseType(StatusCodes.Status403Forbidden)>
    Public Async Function SupprimerAsync(id As Integer) As Task(Of IActionResult)
        Await _service.SupprimerAsync(id)
        Return NoContent()
    End Function

    ' Démonstration de rate limiting : la policy "stricte" (3 req/min) remplace
    ' ici la policy "fixe" du contrôleur, pour rendre le 429 facilement observable.
    <HttpGet("ping")>
    <AllowAnonymous>
    <EnableRateLimiting("stricte")>
    <MapToApiVersion("1.0")>
    Public Function Ping() As IActionResult
        Return Ok("pong")
    End Function
End Class
