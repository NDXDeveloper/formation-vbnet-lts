' ============================================================================
'  Section 11.5 : Coexistence — compilation conditionnelle (#If) multi-cible
'  Description : Une même classe, deux chemins de code selon la cible. En VB.NET,
'                la compilation conditionnelle s'écrit #If … #Else … #End If (et
'                non le #if du C#). Le symbole NET48 n'est défini que pour la
'                build .NET Framework ; le chemin #Else sert .NET moderne.
'  Fichier source : 05-coexistence.md
' ============================================================================

Imports System

Public Class InfoPlateforme

    Public Function Decrire() As String
#If NET48 Then
        Return ".NET Framework (cible net48) — chemin de compatibilité"
#Else
        Return $".NET moderne ({Environment.Version}) — chemin natif"
#End If
    End Function

End Class
