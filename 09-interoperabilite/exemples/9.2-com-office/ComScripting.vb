Option Strict Off
' ============================================================================
'  Section 9.2 : COM et automation Office — liaison tardive (composant intégré)
'  Description : Liaison TARDIVE sur un composant COM toujours présent sous
'                Windows : Scripting.FileSystemObject. Illustre CreateObject
'                (instanciation par ProgID, sans référence d'interop), la
'                résolution de membres sur Object (IDispatch, d'où Option Strict
'                Off EN TÊTE DE CE SEUL FICHIER), la règle « un objet COM = une
'                variable » (chaque RCW intermédiaire est conservé pour être
'                libérable), et le nettoyage par Marshal.ReleaseComObject.
'  Fichier source : 02-com-office.md
' ============================================================================

Imports System
Imports System.IO
Imports System.Runtime.InteropServices

Public Module ComScripting

    Public Sub Executer()
        ' Liaison tardive : aucune référence d'interop, instanciation par ProgID.
        Dim fso As Object = CreateObject("Scripting.FileSystemObject")
        Try
            Dim chemin As String = Path.Combine(Path.GetTempPath(), "demo-com-vbnet.txt")

            ' Règle des « deux points » : on garde chaque RCW intermédiaire dans
            ' sa propre variable, sinon il devient impossible à libérer.
            Dim fichier As Object = fso.CreateTextFile(chemin, True)   ' True = écraser
            fichier.WriteLine("Bonjour COM depuis VB.NET")
            fichier.WriteLine("Ligne 2")
            fichier.Close()
            Marshal.ReleaseComObject(fichier)

            ' Relecture via le même composant COM.
            Dim flux As Object = fso.OpenTextFile(chemin, 1)            ' 1 = ForReading
            Dim contenu As String = flux.ReadAll()
            flux.Close()
            Marshal.ReleaseComObject(flux)

            Dim nbLignes = contenu.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries).Length
            Console.WriteLine($"  CreateObject(""Scripting.FileSystemObject"") OK")
            Console.WriteLine($"  Fichier écrit : {chemin}")
            Console.WriteLine($"  Relu : {nbLignes} ligne(s) ; FileExists = {fso.FileExists(chemin)}")

            fso.DeleteFile(chemin)
            Console.WriteLine($"  Supprimé ; FileExists = {fso.FileExists(chemin)}")
        Finally
            ' Libération du dernier RCW (ReleaseComObject n'est pas un Await : OK en Finally).
            Marshal.ReleaseComObject(fso)
            fso = Nothing
        End Try
    End Sub

End Module
