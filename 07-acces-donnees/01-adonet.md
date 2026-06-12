🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 7.1 ADO.NET

> **Module 7 — Accès aux données** · Section 1
> Socle de l'accès aux données en .NET · Connexions, commandes paramétrées, `DataReader`, `DataSet` / `DataTable`, transactions

---

## 🧭 Introduction : ADO.NET, la fondation

**ADO.NET** est la couche d'accès aux données **de plus bas niveau** de .NET. C'est la fondation sur laquelle tout le reste est bâti : Entity Framework Core (§ 7.2) génère lui-même des commandes ADO.NET, et la plupart des micro-ORM s'appuient dessus. Comprendre ADO.NET, c'est donc comprendre ce qui se passe réellement « sous le capot » de chaque requête.

Pourquoi en avoir besoin si un ORM fait le travail ? Pour trois raisons concrètes :

- **le contrôle total** : vous écrivez le SQL exact qui part vers la base, sans surprise ni traduction ;
- **la performance** : pour des lectures massives ou des traitements par lots, ADO.NET reste imbattable en allocation mémoire et en débit ;
- **le legacy** : une grande part du code VB.NET existant utilise ADO.NET directement — savoir le lire et le maintenir est indispensable.

ADO.NET est constitué de **bibliothèques .NET ordinaires**, donc pleinement utilisables en VB.NET sans aucune restriction. Tous les exemples ci-dessous sont écrits en supposant `Option Strict On` et `Option Infer On` (la configuration recommandée pour un projet neuf, voir § 2.1).

---

## 🏗️ Le modèle de fournisseurs (*provider model*)

ADO.NET ne parle pas directement « SQL Server » ou « PostgreSQL ». Il définit un ensemble de **classes de base abstraites** dans l'espace de noms `System.Data.Common` — `DbConnection`, `DbCommand`, `DbDataReader`, `DbParameter`, `DbTransaction`, `DbDataAdapter` — que chaque **fournisseur** (provider) implémente pour un SGBD donné.

| SGBD | Paquet NuGet | Préfixe des classes | Remarque |
|---|---|---|---|
| **SQL Server / Azure SQL** | `Microsoft.Data.SqlClient` | `Sql…` | Le fournisseur de référence |
| **SQLite** | `Microsoft.Data.Sqlite` | `Sqlite…` | Idéal pour les applications locales |
| **PostgreSQL** | `Npgsql` | `Npgsql…` | Très complet et performant |
| **MySQL / MariaDB** | `MySqlConnector` | `MySql…` | Recommandé (alternative moderne à `MySql.Data`) |
| **OLE DB** | `System.Data.OleDb` | `OleDb…` | Accès, sources diverses — ⚠️ Windows uniquement |
| **ODBC** | `System.Data.Odbc` | `Odbc…` | Tout pilote ODBC |

Comme tous ces fournisseurs partagent la même architecture, **les concepts appris ici sont transposables** : passer de SQL Server à SQLite revient surtout à changer `Sql` en `Sqlite` et la chaîne de connexion.

> ### ⚠️ Point de modernisation essentiel : `Microsoft.Data.SqlClient`, pas `System.Data.SqlClient`
>
> Il existe deux fournisseurs SQL Server, et c'est une source de confusion fréquente :
>
> - **`System.Data.SqlClient`** — l'ancien, intégré historiquement au framework. Il est aujourd'hui en **mode maintenance** : il ne reçoit plus de nouvelles fonctionnalités.
> - **`Microsoft.Data.SqlClient`** — le **fournisseur moderne**, distribué via NuGet, qui concentre tous les nouveaux développements (Always Encrypted, Azure AD, dernières versions du protocole TLS…).
>
> **Pour tout projet .NET 10, utilisez `Microsoft.Data.SqlClient`.** Les exemples de cette section l'emploient via `Imports Microsoft.Data.SqlClient`.

---

## 🔀 Les deux visages d'ADO.NET : connecté et déconnecté

ADO.NET propose deux modèles de travail, qu'il faut savoir distinguer dès le départ.

| | **Mode connecté** | **Mode déconnecté** |
|---|---|---|
| Objet central | `DataReader` | `DataSet` / `DataTable` (+ `DataAdapter`) |
| Connexion | Maintenue **ouverte** pendant la lecture | Ouverte le temps du remplissage, puis **fermée** |
| Sens de lecture | Avant uniquement (*forward-only*), lecture seule | Navigation libre, modification possible |
| Mémoire | Très faible (flux, une ligne à la fois) | Charge tout en mémoire |
| Cas idéal | Grandes lectures, performance, serveur | Liaison de données WinForms, travail hors-ligne, petits volumes |

