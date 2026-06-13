' ============================================================================
'  Section 5.12 : Nouveautés Windows Forms .NET 10
'  Description : Reprend les nouveautés de la section :
'                  · PRESSE-PAPIERS JSON typé : SetDataAsJson(Of T) +
'                    TryGetData(Of T), y compris le format INFÉRÉ du type
'                    (GetType(Personne).FullName) ;
'                  · GLISSER-DÉPOSER typé via ITypedDataObject (AllowDrop,
'                    DragEnter/DragDrop sur FileDrop) ;
'                  · Form.FormScreenCaptureMode = HideContent (anti-capture).
'                Le journal d'auto-test vérifie le round-trip JSON et
'                l'application sans erreur de FormScreenCaptureMode.
'  Fichier source : 12-nouveautes-net10.md
' ============================================================================

Imports System.Linq
Imports System.Windows.Forms

Public Class MainForm

    Private Const JOURNAL As String = "5.12-nouveautes-net10-autotest.log"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Anti-capture d'écran : noircit le formulaire dans les captures via l'API Windows.
        Me.FormScreenCaptureMode = ScreenCaptureMode.HideContent

        VerifierPressePapiers()
        AutoFermeture.Activer(Me)
    End Sub

    ' ---- Presse-papiers JSON typé (avec format inféré du type) ----
    Private Sub VerifierPressePapiers()
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        Dim original As New Personne With {.Nom = "Alice", .Age = 30}
        Dim ok As Boolean = False
        Dim recuperee As Personne = Nothing
        Try
            ' Écriture : format = nom de type complet
            Clipboard.SetDataAsJson(GetType(Personne).FullName, original)
            ' Lecture : format DÉDUIT du type passé
            If Clipboard.TryGetData(recuperee) Then
                ok = recuperee IsNot Nothing AndAlso
                     recuperee.Nom = original.Nom AndAlso
                     recuperee.Age = original.Age
            End If
        Catch ex As Exception
            AutoFermeture.Journaliser(JOURNAL, $"Presse-papiers indisponible : {ex.GetType().Name}")
        End Try

        If recuperee IsNot Nothing Then
            lblPressePapiers.Text = $"Relu (JSON, format inféré) : {recuperee.Nom}, {recuperee.Age} ans"
        End If
        AutoFermeture.Journaliser(JOURNAL, $"Clipboard JSON Personne (format inféré) : {ok}")
        AutoFermeture.Journaliser(JOURNAL, $"FormScreenCaptureMode appliqué : {Me.FormScreenCaptureMode}")
    End Sub

    ' ---- Glisser-déposer typé (ITypedDataObject) ----
    Private Sub panneau_DragEnter(sender As Object, e As DragEventArgs) Handles panneau.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub panneau_DragDrop(sender As Object, e As DragEventArgs) Handles panneau.DragDrop
        If TypeOf e.Data Is ITypedDataObject Then
            Dim donnees As ITypedDataObject = CType(e.Data, ITypedDataObject)
            Dim fichiers As String() = Nothing
            If donnees.TryGetData(DataFormats.FileDrop, fichiers) Then
                lblDepot.Text = $"{fichiers.Length} fichier(s) déposé(s) : " &
                                String.Join(", ", fichiers.Select(Function(f) IO.Path.GetFileName(f)))
            End If
        End If
    End Sub

End Class
