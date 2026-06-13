' ============================================================================
'  Section 5.11 : Internationalisation (i18n/l10n, ressources .resx)
'  Description : On fixe les cultures TÔT, dans l'événement Startup du cadre
'                applicatif (avant l'affichage du premier formulaire), comme
'                le montre la section : CurrentUICulture (choix des ressources)
'                et CurrentCulture (formatage). Ici, on suit la variable
'                d'environnement DEMO_CULTURE si elle est définie (fr / en),
'                sinon « fr-FR ».
'  Fichier source : 11-internationalisation.md
' ============================================================================

Imports System.Globalization
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My

    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object,
                                          e As StartupEventArgs) _
                                          Handles Me.Startup
            Dim nom = Environment.GetEnvironmentVariable("DEMO_CULTURE")
            If String.IsNullOrEmpty(nom) Then nom = "fr-FR"

            Dim culture As New CultureInfo(nom)
            CultureInfo.CurrentUICulture = culture   ' langue de l'interface (ressources)
            CultureInfo.CurrentCulture = culture     ' formatage (dates, nombres, monnaie)
        End Sub

    End Class

End Namespace
