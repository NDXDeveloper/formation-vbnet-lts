' ============================================================================
'  Section 7.2 : Entity Framework Core 10
'  Description : Le DbContext de la section : DbSet par entité, configuration
'                par API Fluent (clé, longueur, précision décimale, relation
'                1-N), TYPE COMPLEXE Profil mappé en JSON (ComplexProperty +
'                ToJson, EF Core 10) et FILTRES GLOBAUX NOMMÉS (HasQueryFilter
'                nommé, EF Core 10).
'  Fichier source : 02-ef-core-10.md
' ============================================================================

Imports Microsoft.EntityFrameworkCore

Public Class BoutiqueContext
    Inherits DbContext

    Public Property Clients As DbSet(Of Client)
    Public Property Commandes As DbSet(Of Commande)
    Public Property Produits As DbSet(Of Produit)

    Private ReadOnly _chaine As String

    Public Sub New(chaineConnexion As String)
        _chaine = chaineConnexion
    End Sub

    Protected Overrides Sub OnConfiguring(options As DbContextOptionsBuilder)
        options.UseSqlite(_chaine)
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
        modelBuilder.Entity(Of Client)(
            Sub(entity)
                entity.HasKey(Function(c) c.Id)
                entity.Property(Function(c) c.Nom).IsRequired().HasMaxLength(100)
                ' Type complexe -> colonne JSON (EF Core 10)
                entity.ComplexProperty(Function(c) c.Profil, Sub(p) p.ToJson())
                ' Filtres globaux NOMMÉS (EF Core 10) : on pourra les ignorer sélectivement
                entity.HasQueryFilter("NonSupprime", Function(c) Not c.EstSupprime)
                entity.HasQueryFilter("ClientActif", Function(c) c.EstActif)
            End Sub)

        modelBuilder.Entity(Of Commande)(
            Sub(entity)
                entity.Property(Function(c) c.Montant).HasPrecision(18, 2)
                entity.HasOne(Function(c) c.Client) _
                      .WithMany(Function(cl) cl.Commandes) _
                      .HasForeignKey(Function(c) c.ClientId)
            End Sub)
    End Sub
End Class
