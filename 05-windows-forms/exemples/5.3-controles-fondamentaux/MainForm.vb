' ============================================================================
'  Section 5.3 : Contrôles fondamentaux
'  Description : Démonstration des briques de base : TextBox (PlaceholderText,
'                TextChanged pour un filtrage en direct), Button (mnémonique
'                &Valider, Click + validation), conteneurs (TableLayoutPanel,
'                GroupBox + RadioButton), MessageBox renvoyant un DialogResult,
'                dialogue commun (OpenFileDialog) et boîte de dialogue modale
'                personnalisée (ClientEditForm).
'  Fichier source : 03-controles-fondamentaux.md
' ============================================================================

Imports System.Windows.Forms

Public Class MainForm

    Private ReadOnly _fruits As String() = {"Abricot", "Banane", "Cerise", "Datte", "Figue"}

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lstFruits.Items.AddRange(_fruits)
        AutoFermeture.Activer(Me)
    End Sub

    ' TextChanged : filtrage en direct (extrait de la section)
    Private Sub txtRecherche_TextChanged(sender As Object, e As EventArgs) Handles txtRecherche.TextChanged
        FiltrerListe(txtRecherche.Text)
    End Sub

    Private Sub FiltrerListe(terme As String)
        lstFruits.BeginUpdate()
        lstFruits.Items.Clear()
        lstFruits.Items.AddRange(
            _fruits.Where(Function(f) f.Contains(terme, StringComparison.OrdinalIgnoreCase)).ToArray())
        lstFruits.EndUpdate()
    End Sub

    ' Click + validation (extrait de la section)
    Private Sub btnValider_Click(sender As Object, e As EventArgs) Handles btnValider.Click
        If String.IsNullOrWhiteSpace(txtNom.Text) Then
            MessageBox.Show("Le nom est obligatoire.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        MessageBox.Show($"Bonjour {txtNom.Text} !", "Info")
    End Sub

    ' MessageBox renvoyant un DialogResult (extrait de la section)
    Private Sub btnConfirmer_Click(sender As Object, e As EventArgs) Handles btnConfirmer.Click
        Dim reponse As DialogResult =
            MessageBox.Show("Enregistrer les modifications avant de fermer ?",
                            "Confirmation",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
        lblStatut.Text = $"Réponse : {reponse}"
    End Sub

    ' Boîte de dialogue commune : OpenFileDialog (extrait de la section)
    Private Sub btnOuvrir_Click(sender As Object, e As EventArgs) Handles btnOuvrir.Click
        Using dlg As New OpenFileDialog()
            dlg.Title = "Choisir un fichier CSV"
            dlg.Filter = "Fichiers CSV (*.csv)|*.csv|Tous les fichiers (*.*)|*.*"
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                lblStatut.Text = $"Fichier : {dlg.FileName}"
            End If
        End Using
    End Sub

    ' Boîte de dialogue modale personnalisée (extrait de la section)
    Private Sub btnFiche_Click(sender As Object, e As EventArgs) Handles btnFiche.Click
        Using dlg As New ClientEditForm()
            dlg.Client = New Client With {.Nom = txtNom.Text, .Ville = ""}
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                Dim saisi As Client = dlg.Client
                lblStatut.Text = $"Client saisi : {saisi.Nom} ({saisi.Ville})"
            End If
        End Using
    End Sub

End Class
