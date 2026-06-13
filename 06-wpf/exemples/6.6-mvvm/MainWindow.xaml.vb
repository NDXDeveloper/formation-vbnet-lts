' ============================================================================
'  Section 6.6 : Architecture MVVM
'  Description : Le code-behind se limite à associer le view-model au
'                DataContext (la communication est à sens unique : la vue
'                pointe vers le VM, jamais l'inverse). Le journal d'auto-test
'                vérifie le comportement de la COMMANDE sans interface :
'                CanExecute est faux tant que Nom est vide, vrai ensuite, et
'                Execute applique la logique métier.
'  Fichier source : 06-mvvm.md
' ============================================================================

Class MainWindow

    Private Const JOURNAL As String = "6.6-mvvm-autotest.log"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        DataContext = New ClientViewModel()
        ExecuterAutoTests()
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub ExecuterAutoTests()
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        ' --- RelayCommand « à la main » ---
        Dim vm As New ClientViewModel()
        Dim avantSaisie = vm.EnregistrerCommand.CanExecute(Nothing)   ' Nom vide -> False
        vm.Nom = "Durand"
        Dim apresSaisie = vm.EnregistrerCommand.CanExecute(Nothing)   ' Nom rempli -> True
        vm.EnregistrerCommand.Execute(Nothing)
        AutoFermeture.Journaliser(JOURNAL, $"RelayCommand maison : CanExecute (Nom vide) = {avantSaisie} ; (Nom rempli) = {apresSaisie}")
        AutoFermeture.Journaliser(JOURNAL, $"Execute -> DernierEnregistre = {vm.DernierEnregistre} ({vm.DernierEnregistre = "Durand"})")

        ' --- Variante CommunityToolkit.Mvvm ---
        Dim vmKit As New ClientViewModelToolkit()
        Dim kitAvant = vmKit.EnregistrerCommand.CanExecute(Nothing)
        vmKit.Nom = "Martin"
        Dim kitApres = vmKit.EnregistrerCommand.CanExecute(Nothing)
        AutoFermeture.Journaliser(JOURNAL, $"CommunityToolkit : CanExecute (vide) = {kitAvant} ; (rempli) = {kitApres}")
    End Sub

End Class
