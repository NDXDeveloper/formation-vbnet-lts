🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.2 Structures (`Structure`) et tuples

La classe (§ 3.1) est un type *référence*. VB.NET propose son pendant en type *valeur* : la **`Structure`**. À cela s'ajoutent les **tuples**, un mécanisme léger pour regrouper plusieurs valeurs sans définir de type nommé. Cette section présente les deux, leurs règles propres à VB.NET, et fournit une grille pour choisir entre structure, tuple, classe et *record*.

---

## Structures (`Structure`)

Une structure regroupe des données — et éventuellement du comportement — dans un **type valeur**. Concrètement : une variable de structure *contient* directement les données, là où une variable de classe ne contient qu'une référence. C'est l'outil adapté aux petites entités qui se comportent comme une valeur unique : un point, une couleur, un montant monétaire, une plage de dates.

### Déclarer une structure

Une structure se déclare avec le bloc `Structure … End Structure` :

```vb
Public Structure Point2D
    Public X As Integer
    Public Y As Integer
End Structure
```

Comme une classe, une structure peut contenir des champs, des propriétés, des méthodes et des constructeurs. La différence fondamentale est sémantique.

### Sémantique de valeur

**Affecter une structure copie ses données**, et non une référence. Les deux variables sont alors totalement indépendantes :

```vb
Dim a As Point2D
a.X = 1
a.Y = 2

Dim b = a        ' COPIE intégrale des données
b.X = 99

Console.WriteLine(a.X)   ' 1 — « a » n'a pas bougé
Console.WriteLine(b.X)   ' 99
```

À comparer avec le comportement d'une classe (§ 3.1), où `b = a` ferait pointer les deux variables vers le **même** objet. La même règle s'applique au passage en argument : par défaut (`ByVal`), une structure est **copiée** lors de l'appel.

### Constructeurs : deux règles propres à VB.NET

Une structure peut définir des constructeurs paramétrés, mais VB.NET impose deux contraintes spécifiques.

**1. Pas de constructeur sans paramètre déclaré.** Vous ne pouvez pas écrire un `Sub New()` sans paramètre dans une structure ; le constructeur sans paramètre est toujours fourni implicitement et initialise chaque membre à sa valeur par défaut.

```vb
Public Structure Point2D
    Public ReadOnly X As Integer
    Public ReadOnly Y As Integer

    Public Sub New(x As Integer, y As Integer)   ' ✓ paramétré
        Me.X = x
        Me.Y = y
    End Sub

    ' Public Sub New()   ' ❌ interdit dans une Structure
End Structure
```

**2. Pas d'initialiseur sur les membres d'instance.** Contrairement à une classe, vous ne pouvez pas initialiser un champ ou une propriété auto *d'instance* à sa déclaration. Seuls les membres `Shared` et les constantes l'autorisent :

```vb
Public Structure Exemple
    ' Public X As Integer = 0          ' ❌ « initialiseur valide uniquement sur Shared/Const »
    Public Const Zero As Integer = 0   ' ✓ constante
    Public Shared ReadOnly Defaut As New Exemple   ' ✓ membre partagé
End Structure
```

### Valeur par défaut et `Nothing`

Une structure est **toujours utilisable**, même sans avoir été explicitement construite : ses champs valent leur valeur par défaut. Une structure ne peut jamais être « nulle ». Affecter `Nothing` à une variable de structure ne la rend pas nulle : cela la réinitialise à sa **valeur par défaut** (tous les champs à zéro) :

```vb
Dim p As Point2D            ' immédiatement valide : X = 0, Y = 0
Dim q As Point2D = Nothing  ' équivaut à la valeur par défaut, pas à « null »
```

Pour représenter l'**absence** d'une valeur de structure, on l'enveloppe dans un type nullable de valeur : `Dim p As Point2D?` (soit `Nullable(Of Point2D)`), qui peut, lui, valoir `Nothing` (§ 2.2).

### Égalité structurelle (par défaut)

