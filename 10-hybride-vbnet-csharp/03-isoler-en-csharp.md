🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 10.3 Isoler les fonctionnalités avancées dans des bibliothèques C#

> **Concevoir la bibliothèque C# de sorte que la machinerie moderne reste un détail interne, et que VB.NET ne voie qu'une surface simple, stable et idiomatique.**

La section 10.2 a tranché *quoi* déléguer ; celle-ci traite la **conception de la frontière**, vue du **côté auteur** de la bibliothèque C# (la consommation depuis VB fait l'objet de la section 10.4). L'enjeu tient en une phrase : faire de la fonctionnalité avancée un **détail d'implémentation**, caché derrière un **contrat public que VB consomme sans effort**.

---

## L'objectif : la fonctionnalité avancée comme détail d'implémentation

L'erreur serait d'exposer à VB la machinerie C# elle-même — des `ref struct`, des setters `init`, des types générés. Le bon réflexe est l'inverse : **cantonner tout cela à l'intérieur** de la bibliothèque et n'offrir, en surface, que des types et des méthodes **simples**.

C'est exactement l'**isolation de la volatilité** évoquée en 10.1. La bibliothèque C# joue le rôle d'une **façade** (ou d'une couche de traduction) : à l'intérieur, le code moderne peut évoluer librement ; à l'extérieur, la surface publique reste **stable et VB-friendly**. VB ne dialogue qu'avec cette façade et ignore tout du reste.

> 💡 **La bonne question à se poser en concevant la bibliothèque n'est pas « comment exploiter telle fonctionnalité C# ? » mais « de quoi l'appelant VB a-t-il besoin ? ».** Le contrat se conçoit **depuis le consommateur**, pas depuis l'implémentation.

---

## Concevoir une surface publique « VB-friendly »

Les règles esquissées en 9.3 deviennent ici des consignes **concrètes et actionnables**. Les voici, puis détaillées.

| Règle | Pourquoi | À la place |
|-------|----------|------------|
| Exposer des **constructeurs** | L'expression `with` échappe à VB ; `init` se règle via `With { }` mais le constructeur est plus direct | Constructeur public, ou record **positionnel** |
| Pas de `Span` / `ref struct` en surface | Consommation limitée en VB | `IEnumerable` / `IReadOnlyList` / tableaux / `Stream` |
| Marquer `[assembly: CLSCompliant(true)]` | Garantir la consommation inter-langages | Laisser le compilateur signaler les écarts |
| Pas de membres distingués par la **casse** | VB est insensible à la casse | Noms publics distincts |
| Éviter les **mots-clés VB** en noms publics | Échappement par `[ ]` côté VB | Renommer si possible |
| Exposer l'async via **`Task` / `Task(Of T)`** | Transparent pour VB (`Await` direct) | Un `IAsyncEnumerable` exposé se consommerait sans `Await For Each` (9.3) |
| **Valider** les arguments explicitement | VB **ignore** les annotations NRT | `ArgumentNullException` + documentation |

### Des constructeurs plutôt que des `init` seuls

VB instancie d'abord par **constructeur**. L'expression `with` de C# lui échappe entièrement ; quant aux propriétés `init`, VB sait les régler à l'initialisation via `With { }` (section 9.3), mais le constructeur reste la voie la plus directe et la plus découvrable. On en expose donc un — ce qu'un **record positionnel** fait gratuitement :

```csharp
// ✅ VB-friendly : le record positionnel génère un constructeur (Id, Libelle, Montant)
public record Enregistrement(int Id, string Libelle, decimal Montant);
```

```csharp
// ⚠️ Moins direct pour VB : pas de constructeur utile — l'appelant devra passer
// par l'initialiseur With { } (possible, mais moins découvrable qu'un constructeur)
public record Enregistrement
{
    public int Id { get; init; }
    public string Libelle { get; init; }
}
// → ajouter un constructeur public, ou préférer la forme positionnelle ci-dessus.
```

### Pas de `Span` ni de `ref struct` en surface publique

La performance `Span`-first reste un **détail interne** ; la frontière expose des types que VB consomme naturellement :

```csharp
public sealed class AnalyseurCsv
{
    // Surface PUBLIQUE : simple, VB-friendly
    public IReadOnlyList<Enregistrement> Analyser(Stream flux) { /* ... */ }

    // Machinerie INTERNE : Span / ref struct, jamais exposés
    private static Enregistrement AnalyserLigne(ReadOnlySpan<char> ligne) { /* ... */ }
}
```

### Conformité CLS

Un seul attribut, et le compilateur signale les écarts de la surface **publique** :

```csharp
[assembly: CLSCompliant(true)]
```

On évite alors, en public, les membres ne différant que par la casse, les entiers non signés, les pointeurs — autant de pièges pour un appelant VB.

### Async : exposer `Task`

L'asynchronie traverse la frontière de façon transparente dès lors qu'on s'en tient à `Task` / `Task<T>` :

```csharp
public Task<IReadOnlyList<Enregistrement>> AnalyserAsync(
    Stream flux, CancellationToken ct = default) { /* ... */ }
```

Côté VB, cela s'appelle par un simple `Await` (section 10.4).

### Nullabilité : valider, ne pas se reposer sur les annotations

On peut — et on doit, par bonne pratique C# — **annoter** la nullabilité en interne. Mais il faut se rappeler que **VB ignore ces annotations** (sections 2.2 et 9.3) : elles n'offrent **aucune** garantie côté appelant VB. La frontière **valide donc explicitement** ses arguments et **documente** ses attentes :

```csharp
public IReadOnlyList<Enregistrement> Analyser(Stream flux)
{
    ArgumentNullException.ThrowIfNull(flux);   // garde explicite, utile aussi pour VB
    // ...
}
```

