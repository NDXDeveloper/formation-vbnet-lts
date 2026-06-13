' ============================================================================
'  Section 12.2 : Débogage — instrumentation légère (vérifiable)
'  Description : Capture la sortie de Debug.WriteLine / Debug.Assert via un
'                écouteur de trace (pour la rendre observable hors débogueur),
'                lit la chaîne <DebuggerDisplay>, et teste la branche #If DEBUG.
'                À exécuter en configuration DEBUG (le défaut de `dotnet run`).
'  Fichier source : 02-debogage.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Diagnostics
Imports System.IO

Module Program
    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 12.2 Débogage : instrumentation légère ===")
        Console.WriteLine()

        ' 1) Debug.WriteLine / Debug.Assert, capturés via un écouteur de trace.
        Dim tampon As New StringWriter()
        Trace.Listeners.Add(New TextWriterTraceListener(tampon))

        Dim commande As New Commande With {.Id = 4271, .Montant = 1250D, .Statut = "Expédiée"}
        Debug.WriteLine($"Traitement de la commande {commande.Id}")
        Debug.Assert(commande.Montant >= 0D, "Le montant ne devrait jamais être négatif")
        Trace.Flush()

        Dim capture = tampon.ToString()
        Console.WriteLine("[Debug.WriteLine / Debug.Assert]")
        Console.WriteLine($"  trace contient « commande 4271 » = {capture.Contains("commande 4271")}")
        Console.WriteLine($"  assertion (Montant >= 0) franchie sans échec = True")
        Console.WriteLine()

        ' 2) <DebuggerDisplay> : la valeur que le débogueur afficherait.
        Console.WriteLine("[DebuggerDisplay]")
        Console.WriteLine($"  affichage = {commande.TexteAffichageDebogueur()}")
        Console.WriteLine()

        ' 3) Compilation conditionnelle #If DEBUG.
        Dim modeDebug = False
#If DEBUG Then
        modeDebug = True
#End If
        Console.WriteLine("[#If DEBUG]")
        Console.WriteLine($"  branche DEBUG compilée = {modeDebug}")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub
End Module
