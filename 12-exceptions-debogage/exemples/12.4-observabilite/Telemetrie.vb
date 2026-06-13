' ============================================================================
'  Section 12.4 : Observabilité — primitives d'instrumentation
'  Description : Les deux sources natives qu'OpenTelemetry écoute. Un Meter
'                expose un Counter (métriques) ; un ActivitySource crée des
'                Activity (= spans, traces). Note VB importante : StartActivity
'                renvoie Nothing s'il n'y a AUCUN écouteur, d'où l'opérateur
'                conditionnel « activite?. ». Activity implémente IDisposable :
'                le bloc Using clôt le span et mesure sa durée.
'  Fichier source : 04-observabilite.md
' ============================================================================

Imports System.Diagnostics
Imports System.Diagnostics.Metrics

' --- Métriques ---
Public Class MetriquesCommandes
    Public Shared ReadOnly Meter As New Meter("Demo.Commandes")

    Public Shared ReadOnly CommandesTraitees As Counter(Of Long) =
        Meter.CreateCounter(Of Long)("commandes.traitees", "commande", "Nombre de commandes traitées")

    Public Shared Sub Traiter()
        CommandesTraitees.Add(1)
    End Sub
End Class

' --- Traces ---
Public Class TracesCommandes
    Public Shared ReadOnly Source As New ActivitySource("Demo.Commandes")

    Public Shared Function Traiter(commandeId As Integer) As String
        Using activite = Source.StartActivity("TraiterCommande")
            activite?.SetTag("commande.id", commandeId)   ' Nothing si aucun écouteur
            ' … travail simulé …
            Return If(activite Is Nothing, "(aucun écouteur)", activite.DisplayName)
        End Using
    End Function
End Class
