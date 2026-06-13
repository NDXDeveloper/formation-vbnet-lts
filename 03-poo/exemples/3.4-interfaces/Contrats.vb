' ============================================================================
'  Section 3.4 : Interfaces
'  Description : Les contrats et implémentations de la section :
'                  · IFormeGeometrique et Cercle — la double obligation VB :
'                    « Implements I » sur la classe ET « Implements I.Membre »
'                    sur CHAQUE membre ;
'                  · Rectangle — le membre implémentant peut porter un AUTRE
'                    nom (CalculerPerimetre satisfait Perimetre) ;
'                  · Ressource — implémentation « masquée » : le membre
'                    Private n'est accessible que par le contrat (l'équivalent
'                    VB de l'implémentation explicite C#), et un même membre
'                    peut satisfaire PLUSIEURS contrats (Dispose + Fermer) ;
'                  · ILisible / IModifiable — héritage d'interfaces ;
'                  · Rapport — implémentation de plusieurs interfaces.
'  Fichier source : 04-interfaces.md
' ============================================================================

Public Interface IFormeGeometrique
    ReadOnly Property Aire As Double
    Function Perimetre() As Double
    Sub Redimensionner(facteur As Double)
End Interface

Public Class Cercle
    Implements IFormeGeometrique

    Public Property Rayon As Double

    Public ReadOnly Property Aire As Double Implements IFormeGeometrique.Aire
        Get
            Return Math.PI * Rayon * Rayon
        End Get
    End Property

    Public Function Perimetre() As Double Implements IFormeGeometrique.Perimetre
        Return 2 * Math.PI * Rayon
    End Function

    Public Sub Redimensionner(facteur As Double) Implements IFormeGeometrique.Redimensionner
        Rayon *= facteur
    End Sub
End Class

''' <summary>Le membre implémentant peut porter un autre nom que celui du contrat.</summary>
Public Class Rectangle
    Implements IFormeGeometrique

    Public Property Largeur As Double
    Public Property Hauteur As Double

    Public ReadOnly Property Aire As Double Implements IFormeGeometrique.Aire
        Get
            Return Largeur * Hauteur
        End Get
    End Property

    Public Function CalculerPerimetre() As Double Implements IFormeGeometrique.Perimetre
        Return 2 * (Largeur + Hauteur)
    End Function

    Public Sub Redimensionner(facteur As Double) Implements IFormeGeometrique.Redimensionner
        Largeur *= facteur
        Hauteur *= facteur
    End Sub
End Class

' ---------------------------------------------------------------------------

Public Interface IFichier
    Sub Fermer()
End Interface

''' <summary>
''' Implémentation masquée (membre Private accessible seulement via le
''' contrat) ET multi-contrats : Liberer satisfait IDisposable.Dispose
''' et IFichier.Fermer en une seule implémentation.
''' </summary>
Public Class Ressource
    Implements IDisposable, IFichier

    Private Sub Liberer() Implements IDisposable.Dispose, IFichier.Fermer
        Console.WriteLine("  Ressource libérée (implémentation masquée).")
    End Sub
End Class

' ---------------------------------------------------------------------------

Public Interface ILisible
    Function Lire() As String
End Interface

Public Interface IModifiable
    Inherits ILisible            ' IModifiable inclut le contrat de ILisible
    Sub Ecrire(contenu As String)
End Interface

''' <summary>Implémenter IModifiable impose Lire ET Ecrire.</summary>
Public Class FichierTexte
    Implements IModifiable

    Private _contenu As String = ""

    Public Function Lire() As String Implements ILisible.Lire
        Return _contenu
    End Function

    Public Sub Ecrire(contenu As String) Implements IModifiable.Ecrire
        _contenu = contenu
    End Sub
End Class

' ---------------------------------------------------------------------------

Public Interface IImprimable
    Sub Imprimer()
End Interface

Public Interface IEnregistrable
    Sub Enregistrer(chemin As String)
End Interface

''' <summary>Plusieurs contrats honorés simultanément.</summary>
Public Class Rapport
    Implements IImprimable, IEnregistrable

    Public Sub Imprimer() Implements IImprimable.Imprimer
        Console.WriteLine("  Rapport imprimé.")
    End Sub

    Public Sub Enregistrer(chemin As String) Implements IEnregistrable.Enregistrer
        Console.WriteLine($"  Rapport enregistré dans {chemin}.")
    End Sub
End Class
