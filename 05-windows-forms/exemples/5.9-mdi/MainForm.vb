' ============================================================================
'  Section 5.9 : Applications MDI et multi-formulaires
'  Description : Conteneur MDI (IsMdiContainer=True) de la section :
'                  · « Nouveau » crée un DocumentForm rattaché par MdiParent ;
'                  · LayoutMdi (Cascade, TileHorizontal) dispose les enfants ;
'                  · le menu Fenêtre se peuple via MdiWindowListItem ;
'                  · « Filtrer » ouvre un FiltreForm non modal et s'abonne à
'                    son événement FiltreApplique (communication découplée).
'                En mode smoke test, deux documents sont ouverts au démarrage.
'  Fichier source : 09-mdi.md
' ============================================================================

Imports System.Windows.Forms

Public Class MainForm

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Ouvre deux documents pour rendre la démo MDI visible immédiatement.
        OuvrirDocument()
        OuvrirDocument()
        LayoutMdi(MdiLayout.TileHorizontal)
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub OuvrirDocument()
        Dim doc As New DocumentForm()
        doc.MdiParent = Me      ' rattache l'enfant au conteneur
        doc.Show()
    End Sub

    Private Sub mnuNouveau_Click(sender As Object, e As EventArgs) Handles mnuNouveau.Click
        OuvrirDocument()
    End Sub

    Private Sub mnuCascade_Click(sender As Object, e As EventArgs) Handles mnuCascade.Click
        LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub mnuMosaiqueH_Click(sender As Object, e As EventArgs) Handles mnuMosaiqueH.Click
        LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    ' ---- Communication découplée avec un formulaire non modal ----
    Private Sub mnuFiltrer_Click(sender As Object, e As EventArgs) Handles mnuFiltrer.Click
        Dim f As New FiltreForm()
        AddHandler f.FiltreApplique, AddressOf SurFiltreApplique
        f.Show(Me)
    End Sub

    Private Sub SurFiltreApplique(sender As Object, e As FiltreEventArgs)
        lblStatut.Text = $"Filtre appliqué : « {e.Terme} » ({MdiChildren.Length} document(s) ouvert(s))"
    End Sub

End Class
