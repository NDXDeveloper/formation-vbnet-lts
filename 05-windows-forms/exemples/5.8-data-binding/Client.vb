' ============================================================================
'  Section 5.8 : Liaison de données WinForms
'  Description : Le modèle liable de la section : Client implémente
'                INotifyPropertyChanged et lève PropertyChanged à chaque
'                changement — indispensable pour que l'édition par code se
'                répercute à l'écran. Commande sert au schéma maître-détail.
'  Fichier source : 08-data-binding.md
' ============================================================================

Imports System.Collections.Generic
Imports System.ComponentModel

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
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Nom)))
            End If
        End Set
    End Property

    Private _email As String
    Public Property Email As String
        Get
            Return _email
        End Get
        Set(value As String)
            If _email <> value Then
                _email = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Email)))
            End If
        End Set
    End Property

    ' Propriété de navigation pour le maître-détail
    Public Property Commandes As New List(Of Commande)
End Class

Public Class Commande
    Public Property Reference As String
    Public Property Montant As Decimal
End Class
