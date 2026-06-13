' ============================================================================
'  Section 5.11 : Internationalisation (i18n/l10n, ressources .resx)
'  Description : Accès fortement typé aux ressources (My.Resources), tel que
'                le génère le concepteur de Visual Studio. Le ResourceManager
'                pointe sur la base « Internationalisation.Resources » ; il
'                résout automatiquement la culture selon resourceCulture
'                (ou CurrentUICulture), avec repli en cascade fr-FR → fr →
'                défaut.
'  Fichier source : 11-internationalisation.md
' ============================================================================

Option Strict On
Option Explicit On

Namespace My.Resources

    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0"),
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>
    Friend Module Resources

        Private resourceMan As Global.System.Resources.ResourceManager
        Private resourceCulture As Global.System.Globalization.CultureInfo

        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As New Global.System.Resources.ResourceManager(
                        "Internationalisation.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property

        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set(value As Global.System.Globalization.CultureInfo)
                resourceCulture = value
            End Set
        End Property

        ''' <summary>Message de bienvenue, résolu selon CurrentUICulture.</summary>
        Friend ReadOnly Property MessageBienvenue() As String
            Get
                Return ResourceManager.GetString("MessageBienvenue", resourceCulture)
            End Get
        End Property

    End Module

End Namespace
