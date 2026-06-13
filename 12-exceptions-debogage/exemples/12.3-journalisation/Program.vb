' ============================================================================
'  Section 12.3 : Journalisation — orchestration et vérification
'  Description : Configure Microsoft.Extensions.Logging (fabrique + fournisseur
'                en mémoire + console, niveau minimal Information), exerce le
'                service, puis PROUVE la structure des logs via les propriétés
'                capturées (CommandeId/DureeMs), vérifie le filtrage (pas de
'                Debug) et la présence d'un log d'erreur. Enfin, Serilog derrière
'                ILogger. NB : la variable de fabrique se nomme « fabrique »
'                (BC30980 si « loggerFactory »).
'  Fichier source : 03-journalisation.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Linq
Imports Microsoft.Extensions.Logging
Imports Serilog

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 12.3 Journalisation ===")
        Console.WriteLine()

        ' --- Microsoft.Extensions.Logging ---
        Dim capture As New CaptureProvider()
        Dim fabrique = LoggerFactory.Create(
            Sub(builder)
                builder.AddProvider(capture)
                builder.AddSimpleConsole(Sub(o) o.SingleLine = True)
                builder.SetMinimumLevel(LogLevel.Information)
            End Sub)

        Dim service As New ServiceCommandes(fabrique.CreateLogger(Of ServiceCommandes)())
        service.Traiter(4271, 12)                            ' Information, structuré
        service.Detail()                                    ' Debug -> écarté (min=Information)
        service.TraiterAvecPortee(4271)                     ' BeginScope
        service.Echouer(4271)                               ' LogError + exception
        fabrique.CreateLogger("Demo").CommandeTraitee(99)   ' LoggerMessage.Define

        fabrique.Dispose()   ' vide les tampons

        Console.WriteLine()
        Console.WriteLine("[Preuve du logging structuré — propriétés capturées]")
        Dim entree = capture.Entrees.First(
            Function(e) e.Proprietes.ContainsKey("CommandeId") AndAlso e.Proprietes.ContainsKey("DureeMs"))
        Console.WriteLine($"  message rendu      : {entree.Message}")
        Console.WriteLine($"  propriété CommandeId = {entree.Proprietes("CommandeId")}")
        Console.WriteLine($"  propriété DureeMs    = {entree.Proprietes("DureeMs")}")
        Console.WriteLine($"  aucune entrée Debug (filtrée par min=Information) = {Not capture.Entrees.Any(Function(e) e.Niveau = LogLevel.Debug)}")
        Console.WriteLine($"  entrée Error avec exception = {capture.Entrees.Any(Function(e) e.Niveau = LogLevel.Error AndAlso e.AvecException)}")
        Console.WriteLine($"  total d'entrées capturées = {capture.Entrees.Count}")
        Console.WriteLine()

        ' --- Serilog derrière ILogger ---
        Console.WriteLine("[Serilog via ILogger]")
        Log.Logger = New LoggerConfiguration() _
            .MinimumLevel.Information() _
            .Enrich.FromLogContext() _
            .WriteTo.Console() _
            .CreateLogger()

        Dim fabriqueSerilog = LoggerFactory.Create(Sub(b) b.AddSerilog())
        Dim loggerSerilog = fabriqueSerilog.CreateLogger("Demo.Serilog")
        loggerSerilog.LogInformation("Pris en charge par Serilog via ILogger : commande {CommandeId}", 4271)
        fabriqueSerilog.Dispose()
        Log.CloseAndFlush()
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub
End Module
