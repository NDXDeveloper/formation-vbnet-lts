' ============================================================================
'  Section 13.2 : Mocking — Moq (VB.NET)
'  Description : Setup/Returns pour préprogrammer une réponse, Verify/Times pour
'                vérifier un appel, It.IsAny pour les correspondances
'                d'arguments, .Throws pour simuler une erreur. La doublure réelle
'                est exposée par .Object. Les méthodes Sub se configurent/vérifient
'                avec une lambda Sub(...).
'  Fichier source : 02-mocking-tdd.md
' ============================================================================

Imports System
Imports Commandes
Imports Moq
Imports Xunit

Public Class ServiceCommandesTests

    <Fact>
    Public Sub Obtenir_CommandeExistante_RetourneLaCommande()
        ' Arrange
        Dim mockRepo As New Mock(Of IRepositoryCommandes)()
        mockRepo.Setup(Function(r) r.Trouver(42)) _
                .Returns(New Commande With {.Id = 42, .Montant = 99D})
        Dim service As New ServiceCommandes(mockRepo.Object)   ' .Object = la doublure

        ' Act
        Dim commande = service.Obtenir(42)

        ' Assert
        Assert.Equal(42, commande.Id)
        mockRepo.Verify(Function(r) r.Trouver(42), Times.Once())   ' appelé exactement une fois
    End Sub

    <Fact>
    Public Sub Obtenir_NImporteQuelId_UtiliseItIsAny()
        Dim mockRepo As New Mock(Of IRepositoryCommandes)()
        mockRepo.Setup(Function(r) r.Trouver(It.IsAny(Of Integer)())) _
                .Returns(New Commande With {.Id = 7})
        Dim service As New ServiceCommandes(mockRepo.Object)

        Assert.Equal(7, service.Obtenir(123).Id)
    End Sub

    <Fact>
    Public Sub Creer_AppelleEnregistrerUneFois()
        Dim mockRepo As New Mock(Of IRepositoryCommandes)()
        Dim service As New ServiceCommandes(mockRepo.Object)

        service.Creer(New Commande With {.Id = 1})

        ' Vérification d'un appel à une méthode Sub : lambda Sub(...)
        mockRepo.Verify(Sub(r) r.Enregistrer(It.IsAny(Of Commande)()), Times.Once())
    End Sub

    <Fact>
    Public Sub Obtenir_DepotEnErreur_PropageLException()
        Dim mockRepo As New Mock(Of IRepositoryCommandes)()
        mockRepo.Setup(Function(r) r.Trouver(It.IsAny(Of Integer)())) _
                .Throws(Of InvalidOperationException)()
        Dim service As New ServiceCommandes(mockRepo.Object)

        Assert.Throws(Of InvalidOperationException)(Sub() service.Obtenir(1))
    End Sub

End Class
