' ============================================================================
'  Section 13.3 : Tests d'intégration — Web API réelle + base réelle
'  Description : ApiIntegrationTests exerce le PIPELINE HTTP complet via
'                WebApplicationFactory (routage → contrôleur → EF Core → SQLite).
'                SqliteDbTests teste EF Core directement contre une VRAIE SQLite
'                (identité générée, persistance entre contextes). Les deux sont
'                catégorisés « Integration » (filtrables séparément des tests
'                unitaires).
'  Fichier source : 03-tests-integration.md
' ============================================================================

Imports System.Linq
Imports System.Net
Imports System.Threading.Tasks
Imports Microsoft.Data.Sqlite
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports MonApi
Imports Xunit

' --- Web API en mémoire + base SQLite de test ---
Public Class ApiIntegrationTests
    Implements IClassFixture(Of FabriqueDeTest)

    Private ReadOnly _factory As FabriqueDeTest

    Public Sub New(factory As FabriqueDeTest)
        _factory = factory
        ' Semer la base de test (schéma + données), idempotent.
        Using scope = _factory.Services.CreateScope()
            Dim ctx = scope.ServiceProvider.GetRequiredService(Of CatalogueContext)()
            ctx.Database.EnsureCreated()
            If Not ctx.Produits.Any() Then
                ctx.Produits.AddRange(
                    New Produit With {.Nom = "Clavier", .Prix = 49.9D},
                    New Produit With {.Nom = "Ecran", .Prix = 220D},
                    New Produit With {.Nom = "Souris", .Prix = 25D})
                ctx.SaveChanges()
            End If
        End Using
    End Sub

    <Fact>
    <Trait("Category", "Integration")>
    Public Async Function GetProduits_RetourneSeed() As Task
        Dim client = _factory.CreateClient()

        Dim reponse = Await client.GetAsync("/api/produits")
        reponse.EnsureSuccessStatusCode()
        Assert.Equal(HttpStatusCode.OK, reponse.StatusCode)

        Dim json = Await reponse.Content.ReadAsStringAsync()
        Assert.Contains("Clavier", json)
        Assert.Contains("Souris", json)
    End Function
End Class

' --- EF Core directement contre une vraie SQLite ---
Public Class SqliteDbTests

    <Fact>
    <Trait("Category", "Integration")>
    Public Sub EfCore_SurSqliteReel_InsereEtRequete()
        Using connexion As New SqliteConnection("DataSource=:memory:")
            connexion.Open()
            Dim options = New DbContextOptionsBuilder(Of CatalogueContext)() _
                .UseSqlite(connexion).Options

            Using ctx As New CatalogueContext(options)
                ctx.Database.EnsureCreated()
                ctx.Produits.Add(New Produit With {.Nom = "Webcam", .Prix = 59.5D})
                ctx.SaveChanges()
            End Using

            ' Nouveau contexte, même connexion → la donnée a bien été persistée par le moteur.
            Using ctx As New CatalogueContext(options)
                Dim p = ctx.Produits.Single()
                Assert.Equal("Webcam", p.Nom)
                Assert.True(p.Id > 0)   ' identité générée par le vrai moteur SQL
            End Using
        End Using
    End Sub
End Class
