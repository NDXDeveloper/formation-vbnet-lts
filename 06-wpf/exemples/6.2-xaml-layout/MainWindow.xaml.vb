' ============================================================================
'  Section 6.2 : XAML fondamentaux et systèmes de layout
'  Description : Le gestionnaire d'événement ROUTÉ de la section, attaché au
'                StackPanel : il capte le Click de TOUS ses boutons. On
'                distingue « sender » (le panneau, où le gestionnaire est
'                attaché) de « e.Source » (le bouton réellement cliqué), puis
'                « e.Handled = True » stoppe la remontée.
'  Fichier source : 02-xaml-layout.md
' ============================================================================

Class MainWindow

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub Boutons_Click(sender As Object, e As RoutedEventArgs)
        Dim bouton = DirectCast(e.Source, Button)   ' e.Source : l'émetteur réel
        lblStatut.Text = $"Navigation : {bouton.Content} (sender = {sender.GetType().Name})"
        e.Handled = True                            ' stoppe la remontée
    End Sub

End Class
