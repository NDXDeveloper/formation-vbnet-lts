' ============================================================================
'  Section 14.5 : Bonnes pratiques — démonstration
'  Description : Liaison anticipée (interface) vs tardive (Object) — même
'                résultat, mais l'anticipée est compilée. Comparaison de chaînes
'                Ordinal (0 allocation) vs ToLower (alloue). Pré-dimensionnement
'                des collections (moins de réallocations). Mesures à l'appui.
'  Fichier source : 05-bonnes-pratiques.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 14.5 Bonnes pratiques de performance ===")
        Console.WriteLine()

        ' 1) Liaison anticipée vs tardive — même résultat.
        Dim service As New ServiceImpl()
        Dim s As IService = service
        Dim resultatAnticipe = s.Traiter(21)                       ' compilé (early)
        Dim resultatTardif = LiaisonTardive.AppelTardif(service, 21) ' réflexion (late)
        Console.WriteLine($"[Liaison] anticipée={resultatAnticipe} ; tardive={resultatTardif} ; identiques={resultatAnticipe = resultatTardif}")
        Console.WriteLine("          (la tardive donne le même résultat, mais résolue à l'exécution → plus lente)")
        Console.WriteLine()

        ' 2) Comparaison de chaînes : Ordinal (0 alloc) vs ToLower (alloue).
        Dim a As String = "Identifiant-ABC"
        Dim b As String = "identifiant-abc"
        Console.WriteLine($"[Comparaison] correction : ToLower={a.ToLower() = b.ToLower()} ; OrdinalIgnoreCase={String.Equals(a, b, StringComparison.OrdinalIgnoreCase)}")
        Const N As Integer = 100000
        Dim octetsToLower = MesurerToLower(a, b, N)
        Dim octetsOrdinal = MesurerOrdinal(a, b, N)
        Console.WriteLine($"              {N} comparaisons : ToLower={octetsToLower:N0} octets ; Ordinal={octetsOrdinal:N0} octets")
        Console.WriteLine($"              Ordinal alloue beaucoup moins = {octetsOrdinal * 10 < octetsToLower}")
        Console.WriteLine()

        ' 3) Pré-dimensionner une collection évite les réallocations.
        Const M As Integer = 10000
        Dim octetsSans = MesurerListe(M, predimensionnee:=False)
        Dim octetsAvec = MesurerListe(M, predimensionnee:=True)
        Console.WriteLine($"[Pré-dimensionnement] {M} ajouts : sans capacité={octetsSans:N0} octets ; avec capacité={octetsAvec:N0} octets")
        Console.WriteLine($"                      pré-dimensionner alloue moins = {octetsAvec < octetsSans}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub

    Private Function MesurerToLower(a As String, b As String, n As Integer) As Long
        Dim w = (a.ToLower() = b.ToLower())
        Dim avant = GC.GetAllocatedBytesForCurrentThread()
        Dim c As Integer = 0
        For i = 1 To n
            If a.ToLower() = b.ToLower() Then c += 1
        Next
        Return GC.GetAllocatedBytesForCurrentThread() - avant
    End Function

    Private Function MesurerOrdinal(a As String, b As String, n As Integer) As Long
        Dim w = String.Equals(a, b, StringComparison.OrdinalIgnoreCase)
        Dim avant = GC.GetAllocatedBytesForCurrentThread()
        Dim c As Integer = 0
        For i = 1 To n
            If String.Equals(a, b, StringComparison.OrdinalIgnoreCase) Then c += 1
        Next
        Return GC.GetAllocatedBytesForCurrentThread() - avant
    End Function

    Private Function MesurerListe(n As Integer, predimensionnee As Boolean) As Long
        ' warm-up JIT
        Dim w = If(predimensionnee, New List(Of Integer)(n), New List(Of Integer)())
        w.Add(1)
        Dim avant = GC.GetAllocatedBytesForCurrentThread()
        Dim liste = If(predimensionnee, New List(Of Integer)(n), New List(Of Integer)())
        For i = 0 To n - 1
            liste.Add(i)
        Next
        Return GC.GetAllocatedBytesForCurrentThread() - avant
    End Function

End Module
