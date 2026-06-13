' ============================================================================
'  Section 16.1 : Autorisation — contrôleur protégé
'  Description : Trois points d'accès illustrant l'autorisation déclarative :
'                - GET /commandes        : <Authorize(Policy)> sur le scope ;
'                - DELETE /commandes/{id}: <Authorize(Roles)> réservé aux admins ;
'                - GET /commandes/moi    : lit les CLAIMS du jeton (sub, scp, roles).
'                Le contrôle est CÔTÉ SERVEUR : sans le bon scope/rôle, l'API
'                répond 403 (interdit) ; sans jeton valide, 401 (non authentifié).
'  Fichier source : 01-auth.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Linq
Imports Microsoft.AspNetCore.Authorization
Imports Microsoft.AspNetCore.Mvc

<ApiController>
<Route("commandes")>
Public Class CommandesController
    Inherits ControllerBase

    ' Lecture : exige le scope « Commandes.Read » (politique basée sur une assertion).
    <HttpGet>
    <Authorize(Policy:="LectureCommandes")>
    Public Function Lister() As IActionResult
        Return Ok(New String() {"CMD-1", "CMD-2"})
    End Function

    ' Suppression : réservée au rôle « Administrateur ».
    <HttpDelete("{id}")>
    <Authorize(Roles:="Administrateur")>
    Public Function Supprimer(id As String) As IActionResult
        Return NoContent()
    End Function

    ' Profil : renvoie les claims lues du jeton (tout utilisateur authentifié).
    <HttpGet("moi")>
    <Authorize>
    Public Function Moi() As IActionResult
        Dim sujet = User.FindFirst("sub")?.Value
        Dim scopes = User.FindFirst("scp")?.Value
        Dim roles = String.Join(",", User.FindAll("roles").Select(Function(c) c.Value))
        Return Ok(New With {.Sujet = sujet, .Scopes = scopes, .Roles = roles})
    End Function

End Class
