' ============================================================================
'  Section 3.2 : Structures (Structure) et tuples
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · sémantique de VALEUR : l'affectation copie (a intact) ;
'                  · Nothing remet une structure à sa valeur par défaut
'                    (jamais « nulle ») ; l'absence se modélise avec T? ;
'                  · égalité structurelle par défaut (ValueType.Equals) ;
'                  · structure immuable + IComparable -> tri d'une liste ;
'                  · boxing : la structure copiée sur le tas via Object ;
'                  · tuples : Item1/Item2, éléments nommés, retour multiple
'                    (DiviserAvecReste) — et PAS de déconstruction en VB ;
'                  · l'ancien System.Tuple (type référence).
'  Fichier source : 02-structures-tuples.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoSemantiqueValeur()
        DemoNothingEtNullable()
        DemoEgaliteStructurelle()
        DemoImmuableEtTri()
        DemoBoxing()
        DemoTuples()
    End Sub

    ' ---- Sémantique de valeur -------------------------------------------------
    Private Sub DemoSemantiqueValeur()
        Console.WriteLine("== Sémantique de valeur (copie) ==")

        Dim a As Point2D
        a.X = 1
        a.Y = 2

        Dim b = a        ' COPIE intégrale des données
        b.X = 99

        Console.WriteLine($"a.X = {a.X} (« a » n'a pas bougé)")
        Console.WriteLine($"b.X = {b.X}")
    End Sub

    ' ---- Nothing et absence ------------------------------------------------------
    Private Sub DemoNothingEtNullable()
        Console.WriteLine()
        Console.WriteLine("== Une structure n'est jamais nulle ==")

        Dim p As Point2D            ' immédiatement valide : X = 0, Y = 0
        Console.WriteLine($"Sans construction : X = {p.X}, Y = {p.Y}")

        Dim q As Point2D = Nothing  ' = valeur par défaut, pas « null »
        Console.WriteLine($"= Nothing -> X = {q.X}, Y = {q.Y} (valeur par défaut)")

        ' Pour modéliser l'ABSENCE : un nullable de valeur (§ 2.2)
        Dim absent As Point2D? = Nothing
        Console.WriteLine($"Point2D? = Nothing -> HasValue = {absent.HasValue}")
    End Sub

    ' ---- Égalité structurelle -------------------------------------------------------
    Private Sub DemoEgaliteStructurelle()
        Console.WriteLine()
        Console.WriteLine("== Égalité structurelle par défaut ==")

        Dim p1 As New PointImmuable(3, 4)
        Dim p2 As New PointImmuable(3, 4)
        Console.WriteLine($"p1.Equals(p2) = {p1.Equals(p2)} (mêmes champs -> égales)")
        ' (ValueType.Equals procède par réflexion : sur un chemin critique,
        '  redéfinir Equals/GetHashCode et implémenter IEquatable(Of T).)
    End Sub

    ' ---- Structure immuable + IComparable ----------------------------------------------
    Private Sub DemoImmuableEtTri()
        Console.WriteLine()
        Console.WriteLine("== Structure immuable + IComparable (tri) ==")

        Dim relevés As New List(Of Temperature) From {
            New Temperature(21.5),
            New Temperature(-3),
            New Temperature(35.2)
        }
        relevés.Sort()   ' possible grâce à IComparable(Of Temperature)
        Console.WriteLine($"Triées : {String.Join(" ; ", relevés)}")
    End Sub

    ' ---- Boxing ----------------------------------------------------------------------------
    Private Sub DemoBoxing()
        Console.WriteLine()
        Console.WriteLine("== Boxing (copie sur le tas) ==")

        Dim t As New Temperature(20)
        Dim o As Object = t          ' boxing : copie de la structure sur le tas
        Console.WriteLine($"o est une copie boxée : {o} (type : {o.GetType().Name})")
    End Sub

    ' ---- Tuples ----------------------------------------------------------------------------
    Public Function DiviserAvecReste(dividende As Integer, diviseur As Integer) _
            As (Quotient As Integer, Reste As Integer)
        Return (dividende \ diviseur, dividende Mod diviseur)
    End Function

    Private Sub DemoTuples()
        Console.WriteLine()
        Console.WriteLine("== Tuples ==")

        Dim point = (10, 20)               ' (Integer, Integer)
        Console.WriteLine($"point.Item1 = {point.Item1} ; point.Item2 = {point.Item2}")

        Dim personne = (Id:=1, Nom:="Alice")
        Console.WriteLine($"personne.Nom = {personne.Nom} (éléments nommés)")

        Dim coord As (X As Integer, Y As Integer) = (10, 20)
        Console.WriteLine($"coord.X = {coord.X}")

        ' Retour multiple d'une fonction : l'usage le plus utile
        Dim r = DiviserAvecReste(17, 5)
        Console.WriteLine($"DiviserAvecReste(17, 5) -> {r.Quotient} reste {r.Reste}")

        ' ❌ Pas de déconstruction en VB : « Dim (q, reste) = ... » n'existe pas.
        '    On lit les membres :
        Dim q = r.Quotient
        Dim reste = r.Reste
        Console.WriteLine($"Accès par membre : q = {q}, reste = {reste}")

        ' L'ancien System.Tuple (type référence), supplanté par ValueTuple :
        Dim ancien = Tuple.Create(1, "Alice")
        Console.WriteLine($"System.Tuple : Item1 = {ancien.Item1}, Item2 = {ancien.Item2}")
    End Sub

End Module
