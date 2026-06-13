' ============================================================================
'  Section 3.6 : Événements et délégués — un point fort idiomatique de VB.NET
'  Description : L'éditeur d'événements de la section, selon la convention
'                .NET : SeuilEventArgs (dérive d'EventArgs), événement
'                EventHandler(Of T), déclenchement par RaiseEvent — sûr par
'                construction (aucun test d'abonnés à écrire) — via le motif
'                Protected Overridable OnXxx. Le seuil est paramétrable
'                (100 par défaut, comme dans le cours).
'                Surveillance illustre l'idiome roi : WithEvents + Handles
'                (câblage déclaratif, re-câblage automatique à la
'                réaffectation du champ).
'  Fichier source : 06-evenements-delegues.md
' ============================================================================

Public Class SeuilEventArgs
    Inherits EventArgs

    Public ReadOnly Property Valeur As Integer

    Public Sub New(valeur As Integer)
        Me.Valeur = valeur
    End Sub
End Class

Public Class Compteur
    Public Event SeuilAtteint As EventHandler(Of SeuilEventArgs)

    Public ReadOnly Property Nom As String
    Private ReadOnly _seuil As Integer
    Private _valeur As Integer

    Public Sub New(Optional nom As String = "compteur", Optional seuil As Integer = 100)
        Me.Nom = nom
        _seuil = seuil
    End Sub

    Public Sub Incrementer()
        _valeur += 1
        If _valeur >= _seuil Then
            OnSeuilAtteint(New SeuilEventArgs(_valeur))
            _valeur = 0
        End If
    End Sub

    ' Le motif OnXxx : les dérivées peuvent intercepter ou déclencher.
    Protected Overridable Sub OnSeuilAtteint(e As SeuilEventArgs)
        RaiseEvent SeuilAtteint(Me, e)   ' sûr : ne fait rien s'il n'y a aucun abonné
    End Sub
End Class

''' <summary>Abonnement déclaratif : WithEvents + Handles (l'idiome VB ⭐).</summary>
Public Class Surveillance
    Private WithEvents _compteur As New Compteur("principal", seuil:=100)

    ''' <summary>
    ''' Re-câblage automatique : en réaffectant le champ WithEvents, VB détache
    ''' les gestionnaires de l'ancien objet et les rattache au nouveau.
    ''' </summary>
    Public Sub ChangerDeCompteur(nouveau As Compteur)
        _compteur = nouveau
    End Sub

    Public Sub Incrementer(fois As Integer)
        For i = 1 To fois
            _compteur.Incrementer()
        Next
    End Sub

    Private Sub Compteur_SeuilAtteint(sender As Object, e As SeuilEventArgs) _
            Handles _compteur.SeuilAtteint
        Dim source = DirectCast(sender, Compteur)
        Console.WriteLine($"  [Surveillance] Seuil atteint à {e.Valeur} (source : {source.Nom}).")
    End Sub
End Class

''' <summary>« Un gestionnaire, plusieurs événements » : Handles à sources multiples.</summary>
Public Class SurveillanceDouble
    Private WithEvents _compteurA As New Compteur("A", seuil:=2)
    Private WithEvents _compteurB As New Compteur("B", seuil:=3)

    Public Sub Stimuler()
        For i = 1 To 6
            _compteurA.Incrementer()
            _compteurB.Incrementer()
        Next
    End Sub

    ' Un seul gestionnaire pour les deux sources ; sender distingue l'origine.
    Private Sub Compteurs_SeuilAtteint(sender As Object, e As SeuilEventArgs) _
            Handles _compteurA.SeuilAtteint, _compteurB.SeuilAtteint
        Console.WriteLine($"  [Double] Compteur {DirectCast(sender, Compteur).Nom} a atteint {e.Valeur}.")
    End Sub
End Class
