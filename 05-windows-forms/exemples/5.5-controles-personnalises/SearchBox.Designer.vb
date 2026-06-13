' ============================================================================
'  Section 5.5 : Contrôles personnalisés et UserControl
'  Description : Partie Concepteur du UserControl SearchBox — c'est ici que
'                figure « Inherits System.Windows.Forms.UserControl », comme
'                pour un formulaire. Compose une TextBox et un Button.
'  Fichier source : 05-controles-personnalises.md
' ============================================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SearchBox
    Inherits System.Windows.Forms.UserControl

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
        Me.txtRecherche = New System.Windows.Forms.TextBox()
        Me.btnRechercher = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        Me.txtRecherche.Location = New System.Drawing.Point(3, 4)
        Me.txtRecherche.Size = New System.Drawing.Size(220, 23)
        '
        Me.btnRechercher.Location = New System.Drawing.Point(229, 3)
        Me.btnRechercher.Size = New System.Drawing.Size(90, 25)
        Me.btnRechercher.Text = "Rechercher"
        '
        Me.Controls.Add(Me.txtRecherche)
        Me.Controls.Add(Me.btnRechercher)
        Me.Name = "SearchBox"
        Me.Size = New System.Drawing.Size(325, 32)
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents txtRecherche As System.Windows.Forms.TextBox
    Friend WithEvents btnRechercher As System.Windows.Forms.Button

End Class
