' ============================================================================
'  Section 8.3 : Limites du web en VB.NET — Minimal API
'  Description : Une Minimal API EN VB.NET, fonctionnelle. Le point de la
'                section : ça compile et ça tourne, mais on a dû monter le
'                projet à la main, tout est dans Module/Sub Main, et chaque
'                espace de noms est importé explicitement — exactement la
'                « cérémonie » que les Minimal APIs voulaient supprimer. La
'                liaison des paramètres (route /somme/{a}/{b}) se fait ici PAR
'                RÉFLEXION (pas de Request Delegate Generator en VB, donc pas
'                de Native AOT).
'  Fichier source : 03-limites-web-vbnet.md
' ============================================================================

Imports System
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Http

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)
        Dim app = builder.Build()

        ' Route texte (l'exemple littéral de la section)
        app.MapGet("/", Function() "Bonjour depuis VB.NET !")

        ' Route renvoyant du JSON (objet anonyme sérialisé automatiquement)
        app.MapGet("/info", Function() New With {
            .message = "Minimal API en VB.NET",
            .langage = "VB.NET",
            .aot = False
        })

        ' Liaison de paramètres de route — fonctionne, mais par réflexion
        app.MapGet("/somme/{a:int}/{b:int}",
                   Function(a As Integer, b As Integer) a + b)

        ' Écho d'un corps JSON
        app.MapPost("/echo",
                    Function(message As Message) Results.Ok(message))

        app.Run()
    End Sub
End Module

' Type de liaison du corps pour /echo
Public Class Message
    Public Property Texte As String
End Class
