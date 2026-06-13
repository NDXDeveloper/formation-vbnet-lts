' ============================================================================
'  Section 2.10 : Génériques avancés
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · Pile(Of T) : type générique maison (Pile.vb) ;
'                  · contraintes : Depot(Of T As {IEntite, New}) et
'                    Trieur(Of T As IComparable(Of T)) (Contraintes.vb) ;
'                  · méthodes génériques (Echanger ByRef, PlusGrand) et
'                    l'inférence de type aux appels ;
'                  · variance : covariance IEnumerable(Of Out T) /
'                    Action(Of In T), et interfaces variantes déclarées
'                    soi-même (Variance.vb) ;
'                  · la valeur par défaut d'un T : Nothing (pas de default(T)).
'  Fichier source : 10-generiques-avances.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoPile()
        DemoContraintes()
        DemoMethodesGeneriques()
        DemoVariance()
        DemoValeurParDefaut()
    End Sub

    ' ---- Type générique maison ------------------------------------------------
    Private Sub DemoPile()
        Console.WriteLine("== Pile(Of T) : type générique maison ==")
        Dim p As New Pile(Of String)   ' instanciation avec un type concret
        p.Empiler("premier")
        p.Empiler("second")
        Console.WriteLine($"Depiler() -> {p.Depiler()} puis {p.Depiler()} (LIFO)")
    End Sub

    ' ---- Contraintes -------------------------------------------------------------
    Private Sub DemoContraintes()
        Console.WriteLine()
        Console.WriteLine("== Contraintes (clause As) ==")

        ' As {IEntite, New} : Creer() peut faire New T()
        Dim depot As New Depot(Of Client)
        Dim client = depot.Creer()
        Console.WriteLine($"Depot(Of Client).Creer() -> {client.GetType().Name} (Id = {client.Id})")

        ' As IComparable(Of T) : EstAvant peut appeler CompareTo
        Dim trieur As New Trieur(Of Integer)
        Console.WriteLine($"Trieur(Of Integer).EstAvant(3, 7) -> {trieur.EstAvant(3, 7)}")
        Dim trieurTexte As New Trieur(Of String)
        Console.WriteLine($"Trieur(Of String).EstAvant(""xyz"", ""abc"") -> {trieurTexte.EstAvant("xyz", "abc")}")
    End Sub

    ' ---- Méthodes génériques --------------------------------------------------------
    Public Sub Echanger(Of T)(ByRef a As T, ByRef b As T)
        Dim temp = a
        a = b
        b = temp
    End Sub

    Public Function PlusGrand(Of T As IComparable(Of T))(a As T, b As T) As T
        Return If(a.CompareTo(b) >= 0, a, b)
    End Function

    Private Sub DemoMethodesGeneriques()
        Console.WriteLine()
        Console.WriteLine("== Méthodes génériques (inférence de type) ==")

        Dim m = PlusGrand(3, 7)          ' T inféré : Integer
        Dim s = PlusGrand("abc", "xyz")  ' T inféré : String
        Console.WriteLine($"PlusGrand(3, 7) = {m} ; PlusGrand(""abc"", ""xyz"") = {s}")
        ' Echanger(m, s)                 ' ERREUR : m et s n'ont pas le même T

        Dim x = 1, y = 2
        Echanger(x, y)                   ' T inféré : Integer (ByRef)
        Console.WriteLine($"Echanger(x, y) -> x = {x}, y = {y}")
    End Sub

    ' ---- Variance ------------------------------------------------------------------------
    Private Sub DemoVariance()
        Console.WriteLine()
        Console.WriteLine("== Variance ==")

        ' Covariance (Out) : IEnumerable(Of String) -> IEnumerable(Of Object)
        Dim chaines As IEnumerable(Of String) = {"a", "b", "c"}
        Dim objets As IEnumerable(Of Object) = chaines
        Console.WriteLine($"Covariance IEnumerable : {String.Join(", ", objets)}")

        ' Contravariance (In) : Action(Of Object) -> Action(Of String)
        Dim afficher As Action(Of Object) = Sub(o) Console.WriteLine($"  affiché : {o}")
        Dim afficherChaine As Action(Of String) = afficher
        afficherChaine("Bonjour")

        ' Interfaces variantes déclarées soi-même (Variance.vb)
        Dim producteurObjets As IProducteur(Of Object) = New ProducteurDeChaines()   ' Out
        Console.WriteLine($"IProducteur(Of Object) <- ProducteurDeChaines : {producteurObjets.Produire()}")

        Dim consommateurChaines As IConsommateur(Of String) = New AfficheurUniversel()  ' In
        consommateurChaines.Consommer("texte accepté par un consommateur d'Object")

        ' Invariance : List(Of T) lit ET écrit T -> aucune conversion :
        ' Dim o As IList(Of Object) = New List(Of String)()   ' NE COMPILE PAS
    End Sub

    ' ---- Valeur par défaut d'un T ------------------------------------------------------------
    Public Function ValeurParDefaut(Of T)() As T
        Dim valeur As T = Nothing   ' valeur par défaut de T, quel qu'il soit
        Return valeur
    End Function

    Private Sub DemoValeurParDefaut()
        Console.WriteLine()
        Console.WriteLine("== Valeur par défaut : Nothing (pas de default(T) en VB) ==")
        Console.WriteLine($"ValeurParDefaut(Of Integer)() = {ValeurParDefaut(Of Integer)()}")
        Console.WriteLine($"ValeurParDefaut(Of Boolean)() = {ValeurParDefaut(Of Boolean)()}")
        Console.WriteLine($"ValeurParDefaut(Of String)() Is Nothing = {ValeurParDefaut(Of String)() Is Nothing}")
    End Sub

End Module