---

## Penser le contrat : petit, stable, piloté par l'appelant

Au-delà des règles de types, la **forme du contrat** compte :

- **Une surface minimale.** Moins il y a de types publics, plus la bibliothèque est facile à consommer et à maintenir. Tout ce qui n'a pas à être public est `internal`.
- **Un point d'entrée unique** (façade). L'application VB dialogue avec **une** classe d'entrée, qui orchestre en interne la machinerie avancée.
- **Une interface, le cas échéant.** Exposer le contrat via une `interface` publique facilite les tests (*mocking* — module 13.2) et découple l'appelant VB de l'implémentation.
- **Masquer les types d'implémentation** : `internal` pour tout ce qui relève des coulisses (types `ref struct`, helpers, types générés).

```csharp
// Façade : un seul point d'entrée public, le reste est internal
public sealed class MoteurDeRapport
{
    public Rapport Construire(DemandeRapport demande) { /* ... */ }
}
```

```vb
' Côté VB : la machinerie C# est invisible
Dim moteur As New MoteurDeRapport()
Dim rapport = moteur.Construire(demande)
```

---

## Exemple fil rouge : un parseur `Span`-first à façade simple

Réunissons les principes sur un cas concret — une bibliothèque C# de parsing CSV performant, conçue pour être consommée depuis VB.

```csharp
[assembly: CLSCompliant(true)]

namespace Societe.Donnees;

// Modèle exposé : record positionnel → constructeur pour VB,
// égalité de valeur et ToString offerts gratuitement.
public record Enregistrement(int Id, string Libelle, decimal Montant);

// Façade : surface publique simple et stable
public sealed class AnalyseurCsv
{
    public IReadOnlyList<Enregistrement> Analyser(Stream flux)
    {
        ArgumentNullException.ThrowIfNull(flux);
        // ... lecture du flux, en déléguant en interne à AnalyserLigne ...
    }

    public Task<IReadOnlyList<Enregistrement>> AnalyserAsync(
        Stream flux, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(flux);
        // ...
    }

    // INTERNE : la performance Span-first reste cachée derrière la façade
    private static Enregistrement AnalyserLigne(ReadOnlySpan<char> ligne)
    {
        // ... parsing zéro-allocation ...
    }
}
```

Côté VB, le bénéfice est intact et la complexité invisible :

```vb
Dim analyseur As New AnalyseurCsv()
Dim lignes = analyseur.Analyser(flux)
For Each e In lignes
    Console.WriteLine($"{e.Id} : {e.Libelle} = {e.Montant}")
Next
```

La **performance vit en C#**, derrière la frontière ; VB ne manipule que des `IReadOnlyList`, des `Stream` et un record consommé par constructeur. Aucune des trois « fonctionnalités avancées » (record, `Span`, async) n'a fui dans la surface publique de façon problématique.

---

## Mise en place du projet

En pratique, on crée une **bibliothèque de classes C#** (`.csproj`) ciblant un TFM compatible (par exemple `net10.0`), que le projet VB référence par **référence de projet**. Quelques principes :

- **Une responsabilité par bibliothèque** : « le parseur performant », « le modèle moderne »… plutôt qu'un fourre-tout.
- Garder la bibliothèque **focalisée** sur la fonctionnalité isolée, sans y faire remonter de logique métier VB.

> 🔗 L'organisation fine d'une **solution mixte** (structure des projets, NuGet, build, tests) est traitée en détail à la **section 10.6**.

---

## Stabilité du contrat : faire vivre l'interne, figer l'externe

C'est le cœur du bénéfice de l'isolation : **la surface publique est un contrat**, dont toute modification se répercute sur l'appelant VB. On la veut donc **stable** et on la fait évoluer de façon **additive** (ajouter sans casser).

À l'inverse, la **machinerie interne** peut **changer librement** — c'était toute la raison d'isoler la volatilité (10.1). Refactoriser le parsing `Span`, remplacer un type généré, optimiser un algorithme : tant que la **façade** ne bouge pas, l'application VB **n'est pas affectée**.

---

## Documenter la frontière

La frontière est l'endroit où la documentation rapporte le plus :

- **Commentaires de documentation XML** sur la surface publique : ils alimentent l'**IntelliSense côté VB** (module 17.4 pour leur génération assistée).
- **Documenter explicitement la nullabilité et les préconditions**, puisque VB ne lit pas les annotations NRT.

Une frontière bien documentée rend la consommation décrite en **10.4** presque triviale.

---

## En résumé

- Faire de la fonctionnalité avancée un **détail interne** : la bibliothèque C# est une **façade** qui isole la volatilité (10.1) et n'expose qu'une surface **simple et stable**.
- Concevoir le contrat **depuis le besoin de l'appelant VB**, pas depuis l'implémentation.
- Surface **VB-friendly** : **constructeurs** (la voie directe pour VB), **pas de `Span`/`ref struct`** en public, `[assembly: CLSCompliant(true)]`, pas de membres distingués par la casse, async via **`Task`**, et **validation explicite** des arguments (VB ignore les annotations NRT).
- Contrat **minimal**, **point d'entrée unique** (façade), interface possible pour les tests, types d'implémentation en `internal`.
- **Figer la surface publique** (évolution additive) ; **laisser vivre l'interne** sans impacter VB.
- **Documenter** la frontière (XML doc → IntelliSense VB ; nullabilité explicite).

> 🔗 **Suite logique** : la section **10.4 — Consommer ces bibliothèques de façon transparente depuis VB.NET** prend le relais, côté **appelant**.

⏭️ [Consommer ces bibliothèques de façon transparente depuis VB.NET](/10-hybride-vbnet-csharp/04-consommer-depuis-vbnet.md)
