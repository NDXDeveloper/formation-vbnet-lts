' ============================================================================
'  Section 11.3 : .NET Framework 4.x → .NET 10 (configuration)
'  Description : Classe d'options fortement typée, liée à la section « Api » de
'                la configuration et injectée via IOptions(Of ApiOptions) —
'                le « pattern Options » de .NET moderne, remplaçant les lectures
'                dispersées de ConfigurationManager.AppSettings.
'  Fichier source : 03-framework-vers-net10.md
' ============================================================================

Public Class ApiOptions
    Public Property TimeoutSeconds As Integer
    Public Property Reessais As Integer
End Class
