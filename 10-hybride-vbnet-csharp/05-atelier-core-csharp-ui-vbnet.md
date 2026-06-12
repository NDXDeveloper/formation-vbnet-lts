🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 10.5 Atelier : cœur en C# (performance / fonctionnalités), UI et métier en VB.NET

> **Une application complète, entièrement écrite, qui démontre l'architecture hybride de bout en bout — un cœur C# performant, une logique métier et une interface de bureau en VB.NET.**

Cet atelier est présenté comme une **construction guidée et entièrement résolue** : chaque couche est donnée *in extenso* et commentée. L'objectif n'est pas de laisser du code à écrire, mais de **voir fonctionner ensemble** tout ce que le chapitre a posé — la décision de déléguer (10.2), la conception de la frontière (10.3) et sa consommation (10.4). On y réutilise le parseur CSV esquissé en 10.3, désormais intégré dans une application réelle.

---

## Ce que nous construisons

Un **importateur de transactions** : une application Windows Forms qui charge un fichier CSV de transactions financières, l'**analyse rapidement**, **applique des règles de gestion**, **agrège** les montants par catégorie, et **affiche** le résultat dans une grille.

Ce scénario se prête parfaitement à l'hybride, car il se découpe **naturellement** selon les forces de chaque langage :

```
┌───────────────────────────────────────────────────────────────┐
│  Importateur.UI        (VB, Windows Forms ⭐)                 │
│  • formulaire, sélection de fichier, barre de progression     │
│  • chargement asynchrone, liaison de données                  │
│                         │                                     │
│                         ▼  appelle                            │
│  Importateur.Metier     (VB)                                  │
│  • règles de gestion (validation)                             │
│  • agrégation par catégorie (LINQ ⭐)                         │
│  • orchestration                                              │
│                         │                                     │
│                         ▼  consomme                           │
│  Importateur.Coeur      (C#, net10.0)                         │
│  • parsing CSV Span-first (performance)                       │
│  • modèle immuable à base de records (fonctionnalités)        │
└───────────────────────────────────────────────────────────────┘

Dépendances :  UI ──▶ Metier ──▶ Coeur
```

Les **deux** motifs de délégation nommés dans le titre apparaissent : la **performance** (parsing `Span`-first) et les **fonctionnalités** (records) vivent dans le cœur C# ; l'**UI** et le **métier** restent en VB.NET, là où il est le plus productif.

---

## Vue d'ensemble de la solution

La solution réunit trois projets :

| Projet | Langage | Rôle |
|--------|---------|------|
| `Importateur.Coeur` | **C#** (`net10.0`) | Parsing performant + modèle moderne (records) |
| `Importateur.Metier` | **VB.NET** | Règles de gestion, agrégation, orchestration |
| `Importateur.UI` | **VB.NET** (Windows Forms) | Interface de bureau, async, liaison de données |

La **dépendance** va de l'UI vers le métier, puis vers le cœur — jamais l'inverse. Le type `Transaction`, défini dans le cœur, **remonte** jusqu'à l'UI à travers le résultat métier (les références de projet exposent les types publics de façon transitive). L'organisation fine d'une telle solution mixte (build, NuGet, tests) est détaillée en **section 10.6** ; ici, on se concentre sur le **code des trois couches**.

---

## Couche 1 — Le cœur en C# (`Importateur.Coeur`)

C'est la façade VB-friendly conçue en 10.3, complète. La machinerie `Span`-first et le record restent **internes** au comportement ; la surface publique n'expose que des types simples.

```csharp
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace Importateur.Coeur;

// Modèle immuable : record positionnel → constructeur pour VB,
// égalité de valeur et ToString offerts gratuitement.
public record Transaction(int Id, DateOnly Date, string Categorie, decimal Montant);

// Façade : surface publique simple et stable.
public sealed class AnalyseurCsv
{
    /// <summary>Analyse un flux CSV et renvoie les transactions. Le flux ne doit pas être null.</summary>
    public IReadOnlyList<Transaction> Analyser(Stream flux)
    {
        ArgumentNullException.ThrowIfNull(flux);

        var resultats = new List<Transaction>();
        using var lecteur = new StreamReader(flux);
        bool premiere = true;
        string? ligne;
        while ((ligne = lecteur.ReadLine()) is not null)
        {
            if (premiere) { premiere = false; continue; } // en-tête
            if (ligne.Length == 0) continue;
            resultats.Add(AnalyserLigne(ligne));
        }
        return resultats;
    }

    /// <summary>Variante asynchrone, pour une interface réactive.</summary>
    public async Task<IReadOnlyList<Transaction>> AnalyserAsync(
        Stream flux, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(flux);

        var resultats = new List<Transaction>();
        using var lecteur = new StreamReader(flux);
        bool premiere = true;
        while (await lecteur.ReadLineAsync(ct) is { } ligne)
        {
            if (premiere) { premiere = false; continue; }
            if (ligne.Length == 0) continue;
            resultats.Add(AnalyserLigne(ligne));
        }
        return resultats;
    }

    // INTERNE : découpage Span-first, sans substrings intermédiaires. Jamais exposé.
    private static Transaction AnalyserLigne(ReadOnlySpan<char> ligne)
    {
        // Format attendu : Id;Date(yyyy-MM-dd);Categorie;Montant
        int p1 = ligne.IndexOf(';');
        var reste1 = ligne[(p1 + 1)..];
        int p2 = reste1.IndexOf(';');
        var reste2 = reste1[(p2 + 1)..];
        int p3 = reste2.IndexOf(';');

        var champId = ligne[..p1];
        var champDate = reste1[..p2];
        var champCategorie = reste2[..p3];
        var champMontant = reste2[(p3 + 1)..];

        int id = int.Parse(champId, CultureInfo.InvariantCulture);
        var date = DateOnly.Parse(champDate, CultureInfo.InvariantCulture);
        string categorie = champCategorie.ToString();
        decimal montant = decimal.Parse(champMontant, CultureInfo.InvariantCulture);

        return new Transaction(id, date, categorie, montant);
    }
}
```

