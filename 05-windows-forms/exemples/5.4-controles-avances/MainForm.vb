' ============================================================================
'  Section 5.4 : Contrôles avancés
'  Description : Démonstration des contrôles riches de la section, regroupés
'                en onglets :
'                  · DataGridView lié à une BindingList(Of Client)
'                    (SelectionMode, AllowUserToAddRows, DataError géré) ;
'                  · TreeView hiérarchique (Nodes, Tag = objet métier,
'                    AfterSelect) ;
'                  · ListView en mode Details (Columns, SubItems, Tag) ;
'                  · MenuStrip (ShortcutKeys Ctrl+S), ToolStrip et StatusStrip.
'  Fichier source : 04-controles-avances.md
' ============================================================================

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms

Public Class MainForm

    Private ReadOnly _clients As New List(Of Client) From {
        New Client With {.Nom = "Dupont", .Ville = "Rouen", .Actif = True},
        New Client With {.Nom = "Martin", .Ville = "Lyon", .Actif = False},
        New Client With {.Nom = "Durand", .Ville = "Paris", .Actif = True}
    }

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ChargerGrille()
        ChargerArbre()
        ChargerListe()
        mnuEnregistrer.ShortcutKeys = Keys.Control Or Keys.S
        lblStatut.Text = "Prêt."
        AutoFermeture.Activer(Me)
    End Sub

    ' ---- DataGridView ----
    Private Sub ChargerGrille()
        dgvClients.AutoGenerateColumns = True
        dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvClients.AllowUserToAddRows = False
        dgvClients.DataSource = New BindingList(Of Client)(_clients)
    End Sub

    Private Sub dgvClients_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) _
            Handles dgvClients.DataError
        MessageBox.Show($"Saisie invalide : {e.Exception.Message}", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
        e.ThrowException = False
    End Sub

    ' ---- TreeView ----
    Private Sub ChargerArbre()
        Dim projet As New Projet With {.Nom = "Projet Démo"}
        projet.Dossiers.Add(New Dossier With {.Nom = "Sources"})
        projet.Dossiers.Add(New Dossier With {.Nom = "Ressources"})
        projet.Dossiers.Add(New Dossier With {.Nom = "Tests"})

        tvDossiers.Nodes.Clear()
        Dim racine As TreeNode = tvDossiers.Nodes.Add(projet.Nom)
        racine.Tag = projet
        For Each dossier In projet.Dossiers
            Dim noeud As TreeNode = racine.Nodes.Add(dossier.Nom)
            noeud.Tag = dossier
        Next
        racine.Expand()
    End Sub

    Private Sub tvDossiers_AfterSelect(sender As Object, e As TreeViewEventArgs) _
            Handles tvDossiers.AfterSelect
        lblStatut.Text = $"Sélection : {e.Node.Text} (Tag = {e.Node.Tag?.GetType().Name})"
    End Sub

    ' ---- ListView ----
    Private Sub ChargerListe()
        lvClients.View = View.Details
        lvClients.FullRowSelect = True
        lvClients.GridLines = True
        lvClients.Columns.Clear()
        lvClients.Columns.Add("Nom", 150)
        lvClients.Columns.Add("Ville", 120)

        lvClients.Items.Clear()
        For Each c In _clients
            Dim item As New ListViewItem(c.Nom)
            item.SubItems.Add(c.Ville)
            item.Tag = c
            lvClients.Items.Add(item)
        Next
    End Sub

    ' ---- MenuStrip / ToolStrip ----
    Private Sub mnuEnregistrer_Click(sender As Object, e As EventArgs) Handles mnuEnregistrer.Click
        lblStatut.Text = "Enregistrer (Ctrl+S)"
        pbProgression.Value = 50
    End Sub

    Private Sub tsbActualiser_Click(sender As Object, e As EventArgs) Handles tsbActualiser.Click
        ChargerGrille()
        lblStatut.Text = "Données actualisées."
    End Sub

End Class
