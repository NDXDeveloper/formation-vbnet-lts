' ============================================================================
'  Section 6.5 : Styles, ressources, templates et triggers
'  Description : Le code-behind se borne à alimenter la liste : tout
'                l'habillage (styles, triggers, templates) est PUREMENT
'                DÉCLARATIF en XAML — identique en VB.NET et en C#.
'  Fichier source : 05-styles-templates.md
' ============================================================================

Imports System.Collections.ObjectModel

Class MainWindow

    Private ReadOnly _clients As New ObservableCollection(Of Client) From {
        New Client With {.Nom = "Durand", .Ville = "Rouen", .EstActif = True},
        New Client With {.Nom = "Martin", .Ville = "Lyon", .EstActif = False},
        New Client With {.Nom = "Petit", .Ville = "Lille", .EstActif = True}
    }

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        listeClients.ItemsSource = _clients
        AutoFermeture.Activer(Me)
    End Sub

End Class
