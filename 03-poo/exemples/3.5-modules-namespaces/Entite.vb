' ============================================================================
'  Section 3.5 : Modules, espaces de noms et classes partielles
'  Description : Première moitié des classes partielles : Entite déclare une
'                MÉTHODE PARTIELLE OnModifie (signature à corps vide,
'                implicitement Private, forcément un Sub) appelée par
'                MettreAJour. EntiteSansHook fait de même… mais personne ne
'                l'implémente : le compilateur SUPPRIME la déclaration et
'                tous ses appels.
'  Fichier source : 05-modules-namespaces.md
' ============================================================================

Partial Class Entite
    Partial Private Sub OnModifie()      ' déclaration sans logique
    End Sub

    Public Sub MettreAJour()
        Console.WriteLine("Entite.MettreAJour()")
        OnModifie()                      ' implémenté dans l'autre partie -> appelé
    End Sub
End Class

Partial Class EntiteSansHook
    Partial Private Sub OnModifie()      ' jamais implémenté
    End Sub

    Public Sub MettreAJour()
        Console.WriteLine("EntiteSansHook.MettreAJour()")
        OnModifie()                      ' appel supprimé à la compilation
    End Sub
End Class
