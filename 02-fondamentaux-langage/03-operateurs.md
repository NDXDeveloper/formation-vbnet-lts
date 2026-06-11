🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.3 Opérateurs et expressions

> VB.NET dispose d'opérateurs souvent plus explicites que leurs équivalents C# — `&` pour concaténer,
> `Mod` pour le modulo, `Is`/`IsNot` pour comparer des références, `AndAlso`/`OrElse` pour la logique à
> court-circuit. Plusieurs cachent toutefois des pièges (deux divisions distinctes, `^` qui signifie
> *puissance* et non *XOR*) qu'il vaut mieux connaître avant d'écrire la première expression.

---

## Opérateurs arithmétiques

```vb
Dim quotientReel = 7 / 2     ' 3.5  — division flottante (renvoie toujours Double)
Dim quotientEntier = 7 \ 2   ' 3    — division entière
Dim reste = 7 Mod 2          ' 1    — modulo (reste)
Dim puissance = 2 ^ 10       ' 1024 — exponentiation (renvoie Double)
```

VB propose les opérateurs habituels (`+`, `-`, `*`) mais **deux divisions** et un opérateur de
puissance qui méritent une attention particulière :

- **`/`** est la division **flottante** : elle renvoie toujours un `Double` (ou `Decimal`), même entre
  deux entiers. `7 / 2` vaut `3.5`.
- **`\`** est la division **entière** : elle renvoie le quotient tronqué. `7 \ 2` vaut `3`.
- **`Mod`** est le modulo (reste de la division), valable aussi en virgule flottante.
- **`^`** est l'**exponentiation**. `2 ^ 10` vaut `1024`.

> ⚠️ **Deux pièges classiques par rapport à C#.**
> En C#, `/` entre deux entiers est une division entière (`7 / 2` → `3`). En VB, `/` est toujours
> flottante : la division entière s'écrit `\`. Par ailleurs, en C# l'opérateur `^` désigne le **XOR
> binaire** ; en VB, `^` est la **puissance**, et le XOR s'écrit `Xor`. Ces deux différences sont une
> source fréquente de bugs lors d'une traduction C# → VB.

---

## Concaténation : l'opérateur `&`

L'opérateur **`&`** assemble des chaînes. C'est l'opérateur de concaténation idiomatique de VB :

```vb
Dim nom = "Ada"
Dim message = "Bonjour " & nom & " !"   ' "Bonjour Ada !"
```

Son avantage sur `+` est l'**absence d'ambiguïté** : `&` convertit ses deux opérandes en chaîne, quels
qu'ils soient.

```vb
Dim etiquette = "Total : " & 42   ' "Total : 42"
```

Avec `+`, l'expression `"Total : " + 42` est rejetée sous `Option Strict On` (pas de conversion
implicite) et risquerait une addition numérique sous `Off`. **Réservez `+` à l'arithmétique et `&` à la
concaténation.** Les chaînes et leur performance (`StringBuilder`, interpolation) sont détaillées en
[section 2.6](06-chaines.md).

---

## Opérateurs de comparaison

```vb
Dim x As Integer = 5          ' '=' : ici, AFFECTATION
If x = 5 Then                 ' '=' : ici, COMPARAISON (renvoie un Boolean)
    Console.WriteLine("égal")
End If
Dim different = (x <> 3)      ' True — inégalité (et non '!=')
```

Les opérateurs sont `=`, `<>`, `<`, `>`, `<=`, `>=`. Deux particularités par rapport à C# :

- **`=` joue un double rôle** : affectation *ou* égalité, selon le contexte. Il n'existe pas de `==`.
- **L'inégalité s'écrit `<>`**, pas `!=`.

Pour les **chaînes**, le comportement des comparaisons (sensibilité à la casse, prise en compte de la
culture) dépend de `Option Compare` ([section 2.1](01-structure-options.md)).

---

## Égalité de référence : `Is` / `IsNot`

Là où `=` compare des **valeurs** (et peut être surchargé), **`Is`** teste si deux variables désignent
**le même objet**, et **`IsNot`** en est la négation lisible :

```vb
Dim a As New Object()
Dim b As Object = a
Dim c As New Object()

Console.WriteLine(a Is b)      ' True  — même objet
Console.WriteLine(a Is c)      ' False — objets distincts
Console.WriteLine(a IsNot c)   ' True
```

`Is` / `IsNot` servent aussi au test d'absence idiomatique, y compris pour les types nullables
([section 2.2](02-types-variables.md)) :

```vb
Dim s As String = Nothing
If s Is Nothing Then Console.WriteLine("référence nulle")
```

---

## Correspondance de motifs : `Like`

L'opérateur **`Like`** confronte une chaîne à un **motif** et renvoie un `Boolean` :

```vb
Console.WriteLine("VB.NET" Like "VB*")          ' True
Console.WriteLine("fichier.txt" Like "*.txt")   ' True
Console.WriteLine("A1" Like "[A-Z]#")           ' True  — une lettre puis un chiffre
```

Les caractères génériques utilisables dans le motif :

| Motif | Correspond à |
|-------|--------------|
| `?` | un caractère quelconque |
| `*` | zéro caractère ou plus |
| `#` | un chiffre (0–9) |
| `[liste]` | un caractère de la liste (ex. `[A-Z]`, `[aeiou]`) |
| `[!liste]` | un caractère **hors** de la liste |

Comme pour les comparaisons de chaînes, la sensibilité à la casse de `Like` est déterminée par
`Option Compare` (`Binary` = sensible, `Text` = insensible). Pour des motifs complexes, on préférera les
expressions régulières (`System.Text.RegularExpressions`).

---

## Opérateurs logiques et bit-à-bit

