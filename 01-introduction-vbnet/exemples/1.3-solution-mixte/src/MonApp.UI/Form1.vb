' ============================================================================
'  Section 1.3 : L'écosystème .NET — Structure d'une solution (solution mixte)
'  Description : Le formulaire principal consomme la logique métier VB
'                (MonApp.Metier), qui s'appuie elle-même sur la brique C#
'                (MonApp.Core) : la chaîne UI (VB) → Métier (VB) → Core (C#)
'                de la solution mixte, dans une même solution .NET.
'  Fichier source : 03-ecosysteme-dotnet.md
' ============================================================================

Imports MonApp.Core
Imports MonApp.Metier

Public Class Form1

    Private ReadOnly _calculateur As New CalculateurFacture()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim lignes = FactureDemo.Lignes()

        For Each ligne In lignes
            lstLignes.Items.Add(
                $"{ligne.Libelle} — {ligne.Quantite} × {ligne.PrixUnitaireHt:0.00} € HT " &
                $"(TVA {BaremeTva.Taux(ligne.Categorie):P1})")
        Next

        lblTotaux.Text =
            $"Total HT  : {_calculateur.TotalHt(lignes):0.00} €" & Environment.NewLine &
            $"TVA       : {_calculateur.TotalTva(lignes):0.00} €" & Environment.NewLine &
            $"Total TTC : {_calculateur.TotalTtc(lignes):0.00} €"
    End Sub

    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        Close()
    End Sub

End Class
