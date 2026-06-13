' ============================================================================
'  Section 5.8 : Liaison de données WinForms
'  Description : Partie Concepteur : BindingNavigator en haut, DataGridView
'                des clients + champs liés (Nom, Email), et DataGridView des
'                commandes (maître-détail) en bas.
'  Fichier source : 08-data-binding.md
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
        Me.components = New System.ComponentModel.Container()
        Me.navClients = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.dgvClients = New System.Windows.Forms.DataGridView()
        Me.lblNom = New System.Windows.Forms.Label()
        Me.txtNom = New System.Windows.Forms.TextBox()
        Me.lblEmail = New System.Windows.Forms.Label()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.lblCommandes = New System.Windows.Forms.Label()
        Me.dgvCommandes = New System.Windows.Forms.DataGridView()
        CType(Me.navClients, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.navClients.SuspendLayout()
        CType(Me.dgvClients, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvCommandes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        Me.navClients.Dock = System.Windows.Forms.DockStyle.Top
        Me.navClients.Location = New System.Drawing.Point(0, 0)
        Me.navClients.Size = New System.Drawing.Size(580, 25)
        '
        Me.dgvClients.Location = New System.Drawing.Point(12, 35)
        Me.dgvClients.Size = New System.Drawing.Size(556, 120)
        Me.dgvClients.Name = "dgvClients"
        '
        Me.lblNom.Location = New System.Drawing.Point(12, 165)
        Me.lblNom.Size = New System.Drawing.Size(70, 23)
        Me.lblNom.Text = "Nom :"
        Me.txtNom.Location = New System.Drawing.Point(85, 162)
        Me.txtNom.Size = New System.Drawing.Size(180, 23)
        '
        Me.lblEmail.Location = New System.Drawing.Point(285, 165)
        Me.lblEmail.Size = New System.Drawing.Size(60, 23)
        Me.lblEmail.Text = "E-mail :"
        Me.txtEmail.Location = New System.Drawing.Point(350, 162)
        Me.txtEmail.Size = New System.Drawing.Size(200, 23)
        '
        Me.lblCommandes.Location = New System.Drawing.Point(12, 195)
        Me.lblCommandes.Size = New System.Drawing.Size(300, 23)
        Me.lblCommandes.Text = "Commandes du client courant (maître-détail) :"
        '
        Me.dgvCommandes.Location = New System.Drawing.Point(12, 220)
        Me.dgvCommandes.Size = New System.Drawing.Size(556, 110)
        Me.dgvCommandes.Name = "dgvCommandes"
        '
        Me.ClientSize = New System.Drawing.Size(580, 345)
        Me.Controls.Add(Me.dgvClients)
        Me.Controls.Add(Me.lblNom)
        Me.Controls.Add(Me.txtNom)
        Me.Controls.Add(Me.lblEmail)
        Me.Controls.Add(Me.txtEmail)
        Me.Controls.Add(Me.lblCommandes)
        Me.Controls.Add(Me.dgvCommandes)
        Me.Controls.Add(Me.navClients)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.8 — Liaison de données"
        CType(Me.navClients, System.ComponentModel.ISupportInitialize).EndInit()
        Me.navClients.ResumeLayout(False)
        Me.navClients.PerformLayout()
        CType(Me.dgvClients, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvCommandes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents navClients As System.Windows.Forms.BindingNavigator
    Friend WithEvents dgvClients As System.Windows.Forms.DataGridView
    Friend WithEvents lblNom As System.Windows.Forms.Label
    Friend WithEvents txtNom As System.Windows.Forms.TextBox
    Friend WithEvents lblEmail As System.Windows.Forms.Label
    Friend WithEvents txtEmail As System.Windows.Forms.TextBox
    Friend WithEvents lblCommandes As System.Windows.Forms.Label
    Friend WithEvents dgvCommandes As System.Windows.Forms.DataGridView

End Class
