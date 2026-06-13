' ============================================================================
'  Section 11.5 : Coexistence — hôte moderne consommant les bibliothèques
'  Description : Consomme la logique métier partagée (.NET Standard 2.0) et la
'                bibliothèque multi-ciblée (chemin .NET moderne). Démontre la
'                « source unique de vérité » : la même CalculateurRemise sert
'                l'ancien et le nouveau monde.
'  Fichier source : 05-coexistence.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports Societe.Metier
Imports Societe.Multicible

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 11.5 Coexistence : .NET Standard 2.0 + multi-ciblage ===")
        Console.WriteLine()

        Dim calc As New CalculateurRemise()
        Console.WriteLine("[Bibliothèque .NET Standard 2.0 — logique métier partagée]")
        Console.WriteLine($"  remise(120 €) = {calc.Calculer(120D)} € (10 %)")
        Console.WriteLine($"  remise(60 €)  = {calc.Calculer(60D)} € (5 %)")
        Console.WriteLine($"  remise(30 €)  = {calc.Calculer(30D)} € (aucune)")
        Console.WriteLine()

        Dim info As New InfoPlateforme()
        Console.WriteLine("[Bibliothèque multi-ciblée — chemin sélectionné à la compilation]")
        Console.WriteLine($"  {info.Decrire()}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub
End Module
