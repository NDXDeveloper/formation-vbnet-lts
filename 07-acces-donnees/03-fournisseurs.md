🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 7.3 Fournisseurs de bases de données

> **Module 7 — Accès aux données** · Section 3
> Un même code, plusieurs SGBD · SQL Server / LocalDB · SQLite · MySQL / MariaDB · PostgreSQL

---

## 🧭 Introduction : un modèle, plusieurs fournisseurs

Vous l'avez vu en § 7.1 (modèle de fournisseurs ADO.NET) et en § 7.2 (paquets fournisseurs EF Core) : .NET ne « parle » pas directement à un SGBD donné, mais via un **fournisseur** (*provider*) dédié. La conséquence est très agréable au quotidien : **changer de base de données ne change presque rien à votre code.**

Concrètement, passer de SQL Server à PostgreSQL revient à :

1. **changer le paquet NuGet** du fournisseur ;
2. **adapter la chaîne de connexion** ;
3. **changer l'appel `UseXxx()`** (en EF Core) ou le préfixe des classes (en ADO.NET).

Vos **entités**, votre **`DbContext`**, vos **requêtes LINQ** restent, eux, identiques. Ce qui diffère réellement d'un fournisseur à l'autre : le **format de la chaîne de connexion**, certaines **correspondances de types**, les **fonctions traduisibles** en SQL, et les **fonctionnalités spécifiques** (types propres à PostgreSQL, par exemple).

| Fournisseur | Paquet ADO.NET | Paquet EF Core | Méthode EF Core | Terrain de prédilection |
|---|---|---|---|---|
| **SQL Server / Azure SQL** | `Microsoft.Data.SqlClient` | `Microsoft.EntityFrameworkCore.SqlServer` | `UseSqlServer` | Entreprise, écosystème Microsoft |
| **SQLite** | `Microsoft.Data.Sqlite` | `Microsoft.EntityFrameworkCore.Sqlite` | `UseSqlite` | Applications locales, tests, embarqué |
| **MySQL / MariaDB** | `MySqlConnector` | `Pomelo.EntityFrameworkCore.MySql` | `UseMySql` | Web, open source, hébergement mutualisé |
| **PostgreSQL** | `Npgsql` | `Npgsql.EntityFrameworkCore.PostgreSQL` | `UseNpgsql` | Open source riche, JSONB, géospatial |

---

## 🟦 1. SQL Server / LocalDB

Le fournisseur **de référence** dans l'écosystème Microsoft, et le plus naturel pour une application VB.NET. Rappel des paquets (voir § 7.1 et § 7.2) :

```
Microsoft.Data.SqlClient                    ' ADO.NET
Microsoft.EntityFrameworkCore.SqlServer     ' EF Core
```

**LocalDB**, qu'on utilise dans tous les exemples de ce module, est une **édition légère de SQL Server destinée au développement** : aucune installation de serveur, démarrage à la demande, base stockée dans un fichier. Idéale pour développer et tester sans infrastructure. Les autres éditions : **Express** (gratuite, bridée), **Developer** (gratuite, complète, non destinée à la production), **Standard** / **Enterprise**, et **Azure SQL** dans le cloud.

Quelques chaînes de connexion typiques :

```
' LocalDB (développement)
Server=(localdb)\MSSQLLocalDB;Database=Boutique;Trusted_Connection=True;TrustServerCertificate=True;

' SQL Server Express local
Server=.\SQLEXPRESS;Database=Boutique;Trusted_Connection=True;TrustServerCertificate=True;

' Serveur distant avec identifiants
Server=monserveur,1433;Database=Boutique;User Id=app;Password=********;TrustServerCertificate=True;

' Azure SQL (le chiffrement y est requis)
Server=tcp:xxx.database.windows.net,1433;Database=Boutique;User Id=app;Password=********;Encrypt=True;
```

Configuration EF Core :

```vb
' Cas courant
options.UseSqlServer(chaineConnexion)

' Variante optimisée pour Azure SQL (depuis EF Core 9) — avec EF Core 10,
' elle active notamment le nouveau type natif json sur Azure / SQL Server 2025
options.UseAzureSql(chaineConnexion)
```

