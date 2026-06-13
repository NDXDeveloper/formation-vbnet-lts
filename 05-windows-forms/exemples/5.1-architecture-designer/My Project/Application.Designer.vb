' ============================================================================
'  Section 5.1 : Introduction, architecture et le Concepteur (Designer)
'  Description : Cadre applicatif VB.NET (généré par Visual Studio depuis
'                l'onglet Application). Il fournit le point d'entrée et
'                désigne le formulaire de démarrage via OnCreateMainForm —
'                aucun Sub Main n'est écrit à la main.
'  Fichier source : 01-introduction-designer.md
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
            Me.MainForm = Global.MonApplication.Form1
        End Sub

    End Class

End Namespace
