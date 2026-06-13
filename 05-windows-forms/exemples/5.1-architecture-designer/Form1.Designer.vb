' ============================================================================
'  Section 5.1 : Introduction, architecture et le Concepteur (Designer)
'  Description : Le code GÉNÉRÉ par le Concepteur, repris à l'identique de la
'                section : déclaration des contrôles, InitializeComponent, et
'                surtout « Friend WithEvents btnValider » qui autorise la
'                clause Handles côté Form1.vb. (Dans Visual Studio, ce fichier
'                est produit et maintenu par le Concepteur.)
'  Fichier source : 01-introduction-designer.md
' ============================================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    ' Méthode requise par le Concepteur — ne pas modifier à la main
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.btnValider = New System.Windows.Forms.Button()
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        ' btnValider
        '
        Me.btnValider.Location = New System.Drawing.Point(30, 110)
        Me.btnValider.Name = "btnValider"
        Me.btnValider.Size = New System.Drawing.Size(120, 30)
        Me.btnValider.TabIndex = 0
        Me.btnValider.Text = "&Valider"
        '
        ' lblInfo
        '
        Me.lblInfo.Location = New System.Drawing.Point(30, 20)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(520, 70)
        Me.lblInfo.TabIndex = 1
        '
        ' Form1
        '
        Me.ClientSize = New System.Drawing.Size(580, 170)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.btnValider)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.1 — Architecture et Concepteur"
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents btnValider As System.Windows.Forms.Button
    Friend WithEvents lblInfo As System.Windows.Forms.Label

End Class
