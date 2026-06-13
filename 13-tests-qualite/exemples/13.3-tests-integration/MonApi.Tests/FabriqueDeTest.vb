' ============================================================================
'  Section 13.3 : Tests d'intégration — fabrique de test personnalisée
'  Description : Dérive WebApplicationFactory et redéfinit ConfigureWebHost pour
'                REMPLACER le DbContext de production par une SQLite in-memory de
'                test. La connexion est gardée OUVERTE pour toute la durée de vie
'                de la fabrique (une base SQLite in-memory vit tant que sa
'                connexion est ouverte).
'  Fichier source : 03-tests-integration.md
' ============================================================================

Imports System.Linq
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Mvc.Testing
Imports Microsoft.Data.Sqlite
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports MonApi

Public Class FabriqueDeTest
    Inherits WebApplicationFactory(Of Program)

    Private ReadOnly _connexion As SqliteConnection

    Public Sub New()
        _connexion = New SqliteConnection("DataSource=:memory:")
        _connexion.Open()
    End Sub

    Protected Overrides Sub ConfigureWebHost(builder As IWebHostBuilder)
        builder.ConfigureServices(
            Sub(services)
                ' Retirer l'enregistrement réel du DbContext…
                Dim descripteur = services.SingleOrDefault(
                    Function(d) d.ServiceType Is GetType(DbContextOptions(Of CatalogueContext)))
                If descripteur IsNot Nothing Then services.Remove(descripteur)
                ' …puis le réenregistrer vers la SQLite in-memory de test.
                services.AddDbContext(Of CatalogueContext)(Sub(o) o.UseSqlite(_connexion))
            End Sub)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        If disposing Then _connexion.Dispose()
    End Sub
End Class
