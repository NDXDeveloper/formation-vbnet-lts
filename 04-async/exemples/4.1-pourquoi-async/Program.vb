' ============================================================================
'  Section 4.1 : Pourquoi l'asynchronie (UI réactive, opérations d'E/S)
'  Description : Exemple complet transposant en console les idées de la
'                section (les extraits du cours sont des gestionnaires
'                Windows Forms — le concept est identique) :
'                  · appel BLOQUANT : pendant un Thread.Sleep, le thread ne
'                    peut rien faire d'autre (la « fenêtre figée ») ;
'                  · appel ASYNCHRONE : pendant l'Await, le thread est rendu —
'                    un autre travail progresse pendant l'attente ;
'                  · les E/S asynchrones réelles du cours :
'                    File.ReadAllTextAsync (l'appel réseau HttpClient
'                    fonctionne à l'identique, non exécuté ici pour rester
'                    hors ligne) ;
'                  · E/S (thread libéré) vs calcul (CPU occupé -> 4.5).
'  Fichier source : 01-pourquoi-async.md
'  Compilation    : dotnet build      Exécution : dotnet run
'  Note           : les durées affichées varient légèrement d'une machine à
'                   l'autre ; les ordres de grandeur et l'ordre des messages
'                   sont, eux, stables.
' ============================================================================

Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Threading

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        ' Pas d'Async Main en VB : pont unique au sommet (détaillé en 4.2).
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        DemoBlocage()
        Await DemoAsynchronie()
        Await DemoEsAsynchrones()
    End Function

    ' ---- 1. L'appel bloquant : rien d'autre ne peut se faire ----------------
    Private Sub DemoBlocage()
        Console.WriteLine("== Appel BLOQUANT (le thread attend sans rien faire) ==")
        Dim chrono = Stopwatch.StartNew()

        ' L'équivalent du _service.Rechercher(...) synchrone du cours :
        Thread.Sleep(400)        ' le thread est immobilisé 400 ms
        Console.WriteLine($"Le thread n'a RIEN pu faire pendant {chrono.ElapsedMilliseconds} ms " &
                          "(une UI serait restée figée).")
    End Sub

    ' ---- 2. L'appel asynchrone : la main est rendue pendant l'attente --------
    Private Async Function DemoAsynchronie() As Task
        Console.WriteLine()
        Console.WriteLine("== Appel ASYNCHRONE (la main est rendue pendant l'attente) ==")

        ' L'équivalent du _service.RechercherAsync(...) du cours :
        Dim tacheLongue = RechercherAsync("VB.NET")   ' démarre l'attente (400 ms)

        ' Pendant que l'opération attend, le programme reste « réactif » :
        ' il fait progresser un autre travail (l'UI traiterait ses messages).
        Dim battements = 0
        Do While Not tacheLongue.IsCompleted
            battements += 1
            Await Task.Delay(50)
        Loop

        Console.WriteLine($"Résultat reçu : « {Await tacheLongue} »")
        Console.WriteLine($"Pendant l'attente, le programme a exécuté {battements} battements " &
                          "(il est resté réactif).")
        Console.WriteLine($"Réactif pendant l'attente : {battements >= 3}")
    End Function

    ''' <summary>Simule la recherche asynchrone du cours (E/S de 400 ms).</summary>
    Private Async Function RechercherAsync(terme As String) As Task(Of String)
        Await Task.Delay(400)            ' pendant ce délai, AUCUN thread n'est immobilisé
        Return $"3 résultats pour « {terme} »"
    End Function

    ' ---- 3. Les E/S asynchrones réelles ------------------------------------------
    Private Async Function DemoEsAsynchrones() As Task
        Console.WriteLine()
        Console.WriteLine("== E/S asynchrones réelles (suffixe Async, renvoie une Task) ==")

        ' Fichier (l'exemple du cours) — écrit puis relu de façon asynchrone :
        Dim chemin = Path.Combine(Path.GetTempPath(), "pourquoi-async-demo.txt")
        Await File.WriteAllTextAsync(chemin, "Contenu écrit par File.WriteAllTextAsync.")
        Dim contenu = Await File.ReadAllTextAsync(chemin)
        Console.WriteLine($"File.ReadAllTextAsync -> « {contenu} »")
        File.Delete(chemin)

        ' Réseau (cours) :   Dim json = Await client.GetStringAsync(url)
        ' Base de données :  Dim clients = Await db.Clients.…ToListAsync()  (module 7.2)
        Console.WriteLine("Réseau et base de données : mêmes API « …Async » (voir cours).")

        ' I/O-bound vs CPU-bound : Await n'accélère PAS un calcul (-> 4.5)
        Console.WriteLine()
        Console.WriteLine("== E/S n'est pas calcul ==")
        Console.WriteLine("E/S (attendre)  -> Async/Await libère le thread (cette section).")
        Console.WriteLine("Calcul (CPU)    -> parallélisme / Task.Run (section 4.5).")
    End Function

End Module
