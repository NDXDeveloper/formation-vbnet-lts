🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 7. Accès aux données ⭐ 🔄

> **Partie 3 — Données et persistance**
> Module **cœur de VB.NET** · Actualisé pour **.NET 10 LTS** et **EF Core 10**

---

## 🧭 Introduction

L'accès aux données est l'un des **terrains de prédilection historiques de VB.NET**. La très grande majorité des applications écrites en Visual Basic sont des applications **métier** (LOB, *line of business*) : elles lisent, affichent, valident et persistent des données dans une base. Saisir une commande, consulter un stock, éditer une facture, produire un état — derrière chacune de ces actions, il y a un aller-retour avec une source de données.

Or c'est précisément le type de scénario que Microsoft place au **cœur de VB.NET**, aux côtés de Windows Forms et des bibliothèques :

> *« Nous continuerons à investir dans […] l'interopérabilité avec C#, en particulier dans les scénarios cœur de Visual Basic comme Windows Forms et les bibliothèques. »*
> — Stratégie du langage Visual Basic, Microsoft Learn (le lien figure dans le [SOMMAIRE](/SOMMAIRE.md))

La bonne nouvelle de ce module, c'est qu'**ici, VB.NET n'a strictement rien à envier à C#**. Toute la pile d'accès aux données de .NET — **ADO.NET**, **Entity Framework Core**, `System.Text.Json`, `System.IO` — est constituée de bibliothèques .NET « ordinaires », pleinement **consommables depuis VB.NET** (c'est exactement le périmètre *consumption-only* sur lequel le langage est positionné). Mieux : **LINQ**, que VB.NET intègre directement dans sa syntaxe avec une élégance reconnue (voir [§ 2.9](../02-fondamentaux-langage/09-linq.md)), s'applique aussi bien aux objets en mémoire qu'aux requêtes traduites en SQL par EF Core.

Autrement dit : ce chapitre n'est **pas** un de ceux où l'on signale des limites ou des renvois vers C#. C'est un chapitre où VB.NET est pleinement à sa place.

---

## 🎯 Objectifs du module

À la fin de ce module, vous saurez :

- **choisir** entre **ADO.NET** (contrôle fin, bas niveau) et **Entity Framework Core** (productivité, ORM) selon le contexte ;
- écrire des requêtes **paramétrées et sécurisées**, à l'abri de l'injection SQL ;
- mettre en œuvre **EF Core 10** : `DbContext` / `DbSet`, migrations **Code-First**, *scaffolding* **Database-First** (précieux pour le legacy), relations et stratégies de chargement (*lazy* / *eager* / *explicit*) ;
- exploiter **LINQ to Entities** pour des requêtes lisibles, **asynchrones**, paginées, triées et filtrées dynamiquement, en gérant la concurrence ;
- vous connecter aux principaux **SGBD** : SQL Server / LocalDB, SQLite, MySQL / MariaDB, PostgreSQL ;
- **sérialiser** vos données (**JSON**, XML, CSV) et manipuler **fichiers et flux** (`Stream`, compression, chiffrement) ;
- **situer** les options de stockage **NoSQL** (MongoDB, Cosmos DB) et de **cache** (Redis), pour savoir quand les envisager.

---

## 📐 Deux approches complémentaires : ADO.NET et EF Core

Tout l'accès relationnel en .NET repose sur deux niveaux, qu'il faut savoir distinguer — et combiner.

| Critère | **ADO.NET** ([§ 7.1](01-adonet.md)) | **EF Core 10** ([§ 7.2](02-ef-core-10.md)) |
|---|---|---|
| Niveau d'abstraction | Bas niveau, proche du SQL | Haut niveau (ORM, mapping objet ↔ table) |
| Contrôle sur les requêtes | Maximal (vous écrivez le SQL) | Délégué au framework (SQL généré) |
| Productivité | Plus de code « plomberie » | Très rapide à mettre en œuvre |
| Courbe d'apprentissage | Simple à comprendre | Davantage de concepts à maîtriser |
| Performance | Optimale si bien écrit | Excellente, fine surcouche au-dessus d'ADO.NET |
| Cas d'usage idéal | Requêtes critiques, traitements par lots, code legacy | CRUD métier, modèles riches, développement rapide |

Ces deux approches **ne s'excluent pas** : EF Core s'appuie lui-même sur ADO.NET, et il est courant, dans une même application, d'utiliser EF Core pour 90 % des cas et de redescendre ponctuellement au SQL brut (via ADO.NET ou les API « raw SQL » d'EF) pour les requêtes les plus exigeantes. Comprendre ADO.NET (§ 7.1) reste donc indispensable, même si vous travaillez principalement avec un ORM.

---

## 🆕 Ce que .NET 10 et EF Core 10 apportent

