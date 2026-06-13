' ============================================================================
'  Section 5.9 : Applications MDI et multi-formulaires
'  Description : Partie Concepteur du conteneur MDI : IsMdiContainer=True,
'                un MenuStrip (Fichier > Nouveau ; Fenêtre > Cascade /
'                Mosaïque, avec MdiWindowListItem pour lister les fenêtres),
'                et une barre d'état.
'  Fichier source : 09-mdi.md
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
        Me.mnuNouveau = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFiltrer = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFenetre = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCascade = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuMosaiqueH = New System.Windows.Forms.ToolStripMenuItem()
        Me.barreEtat = New System.Windows.Forms.StatusStrip()
        Me.lblStatut = New System.Windows.Forms.ToolStripStatusLabel()
        Me.menuPrincipal.SuspendLayout()
        Me.barreEtat.SuspendLayout()
        Me.SuspendLayout()
        '
        Me.menuPrincipal.Items.Add(Me.mnuFichier)
        Me.menuPrincipal.Items.Add(Me.mnuFenetre)
        Me.menuPrincipal.Location = New System.Drawing.Point(0, 0)
        Me.menuPrincipal.Size = New System.Drawing.Size(640, 24)
        Me.menuPrincipal.MdiWindowListItem = Me.mnuFenetre   ' liste auto des fenêtres
        '
        Me.mnuFichier.Text = "&Fichier"
        Me.mnuFichier.DropDownItems.Add(Me.mnuNouveau)
        Me.mnuFichier.DropDownItems.Add(Me.mnuFiltrer)
        Me.mnuNouveau.Text = "&Nouveau document"
        Me.mnuFiltrer.Text = "&Filtrer…"
        '
        Me.mnuFenetre.Text = "Fe&nêtre"
        Me.mnuFenetre.DropDownItems.Add(Me.mnuCascade)
        Me.mnuFenetre.DropDownItems.Add(Me.mnuMosaiqueH)
        Me.mnuCascade.Text = "&Cascade"
        Me.mnuMosaiqueH.Text = "&Mosaïque horizontale"
        '
        Me.barreEtat.Location = New System.Drawing.Point(0, 418)
        Me.barreEtat.Items.Add(Me.lblStatut)
        Me.barreEtat.Size = New System.Drawing.Size(640, 22)
        Me.lblStatut.Text = "Conteneur MDI prêt."
        '
        Me.ClientSize = New System.Drawing.Size(640, 440)
        Me.Controls.Add(Me.barreEtat)
        Me.Controls.Add(Me.menuPrincipal)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.menuPrincipal
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.9 — Application MDI"
        Me.menuPrincipal.ResumeLayout(False)
        Me.barreEtat.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents menuPrincipal As System.Windows.Forms.MenuStrip
    Friend WithEvents mnuFichier As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuNouveau As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFiltrer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFenetre As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCascade As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuMosaiqueH As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents barreEtat As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatut As System.Windows.Forms.ToolStripStatusLabel

End Class
