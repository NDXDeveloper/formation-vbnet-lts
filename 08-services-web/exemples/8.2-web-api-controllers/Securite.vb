' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Paramètres de signature JWT partagés par la VALIDATION (dans
'                Program.vb) et l'ÉMISSION de jeton de TEST (JetonsController).
'                ⚠️ Le cours rappelle qu'une Web API ne FABRIQUE pas les jetons
'                — elle les VALIDE (jetons émis par Entra ID, Auth0, Keycloak…).
'                Ici, pour rendre la démonstration exécutable HORS LIGNE, on
'                utilise une clé symétrique locale et un petit émetteur de test
'                (même esprit que le serveur HttpListener de 8.1, ou SQLite au
'                module 7 à la place de SQL Server).
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System.Text
Imports Microsoft.IdentityModel.Tokens

Public Module Securite
    Public Const Emetteur As String = "formation-vbnet"
    Public Const Audience As String = "api-catalogue"

    ' HS256 exige une clé d'au moins 256 bits (32 octets).
    Public ReadOnly CleSignature As SymmetricSecurityKey =
        New SymmetricSecurityKey(Encoding.UTF8.GetBytes("cle-de-demonstration-formation-vbnet-2026!"))
End Module
