' ============================================================================
'  Section 5.2 : Windows Forms sur .NET 10 (modernisation)
'  Description : Partie Concepteur de la boîte de dialogue : un bouton OK
'                (DialogResult.OK) et un bouton Annuler (DialogResult.Cancel),
'                avec AcceptButton/CancelButton.
'  Fichier source : 02-winforms-net10.md
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
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnAnnuler = New System.Windows.Forms.Button()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        ' lblMessage
        '
        Me.lblMessage.Location = New System.Drawing.Point(20, 20)
        Me.lblMessage.Size = New System.Drawing.Size(300, 40)
        Me.lblMessage.Text = "Boîte de dialogue ouverte par ShowDialogAsync."
        '
        ' btnOk
        '
        Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOk.Location = New System.Drawing.Point(120, 80)
        Me.btnOk.Size = New System.Drawing.Size(90, 30)
        Me.btnOk.Text = "OK"
        '
        ' btnAnnuler
        '
        Me.btnAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnAnnuler.Location = New System.Drawing.Point(220, 80)
        Me.btnAnnuler.Size = New System.Drawing.Size(90, 30)
        Me.btnAnnuler.Text = "Annuler"
        '
        ' ClientEditForm
        '
        Me.AcceptButton = Me.btnOk
        Me.CancelButton = Me.btnAnnuler
        Me.ClientSize = New System.Drawing.Size(340, 130)
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.btnAnnuler)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Modifier le client"
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents btnAnnuler As System.Windows.Forms.Button
    Friend WithEvents lblMessage As System.Windows.Forms.Label

End Class
