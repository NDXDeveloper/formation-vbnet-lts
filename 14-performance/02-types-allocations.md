🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 14.2 — Types valeur vs référence, allocations

**Comprendre où vit la mémoire et reconnaître les allocations cachées qui nourrissent le GC**

---

## 🎯 Pourquoi cette distinction est au cœur de la performance

Chaque objet créé sur le **tas** (*heap*) est, tôt ou tard, du travail pour le ramasse-miettes
(GC). Réduire la performance d'une application .NET à un seul levier serait abusif, mais s'il
fallait en désigner un de portée générale, ce serait celui-ci : **allouer moins**. Et pour
allouer moins, il faut d'abord savoir **ce qui** alloue — ce qui n'est pas toujours évident,
car beaucoup d'allocations sont **invisibles** dans le code source.

Tout part d'une distinction fondamentale du modèle mémoire .NET : **types valeur** contre
**types référence**. Elle détermine où vivent les données, comment elles sont copiées, et ce
qui finit — ou non — sur le tas.

Cette section pose la mécanique ; la **[14.3 (Garbage Collector)](03-gc.md)** explique ce que
le GC en fait, et la **[14.4 (`Span`, pooling)](04-span-pooling.md)** comment réduire les
allocations sur les chemins critiques.

---

## 🧱 La distinction fondamentale

| | **Types valeur** | **Types référence** |
|---|---|---|
| **Déclaration VB** | `Structure`, `Enum` | `Class`, `Interface`, `Delegate` |
| **Exemples** | `Integer`, `Long`, `Double`, `Decimal`, `Boolean`, `Char`, `Date`, énumérations, **tuples** `(a, b)`, `Nullable(Of T)` | `Object`, `String`, **tableaux** (même de types valeur !), collections, délégués |
| **Contient** | La **donnée** elle-même | Une **référence** (un pointeur) vers la donnée |
| **Affectation** | Copie **la valeur** | Copie **la référence** (pas l'objet) |
| **Vit où** | Selon le contexte (souvent la pile) | L'objet : **toujours** sur le tas |
| **Hérite** | Non (sauf interfaces) | Oui |
| **Défaut** | `Nothing` = zéro de la structure | `Nothing` = référence nulle |

Deux pièges de vocabulaire à lever d'emblée :

- **`String` est un type référence**, mais **immuable** : toute « modification » crée une
  nouvelle chaîne. C'est pourquoi la concaténation en boucle est coûteuse (voir plus bas) et
  qu'on lui préfère `StringBuilder` (→ **[module 2.6](../02-fondamentaux-langage/06-chaines.md)**).
- **Un tableau est toujours un type référence**, même un tableau de types valeur. `Dim n(99)
  As Integer` alloue **un seul** objet sur le tas contenant 100 entiers **côte à côte**
  (*inline*), sans boxing ni allocation par élément.

---

## 📑 Sémantique de copie : la conséquence pratique

Un type valeur se copie **intégralement** lors d'une affectation ou d'un passage de paramètre ;
un type référence ne copie que le pointeur — les deux variables désignent alors **le même
objet**.

```vb
' --- Type valeur : copie indépendante ---
Structure Point
    Public X As Integer
    Public Y As Integer
End Structure

Dim a As New Point With {.X = 1, .Y = 2}
Dim b = a          ' b est une COPIE
b.X = 99
' a.X vaut toujours 1 — a et b sont indépendants

' --- Type référence : partage du même objet ---
Class Boite
    Public Valeur As Integer
End Class

Dim x As New Boite With {.Valeur = 1}
Dim y = x          ' y pointe vers le MÊME objet
y.Valeur = 99
' x.Valeur vaut maintenant 99 — x et y partagent l'objet
```

C'est de là que vient la règle d'or des structures : **une structure devrait être immuable**.
Une structure *mutable* surprend constamment, car on modifie souvent une copie sans s'en rendre
compte (un élément retourné par une propriété, une variable de boucle, un argument passé `ByVal`).
On y revient dans le choix `Structure` vs `Class`.

---

## 🗺️ Où vit la mémoire : pile et tas

On enseigne souvent — par commodité — que *« les types valeur vont sur la pile, les types
référence sur le tas »*. C'est une **simplification utile mais inexacte**. La règle précise est
la suivante :

- **Un objet de type référence est *toujours* alloué sur le tas.** Seule la **référence** (le
  pointeur) peut se trouver sur la pile, quand c'est une variable locale.
- **L'emplacement d'un type valeur dépend de son contexte :**
  - variable **locale** ou **paramètre** → généralement sur la **pile** (voire dans un registre) ;
  - **champ d'une classe** → stocké *inline* **dans l'objet** de cette classe, donc **sur le tas** ;
  - **élément d'un tableau** → *inline* dans le tableau, donc **sur le tas** ;
  - **capturé par une lambda** (*closure*) → déplacé dans un objet généré, donc **sur le tas** ;
  - **boxé** → une copie est placée **sur le tas** (voir ci-dessous).

La distinction qui compte vraiment pour la performance n'est donc pas « pile ou tas » dans
l'absolu, mais : **est-ce que ceci provoque une allocation sur le tas ?** Car :

- l'« allocation » sur la pile est quasi **gratuite** (un simple déplacement du pointeur de pile,
  libéré automatiquement à la sortie de la portée) ;
