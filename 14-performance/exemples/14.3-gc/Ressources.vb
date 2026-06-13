' ============================================================================
'  Section 14.3 : Garbage Collector — libération déterministe
'  Description : Trois cas. RessourceSimple : on ne détient que de la mémoire
'                managée → Dispose minimal. RessourceNative : détention DIRECTE
'                d'une ressource non managée → pattern Dispose complet (Dispose,
'                Dispose(disposing), Finalize via Overrides, SuppressFinalize).
'                ConnexionAsync : IAsyncDisposable. En VB, le finaliseur se
'                REDÉFINIT (Overrides Sub Finalize), pas de syntaxe ~Classe().
'  Fichier source : 03-gc.md
' ============================================================================

Imports System
Imports System.Threading.Tasks

' Cas simple : Dispose direct, sans finaliseur.
Public Class RessourceSimple
    Implements IDisposable
    Public Property EstLibere As Boolean

    Public Sub Dispose() Implements IDisposable.Dispose
        EstLibere = True
    End Sub
End Class

' Cas complet : détention directe d'une ressource non managée (illustrée par _handle).
Public Class RessourceNative
    Implements IDisposable

    Private _disposed As Boolean = False
    Private _handle As IntPtr = IntPtr.Zero     ' ressource NON managée (exemple)
    Public Property EstLibere As Boolean
    Public Property NbAppelsDispose As Integer

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)                 ' nettoyage fait : pas besoin de finaliser
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        NbAppelsDispose += 1
        If _disposed Then Return                ' idempotent
        If disposing Then
            ' libérer ici les ressources MANAGÉES détenues
        End If
        ' libérer ici les ressources NON managées (_handle…)
        _handle = IntPtr.Zero
        EstLibere = True
        _disposed = True
    End Sub

    ' Finaliseur — FILET DE SÉCURITÉ (syntaxe VB : Overrides Sub Finalize).
    Protected Overrides Sub Finalize()
        Dispose(disposing:=False)
        MyBase.Finalize()
    End Sub
End Class

' Cas asynchrone : IAsyncDisposable.
Public Class ConnexionAsync
    Implements IAsyncDisposable
    Public Property EstLibere As Boolean

    ' ⚠️ Spécificité VB : « Async » ne peut PAS retourner ValueTask (BC36945) ;
    ' seuls Sub, Task ou Task(Of T) sont permis. On enveloppe donc une méthode
    ' Async retournant Task dans un New ValueTask(...).
    Public Function DisposeAsync() As ValueTask Implements IAsyncDisposable.DisposeAsync
        Return New ValueTask(LibererAsync())
    End Function

    Private Async Function LibererAsync() As Task
        Await Task.Delay(5)
        EstLibere = True
    End Function
End Class
