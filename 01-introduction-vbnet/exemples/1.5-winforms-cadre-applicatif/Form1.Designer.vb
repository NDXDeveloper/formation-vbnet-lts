' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Partie « Concepteur » de Form1 (classe partielle) pour la
'                variante « cadre applicatif » : un Label et un Button.
'  Fichier source : 05-premier-projet.md
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

    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Label1 = New System.Windows.Forms.Label()
        Button1 = New System.Windows.Forms.Button()
        SuspendLayout()
        '
        'Label1
        '
        Label1.Location = New System.Drawing.Point(20, 20)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(440, 40)
        Label1.TabIndex = 0
        Label1.Text = "Démarrage par le cadre applicatif (Application Framework) : " &
            "pas de Sub Main dans ce projet — voir My Project\Application.Designer.vb."
        '
        'Button1
        '
        Button1.Location = New System.Drawing.Point(170, 80)
        Button1.Name = "Button1"
        Button1.Size = New System.Drawing.Size(140, 40)
        Button1.TabIndex = 1
        Button1.Text = "Dire bonjour"
        Button1.UseVisualStyleBackColor = True
        '
        'Form1
        '
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        ClientSize = New System.Drawing.Size(480, 150)
        Controls.Add(Label1)
        Controls.Add(Button1)
        Name = "Form1"
        Text = "Cadre applicatif VB"
        ResumeLayout(False)
    End Sub

End Class
