' ============================================================================
'  Section 16.1 : Authentification — point d'entrée du client MSAL
'  Description : Décrit le rôle du client. L'acquisition réelle n'est PAS exécutée
'                (elle exige une inscription d'application Entra ID + un navigateur) ;
'                l'usage réel est laissé en commentaire. L'intérêt de l'exemple est
'                que ServiceAuthentification COMPILE et illustre le contournement de
'                BC36943 (pas d'Await dans un Catch).
'  Fichier source : 01-auth.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 16.1 Client MSAL.NET (compilation seule) ===")
        Console.WriteLine("ServiceAuthentification acquiert un jeton : silencieux (cache) puis interactif.")
        Console.WriteLine("Non exécuté ici : l'acquisition réelle exige une app Entra ID + un navigateur.")
        Console.WriteLine("Motif VB : 'Await' interdit dans un Catch (BC36943) -> drapeau interactionRequise.")

        ' Usage réel (nécessite des identifiants Entra ID valides) :
        '
        '   Dim svc As New ServiceAuthentification("<ClientId>", "<TenantId>")
        '   Dim jeton As String = Await svc.ObtenirJetonAsync()
        '   client.DefaultRequestHeaders.Authorization =
        '       New AuthenticationHeaderValue("Bearer", jeton)
    End Sub
End Module