> ### ⚠️ Deux rappels
> - **Le piège du certificat** (`Encrypt=True` par défaut avec `Microsoft.Data.SqlClient`) reste valable : `TrustServerCertificate=True` en développement, vrai certificat en production (voir § 7.1).
> - **LocalDB est exclusif à Windows.** Sur un autre OS (ou en conteneur Linux), on utilise SQL Server en conteneur Docker, ou un autre SGBD comme SQLite ou PostgreSQL.

---

## 🟩 2. SQLite (applications locales) ⭐

SQLite est sans doute le fournisseur **le plus pertinent pour une application VB.NET de bureau** mono-utilisateur — précisément le scénario cœur du langage. C'est une base **embarquée, sans serveur** : tout tient dans un **fichier unique**, sans aucune installation ni configuration.

```
Microsoft.Data.Sqlite                    ' ADO.NET
Microsoft.EntityFrameworkCore.Sqlite     ' EF Core
```

Les chaînes de connexion sont d'une simplicité radicale :

```
' Fichier sur disque (créé au besoin)
Data Source=boutique.db

' Base entièrement en mémoire (parfaite pour les tests)
Data Source=:memory:

' Lecture seule
Data Source=boutique.db;Mode=ReadOnly
```

Configuration EF Core :

```vb
options.UseSqlite("Data Source=boutique.db")
```

**Pourquoi c'est idéal en bureau** : zéro dépendance externe, la base se déploie comme un simple fichier au côté de l'exécutable, sauvegarde et copie triviales, performances excellentes en accès local. C'est aussi le choix de prédilection pour les **tests d'intégration** (avec `:memory:`) et le prototypage.

