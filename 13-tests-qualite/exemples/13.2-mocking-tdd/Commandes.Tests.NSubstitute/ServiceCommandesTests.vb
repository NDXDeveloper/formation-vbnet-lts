' ============================================================================
'  Section 13.2 : Mocking — NSubstitute (VB.NET)
'  Description : Substitute.For renvoie directement la doublure (consommée telle
'                quelle, sans .Object) ; .Returns préprogramme une réponse ;
'                .Received(n) vérifie le nombre d'appels ; Arg.Any pour
'                n'importe quel argument. Même contrainte que Moq : interfaces
'                ou membres Overridable.
'  Fichier source : 02-mocking-tdd.md
' ============================================================================

Imports Commandes
Imports NSubstitute
Imports Xunit

Public Class ServiceCommandesTests

    <Fact>
    Public Sub Obtenir_CommandeExistante_RetourneLaCommande()
        ' Arrange
        Dim repo = Substitute.For(Of IRepositoryCommandes)()
        repo.Trouver(42).Returns(New Commande With {.Id = 42, .Montant = 99D})
        Dim service As New ServiceCommandes(repo)   ' pas de .Object : repo EST la doublure

        ' Act
        Dim commande = service.Obtenir(42)

        ' Assert
        Assert.Equal(42, commande.Id)
        repo.Received(1).Trouver(42)   ' vérifie l'appel
    End Sub

    <Fact>
    Public Sub Obtenir_NImporteQuelId_UtiliseArgAny()
        Dim repo = Substitute.For(Of IRepositoryCommandes)()
        repo.Trouver(Arg.Any(Of Integer)()).Returns(New Commande With {.Id = 7})
        Dim service As New ServiceCommandes(repo)

        Assert.Equal(7, service.Obtenir(123).Id)
    End Sub

    <Fact>
    Public Sub Creer_AppelleEnregistrer()
        Dim repo = Substitute.For(Of IRepositoryCommandes)()
        Dim service As New ServiceCommandes(repo)
        Dim cmd As New Commande With {.Id = 1}

        service.Creer(cmd)

        repo.Received(1).Enregistrer(cmd)
    End Sub

End Class
