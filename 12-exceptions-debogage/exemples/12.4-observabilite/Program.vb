' ============================================================================
'  Section 12.4 : Observabilité — orchestration et vérification
'  Description : Émet une métrique (Meter + Counter) et vérifie l'instrument ;
'                crée une trace (ActivitySource + Activity) observée via un
'                ActivityListener et vérifie nom/tag/durée. Note VB : le rappel
'                de Sample porte un paramètre ByRef → méthode nommée (AddressOf).
'                ⚠️ Frontière VB : lire une métrique en process avec
'                MeterListener exige un rappel à ReadOnlySpan que VB ne peut PAS
'                déclarer (BC30668) — VB émet, un collecteur lit.
'  Fichier source : 04-observabilite.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Diagnostics

Module Program

    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 12.4 Observabilité : métriques et traces ===")
        Console.WriteLine()

        DemoMetriques()
        DemoTraces()

        Console.WriteLine("Terminé.")
    End Sub

    Private Sub DemoMetriques()
        Console.WriteLine("[Métriques — Meter + Counter (émission)]")
        Dim compteur = MetriquesCommandes.CommandesTraitees
        Console.WriteLine($"  instrument : nom={compteur.Name}, unité={compteur.Unit}, type={compteur.GetType().Name}")

        For i = 1 To 5
            MetriquesCommandes.Traiter()   ' émission ; un collecteur agrège
        Next
        Console.WriteLine("  5 mesures émises sans erreur = True")
        Console.WriteLine("  (lecture en process : MeterListener attend un rappel à ReadOnlySpan que VB ne")
        Console.WriteLine("   peut déclarer — BC30668 ; VB ÉMET, un collecteur LIT : dotnet-counters / OpenTelemetry)")
        Console.WriteLine()
    End Sub

    Private Sub DemoTraces()
        Console.WriteLine("[Traces — ActivitySource + Activity, via ActivityListener]")
        Dim activiteArretee As Activity = Nothing

        Using listener As New ActivityListener()
            listener.ShouldListenTo = Function(src) src.Name = "Demo.Commandes"
            listener.Sample = AddressOf Echantillonner
            listener.ActivityStopped = Sub(act) activiteArretee = act
            ActivitySource.AddActivityListener(listener)

            Dim nom = TracesCommandes.Traiter(4271)
            Console.WriteLine($"  StartActivity (écouteur présent) -> {nom}")
        End Using

        Console.WriteLine($"  activité arrêtée   : {If(activiteArretee?.DisplayName, "(nulle)")}")
        Console.WriteLine($"  tag commande.id    : {activiteArretee?.GetTagItem("commande.id")}")
        Console.WriteLine($"  durée mesurée >= 0 : {activiteArretee IsNot Nothing AndAlso activiteArretee.Duration.Ticks >= 0}")
        Console.WriteLine()
    End Sub

    ' Échantillonnage : paramètre ByRef → méthode nommée obligatoire (lambda VB impossible).
    Private Function Echantillonner(ByRef options As ActivityCreationOptions(Of ActivityContext)) As ActivitySamplingResult
        Return ActivitySamplingResult.AllData
    End Function

End Module