On les détaille respectivement dans les sections **4** (`DataReader`) et **5** (`DataSet` / `DataTable`).

---

## 🔌 1. La connexion

Tout commence par une connexion à la base, décrite par une **chaîne de connexion**.

```vb
Imports Microsoft.Data.SqlClient

Public Class ServiceClients

    ' En production : à externaliser (appsettings.json, Key Vault…) — voir module 16
    Private Const ChaineConnexion As String =
        "Server=(localdb)\MSSQLLocalDB;Database=Boutique;Trusted_Connection=True;TrustServerCertificate=True;"

    Public Async Function VerifierConnexionAsync() As Task
        Using connexion As New SqlConnection(ChaineConnexion)
            Await connexion.OpenAsync()
            Console.WriteLine($"Connecté à la base : {connexion.Database}")
        End Using ' Dispose() est appelé ici, même en cas d'exception
    End Function

End Class
```

Trois points structurants :

**Le bloc `Using` est non négociable.** Une connexion est une ressource précieuse (et limitée). Le bloc `Using` garantit l'appel de `Dispose()` — donc la fermeture de la connexion — **même si une exception survient**. Sans lui, des connexions « fuient » et finissent par épuiser le serveur. C'est l'erreur la plus coûteuse du débutant en ADO.NET.

**Le pool de connexions est automatique.** Fermer une connexion ne la détruit pas réellement : le fournisseur la **renvoie dans un pool** pour la réutiliser. Ouvrir et fermer fréquemment n'est donc pas un problème ; au contraire, la bonne pratique est d'**ouvrir au plus tard et fermer au plus tôt** — ce que le bloc `Using` fait naturellement.

**Préférez `OpenAsync()`** dans une application à interface graphique (pour ne pas figer l'UI) ou serveur (pour libérer le thread). Voir le module 4 sur l'asynchronisme.

> ### ⚠️ Le piège du certificat avec `Microsoft.Data.SqlClient`
> Depuis sa version 4.0, ce fournisseur chiffre la connexion **par défaut** (`Encrypt=True`). Sur un serveur de développement (LocalDB, instance locale) au certificat auto-signé, vous obtiendrez une erreur de chaîne de certification. En développement, on ajoute `TrustServerCertificate=True` (comme ci-dessus). **En production, configurez un vrai certificat de confiance** plutôt que de désactiver la vérification.

---

## ⚙️ 2. Les commandes

Une fois connecté, une **commande** (`SqlCommand`) porte l'instruction SQL — ou le nom d'une procédure stockée — à exécuter. Trois méthodes d'exécution couvrent tous les besoins :

| Méthode | Renvoie | Pour quoi |
|---|---|---|
| `ExecuteReader()` | un `DataReader` | un `SELECT` qui retourne **plusieurs lignes** |
| `ExecuteScalar()` | un `Object` (1ʳᵉ colonne, 1ʳᵉ ligne) | une **valeur unique** : `COUNT`, `MAX`, un identifiant… |
| `ExecuteNonQuery()` | un `Integer` (lignes affectées) | un `INSERT` / `UPDATE` / `DELETE` ou du DDL |

Chacune possède sa variante asynchrone (`ExecuteReaderAsync`, `ExecuteScalarAsync`, `ExecuteNonQueryAsync`).

**`ExecuteScalar` — récupérer une valeur :**

```vb
Public Async Function CompterClientsActifsAsync() As Task(Of Integer)
    Dim sql = "SELECT COUNT(*) FROM Clients WHERE EstActif = 1"

    Using connexion As New SqlConnection(ChaineConnexion),
          commande As New SqlCommand(sql, connexion)

        Await connexion.OpenAsync()
        Dim resultat = Await commande.ExecuteScalarAsync()
        Return Convert.ToInt32(resultat)
    End Using
End Function
```

*Remarque : `ExecuteScalar` renvoie `Nothing` si aucune ligne n'est retournée et `DBNull.Value` si la valeur est NULL — à anticiper selon la requête. Pour un `COUNT(*)`, le résultat est toujours présent.*

**`ExecuteNonQuery` — modifier des données :**

```vb
Public Async Function ActiverClientAsync(idClient As Integer) As Task(Of Boolean)
    Dim sql = "UPDATE Clients SET EstActif = 1 WHERE Id = @id"

    Using connexion As New SqlConnection(ChaineConnexion),
          commande As New SqlCommand(sql, connexion)

        commande.Parameters.Add("@id", SqlDbType.Int).Value = idClient
        Await connexion.OpenAsync()

        Dim lignesAffectees = Await commande.ExecuteNonQueryAsync()
        Return lignesAffectees > 0 ' False si aucun client avec cet Id
    End Using
End Function
```

Notez la propriété `CommandType` : par défaut `CommandType.Text` (du SQL brut). Pour appeler une procédure stockée, on passe `commande.CommandType = CommandType.StoredProcedure` et `CommandText` reçoit alors simplement le nom de la procédure.

---

## 🛡️ 3. Les commandes paramétrées — la règle d'or

C'est **le point le plus important de toute cette section.** La tentation du débutant est de construire le SQL en concaténant des chaînes :

```vb
' ❌❌❌ À NE JAMAIS FAIRE — faille d'injection SQL
Dim sql = "SELECT * FROM Clients WHERE Nom = '" & nomSaisi & "'"
```

Si `nomSaisi` vaut `Martin`, tout va bien. Mais si un utilisateur malveillant saisit :

```
'; DROP TABLE Clients; --
```

…la requête envoyée à la base devient deux instructions, dont une qui **supprime la table**. C'est l'**injection SQL**, l'une des vulnérabilités les plus anciennes et les plus dévastatrices (n°1 historique de l'OWASP — voir § 16.3).

