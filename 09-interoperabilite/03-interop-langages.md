🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 9.3 Interopérabilité entre langages .NET (VB ↔ C#, F#)

> **Faire collaborer VB.NET, C# et F# dans une même solution — le socle technique sur lequel repose toute la stratégie hybride de 2026.**

Cette section est sans doute la plus importante du chapitre au regard de la stratégie d'ensemble de cette formation. Alors que la 9.1 et la 9.2 tournaient le regard vers le monde **natif** (Win32, COM), celle-ci décrit l'interopérabilité **à l'intérieur** de l'écosystème .NET : comment un projet VB.NET consomme, sans la moindre friction, du code écrit en **C#** (et, plus rarement, en **F#**). C'est précisément ce mécanisme qui rend possible la réponse pragmatique au gel du langage : **déléguer à C# les briques modernes et les consommer depuis VB.NET** — l'architecture détaillée au **module 10** 🔗.

---

## Le socle commun : un seul runtime, un seul système de types

Si les langages .NET interopèrent aussi naturellement, c'est qu'ils partagent les mêmes fondations :

- ils compilent tous vers un **langage intermédiaire commun**, l'**IL** (*Intermediate Language*) ;
- ils s'exécutent sur le même **CLR** (*Common Language Runtime*) ;
- ils s'appuient sur le **CTS** (*Common Type System*), un système de types unifié.

Conséquence concrète : une classe écrite en C# n'est, **au niveau de l'IL, qu'un type .NET** comme un autre. Du point de vue de VB.NET, rien ne la distingue d'une classe VB. Un projet VB peut donc **référencer** un projet ou une assembly C# (ou F#) — par référence de projet dans une même solution, par paquet NuGet ou par référence de DLL — et en utiliser les types directement.

> 💡 **La réalité quotidienne.** L'écrasante majorité des paquets **NuGet** sont écrits en C#. Lorsque vous installez un paquet et en consommez les types depuis VB.NET, vous faites déjà, sans y penser, de l'interopérabilité inter-langages. La stratégie hybride ne fait que **systématiser** cette pratique en y ajoutant **vos propres** bibliothèques C#.

---

## Le CLS : garantir la consommation inter-langages

Le CTS est vaste, et tous les langages n'en exploitent pas chaque recoin. Le **CLS** (*Common Language Specification*) définit le **sous-ensemble** de fonctionnalités que **tout** langage .NET est tenu de comprendre. Une règle simple en découle :

> **Si la surface publique d'une bibliothèque reste conforme au CLS, elle est garantie consommable depuis n'importe quel langage .NET** — dont VB.NET.

On active la vérification au niveau de l'assembly :

```vb
<Assembly: CLSCompliant(True)>
```

Le compilateur signale alors tout élément **public** non conforme. Parmi les pièges classiques côté CLS, plusieurs concernent directement VB :

- des membres publics qui **ne diffèrent que par la casse** (interdits — voir le piège de la casse plus bas) ;
- certains types entiers exposés publiquement : les **non signés** (`UInteger`, `ULong`, `UShort`) et, à l'inverse, `SByte` (l'octet **signé**) ;
- des **pointeurs** dans la surface publique.

La conformité CLS n'a d'importance que pour l'**API publique** d'une bibliothèque destinée à être consommée hors de son langage d'origine — exactement le cas d'une bibliothèque C# que VB.NET doit utiliser.

---

## La traduction des fonctionnalités au passage de la frontière

La plupart des constructions traversent la frontière de façon **transparente** ; quelques-unes, propres à C#, sont **consommables mais non déclarables** en VB — le fil rouge de cette formation. Vue d'ensemble :

