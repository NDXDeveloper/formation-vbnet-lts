' ============================================================================
'  Section 8.1 : Consommer des API REST
'  Description : Point d'entrée. Démarre le serveur local, configure l'hôte
'                générique avec IHttpClientFactory + CLIENT TYPÉ
'                (AddHttpClient(Of CatalogueClient)) et la RÉSILIENCE Polly
'                (AddStandardResilienceHandler : retries, disjoncteur,
'                délais d'attente), résout le client et enchaîne GET liste /
'                GET par id (existant + 404) / POST création.
'  Fichier source : 01-consommer-api-rest.md
' ============================================================================

Imports System
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Module Program

    Function Main(args As String()) As Integer
        Return MainAsync(args).GetAwaiter().GetResult()
    End Function

    Async Function MainAsync(args As String()) As Task(Of Integer)
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 8.1 Consommer des API REST (client typé + résilience) ===")
        Console.WriteLine()

        ' 1) Serveur local (tient lieu d'API distante), sur un port fixe.
        Using serveur As New ServeurLocal(5099)
            serveur.Demarrer()
            Console.WriteLine($"Serveur local démarré sur {serveur.AdresseBase}")
            Console.WriteLine()

            ' 2) Hôte générique : IHttpClientFactory + client typé + résilience.
            Dim builder = Host.CreateApplicationBuilder(args)
            ' On réduit le bruit des journaux (HttpClient/Polly loggent en Information).
            builder.Logging.SetMinimumLevel(LogLevel.Warning)
            builder.Services.AddHttpClient(Of CatalogueClient)(
                Sub(c) c.BaseAddress = New Uri(serveur.AdresseBase)
            ).AddStandardResilienceHandler()

            Using hote = builder.Build()
                Dim client = hote.Services.GetRequiredService(Of CatalogueClient)()

                ' --- GET liste ---
                Dim produits = Await client.ObtenirProduitsAsync()
                Console.WriteLine($"GET /api/produits -> {produits.Count} produit(s) reçu(s) :")
                For Each p In produits
                    Console.WriteLine($"  #{p.Id} {p.Nom} — {p.Prix:0.00} € — en stock : {p.EnStock}")
                Next
                Console.WriteLine()

                ' --- GET par id (existant) ---
                Dim ecran = Await client.ObtenirParIdAsync(2)
                Console.WriteLine($"GET /api/produits/2 -> {ecran.Nom} ({ecran.Prix:0.00} €)")

                ' --- GET par id (absent) : 404 traité, renvoie Nothing ---
                Dim absent = Await client.ObtenirParIdAsync(999)
                Console.WriteLine($"GET /api/produits/999 -> {If(absent Is Nothing, "introuvable (404 géré)", absent.Nom)}")
                Console.WriteLine()

                ' --- POST création ---
                Dim cree = Await client.CreerAsync(New Produit With {.Nom = "Webcam", .Prix = 59.5D, .EnStock = True})
                Console.WriteLine($"POST /api/produits -> créé avec id {cree.Id} : {cree.Nom}")

                ' Vérification : la liste contient désormais un produit de plus.
                Dim apres = Await client.ObtenirProduitsAsync()
                Console.WriteLine($"GET /api/produits (après POST) -> {apres.Count} produit(s)")
            End Using
        End Using

        Console.WriteLine()
        Console.WriteLine("Terminé.")
        Return 0
    End Function

End Module
