' ============================================================================
'  Section 1.3 : L'écosystème .NET — Structure d'une solution (solution mixte)
'  Description : Point d'entrée explicite de l'interface Windows Forms
'                (démarrage « à la C# », identique au modèle
'                « dotnet new winforms -lang VB » — voir section 1.5).
'  Fichier source : 03-ecosysteme-dotnet.md
'  Compilation    : dotnet build MaSolution.sln
'  Exécution      : dotnet run --project src/MonApp.UI (ou F5 dans VS 2026)
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
