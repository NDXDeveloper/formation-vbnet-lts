' ============================================================================
'  Section 6.3 : Contrôles et contrôles de données
'  Description : Le réflexe WPF de la section — on LIE une ObservableCollection
'                (et non on ajoute des lignes une à une) : tout ajout/retrait
'                se répercute aussitôt à l'écran. Le journal d'auto-test
'                vérifie cette synchronisation (le Count suit Add/Remove).
'  Fichier source : 03-controles.md
' ============================================================================

Imports System.Collections.ObjectModel

Class MainWindow

    Private Const JOURNAL As String = "6.3-controles-autotest.log"

    Private ReadOnly _clients As New ObservableCollection(Of Client)()
    Private ReadOnly _produits As New ObservableCollection(Of Produit)()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        _clients.Add(New Client With {.Nom = "Durand", .Ville = "Rouen", .EstActif = True})
        _clients.Add(New Client With {.Nom = "Martin", .Ville = "Lyon", .EstActif = False})
        grilleClients.ItemsSource = _clients

        _produits.Add(New Produit With {.Reference = "REF-001", .Designation = "Clavier", .Prix = 45D})
        _produits.Add(New Produit With {.Reference = "REF-002", .Designation = "Écran 27""", .Prix = 220D})
        listeProduits.ItemsSource = _produits

        ExecuterAutoTests()
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub Ajouter_Click(sender As Object, e As RoutedEventArgs)
        _clients.Add(New Client With {.Nom = "Nouveau", .Ville = "Paris", .EstActif = True})
    End Sub

    Private Sub Supprimer_Click(sender As Object, e As RoutedEventArgs)
        Dim selection = TryCast(grilleClients.SelectedItem, Client)
        If selection IsNot Nothing Then _clients.Remove(selection)   ' la grille se rafraîchit seule
    End Sub

    Private Sub ExecuterAutoTests()
        AutoFermeture.ReinitialiserJournal(JOURNAL)
        Dim avant = _clients.Count
        _clients.Add(New Client With {.Nom = "Temporaire", .Ville = "Nice"})
        Dim apresAjout = _clients.Count
        _clients.RemoveAt(_clients.Count - 1)
        Dim apresRetrait = _clients.Count
        AutoFermeture.Journaliser(JOURNAL, $"ObservableCollection liée au DataGrid : {avant} -> Add -> {apresAjout} -> Remove -> {apresRetrait}")
        AutoFermeture.Journaliser(JOURNAL, $"Synchronisation cohérente : {apresAjout = avant + 1 AndAlso apresRetrait = avant}")
    End Sub

End Class
