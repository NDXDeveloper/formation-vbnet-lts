' ============================================================================
'  Section 17.8 (Cas 3) : ViewModel MVVM correct en VB (sans source generator)
'  Description : Ce que l'IA propose (et qui est INERTE en VB) :
'
'                    ' ❌ Source generators C#-only : reconnus mais SANS EFFET en VB.
'                    ' <ObservableProperty>
'                    ' Private _nom As String
'                    ' <RelayCommand>
'                    ' Private Sub Enregistrer()
'                    ' End Sub
'
'                En VB, on hérite d'ObservableObject et on appelle SetProperty
'                (qui lève PropertyChanged), et on instancie RelayCommand à la main.
'  Fichier source : 08-cas-concrets.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Windows.Input
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

''' <summary>ViewModel de démonstration : propriété observable + commande, écrits à la main.</summary>
Public Class ClientViewModel
    Inherits ObservableObject

    Private _nom As String = ""
    ''' <summary>Nom du client ; lève PropertyChanged via SetProperty lorsqu'il change.</summary>
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            SetProperty(_nom, value)          ' fourni par ObservableObject (lève PropertyChanged si différent)
        End Set
    End Property

    ''' <summary>Nombre de fois où la commande Enregistrer a été exécutée.</summary>
    Public ReadOnly Property Enregistrements As Integer
        Get
            Return _enregistrements
        End Get
    End Property
    Private _enregistrements As Integer

    ''' <summary>Commande liable en XAML ; instanciée à la main (pas de générateur).</summary>
    Public ReadOnly Property EnregistrerCommand As ICommand =
        New RelayCommand(AddressOf Enregistrer)

    Private Sub Enregistrer()
        _enregistrements += 1
    End Sub

End Class
