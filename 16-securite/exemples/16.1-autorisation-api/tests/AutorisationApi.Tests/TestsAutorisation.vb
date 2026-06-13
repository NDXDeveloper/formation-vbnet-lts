' ============================================================================
'  Section 16.1 : Autorisation — vérification de la matrice d'accès
'  Description : Six tests d'intégration in-process. Un jeton HS256 est forgé
'                localement avec les claims voulus (scp, roles, sub) signés par la
'                clé de démo partagée, puis envoyé en en-tête Bearer. On vérifie
'                que l'API applique bien les politiques côté serveur.
'  Fichier source : 01-auth.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Text
Imports AutorisationApi
Imports Microsoft.AspNetCore.Mvc.Testing
Imports Microsoft.IdentityModel.JsonWebTokens
Imports Microsoft.IdentityModel.Tokens
Imports Xunit

Public Class TestsAutorisation
    Implements IClassFixture(Of WebApplicationFactory(Of Program))

    Private ReadOnly _usine As WebApplicationFactory(Of Program)

    Public Sub New(usine As WebApplicationFactory(Of Program))
        _usine = usine
    End Sub

    ' Forge un jeton HS256 de test avec les claims demandés.
    Private Shared Function CreerJeton(scp As String, roles As String()) As String
        Dim cle As New SymmetricSecurityKey(Encoding.UTF8.GetBytes(ClesDev.CleSignature))
        Dim claims As New Dictionary(Of String, Object) From {{"sub", "alice"}}
        If scp IsNot Nothing Then claims("scp") = scp
        If roles IsNot Nothing AndAlso roles.Length > 0 Then claims("roles") = roles

        Dim descripteur As New SecurityTokenDescriptor With {
            .Issuer = ClesDev.Emetteur,
            .Audience = ClesDev.Audience,
            .Expires = DateTime.UtcNow.AddMinutes(5),
            .SigningCredentials = New SigningCredentials(cle, SecurityAlgorithms.HmacSha256),
            .Claims = claims
        }
        Return New JsonWebTokenHandler().CreateToken(descripteur)
    End Function

    Private Function ClientAvecJeton(scp As String, roles As String()) As HttpClient
        Dim client = _usine.CreateClient()
        client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", CreerJeton(scp, roles))
        Return client
    End Function

    <Fact>
    Public Async Function SansJeton_LectureRenvoie401() As Task
        Dim reponse = Await _usine.CreateClient().GetAsync("/commandes")
        Assert.Equal(HttpStatusCode.Unauthorized, reponse.StatusCode)
    End Function

    <Fact>
    Public Async Function AvecScopeLecture_Renvoie200() As Task
        Dim reponse = Await ClientAvecJeton("Profil.Read Commandes.Read", Nothing).GetAsync("/commandes")
        Assert.Equal(HttpStatusCode.OK, reponse.StatusCode)
        Assert.Contains("CMD-1", Await reponse.Content.ReadAsStringAsync())
    End Function

    <Fact>
    Public Async Function SansLeBonScope_Renvoie403() As Task
        Dim reponse = Await ClientAvecJeton("Profil.Read", Nothing).GetAsync("/commandes")
        Assert.Equal(HttpStatusCode.Forbidden, reponse.StatusCode)
    End Function

    <Fact>
    Public Async Function Admin_PeutSupprimer_Renvoie204() As Task
        Dim reponse = Await ClientAvecJeton(Nothing, New String() {"Administrateur"}).DeleteAsync("/commandes/5")
        Assert.Equal(HttpStatusCode.NoContent, reponse.StatusCode)
    End Function

    <Fact>
    Public Async Function NonAdmin_NePeutPasSupprimer_Renvoie403() As Task
        Dim reponse = Await ClientAvecJeton("Commandes.Read", New String() {"Utilisateur"}).DeleteAsync("/commandes/5")
        Assert.Equal(HttpStatusCode.Forbidden, reponse.StatusCode)
    End Function

    <Fact>
    Public Async Function Moi_RenvoieLesClaimsDuJeton() As Task
        Dim reponse = Await ClientAvecJeton("Commandes.Read", New String() {"Administrateur"}).GetAsync("/commandes/moi")
        Assert.Equal(HttpStatusCode.OK, reponse.StatusCode)
        Dim corps = Await reponse.Content.ReadAsStringAsync()
        Assert.Contains("alice", corps)
        Assert.Contains("Commandes.Read", corps)
        Assert.Contains("Administrateur", corps)
    End Function

End Class
