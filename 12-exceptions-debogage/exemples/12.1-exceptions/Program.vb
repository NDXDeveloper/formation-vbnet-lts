' ============================================================================
'  Section 12.1 : Try/Catch/Finally, filtres When, hiérarchie, exceptions perso
'  Description : Démonstration vérifiable de chaque mécanisme de gestion des
'                exceptions. Sorties déterministes (on teste les types, codes et
'                la présence d'un cadre de pile, jamais le texte localisé ni la
'                pile brute).
'  Fichier source : 01-exceptions.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Runtime.ExceptionServices

Module Program

    Private _journalise As Boolean

    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 12.1 Exceptions ===")
        Console.WriteLine()

        Console.WriteLine("[Propriétés de Exception]")
        DemoProprietes()

        Console.WriteLine("[Gardes ThrowIf]")
        DemoGardes()

        Console.WriteLine("[Catch du plus précis au plus général]")
        DemoOrdre()

        Console.WriteLine("[Filtres When]")
        DemoFiltresWhen()

        Console.WriteLine("[When → False : journaliser sans capturer]")
        DemoJournaliserSansCapturer()

        Console.WriteLine("[Throw vs Throw ex : préservation de la pile]")
        DemoThrowVsThrowEx()

        Console.WriteLine("[Enveloppement (InnerException)]")
        DemoEnveloppement()

        Console.WriteLine("[ExceptionDispatchInfo : relancer ailleurs]")
        DemoExceptionDispatchInfo()

        Console.WriteLine("[Exception personnalisée]")
        DemoExceptionPersonnalisee()

        Console.WriteLine("[Dictionnaire Data]")
        DemoData()

        Console.WriteLine("Terminé.")
    End Sub

    Private Sub DemoProprietes()
        Try
            Dim valeur = Integer.Parse("abc")
        Catch ex As FormatException
            Console.WriteLine($"  type = {ex.GetType().Name} ; message non vide = {Not String.IsNullOrEmpty(ex.Message)}")
        End Try
        Console.WriteLine()
    End Sub

    Private Sub DemoGardes()
        Dim client As New Object()
        Essayer("client Nothing  ", Sub() EnregistrerClient(Nothing, "Alice", 30))
        Essayer("nom vide        ", Sub() EnregistrerClient(client, "  ", 30))
        Essayer("âge négatif     ", Sub() EnregistrerClient(client, "Alice", -1))
        Console.WriteLine()
    End Sub

    Private Sub EnregistrerClient(client As Object, nom As String, age As Integer)
        ArgumentNullException.ThrowIfNull(client)
        ArgumentException.ThrowIfNullOrWhiteSpace(nom)
        ArgumentOutOfRangeException.ThrowIfNegative(age)
    End Sub

    Private Sub Essayer(etiquette As String, action As Action)
        Try
            action()
            Console.WriteLine($"  {etiquette} -> (aucune exception)")
        Catch ex As Exception
            Console.WriteLine($"  {etiquette} -> {ex.GetType().Name}")
        End Try
    End Sub

    Private Sub DemoOrdre()
        Try
            Throw New ArgumentNullException("param")
        Catch ex As ArgumentNullException
            Console.WriteLine("  intercepté par le Catch SPÉCIFIQUE (ArgumentNullException)")
        Catch ex As ArgumentException
            Console.WriteLine("  intercepté par ArgumentException (ne devrait pas)")
        Catch ex As Exception
            Console.WriteLine("  intercepté par Exception (ne devrait pas)")
        End Try
        Console.WriteLine()
    End Sub

    Private Sub DemoFiltresWhen()
        For Each code In {404, 503, 500}
            Try
                Throw New ErreurReseau(code, $"erreur {code}")
            Catch ex As ErreurReseau When ex.Code = 404
                Console.WriteLine($"  code {code} -> introuvable (404)")
            Catch ex As ErreurReseau When ex.Code = 503
                Console.WriteLine($"  code {code} -> indisponible (503)")
            Catch ex As ErreurReseau
                Console.WriteLine($"  code {code} -> autre erreur ({ex.Code})")
            End Try
        Next
        Console.WriteLine()
    End Sub

    Private Sub DemoJournaliserSansCapturer()
        _journalise = False
        Try
            Try
                Throw New InvalidOperationException("échec simulé")
            Catch ex As Exception When Enregistrer(ex)
                Console.WriteLine("  NE DOIT PAS s'afficher")
            End Try
        Catch ex As Exception
            Console.WriteLine($"  exception remontée (When n'a pas capturé) ; journalisée = {_journalise}")
        End Try
        Console.WriteLine()
    End Sub

    Private Function Enregistrer(ex As Exception) As Boolean
        _journalise = True
        Return False   ' l'exception N'EST PAS interceptée : elle continue de remonter
    End Function

    Private Sub DemoThrowVsThrowEx()
        Dim pileThrow = CapturerPile(AddressOf RelancerAvecThrow)
        Dim pileThrowEx = CapturerPile(AddressOf RelancerAvecThrowEx)
        Console.WriteLine($"  Throw    : pile contient « LeverProfond » = {pileThrow.Contains("LeverProfond")}")
        Console.WriteLine($"  Throw ex : pile contient « LeverProfond » = {pileThrowEx.Contains("LeverProfond")}")
        Console.WriteLine()
    End Sub

    Private Sub LeverProfond()
        Throw New InvalidOperationException("origine profonde")
    End Sub

    Private Sub RelancerAvecThrow()
        Try
            LeverProfond()
        Catch ex As Exception
            Throw          ' préserve la pile d'origine
        End Try
    End Sub

    Private Sub RelancerAvecThrowEx()
        Try
            LeverProfond()
        Catch ex As Exception
            Throw ex       ' réinitialise la pile à cette ligne
        End Try
    End Sub

    Private Function CapturerPile(action As Action) As String
        Try
            action()
        Catch ex As Exception
            Return If(ex.StackTrace, "")
        End Try
        Return ""
    End Function

    Private Sub DemoEnveloppement()
        Try
            Try
                Dim x = Integer.Parse("pas un nombre")
            Catch ex As FormatException
                Throw New DataAccessException("Échec de lecture des données.", ex)
            End Try
        Catch ex As DataAccessException
            Console.WriteLine($"  message = {ex.Message}")
            Console.WriteLine($"  InnerException = {ex.InnerException.GetType().Name}")
            Console.WriteLine($"  GetBaseException = {ex.GetBaseException().GetType().Name}")
        End Try
        Console.WriteLine()
    End Sub

    Private Sub DemoExceptionDispatchInfo()
        Dim capturee As ExceptionDispatchInfo = Nothing
        Try
            LeverProfond()
        Catch ex As Exception
            capturee = ExceptionDispatchInfo.Capture(ex)
        End Try

        Try
            capturee?.Throw()
        Catch ex As Exception
            Console.WriteLine($"  relancée = {ex.GetType().Name} ; pile contient « LeverProfond » = {If(ex.StackTrace, "").Contains("LeverProfond")}")
        End Try
        Console.WriteLine()
    End Sub

    Private Sub DemoExceptionPersonnalisee()
        Try
            Throw New CommandeIntrouvableException(4271)
        Catch ex As CommandeIntrouvableException
            Console.WriteLine($"  CommandeId = {ex.CommandeId} ; message = {ex.Message}")
        End Try
        Console.WriteLine()
    End Sub

    Private Sub DemoData()
        Try
            Dim ex As New InvalidOperationException("Transition d'état invalide.")
            ex.Data("EtatActuel") = "Brouillon"
            ex.Data("EtatDemande") = "Expédiée"
            Throw ex
        Catch ex As InvalidOperationException
            Console.WriteLine($"  EtatActuel = {ex.Data("EtatActuel")} ; EtatDemande = {ex.Data("EtatDemande")}")
        End Try
        Console.WriteLine()
    End Sub

End Module
