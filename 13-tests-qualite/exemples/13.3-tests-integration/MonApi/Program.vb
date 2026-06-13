' ============================================================================
'  Section 13.3 : Tests d'intégration — point d'entrée de la Web API
'  Description : Point d'entrée EXPLICITE et Public (Public Class Program) — la
'                « limite » VB (pas de top-level statements) devient un avantage :
'                WebApplicationFactory(Of Program) peut le référencer sans
'                l'astuce C# « public partial class Program ». Enregistre les
'                contrôleurs et le DbContext SQLite ; le projet de test
'                substitue la connexion par une SQLite de test.
'  Fichier source : 03-tests-integration.md
' ============================================================================

Imports Microsoft.AspNetCore.Builder
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection

Public Class Program
    Public Shared Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()

        Dim cs = builder.Configuration.GetConnectionString("Db")
        If String.IsNullOrEmpty(cs) Then cs = "Data Source=catalogue.db"
        builder.Services.AddDbContext(Of CatalogueContext)(Sub(o) o.UseSqlite(cs))

        Dim app = builder.Build()

        ' Schéma créé au démarrage (le test fournit sa propre base SQLite).
        Using scope = app.Services.CreateScope()
            scope.ServiceProvider.GetRequiredService(Of CatalogueContext)().Database.EnsureCreated()
        End Using

        app.MapControllers()
        app.Run()
    End Sub
End Class
