' ============================================================================
'  Section 7.1 : ADO.NET
'  Description : Exemple complet reprenant tous les extraits de la section,
'                portés sur SQLite (préfixe Sqlite… au lieu de Sql…) :
'                  · connexion dans un Using (libération garantie), OpenAsync ;
'                  · commandes PARAMÉTRÉES (la règle d'or anti-injection) :
'                    démonstration qu'une saisie malveillante n'est PAS
'                    interprétée comme du SQL ;
'                  · ExecuteScalar (COUNT), ExecuteNonQuery (UPDATE) ;
'                  · DataReader : GetOrdinal hors boucle, accesseurs typés,
'                    IsDBNull pour les colonnes nullables ;
'                  · DataSet/DataTable + Field(Of T) (gère le NULL) ;
'                  · transaction : Commit en fin de Try, capture de
'                    l'exception, Rollback HORS du Catch (Await y est interdit).
'  Fichier source : 01-adonet.md
'  Compilation    : dotnet run   (restaure Microsoft.Data.Sqlite)
'  Note           : la base est un fichier SQLite temporaire, recréé à chaque
'                   exécution, puis supprimé à la fin.
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Runtime.ExceptionServices
Imports Microsoft.Data.Sqlite

Module Program

    Private ReadOnly CheminBase As String = Path.Combine(Path.GetTempPath(), "adonet-demo.db")
    Private ReadOnly ChaineConnexion As String = $"Data Source={CheminBase}"

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        If File.Exists(CheminBase) Then File.Delete(CheminBase)
        Await CreerSchemaEtDonneesAsync()

        Await DemoScalaire()
        Await DemoParametres()
        Await DemoDataReader()
        DemoDataTable()
        Await DemoTransaction()

        ' SQLite met les connexions en pool : on vide le pool pour libérer le
        ' fichier avant de pouvoir le supprimer.
        SqliteConnection.ClearAllPools()
        File.Delete(CheminBase)
        Console.WriteLine()
        Console.WriteLine("Base temporaire supprimée.")
    End Function

    ' ---- Création du schéma + jeu de données ---------------------------------
    Private Async Function CreerSchemaEtDonneesAsync() As Task
        Using connexion As New SqliteConnection(ChaineConnexion)
            Await connexion.OpenAsync()
            Using commande = connexion.CreateCommand()
                commande.CommandText =
                    "CREATE TABLE Clients (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Email TEXT,
                        EstActif INTEGER NOT NULL,
                        DateInscription TEXT NOT NULL);
                     CREATE TABLE Comptes (Id INTEGER PRIMARY KEY, Solde REAL NOT NULL);"
                commande.ExecuteNonQuery()
            End Using

            ' Insertions paramétrées
            Dim clients = {("Dupont", "dupont@exemple.fr", 1), ("Martin", CType(Nothing, String), 0), ("Durand", "durand@exemple.fr", 1)}
            For Each c In clients
                Using cmd = connexion.CreateCommand()
                    cmd.CommandText = "INSERT INTO Clients (Nom, Email, EstActif, DateInscription) VALUES (@nom, @email, @actif, @date)"
                    cmd.Parameters.Add("@nom", SqliteType.Text).Value = c.Item1
                    cmd.Parameters.Add("@email", SqliteType.Text).Value = If(c.Item2, CObj(DBNull.Value))
                    cmd.Parameters.Add("@actif", SqliteType.Integer).Value = c.Item3
                    cmd.Parameters.Add("@date", SqliteType.Text).Value = "2025-03-15"
                    cmd.ExecuteNonQuery()
                End Using
            Next

            Using cmd = connexion.CreateCommand()
                cmd.CommandText = "INSERT INTO Comptes (Id, Solde) VALUES (1, 1000), (2, 500)"
                cmd.ExecuteNonQuery()
            End Using
        End Using
        Console.WriteLine("== Schéma créé (3 clients, 2 comptes) ==")
    End Function

    ' ---- ExecuteScalar : une valeur unique -----------------------------------
    Private Async Function DemoScalaire() As Task
        Console.WriteLine()
        Console.WriteLine("== ExecuteScalar (COUNT) ==")
        Using connexion As New SqliteConnection(ChaineConnexion),
              commande As New SqliteCommand("SELECT COUNT(*) FROM Clients WHERE EstActif = 1", connexion)
            Await connexion.OpenAsync()
            Dim resultat = Await commande.ExecuteScalarAsync()
            Console.WriteLine($"Clients actifs : {Convert.ToInt32(resultat)}")
        End Using
    End Function

    ' ---- Commandes paramétrées : anti-injection -------------------------------
    Private Async Function DemoParametres() As Task
        Console.WriteLine()
        Console.WriteLine("== Commandes paramétrées (anti-injection SQL) ==")

        ' Une saisie malveillante : avec une commande paramétrée, elle n'est
        ' PAS interprétée comme du SQL — juste cherchée comme un nom littéral.
        Dim saisieMalveillante = "'; DROP TABLE Clients; --"
        Using connexion As New SqliteConnection(ChaineConnexion),
              commande As New SqliteCommand("SELECT COUNT(*) FROM Clients WHERE Nom = @nom", connexion)
            commande.Parameters.Add("@nom", SqliteType.Text).Value = saisieMalveillante
            Await connexion.OpenAsync()
            Dim n = Convert.ToInt32(Await commande.ExecuteScalarAsync())
            Console.WriteLine($"Recherche du nom littéral « {saisieMalveillante} » -> {n} résultat(s)")
        End Using

        ' La table existe toujours : l'injection a été neutralisée.
        Using connexion As New SqliteConnection(ChaineConnexion),
              commande As New SqliteCommand("SELECT COUNT(*) FROM Clients", connexion)
            Await connexion.OpenAsync()
            Console.WriteLine($"Table Clients intacte après la tentative : {Convert.ToInt32(Await commande.ExecuteScalarAsync())} lignes")
        End Using

        ' ExecuteNonQuery : UPDATE paramétré
        Using connexion As New SqliteConnection(ChaineConnexion),
              commande As New SqliteCommand("UPDATE Clients SET EstActif = 1 WHERE Nom = @nom", connexion)
            commande.Parameters.Add("@nom", SqliteType.Text).Value = "Martin"
            Await connexion.OpenAsync()
            Dim lignes = Await commande.ExecuteNonQueryAsync()
            Console.WriteLine($"UPDATE (activer Martin) -> {lignes} ligne(s) affectée(s)")
        End Using
    End Function

    ' ---- DataReader (mode connecté) -------------------------------------------
    Private Async Function DemoDataReader() As Task
        Console.WriteLine()
        Console.WriteLine("== DataReader (lecture en flux) ==")
        Dim clients As New List(Of (Id As Integer, Nom As String, Email As String))

        Using connexion As New SqliteConnection(ChaineConnexion),
              commande As New SqliteCommand("SELECT Id, Nom, Email FROM Clients ORDER BY Nom", connexion)
            Await connexion.OpenAsync()
            Using lecteur = Await commande.ExecuteReaderAsync()
                ' Index résolus UNE SEULE FOIS, hors de la boucle
                Dim ordId = lecteur.GetOrdinal("Id")
                Dim ordNom = lecteur.GetOrdinal("Nom")
                Dim ordEmail = lecteur.GetOrdinal("Email")
                While Await lecteur.ReadAsync()
                    ' IsDBNull AVANT de lire une colonne nullable
                    Dim email = If(lecteur.IsDBNull(ordEmail), Nothing, lecteur.GetString(ordEmail))
                    clients.Add((lecteur.GetInt32(ordId), lecteur.GetString(ordNom), email))
                End While
            End Using
        End Using

        For Each c In clients
            Console.WriteLine($"  {c.Id} — {c.Nom} — {If(c.Email, "(sans e-mail)")}")
        Next
    End Function

    ' ---- DataSet / DataTable (mode déconnecté) --------------------------------
    Private Sub DemoDataTable()
        Console.WriteLine()
        Console.WriteLine("== DataTable + Field(Of T) (mode déconnecté) ==")

        Dim table As New DataTable("Clients")
        Using connexion As New SqliteConnection(ChaineConnexion)
            connexion.Open()
            Using commande As New SqliteCommand("SELECT Id, Nom, Email FROM Clients", connexion)
                Using lecteur = commande.ExecuteReader()
                    table.Load(lecteur)   ' charge la table en mémoire, puis on travaille hors-ligne
                End Using
            End Using
        End Using

        For Each ligne As DataRow In table.Rows
            Dim id = ligne.Field(Of Long)("Id")             ' SQLite : INTEGER -> Int64
            Dim nom = ligne.Field(Of String)("Nom")
            Dim email = ligne.Field(Of String)("Email")     ' Nothing si NULL (pas de DBNull à gérer)
            Console.WriteLine($"  {id} — {nom} — {If(email, "(sans e-mail)")}")
        Next
        Console.WriteLine($"DataTable contient {table.Rows.Count} lignes (connexion déjà fermée).")
    End Sub

    ' ---- Transaction (atomicité) ----------------------------------------------
    Private Async Function DemoTransaction() As Task
        Console.WriteLine()
        Console.WriteLine("== Transaction (virement atomique) ==")

        Await TransfererAsync(1, 2, 200D)
        Await AfficherSoldes()

        ' Tentative invalide (compte inexistant après débit) : on annule tout.
        Try
            Await TransfererAsync(1, 999, 100D, forcerEchec:=True)
        Catch ex As InvalidOperationException
            Console.WriteLine($"Virement annulé : {ex.Message}")
        End Try
        Await AfficherSoldes()   ' soldes inchangés -> rollback effectif
    End Function

    Private Async Function TransfererAsync(idSource As Integer, idDest As Integer, montant As Decimal,
                                           Optional forcerEchec As Boolean = False) As Task
        Using connexion As New SqliteConnection(ChaineConnexion)
            Await connexion.OpenAsync()
            Using transaction = Await connexion.BeginTransactionAsync()
                Dim erreur As ExceptionDispatchInfo = Nothing
                Try
                    Await ExecuterAsync(connexion, transaction, "UPDATE Comptes SET Solde = Solde - @m WHERE Id = @id", montant, idSource)
                    Await ExecuterAsync(connexion, transaction, "UPDATE Comptes SET Solde = Solde + @m WHERE Id = @id", montant, idDest)
                    If forcerEchec Then Throw New InvalidOperationException("le compte destination n'existe pas")
                    Await transaction.CommitAsync()
                Catch ex As Exception
                    erreur = ExceptionDispatchInfo.Capture(ex)   ' capture (pas d'Await dans le Catch)
                End Try

                If erreur IsNot Nothing Then
                    Await transaction.RollbackAsync()             ' Await autorisé hors du Catch
                    erreur.Throw()
                End If
            End Using
        End Using
    End Function

    Private Async Function ExecuterAsync(connexion As SqliteConnection, transaction As Common.DbTransaction,
                                         sql As String, montant As Decimal, id As Integer) As Task
        Using cmd As New SqliteCommand(sql, connexion, CType(transaction, SqliteTransaction))
            cmd.Parameters.Add("@m", SqliteType.Real).Value = montant
            cmd.Parameters.Add("@id", SqliteType.Integer).Value = id
            Await cmd.ExecuteNonQueryAsync()
        End Using
    End Function

    Private Async Function AfficherSoldes() As Task
        Using connexion As New SqliteConnection(ChaineConnexion),
              commande As New SqliteCommand("SELECT Id, Solde FROM Comptes ORDER BY Id", connexion)
            Await connexion.OpenAsync()
            Using lecteur = Await commande.ExecuteReaderAsync()
                Dim soldes As New List(Of String)
                While Await lecteur.ReadAsync()
                    soldes.Add($"compte {lecteur.GetInt32(0)} = {lecteur.GetDouble(1)}")
                End While
                Console.WriteLine("  Soldes : " & String.Join(" ; ", soldes))
            End Using
        End Using
    End Function

End Module
