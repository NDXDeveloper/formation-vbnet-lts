' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Implémentation en mémoire de IProduitService (Implements).
'                Dans une vraie application, le corps s'appuierait sur EF Core
'                ou ADO.NET (module 7) ; ici un dictionnaire suffit à rendre
'                l'API exécutable et vérifiable. Enregistré en AddScoped, mais
'                les données sont partagées via un store Shared (singleton de
'                fait) pour persister entre les requêtes du test.
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks

Public Class ProduitService
    Implements IProduitService

    ' Store partagé par toutes les requêtes (l'instance du service, elle, est Scoped).
    Private Shared ReadOnly _store As New ConcurrentDictionary(Of Integer, Produit)(
        {
            New KeyValuePair(Of Integer, Produit)(1, New Produit With {.Id = 1, .Nom = "Clavier", .Prix = 49.9D, .EnStock = True}),
            New KeyValuePair(Of Integer, Produit)(2, New Produit With {.Id = 2, .Nom = "Écran", .Prix = 220D, .EnStock = True}),
            New KeyValuePair(Of Integer, Produit)(3, New Produit With {.Id = 3, .Nom = "Souris", .Prix = 25D, .EnStock = False})
        })
    Private Shared _prochainId As Integer = 3

    Public Function ListerAsync() As Task(Of IReadOnlyList(Of Produit)) Implements IProduitService.ListerAsync
        Dim liste As IReadOnlyList(Of Produit) = _store.Values.OrderBy(Function(p) p.Id).ToList()
        Return Task.FromResult(liste)
    End Function

    Public Function ListerEnrichiAsync() As Task(Of IReadOnlyList(Of Produit)) Implements IProduitService.ListerEnrichiAsync
        ' « Variante v2 » : ici on trie par prix décroissant pour matérialiser la différence.
        Dim liste As IReadOnlyList(Of Produit) = _store.Values.OrderByDescending(Function(p) p.Prix).ToList()
        Return Task.FromResult(liste)
    End Function

    Public Function ObtenirAsync(id As Integer) As Task(Of Produit) Implements IProduitService.ObtenirAsync
        Dim p As Produit = Nothing
        _store.TryGetValue(id, p)
        Return Task.FromResult(p)
    End Function

    Public Function ExisteAsync(nom As String) As Task(Of Boolean) Implements IProduitService.ExisteAsync
        Dim existe = _store.Values.Any(Function(p) String.Equals(p.Nom, nom, StringComparison.OrdinalIgnoreCase))
        Return Task.FromResult(existe)
    End Function

    Public Function CreerAsync(nouveau As CreationProduit) As Task(Of Produit) Implements IProduitService.CreerAsync
        Dim id = Interlocked.Increment(_prochainId)
        Dim p As New Produit With {.Id = id, .Nom = nouveau.Nom, .Prix = nouveau.Prix, .EnStock = nouveau.EnStock}
        _store(id) = p
        Return Task.FromResult(p)
    End Function

    Public Function MettreAJourAsync(id As Integer, maj As MiseAJourProduit) As Task(Of Boolean) Implements IProduitService.MettreAJourAsync
        Dim existant As Produit = Nothing
        If Not _store.TryGetValue(id, existant) Then Return Task.FromResult(False)
        existant.Nom = maj.Nom
        existant.Prix = maj.Prix
        existant.EnStock = maj.EnStock
        Return Task.FromResult(True)
    End Function

    Public Function SupprimerAsync(id As Integer) As Task Implements IProduitService.SupprimerAsync
        Dim retire As Produit = Nothing
        _store.TryRemove(id, retire)
        Return Task.CompletedTask
    End Function
End Class