> ### ⚠️ Les limites de SQLite à connaître
> - **Migrations EF Core contraintes** : SQLite ne supporte pas la plupart des opérations `ALTER TABLE`. EF Core procède alors par **reconstruction de table** (créer une nouvelle table, copier les données, supprimer l'ancienne, renommer) pour beaucoup de changements de schéma — plus lent, et certaines opérations restent impossibles.
> - **Typage dynamique** : SQLite stocke les types de façon souple (*type affinity*). Des types .NET comme `Decimal`, `DateTimeOffset` ou `TimeSpan` sont rangés en **texte**, ce qui peut empêcher certaines opérations LINQ d'être traduites côté serveur (tris ou comparaisons sur ces colonnes notamment).
> - **Concurrence en écriture limitée** : un seul écrivain à la fois (verrou au niveau de la base). Parfait pour un usage mono-utilisateur ou en lecture massive, inadapté à une forte concurrence multi-utilisateurs.
>
> En clair : SQLite excelle en **local** ; pour un serveur multi-utilisateurs, on lui préfère SQL Server, PostgreSQL ou MySQL.

---

## 🟧 3. MySQL / MariaDB

Très répandu dans le monde web et l'hébergement mutualisé. Deux briques :

- **ADO.NET** : `MySqlConnector` — le connecteur moderne, entièrement asynchrone et open source, à préférer à l'historique `MySql.Data` d'Oracle.
- **EF Core** : `Pomelo.EntityFrameworkCore.MySql` — le fournisseur communautaire de référence, **construit au-dessus de MySqlConnector**.

Particularité de Pomelo : il faut lui indiquer la **version du serveur**, car le SQL généré en dépend.

```vb
Dim chaine = "Server=localhost;Database=boutique;User=app;Password=********;"

' Détection automatique (ouvre une connexion au démarrage pour interroger le serveur)
Dim version = ServerVersion.AutoDetect(chaine)
options.UseMySql(chaine, version)

' En production, on fige souvent la version pour éviter ce round-trip au démarrage :
' MySQL  :  options.UseMySql(chaine, New MySqlServerVersion(New Version(8, 4)))
' MariaDB:  options.UseMySql(chaine, New MariaDbServerVersion(New Version(11, 4)))
```

**MariaDB**, fork de MySQL largement compatible, est pris en charge par le même fournisseur Pomelo : on précise simplement `MariaDbServerVersion` au lieu de `MySqlServerVersion`.

> ### ⚠️ Cadence de publication et .NET 10 — à vérifier
> Pomelo est un projet **communautaire** dont la prise en charge d'une nouvelle version majeure d'EF Core peut **arriver avec un léger décalage** après la sortie de .NET. Avant d'épingler une version, **vérifiez sur NuGet / GitHub la compatibilité EF Core annoncée** par la version publiée.
>
> Bonne nouvelle pour le runtime : un projet **Pomelo + EF Core 9 fonctionne sur .NET 10**. Vous pouvez donc cibler `net10.0` (et profiter des gains de runtime et des évolutions du langage) en restant sur l'édition EF Core 9 du fournisseur. La nuance : les **nouveautés *ORM* d'EF Core 10** (`LeftJoin` natif, filtres nommés…) nécessitent, elles, le fournisseur **aligné sur EF Core 10**. C'est une illustration concrète d'une réalité des fournisseurs tiers, qu'il faut intégrer à sa planification.

---

## 🟦 4. PostgreSQL (Npgsql)

PostgreSQL, et son fournisseur **Npgsql**, sont souvent considérés comme l'un des couples les plus aboutis de l'écosystème .NET — fonctionnellement très riche.

```
Npgsql                                       ' ADO.NET
Npgsql.EntityFrameworkCore.PostgreSQL        ' EF Core
```

Chaîne de connexion (noter `Host` et `Username`, différents de SQL Server) :

```
Host=localhost;Port=5432;Database=boutique;Username=postgres;Password=********;
```

Configuration EF Core :

```vb
options.UseNpgsql("Host=localhost;Database=boutique;Username=postgres;Password=********;")
```

**Ses atouts** : une prise en charge remarquable des fonctionnalités natives de PostgreSQL — colonnes **`JSONB`**, **tableaux** (`array`), **plages** (`range`), recherche **plein texte**, types **géospatiaux** (PostGIS), énumérations… autant de capacités exposées directement en LINQ. Npgsql **suit de près les versions d'EF Core** : la prise en charge d'**EF Core 10** est disponible (y compris pour les types complexes mappés en JSON, vus en § 7.2).

---

## 🧭 Synthèse : quel fournisseur choisir ?

| Votre contexte | Fournisseur recommandé |
|---|---|
| Application de bureau mono-utilisateur, persistance locale | **SQLite** ⭐ |
| Tests d'intégration | **SQLite** (`:memory:`) |
| Écosystème Microsoft, entreprise, Azure | **SQL Server** / **Azure SQL** |
| Application web, hébergement mutualisé, contexte LAMP | **MySQL / MariaDB** |
| Besoins riches (JSONB, tableaux, géospatial), open source | **PostgreSQL** |

Et rappelez-vous : ce choix porte surtout sur l'**infrastructure**. Grâce au modèle de fournisseurs, votre **code applicatif change très peu** d'un SGBD à l'autre — ce qui permet, par exemple, de développer et tester sur SQLite puis de déployer sur PostgreSQL, moyennant quelques vérifications de types et de fonctions traduites.

---

## ✅ À retenir

- **Un modèle, plusieurs fournisseurs** : changer de SGBD = changer le paquet, la chaîne de connexion et l'appel `UseXxx()`. Le reste du code (entités, `DbContext`, LINQ) bouge à peine.
- **SQL Server / LocalDB** : la valeur sûre Microsoft ; LocalDB pour développer (Windows uniquement), `UseAzureSql` pour Azure (EF Core 10).
- **SQLite** ⭐ : le compagnon idéal des **applications de bureau locales** et des tests — un simple fichier —, en gardant à l'esprit ses limites (migrations par reconstruction, typage souple, écriture mono-utilisateur).
- **MySQL / MariaDB** : `MySqlConnector` (ADO.NET) + **Pomelo** (EF Core, avec `ServerVersion`) ; surveiller la **cadence de publication** de Pomelo vis-à-vis d'EF Core 10.
- **PostgreSQL** : **Npgsql**, riche et à jour pour EF Core 10 (JSONB, tableaux, géospatial).

---

> **Prochaine étape →** [§ 7.4 — Autres stockages (notions)](04-autres-stockages.md) : au-delà du relationnel, un panorama du **NoSQL** (MongoDB, Cosmos DB) et du **cache** (Redis) — quand et pourquoi les envisager depuis une application VB.NET.

⏭️ [Autres stockages (notions) : NoSQL (MongoDB, Cosmos DB), Redis (cache)](/07-acces-donnees/04-autres-stockages.md)
