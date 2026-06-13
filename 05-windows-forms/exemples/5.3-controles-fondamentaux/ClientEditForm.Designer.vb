' ============================================================================
'  Section 5.3 : Contrôles fondamentaux
'  Description : Partie Concepteur de la boîte de dialogue : deux Label + deux
'                TextBox, un bouton OK (DialogResult.OK) et Annuler
'                (DialogResult.Cancel, CausesValidation=False), AcceptButton
'                et CancelButton.
'  Fichier source : 03-controles-fondamentaux.md
' ============================================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ClientEditForm
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
        Me.lblNom = New System.Windows.Forms.Label()
        Me.txtNom = New System.Windows.Forms.TextBox()
        Me.lblVille = New System.Windows.Forms.Label()
        Me.txtVille = New System.Windows.Forms.TextBox()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnAnnuler = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        Me.lblNom.Location = New System.Drawing.Point(20, 20)
        Me.lblNom.Size = New System.Drawing.Size(80, 23)
        Me.lblNom.Text = "Nom :"
        '
        Me.txtNom.Location = New System.Drawing.Point(110, 18)
        Me.txtNom.Size = New System.Drawing.Size(220, 23)
        Me.txtNom.PlaceholderText = "Nom du client"
        '
        Me.lblVille.Location = New System.Drawing.Point(20, 55)
        Me.lblVille.Size = New System.Drawing.Size(80, 23)
        Me.lblVille.Text = "Ville :"
        '
        Me.txtVille.Location = New System.Drawing.Point(110, 53)
        Me.txtVille.Size = New System.Drawing.Size(220, 23)
        '
        Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOk.Location = New System.Drawing.Point(140, 95)
        Me.btnOk.Size = New System.Drawing.Size(90, 30)
        Me.btnOk.Text = "OK"
        '
        Me.btnAnnuler.CausesValidation = False
        Me.btnAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnAnnuler.Location = New System.Drawing.Point(240, 95)
        Me.btnAnnuler.Size = New System.Drawing.Size(90, 30)
        Me.btnAnnuler.Text = "Annuler"
        '
        Me.AcceptButton = Me.btnOk
        Me.CancelButton = Me.btnAnnuler
        Me.ClientSize = New System.Drawing.Size(360, 145)
        Me.Controls.Add(Me.lblNom)
        Me.Controls.Add(Me.txtNom)
        Me.Controls.Add(Me.lblVille)
        Me.Controls.Add(Me.txtVille)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.btnAnnuler)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Fiche client"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblNom As System.Windows.Forms.Label
    Friend WithEvents txtNom As System.Windows.Forms.TextBox
    Friend WithEvents lblVille As System.Windows.Forms.Label
    Friend WithEvents txtVille As System.Windows.Forms.TextBox
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents btnAnnuler As System.Windows.Forms.Button

End Class
