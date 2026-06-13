' ============================================================================
'  Section 6.4 : Liaison de données
'  Description : Le code-behind fournit le DataContext (un Client) et la
'                collection liée. Le journal d'auto-test vérifie deux
'                mécanismes clés sans interface : la NOTIFICATION
'                INotifyPropertyChanged (modifier Nom déclenche PropertyChanged)
'                et le CONVERTISSEUR (EstActif -> pinceau vert/rouge).
'  Fichier source : 04-data-binding.md
' ============================================================================

Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Windows.Media

Class MainWindow

    Private Const JOURNAL As String = "6.4-data-binding-autotest.log"

    Private ReadOnly _courant As New Client With {.Nom = "Durand", .Ville = "Rouen", .EstActif = True}
    Private ReadOnly _clients As New ObservableCollection(Of Client)()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ' DataContext : la source par défaut de toutes les liaisons enfants
        DataContext = _courant

        _clients.Add(_courant)
        _clients.Add(New Client With {.Nom = "Martin", .Ville = "Lyon", .EstActif = False})
        listeClients.ItemsSource = _clients

        ExecuterAutoTests()
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub ExecuterAutoTests()
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        ' 1. INotifyPropertyChanged : modifier Nom déclenche PropertyChanged
        Dim c As New Client With {.Nom = "Avant"}
        Dim notifie As String = Nothing
        AddHandler c.PropertyChanged, Sub(s, ev) notifie = ev.PropertyName
        c.Nom = "Après"
        AutoFermeture.Journaliser(JOURNAL, $"INotifyPropertyChanged : Nom modifié -> PropertyChanged(""{notifie}"") -> {notifie = "Nom"}")

        ' 2. Convertisseur : EstActif -> couleur
        Dim conv As New BoolEnCouleurConverter()
        Dim vrai = conv.Convert(True, GetType(Brush), Nothing, CultureInfo.InvariantCulture)
        Dim faux = conv.Convert(False, GetType(Brush), Nothing, CultureInfo.InvariantCulture)
        AutoFermeture.Journaliser(JOURNAL, $"Convertisseur True  -> {DirectCast(vrai, SolidColorBrush).Color} (Green attendu : {vrai Is Brushes.Green})")
        AutoFermeture.Journaliser(JOURNAL, $"Convertisseur False -> {DirectCast(faux, SolidColorBrush).Color} (Red attendu : {faux Is Brushes.Red})")

        ' 3. ValidationRule : champ vide rejeté
        Dim regle As New ChampRequisRule()
        Dim vide = regle.Validate("", CultureInfo.InvariantCulture)
        Dim rempli = regle.Validate("Durand", CultureInfo.InvariantCulture)
        AutoFermeture.Journaliser(JOURNAL, $"ValidationRule : vide IsValid={vide.IsValid} ; rempli IsValid={rempli.IsValid}")
    End Sub

End Class
