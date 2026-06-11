🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.8 Tableaux et collections

> Les tableaux sont la structure la plus basique (taille fixe, accès par indice) ; les **collections
> génériques** (`List(Of T)`, `Dictionary(Of TKey, TValue)`) sont l'outil de tous les jours ; et des
> **collections spécialisées** répondent à des besoins précis — liaison de données (`ObservableCollection`)
> ou accès concurrent. Un piège VB à retenir d'emblée : dans `Dim a(4)`, le `4` est la **borne
> supérieure**, pas la taille.

---

## Tableaux

### Tableaux unidimensionnels

> ⚠️ **Le nombre entre parenthèses est la borne supérieure (dernier indice), pas le nombre d'éléments.**
> `Dim nombres(4) As Integer` crée un tableau de **5 éléments**, d'indices 0 à 4. C'est l'une des
> différences les plus surprenantes avec C# (où `new int[4]` donne 4 éléments).

```vb
Dim nombres(4) As Integer          ' 5 éléments (indices 0..4), initialisés à 0
nombres(0) = 10                    ' accès par parenthèses (pas de crochets)
Console.WriteLine(nombres.Length)  ' 5

' Avec initialiseur : la taille est déduite
Dim premiers() As Integer = {2, 3, 5, 7, 11}
Dim mots = New String() {"a", "b", "c"}
```

Un tableau est un **type référence** de **taille fixe** une fois créé. `.Length` donne le nombre total
d'éléments ; en .NET, l'indice inférieur est toujours 0.

VB offre un mécanisme propre pour **redimensionner** : `ReDim`. Seul, il crée un nouveau tableau (le
contenu est perdu) ; avec **`Preserve`**, il conserve les éléments existants :

```vb
ReDim Preserve nombres(9)   ' passe à 10 éléments, contenu conservé
```

> `ReDim Preserve` **recopie** les éléments (coût en O(n)). Pour une structure qui grandit souvent,
> préférez `List(Of T)` (ci-dessous), conçue pour cela.

### Tableaux multidimensionnels

Deux familles, à la syntaxe distincte :

- **Rectangulaires** (`(,)`) : une vraie grille, toutes les lignes de même longueur.

  ```vb
  Dim grille(2, 3) As Integer        ' 3 × 4 = 12 éléments (bornes 0..2 et 0..3)
  grille(1, 2) = 7
  Console.WriteLine(grille.GetLength(0))  ' 3   (taille de la 1re dimension)
  Console.WriteLine(grille.Rank)          ' 2   (nombre de dimensions)

  Dim matrice = New Integer(,) {{1, 2, 3}, {4, 5, 6}}   ' 2 × 3
  ```

- **Dentelés** (*jagged*, `()()`) : un tableau de tableaux, dont chaque ligne peut avoir une longueur
  différente.

  ```vb
  Dim lignes()() As Integer = New Integer(1)() {}   ' 2 lignes
  lignes(0) = New Integer() {1, 2, 3}
  lignes(1) = New Integer() {9, 8}
  Console.WriteLine(lignes(0)(1))   ' 2  (accès en deux temps)
  ```

Utilisez le rectangulaire pour une matrice fixe, le dentelé pour des données de longueurs variables.

### Méthodes utilitaires

La classe `Array` fournit `Sort`, `Reverse`, `IndexOf`, `Copy`, `Resize`, `Find`, `ForEach`… Et comme
tout tableau implémente `IEnumerable`, **LINQ** s'applique directement ([section 2.9](09-linq.md)).

---

## Collections génériques

C'est l'outil de référence pour la plupart des besoins : taille dynamique et API riche. Espace
`System.Collections.Generic`.

> **Syntaxe générique.** VB écrit **`(Of T)`** là où C# écrit `<T>` : `List(Of String)` correspond à
> `List<string>`. C'est la différence de syntaxe la plus visible (les génériques sont approfondis en
> [section 2.10](10-generiques-avances.md)).

