' ============================================================================
'  Section 3.3 : Héritage et polymorphisme
'  Description : Les classes abstraites de la section : Forme (MustInherit)
'                impose Aire() (MustOverride, sans corps) et fournit la
'                méthode concrète Decrire() ; Cercle et Rectangle sont les
'                implémentations concrètes — « New Forme() » serait une
'                erreur de compilation.
'  Fichier source : 03-heritage-polymorphisme.md
' ============================================================================

Public MustInherit Class Forme
    Public MustOverride Function Aire() As Double      ' abstrait : pas de corps

    Public Function Decrire() As String                ' méthode concrète héritée
        Return $"Cette forme a une aire de {Aire():F2}."
    End Function
End Class

Public Class Cercle
    Inherits Forme

    Public Property Rayon As Double

    Public Overrides Function Aire() As Double
        Return Math.PI * Rayon * Rayon
    End Function
End Class

Public Class Rectangle
    Inherits Forme

    Public Property Largeur As Double
    Public Property Hauteur As Double

    Public Overrides Function Aire() As Double
        Return Largeur * Hauteur
    End Function
End Class
