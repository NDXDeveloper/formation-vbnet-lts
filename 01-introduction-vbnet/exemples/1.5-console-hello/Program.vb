' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Le « code généré » exact du Projet 1 de la section — celui
'                que produit « dotnet new console -lang VB » (vérifié à
'                l'identique avec le SDK .NET 10.0.301) :
'                  · Imports System (optionnel : System fait partie des
'                    imports par défaut du projet) ;
'                  · Module Program : conteneur de membres partagés ;
'                  · Sub Main(args As String()) : le point d'entrée ;
'                  · pas de top-level statements en VB, contrairement à C#.
'  Fichier source : 05-premier-projet.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")
    End Sub
End Module
