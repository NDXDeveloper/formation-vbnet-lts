' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Émetteur de jeton de TEST UNIQUEMENT — NE FAIT PAS partie du
'                cours. En production, c'est un fournisseur d'identité externe
'                (Entra ID, Auth0…) qui émet le jeton ; l'API se contente de le
'                VALIDER. Ce contrôleur existe seulement pour démontrer la
'                chaîne d'autorisation HORS LIGNE : il signe un JWT avec la clé
'                locale partagée (cf. Securite.vb), afin de tester le chemin
'                « avec jeton valide » sur l'action DELETE protégée.
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Security.Claims
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.IdentityModel.JsonWebTokens
Imports Microsoft.IdentityModel.Tokens

<ApiController>
<Route("dev/jeton")>
Public Class JetonsController
    Inherits ControllerBase

    ' GET /dev/jeton?role=Administrateur  ->  { "access_token": "..." }
    <HttpGet>
    Public Function Emettre(<FromQuery> role As String) As IActionResult
        Dim handler As New JsonWebTokenHandler()
        Dim descripteur As New SecurityTokenDescriptor With {
            .Issuer = Securite.Emetteur,
            .Audience = Securite.Audience,
            .Expires = DateTime.UtcNow.AddMinutes(30),
            .SigningCredentials = New SigningCredentials(Securite.CleSignature, SecurityAlgorithms.HmacSha256),
            .Claims = New Dictionary(Of String, Object) From {
                {ClaimTypes.Name, "testeur"},
                {ClaimTypes.Role, If(String.IsNullOrEmpty(role), "Utilisateur", role)}
            }
        }

        Dim jeton As String = handler.CreateToken(descripteur)
        Return Ok(New With {.access_token = jeton})
    End Function
End Class
