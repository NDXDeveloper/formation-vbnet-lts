' ============================================================================
'  Section 6.8 : Thèmes et Fluent Design (.NET 10)
'  Description : Active le thème Fluent via la propriété ThemeMode (API
'                expérimentale : l'avertissement WPF0001 est neutralisé dans
'                le .vbproj). On suit le réglage clair/sombre de Windows
'                (ThemeMode.System). Le journal d'auto-test consigne le
'                ThemeMode effectivement appliqué.
'  Fichier source : 08-fluent-design-net10.md
' ============================================================================

Class MainWindow

    Private Const JOURNAL As String = "6.8-fluent-design-autotest.log"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        ' Active le thème Fluent, en suivant le thème de Windows.
        Me.ThemeMode = ThemeMode.System

        lblTheme.Text = $"ThemeMode fenêtre : {Me.ThemeMode}" & Environment.NewLine &
                        $"ThemeMode application : {Application.Current.ThemeMode}"

        AutoFermeture.Journaliser(JOURNAL, $"ThemeMode appliqué (fenêtre) : {Me.ThemeMode}")
        AutoFermeture.Journaliser(JOURNAL, $"Fluent actif (≠ None) : {Me.ThemeMode <> ThemeMode.None}")

        AutoFermeture.Activer(Me)
    End Sub

End Class
