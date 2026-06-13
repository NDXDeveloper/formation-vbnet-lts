' ============================================================================
'  Section 9.2 : COM et automation Office — orchestration
'  Description : Point d'entrée (Option Strict On). Enchaîne la démonstration de
'                liaison tardive sur un composant COM intégré (runnable partout)
'                puis l'automation Office (gardée selon la présence d'Office).
'  Fichier source : 02-com-office.md
' ============================================================================

Imports System

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 9.2 COM et automation Office ===")
        Console.WriteLine()

        Console.WriteLine("[Liaison tardive — composant COM Windows intégré (Scripting.FileSystemObject)]")
        ComScripting.Executer()
        Console.WriteLine()

        Console.WriteLine("[Automation Office — Excel en liaison tardive]")
        OfficeAutomation.Executer()
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub
End Module