VB distingue les opérateurs **avec** et **sans** court-circuit — une différence essentielle pour la
sûreté du code.

- **`And`, `Or`, `Xor`, `Not`** : opérateurs **sans court-circuit**. Ils évaluent **toujours les deux
  opérandes**. Ils servent à la fois à la logique booléenne et aux opérations **bit-à-bit** sur les
  entiers.
- **`AndAlso`, `OrElse`** : opérateurs logiques **à court-circuit** (les équivalents de `&&` / `||` en
  C#). `AndAlso` n'évalue pas la partie droite si la gauche est `False` ; `OrElse` ne l'évalue pas si la
  gauche est `True`.

La conséquence pratique est importante : pour les **conditions**, employez `AndAlso` / `OrElse`, qui
évitent d'évaluer une expression inutile — voire dangereuse :

```vb
' AndAlso : 'client.Solde' n'est lu que si 'client' n'est pas Nothing → sûr
If client IsNot Nothing AndAlso client.Solde > 0 Then
    ' ...
End If

' Avec 'And', les DEUX côtés seraient évalués
' → NullReferenceException si 'client' vaut Nothing
```

Réservez `And` / `Or` / `Xor` aux opérations **bit-à-bit** sur des entiers, accompagnées au besoin des
décalages `<<` (gauche) et `>>` (droite) :

```vb
Dim masque = &B1100 And &B1010   ' &B1000  (8)
Dim drapeaux = &B0001 Or &B0100  ' &B0101  (5)
Dim decale = 1 << 4              ' 16
```

---

## Opérateurs d'affectation composée

VB propose les affectations composées habituelles — y compris **`&=`** pour la concaténation — mais
**ni `++` ni `--`** :

```vb
Dim total = 0
total += 10        ' 10
total *= 2         ' 20
total \= 3         ' 6   (division entière composée)

Dim texte = "a"
texte &= "b"       ' "ab"

' Pas d'incrément '++' : on écrit explicitement
total += 1
```

---

## Conversions dans les expressions

Lorsqu'une expression mêle des types, on convertit explicitement (rappel de
[section 2.1](01-structure-options.md), indispensable sous `Option Strict On`). Quatre formes courantes :

```vb
Dim o As Object = "123"
Dim n As Integer = CInt("42")        ' fonctions Cxxx (CInt, CStr, CDbl, CBool…)
Dim s1 As String = CType(o, String)  ' conversion générale (peut invoquer une conversion)
Dim s2 As String = DirectCast(o, String) ' transtypage strict, sans conversion (rapide)
Dim s3 As String = TryCast(o, String)     ' renvoie Nothing si le type ne correspond pas
```

`DirectCast` exige une relation directe d'héritage ou d'implémentation entre le type réel de l'objet
et le type cible, sans appliquer la moindre conversion (sinon `InvalidCastException`) ;
`TryCast` (réservé aux types référence) renvoie `Nothing` au lieu de lever une exception — pratique pour
un test de type suivi d'une utilisation.

---

## Priorité des opérateurs

En cas de doute, parenthésez. À titre de référence, voici l'ordre d'évaluation, du **plus prioritaire**
au **moins prioritaire** :

1. `Await` (voir [module 4](../04-async/README.md))
2. `^` (exponentiation)
3. `+`, `-` unaires
4. `*`, `/`
5. `\` (division entière)
6. `Mod`
7. `+`, `-` (binaires)
8. `&` (concaténation)
9. `<<`, `>>` (décalages)
10. comparaisons : `=`, `<>`, `<`, `<=`, `>`, `>=`, `Is`, `IsNot`, `Like`, `TypeOf…Is`
11. `Not`
12. `And`, `AndAlso`
13. `Or`, `OrElse`
14. `Xor`

Notez que `And` / `AndAlso` sont **prioritaires** sur `Or` / `OrElse` : `a Or b AndAlso c` équivaut à
`a Or (b AndAlso c)`. Le parenthésage explicite reste la meilleure protection contre ce genre de
surprise.

---

## Et l'IA dans tout ça ? 🤖

Les opérateurs concentrent quelques-unes des erreurs de traduction C# → VB les plus tenaces. À vérifier
systématiquement dans le code généré :

- **`==` / `!=`** (C#) → **`=` / `<>`** en VB ; **`&&` / `||`** → **`AndAlso` / `OrElse`**.
- **`/` entre entiers** : en C# elle tronque, en VB elle est flottante. La division entière voulue
  s'écrit **`\`** — une confusion silencieuse et fréquente.
- **`^`** : XOR en C#, **puissance** en VB. Un `a ^ b` laissé tel quel introduit un bug sémantique ; le
  XOR s'écrit **`Xor`**.
- **`++` / `--`** : inexistants en VB → **`x += 1`**.

La méthode générale (préciser « **Visual Basic .NET** » et la version cible) est détaillée au
[module 17](../17-developpement-ia/README.md).

## En résumé

- VB a **deux divisions** : `/` (flottante, toujours `Double`) et `\` (entière) ; `^` est la
  **puissance** (le XOR est `Xor`).
- **`&`** concatène sans ambiguïté ; réservez `+` à l'arithmétique.
- **`=`** sert à la fois d'affectation et d'égalité ; l'inégalité est **`<>`**.
- **`Is` / `IsNot`** comparent des **références** (et testent `Nothing`) ; `=` compare des valeurs.
- **`Like`** filtre selon un motif (sensibilité régie par `Option Compare`).
- Utilisez **`AndAlso` / `OrElse`** (court-circuit) pour les conditions, et `And` / `Or` / `Xor` pour le
  bit-à-bit.

---

⏭️ [Structures conditionnelles](/02-fondamentaux-langage/04-conditions.md)
