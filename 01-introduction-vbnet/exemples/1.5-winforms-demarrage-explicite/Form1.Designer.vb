' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Partie « Concepteur » de Form1 (classe partielle, générée
'                par le Designer dans Visual Studio — « on ne l'édite pas à
'                la main » ; elle est écrite ici dans le style généré, pour
'                que le projet soit complet hors de Visual Studio).
'                Le bouton est déclaré « Friend WithEvents » : c'est ce qui
'                permet la clause Handles de Form1.vb.
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

    Friend WithEvents Button1 As System.Windows.Forms.Button

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Button1 = New System.Windows.Forms.Button()
        SuspendLayout()
        '
        'Button1
        '
        Button1.Location = New System.Drawing.Point(330, 195)
        Button1.Name = "Button1"
        Button1.Size = New System.Drawing.Size(140, 60)
        Button1.TabIndex = 0
        Button1.Text = "Button1"
        Button1.UseVisualStyleBackColor = True
        '
        'Form1
        '
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        ClientSize = New System.Drawing.Size(800, 450)
        Controls.Add(Button1)
        Name = "Form1"
        Text = "Form1"
        ResumeLayout(False)
    End Sub

End Class
