# 💻 Exemples du module 10 — Architecture hybride VB.NET / C#

Ce module **construit une seule application**, de façon incrémentale, au fil des sections 10.3
à 10.6 (les sections 10.1 et 10.2 sont **conceptuelles** : pas de code, ou le simple extrait
« consommer un parseur C# depuis VB » que l'atelier réalise pleinement). On fournit donc **une
solution mixte complète, compilée et testée** : l'**Importateur de transactions** de l'atelier
(10.5), structuré et outillé selon la **gestion de solution mixte** (10.6).

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

---

## 🧱 L'architecture (cœur C#, métier + UI VB)

```
┌───────────────────────────────────────────────────────────────┐
│  Importateur.UI        (VB, Windows Forms)                    │
│  • formulaire, chargement asynchrone, liaison de données      │
│                         │ appelle                             │
│  Importateur.Metier     (VB)                                  │
│  • règles de gestion (validation), agrégation par LINQ        │
│                         │ consomme (référence de projet)      │
│  Importateur.Coeur      (C#, net10.0)                         │
│  • parsing CSV Span-first (perf) + record Transaction (fonct.)│
└───────────────────────────────────────────────────────────────┘

Dépendances :  UI ──▶ Metier ──▶ Coeur        (jamais l'inverse)
```

Les **deux** motifs de délégation sont à l'œuvre : la **performance** (`Span`-first) et les
**fonctionnalités** (records) vivent dans le cœur **C#** ; l'**UI** et le **métier** restent en
**VB.NET**. La couture est invisible : VB ne voit que `IReadOnlyList`, `Stream`, `Task` et un
record consommé par constructeur — jamais un `Span` ni un `ref struct`.

---

## 🗂️ Correspondance sections du cours → projets / fichiers

| Section du cours | Réalisation dans la solution |
|---|---|
| `01-pourquoi-hybride.md` (10.1) | *conceptuel* — l'extrait « `ParseLignes` consommé depuis VB » est réalisé par `Importateur.Coeur` + `Importateur.Metier` |
| `02-quand-deleguer.md` (10.2) | *conceptuel* (grille de décision) — la délégation choisie ici est une **brique consommée** (perf + records) |
| `03-isoler-en-csharp.md` (10.3) | `Importateur.Coeur` : façade `AnalyseurCsv`, `IAnalyseurCsv`, record `Transaction`, `[assembly: CLSCompliant(true)]`, `Span` interne, validation explicite |
| `04-consommer-depuis-vbnet.md` (10.4) | `Importateur.Metier` : consommation **sous `Option Strict On`**, `Await`, LINQ |
| `05-atelier-...md` (10.5) | la solution **entière** : `Coeur` (C#) + `Metier` (VB) + `UI` (VB WinForms) |
| `06-solution-mixte.md` (10.6) | `.sln` (`src/` + `tests/`), `Directory.Build.props`, `Directory.Packages.props` (CPM), interface + **injection** pour la testabilité, projets de **tests** dans les deux langages |

### Projets de la solution `ImportateurTransactions/`

| Projet | Langage | TFM | Rôle |
|---|---|---|---|
| `src/Importateur.Coeur` | **C#** | net10.0 | parsing `Span`-first + record (la façade VB-friendly) |
| `src/Importateur.Metier` | **VB** | net10.0 | règles + agrégation LINQ ; reçoit `IAnalyseurCsv` par constructeur |
| `src/Importateur.UI` | **VB** WinForms | net10.0-windows | UI de bureau, import async, liaison de données |
| `tests/Importateur.Coeur.Tests` | **C#** (xUnit) | net10.0 | teste le parsing du cœur |
| `tests/Importateur.Metier.Tests` | **VB** (xUnit + Moq) | net10.0 | teste les règles/agrégation en **simulant** le cœur |

---

## ▶️ Comment compiler, tester et lancer

L'outillage .NET est **agnostique du langage** : les commandes opèrent sur toute la solution
mixte sans aménagement.

```bash
cd ImportateurTransactions

dotnet build                 # compile les 5 projets (ordre : Coeur → Metier → UI/tests)
dotnet test                  # exécute les 2 projets de tests (C# + VB)

# Lancer l'UI (WinForms) :
cd src/Importateur.UI
dotnet run                   # ou F5 dans Visual Studio 2026
#   Mode auto-test non interactif (charge le CSV livré, journalise, ferme) :
#   PowerShell :  $env:DEMO_AUTOCLOSE='1'; dotnet run
```

---

## ✅ Sortie attendue (vérifiée)

### `dotnet test` (les valeurs métier, déterministes)

```text
Réussi! - échec : 0, réussite : 4, … - Importateur.Coeur.Tests.dll (net10.0)
Réussi! - échec : 0, réussite : 1, … - Importateur.Metier.Tests.dll (net10.0)
```

- **Coeur.Tests** (4) : le parseur lit correctement les champs (`Id`, `DateOnly`, `Categorie`,
  `Montant`), ignore en-tête et lignes vides, lève `ArgumentNullException` sur flux `null`, et le
  record `Transaction` a l'**égalité de valeur**.
- **Metier.Tests** (1) : avec un `Mock(Of IAnalyseurCsv)` renvoyant 4 transactions (2 valides,
  2 rejetées), `ServiceImport` applique les règles et agrège : 2 valides, 2 rejetées,
  total **57,5**, `Alimentation`=42,5, `Transport`=15.

### UI en auto-test (`%TEMP%\importateur-autotest.log`, sur le CSV livré)

Le fichier `exemple-transactions.csv` contient 7 lignes : 5 valides, 2 rejetées (catégorie
inconnue, montant ≤ 0).

```text
Transactions valides = 5
Rejetees = 2
TotalGeneral = 899.75
Alimentation = 54.75
Logement = 800.00
Loisirs = 30.00
Transport = 15.00
```

- **Comportement vérifié** : l'UI charge le CSV **en asynchrone** (métier VB → cœur C#), applique
  les règles, agrège par catégorie et affiche le tout ; le total **899,75** et les sous-totaux
  correspondent exactement aux données valides. La fenêtre se ferme seule en mode auto-test
  (code de sortie 0).

---

## 🔍 Ce que la solution démontre

- **10.3 (isoler)** : `Importateur.Coeur` expose une **façade** (`Analyser`/`AnalyserAsync`
  renvoyant `IReadOnlyList(Of Transaction)`, validation `ArgumentNullException`, CLS-conforme) ;
  le `Span`-first (`AnalyserLigne(ReadOnlySpan(Of Char))`) reste **interne**.
- **10.4 (consommer)** : `Importateur.Metier` appelle le cœur **comme du .NET ordinaire**, sous
  **`Option Strict On`** (contraste assumé avec l'automation COM/Office du module 9.2), avec
  `Await` et **LINQ** (`GroupBy`/`ToDictionary`).
- **10.5 (atelier)** : trois couches, dépendance `UI → Metier → Coeur` ; le type `Transaction`
  (record C#) **remonte** jusqu'à l'UI et alimente la grille par liaison de données.
- **10.6 (solution mixte)** : un **langage par projet**, **une** solution ; `Directory.Build.props`
  conditionne les options par langage ; `Directory.Packages.props` centralise les versions NuGet
  (CPM) ; le cœur expose **`IAnalyseurCsv`** et `ServiceImport` la reçoit par **constructeur**
  (injection), ce qui rend le **mocking** trivial (Moq, côté tests VB).

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build` (xUnit/Moq restaurés depuis le cache). Le journal d'auto-test
(`%TEMP%\importateur-autotest.log`) est réécrit à chaque exécution.

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR) · xUnit 2.9.2 · Moq 4.20.72
