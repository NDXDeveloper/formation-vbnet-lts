' ============================================================================
'  Section 4.8 : Services en arrière-plan : Generic Host et BackgroundService
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · le câblage manuel du Generic Host (pas de modèle Worker
'                    en VB) : Host.CreateApplicationBuilder, AddHostedService,
'                    Build, Run ;
'                  · ⚠️ le piège VB : la variable s'appelle « hote », PAS
'                    « host » — VB est insensible à la casse, « host »
'                    masquerait la classe Host (BC32000) ;
'                  · host.Run() est SYNCHRONE : l'absence d'Async Main (4.2)
'                    est sans conséquence, un Sub Main ordinaire suffit ;
'                  · enregistrement de plusieurs BackgroundService, d'un
'                    service scoped (UniteDeTravail) et réglage du
'                    ShutdownTimeout ;
'                  · arrêt propre : chaque service surveille stoppingToken.
'                Trois services tournent quelques cycles puis demandent
'                l'arrêt, pour que la démo se TERMINE.
'  Fichier source : 08-background-services.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        Dim builder = Host.CreateApplicationBuilder(args)

        ' Journalisation console lisible (sans horodatage ni catégorie verbeuse)
        builder.Logging.ClearProviders()
        builder.Logging.AddSimpleConsole(Sub(o)
                                             o.SingleLine = True
                                         End Sub)
        builder.Logging.SetMinimumLevel(LogLevel.Information)

        ' Service scoped (tient lieu de DbContext) + services de fond
        builder.Services.AddScoped(Of UniteDeTravail)()
        builder.Services.AddHostedService(Of ServiceDeNettoyage)()
        builder.Services.AddHostedService(Of ServicePeriodique)()
        builder.Services.AddHostedService(Of ServiceAvecPortee)()

        ' ⚠️ « hote », jamais « host » : VB est insensible à la casse, et une
        ' variable « host » masquerait la classe Host -> BC32000.
        Dim hote = builder.Build()

        Console.WriteLine("Hôte démarré (Ctrl+C pour arrêter ; ici, arrêt automatique après quelques cycles).")
        Console.WriteLine("----------------------------------------------------------------------")
        hote.Run()                   ' SYNCHRONE : bloque jusqu'à l'arrêt (pas besoin d'async Main)
        Console.WriteLine("----------------------------------------------------------------------")
        Console.WriteLine("Hôte arrêté proprement (graceful shutdown).")
    End Sub

End Module
