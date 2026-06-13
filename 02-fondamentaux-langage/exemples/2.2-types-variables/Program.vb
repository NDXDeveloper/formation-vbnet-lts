' ============================================================================
'  Section 2.2 : Types de données et variables
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · types valeur (copie) vs types référence (même objet) ;
'                  · Nothing contextuel (zéro du type / référence nulle) ;
'                  · littéraux (suffixes, _, &H, &B, #date#) ;
'                  · déclarations multiples et variable locale Static ;
'                  · types nullables de valeur (Integer?) — et la différence
'                    avec les nullable reference types de C#, absents de VB ;
'                  · inférence de type ; constantes ; énumérations (Enums.vb).
'  Fichier source : 02-types-variables.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoValeurVsReference()
        DemoNothing()
        DemoLitteraux()
        DemoStatic()
        DemoNullables()
        DemoInference()
        DemoConstantesEtEnums()
    End Sub

    ' ---- Types valeur vs types référence --------------------------------
    Private Sub DemoValeurVsReference()
        Console.WriteLine("== Types valeur vs types référence ==")

        ' Type valeur : copie indépendante
        Dim a As Integer = 10
        Dim b As Integer = a
        b = 99
        Console.WriteLine($"a = {a} (inchangé : la valeur a été copiée)")

        ' Type référence : copie de la référence (même objet)
        Dim liste1 As New List(Of Integer) From {1, 2, 3}
        Dim liste2 = liste1
        liste2.Add(4)
        Console.WriteLine($"liste1.Count = {liste1.Count} (liste1 et liste2 pointent le même objet)")
    End Sub

    ' ---- Nothing : un mot-clé au sens contextuel --------------------------
    Private Sub DemoNothing()
        Console.WriteLine()
        Console.WriteLine("== Nothing contextuel ==")

        Dim n As Integer = Nothing   ' valeur par défaut du type : 0
        Dim s As String = Nothing    ' référence nulle
        Console.WriteLine($"Dim n As Integer = Nothing -> n = {n}")
        Console.WriteLine($"Dim s As String = Nothing  -> s Is Nothing = {s Is Nothing}")
    End Sub

    ' ---- Littéraux ---------------------------------------------------------
    Private Sub DemoLitteraux()
        Console.WriteLine()
        Console.WriteLine("== Littéraux ==")

        Dim grand As Long = 1_000_000L     ' suffixe L, séparateur de chiffres '_'
        Dim hexa As Integer = &HFF         ' hexadécimal
        Dim binaire As Integer = &B1010    ' binaire
        Dim prix As Decimal = 19.99D       ' suffixe D -> Decimal (montants)
        Dim lettre As Char = "A"c          ' suffixe c -> Char
        Dim noel As Date = #2026-12-25#    ' littéral de date (forme ISO recommandée)

        Console.WriteLine($"1_000_000L -> {grand}")
        Console.WriteLine($"&HFF -> {hexa}")
        Console.WriteLine($"&B1010 -> {binaire}")
        Console.WriteLine($"19.99D -> {prix}")
        Console.WriteLine($"""A""c -> {lettre}")
        Console.WriteLine($"#2026-12-25# -> {noel:yyyy-MM-dd}")
    End Sub

    ' ---- Variable locale Static -------------------------------------------
    Private Sub DemoStatic()
        Console.WriteLine()
        Console.WriteLine("== Variable locale Static (conserve sa valeur entre les appels) ==")
        Compter()   ' 1
        Compter()   ' 2
        Compter()   ' 3
    End Sub

    Private Sub Compter()
        Static appels As Integer = 0
        appels += 1
        Console.WriteLine($"Appel n° {appels}")
    End Sub

    ' ---- Types nullables de valeur ------------------------------------------
    Private Sub DemoNullables()
        Console.WriteLine()
        Console.WriteLine("== Types nullables de valeur (Integer?) ==")

        Dim age As Integer? = Nothing      ' équivalent : Nullable(Of Integer)
        Console.WriteLine($"age.HasValue = {age.HasValue} ; age Is Nothing = {age Is Nothing}")

        age = 30
        Console.WriteLine($"après age = 30 : HasValue = {age.HasValue}, Value = {age.Value}")

        ' Valeur de repli : pas d'opérateur ?? en VB -> If() à deux arguments
        Dim absent As Integer? = Nothing
        Console.WriteLine($"If(age, 0) = {If(age, 0)} ; If(absent, 0) = {If(absent, 0)}")
        Console.WriteLine($"absent.GetValueOrDefault() = {absent.GetValueOrDefault()}")

        ' ⚠️ Le '?' ne s'applique qu'aux types VALEUR — String est un type référence :
        ' Dim nom As String?   ' ERREUR BC33101 (vérifiée) : « Le type 'String' doit
        '                      ' être un type valeur [...] pour pouvoir être utilisé
        '                      ' avec 'Nullable' ou le modificateur '?' »
        '                      ' (les nullable reference types sont propres à C#).
    End Sub

    ' ---- Inférence de type ---------------------------------------------------
    Private Sub DemoInference()
        Console.WriteLine()
        Console.WriteLine("== Inférence de type (Option Infer On) ==")

        Dim compteur = 0                    ' inféré : Integer
        Dim message = "Bonjour"             ' inféré : String
        Dim nombres = New List(Of Integer)  ' inféré : List(Of Integer)

        ' Le type est statique et figé : affecter une chaîne à 'compteur'
        ' serait une erreur de compilation.
        Console.WriteLine($"compteur : {compteur.GetType().Name}")
        Console.WriteLine($"message  : {message.GetType().Name}")
        Console.WriteLine($"nombres  : {nombres.GetType().Name}")
    End Sub

    ' ---- Constantes et énumérations -------------------------------------------
    Const TauxTVA As Decimal = 0.2D          ' résolue à la compilation
    Const NomApp As String = "MonApplication"

    Private Sub DemoConstantesEtEnums()
        Console.WriteLine()
        Console.WriteLine("== Constantes et énumérations ==")
        Console.WriteLine($"Const TauxTVA = {TauxTVA} ; NomApp = {NomApp}")

        ' Énumération simple : valeurs auto-incrémentées depuis 0
        Dim jour As JourSemaine = JourSemaine.Mercredi
        Console.WriteLine($"jour = {jour} ; CInt(jour) = {CInt(jour)}")

        ' Type sous-jacent et valeurs explicites
        Console.WriteLine($"CodeHttp.NonTrouve = {CInt(CodeHttp.NonTrouve)}")

        ' <Flags> : indicateurs combinables par bits
        Dim p As Permissions = Permissions.Lecture Or Permissions.Ecriture
        Console.WriteLine($"p.HasFlag(Lecture) = {p.HasFlag(Permissions.Lecture)}")
        Console.WriteLine($"p = {p}")
    End Sub

End Module
