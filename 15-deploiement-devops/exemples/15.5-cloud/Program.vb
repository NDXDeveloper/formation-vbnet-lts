' ============================================================================
'  Section 15.5 : Cloud (essentiels via SDK) — consommation
'  Description : Construit les clients Azure (sans appel réseau) et lit leurs
'                propriétés locales, montrant que la consommation des SDK cloud
'                est identique en VB et en C#. DefaultAzureCredential : même code
'                en local (identifiants de dev) et en Azure (identité managée),
'                sans secret. Les opérations live (Upload, GetSecret) exigeraient
'                un compte Azure réel.
'  Fichier source : 05-cloud-essentiels.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports Azure.Identity
Imports Azure.Security.KeyVault.Secrets
Imports Azure.Storage.Blobs

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 15.5 Cloud : consommation des SDK Azure ===")
        Console.WriteLine()

        ' Fil conducteur : authentification sans secret. La construction ne
        ' déclenche AUCUN appel réseau (la résolution est différée au 1er jeton).
        Dim credential As New DefaultAzureCredential()
        Console.WriteLine($"[Azure.Identity] {credential.GetType().Name} construit (sans secret)")

        ' Blob Storage : compte → conteneur → objet (clients construits localement).
        Dim service As New BlobServiceClient(
            New Uri("https://moncompte.blob.core.windows.net"), credential)
        Dim conteneur = service.GetBlobContainerClient("documents")
        Dim blob = conteneur.GetBlobClient("rapport.pdf")
        Console.WriteLine($"[Blob Storage] compte={service.AccountName} ; conteneur={conteneur.Name} ; objet={blob.Name}")

        ' Key Vault : client de secrets.
        Dim secrets As New SecretClient(
            New Uri("https://moncoffre.vault.azure.net"), credential)
        Console.WriteLine($"[Key Vault] VaultUri={secrets.VaultUri}")
        Console.WriteLine()

        Console.WriteLine("Clients construits sans appel réseau — la consommation des SDK est identique à C#.")
        Console.WriteLine("⚠️ Azure Functions : pas de support VB officiel → écrire la Function en C# (hybride).")
        Console.WriteLine()
        Console.WriteLine("Terminé.")
    End Sub
End Module
