' ============================================================================
'  Section 6.4 : Liaison de données
'  Description : La source de liaison de la section : Client implémente
'                INotifyPropertyChanged avec une méthode NotifierChangement
'                exploitant <CallerMemberName> (le nom de la propriété est
'                injecté par le compilateur). Indispensable pour qu'une
'                modification en code se répercute à l'écran.
'  Fichier source : 04-data-binding.md
' ============================================================================

Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class Client
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private _nom As String
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            If _nom <> value Then
                _nom = value
                NotifierChangement()   ' transmet "Nom" automatiquement
            End If
        End Set
    End Property

    Private _ville As String
    Public Property Ville As String
        Get
            Return _ville
        End Get
        Set(value As String)
            If _ville <> value Then
                _ville = value
                NotifierChangement()
            End If
        End Set
    End Property

    Private _estActif As Boolean
    Public Property EstActif As Boolean
        Get
            Return _estActif
        End Get
        Set(value As Boolean)
            If _estActif <> value Then
                _estActif = value
                NotifierChangement()
            End If
        End Set
    End Property

    ' Propriété simple (sans notification) pour la démonstration de StringFormat.
    Public Property DateInscription As Date = Date.Today

    Private Sub NotifierChangement(<CallerMemberName> Optional propriete As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propriete))
    End Sub
End Class
