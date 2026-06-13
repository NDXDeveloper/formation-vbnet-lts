' ============================================================================
'  Section 5.3 : Controles fondamentaux
'  Description : Cadre applicatif VB.NET (genere par Visual Studio). Designe
'                le formulaire de demarrage via OnCreateMainForm.
'  Fichier source : 03-controles-fondamentaux.md
' ============================================================================

Option Strict On
Option Explicit On

Namespace My

    Partial Friend Class MyApplication

        <Global.System.Diagnostics.DebuggerStepThroughAttribute()>
        Public Sub New()
            MyBase.New(Global.Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
            Me.IsSingleInstance = False
            Me.EnableVisualStyles = True
            Me.SaveMySettingsOnExit = True
            Me.ShutDownStyle = Global.Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses
            Me.HighDpiMode = Global.System.Windows.Forms.HighDpiMode.SystemAware
        End Sub

        <Global.System.Diagnostics.DebuggerStepThroughAttribute()>
        Protected Overrides Sub OnCreateMainForm()
            Me.MainForm = Global.ControlesFondamentaux.MainForm
        End Sub

    End Class

End Namespace
