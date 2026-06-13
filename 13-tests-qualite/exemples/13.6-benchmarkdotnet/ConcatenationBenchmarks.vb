' ============================================================================
'  Section 13.6 : BenchmarkDotNet — la classe de benchmark
'  Description : Compare la concaténation par &= (qui réalloue à chaque
'                itération) et StringBuilder. <MemoryDiagnoser> ajoute les
'                colonnes d'allocation, <Params> exécute pour plusieurs tailles,
'                <Baseline> désigne la référence du ratio. On RENVOIE le résultat
'                pour empêcher l'élimination de code mort.
'  Fichier source : 06-benchmarkdotnet.md
' ============================================================================

Imports System.Text
Imports BenchmarkDotNet.Attributes

<MemoryDiagnoser>
Public Class ConcatenationBenchmarks

    <Params(100, 10000)>
    Public Property NombreItems As Integer

    <Benchmark(Baseline:=True)>
    Public Function AvecConcatenation() As String
        Dim resultat = ""
        For i = 1 To NombreItems
            resultat &= i.ToString()
        Next
        Return resultat
    End Function

    <Benchmark>
    Public Function AvecStringBuilder() As String
        Dim sb As New StringBuilder()
        For i = 1 To NombreItems
            sb.Append(i)
        Next
        Return sb.ToString()
    End Function

End Class
