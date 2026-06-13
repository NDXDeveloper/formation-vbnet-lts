' ============================================================================
'  Section 6.1 : Introduction ; WPF vs Windows Forms
'  Description : Le code-behind WPF de la section, réduit au COMPORTEMENT :
'                le gestionnaire reçoit un RoutedEventArgs (et non un simple
'                EventArgs), et l'apparence vit dans le XAML. Un DataContext
'                minimal alimente le binding {Binding NomClient}.
'  Fichier source : 01-introduction-wpf-vs-winforms.md
' ============================================================================

Class MainWindow

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ' DataContext : la source par défaut des bindings (voir 6.4)
        DataContext = New With {.NomClient = "Valeur fournie par binding"}
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub Bouton_Click(sender As Object, e As RoutedEventArgs)
        MessageBox.Show("Bonjour depuis WPF !")
    End Sub

End Class
