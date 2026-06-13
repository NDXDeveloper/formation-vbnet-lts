' ============================================================================
'  Section 5.1 : Introduction, architecture et le Concepteur (Designer)
'  Description : Utilitaire de TEST (hors cours). Quand la variable
'                d'environnement DEMO_AUTOCLOSE vaut "1", le formulaire passé
'                se ferme automatiquement après ~1,5 s — ce qui permet un
'                « smoke test » automatisé (lancer / ne pas planter / fermer)
'                sans interaction. Sans cette variable, l'application
'                fonctionne normalement et reste ouverte.
'                Ce module est répliqué à l'identique dans chaque exemple.
' ============================================================================

Imports System.Windows.Forms

Public Module AutoFermeture

    ''' <summary>Active l'auto-fermeture si DEMO_AUTOCLOSE=1 (pour les tests).</summary>
    Public Sub Activer(formulaire As Form, Optional delaiMs As Integer = 1500)
        If Environment.GetEnvironmentVariable("DEMO_AUTOCLOSE") <> "1" Then Return

        Dim minuteur As New Timer With {.Interval = delaiMs}
        AddHandler minuteur.Tick,
            Sub()
                minuteur.Stop()
                minuteur.Dispose()
                formulaire.Close()
            End Sub
        minuteur.Start()
    End Sub

    ''' <summary>Écrit une ligne dans le journal d'auto-test (%TEMP%\<nom>-autotest.log).</summary>
    Public Sub Journaliser(nomFichier As String, ligne As String)
        Dim chemin = IO.Path.Combine(IO.Path.GetTempPath(), nomFichier)
        IO.File.AppendAllText(chemin, ligne & Environment.NewLine)
    End Sub

    ''' <summary>Réinitialise le journal d'auto-test (au premier appel d'un Load).</summary>
    Public Sub ReinitialiserJournal(nomFichier As String)
        Dim chemin = IO.Path.Combine(IO.Path.GetTempPath(), nomFichier)
        If IO.File.Exists(chemin) Then IO.File.Delete(chemin)
    End Sub

End Module
