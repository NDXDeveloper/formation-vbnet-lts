' ============================================================================
'  Section 2.12 : L'espace My — un raccourci propre à VB.NET
'  Description : Accès fortement typé aux paramètres (My.Settings), tel que
'                le génère le Concepteur de paramètres de Visual Studio à
'                partir de Settings.settings — reproduit ici à l'identique.
'                Deux paramètres de portée utilisateur : NombreDeLancements
'                (défaut 0) et ThemeUtilisateur (défaut "Clair").
'  Fichier source : 12-espace-my.md
' ============================================================================

Option Strict On
Option Explicit On

Namespace My

    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.0.0")>
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase

        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()), MySettings)

        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                Return defaultInstance
            End Get
        End Property

        <Global.System.Configuration.UserScopedSettingAttribute(),
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
         Global.System.Configuration.DefaultSettingValueAttribute("0")>
        Public Property NombreDeLancements() As Integer
            Get
                Return CType(Me("NombreDeLancements"), Integer)
            End Get
            Set(value As Integer)
                Me("NombreDeLancements") = value
            End Set
        End Property

        <Global.System.Configuration.UserScopedSettingAttribute(),
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
         Global.System.Configuration.DefaultSettingValueAttribute("Clair")>
        Public Property ThemeUtilisateur() As String
            Get
                Return CType(Me("ThemeUtilisateur"), String)
            End Get
            Set(value As String)
                Me("ThemeUtilisateur") = value
            End Set
        End Property

    End Class

    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>
    Friend Module MySettingsProperty
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>
        Friend ReadOnly Property Settings() As Global.EspaceMy.My.MySettings
            Get
                Return Global.EspaceMy.My.MySettings.Default
            End Get
        End Property
    End Module

End Namespace
