' ============================================================================
'  Section 7.4 : Autres stockages (notions)
'  Description : Illustre, de façon exécutable et SANS serveur, les concepts
'                de la section :
'                  · NoSQL documentaire -> un objet sérialisé en document JSON
'                    (chaque « document » est autonome, schéma souple) ;
'                  · cache-aside -> on regarde d'abord le cache, en cas de
'                    « miss » on lit la « base » (simulée), on remplit le cache
'                    avec un TTL, puis les lectures suivantes sont des « hits ».
'                IMemoryCache tient lieu de Redis (mêmes idées : clé/valeur,
'                expiration). Les vraies API serveur sont documentées.
'  Fichier source : 04-autres-stockages.md
'  Compilation    : dotnet run
' ============================================================================

Imports System
Imports System.Text.Json
Imports System.Threading
Imports Microsoft.Extensions.Caching.Memory

Module Program

    ' « Base » simulée + compteur d'accès pour distinguer hit / miss
    Private ReadOnly _base As New Dictionary(Of Integer, Produit) From {
        {42, New Produit With {.Id = 42, .Libelle = "Clavier", .Prix = 49.9D, .Tags = {"périphérique", "bureau"}}}
    }
    Private _accesBase As Integer = 0

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoDocumentJson()
        DemoCacheAside()
        DocumenterApisReelles()
    End Sub

    ' ---- NoSQL documentaire : un objet = un document JSON --------------------
    Private Sub DemoDocumentJson()
        Console.WriteLine("== NoSQL documentaire (document JSON autonome) ==")
        Dim produit As New Produit With {
            .Id = 1, .Libelle = "Clavier mécanique", .Prix = 89.9D,
            .Tags = {"périphérique", "gaming"}
        }
        ' Un document : structure imbriquée, schéma souple (les Tags varient d'un doc à l'autre)
        Dim document = JsonSerializer.Serialize(produit, New JsonSerializerOptions With {.WriteIndented = False})
        Console.WriteLine($"Document : {document}")

        Dim relu = JsonSerializer.Deserialize(Of Produit)(document)
        Console.WriteLine($"Relu : {relu.Libelle} ({relu.Tags.Length} tags) — round-trip OK : {relu.Libelle = produit.Libelle}")
    End Sub

    ' ---- Cache-aside : cache d'abord, base en cas de miss --------------------
    Private Sub DemoCacheAside()
        Console.WriteLine()
        Console.WriteLine("== Motif cache-aside (IMemoryCache, façon Redis) ==")

        Using cache As New MemoryCache(New MemoryCacheOptions())
            ' 1er accès : cache MISS -> lecture en base, puis remplissage du cache
            Dim p1 = LireAvecCache(cache, 42)
            ' 2e accès : cache HIT -> aucune lecture en base
            Dim p2 = LireAvecCache(cache, 42)
            ' 3e accès : toujours un HIT
            Dim p3 = LireAvecCache(cache, 42)

            Console.WriteLine($"3 lectures de la clé 42, mais la « base » n'a été touchée que {_accesBase} fois (cache-aside).")
            Console.WriteLine($"Valeur servie : {p3.Libelle} à {p3.Prix} €")
        End Using
    End Sub

    Private Function LireAvecCache(cache As IMemoryCache, id As Integer) As Produit
        Dim cle = $"produit:{id}"
        Dim produit As Produit = Nothing
        If cache.TryGetValue(cle, produit) Then
            Console.WriteLine($"  clé {cle} -> HIT (servi depuis le cache)")
            Return produit
        End If

        ' MISS : on lit la « base » (coûteux), puis on remplit le cache avec un TTL
        _accesBase += 1
        produit = _base(id)
        Console.WriteLine($"  clé {cle} -> MISS (lecture en base n° {_accesBase})")
        cache.Set(cle, produit, New MemoryCacheEntryOptions With {
            .AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        })
        Return produit
    End Function

    ' ---- Les vraies API (documentaire) ---------------------------------------
    Private Sub DocumenterApisReelles()
        Console.WriteLine()
        Console.WriteLine("== Pour de vrai (serveurs requis — non exécutés ici) ==")
        Console.WriteLine("  MongoDB   : New MongoClient(""mongodb://localhost:27017"").GetDatabase(...).GetCollection(Of T)(...)")
        Console.WriteLine("              ou EF Core : options.UseMongoDB(client, ""boutique"")")
        Console.WriteLine("  Cosmos DB : options.UseCosmos(endpoint, key, ""Boutique"")  — penser RU + clé de partition")
        Console.WriteLine("  Redis     : ConnectionMultiplexer.Connect(""localhost:6379"").GetDatabase()")
        Console.WriteLine("              ou IDistributedCache (AddStackExchangeRedisCache) — même API que ci-dessus")
        Console.WriteLine("  -> côté VB.NET, ce ne sont que des bibliothèques .NET à consommer (aucune limite).")
    End Sub

End Module

''' <summary>« Document » NoSQL : schéma souple, tableau de tags imbriqué.</summary>
Public Class Produit
    Public Property Id As Integer
    Public Property Libelle As String
    Public Property Prix As Decimal
    Public Property Tags As String()
End Class
