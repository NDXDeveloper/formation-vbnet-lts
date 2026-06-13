' ============================================================================
'  Section 5.4 : Contrôles avancés
'  Description : Partie Concepteur : MenuStrip + ToolStrip + StatusStrip
'                (famille ToolStrip), et un TabControl à trois onglets
'                hébergeant DataGridView, TreeView et ListView.
'  Fichier source : 04-controles-avances.md
' ============================================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.menuPrincipal = New System.Windows.Forms.MenuStrip()
        Me.mnuFichier = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEnregistrer = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolBar = New System.Windows.Forms.ToolStrip()
        Me.tsbActualiser = New System.Windows.Forms.ToolStripButton()
        Me.barreEtat = New System.Windows.Forms.StatusStrip()
        Me.lblStatut = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pbProgression = New System.Windows.Forms.ToolStripProgressBar()
        Me.tabs = New System.Windows.Forms.TabControl()
        Me.tabGrille = New System.Windows.Forms.TabPage()
        Me.dgvClients = New System.Windows.Forms.DataGridView()
        Me.tabArbre = New System.Windows.Forms.TabPage()
        Me.tvDossiers = New System.Windows.Forms.TreeView()
        Me.tabListe = New System.Windows.Forms.TabPage()
        Me.lvClients = New System.Windows.Forms.ListView()
        Me.menuPrincipal.SuspendLayout()
        Me.toolBar.SuspendLayout()
        Me.barreEtat.SuspendLayout()
        Me.tabs.SuspendLayout()
        Me.tabGrille.SuspendLayout()
        Me.tabArbre.SuspendLayout()
        Me.tabListe.SuspendLayout()
        CType(Me.dgvClients, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' menuPrincipal
        '
        Me.menuPrincipal.Items.Add(Me.mnuFichier)
        Me.menuPrincipal.Location = New System.Drawing.Point(0, 0)
        Me.menuPrincipal.Name = "menuPrincipal"
        Me.menuPrincipal.Size = New System.Drawing.Size(560, 24)
        Me.mnuFichier.Text = "&Fichier"
        Me.mnuFichier.DropDownItems.Add(Me.mnuEnregistrer)
        Me.mnuEnregistrer.Text = "&Enregistrer"
        '
        ' toolBar
        '
        Me.toolBar.Location = New System.Drawing.Point(0, 24)
        Me.toolBar.Items.Add(Me.tsbActualiser)
        Me.toolBar.Size = New System.Drawing.Size(560, 25)
        Me.tsbActualiser.Text = "Actualiser"
        Me.tsbActualiser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        '
        ' barreEtat
        '
        Me.barreEtat.Location = New System.Drawing.Point(0, 366)
        Me.barreEtat.Items.Add(Me.lblStatut)
        Me.barreEtat.Items.Add(Me.pbProgression)
        Me.barreEtat.Size = New System.Drawing.Size(560, 22)
        Me.lblStatut.Spring = True
        Me.lblStatut.Text = ""
        Me.lblStatut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.pbProgression.Size = New System.Drawing.Size(100, 16)
        '
        ' tabs
        '
        Me.tabs.Location = New System.Drawing.Point(0, 52)
        Me.tabs.Size = New System.Drawing.Size(560, 311)
        Me.tabs.Controls.Add(Me.tabGrille)
        Me.tabs.Controls.Add(Me.tabArbre)
        Me.tabs.Controls.Add(Me.tabListe)
        Me.tabGrille.Text = "DataGridView"
        Me.tabGrille.Controls.Add(Me.dgvClients)
        Me.tabArbre.Text = "TreeView"
        Me.tabArbre.Controls.Add(Me.tvDossiers)
        Me.tabListe.Text = "ListView"
        Me.tabListe.Controls.Add(Me.lvClients)
        '
        ' dgvClients / tvDossiers / lvClients
        '
        Me.dgvClients.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvClients.Name = "dgvClients"
        Me.tvDossiers.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvDossiers.Name = "tvDossiers"
        Me.lvClients.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvClients.Name = "lvClients"
        '
        ' MainForm
        '
        Me.ClientSize = New System.Drawing.Size(560, 388)
        Me.Controls.Add(Me.tabs)
        Me.Controls.Add(Me.toolBar)
        Me.Controls.Add(Me.barreEtat)
        Me.Controls.Add(Me.menuPrincipal)
        Me.MainMenuStrip = Me.menuPrincipal
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.4 — Contrôles avancés"
        Me.menuPrincipal.ResumeLayout(False)
        Me.toolBar.ResumeLayout(False)
        Me.barreEtat.ResumeLayout(False)
        Me.tabs.ResumeLayout(False)
        Me.tabGrille.ResumeLayout(False)
        Me.tabArbre.ResumeLayout(False)
        Me.tabListe.ResumeLayout(False)
        CType(Me.dgvClients, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents menuPrincipal As System.Windows.Forms.MenuStrip
    Friend WithEvents mnuFichier As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEnregistrer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolBar As System.Windows.Forms.ToolStrip
    Friend WithEvents tsbActualiser As System.Windows.Forms.ToolStripButton
    Friend WithEvents barreEtat As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatut As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pbProgression As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents tabs As System.Windows.Forms.TabControl
    Friend WithEvents tabGrille As System.Windows.Forms.TabPage
    Friend WithEvents dgvClients As System.Windows.Forms.DataGridView
    Friend WithEvents tabArbre As System.Windows.Forms.TabPage
    Friend WithEvents tvDossiers As System.Windows.Forms.TreeView
    Friend WithEvents tabListe As System.Windows.Forms.TabPage
    Friend WithEvents lvClients As System.Windows.Forms.ListView

End Class
