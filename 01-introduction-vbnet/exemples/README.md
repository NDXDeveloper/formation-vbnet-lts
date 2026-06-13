# 💻 Exemples du module 1 — Introduction : VB.NET et l'écosystème .NET 10

Projets **complets, compilés et vérifiés** accompagnant les sections du module 1.
Chaque exemple reprend les codes sources et extraits du cours (à l'identique lorsqu'ils sont
montrés tels quels), complétés lorsque nécessaire pour former un projet exécutable ; chaque
fichier source porte un en-tête précisant la **section concernée**, la **description** et le
**fichier du cours** dont il provient.

**Environnement de validation** (juin 2026) :

| Outil | Version vérifiée |
|---|---|
| Visual Studio Community 2026 | 18.7.11903.348 |
| SDK .NET | 10.0.301 (`dotnet --version`) |
| Runtime .NET | 10.0.9 (`Environment.Version`) |

> 💡 Tous les exemples se compilent avec le SDK .NET 10 installé avec Visual Studio 2026 —
> en ligne de commande (`dotnet build` / `dotnet run`) comme depuis l'IDE (ouvrir le `.sln`,
> le `.vbproj` ou le dossier, puis **F5**).

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Codes repérés | Exemple(s) |
|---|---|---|
| `README.md` (module) | aucun code | — |
| `01-quest-ce-que-vbnet.md` | aperçu `List(Of String)` / `For Each` / `$"…"` | [`1.1-bonjour-villes`](#11-bonjour-villes) |
| `02-histoire-evolution.md` | aucun code compilable (ruptures décrites en prose) | [`1.2-rupture-vb6-vbnet`](#12-rupture-vb6-vbnet) *(exemple ajouté)* |
| `03-ecosysteme-dotnet.md` | commandes CLI `dotnet`, squelette `.vbproj`, structure de solution mixte | [`1.3-squelette-vbproj`](#13-squelette-vbproj), [`1.3-solution-mixte`](#13-solution-mixte) |
| `04-installation-outils.md` | commandes CLI (`--info`, `--version`, `new`, `build`, `run`, `test`) | → [Commandes du cours vérifiées](#-commandes-du-cours-vérifiées) |
| `05-premier-projet.md` | `Program.vb` console, `Program.vb` WinForms, `Form1` avec `Handles` | [`1.5-console-hello`](#15-console-hello), [`1.5-winforms-demarrage-explicite`](#15-winforms-demarrage-explicite), [`1.5-winforms-cadre-applicatif`](#15-winforms-cadre-applicatif) |
| `06-positionnement-2026.md` | repère vérifiable « 12 modèles / 5 familles » | [`1.6-modeles-projet-vb`](#16-modeles-projet-vb) |
| `06.1-vbnet-vs-csharp.md` | aucun code (grille de décision) | — (illustré par `1.6.2`) |
| `06.2-strategie-hybride.md` | schéma de solution hybride, consommation records / `init-only` | [`1.6.2-hybride-record-csharp`](#162-hybride-record-csharp) |
| `06.3-ere-ia.md` | aucun code (pratiques IA) | — |

---

## 1.1-bonjour-villes

- **Section concernée** : 1.1 — Qu'est-ce que VB.NET et à quoi il sert réellement en 2026
- **Fichier concerné** : `01-quest-ce-que-vbnet.md`
- **Description** : l'aperçu exact du cours (`Dim villes As New List(Of String) From {…}`,
  `For Each`, interpolation `$"…"`), complété par deux démonstrations de la section :
  VB.NET utilise la **même BCL** (`List.Sort`, `String.Join`, `Math.Max`) et le **même
  runtime** que C# (`Environment.Version`).
- **Compiler / exécuter** :
  ```bash
  cd 1.1-bonjour-villes
  dotnet run
  ```
- **Sortie attendue** (vérifiée) :
  ```text
  Bonjour Paris !
  Bonjour Lyon !
  Bonjour Marseille !

  Villes triées : Lyon, Marseille, Paris
  Math.Max(10, 25) = 25
  Version du runtime .NET : 10.0.9
  ```
  (la dernière ligne affiche `10.0.x` selon le runtime .NET 10 installé)
- **Comportement attendu** : les trois premières lignes correspondent exactement à
  l'extrait du cours ; le programme se termine avec le code de sortie 0.

## 1.2-rupture-vb6-vbnet

- **Section concernée** : 1.2 — De Visual Basic 6 à VB.NET (histoire et héritage legacy)
- **Fichier concerné** : `02-histoire-evolution.md`
- **Description** : **exemple ajouté à la formation** (la section ne contient pas de code
  compilable). Il rend concrets les « pièges emblématiques » de la rupture VB6 → VB.NET
  décrits dans la section : `Integer` 16 bits → 32 bits (`Short` vs `Integer` vs `Long`),
  tableaux indexés à partir de zéro, disparition du `Variant` au profit d'`Object`, et
  héritage d'implémentation / polymorphisme (`MustInherit`, `Inherits`, `Overrides` —
  voir `Animaux.vb`), impossibles en VB6.
- **Compiler / exécuter** :
  ```bash
  cd 1.2-rupture-vb6-vbnet
  dotnet run
  ```
- **Sortie attendue** (vérifiée) :
  ```text
  == Tailles des types entiers ==
  Short.MaxValue   =                32767  (16 bits — l'« Integer » de VB6)
  Integer.MaxValue =           2147483647  (32 bits — l'Integer de VB.NET)
  Long.MaxValue    =  9223372036854775807  (64 bits)

  == Tableaux indexés à partir de zéro ==
  jours(0) = lundi  (premier élément : indice 0, pas 1)
  jours(4) = vendredi  (dernier élément : Length - 1)

  == Object remplace Variant ==
  valeur contient un Int32 : 42
  valeur contient maintenant un String : quarante-deux

  == Héritage d'implémentation (impossible en VB6) ==
  Rex (Chien) dit : Wouf !
  Mistigri (Chat) dit : Miaou !
  ```
- **Comportement attendu** : valeurs limites exactes (`32767`, `2147483647`,
  `9223372036854775807`), typage dynamique d'`Object` affiché à l'exécution, appels
  polymorphes résolus sur les types dérivés.

## 1.3-squelette-vbproj

- **Section concernée** : 1.3 — L'écosystème .NET (runtime, SDK, NuGet, solution)
- **Fichier concerné** : `03-ecosysteme-dotnet.md`
- **Description** : le « squelette typique d'un projet console moderne » du cours,
  repris à l'identique dans `MonApp.vbproj` (SDK-style, `OptionStrict`/`OptionExplicit`,
  `PackageReference Microsoft.Extensions.Logging` **10.0.0**). Le `Program.vb` consomme
  réellement le paquet NuGet (fabrique de loggers, messages structurés).
  **Ajout par rapport au cours** : le paquet `Microsoft.Extensions.Logging.Console`
  (le *fournisseur* qui écrit dans la console — sans lui, aucun journal n'est visible),
  soit l'équivalent de `dotnet add package Microsoft.Extensions.Logging.Console`.
- **Compiler / exécuter** :
  ```bash
  cd 1.3-squelette-vbproj
  dotnet run     # la restauration NuGet (dotnet restore) est automatique
  ```
- **Sortie attendue** (vérifiée) :
  ```text
  info: MonApp[0] Le paquet NuGet est restauré et opérationnel.
  warn: MonApp[0] Exemple d'avertissement avec un paramètre structuré.
  fail: MonApp[0] Exemple d'erreur (sans exception réelle).
  Fin du programme.
  ```
- **Comportement attendu** : la première compilation **télécharge les paquets NuGet**
  (connexion réseau requise une seule fois, cache local ensuite) ; les trois niveaux
  de journalisation s'affichent préfixés `info:` / `warn:` / `fail:`, puis le programme
  se termine proprement (le `Using` vide les tampons du logger).

## 1.3-solution-mixte

- **Section concernée** : 1.3 — Structure d'une solution (et stratégie hybride 🔗)
- **Fichier concerné** : `03-ecosysteme-dotnet.md`
- **Description** : la structure de solution **exactement telle que dessinée dans le
  cours**, matérialisée en projets réels et fonctionnels :
  ```text
  MaSolution.sln
  ├── src/
  │   ├── MonApp.UI/       (VB.NET — Windows Forms)            → référence Metier
  │   ├── MonApp.Metier/   (VB.NET — bibliothèque de classes)  → référence Core 🔗
  │   └── MonApp.Core/     (C#     — briques modernes : record, switch expression)
  └── tests/
      └── MonApp.Tests/    (VB.NET — xUnit, généré par « dotnet new xunit -lang VB »)
  ```
  Métier de démonstration : une facture de 3 lignes aux trois taux de TVA
  (5,5 % / 10 % / 20 %) — total HT 71,00 €, TVA 7,98 €, TTC 78,98 €.
- **Compiler / tester / exécuter** :
  ```bash
  cd 1.3-solution-mixte
  dotnet build MaSolution.sln    # compile les 4 projets (VB + C#)
  dotnet test                    # exécute les 5 tests xUnit
  dotnet run --project src/MonApp.UI    # ouvre la fenêtre (sous Windows)
  ```
- **Sortie attendue** (vérifiée) :
  - `dotnet build` : `La génération a réussi. 0 Avertissement(s) 0 Erreur(s)` ;
  - `dotnet test` : `Réussi! - échec : 0, réussite : 5, ignorée(s) : 0, total : 5` ;
  - `MonApp.UI` : fenêtre « MaSolution — Facture de démonstration » listant les
    3 lignes avec leur taux de TVA et affichant
    `Total HT : 71,00 € / TVA : 7,98 € / Total TTC : 78,98 €`
    (démarrage sans plantage vérifié automatiquement ; montants selon la culture
    d'affichage Windows).
- **Comportement attendu** : la chaîne **UI (VB) → Métier (VB) → Core (C#)** compile et
  s'exécute dans une seule solution ; les tests VB vérifient au passage la consommation
  de la brique C# (barème par *switch expression*, égalité par valeur du record).

## 1.5-console-hello

- **Section concernée** : 1.5 — Premier projet pas à pas (Projet 1 : Console)
- **Fichier concerné** : `05-premier-projet.md`
- **Description** : le « code généré » exact du cours — `Imports System`,
  `Module Program`, `Sub Main(args As String())`, `Console.WriteLine("Hello World!")` —
  vérifié **identique mot pour mot** à ce que produit
  `dotnet new console -lang VB -o MaConsole` avec le SDK 10.0.301. Pas de
  *top-level statements* en VB : la structure `Module` / `Sub Main` est explicite.
- **Compiler / exécuter** :
  ```bash
  cd 1.5-console-hello
  dotnet run     # équivalents Visual Studio : F5 / Ctrl+F5
  ```
- **Sortie attendue** (vérifiée) :
  ```text
  Hello World!
  ```

## 1.5-winforms-demarrage-explicite

- **Section concernée** : 1.5 — Premier projet pas à pas (Projet 2 : Windows Forms)
- **Fichier concerné** : `05-premier-projet.md`
- **Description** : le projet Windows Forms **« modèle CLI »** décrit dans la section :
  `Program.vb` à démarrage explicite (`<STAThread>`, `SetHighDpiMode`,
  `EnableVisualStyles`, `Application.Run(New Form1)`) — vérifié **identique** à ce que
  génère `dotnet new winforms -lang VB`, y compris `<StartupObject>Sub Main</StartupObject>`
  et l'absence d'`ApplicationEvents.vb` / `.resx`. S'y ajoute le gestionnaire d'événement
  exact du cours : `Button1_Click … Handles Button1.Click` → `MessageBox.Show(…)`
  (le bouton étant déclaré `Friend WithEvents` dans `Form1.Designer.vb`, la moitié
  « Concepteur » de la classe partielle).
- **Compiler / exécuter** :
  ```bash
  cd 1.5-winforms-demarrage-explicite
  dotnet build       # vérification automatisée
  dotnet run         # ouvre la fenêtre (sous Windows)
  ```
- **Sortie / comportement attendus** : pas de sortie console. Une fenêtre `Form1`
  (800 × 450) s'ouvre avec un bouton **Button1** ; un clic affiche la boîte de message
  **« Bonjour depuis VB.NET ! »**. Vérifié automatiquement : compilation sans erreur ni
  avertissement, démarrage stable (l'application reste ouverte, fermée ensuite par le
  test) ; le clic, interactif, se vérifie manuellement.

## 1.5-winforms-cadre-applicatif

- **Section concernée** : 1.5 — Premier projet pas à pas (« Le démarrage : le cadre applicatif VB (≠ C#) »)
- **Fichier concerné** : `05-premier-projet.md`
- **Description** : la **seconde structure** décrite dans la section — celle que crée
  Visual Studio : **aucun `Sub Main` écrit**. Le démarrage est piloté par le **cadre
  applicatif** (*Application Framework*) : `<MyType>WindowsForms</MyType>` dans le
  `.vbproj` (le compilateur génère le point d'entrée dans `My.MyApplication`),
  formulaire de démarrage désigné dans `My Project/Application.Designer.vb`
  (`OnCreateMainForm`), réglages dans `Application.myapp`, et événements globaux
  (`Startup`, `Shutdown`, `UnhandledException`…) dans **`ApplicationEvents.vb`** —
  avec un gestionnaire `Startup` d'exemple.
- **Compiler / exécuter** :
  ```bash
  cd 1.5-winforms-cadre-applicatif
  dotnet build       # vérification automatisée
  dotnet run         # ouvre la fenêtre (sous Windows)
  ```
- **Sortie / comportement attendus** : pas de sortie console. Une fenêtre
  « Cadre applicatif VB » s'ouvre (un label explicatif + un bouton **Dire bonjour** →
  boîte de message « Bonjour depuis le cadre applicatif VB ! ») ; l'événement global
  `Startup` écrit une trace de débogage avant l'affichage (visible dans la fenêtre
  Sortie de Visual Studio en débogage). Vérifié automatiquement : compilation sans
  erreur, démarrage stable — donc point d'entrée généré par le compilateur fonctionnel.
- **À comparer** avec `1.5-winforms-demarrage-explicite` : mêmes capacités, **deux modes
  de démarrage** ; la formation privilégie ensuite le cadre applicatif (projet Visual
  Studio).

## 1.6-modeles-projet-vb

- **Section concernée** : 1.6 — VB.NET en 2026 : positionnement honnête (« Lecture du périmètre réel »)
- **Fichier concerné** : `06-positionnement-2026.md`
- **Description** : script PowerShell qui **vérifie le repère chiffré** de la section :
  il liste les modèles de **projet** disponibles en VB
  (`dotnet new list --language VB --type project` — les *éléments* comme
  `mstest-class` / `nunit-test` sont hors décompte), les classe par famille et conclut.
- **Exécuter** :
  ```powershell
  cd 1.6-modeles-projet-vb
  pwsh -File .\lister-modeles-vb.ps1
  ```
- **Sortie attendue** (vérifiée, SDK 10.0.301) :
  ```text
  Modèles de PROJET disponibles en VB.NET (dotnet new list --language VB --type project) :

    Bibliothèque de classes                  : classlib
    Console                                  : console
    Projets de test (MSTest, NUnit, xUnit)   : mstest, nunit, xunit
    Windows Forms                            : winforms, winformscontrollib, winformslib
    WPF                                      : wpf, wpfcustomcontrollib, wpflib, wpfusercontrollib

  Total : 12 modèles de projet, en 5 familles.

  VERDICT : conforme au repère de la section 1.6 — « douze modèles, cinq familles ».
  ```
- **Comportement attendu** : code de sortie 0 si le repère « 12 modèles / 5 familles »
  est vérifié, 1 sinon (le décompte peut varier si des modèles tiers sont installés).

## 1.6.2-hybride-record-csharp

- **Section concernée** : 1.6.2 — La stratégie hybride VB.NET / C# en une page 🔗
- **Fichier concerné** : `06.2-strategie-hybride.md`
- **Description** : le « Comment ça marche, concrètement » de la section, pas à pas :
  **1)** une bibliothèque de classes C# (`ModerneLib`) déclare un **record positionnel**
  et des **propriétés `init-only`** — non déclarables en VB (langage figé à 16.9) ;
  **2)** le projet VB (`AppVB`) la **référence** (`ProjectReference`) ;
  **3)** côté VB, on **consomme** : constructeur du record, `ToString()` généré,
  **égalité par valeur** (`p1 = p2`), et initialiseur `With {…}` sur les propriétés
  `init-only` — précisément la capacité de consommation apportée par **VB 16.9**.
  En commentaire : l'erreur **BC37311** (vérifiée) si l'on tente d'affecter une
  propriété `init-only` hors initialiseur.
- **Compiler / exécuter** :
  ```bash
  cd 1.6.2-hybride-record-csharp
  dotnet build HybrideDemo.sln
  dotnet run --project AppVB
  ```
- **Sortie attendue** (vérifiée) :
  ```text
  == Un record C# consommé depuis VB.NET ==
  p1 : Produit { Nom = Café moulu, PrixEuros = 4 }
  p2 : Produit { Nom = Café moulu, PrixEuros = 4 }
  Égalité par valeur  (p1 = p2)  : True
  Références distinctes (p1 Is p2) : False

  == Propriétés init-only consommées depuis VB ==
  Article : Thé vert, quantité : 12
  ```
- **Comportement attendu** : deux instances distinctes du record (`Is` → `False`) sont
  **égales par valeur** (`=` → `True`) ; le `ToString()` affiché est celui généré par le
  record C#.

---

## ✅ Commandes du cours vérifiées

Commandes citées dans `03-ecosysteme-dotnet.md`, `04-installation-outils.md` et
`05-premier-projet.md`, toutes vérifiées sur l'environnement ci-dessus :

| Commande | Résultat vérifié |
|---|---|
| `dotnet --version` | `10.0.301` |
| `dotnet --info` | SDK 10.0.301 + runtimes installés (sortie détaillée) |
| `dotnet new console -lang VB` | génère exactement le `Program.vb` et le `.vbproj` montrés dans le cours |
| `dotnet new winforms -lang VB` | génère exactement le `Program.vb` « à la C# » du cours, avec `<StartupObject>Sub Main</StartupObject>`, **sans** `ApplicationEvents.vb` ni `.resx` |
| `dotnet new list -lang VB` | 12 modèles de **projet** en 5 familles (+ 2 modèles d'*élément* de test) |
| `dotnet build` / `dotnet run` / `dotnet test` | utilisés pour valider chaque exemple (voir ci-dessus) |
| `dotnet add package <Paquet>` | équivalent du `PackageReference` de `1.3-squelette-vbproj` |
| `dotnet new sln` | ⚠️ avec le SDK .NET 10, crée par défaut un fichier **`.slnx`** (nouveau format XML évoqué en 1.3) ; utiliser `dotnet new sln --format sln` pour le `.sln` classique (format retenu ici, conforme au schéma du cours) |

## 🛠️ Ouvrir dans Visual Studio 2026

- **Solutions** : double-cliquer `1.3-solution-mixte/MaSolution.sln` ou
  `1.6.2-hybride-record-csharp/HybrideDemo.sln`.
- **Projets seuls** : ouvrir directement le `.vbproj` (ou le dossier de l'exemple).
- Lancer avec **F5** (débogage) ou **Ctrl+F5**. Pour les projets Windows Forms, le
  **Concepteur** s'ouvre en double-cliquant `Form1.vb` dans l'Explorateur de solutions.
- Charge de travail requise : **« Développement .NET Desktop »** (voir section 1.4).

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` (produits de compilation) **ne sont pas conservés** dans le
dépôt — ils se régénèrent à la première compilation (`dotnet build` ou F5 ; la
restauration NuGet est automatique, réseau requis une fois pour `1.3-squelette-vbproj`
et `1.3-solution-mixte`). Pour nettoyer après usage :

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
# (ou, projet par projet : dotnet clean)
```

## 📝 Notes

- **Accents dans la console Windows** : les exemples ajoutés règlent
  `Console.OutputEncoding = System.Text.Encoding.UTF8` en début de `Main` pour un
  affichage fiable des caractères accentués (le « code généré » de 1.5 est laissé tel
  quel, sa sortie n'ayant pas d'accent).
- **Montants et culture** : les sorties console vérifiées n'utilisent que des valeurs
  insensibles à la culture ; l'interface de `1.3-solution-mixte` formate ses montants
  selon la culture Windows de la machine (`71,00 €` en français).
- **En-têtes de description** : format `' =====…` dans les fichiers VB, `/* =====… */`
  dans les fichiers C#, `<!-- =====… -->` dans les `.vbproj`/`.csproj`, `# =====…` dans
  les scripts PowerShell.

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11
