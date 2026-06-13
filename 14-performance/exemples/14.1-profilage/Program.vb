' ============================================================================
'  Section 14.1 : Profilage — émission de la métrique
'  Description : Simule un traitement qui incrémente le compteur personnalisé,
'                et vérifie ses métadonnées (nom, type). En production, on
'                observerait ce compteur en direct via dotnet-counters.
'  Fichier source : 01-profilage.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 14.1 Profilage : métrique personnalisée ===")
        Console.WriteLine()

        Console.WriteLine($"Compteur : nom={Telemetrie.CommandesTraitees.Name} ; " &
                          $"meter={Telemetrie.CommandesTraitees.Meter.Name} ; " &
                          $"type={Telemetrie.CommandesTraitees.GetType().Name}")

        ' Traitement simulé : chaque « commande » incrémente le compteur.
        For i = 1 To 5
            TraiterCommande(i)
        Next
        Console.WriteLine("5 commandes traitées — compteur incrémenté (émission sans erreur).")
        Console.WriteLine()
        Console.WriteLine("Observation en direct : dotnet-counters monitor -n Profilage --counters MonApp.Metier")
        Console.WriteLine()
        Console.WriteLine("Terminé.")
    End Sub

    Private Sub TraiterCommande(id As Integer)
        ' … logique métier …
        Telemetrie.CommandesTraitees.Add(1)
    End Sub
End Module
