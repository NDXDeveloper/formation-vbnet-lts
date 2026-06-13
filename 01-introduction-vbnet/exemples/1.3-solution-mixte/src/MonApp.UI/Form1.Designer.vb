' ============================================================================
'  Section 1.3 : L'écosystème .NET — Structure d'une solution (solution mixte)
'  Description : Partie « Concepteur » de Form1 (classe partielle). Dans
'                Visual Studio, ce fichier est généré par le Designer ;
'                il est écrit ici à la main, dans le même style.
'  Fichier source : 03-ecosysteme-dotnet.md
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

    Friend WithEvents lstLignes As System.Windows.Forms.ListBox
    Friend WithEvents lblTotaux As System.Windows.Forms.Label
    Friend WithEvents btnFermer As System.Windows.Forms.Button

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        lstLignes = New System.Windows.Forms.ListBox()
        lblTotaux = New System.Windows.Forms.Label()
        btnFermer = New System.Windows.Forms.Button()
        SuspendLayout()
        '
        'lstLignes
        '
        lstLignes.ItemHeight = 15
        lstLignes.Location = New System.Drawing.Point(12, 12)
        lstLignes.Name = "lstLignes"
        lstLignes.Size = New System.Drawing.Size(460, 124)
        lstLignes.TabIndex = 0
        '
        'lblTotaux
        '
        lblTotaux.Location = New System.Drawing.Point(12, 150)
        lblTotaux.Name = "lblTotaux"
        lblTotaux.Size = New System.Drawing.Size(330, 70)
        lblTotaux.TabIndex = 1
        lblTotaux.Text = "Totaux"
        '
        'btnFermer
        '
        btnFermer.Location = New System.Drawing.Point(372, 190)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New System.Drawing.Size(100, 30)
        btnFermer.TabIndex = 2
        btnFermer.Text = "Fermer"
        '
        'Form1
        '
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        ClientSize = New System.Drawing.Size(484, 232)
        Controls.Add(lstLignes)
        Controls.Add(lblTotaux)
        Controls.Add(btnFermer)
        Name = "Form1"
        Text = "MaSolution — Facture de démonstration"
        ResumeLayout(False)
    End Sub

End Class
