# 💻 Exemples du module 12 — Exceptions, débogage et journalisation

C'est l'un des rares modules où la stratégie « langage stabilisé » de VB.NET **ne coûte rien** :
exceptions, débogage, journalisation et observabilité reposent sur le **runtime**, la **BCL**,
des **paquets NuGet** et l'**IDE** — pas sur la syntaxe. Les **quatre sections** sont reconstruites
ici en exemples **complets, compilés et vérifiés**. Une partie de 12.2 (points d'arrêt, espions,
Hot Reload) relève de l'IDE et n'est pas scriptable : seule sa part **code** est reproduite ; de
même, les *health checks* et le câblage OpenTelemetry de 12.4 sont des **notions web** documentées
ci-dessous.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Type | Ce qui est démontré |
|---|---|---|---|
| **12.1** Exceptions | [`12.1-exceptions`](#121-exceptions) | Console VB | Try/Catch/Finally, When, Throw vs Throw ex, InnerException, ExceptionDispatchInfo, exception perso, Data |
| **12.2** Débogage | [`12.2-debogage`](#122-debogage) | Console VB (Debug) | `<DebuggerDisplay>`, `Debug.WriteLine`/`Assert`, `#If DEBUG` (le reste = IDE) |
| **12.3** Journalisation | [`12.3-journalisation`](#123-journalisation) | Console VB | `Microsoft.Extensions.Logging`, logging structuré, `BeginScope`, `LoggerMessage.Define`, Serilog |
| **12.4** Observabilité | [`12.4-observabilite`](#124-observabilite) | Console VB | métriques (`Meter`/`Counter`) + traces (`ActivitySource`/`Activity`) ; OTel/health checks = notions |

---

## ▶️ Comment compiler et lancer

```bash
cd 12.1-exceptions     && dotnet run -c Release
cd 12.2-debogage       && dotnet run            # ⚠️ config DEBUG (le défaut) : Debug.* et #If DEBUG actifs
cd 12.3-journalisation && dotnet run -c Release
cd 12.4-observabilite  && dotnet run -c Release
```

Toutes les valeurs ci-dessous ont été **observées à l'exécution**.

---

## 12.1-exceptions

- **Section** : 12.1 — Exceptions · **Fichier** : `01-exceptions.md`
- **Description** : panorama vérifiable de la gestion structurée : propriétés de `Exception`,
  **gardes** `ArgumentNullException.ThrowIfNull` & co, **filtres `When`** (y compris `When → False`
  pour journaliser **sans capturer**), **`Throw` vs `Throw ex`** (préservation de la pile),
  **enveloppement** (`InnerException`), **`ExceptionDispatchInfo`**, **exception personnalisée**
  (3 constructeurs + donnée métier), dictionnaire **`Data`**.
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  [Filtres When]   code 404 -> introuvable (404) ; 503 -> indisponible ; 500 -> autre
  [When → False]   exception remontée (When n'a pas capturé) ; journalisée = True
  [Throw vs Throw ex]  Throw : pile contient « LeverProfond » = True ; Throw ex : = False
  [Enveloppement]  InnerException = FormatException ; GetBaseException = FormatException
  [ExceptionDispatchInfo]  relancée = InvalidOperationException ; pile contient « LeverProfond » = True
  [Exception personnalisée]  CommandeId = 4271 ; message = La commande 4271 est introuvable.
  [Data]  EtatActuel = Brouillon ; EtatDemande = Expédiée
  ```
- **Comportement vérifié** : `Throw` **préserve** la pile d'origine (point profond visible),
  `Throw ex` la **réinitialise** ; `ExceptionDispatchInfo` préserve la pile à travers une relance
  différée ; le filtre `When → False` laisse remonter l'exception tout en l'ayant journalisée.

## 12.2-debogage

- **Section** : 12.2 — Débogage · **Fichier** : `02-debogage.md`
- **Description** : la **part code** de la section (le reste — points d'arrêt, espions, Hot Reload,
  débogage asynchrone — est une affaire d'**IDE**, identique en VB et C#). Démontre l'attribut
  **`<DebuggerDisplay>`** (motif « propriété privée + `,nq` »), l'instrumentation
  **`Debug.WriteLine` / `Debug.Assert`** (capturée ici via un `TextWriterTraceListener` pour être
  observable hors débogueur), et la **compilation conditionnelle `#If DEBUG`**.
- **À exécuter en configuration DEBUG** (le défaut de `dotnet run`) : en `Release`, `Debug.*` et la
  branche `#If DEBUG` **disparaissent**.
- **Sortie attendue** (vérifiée) :
  ```text
  trace contient « commande 4271 » = True
  affichage = Commande 4271 — 1250 (Expédiée)
  branche DEBUG compilée = True
  ```
- **Comportement vérifié** : `Debug.WriteLine` écrit bien dans l'écouteur de trace en Debug ;
  `<DebuggerDisplay>` compose la chaîne attendue via sa propriété privée ; `#If DEBUG` est actif.

## 12.3-journalisation

- **Section** : 12.3 — Journalisation · **Fichier** : `03-journalisation.md`
- **Description** : journalisation moderne via **`Microsoft.Extensions.Logging`** :
  `LoggerFactory.Create` (variable nommée **`fabrique`** — `loggerFactory` provoquerait **BC30980**,
  collision avec la classe), **logging structuré** (modèles `{…}`, jamais d'interpolation),
  **niveaux + filtrage**, **`BeginScope`**, **`IsEnabled`**, **`LoggerMessage.Define`** (l'équivalent
  VB du générateur `[LoggerMessage]` réservé à C#), puis **Serilog** derrière `ILogger`. Un
  **fournisseur en mémoire** maison prouve la **structure** (propriétés requêtables).
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  info: ...ServiceCommandes[0] Commande 4271 traitée en 12 ms
  [Preuve du logging structuré]
    propriété CommandeId = 4271 ; propriété DureeMs = 12
    aucune entrée Debug (filtrée par min=Information) = True
    entrée Error avec exception = True ; total d'entrées capturées = 5
  [Serilog via ILogger]  [hh:mm:ss INF] Pris en charge par Serilog via ILogger : commande 4271
  ```
- **Comportement vérifié** : le message à modèle conserve **`CommandeId` et `DureeMs` comme
  propriétés** (logging structuré) ; le niveau `Debug` est **filtré** (min=Information) ; l'erreur
  porte l'exception ; Serilog rend bien le log via l'abstraction `ILogger`.
- **Paquets** : Microsoft.Extensions.Logging(.Console) 10.0.0 · Serilog 4.2.0 ·
  Serilog.Sinks.Console 6.0.0 · Serilog.Extensions.Logging 9.0.2.

## 12.4-observabilite

- **Section** : 12.4 — Observabilité (notions) · **Fichier** : `04-observabilite.md`
- **Description** : les **primitives natives** qu'OpenTelemetry collecte —
  **`System.Diagnostics.Metrics`** (`Meter` + `Counter`) et **`System.Diagnostics.Activity`**
  (`ActivitySource` + `Activity` + tags). Les traces sont observées via un **`ActivityListener`** et
  vérifiées de bout en bout. Les *health checks* et le câblage `AddOpenTelemetry()` relèvent de
  l'**hôte web** (cf. module 8) et sont décrits comme notions.
- **Sortie attendue** (vérifiée) :
  ```text
  [Métriques] instrument : nom=commandes.traitees, unité=commande, type=Counter`1 ; 5 mesures émises
  [Traces]    StartActivity -> TraiterCommande ; activité arrêtée : TraiterCommande
              tag commande.id : 4271 ; durée mesurée >= 0 : True
  ```
- **Comportement vérifié** : l'activité n'est créée que parce qu'un **écouteur** est présent
  (`StartActivity` renvoie `Nothing` sinon — d'où `activite?.`), le tag et la durée sont capturés à
  l'arrêt du span.
- **⚠️ Frontière VB rencontrée et documentée** : **lire** une métrique en process via
  `MeterListener` exige un rappel dont le paramètre est un `ReadOnlySpan(Of KeyValuePair(...))` —
  que le compilateur VB **refuse de déclarer** (**BC30668**, « types with embedded references »).
  En pratique, **VB émet** la métrique et un **collecteur la lit** (`dotnet-counters`, OpenTelemetry).
  De même, les rappels à paramètre **`ByRef`** (échantillonnage de trace) imposent une **méthode
  nommée** (`AddressOf`), un lambda VB ne pouvant les porter.

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier `dotnet run`
(les paquets MEL/Serilog sont restaurés depuis le cache).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
