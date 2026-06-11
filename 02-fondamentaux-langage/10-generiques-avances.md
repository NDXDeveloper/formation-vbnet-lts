🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.10 Génériques avancés

> Les génériques ont déjà servi pour les collections ([section 2.8](08-tableaux-collections.md)), avec la
> syntaxe **`(Of T)`** (là où C# écrit `<T>`). Cette section va plus loin : définir ses propres types
> génériques, **contraindre** les paramètres de type pour exploiter leurs capacités, écrire des
> **méthodes génériques**, et comprendre la **variance** (co/contravariance).

---

## Définir ses propres types génériques

Une classe (ou structure, interface, délégué) peut être paramétrée par un ou plusieurs types :

```vb
Public Class Pile(Of T)
    Private elements As New List(Of T)

    Public Sub Empiler(item As T)
        elements.Add(item)
    End Sub

    Public Function Depiler() As T
        Dim dernier = elements(elements.Count - 1)
        elements.RemoveAt(elements.Count - 1)
        Return dernier
    End Function
End Class

Dim p As New Pile(Of String)   ' instanciation avec un type concret
```

Plusieurs paramètres de type se séparent par des virgules : `Public Class Paire(Of TCle, TValeur)`.

---

## Contraintes (clause `As`)

Sans contrainte, on ne peut presque rien faire de `T` (à part le stocker), car le compilateur ignore ses
capacités. Une **contrainte** restreint les types acceptés **et** débloque les opérations
correspondantes. Elle s'exprime avec `As` :

| Contrainte | Signification |
|------------|---------------|
| `As New` | `T` doit avoir un constructeur public sans paramètre (autorise `New T()`) |
| `As Class` | `T` doit être un **type référence** (autorise la comparaison à `Nothing`) |
| `As Structure` | `T` doit être un **type valeur** |
| `As UneClasseBase` | `T` doit **dériver** de cette classe (ou en être une) |
| `As UneInterface` | `T` doit **implémenter** cette interface |
| `As TAutre` | `T` doit dériver d'un autre paramètre de type |

Pour combiner plusieurs contraintes, on les regroupe entre **accolades** (le `New` est placé en dernier
par convention) :

```vb
Public Class Depot(Of T As {IEntite, New})
    Public Function Creer() As T
        Return New T()          ' possible grâce à 'As New'
    End Function
End Class

Public Class Trieur(Of T As IComparable(Of T))
    Public Function EstAvant(a As T, b As T) As Boolean
        Return a.CompareTo(b) < 0   ' possible grâce à 'As IComparable(Of T)'
    End Function
End Class
```

---

## Méthodes génériques

Une **méthode** peut introduire son propre paramètre de type, indépendamment du type qui la contient :

```vb
Public Sub Echanger(Of T)(ByRef a As T, ByRef b As T)
    Dim temp = a
    a = b
    b = temp
End Sub

Public Function PlusGrand(Of T As IComparable(Of T))(a As T, b As T) As T
    Return If(a.CompareTo(b) >= 0, a, b)
End Function
```

Comme en C#, **l'inférence de type** dispense le plus souvent de préciser l'argument de type : le
compilateur le déduit des arguments.

```vb
Dim m = PlusGrand(3, 7)          ' T inféré : Integer → 7
Dim s = PlusGrand("abc", "xyz")  ' T inféré : String → "xyz"
Echanger(m, s)                   ' ERREUR : m et s n'ont pas le même T
```

Les méthodes d'extension de LINQ ([section 2.9](09-linq.md)) sont précisément des méthodes génériques.

---

## Variance : covariance et contravariance

La variance détermine si l'on peut **assigner** `IFoo(Of Derive)` à une variable `IFoo(Of Base)`. Elle ne
concerne que les **interfaces et délégués génériques** (jamais les classes génériques), et se déclare
avec les modificateurs **`Out`** (covariance) et **`In`** (contravariance).

### Covariance (`Out T`)

Un paramètre **covariant** ne peut apparaître qu'en **sortie** (valeurs de retour). Il autorise un type
**plus dérivé** là où un type plus général est attendu. `IEnumerable(Of T)` est déclaré
`IEnumerable(Of Out T)` :

```vb
Dim chaines As IEnumerable(Of String) = {"a", "b", "c"}
Dim objets As IEnumerable(Of Object) = chaines   ' OK : IEnumerable est covariant
```

### Contravariance (`In T`)

Un paramètre **contravariant** ne peut apparaître qu'en **entrée** (paramètres). Il autorise un type
**moins dérivé**. `Action(Of T)` est déclaré `Action(Of In T)` :

```vb
Dim afficher As Action(Of Object) = Sub(o) Console.WriteLine(o)
Dim afficherChaine As Action(Of String) = afficher   ' OK : Action est contravariant
afficherChaine("Bonjour")
```

Un consommateur d'`Object` (qui accepte tout) peut tenir le rôle d'un consommateur de `String`. C'est
aussi le cas d'`IComparer(Of In T)` et `IEqualityComparer(Of In T)`.

### Invariance (par défaut)

Sans modificateur, un paramètre est **invariant** : aucune conversion. C'est le cas de `IList(Of T)`
(et `List(Of T)`), qui **lit et écrit** `T` :

```vb
' Dim o As IList(Of Object) = New List(Of String)()   ' NE COMPILE PAS (invariant)
```

### Déclarer ses propres interfaces variantes

```vb
Public Interface IProducteur(Of Out T)     ' covariant : T uniquement en sortie
    Function Produire() As T
End Interface

Public Interface IConsommateur(Of In T)    ' contravariant : T uniquement en entrée
    Sub Consommer(item As T)
End Interface
```

En pratique, on **consomme** la variance intégrée (la covariance d'`IEnumerable` est omniprésente,
notamment avec LINQ) bien plus souvent qu'on ne la **déclare** soi-même.

---

## La valeur par défaut d'un type générique

Comment obtenir la valeur par défaut d'un `T` inconnu ? En VB, il suffit d'assigner **`Nothing`** : grâce
à son sens contextuel ([section 2.2](02-types-variables.md)), `Nothing` désigne la valeur par défaut du
type — `0` pour un type valeur, une référence nulle pour un type référence.

```vb
Public Function ValeurParDefaut(Of T)() As T
    Dim valeur As T = Nothing   ' valeur par défaut de T, quel qu'il soit
    Return valeur
End Function
```

VB n'a donc **pas besoin** de l'opérateur `default(T)` de C# : `Nothing` joue ce rôle directement.

---

## Délégués génériques (notions)

Les délégués génériques de la bibliothèque sont omniprésents : `Func(Of …, TResult)`, `Action(Of …)`,
`Predicate(Of T)`, `EventHandler(Of T)`. On les manipule avec des lambdas (`Function(…)`, `Sub(…)`) ; ils
sont au cœur des événements et des délégués, traités en [section 3.6](../03-poo/06-evenements-delegues.md).

---

## Et l'IA dans tout ça ? 🤖

Les génériques avancés cumulent plusieurs divergences de syntaxe avec C#, à corriger dans le code
généré :

- **Paramètres de type** : `<T>` → **`(Of T)`**.
- **Contraintes** : `where T : class, IComparable<T>, new()` (C#) → **`(Of T As {Class, IComparable(Of T), New})`**.
- **Variance** : `out T` / `in T` → **`Out T`** / **`In T`**.
- **Valeur par défaut** : `default(T)` / `default` → **`Nothing`** (pas d'opérateur `default` en VB).
- **Lambdas** : `=>` → **`Function`** / **`Sub`**.

La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- On déclare des types et méthodes génériques avec **`(Of T)`** ; les **méthodes génériques** bénéficient
  de l'**inférence de type**.
- Les **contraintes** (`As New`, `As Class`, `As Structure`, classe de base, interface) restreignent `T`
  **et** débloquent les opérations correspondantes ; on les combine entre **accolades**.
- La **variance** (sur interfaces/délégués) : **`Out T`** (covariance, positions de sortie), **`In T`**
  (contravariance, positions d'entrée), invariance par défaut. On la **consomme** plus qu'on ne la
  déclare.
- La valeur par défaut d'un `T` s'obtient avec **`Nothing`** (pas de `default(T)`).

---

⏭️ [Portée, visibilité et modificateurs d'accès (Public/Private/Protected/Friend)](/02-fondamentaux-langage/11-portee-visibilite.md)
