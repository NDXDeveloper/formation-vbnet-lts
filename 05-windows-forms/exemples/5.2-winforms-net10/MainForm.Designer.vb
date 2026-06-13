' ============================================================================
'  Section 5.2 : Windows Forms sur .NET 10 (modernisation)
'  Description : Partie Concepteur du formulaire de démonstration .NET 10.
'  Fichier source : 02-winforms-net10.md
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
        Me.lblColorMode = New System.Windows.Forms.Label()
        Me.btnModifier = New System.Windows.Forms.Button()
        Me.lblResultatDialogue = New System.Windows.Forms.Label()
        Me.lblPressePapiers = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        ' lblColorMode
        '
        Me.lblColorMode.Location = New System.Drawing.Point(20, 20)
        Me.lblColorMode.Size = New System.Drawing.Size(520, 50)
        Me.lblColorMode.Text = "Mode de couleur"
        '
        ' btnModifier
        '
        Me.btnModifier.Location = New System.Drawing.Point(20, 90)
        Me.btnModifier.Size = New System.Drawing.Size(220, 32)
        Me.btnModifier.Text = "Modifier (ShowDialogAsync)"
        '
        ' lblResultatDialogue
        '
        Me.lblResultatDialogue.Location = New System.Drawing.Point(20, 135)
        Me.lblResultatDialogue.Size = New System.Drawing.Size(520, 25)
        '
        ' lblPressePapiers
        '
        Me.lblPressePapiers.Location = New System.Drawing.Point(20, 170)
        Me.lblPressePapiers.Size = New System.Drawing.Size(520, 25)
        '
        ' MainForm
        '
        Me.ClientSize = New System.Drawing.Size(560, 210)
        Me.Controls.Add(Me.lblColorMode)
        Me.Controls.Add(Me.btnModifier)
        Me.Controls.Add(Me.lblResultatDialogue)
        Me.Controls.Add(Me.lblPressePapiers)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.2 — Windows Forms sur .NET 10"
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents lblColorMode As System.Windows.Forms.Label
    Friend WithEvents btnModifier As System.Windows.Forms.Button
    Friend WithEvents lblResultatDialogue As System.Windows.Forms.Label
    Friend WithEvents lblPressePapiers As System.Windows.Forms.Label

End Class
