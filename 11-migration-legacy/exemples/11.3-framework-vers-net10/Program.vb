' ============================================================================
'  Section 11.3 : .NET Framework 4.x → .NET 10 (configuration)
'  Description : Construit une configuration EN COUCHES (appsettings.json +
'                appsettings.Development.json + variables d'environnement), la
'                lit via IConfiguration (indexeur et GetConnectionString),
'                expose une section en IOptions(Of ApiOptions), et illustre le
'                rappel d'encodage (Encoding.Default = UTF-8 sur .NET moderne).
'  Fichier source : 03-framework-vers-net10.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Options

Module Program

    Sub Main()
        Console.OutputEncoding = Encoding.UTF8
        Console.WriteLine("=== 11.3 Framework → .NET 10 : App.config → appsettings.json ===")
        Console.WriteLine()

        ' Une variable d'environnement (couche la plus prioritaire) surcharge un
        ' réglage imbriqué : Api:TimeoutSeconds via la convention « __ ».
        Environment.SetEnvironmentVariable("Api__TimeoutSeconds", "99")

        ' Configuration en couches : base, surcharge Development, variables d'env.
        Dim config As IConfiguration = New ConfigurationBuilder() _
            .SetBasePath(AppContext.BaseDirectory) _
            .AddJsonFile("appsettings.json", optional:=False) _
            .AddJsonFile("appsettings.Development.json", optional:=True) _
            .AddEnvironmentVariables() _
            .Build()

        ' Lecture directe (équivalents de ConfigurationManager).
        Dim url As String = config("ApiBaseUrl")
        Dim cs As String = config.GetConnectionString("Db")
        Console.WriteLine("[Lecture IConfiguration]")
        Console.WriteLine($"  ApiBaseUrl        = {url}        (surchargé par appsettings.Development.json)")
        Console.WriteLine($"  ConnectionStrings:Db = {cs}")
        Console.WriteLine()

        ' Pattern Options : section « Api » liée à une classe, via IOptions(Of T).
        Dim services As New ServiceCollection()
        services.Configure(Of ApiOptions)(config.GetSection("Api"))
        Dim fournisseur = services.BuildServiceProvider()
        Dim options = fournisseur.GetRequiredService(Of IOptions(Of ApiOptions))().Value
        Console.WriteLine("[Pattern Options : IOptions(Of ApiOptions)]")
        Console.WriteLine($"  Api:TimeoutSeconds = {options.TimeoutSeconds}   (surchargé par variable d'environnement)")
        Console.WriteLine($"  Api:Reessais       = {options.Reessais}")
        Console.WriteLine()

        ' Rappel du saut Framework → moderne : Encoding.Default vaut UTF-8.
        Console.WriteLine("[Encodage] saut Framework → moderne")
        Console.WriteLine($"  Encoding.Default = {Encoding.Default.WebName} ({Encoding.Default.EncodingName})")
        Console.WriteLine($"  EstUtf8 = {Encoding.Default.WebName = "utf-8"}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub

End Module