| Fonctionnalité C# / F# | Côté VB.NET |
|------------------------|-------------|
| Types, classes, interfaces, énumérations | ✅ Transparents (mêmes types CLR) |
| Paramètres `out` / `ref` | ✅ Passés en `ByRef` (variable déclarée au préalable) |
| Tuples de valeur `(T1, T2)` | ✅ Même `ValueTuple`, noms d'éléments inclus |
| Méthodes d'extension | ✅ Consommables (et déclarables en VB via `<Extension>`) |
| Paramètres optionnels / arguments nommés | ✅ Compatibles |
| `async` / `Task` | ✅ Transparents (`Await` fonctionne tel quel) |
| `IAsyncEnumerable(Of T)` | ⚠️ Consommable, mais **sans `Await For Each`** en VB → énumérateur manuel (module 4.6) |
| *Records* (`record`) | ⚠️ Consommables (instancier via le constructeur) — **non déclarables** en VB 🔗 |
| `init`-only, expression `with` | ⚠️ `init` se règle par le constructeur **ou** l'initialiseur `With { }` (VB 16.9) ; seule l'expression `with` de copie reste C#-only |
| `Span(Of T)` / `ref struct` | ⚠️ Consommation **limitée** — déléguer la manipulation fine à C# |
| *Source generators* (résultat compilé) | ✅ Consommables — ⚠️ **écrire** un générateur reste C#-only 🔗 |
| *Nullable reference types* (`string?`) | ⚠️ Annotations **C#-only**, ignorées en VB (≠ `Nullable(Of T)`) |

Les cas transparents méritent peu de commentaires ; les cas ⚠️ valent qu'on s'y arrête.

### Paramètres `out` / `ref`

VB ne distingue pas `out` et `ref` : les deux se passent en **`ByRef`**. À la différence de C#, VB **n'a pas de déclaration en ligne** (`out var`) : la variable doit être déclarée au préalable.

```vb
' Appel d'une méthode C# : public bool TryParse(string s, out int value)
Dim valeur As Integer
If parseur.TryParse("42", valeur) Then
    Console.WriteLine(valeur)
End If
```

### *Records* C#

VB ne **déclare** pas de `record`, mais en **consomme** un sans difficulté. Un record positionnel C# expose un **constructeur** : on l'instancie par là, et l'**égalité de valeur** comme le `ToString` générés fonctionnent normalement.

```vb
' Côté C# : public record Personne(string Nom, int Age);
Dim p1 = New Personne("Alice", 30)
Console.WriteLine(p1.Nom)            ' lecture de propriété : OK

Dim p2 = New Personne("Alice", 30)
Dim identiques = p1.Equals(p2)       ' True : égalité de valeur
```

Les propriétés **`init`-only**, elles, se règlent côté VB **au choix** par le constructeur ou par l'**initialiseur `With { }`** — c'est précisément la capacité de consommation apportée par VB 16.9, vérifiée sur .NET 10 (le compilateur les verrouille ensuite, erreur BC37311 à la réassignation). Seule l'expression **`with`** de C# (mutation non destructive) reste hors de portée : pour la copie modifiée, on prévoit des méthodes `WithXxx` côté C#. (Rappel du module 3.7 et de l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md) § B.7.)

### `Span(Of T)` et `ref struct`

