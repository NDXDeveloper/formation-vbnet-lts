' ============================================================================
'  Section 4.3 : Gestion des exceptions asynchrones
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · l'exception voyage dans la Task et n'est relancée qu'à
'                    l'Await -> Try/Catch ordinaire ;
'                  · Finally et Using s'exécutent même si l'Await échoue ;
'                  · ⚠️ Await interdit DANS Catch/Finally (BC36943, vérifiée) :
'                    le contournement capture-puis-Await-après, à l'identique ;
'                  · les filtres Catch ... When ⭐ sur
'                    HttpRequestException.StatusCode (404 = cas métier) ;
'                  · même les exceptions SYNCHRONES d'une méthode Async
'                    sortent à l'Await, pas à l'appel (vérifié) ;
'                  · Task.WhenAll : l'Await relance la PREMIÈRE exception,
'                    toutes sont dans Exception.InnerExceptions ;
'                  · .Result livre une AggregateException ;
'                    GetAwaiter().GetResult() livre l'exception d'origine ;
'                  · le piège Async Sub : Try/Catch OBLIGATOIRE à l'intérieur.
'  Fichier source : 03-exceptions-async.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Net
Imports System.Net.Http
Imports System.Threading

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        Await DemoTryCatchAwait()
        Await DemoFinallyEtUsing()
        Await DemoAwaitHorsCatch()
        Await DemoFiltresWhen()
        Await DemoExceptionSynchrone()
        Await DemoWhenAllMultiple()
        DemoDeballage()
        Await DemoAsyncSub()
    End Function

    ' ---- L'exception est relancée à l'Await -----------------------------------
    Private Async Function DemoTryCatchAwait() As Task
        Console.WriteLine("== Try/Catch autour d'Await ==")
        Try
            Dim texte = Await ChargerTexteAsync(echouer:=True)
            Console.WriteLine(texte)
        Catch ex As InvalidOperationException
            Console.WriteLine($"Catch -> {ex.GetType().Name} : {ex.Message}")
        End Try
    End Function

    Private Async Function ChargerTexteAsync(echouer As Boolean) As Task(Of String)
        Await Task.Delay(50)
        If echouer Then Throw New InvalidOperationException("Échec réseau simulé.")
        Return "contenu"
    End Function

    ' ---- Finally et Using face à un Await en échec ---------------------------------
    Private Async Function DemoFinallyEtUsing() As Task
        Console.WriteLine()
        Console.WriteLine("== Finally et Using s'exécutent malgré l'échec ==")
        Try
            Using ressource As New RessourceTracee()
                Await ChargerTexteAsync(echouer:=True)   ' échoue ici…
            End Using                                     ' …la ressource est bien libérée
        Catch ex As InvalidOperationException
            Console.WriteLine("Catch -> l'échec a été intercepté APRÈS la libération du Using")
        Finally
            Console.WriteLine("Finally -> toujours exécuté")
        End Try
    End Function

    Private Class RessourceTracee
        Implements IDisposable

        Public Sub Dispose() Implements IDisposable.Dispose
            Console.WriteLine("Using -> RessourceTracee.Dispose() appelée")
        End Sub
    End Class

    ' ---- Await interdit dans Catch : le contournement -------------------------------
    Private Async Function DemoAwaitHorsCatch() As Task
        Console.WriteLine()
        Console.WriteLine("== Await interdit dans Catch/Finally (BC36943) : contournement ==")

        ' Await JournaliserAsync(ex) DANS le Catch ne compile pas en VB.
        ' Le motif exact du cours : capturer, puis attendre APRÈS le bloc.
        Dim erreur As Exception = Nothing
        Try
            Await ChargerTexteAsync(echouer:=True)
        Catch ex As Exception
            erreur = ex                      ' on capture — aucun Await ici
        End Try

        If erreur IsNot Nothing Then
            Await JournaliserAsync(erreur)   ' l'Await se fait HORS du Catch
        End If
    End Function

    Private Async Function JournaliserAsync(ex As Exception) As Task
        Await Task.Delay(30)                 ' simule une journalisation asynchrone
        Console.WriteLine($"JournaliserAsync (hors Catch) -> {ex.Message}")
    End Function

    ' ---- Filtres When ⭐ ---------------------------------------------------------------
    Private Async Function DemoFiltresWhen() As Task
        Console.WriteLine()
        Console.WriteLine("== Filtres Catch ... When (VB depuis 2002) ==")

        Dim resultat404 = Await AppelerApiAsync(HttpStatusCode.NotFound)
        Console.WriteLine($"Statut 404 -> résultat : {If(resultat404, "Nothing (absence, pas une erreur)")}")
        Try
            Await AppelerApiAsync(HttpStatusCode.InternalServerError)
        Catch ex As HttpRequestException
            Console.WriteLine($"Statut 500 -> relancée puis attrapée ici : {ex.StatusCode}")
        End Try
    End Function

    ''' <summary>Le motif exact du cours : 404 filtré comme cas métier, le reste relancé.</summary>
    Private Async Function AppelerApiAsync(statut As HttpStatusCode) As Task(Of String)
        Try
            Await LeverHttpAsync(statut)
            Return "contenu"
        Catch ex As HttpRequestException When ex.StatusCode = HttpStatusCode.NotFound
            ' 404 : cas métier attendu, pas une vraie erreur -> absence
            Return Nothing
        Catch ex As HttpRequestException
            Console.WriteLine($"  journalisation de l'erreur HTTP {CInt(ex.StatusCode)} avant relance")
            Throw
        End Try
    End Function

    Private Async Function LeverHttpAsync(statut As HttpStatusCode) As Task
        Await Task.Delay(30)
        Throw New HttpRequestException($"Réponse {CInt(statut)} simulée", Nothing, statut)
    End Function

    ' ---- Les exceptions synchrones sortent à l'Await -------------------------------------
    Private Async Function DemoExceptionSynchrone() As Task
        Console.WriteLine()
        Console.WriteLine("== Exception synchrone d'une méthode Async : levée à l'Await ==")

        Dim tache = ValiderEtChargerAsync(-1)    ' l'APPEL ne lève rien…
        Console.WriteLine("L'appel ValiderEtChargerAsync(-1) n'a PAS levé d'exception.")
        Try
            Await tache                           ' …c'est l'Await qui la relance
        Catch ex As ArgumentException
            Console.WriteLine($"L'Await, lui, relance : {ex.GetType().Name} (« {ex.ParamName} »)")
        End Try
    End Function

    Private Async Function ValiderEtChargerAsync(id As Integer) As Task(Of String)
        If id <= 0 Then
            Throw New ArgumentException("Identifiant invalide", NameOf(id))
        End If
        Await Task.Delay(10)
        Return $"profil {id}"
    End Function

    ' ---- Task.WhenAll : plusieurs échecs ------------------------------------------------------
    Private Async Function DemoWhenAllMultiple() As Task
        Console.WriteLine()
        Console.WriteLine("== Task.WhenAll : première exception à l'Await, toutes dans InnerExceptions ==")

        Dim t1 = Task.Delay(30)
        Dim t2 = EchouerAsync("défaillance A")
        Dim t3 = EchouerAsync("défaillance B")

        Dim toutes = Task.WhenAll(t1, t2, t3)
        Try
            Await toutes
        Catch ex As Exception
            Console.WriteLine($"Await relance la PREMIÈRE : « {ex.Message} »")
            Console.WriteLine($"toutes.Exception.InnerExceptions en contient : {toutes.Exception.InnerExceptions.Count}")
            For Each interne In toutes.Exception.InnerExceptions
                Console.WriteLine($"  - {interne.Message}")
            Next
        End Try
    End Function

    Private Async Function EchouerAsync(message As String) As Task
        Await Task.Delay(50)
        Throw New InvalidOperationException(message)
    End Function

    ' ---- AggregateException et le déballage ------------------------------------------------------
    Private Sub DemoDeballage()
        Console.WriteLine()
        Console.WriteLine("== .Result (emballée) vs GetAwaiter().GetResult() (déballée) ==")

        Dim fautee = EchouerAsync("erreur d'origine")

        Try
            fautee.Wait()                ' ⚠️ .Wait()/.Result livrent une AggregateException
        Catch ex As Exception
            Console.WriteLine($".Wait()                  -> {ex.GetType().Name}")
        End Try

        Try
            fautee.GetAwaiter().GetResult()   ' livre l'exception D'ORIGINE
        Catch ex As Exception
            Console.WriteLine($"GetAwaiter().GetResult() -> {ex.GetType().Name} (« {ex.Message} »)")
        End Try
    End Sub

    ' ---- Le piège Async Sub -----------------------------------------------------------------------
    Private ReadOnly _signalAsyncSub As New SemaphoreSlim(0)

    Private Async Function DemoAsyncSub() As Task
        Console.WriteLine()
        Console.WriteLine("== Async Sub : Try/Catch OBLIGATOIRE à l'intérieur ==")

        ' Un Async Sub ne renvoie pas de Task : personne ne peut attraper ses
        ' exceptions de l'extérieur. La règle du cours : tout le corps dans Try/Catch.
        GestionnaireEvenementSimule(Nothing, EventArgs.Empty)
        Await _signalAsyncSub.WaitAsync()    ' attend la fin du Async Sub (démo déterministe)
    End Function

    ''' <summary>Le gestionnaire d'événement type du cours (seul usage légitime d'Async Sub).</summary>
    Private Async Sub GestionnaireEvenementSimule(sender As Object, e As EventArgs)
        Try
            Await ChargerTexteAsync(echouer:=True)
            Console.WriteLine("Envoi réussi.")
        Catch ex As Exception
            Console.WriteLine($"Async Sub -> échec géré À L'INTÉRIEUR : {ex.Message}")
        Finally
            _signalAsyncSub.Release()
        End Try
    End Sub

End Module