Ce module est aligné sur **EF Core 10**, la version livrée avec **.NET 10 LTS** (novembre 2025, support jusqu'au 14 novembre 2028). Les évolutions les plus utiles au quotidien :

- **`LeftJoin` natif** : une jointure externe gauche enfin exprimable directement, sans le verbeux *group-join + `DefaultIfEmpty`* ;
- ***named query filters*** : possibilité de définir et de nommer plusieurs filtres de requête globaux sur une même entité (pour les activer/désactiver sélectivement) ;
- **colonnes JSON** enrichies : mapping plus complet de documents JSON stockés en colonne ;
- **types complexes** (*complex types*) consolidés, pour modéliser des valeurs sans identité propre.

Côté plateforme, .NET 10 améliore aussi « gratuitement » les performances d'exécution (JIT, PGO) sans modification de votre code — un point traité au [module 14.6](../14-performance/06-apports-net10.md).

---

## 🔒 Un fil rouge : la sécurité des requêtes

Tout au long de ce module, une exigence revient en permanence : **ne jamais construire une requête par concaténation de chaînes** à partir d'une saisie utilisateur. C'est la porte ouverte à l'**injection SQL**, l'une des vulnérabilités les plus anciennes et les plus dévastatrices.

La parade est systématique et simple : les **commandes paramétrées** (ADO.NET) et le **requêtage typé** (LINQ to Entities), qui séparent la structure de la requête de ses valeurs. Ce réflexe est introduit dès la section ADO.NET et approfondi dans le [module 16.3 — OWASP Top 10 pour .NET](../16-securite/03-owasp.md).

---

## 📚 Plan du module

| Section | Contenu | Repère |
|---|---|---|
| **[7.1 — ADO.NET](01-adonet.md)** | La fondation de l'accès aux données : connexions, commandes paramétrées, `DataReader`, `DataSet` / `DataTable`, transactions. | Socle |
| **[7.2 — Entity Framework Core 10](02-ef-core-10.md)** | L'ORM moderne : `DbContext`, Code-First & migrations, Database-First / *scaffolding*, LINQ to Entities, relations, chargement, requêtes async, pagination, concurrence, nouveautés EF Core 10. | 🆕 |
| **[7.3 — Fournisseurs de bases de données](03-fournisseurs.md)** | SQL Server / LocalDB · SQLite (applications locales) · MySQL / MariaDB · PostgreSQL (Npgsql). | — |
| **[7.4 — Autres stockages (notions)](04-autres-stockages.md)** | NoSQL (MongoDB, Cosmos DB) et cache (Redis) : quand et pourquoi les envisager. | Notions |
| **[7.5 — Sérialisation](05-serialisation.md)** | `System.Text.Json`, XML et CSV : transformer vos objets en données échangeables et persistables. | — |
| **[7.6 — Fichiers, flux (`Stream`) et E/S](06-fichiers-io.md)** | `System.IO` (`File`, `Directory`, `Path`), flux (`FileStream`, `MemoryStream`, lecteurs/écrivains), flux bufferisés, de compression (`GZipStream`, `BrotliStream`) et de chiffrement (`CryptoStream`). | — |

---

## 🔗 Prérequis et prolongements

**À maîtriser avant d'aborder ce module :**

- les **génériques et collections** — `List(Of T)`, `Dictionary(Of TKey, TValue)` ([§ 2.8](../02-fondamentaux-langage/08-tableaux-collections.md)) ;
- **LINQ** — syntaxe requête et méthodes d'extension ([§ 2.9](../02-fondamentaux-langage/09-linq.md)), réinvesti massivement avec EF Core ;
- la **programmation asynchrone** — `Async` / `Await` ([module 4](../04-async/README.md)), indispensable pour les requêtes non bloquantes.

**Pour aller plus loin (modules connexes) :**

- [Module 11 — Migration et maintenance du code legacy](../11-migration-legacy/README.md) : le *scaffolding* Database-First (§ 7.2) y est un outil clé pour reprendre une base existante ;
- [Module 16 — Sécurité des applications](../16-securite/README.md) : injection SQL, gestion des secrets de connexion, chiffrement ;
- [Module 14 — Performance et gestion de la mémoire](../14-performance/README.md) : profilage des requêtes et bonnes pratiques ;
- [Module 8 — Consommer et exposer des services](../08-services-web/README.md) : exposer ces données via une Web API.

---

> **À retenir.** L'accès aux données est un **point fort assumé de VB.NET**, sans compromis ni renvoi vers C#. ADO.NET pour le contrôle, EF Core 10 pour la productivité, LINQ comme dénominateur commun élégant : vous disposez en VB.NET de toute la puissance de la plateforme .NET pour bâtir des applications métier robustes.

⏭️ [ADO.NET (connexions, commandes paramétrées, DataReader, DataSet/DataTable, transactions)](/07-acces-donnees/01-adonet.md)
