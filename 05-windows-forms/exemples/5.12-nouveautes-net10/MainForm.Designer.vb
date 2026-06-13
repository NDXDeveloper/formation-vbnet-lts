' ============================================================================
'  Section 5.12 : Nouveautés Windows Forms .NET 10
'  Description : Partie Concepteur : un panneau « cible de dépôt » (AllowDrop
'                = True) pour le glisser-déposer typé, et des libellés d'état
'                (presse-papiers, fichiers déposés).
'  Fichier source : 12-nouveautes-net10.md
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
        Me.lblPressePapiers = New System.Windows.Forms.Label()
        Me.panneau = New System.Windows.Forms.Panel()
        Me.lblConsigne = New System.Windows.Forms.Label()
        Me.lblDepot = New System.Windows.Forms.Label()
        Me.panneau.SuspendLayout()
        Me.SuspendLayout()
        '
        Me.lblPressePapiers.Location = New System.Drawing.Point(20, 20)
        Me.lblPressePapiers.Size = New System.Drawing.Size(450, 25)
        Me.lblPressePapiers.Text = "Presse-papiers JSON…"
        '
        Me.panneau.AllowDrop = True
        Me.panneau.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panneau.BackColor = System.Drawing.SystemColors.ControlLight
        Me.panneau.Location = New System.Drawing.Point(20, 55)
        Me.panneau.Size = New System.Drawing.Size(450, 90)
        Me.panneau.Controls.Add(Me.lblConsigne)
        '
        Me.lblConsigne.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblConsigne.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblConsigne.Text = "Glissez-déposez des fichiers ici (ITypedDataObject)"
        '
        Me.lblDepot.Location = New System.Drawing.Point(20, 155)
        Me.lblDepot.Size = New System.Drawing.Size(450, 40)
        '
        Me.ClientSize = New System.Drawing.Size(490, 205)
        Me.Controls.Add(Me.lblPressePapiers)
        Me.Controls.Add(Me.panneau)
        Me.Controls.Add(Me.lblDepot)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.12 — Nouveautés .NET 10"
        Me.panneau.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents lblPressePapiers As System.Windows.Forms.Label
    Friend WithEvents panneau As System.Windows.Forms.Panel
    Friend WithEvents lblConsigne As System.Windows.Forms.Label
    Friend WithEvents lblDepot As System.Windows.Forms.Label

End Class
