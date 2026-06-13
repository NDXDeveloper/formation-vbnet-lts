' ============================================================================
'  Section 14.3 : Garbage Collector — démonstration
'  Description : Using → Dispose garanti ; pattern Dispose complet (idempotent) ;
'                IAsyncDisposable libéré « à la main » (Await hors du Try, motif
'                capturer/libérer/relancer) ; générations (promotion d'un objet
'                survivant, comptage des collectes gen0) ; mode GC effectif.
'  Fichier source : 03-gc.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Runtime
Imports System.Runtime.ExceptionServices
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return MainAsync().GetAwaiter().GetResult()
    End Function

    Async Function MainAsync() As Task(Of Integer)
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 14.3 Garbage Collector ===")
        Console.WriteLine()

        ' 1) Using : Dispose garanti à la sortie du bloc.
        Dim ressource As New RessourceSimple()
        Using ressource
            ' … utilisation …
        End Using
        Console.WriteLine($"[Using] après le bloc : EstLibere={ressource.EstLibere}")
        Console.WriteLine()

        ' 2) Pattern Dispose complet (idempotent).
        Dim native As New RessourceNative()
        native.Dispose()
        native.Dispose()   ' second appel : sans effet (idempotent)
        Console.WriteLine($"[Dispose complet] EstLibere={native.EstLibere} ; appels Dispose(disposing)={native.NbAppelsDispose}")
        Console.WriteLine()

        ' 3) IAsyncDisposable libéré à la main (Await interdit dans Finally).
        Dim cnx As New ConnexionAsync()
        Dim erreur As ExceptionDispatchInfo = Nothing
        Try
            ' … utilisation asynchrone …
        Catch ex As Exception
            erreur = ExceptionDispatchInfo.Capture(ex)
        End Try
        Await cnx.DisposeAsync()     ' hors du Try : Await autorisé
        erreur?.Throw()
        Console.WriteLine($"[IAsyncDisposable] EstLibere={cnx.EstLibere}")
        Console.WriteLine()

        ' 4) Générations : un objet survivant est PROMU.
        Dim survivant As New Object()
        Dim genAvant = GC.GetGeneration(survivant)
        GC.Collect()
        GC.WaitForPendingFinalizers()
        Dim genApres = GC.GetGeneration(survivant)
        Console.WriteLine($"[Générations] gen initiale={genAvant} ; après GC.Collect={genApres} ; promu={genApres > genAvant}")

        ' Allocations éphémères → collectes Gen 0.
        Dim gen0Avant = GC.CollectionCount(0)
        Dim garde As Object = Nothing
        For i = 1 To 2_000_000
            garde = New Byte(15) {}   ' échappe (stocké) → vraie allocation sur le tas
        Next
        GC.KeepAlive(garde)
        Console.WriteLine($"              collectes Gen 0 déclenchées = {GC.CollectionCount(0) - gen0Avant} (> 0)")
        Console.WriteLine()

        ' 5) Mode GC effectif (issu de la configuration MSBuild).
        Console.WriteLine($"[Mode GC] IsServerGC={GCSettings.IsServerGC} ; LatencyMode={GCSettings.LatencyMode}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
        Return 0
    End Function

End Module
