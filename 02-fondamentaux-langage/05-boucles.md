🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.5 Boucles et itérations

> VB.NET propose quatre constructions d'itération : `For…Next` (compteur), `For Each` (parcours d'une
> séquence), `Do…Loop` (la plus flexible) et `While…End While`. Quelques différences avec C# méritent
> l'attention — borne supérieure **inclusive**, mots-clés de sortie **typés** (`Exit For`…), et
> `End While` à la place de l'ancien `Wend`.

---

## `For…Next`

La boucle à compteur parcourt une plage de valeurs, **borne supérieure incluse** :

```vb
For i As Integer = 1 To 5
    Console.WriteLine(i)   ' 1, 2, 3, 4, 5
Next
```

Le pas d'incrémentation se règle avec **`Step`** (positif ou négatif) :

```vb
For i As Integer = 10 To 0 Step -2
    Console.WriteLine(i)   ' 10, 8, 6, 4, 2, 0
Next
```

Points à connaître :

- Le **compteur se déclare en ligne** (`For i As Integer = …`), ce qui le limite à la portée de la
  boucle — c'est l'usage recommandé. Le mot-clé `Next` peut éventuellement nommer la variable (`Next i`),
  mais c'est facultatif.
- Les **bornes (`To` et `Step`) sont évaluées une seule fois**, au démarrage. Modifier la variable qui
  sert de limite à l'intérieur de la boucle n'a aucun effet sur le nombre d'itérations.
- Le compteur peut être de n'importe quel type numérique ; avec un `Step` en `Double`/`Decimal`,
  méfiez-vous de l'accumulation d'erreurs en virgule flottante.

> ⚠️ **Borne inclusive (différence avec C#).** En C, `for (i = 0; i < n; i++)` s'arrête **avant** `n`.
> En VB, `For i = 0 To n` itère **jusqu'à `n` inclus**, soit `n + 1` fois. Pour `n` itérations en base 0,
> écrivez `For i = 0 To n - 1` (ou `For i = 1 To n`).

`Exit For` quitte la boucle, `Continue For` passe à l'itération suivante :

```vb
For i As Integer = 1 To 100
    If i Mod 3 <> 0 Then Continue For   ' ignore les non-multiples de 3
    If i > 12 Then Exit For             ' arrête au-delà de 12
    Console.Write(i & " ")              ' 3 6 9 12
Next
```

---

## `For Each…Next`

`For Each` parcourt toute séquence qui implémente `IEnumerable` / `IEnumerable(Of T)` : collections,
tableaux, chaînes (caractère par caractère), résultats LINQ, etc.

```vb
Dim noms = New List(Of String) From {"Ada", "Alan", "Grace"}
For Each nom As String In noms
    Console.WriteLine(nom)
Next
```

L'élément se déclare en ligne avec `As Type` (recommandé). `Exit For` et `Continue For` s'y appliquent
également.

> ⚠️ **Ne modifiez pas la collection pendant le parcours.** Ajouter ou retirer un élément d'une
> collection en cours de `For Each` lève une `InvalidOperationException`. Pour **supprimer** des
> éléments, parcourez à l'envers avec un index, ou utilisez `RemoveAll` / une projection LINQ :

```vb
' Suppression sûre : parcours à rebours avec index
For i As Integer = nombres.Count - 1 To 0 Step -1
    If nombres(i) < 0 Then nombres.RemoveAt(i)
Next

' Ou, plus déclaratif :
nombres.RemoveAll(Function(n) n < 0)
```

---

## `Do…Loop`

C'est la boucle la plus flexible : la condition peut être **en tête** (testée avant chaque itération,
donc potentiellement zéro exécution) ou **en queue** (testée après, donc au moins une exécution), et
s'exprimer avec `While` (tant que vrai) ou `Until` (jusqu'à ce que vrai).