**La parade est simple et systématique : les paramètres.** On n'insère jamais les valeurs dans le texte SQL ; on insère des **emplacements nommés** (`@nom`), et l'on fournit les valeurs séparément. Le fournisseur les transmet à la base de façon typée, où elles ne peuvent **jamais** être interprétées comme du code.

```vb
' ✅ Commande paramétrée — sûre
Dim sql = "SELECT Id, Nom, Email FROM Clients WHERE Nom = @nom AND EstActif = @actif"

Using commande As New SqlCommand(sql, connexion)
    commande.Parameters.Add("@nom", SqlDbType.NVarChar, 100).Value = nomSaisi
    commande.Parameters.Add("@actif", SqlDbType.Bit).Value = True
    ' … exécution
End Using
```

> ### `Add` ou `AddWithValue` ?
> Vous rencontrerez souvent `Parameters.AddWithValue("@nom", nomSaisi)`, plus court car il **infère** le type du paramètre. C'est pratique mais **risqué** : l'inférence peut se tromper (le grand classique : une chaîne `.NET` interprétée en `NVarChar` alors que la colonne est `VarChar`, ce qui empêche l'utilisation de l'index et **dégrade les performances**). La bonne pratique est de **spécifier explicitement le type** (et la longueur pour les chaînes) avec `Add`, comme ci-dessus. Réservez `AddWithValue` au prototypage rapide.

Le préfixe `@` est celui de SQL Server. PostgreSQL et Oracle utilisent traditionnellement `:` pour les paramètres nommés (Npgsql accepte aussi `@`), et OLE DB / ODBC fonctionnent par paramètres **positionnels** (`?`). Le principe — ne jamais concaténer — reste identique partout.

---

## 📖 4. Le `DataReader` (mode connecté)

Le `DataReader` est un curseur **rapide, en lecture seule, en avant uniquement**. Il diffuse les résultats **ligne par ligne** sans tout charger en mémoire : c'est l'outil idéal pour parcourir de gros volumes.

Supposons une classe métier simple :

```vb
Public Class Client
    Public Property Id As Integer
    Public Property Nom As String
    Public Property Email As String        ' peut être NULL en base
    Public Property DateInscription As Date
End Class
```

La lecture type :

```vb
Public Async Function ListerClientsAsync() As Task(Of List(Of Client))
    Dim sql = "SELECT Id, Nom, Email, DateInscription FROM Clients ORDER BY Nom"
    Dim clients As New List(Of Client)

    Using connexion As New SqlConnection(ChaineConnexion),
          commande As New SqlCommand(sql, connexion)

        Await connexion.OpenAsync()

        Using lecteur = Await commande.ExecuteReaderAsync()

            ' Résoudre les index de colonnes UNE SEULE FOIS, hors de la boucle
            Dim ordId = lecteur.GetOrdinal("Id")
            Dim ordNom = lecteur.GetOrdinal("Nom")
            Dim ordEmail = lecteur.GetOrdinal("Email")
            Dim ordDate = lecteur.GetOrdinal("DateInscription")

            While Await lecteur.ReadAsync()
                clients.Add(New Client With {
                    .Id = lecteur.GetInt32(ordId),
                    .Nom = lecteur.GetString(ordNom),
                    .Email = If(lecteur.IsDBNull(ordEmail), Nothing, lecteur.GetString(ordEmail)),
                    .DateInscription = lecteur.GetDateTime(ordDate)
                })
            End While

        End Using ' ferme le lecteur
    End Using

    Return clients
End Function
```

