' ============================================================================
'  Section 11.6 : Moderniser (async, LINQ, injection de dépendances, testabilité)
'  Description : Démonstration vérifiable des axes de modernisation. LINQ
'                (impératif → déclaratif, même résultat), Async/Await (sans
'                blocage), et injection de dépendances avec une couture testable
'                (horloge système vs horloge fixe → sortie déterministe).
'  Fichier source : 06-moderniser.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.Extensions.DependencyInjection

Module Program

    Function Main() As Integer
        Return MainAsync().GetAwaiter().GetResult()
    End Function

    Async Function MainAsync() As Task(Of Integer)
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 11.6 Moderniser : LINQ, async, injection de dépendances ===")
        Console.WriteLine()

        ' --- LINQ : boucle impérative → requête déclarative (même résultat) ---
        Dim clients As New List(Of Client) From {
            New Client("Alice", True), New Client("Bob", False),
            New Client("Charlie", True), New Client("Diane", True)
        }

        ' Avant — impératif
        Dim actifsImp As New List(Of Client)
        For Each c In clients
            If c.EstActif Then actifsImp.Add(c)
        Next

        ' Après — LINQ (syntaxe requête, idiomatique en VB)
        Dim actifsLinq = (From c In clients
                          Where c.EstActif
                          Select c).ToList()

        Console.WriteLine("[LINQ] impératif vs déclaratif")
        Console.WriteLine($"  impératif = {actifsImp.Count} actifs ; LINQ = {actifsLinq.Count} actifs ; identiques = {actifsImp.SequenceEqual(actifsLinq)}")
        Console.WriteLine($"  actifs : {String.Join(", ", actifsLinq.Select(Function(c) c.Nom))}")
        Console.WriteLine()

        ' --- Injection de dépendances : composition au point d'entrée ---
        Dim services As New ServiceCollection()
        services.AddSingleton(Of IHorloge, HorlogeSysteme)()
        services.AddTransient(Of ServiceSalutation)()
        Using fournisseur = services.BuildServiceProvider()
            Dim svc = fournisseur.GetRequiredService(Of ServiceSalutation)()
            Console.WriteLine("[Injection de dépendances] horloge système (date du jour)")
            Console.WriteLine($"  {svc.Saluer("Alice")}")
            Console.WriteLine()

            ' --- Async : E/S sans blocage ---
            Console.WriteLine("[Async/Await]")
            Console.WriteLine($"  {Await svc.ChargerMessageAsync()}")
            Console.WriteLine()
        End Using

        ' --- Testabilité : la couture (horloge fixe) rend la sortie déterministe ---
        Dim svcTest As New ServiceSalutation(New HorlogeFixe(New DateOnly(2026, 1, 1)))
        Console.WriteLine("[Testabilité] même service, horloge FIXE injectée")
        Console.WriteLine($"  {svcTest.Saluer("Bob")}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
        Return 0
    End Function

End Module
