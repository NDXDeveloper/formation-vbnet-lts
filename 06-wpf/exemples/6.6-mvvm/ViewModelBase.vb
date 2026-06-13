' ============================================================================
'  Section 6.6 : Architecture MVVM
'  Description : La classe de base « à la main » de la section : implémente
'                INotifyPropertyChanged une fois pour toutes, avec
'                <CallerMemberName> (le nom de la propriété est injecté).
'  Fichier source : 06-mvvm.md
' ============================================================================

Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public MustInherit Class ViewModelBase
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propriete As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propriete))
    End Sub
End Class
