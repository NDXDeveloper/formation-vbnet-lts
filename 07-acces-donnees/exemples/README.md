# 💻 Exemples du module 7 — Accès aux données

Six projets **console complets, compilés et exécutés** (un par section ; le README du module
n'a pas de code). Chaque projet reprend les codes de sa section, assemblés en un programme
exécutable ; chaque fichier source porte un en-tête **section concernée / description /
fichier du cours**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

> 💡 **Choix de SGBD** — le cours illustre l'accès relationnel avec **SQL Server / LocalDB**.
> Pour que les exemples soient **exécutables partout sans serveur**, ils emploient **SQLite**
> (`Microsoft.Data.Sqlite`, `Microsoft.EntityFrameworkCore.Sqlite`). Le cours le présente
> lui-même comme trivial : *« passer de SQL Server à SQLite revient surtout à changer `Sql`
> en `Sqlite` et la chaîne de connexion »* (§ 7.3). Les concepts (Using, paramètres,
> DataReader, DbContext, LINQ, transactions) sont identiques.

## ▶️ Comment compiler et lancer

```bash
cd <dossier-de-l-exemple>
dotnet run            # ou : ouvrir le .vbproj dans VS 2026, puis F5
```

Ce sont des **applications console** : la « sortie » est le texte affiché, **directement
vérifiable**. Chaque exemple crée ses données dans une base/des fichiers **temporaires**,
puis les **supprime** à la fin. Toutes les valeurs ci-dessous ont été **observées à
l'exécution**.

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Paquets NuGet |
|---|---|---|
| `README.md` (module) | — (aucun code) | |
| `01-adonet.md` | [`7.1-adonet`](#71-adonet) | Microsoft.Data.Sqlite |
| `02-ef-core-10.md` | [`7.2-ef-core-10`](#72-ef-core-10) | Microsoft.EntityFrameworkCore.Sqlite |
| `03-fournisseurs.md` | [`7.3-fournisseurs`](#73-fournisseurs) | Microsoft.Data.Sqlite |
| `04-autres-stockages.md` | [`7.4-autres-stockages`](#74-autres-stockages) | Microsoft.Extensions.Caching.Memory |
| `05-serialisation.md` | [`7.5-serialisation`](#75-serialisation) | CsvHelper |
| `06-fichiers-io.md` | [`7.6-fichiers-io`](#76-fichiers-io) | — (tout intégré) |

---

## 7.1-adonet

- **Section** : 7.1 — ADO.NET · **Fichier** : `01-adonet.md`
- **Description** : connexion dans un `Using` + `OpenAsync`, **commandes paramétrées**
  (anti-injection), `ExecuteScalar` (`COUNT`), `ExecuteNonQuery` (`UPDATE`), **`DataReader`**
  (`GetOrdinal` hors boucle, accesseurs typés, `IsDBNull`), **`DataTable`** + `Field(Of T)`
  (gère le NULL), et **transaction** (Commit / capture / Rollback **hors** du `Catch`).
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  Clients actifs : 2
  Recherche du nom littéral « '; DROP TABLE Clients; -- » -> 0 résultat(s)
  Table Clients intacte après la tentative : 3 lignes
    2 — Martin — (sans e-mail)
  Soldes : compte 1 = 800 ; compte 2 = 700        (après virement de 200)
  Virement annulé : le compte destination n'existe pas
  Soldes : compte 1 = 800 ; compte 2 = 700        (rollback : inchangés)
  ```
- **Comportement vérifié** : l'injection SQL est neutralisée (table intacte) ; le NULL est
  géré ; le rollback laisse les soldes inchangés. **Note** : SQLite met les connexions en
  pool — on appelle `SqliteConnection.ClearAllPools()` avant de supprimer le fichier.

## 7.2-ef-core-10

- **Section** : 7.2 — Entity Framework Core 10 · **Fichier** : `02-ef-core-10.md`
- **Description** : `DbContext`/`DbSet`, **`EnsureCreated`** (la génération de migrations est
  C#-only, cf. cours), LINQ to Entities (syntaxe requête **et** méthode, `ToQueryString`
  affiche le SQL), relations **1-N** (`Include`/`ThenInclude`) et **N-N**, async, pagination
  `Skip`/`Take`, **filtrage dynamique** sur `IQueryable`, et les **nouveautés EF Core 10** :
  **`LeftJoin`** natif, **filtres globaux nommés** (`HasQueryFilter` nommé +
  `IgnoreQueryFilters`), **type complexe** `Profil` mappé en **JSON**, et **concurrence
  optimiste** (`DbUpdateConcurrencyException`).
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  == Base créée (EnsureCreated) : 3 clients, 2 commandes, 2 produits ==
  Inscrits depuis 2025 : Dupont
  Dupont : 2 commande(s), 2 produit(s) distinct(s)
  Page 1 (taille 1) : CMD-001 ; Page 2 : CMD-002
  Montant >= 100 : 1 commande(s)
  CMD-001 -> Dupont        (LeftJoin natif EF Core 10)
  Filtres actifs : 1 ; sans « ClientActif » : 2 ; sans aucun filtre : 3
  Clients dont le SiteWeb finit en .fr : Dupont        (requête sur colonne JSON)
  Conflit détecté : un autre contexte avait déjà modifié l'enregistrement.
  ```
- **Comportement vérifié** : les filtres nommés s'ignorent **sélectivement** (1/2/3) ; la
  requête sur une propriété du JSON est traduite en SQL ; le conflit de concurrence est bien
  levé puis capturé (Await interdit dans le `Catch`).

## 7.3-fournisseurs

- **Section** : 7.3 — Fournisseurs de bases de données · **Fichier** : `03-fournisseurs.md`
- **Description** : le **modèle de fournisseurs** — le même code de démonstration tourne sur
  SQLite **en mémoire** (`:memory:`) et **fichier**, le seul fournisseur sans serveur. Les
  chaînes de connexion des quatre SGBD (SQL Server, SQLite, MySQL/MariaDB, PostgreSQL) et
  leur méthode `UseXxx` sont affichées à titre documentaire.
- **Sortie attendue** (vérifiée) :
  ```text
  Base en mémoire : 3 produits, total 290 €
  Base sur disque : 3 produits, total 290 €
  Fichier créé : fournisseurs-demo.db (8192 octets) — un simple fichier, zéro serveur.
  ```
- **Comportement vérifié** : code identique pour les deux sources ; seul change la chaîne de
  connexion.

## 7.4-autres-stockages

- **Section** : 7.4 — Autres stockages (notions) · **Fichier** : `04-autres-stockages.md`
- **Description** : MongoDB, Cosmos DB et Redis exigeant des **serveurs**, cet exemple
  illustre les **concepts** sans serveur : le motif « document » via `System.Text.Json`, et
  surtout le motif **cache-aside** via **`IMemoryCache`** (qui partage l'esprit de Redis :
  clé/valeur + expiration). Les vraies API serveur sont documentées en fin de sortie.
- **Sortie attendue** (vérifiée) :
  ```text
  Document : {"Id":1,"Libelle":"Clavier mécanique","Prix":89.9,"Tags":["périphérique","gaming"]}
  Relu : Clavier mécanique (2 tags) — round-trip OK : True
    clé produit:42 -> MISS (lecture en base n° 1)
    clé produit:42 -> HIT (servi depuis le cache)
    clé produit:42 -> HIT (servi depuis le cache)
  3 lectures de la clé 42, mais la « base » n'a été touchée que 1 fois (cache-aside).
  ```
- **Comportement vérifié** : malgré 3 lectures, la « base » n'est interrogée qu'**une seule
  fois** — le cache sert les suivantes. (Le JSON échappe les non-ASCII par défaut : `é`
  pour « é » — comportement standard de `System.Text.Json`.)

## 7.5-serialisation

- **Section** : 7.5 — Sérialisation · **Fichier** : `05-serialisation.md`
- **Description** : **JSON** (`System.Text.Json` : options, attributs `<JsonPropertyName>` /
  `<JsonIgnore>`, DOM `JsonNode`, async sur flux), **XML** (`XmlSerializer` **et** les
  **littéraux XML natifs de VB.NET** avec propriétés d'axe `.<élément>` / `.@attribut` — un
  atout absent de C#), **CSV** (`CsvHelper` lecture/écriture typée, et **`TextFieldParser`**
  intégré au runtime VB, avec le piège de la culture et le séparateur `;`).
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  Brut    : {"identifiant":1,"Nom":"Martin"}
  Options : {"identifiant":1,"nom":"Martin"}   (MotDePasse ignoré, 'identifiant' renommé)
  Premier nom (.<client>.<nom>.Value) : Martin
    client 1 : Martin <martin@exemple.fr>
  Écrits puis relus : 2 clients (Martin, Durand)
    id=1, libelle="Clavier; AZERTY", prix=49,90        (le ';' entre guillemets est préservé)
  ```
- **Comportement vérifié** : `MotDePasse` (`<JsonIgnore>`) absent du JSON ; les littéraux XML
  VB se naviguent par propriétés d'axe ; `TextFieldParser` gère correctement un champ
  contenant le délimiteur `;` (entre guillemets).

## 7.6-fichiers-io

- **Section** : 7.6 — Fichiers, flux (`Stream`) et E/S · **Fichier** : `06-fichiers-io.md`
- **Description** : `File`/`Path.Combine`/`Directory.EnumerateFiles`, flux
  (`FileStream`/`MemoryStream`/`CopyToAsync`), texte (`StreamReader`/`Writer`) et binaire
  (`BinaryReader`/`Writer`), **compression `GZipStream`** (ratio mesuré), **chiffrement AES
  via `CryptoStream`** (round-trip) — en nommant la variable **`algo`** (et non `aes`).
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  Directory.EnumerateFiles(*.txt) -> 2 fichier(s)
  CopyToAsync : 33 octets copiés -> identiques : True
  Relu : 42, 3,14, « Martin » — round-trip OK : True
  Original 18000 octets -> compressé 212 octets (ratio 1 %)
  Décompression : round-trip OK : True
  Déchiffré : « Données confidentielles à protéger. » — round-trip OK : True
  ```
- **Comportement vérifié** : tous les round-trips (copie, binaire, compression,
  chiffrement) **retrouvent l'original**. La compression d'un texte répétitif est
  spectaculaire (18000 → 212 octets).
- **Piège du cours vérifié** : `Dim aes = Aes.Create()` provoque **BC30980** (« Le type de
  'aes' ne peut pas être déduit à partir d'une expression contenant 'aes' ») — VB étant
  insensible à la casse, la variable `aes` masque le type `Aes`. D'où la variable **`algo`**
  dans l'exemple (même piège que `host`/`Host` au module 4.8).

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build` (les paquets NuGet — SQLite, EF Core, CsvHelper — sont restaurés depuis le
cache). Les bases et fichiers temporaires sont supprimés par les programmes eux-mêmes.

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
