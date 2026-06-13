' ============================================================================
'  Section 1.3 : L'écosystème .NET — Runtime, SDK, NuGet, structure d'une solution
'  Description : Exemple complet bâti sur le squelette de .vbproj montré dans
'                la section : la dépendance NuGet Microsoft.Extensions.Logging
'                (déclarée en PackageReference) est restaurée par
'                « dotnet restore » puis consommée depuis VB.NET, comme
'                n'importe quel paquet de l'écosystème.
'  Fichier source : 03-ecosysteme-dotnet.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports Microsoft.Extensions.Logging

Module Program

    Sub Main(args As String())
        ' Affichage correct des caractères accentués dans la console Windows.
        Console.OutputEncoding = System.Text.Encoding.UTF8

        ' La fabrique de loggers vient du paquet NuGet Microsoft.Extensions.Logging ;
        ' AddSimpleConsole vient du fournisseur Microsoft.Extensions.Logging.Console.
        ' Le bloc Using garantit que les journaux en attente sont bien écrits
        ' avant la fin du programme (le fournisseur console est asynchrone).
        Using fabrique As ILoggerFactory = LoggerFactory.Create(
            Sub(builder)
                builder.AddSimpleConsole(Sub(options) options.SingleLine = True)
                builder.SetMinimumLevel(LogLevel.Information)
            End Sub)

            Dim logger As ILogger = fabrique.CreateLogger("MonApp")

            logger.LogInformation("Le paquet NuGet est restauré et opérationnel.")
            logger.LogWarning("Exemple d'avertissement avec un paramètre {Style}.", "structuré")
            logger.LogError("Exemple d'erreur (sans exception réelle).")
        End Using

        Console.WriteLine("Fin du programme.")
    End Sub

End Module
