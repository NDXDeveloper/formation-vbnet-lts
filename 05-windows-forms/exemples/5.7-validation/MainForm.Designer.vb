' ============================================================================
'  Section 5.7 : Validation (ErrorProvider, DataAnnotations, règles personnalisées)
'  Description : Partie Concepteur : champs de saisie (Nom, Email, Age,
'                CodePostal), un ErrorProvider (composant non visuel) et les
'                boutons Enregistrer (validation) / Annuler (CausesValidation
'                = False, pour ne pas piéger l'utilisateur dans un champ
'                invalide).
'  Fichier source : 07-validation.md
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
        Me.components = New System.ComponentModel.Container()
        Me.lblNom = New System.Windows.Forms.Label()
        Me.txtNom = New System.Windows.Forms.TextBox()
        Me.lblEmail = New System.Windows.Forms.Label()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.lblAge = New System.Windows.Forms.Label()
        Me.numAge = New System.Windows.Forms.NumericUpDown()
        Me.lblCP = New System.Windows.Forms.Label()
        Me.txtCodePostal = New System.Windows.Forms.TextBox()
        Me.btnEnregistrer = New System.Windows.Forms.Button()
        Me.btnAnnuler = New System.Windows.Forms.Button()
        Me.errProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        CType(Me.numAge, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        Me.lblNom.Location = New System.Drawing.Point(20, 20)
        Me.lblNom.Size = New System.Drawing.Size(90, 23)
        Me.lblNom.Text = "Nom :"
        Me.txtNom.Location = New System.Drawing.Point(120, 18)
        Me.txtNom.Size = New System.Drawing.Size(220, 23)
        '
        Me.lblEmail.Location = New System.Drawing.Point(20, 55)
        Me.lblEmail.Size = New System.Drawing.Size(90, 23)
        Me.lblEmail.Text = "E-mail :"
        Me.txtEmail.Location = New System.Drawing.Point(120, 53)
        Me.txtEmail.Size = New System.Drawing.Size(220, 23)
        '
        Me.lblAge.Location = New System.Drawing.Point(20, 90)
        Me.lblAge.Size = New System.Drawing.Size(90, 23)
        Me.lblAge.Text = "Âge :"
        Me.numAge.Location = New System.Drawing.Point(120, 88)
        Me.numAge.Size = New System.Drawing.Size(80, 23)
        Me.numAge.Maximum = New Decimal(New Integer() {200, 0, 0, 0})
        '
        Me.lblCP.Location = New System.Drawing.Point(20, 125)
        Me.lblCP.Size = New System.Drawing.Size(90, 23)
        Me.lblCP.Text = "Code postal :"
        Me.txtCodePostal.Location = New System.Drawing.Point(120, 123)
        Me.txtCodePostal.Size = New System.Drawing.Size(100, 23)
        '
        Me.btnEnregistrer.Location = New System.Drawing.Point(120, 165)
        Me.btnEnregistrer.Size = New System.Drawing.Size(110, 30)
        Me.btnEnregistrer.Text = "Enregistrer"
        '
        Me.btnAnnuler.CausesValidation = False
        Me.btnAnnuler.Location = New System.Drawing.Point(240, 165)
        Me.btnAnnuler.Size = New System.Drawing.Size(100, 30)
        Me.btnAnnuler.Text = "Annuler"
        '
        Me.errProvider.ContainerControl = Me
        '
        Me.ClientSize = New System.Drawing.Size(380, 215)
        Me.Controls.Add(Me.lblNom)
        Me.Controls.Add(Me.txtNom)
        Me.Controls.Add(Me.lblEmail)
        Me.Controls.Add(Me.txtEmail)
        Me.Controls.Add(Me.lblAge)
        Me.Controls.Add(Me.numAge)
        Me.Controls.Add(Me.lblCP)
        Me.Controls.Add(Me.txtCodePostal)
        Me.Controls.Add(Me.btnEnregistrer)
        Me.Controls.Add(Me.btnAnnuler)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "5.7 — Validation"
        CType(Me.numAge, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblNom As System.Windows.Forms.Label
    Friend WithEvents txtNom As System.Windows.Forms.TextBox
    Friend WithEvents lblEmail As System.Windows.Forms.Label
    Friend WithEvents txtEmail As System.Windows.Forms.TextBox
    Friend WithEvents lblAge As System.Windows.Forms.Label
    Friend WithEvents numAge As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblCP As System.Windows.Forms.Label
    Friend WithEvents txtCodePostal As System.Windows.Forms.TextBox
    Friend WithEvents btnEnregistrer As System.Windows.Forms.Button
    Friend WithEvents btnAnnuler As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider

End Class