Trois fonctionnalités « avancées » sont à l'œuvre — un **record**, du **`Span`** et de l'**async** — et **aucune** ne transparaît dans la surface publique : VB ne verra que `IReadOnlyList(Of Transaction)`, `Stream` et un record consommé par constructeur.

---

## Couche 2 — Le métier en VB.NET (`Importateur.Metier`)

Cette couche **consomme** le cœur C# (sous `Option Strict On`, conformément à 10.4), applique les règles de gestion et agrège — en s'appuyant sur LINQ, un point fort de VB.

```vb
Option Strict On
Option Explicit On

Imports System.Linq
Imports Importateur.Coeur

Namespace Importateur.Metier

    ' Type de résultat renvoyé à l'UI
    Public Class ResultatImport
        Public Property Transactions As IReadOnlyList(Of Transaction)
        Public Property Rejetees As IReadOnlyList(Of Transaction)
        Public Property TotauxParCategorie As IReadOnlyDictionary(Of String, Decimal)
        Public Property TotalGeneral As Decimal
    End Class

    Public Class ServiceImport
        Private Shared ReadOnly CategoriesValides As String() =
            {"Alimentation", "Transport", "Loisirs", "Logement"}

        ' Orchestration : appelle le cœur C#, applique les règles, agrège.
        Public Async Function ImporterAsync(flux As IO.Stream) As Task(Of ResultatImport)
            Dim analyseur As New AnalyseurCsv()
            Dim brutes = Await analyseur.AnalyserAsync(flux)        ' appel transparent au cœur C#

            Dim valides = brutes.Where(AddressOf EstValide).ToList()
            Dim rejetees = brutes.Where(Function(t) Not EstValide(t)).ToList()

            ' Agrégation LINQ (idiomatique en VB)
            Dim totaux = valides _
                .GroupBy(Function(t) t.Categorie) _
                .ToDictionary(Function(g) g.Key,
                              Function(g) g.Sum(Function(t) t.Montant))

            Return New ResultatImport With {
                .Transactions = valides,
                .Rejetees = rejetees,
                .TotauxParCategorie = totaux,
                .TotalGeneral = valides.Sum(Function(t) t.Montant)
            }
        End Function

        ' Règle de gestion : montant strictement positif et catégorie connue.
        Private Shared Function EstValide(t As Transaction) As Boolean
            Return t.Montant > 0D AndAlso CategoriesValides.Contains(t.Categorie)
        End Function
    End Class

End Namespace
```

Remarquez à quel point la consommation est **transparente** : `Transaction` vient de C#, mais on lit `t.Montant` et `t.Categorie` comme sur n'importe quel objet VB, on les passe à LINQ, on les agrège — le tout **sans quitter `Option Strict On`**, et sans qu'aucune trace de la machinerie `Span` ne remonte.

---

## Couche 3 — L'UI en VB.NET Windows Forms (`Importateur.UI`)

Enfin, l'interface de bureau ⭐ : un formulaire qui sélectionne un fichier, le charge **de façon asynchrone** (interface réactive — module 5), affiche les transactions par **liaison de données** et présente les totaux.

> Les contrôles `btnImporter`, `progressBar`, `grilleTransactions`, `lblTotal`, `lblRejetees` et `lstCategories` sont définis dans le concepteur (`FormPrincipal.Designer.vb`, non reproduit ici car généré).

