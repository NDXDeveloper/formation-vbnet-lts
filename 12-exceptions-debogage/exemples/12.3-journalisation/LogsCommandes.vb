' ============================================================================
'  Section 12.3 : Journalisation — LoggerMessage.Define (équivalent VB)
'  Description : Le générateur de source [LoggerMessage] (journalisation haute
'                fréquence sans allocation) est RÉSERVÉ À C#. En VB.NET on obtient
'                un résultat équivalent avec l'API d'exécution LoggerMessage.Define :
'                le délégué fortement typé est défini UNE fois et réutilisé à
'                chaque appel, exposé comme méthode d'extension sur ILogger.
'  Fichier source : 03-journalisation.md
' ============================================================================

Imports System
Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.Logging

Public Module LogsCommandes

    ' Délégué défini une seule fois, réutilisé à chaque appel.
    Private ReadOnly _commandeTraitee As Action(Of ILogger, Integer, Exception) =
        LoggerMessage.Define(Of Integer)(
            LogLevel.Information,
            New EventId(1, NameOf(CommandeTraitee)),
            "Commande {CommandeId} traitée avec succès")

    <Extension>
    Public Sub CommandeTraitee(logger As ILogger, commandeId As Integer)
        _commandeTraitee(logger, commandeId, Nothing)
    End Sub

End Module
