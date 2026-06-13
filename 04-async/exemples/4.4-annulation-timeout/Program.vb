' ============================================================================
'  Section 4.4 : Annulation et timeout (CancellationToken)
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · le modèle coopératif : CancellationTokenSource (demande)
'                    et CancellationToken (observe) — le schéma « bouton
'                    Annuler » du cours, transposé en console ;
'                  · propager le token (Optional ct = Nothing) et répondre :
'                    ThrowIfCancellationRequested (point de contrôle en
'                    boucle) et IsCancellationRequested (arrêt propre) ;
'                  · ThrowIfCancellationRequested -> tâche à l'état CANCELED,
'                    exception ordinaire -> état FAULTED (vérifié) ;
'                  · timeout : CancellationTokenSource(TimeSpan) converti en
'                    TimeoutException ;
'                  · CreateLinkedTokenSource + filtre When pour distinguer
'                    timeout et annulation utilisateur (les 2 scénarios) ;
'                  · Task.WaitAsync (.NET 6+) : borne l'ATTENTE, mais
'                    l'opération d'origine CONTINUE (piège vérifié).
'  Fichier source : 04-annulation-timeout.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Threading

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        Await DemoAnnulationUtilisateur()
        Await DemoPointsDeControle()
        Await DemoCanceledContreFaulted()
        Await DemoTimeoutSimple()
        Await DemoSourcesLiees()
        Await DemoWaitAsync()
    End Function

    ' ---- Le schéma « bouton Annuler » -----------------------------------------
    Private Async Function DemoAnnulationUtilisateur() As Task
        Console.WriteLine("== Annulation coopérative (le « bouton Annuler ») ==")

        Using cts As New CancellationTokenSource()
            ' Simule le clic sur « Annuler » 300 ms plus tard :
            Dim clicAnnuler = Task.Run(Async Function()
                                           Await Task.Delay(300)
                                           cts.Cancel()
                                       End Function)
            Try
                Await LongTraitementAsync(cts.Token)
                Console.WriteLine("Traitement terminé (ne devrait pas s'afficher).")
            Catch ex As OperationCanceledException
                Console.WriteLine("Opération annulée.")          ' le message du cours
            End Try
            Await clicAnnuler
        End Using
    End Function

    ''' <summary>Token propagé en dernier paramètre, transmis à Task.Delay.</summary>
    Private Async Function LongTraitementAsync(Optional ct As CancellationToken = Nothing) As Task
        For i = 1 To 20
            Await Task.Delay(50, ct)     ' on TRANSMET le token à chaque attente
        Next
    End Function

    ' ---- Points de contrôle dans une boucle ----------------------------------------
    Private Async Function DemoPointsDeControle() As Task
        Console.WriteLine()
        Console.WriteLine("== ThrowIfCancellationRequested (points de contrôle) ==")

        Using cts As New CancellationTokenSource()
            Dim traites = 0
            Try
                ' L'« utilisateur » annule après le 2e fichier (déterministe).
                Await TraiterFichiersAsync({"a.txt", "b.txt", "c.txt", "d.txt"},
                                           Sub()
                                               traites += 1
                                               If traites = 2 Then cts.Cancel()
                                           End Sub,
                                           cts.Token)
            Catch ex As OperationCanceledException
                Console.WriteLine($"Annulé au point de contrôle après {traites} fichiers traités.")
            End Try
        End Using
    End Function

    Private Async Function TraiterFichiersAsync(fichiers As IEnumerable(Of String),
                                                apresChaqueFichier As Action,
                                                ct As CancellationToken) As Task
        For Each fichier In fichiers
            ct.ThrowIfCancellationRequested()                 ' point de contrôle
            Await Task.Delay(40)                              ' simule la lecture
            Console.WriteLine($"  fichier traité : {fichier}")
            apresChaqueFichier()
        Next
    End Function

    ' ---- Canceled vs Faulted ------------------------------------------------------------
    Private Async Function DemoCanceledContreFaulted() As Task
        Console.WriteLine()
        Console.WriteLine("== État de la tâche : Canceled vs Faulted ==")

        ' ThrowIfCancellationRequested -> état CANCELED
        Using cts As New CancellationTokenSource()
            cts.Cancel()
            Dim annulee = TraiterFichiersAsync({"x"}, Sub()
                                                      End Sub, cts.Token)
            Try
                Await annulee
            Catch ex As OperationCanceledException
            End Try
            Console.WriteLine($"ThrowIfCancellationRequested -> IsCanceled = {annulee.IsCanceled}, IsFaulted = {annulee.IsFaulted}")
        End Using

        ' Exception ordinaire -> état FAULTED
        Dim fautee = EchouerAsync()
        Try
            Await fautee
        Catch ex As InvalidOperationException
        End Try
        Console.WriteLine($"Exception ordinaire          -> IsCanceled = {fautee.IsCanceled}, IsFaulted = {fautee.IsFaulted}")
    End Function

    Private Async Function EchouerAsync() As Task
        Await Task.Delay(10)
        Throw New InvalidOperationException("défaillance")
    End Function

    ' ---- Timeout simple --------------------------------------------------------------------
    Private Async Function DemoTimeoutSimple() As Task
        Console.WriteLine()
        Console.WriteLine("== Timeout : CancellationTokenSource(TimeSpan) ==")

        Using cts As New CancellationTokenSource(TimeSpan.FromMilliseconds(300))
            Try
                Await LongTraitementAsync(cts.Token)          ' ~1 s : dépasse le délai
            Catch ex As OperationCanceledException
                Console.WriteLine("Converti en TimeoutException : « Le chargement a dépassé le délai. »")
            End Try
        End Using
    End Function

    ' ---- Sources liées : timeout OU utilisateur ------------------------------------------------
    Private Async Function DemoSourcesLiees() As Task
        Console.WriteLine()
        Console.WriteLine("== CreateLinkedTokenSource + filtre When (2 causes distinguées) ==")

        ' Scénario A : le DÉLAI expire (aucune annulation utilisateur)
        Try
            Await ChargerAvecDelaiAsync(TimeSpan.FromMilliseconds(300), CancellationToken.None)
        Catch ex As TimeoutException
            Console.WriteLine($"Scénario A -> {ex.GetType().Name} : {ex.Message}")
        End Try

        ' Scénario B : l'UTILISATEUR annule avant le délai
        Using ctsUtilisateur As New CancellationTokenSource(TimeSpan.FromMilliseconds(150))
            Try
                Await ChargerAvecDelaiAsync(TimeSpan.FromSeconds(10), ctsUtilisateur.Token)
            Catch ex As OperationCanceledException
                Console.WriteLine($"Scénario B -> {ex.GetType().Name} : annulation utilisateur (pas un timeout)")
            End Try
        End Using
    End Function

    ''' <summary>Le motif exact du cours : sources liées + When sur la cause.</summary>
    Private Async Function ChargerAvecDelaiAsync(delai As TimeSpan,
                                                 tokenUtilisateur As CancellationToken) As Task(Of String)
        Using ctsDelai As New CancellationTokenSource(delai)
            Using ctsLie = CancellationTokenSource.CreateLinkedTokenSource(
                                tokenUtilisateur, ctsDelai.Token)
                Try
                    Await LongTraitementAsync(ctsLie.Token)    ' ~1 s
                    Return "contenu"
                Catch ex As OperationCanceledException When ctsDelai.IsCancellationRequested
                    ' Seul le minuteur a déclenché -> timeout, pas l'utilisateur
                    Throw New TimeoutException($"Délai de {delai.TotalMilliseconds:N0} ms dépassé.")
                End Try
            End Using
        End Using
    End Function

    ' ---- Task.WaitAsync : borne l'attente, pas l'opération ---------------------------------------
    Private _operationTerminee As Boolean = False

    Private Async Function DemoWaitAsync() As Task
        Console.WriteLine()
        Console.WriteLine("== Task.WaitAsync (.NET 6+) : un garde-fou, pas une annulation ==")

        Dim operation = OperationSansTokenAsync()
        Try
            Await operation.WaitAsync(TimeSpan.FromMilliseconds(300))
        Catch ex As TimeoutException
            Console.WriteLine($"TimeoutException après 300 ms ; opération terminée ? {_operationTerminee}")
        End Try

        Await operation   ' ⚠️ l'opération d'origine a CONTINUÉ de tourner…
        Console.WriteLine($"…et s'est achevée quand même : opération terminée ? {_operationTerminee}")
    End Function

    Private Async Function OperationSansTokenAsync() As Task
        Await Task.Delay(900)            ' n'accepte aucun token : inarrêtable
        _operationTerminee = True
    End Function

End Module
