' ============================================================================
'  Section 12.1 : Exceptions personnalisées
'  Description : Exceptions porteuses de sens métier. Patron standard : nom en
'                « …Exception », héritage DIRECT de Exception (jamais
'                ApplicationException), trois constructeurs habituels, et une
'                surcharge enrichissant le contexte. Pas de <Serializable> ni de
'                constructeur de sérialisation (inutile/déconseillé sur .NET
'                moderne). DataAccessException illustre l'enveloppement.
'  Fichier source : 01-exceptions.md
' ============================================================================

Imports System

' Erreur métier : commande introuvable, avec l'identifiant concerné.
Public Class CommandeIntrouvableException
    Inherits Exception

    Public ReadOnly Property CommandeId As Integer

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    ' Surcharge pratique qui enrichit le contexte métier.
    Public Sub New(commandeId As Integer)
        MyBase.New($"La commande {commandeId} est introuvable.")
        Me.CommandeId = commandeId
    End Sub
End Class

' Exception d'accès aux données : enveloppe la cause technique (InnerException).
Public Class DataAccessException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub
End Class

' Erreur réseau simulée, avec un code, pour démontrer les filtres « When ».
Public Class ErreurReseau
    Inherits Exception

    Public ReadOnly Property Code As Integer

    Public Sub New(code As Integer, message As String)
        MyBase.New(message)
        Me.Code = code
    End Sub
End Class
