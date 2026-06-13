' ============================================================================
'  Section 4.5 : Parallélisme pragmatique (Parallel.For/ForEach, PLINQ)
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · Parallel.For / Parallel.ForEach (écriture dans des cases
'                    distinctes : correct sans verrou) ;
'                  · ParallelOptions (MaxDegreeOfParallelism, annulation) ;
'                  · ParallelLoopState (Stop) + ParallelLoopResult ;
'                  · PLINQ (AsParallel, WithDegreeOfParallelism, ForAll vers
'                    un sac concurrent) ;
'                  · ⚠️ le DANGER n°1 : race condition (somme += ...) -> faux,
'                    puis les trois remèdes : agrégation locale (localInit/
'                    localFinally + Interlocked.Add), résultat EXACT vérifié ;
'                  · AggregateException dans une boucle parallèle ;
'                  · ⚠️ limite VB : Parallel.ForEachAsync via une lambda NON-
'                    async qui enveloppe une Async...As Task dans New ValueTask.
'  Fichier source : 05-parallelisme.md
'  Compilation    : dotnet build      Exécution : dotnet run
'  Note           : les nombres premiers et les sommes sont DÉTERMINISTES ;
'                   seules les durées varient d'une machine à l'autre.
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
        DemoParallelFor()
        DemoParallelOptions()
        DemoLoopState()
        DemoPLinq()
        DemoRaceCondition()
        DemoAgregationLocale()
        DemoExceptionsParalleles()
        Await DemoForEachAsync()
    End Function

    ' ---- Parallel.For : écriture dans des cases distinctes -------------------
    Private Sub DemoParallelFor()
        Console.WriteLine("== Parallel.For (cases distinctes : sûr sans verrou) ==")

        Dim points = Enumerable.Range(0, 8).ToArray()
        Dim resultats(points.Length - 1) As Integer

        Parallel.For(0, points.Length,
                     Sub(i)
                         resultats(i) = points(i) * points(i)   ' chaque thread écrit SA case
                     End Sub)

        Console.WriteLine($"Carrés : {String.Join(", ", resultats)}")
    End Sub

    ' ---- ParallelOptions -----------------------------------------------------------
    Private Sub DemoParallelOptions()
        Console.WriteLine()
        Console.WriteLine("== ParallelOptions (MaxDegreeOfParallelism, annulation) ==")

        Dim valeurs = Enumerable.Range(1, 6).ToArray()
        Dim resultats(valeurs.Length - 1) As Integer

        Dim options As New ParallelOptions With {
            .MaxDegreeOfParallelism = Environment.ProcessorCount
        }

        Parallel.For(0, valeurs.Length, options,
                     Sub(i)
                         resultats(i) = valeurs(i) * 10
                     End Sub)

        Console.WriteLine($"MaxDegreeOfParallelism = {options.MaxDegreeOfParallelism} ; résultats : {String.Join(", ", resultats)}")
    End Sub

    ' ---- ParallelLoopState : interrompre -------------------------------------------
    Private Sub DemoLoopState()
        Console.WriteLine()
        Console.WriteLine("== ParallelLoopState (Stop) ==")

        Dim elements = Enumerable.Range(0, 1000).ToArray()
        Dim cibleTrouvee As Integer = -1

        Dim resultat = Parallel.For(0, elements.Length,
                                    Sub(i, etat)
                                        If elements(i) = 42 Then
                                            cibleTrouvee = elements(i)
                                            etat.Stop()    ' arrête au plus vite
                                        End If
                                    End Sub)

        Console.WriteLine($"Cible trouvée : {cibleTrouvee} ; boucle complétée : {resultat.IsCompleted}")
    End Sub

    ' ---- PLINQ -----------------------------------------------------------------------------
    Private Sub DemoPLinq()
        Console.WriteLine()
        Console.WriteLine("== PLINQ (AsParallel) ==")

        Dim nombres = Enumerable.Range(2, 99_999)   ' 2..100000

        ' Comptage des premiers : résultat DÉTERMINISTE (9592 premiers <= 100000)
        Dim premiers = nombres.AsParallel().Where(AddressOf EstPremier).ToList()
        Console.WriteLine($"Nombre de premiers entre 2 et 100000 : {premiers.Count}")

        ' Opérateurs dédiés + ForAll vers un récepteur thread-safe
        Dim sac As New ConcurrentBag(Of Integer)
        Enumerable.Range(1, 1000).AsParallel().
            WithDegreeOfParallelism(4).
            Where(Function(n) n Mod 100 = 0).
            ForAll(Sub(n) sac.Add(n))
        Console.WriteLine($"Multiples de 100 (1..1000) collectés via ForAll : {sac.Count}")
    End Sub

    Private Function EstPremier(n As Integer) As Boolean
        If n < 2 Then Return False
        For d = 2 To CInt(Math.Sqrt(n))
            If n Mod d = 0 Then Return False
        Next
        Return True
    End Function

    ' ---- Le danger n°1 : race condition --------------------------------------------------------
    Private Sub DemoRaceCondition()
        Console.WriteLine()
        Console.WriteLine("== ⚠️ Race condition (somme += ... non atomique) ==")

        Dim valeurs = Enumerable.Repeat(1L, 1_000_000).ToArray()   ' somme correcte = 1 000 000

        Dim sommeFausse As Long = 0
        Parallel.For(0, valeurs.Length,
                     Sub(i)
                         sommeFausse += valeurs(i)        ' FAUX : lecture-modification-écriture concurrente
                     End Sub)

        Console.WriteLine($"Somme attendue : 1000000")
        Console.WriteLine($"Somme « somme += » (non protégée) : {sommeFausse} -> exacte ? {sommeFausse = 1_000_000}")
        Console.WriteLine("(le résultat est presque toujours FAUX et non déterministe)")
    End Sub

    ' ---- Remède : agrégation locale + Interlocked ---------------------------------------------------
    Private Sub DemoAgregationLocale()
        Console.WriteLine()
        Console.WriteLine("== Remède : agrégation locale par thread (localInit/localFinally) ==")

        Dim valeurs = Enumerable.Repeat(1L, 1_000_000).ToArray()
        Dim somme As Long = 0

        Parallel.For(0, valeurs.Length,
                     Function() 0L,                                       ' init : sous-total local
                     Function(i, etat, sousTotal) sousTotal + valeurs(i), ' accumulation sans verrou
                     Sub(sousTotal) Interlocked.Add(somme, sousTotal))    ' fusion atomique

        Console.WriteLine($"Somme par agrégation locale : {somme} -> exacte ? {somme = 1_000_000}")
    End Sub

    ' ---- Exceptions dans une boucle parallèle -------------------------------------------------------------
    Private Sub DemoExceptionsParalleles()
        Console.WriteLine()
        Console.WriteLine("== AggregateException dans Parallel.For ==")

        Try
            Parallel.For(0, 10,
                         Sub(i)
                             If i Mod 4 = 0 Then Throw New InvalidOperationException($"échec sur i={i}")
                         End Sub)
        Catch ex As AggregateException
            Console.WriteLine($"AggregateException : {ex.InnerExceptions.Count} itération(s) en échec")
        End Try
    End Sub

    ' ---- Parallel.ForEachAsync : la limite VB (New ValueTask) ---------------------------------------------------
    Private Async Function DemoForEachAsync() As Task
        Console.WriteLine()
        Console.WriteLine("== Parallel.ForEachAsync (limite VB : lambda non-async + New ValueTask) ==")

        Dim urls = Enumerable.Range(1, 20).Select(Function(n) $"ressource-{n}").ToArray()
        Dim options As New ParallelOptions With {.MaxDegreeOfParallelism = 5}

        ' ⚠️ VB ne sait pas écrire de lambda Async ... As ValueTask (BC36945).
        ' Le contournement du cours : lambda NON-async qui enveloppe la Task.
        Await Parallel.ForEachAsync(urls, options,
            Function(url, ct) New ValueTask(TraiterUrlAsync(url, ct)))

        Console.WriteLine($"20 ressources traitées, au plus 5 simultanément.")
        Console.WriteLine($"Concurrence maximale observée : {_concurrenceMax} (≤ 5 attendu : {_concurrenceMax <= 5})")
    End Function

    Private _concurrenceActuelle As Integer = 0
    Private _concurrenceMax As Integer = 0

    Private Async Function TraiterUrlAsync(url As String, ct As CancellationToken) As Task
        Dim actuelle = Interlocked.Increment(_concurrenceActuelle)
        ' Mémorise le pic de concurrence (max atomique simplifié)
        Dim ancien = _concurrenceMax
        While actuelle > ancien
            ancien = Interlocked.CompareExchange(_concurrenceMax, actuelle, ancien)
        End While

        Await Task.Delay(40, ct)            ' simule une E/S

        Interlocked.Decrement(_concurrenceActuelle)
    End Function

End Module
