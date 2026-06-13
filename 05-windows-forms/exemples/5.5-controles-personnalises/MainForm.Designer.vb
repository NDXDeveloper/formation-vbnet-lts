' ============================================================================
'  Section 5.5 : Contrôles personnalisés et UserControl
'  Description : Partie Concepteur. Les contrôles personnalisés (SearchBox,
'                NumericTextBox, LedIndicator) s'instancient ici comme tout
'                contrôle — une fois le projet compilé, ils apparaîtraient
'                dans la Boîte à outils de Visual Studio.
'  Fichier source : 05-controles-personnalises.md
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
        Me.searchBox1 = New SearchBox()
        Me.lblNumLib = New System.Windows.Forms.Label()
        Me.txtNumerique = New NumericTextBox()
        Me.led = New LedIndicator()
        Me.btnBasculer = New System.Windows.Forms.Button()
        Me.lblResultat = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        Me.searchBox1.Location = New System.Drawing.Point(20, 20)
        Me.searchBox1.Name = "searchBox1"
        '
        Me.lblNumLib.Location = New System.Drawing.Point(20, 70)
        Me.lblNumLib.Size = New System.Drawing.Size(180, 23)
        Me.lblNumLib.Text = "NumericTextBox (chiffres) :"
        '
        Me.txtNumerique.Location = New System.Drawing.Point(210, 67)
        Me.txtNumerique.Size = New System.Drawing.Size(135, 23)
        '
        Me.led.Location = New System.Drawing.Point(20, 110)
        Me.led.Size = New System.Drawing.Size(40, 40)
        Me.led.Name = "led"
        '
        Me.btnBasculer.Location = New System.Drawing.Point(70, 115)
        Me.btnBasculer.Size = New System.Drawing.Size(150, 30)
        Me.btnBasculer.Text = "Basculer la LED"
        '
        Me.lblResultat.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblResultat.Location = New System.Drawing.Point(20, 165)
        Me.lblResultat.Size = New System.Drawing.Size(360, 25)
        '
        Me.ClientSize = New System.Drawing.Size(400, 205)
        Me.Controls.Add(Me.searchBox1)
        Me.Controls.Add(Me.lblNumLib)
        Me.Controls.Add(Me.txtNumerique)
        Me.Controls.Add(Me.led)
        Me.Controls.Add(Me.btnBasculer)
        Me.Controls.Add(Me.lblResultat)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.5 — Contrôles personnalisés"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents searchBox1 As SearchBox
    Friend WithEvents lblNumLib As System.Windows.Forms.Label
    Friend WithEvents txtNumerique As NumericTextBox
    Friend WithEvents led As LedIndicator
    Friend WithEvents btnBasculer As System.Windows.Forms.Button
    Friend WithEvents lblResultat As System.Windows.Forms.Label

End Class
