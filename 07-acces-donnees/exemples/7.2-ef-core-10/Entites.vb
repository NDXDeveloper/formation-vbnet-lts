' ============================================================================
'  Section 7.2 : Entity Framework Core 10
'  Description : Les entités de la section : Client (avec un type COMPLEXE
'                Profil mappé en JSON — EF Core 10), Commande (relation 1-N
'                vers Client, N-N vers Produit, jeton de concurrence) et
'                Produit. Pas de mot-clé « required » (inexistant en VB) ni de
'                « record » : de simples Class, comme le recommande la section.
'  Fichier source : 02-ef-core-10.md
' ============================================================================

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Public Class Client
    Public Property Id As Integer

    <Required, MaxLength(100)>
    Public Property Nom As String = ""

    <MaxLength(255)>
    Public Property Email As String

    Public Property DateInscription As Date

    ' Drapeaux pour les filtres globaux nommés (EF Core 10)
    Public Property EstActif As Boolean = True
    Public Property EstSupprime As Boolean = False

    ' Type complexe mappé en colonne JSON (EF Core 10)
    Public Property Profil As Profil

    ' Navigation 1-N : les commandes de ce client
    Public Property Commandes As ICollection(Of Commande) = New List(Of Commande)
End Class

Public Class Commande
    Public Property Id As Integer
    Public Property Reference As String = ""
    Public Property Montant As Decimal
    Public Property DateCommande As Date

    ' Clé étrangère (convention <Navigation>Id) + navigation inverse
    Public Property ClientId As Integer
    Public Property Client As Client

    ' Jeton de concurrence (optimiste) : EF l'inclut dans le WHERE de l'UPDATE
    <ConcurrencyCheck>
    Public Property Version As Integer

    ' Navigation N-N : EF crée la table de jonction automatiquement
    Public Property Produits As ICollection(Of Produit) = New List(Of Produit)
End Class

Public Class Produit
    Public Property Id As Integer
    Public Property Libelle As String = ""
    Public Property Commandes As ICollection(Of Commande) = New List(Of Commande)
End Class

' Type complexe : une valeur SANS identité propre (mappée en JSON)
Public Structure Profil
    Public Property Bio As String
    Public Property SiteWeb As String
End Structure
