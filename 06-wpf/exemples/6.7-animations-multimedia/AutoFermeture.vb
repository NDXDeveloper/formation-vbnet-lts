' ============================================================================
'  Utilitaire de TEST (hors cours) — réutilisé dans chaque exemple WPF.
'  Si la variable d'environnement DEMO_AUTOCLOSE vaut "1", la fenêtre passée
'  se ferme automatiquement après ~1,8 s : « smoke test » automatisé
'  (lancer / ne pas planter / fermer) sans interaction. Sans la variable,
'  l'application fonctionne normalement et reste ouverte.
' ============================================================================

Imports System.Windows
Imports System.Windows.Threading

Public Module AutoFermeture

    Public Sub Activer(fenetre As Window, Optional delaiMs As Integer = 1800)
        If Environment.GetEnvironmentVariable("DEMO_AUTOCLOSE") <> "1" Then Return
        Dim minuteur As New DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(delaiMs)}
        AddHandler minuteur.Tick,
            Sub()
                minuteur.Stop()
                fenetre.Close()
            End Sub
        minuteur.Start()
    End Sub

    Public Sub Journaliser(nomFichier As String, ligne As String)
        IO.File.AppendAllText(IO.Path.Combine(IO.Path.GetTempPath(), nomFichier), ligne & Environment.NewLine)
    End Sub

    Public Sub ReinitialiserJournal(nomFichier As String)
        Dim chemin = IO.Path.Combine(IO.Path.GetTempPath(), nomFichier)
        If IO.File.Exists(chemin) Then IO.File.Delete(chemin)
    End Sub

End Module
