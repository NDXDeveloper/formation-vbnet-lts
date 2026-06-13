' ============================================================================
'  Section 3.4 : Interfaces
'  Description : Les interfaces génériques de la section : Temperature
'                implémente IComparable(Of Temperature) (tri par List.Sort),
'                et IDepot(Of T) — interface générique maison — est
'                implémentée par DepotClients avec l'argument de type Client.
'  Fichier source : 04-interfaces.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

''' <summary>Implémenter IComparable(Of T) rend le type triable.</summary>
Public Class Temperature
    Implements IComparable(Of Temperature)

    Public ReadOnly Property Celsius As Double

    Public Sub New(celsius As Double)
        Me.Celsius = celsius
    End Sub

    Public Function CompareTo(other As Temperature) As Integer _
            Implements IComparable(Of Temperature).CompareTo
        Return Celsius.CompareTo(other.Celsius)
    End Function

    Public Overrides Function ToString() As String
        Return $"{Celsius} °C"
    End Function
End Class

Public Class Client
    Public Property Id As Integer
    Public Property Nom As String
End Class

''' <summary>Interface générique maison : le contrat d'un dépôt de données.</summary>
Public Interface IDepot(Of T)
    Function ObtenirParId(id As Integer) As T
    Sub Ajouter(element As T)
    Function ObtenirTout() As IEnumerable(Of T)
End Interface

''' <summary>Implémentation : l'argument de type est fixé à Client.</summary>
Public Class DepotClients
    Implements IDepot(Of Client)

    Private ReadOnly _clients As New List(Of Client)

    Public Function ObtenirParId(id As Integer) As Client _
            Implements IDepot(Of Client).ObtenirParId
        Return _clients.FirstOrDefault(Function(c) c.Id = id)
    End Function

    Public Sub Ajouter(element As Client) Implements IDepot(Of Client).Ajouter
        _clients.Add(element)
    End Sub

    Public Function ObtenirTout() As IEnumerable(Of Client) _
            Implements IDepot(Of Client).ObtenirTout
        Return _clients
    End Function
End Class
