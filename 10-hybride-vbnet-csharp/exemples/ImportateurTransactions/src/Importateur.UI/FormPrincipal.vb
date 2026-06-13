' ============================================================================
'  Section 10.5 / 10.4 : Atelier — l'UI Windows Forms (VB)
'  Description : Le formulaire. Sélectionne un fichier, le charge de façon
'                ASYNCHRONE (interface réactive), affiche les transactions par
'                LIAISON DE DONNÉES et les totaux par catégorie. Il ne connaît
'                que le métier VB (ServiceImport) — la machinerie C# du cœur lui
'                est invisible, sous Option Strict On. En mode auto-test
'                (DEMO_AUTOCLOSE=1), charge le CSV livré, journalise, ferme.
'  Fichier source : 05-atelier-core-csharp-ui-vbnet.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms
Imports Importateur.Metier

Public Class FormPrincipal

    Private ReadOnly _service As New ServiceImport()    ' ne connaît que le métier VB
    Private ReadOnly _autotest As Boolean = (Environment.GetEnvironmentVariable("DEMO_AUTOCLOSE") = "1")
    Private ReadOnly _journal As String = Path.Combine(Path.GetTempPath(), "importateur-autotest.log")
    Private _dernier As ResultatImport

    ' Mode auto-test : charger le CSV exemple, journaliser les valeurs, fermer.
    Private Async Sub FormPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not _autotest Then Return
        Try
            Await ChargerEtAfficherAsync(Path.Combine(AppContext.BaseDirectory, "exemple-transactions.csv"))
            JournaliserResultat()
        Catch ex As Exception
            Journaliser($"ERREUR : {ex.GetType().Name} : {ex.Message}")
        Finally
            FermerSiAutotest()
        End Try
    End Sub

    Private Async Sub btnImporter_Click(sender As Object, e As EventArgs) Handles btnImporter.Click
        Using dialogue As New OpenFileDialog() With {.Filter = "Fichiers CSV|*.csv"}
            If dialogue.ShowDialog() <> DialogResult.OK Then Return
            Try
                Await ChargerEtAfficherAsync(dialogue.FileName)
            Catch ex As Exception
                MessageBox.Show($"Échec de l'import : {ex.Message}",
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    ' Cœur partagé des deux chemins : ouvre le flux, importe (métier VB → cœur C#), affiche.
    Private Async Function ChargerEtAfficherAsync(chemin As String) As Task
        btnImporter.Enabled = False
        progressBar.Visible = True
        Try
            Using flux = File.OpenRead(chemin)
                _dernier = Await _service.ImporterAsync(flux)
                AfficherResultat(_dernier)
            End Using
        Finally
            progressBar.Visible = False
            btnImporter.Enabled = True
        End Try
    End Function

    Private Sub AfficherResultat(resultat As ResultatImport)
        ' Liaison de données : la grille affiche les transactions (records) valides.
        grilleTransactions.DataSource = resultat.Transactions.ToList()

        lblTotal.Text = $"Total : {resultat.TotalGeneral:C}"
        lblRejetees.Text = $"Rejetées : {resultat.Rejetees.Count}"

        lstCategories.Items.Clear()
        For Each kv In resultat.TotauxParCategorie
            lstCategories.Items.Add($"{kv.Key} : {kv.Value:C}")
        Next
    End Sub

    ' --- Auto-test : journalisation déterministe (culture invariante) ---

    Private Sub JournaliserResultat()
        Dim inv = CultureInfo.InvariantCulture
        Journaliser($"Transactions valides = {_dernier.Transactions.Count}")
        Journaliser($"Rejetees = {_dernier.Rejetees.Count}")
        Journaliser($"TotalGeneral = {_dernier.TotalGeneral.ToString(inv)}")
        For Each kv In _dernier.TotauxParCategorie.OrderBy(Function(p) p.Key)
            Journaliser($"{kv.Key} = {kv.Value.ToString(inv)}")
        Next
    End Sub

    Private Sub Journaliser(ligne As String)
        Try
            File.AppendAllText(_journal, ligne & Environment.NewLine)
        Catch
        End Try
    End Sub

    Private Sub FermerSiAutotest()
        If Not _autotest Then Return
        Dim t As New System.Windows.Forms.Timer() With {.Interval = 300}
        AddHandler t.Tick, Sub()
                               t.Stop()
                               Close()
                           End Sub
        t.Start()
    End Sub

End Class