- l'allocation sur le tas a un coût d'allocation, **et surtout** un coût différé : le GC devra la
  suivre puis la collecter. C'est ce qu'on appelle la **pression mémoire** (*memory pressure*).

---

## 📦 Le boxing : le piège classique

Le **boxing** (« mise en boîte ») se produit lorsqu'un **type valeur est converti en type
référence** — vers `Object`, ou vers une interface qu'il implémente. Le runtime **alloue alors
une boîte sur le tas**, y **copie** la valeur, et renvoie une référence. L'opération inverse,
l'*unboxing*, recopie la valeur depuis la boîte. C'est à la fois une **allocation** *et* une
**copie** — et la boîte deviendra un déchet à collecter.

```vb
Dim n As Integer = 42
Dim o As Object = n      ' BOXING : allocation d'une boîte sur le tas + copie
Dim m As Integer = CInt(o) ' UNBOXING : copie depuis la boîte
```

Le danger du boxing, c'est qu'il est souvent **invisible**. Quelques sources fréquentes :

- **Les collections non génériques** (`ArrayList`, `Hashtable`, l'ancien `Collection` de VB) :
  elles stockent des `Object`, donc **chaque** type valeur ajouté est boxé.
- **L'affectation à une variable `Object`** ou le passage à une API typée `Object`.
- **L'appel d'une méthode d'interface** sur une valeur détenue *via* une variable de type
  interface (la valeur est boxée pour être manipulée comme référence).
- **L'interpolation de chaînes en VB.NET** — un point spécifique au langage : `$"..."` est
  traduit en appel à `String.Format`, dont les arguments sont de type `Object`. Les types valeur
  insérés y sont donc **boxés**.

> ⚠️ **Nuance VB.NET (et exemple concret du « langage figé »).** En C#, l'interpolation bénéficie
> depuis .NET 6 d'un *interpolated string handler* capable d'**éviter** ce boxing. VB.NET, en mode
> *consumption-only*, n'a **pas** reçu cette optimisation de langage : son interpolation continue
> de boxer les valeurs. En pratique, cela reste négligeable dans la plupart des cas, mais devient
> mesurable dans une **boucle chaude** produisant beaucoup de chaînes — un endroit où l'on
> privilégiera un format explicite ou `StringBuilder`.

**Le remède au boxing : les génériques.** Une collection générique stocke les types valeur
*inline*, **sans** boxing.

```vb
' ❌ Boxe chaque entier (stockage en Object)
Dim ancienne As New ArrayList()
ancienne.Add(42)

' ✅ Aucun boxing : les Integer sont stockés tels quels
Dim moderne As New List(Of Integer)()
moderne.Add(42)
```

C'est la raison de fond pour laquelle on préfère **toujours** `List(Of T)` et
`Dictionary(Of TKey, TValue)` aux collections d'antan (→ **[module 2.8](../02-fondamentaux-langage/08-tableaux-collections.md)**).

---

## 🕳️ Les autres allocations cachées

Le boxing n'est pas la seule allocation invisible. Voici les principales à connaître — non pour
les bannir (la lisibilité prime hors des chemins chauds), mais pour les **reconnaître** quand le
profilage pointe une pression d'allocation.

- **Fermetures et lambdas capturantes.** Une lambda qui **capture** une variable locale fait
  générer par le compilateur une **classe de fermeture** (sur le tas), plus l'instance de
  délégué. Dans une boucle, ces allocations répétées s'accumulent vite. À l'inverse, une lambda **sans
  capture** est mise en cache par le compilateur VB dans un champ statique : elle n'est allouée
  **qu'une fois**.

  ```vb
  ' Capture 'seuil' → alloue une fermeture (à chaque itération de la boucle externe)
  For Each lot In lots
      Dim seuil = lot.Seuil
      Dim filtres = elements.Where(Function(e) e.Score > seuil)
      ' ...
  Next
  ```

- **LINQ.** Les opérateurs de requête allouent des **itérateurs**, des **délégués**, et parfois
  des **tampons intermédiaires**. LINQ est un **point fort** de VB.NET (→ **[2.9](../02-fondamentaux-langage/09-linq.md)**)
  et reste le bon choix pour la clarté ; on réserve simplement sa réécriture en boucle explicite
  aux chemins critiques **identifiés par la mesure**.

- **Concaténation de chaînes en boucle.** Chaque `&` produit une **nouvelle** chaîne (immuabilité
  oblige). Sur de nombreuses itérations, on accumule autant de chaînes intermédiaires jetables.

  ```vb
  ' ❌ N allocations de chaînes intermédiaires
  Dim s As String = ""
  For Each ligne In lignes
      s &= ligne & vbCrLf
  Next

  ' ✅ Un seul tampon réutilisé
  Dim sb As New System.Text.StringBuilder()
  For Each ligne In lignes
      sb.AppendLine(ligne)
  Next
  Dim resultat = sb.ToString()
  ```

- **Énumérateurs.** `For Each` sur une `List(Of T)` utilise un **énumérateur de type valeur** :
  **aucune allocation**. Mais itérer cette même liste *via* une variable de type `IEnumerable(Of T)`
  **boxe** l'énumérateur — une allocation supplémentaire par parcours.

- **`ParamArray`.** Un paramètre `ParamArray` alloue un **tableau** à chaque appel, même pour
  zéro argument transmis.

- **Machines à états asynchrones.** Une méthode `Async` qui bascule réellement en asynchrone
  alloue une machine à états et une `Task`. C'est rarement un problème, mais sur des chemins très
  sollicités, `ValueTask` peut l'atténuer (→ **[module 4.6](../04-async/06-async-streams.md)**).

---

## ⚖️ `Structure` ou `Class` ? La règle de décision

Par **défaut, choisir `Class`.** On opte délibérément pour `Structure` lorsque le type réunit
**toutes** les conditions suivantes :

- il **représente logiquement une seule valeur**, à la manière d'un type primitif (un point, une
  couleur, un montant, une plage) ;
