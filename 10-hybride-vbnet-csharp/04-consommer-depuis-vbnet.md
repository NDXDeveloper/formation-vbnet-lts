🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 10.4 Consommer ces bibliothèques de façon transparente depuis VB.NET

> **Côté appelant : une bibliothèque C# bien conçue se consomme exactement comme une bibliothèque VB — la couture hybride disparaît au site d'appel.**

La section 10.3 a conçu la frontière du **côté auteur**. Celle-ci prend le relais du **côté consommateur** : comment, concrètement, le code VB.NET appelle une bibliothèque C#, quels idiomes il emploie, et quels rares points méritent sa vigilance. Le mécanisme sous-jacent ayant été établi au chapitre 9, on se concentre ici sur la **pratique de l'appel**.

---

## Le principe : du .NET ordinaire

C'est le mot du titre : **transparent**. Une fois la bibliothèque référencée, un type écrit en C# n'est, pour VB, qu'un **type .NET comme un autre**. On l'instancie, on appelle ses méthodes, on lit ses propriétés, on s'abonne à ses événements — **comme s'il était écrit en VB**.

```vb
Imports Societe.Donnees          ' l'espace de noms de la bibliothèque C#

Dim analyseur As New AnalyseurCsv()
Dim lignes = analyseur.Analyser(flux)
```

Rien dans ce code ne trahit que `AnalyseurCsv` vient de C#. C'est précisément l'objectif de la frontière VB-friendly conçue en 10.3 : **rendre l'origine du code indétectable** au point d'utilisation.

---

## Référencer et importer

Trois voies donnent accès à la bibliothèque, selon le contexte :

- **Référence de projet** — le cas courant de l'hybride : la bibliothèque C# et l'application VB cohabitent dans une **même solution** (organisation détaillée en **section 10.6**).
- **Paquet NuGet** — si la bibliothèque C# est empaquetée (qu'elle soit interne ou tierce).
- **Référence de DLL** — pour une assembly compilée fournie telle quelle.

Dans tous les cas, on importe ensuite l'espace de noms avec `Imports`, et l'IntelliSense expose la surface publique.

---

## Consommer les fonctionnalités déléguées, côté VB

Voici, pour chaque catégorie déléguée (section 10.2), **ce que le code VB écrit réellement** — et la seule limite à connaître.

| Fonctionnalité déléguée | Côté VB, on écrit… | Limite éventuelle |
|-------------------------|--------------------|-------------------|
| **Record C#** | `New Type(args)`, lecture des propriétés, `.Equals` (et `With { }` pour les propriétés `init`) | Pas d'expression `with` → **reconstruire par constructeur** (ou `WithXxx` côté C#) |
| **API encapsulant `Span` / perf** | un appel **ordinaire** (`IEnumerable`, `Stream`…) | **aucune** — le `Span` est caché |
| **Type issu d'un générateur** | un usage **ordinaire** | **aucune** |
| **Méthode async (`Task`)** | `Await methode(...)` | (dans une méthode `Async`) |
| **Paramètre `out` / `ref`** | `ByRef`, variable **déclarée au préalable** | pas de `out var` en ligne |
| **Méthode d'extension** | appel **comme une extension** (espace importé) | — |
| **Tuple de valeur** | lecture **par nom d'élément** | — |

Les cas notables :

### Records

On instancie par **constructeur**, on lit les propriétés, l'**égalité de valeur** fonctionne :

```vb
' Côté C# : public record Enregistrement(int Id, string Libelle, decimal Montant);
Dim e1 = New Enregistrement(1, "Article", 9.9D)
Console.WriteLine(e1.Libelle)             ' lecture : OK

Dim e2 = New Enregistrement(1, "Article", 9.9D)
Dim memeValeur = e1.Equals(e2)            ' True : égalité de valeur
```

Ce que VB **ne fait pas** : l'expression **`with`** de C# (copie modifiée). Pour obtenir une variante, on **reconstruit** un nouvel objet par le constructeur — ou la bibliothèque prévoit des méthodes `WithXxx` (section 9.3). Les propriétés **`init`**, elles, se règlent sans difficulté à l'initialisation, par le constructeur ou par `With { }`.

### API encapsulant `Span` / la performance

C'est le cas le plus satisfaisant : **il n'y a rien de spécial à faire**. La machinerie `Span`-first étant cachée derrière la façade (10.3), VB ne voit qu'une surface simple :

```vb
Dim lignes = analyseur.Analyser(flux)     ' IReadOnlyList(Of Enregistrement)
For Each e In lignes
    ' ... logique métier en VB ...
Next
```

### Async

`Await` consomme une méthode C# renvoyant `Task` de façon transparente :

```vb
' Côté C# : Task<IReadOnlyList<Enregistrement>> AnalyserAsync(Stream flux, CancellationToken ct = default)
Dim lignes = Await analyseur.AnalyserAsync(flux)   ' dans une méthode Async
```

### Paramètres `out` / `ref`

VB les passe en **`ByRef`** ; la variable doit être **déclarée au préalable** (pas de déclaration en ligne) :

