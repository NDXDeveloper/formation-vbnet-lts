' ============================================================================
'  Section 6.7 : Animations et multimédia (notions)
'  Description : Le pilotage d'animation EN CODE de la section, via
'                BeginAnimation : une DoubleAnimation sur l'opacité du bouton.
'                (Le multimédia — MediaElement — dépend des codecs du poste et
'                n'est pas exécuté ici.)
'  Fichier source : 07-animations-multimedia.md
' ============================================================================

Imports System.Windows.Media.Animation

Class MainWindow

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub btnAnimer_Click(sender As Object, e As RoutedEventArgs)
        ' Animation déclenchée en code (aller-retour d'opacité)
        Dim anim As New DoubleAnimation(1, 0.3, New Duration(TimeSpan.FromMilliseconds(250))) With {
            .AutoReverse = True,
            .RepeatBehavior = New RepeatBehavior(2)
        }
        btnAnimer.BeginAnimation(UIElement.OpacityProperty, anim)
    End Sub

End Class