```vb
' Condition en tête
Do While file.Count > 0
    Traiter(file.Dequeue())
Loop

Do Until file.Count = 0      ' équivalent, formulé à l'envers
    Traiter(file.Dequeue())
Loop

' Condition en queue (s'exécute au moins une fois)
Do
    ligne = Console.ReadLine()
    Traiter(ligne)
Loop Until ligne Is Nothing
```

`Exit Do` quitte la boucle, `Continue Do` passe à l'itération suivante. La variante `Until` est souvent
plus lisible lorsqu'on attend qu'un événement se produise.

---

## `While…End While`

Boucle à condition en tête, équivalente à `Do While … Loop` :

```vb
Dim total = 0
While total < 100
    total += LireMontant()
End While
```

> ⚠️ La forme est `While … End While`, **pas `While … Wend`** : `Wend` appartenait à VB6 et n'existe plus
> en VB.NET.

`Exit While` et `Continue While` complètent la construction.

---

## Sortir et continuer : des mots-clés **typés**

Contrairement à C# qui utilise les mots-clés génériques `break` et `continue`, VB nomme **explicitement
le type de boucle** :

| Action | `For` / `For Each` | `Do` | `While` |
|--------|--------------------|------|---------|
| Quitter la boucle | `Exit For` | `Exit Do` | `Exit While` |
| Itération suivante | `Continue For` | `Continue Do` | `Continue While` |

Pour quitter la méthode englobante, on utilise `Return` (ou `Exit Sub` / `Exit Function`).

---

## Quelle boucle choisir ?

- **`For Each`** quand on parcourt une séquence sans avoir besoin de l'indice : c'est la forme la plus
  lisible et la plus sûre.
- **`For…Next`** quand on a besoin de la **position** (indice) ou que l'on agit par index (suppression à
  rebours, par exemple).
- **`Do…Loop`** quand la condition d'arrêt n'est pas un simple comptage, ou qu'il faut garantir au moins
  une exécution.

Enfin, dès qu'une boucle **filtre, transforme ou agrège** une séquence, l'écriture **LINQ** est souvent
plus claire et plus expressive que la boucle impérative équivalente — c'est l'un des points forts de
VB.NET, détaillé en [section 2.9](09-linq.md). Réservez les boucles au **flux de contrôle impératif**, et
LINQ aux **requêtes** sur les données. Les considérations de performance des boucles (notamment sur les
tableaux) relèvent du [module 14](../14-performance/README.md).

---

## Et l'IA dans tout ça ? 🤖

Les boucles concentrent quelques divergences mécaniques avec C#, à corriger dans le code généré :

- **`break` / `continue`** (C#) → **`Exit For` / `Continue For`** (et variantes `Do` / `While`) ; pas de
  mot-clé générique en VB.
- **Borne de `For…Next` inclusive** : un `for (i = 0; i < n; i++)` traduit naïvement en `For i = 0 To n`
  introduit une itération de trop. La forme correcte est `For i = 0 To n - 1`.
- **`foreach`** → **`For Each … In …`** ; **`do { … } while(c)`** → **`Do … Loop While c`**.
- **`Wend`** : forme VB6 obsolète → **`End While`**.

La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- **`For…Next`** : compteur, `Step`, **borne supérieure incluse** (attention à l'écart d'un par rapport
  à C#) ; bornes évaluées une seule fois.
- **`For Each…Next`** : parcourt tout `IEnumerable` ; **ne pas modifier la collection** pendant le
  parcours (supprimer à rebours ou via `RemoveAll`).
- **`Do…Loop`** : la plus flexible (condition en tête ou en queue, `While` ou `Until`).
- **`While…End While`** : condition en tête (et non `Wend`).
- Sortie/continuation via des mots-clés **typés** (`Exit For`, `Continue Do`…). Pour les opérations de
  requête, préférez **LINQ**.

---

⏭️ [Chaînes et manipulation de texte (String, StringBuilder, interpolation $"…")](/02-fondamentaux-langage/06-chaines.md)
