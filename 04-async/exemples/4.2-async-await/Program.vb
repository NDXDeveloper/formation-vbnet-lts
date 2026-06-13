' ============================================================================
'  Section 4.2 : Async/Await (Task, Task(Of T))
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · ⚠️ pas d'Async Main en VB : le pont exact du cours —
'                    Sub Main synchrone + MainAsync().GetAwaiter().GetResult() ;
'                  · Async Function ... As Task(Of T) : on renvoie LA VALEUR,
'                    le compilateur l'emballe (ChargerTexteAsync) ; As Task
'                    sans valeur (SauvegarderAsync, fichier réel) ;
'                  · enchaîner (séquence) vs composer (Task.WhenAll : les
'                    opérations progressent SIMULTANÉMENT — vérifié par
'                    chronomètre) ; Task.WhenAny (la première gagne) ;
'                  · Task.Run pour un calcul CPU + AddressOf pour une
'                    méthode nommée (spécificité VB) ;
'                  · relayer une Task sans Async ; Task.FromResult /
'                    Task.CompletedTask ;
'                  · ConfigureAwait(False) dans le code « bibliothèque ».
'  Fichier source : 02-async-await.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Linq

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        ' Unique point de blocage, au sommet de la chaîne (pas d'Async Main en VB).
        ' GetAwaiter().GetResult() relance l'exception D'ORIGINE (pas d'AggregateException).
        MainAsync(args).GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync(args As String()) As Task
        Await DemoAnatomie()
        Await DemoSequenceContreWhenAll()
        Await DemoWhenAny()
        Await DemoTaskRun()
        Await DemoSansAsync()
    End Function

    ' ---- Anatomie : Async Function As Task / Task(Of T) -------------------------
    Private Async Function DemoAnatomie() As Task
        Console.WriteLine("== Anatomie d'une méthode asynchrone ==")

        ' Task(Of String) : Await déballe la valeur
        Dim texte As String = Await ChargerTexteAsync("document-1")
        Console.WriteLine($"ChargerTexteAsync -> « {texte} »")

        ' Task (sans valeur) : Await s'emploie comme une instruction
        Dim chemin = Path.Combine(Path.GetTempPath(), "async-await-demo.bin")
        Await SauvegarderAsync(chemin, New Byte() {1, 2, 3})
        Console.WriteLine($"SauvegarderAsync -> {New FileInfo(chemin).Length} octets écrits")
        File.Delete(chemin)
    End Function

    ''' <summary>Renvoie une valeur : As Task(Of String) — on Return la valeur brute.
    ''' (Le cours utilise HttpClient.GetStringAsync ; simulé ici hors ligne.)</summary>
    Private Async Function ChargerTexteAsync(ressource As String) As Task(Of String)
        Await Task.Delay(80)                       ' simule la latence d'E/S
        Return $"contenu de {ressource}"           ' la valeur, PAS une Task
    End Function

    ''' <summary>Aucune valeur : As Task (l'exemple fichier exact du cours).</summary>
    Private Async Function SauvegarderAsync(chemin As String, donnees As Byte()) As Task
        Await File.WriteAllBytesAsync(chemin, donnees)
    End Function

    ' ---- Séquence vs WhenAll ---------------------------------------------------------
    Private Async Function DemoSequenceContreWhenAll() As Task
        Console.WriteLine()
        Console.WriteLine("== En séquence vs Task.WhenAll ==")

        ' En SÉQUENCE : chaque Await attend le précédent (~300 + 200 ms)
        Dim chrono = Stopwatch.StartNew()
        Dim meteo = Await ChargerMeteoAsync("Paris")
        Dim trafic = Await ChargerTraficAsync("Paris")
        Dim dureeSequence = chrono.ElapsedMilliseconds

        ' En PARALLÈLE : on DÉMARRE les deux, puis on les attend ensemble (~max = 300 ms)
        chrono.Restart()
        Dim tMeteo = ChargerMeteoAsync("Paris")        ' démarre…
        Dim tTrafic = ChargerTraficAsync("Paris")      ' …démarre aussi, sans attendre
        Dim resultats = Await Task.WhenAll(tMeteo, tTrafic)
        Dim dureeParallele = chrono.ElapsedMilliseconds

        Console.WriteLine($"Résultats WhenAll : {resultats(0)} | {resultats(1)}")
        Console.WriteLine($"Séquence ≈ {dureeSequence} ms ; WhenAll ≈ {dureeParallele} ms")
        Console.WriteLine($"WhenAll plus rapide que la séquence : {dureeParallele < dureeSequence}")
    End Function

    Private Async Function ChargerMeteoAsync(ville As String) As Task(Of String)
        Await Task.Delay(300)
        Return $"Météo {ville} : 21 °C"
    End Function

    Private Async Function ChargerTraficAsync(ville As String) As Task(Of String)
        Await Task.Delay(200)
        Return $"Trafic {ville} : fluide"
    End Function

    ' ---- WhenAny : la première terminée -------------------------------------------------
    Private Async Function DemoWhenAny() As Task
        Console.WriteLine()
        Console.WriteLine("== Task.WhenAny (course entre deux sources) ==")

        Dim tSourceA = RepondreAsync("miroir rapide", 100)
        Dim tSourceB = RepondreAsync("miroir lent", 400)

        Dim premiere = Await Task.WhenAny(tSourceA, tSourceB)   ' la Task terminée en premier
        Dim valeur = Await premiere
        Console.WriteLine($"Première réponse : « {valeur} »")
        Await Task.WhenAll(tSourceA, tSourceB)   ' on laisse l'autre finir proprement
    End Function

    Private Async Function RepondreAsync(nom As String, delaiMs As Integer) As Task(Of String)
        Await Task.Delay(delaiMs)
        Return nom
    End Function

    ' ---- Task.Run : décharger un calcul CPU ------------------------------------------------
    Private Async Function DemoTaskRun() As Task
        Console.WriteLine()
        Console.WriteLine("== Task.Run (calcul CPU déchargé) ==")

        ' Lambda : calcul lourd hors du thread appelant
        Dim somme = Await Task.Run(Function() Enumerable.Range(1, 1000).Sum(Function(n) CLng(n) * n))
        Console.WriteLine($"Somme des carrés 1..1000 = {somme}")

        ' Méthode nommée : AddressOf obligatoire en VB
        Dim stats = Await Task.Run(AddressOf CalculerStatistiques)
        Console.WriteLine($"CalculerStatistiques (via AddressOf) = {stats}")
    End Function

    Private Function CalculerStatistiques() As Double
        Return Enumerable.Range(1, 100).Average(Function(n) CDbl(n))   ' 50.5
    End Function

    ' ---- Renvoyer une Task sans Async ----------------------------------------------------------
    Private Async Function DemoSansAsync() As Task
        Console.WriteLine()
        Console.WriteLine("== Renvoyer une Task sans Async ==")

        ' Relais pur : on transmet la Task telle quelle (pas de machine à états)
        Console.WriteLine($"Relais          : {Await ChargerProfilAsync(7)}")

        ' Valeur déjà disponible : tâches préfabriquées, toujours SANS Async
        Console.WriteLine($"Task.FromResult : {Await ObtenirParDefautAsync()}")
        Await RienAFaireAsync()
        Console.WriteLine("Task.CompletedTask : terminé immédiatement")

        ' ConfigureAwait(False) : code « bibliothèque », pas de retour au contexte
        Dim texte = Await ChargerTexteAsync("bibliothèque").ConfigureAwait(False)
        Console.WriteLine($"ConfigureAwait(False) -> « {texte} »")
    End Function

    ''' <summary>Relais pur : pas d'Async/Await, on renvoie la Task de l'appel.</summary>
    Private Function ChargerProfilAsync(id As Integer) As Task(Of String)
        Return ChargerTexteAsync($"profil-{id}")
    End Function

    Private Function ObtenirParDefautAsync() As Task(Of Integer)
        Return Task.FromResult(42)        ' Task(Of T) déjà terminée
    End Function

    Private Function RienAFaireAsync() As Task
        Return Task.CompletedTask         ' Task déjà terminée
    End Function

End Module
