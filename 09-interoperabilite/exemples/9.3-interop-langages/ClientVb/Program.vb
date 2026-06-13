' ============================================================================
'  Section 9.3 : Interopérabilité entre langages .NET (VB <- C#)
'  Description : Démonstration vérifiable : VB.NET consomme chaque construction
'                exposée par la bibliothèque C#. Couvre record (égalité de
'                valeur), out -> ByRef, tuple nommé, init-only via With { },
'                méthode d'extension, async/Await, IAsyncEnumerable parcouru par
'                énumérateur MANUEL (pas d'Await For Each) + motif « capturer,
'                libérer, relancer », et l'échappement d'un mot-clé VB ([Stop]).
'  Fichier source : 03-interop-langages.md
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Runtime.ExceptionServices
Imports System.Threading.Tasks
Imports BibliothequeCs

Module Program

    Function Main() As Integer
        Return MainAsync().GetAwaiter().GetResult()
    End Function

    Async Function MainAsync() As Task(Of Integer)
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 9.3 Interop entre langages : VB.NET consomme C# ===")
        Console.WriteLine()

        ' 1) Record positionnel C# : constructeur + égalité de valeur + ToString.
        Dim p1 = New Personne("Alice", 30)
        Dim p2 = New Personne("Alice", 30)
        Console.WriteLine($"record Personne : Nom={p1.Nom}, Age={p1.Age} ; p1.Equals(p2) = {p1.Equals(p2)}")
        Console.WriteLine($"  ToString() généré : {p1}")

        Dim analyseur As New Analyseur()

        ' 2) Paramètre out C# -> ByRef VB (variable déclarée au préalable).
        Dim valeur As Integer
        If analyseur.TryParse("42", valeur) Then
            Console.WriteLine($"TryParse(out -> ByRef) : « 42 » -> {valeur}")
        End If

        ' 3) Tuple de valeur nommé.
        Dim contact = analyseur.PremierContact()
        Console.WriteLine($"Tuple nommé : Nom={contact.Nom}, Age={contact.Age}")

        ' 4) Propriétés init-only réglées par l'initialiseur With { } (VB 16.9).
        Dim prod = New Produit With {.Nom = "Clavier", .Prix = 49.9D}
        Console.WriteLine($"record init-only via With {{}} : {prod.Nom} = {prod.Prix:0.00} €")
        ' prod.Nom = "X"   '  <-- réassignation interdite : init-only verrouillé (BC37311)

        ' 5) Méthode d'extension C#.
        Dim rep = "ab".Repeter(3)
        Console.WriteLine($"Méthode d'extension : ""ab"".Repeter(3) = {rep}")

        ' 6) async / Task : Await transparent.
        Dim calc = Await analyseur.CalculerAsync()
        Console.WriteLine($"Await CalculerAsync() = {calc}")

        ' 7) IAsyncEnumerable : consommé SANS Await For Each (énumérateur manuel).
        Dim somme = Await SommerAsync(analyseur.CompterAsync(5))
        Console.WriteLine($"IAsyncEnumerable CompterAsync(5) -> somme 1..5 = {somme}")

        ' 8) Membre dont le nom est un mot-clé VB : échappement par crochets.
        Console.WriteLine($"Méthode nommée Stop : analyseur.[Stop]() = {analyseur.[Stop]()}")

        Console.WriteLine()
        Console.WriteLine("Terminé.")
        Return 0
    End Function

    ' Parcours MANUEL d'un IAsyncEnumerable (pas d'Await For Each en VB), avec le
    ' motif « capturer, libérer, relancer » (Await interdit dans un Finally).
    Private Async Function SommerAsync(source As IAsyncEnumerable(Of Integer)) As Task(Of Integer)
        Dim total = 0
        Dim e = source.GetAsyncEnumerator()
        Dim capture As ExceptionDispatchInfo = Nothing
        Try
            While Await e.MoveNextAsync()
                total += e.Current
            End While
        Catch ex As Exception
            capture = ExceptionDispatchInfo.Capture(ex)
        End Try
        Await e.DisposeAsync()
        capture?.Throw()
        Return total
    End Function

End Module