VB peut **consommer** certaines API fondées sur `Span(Of T)`, mais le langage prend en charge de façon **limitée** la mécanique des `ref struct` (types cantonnés à la pile, non boxables, restrictions d'usage). Pour toute **manipulation fine** — traitements haute performance, parsing sans allocation — on délègue à une **bibliothèque C#** que VB consomme via une surface simple (tableaux, `IEnumerable`). Voir le **module 14.4** et l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**.

### *Source generators*

Une bibliothèque C# peut s'appuyer sur des **générateurs de source** pour produire du code répétitif : du point de vue de VB, on consomme simplement les **types compilés** qui en résultent, de façon transparente. Mais **écrire** un générateur de source — ou en faire exécuter un **sur du code VB** — reste **C#-only**. C'est la raison pour laquelle, par exemple, on utilise en VB les classes de base de `CommunityToolkit.Mvvm` (`ObservableObject`, `RelayCommand`) plutôt que ses attributs générateurs (module 6.6). 🔗

### *Nullable reference types*

Les **annotations de nullabilité des types référence** de C# (`string?`, `#nullable enable`) sont de simples **métadonnées de compilation**, sans effet à l'exécution. VB.NET **ne les interprète pas** : en consommant une bibliothèque C# annotée, VB ne reçoit aucun avertissement de nullabilité. À ne pas confondre avec `Nullable(Of T)` / `T?`, qui désigne en VB les **types valeur** nullables (module 2.2). ⚠️

---

## ⚠️ Le piège de la casse — et les mots-clés

**VB.NET est insensible à la casse ; C# et F# y sont sensibles.** Deux conséquences pratiques au passage de la frontière :

- Une bibliothèque C# qui exposerait deux membres publics ne différant **que par la casse** (`Valeur` et `valeur`) serait **inutilisable** depuis VB, qui ne saurait les distinguer. C'est précisément ce que la conformité **CLS interdit**.
- Si une bibliothèque expose un membre dont le nom est un **mot-clé VB** (par exemple une méthode `Stop`), on l'**échappe** avec des crochets côté VB :

```vb
objet.[Stop]()        ' "Stop" est un mot-clé VB : crochets obligatoires
```

---

## Concevoir une frontière « VB-friendly » 🔗

Quand on conçoit une bibliothèque C# **destinée à être consommée depuis VB** (le cœur de la stratégie hybride), quelques principes rendent l'usage fluide :

- **exposer des constructeurs** pour instancier les records et objets immuables — VB sait certes régler les propriétés `init` via `With { }`, mais le constructeur reste la voie la plus directe et la plus découvrable ;
- **proscrire les membres publics qui ne diffèrent que par la casse** ;
- **éviter d'exposer** `ref struct` / `Span(Of T)` dans la surface publique consommée par VB ; offrir des surcharges à base de tableaux ou d'`IEnumerable` ;
- pour une conformité stricte, **éviter les entiers non signés** dans l'API publique ;
- **marquer l'assembly `<CLSCompliant(True)>`** et laisser le compilateur signaler les écarts.

Ces règles transforment le mot d'ordre du module 10 — *« cœur en C#, UI et métier en VB.NET »* — en une réalité **sans couture**.

---

## Et F# ?

F# partage le même CLR et le même IL : une bibliothèque F# **conforme au CLS** se consomme depuis VB comme n'importe quelle autre. La nuance vient de l'**idiomatique** F# : certains types très F# (unions discriminées, `option`, listes F#) sont **peu commodes** à consommer depuis VB ou C#. Les bibliothèques F# pensées pour un usage inter-langages exposent donc en général une **surface compatible** (classes, interfaces, types CLR standards). En pratique, le scénario de mixité réaliste reste **VB ↔ C#** ; l'association **VB ↔ F#** est nettement plus rare.

---

## La place de cette section dans la formation 🔗

L'interopérabilité inter-langages est le **moyen technique** ; trois autres parties en exploitent les conséquences :

- **Module 10 — Architecture hybride VB.NET / C#** : l'**architecture** qui généralise cette mécanique (isoler les briques modernes en C#, les consommer depuis VB, gérer une solution mixte — module 10.6).
- **[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)** : le **catalogue** des sujets à déléguer à C# (records, source generators, `Span`-first, Minimal APIs, Native AOT…) avec, pour chacun, la façon de le consommer depuis VB.
- **Module 11 — Migration** : la coexistence progressive de l'ancien et du nouveau pendant une modernisation.

---

## En résumé

- VB, C# et F# partagent **un seul runtime** (CLR), **un seul IL** et **un seul système de types** (CTS) : un projet VB référence et consomme une bibliothèque C# de façon **transparente** — c'est déjà ce qu'on fait avec **NuGet**.
- Le **CLS** garantit la consommation inter-langages d'une **API publique** ; on l'active avec `<Assembly: CLSCompliant(True)>`.
- La plupart des constructions traversent la frontière sans effort (`out`/`ref` → `ByRef`, tuples, méthodes d'extension, `async`…). Quelques-unes sont **consommables mais non déclarables** en VB : **records**, **source generators**, **`Span`/`ref struct`**, **`IAsyncEnumerable`** (consommé sans `Await For Each`, module 4.6) et les **annotations NRT** (ignorées). 🔗
- Attention à la **casse** (VB insensible) et aux **mots-clés** (échappés par `[ ]`).
- Pour une bibliothèque C# destinée à VB, **concevoir une frontière VB-friendly** : constructeurs explicites, pas de membres distingués par la seule casse, surfaces sans `ref struct`, assembly CLS-conforme.

> 🔗 **Suite logique** : la section **9.4 — WebView2** clôt le chapitre en ouvrant l'interopérabilité vers le **web moderne** embarqué dans une application de bureau.

⏭️ [WebView2 (intégrer du web moderne dans une application de bureau)](/09-interoperabilite/04-webview2.md)
