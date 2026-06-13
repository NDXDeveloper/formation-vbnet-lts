Option Strict Off
' ============================================================================
'  Section 9.2 : COM et automation Office — automation Office (liaison tardive)
'  Description : Pilotage d'Excel en LIAISON TARDIVE (CreateObject par ProgID,
'                indépendant de la version d'Office). Applique la règle des
'                « deux points » (Workbooks gardé en variable) et le nettoyage
'                explicite (Quit + ReleaseComObject en ordre inverse + GC) pour
'                éviter les processus EXCEL.EXE fantômes. ⚠️ Office n'étant pas
'                installé sur la machine de validation, l'exécution est gardée
'                par une détection de ProgID ; le code reste représentatif.
'  Fichier source : 02-com-office.md
' ============================================================================

Imports System
Imports System.Runtime.InteropServices

Public Module OfficeAutomation

    Public Sub Executer()
        If Type.GetTypeFromProgID("Excel.Application") Is Nothing Then
            Console.WriteLine("  Excel non installé : code montré à titre documentaire, non exécuté.")
            Console.WriteLine("  (La mécanique COM — RCW, ReleaseComObject — est identique au cas FSO ci-dessus.)")
            Return
        End If

        Dim app As Object = CreateObject("Excel.Application")
        app.Visible = False
        app.DisplayAlerts = False

        ' « Un objet COM = une variable » : Workbooks gardé pour être libérable.
        Dim books As Object = app.Workbooks
        Dim book As Object = books.Add()
        Dim sheet As Object = book.ActiveSheet

        sheet.Range("A1").Value = "Bonjour"
        Dim lu As String = CStr(sheet.Range("A1").Value)
        Console.WriteLine($"  Excel : A1 = « {lu} »")

        book.Close(False)
        app.Quit()

        ' Libération dans l'ordre inverse de la création.
        Marshal.ReleaseComObject(sheet)
        Marshal.ReleaseComObject(book)
        Marshal.ReleaseComObject(books)
        Marshal.ReleaseComObject(app)
        sheet = Nothing : book = Nothing : books = Nothing : app = Nothing

        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
        GC.WaitForPendingFinalizers()

        Console.WriteLine("  Excel fermé et objets COM libérés (pas de processus fantôme).")
    End Sub

End Module
