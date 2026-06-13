' ============================================================================
'  Section 2.12 : L'espace My — un raccourci propre à VB.NET
'  Description : Partie « Concepteur » de Form1 : une zone de texte
'                multiligne en lecture seule (le rapport My) et un bouton.
'  Fichier source : 12-espace-my.md
' ============================================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    Friend WithEvents txtInfos As System.Windows.Forms.TextBox
    Friend WithEvents btnFermer As System.Windows.Forms.Button

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        txtInfos = New System.Windows.Forms.TextBox()
        btnFermer = New System.Windows.Forms.Button()
        SuspendLayout()
        '
        'txtInfos
        '
        txtInfos.Font = New System.Drawing.Font("Consolas", 9.75F)
        txtInfos.Location = New System.Drawing.Point(12, 12)
        txtInfos.Multiline = True
        txtInfos.Name = "txtInfos"
        txtInfos.ReadOnly = True
        txtInfos.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        txtInfos.Size = New System.Drawing.Size(660, 420)
        txtInfos.TabIndex = 0
        '
        'btnFermer
        '
        btnFermer.Location = New System.Drawing.Point(572, 444)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New System.Drawing.Size(100, 30)
        btnFermer.TabIndex = 1
        btnFermer.Text = "Fermer"
        btnFermer.UseVisualStyleBackColor = True
        '
        'Form1
        '
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        ClientSize = New System.Drawing.Size(684, 486)
        Controls.Add(txtInfos)
        Controls.Add(btnFermer)
        Name = "Form1"
        Text = "L'espace My en Windows Forms (.NET 10)"
        ResumeLayout(False)
        PerformLayout()
    End Sub

End Class
