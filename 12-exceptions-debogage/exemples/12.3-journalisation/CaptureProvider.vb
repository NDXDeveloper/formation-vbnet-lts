' ============================================================================
'  Section 12.3 : Journalisation — fournisseur en mémoire (preuve de structure)
'  Description : Un ILoggerProvider/ILogger maison qui CAPTURE chaque log avec
'                ses PROPRIÉTÉS NOMMÉES (le state d'un log structuré implémente
'                IReadOnlyList(Of KeyValuePair)). Il permet de PROUVER, hors
'                d'un backend externe, que « Commande {CommandeId} … » conserve
'                CommandeId comme propriété requêtable — et non comme texte plat.
'  Fichier source : 03-journalisation.md
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.Extensions.Logging

Public Class EntreeLog
    Public Property Niveau As LogLevel
    Public Property Message As String
    Public Property Proprietes As Dictionary(Of String, Object)
    Public Property AvecException As Boolean
End Class

Public NotInheritable Class CaptureProvider
    Implements ILoggerProvider

    Public ReadOnly Entrees As New List(Of EntreeLog)

    Public Function CreateLogger(categoryName As String) As ILogger Implements ILoggerProvider.CreateLogger
        Return New CaptureLogger(Entrees)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class

Friend NotInheritable Class CaptureLogger
    Implements ILogger

    Private ReadOnly _entrees As List(Of EntreeLog)

    Public Sub New(entrees As List(Of EntreeLog))
        _entrees = entrees
    End Sub

    Public Function BeginScope(Of TState)(state As TState) As IDisposable Implements ILogger.BeginScope
        Return PorteeVide.Instance
    End Function

    Public Function IsEnabled(logLevel As LogLevel) As Boolean Implements ILogger.IsEnabled
        Return True   ' le filtrage par niveau est appliqué en amont par la fabrique
    End Function

    Public Sub Log(Of TState)(logLevel As LogLevel, eventId As EventId, state As TState,
                              exception As Exception, formatter As Func(Of TState, Exception, String)) Implements ILogger.Log
        Dim props As New Dictionary(Of String, Object)
        Dim paires = TryCast(state, IReadOnlyList(Of KeyValuePair(Of String, Object)))
        If paires IsNot Nothing Then
            For Each kv In paires
                props(kv.Key) = kv.Value
            Next
        End If

        _entrees.Add(New EntreeLog With {
            .Niveau = logLevel,
            .Message = formatter(state, exception),
            .Proprietes = props,
            .AvecException = exception IsNot Nothing
        })
    End Sub
End Class

Friend NotInheritable Class PorteeVide
    Implements IDisposable
    Public Shared ReadOnly Instance As New PorteeVide()
    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class
