' ============================================================================
'  Section 2.12 : L'espace My — un raccourci propre à VB.NET
'  Description : Cadre applicatif (généré par Visual Studio depuis l'onglet
'                Application ; reproduit ici à l'identique). My.Application
'                expose, grâce à lui, les événements globaux, l'écran de
'                démarrage et My.Application.OpenForms.
'  Fichier source : 12-espace-my.md
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
            Me.MainForm = Global.EspaceMy.Form1
        End Sub

    End Class

End Namespace