### `List(Of T)`

Un tableau dynamique, redimensionné automatiquement :

```vb
Dim noms As New List(Of String) From {"Ada", "Alan"}
noms.Add("Grace")
noms.Insert(0, "Katherine")
noms.RemoveAll(Function(n) n.StartsWith("A"))   ' supprime selon un prédicat
Console.WriteLine(noms.Count)
Console.WriteLine(noms(0))                       ' accès par indice
```

Méthodes courantes : `Add`, `AddRange`, `Insert`, `Remove`, `RemoveAt`, `RemoveAll`, `Clear`, `Contains`,
`IndexOf`, `Find`, `Sort`, propriété `Count`, indexeur `liste(i)`. (Rappel de [section 2.5](05-boucles.md) :
ne pas modifier une collection pendant un `For Each`.)

### `Dictionary(Of TKey, TValue)`

Des paires clé/valeur avec recherche rapide par clé (table de hachage) :

```vb
Dim ages As New Dictionary(Of String, Integer) From {{"Ada", 36}, {"Alan", 41}}
ages("Grace") = 45            ' l'indexeur ajoute OU met à jour

' Lecture sûre : TryGetValue évite l'exception sur clé absente
Dim age As Integer
If ages.TryGetValue("Ada", age) Then Console.WriteLine(age)

For Each paire In ages        ' paire : KeyValuePair(Of String, Integer)
    Console.WriteLine($"{paire.Key} : {paire.Value}")
Next
```

> ⚠️ **Pièges du dictionnaire.** L'indexeur en **lecture** sur une clé absente lève
> `KeyNotFoundException` ; `Add` sur une clé **déjà présente** lève une exception. Utilisez
> `TryGetValue` / `ContainsKey` pour lire, et l'**affectation par indexeur** (`dico(clé) = valeur`) pour
> insérer-ou-mettre-à-jour sans risque.

### Autres collections génériques

`HashSet(Of T)` (éléments uniques, opérations ensemblistes), `Queue(Of T)` (file FIFO — premier
entré, premier sorti : `Enqueue`/`Dequeue`), `Stack(Of T)` (pile LIFO — dernier entré, premier
sorti : `Push`/`Pop`), ainsi que les variantes triées (`SortedDictionary`, `SortedSet`…).

### Programmer sur les interfaces

Bonne pratique : exposer et manipuler les collections via leurs **interfaces** plutôt que leurs types
concrets — `IEnumerable(Of T)`, `ICollection(Of T)`, `IList(Of T)`, `IReadOnlyList(Of T)`,
`IDictionary(Of TKey, TValue)`. Une méthode qui renvoie `IReadOnlyList(Of T)` (plutôt que `List(Of T)`)
n'expose pas la mutabilité interne et reste libre de changer d'implémentation.

---

## Collections spécialisées

### `ObservableCollection(Of T)` — pour la liaison de données

`System.Collections.ObjectModel.ObservableCollection(Of T)` notifie l'interface utilisateur **à chaque
ajout, suppression ou remplacement** (elle implémente `INotifyCollectionChanged`). C'est **la**
collection des scénarios de **liaison de données** : l'UI se met à jour automatiquement.

```vb
Dim taches As New ObservableCollection(Of String)
taches.Add("Acheter du café")   ' l'interface liée se rafraîchit toute seule
```

Elle est centrale en WPF ([module 6.4](../06-wpf/04-data-binding.md)) et utile en Windows Forms
([module 5.8](../05-windows-forms/08-data-binding.md)). Note : elle signale les changements **de la
collection** ; pour réagir aux changements **d'une propriété d'un élément**, l'élément doit implémenter
`INotifyPropertyChanged` (voir module 6.4).

### Collections concurrentes — pour l'accès multithread

