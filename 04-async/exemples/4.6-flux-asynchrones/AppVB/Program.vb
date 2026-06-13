' ============================================================================
'  Section 4.6 : Consommer IAsyncEnumerable / flux asynchrones ; ValueTask
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · ⚠️ VB n'a pas d'« Await For Each » : on déroule
'                    l'énumérateur à la main (GetAsyncEnumerator ->
'                    Do While Await MoveNextAsync() -> Current) ;
'                  · le VRAI point dur : libération sûre face aux exceptions
'                    — pas d'Await en Finally, pas d'Await Using — via le motif
'                    capture / DisposeAsync HORS Try / relance par
'                    ExceptionDispatchInfo (flux qui échoue au 3e élément) ;
'                  · option pragmatique : matérialiser si pas besoin de
'                    diffuser ;
'                  · ValueTask : on l'attend (MoveNextAsync), conversion
'                    .AsTask() si besoin de l'attendre plusieurs fois ;
'                  · IAsyncDisposable en VB (ConnexionAsync.vb).
'  Fichier source : 06-async-streams.md
'  Compilation    : dotnet build FluxAsynchrones.sln
'  Exécution      : dotnet run --project AppVB
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Runtime.ExceptionServices
Imports System.Threading
Imports System.Threading.Tasks
Imports FluxCsharp

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        Await DemoConsommationManuelle()
        Await DemoLiberationSure()
        Await DemoMaterialiser()
        Await DemoValueTask()
        Await DemoAsyncDisposable()
    End Function

    ' ---- Consommer un flux à la main (pas d'Await For Each en VB) ------------
    Private Async Function DemoConsommationManuelle() As Task
        Console.WriteLine("== Consommer un flux asynchrone (déroulé manuel) ==")

        Dim flux = Capteur.MesurerAsync()
        Dim enumerateur = flux.GetAsyncEnumerator()
        Dim recues As New List(Of Integer)
        Try
            Do While Await enumerateur.MoveNextAsync()      ' Await ValueTask(Of Boolean)
                recues.Add(enumerateur.Current)
            Loop
        Finally
            ' (libération minimale ici ; le motif sûr est démontré juste après)
        End Try
        Await enumerateur.DisposeAsync()

        Console.WriteLine($"Mesures reçues une à une : {String.Join(", ", recues)}")
    End Function

    ' ---- Libération sûre face aux exceptions ---------------------------------------
    Private Async Function DemoLiberationSure() As Task
        Console.WriteLine()
        Console.WriteLine("== Libération sûre (capture / DisposeAsync hors Try / relance) ==")

        Dim recues As New List(Of Integer)
        Dim erreur As String = Nothing
        Try
            Await ConsommerAvecLiberationSureAsync(Capteur.MesurerAvecEchecAsync(), recues)
        Catch ex As InvalidOperationException
            erreur = ex.Message                ' l'exception d'origine est bien relancée
        End Try

        Console.WriteLine($"Reçues avant l'échec : {String.Join(", ", recues)}")
        Console.WriteLine($"Exception relancée (pile préservée) : {erreur}")
    End Function

    ''' <summary>Le motif EXACT du cours : capturer, libérer hors Try, relancer.</summary>
    Private Async Function ConsommerAvecLiberationSureAsync(flux As IAsyncEnumerable(Of Integer),
                                                            recues As List(Of Integer)) As Task
        Dim enumerateur = flux.GetAsyncEnumerator()
        Dim capture As ExceptionDispatchInfo = Nothing
        Try
            Do While Await enumerateur.MoveNextAsync()
                recues.Add(enumerateur.Current)
            Loop
        Catch ex As Exception
            capture = ExceptionDispatchInfo.Capture(ex)    ' on mémorise (pas d'Await ici)
        End Try

        Console.WriteLine("  -> DisposeAsync appelé HORS du Try (Await y est de nouveau permis)")
        Await enumerateur.DisposeAsync()                   ' Await autorisé : hors Try/Catch/Finally
        capture?.Throw()                                   ' relance en conservant la pile
    End Function

    ' ---- Option pragmatique : matérialiser ------------------------------------------------
    Private Async Function DemoMaterialiser() As Task
        Console.WriteLine()
        Console.WriteLine("== Option pragmatique : matérialiser si pas besoin de diffuser ==")

        ' Si l'on n'a pas besoin du traitement incrémental, on collecte tout.
        Dim tout As New List(Of Integer)
        Dim enumerateur = Capteur.MesurerAsync().GetAsyncEnumerator()
        Try
            Do While Await enumerateur.MoveNextAsync()
                tout.Add(enumerateur.Current)
            Loop
        Finally
        End Try
        Await enumerateur.DisposeAsync()

        Console.WriteLine($"Collection matérialisée : [{String.Join(", ", tout)}] (somme = {tout.Sum()})")
    End Function

    ' ---- ValueTask : attendre une fois, .AsTask() sinon -----------------------------------------
    Private Async Function DemoValueTask() As Task
        Console.WriteLine()
        Console.WriteLine("== ValueTask (notions) ==")

        Dim enumerateur = Capteur.MesurerAsync().GetAsyncEnumerator()

        ' MoveNextAsync renvoie un ValueTask(Of Boolean) : on l'attend une fois.
        Dim premier = Await enumerateur.MoveNextAsync()
        Console.WriteLine($"Premier MoveNextAsync (ValueTask attendu une fois) : {premier} ; Current = {enumerateur.Current}")

        ' Pour l'attendre plusieurs fois / le composer : conversion en Task via .AsTask()
        Dim commeTask As Task(Of Boolean) = enumerateur.MoveNextAsync().AsTask()
        Console.WriteLine($"ValueTask converti en Task via .AsTask() : {Await commeTask}")

        Await enumerateur.DisposeAsync()
    End Function

    ' ---- IAsyncDisposable en VB ----------------------------------------------------------------------
    Private Async Function DemoAsyncDisposable() As Task
        Console.WriteLine()
        Console.WriteLine("== Implémenter IAsyncDisposable en VB (ValueTask à la main) ==")

        ' Pas d'« Await Using » en VB : on appelle DisposeAsync explicitement.
        Dim simple As New RessourceSimple()
        Await simple.DisposeAsync()

        Dim connexion As New ConnexionAsync()
        Await connexion.DisposeAsync()
    End Function

End Module
