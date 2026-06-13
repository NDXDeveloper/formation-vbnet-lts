' ============================================================================
'  Section 5.10 : Préférences et paramètres utilisateur (My.Settings)
'  Description : Partie Concepteur : un compteur de lancements, le dernier
'                dossier mémorisé, une case « mode sombre » et un bouton pour
'                choisir un dossier.
'  Fichier source : 10-preferences.md
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
        Me.lblCompteur = New System.Windows.Forms.Label()
        Me.lblDossier = New System.Windows.Forms.Label()
        Me.chkModeSombre = New System.Windows.Forms.CheckBox()
        Me.btnChoisirDossier = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        Me.lblCompteur.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.lblCompteur.Location = New System.Drawing.Point(20, 20)
        Me.lblCompteur.Size = New System.Drawing.Size(420, 28)
        Me.lblCompteur.Text = "Nombre de lancements : ?"
        '
        Me.lblDossier.Location = New System.Drawing.Point(20, 60)
        Me.lblDossier.Size = New System.Drawing.Size(420, 40)
        Me.lblDossier.Text = "Dernier dossier : (aucun)"
        '
        Me.chkModeSombre.Location = New System.Drawing.Point(20, 105)
        Me.chkModeSombre.Size = New System.Drawing.Size(200, 24)
        Me.chkModeSombre.Text = "Préférence : mode sombre"
        '
        Me.btnChoisirDossier.Location = New System.Drawing.Point(20, 140)
        Me.btnChoisirDossier.Size = New System.Drawing.Size(220, 30)
        Me.btnChoisirDossier.Text = "Choisir un dossier (mémorisé)"
        '
        Me.ClientSize = New System.Drawing.Size(460, 190)
        Me.Controls.Add(Me.lblCompteur)
        Me.Controls.Add(Me.lblDossier)
        Me.Controls.Add(Me.chkModeSombre)
        Me.Controls.Add(Me.btnChoisirDossier)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.10 — Préférences (My.Settings)"
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents lblCompteur As System.Windows.Forms.Label
    Friend WithEvents lblDossier As System.Windows.Forms.Label
    Friend WithEvents chkModeSombre As System.Windows.Forms.CheckBox
    Friend WithEvents btnChoisirDossier As System.Windows.Forms.Button

End Class
