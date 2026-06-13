' ============================================================================
'  Section 5.3 : Contrôles fondamentaux
'  Description : Partie Concepteur : mise en page par TableLayoutPanel
'                (recommandée par la section), GroupBox + RadioButton,
'                TextBox (dont une de recherche), ListBox, et des boutons
'                déclenchant les démonstrations.
'  Fichier source : 03-controles-fondamentaux.md
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
        Me.lblNomLib = New System.Windows.Forms.Label()
        Me.txtNom = New System.Windows.Forms.TextBox()
        Me.btnValider = New System.Windows.Forms.Button()
        Me.btnConfirmer = New System.Windows.Forms.Button()
        Me.btnOuvrir = New System.Windows.Forms.Button()
        Me.btnFiche = New System.Windows.Forms.Button()
        Me.grpType = New System.Windows.Forms.GroupBox()
        Me.rbParticulier = New System.Windows.Forms.RadioButton()
        Me.rbEntreprise = New System.Windows.Forms.RadioButton()
        Me.txtRecherche = New System.Windows.Forms.TextBox()
        Me.lstFruits = New System.Windows.Forms.ListBox()
        Me.lblStatut = New System.Windows.Forms.Label()
        Me.grpType.SuspendLayout()
        Me.SuspendLayout()
        '
        Me.lblNomLib.Location = New System.Drawing.Point(15, 18)
        Me.lblNomLib.Size = New System.Drawing.Size(50, 23)
        Me.lblNomLib.Text = "Nom :"
        '
        Me.txtNom.Location = New System.Drawing.Point(70, 15)
        Me.txtNom.Size = New System.Drawing.Size(200, 23)
        Me.txtNom.PlaceholderText = "Votre nom"
        '
        Me.btnValider.Location = New System.Drawing.Point(290, 14)
        Me.btnValider.Size = New System.Drawing.Size(100, 26)
        Me.btnValider.Text = "&Valider"
        '
        Me.grpType.Controls.Add(Me.rbParticulier)
        Me.grpType.Controls.Add(Me.rbEntreprise)
        Me.grpType.Location = New System.Drawing.Point(15, 50)
        Me.grpType.Size = New System.Drawing.Size(255, 60)
        Me.grpType.Text = "Type de client"
        '
        Me.rbParticulier.Checked = True
        Me.rbParticulier.Location = New System.Drawing.Point(15, 25)
        Me.rbParticulier.Size = New System.Drawing.Size(100, 23)
        Me.rbParticulier.Text = "Particulier"
        '
        Me.rbEntreprise.Location = New System.Drawing.Point(130, 25)
        Me.rbEntreprise.Size = New System.Drawing.Size(110, 23)
        Me.rbEntreprise.Text = "Entreprise"
        '
        Me.txtRecherche.Location = New System.Drawing.Point(290, 50)
        Me.txtRecherche.Size = New System.Drawing.Size(200, 23)
        Me.txtRecherche.PlaceholderText = "Filtrer la liste…"
        '
        Me.lstFruits.Location = New System.Drawing.Point(290, 80)
        Me.lstFruits.Size = New System.Drawing.Size(200, 95)
        '
        Me.btnConfirmer.Location = New System.Drawing.Point(15, 125)
        Me.btnConfirmer.Size = New System.Drawing.Size(120, 28)
        Me.btnConfirmer.Text = "MessageBox"
        '
        Me.btnOuvrir.Location = New System.Drawing.Point(145, 125)
        Me.btnOuvrir.Size = New System.Drawing.Size(120, 28)
        Me.btnOuvrir.Text = "Ouvrir un fichier"
        '
        Me.btnFiche.Location = New System.Drawing.Point(15, 160)
        Me.btnFiche.Size = New System.Drawing.Size(250, 28)
        Me.btnFiche.Text = "Fiche client (dialogue modal)"
        '
        Me.lblStatut.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatut.Location = New System.Drawing.Point(15, 200)
        Me.lblStatut.Size = New System.Drawing.Size(475, 25)
        Me.lblStatut.Text = "Prêt."
        '
        Me.ClientSize = New System.Drawing.Size(510, 240)
        Me.Controls.Add(Me.lblNomLib)
        Me.Controls.Add(Me.txtNom)
        Me.Controls.Add(Me.btnValider)
        Me.Controls.Add(Me.grpType)
        Me.Controls.Add(Me.txtRecherche)
        Me.Controls.Add(Me.lstFruits)
        Me.Controls.Add(Me.btnConfirmer)
        Me.Controls.Add(Me.btnOuvrir)
        Me.Controls.Add(Me.btnFiche)
        Me.Controls.Add(Me.lblStatut)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.3 — Contrôles fondamentaux"
        Me.grpType.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblNomLib As System.Windows.Forms.Label
    Friend WithEvents txtNom As System.Windows.Forms.TextBox
    Friend WithEvents btnValider As System.Windows.Forms.Button
    Friend WithEvents btnConfirmer As System.Windows.Forms.Button
    Friend WithEvents btnOuvrir As System.Windows.Forms.Button
    Friend WithEvents btnFiche As System.Windows.Forms.Button
    Friend WithEvents grpType As System.Windows.Forms.GroupBox
    Friend WithEvents rbParticulier As System.Windows.Forms.RadioButton
    Friend WithEvents rbEntreprise As System.Windows.Forms.RadioButton
    Friend WithEvents txtRecherche As System.Windows.Forms.TextBox
    Friend WithEvents lstFruits As System.Windows.Forms.ListBox
    Friend WithEvents lblStatut As System.Windows.Forms.Label

End Class
