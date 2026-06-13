' ============================================================================
'  Section 8.5 : gRPC et GraphQL — orchestration (client VB)
'  Description : Point d'entrée. Enchaîne les trois consommations sur le serveur
'                de services (lancé à part) : gRPC natif (port HTTP/2), REST
'                transcodé et GraphQL (port HTTP/1.1). Illustre le fil conducteur
'                du module 8 : « consommer est toujours possible en VB ; créer
'                les briques à fort outillage se délègue à C# ».
'  Fichier source : 05-grpc-graphql.md
' ============================================================================

Imports System
Imports System.Threading.Tasks

Module Program

    Function Main(args As String()) As Integer
        Return MainAsync().GetAwaiter().GetResult()
    End Function

    Async Function MainAsync() As Task(Of Integer)
        Console.OutputEncoding = Text.Encoding.UTF8
        Dim adresseGrpc = "http://localhost:5185"      ' HTTP/2 (gRPC natif)
        Dim adresseHttp = "http://localhost:5186"      ' HTTP/1.1 (REST + GraphQL)

        Console.WriteLine("=== 8.5 gRPC et GraphQL — consommation depuis VB.NET ===")
        Console.WriteLine()

        Console.WriteLine("[gRPC natif] via la bibliothèque cliente C# (Catalogue.Grpc)")
        Await DemoGrpc.ExecuterAsync(adresseGrpc)
        Console.WriteLine()

        Console.WriteLine("[gRPC -> REST] JSON transcoding, consommé comme une API REST (§ 8.1)")
        Await DemoRest.ExecuterAsync(adresseHttp)
        Console.WriteLine()

        Console.WriteLine("[GraphQL] HttpClient + JSON brut (POST query/variables)")
        Await DemoGraphQL.ExecuterAsync(adresseHttp)
        Console.WriteLine()

        Console.WriteLine("Terminé.")
        Return 0
    End Function

End Module
