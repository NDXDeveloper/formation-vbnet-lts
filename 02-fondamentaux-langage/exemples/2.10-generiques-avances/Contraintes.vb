' ============================================================================
'  Section 2.10 : Génériques avancés
'  Description : Les contraintes de la section : « As {IEntite, New} »
'                (combinaison entre accolades — débloque New T()) et
'                « As IComparable(Of T) » (débloque CompareTo). La classe
'                Client est l'entité concrète servant aux démonstrations.
'  Fichier source : 10-generiques-avances.md
' ============================================================================

''' <summary>Contrat minimal d'une entité (pour la contrainte du Depot).</summary>
Public Interface IEntite
    Property Id As Integer
End Interface

''' <summary>Entité concrète : possède un constructeur public sans paramètre.</summary>
Public Class Client
    Implements IEntite

    Public Property Id As Integer Implements IEntite.Id
End Class

''' <summary>
''' Contraintes combinées entre accolades : T doit implémenter IEntite
''' ET avoir un constructeur public sans paramètre (New en dernier).
''' </summary>
Public Class Depot(Of T As {IEntite, New})
    Public Function Creer() As T
        Return New T()          ' possible grâce à 'As New'
    End Function
End Class

''' <summary>Contrainte d'interface : débloque a.CompareTo(b).</summary>
Public Class Trieur(Of T As IComparable(Of T))
    Public Function EstAvant(a As T, b As T) As Boolean
        Return a.CompareTo(b) < 0   ' possible grâce à 'As IComparable(Of T)'
    End Function
End Class
