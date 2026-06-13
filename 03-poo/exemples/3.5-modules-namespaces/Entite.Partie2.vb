' ============================================================================
'  Section 3.5 : Modules, espaces de noms et classes partielles
'  Description : Seconde moitié de la classe partielle Entite (même type,
'                autre fichier — fusionnés à la compilation, comme
'                Form1.vb / Form1.Designer.vb en Windows Forms).
'                Elle fournit l'implémentation OPTIONNELLE de la méthode
'                partielle OnModifie.
'  Fichier source : 05-modules-namespaces.md
' ============================================================================

Partial Class Entite
    Private Sub OnModifie()
        Console.WriteLine("  -> hook OnModifie exécuté (implémenté dans Entite.Partie2.vb)")
    End Sub
End Class
