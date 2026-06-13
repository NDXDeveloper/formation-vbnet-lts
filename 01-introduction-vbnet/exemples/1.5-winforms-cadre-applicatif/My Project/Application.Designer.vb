' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Configuration du cadre applicatif (Application Framework).
'                Dans Visual Studio, ce fichier est généré depuis l'onglet
'                « Propriétés du projet → Application » (et Application.myapp) ;
'                il est reproduit ici à l'identique. C'est OnCreateMainForm
'                qui désigne le formulaire de démarrage — il n'y a pas de
'                Sub Main écrit : le compilateur le génère dans
'                My.MyApplication (démarrage piloté par événements).
'  Fichier source : 05-premier-projet.md
' ============================================================================

Option Strict On
Option Explicit On

Namespace My

    'NOTE : ce fichier est généré automatiquement par Visual Studio ;
    'ne le modifiez pas directement — passez par les propriétés du projet
    '(onglet Application).
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
            Me.MainForm = Global.MonAppCadre.Form1
        End Sub

    End Class

End Namespace
