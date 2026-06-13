' ============================================================================
'  Section 10.5 : Atelier — concepteur du formulaire (VB, généré/écrit à la main)
'  Description : Partie « concepteur » de FormPrincipal (classe partielle).
'                Déclare et dispose les contrôles nommés que le code utilise :
'                btnImporter, progressBar, grilleTransactions, lblTotal,
'                lblRejetees, lstCategories. (Dans VS, ce fichier est généré ;
'                il est écrit ici pour fournir un projet complet et compilable.)
'  Fichier source : 05-atelier-core-csharp-ui-vbnet.md
' ============================================================================

Imports System.ComponentModel
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormPrincipal
    Inherits System.Windows.Forms.Form

    ' Constructeur (généré par le concepteur VS) : appelle InitializeComponent.
    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Private components As IContainer

    Friend WithEvents btnImporter As Button
    Friend WithEvents progressBar As ProgressBar
    Friend WithEvents grilleTransactions As DataGridView
    Friend WithEvents lblTotal As Label
    Friend WithEvents lblRejetees As Label
    Friend WithEvents lstCategories As ListBox

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub InitializeComponent()
        Me.components = New Container()
        Me.btnImporter = New Button()
        Me.progressBar = New ProgressBar()
        Me.grilleTransactions = New DataGridView()
        Me.lblTotal = New Label()
        Me.lblRejetees = New Label()
        Me.lstCategories = New ListBox()
        CType(Me.grilleTransactions, ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' btnImporter
        '
        Me.btnImporter.Location = New System.Drawing.Point(12, 12)
        Me.btnImporter.Name = "btnImporter"
        Me.btnImporter.Size = New System.Drawing.Size(120, 30)
        Me.btnImporter.Text = "Importer…"
        '
        ' progressBar
        '
        Me.progressBar.Location = New System.Drawing.Point(150, 12)
        Me.progressBar.Name = "progressBar"
        Me.progressBar.Size = New System.Drawing.Size(300, 30)
        Me.progressBar.Style = ProgressBarStyle.Marquee
        Me.progressBar.Visible = False
        '
        ' grilleTransactions
        '
        Me.grilleTransactions.AllowUserToAddRows = False
        Me.grilleTransactions.[ReadOnly] = True
        Me.grilleTransactions.Location = New System.Drawing.Point(12, 60)
        Me.grilleTransactions.Name = "grilleTransactions"
        Me.grilleTransactions.Size = New System.Drawing.Size(560, 300)
        '
        ' lblTotal
        '
        Me.lblTotal.AutoSize = True
        Me.lblTotal.Location = New System.Drawing.Point(12, 375)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Text = "Total : —"
        '
        ' lblRejetees
        '
        Me.lblRejetees.AutoSize = True
        Me.lblRejetees.Location = New System.Drawing.Point(220, 375)
        Me.lblRejetees.Name = "lblRejetees"
        Me.lblRejetees.Text = "Rejetées : —"
        '
        ' lstCategories
        '
        Me.lstCategories.Location = New System.Drawing.Point(590, 60)
        Me.lstCategories.Name = "lstCategories"
        Me.lstCategories.Size = New System.Drawing.Size(220, 300)
        '
        ' FormPrincipal
        '
        Me.ClientSize = New System.Drawing.Size(830, 410)
        Me.Controls.Add(Me.btnImporter)
        Me.Controls.Add(Me.progressBar)
        Me.Controls.Add(Me.grilleTransactions)
        Me.Controls.Add(Me.lblTotal)
        Me.Controls.Add(Me.lblRejetees)
        Me.Controls.Add(Me.lstCategories)
        Me.Name = "FormPrincipal"
        Me.Text = "Importateur de transactions"
        CType(Me.grilleTransactions, ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

End Class
