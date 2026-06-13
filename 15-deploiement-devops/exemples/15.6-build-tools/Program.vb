' ============================================================================
'  Section 15.6 : Outils de build .NET 10 — code utilisant System.Text.Json
'  Description : Utilise System.Text.Json (fourni par le runtime .NET 10). La
'                référence NuGet directe étant redondante, l'élagage la neutralise
'                (NU1510 au build) et c'est la copie du runtime qui sert.
'  Fichier source : 06-outils-build-net10.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Text.Json

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Dim json = JsonSerializer.Serialize(New With {.Nom = "VB.NET", .Version = 10})
        Console.WriteLine("=== 15.6 Outils de build : élagage NuGet ===")
        Console.WriteLine($"System.Text.Json (runtime) -> {json}")
    End Sub
End Module
