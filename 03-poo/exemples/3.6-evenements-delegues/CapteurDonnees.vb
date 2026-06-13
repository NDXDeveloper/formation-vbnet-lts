' ============================================================================
'  Section 3.6 : Événements et délégués — un point fort idiomatique de VB.NET
'  Description : Le Custom Event de la section, rendu observable : les TROIS
'                blocs (AddHandler / RemoveHandler / RaiseEvent — ce dernier
'                absent de C#) maintiennent ici une liste d'abonnés manuelle
'                et tracent chaque opération.
'  Fichier source : 06-evenements-delegues.md
' ============================================================================

Imports System.Collections.Generic

Public Class DonneesEventArgs
    Inherits EventArgs

    Public ReadOnly Property Contenu As String

    Public Sub New(contenu As String)
        Me.Contenu = contenu
    End Sub
End Class

Public Class CapteurDonnees

    Private ReadOnly _abonnes As New List(Of EventHandler(Of DonneesEventArgs))

    Public Custom Event DonneesRecues As EventHandler(Of DonneesEventArgs)
        AddHandler(value As EventHandler(Of DonneesEventArgs))
            _abonnes.Add(value)                          ' enregistrer l'abonné
            Console.WriteLine($"  [Custom Event] abonné ajouté (total : {_abonnes.Count})")
        End AddHandler
        RemoveHandler(value As EventHandler(Of DonneesEventArgs))
            _abonnes.Remove(value)                       ' retirer l'abonné
            Console.WriteLine($"  [Custom Event] abonné retiré (total : {_abonnes.Count})")
        End RemoveHandler
        RaiseEvent(sender As Object, e As DonneesEventArgs)
            For Each abonne In _abonnes                  ' invoquer les abonnés
                abonne.Invoke(sender, e)
            Next
        End RaiseEvent
    End Event

    Public Sub Recevoir(contenu As String)
        RaiseEvent DonneesRecues(Me, New DonneesEventArgs(contenu))
    End Sub

End Class
