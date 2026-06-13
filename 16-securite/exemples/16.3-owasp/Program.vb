' ============================================================================
'  Section 16.3 : OWASP — démonstration (injection, validation, encodage)
'  Description : (1) Crée un SQLite EN MÉMOIRE avec un utilisateur, puis compare
'                l'authentification VULNÉRABLE (concaténée) et PARAMÉTRÉE face à
'                l'injection « ' OR '1'='1 » : la première est contournée, la
'                seconde résiste. (2) Valide un DTO correct puis un DTO fautif
'                (DataAnnotations). (3) Encode une charge XSS selon trois
'                contextes (HTML / URL / JavaScript). Toutes les sorties sont
'                vérifiables.
'  Fichier source : 03-owasp.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Text.Encodings.Web
Imports Microsoft.Data.Sqlite

Module Program

    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 16.3 OWASP : injection SQL, validation, encodage ===")
        Console.WriteLine()

        DemoInjectionSql()
        Console.WriteLine()
        DemoValidation()
        Console.WriteLine()
        DemoEncodage()
    End Sub

    ' ----- (1) Injection SQL : vulnérable vs paramétré -------------------------
    Private Sub DemoInjectionSql()
        Console.WriteLine("--- (1) Injection SQL ---")
        Using cnx As New SqliteConnection("Data Source=:memory:")
            cnx.Open()
            Using cmd As SqliteCommand = cnx.CreateCommand()
                cmd.CommandText =
                    "CREATE TABLE Utilisateurs (Nom TEXT, MotDePasse TEXT);" &
                    "INSERT INTO Utilisateurs (Nom, MotDePasse) VALUES ('alice', 'S3cret!');"
                cmd.ExecuteNonQuery()
            End Using

            Const injection As String = "' OR '1'='1"

            ' Connexion légitime (les deux fonctions l'acceptent).
            Console.WriteLine($"  Légitime alice/S3cret!  → vulnérable={InjectionSql.AuthentifierVulnerable(cnx, "alice", "S3cret!")}, paramétré={InjectionSql.AuthentifierParametre(cnx, "alice", "S3cret!")}")

            ' Attaque : mot de passe = ' OR '1'='1  (l'attaquant ne connaît pas le mot de passe)
            Dim contourneVuln As Boolean = InjectionSql.AuthentifierVulnerable(cnx, "alice", injection)
            Dim contourneParam As Boolean = InjectionSql.AuthentifierParametre(cnx, "alice", injection)
            Console.WriteLine($"  Attaque  alice/{injection}  → vulnérable={contourneVuln} (CONTOURNÉ !), paramétré={contourneParam} (bloqué)")
        End Using
    End Sub

    ' ----- (2) Validation d'entrée (DataAnnotations) ---------------------------
    Private Sub DemoValidation()
        Console.WriteLine("--- (2) Validation d'entrée (liste blanche) ---")

        Dim bon As New CompteDto With {.Nom = "alice", .Courriel = "alice@exemple.fr", .Age = 30}
        Dim erreursBon = ValidationModele.Valider(bon)
        Console.WriteLine($"  DTO valide   → {erreursBon.Count} erreur(s)")

        Dim mauvais As New CompteDto With {.Nom = "ab", .Courriel = "pas-un-courriel", .Age = 5}
        Dim erreursMauvais = ValidationModele.Valider(mauvais)
        Console.WriteLine($"  DTO invalide → {erreursMauvais.Count} erreur(s) :")
        For Each message In erreursMauvais
            Console.WriteLine($"      - {message}")
        Next
    End Sub

    ' ----- (3) Encodage de sortie contextuel -----------------------------------
    Private Sub DemoEncodage()
        Console.WriteLine("--- (3) Encodage de sortie contextuel ---")
        Const payload As String = "<script>alert('xss')</script>"
        Console.WriteLine($"  Entrée brute : {payload}")
        Console.WriteLine($"  HTML       → {HtmlEncoder.Default.Encode(payload)}")
        Console.WriteLine($"  URL        → {UrlEncoder.Default.Encode(payload)}")
        Console.WriteLine($"  JavaScript → {JavaScriptEncoder.Default.Encode(payload)}")
    End Sub

End Module
