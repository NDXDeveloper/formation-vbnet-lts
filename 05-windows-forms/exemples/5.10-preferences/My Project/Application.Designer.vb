' ============================================================================
'  Section 5.10 : Preferences et parametres utilisateur (My.Settings)
'  Description : Cadre applicatif VB.NET (designe le formulaire de demarrage).
'  Fichier source : 10-preferences.md
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
            Me.MainForm = Global.Preferences.MainForm
        End Sub

    End Class

End Namespace
