' ============================================================================
'  Section 2.10 : Génériques avancés
'  Description : Le type générique exact de la section : une pile maison
'                paramétrée par (Of T) — la syntaxe VB des génériques
'                (équivalent du <T> de C#).
'  Fichier source : 10-generiques-avances.md
' ============================================================================

Imports System.Collections.Generic

Public Class Pile(Of T)
    Private elements As New List(Of T)

    Public Sub Empiler(item As T)
        elements.Add(item)
    End Sub

    Public Function Depiler() As T
        Dim dernier = elements(elements.Count - 1)
        elements.RemoveAt(elements.Count - 1)
        Return dernier
    End Function
End Class
