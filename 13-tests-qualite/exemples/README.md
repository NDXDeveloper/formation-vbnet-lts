# 💻 Exemples du module 13 — Tests et qualité du code

Domaine où le gel du langage VB **ne coûte rien** : frameworks de test, *mocking*, analyseurs,
couverture et benchmark sont des **bibliothèques** et de l'**outillage** — et VB.NET dispose même
de **modèles de projet de test natifs**. Les **six sections** sont reconstruites ici en exemples
**complets, compilés et exécutés**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR) · Docker **absent** (→ 13.3 utilise
SQLite, pas Testcontainers).

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Vérifié |
|---|---|---|
| **13.1** Tests unitaires | [`13.1-tests-unitaires`](#131-tests-unitaires) | xUnit 6/6 · NUnit 6/6 (`[Is]`) · MSTest 6/6 (MTP) |
| **13.2** Mocking + TDD | [`13.2-mocking-tdd`](#132-mocking-tdd) | Moq 4/4 · NSubstitute 3/3 |
| **13.3** Tests d'intégration | [`13.3-tests-integration`](#133-tests-integration) | 2/2 (WebApplicationFactory + SQLite) |
| **13.4** Analyse statique | [`13.4-analyse-statique`](#134-analyse-statique) | CA1822 + IDE0051 levés en VB |
| **13.5** Couverture | [`13.5-couverture`](#135-couverture) | couverture 50 % (Tripler non testé) |
| **13.6** BenchmarkDotNet | [`13.6-benchmarkdotnet`](#136-benchmarkdotnet) | StringBuilder ≈ 8× plus rapide, ~2000× moins d'allocations |

---

## ▶️ Comment compiler et lancer

```bash
# 13.1 (3 projets) — un par framework
cd 13.1-tests-unitaires/Calculatrice.Tests.XUnit  && dotnet test
cd 13.1-tests-unitaires/Calculatrice.Tests.NUnit  && dotnet test
cd 13.1-tests-unitaires/Calculatrice.Tests.MSTest && dotnet test     # ou : dotnet run (exécutable MTP)

cd 13.2-mocking-tdd/Commandes.Tests.Moq          && dotnet test
cd 13.2-mocking-tdd/Commandes.Tests.NSubstitute  && dotnet test

cd 13.3-tests-integration/MonApi.Tests           && dotnet test       # WebApplicationFactory + SQLite

cd 13.4-analyse-statique                          && dotnet build      # diagnostics CA/IDE en avertissement

cd 13.5-couverture/Couverture.Tests               && dotnet test --collect:"XPlat Code Coverage"

cd 13.6-benchmarkdotnet                           && dotnet run -c Release -- dry   # rapide ; sans « dry » = mesure réelle (minutes)
```

Toutes les valeurs ci-dessous ont été **observées à l'exécution**.

---

## 13.1-tests-unitaires

- **Section** : 13.1 · **Fichiers** : `01-tests-unitaires.md`
- **Description** : une lib `Calculatrice` (+ `ServiceCatalogue` async) testée par **trois projets
  VB.NET** — MSTest, NUnit, xUnit. Chacun couvre AAA, test **paramétré**, **asynchrone** et
  **d'exception**. NUnit met en évidence **le piège VB** : `Is` étant un mot-clé, on écrit
  **`[Is].EqualTo(...)`**. Le projet MSTest active **Microsoft.Testing.Platform** (`EnableMSTestRunner`
  + `OutputType=Exe`) : il devient un **exécutable autonome**.
- **Sortie attendue** (vérifiée) :
  ```text
  xUnit  : Réussi! réussite : 6, total : 6
  NUnit  : Réussi! réussite : 6, total : 6
  MSTest : Réussi! réussite : 6, total : 6        (dotnet test ET dotnet run → « Réussite! total: 6 »)
  ```
- **Comportement vérifié** : les trois frameworks fonctionnent en VB ; le projet MSTest s'exécute
  aussi **directement** (`dotnet run`) via MTP, sans hôte VSTest.

## 13.2-mocking-tdd

- **Section** : 13.2 · **Fichier** : `02-mocking-tdd.md`
- **Description** : `ServiceCommandes` dépend de l'**interface** `IRepositoryCommandes` **injectée**
  (condition du mocking). Deux projets de test xUnit : **Moq** (`Setup`/`Returns`/`Verify`/`It.IsAny`,
  exception simulée) et **NSubstitute** (`Substitute.For`/`Returns`/`Received`/`Arg.Any`). Le TDD
  (rouge → vert → refactor) est la démarche qui a produit cette conception testable.
- **Sortie attendue** (vérifiée) : `Moq : 4/4` · `NSubstitute : 3/3`.
- **Comportement vérifié** : la doublure renvoie les réponses programmées et les appels sont
  vérifiés (`Verify`/`Received`) ; l'exception simulée se propage.

## 13.3-tests-integration

- **Section** : 13.3 · **Fichier** : `03-tests-integration.md`
- **Description** : une **Web API VB** (`MonApi`, **`Public Class Program`** — l'absence de
  top-level statements évite le piège C# du `Program` interne) + un projet de test :
  **`WebApplicationFactory(Of Program)`** exécute toute l'API en mémoire (vraies requêtes HTTP),
  une **`FabriqueDeTest`** substitue le `DbContext` par une **SQLite in-memory** (vrai moteur SQL,
  pas le fournisseur InMemory trompeur), et un test EF Core direct vérifie la persistance. Tests
  catégorisés `<Trait("Category","Integration")>`.
- **Sortie attendue** (vérifiée) : `Réussi! réussite : 2, total : 2`.
- **Comportement vérifié** : `GET /api/produits` renvoie **200** avec les produits semés
  (« Clavier », « Souris ») via le pipeline complet HTTP → contrôleur → EF Core → SQLite ; le test
  EF Core direct confirme l'identité générée et la persistance entre contextes.
- **Note** : **Testcontainers** (base cible réelle en conteneur, le plus fidèle) exige **Docker**,
  absent ici — d'où SQLite, que le cours présente comme une option sérieuse (un vrai moteur SQL).

## 13.4-analyse-statique

- **Section** : 13.4 · **Fichier** : `04-analyse-statique.md`
- **Description** : montre que le **socle Roslyn** (règles **`CAxxxx`** de qualité, **`IDExxxx`** de
  style) prend **VB.NET en charge**. La rigueur se règle par MSBuild (`EnableNETAnalyzers`,
  `EnforceCodeStyleInBuild`) et **`.editorconfig`** (sévérité par règle, préfixes `dotnet_…` /
  `visual_basic_…`). Le code déclenche **CA1822** (peut être `Shared`) et **IDE0051** (membre privé
  inutilisé), réglés en **avertissement**.
- **Sortie attendue** (vérifiée) :
  ```text
  warning CA1822: Le membre 'Doubler' ... peut être marqué comme étant static
  warning IDE0051: Le membre privé 'AvecDiagnostic.MethodeInutilisee' n'est pas utilisé
  La génération a réussi.   (3 avertissements, 0 erreur)
  ```
- **Comportement vérifié** : les diagnostics **apparaissent au build** en VB ; passer une règle en
  `error` (ou `TreatWarningsAsErrors`) **bloquerait** le build. **StyleCop**/**Roslynator** sont
  **C# uniquement** ; **SonarQube**/**SecurityCodeScan** couvrent VB.

## 13.5-couverture

- **Section** : 13.5 · **Fichier** : `05-couverture-tests-ia.md`
- **Description** : `Operations` a deux méthodes ; seule `Doubler` est testée, `Tripler` ne l'est
  pas. **Coverlet** (`coverlet.collector`) mesure la couverture lors de
  `dotnet test --collect:"XPlat Code Coverage"` (rapport **Cobertura**).
- **Sortie attendue** (vérifiée — `coverage.cobertura.xml`) :
  ```text
  <coverage line-rate="0.5" lines-covered="2" lines-valid="4" ...>
  classe Couverture.Operations : line-rate=0.5
  méthode Doubler : line-rate=1   |   méthode Tripler : line-rate=0
  ```
- **Comportement vérifié** : la couverture **révèle** la zone non testée (`Tripler` à 0 %) — son
  vrai usage. La couverture mesure ce qui est *exécuté*, pas ce qui est *vérifié* (génération de
  tests par IA : conceptuelle, l'outillage `@test` de VS 2026 vise C#, repli sur Copilot générique
  en VB).

## 13.6-benchmarkdotnet

- **Section** : 13.6 · **Fichier** : `06-benchmarkdotnet.md`
- **Description** : micro-benchmark comparant la concaténation par **`&=`** (réalloue à chaque
  itération) et **`StringBuilder`**, avec `<MemoryDiagnoser>`, `<Params(100, 10000)>` et
  `<Baseline>`. ⚠️ **Release obligatoire**. Le `Main` accepte l'argument **`dry`** (`Job.Dry`) pour
  une exécution rapide de validation ; sans argument, c'est la mesure réelle (plusieurs minutes).
- **Sortie attendue** (vérifiée, run `dry`, extrait — `NombreItems=10000`) :
  ```text
  | Method            | NombreItems | Mean        | Ratio | Allocated   |
  | AvecConcatenation | 10000       | 29 845 us   | 1.00  | 379 440 496 B |
  | AvecStringBuilder | 10000       |  3 730 us   | 0.12  |    159 832 B  |
  ```
- **Comportement vérifié** : `StringBuilder` est **~8× plus rapide** et alloue **~2000× moins** à
  10 000 items — le résultat connu, produit par un vrai tableau BenchmarkDotNet. (En `dry`, les
  chiffres ne sont pas statistiquement fiables — `Error = NA` ; ils le deviennent en run complet.)

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/`, `obj/`, `TestResults/` et les rapports de couverture ne sont pas conservés ;
ils se régénèrent au premier `dotnet test`/`build`/`run` (paquets restaurés depuis le cache).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj, TestResults | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
