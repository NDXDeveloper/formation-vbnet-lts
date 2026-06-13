' ============================================================================
'  Section 10.5 : Atelier — point d'entrée de l'UI (VB WinForms)
'  Description : Démarre l'application Windows Forms. Réinitialise le journal
'                d'auto-test puis lance la boucle de messages sur le formulaire
'                principal. <STAThread> est requis par Windows Forms.
'  Fichier source : 05-atelier-core-csharp-ui-vbnet.md
' ============================================================================

Imports System
Imports System.IO
Imports System.Windows.Forms

Module Program
    <STAThread>
    Sub Main()
        Try
            File.Delete(Path.Combine(Path.GetTempPath(), "importateur-autotest.log"))
        Catch
        End Try

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New FormPrincipal())
    End Sub
End Module
