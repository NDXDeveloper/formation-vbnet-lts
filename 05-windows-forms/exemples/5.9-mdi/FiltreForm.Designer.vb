' ============================================================================
'  Section 5.9 : Applications MDI et multi-formulaires
'  Description : Partie Concepteur de la fenêtre de filtre non modale.
'  Fichier source : 09-mdi.md
' ============================================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FiltreForm
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
        Me.txtTerme = New System.Windows.Forms.TextBox()
        Me.btnAppliquer = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        Me.txtTerme.Location = New System.Drawing.Point(12, 15)
        Me.txtTerme.Size = New System.Drawing.Size(180, 23)
        Me.txtTerme.PlaceholderText = "Terme du filtre…"
        '
        Me.btnAppliquer.Location = New System.Drawing.Point(200, 14)
        Me.btnAppliquer.Size = New System.Drawing.Size(90, 25)
        Me.btnAppliquer.Text = "Appliquer"
        '
        Me.ClientSize = New System.Drawing.Size(304, 55)
        Me.Controls.Add(Me.txtTerme)
        Me.Controls.Add(Me.btnAppliquer)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Text = "Filtre"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents txtTerme As System.Windows.Forms.TextBox
    Friend WithEvents btnAppliquer As System.Windows.Forms.Button

End Class
