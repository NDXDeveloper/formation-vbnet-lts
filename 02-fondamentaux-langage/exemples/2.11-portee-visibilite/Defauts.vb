' ============================================================================
'  Section 2.11 : Portée, visibilité et modificateurs d'accès
'  Description : L'asymétrie des niveaux d'accès PAR DÉFAUT signalée par la
'                section : un champ déclaré avec Dim est Public par défaut
'                dans une Structure... mais Private par défaut dans une Class.
'                D'où la bonne pratique : toujours écrire le modificateur.
'  Fichier source : 11-portee-visibilite.md
' ============================================================================

''' <summary>Dans une Structure, un champ Dim est PUBLIC par défaut.</summary>
Public Structure PointStruct
    Dim X As Integer        ' Public par défaut (asymétrie !)
End Structure

''' <summary>Dans une Class, un champ Dim est PRIVATE par défaut.</summary>
Public Class PointClasse
    Dim Y As Integer        ' Private par défaut

    ''' <summary>Le champ privé reste utilisable À L'INTÉRIEUR du type.</summary>
    Public Function LireY() As Integer
        Return Y
    End Function
End Class
