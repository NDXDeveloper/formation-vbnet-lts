' ============================================================================
'  Section 5.5 : Contrôles personnalisés et UserControl
'  Description : Le UserControl exact de la section : compose une TextBox et
'                un Button, expose une propriété publique « Placeholder »
'                (avec <Category>/<DefaultValue>/<Description> — DefaultValue
'                satisfait aussi l'analyseur WFO1000) et un événement
'                personnalisé RechercheDemandee (idiome VB Event/RaiseEvent).
'                Le « Inherits UserControl » est dans SearchBox.Designer.vb.
'  Fichier source : 05-controles-personnalises.md
' ============================================================================

Imports System.ComponentModel

Public Class SearchBox

    <Category("Apparence")>
    <DefaultValue("")>
    <Description("Texte d'invite affiché dans le champ de recherche.")>
    Public Property Placeholder As String
        Get
            Return txtRecherche.PlaceholderText
        End Get
        Set(value As String)
            txtRecherche.PlaceholderText = value
        End Set
    End Property

    ' Événement personnalisé (idiome VB Event/RaiseEvent, module 3.6)
    Public Event RechercheDemandee As EventHandler(Of RechercheEventArgs)

    Private Sub btnRechercher_Click(sender As Object, e As EventArgs) Handles btnRechercher.Click
        RaiseEvent RechercheDemandee(Me, New RechercheEventArgs(txtRecherche.Text))
    End Sub

End Class

''' <summary>EventArgs dédié transportant le terme recherché.</summary>
Public Class RechercheEventArgs
    Inherits EventArgs

    Public ReadOnly Property Terme As String

    Public Sub New(terme As String)
        Me.Terme = terme
    End Sub
End Class
