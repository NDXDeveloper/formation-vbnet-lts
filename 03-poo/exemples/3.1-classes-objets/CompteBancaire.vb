' ============================================================================
'  Section 3.1 : Classes et objets
'  Description : Le CompteBancaire de la section : propriété COMPLÈTE avec
'                validation dans le Set (exception si solde négatif) et
'                méthodes membres (Deposer, PeutRetirer). Plus la classe
'                Configuration et son constructeur partagé (Shared Sub New),
'                exécuté une seule fois avant la première utilisation.
'  Fichier source : 01-classes-objets.md
' ============================================================================

Public Class CompteBancaire

    Private _solde As Decimal

    ''' <summary>Propriété complète : le Set valide avant d'écrire.</summary>
    Public Property Solde As Decimal
        Get
            Return _solde
        End Get
        Set(value As Decimal)
            If value < 0D Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(value), "Le solde ne peut pas être négatif.")
            End If
            _solde = value
        End Set
    End Property

    Public Sub Deposer(montant As Decimal)
        If montant <= 0D Then
            Throw New ArgumentOutOfRangeException(NameOf(montant))
        End If
        _solde += montant
    End Sub

    Public Function PeutRetirer(montant As Decimal) As Boolean
        Return montant > 0D AndAlso montant <= _solde
    End Function

End Class

''' <summary>Constructeur partagé : initialise l'état de niveau classe.</summary>
Public Class Configuration
    Public Shared ReadOnly Version As String

    Shared Sub New()
        Version = "1.0"
    End Sub
End Class
