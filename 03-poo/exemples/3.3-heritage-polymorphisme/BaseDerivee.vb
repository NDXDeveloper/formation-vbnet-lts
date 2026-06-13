' ============================================================================
'  Section 3.3 : Héritage et polymorphisme
'  Description : Les deux démonstrations « Base / Derivee » de la section :
'                  · MyClass (propre à VB) : ViaMe() suit l'objet réel
'                    (appel virtuel), ViaMyClass() force l'implémentation
'                    définie dans la classe courante ;
'                  · Shadows : masque un membre (résolution selon le TYPE
'                    DÉCLARÉ de la variable, sans polymorphisme).
'  Fichier source : 03-heritage-polymorphisme.md
' ============================================================================

Public Class Base
    Public Overridable Function Valeur() As Integer
        Return 1
    End Function

    Public Function ViaMe() As Integer
        Return Me.Valeur()        ' appel virtuel : version la plus dérivée
    End Function

    Public Function ViaMyClass() As Integer
        Return MyClass.Valeur()   ' toujours Base.Valeur, même depuis une dérivée
    End Function

    Public Sub Afficher()
        Console.WriteLine("Base")
    End Sub
End Class

Public Class Derivee
    Inherits Base

    Public Overrides Function Valeur() As Integer
        Return 2
    End Function

    Public Shadows Sub Afficher()       ' masque Base.Afficher (pas de polymorphisme)
        Console.WriteLine("Derivee")
    End Sub
End Class
