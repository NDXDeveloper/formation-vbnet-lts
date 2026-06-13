' ============================================================================
'  Section 5.6 : Gestion des événements (souris, clavier, cycle de vie)
'  Description : Démonstration des trois familles d'événements de la section :
'                  · SOURIS : MouseDown (bouton gauche/droit via e.Button,
'                    position e.Location) ;
'                  · CLAVIER : KeyPreview=True + Form_KeyDown pour un raccourci
'                    Ctrl+S global (e.Handled / e.SuppressKeyPress) ;
'                  · TEMPS : composant Timer (Tick chaque seconde -> horloge) ;
'                  · CYCLE DE VIE : Load / Shown / FormClosing ANNULABLE
'                    (confirmation si modifications non enregistrées).
'                Un gestionnaire unique traite trois boutons (Handles multiple).
'  Fichier source : 06-evenements.md
' ============================================================================

Imports System.Windows.Forms

Public Class MainForm

    Private WithEvents tmrHorloge As New Timer()
    Private _modifieNonEnregistre As Boolean = False

    ' ---- Cycle de vie : Load (initialisation des données) ----
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        tmrHorloge.Interval = 1000
        tmrHorloge.Start()
        lblCycle.Text = "Événement : Load (données initialisées)"
        AutoFermeture.Activer(Me)
    End Sub

    ' ---- Cycle de vie : Shown (après le premier affichage) ----
    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        lblCycle.Text = "Événement : Shown (fenêtre visible)"
    End Sub

    ' ---- Timer : horloge (sur le thread d'interface) ----
    Private Sub tmrHorloge_Tick(sender As Object, e As EventArgs) Handles tmrHorloge.Tick
        lblHeure.Text = DateTime.Now.ToLongTimeString()
    End Sub

    ' ---- Souris : MouseDown (bouton et position) ----
    Private Sub panneau_MouseDown(sender As Object, e As MouseEventArgs) Handles panneau.MouseDown
        Select Case e.Button
            Case MouseButtons.Left
                lblSouris.Text = $"Clic GAUCHE en ({e.X}, {e.Y})"
            Case MouseButtons.Right
                lblSouris.Text = $"Clic DROIT en ({e.X}, {e.Y})"
        End Select
        _modifieNonEnregistre = True
    End Sub

    ' ---- Clavier : raccourci global Ctrl+S (KeyPreview) ----
    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.S Then
            lblClavier.Text = "Raccourci Ctrl+S capté au niveau du formulaire."
            _modifieNonEnregistre = False
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    ' ---- Un gestionnaire, trois boutons (Handles multiple) ----
    Private Sub Couleur_Click(sender As Object, e As EventArgs) _
            Handles btnRouge.Click, btnVert.Click, btnBleu.Click
        Dim bouton = DirectCast(sender, Button)
        panneau.BackColor = bouton.BackColor
        _modifieNonEnregistre = True
    End Sub

    ' ---- Cycle de vie : FormClosing annulable ----
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' En mode smoke test (DEMO_AUTOCLOSE), on ne bloque jamais la fermeture.
        If Environment.GetEnvironmentVariable("DEMO_AUTOCLOSE") = "1" Then Return

        If _modifieNonEnregistre Then
            Dim reponse As DialogResult =
                MessageBox.Show("Des modifications non enregistrées seront perdues. Fermer quand même ?",
                                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If reponse = DialogResult.No Then e.Cancel = True
        End If
    End Sub

End Class
