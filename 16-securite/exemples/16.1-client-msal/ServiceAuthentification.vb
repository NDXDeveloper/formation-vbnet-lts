' ============================================================================
'  Section 16.1 : Authentification — service d'acquisition de jeton (MSAL.NET)
'  Description : Acquiert un jeton d'accès en privilégiant le CACHE (silencieux),
'                puis bascule sur le flux INTERACTIF si le cache est vide/expiré.
'                ⚠️ Piège VB : « Await » est INTERDIT dans un bloc Catch (BC36943).
'                On ne peut donc pas appeler directement AcquireTokenInteractive
'                dans le Catch(MsalUiRequiredException) : on lève un DRAPEAU et on
'                relance l'acquisition interactive APRÈS le Try. (En C#, on
'                « await » directement dans le catch — d'où ce contournement.)
'  Fichier source : 01-auth.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.Identity.Client

''' <summary>Acquisition de jeton d'accès via un client public MSAL.NET.</summary>
Public Class ServiceAuthentification

    Private ReadOnly _app As IPublicClientApplication
    Private ReadOnly _scopes As String() = {"api://commandes/Commandes.Read"}

    Public Sub New(clientId As String, tenantId As String)
        _app = PublicClientApplicationBuilder _
            .Create(clientId) _
            .WithAuthority(AzureCloudInstance.AzurePublic, tenantId) _
            .WithRedirectUri("http://localhost") _
            .Build()
    End Sub

    ''' <summary>Renvoie un jeton d'accès (silencieux si possible, interactif sinon).</summary>
    Public Async Function ObtenirJetonAsync() As Task(Of String)
        Dim comptes = Await _app.GetAccountsAsync()
        Dim resultat As AuthenticationResult = Nothing
        Dim interactionRequise As Boolean = False

        Try
            ' Chemin rapide : réutilise un jeton mis en cache.
            resultat = Await _app.AcquireTokenSilent(_scopes, comptes.FirstOrDefault()).ExecuteAsync()
        Catch ex As MsalUiRequiredException
            ' BC36943 : pas de « Await » ici. On signale qu'une interaction est requise.
            interactionRequise = True
        End Try

        If interactionRequise Then
            ' Hors du Catch, l'Await redevient autorisé : on ouvre le flux interactif.
            resultat = Await _app.AcquireTokenInteractive(_scopes).ExecuteAsync()
        End If

        Return resultat.AccessToken
    End Function

End Class
