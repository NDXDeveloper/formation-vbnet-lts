' ============================================================================
'  Section 7.3 : Fournisseurs de bases de données
'  Description : Illustre le « modèle de fournisseurs » de la section : le
'                code applicatif change à peine d'un SGBD à l'autre — seules
'                la chaîne de connexion et la fabrique de connexion varient.
'                On exécute réellement SQLite sous deux formes (fichier et
'                :memory:), et l'on affiche les chaînes de connexion typiques
'                des quatre SGBD du cours.
'  Fichier source : 03-fournisseurs.md
'  Compilation    : dotnet run
' ============================================================================

Imports System
Imports System.IO
Imports Microsoft.Data.Sqlite

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoSqliteMemoire()
        DemoSqliteFichier()
        AfficherChainesConnexion()
    End Sub

    ' ---- SQLite en mémoire (idéal pour les tests) ----------------------------
    Private Sub DemoSqliteMemoire()
        Console.WriteLine("== SQLite en mémoire (Data Source=:memory:) ==")
        ' La base en mémoire n'existe que tant que LA connexion reste ouverte.
        Using connexion As New SqliteConnection("Data Source=:memory:")
            connexion.Open()
            ExecuterDemo(connexion, "en mémoire")
        End Using
    End Sub

    ' ---- SQLite fichier (idéal pour le bureau mono-utilisateur) --------------
    Private Sub DemoSqliteFichier()
        Console.WriteLine()
        Console.WriteLine("== SQLite fichier (Data Source=...) ==")
        Dim chemin = Path.Combine(Path.GetTempPath(), "fournisseurs-demo.db")
        If File.Exists(chemin) Then File.Delete(chemin)

        Using connexion As New SqliteConnection($"Data Source={chemin}")
            connexion.Open()
            ExecuterDemo(connexion, "sur disque")
        End Using

        SqliteConnection.ClearAllPools()
        Dim taille = New FileInfo(chemin).Length
        Console.WriteLine($"Fichier créé : {Path.GetFileName(chemin)} ({taille} octets) — un simple fichier, zéro serveur.")
        File.Delete(chemin)
    End Sub

    ' Le MÊME code de démonstration, quelle que soit la source.
    Private Sub ExecuterDemo(connexion As SqliteConnection, libelle As String)
        Using cmd = connexion.CreateCommand()
            cmd.CommandText = "CREATE TABLE Produits (Id INTEGER PRIMARY KEY, Libelle TEXT, Prix REAL)"
            cmd.ExecuteNonQuery()
        End Using
        Using cmd = connexion.CreateCommand()
            cmd.CommandText = "INSERT INTO Produits VALUES (1,'Clavier',45),(2,'Écran',220),(3,'Souris',25)"
            cmd.ExecuteNonQuery()
        End Using
        Using cmd = connexion.CreateCommand()
            cmd.CommandText = "SELECT COUNT(*), SUM(Prix) FROM Produits"
            Using lecteur = cmd.ExecuteReader()
                lecteur.Read()
                Console.WriteLine($"Base {libelle} : {lecteur.GetInt32(0)} produits, total {lecteur.GetDouble(1)} €")
            End Using
        End Using
    End Sub

    ' ---- Chaînes de connexion des autres SGBD (documentaire) -----------------
    Private Sub AfficherChainesConnexion()
        Console.WriteLine()
        Console.WriteLine("== Chaînes de connexion typiques (cf. cours) ==")
        Dim chaines = New (Sgbd As String, Methode As String, Chaine As String)() {
            ("SQL Server / LocalDB", "UseSqlServer", "Server=(localdb)\MSSQLLocalDB;Database=Boutique;Trusted_Connection=True;TrustServerCertificate=True;"),
            ("SQLite", "UseSqlite", "Data Source=boutique.db"),
            ("MySQL / MariaDB", "UseMySql", "Server=localhost;Database=boutique;User=app;Password=********;"),
            ("PostgreSQL (Npgsql)", "UseNpgsql", "Host=localhost;Port=5432;Database=boutique;Username=postgres;Password=********;")
        }
        For Each c In chaines
            Console.WriteLine($"  {c.Sgbd,-22} EF Core : {c.Methode}")
            Console.WriteLine($"  {"",-22} {c.Chaine}")
        Next
        Console.WriteLine()
        Console.WriteLine("Le code (entités, DbContext, LINQ) reste identique : seuls le paquet,")
        Console.WriteLine("la chaîne de connexion et l'appel UseXxx() changent.")
    End Sub

End Module
