🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.2 Types de données et variables

> .NET repose sur un système de types unifié. Comprendre la frontière entre **types valeur** et
> **types référence**, connaître les primitifs, et maîtriser les **types nullables** — sans les
> confondre avec une fonctionnalité voisine mais distincte de C# — constitue le socle de tout le reste.

---

## Types valeur vs types référence

C'est la distinction fondamentale du système de types .NET, et elle gouverne le comportement de
**l'affectation** et du **passage en argument**.

- Les **types valeur** contiennent directement leur donnée. À l'affectation, la valeur est **copiée** :
  les deux variables sont indépendantes. Ils dérivent de `System.ValueType`. Exemples : `Integer`,
  `Double`, `Boolean`, `Date`, les `Structure` ([section 3.2](../03-poo/02-structures-tuples.md)) et les
  `Enum`.
- Les **types référence** contiennent une **référence** vers un objet alloué sur le tas. À l'affectation,
  c'est la référence qui est copiée : les deux variables désignent **le même objet**. Exemples : les
  `Class`, `String`, les tableaux, les délégués, les collections.

```vb
' Type valeur : copie indépendante
Dim a As Integer = 10
Dim b As Integer = a
b = 99
Console.WriteLine(a)   ' 10 — a est inchangé

' Type référence : copie de la référence (même objet)
Dim liste1 As New List(Of Integer) From {1, 2, 3}
Dim liste2 = liste1
liste2.Add(4)
Console.WriteLine(liste1.Count)   ' 4 — liste1 et liste2 pointent le même objet
```

### `Nothing` : un mot-clé au sens contextuel

En VB.NET, **`Nothing`** ne signifie pas exactement « null ». Il représente la **valeur par défaut du
type** considéré :

- pour un type **référence**, c'est une référence nulle ;
- pour un type **valeur**, c'est la valeur « zéro » du type (`0`, `False`, `Date` minimale…).

```vb
Dim n As Integer = Nothing   ' n vaut 0 (valeur par défaut), pas "null"
Dim s As String = Nothing    ' s est une référence nulle
Console.WriteLine(n)         ' 0
Console.WriteLine(s Is Nothing)   ' True
```

À noter : l'opérateur `Is` / `IsNot` ne s'utilise qu'avec des types référence (et des nullables, voir
plus bas) ; tester `unEntier Is Nothing` est une erreur de compilation.

---

## Les types primitifs

VB.NET expose des mots-clés qui sont des **alias** de types de la bibliothèque .NET. Les deux écritures
sont strictement équivalentes (`Integer` ≡ `System.Int32`).

| Mot-clé VB | Type .NET | Description |
|------------|-----------|-------------|
| `Boolean` | `System.Boolean` | `True` / `False` |
| `Byte` / `SByte` | `Byte` / `SByte` | Entier 8 bits non signé / signé |
| `Short` / `UShort` | `Int16` / `UInt16` | Entier 16 bits |
| `Integer` / `UInteger` | `Int32` / `UInt32` | Entier 32 bits (le plus courant) |
| `Long` / `ULong` | `Int64` / `UInt64` | Entier 64 bits |
| `Single` | `Single` | Flottant 32 bits |
| `Double` | `Double` | Flottant 64 bits (flottant par défaut) |
| `Decimal` | `Decimal` | Décimal 128 bits, **sans erreur binaire** |
| `Char` | `Char` | Un caractère Unicode 16 bits |
| `String` | `String` | Chaîne de caractères (type **référence**) |
| `Date` | `DateTime` | Date et heure |
| `Object` | `Object` | Type de base universel |

Deux points méritent l'attention :

- **`Date` désigne `System.DateTime`** : c'est une particularité de VB. On manipule les dates avec le
  mot-clé `Date` ([section 2.7](07-dates-nombres-culture.md)).
- **`Decimal` pour les montants** : contrairement à `Double`/`Single` (virgule flottante binaire),
  `Decimal` représente exactement les valeurs décimales. C'est le type des calculs financiers.

`String` est techniquement un type *référence*, mais immuable et doté d'une sémantique proche d'une
valeur ; ses spécificités sont détaillées en [section 2.6](06-chaines.md).

### Littéraux

```vb
Dim grand As Long = 1_000_000L     ' suffixe L, séparateur de chiffres '_'
Dim hexa As Integer = &HFF         ' hexadécimal → 255
Dim binaire As Integer = &B1010    ' binaire → 10
Dim taux As Double = 3.14          ' Double par défaut
Dim ratio As Single = 1.5F         ' suffixe F → Single
Dim prix As Decimal = 19.99D       ' suffixe D → Decimal
Dim lettre As Char = "A"c          ' suffixe c → Char
Dim noel As Date = #2026-12-25#    ' littéral de date entre #...#
```

