' ============================================================================
'  Section 2.7 : Dates, nombres, formatage et culture
'  Description : Exemple complet reprenant tous les extraits de la section,
'                avec des CULTURES EXPLICITES pour des sorties déterministes :
'                  · le type Date (= DateTime), littéraux #...# et leur piège
'                    (format invariant M/d/yyyy -> préférer #yyyy-MM-dd#) ;
'                  · immuabilité, AddDays, soustraction -> TimeSpan ;
'                  · DateOnly / TimeOnly / DateTimeOffset (.NET moderne) ;
'                  · TryParse avec culture, ParseExact sur format imposé ;
'                  · formats standard et personnalisés (MM = mois, mm = minutes) ;
'                  · Double binaire imprécis (0.1 + 0.2) vs Decimal exact ;
'                  · le piège du séparateur décimal et NumberStyles ;
'                  · formats numériques (N2, C2, P1, D5, X) selon la culture ;
'                  · le piège du « i » turc -> comparaisons ordinales pour
'                    les identifiants.
'  Fichier source : 07-dates-nombres-culture.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Globalization

Module Program

    ' Les trois cultures utilisées partout dans la démonstration.
    Private ReadOnly Fr As New CultureInfo("fr-FR")
    Private ReadOnly En As New CultureInfo("en-US")
    Private ReadOnly Inv As CultureInfo = CultureInfo.InvariantCulture

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoDates()
        DemoTypesComplementaires()
        DemoAnalyseDates()
        DemoFormatageDates()
        DemoDoubleContreDecimal()
        DemoAnalyseNombres()
        DemoFormatageNombres()
        DemoPiegeTurc()
    End Sub

    ' ---- Le type Date ---------------------------------------------------------
    Private Sub DemoDates()
        Console.WriteLine("== Le type Date (= DateTime) ==")

        ' Now / Today / UtcNow varient à chaque exécution : on vérifie leur Kind.
        Console.WriteLine($"Date.Now.Kind = {Date.Now.Kind} ; Date.UtcNow.Kind = {Date.UtcNow.Kind}")

        Dim precise = New Date(2026, 6, 10)            ' via le constructeur
        Dim litteral = #2026-06-10#                    ' littéral ISO (recommandé)
        Console.WriteLine($"New Date(2026, 6, 10) = {precise:yyyy-MM-dd}")

        ' ⚠️ Le littéral est interprété de façon INVARIANTE (M/d/yyyy) :
        '    #6/10/2026# est le 10 JUIN, pas le 6 octobre.
        Console.WriteLine($"#6/10/2026# = #2026-06-10# -> {#6/10/2026# = litteral}")

        ' Immuabilité : AddDays renvoie une NOUVELLE date ; date - date -> TimeSpan
        Dim aujourdhui = precise
        Dim echeance = aujourdhui.AddDays(30)
        Dim duree As TimeSpan = echeance - aujourdhui
        Console.WriteLine($"echeance = {echeance:yyyy-MM-dd} ; duree.TotalDays = {duree.TotalDays}")
        Console.WriteLine($"Composants : Year = {precise.Year}, Month = {precise.Month}, Day = {precise.Day}")
    End Sub

    ' ---- DateOnly, TimeOnly, DateTimeOffset --------------------------------------
    Private Sub DemoTypesComplementaires()
        Console.WriteLine()
        Console.WriteLine("== DateOnly / TimeOnly / DateTimeOffset ==")

        Dim naissance As New DateOnly(1990, 5, 12)           ' une date, sans heure
        Dim ouverture As New TimeOnly(9, 30)                 ' une heure, sans date
        Dim instant As DateTimeOffset = DateTimeOffset.Now   ' instant + décalage UTC

        Console.WriteLine($"DateOnly : {naissance.ToString("yyyy-MM-dd", Inv)}")
        Console.WriteLine($"TimeOnly : {ouverture.ToString("HH\:mm", Inv)}")
        Console.WriteLine($"DateTimeOffset.Now est de type : {instant.GetType().Name} (décalage : {instant.Offset})")
    End Sub

    ' ---- Analyser une date ----------------------------------------------------------
    Private Sub DemoAnalyseDates()
        Console.WriteLine()
        Console.WriteLine("== Analyser une date (TryParse + culture) ==")

        Dim texte = "10/06/2026"
        Dim resultat As Date
        If Date.TryParse(texte, Fr, DateTimeStyles.None, resultat) Then
            Console.WriteLine($"""{texte}"" lu en fr-FR (jour/mois)  -> {resultat:yyyy-MM-dd}")
        End If
        If Date.TryParse(texte, En, DateTimeStyles.None, resultat) Then
            Console.WriteLine($"""{texte}"" lu en en-US (mois/jour)  -> {resultat:yyyy-MM-dd} (autre date !)")
        End If

        ' Format strictement imposé :
        Dim iso = Date.ParseExact("2026-06-10", "yyyy-MM-dd", Inv)
        Console.WriteLine($"ParseExact ISO -> {iso:yyyy-MM-dd}")
    End Sub

    ' ---- Formater une date -------------------------------------------------------------
    Private Sub DemoFormatageDates()
        Console.WriteLine()
        Console.WriteLine("== Formater une date (culture explicite) ==")

        Dim d = New Date(2026, 6, 10, 14, 30, 0)
        Console.WriteLine($"""d"" fr-FR : {d.ToString("d", Fr)}")
        Console.WriteLine($"""D"" fr-FR : {d.ToString("D", Fr)}")
        Console.WriteLine($"""d"" en-US : {d.ToString("d", En)} (l'ordre jour/mois s'inverse)")
        Console.WriteLine($"""t"" fr-FR : {d.ToString("t", Fr)}")
        Console.WriteLine($"""o""       : {d.ToString("o", Inv)} (aller-retour ISO 8601)")
        Console.WriteLine($"""s""       : {d.ToString("s", Inv)}")
        Console.WriteLine($"Personnalisé yyyy-MM-dd HH:mm : {d.ToString("yyyy-MM-dd HH:mm", Inv)}")

        ' ⚠️ MM = mois, mm = minutes (HH = 24 h, hh = 12 h)
        Console.WriteLine($"⚠️ MM vs mm : MM -> {d.ToString("MM", Inv)} (mois) ; mm -> {d.ToString("mm", Inv)} (minutes)")

        ' La règle d'or de la section :
        Dim horodatage = Date.UtcNow.ToString("o", Inv)            ' stockage/échange
        Dim affichage = Date.Now.ToString("D", CultureInfo.CurrentCulture)  ' affichage
        Console.WriteLine($"(stockage ISO : {horodatage.Substring(0, 4)}... ; affichage : culture courante)")
    End Sub

    ' ---- Double (binaire) vs Decimal (exact) ----------------------------------------------
    Private Sub DemoDoubleContreDecimal()
        Console.WriteLine()
        Console.WriteLine("== Double (flottant binaire) vs Decimal (exact) ==")

        Dim enDouble As Double = 0.1 + 0.2
        Dim enDecimal As Decimal = 0.1D + 0.2D
        Console.WriteLine($"0.1 + 0.2 en Double  : {enDouble.ToString(Inv)}")
        Console.WriteLine($"0.1D + 0.2D en Decimal : {enDecimal.ToString(Inv)} -> Decimal pour l'argent !")
    End Sub

    ' ---- Le piège du séparateur décimal ------------------------------------------------------
    Private Sub DemoAnalyseNombres()
        Console.WriteLine()
        Console.WriteLine("== Analyser un nombre : le piège du séparateur ==")

        Dim brut = "1234.56"   ' donnée d'échange : point décimal

        ' En culture française, le point n'est PAS le séparateur décimal :
        Dim enFrancais As Double
        Dim okFr = Double.TryParse(brut, NumberStyles.Float, Fr, enFrancais)
        Console.WriteLine($"TryParse(""{brut}"", fr-FR)      -> ok = {okFr}, valeur = {enFrancais.ToString(Inv)} (interprétation inattendue !)")

        ' ✅ Toujours la culture invariante pour des données d'échange :
        Dim valeur = Double.Parse(brut, Inv)
        Console.WriteLine($"Parse(""{brut}"", Invariant)    -> {valeur.ToString(Inv)}")

        Dim n As Double
        If Double.TryParse(brut, NumberStyles.Float, Inv, n) Then
            Console.WriteLine($"TryParse + NumberStyles.Float -> {n.ToString(Inv)} (forme sûre)")
        End If
    End Sub

    ' ---- Formater des nombres ------------------------------------------------------------------
    Private Sub DemoFormatageNombres()
        Console.WriteLine()
        Console.WriteLine("== Formater des nombres (culture explicite) ==")

        Dim montant = 1234.5D
        Console.WriteLine($"C2 fr-FR     : {montant.ToString("C2", Fr)}")
        Console.WriteLine($"N2 fr-FR     : {montant.ToString("N2", Fr)}")
        Console.WriteLine($"N2 Invariant : {montant.ToString("N2", Inv)}")
        Console.WriteLine($"P1 fr-FR     : {(0.125).ToString("P1", Fr)}")
        Console.WriteLine($"D5           : {42.ToString("D5", Inv)}")
        Console.WriteLine($"X            : {255.ToString("X", Inv)}")
        Console.WriteLine($"#,##0.00 fr  : {montant.ToString("#,##0.00", Fr)}")
    End Sub

    ' ---- Le piège du « i » turc ---------------------------------------------------------------------
    Private Sub DemoPiegeTurc()
        Console.WriteLine()
        Console.WriteLine("== Pourquoi l'ordinal pour les identifiants : le « i » turc ==")

        Dim tr As New CultureInfo("tr-TR")
        Console.WriteLine($"""i"".ToUpper(en-US) = {"i".ToUpper(En)}")
        Console.WriteLine($"""i"".ToUpper(tr-TR) = {"i".ToUpper(tr)} (İ : I pointé majuscule !)")
        Console.WriteLine($"-> clés/identifiants : StringComparison.Ordinal ; affichage : culture de l'utilisateur")
    End Sub

End Module
