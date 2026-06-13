' ============================================================================
'  Section 18.4 : le filet de tests rattrape le piège « ^ »
'  Description : Vérifie que la bibliothèque C# migrée préserve la sémantique du « ^ »
'                de VB (Math.Pow), et épingle le résultat d'une conversion littérale
'                (XOR) pour prouver qu'elle DIVERGE. Le compilateur accepterait les
'                deux ; seul le test distingue le correct du faux.
'  Fichier source : 04-migrer-vers-csharp.md
' ============================================================================

Option Strict On
Option Explicit On

Imports CalculsCsharp
Imports Xunit

Public Class TestsOperateurs

    <Fact>
    Public Sub Puissance_EquivautAuCirconflexeVb()
        Dim a As Integer = 5, b As Integer = 3
        Dim puissanceVb As Double = a ^ b          ' VB : 5 ^ 3 = 125
        Assert.Equal(puissanceVb, OperateursMigres.Puissance(a, b))
    End Sub

    <Fact>
    Public Sub ConversionLitterale_Xor_DivergeDeLaPuissance()
        ' « a ^ b » converti tel quel en C# donne un XOR (6), pas la puissance (125).
        Assert.Equal(6, OperateursMigres.XorBinaire(5, 3))
        Assert.NotEqual(CInt(5 ^ 3), OperateursMigres.XorBinaire(5, 3))
    End Sub

End Class