Les bonnes pratiques visibles ici :

**Les accesseurs typés** (`GetInt32`, `GetString`, `GetDateTime`…) sont **plus rapides** que l'indexeur générique `lecteur("Nom")` : ils évitent le *boxing* (l'emballage en `Object`) et la conversion. Privilégiez-les.

**`GetOrdinal` une seule fois.** Récupérer l'index d'une colonne par son nom a un coût ; on le fait **avant** la boucle, pas à chaque ligne.

> ### ⚠️ Le piège classique : `DBNull.Value` n'est pas `Nothing`
> Quand une colonne vaut NULL en base, le `DataReader` renvoie `DBNull.Value` — un objet sentinelle — et **non** `Nothing`. Tester `If valeur Is Nothing` ne détectera **pas** le NULL. Pire, appeler `GetString` sur une colonne NULL lève une exception. On teste donc systématiquement avec **`IsDBNull(ordinal)`** avant de lire une colonne susceptible d'être nulle, comme pour `Email` ci-dessus.

**Plusieurs jeux de résultats.** Si votre commande renvoie plusieurs `SELECT`, `lecteur.NextResult()` (ou `NextResultAsync`) passe au jeu suivant.

*Note sur l'asynchronisme :* après `Await ReadAsync()`, la ligne courante est déjà en mémoire ; les accès `GetXxx` qui suivent peuvent rester synchrones sans pénalité. Pour des colonnes très volumineuses (gros `varbinary`/`text`), des variantes `Await GetFieldValueAsync(Of T)(ord)` et `Await IsDBNullAsync(ord)` existent.

---

## 🗃️ 5. `DataSet` / `DataTable` (mode déconnecté)

Le modèle déconnecté charge les données en mémoire, **ferme la connexion**, puis vous laisse travailler hors-ligne — naviguer, trier, filtrer, modifier — avant, éventuellement, de **répercuter** les changements en base.

- une **`DataTable`** est une table en mémoire (lignes `DataRow`, colonnes `DataColumn`) ;
- un **`DataSet`** est un ensemble de `DataTable` reliées entre elles — une petite base relationnelle en mémoire ;
- un **`DataAdapter`** (`SqlDataAdapter`) fait le pont entre la base et ces structures.

**Charger une `DataTable` :**

```vb
Public Function ChargerClients() As DataTable
    Dim sql = "SELECT Id, Nom, Email FROM Clients"
    Dim table As New DataTable("Clients")

    Using connexion As New SqlConnection(ChaineConnexion),
          adaptateur As New SqlDataAdapter(sql, connexion)

        ' Fill ouvre PUIS ferme la connexion automatiquement
        adaptateur.Fill(table)
    End Using

    Return table
End Function
```

`Fill` se charge de tout le cycle de la connexion. À l'issue, la `DataTable` vit de manière autonome.

**Lire les lignes — l'idiome VB.NET élégant : `Field(Of T)`**

```vb
For Each ligne As DataRow In table.Rows
    Dim id = ligne.Field(Of Integer)("Id")
    Dim nom = ligne.Field(Of String)("Nom")
    Dim email = ligne.Field(Of String)("Email")   ' renvoie Nothing si NULL — pas de DBNull à gérer
    Console.WriteLine($"{id} — {nom} — {If(email, "(sans e-mail)")}")
Next
```

La méthode d'extension **`Field(Of T)`** est la façon recommandée d'accéder aux valeurs : elle est **typée** (pas de transtypage manuel) et **gère le NULL proprement** — pour un type référence elle renvoie `Nothing`, et pour un type valeur nullable on écrit `ligne.Field(Of Integer?)("…")`. Elle évite tout le bruit du `DBNull` qu'on subissait avec l'indexeur. *Sur .NET 10, elle est disponible nativement (intégrée à `System.Data.Common`), sans paquet NuGet supplémentaire.* Sa jumelle `SetField(Of T)` écrit une valeur en gérant symétriquement le NULL.

**Répercuter les modifications en base :**

