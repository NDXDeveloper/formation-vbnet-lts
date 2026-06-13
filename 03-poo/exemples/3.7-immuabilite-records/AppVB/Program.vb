' ============================================================================
'  Section 3.7 : Types immuables et records
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · le type immuable VB « maison » (Argent) : mise à jour
'                    non destructive, égalité de valeur, original intact ;
'                  · l'immuabilité profonde (ImmutableArray) ;
'                  · la consommation des records C# 🔗 :
'                    ✅ constructeur positionnel, propriétés, ToString généré,
'                       égalité de valeur, init via With { } (VB 16.9),
'                       méthode de copie WithAge (« VB-friendly ») ;
'                    ❌ pas d'expression « with » ni de déconstruction en VB
'                       (on reconstruit / on lit les propriétés).
'  Fichier source : 07-immuabilite-records.md
'  Compilation    : dotnet build ImmuabiliteRecords.sln
'  Exécution      : dotnet run --project AppVB
' ============================================================================

Imports System
Imports ModeleCsharp

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoImmuableVB()
        DemoImmuabiliteProfonde()
        DemoRecordConsomme()
        DemoProprietesInit()
    End Sub

    ' ---- Type immuable VB « maison » -----------------------------------------
    Private Sub DemoImmuableVB()
        Console.WriteLine("== Type immuable VB (Argent) ==")

        Dim prix As New Argent(19.99D, "EUR")
        Dim solde = prix.AvecMontant(14.99D)   ' nouvel objet, l'original est intact
        Console.WriteLine($"prix  = {prix} ; solde = prix.AvecMontant(14.99) = {solde}")
        Console.WriteLine($"prix est resté {prix} (mise à jour non destructive)")

        Dim a As New Argent(10D, "EUR")
        Dim b As New Argent(10D, "EUR")
        Console.WriteLine($"a.Equals(b) = {a.Equals(b)} (égalité de VALEUR redéfinie)")
        Console.WriteLine($"a Is b      = {a Is b} (objets distincts)")
    End Sub

    ' ---- Immuabilité profonde ----------------------------------------------------
    Private Sub DemoImmuabiliteProfonde()
        Console.WriteLine()
        Console.WriteLine("== Immuabilité profonde (ImmutableArray) ==")

        Dim commande As New Commande({
            New LigneCommande("Café en grains", 2),
            New LigneCommande("Filtres", 1)
        })
        Console.WriteLine($"commande.Lignes ({commande.Lignes.Length} lignes, figées) : " &
                          String.Join(" ; ", commande.Lignes))
        ' commande.Lignes.Add(...) n'existe pas en mutation : ImmutableArray.Add
        ' renvoie une NOUVELLE collection — la commande reste intacte.
    End Sub

    ' ---- Consommer un record C# 🔗 --------------------------------------------------
    Private Sub DemoRecordConsomme()
        Console.WriteLine()
        Console.WriteLine("== Record C# consommé depuis VB ==")

        Dim p As New Personne("Alice", 30)      ' ✅ constructeur positionnel
        Console.WriteLine($"p.Nom = {p.Nom} ; p.Age = {p.Age}")
        Console.WriteLine($"p.ToString() = {p} (généré par le record)")

        Dim p2 As New Personne("Alice", 30)
        Console.WriteLine($"p.Equals(p2) = {p.Equals(p2)} (égalité de valeur générée)")
        Console.WriteLine($"p Is p2      = {p Is p2} (références distinctes)")

        ' ❌ Pas d'expression « with » en VB : on reconstruit à la main...
        Dim plusVieux = New Personne(p.Nom, p.Age + 1)
        Console.WriteLine($"Reconstruction manuelle : {plusVieux}")

        ' ...ou, mieux : la méthode de copie « VB-friendly » exposée par le C#.
        Console.WriteLine($"p.WithAge(31)           : {p.WithAge(31)}")

        ' ❌ Pas de déconstruction non plus : on lit simplement p.Nom, p.Age.
    End Sub

    ' ---- Propriétés init : réglables via With { } (VB 16.9) ---------------------------
    Private Sub DemoProprietesInit()
        Console.WriteLine()
        Console.WriteLine("== Propriétés init (record Adresse) ==")

        Dim a As New Adresse("12 rue des Lilas", "Paris") With {.CodePostal = "75001"}
        Console.WriteLine($"a = {a}")
        ' a.CodePostal = "13001"   ' ❌ BC37311 (vérifiée au chapitre 1) :
        '                          ' propriété init, plus modifiable après l'initialisation.
    End Sub

End Module
