' ============================================================================
'  Section 13.3 : Tests d'intégration — modèle et contexte EF Core
'  Description : Le modèle (Produit), le DbContext (CatalogueContext) et le
'                contrôleur. Le contrôleur lit la base via EF Core : c'est ce
'                que le test d'intégration exercera « pour de vrai » (HTTP →
'                contrôleur → EF Core → SQLite).
'  Fichier source : 03-tests-integration.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.EntityFrameworkCore

Public Class Produit
    Public Property Id As Integer
    Public Property Nom As String
    Public Property Prix As Decimal
End Class

Public Class CatalogueContext
    Inherits DbContext

    Public Sub New(options As DbContextOptions(Of CatalogueContext))
        MyBase.New(options)
    End Sub

    Public Property Produits As DbSet(Of Produit)
End Class

<ApiController>
<Route("api/produits")>
Public Class ProduitsController
    Inherits ControllerBase

    Private ReadOnly _ctx As CatalogueContext

    Public Sub New(ctx As CatalogueContext)
        _ctx = ctx
    End Sub

    <HttpGet>
    Public Async Function Lister() As Task(Of ActionResult(Of IEnumerable(Of Produit)))
        Return Ok(Await _ctx.Produits.OrderBy(Function(p) p.Id).ToListAsync())
    End Function
End Class
