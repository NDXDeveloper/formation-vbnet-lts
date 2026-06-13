' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Le Program.vb « à la C# » exact que génère le modèle CLI
'                « dotnet new winforms -lang VB » (vérifié à l'identique avec
'                le SDK .NET 10.0.301) : démarrage explicite par Sub Main,
'                sans cadre applicatif (pas d'ApplicationEvents.vb).
'                Au passage : « New Form1 » s'écrit sans parenthèses — en VB,
'                elles sont optionnelles quand le constructeur n'a pas
'                d'argument.
'  Fichier source : 05-premier-projet.md
'  Compilation    : dotnet build      Exécution : dotnet run (fenêtre)
' ============================================================================

Friend Module Program

    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1)
    End Sub

End Module