> Les anciens **caractères de type** (`%` Integer, `&` Long, `!` Single, `#` Double, `@` Decimal,
> `$` String) restent valides mais sont un héritage de VB6 : préférez systématiquement la clause `As`.

---

## Déclarer des variables

La forme canonique est `Dim nom As Type`, avec une initialisation facultative :

```vb
Dim compteur As Integer            ' initialisé à 0 (valeur par défaut)
Dim message As String = "Bonjour"  ' avec initialisation
```

Deux subtilités utiles :

- **Déclarations multiples.** `Dim x, y, z As Integer` déclare les **trois** comme `Integer` — un point
  qui surprend les habitués de VB6, où seule la dernière variable recevait le type. Pour des types
  différents, on sépare par des virgules : `Dim nom As String, age As Integer`.
- **Variables locales `Static`.** Une variable locale déclarée `Static` **conserve sa valeur entre les
  appels** de la procédure (sans pour autant être visible ailleurs) :

  ```vb
  Sub Compter()
      Static appels As Integer = 0
      appels += 1
      Console.WriteLine(appels)   ' 1, 2, 3... d'un appel à l'autre
  End Sub
  ```

La portée et la durée de vie des variables (blocs, membres, modificateurs d'accès) sont traitées en
[section 2.11](11-portee-visibilite.md).

---

## Types nullables de valeur ⚠️

Un type valeur ne peut normalement pas être « absent » : un `Integer` contient toujours un nombre.
**`Nullable(Of T)`** (où `T` est un type valeur) enveloppe ce type pour lui ajouter un état « pas de
valeur » (`Nothing`). VB propose une syntaxe abrégée avec le **suffixe `?`** :

```vb
Dim age As Nullable(Of Integer)    ' forme longue
Dim age2 As Integer?               ' forme abrégée, strictement équivalente
```

On l'interroge avec `HasValue` et `Value`, et l'on teste son absence avec `Is Nothing` (idiomatique) :

```vb
Dim age As Integer? = Nothing
Console.WriteLine(age.HasValue)    ' False
Console.WriteLine(age Is Nothing)  ' True

age = 30
Console.WriteLine(age.HasValue)    ' True
Console.WriteLine(age.Value)       ' 30
```

Pour fournir une **valeur de repli**, VB n'a pas l'opérateur `??` de C# : on emploie l'opérateur `If()`
à deux arguments, ou `GetValueOrDefault()` :

```vb
Dim effectif As Integer = If(age, 0)               ' 30 (ou 0 si age est Nothing)
Dim parDefaut As Integer = age.GetValueOrDefault() ' équivalent ici
```

### La distinction à ne pas manquer : nullables ≠ *nullable reference types* de C#

C'est l'un des pièges les plus fréquents lorsqu'on lit du C# ou qu'on s'appuie sur une IA. **Deux
fonctionnalités différentes portent un nom voisin** :

- **Types nullables de *valeur*** (`Nullable(Of T)` / `T?`) : une fonctionnalité d'**exécution**,
  présente à l'identique en VB et en C#. C'est ce que décrit cette section.
- **Types référence nullables** (*nullable reference types*, NRT) : une fonctionnalité de **C# 8+**, purement
  d'**analyse statique à la compilation**. Elle annote les types référence comme `string?`
  (peut être null) ou `string` (non null) pour produire des **avertissements**. **Cette fonctionnalité
  n'existe pas en VB.NET.**

Conséquence concrète : en VB, le modificateur `?` **ne s'applique qu'aux types valeur**. Tous les types
référence sont implicitement « nullables » (peuvent valoir `Nothing`), sans système d'annotation ni
d'avertissement du compilateur.

```vb
Dim age As Integer?      ' OK — Integer est un type valeur
' Dim nom As String?     ' NE COMPILE PAS — String est un type référence
```

Si une IA vous propose `Dim nom As String?` (en pensant aux NRT de C#), c'est une erreur de syntaxe en
VB. Le sujet est replacé dans le contexte de l'architecture hybride à
l'[Annexe B.7](../annexes/frontiere-vbnet-csharp/README.md).

---

## Inférence de type

Lorsque `Option Infer` est sur `On` ([section 2.1](01-structure-options.md)), le compilateur **déduit le
type** d'une variable locale à partir de sa valeur d'initialisation :

```vb
Dim compteur = 0                    ' inféré : Integer
Dim message = "Bonjour"             ' inféré : String
Dim nombres = New List(Of Integer)  ' inféré : List(Of Integer)
```

Trois choses à retenir :

- l'inférence ne concerne **que les variables locales** (ni les champs, ni les paramètres) ;
- le type inféré est **statique et figé** : il ne s'agit pas de typage dynamique. `Dim compteur = 0`
  reste un `Integer`, et lui affecter une chaîne ensuite est une erreur de compilation ;
- combinée à `Option Strict On`, l'inférence n'affaiblit en rien le typage — elle améliore seulement la
  lisibilité.

En pratique : laissez inférer quand le type est évident à droite (`New List(Of String)`), et conservez
une clause `As` explicite quand elle clarifie l'intention.

---

## Constantes

Une **constante** (`Const`) est une valeur connue à la **compilation**, qui ne peut plus changer :

```vb
Const TauxTVA As Decimal = 0.2D     ' Decimal : cohérent avec un usage monétaire
Const NomApp As String = "MonApplication"
```

Une constante est, par nature, `Shared` et résolue à la compilation (sa valeur est intégrée dans le code
généré). Lorsqu'une valeur doit être calculée à l'exécution mais rester non modifiable ensuite, on
utilise plutôt un champ **`ReadOnly`** (initialisé dans le constructeur) — voir
[section 3.1](../03-poo/01-classes-objets.md).

---

## Énumérations

Une **énumération** (`Enum`) définit un ensemble de constantes nommées. Le type sous-jacent est
`Integer` par défaut, et les valeurs s'incrémentent automatiquement à partir de 0 :

```vb
Enum JourSemaine
    Lundi      ' 0
    Mardi      ' 1
    Mercredi   ' 2
    Jeudi      ' 3
    Vendredi   ' 4
    Samedi     ' 5
    Dimanche   ' 6
End Enum
```

On peut imposer un **type sous-jacent** et des **valeurs explicites** :

```vb
Enum CodeHttp As Integer
    Ok = 200
    NonTrouve = 404
    ErreurServeur = 500
End Enum

Dim jour As JourSemaine = JourSemaine.Mercredi
Console.WriteLine(jour)          ' Mercredi
Console.WriteLine(CInt(jour))    ' 2
```

L'attribut **`<Flags>`** transforme l'énumération en jeu d'indicateurs combinables par bits (valeurs en
puissances de deux), avec les opérateurs `Or` / `And` et la méthode `HasFlag` :

```vb
<Flags>
Enum Permissions
    Aucune = 0
    Lecture = 1
    Ecriture = 2
    Execution = 4
End Enum

Dim p As Permissions = Permissions.Lecture Or Permissions.Ecriture
Console.WriteLine(p.HasFlag(Permissions.Lecture))   ' True
Console.WriteLine(p)                                ' "Lecture, Ecriture"
```

> Les attributs s'écrivent en VB entre chevrons `<...>` (et non entre crochets `[...]` comme en C#).

---

## Et l'IA dans tout ça ? 🤖

Trois confusions classiques quand on génère du code de typage avec un assistant :

- **`Dim nom As String?`** : produit par analogie avec les NRT de C#, mais **invalide en VB** (le `?` ne
  s'applique qu'aux types valeur). À corriger en `Dim nom As String`.
