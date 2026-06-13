' ============================================================================
'  Section 2.4 : Structures conditionnelles
'  Description : Hiérarchie minimale (Forme, Cercle, Rectangle) servant à
'                l'aiguillage par type TypeOf...Is / DirectCast de la section.
'  Fichier source : 04-conditions.md
' ============================================================================

''' <summary>Classe de base de l'aiguillage par type.</summary>
Public MustInherit Class Forme
End Class

Public Class Cercle
    Inherits Forme

    Public ReadOnly Property Rayon As Double

    Public Sub New(rayon As Double)
        Me.Rayon = rayon
    End Sub
End Class

Public Class Rectangle
    Inherits Forme

    Public ReadOnly Property Largeur As Double
    Public ReadOnly Property Hauteur As Double

    Public Sub New(largeur As Double, hauteur As Double)
        Me.Largeur = largeur
        Me.Hauteur = hauteur
    End Sub
End Class
