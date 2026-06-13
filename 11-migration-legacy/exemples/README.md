# 💻 Exemples du module 11 — Migration et maintenance du legacy

Ce module est **stratégique** autant que technique : plusieurs sections sont des guides de
décision sans code (11.1 — évaluer/stratégies ; 11.4 — Web Forms, une **impasse** sans chemin
de migration). Les **cinq sections porteuses de code** sont reconstruites ici en exemples
**complets, compilés et vérifiés**. Le geste actionnable de la 11.4 — *extraire la logique
métier vers une bibliothèque `.NET Standard 2.0`* — est réalisé dans l'exemple **11.5**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Type | Ce qui est démontré |
|---|---|---|---|
| 11.1 évaluer/stratégies | — | *conceptuel* | grille risque/valeur, incrémental vs big-bang (pas de code) |
| **11.2** VB6 → VB.NET | [`11.2-vb6-faux-amis`](#112-vb6-faux-amis) | Console VB | équivalents corrects des « faux-amis » VB6 |
| **11.3** Framework → .NET 10 | [`11.3-framework-vers-net10`](#113-framework-vers-net10) | Console VB | `App.config` → `appsettings.json` / `IConfiguration` + Options |
| 11.4 Web Forms | *(réalisé en 11.5)* | *conceptuel* | impasse : rester / réécrire ; extraire le métier en `.NET Standard` |
| **11.5** Coexistence | [`11.5-coexistence`](#115-coexistence) | Lib `.NET Standard 2.0` + multi-ciblage | pont Framework ↔ moderne, `#If` multi-cible |
| **11.6** Moderniser | [`11.6-moderniser`](#116-moderniser) | Console VB | LINQ, async, injection de dépendances, testabilité |
| **11.7** Gestion des risques | [`11.7-gestion-risques`](#117-gestion-risques) | Lib + tests xUnit | caractérisation + golden master (filet de non-régression) |

---

## ▶️ Comment compiler et lancer

```bash
cd 11.2-vb6-faux-amis        && dotnet run        # console, sortie vérifiable
cd 11.3-framework-vers-net10 && dotnet run        # lit appsettings*.json (copiés en sortie)
cd 11.6-moderniser           && dotnet run        # console
# 11.5 — coexistence (3 projets) :
cd 11.5-coexistence/Societe.Multicible && dotnet build   # produit net48 ET net10.0-windows
cd 11.5-coexistence/Hote.Moderne       && dotnet run     # consomme les deux bibliothèques
# 11.7 — filet de non-régression :
cd 11.7-gestion-risques/Heritage.Tests && dotnet test    # 10 tests
```

Toutes les valeurs ci-dessous ont été **observées à l'exécution**.

---

## 11.2-vb6-faux-amis

- **Section** : 11.2 — VB6 → VB.NET · **Fichier** : `02-vb6-vers-vbnet.md`
- **Description** : montre les **équivalents VB.NET corrects** des pièges silencieux de la
  conversion VB6 : tailles d'entiers (`Integer`→`Short`, `Long`→`Integer`), **`ByVal` par défaut**
  (VB6 : `ByRef`), `Dim x, y As Integer` (les deux `Integer`), **tableaux base 0**,
  `Currency`→`Decimal`, `Variant`→`Object`, `Type`→`Structure`, `Collection`→`List(Of T)`,
  `On Error`→`Try/Catch`, et reconstruction d'un **« control array »** par `AddHandler`.
- **Sortie attendue** (vérifiée, extraits) :
  ```text
  VB6 Integer (16 bits) → VB.NET Short   : 2 octets
  VB6 Long    (32 bits) → VB.NET Integer : 4 octets
  VB6 Currency          → VB.NET Decimal : 16 octets
  après DoublerByVal : v = 10  (inchangé)   /   après DoublerByRef : v = 20  (modifié)
  [Tableau] base 0 : a(0)=10, borne sup=2, longueur=3
  [On Error → Try/Catch] 10 \ 0 → erreur gérée (division par zéro)
  [Control array → AddHandler] 3 déclenchement(s) sur 3 capteurs
  ```
- **Comportement vérifié** : chaque équivalent produit la valeur attendue ; les tailles et la
  sémantique `ByVal`/`ByRef` sont **déterministes**.

## 11.3-framework-vers-net10

- **Section** : 11.3 — .NET Framework 4.x → .NET 10 · **Fichier** : `03-framework-vers-net10.md`
- **Description** : passage de `App.config` (XML, `ConfigurationManager` — fourni en regard dans
  `App.config.legacy.xml`) à **`appsettings.json`** + **`IConfiguration`**. Configuration **en
  couches** (`appsettings.json` + `appsettings.Development.json` + variables d'environnement),
  lecture par indexeur et `GetConnectionString`, **pattern Options** (`IOptions(Of ApiOptions)`),
  et rappel d'encodage `Encoding.Default` = **UTF-8**.
- **Sortie attendue** (vérifiée) :
  ```text
  ApiBaseUrl           = https://localhost:5001   (surchargé par appsettings.Development.json)
  ConnectionStrings:Db = Server=(localdb)\MSSQLLocalDB;Database=AppDev
  Api:TimeoutSeconds = 99   (surchargé par variable d'environnement)
  Api:Reessais       = 3
  Encoding.Default = utf-8 (Unicode (UTF-8)) ; EstUtf8 = True
  ```
- **Comportement vérifié** : la **superposition des couches** fonctionne (Development surcharge la
  base ; la variable d'environnement surcharge la section `Api`) ; `Encoding.Default` vaut bien
  UTF-8 sur .NET moderne.

## 11.5-coexistence

- **Section** : 11.5 — Coexistence `.NET Standard` (+ geste 11.4) · **Fichier** : `05-coexistence.md`
- **Description** : trois projets. `Societe.Metier` (**`.NET Standard 2.0`**, VB, pur) porte la
  logique métier partagée — le **pont** consommable par .NET Framework **et** .NET 10.
  `Societe.Multicible` est **multi-ciblé** `net48;net10.0-windows` avec compilation conditionnelle
  **`#If NET48`** (et `Microsoft.NETFramework.ReferenceAssemblies` pour bâtir net48 sans pack de
  ciblage). `Hote.Moderne` (.NET 10) consomme les deux.
- **Sortie attendue** (vérifiée — hôte) :
  ```text
  [Bibliothèque .NET Standard 2.0]  remise(120 €)=12,0 €  remise(60 €)=3,00 €  remise(30 €)=0 €
  [Bibliothèque multi-ciblée]       .NET moderne (10.0.9) — chemin natif
  ```
- **Comportement vérifié** : `dotnet build` de `Societe.Multicible` produit **deux** assemblys
  (`bin/Release/net48/…` **et** `bin/Release/net10.0-windows/…`) ; l'hôte moderne consomme la
  bibliothèque `.NET Standard` (source unique de vérité) et le **chemin `#Else`** de la
  bibliothèque multi-ciblée.

## 11.6-moderniser

- **Section** : 11.6 — Moderniser · **Fichier** : `06-moderniser.md`
- **Description** : axes de modernisation idiomatique. **LINQ** (boucle impérative → requête, même
  résultat), **Async/Await** (E/S sans blocage), **injection de dépendances**
  (`Microsoft.Extensions.DependencyInjection`) remplaçant les `New` en dur et la dépendance
  statique `DateTime.Now` par une **couture testable** (`IHorloge` injectée). (EF Core : module 7.)
- **Sortie attendue** (vérifiée ; `*` = dépend du jour) :
  ```text
  [LINQ] impératif = 3 actifs ; LINQ = 3 actifs ; identiques = True   (Alice, Charlie, Diane)
  [Injection de dépendances] Bonjour Alice, nous sommes le 2026-06-13            *
  [Async/Await] message chargé de façon asynchrone
  [Testabilité] horloge FIXE : Bonjour Bob, nous sommes le 2026-01-01  (déterministe)
  ```
- **Comportement vérifié** : LINQ et la boucle donnent le **même** résultat (`SequenceEqual`) ;
  l'horloge **injectée** rend la salutation déterministe en test — la DI **débloque la
  testabilité**.

## 11.7-gestion-risques

- **Section** : 11.7 — Gestion des risques · **Fichier** : `07-gestion-risques.md`
- **Description** : un **filet de non-régression** verrouillant le comportement **actuel** (bugs
  compris) d'un code hérité avant migration. `Heritage` expose deux comportements surprenants mais
  assumés : **arrondi banquier** de `CInt` (0,5→0, 1,5→2, 2,5→2…) et **chaîne de longueur fixe**
  (complétée/tronquée à 5). `Heritage.Tests` (xUnit) les capture par des **tests de
  caractérisation** et un **golden master** (test d'approbation).
- **Sortie attendue** (vérifiée) :
  ```text
  dotnet test -> Réussi! échec : 0, réussite : 10, total : 10
  ```
- **Comportement vérifié** : les 6 cas d'arrondi banquier + 3 cas de longueur fixe + le golden
  master (`0.5=0;1.5=2;2.5=2;3.5=4;4.5=4;5.5=6;6.5=6;7.5=8`) passent — toute migration qui
  changerait ces sorties **casserait** le filet, exactement le but recherché.

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build`/`run`/`test` (paquets restaurés depuis le cache). Les fichiers `appsettings*.json`
sont des sources (copiés en sortie au build).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR) · xUnit 2.9.2
