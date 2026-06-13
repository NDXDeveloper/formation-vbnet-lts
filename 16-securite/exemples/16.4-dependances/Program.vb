' ============================================================================
'  Section 16.4 : Dépendances et vulnérabilités — code utilisant le paquet audité
'  Description : Utilise Newtonsoft.Json (le paquet vulnérable référencé) pour que
'                la dépendance soit RÉELLE. L'intérêt de l'exemple n'est pas la
'                sortie du programme mais l'AVERTISSEMENT NU190x émis au build par
'                l'audit NuGet. En conditions réelles, on corrige en montant à
'                13.0.1+ (ou, mieux, en utilisant System.Text.Json du runtime).
'  Fichier source : 04-dependances-vulnerabilites.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports Newtonsoft.Json

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Dim json As String = JsonConvert.SerializeObject(New With {.Outil = "audit NuGet", .Section = "16.4"})
        Console.WriteLine("=== 16.4 Dépendances : audit NuGet ===")
        Console.WriteLine($"Newtonsoft.Json -> {json}")
        Console.WriteLine("(Le point clé est l'avertissement NU190x au build, pas cette sortie.)")
    End Sub
End Module
