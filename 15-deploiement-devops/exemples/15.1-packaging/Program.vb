' ============================================================================
'  Section 15.1 : Packaging desktop — application de démonstration
'  Description : Affiche des informations sur le LIVRABLE en cours d'exécution
'                (framework, RID, chemin de l'exécutable). On publie cet exe en
'                framework-dependent, self-contained et fichier unique, puis on
'                l'exécute pour vérifier que chaque mode produit un livrable
'                fonctionnel (cf. README pour les commandes et les artefacts).
'  Fichier source : 01-packaging-desktop.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Runtime.InteropServices

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("MARQUEUR: Bonjour depuis le livrable VB.NET")
        Console.WriteLine($"Framework : {RuntimeInformation.FrameworkDescription}")
        Console.WriteLine($"RID       : {RuntimeInformation.RuntimeIdentifier}")
        Console.WriteLine($"Exe       : {Environment.ProcessPath}")
    End Sub
End Module
