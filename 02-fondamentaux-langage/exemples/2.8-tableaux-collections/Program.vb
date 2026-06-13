' ============================================================================
'  Section 2.8 : Tableaux et collections
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · tableaux : Dim a(4) crée 5 ÉLÉMENTS (4 = borne supérieure,
'                    pas la taille !), initialiseurs, ReDim Preserve ;
'                  · multidimensionnels : rectangulaires (,) vs dentelés ()() ;
'                  · List(Of T) : Add, Insert, RemoveAll (prédicat) ;
'                  · Dictionary(Of TKey, TValue) : indexeur ajoute-ou-met-à-jour,
'                    TryGetValue (lecture sûre), parcours en KeyValuePair ;
'                  · HashSet / Queue (FIFO) / Stack (LIFO) ;
'                  · ObservableCollection : notification CollectionChanged ;
'                  · ConcurrentDictionary : AddOrUpdate atomique ;
'                  · vue en lecture seule (AsReadOnly) et interfaces.
'  Fichier source : 08-tableaux-collections.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Linq

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoTableaux()
        DemoMultidimensionnels()
        DemoList()
        DemoDictionary()
        DemoAutresCollections()
        DemoObservableCollection()
        DemoConcurrent()
        DemoLectureSeule()
    End Sub

    ' ---- Tableaux : la borne supérieure, pas la taille ! ---------------------
    Private Sub DemoTableaux()
        Console.WriteLine("== Tableaux (le piège de la borne supérieure) ==")

        Dim nombres(4) As Integer          ' 5 éléments (indices 0..4) !
        nombres(0) = 10
        Console.WriteLine($"Dim nombres(4) -> nombres.Length = {nombres.Length} (et non 4)")

        Dim premiers() As Integer = {2, 3, 5, 7, 11}
        Console.WriteLine($"premiers = {String.Join(", ", premiers)} (taille déduite : {premiers.Length})")

        ReDim Preserve nombres(9)          ' passe à 10 éléments, contenu conservé
        Console.WriteLine($"après ReDim Preserve nombres(9) : Length = {nombres.Length}, nombres(0) = {nombres(0)} (conservé)")
    End Sub

    ' ---- Rectangulaires (,) vs dentelés ()() -----------------------------------
    Private Sub DemoMultidimensionnels()
        Console.WriteLine()
        Console.WriteLine("== Tableaux multidimensionnels ==")

        Dim grille(2, 3) As Integer        ' 3 x 4 = 12 éléments
        grille(1, 2) = 7
        Console.WriteLine($"grille(2, 3) : GetLength(0) = {grille.GetLength(0)}, GetLength(1) = {grille.GetLength(1)}, Rank = {grille.Rank}")

        Dim matrice = New Integer(,) {{1, 2, 3}, {4, 5, 6}}
        Console.WriteLine($"matrice(1, 2) = {matrice(1, 2)}")

        ' Dentelé : un tableau de tableaux, lignes de longueurs différentes
        Dim lignes()() As Integer = New Integer(1)() {}
        lignes(0) = New Integer() {1, 2, 3}
        lignes(1) = New Integer() {9, 8}
        Console.WriteLine($"dentelé : lignes(0)(1) = {lignes(0)(1)} ; longueurs {lignes(0).Length} et {lignes(1).Length}")
    End Sub

    ' ---- List(Of T) ----------------------------------------------------------------
    Private Sub DemoList()
        Console.WriteLine()
        Console.WriteLine("== List(Of T) ==")

        Dim noms As New List(Of String) From {"Ada", "Alan"}
        noms.Add("Grace")
        noms.Insert(0, "Katherine")
        Console.WriteLine($"après Add/Insert : {String.Join(", ", noms)}")

        noms.RemoveAll(Function(n) n.StartsWith("A"))   ' suppression par prédicat
        Console.WriteLine($"après RemoveAll(StartsWith(""A"")) : {String.Join(", ", noms)} (Count = {noms.Count})")
        Console.WriteLine($"noms(0) = {noms(0)}")
    End Sub

    ' ---- Dictionary(Of TKey, TValue) --------------------------------------------------
    Private Sub DemoDictionary()
        Console.WriteLine()
        Console.WriteLine("== Dictionary(Of TKey, TValue) ==")

        Dim ages As New Dictionary(Of String, Integer) From {{"Ada", 36}, {"Alan", 41}}
        ages("Grace") = 45            ' l'indexeur ajoute OU met à jour

        ' Lecture sûre : TryGetValue évite KeyNotFoundException
        Dim age As Integer
        If ages.TryGetValue("Ada", age) Then Console.WriteLine($"TryGetValue(""Ada"") -> {age}")
        Console.WriteLine($"TryGetValue(""Linus"") -> {ages.TryGetValue("Linus", age)} (clé absente, pas d'exception)")

        For Each paire In ages        ' KeyValuePair(Of String, Integer)
            Console.WriteLine($"  {paire.Key} : {paire.Value}")
        Next
    End Sub

    ' ---- HashSet, Queue, Stack ------------------------------------------------------------
    Private Sub DemoAutresCollections()
        Console.WriteLine()
        Console.WriteLine("== HashSet / Queue (FIFO) / Stack (LIFO) ==")

        Dim uniques As New HashSet(Of Integer) From {1, 2, 3}
        Console.WriteLine($"HashSet.Add(2) (déjà présent) -> {uniques.Add(2)} ; Count = {uniques.Count}")

        Dim file As New Queue(Of String)
        file.Enqueue("premier")
        file.Enqueue("second")
        Console.WriteLine($"Queue.Dequeue() -> {file.Dequeue()} (premier entré, premier sorti)")

        Dim pile As New Stack(Of String)
        pile.Push("premier")
        pile.Push("second")
        Console.WriteLine($"Stack.Pop()     -> {pile.Pop()} (dernier entré, premier sorti)")
    End Sub

    ' ---- ObservableCollection : la collection de la liaison de données ----------------------
    Private Sub DemoObservableCollection()
        Console.WriteLine()
        Console.WriteLine("== ObservableCollection (notifications) ==")

        Dim taches As New ObservableCollection(Of String)

        ' En WPF/WinForms, c'est l'UI liée qui s'abonne ; ici, la console « joue l'UI ».
        AddHandler taches.CollectionChanged,
            Sub(expediteur As Object, e As NotifyCollectionChangedEventArgs)
                Console.WriteLine($"  notification : {e.Action} -> {e.NewItems(0)}")
            End Sub

        taches.Add("Acheter du café")     ' l'abonné est prévenu automatiquement
        taches.Add("Relire le module 2")
    End Sub

    ' ---- Collections concurrentes ----------------------------------------------------------------
    Private Sub DemoConcurrent()
        Console.WriteLine()
        Console.WriteLine("== ConcurrentDictionary (multithread) ==")

        Dim compteurs As New ConcurrentDictionary(Of String, Integer)
        compteurs.AddOrUpdate("clics", 1, Function(cle, ancien) ancien + 1)   ' opération atomique
        compteurs.AddOrUpdate("clics", 1, Function(cle, ancien) ancien + 1)
        Console.WriteLine($"compteurs(""clics"") après 2 AddOrUpdate = {compteurs("clics")}")
    End Sub

    ' ---- Vue en lecture seule -----------------------------------------------------------------------
    Private Sub DemoLectureSeule()
        Console.WriteLine()
        Console.WriteLine("== Lecture seule et interfaces ==")

        Dim interne As New List(Of Integer) From {1, 2, 3}
        Dim vue As ReadOnlyCollection(Of Integer) = interne.AsReadOnly()
        Console.WriteLine($"AsReadOnly -> {vue.GetType().Name} (Count = {vue.Count})")

        ' Bonne pratique : exposer une interface plutôt qu'un type concret.
        Dim exposee As IReadOnlyList(Of Integer) = interne
        Console.WriteLine($"exposée en IReadOnlyList(Of Integer) : premier élément = {exposee(0)}")
    End Sub

End Module