```vb
Option Strict On
Option Explicit On

Imports System.Linq
Imports Importateur.Metier

Public Class FormPrincipal

    Private ReadOnly _service As New ServiceImport()

    Private Async Sub btnImporter_Click(sender As Object, e As EventArgs) _
            Handles btnImporter.Click

        Using dialogue As New OpenFileDialog() With {.Filter = "Fichiers CSV|*.csv"}
            If dialogue.ShowDialog() <> DialogResult.OK Then Return

            btnImporter.Enabled = False
            progressBar.Visible = True
            Try
                Using flux = IO.File.OpenRead(dialogue.FileName)
                    Dim resultat = Await _service.ImporterAsync(flux)   ' métier VB → cœur C#
                    AfficherResultat(resultat)
                End Using
            Catch ex As Exception
                MessageBox.Show($"Échec de l'import : {ex.Message}",
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                progressBar.Visible = False
                btnImporter.Enabled = True
            End Try
        End Using
    End Sub

    Private Sub AfficherResultat(resultat As ResultatImport)
        ' Liaison de données : la grille affiche les transactions valides
        grilleTransactions.DataSource = resultat.Transactions.ToList()

        lblTotal.Text = $"Total : {resultat.TotalGeneral:C}"
        lblRejetees.Text = $"Rejetées : {resultat.Rejetees.Count}"

        ' Récapitulatif par catégorie
        lstCategories.Items.Clear()
        For Each kv In resultat.TotauxParCategorie
            lstCategories.Items.Add($"{kv.Key} : {kv.Value:C}")
        Next
    End Sub

End Class
```

L'UI ne connaît que le **métier VB** ; elle ignore tout du cœur C#. Le chargement asynchrone garde l'interface réactive pendant l'analyse, et la grille affiche directement les **records** `Transaction` (leurs propriétés deviennent les colonnes).

---

## Le flux de bout en bout

Suivons une exécution, couche après couche :

1. L'utilisateur clique sur **Importer** et choisit un fichier CSV (UI, VB).
2. Le gestionnaire **asynchrone** ouvre le flux et appelle `ServiceImport.ImporterAsync` (UI → Métier, VB).
3. Le service **consomme le cœur C#** via `AnalyseurCsv.AnalyserAsync` (Métier → Cœur).
4. Le cœur **analyse le CSV** ligne par ligne avec son parsing `Span`-first interne et renvoie une `IReadOnlyList(Of Transaction)` (C#).
5. De retour côté métier, VB **applique les règles** (valides / rejetées) et **agrège** par catégorie avec LINQ.
6. L'UI **affiche** transactions, total et récapitulatif (Métier → UI, VB).

À aucun moment l'UI ou le métier ne manipulent un `Span`, un `ref struct` ou une syntaxe `record` : la complexité moderne est restée **derrière la façade**.

---

## La frontière en action — ce que l'atelier démontre

Cet exemple condense l'ensemble du chapitre :

- **10.1 (pourquoi)** : le gel du langage ne gêne jamais — la performance et les records, qu'on ne *déclare* pas en VB, sont *consommés* sans friction.
- **10.2 (quand / forme)** : on délègue une **brique consommée** (le cœur), pas une surface séparée ; la performance n'est isolée que parce qu'elle est **réellement** dans un chemin chaud.
- **10.3 (isoler)** : le cœur expose une **façade VB-friendly** (constructeurs, `IReadOnlyList`, `Task`, CLS, validation explicite) et cache la machinerie `Span`.
- **10.4 (consommer)** : VB appelle le cœur **comme du .NET ordinaire**, sous `Option Strict On`, avec IntelliSense et débogage continus.

Chaque langage joue exactement son rôle : **C# pour la vitesse et les fonctionnalités récentes, VB.NET pour l'interface et le métier** — et la couture est invisible.

---

## Aller plus loin

L'architecture s'**étend naturellement**, sans rien remettre en cause :

- **Tests** (module 13) : le métier VB se teste en isolant le cœur derrière une interface ; le cœur C# se teste à part. La frontière facilite le *mocking*.
- **Empaquetage** (module 15) : l'ensemble se déploie comme une application de bureau classique (ClickOnce / MSIX) ; le cœur peut aussi devenir un paquet NuGet interne réutilisable.
- **Évolution** : ajouter une règle de gestion se fait **en VB** (couche métier) ; optimiser le parsing se fait **en C#** (cœur) — sans impact mutuel, exactement comme le voulait l'isolation de la volatilité (10.1).

---

## En résumé

- Un **importateur de transactions** complet illustre l'hybride : cœur **C#** (parsing `Span`-first + records), métier et UI **VB.NET**.
- **Trois projets**, dépendance `UI → Metier → Coeur` ; le type `Transaction` remonte de façon transitive.
- Le **cœur** est une **façade** qui cache `Span`, record et async derrière une surface simple (10.3).
- Le **métier** consomme le cœur **transparemment**, sous `Option Strict On`, et agrège en **LINQ** (10.4).
- L'**UI** Windows Forms ⭐ charge en **asynchrone** et affiche les records par **liaison de données**, sans rien savoir du cœur C#.
- Résultat : chaque langage à sa place, **couture invisible** — la stratégie hybride démontrée de bout en bout.

> 🔗 **Suite logique** : la section **10.6 — Gérer une solution mixte** détaille l'organisation des projets, NuGet, le build et les tests d'une telle solution à deux langages.

⏭️ [Gérer une solution mixte (NuGet, projets, build, tests)](/10-hybride-vbnet-csharp/06-solution-mixte.md)
