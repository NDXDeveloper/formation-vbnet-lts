' ============================================================================
'  Section 5.9 : Applications MDI et multi-formulaires
'  Description : Partie Concepteur de la fenêtre document MDI (un simple
'                éditeur de texte multiligne).
'  Fichier source : 09-mdi.md
' ============================================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DocumentForm
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
        Me.txtContenu = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        Me.txtContenu.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtContenu.Multiline = True
        Me.txtContenu.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtContenu.Name = "txtContenu"
        '
        Me.ClientSize = New System.Drawing.Size(300, 200)
        Me.Controls.Add(Me.txtContenu)
        Me.Text = "Document"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents txtContenu As System.Windows.Forms.TextBox

End Class
