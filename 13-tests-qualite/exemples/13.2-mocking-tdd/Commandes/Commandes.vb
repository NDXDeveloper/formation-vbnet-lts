' ============================================================================
'  Section 13.2 : Mocking et TDD — système sous test
'  Description : ServiceCommandes reçoit son dépôt par CONSTRUCTEUR (injection
'                de dépendances), via l'interface IRepositoryCommandes. Cette
'                conception (issue d'une démarche TDD : rouge → vert → refactor)
'                rend l'unité ISOLABLE : en test, le dépôt réel est remplacé par
'                une doublure (Moq / NSubstitute).
'  Fichier source : 02-mocking-tdd.md
' ============================================================================

Public Class Commande
    Public Property Id As Integer
    Public Property Montant As Decimal
End Class

Public Interface IRepositoryCommandes
    Function Trouver(id As Integer) As Commande
    Sub Enregistrer(commande As Commande)
End Interface

Public Class ServiceCommandes
    Private ReadOnly _repo As IRepositoryCommandes

    Public Sub New(repo As IRepositoryCommandes)
        _repo = repo
    End Sub

    Public Function Obtenir(id As Integer) As Commande
        Return _repo.Trouver(id)
    End Function

    Public Sub Creer(commande As Commande)
        _repo.Enregistrer(commande)
    End Sub
End Class
