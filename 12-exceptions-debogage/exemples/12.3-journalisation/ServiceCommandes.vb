' ============================================================================
'  Section 12.3 : Journalisation — service journalisé
'  Description : Reçoit un ILogger(Of ServiceCommandes) (catégorie = nom du
'                type). Démontre le logging STRUCTURÉ (modèle à espaces réservés,
'                JAMAIS d'interpolation $"…"), la garde IsEnabled pour un argument
'                coûteux, les portées BeginScope, et la journalisation d'exception
'                (exception en PREMIER argument de LogError).
'  Fichier source : 03-journalisation.md
' ============================================================================

Imports System
Imports Microsoft.Extensions.Logging

Public Class ServiceCommandes
    Private ReadOnly _logger As ILogger(Of ServiceCommandes)

    Public Sub New(logger As ILogger(Of ServiceCommandes))
        _logger = logger
    End Sub

    ' ✅ Modèle de message à espaces réservés nommés (structuré).
    Public Sub Traiter(commandeId As Integer, dureeMs As Integer)
        _logger.LogInformation("Commande {CommandeId} traitée en {DureeMs} ms", commandeId, dureeMs)
    End Sub

    ' Garde IsEnabled : on n'évalue l'argument coûteux que si le niveau est actif.
    Public Sub Detail()
        If _logger.IsEnabled(LogLevel.Debug) Then
            _logger.LogDebug("État détaillé : {Etat}", CalculerEtatCouteux())
        End If
    End Sub

    Private Function CalculerEtatCouteux() As String
        Return "état calculé"
    End Function

    ' Portée : toutes les lignes émises portent CommandeId.
    Public Sub TraiterAvecPortee(commandeId As Integer)
        Using _logger.BeginScope("Commande {CommandeId}", commandeId)
            _logger.LogInformation("Début du traitement")
            _logger.LogInformation("Traitement terminé")
        End Using
    End Sub

    ' Journalisation d'exception : ex en premier argument (pile capturée).
    Public Sub Echouer(commandeId As Integer)
        Try
            Throw New InvalidOperationException("échec simulé")
        Catch ex As Exception
            _logger.LogError(ex, "Échec du traitement de la commande {CommandeId}", commandeId)
        End Try
    End Sub
End Class
