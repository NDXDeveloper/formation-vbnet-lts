' ============================================================================
'  Section 13.6 : BenchmarkDotNet — point d'entrée
'  Description : Lance le benchmark. Par défaut, exécution COMPLÈTE
'                (BenchmarkRunner.Run(Of T)()) — plusieurs minutes, statistiques
'                fiables. Avec l'argument « dry », exécution RAPIDE (Job.Dry :
'                un lancement/itération) pour valider la chaîne sans attendre —
'                les chiffres ne sont alors pas significatifs (Dry), mais le
'                tableau de synthèse est bien produit.
'  Fichier source : 06-benchmarkdotnet.md
' ============================================================================

Imports System.Linq
Imports BenchmarkDotNet.Configs
Imports BenchmarkDotNet.Jobs
Imports BenchmarkDotNet.Running

Module Program
    Sub Main(args As String())
        If args.Contains("dry") Then
            ' Vérification rapide : un seul lancement/itération (chiffres non significatifs).
            Dim config = DefaultConfig.Instance.AddJob(Job.Dry)
            BenchmarkRunner.Run(Of ConcatenationBenchmarks)(config)
        Else
            ' Mesure réelle (plusieurs minutes).
            BenchmarkRunner.Run(Of ConcatenationBenchmarks)()
        End If
    End Sub
End Module