Là où une classe compare par référence, une structure offre par défaut une **égalité structurelle** : deux instances sont égales si tous leurs champs le sont. C'est `ValueType.Equals` qui s'en charge — corrects mais **lent**, car il procède par réflexion. Pour tout code sur un chemin critique, il est recommandé de **redéfinir `Equals` et `GetHashCode`**, et idéalement d'implémenter `IEquatable(Of T)` pour éviter la réflexion.

### Limites : pas d'héritage, et pas de `ReadOnly`/`Ref Structure`

Une structure **ne peut pas hériter** d'une autre structure ni d'une classe (elle dérive implicitement de `System.ValueType` et est scellée). Elle peut en revanche **implémenter des interfaces** :

```vb
Public Structure Temperature
    Implements IComparable(Of Temperature)

    Public ReadOnly Property Celsius As Double

    Public Sub New(celsius As Double)
        Me.Celsius = celsius
    End Sub

    Public Function CompareTo(other As Temperature) As Integer _
            Implements IComparable(Of Temperature).CompareTo
        Return Celsius.CompareTo(other.Celsius)
    End Function
End Structure
```

> ⚠️ 🔗 **Fonctionnalités C# non déclarables en VB.NET.** VB ne permet pas de déclarer de `ReadOnly Structure` (le `readonly struct` de C#) ni de `Ref Structure` (le `ref struct` de C#, ni le `record struct`). Pour **approcher** une structure immuable, déclarez tous les champs `ReadOnly` et n'exposez aucune méthode mutante — c'est l'approche retenue dans l'exemple `Temperature` ci-dessus. Quant aux *ref structs* tels que `Span(Of T)`, VB peut les **consommer** dans des scénarios restreints, sans pouvoir les définir (→ § 14.4 et **[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**).

### Le piège du *boxing*

Dès qu'une structure est traitée comme un `Object` ou via un type d'**interface**, elle est *boxée* : copiée sur le tas (heap), ce qui annule son avantage de performance. Un *boxing* répété dans une boucle chaude est un anti-pattern à surveiller.

```vb
Dim t As New Temperature(20)
Dim o As Object = t          ' boxing : copie sur le tas
```

### Quand utiliser une structure

Les recommandations de Microsoft tiennent en quatre critères, à réunir **tous** :

- le type représente logiquement **une seule valeur** (un nombre, un point, une couleur) ;
- il est **petit** (en pratique ≤ 16 octets) ;
- il est **immuable** ;
- il ne sera **pas boxé fréquemment**.

Dès qu'un de ces critères n'est pas rempli, préférez une **classe**. Les structures rendent par ailleurs de grands services en **interopérabilité** (marshaling de structures natives en P/Invoke, § 9.1), où leur disposition mémoire prévisible est un atout.

---

## Tuples

Un tuple regroupe plusieurs valeurs en une seule, **sans définir de type nommé**. C'est l'outil idéal pour des regroupements **locaux et transitoires** — au premier rang desquels le retour de plusieurs valeurs depuis une fonction.

### Le tuple de valeur (`ValueTuple`)

Depuis VB 15 (Visual Studio 2017), VB.NET prend en charge les *tuples de valeur* (`System.ValueTuple`). On les crée par simple énumération entre parenthèses :

```vb
Dim point = (10, 20)               ' type : (Integer, Integer)
Console.WriteLine(point.Item1)     ' 10
Console.WriteLine(point.Item2)     ' 20
```

### Nommer les éléments

Des noms rendent le tuple bien plus lisible. On peut les attribuer au littéral ou les déclarer dans le type :

```vb
Dim personne = (Id:=1, Nom:="Alice")
Console.WriteLine(personne.Nom)            ' Alice

Dim coord As (X As Integer, Y As Integer) = (10, 20)
Console.WriteLine(coord.X)                 ' 10
```

### Retourner plusieurs valeurs d'une fonction

C'est l'usage le plus fréquent et le plus utile : éviter un paramètre `ByRef` ou un type ad hoc pour rendre deux résultats liés.

```vb
Public Function DiviserAvecReste(dividende As Integer, diviseur As Integer) _
        As (Quotient As Integer, Reste As Integer)
    Return (dividende \ diviseur, dividende Mod diviseur)
End Function

' Appel :
Dim r = DiviserAvecReste(17, 5)
Console.WriteLine($"{r.Quotient} reste {r.Reste}")   ' 3 reste 2
```

