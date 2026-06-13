' ============================================================================
'  Section 2.3 : Opérateurs et expressions
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · arithmétique : les DEUX divisions (/ flottante, \ entière),
'                    Mod, et ^ qui est la PUISSANCE (le XOR s'écrit Xor) ;
'                  · concaténation & (sans ambiguïté, contrairement à +) ;
'                  · comparaisons (= double rôle, <> pour l'inégalité) ;
'                  · égalité de référence Is / IsNot ;
'                  · correspondance de motifs Like ;
'                  · logique à court-circuit AndAlso / OrElse (démontrée par
'                    compteur d'évaluations) vs And / Or bit-à-bit ;
'                  · affectations composées (pas de ++ en VB) ;
'                  · conversions CInt / CType / DirectCast / TryCast ;
'                  · priorité des opérateurs (And prioritaire sur Or).
'  Fichier source : 03-operateurs.md
'  Compilation    : dotnet build      Exécution : dotnet run
'  Note           : les flottants s'affichent selon la culture de la machine
'                   (« 3,5 » sur un poste en français).
' ============================================================================

Imports System

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoArithmetique()
        DemoConcatenation()
        DemoComparaisons()
        DemoIsIsNot()
        DemoLike()
        DemoCourtCircuit()
        DemoBitABit()
        DemoAffectationsComposees()
        DemoConversions()
        DemoPriorite()
    End Sub

    ' ---- Arithmétique : deux divisions et la puissance ---------------------
    Private Sub DemoArithmetique()
        Console.WriteLine("== Arithmétique ==")
        Dim quotientReel = 7 / 2     ' division flottante : renvoie toujours Double
        Dim quotientEntier = 7 \ 2   ' division entière
        Dim reste = 7 Mod 2          ' modulo
        Dim puissance = 2 ^ 10       ' exponentiation (PAS un XOR !)

        Console.WriteLine($"7 / 2   = {quotientReel} (division flottante)")
        Console.WriteLine($"7 \ 2   = {quotientEntier} (division entière)")
        Console.WriteLine($"7 Mod 2 = {reste}")
        Console.WriteLine($"2 ^ 10  = {puissance} (puissance — le XOR s'écrit Xor)")
        Console.WriteLine($"6 Xor 3 = {6 Xor 3}")
    End Sub

    ' ---- Concaténation : & ---------------------------------------------------
    Private Sub DemoConcatenation()
        Console.WriteLine()
        Console.WriteLine("== Concaténation (&) ==")
        Dim nom = "Ada"
        Dim message = "Bonjour " & nom & " !"
        Dim etiquette = "Total : " & 42   ' & convertit ses opérandes en chaîne
        Console.WriteLine(message)
        Console.WriteLine(etiquette)
        ' "Total : " + 42 -> rejeté sous Option Strict On : réservez + à l'arithmétique.
    End Sub

    ' ---- Comparaisons ----------------------------------------------------------
    Private Sub DemoComparaisons()
        Console.WriteLine()
        Console.WriteLine("== Comparaisons ==")
        Dim x As Integer = 5            ' '=' : ici, AFFECTATION
        If x = 5 Then                   ' '=' : ici, COMPARAISON
            Console.WriteLine("x = 5 -> égal (pas de == en VB)")
        End If
        Dim different = (x <> 3)        ' inégalité : <> (pas !=)
        Console.WriteLine($"x <> 3 -> {different}")
    End Sub

    ' ---- Is / IsNot : égalité de référence --------------------------------------
    Private Sub DemoIsIsNot()
        Console.WriteLine()
        Console.WriteLine("== Is / IsNot (références) ==")
        Dim a As New Object()
        Dim b As Object = a
        Dim c As New Object()

        Console.WriteLine($"a Is b    -> {a Is b} (même objet)")
        Console.WriteLine($"a Is c    -> {a Is c} (objets distincts)")
        Console.WriteLine($"a IsNot c -> {a IsNot c}")

        Dim s As String = Nothing
        If s Is Nothing Then Console.WriteLine("s Is Nothing -> référence nulle")
    End Sub

    ' ---- Like : correspondance de motifs ------------------------------------------
    Private Sub DemoLike()
        Console.WriteLine()
        Console.WriteLine("== Like (motifs) ==")
        Console.WriteLine($"""VB.NET"" Like ""VB*""          -> {"VB.NET" Like "VB*"}")
        Console.WriteLine($"""fichier.txt"" Like ""*.txt""   -> {"fichier.txt" Like "*.txt"}")
        Console.WriteLine($"""A1"" Like ""[A-Z]#""           -> {"A1" Like "[A-Z]#"} (une lettre puis un chiffre)")
        Console.WriteLine($"""B9"" Like ""[!A]#""            -> {"B9" Like "[!A]#"} (un caractère hors liste)")
    End Sub

    ' ---- AndAlso / OrElse : court-circuit -------------------------------------------
    Private _evaluationsDroites As Integer = 0

    Private Function CoteDroitEvalue() As Boolean
        _evaluationsDroites += 1
        Return True
    End Function

    Private Sub DemoCourtCircuit()
        Console.WriteLine()
        Console.WriteLine("== AndAlso / OrElse (court-circuit) ==")

        _evaluationsDroites = 0
        Dim r1 = False And CoteDroitEvalue()      ' And évalue TOUJOURS les deux côtés
        Console.WriteLine($"False And f()     -> {r1} ; f() évaluée {_evaluationsDroites} fois")

        _evaluationsDroites = 0
        Dim r2 = False AndAlso CoteDroitEvalue()  ' AndAlso s'arrête à gauche
        Console.WriteLine($"False AndAlso f() -> {r2} ; f() évaluée {_evaluationsDroites} fois (court-circuit)")

        ' Le cas d'école de la section : le test de Nothing sûr
        Dim client As Client = Nothing
        If client IsNot Nothing AndAlso client.Solde > 0 Then
            Console.WriteLine("Client créditeur")
        Else
            Console.WriteLine("client IsNot Nothing AndAlso client.Solde > 0 -> sûr même avec client = Nothing")
        End If
        ' Avec 'And', client.Solde serait évalué -> NullReferenceException.
    End Sub

    ' ---- Bit-à-bit et décalages --------------------------------------------------
    Private Sub DemoBitABit()
        Console.WriteLine()
        Console.WriteLine("== Bit-à-bit et décalages ==")
        Dim masque = &B1100 And &B1010
        Dim drapeaux = &B0001 Or &B0100
        Dim decale = 1 << 4
        Console.WriteLine($"&B1100 And &B1010 = {masque}")
        Console.WriteLine($"&B0001 Or &B0100  = {drapeaux}")
        Console.WriteLine($"1 << 4            = {decale}")
    End Sub

    ' ---- Affectations composées -----------------------------------------------------
    Private Sub DemoAffectationsComposees()
        Console.WriteLine()
        Console.WriteLine("== Affectations composées (pas de ++/--) ==")
        Dim total = 0
        total += 10
        total *= 2
        total \= 3
        Console.WriteLine($"0 +=10 *=2 \=3 -> {total}")

        Dim texte = "a"
        texte &= "b"
        Console.WriteLine($"""a"" &= ""b"" -> {texte}")

        total += 1   ' l'incrément s'écrit explicitement
        Console.WriteLine($"total += 1 -> {total}")
    End Sub

    ' ---- Conversions dans les expressions ----------------------------------------------
    Private Sub DemoConversions()
        Console.WriteLine()
        Console.WriteLine("== Conversions (CInt, CType, DirectCast, TryCast) ==")

        Dim o As Object = "123"
        Dim n As Integer = CInt("42")              ' fonctions Cxxx
        Dim s1 As String = CType(o, String)        ' conversion générale
        Dim s2 As String = DirectCast(o, String)   ' transtypage strict, sans conversion
        Dim s3 As String = TryCast(o, String)      ' Nothing si le type ne correspond pas

        Console.WriteLine($"CInt(""42"") = {n}")
        Console.WriteLine($"CType / DirectCast / TryCast -> ""{s1}"" / ""{s2}"" / ""{s3}""")

        Dim autre As Object = 123                  ' un Integer, pas une chaîne
        Console.WriteLine($"TryCast(123, String) Is Nothing -> {TryCast(autre, String) Is Nothing}")
    End Sub

    ' ---- Priorité des opérateurs ----------------------------------------------------------
    Private Sub DemoPriorite()
        Console.WriteLine()
        Console.WriteLine("== Priorité : And/AndAlso avant Or/OrElse ==")
        ' a Or b AndAlso c  ==  a Or (b AndAlso c)
        Console.WriteLine($"True Or False AndAlso False   -> {True Or False AndAlso False} (lu : True Or (False AndAlso False))")
        Console.WriteLine($"(True Or False) AndAlso False -> {(True Or False) AndAlso False} (le parenthésage change tout)")
    End Sub

End Module
