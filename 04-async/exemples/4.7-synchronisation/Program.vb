' ============================================================================
'  Section 4.7 : Synchronisation et thread-safety (SyncLock, Interlocked, SemaphoreSlim)
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · le problème : compteur += 1 non atomique -> résultat
'                    FAUX (vérifié) ;
'                  · SyncLock sur un objet privé dédié -> résultat EXACT ;
'                  · Interlocked.Increment -> exact et lock-free ;
'                  · SemaphoreSlim(1, 1) : le verrou ASYNCHRONE (WaitAsync /
'                    Release) — ce que SyncLock ne sait pas faire ;
'                  · SemaphoreSlim(N) : throttling (au plus N simultanés,
'                    pic mesuré) ;
'                  · éviter le verrou : ConcurrentDictionary.GetOrAdd.
'  Fichier source : 07-synchronisation.md
'  Compilation    : dotnet build      Exécution : dotnet run
'  Note           : les totaux sont DÉTERMINISTES (sauf le compteur « faux »,
'                   dont l'inexactitude est précisément le sujet).
' ============================================================================

Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        DemoProbleme()
        DemoSyncLock()
        DemoInterlocked()
        Await DemoVerrouAsync()
        Await DemoThrottling()
        DemoEviterLeVerrou()
    End Function

    Private Const N As Integer = 100_000

    ' ---- Le problème : état mutable partagé non protégé ----------------------
    Private Sub DemoProbleme()
        Console.WriteLine("== ⚠️ Le problème : compteur += 1 non atomique ==")

        Dim compteur As Integer = 0
        Parallel.For(0, N, Sub(i) compteur += 1)    ' race condition

        Console.WriteLine($"Attendu : {N} ; obtenu : {compteur} -> exact ? {compteur = N}")
        Console.WriteLine("(presque toujours inférieur : des incréments sont perdus)")
    End Sub

    ' ---- SyncLock : exclusion mutuelle synchrone -----------------------------------
    Private _verrou As New Object()
    Private _compteurVerrou As Integer = 0

    Private Sub DemoSyncLock()
        Console.WriteLine()
        Console.WriteLine("== SyncLock (objet privé dédié) ==")

        _compteurVerrou = 0
        Parallel.For(0, N,
                     Sub(i)
                         SyncLock _verrou
                             _compteurVerrou += 1     ' un seul thread à la fois
                         End SyncLock
                     End Sub)

        Console.WriteLine($"Attendu : {N} ; obtenu : {_compteurVerrou} -> exact ? {_compteurVerrou = N}")
    End Sub

    ' ---- Interlocked : atomique et lock-free -----------------------------------------
    Private Sub DemoInterlocked()
        Console.WriteLine()
        Console.WriteLine("== Interlocked.Increment (atomique, sans verrou) ==")

        Dim compteur As Integer = 0
        Parallel.For(0, N, Sub(i) Interlocked.Increment(compteur))

        Console.WriteLine($"Attendu : {N} ; obtenu : {compteur} -> exact ? {compteur = N}")
    End Sub

    ' ---- SemaphoreSlim(1, 1) : le verrou ASYNCHRONE ----------------------------------------
    Private ReadOnly _verrouAsync As New SemaphoreSlim(1, 1)
    Private _ressourcePartagee As Integer = 0

    Private Async Function DemoVerrouAsync() As Task
        Console.WriteLine()
        Console.WriteLine("== SemaphoreSlim(1, 1) : section critique AVEC Await ==")

        ' 50 tâches concurrentes incrémentent une ressource partagée,
        ' chacune via une section critique qui contient un Await.
        _ressourcePartagee = 0
        Dim taches = Enumerable.Range(0, 50).Select(Function(i) MettreAJourAsync()).ToArray()
        Await Task.WhenAll(taches)

        Console.WriteLine($"Attendu : 50 ; obtenu : {_ressourcePartagee} -> exact ? {_ressourcePartagee = 50}")
        Console.WriteLine("(SyncLock interdirait l'Await ici ; SemaphoreSlim l'autorise)")
    End Function

    Private Async Function MettreAJourAsync() As Task
        Await _verrouAsync.WaitAsync()        ' attente du jeton (HORS Try)
        Try
            Dim lu = _ressourcePartagee
            Await Task.Delay(1)               ' on PEUT attendre dans la section critique
            _ressourcePartagee = lu + 1
        Finally
            _verrouAsync.Release()            ' synchrone -> permis dans Finally
        End Try
    End Function

    ' ---- SemaphoreSlim(N) : throttling --------------------------------------------------------
    Private ReadOnly _limiteur As New SemaphoreSlim(5)
    Private _enCours As Integer = 0
    Private _picSimultane As Integer = 0

    Private Async Function DemoThrottling() As Task
        Console.WriteLine()
        Console.WriteLine("== SemaphoreSlim(5) : au plus 5 opérations simultanées ==")

        Dim taches = Enumerable.Range(1, 30).Select(Function(i) TelechargerAsync($"url-{i}")).ToArray()
        Await Task.WhenAll(taches)

        Console.WriteLine($"30 «téléchargements» terminés ; pic de concurrence : {_picSimultane} (≤ 5 : {_picSimultane <= 5})")
    End Function

    Private Async Function TelechargerAsync(url As String) As Task
        Await _limiteur.WaitAsync()
        Try
            Dim actuel = Interlocked.Increment(_enCours)
            Dim ancien = _picSimultane
            While actuel > ancien
                ancien = Interlocked.CompareExchange(_picSimultane, actuel, ancien)
            End While
            Await Task.Delay(30)
            Interlocked.Decrement(_enCours)
        Finally
            _limiteur.Release()
        End Try
    End Function

    ' ---- Éviter le verrou : ConcurrentDictionary --------------------------------------------------
    Private Sub DemoEviterLeVerrou()
        Console.WriteLine()
        Console.WriteLine("== Mieux : éviter le verrou (ConcurrentDictionary) ==")

        Dim cache As New ConcurrentDictionary(Of Integer, Integer)()
        Parallel.For(0, 1000,
                     Sub(i)
                         ' GetOrAdd : thread-safe, sans verrou explicite
                         cache.GetOrAdd(i Mod 10, Function(cle) cle * cle)
                     End Sub)

        Console.WriteLine($"Clés distinctes dans le cache : {cache.Count} (attendu : 10)")
        Console.WriteLine($"cache(3) = {cache(3)} (= 3 * 3)")
    End Sub

End Module
