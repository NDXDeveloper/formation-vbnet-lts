' ============================================================================
'  Section 11.7 : Gestion des risques — tests de caractérisation
'  Description : Capturent le comportement ACTUEL (bugs compris), pas le
'                comportement « correct ». Ils verrouillent l'arrondi banquier
'                de CInt et la troncature à longueur fixe — afin de détecter le
'                moindre écart qu'introduirait la migration.
'  Fichier source : 07-gestion-risques.md
' ============================================================================

Imports Heritage
Imports Xunit

Public Class TestsCaracterisation

    ' CInt = arrondi banquier : les demi-entiers vont vers le PAIR le plus proche.
    <Theory>
    <InlineData(0.5, 0)>
    <InlineData(1.5, 2)>
    <InlineData(2.5, 2)>
    <InlineData(3.5, 4)>
    <InlineData(4.5, 4)>
    <InlineData(5.5, 6)>
    Public Sub Arrondir_VerrouilleArrondiBanquier(valeur As Double, attendu As Integer)
        Assert.Equal(attendu, New CalculateurLegacy().Arrondir(valeur))
    End Sub

    ' Chaîne de longueur fixe (5) : complétée à droite, ou tronquée.
    <Theory>
    <InlineData("AB", "AB   ")>
    <InlineData("ABCDE", "ABCDE")>
    <InlineData("ABCDEFG", "ABCDE")>
    Public Sub FormaterCode_VerrouilleLongueurFixe(entree As String, attendu As String)
        Assert.Equal(attendu, New CalculateurLegacy().FormaterCode(entree))
    End Sub

End Class
