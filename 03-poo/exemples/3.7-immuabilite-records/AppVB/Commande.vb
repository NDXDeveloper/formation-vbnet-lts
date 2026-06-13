' ============================================================================
'  Section 3.7 : Types immuables et records
'  Description : L'immuabilité PROFONDE de la section : un champ ReadOnly qui
'                référencerait une List(Of T) resterait modifiable en
'                contenu ; ImmutableArray (System.Collections.Immutable,
'                inclus dans .NET) fige réellement la collection.
'  Fichier source : 07-immuabilite-records.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Collections.Immutable

Public Class LigneCommande
    Public ReadOnly Property Libelle As String
    Public ReadOnly Property Quantite As Integer

    Public Sub New(libelle As String, quantite As Integer)
        Me.Libelle = libelle
        Me.Quantite = quantite
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Quantite} × {Libelle}"
    End Function
End Class

Public Class Commande
    Private ReadOnly _lignes As ImmutableArray(Of LigneCommande)

    Public Sub New(lignes As IEnumerable(Of LigneCommande))
        _lignes = lignes.ToImmutableArray()    ' copie réellement immuable
    End Sub

    Public ReadOnly Property Lignes As ImmutableArray(Of LigneCommande)
        Get
            Return _lignes
        End Get
    End Property
End Class
