' ============================================================================
'  Section 2.5 : Boucles et itérations
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · For...Next : borne supérieure INCLUSE, Step négatif,
'                    Exit For / Continue For ;
'                  · For Each : parcours d'une séquence, et la suppression
'                    SÛRE d'éléments (parcours à rebours, RemoveAll) ;
'                  · Do...Loop : condition en tête (While / Until) ou en
'                    queue (au moins une exécution) — la lecture de lignes du
'                    cours est simulée de façon déterministe ;
'                  · While...End While (et non Wend, disparu avec VB6) ;
'                  · les mots-clés de sortie typés (Exit For, Continue Do...).
'  Fichier source : 05-boucles.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoForNext()
        DemoForEach()
        DemoSuppressionSure()
        DemoDoLoop()
        DemoWhile()
    End Sub

    ' ---- For...Next ---------------------------------------------------------
    Private Sub DemoForNext()
        Console.WriteLine("== For...Next (borne supérieure INCLUSE) ==")

        Console.Write("For i = 1 To 5          -> ")
        For i As Integer = 1 To 5
            Console.Write(i & " ")   ' 1 2 3 4 5 : 5 inclus
        Next
        Console.WriteLine()

        Console.Write("For i = 10 To 0 Step -2 -> ")
        For i As Integer = 10 To 0 Step -2
            Console.Write(i & " ")   ' 10 8 6 4 2 0
        Next
        Console.WriteLine()

        ' Exit For / Continue For
        Console.Write("Multiples de 3 jusqu'à 12 -> ")
        For i As Integer = 1 To 100
            If i Mod 3 <> 0 Then Continue For   ' ignore les non-multiples de 3
            If i > 12 Then Exit For             ' arrête au-delà de 12
            Console.Write(i & " ")              ' 3 6 9 12
        Next
        Console.WriteLine()
    End Sub

    ' ---- For Each...Next -------------------------------------------------------
    Private Sub DemoForEach()
        Console.WriteLine()
        Console.WriteLine("== For Each...Next ==")

        Dim noms = New List(Of String) From {"Ada", "Alan", "Grace"}
        For Each nom As String In noms
            Console.WriteLine(nom)
        Next

        ' Une chaîne se parcourt caractère par caractère
        Console.Write("Caractères de ""VB"" : ")
        For Each c As Char In "VB"
            Console.Write($"[{c}]")
        Next
        Console.WriteLine()
    End Sub

    ' ---- Supprimer pendant un parcours : les formes sûres ------------------------
    Private Sub DemoSuppressionSure()
        Console.WriteLine()
        Console.WriteLine("== Suppression sûre d'éléments (jamais pendant un For Each !) ==")

        ' Suppression sûre : parcours à rebours avec index
        Dim nombres = New List(Of Integer) From {3, -1, 4, -5, 9}
        For i As Integer = nombres.Count - 1 To 0 Step -1
            If nombres(i) < 0 Then nombres.RemoveAt(i)
        Next
        Console.WriteLine($"Parcours à rebours : {String.Join(", ", nombres)}")

        ' Ou, plus déclaratif :
        Dim valeurs = New List(Of Integer) From {3, -1, 4, -5, 9}
        valeurs.RemoveAll(Function(n) n < 0)
        Console.WriteLine($"RemoveAll          : {String.Join(", ", valeurs)}")
    End Sub

    ' ---- Do...Loop -----------------------------------------------------------------
    Private Sub DemoDoLoop()
        Console.WriteLine()
        Console.WriteLine("== Do...Loop ==")

        ' Condition en tête (While) : testée avant chaque itération
        Dim file As New Queue(Of String)(New String() {"commande A", "commande B", "commande C"})
        Do While file.Count > 0
            Traiter(file.Dequeue())
        Loop

        ' Condition en tête (Until) : équivalent, formulé à l'envers
        Dim file2 As New Queue(Of String)(New String() {"requête 1", "requête 2"})
        Do Until file2.Count = 0
            Traiter(file2.Dequeue())
        Loop

        ' Condition en queue : s'exécute AU MOINS une fois.
        ' (La lecture Console.ReadLine() du cours est simulée par LireLigneSimulee,
        '  qui renvoie Nothing à l'épuisement — déterministe et testable.)
        Dim ligne As String
        Do
            ligne = LireLigneSimulee()
            If ligne IsNot Nothing Then Traiter(ligne)
        Loop Until ligne Is Nothing
        Console.WriteLine("Fin de la saisie simulée (Nothing reçu).")
    End Sub

    Private Sub Traiter(element As String)
        Console.WriteLine($"Traitement de : {element}")
    End Sub

    Private ReadOnly _saisies As New Queue(Of String)(New String() {"première saisie", "deuxième saisie"})

    ''' <summary>Simule Console.ReadLine() : renvoie Nothing quand il n'y a plus rien à lire.</summary>
    Private Function LireLigneSimulee() As String
        Return If(_saisies.Count > 0, _saisies.Dequeue(), Nothing)
    End Function

    ' ---- While...End While ------------------------------------------------------------
    Private ReadOnly _montants As New Queue(Of Integer)(New Integer() {30, 45, 50})

    ''' <summary>Simule la lecture d'un montant (déterministe).</summary>
    Private Function LireMontant() As Integer
        Return _montants.Dequeue()
    End Function

    Private Sub DemoWhile()
        Console.WriteLine()
        Console.WriteLine("== While...End While (et non Wend) ==")

        Dim total = 0
        While total < 100
            total += LireMontant()
            Console.WriteLine($"total = {total}")
        End While
        Console.WriteLine($"Seuil de 100 atteint : total final = {total}")
    End Sub

End Module
