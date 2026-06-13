' ============================================================================
'  Section 6.6 : Architecture MVVM
'  Description : La variante CommunityToolkit.Mvvm de la section : on hérite
'                d'ObservableObject (méthode SetProperty) et l'on utilise le
'                RelayCommand de la bibliothèque. ⚠️ Les *source generators*
'                ([ObservableProperty]/[RelayCommand]) sont C# UNIQUEMENT ;
'                en VB on écrit la propriété complète et on instancie le
'                RelayCommand — un cran plus verbeux, sans perte de
'                fonctionnalité. Les types du toolkit sont qualifiés
'                explicitement pour éviter toute ambiguïté avec le
'                RelayCommand « maison » de cet exemple.
'  Fichier source : 06-mvvm.md
' ============================================================================

Imports CommunityToolkit.Mvvm.ComponentModel

Public Class ClientViewModelToolkit
    Inherits ObservableObject

    Private _nom As String
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            If SetProperty(_nom, value) Then            ' notifie l'UI si la valeur change
                EnregistrerCommand.NotifyCanExecuteChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property EnregistrerCommand As CommunityToolkit.Mvvm.Input.IRelayCommand

    Public Sub New()
        _EnregistrerCommand = New CommunityToolkit.Mvvm.Input.RelayCommand(
            AddressOf Enregistrer, AddressOf PeutEnregistrer)
    End Sub

    Private Sub Enregistrer()
        ' logique métier de sauvegarde
    End Sub

    Private Function PeutEnregistrer() As Boolean
        Return Not String.IsNullOrWhiteSpace(Nom)
    End Function
End Class
