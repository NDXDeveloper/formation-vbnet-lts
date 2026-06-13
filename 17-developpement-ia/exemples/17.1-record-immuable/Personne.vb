' ============================================================================
'  Section 17.1 : Classe immuable VB (équivalent d'un record C#)
'  Description : Ce que l'IA devrait produire au lieu du record halluciné. La
'                construction C# invalide en VB serait :
'
'                    ' ❌ N'EXISTE PAS en VB : on ne DÉCLARE pas de record.
'                    ' Public Record Personne(Nom As String, Age As Integer)
'
'                L'équivalent VB correct est une classe immuable écrite à la main :
'                propriétés en lecture seule, égalité PAR VALEUR (IEquatable), hash
'                cohérent, ToString lisible, et une copie non destructive « Avec ».
'  Fichier source : 01-pourquoi-ia-vbnet.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System

''' <summary>Personne immuable comparée par valeur (équivalent d'un record C#).</summary>
Public NotInheritable Class Personne
    Implements IEquatable(Of Personne)

    Public ReadOnly Property Nom As String
    Public ReadOnly Property Age As Integer

    Public Sub New(nom As String, age As Integer)
        Me.Nom = nom
        Me.Age = age
    End Sub

    ''' <summary>Copie non destructive : équivalent de l'expression « with » de C#.</summary>
    Public Function Avec(Optional nom As String = Nothing,
                         Optional age As Integer? = Nothing) As Personne
        Return New Personne(If(nom, Me.Nom), If(age.HasValue, age.Value, Me.Age))
    End Function

    ' --- Égalité par valeur (ce qu'un record fournit automatiquement) ---

    Public Overloads Function Equals(autre As Personne) As Boolean Implements IEquatable(Of Personne).Equals
        If autre Is Nothing Then Return False
        Return Nom = autre.Nom AndAlso Age = autre.Age
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, Personne))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return HashCode.Combine(Nom, Age)
    End Function

    Public Overrides Function ToString() As String
        Return $"Personne {{ Nom = {Nom}, Age = {Age} }}"
    End Function

End Class
