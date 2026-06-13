' ============================================================================
'  Section 17.8 (Cas 1) : le test révèle la divergence sémantique
'  Description : Avec l'entrée {"10","abc","20","30"}, le comportement de référence
'                (VB6 / Resume Next) ignore "abc" et calcule (10+20+30)/3 = 20.
'                La traduction correcte donne 20 ; la fausse s'arrête à "abc" et
'                renvoie 10/1 = 10. Le test de caractérisation distingue les deux.
'  Fichier source : 08-cas-concrets.md
' ============================================================================

Option Strict On
Option Explicit On

Imports Xunit

Public Class TestsMoyenne

    ' Comportement de RÉFÉRENCE : "abc" ignorée, les trois nombres sommés -> 20.
    Private Shared ReadOnly Entree As String() = {"10", "abc", "20", "30"}

    <Fact>
    Public Sub MoyenneCorrecte_IgnoreLaLigneIllisible_EtCalcule()
        Assert.Equal(20.0, TraductionVb6.MoyenneCorrecte(Entree))
    End Sub

    <Fact>
    Public Sub MoyenneFausse_DivergeDuComportementAttendu()
        ' La fausse traduction quitte la boucle dès "abc" : elle ne voit que "10".
        Assert.NotEqual(20.0, TraductionVb6.MoyenneFausse(Entree))
        Assert.Equal(10.0, TraductionVb6.MoyenneFausse(Entree))   ' valeur boguée épinglée
    End Sub

    <Fact>
    Public Sub MoyenneCorrecte_ToutesLignesIllisibles_RenvoieZero()
        Assert.Equal(0.0, TraductionVb6.MoyenneCorrecte({"x", "y"}))
    End Sub

End Class
