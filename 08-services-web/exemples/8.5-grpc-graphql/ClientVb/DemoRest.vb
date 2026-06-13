' ============================================================================
'  Section 8.5 : gRPC et GraphQL — l'échappatoire JSON transcoding
'  Description : Le MÊME service gRPC, mais consommé en REST/JSON grâce au JSON
'                transcoding activé côté serveur. Côté VB : un simple HttpClient,
'                AUCUN outillage gRPC — exactement comme consommer une API REST
'                (§ 8.1). C'est « souvent la voie la plus simple en VB » quand on
'                contrôle (ou peut influencer) le serveur.
'  Fichier source : 05-grpc-graphql.md
' ============================================================================

Imports System
Imports System.Net.Http
Imports System.Text.Json
Imports System.Threading.Tasks

Public Module DemoRest
    Public Async Function ExecuterAsync(adresseRest As String) As Task
        Using client As New HttpClient()
            Dim json = Await client.GetStringAsync($"{adresseRest}/v1/produits/42")
            Using doc = JsonDocument.Parse(json)
                Dim r = doc.RootElement
                Console.WriteLine($"  REST transcodé : #{r.GetProperty("id").GetInt32()} " &
                                  $"{r.GetProperty("nom").GetString()} — {r.GetProperty("prix").GetDouble():0.00} €")
            End Using
        End Using
    End Function
End Module
