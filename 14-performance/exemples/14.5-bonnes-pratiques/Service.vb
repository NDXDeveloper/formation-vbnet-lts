' ============================================================================
'  Section 14.5 : Bonnes pratiques — liaison anticipée (early binding)
'  Description : Avec Option Strict On, appeler une méthode via une interface
'                est résolu À LA COMPILATION (liaison anticipée) : rapide et sûr.
'                À comparer avec la liaison tardive (fichier LiaisonTardive.vb).
'  Fichier source : 05-bonnes-pratiques.md
' ============================================================================

Public Interface IService
    Function Traiter(x As Integer) As Integer
End Interface

Public Class ServiceImpl
    Implements IService
    Public Function Traiter(x As Integer) As Integer Implements IService.Traiter
        Return x * 2
    End Function
End Class
