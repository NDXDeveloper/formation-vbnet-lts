' ============================================================================
'  Section 2.6 : Chaînes et manipulation de texte
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · String immuable : chaque « modification » crée une
'                    nouvelle chaîne (réaffectation nécessaire) ;
'                  · littéraux : PAS d'échappement antislash en VB — guillemet
'                    doublé "", Environment.NewLine / vbTab, littéral
'                    multi-ligne (VB 14+) qui embarque l'indentation ;
'                  · String.IsNullOrEmpty / IsNullOrWhiteSpace ;
'                  · opérations et méthodes courantes (base 0) vs fonctions
'                    héritées VB6 (base 1 — InStr renvoie 0 si absent) ;
'                  · StringBuilder pour les assemblages en boucle ;
'                  · interpolation $"..." : format, alignement, {{ }},
'                    culture courante par défaut et FormattableString.Invariant ;
'                  · comparaison explicite avec StringComparison.
'  Fichier source : 06-chaines.md
'  Compilation    : dotnet build      Exécution : dotnet run
'  Note           : {prix:C2} s'affiche selon la culture machine (fr-FR ici).
' ============================================================================

Imports System
Imports System.Globalization

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoImmuabilite()
        DemoLitteraux()
        DemoVideOuNulle()
        DemoOperationsDeBase()
        DemoMethodesCourantes()
        DemoFonctionsHeritees()
        DemoStringBuilder()
        DemoInterpolation()
        DemoComparaison()
    End Sub

    ' ---- String est immuable ------------------------------------------------
    Private Sub DemoImmuabilite()
        Console.WriteLine("== String est immuable ==")
        Dim mot = "bonjour"
        mot.ToUpper()              ' renvoie "BONJOUR" mais ne modifie PAS 'mot'
        Console.WriteLine($"après mot.ToUpper() sans réaffectation : {mot}")
        mot = mot.ToUpper()        ' il faut réaffecter
        Console.WriteLine($"après mot = mot.ToUpper()             : {mot}")
    End Sub

    ' ---- Littéraux : pas d'antislash ------------------------------------------
    Private Sub DemoLitteraux()
        Console.WriteLine()
        Console.WriteLine("== Littéraux (pas d'échappement antislash) ==")

        Dim citation = "Il a dit ""bonjour""."   ' guillemet doublé
        Console.WriteLine(citation)

        Dim deuxLignes = "Première ligne" & Environment.NewLine & "Deuxième ligne"
        Console.WriteLine(deuxLignes)

        Dim tabule = "Nom" & vbTab & "Âge"
        Console.WriteLine(tabule)

        ' Littéral multi-ligne (VB 14+) : les retours à la ligne du source
        ' sont conservés — et l'indentation du code entrerait dans la chaîne.
        Dim bloc = "Ligne A
