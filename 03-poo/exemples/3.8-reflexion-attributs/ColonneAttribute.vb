' ============================================================================
'  Section 3.8 : Réflexion et attributs
'  Description : L'attribut personnalisé exact de la section (hérite
'                d'Attribute, suffixe « Attribute », cible délimitée par
'                <AttributeUsage>) et le module Mapping qui le RELIT par
'                réflexion — le principe d'un ORM ou d'un sérialiseur en
'                miniature.
'  Fichier source : 08-reflexion-attributs.md
' ============================================================================

Imports System.Reflection

<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False)>
Public Class ColonneAttribute
    Inherits Attribute

    Public ReadOnly Property Nom As String          ' argument positionnel (constructeur)
    Public Property Obligatoire As Boolean = False  ' argument nommé (propriété)

    Public Sub New(nom As String)
        Me.Nom = nom
    End Sub
End Class

Public Module Mapping
    Public Sub DecrireColonnes(type As Type)
        For Each prop In type.GetProperties()
            Dim attr = prop.GetCustomAttribute(Of ColonneAttribute)()
            If attr IsNot Nothing Then
                Dim mention = If(attr.Obligatoire, " (obligatoire)", "")
                Console.WriteLine($"{prop.Name} → colonne « {attr.Nom} »{mention}")
            End If
        Next
    End Sub
End Module