- il est **petit** (repère usuel : autour de **16 octets** ; au-delà de ~24 octets, le coût de
  copie tend à dépasser le bénéfice d'éviter le tas) ;
- il est **immuable** ;
- il ne sera **pas boxé fréquemment** (sinon les allocations de boîtes annulent l'intérêt).

Si l'un de ces points n'est pas vérifié, c'est une `Class`. Un type valeur volumineux, mutable
ou souvent boxé combine généralement le pire des deux mondes : coûts de copie **et** allocations.

### Spécificités VB.NET à connaître

- **Pas de `readonly struct`.** VB.NET ne permet pas de déclarer une structure entière en lecture
  seule (le `readonly struct` de C# 7.2). On obtient l'immuabilité **par convention** : tous les
  champs en `ReadOnly`, aucune méthode mutante.
- **Pas de paramètre `in` (référence en lecture seule).** Pour passer une **grande** structure
  sans la copier, on utilise `ByRef` — mais sans la garantie de non-modification qu'offrirait le
  `in` de C#. C'est un compromis à assumer, ou à isoler côté C# si la performance l'exige.
- **Pas de `ref struct`.** Les structures « pile uniquement » (comme `Span(Of T)`) ne se
  **déclarent** pas en VB — elles se **consomment** (→ **[14.4](04-span-pooling.md)** et la
  **[stratégie hybride, module 10](../10-hybride-vbnet-csharp/README.md)** 🔗).
- **Tuples valeur.** La syntaxe `(a, b)` repose sur `ValueTuple`, un **type valeur** : pas
  d'allocation sur le tas, contrairement à l'ancien `Tuple(Of …)` qui était une classe
  (→ **[module 3.2](../03-poo/02-structures-tuples.md)**).

---

## 🔬 Voir les allocations, ne pas les deviner

Le principe de la **[14.1](01-profilage.md)** s'applique tout autant ici : on ne **suppose** pas
qu'un code alloue, on le **mesure**. Pour rendre les allocations visibles :

- **Visual Studio 2026** — l'outil **Suivi des allocations d'objets .NET** : chaque allocation et
  sa pile d'appel, type par type.
- **`dotnet-trace collect --profile gc-verbose`** — la trace des allocations et de l'activité GC,
  hors débogueur.
- **`dotnet-counters`** — le compteur **`alloc-rate`** (octets alloués par seconde) pour repérer
  d'un coup d'œil une pression d'allocation anormale.

Un signal caractéristique à surveiller : un **taux d'allocation élevé** accompagné de **GC gen-0
fréquents** trahit presque toujours un excès d'objets éphémères — souvent du boxing ou des
fermetures dans une boucle chaude.

---

## 🔁 En résumé

- **Types valeur** : la donnée elle-même, copiée par valeur, souvent hors du tas — mais *inline*
  sur le tas dès qu'ils sont champ, élément de tableau, capturés ou boxés.
- **Types référence** : l'objet vit **toujours** sur le tas ; l'affectation partage le pointeur.
- Le vrai critère de performance n'est pas « pile ou tas », mais **« est-ce que ceci alloue sur
  le tas ? »** — chaque allocation est du travail futur pour le GC.
- Le **boxing** et les **allocations cachées** (fermetures, LINQ, concaténation, énumérateurs
  d'interface, `ParamArray`) sont les coupables les plus fréquents ; les **génériques** éliminent
  le boxing des collections.
- On choisit `Structure` avec parcimonie — petit, immuable, à sémantique de valeur — et `Class`
  par défaut.

La suite logique : comprendre **comment le GC traite tout cela** — générations, modes et
libération déterministe des ressources — dans la **[14.3 (Garbage Collector)](03-gc.md)**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Garbage Collector (générations, modes, tuning, IDisposable/Using, finaliseurs)](/14-performance/03-gc.md)
