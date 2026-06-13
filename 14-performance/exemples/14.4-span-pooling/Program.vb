' ============================================================================
'  Section 14.4 : Span / Memory (consommation), pooling — démonstration
'  Description : Span consommé uniquement EN EXPRESSION ; mesure de l'économie
'                d'allocation (Substring vs AsSpan) ; Memory(Of T) asynchrone ;
'                ArrayPool et ObjectPool. La déclaration d'une variable Span est
'                volontairement laissée en commentaire (elle ne compilerait pas :
'                BC30668).
'  Fichier source : 04-span-pooling.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Buffers
Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.Extensions.ObjectPool

Module Program

    Function Main() As Integer
        Return MainAsync().GetAwaiter().GetResult()
    End Function

    Async Function MainAsync() As Task(Of Integer)
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 14.4 Span / Memory (consommation), pooling ===")
        Console.WriteLine()

        Dim ligne As String = "ABCDEFGH1234"   ' 8 caractères puis « 1234 »

        ' 1) Span consommé EN EXPRESSION (aucune sous-chaîne allouée).
        Dim valeur As Integer = Integer.Parse(ligne.AsSpan(8, 4))
        Dim position As Integer = "abc,def".AsSpan().IndexOf(","c)
        Console.WriteLine($"[Span en expression] Integer.Parse(AsSpan(8,4))={valeur} ; AsSpan().IndexOf("","")={position}")

        ' ❌ Déclarer une variable Span ne compile PAS en VB (BC30668) :
        ' Dim vue As ReadOnlySpan(Of Char) = ligne.AsSpan(8, 4)

        ' Lecture par tampon : Read(Span(Of Byte)) — l'argument est éphémère.
        Using flux As New MemoryStream(New Byte() {10, 20, 30, 40})
            Dim tampon(15) As Byte
            Dim lus As Integer = flux.Read(tampon.AsSpan())
            Console.WriteLine($"[Read(Span)] octets lus={lus} ; tampon(0)={tampon(0)}")
        End Using
        Console.WriteLine()

        ' 2) Mesure : Substring (alloue) vs AsSpan (n'alloue pas).
        Const N As Integer = 100000
        Dim octetsSubstring = MesurerSubstring(ligne, N)
        Dim octetsAsSpan = MesurerAsSpan(ligne, N)
        Console.WriteLine($"[Allocations, {N} parses]")
        Console.WriteLine($"  via Substring : {octetsSubstring:N0} octets")
        Console.WriteLine($"  via AsSpan    : {octetsAsSpan:N0} octets")
        Console.WriteLine($"  AsSpan alloue beaucoup moins = {octetsAsSpan * 10 < octetsSubstring}")
        Console.WriteLine()

        ' 3) Memory(Of T) : déclarable, traverse l'Await ; accès via .Span(i).
        Dim donnees(15) As Byte
        Dim memoire As Memory(Of Byte) = donnees.AsMemory()
        Using flux As New MemoryStream(New Byte() {7, 8, 9})
            Dim lus As Integer = Await flux.ReadAsync(memoire)
            Console.WriteLine($"[Memory(Of Byte)] ReadAsync → lus={lus} ; premier octet (memoire.Span(0))={memoire.Span(0)}")
        End Using
        Console.WriteLine()

        ' 4) ArrayPool : emprunter / rendre (Try…Finally).
        Dim pool = ArrayPool(Of Byte).Shared
        Dim emprunte = pool.Rent(256)
        Try
            emprunte(0) = 42
            Console.WriteLine($"[ArrayPool] Rent(256) → Length réel={emprunte.Length} (>=256) ; emprunte(0)={emprunte(0)}")
        Finally
            pool.Return(emprunte, clearArray:=True)
        End Try
        Console.WriteLine()

        ' 5) ObjectPool(Of StringBuilder) : réutiliser un objet coûteux.
        Dim fournisseur As ObjectPoolProvider = New DefaultObjectPoolProvider()
        Dim poolSb As ObjectPool(Of StringBuilder) = fournisseur.CreateStringBuilderPool()
        Dim sb = poolSb.Get()
        Dim message As String
        Try
            sb.Append("Bonjour ").Append("VB.NET")
            message = sb.ToString()
        Finally
            poolSb.Return(sb)   ' réinitialisé puis remis à disposition
        End Try
        Console.WriteLine($"[ObjectPool] message={message}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
        Return 0
    End Function

    Private Function MesurerSubstring(s As String, n As Integer) As Long
        Dim w = Integer.Parse(s.Substring(8, 4))   ' warm-up JIT
        Dim avant = GC.GetAllocatedBytesForCurrentThread()
        Dim somme As Long = 0
        For i = 1 To n
            somme += Integer.Parse(s.Substring(8, 4))   ' alloue une sous-chaîne
        Next
        Return GC.GetAllocatedBytesForCurrentThread() - avant
    End Function

    Private Function MesurerAsSpan(s As String, n As Integer) As Long
        Dim w = Integer.Parse(s.AsSpan(8, 4))      ' warm-up JIT
        Dim avant = GC.GetAllocatedBytesForCurrentThread()
        Dim somme As Long = 0
        For i = 1 To n
            somme += Integer.Parse(s.AsSpan(8, 4))      ' aucune allocation
        Next
        Return GC.GetAllocatedBytesForCurrentThread() - avant
    End Function

End Module
