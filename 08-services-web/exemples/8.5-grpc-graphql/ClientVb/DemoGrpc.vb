' ============================================================================
'  Section 8.5 : gRPC et GraphQL — gRPC natif depuis VB
'  Description : Consommation gRPC NATIVE. Le client (CatalogueServiceClient)
'                et les messages (ProduitRequete/ProduitReponse) sont des types
'                .NET ordinaires, GÉNÉRÉS dans la bibliothèque C# Catalogue.Grpc
'                et simplement référencés ici. VB n'a aucun outillage gRPC : il
'                consomme. (h2c : on autorise HTTP/2 en clair pour la démo locale.)
'  Fichier source : 05-grpc-graphql.md
' ============================================================================

Imports System
Imports System.Threading.Tasks
Imports Catalogue.Grpc
Imports Grpc.Net.Client

Public Module DemoGrpc
    Public Async Function ExecuterAsync(adresseGrpc As String) As Task
        ' HTTP/2 en clair (pas de TLS dans cette démonstration hors ligne).
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", True)

        Using canal = GrpcChannel.ForAddress(adresseGrpc)
            Dim client = New CatalogueService.CatalogueServiceClient(canal)
            Dim reponse = Await client.ObtenirProduitAsync(New ProduitRequete With {.Id = 42})
            Console.WriteLine($"  gRPC natif : #{reponse.Id} {reponse.Nom} — {reponse.Prix:0.00} €")
        End Using
    End Function
End Module
