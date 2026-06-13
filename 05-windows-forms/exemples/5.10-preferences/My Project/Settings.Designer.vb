' ============================================================================
'  Section 5.10 : Préférences et paramètres utilisateur (My.Settings)
'  Description : Accès fortement typé aux paramètres (My.Settings), tel que le
'                génère le concepteur de paramètres de Visual Studio à partir
'                de Settings.settings. Quatre paramètres de portée USER :
'                DernierDossier, ModeSombre, CompteurLancements et MettreAJour
'                (vrai par défaut, pour le motif Upgrade()).
'  Fichier source : 10-preferences.md
' ============================================================================

Option Strict On
Option Explicit On

Namespace My

    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.0.0")>
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase

        Private Shared defaultInstance As MySettings =
            CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()), MySettings)

        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                Return defaultInstance
            End Get
        End Property

        <Global.System.Configuration.UserScopedSettingAttribute(),
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
         Global.System.Configuration.DefaultSettingValueAttribute("")>
        Public Property DernierDossier() As String
            Get
                Return CType(Me("DernierDossier"), String)
            End Get
            Set(value As String)
                Me("DernierDossier") = value
            End Set
        End Property

        <Global.System.Configuration.UserScopedSettingAttribute(),
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
         Global.System.Configuration.DefaultSettingValueAttribute("False")>
        Public Property ModeSombre() As Boolean
            Get
                Return CType(Me("ModeSombre"), Boolean)
            End Get
            Set(value As Boolean)
                Me("ModeSombre") = value
            End Set
        End Property

        <Global.System.Configuration.UserScopedSettingAttribute(),
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
         Global.System.Configuration.DefaultSettingValueAttribute("0")>
        Public Property CompteurLancements() As Integer
            Get
                Return CType(Me("CompteurLancements"), Integer)
            End Get
            Set(value As Integer)
                Me("CompteurLancements") = value
            End Set
        End Property

        <Global.System.Configuration.UserScopedSettingAttribute(),
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
         Global.System.Configuration.DefaultSettingValueAttribute("True")>
        Public Property MettreAJour() As Boolean
            Get
                Return CType(Me("MettreAJour"), Boolean)
            End Get
            Set(value As Boolean)
                Me("MettreAJour") = value
            End Set
        End Property

    End Class

    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>
    Friend Module MySettingsProperty
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>
        Friend ReadOnly Property Settings() As Global.Preferences.My.MySettings
            Get
                Return Global.Preferences.My.MySettings.Default
            End Get
        End Property
    End Module

End Namespace
