' ============================================================================
'  Section 2.12 : L'espace My — un raccourci propre à VB.NET
'  Description : Accès fortement typé aux ressources (My.Resources), tel que
'                le génère le Concepteur de ressources de Visual Studio —
'                reproduit ici à l'identique pour la chaîne MessageBienvenue.
'  Fichier source : 12-espace-my.md
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
                    Dim temp As New Global.System.Resources.ResourceManager("EspaceMy.Resources", GetType(Resources).Assembly)
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

        ''' <summary>Chaîne de bienvenue, utile pour l'internationalisation (→ 5.11).</summary>
        Friend ReadOnly Property MessageBienvenue() As String
            Get
                Return ResourceManager.GetString("MessageBienvenue", resourceCulture)
            End Get
        End Property

    End Module

End Namespace
