' ============================================================================
'  Section 3.6 : Événements et délégués — un point fort idiomatique de VB.NET
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · délégués : types maison (Delegate Function/Sub),
'                    AddressOf, et les génériques Action / Func / Predicate ;
'                  · lambdas mono- et multi-lignes, et CAPTURE de variable
'                    (closure : changer « seuil » change le prédicat) ;
'                  · événement + RaiseEvent (sûr sans abonné) via le motif
'                    OnXxx ; abonnement déclaratif WithEvents + Handles ;
'                  · « un gestionnaire, plusieurs événements » (Handles à
'                    sources multiples) et RE-CÂBLAGE AUTOMATIQUE lors de la
'                    réaffectation d'un champ WithEvents ;
'                  · AddHandler / RemoveHandler (dynamique), y compris lambda ;
'                  · Custom Event (3 blocs) ;
'                  · le pattern Observer complet (Thermostat).
'  Fichier source : 06-evenements-delegues.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System

Module Program

    ' ---- Délégués maison (types) ----
    Public Delegate Function Operation(a As Integer, b As Integer) As Integer
    Public Delegate Sub Notification(message As String)

    Public Function Additionner(a As Integer, b As Integer) As Integer
        Return a + b
    End Function

    Public Function VerifierParite(n As Integer) As Boolean
        Return n Mod 2 = 0
    End Function

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoDelegues()
        DemoLambdas()
        DemoEvenementDeclaratif()
        DemoMultiSourcesEtRecablage()
        DemoAddRemoveHandler()
        DemoCustomEvent()
        DemoObserver()
    End Sub

    ' ---- Délégués ---------------------------------------------------------------
    Private Sub DemoDelegues()
        Console.WriteLine("== Délégués et AddressOf ==")

        Dim calcul As Operation = AddressOf Additionner
        Console.WriteLine($"calcul(3, 4) = {calcul(3, 4)} (appel indirect via le délégué)")

        Dim afficher As Action(Of String) = AddressOf Console.WriteLine
        afficher("Action(Of String) -> Console.WriteLine")

        Dim addition As Func(Of Integer, Integer, Integer) = AddressOf Additionner
        Console.WriteLine($"Func : addition(5, 6) = {addition(5, 6)}")

        Dim estPair As Predicate(Of Integer) = AddressOf VerifierParite
        Console.WriteLine($"Predicate : estPair(4) = {estPair(4)}")
    End Sub

    ' ---- Lambdas et closures ---------------------------------------------------------
    Private Sub DemoLambdas()
        Console.WriteLine()
        Console.WriteLine("== Lambdas et closures ==")

        Dim carre = Function(x As Integer) x * x
        Dim saluer = Sub(nom As String) Console.WriteLine($"Bonjour {nom}")
        Console.WriteLine($"carre(5) = {carre(5)}")
        saluer("Ada")

        Dim analyser = Function(texte As String)
                           Dim mots = texte.Split(" "c)
                           Return mots.Length
                       End Function
        Console.WriteLine($"analyser(""un deux trois"") = {analyser("un deux trois")} mots")

        ' Closure : la lambda capture LA VARIABLE seuil, pas sa valeur.
        Dim seuil = 10
        Dim depasse As Predicate(Of Integer) = Function(n) n > seuil
        Console.WriteLine($"seuil = 10  -> depasse(11) = {depasse(11)}")
        seuil = 100
        Console.WriteLine($"seuil = 100 -> depasse(11) = {depasse(11)} (la capture suit la variable)")
    End Sub

    ' ---- WithEvents + Handles -----------------------------------------------------------
    Private Sub DemoEvenementDeclaratif()
        Console.WriteLine()
        Console.WriteLine("== Event + RaiseEvent + WithEvents/Handles ==")

        Dim surveillance As New Surveillance()
        Console.WriteLine("100 incréments du compteur surveillé (seuil : 100)...")
        surveillance.Incrementer(100)    ' déclenche SeuilAtteint à 100
    End Sub

    ' ---- Multi-sources et re-câblage ------------------------------------------------------
    Private Sub DemoMultiSourcesEtRecablage()
        Console.WriteLine()
        Console.WriteLine("== Un gestionnaire, plusieurs événements ==")
        Dim duo As New SurveillanceDouble()
        duo.Stimuler()                   ' A (seuil 2) et B (seuil 3) notifient le MÊME handler

        Console.WriteLine()
        Console.WriteLine("== Re-câblage automatique du champ WithEvents ==")
        Dim surveillance As New Surveillance()
        Dim remplacant As New Compteur("remplaçant", seuil:=2)
        surveillance.ChangerDeCompteur(remplacant)   ' les Handles suivent le NOUVEL objet
        remplacant.Incrementer()
        remplacant.Incrementer()         ' seuil 2 -> notification captée par Surveillance
    End Sub

    ' ---- AddHandler / RemoveHandler --------------------------------------------------------
    Private Sub GererSeuil(sender As Object, e As SeuilEventArgs)
        Console.WriteLine($"  [GererSeuil] notifié à {e.Valeur}.")
    End Sub

    Private Sub DemoAddRemoveHandler()
        Console.WriteLine()
        Console.WriteLine("== AddHandler / RemoveHandler (abonnement dynamique) ==")

        Dim compteur As New Compteur("dynamique", seuil:=2)

        AddHandler compteur.SeuilAtteint, AddressOf GererSeuil
        compteur.Incrementer() : compteur.Incrementer()      ' notifie GererSeuil

        RemoveHandler compteur.SeuilAtteint, AddressOf GererSeuil
        compteur.Incrementer() : compteur.Incrementer()      ' plus d'abonné : silence
        Console.WriteLine("(après RemoveHandler : aucun message — RaiseEvent sans abonné est sûr)")

        ' Lambda abonnée directement (non désabonnable faute de référence) :
        AddHandler compteur.SeuilAtteint,
            Sub(s, e) Console.WriteLine($"  [lambda] Atteint : {e.Valeur}")
        compteur.Incrementer() : compteur.Incrementer()
    End Sub

    ' ---- Custom Event -------------------------------------------------------------------------
    Private Sub DemoCustomEvent()
        Console.WriteLine()
        Console.WriteLine("== Custom Event (3 blocs, dont RaiseEvent) ==")

        Dim capteur As New CapteurDonnees()
        Dim abonne As EventHandler(Of DonneesEventArgs) =
            Sub(s, e) Console.WriteLine($"  [abonné] données : {e.Contenu}")

        AddHandler capteur.DonneesRecues, abonne     ' passe par le bloc AddHandler
        capteur.Recevoir("trame n° 1")               ' passe par le bloc RaiseEvent
        RemoveHandler capteur.DonneesRecues, abonne  ' passe par le bloc RemoveHandler
        capteur.Recevoir("trame n° 2")               ' plus d'abonné : silence
    End Sub

    ' ---- Pattern Observer -----------------------------------------------------------------------
    Private Sub DemoObserver()
        Console.WriteLine()
        Console.WriteLine("== Pattern Observer (Thermostat) ==")

        Dim thermostat As New Thermostat()
        Dim panneau As New AffichagePanneau(thermostat)        ' observateur déclaratif

        AddHandler thermostat.TemperatureChangee,              ' observateur dynamique
            Sub(s, e) Console.WriteLine($"  Journal : {e.Valeur:F1} °C")

        thermostat.Temperature = 21    ' notifie tous les observateurs en une fois
        thermostat.Temperature = 21    ' valeur inchangée -> aucun événement
        thermostat.Temperature = 23.5  ' nouvelle notification
    End Sub

End Module
