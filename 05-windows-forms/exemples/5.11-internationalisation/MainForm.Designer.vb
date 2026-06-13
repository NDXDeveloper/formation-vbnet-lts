' ============================================================================
'  Section 5.11 : Internationalisation (i18n/l10n, ressources .resx)
'  Description : Partie Concepteur : libellés affichant le message localisé,
'                la culture courante et un exemple de formatage.
'  Fichier source : 11-internationalisation.md
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
        Me.lblBienvenue = New System.Windows.Forms.Label()
        Me.lblCulture = New System.Windows.Forms.Label()
        Me.lblPrix = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        Me.lblBienvenue.Font = New System.Drawing.Font("Segoe UI", 13.0!)
        Me.lblBienvenue.Location = New System.Drawing.Point(20, 25)
        Me.lblBienvenue.Size = New System.Drawing.Size(460, 32)
        Me.lblBienvenue.Text = "(message localisé)"
        '
        Me.lblCulture.Location = New System.Drawing.Point(20, 70)
        Me.lblCulture.Size = New System.Drawing.Size(460, 25)
        '
        Me.lblPrix.Location = New System.Drawing.Point(20, 100)
        Me.lblPrix.Size = New System.Drawing.Size(460, 25)
        '
        Me.ClientSize = New System.Drawing.Size(500, 145)
        Me.Controls.Add(Me.lblBienvenue)
        Me.Controls.Add(Me.lblCulture)
        Me.Controls.Add(Me.lblPrix)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.11 — Internationalisation"
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents lblBienvenue As System.Windows.Forms.Label
    Friend WithEvents lblCulture As System.Windows.Forms.Label
    Friend WithEvents lblPrix As System.Windows.Forms.Label

End Class
