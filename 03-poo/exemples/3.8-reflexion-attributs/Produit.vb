' ============================================================================
'  Section 3.8 : Réflexion et attributs
'  Description : La classe Produit de la section, cible de toutes les
'                démonstrations : propriétés décorées de l'attribut
'                personnalisé <Colonne(...)> (chevrons en VB, pas de
'                crochets !), propriété sans attribut (ignorée du mappage),
'                méthode Recalculer invoquée par MethodInfo.Invoke et
'                constructeurs utilisés par Activator.CreateInstance.
'                ConfigurationLegacy porte un <Obsolete> relu par réflexion.
'  Fichier source : 08-reflexion-attributs.md
' ============================================================================

Public Class Produit

    <Colonne("produit_id")>
    Public Property Id As Integer

    <Colonne("libelle", Obligatoire:=True)>
    Public Property Nom As String

    Public Property Prix As Decimal

    Public Property Interne As String   ' aucun attribut -> ignorée par le mappage

    Public Sub New()
    End Sub

    Public Sub New(nom As String, prix As Decimal)
        Me.Nom = nom
        Me.Prix = prix
    End Sub

    ''' <summary>Invoquée dynamiquement par MethodInfo.Invoke dans la démo.</summary>
    Public Sub Recalculer(taux As Decimal)
        Prix *= (1 + taux)
    End Sub

End Class

''' <summary>Attribut intégré : &lt;Obsolete&gt; (relu par réflexion dans la démo).</summary>
Public Class ConfigurationLegacy
    <Obsolete("Utilisez NouveauChamp à la place.")>
    Public AncienChamp As String

    Public NouveauChamp As String
End Class
