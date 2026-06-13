' ============================================================================
'  Section 2.4 : Structures conditionnelles
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · If...Then...Else en bloc (ElseIf en un mot) et sur une ligne ;
'                  · l'opérateur If() — ternaire et coalescence — et le piège
'                    de la fonction héritée IIf() (évalue TOUJOURS les deux
'                    branches), démontré par compteurs d'évaluation ;
'                  · Select Case : plages To, listes de valeurs, Case Is <,
'                    Case Else, chaînes — sans fall-through ;
'                  · TypeOf...Is puis transtypage en deux temps, et
'                    l'aiguillage par type If/ElseIf (Formes.vb) — la limite
'                    assumée de VB face au pattern matching de C#.
'  Fichier source : 04-conditions.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoIfThenElse()
        DemoOperateurIf()
        DemoIfContreIIf()
        DemoSelectCase()
        DemoTypeOfEtAiguillage()
    End Sub

    ' ---- If...Then...Else -------------------------------------------------
    Private Sub DemoIfThenElse()
        Console.WriteLine("== If...Then...Else ==")
        For Each solde In {100D, 0D, -50D}
            Console.Write($"solde = {solde,4} -> ")
            If solde > 0 Then
                Console.WriteLine("Créditeur")
            ElseIf solde = 0 Then          ' ElseIf en un seul mot
                Console.WriteLine("À zéro")
            Else
                Console.WriteLine("Débiteur")
            End If
        Next

        ' Forme sur une ligne (sans End If)
        Dim x = 5
        If x > 0 Then Console.WriteLine("(une ligne) x = 5 -> positif") Else Console.WriteLine("négatif ou nul")
    End Sub

    ' ---- L'opérateur If() : ternaire et coalescence --------------------------
    Private Sub DemoOperateurIf()
        Console.WriteLine()
        Console.WriteLine("== Opérateur If() ==")

        Dim age = 20
        Dim statut = If(age >= 18, "majeur", "mineur")     ' ternaire
        Console.WriteLine($"age = {age} -> {statut}")

        Dim saisie As String = Nothing
        Dim nom = If(saisie, "Anonyme")                    ' coalescence (le ?? de C#)
        Console.WriteLine($"saisie = Nothing -> If(saisie, ""Anonyme"") = {nom}")
    End Sub

    ' ---- If() vs IIf() : le piège de l'évaluation ------------------------------
    Private _evalVraie As Integer = 0
    Private _evalFausse As Integer = 0

    Private Function BrancheVraie() As String
        _evalVraie += 1
        Return "vrai"
    End Function

    Private Function BrancheFausse() As String
        _evalFausse += 1
        Return "faux"
    End Function

    Private Sub DemoIfContreIIf()
        Console.WriteLine()
        Console.WriteLine("== If() vs IIf() : le piège de l'évaluation ==")

        ' IIf est un APPEL DE FONCTION : les deux branches sont évaluées.
        _evalVraie = 0 : _evalFausse = 0
        Dim resultatIIf = IIf(True, BrancheVraie(), BrancheFausse())
        Console.WriteLine($"IIf(True, ...) -> branche vraie évaluée {_evalVraie} fois, branche fausse évaluée {_evalFausse} fois (LES DEUX !)")

        ' L'opérateur If() court-circuite : seule la branche retenue est évaluée.
        _evalVraie = 0 : _evalFausse = 0
        Dim resultatIf = If(True, BrancheVraie(), BrancheFausse())
        Console.WriteLine($"If(True, ...)  -> branche vraie évaluée {_evalVraie} fois, branche fausse évaluée {_evalFausse} fois (court-circuit)")

        ' Conclusion de la section : utilisez TOUJOURS l'opérateur If(), jamais IIf().
    End Sub

    ' ---- Select Case ---------------------------------------------------------------
    Private Sub DemoSelectCase()
        Console.WriteLine()
        Console.WriteLine("== Select Case ==")

        For Each note In {95, 75, 55, 30, 150}
            Console.Write($"note = {note,3} -> ")
            Select Case note
                Case 90 To 100
                    Console.WriteLine("Excellent")     ' plage inclusive
                Case 70 To 89
                    Console.WriteLine("Bien")
                Case 50, 55, 60
                    Console.WriteLine("Passable")      ' plusieurs valeurs
                Case Is < 50
                    Console.WriteLine("Insuffisant")   ' comparaison relationnelle
                Case Else
                    Console.WriteLine("Note invalide")
            End Select
        Next

        ' Sur des chaînes (sensibilité à la casse : Option Compare, ici Binary)
        For Each commande In {"Ouvrir", "CLOSE", "imprimer"}
            Console.Write($"commande ""{commande}"" -> ")
            Select Case commande.ToLower()
                Case "ouvrir", "open"
                    Console.WriteLine("action : ouverture")
                Case "fermer", "close"
                    Console.WriteLine("action : fermeture")
                Case Else
                    Console.WriteLine("Commande inconnue")
            End Select
        Next
    End Sub

    ' ---- TypeOf...Is et aiguillage par type ----------------------------------------
    Private Sub DemoTypeOfEtAiguillage()
        Console.WriteLine()
        Console.WriteLine("== TypeOf...Is et aiguillage par type ==")

        ' L'idiome VB : tester d'abord, transtyper ensuite (deux temps).
        Dim o As Object = "Bonjour"
        If TypeOf o Is String Then
            Dim s = DirectCast(o, String)
            Console.WriteLine($"o est un String de longueur {s.Length}")
        End If

        ' Aiguillage par type via If/ElseIf — la limite assumée de VB
        ' (pas d'expression switch ni de liaison de motif comme en C#).
        For Each forme As Forme In New Forme() {New Cercle(2), New Rectangle(3, 4)}
            Dim aire As Double
            If TypeOf forme Is Cercle Then
                Dim c = DirectCast(forme, Cercle)
                aire = Math.PI * c.Rayon ^ 2
            ElseIf TypeOf forme Is Rectangle Then
                Dim r = DirectCast(forme, Rectangle)
                aire = r.Largeur * r.Hauteur
            End If
            Console.WriteLine($"aire du {forme.GetType().Name} : {aire:F2}")
        Next
    End Sub

End Module
