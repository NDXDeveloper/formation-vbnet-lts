' ============================================================================
'  Section 16.3 : OWASP — injection SQL (A05 / A03)
'  Description : Deux implémentations d'une « authentification » comparées :
'                - Vulnerable : la requête est CONSTRUITE PAR CONCATÉNATION de
'                  l'entrée utilisateur (opérateur & / interpolation). Le mot de
'                  passe « ' OR '1'='1 » réécrit la clause WHERE et CONTOURNE le
'                  contrôle (l'attaquant entre sans connaître le mot de passe).
'                - Parametree : la requête utilise des PARAMÈTRES (@nom, @mdp) ;
'                  l'entrée n'est jamais interprétée comme du SQL → attaque
'                  NEUTRALISÉE. C'est la seule forme acceptable.
'  Fichier source : 03-owasp.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports Microsoft.Data.Sqlite

''' <summary>Compare une requête concaténée (vulnérable) et paramétrée (sûre).</summary>
Public Module InjectionSql

    ''' <summary>VULNÉRABLE — ne JAMAIS écrire cela : entrée concaténée dans le SQL.</summary>
    Public Function AuthentifierVulnerable(cnx As SqliteConnection, nom As String, motDePasse As String) As Boolean
        ' ⚠️ Anti-exemple : l'entrée fait partie du texte de la requête.
        Dim sql As String =
            $"SELECT COUNT(*) FROM Utilisateurs WHERE Nom = '{nom}' AND MotDePasse = '{motDePasse}'"
        Using cmd As SqliteCommand = cnx.CreateCommand()
            cmd.CommandText = sql
            Return Convert.ToInt64(cmd.ExecuteScalar()) > 0
        End Using
    End Function

    ''' <summary>SÛRE — l'entrée passe par des paramètres, jamais par le texte SQL.</summary>
    Public Function AuthentifierParametre(cnx As SqliteConnection, nom As String, motDePasse As String) As Boolean
        Using cmd As SqliteCommand = cnx.CreateCommand()
            cmd.CommandText = "SELECT COUNT(*) FROM Utilisateurs WHERE Nom = @nom AND MotDePasse = @mdp"
            ' En production sur SQL Server : Parameters.Add(nom, SqlDbType.NVarChar, 20) (type explicite)
            ' plutôt que AddWithValue. Ici, SQLite est typé dynamiquement.
            cmd.Parameters.AddWithValue("@nom", nom)
            cmd.Parameters.AddWithValue("@mdp", motDePasse)
            Return Convert.ToInt64(cmd.ExecuteScalar()) > 0
        End Using
    End Function

End Module