```vb
' Côté C# : bool TryConvertir(string texte, out decimal valeur)
Dim valeur As Decimal
If convertisseur.TryConvertir("12.5", valeur) Then
    ' ... utiliser valeur ...
End If
```

---

## ⚠️ Points de vigilance côté consommation

La frontière étant bien conçue, les surprises sont rares — mais il faut connaître celles qui restent.

- **Garder `Option Strict On`.** La consommation d'une bibliothèque C# typée est **entièrement à liaison anticipée** : pas de `Object` dynamique, pas besoin de désactiver quoi que ce soit. C'est un **contraste net avec l'automation COM/Office** (section 9.2), qui exigeait `Option Strict Off`. Ici, on **conserve** toutes les garanties de `Option Strict On`. ✅
- **La nullabilité n'est pas vérifiée.** VB **ignore** les annotations NRT de la bibliothèque (sections 2.2 et 9.3) : aucun avertissement n'est émis. On **vérifie donc soi-même** les valeurs susceptibles d'être `Nothing` :

```vb
Dim resultat = service.RechercherOuNothing(cle)
If resultat IsNot Nothing Then
    ' VB n'a reçu aucun filet de sécurité de nullabilité : on contrôle explicitement
End If
```

- **Mots-clés VB.** Si un membre C# porte un nom qui est un **mot-clé VB**, on l'échappe avec des crochets côté appelant :

```vb
objet.[Stop]()        ' "Stop" est un mot-clé VB
```

- **Casse.** VB étant insensible à la casse, une bibliothèque exposant deux membres ne différant **que par la casse** serait ambiguë — mais une bibliothèque **conforme CLS** (garantie au moment de sa conception, 10.3) ne présente jamais ce cas.

---

## L'expérience de développement

L'hybride n'est pas qu'une affaire de compilation : l'outillage suit la frontière.

- **IntelliSense** expose la surface publique de la bibliothèque C# — avec sa **documentation XML** si elle a été rédigée (10.3), directement dans l'éditeur VB.
- **Atteindre la définition** (*Go to Definition*) navigue vers le type C# (source si même solution, vue décompilée sinon).
- **Le débogage traverse la frontière** : lorsque les symboles sont disponibles (cas d'une même solution), on **entre pas à pas** dans le code C# depuis le code VB, et inversement.

Autrement dit, l'expérience reste **continue** : la frontière VB/C# se franchit aussi naturellement à l'édition et au débogage qu'à l'exécution.

---

## 🤖 Traduire un exemple d'appel C# en VB

En pratique, la documentation et les réponses d'IA fournissent souvent un **exemple d'utilisation en C#**. Le transposer en VB au site d'appel est **direct** : c'est précisément l'objet de l'**[Annexe A — Correspondance syntaxique VB.NET ↔ C#](../annexes/correspondance-vbnet-csharp/README.md)**, et une tâche que les assistants IA réalisent bien (module 17). On précise « **Visual Basic .NET** » dans la requête, et l'on vérifie la transposition — `new` → `New`, `await` → `Await`, `out` → `ByRef`, etc.

---

## Le résultat : la couture disparaît

Mis bout à bout, ces éléments produisent l'effet recherché : **au site d'appel, on ne distingue plus une bibliothèque C# d'une bibliothèque VB**. On instancie, on appelle, on `Await`, sous `Option Strict On`, avec IntelliSense et débogage continus. La complexité moderne reste **de l'autre côté de la façade** ; l'application VB.NET, elle, ne manipule que des types simples et idiomatiques.

C'est l'aboutissement concret de la stratégie hybride : VB **consomme** pleinement le .NET moderne, sans renoncer à ce qui fait sa productivité.

---

## En résumé

- Une bibliothèque C# référencée est, pour VB, **du .NET ordinaire** : on l'utilise comme un type VB, après `Imports` de son espace de noms.
- Trois voies d'accès : **référence de projet** (cas hybride courant), **NuGet**, **DLL**.
- Côté VB on écrit : `New Type(args)` pour les **records** (lecture + égalité ; `init` se règle via `With { }`, seule l'expression `with` échappe à VB) ; un appel **ordinaire** pour les API encapsulant `Span` ; `Await` pour l'**async** ; **`ByRef`** pour `out`/`ref` (variable déclarée).
- Vigilance : **garder `Option Strict On`** (contraste avec COM/Office) ✅ ; **vérifier soi-même les `Nothing`** (NRT ignorées) ; échapper les **mots-clés VB** par `[ ]`.
- L'**outillage suit** : IntelliSense, *Go to Definition* et **débogage** traversent la frontière.
- Bien menée, la consommation est **transparente** : la couture hybride **disparaît** au site d'appel.

> 🔗 **Suite logique** : la section **10.5 — Atelier** met en œuvre, de bout en bout, un cœur en C# (performance / fonctionnalités) avec une UI et un métier en VB.NET.

⏭️ [Atelier : cœur en C# (performance / fonctionnalités), UI et métier en VB.NET](/10-hybride-vbnet-csharp/05-atelier-core-csharp-ui-vbnet.md)
