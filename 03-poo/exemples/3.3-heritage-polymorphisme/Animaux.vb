' ============================================================================
'  Section 3.3 : Héritage et polymorphisme
'  Description : La hiérarchie Animal de la section, complète :
'                  · Inherits (Chien hérite de Manger, ajoute Aboyer) ;
'                  · constructeurs et MyBase.New (Animal n'impose rien ici
'                    grâce à son Sub New() ; Chien chaîne explicitement) ;
'                  · Overridable / Overrides (Crier) ;
'                  · MyBase.Crier() réutilisé par ChienPoli ;
'                  · NotOverridable Overrides (ChienDeGarde scelle Crier) ;
'                  · NotInheritable (Devise, non héritable).
'  Fichier source : 03-heritage-polymorphisme.md
' ============================================================================

Public Class Animal
    Public Property Nom As String

    Public Sub New()
    End Sub

    Public Sub New(nom As String)
        Me.Nom = nom
    End Sub

    Public Sub Manger()
        Console.WriteLine($"{Nom} mange.")
    End Sub

    Public Overridable Function Crier() As String
        Return "..."                 ' comportement par défaut
    End Function
End Class

Public Class Chien
    Inherits Animal          ' Chien hérite de tout ce qu'expose Animal

    Public Property Race As String

    Public Sub New()
    End Sub

    Public Sub New(nom As String, race As String)
        MyBase.New(nom)      ' appel explicite du constructeur de base
        Me.Race = race
    End Sub

    Public Sub Aboyer()
        Console.WriteLine("Ouaf !")
    End Sub

    Public Overrides Function Crier() As String
        Return "Ouaf"
    End Function
End Class

Public Class Chat
    Inherits Animal

    Public Overrides Function Crier() As String
        Return "Miaou"
    End Function
End Class

''' <summary>Réutilise l'implémentation héritée via MyBase.</summary>
Public Class ChienPoli
    Inherits Chien

    Public Overrides Function Crier() As String
        Return MyBase.Crier() & " (poliment)"   ' réutilise « Ouaf »
    End Function
End Class

''' <summary>Scelle la redéfinition : aucune sous-classe ne pourra redéfinir Crier.</summary>
Public Class ChienDeGarde
    Inherits Animal

    Public NotOverridable Overrides Function Crier() As String
        Return "GRRR"
    End Function
End Class

''' <summary>Classe scellée : Inherits Devise serait une erreur de compilation.</summary>
Public NotInheritable Class Devise
    Public ReadOnly Property Code As String

    Public Sub New(code As String)
        Me.Code = code
    End Sub
End Class
