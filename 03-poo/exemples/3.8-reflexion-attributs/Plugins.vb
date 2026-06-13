' ============================================================================
'  Section 3.8 : Réflexion et attributs
'  Description : Le scénario « plugins » de la section : parcourir les types
'                d'un assembly et découvrir ceux qui implémentent IPlugin
'                (IsAssignableFrom + Not IsAbstract) — la base d'une
'                architecture à extensions.
'  Fichier source : 08-reflexion-attributs.md
' ============================================================================

Public Interface IPlugin
    ReadOnly Property Nom As String
    Sub Executer()
End Interface

''' <summary>Abstraite : doit être EXCLUE par le filtre Not IsAbstract.</summary>
Public MustInherit Class PluginBase
    Implements IPlugin

    Public MustOverride ReadOnly Property Nom As String Implements IPlugin.Nom

    Public Sub Executer() Implements IPlugin.Executer
        Console.WriteLine($"  exécution du plugin « {Nom} »")
    End Sub
End Class

Public Class PluginExport
    Inherits PluginBase

    Public Overrides ReadOnly Property Nom As String
        Get
            Return "Export CSV"
        End Get
    End Property
End Class

Public Class PluginImport
    Inherits PluginBase

    Public Overrides ReadOnly Property Nom As String
        Get
            Return "Import XML"
        End Get
    End Property
End Class
