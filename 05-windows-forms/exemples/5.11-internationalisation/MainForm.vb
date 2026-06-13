' ============================================================================
'  Section 5.11 : Internationalisation (i18n/l10n, ressources .resx)
'  Description : Reprend les extraits de la section :
'                  · affichage de My.Resources.MessageBienvenue (résolu selon
'                    CurrentUICulture, fixée dans ApplicationEvents.vb) ;
'                  · formatage sensible à la culture (prix en ToString("C"),
'                    date courte) selon CurrentCulture ;
'                  · journal d'auto-test : on interroge le ResourceManager
'                    avec des cultures EXPLICITES pour vérifier la résolution
'                    fr / en et le REPLI (de → ressource par défaut).
'                Astuce : relancez avec la variable DEMO_CULTURE=en pour voir
'                l'interface en anglais.
'  Fichier source : 11-internationalisation.md
' ============================================================================

Imports System.Globalization

Public Class MainForm

    Private Const JOURNAL As String = "5.11-internationalisation-autotest.log"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Texte localisé, résolu selon CurrentUICulture
        lblBienvenue.Text = My.Resources.MessageBienvenue
        lblCulture.Text = $"CurrentUICulture = {CultureInfo.CurrentUICulture.Name} ; " &
                          $"CurrentCulture = {CultureInfo.CurrentCulture.Name}"

        ' Formatage sensible à la culture (CurrentCulture)
        Dim prix As Decimal = 1234.5D
        lblPrix.Text = $"Prix : {prix.ToString("C")} — Date : {DateTime.Now.ToShortDateString()}"

        ExecuterAutoTests()
        AutoFermeture.Activer(Me)
    End Sub

    ' ---- Vérifie la résolution des ressources par culture (journal) ----
    Private Sub ExecuterAutoTests()
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        Dim rm = My.Resources.Resources.ResourceManager
        Dim fr = rm.GetString("MessageBienvenue", New CultureInfo("fr"))
        Dim en = rm.GetString("MessageBienvenue", New CultureInfo("en"))
        Dim de = rm.GetString("MessageBienvenue", New CultureInfo("de"))   ' pas de satellite -> repli

        AutoFermeture.Journaliser(JOURNAL, $"fr -> {fr}")
        AutoFermeture.Journaliser(JOURNAL, $"en -> {en}")
        AutoFermeture.Journaliser(JOURNAL, $"de (aucun satellite -> repli défaut) -> {de}")
        AutoFermeture.Journaliser(JOURNAL, $"Résolution fr correcte : {fr = "Bienvenue dans l'application !"}")
        AutoFermeture.Journaliser(JOURNAL, $"Résolution en correcte : {en = "Welcome to the application!"}")
        AutoFermeture.Journaliser(JOURNAL, $"Repli de -> défaut : {de = "Welcome (default / neutral)"}")
    End Sub

End Class
