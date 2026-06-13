' ============================================================================
'  Section 14.1 : Profilage — métrique personnalisée
'  Description : Expose un compteur applicatif via System.Diagnostics.Metrics.
'                Ces métriques sont observables EN DIRECT, sans débogueur, par
'                `dotnet-counters monitor -n Profilage --counters MonApp.Metier`.
'                C'est pleinement dans le périmètre « consommation » de VB.NET.
'  Fichier source : 01-profilage.md
' ============================================================================

Imports System.Diagnostics.Metrics

Public Module Telemetrie
    Private ReadOnly Compteur As New Meter("MonApp.Metier", "1.0.0")

    Public ReadOnly CommandesTraitees As Counter(Of Long) =
        Compteur.CreateCounter(Of Long)("commandes-traitees")
End Module
