' ============================================================================
'  Section 15.4 : Conteneurisation — point d'entrée du Worker
'  Description : Câble le Generic Host à la main (pas de modèle « Worker » en VB)
'                et enregistre le BackgroundService. ⚠️ La variable d'hôte est
'                nommée « hote », jamais « host » (collision avec la classe Host,
'                BC30980 — piège VB du module 4.8).
'  Fichier source : 04-docker.md
' ============================================================================

Option Strict On
Option Explicit On

Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Module Program
    Sub Main(args As String())
        Dim builder = Host.CreateApplicationBuilder(args)
        builder.Services.AddHostedService(Of Travailleur)()
        Dim hote = builder.Build()
        hote.Run()
    End Sub
End Module
