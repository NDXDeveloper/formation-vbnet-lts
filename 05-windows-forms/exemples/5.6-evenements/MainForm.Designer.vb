' ============================================================================
'  Section 5.6 : Gestion des événements (souris, clavier, cycle de vie)
'  Description : Partie Concepteur : un panneau cliquable (événements souris),
'                trois boutons de couleur partageant un gestionnaire, et des
'                libellés d'état (heure, souris, clavier, cycle de vie).
'  Fichier source : 06-evenements.md
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
        Me.lblHeure = New System.Windows.Forms.Label()
        Me.panneau = New System.Windows.Forms.Panel()
        Me.lblSouris = New System.Windows.Forms.Label()
        Me.lblClavier = New System.Windows.Forms.Label()
        Me.lblCycle = New System.Windows.Forms.Label()
        Me.btnRouge = New System.Windows.Forms.Button()
        Me.btnVert = New System.Windows.Forms.Button()
        Me.btnBleu = New System.Windows.Forms.Button()
        Me.panneau.SuspendLayout()
        Me.SuspendLayout()
        '
        Me.lblHeure.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.lblHeure.Location = New System.Drawing.Point(20, 15)
        Me.lblHeure.Size = New System.Drawing.Size(200, 30)
        Me.lblHeure.Text = "--:--:--"
        '
        Me.panneau.BackColor = System.Drawing.SystemColors.ControlLight
        Me.panneau.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panneau.Location = New System.Drawing.Point(20, 55)
        Me.panneau.Size = New System.Drawing.Size(440, 100)
        Me.panneau.Controls.Add(Me.lblSouris)
        '
        Me.lblSouris.Location = New System.Drawing.Point(10, 10)
        Me.lblSouris.Size = New System.Drawing.Size(420, 23)
        Me.lblSouris.Text = "Cliquez ici (gauche / droit)…"
        '
        Me.btnRouge.BackColor = System.Drawing.Color.IndianRed
        Me.btnRouge.Location = New System.Drawing.Point(20, 165)
        Me.btnRouge.Size = New System.Drawing.Size(90, 30)
        Me.btnRouge.Text = "Rouge"
        '
        Me.btnVert.BackColor = System.Drawing.Color.MediumSeaGreen
        Me.btnVert.Location = New System.Drawing.Point(120, 165)
        Me.btnVert.Size = New System.Drawing.Size(90, 30)
        Me.btnVert.Text = "Vert"
        '
        Me.btnBleu.BackColor = System.Drawing.Color.CornflowerBlue
        Me.btnBleu.Location = New System.Drawing.Point(220, 165)
        Me.btnBleu.Size = New System.Drawing.Size(90, 30)
        Me.btnBleu.Text = "Bleu"
        '
        Me.lblClavier.Location = New System.Drawing.Point(20, 205)
        Me.lblClavier.Size = New System.Drawing.Size(440, 23)
        Me.lblClavier.Text = "Appuyez sur Ctrl+S…"
        '
        Me.lblCycle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCycle.Location = New System.Drawing.Point(20, 235)
        Me.lblCycle.Size = New System.Drawing.Size(440, 25)
        '
        Me.ClientSize = New System.Drawing.Size(480, 275)
        Me.Controls.Add(Me.lblHeure)
        Me.Controls.Add(Me.panneau)
        Me.Controls.Add(Me.btnRouge)
        Me.Controls.Add(Me.btnVert)
        Me.Controls.Add(Me.btnBleu)
        Me.Controls.Add(Me.lblClavier)
        Me.Controls.Add(Me.lblCycle)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.6 — Gestion des événements"
        Me.panneau.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents lblHeure As System.Windows.Forms.Label
    Friend WithEvents panneau As System.Windows.Forms.Panel
    Friend WithEvents lblSouris As System.Windows.Forms.Label
    Friend WithEvents lblClavier As System.Windows.Forms.Label
    Friend WithEvents lblCycle As System.Windows.Forms.Label
    Friend WithEvents btnRouge As System.Windows.Forms.Button
    Friend WithEvents btnVert As System.Windows.Forms.Button
    Friend WithEvents btnBleu As System.Windows.Forms.Button

End Class