```vb
Using connexion As New SqlConnection(ChaineConnexion),
      adaptateur As New SqlDataAdapter("SELECT Id, Nom, Email FROM Clients", connexion)

    ' Le CommandBuilder génère automatiquement INSERT / UPDATE / DELETE
    Dim builder As New SqlCommandBuilder(adaptateur)

    Dim table As New DataTable()
    adaptateur.Fill(table)

    ' Modifications EN MÉMOIRE (connexion déjà refermée)
    table.Rows(0).SetField("Nom", "Nom corrigé")

    Dim nouvelle = table.NewRow()
    nouvelle.SetField("Nom", "Client ajouté")
    table.Rows.Add(nouvelle)

    ' Synchronisation : l'adaptateur n'envoie QUE les lignes modifiées
    adaptateur.Update(table)
End Using
```

La `DataTable` suit l'état de chaque ligne (`DataRowState` : `Added`, `Modified`, `Deleted`, `Unchanged`), et `Update` ne génère du SQL que pour les lignes réellement touchées.

> ### ⚠️ Deux limites à connaître
> - **`SqlCommandBuilder` est commode mais limité** : il exige que le `SELECT` interroge **une seule table** dotée d'une **clé primaire**, et génère un SQL verbeux. Pour du code de production exigeant, on fournit souvent ses propres `InsertCommand` / `UpdateCommand` / `DeleteCommand`.
> - **Le `DataAdapter` n'a pas de version asynchrone** : `Fill` et `Update` sont **synchrones** (il n'existe pas de `FillAsync`). Ce modèle est antérieur à `Async`/`Await`. Si l'asynchronisme vous importe pour de gros volumes, préférez le `DataReader` (§ 4) ou Entity Framework Core (§ 7.2).

**Quand l'utiliser ?** Le mode déconnecté garde toute sa pertinence pour la **liaison de données Windows Forms** (`BindingSource` se branche directement sur une `DataTable` — voir § 5.8), pour les scénarios **hors-ligne**, et pour les volumes petits à moyens que l'on souhaite manipuler librement après fermeture de la connexion.

---

## 🔁 6. Les transactions

Une **transaction** garantit qu'un ensemble d'opérations réussit **en bloc**, ou est **intégralement annulé** — la propriété d'atomicité (le « A » d'ACID). L'exemple canonique : un virement, où le débit d'un compte et le crédit d'un autre doivent être indissociables.

```vb
Imports System.Runtime.ExceptionServices   ' ExceptionDispatchInfo

Public Async Function TransfererAsync(idSource As Integer,
                                      idDestination As Integer,
                                      montant As Decimal) As Task
    Using connexion As New SqlConnection(ChaineConnexion)
        Await connexion.OpenAsync()

        ' La transaction démarre sur la connexion ouverte
        Using transaction = Await connexion.BeginTransactionAsync()
            Dim erreur As ExceptionDispatchInfo = Nothing
            Try
                ' Chaque commande doit être RATTACHÉE à la transaction
                Using debit As New SqlCommand(
                    "UPDATE Comptes SET Solde = Solde - @m WHERE Id = @id",
                    connexion, CType(transaction, SqlTransaction))

                    debit.Parameters.Add("@m", SqlDbType.Decimal).Value = montant
                    debit.Parameters.Add("@id", SqlDbType.Int).Value = idSource
                    Await debit.ExecuteNonQueryAsync()
                End Using

                Using credit As New SqlCommand(
                    "UPDATE Comptes SET Solde = Solde + @m WHERE Id = @id",
                    connexion, CType(transaction, SqlTransaction))

                    credit.Parameters.Add("@m", SqlDbType.Decimal).Value = montant
                    credit.Parameters.Add("@id", SqlDbType.Int).Value = idDestination
                    Await credit.ExecuteNonQueryAsync()
                End Using

                ' Tout s'est bien passé → on valide
                Await transaction.CommitAsync()

            Catch ex As Exception
                ' On CAPTURE l'erreur — le rollback asynchrone se fera hors du Catch
                erreur = ExceptionDispatchInfo.Capture(ex)
            End Try

            ' Hors du Catch, Await est de nouveau autorisé
            If erreur IsNot Nothing Then
                Await transaction.RollbackAsync()   ' la moindre erreur → on annule tout
                erreur.Throw()                      ' relance en préservant la pile d'origine
            End If
        End Using
    End Using
End Function
```

