' ============================================================================
'  Section 3.7 : Types immuables et records
'  Description : Le type immuable « maison » exact de la section — tout ce
'                qu'un record C# génère automatiquement, écrit à la main :
'                  · propriétés ReadOnly assignées dans le constructeur
'                    (avec validation) ;
'                  · AUCUNE méthode mutante : AvecMontant renvoie une
'                    NOUVELLE instance (équivalent manuel du « with » C#) ;
'                  · égalité de VALEUR : Equals + GetHashCode redéfinis ;
'                  · ToString lisible.
'  Fichier source : 07-immuabilite-records.md
' ============================================================================

Public Class Argent

    Public ReadOnly Property Montant As Decimal
    Public ReadOnly Property Devise As String

    Public Sub New(montant As Decimal, devise As String)
        If String.IsNullOrWhiteSpace(devise) Then
            Throw New ArgumentException("Devise requise.", NameOf(devise))
        End If
        Me.Montant = montant       ' assignation autorisée : on est dans le constructeur
        Me.Devise = devise
    End Sub

    ''' <summary>Mise à jour non destructive : renvoie une nouvelle instance.</summary>
    Public Function AvecMontant(nouveauMontant As Decimal) As Argent
        Return New Argent(nouveauMontant, Devise)   ' l'original est intact
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim autre = TryCast(obj, Argent)
        Return autre IsNot Nothing AndAlso
               Montant = autre.Montant AndAlso
               Devise = autre.Devise
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return HashCode.Combine(Montant, Devise)
    End Function

    Public Overrides Function ToString() As String
        Return $"{Montant:0.00} {Devise}"
    End Function

End Class
