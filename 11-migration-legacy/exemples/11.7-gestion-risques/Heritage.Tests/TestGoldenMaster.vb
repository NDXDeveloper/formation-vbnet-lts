' ============================================================================
'  Section 11.7 : Gestion des risques — golden master (test d'approbation)
'  Description : On capture les SORTIES pour un large éventail d'entrées AVANT
'                la migration (la « référence » ci-dessous), puis on compare
'                APRÈS. Idéal pour du code hérité aux sorties nombreuses et sans
'                tests existants : toute divergence fait échouer le test.
'  Fichier source : 07-gestion-risques.md
' ============================================================================

Imports System.Globalization
Imports System.Linq
Imports Heritage
Imports Xunit

Public Class TestGoldenMaster

    ' Référence capturée « avant migration » (comportement à préserver).
    Private Const Reference As String =
        "0.5=0;1.5=2;2.5=2;3.5=4;4.5=4;5.5=6;6.5=6;7.5=8"

    <Fact>
    Public Sub Arrondir_CorrespondAuGoldenMaster()
        Dim calc As New CalculateurLegacy()
        Dim entrees = {0.5, 1.5, 2.5, 3.5, 4.5, 5.5, 6.5, 7.5}

        Dim actuel = String.Join(";",
            entrees.Select(Function(v) $"{v.ToString(CultureInfo.InvariantCulture)}={calc.Arrondir(v)}"))

        Assert.Equal(Reference, actuel)
    End Sub

End Class