> ### ⚠️ Pourquoi pas `Await transaction.RollbackAsync()` directement dans le `Catch` ?
> Parce qu'en VB.NET, **`Await` est interdit dans un bloc `Catch` ou `Finally`** (erreur `BC36943`) — alors que
> C# l'autorise depuis C# 6. Tous les exemples C# de la documentation placent donc `await tx.RollbackAsync()`
> dans le `catch` : **ne les transposez pas tels quels**. Le motif VB, vérifié sur .NET 10, est celui ci-dessus —
> *capturer* l'exception (`ExceptionDispatchInfo`), exécuter le rollback **après** le `Try`, puis relancer avec
> `erreur.Throw()`. Il est détaillé au [module 4.3](../04-async/03-exceptions-async.md). (Autre issue, plus simple
> mais bloquante : appeler le `Rollback()` **synchrone** dans le `Catch`, ce qui est permis.)

Les points clés :

**Chaque commande doit être rattachée à la transaction** (ici via le constructeur, sinon par la propriété `commande.Transaction`). Une commande non rattachée s'exécuterait **hors** de la transaction.

**Le schéma `Try` / `Commit` / capture / `Rollback` / relance** est l'ossature standard : on valide en cas de succès, on annule sur exception, et l'on **relaie** l'erreur (`erreur.Throw()`) pour que l'appelant en soit informé.

**Les niveaux d'isolation** se précisent au démarrage : `connexion.BeginTransaction(IsolationLevel.Serializable)`. Le défaut (`ReadCommitted`) convient à la plupart des cas ; les niveaux plus stricts (`RepeatableRead`, `Serializable`) renforcent la cohérence au prix de la concurrence.

> ### `TransactionScope` — pour aller plus loin (notion)
> `System.Transactions.TransactionScope` offre une transaction **ambiante** : toute opération réalisée dans son bloc y est automatiquement inscrite, et `scope.Complete()` la valide. Deux précautions, cependant :
> - en présence d'`Await` à l'intérieur du bloc, il **faut** activer `TransactionScopeAsyncFlowOption.Enabled`, sans quoi la transaction ambiante est perdue après la première continuation asynchrone ;
> - selon les ressources impliquées, elle peut **escalader en transaction distribuée** (MSDTC), plus lourde.
>
> Pour une transaction sur une seule connexion, la `DbTransaction` explicite ci-dessus reste préférable.

---

## 🧭 Synthèse : quel outil pour quel besoin ?

| Besoin | Outil ADO.NET |
|---|---|
| Lire beaucoup de lignes, vite, en mémoire minimale | `DataReader` (§ 4) |
| Récupérer une seule valeur (`COUNT`, identifiant…) | `ExecuteScalar` (§ 2) |
| Insérer / mettre à jour / supprimer | `ExecuteNonQuery` (§ 2) |
| Travailler hors-ligne, lier à des contrôles WinForms | `DataSet` / `DataTable` (§ 5) |
| Garantir l'atomicité d'un groupe d'opérations | Transaction (§ 6) |
| Empêcher l'injection SQL | **Toujours** des commandes paramétrées (§ 3) |

---

## ✅ À retenir

- **ADO.NET est la fondation** : EF Core et les ORM s'appuient dessus. Le maîtriser éclaire tout l'accès aux données et reste irremplaçable pour la performance et le legacy.
- **`Microsoft.Data.SqlClient`**, jamais `System.Data.SqlClient`, pour tout projet .NET 10.
- **`Using` partout** (connexion, commande, lecteur, transaction) : c'est la garantie de libération des ressources et du bon fonctionnement du pool de connexions.
- **Commandes paramétrées, sans exception** : la seule défense fiable contre l'injection SQL. Spécifiez le type des paramètres (`Add` plutôt que `AddWithValue`).
- **`DataReader`** pour la lecture rapide en flux ; **`DataSet` / `DataTable`** pour le déconnecté et la liaison WinForms — en gardant à l'esprit l'absence d'API asynchrone sur le `DataAdapter`.
- **Attention à `DBNull.Value`** (≠ `Nothing`) : `IsDBNull` avec le `DataReader`, ou l'élégant **`Field(Of T)`** avec la `DataTable`.
- **Transactions** : `Commit` en fin de `Try`, **capture** de l'exception, `Rollback` **hors** du `Catch` (`Await` y est interdit — BC36943), chaque commande rattachée à la transaction.

---

> **Prochaine étape →** [§ 7.2 — Entity Framework Core 10](02-ef-core-10.md) : monter d'un cran en abstraction avec l'ORM moderne de .NET, tout en sachant — grâce à ce que vous venez de voir — ce qu'il fait réellement à votre place.

⏭️ [Entity Framework Core 10](/07-acces-donnees/02-ef-core-10.md)
