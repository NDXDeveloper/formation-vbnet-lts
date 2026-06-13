' ============================================================================
'  Section 2.10 : Génériques avancés
'  Description : Les interfaces variantes déclarées dans la section —
'                IProducteur(Of Out T) : covariante (T en sortie uniquement) ;
'                IConsommateur(Of In T) : contravariante (T en entrée
'                uniquement) — avec deux implémentations pour la démo.
'  Fichier source : 10-generiques-avances.md
' ============================================================================

Public Interface IProducteur(Of Out T)     ' covariant : T uniquement en sortie
    Function Produire() As T
End Interface

Public Interface IConsommateur(Of In T)    ' contravariant : T uniquement en entrée
    Sub Consommer(item As T)
End Interface

''' <summary>Produit des String : assignable à IProducteur(Of Object) (covariance).</summary>
Public Class ProducteurDeChaines
    Implements IProducteur(Of String)

    Public Function Produire() As String Implements IProducteur(Of String).Produire
        Return "chaîne produite"
    End Function
End Class

''' <summary>Consomme des Object : assignable à IConsommateur(Of String) (contravariance).</summary>
Public Class AfficheurUniversel
    Implements IConsommateur(Of Object)

    Public Sub Consommer(item As Object) Implements IConsommateur(Of Object).Consommer
        Console.WriteLine($"  consommé : {item}")
    End Sub
End Class
