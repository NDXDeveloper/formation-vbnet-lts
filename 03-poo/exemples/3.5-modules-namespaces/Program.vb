' ============================================================================
'  Section 3.5 : Modules, espaces de noms et classes partielles
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · module : accès NON QUALIFIÉ (et qualifié, facultatif) ;
'                  · méthode d'extension (module + <Extension()>) ;
'                  · espaces de noms imbriqués / forme pointée ;
'                  · ⚠️ Root Namespace : le projet a RootNamespace
'                    « Contoso.Ventes » -> FullName VÉRIFIE que tout
'                    Namespace du code est imbriqué dessous ;
'                  · Imports avec ALIAS (IO = System.IO) et mot-clé Global ;
'                  · classes partielles et méthode partielle (implémentée ->
'                    exécutée ; non implémentée -> appel supprimé).
'  Fichier source : 05-modules-namespaces.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports IO = System.IO            ' alias : « IO.Path.Combine(...) »

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoModules()
        DemoRootNamespace()
        DemoImportsEtGlobal()
        DemoPartial()
    End Sub

    ' ---- Modules : accès non qualifié et extensions ---------------------------
    Private Sub DemoModules()
        Console.WriteLine("== Module : accès non qualifié ==")
        Dim ttc = AppliquerTVA(100D)                ' accès direct
        Dim ttc2 = Utilitaires.AppliquerTVA(100D)   ' qualification facultative
        Console.WriteLine($"AppliquerTVA(100) = {ttc} ; Utilitaires.AppliquerTVA(100) = {ttc2}")

        Console.WriteLine()
        Console.WriteLine("== Méthode d'extension (module + <Extension()>) ==")
        Console.WriteLine($"""   "".EstVideOuBlanc() = {"   ".EstVideOuBlanc()}")
        Console.WriteLine($"""abc"".EstVideOuBlanc() = {"abc".EstVideOuBlanc()}")
    End Sub

    ' ---- Le piège du Root Namespace, vérifié ------------------------------------
    Private Sub DemoRootNamespace()
        Console.WriteLine()
        Console.WriteLine("== Root Namespace (RootNamespace = Contoso.Ventes) ==")

        ' Le code déclare « Namespace Donnees » ; l'espace RÉEL est imbriqué :
        Console.WriteLine($"FullName : {GetType(Donnees.DepotFactures).FullName}")
        Console.WriteLine($"FullName : {GetType(Facturation.Facture).FullName}")
        Console.WriteLine($"Taxes.MontantTVA(100) = {Facturation.Calculs.Taxes.MontantTVA(100D)}")
        Console.WriteLine($"Arrondis (forme pointée) = {Facturation.Calculs.Arrondis.ArrondirCentimes(12.346D)}")
    End Sub

    ' ---- Imports alias et Global ----------------------------------------------------
    Private Sub DemoImportsEtGlobal()
        Console.WriteLine()
        Console.WriteLine("== Imports alias et Global ==")

        Dim chemin = IO.Path.Combine("dossier", "fichier.txt")          ' via l'alias
        Dim chemin2 = Global.System.IO.Path.Combine("dossier", "fichier.txt")  ' racine absolue
        Console.WriteLine($"Alias IO  : {chemin}")
        Console.WriteLine($"Global    : {chemin2}")
    End Sub

    ' ---- Classes partielles et méthodes partielles -------------------------------------
    Private Sub DemoPartial()
        Console.WriteLine()
        Console.WriteLine("== Classes partielles et méthode partielle ==")

        Dim e As New Entite()
        e.MettreAJour()              ' OnModifie est implémenté -> le hook s'exécute

        Dim sansHook As New EntiteSansHook()
        sansHook.MettreAJour()       ' OnModifie non implémenté -> appel SUPPRIMÉ (aucun hook)
    End Sub

End Module