- **`Double` au lieu de `Decimal`** pour des montants : exigez `Decimal` (suffixe `D`) dès qu'il s'agit
  d'argent.
- **`DateTime` vs `Date`** : les deux fonctionnent (`Date` est l'alias VB de `DateTime`), mais le code
  VB idiomatique emploie `Date`.

La marche à suivre générale — préciser « **Visual Basic .NET** » et la version cible — figure au
[module 17](../17-developpement-ia/README.md).

## En résumé

- **Valeur vs référence** : les types valeur se copient ; les types référence partagent un même objet.
  `Nothing` désigne la valeur par défaut du type (référence nulle *ou* zéro d'un type valeur).
- Les **primitifs** VB sont des alias de types .NET ; retenir `Date` (= `DateTime`) et `Decimal`
  (montants).
- Les **types nullables de valeur** (`Integer?`) ajoutent un état « absent » aux types valeur — à ne
  **pas** confondre avec les *nullable reference types* de C#, absents de VB ⚠️.
- L'**inférence** (`Option Infer On`) produit un type statique, jamais dynamique.
- `Const` est résolue à la compilation ; les `Enum` (éventuellement `<Flags>`) regroupent des constantes
  nommées.

---

⏭️ [Opérateurs et expressions (&, Mod, Like, Is/IsNot, AndAlso/OrElse)](/02-fondamentaux-langage/03-operateurs.md)
