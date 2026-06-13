' ============================================================================
'  Section 17.8 (Cas 3) : le ViewModel VB fonctionne (générateurs inutiles)
'  Description : Preuve que l'approche par classes de base donne ce que les
'                attributs <ObservableProperty>/<RelayCommand> auraient dû produire
'                (et qu'ils ne produisent PAS en VB) : notification de changement et
'                commande exécutable.
'  Fichier source : 08-cas-concrets.md
' ============================================================================

Option Strict On
Option Explicit On

Imports Xunit

Public Class TestsViewModel

    <Fact>
    Public Sub Nom_Modifie_LevePropertyChanged()
        Dim vm As New ClientViewModel()
        Dim proprieteRecue As String = Nothing
        AddHandler vm.PropertyChanged, Sub(expediteur, e) proprieteRecue = e.PropertyName

        vm.Nom = "Alice"

        Assert.Equal("Nom", proprieteRecue)
        Assert.Equal("Alice", vm.Nom)
    End Sub

    <Fact>
    Public Sub Nom_MemeValeur_NeLevePas()
        Dim vm As New ClientViewModel() With {.Nom = "X"}   ' positionne _nom avant l'abonnement
        Dim compteur As Integer = 0
        AddHandler vm.PropertyChanged, Sub(expediteur, e) compteur += 1

        vm.Nom = "X"   ' valeur identique : SetProperty renvoie False, aucun événement

        Assert.Equal(0, compteur)
    End Sub

    <Fact>
    Public Sub EnregistrerCommand_Execute_InvoqueLaMethode()
        Dim vm As New ClientViewModel()

        vm.EnregistrerCommand.Execute(Nothing)
        vm.EnregistrerCommand.Execute(Nothing)

        Assert.Equal(2, vm.Enregistrements)
    End Sub

End Class