### Accès aux éléments — pas de déconstruction en VB.NET

> ⚠️ **Différence majeure avec C#.** VB.NET ne prend **pas** en charge la *déconstruction* de tuples (le `Dim (q, reste) = …` n'existe pas). On accède toujours aux valeurs **par membre** : `.Item1`, `.Item2`, … ou par leur nom.

```vb
' ❌ Pas en VB.NET :
' Dim (q, reste) = DiviserAvecReste(17, 5)

' ✓ En VB.NET :
Dim resultat = DiviserAvecReste(17, 5)
Dim q = resultat.Quotient
Dim reste = resultat.Reste
```

### L'ancien `Tuple(Of T…)`

Avant les tuples de valeur existait `System.Tuple` — un type **référence**, immuable, dont les éléments s'appellent eux aussi `Item1`, `Item2`… Il subsiste pour la compatibilité mais est aujourd'hui supplanté par `ValueTuple` dans la plupart des cas :

```vb
Dim t = Tuple.Create(1, "Alice")   ' System.Tuple (type référence)
Console.WriteLine(t.Item1)
```

### Quand utiliser un tuple

Un tuple convient à un regroupement **léger, local et de courte durée**, typiquement le retour multiple d'une fonction interne. À l'inverse, **évitez-le dans une API publique** durable : un tuple n'a ni nom de type signifiant, ni comportement, et ses noms d'éléments sont une commodité fragile. Pour une donnée structurée exposée et documentée, préférez une **structure**, une **classe** ou un *record* (consommé depuis C#, § 3.7).

---

## Structure, tuple, classe ou *record* ?

| Critère | `Structure` | Tuple | `Class` | *record* (C#, consommé) |
|---|---|---|---|---|
| Sémantique | Valeur | Valeur | Référence | Référence (par défaut) |
| Égalité par défaut | Structurelle | Structurelle | Référence | Structurelle |
| **Déclarable en VB.NET** | ✓ | ✓ (littéral) | ✓ | ✗ — consommation 🔗 |
| Noms de membres stables | ✓ | fragiles | ✓ | ✓ |
| Peut porter du comportement | ✓ | ✗ | ✓ | ✓ |
| Usage typique | petite valeur immuable | regroupement local transitoire | objet métier | DTO / valeur immuable |

En résumé : **structure** pour une petite valeur immuable, **tuple** pour un regroupement local et jetable, **classe** pour un objet métier à part entière, et **record** (écrit en C#, consommé en VB) lorsqu'on veut une valeur immuable à égalité structurelle avec une syntaxe concise (§ 3.7, module 10).

---

## Spécificités VB.NET à retenir

- `Structure … End Structure` déclare un type valeur ; l'affectation **copie** les données.
- Dans une structure : **aucun constructeur sans paramètre déclaré**, et **aucun initialiseur** sur les membres d'instance.
- Une structure n'est jamais nulle ; `Nothing` la ramène à sa valeur par défaut. Pour l'absence, utilisez `T?` (§ 2.2).
- Pas de `readonly struct`, `ref struct` ni `record struct` déclarables en VB — uniquement consommables.
- Tuples de valeur disponibles, **mais sans déconstruction** : accès par `.Item1` / membres nommés.

> 🤖 **Astuce IA.** Les assistants génèrent volontiers des constructions C# absentes de VB : `readonly struct`, `record struct`, ou la déconstruction `var (x, y) = …`. Demandez explicitement du « **Visual Basic .NET** » et remplacez ces formes par leurs équivalents VB (champs `ReadOnly`, accès `.Item1`/nommé). La correspondance complète figure en **[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** ; le prompting au **[module 17](../17-developpement-ia/README.md)**.

---

Structures et classes diffèrent par leur sémantique, mais partagent l'essentiel du modèle objet. La section suivante introduit le mécanisme qui distingue véritablement les classes — l'**héritage** — et le **polymorphisme** qui l'accompagne.

⏭️ [Héritage et polymorphisme](/03-poo/03-heritage-polymorphisme.md)
