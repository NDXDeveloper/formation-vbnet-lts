' ============================================================================
'  Section 3.6 : Événements et délégués — un point fort idiomatique de VB.NET
'  Description : Le pattern Observer exact de la section : le sujet
'                (Thermostat) notifie via un événement déclenché dans le
'                Set de sa propriété (motif OnXxx + RaiseEvent) ; l'observateur
'                déclaratif (AffichagePanneau) s'abonne par WithEvents/Handles ;
'                un observateur dynamique (lambda) s'ajoute par AddHandler.
'  Fichier source : 06-evenements-delegues.md
' ============================================================================

' --- Sujet (observable) ---
Public Class Thermostat
    Private _temperature As Double

    Public Event TemperatureChangee As EventHandler(Of TemperatureEventArgs)

    Public Property Temperature As Double
        Get
            Return _temperature
        End Get
        Set(value As Double)
            If value <> _temperature Then
                _temperature = value
                OnTemperatureChangee(New TemperatureEventArgs(value))
            End If
        End Set
    End Property

    Protected Overridable Sub OnTemperatureChangee(e As TemperatureEventArgs)
        RaiseEvent TemperatureChangee(Me, e)
    End Sub
End Class

Public Class TemperatureEventArgs
    Inherits EventArgs

    Public ReadOnly Property Valeur As Double

    Public Sub New(valeur As Double)
        Me.Valeur = valeur
    End Sub
End Class

' --- Observateur déclaratif (WithEvents + Handles) ---
Public Class AffichagePanneau
    Private WithEvents _thermostat As Thermostat

    Public Sub New(thermostat As Thermostat)
        _thermostat = thermostat
    End Sub

    Private Sub SurChangement(sender As Object, e As TemperatureEventArgs) _
            Handles _thermostat.TemperatureChangee
        Console.WriteLine($"  Panneau : {e.Valeur:F1} °C")
    End Sub
End Class