Une `List` ou un `Dictionary` ordinaire n'est **pas thread-safe** : y accéder depuis plusieurs threads
sans synchronisation corrompt l'état ou lève des exceptions. L'espace `System.Collections.Concurrent`
fournit des collections **sûres pour la concurrence** : `ConcurrentDictionary(Of TKey, TValue)`,
`ConcurrentQueue(Of T)`, `ConcurrentStack(Of T)`, `ConcurrentBag(Of T)`, `BlockingCollection(Of T)`.

```vb
Dim compteurs As New ConcurrentDictionary(Of String, Integer)
compteurs.AddOrUpdate("clics", 1, Function(cle, ancien) ancien + 1)   ' opération atomique
```

`ConcurrentDictionary` propose des opérations atomiques (`GetOrAdd`, `AddOrUpdate`, `TryAdd`,
`TryRemove`). À privilégier dès que plusieurs threads lisent et écrivent — sujet relié à la
synchronisation du [module 4.7](../04-async/07-synchronisation.md).

### Collections immuables et lectures seules (notions)

`System.Collections.Immutable` (`ImmutableList(Of T)`, `ImmutableArray(Of T)`,
`ImmutableDictionary`…) offre des collections **immuables**, intrinsèquement thread-safe et adaptées à un
style fonctionnel (en lien avec l'immuabilité, [section 3.7](../03-poo/07-immuabilite-records.md)). Pour
exposer une vue **en lecture seule** d'une liste existante, `List.AsReadOnly()` renvoie une
`ReadOnlyCollection(Of T)`.

---

## Choisir la bonne collection

| Besoin | Structure recommandée |
|--------|-----------------------|
| Taille fixe, connue à la création | Tableau |
| Taille dynamique, usage général | `List(Of T)` |
| Recherche rapide par clé | `Dictionary(Of TKey, TValue)` |
| Éléments uniques / ensembles | `HashSet(Of T)` |
| File d'attente / pile | `Queue(Of T)` / `Stack(Of T)` |
| Liaison de données (UI auto-rafraîchie) | `ObservableCollection(Of T)` |
| Accès depuis plusieurs threads | `Concurrent…` |

Les considérations d'allocation et de performance (capacité initiale, `Span(Of T)`, pooling) relèvent du
[module 14](../14-performance/README.md).

---

## Et l'IA dans tout ça ? 🤖

Deux divergences majeures avec C#, à corriger systématiquement dans le code généré :

- **Syntaxe générique** : `List<string>`, `Dictionary<string, int>` (C#) → **`List(Of String)`**,
  **`Dictionary(Of String, Integer)`** en VB.
- **Déclaration de tableau** : `new int[5]` (C#, **5 éléments**) ne se traduit **pas** par `Dim a(5)`
  (qui en ferait **6**) mais par **`Dim a(4) As Integer`** ou `New Integer(4) {}`. Un bug d'un élément de
  trop, très fréquent.
- Rappels : `a[i]` (crochets) → `a(i)` (parenthèses) ; `var` → `Dim`. Et `ReDim Preserve` est propre à
  VB (C# utilise `Array.Resize` ou une `List`).

La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- **Tableaux** : `Dim a(n)` crée `n + 1` éléments (n = **borne supérieure**) ⚠️ ; taille fixe ;
  `ReDim Preserve` pour redimensionner (coûteux). Rectangulaires `(,)` vs dentelés `()()`.
- **`List(Of T)`** pour la taille dynamique ; **`Dictionary(Of TKey, TValue)`** pour la recherche par
  clé (préférez `TryGetValue` et l'affectation par indexeur).
- **`ObservableCollection(Of T)`** pour la liaison de données ; **collections concurrentes** pour le
  multithread.
- Manipulez les collections via leurs **interfaces** (`IEnumerable(Of T)`, `IReadOnlyList(Of T)`…).

---

⏭️ [LINQ — un point fort de VB.NET](/02-fondamentaux-langage/09-linq.md)
