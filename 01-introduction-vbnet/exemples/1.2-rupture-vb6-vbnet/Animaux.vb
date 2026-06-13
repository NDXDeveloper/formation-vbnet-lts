' ============================================================================
'  Section 1.2 : De Visual Basic 6 à VB.NET (histoire et héritage legacy)
'  Description : Hiérarchie de classes minimaliste démontrant l'héritage
'                d'implémentation (Inherits) et le polymorphisme (Overrides),
'                deux apports majeurs de VB.NET absents de VB6.
'  Fichier source : 02-histoire-evolution.md
' ============================================================================

''' <summary>Classe de base abstraite : ne peut pas être instanciée directement.</summary>
Public MustInherit Class Animal

    Public ReadOnly Property Nom As String

    Public Sub New(nom As String)
        Me.Nom = nom
    End Sub

    ''' <summary>Chaque animal concret doit fournir son cri.</summary>
    Public MustOverride Function Cri() As String

    ''' <summary>Comportement commun, hérité par toutes les classes dérivées.</summary>
    Public Function SePresenter() As String
        Return $"{Nom} ({Me.GetType().Name}) dit : {Cri()}"
    End Function

End Class

Public Class Chien
    Inherits Animal

    Public Sub New(nom As String)
        MyBase.New(nom)
    End Sub

    Public Overrides Function Cri() As String
        Return "Wouf !"
    End Function

End Class

Public Class Chat
    Inherits Animal

    Public Sub New(nom As String)
        MyBase.New(nom)
    End Sub

    Public Overrides Function Cri() As String
        Return "Miaou !"
    End Function

End Class
