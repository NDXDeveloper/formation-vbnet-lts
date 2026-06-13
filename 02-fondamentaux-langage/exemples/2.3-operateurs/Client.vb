' ============================================================================
'  Section 2.3 : Opérateurs et expressions
'  Description : Petite classe support pour la démonstration du court-circuit
'                AndAlso (« client IsNot Nothing AndAlso client.Solde > 0 »).
'  Fichier source : 03-operateurs.md
' ============================================================================

''' <summary>Client minimal : seul le solde nous intéresse ici.</summary>
Public Class Client
    Public Property Solde As Decimal
End Class
