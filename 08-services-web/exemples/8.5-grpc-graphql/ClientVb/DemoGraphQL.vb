' ============================================================================
'  Section 8.5 : gRPC et GraphQL — consommer GraphQL en VB pur
'  Description : La bonne nouvelle de la section : consommer GraphQL ne demande
'                AUCUN C#. Sur le réseau, c'est un simple POST HTTP avec un corps
'                JSON { "query": …, "variables": … } et une réponse JSON
'                { "data": …, "errors": … }. On l'attaque avec HttpClient +
'                System.Text.Json (PostAsJsonAsync, puis JsonDocument et
'                propriétés d'axe) — les outils du § 8.1. On perd le typage fort
'                (réservé à StrawberryShake, C#), mais la consommation est native.
'  Fichier source : 05-grpc-graphql.md
' ============================================================================

Imports System
Imports System.Net.Http
Imports System.Net.Http.Json
Imports System.Text.Json
Imports System.Threading.Tasks

Public Module DemoGraphQL
    Public Async Function ExecuterAsync(adresseGraphQL As String) As Task
        Using client As New HttpClient()
            ' Corps de la requête GraphQL (objet JSON anonyme : query + variables).
            ' NB : le schéma HotChocolate type l'argument « id » en String (issu du
            ' paramètre C# string), d'où $id: String! (le cours illustrait ID!).
            Dim requete = New With {
                .query = "query($id: String!) { produit(id: $id) { nom prix } }",
                .variables = New With {.id = "42"}
            }

            Dim reponse = Await client.PostAsJsonAsync($"{adresseGraphQL}/graphql", requete)
            reponse.EnsureSuccessStatusCode()

            ' Réponse GraphQL : { "data": { "produit": { … } }, "errors": [ … ] }
            Using doc = Await JsonDocument.ParseAsync(Await reponse.Content.ReadAsStreamAsync())
                Dim produit = doc.RootElement.GetProperty("data").GetProperty("produit")
                Console.WriteLine($"  GraphQL : {produit.GetProperty("nom").GetString()} — " &
                                  $"{produit.GetProperty("prix").GetDouble():0.00} €")
            End Using
        End Using
    End Function
End Module
