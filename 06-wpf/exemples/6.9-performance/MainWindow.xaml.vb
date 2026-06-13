' ============================================================================
'  Section 6.9 : Performance WPF (virtualisation, binding, rendu)
'  Description : Reprend les leviers de la section, avec vérification :
'                  · charge 10 000 éléments dans une ListBox virtualisée
'                    (l'affectation en bloc évite un événement par ajout) ;
'                  · GÈLE un Freezable (SolidColorBrush) : immuable et
'                    thread-safe -> IsFrozen = True (consigné) ;
'                  · lit RenderCapability.Tier (niveau d'accélération
'                    matérielle disponible sur le poste).
'  Fichier source : 09-performance.md
' ============================================================================

Imports System.Linq
Imports System.Windows.Interop   ' RenderCapability
Imports System.Windows.Media

Class MainWindow

    Private Const JOURNAL As String = "6.9-performance-autotest.log"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        ' Charger en BLOC (une List construite d'avance, affectée d'un coup)
        Dim donnees = Enumerable.Range(1, 10000).Select(Function(n) $"Élément {n:N0}").ToList()
        grandeListe.ItemsSource = donnees

        ' Geler un Freezable : immuable, thread-safe, moins coûteux
        Dim pinceau As New SolidColorBrush(Colors.SteelBlue)
        pinceau.Freeze()

        ' Niveau d'accélération matérielle (0 = logiciel, 1 partiel, 2 complet)
        Dim tier = RenderCapability.Tier >> 16

        lblInfo.Text = $"{donnees.Count:N0} éléments dans une ListBox virtualisée (Recycling)." & Environment.NewLine &
                       $"Pinceau gelé : IsFrozen = {pinceau.IsFrozen} ; RenderCapability.Tier = {tier}"

        AutoFermeture.Journaliser(JOURNAL, $"Éléments chargés : {donnees.Count}")
        AutoFermeture.Journaliser(JOURNAL, $"Freezable gelé : IsFrozen = {pinceau.IsFrozen}")
        AutoFermeture.Journaliser(JOURNAL, $"RenderCapability.Tier = {tier}")

        AutoFermeture.Activer(Me)
    End Sub

End Class
