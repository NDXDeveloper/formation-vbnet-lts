' ============================================================================
'  Section 6.6 : Architecture MVVM
'  Description : Le view-model « à la main » de la section (ViewModelBase +
'                RelayCommand). Il expose une PROPRIÉTÉ (Nom) et une COMMANDE
'                (EnregistrerCommand, désactivée tant que Nom est vide). Le
'                view-model IGNORE la vue : aucune référence à un contrôle.
'  Fichier source : 06-mvvm.md
' ============================================================================

Public Class ClientViewModel
    Inherits ViewModelBase

    Private _nom As String
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            If _nom <> value Then
                _nom = value
                OnPropertyChanged()
                EnregistrerCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property

    Public Property DernierEnregistre As String

    Public ReadOnly Property EnregistrerCommand As RelayCommand

    Public Sub New()
        _EnregistrerCommand = New RelayCommand(AddressOf Enregistrer, AddressOf PeutEnregistrer)
    End Sub

    Private Sub Enregistrer()
        DernierEnregistre = Nom        ' logique métier (ici, on mémorise)
        OnPropertyChanged(NameOf(DernierEnregistre))
    End Sub

    Private Function PeutEnregistrer() As Boolean
        Return Not String.IsNullOrWhiteSpace(Nom)
    End Function
End Class
