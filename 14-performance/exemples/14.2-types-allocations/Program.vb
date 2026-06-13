' ============================================================================
'  Section 14.2 : Types valeur vs référence, allocations — démonstration
'  Description : Sémantique de copie, boxing/unboxing, et MESURES d'allocation
'                (octets alloués sur le thread) prouvant que le boxing
'                (collections non génériques, énumérateur via IEnumerable) coûte,
'                là où les génériques n'allouent pas de boîtes.
'  Fichier source : 02-types-allocations.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Collections
Imports System.Collections.Generic

Module Program

    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 14.2 Types valeur/référence, allocations ===")
        Console.WriteLine()

        ' 1) Sémantique de copie.
        Dim a As New Point With {.X = 1, .Y = 2}
        Dim b = a : b.X = 99
        Console.WriteLine($"[Copie] Structure : a.X={a.X} (inchangé), b.X={b.X}  → indépendants")

        Dim x As New Boite With {.Valeur = 1}
        Dim y = x : y.Valeur = 99
        Console.WriteLine($"[Copie] Class     : x.Valeur={x.Valeur}, y.Valeur={y.Valeur}  → même objet partagé")
        Console.WriteLine()

        ' 2) Boxing / unboxing.
        Dim n As Integer = 42
        Dim o As Object = n             ' BOXING (alloue une boîte)
        Dim m As Integer = CInt(o)      ' UNBOXING
        Console.WriteLine($"[Boxing] o.GetType()={o.GetType().Name}, valeur déballée={m}")
        Console.WriteLine()

        ' 3) Mesure : ArrayList (boxing) vs List(Of Integer) (sans boxing).
        Const Taille As Integer = 10000
        Dim octetsArrayList = Mesurer(Sub() RemplirArrayList(Taille))
        Dim octetsList = Mesurer(Sub() RemplirList(Taille))
        Console.WriteLine($"[Allocations, N={Taille}]")
        Console.WriteLine($"  ArrayList (boxing) : {octetsArrayList:N0} octets")
        Console.WriteLine($"  List(Of Integer)   : {octetsList:N0} octets")
        Console.WriteLine($"  ArrayList alloue beaucoup plus = {octetsArrayList > 4 * octetsList}")
        Console.WriteLine()

        ' 4) ValueTuple est un type valeur ; l'ancien Tuple est une classe.
        Dim vt = (1, 2)
        Console.WriteLine($"[Tuples] ValueTuple IsValueType={vt.GetType().IsValueType} ; " &
                          $"Tuple IsValueType={Tuple.Create(1, 2).GetType().IsValueType}")
        Console.WriteLine()

        ' 5) Un tableau est UN seul objet de type référence (même de types valeur).
        Dim tab(99) As Integer
        Console.WriteLine($"[Tableau] Integer(99) : IsArray={tab.GetType().IsArray}, " &
                          $"IsValueType={tab.GetType().IsValueType} (objet unique sur le tas, 100 entiers inline)")
        Console.WriteLine()

        ' 6) Énumérateur : For Each sur List = struct (0 alloc) ; via IEnumerable = boxé.
        Dim liste As New List(Of Integer)(Enumerable_Range(0, 100))
        Dim octetsForEachList = Mesurer(Sub() ParcourirListe(liste, 1000))
        Dim octetsForEachIEnum = Mesurer(Sub() ParcourirIEnumerable(liste, 1000))
        Console.WriteLine("[Énumérateur, 1000 parcours d'une liste de 100]")
        Console.WriteLine($"  For Each sur List(Of T)     : {octetsForEachList:N0} octets")
        Console.WriteLine($"  For Each via IEnumerable(Of T): {octetsForEachIEnum:N0} octets (énumérateur boxé)")
        Console.WriteLine($"  IEnumerable alloue plus = {octetsForEachIEnum > octetsForEachList}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub

    ' Mesure les octets alloués par 'action' sur le thread courant (avec préchauffage JIT).
    Private Function Mesurer(action As Action) As Long
        action()   ' warm-up : JIT de la méthode
        Dim avant = GC.GetAllocatedBytesForCurrentThread()
        action()
        Return GC.GetAllocatedBytesForCurrentThread() - avant
    End Function

    Private Sub RemplirArrayList(taille As Integer)
        Dim liste As New ArrayList(taille)
        For i = 0 To taille - 1
            liste.Add(i)        ' boxe chaque Integer
        Next
    End Sub

    Private Sub RemplirList(taille As Integer)
        Dim liste As New List(Of Integer)(taille)
        For i = 0 To taille - 1
            liste.Add(i)        ' aucun boxing
        Next
    End Sub

    Private Sub ParcourirListe(liste As List(Of Integer), repetitions As Integer)
        Dim somme As Long = 0
        For r = 1 To repetitions
            For Each v In liste                 ' énumérateur de type valeur (struct)
                somme += v
            Next
        Next
    End Sub

    Private Sub ParcourirIEnumerable(liste As List(Of Integer), repetitions As Integer)
        Dim somme As Long = 0
        For r = 1 To repetitions
            For Each v In CType(liste, IEnumerable(Of Integer))   ' énumérateur boxé
                somme += v
            Next
        Next
    End Sub

    ' Petit utilitaire pour amorcer la liste (évite une dépendance LINQ dans la mesure).
    Private Function Enumerable_Range(debut As Integer, nombre As Integer) As IEnumerable(Of Integer)
        Dim r As New List(Of Integer)(nombre)
        For i = 0 To nombre - 1
            r.Add(debut + i)
        Next
        Return r
    End Function

End Module