Ligne B"
        Console.WriteLine(bloc)
    End Sub

    ' ---- Chaîne vide ou nulle ----------------------------------------------------
    Private Sub DemoVideOuNulle()
        Console.WriteLine()
        Console.WriteLine("== IsNullOrEmpty / IsNullOrWhiteSpace ==")
        Dim vide = ""
        Dim espaces = "   "
        Dim nulle As String = Nothing
        Console.WriteLine($"IsNullOrEmpty("""")          -> {String.IsNullOrEmpty(vide)}")
        Console.WriteLine($"IsNullOrEmpty(""   "")       -> {String.IsNullOrEmpty(espaces)}")
        Console.WriteLine($"IsNullOrWhiteSpace(""   "")  -> {String.IsNullOrWhiteSpace(espaces)}")
        Console.WriteLine($"IsNullOrWhiteSpace(Nothing) -> {String.IsNullOrWhiteSpace(nulle)}")
    End Sub

    ' ---- Opérations de base ---------------------------------------------------------
    Private Sub DemoOperationsDeBase()
        Console.WriteLine()
        Console.WriteLine("== Opérations de base ==")
        Dim s = "Bonjour"
        Console.WriteLine($"s.Length = {s.Length}")
        Console.WriteLine($"s(0) = {s(0)} (parenthèses, pas de crochets — renvoie un Char)")

        Dim noms = {"Ada", "Alan", "Grace"}
        Dim ligne = String.Join(", ", noms)
        Console.WriteLine($"String.Join : {ligne}")
    End Sub

    ' ---- Méthodes courantes (base 0) ---------------------------------------------------
    Private Sub DemoMethodesCourantes()
        Console.WriteLine()
        Console.WriteLine("== Méthodes courantes (.NET, base 0) ==")

        Dim chemin = "  rapport-2026.pdf  "
        Dim propre = chemin.Trim()
        Dim extension = propre.Substring(propre.LastIndexOf("."c) + 1)
        Dim morceaux = "a;b;c".Split(";"c)

        Console.WriteLine($"Trim            : ""{propre}""")
        Console.WriteLine($"extension       : {extension}")
        Console.WriteLine($"Split(""a;b;c"") : {String.Join(" | ", morceaux)}")
        Console.WriteLine($"Replace         : {propre.Replace("2026", "2027")}")
        Console.WriteLine($"PadLeft(20, .)  : {propre.PadLeft(20, "."c)}")
    End Sub

    ' ---- Fonctions héritées VB6 : base 1 ------------------------------------------------
    Private Sub DemoFonctionsHeritees()
        Console.WriteLine()
        Console.WriteLine("== Fonctions héritées (base 1) vs méthodes .NET (base 0) ==")
        Dim s = "Bonjour"
        ' InStr (héritée) renvoie une position EN BASE 1, et 0 si absent ;
        ' IndexOf (.NET) renvoie une position EN BASE 0, et -1 si absent.
        Console.WriteLine($"InStr(""{s}"", ""jour"")   = {InStr(s, "jour")} (base 1)")
        Console.WriteLine($"s.IndexOf(""jour"")        = {s.IndexOf("jour")} (base 0)")
        Console.WriteLine($"InStr absent -> {InStr(s, "xyz")} ; IndexOf absent -> {s.IndexOf("xyz")}")
        ' Préférez les méthodes .NET pour la cohérence avec tout l'écosystème.
    End Sub

    ' ---- StringBuilder --------------------------------------------------------------------
    Private Sub DemoStringBuilder()
        Console.WriteLine()
        Console.WriteLine("== StringBuilder (assemblage en boucle) ==")

        Dim sb As New System.Text.StringBuilder()
        For i As Integer = 1 To 1000
            sb.Append("Ligne ").Append(i).AppendLine()
        Next
        Dim resultat = sb.ToString()

        Dim lignes = resultat.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
        Console.WriteLine($"Nombre de lignes assemblées : {lignes.Length}")
        Console.WriteLine($"Première : ""{lignes(0)}"" — dernière : ""{lignes(lignes.Length - 1)}""")
    End Sub

    ' ---- Interpolation $"..." ----------------------------------------------------------------
    Private Sub DemoInterpolation()
        Console.WriteLine()
        Console.WriteLine("== Interpolation $""..."" ==")

        Dim nom = "Ada"
        Dim age = 36
        Console.WriteLine($"Bonjour {nom}, vous avez {age} ans.")

        Dim prix = 19.9D
        Console.WriteLine($"Prix : {prix:C2} — réf. {{A-100}} (culture courante de la machine)")
        Console.WriteLine($"Alignement : [{nom,-10}][{age,5}]")

        ' Format INVARIANT explicite, indépendant de la machine :
        Console.WriteLine(FormattableString.Invariant($"Invariant : prix = {prix:F2}"))
    End Sub

    ' ---- Comparaison explicite ------------------------------------------------------------------
    Private Sub DemoComparaison()
        Console.WriteLine()
        Console.WriteLine("== Comparaison explicite (StringComparison) ==")
        Dim a = "ADA", b = "ada"
        Console.WriteLine($"String.Equals(a, b, OrdinalIgnoreCase) -> {String.Equals(a, b, StringComparison.OrdinalIgnoreCase)}")
        Console.WriteLine($"String.Equals(a, b, Ordinal)           -> {String.Equals(a, b, StringComparison.Ordinal)}")
    End Sub

End Module
