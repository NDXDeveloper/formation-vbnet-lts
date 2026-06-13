' ============================================================================
'  Section 6.6 : Architecture MVVM
'  Description : Le RelayCommand « à la main » de la section : une
'                implémentation générique d'ICommand encapsulant de simples
'                délégués (Execute / CanExecute). Le contrôle lié se
'                désactive de lui-même quand CanExecute renvoie False.
'  Fichier source : 06-mvvm.md
' ============================================================================

Imports System.Windows.Input

Public Class RelayCommand
    Implements ICommand

    Private ReadOnly _executer As Action
    Private ReadOnly _peutExecuter As Func(Of Boolean)

    Public Sub New(executer As Action, Optional peutExecuter As Func(Of Boolean) = Nothing)
        _executer = executer
        _peutExecuter = peutExecuter
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean _
            Implements ICommand.CanExecute
        Return _peutExecuter Is Nothing OrElse _peutExecuter()
    End Function

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _executer()
    End Sub

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub
End Class
