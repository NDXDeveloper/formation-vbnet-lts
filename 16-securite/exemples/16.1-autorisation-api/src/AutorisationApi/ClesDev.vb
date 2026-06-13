' ============================================================================
'  Section 16.1 : Authentification — paramètres de DÉMONSTRATION
'  Description : Émetteur, audience et clé de signature SYMÉTRIQUE (HS256) partagés
'                par l'API (validation) et le harnais de test (émission). Cela rend
'                l'exemple vérifiable hors ligne. ⚠️ EN PRODUCTION : la clé n'est
'                JAMAIS dans le code — c'est un IdP (Entra ID) qui signe avec une clé
'                ASYMÉTRIQUE, et l'API ne connaît que la clé PUBLIQUE (via OIDC).
'  Fichier source : 01-auth.md
' ============================================================================

Option Strict On
Option Explicit On

Public Module ClesDev
    Public Const Emetteur As String = "https://formation-vbnet.local/issuer"
    Public Const Audience As String = "api://commandes"
    ' HS256 exige une clé d'au moins 256 bits (32 octets). Démo uniquement.
    Public Const CleSignature As String = "cle-de-demonstration-formation-vbnet-secrete-256bits!!"
End Module
