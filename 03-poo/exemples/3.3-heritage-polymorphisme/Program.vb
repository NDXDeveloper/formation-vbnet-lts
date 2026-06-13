' ============================================================================
'  Section 3.3 : Héritage et polymorphisme
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · héritage simple : membres hérités + membres propres ;
'                  · MyBase.New (constructeur de base appelé en premier) ;
'                  · polymorphisme : une List(Of Animal) hétérogène où chaque
'                    objet répond pour lui-même (Overridable/Overrides) ;
'                  · MyBase.Crier() (ChienPoli) et NotOverridable (ChienDeGarde) ;
'                  · MyClass : ViaMe() = version dérivée, ViaMyClass() = version
'                    de la classe de déclaration — propre à VB ;
'                  · MustInherit/MustOverride : Forme abstraite (BC30569
'                    vérifiée si l'on tente New Forme()) ;
'                  · NotInheritable : Devise scellée (BC30299 vérifiée) ;
'                  · Shadows vs Overrides : résolution statique vs dynamique ;
'                  · TypeOf...Is + DirectCast et TryCast + test de nullité.
'  Fichier source : 03-heritage-polymorphisme.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoHeritage()
        DemoPolymorphisme()
        DemoMyClass()
        DemoAbstraction()
        DemoShadows()
        DemoReferencesPolymorphes()
    End Sub

    ' ---- Héritage simple et MyBase.New ----------------------------------------
    Private Sub DemoHeritage()
        Console.WriteLine("== Inherits et MyBase.New ==")

        Dim rex As New Chien With {.Nom = "Rex"}
        rex.Manger()      ' méthode héritée d'Animal
        rex.Aboyer()      ' méthode propre à Chien

        Dim medor As New Chien("Médor", "Border Collie")   ' MyBase.New(nom) puis Race
        Console.WriteLine($"{medor.Nom} est un {medor.Race}.")
    End Sub

    ' ---- Polymorphisme ------------------------------------------------------------
    Private Sub DemoPolymorphisme()
        Console.WriteLine()
        Console.WriteLine("== Polymorphisme (résolution dynamique) ==")

        Dim animaux As New List(Of Animal) From {
            New Chien With {.Nom = "Rex"},
            New Chat With {.Nom = "Félix"},
            New ChienPoli With {.Nom = "Médor"},
            New ChienDeGarde With {.Nom = "Brutus"}
        }

        For Each a In animaux
            Console.WriteLine($"{a.Nom,-7} -> {a.Crier()}")   ' chaque objet répond pour lui-même
        Next
    End Sub

    ' ---- MyClass (propre à VB) ------------------------------------------------------
    Private Sub DemoMyClass()
        Console.WriteLine()
        Console.WriteLine("== MyClass : forcer l'implémentation de la classe courante ==")

        Dim d As New Derivee()
        Console.WriteLine($"d.ViaMe()      = {d.ViaMe()} (appel virtuel : version de Derivee)")
        Console.WriteLine($"d.ViaMyClass() = {d.ViaMyClass()} (force la version de Base)")
    End Sub

    ' ---- MustInherit / NotInheritable ---------------------------------------------------
    Private Sub DemoAbstraction()
        Console.WriteLine()
        Console.WriteLine("== MustInherit / MustOverride ==")

        Dim formes As New List(Of Forme) From {
            New Cercle With {.Rayon = 2},
            New Rectangle With {.Largeur = 3, .Hauteur = 4}
        }
        For Each f In formes
            Console.WriteLine($"{f.GetType().Name,-9} : {f.Decrire()}")
        Next
        ' Dim impossible As New Forme()      ' ERREUR BC30569 (vérifiée) :
        '                                    ' « 'New' ne peut pas être utilisé dans une
        '                                    '   classe déclarée 'MustInherit' »

        Dim euro As New Devise("EUR")
        Console.WriteLine($"Devise scellée : {euro.Code} (NotInheritable)")
        ' Public Class DeviseSpeciale : Inherits Devise   ' ERREUR BC30299 (vérifiée) :
        '                                    ' héritage impossible d'une classe NotInheritable
    End Sub

    ' ---- Shadows vs Overrides --------------------------------------------------------------
    Private Sub DemoShadows()
        Console.WriteLine()
        Console.WriteLine("== Shadows : résolution selon le TYPE DÉCLARÉ ==")

        Dim d As New Derivee()
        Console.Write("d.Afficher() (type déclaré Derivee) -> ")
        d.Afficher()                 ' « Derivee »

        Dim b As Base = d
        Console.Write("b.Afficher() (type déclaré Base)    -> ")
        b.Afficher()                 ' « Base » — le masquage ne suit pas l'objet réel
    End Sub

    ' ---- Références polymorphes : TypeOf / TryCast --------------------------------------------
    Private Sub DemoReferencesPolymorphes()
        Console.WriteLine()
        Console.WriteLine("== TypeOf...Is et TryCast ==")

        Dim animaux As New List(Of Animal) From {
            New Chien With {.Nom = "Rex"},
            New Chat With {.Nom = "Félix"}
        }

        For Each a In animaux
            If TypeOf a Is Chien Then
                Dim chien = DirectCast(a, Chien)
                Console.Write($"{a.Nom} est un Chien : ")
                chien.Aboyer()
            End If
        Next

        Dim animal As Animal = animaux(1)
        Dim peutEtreChat = TryCast(animal, Chat)        ' Nothing si échec, sans exception
        If peutEtreChat IsNot Nothing Then
            Console.WriteLine($"TryCast réussi : {peutEtreChat.Nom} dit {peutEtreChat.Crier()}")
        End If
    End Sub

End Module
