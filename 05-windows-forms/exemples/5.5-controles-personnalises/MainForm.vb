' ============================================================================
'  Section 5.5 : Contrôles personnalisés et UserControl
'  Description : Formulaire hôte des trois contrôles personnalisés de la
'                section : le UserControl SearchBox (et son événement
'                RechercheDemandee), le NumericTextBox (saisie filtrée) et le
'                LedIndicator (dessiné). Un bouton bascule la LED.
'  Fichier source : 05-controles-personnalises.md
' ============================================================================

Imports System.Windows.Forms

Public Class MainForm

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        searchBox1.Placeholder = "Tapez un terme…"
        AutoFermeture.Activer(Me)
    End Sub

    ' L'événement personnalisé du UserControl
    Private Sub searchBox1_RechercheDemandee(sender As Object, e As RechercheEventArgs) _
            Handles searchBox1.RechercheDemandee
        lblResultat.Text = $"Recherche demandée : « {e.Terme} »"
    End Sub

    Private Sub btnBasculer_Click(sender As Object, e As EventArgs) Handles btnBasculer.Click
        led.Allume = Not led.Allume
        lblResultat.Text = $"LED : {If(led.Allume, "allumée", "éteinte")}"
    End Sub

End Class
