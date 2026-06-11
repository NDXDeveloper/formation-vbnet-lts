🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe A — Correspondance syntaxique VB.NET ↔ C# (aide-mémoire)

Tableaux de conversion entre **VB.NET** et **C#** : mots-clés, types, structures, événements, LINQ et async.

Cette annexe est volontairement transversale et **utilitaire**. En 2026, la documentation officielle, les
exemples des bibliothèques NuGet et — surtout — les réponses des assistants IA sont massivement écrits en
**C#** (voir module 17, *le biais C# des modèles* 🤖). Savoir **lire du C# et le transposer en VB.NET** n'est
donc pas un confort : c'est une compétence de travail quotidienne. L'usage le plus fréquent de cet aide-mémoire
va dans ce sens — *« on me donne du C#, je le réécris en VB »* — mais les tableaux se lisent dans les deux sens.

> ⚠️ La majorité des différences sont purement **syntaxiques** (mêmes types CLR, même runtime). Mais quelques
> pièges sont **sémantiques** : les opérateurs `^`, `&`, `\` et `=` ne signifient pas la même chose dans les deux
> langages. Ils sont signalés par ⚠️ et regroupés en fin de document ([§ A.13](#a13--pièges-fréquents-vbnet--c-)).

---

## A.1 — Comment utiliser cet aide-mémoire

- Les exemples VB.NET supposent les options modernes recommandées : `Option Strict On`, `Option Infer On`,
  `Option Explicit On` (voir module 2.1). Sous `Option Strict Off`, certaines conversions implicites et le
  *late binding* deviennent permis — ce qui n'a **aucun** équivalent en C# (toujours strict).
- VB.NET est **insensible à la casse** ; C# est **sensible à la casse**. `Customer`, `customer` et `CUSTOMER`
  désignent le même identifiant en VB, trois identifiants distincts en C#.
- VB délimite les blocs par des mots-clés `End …` / `Next` / `Loop` ; C# par des accolades `{ }` et termine
  chaque instruction par `;`.
- Rappel de périmètre : VB **consomme** des constructions C# qu'il ne sait pas **déclarer** (records, `init`,
  types `Span`-first…). Voir [Annexe B](../frontiere-vbnet-csharp/README.md) 🔗.

---

## A.2 — Squelette d'un fichier et directives

| VB.NET | C# | Remarque |
|--------|-----|----------|
| `Imports System.Text` | `using System.Text;` | Import d'espace de noms |
| `Imports SB = System.Text.StringBuilder` | `using SB = System.Text.StringBuilder;` | Alias |
| `Imports System.Math` | `using static System.Math;` | Import statique des membres |
| `Namespace App.Core` … `End Namespace` | `namespace App.Core { … }` | C# 10+ : aussi `namespace App.Core;` (forme fichier) — pas d'équivalent VB |
| `' commentaire` ou `REM commentaire` | `// commentaire` | Commentaire de ligne |
| *(pas de bloc natif)* | `/* … */` | Commentaire de bloc — VB n'en a pas |
| `''' <summary>…</summary>` | `/// <summary>…</summary>` | Documentation XML |
| `#Region "Nom"` … `#End Region` | `#region Nom` … `#endregion` | Régions de code |
| `#If DEBUG Then` … `#End If` | `#if DEBUG` … `#endif` | Directives de compilation conditionnelle |
| `_` (en fin de ligne) | *(inutile)* | Continuation de ligne explicite (souvent implicite en VB moderne) |
| *(une instruction par ligne)* | `;` | Fin d'instruction |

---

## A.3 — Mots-clés essentiels

### Modificateurs d'accès et de membres

| VB.NET | C# | Remarque |
|--------|-----|----------|
| `Public` | `public` | |
| `Private` | `private` | |
| `Protected` | `protected` | |
| `Friend` | `internal` | Visible dans l'assembly |
| `Protected Friend` | `protected internal` | |
| `Private Protected` | `private protected` | |
| `Shared` | `static` | Membre de type (non d'instance) |
| `ReadOnly` | `readonly` | Champ ; ou propriété en lecture seule |
| `Const` | `const` | Constante de compilation |
| `Overridable` | `virtual` | Membre redéfinissable |
| `Overrides` | `override` | Redéfinit un membre |
| `MustOverride` | `abstract` | Membre abstrait (sans corps) |
| `MustInherit` | `abstract` *(classe)* | Classe abstraite, non instanciable |
| `NotOverridable` | `sealed` | Membre qui clôt la redéfinition (combiné à `Overrides`) |
| `NotInheritable` | `sealed` *(classe)* | Classe scellée, non héritable |
| `Overloads` | *(implicite)* | C# n'exige aucun mot-clé pour surcharger |
| `Shadows` | `new` | Masquage d'un membre hérité ⚠️ VB masque par **nom** (toutes surcharges) ; C# `new` masque par **signature** |
| `Partial` | `partial` | Type/méthode partiel(le) |

### Types, instanciation et conversions

| VB.NET | C# | Remarque |
|--------|-----|----------|
| `Class` / `Structure` / `Interface` / `Enum` | `class` / `struct` / `interface` / `enum` | |
| `Module` | `static class` | Membres accessibles **sans qualification** (≈ fonctions globales) — nuance importante |
| `Delegate` | `delegate` | |
| `Inherits Base` | `: Base` | Héritage |
| `Implements IFoo` | `: IFoo` | Implémentation d'interface |
| `New Customer()` | `new Customer()` | Instanciation |
| `Shared Sub New()` | `static Customer()` | Constructeur statique (initialisation de type, exécuté une seule fois) |
| `Nothing` | `null` *et* `default` | ⚠️ `Nothing` est aussi la valeur par défaut des types valeur (`Dim i As Integer = Nothing` → `0`) |
| `Me` | `this` | Instance courante |
| `MyBase` | `base` | Classe de base |
| `MyClass` | *(pas d'équivalent)* | Appelle l'implémentation **de cette classe**, sans tenir compte des redéfinitions |
| `GetType(Customer)` | `typeof(Customer)` | Objet `Type` |
| `customer.GetType()` | `customer.GetType()` | Identique (méthode d'instance) |
| `TypeOf x Is Customer` | `x is Customer` | Test de type |
| `DirectCast(o, Customer)` | `(Customer)o` | Conversion sans coercition |
| `CType(o, Customer)` | `(Customer)o` | Conversion avec coercition |
| `TryCast(o, Customer)` | `o as Customer` | Conversion sûre (`Nothing`/`null` si échec) |
| `CInt(x)`, `CStr(x)`, `CDbl(x)`, `CBool(x)`… | `Convert.ToInt32(x)`, `(int)x`… | Fonctions de conversion VB |
| `AddressOf MaMethode` | `MaMethode` | Création de délégué (implicite en C#) |

### Contrôle de flux et divers (mots-clés)

| VB.NET | C# |
|--------|-----|
| `Return` | `return` |
| `Throw` | `throw` |
| `Try` / `Catch` / `Finally` | `try` / `catch` / `finally` |
| `Using` … `End Using` | `using (…) { … }` |
| `SyncLock obj` … `End SyncLock` | `lock (obj) { … }` |
| `Continue For` / `Continue While` | `continue` |
| `Exit For` / `Exit Sub` / `Exit Function` | `break` / `return` |
| `Yield value` | `yield return value;` |
| `Await task` | `await task` |
| `Async` | `async` |
| `Iterator` (fonction) | *(inféré du `yield`)* |
| `With obj` … `End With` | *(pas d'équivalent — répéter `obj.`)* |
| `Static n As Integer = 0` *(variable locale persistante)* | *(pas d'équivalent — utiliser un champ privé)* |

---

## A.4 — Types de données

| VB.NET | C# | Type .NET (CLR) |
|--------|-----|-----------------|
| `Boolean` | `bool` | `System.Boolean` |
| `Byte` | `byte` | `System.Byte` |
| `SByte` | `sbyte` | `System.SByte` |
| `Short` | `short` | `System.Int16` |
| `UShort` | `ushort` | `System.UInt16` |
| `Integer` | `int` | `System.Int32` |
| `UInteger` | `uint` | `System.UInt32` |
| `Long` | `long` | `System.Int64` |
| `ULong` | `ulong` | `System.UInt64` |
| `Single` | `float` | `System.Single` |
| `Double` | `double` | `System.Double` |
| `Decimal` | `decimal` | `System.Decimal` |
| `Char` | `char` | `System.Char` |
| `String` | `string` | `System.String` |
| `Date` | `DateTime` | `System.DateTime` |
| `Object` | `object` | `System.Object` |
| *(via `IntPtr`)* | `nint` | `System.IntPtr` |
| *(via `UIntPtr`)* | `nuint` | `System.UIntPtr` |
| `Integer?` ou `Nullable(Of Integer)` | `int?` ou `Nullable<int>` | `System.Nullable(Of T)` |

> ⚠️ Le mot-clé VB `Date` correspond à `System.DateTime`, **pas** à `DateOnly`. Les types `DateOnly` et
> `TimeOnly` (depuis .NET 6) s'utilisent par leur nom complet dans les deux langages.
>
> ⚠️ Les types nullables ci-dessus sont des **types valeur nullables** (`Nullable(Of T)`). Ils n'ont rien à voir
> avec les *nullable reference types* de C# (annotations `string?`), une fonctionnalité de C# **sans équivalent
> en VB** (voir module 2.2).

---

## A.5 — Variables, constantes, énumérations et tableaux

| VB.NET | C# |
|--------|-----|
| `Dim x As Integer = 5` | `int x = 5;` |
| `Dim a, b As Integer` | `int a, b;` |
| `Dim x = 5` *(Option Infer On)* | `var x = 5;` |
| `Const Pi As Double = 3.14` | `const double Pi = 3.14;` |
| `Dim arr(4) As Integer` | `int[] arr = new int[5];` |
| `Dim arr() As Integer = {1, 2, 3}` | `int[] arr = { 1, 2, 3 };` |
| `Dim m(2, 3) As Integer` | `int[,] m = new int[3, 4];` |
| `Dim j()() As Integer` | `int[][] j;` |
| `ReDim Preserve arr(9)` | `Array.Resize(ref arr, 10);` |

```vb
Enum Couleur
    Rouge
    Vert = 10
    Bleu
End Enum
```
```csharp
enum Couleur {
    Rouge,
    Vert = 10,
    Bleu
}
```

> ⚠️ **Piège majeur des tableaux.** En VB, le nombre entre parenthèses est l'**indice maximal**, pas la taille :
> `Dim arr(4)` crée **5** éléments (indices 0 à 4). En C#, `new int[4]` crée **4** éléments (indices 0 à 3).
> C'est l'une des erreurs de transposition les plus courantes (et les plus silencieuses).

---

## A.6 — Opérateurs

| VB.NET | C# | Signification | Piège |
|--------|-----|---------------|-------|
| `=` | `=` / `==` | Affectation **ou** égalité selon le contexte | ⚠️ En C#, `==` pour comparer, `=` pour affecter |
| `<>` | `!=` | Différent | |
| `<` `>` `<=` `>=` | `<` `>` `<=` `>=` | Comparaisons | |
| `AndAlso` | `&&` | ET logique court-circuit | |
| `OrElse` | `\|\|` | OU logique court-circuit | |
| `And` | `&` | ET (logique non court-circuit / binaire) | ⚠️ En VB, `&` = **concaténation** |
| `Or` | `\|` | OU (logique non court-circuit / binaire) | |
| `Xor` | `^` | OU exclusif | ⚠️ En C#, `^` = XOR ; en VB, `^` = **puissance** |
| `Not` | `!` (logique) / `~` (binaire) | NON | |
| `&` | `+` | Concaténation de chaînes | ⚠️ Voir ci-dessus |
| `+` `-` `*` `/` | `+` `-` `*` `/` | Arithmétique | |
| `\` | `/` *(opérandes entiers)* | Division **entière** | ⚠️ En VB, `/` renvoie toujours un flottant |
| `Mod` | `%` | Modulo | |
| `^` | `Math.Pow(a, b)` | Puissance | ⚠️ Aucun opérateur de puissance en C# |
| `<<` `>>` | `<<` `>>` | Décalages de bits | |
| `+=` `-=` `*=` `/=` | `+=` `-=` `*=` `/=` | Affectations composées | |
| `&=` | `+=` | Concaténation composée | |
| `If(a, b)` | `a ?? b` | Coalescence des nuls | |
| `?.` `?(…)` | `?.` `?[…]` | Accès conditionnel aux nuls | Identique (ajouté en VB 14) |
| `If(cond, a, b)` | `cond ? a : b` | Opérateur ternaire | |
| `NameOf(client)` | `nameof(client)` | Nom d'un symbole en chaîne | Identique — omniprésent en C# moderne (`ArgumentNullException(nameof(x))`) |
| `$"Total : {total}"` | `$"Total : {total}"` | Interpolation de chaînes | Identique (VB 14 / C# 6) |
| `Is` | `==` *(références)* / `is null` | Égalité de référence / test de nullité | |
| `IsNot` | `!=` *(références)* / `is not null` | Inégalité de référence | |
| `Like` | *(pas d'équivalent)* | Correspondance par motif (`*`, `?`, `#`, `[…]`) | À remplacer par `Regex` |
| `TypeOf x Is T` | `x is T` | Test de type | |

---

## A.7 — Structures de contrôle

### Condition `If`

```vb
If x > 0 Then
    Positif()
ElseIf x < 0 Then
    Negatif()
Else
    Zero()
End If

' Forme sur une ligne
If x > 0 Then Positif()
```
```csharp
if (x > 0) {
    Positif();
} else if (x < 0) {
    Negatif();
} else {
    Zero();
}

// Forme sur une ligne
if (x > 0) Positif();
```

### `Select Case` ↔ `switch`

```vb
Select Case note
    Case 1
        Afficher("Un")
    Case 2, 3
        Afficher("Deux ou trois")
    Case 4 To 6
        Afficher("Entre quatre et six")
    Case Is > 10
        Afficher("Grand")
    Case Else
        Afficher("Autre")
End Select
```
```csharp
switch (note) {
    case 1:
        Afficher("Un");
        break;
    case 2:
    case 3:
        Afficher("Deux ou trois");
        break;
    case >= 4 and <= 6:        // C# 9+ (motifs relationnels)
        Afficher("Entre quatre et six");
        break;
    case > 10:
        Afficher("Grand");
        break;
    default:
        Afficher("Autre");
        break;
}
```

> ⚠️ `Select Case` gère nativement les plages (`4 To 6`) et les comparaisons (`Is > 10`). En C# « classique »,
> il faut les motifs relationnels (C# 9+) ou des gardes `when`. À noter aussi : en C#, chaque `case` doit se
> terminer par `break` (pas de *fall-through* implicite) ; VB le fait automatiquement.

### Boucles

```vb
For i As Integer = 0 To 9
    Traiter(i)
Next

For i = 10 To 0 Step -2
    Traiter(i)
Next

For Each item In collection
    Traiter(item)
Next

While condition
    Traiter()
End While

Do While condition
    Traiter()
Loop

Do
    Traiter()
Loop Until condition
```
```csharp
for (int i = 0; i <= 9; i++) {
    Traiter(i);
}

for (int i = 10; i >= 0; i -= 2) {
    Traiter(i);
}

foreach (var item in collection) {
    Traiter(item);
}

while (condition) {
    Traiter();
}

while (condition) {
    Traiter();
}

do {
    Traiter();
} while (!condition);
```

> ⚠️ Dans `For i = 0 To 9`, la borne `9` est **incluse** : la boucle fait 10 itérations, comme `i <= 9` en C#
> (et non `i < 9`).

---

## A.8 — Méthodes, paramètres et propriétés

```vb
' Procédure (ne renvoie rien)
Sub Saluer(nom As String)
    Console.WriteLine($"Bonjour {nom}")
End Sub

' Fonction (renvoie une valeur)
Function Additionner(a As Integer, b As Integer) As Integer
    Return a + b
End Function
```
```csharp
// Méthode void
void Saluer(string nom) {
    Console.WriteLine($"Bonjour {nom}");
}

// Méthode avec valeur de retour
int Additionner(int a, int b) {
    return a + b;
}
```

### Modificateurs de paramètres

| VB.NET | C# | Remarque |
|--------|-----|----------|
| `(x As Integer)` | `(int x)` | Passage par valeur (défaut) |
| `ByVal x As Integer` | `(int x)` | Par valeur (explicite ; `ByVal` est le défaut en VB) |
| `ByRef x As Integer` | `ref int x` | Par référence |
| *(`ByRef` + attribut `<Out>`)* | `out int x` | Paramètre de sortie |
| `Optional p As Integer = 1` | `int p = 1` | Paramètre optionnel |
| `ParamArray items() As Object` | `params object[] items` | Nombre variable d'arguments |
| `Appel(nom:="Erreur", niveau:=3)` | `Appel(nom: "Erreur", niveau: 3)` | Arguments nommés (`:=` vs `:`) |

### Propriétés

```vb
' Propriété automatique
Public Property Nom As String

' Avec valeur initiale
Public Property Compte As Integer = 0

' Lecture seule
Public ReadOnly Property Id As Guid

' Propriété complète
Private _nom As String
Public Property NomComplet As String
    Get
        Return _nom
    End Get
    Set(value As String)
        _nom = value
    End Set
End Property
```
```csharp
// Propriété automatique
public string Nom { get; set; }

// Avec valeur initiale
public int Compte { get; set; } = 0;

// Lecture seule
public Guid Id { get; }

// Propriété complète
private string _nom;
public string NomComplet {
    get { return _nom; }
    set { _nom = value; }
}
```

### Propriété par défaut ↔ indexeur

Ce que C# appelle *indexeur* (`this[…]`) est en VB une **propriété par défaut** (`Default Property`) — la
construction qui permet d'écrire `panier(0)` au lieu de `panier.Item(0)`.

```vb
Public Class Panier
    Private ReadOnly _elements As New List(Of String)

    Default Public Property Item(index As Integer) As String
        Get
            Return _elements(index)
        End Get
        Set(value As String)
            _elements(index) = value
        End Set
    End Property
End Class

' Usage : panier(0) = "pomme"  — ou la forme longue panier.Item(0)
```
```csharp
public class Panier {
    private readonly List<string> _elements = new();

    public string this[int index] {
        get { return _elements[index]; }
        set { _elements[index] = value; }
    }
}

// Usage : panier[0] = "pomme";
```

> ⚠️ En VB, une propriété par défaut doit **obligatoirement avoir au moins un paramètre requis** (BC31048 —
> pas de « propriété par défaut sans index »), et l'accès se fait avec des **parenthèses** — ce qui se confond
> visuellement avec un appel de méthode ou un accès de tableau. En C#, les crochets `[…]` lèvent l'ambiguïté.

---

## A.9 — Classes, héritage, interfaces et génériques

```vb
Public MustInherit Class Forme
    Public Property Nom As String

    Public Sub New(nom As String)
        Me.Nom = nom
    End Sub

    Public MustOverride Function Aire() As Double
End Class

Public NotInheritable Class Cercle
    Inherits Forme
    Implements IComparable(Of Cercle)

    Public Property Rayon As Double

    Public Sub New(rayon As Double)
        MyBase.New("Cercle")
        Me.Rayon = rayon
    End Sub

    Public Overrides Function Aire() As Double
        Return Math.PI * Rayon ^ 2
    End Function

    Public Function CompareTo(other As Cercle) As Integer _
        Implements IComparable(Of Cercle).CompareTo
        Return Rayon.CompareTo(other.Rayon)
    End Function
End Class
```
```csharp
public abstract class Forme {
    public string Nom { get; set; }

    public Forme(string nom) {
        this.Nom = nom;
    }

    public abstract double Aire();
}

public sealed class Cercle : Forme, IComparable<Cercle> {
    public double Rayon { get; set; }

    public Cercle(double rayon) : base("Cercle") {
        this.Rayon = rayon;
    }

    public override double Aire() {
        return Math.PI * Math.Pow(Rayon, 2);
    }

    public int CompareTo(Cercle other) {
        return Rayon.CompareTo(other.Rayon);
    }
}
```

> ⚠️ VB exige une clause `Implements IFoo.Membre` **explicite** sur chaque membre d'interface ; C# associe les
> membres par leur **signature** (l'implémentation explicite `IFoo.Membre` y existe mais reste l'exception).

### `Module` ↔ classe statique

```vb
Module Outils
    Public Function Doubler(x As Integer) As Integer
        Return x * 2
    End Function
End Module

' Appel : Doubler(21)  ' sans qualification
```
```csharp
public static class Outils {
    public static int Doubler(int x) {
        return x * 2;
    }
}

// Appel : Outils.Doubler(21);  // qualification requise
```

### Génériques et contraintes

```vb
Public Class Boite(Of T)
    Public Property Valeur As T
End Class

Public Function Max(Of T As IComparable(Of T))(a As T, b As T) As T
    Return If(a.CompareTo(b) >= 0, a, b)
End Function
```
```csharp
public class Boite<T> {
    public T Valeur { get; set; }
}

public T Max<T>(T a, T b) where T : IComparable<T> {
    return a.CompareTo(b) >= 0 ? a : b;
}
```

| Contrainte VB.NET | Contrainte C# |
|-------------------|---------------|
| `(Of T As Class)` | `where T : class` |
| `(Of T As Structure)` | `where T : struct` |
| `(Of T As New)` | `where T : new()` |
| `(Of T As BaseClass)` | `where T : BaseClass` |
| `(Of T As IFoo)` | `where T : IFoo` |
| `(Of T As {Class, IFoo, New})` | `where T : class, IFoo, new()` |
| `(Of In T)` / `(Of Out T)` | `<in T>` / `<out T>` *(variance)* |

### Surcharge d'opérateurs et conversions personnalisées

```vb
Public Structure Montant
    Public ReadOnly Property Valeur As Decimal

    Public Sub New(valeur As Decimal)
        Me.Valeur = valeur
    End Sub

    Public Shared Operator +(a As Montant, b As Montant) As Montant
        Return New Montant(a.Valeur + b.Valeur)
    End Operator

    ' Conversion implicite (élargissante) et explicite (restrictive)
    Public Shared Widening Operator CType(d As Decimal) As Montant
        Return New Montant(d)
    End Operator

    Public Shared Narrowing Operator CType(m As Montant) As Decimal
        Return m.Valeur
    End Operator
End Structure

' Usage :
Dim m As Montant = 10D            ' Widening : conversion implicite
Dim total = m + New Montant(5D)   ' opérateur +
Dim d = CType(total, Decimal)     ' Narrowing : conversion explicite
```
```csharp
public struct Montant {
    public decimal Valeur { get; }

    public Montant(decimal valeur) { Valeur = valeur; }

    public static Montant operator +(Montant a, Montant b)
        => new Montant(a.Valeur + b.Valeur);

    // Conversion implicite et explicite
    public static implicit operator Montant(decimal d) => new Montant(d);
    public static explicit operator decimal(Montant m) => m.Valeur;
}

// Usage :
Montant m = 10m;                  // implicit
var total = m + new Montant(5m);  // opérateur +
var d = (decimal)total;           // explicit (cast)
```

> 💡 Correspondance à retenir : `Widening Operator CType` ↔ `implicit operator` ;
> `Narrowing Operator CType` ↔ `explicit operator`. Sous `Option Strict On`, une conversion
> `Narrowing` exige un `CType` explicite — exactement comme le *cast* C#.

---

## A.10 — Événements, délégués et lambdas ⭐

Domaine où VB possède une syntaxe **déclarative** idiomatique (`WithEvents` / `Handles`) sans équivalent direct
en C# (voir module 3.6).

### Délégués et déclaration d'événement

```vb
Public Delegate Sub Notifier(message As String)
Public Delegate Function Transformer(x As Integer) As Integer

Public Event DonneesRecues As EventHandler(Of DonneesEventArgs)
```
```csharp
public delegate void Notifier(string message);
public delegate int Transformer(int x);

public event EventHandler<DonneesEventArgs> DonneesRecues;
```

### Déclencher un événement

```vb
RaiseEvent DonneesRecues(Me, args)
```
```csharp
DonneesRecues?.Invoke(this, args);
```

### S'abonner — idiome déclaratif VB (`WithEvents` / `Handles`)

```vb
Private WithEvents _minuteur As Timer

Private Sub AuTop(sender As Object, e As EventArgs) _
    Handles _minuteur.Tick
    ' …
End Sub
```
```csharp
// Aucun équivalent déclaratif : abonnement impératif
_minuteur.Tick += AuTop;

private void AuTop(object sender, EventArgs e) {
    // …
}
```

### S'abonner — forme impérative

```vb
AddHandler bouton.Click, AddressOf AuClic
RemoveHandler bouton.Click, AddressOf AuClic
```
```csharp
bouton.Click += AuClic;
bouton.Click -= AuClic;
```

### Lambdas

```vb
Dim saluer = Sub(nom As String) Console.WriteLine(nom)
Dim carre = Function(x As Integer) x * x

Dim traiter = Function(x As Integer)
                  Dim y = x * 2
                  Return y + 1
              End Function
```
```csharp
Action<string> saluer = nom => Console.WriteLine(nom);
Func<int, int> carre = x => x * x;

Func<int, int> traiter = x => {
    var y = x * 2;
    return y + 1;
};
```

---

## A.11 — LINQ ⭐

Les **méthodes** LINQ (`Where`, `Select`, `OrderBy`…) sont identiques dans les deux langages : ce sont des
méthodes .NET. Seules la **syntaxe requête** et les lambdas diffèrent.

### Syntaxe requête

```vb
Dim resultats = From p In personnes
                Where p.Age > 18
                Order By p.Nom
                Select p.Nom
```
```csharp
var resultats = from p in personnes
                where p.Age > 18
                orderby p.Nom
                select p.Nom;
```

### Syntaxe méthodes

```vb
Dim resultats = personnes.
    Where(Function(p) p.Age > 18).
    OrderBy(Function(p) p.Nom).
    Select(Function(p) p.Nom)
```
```csharp
var resultats = personnes
    .Where(p => p.Age > 18)
    .OrderBy(p => p.Nom)
    .Select(p => p.Nom);
```

### Correspondance des clauses

| VB.NET | C# |
|--------|-----|
| `From x In xs` | `from x in xs` |
| `Where cond` | `where cond` |
| `Select expr` | `select expr` |
| `Order By x Ascending / Descending` | `orderby x ascending / descending` |
| `Group x By clé Into Group` | `group x by clé into g` |
| `Join y In ys On x.K Equals y.K` | `join y in ys on x.K equals y.K` |
| `Let z = expr` | `let z = expr` |
| `Aggregate … Into …` | *(méthodes d'agrégation)* |

### Regroupement

```vb
Dim parCategorie = From p In produits
                   Group p By p.Categorie Into Groupe = Group
                   Select Categorie, Elements = Groupe
```
```csharp
var parCategorie = from p in produits
                   group p by p.Categorie into g
                   select new { Categorie = g.Key, Elements = g };
```

---

## A.12 — Asynchronisme et itérateurs

```vb
Public Async Function ObtenirDonneesAsync() As Task(Of String)
    Dim resultat = Await client.GetStringAsync(url)
    Return resultat
End Function

Await Task.WhenAll(tache1, tache2)
Await tache.ConfigureAwait(False)
```
```csharp
public async Task<string> ObtenirDonneesAsync() {
    var resultat = await client.GetStringAsync(url);
    return resultat;
}

await Task.WhenAll(tache1, tache2);
await tache.ConfigureAwait(false);
```

| VB.NET | C# |
|--------|-----|
| `Async Function … As Task` | `async Task …` |
| `Async Function … As Task(Of T)` | `async Task<T> …` |
| `Async Sub …` *(à réserver aux gestionnaires d'événements)* | `async void …` |
| `Await expr` | `await expr` |

### Itérateurs (`yield`)

```vb
Public Iterator Function Nombres() As IEnumerable(Of Integer)
    Yield 1
    Yield 2
    Yield 3
End Function
```
```csharp
public IEnumerable<int> Nombres() {
    yield return 1;
    yield return 2;
    yield return 3;
}
```

> ⚠️ VB exige le mot-clé `Iterator` sur la fonction ; C# l'infère de la présence de `yield`.

### ⚠️ Consommation de flux asynchrones (`IAsyncEnumerable`)

C# dispose de `await foreach`. **VB.NET n'a pas d'équivalent** : il faut parcourir manuellement l'énumérateur
asynchrone (voir module 4.6). C'est une limite réelle du langage, pas un oubli de cet aide-mémoire.

```vb
' VB.NET — parcours manuel (pas de "Await For Each").
' Attention : Await est INTERDIT dans un Finally (BC36943) — d'où ce motif
' « capturer, libérer hors du Try, relancer » (détaillé au module 4.6) :
Dim enumerateur = source.GetAsyncEnumerator()
Dim capture As ExceptionDispatchInfo = Nothing
Try
    Do While Await enumerateur.MoveNextAsync()
        Traiter(enumerateur.Current)
    Loop
Catch ex As Exception
    capture = ExceptionDispatchInfo.Capture(ex)
End Try
Await enumerateur.DisposeAsync()
capture?.Throw()
```
```csharp
// C# — construction dédiée
await foreach (var element in source) {
    Traiter(element);
}
```

---

## A.13 — Pièges fréquents VB.NET ↔ C# ⚠️

À relire avant toute transposition. Ces différences sont **sémantiques** (le code compile mais ne fait pas la
même chose), donc bien plus dangereuses que les écarts de syntaxe.

| Sujet | VB.NET | C# | Conséquence |
|-------|--------|-----|-------------|
| **Opérateur `^`** | Puissance (`2 ^ 3` → 8) | OU exclusif (`2 ^ 3` → 1) | Résultat radicalement faux et silencieux |
| **Opérateur `&`** | Concaténation de chaînes | ET binaire / logique | Idem |
| **Opérateur `\` vs `/`** | `\` = division entière ; `/` = division flottante (`5 / 2` → 2.5) | `/` entre entiers = division entière (`5 / 2` → 2) | Perte ou ajout inattendu de décimales |
| **`=`** | Affectation **et** égalité (selon contexte) | `=` affecte, `==` compare | Erreur de compilation ou de logique |
| **Indice des tableaux** | `Dim a(4)` → 5 éléments (borne sup.) | `new int[4]` → 4 éléments (taille) | Décalage de 1, `IndexOutOfRange` |
| **`Nothing`** | Sert aussi de valeur par défaut des types valeur (`Integer = Nothing` → 0) | `null` interdit sur un type valeur non nullable | Comparaisons `Is Nothing` mal transposées |
| **Casse** | Insensible | Sensible | Collisions ou « introuvable » lors de la transposition |
| **`Bool` → entier** | `CInt(True)` → **-1** | Pas de conversion implicite `bool`→`int` | Hypothèses fausses sur la valeur numérique |
| **Échappement de chaînes** | **Aucune séquence** : `\` est littéral, seul `""` note un guillemet | `\n`, `\t`, `\"`, `\\`… (ou verbatim `@"…"`) | `"C:\new"` C# contient un saut de ligne, pas en VB ; transposer `\n` en `vbCrLf`/`Environment.NewLine` |
| **`Option Compare Text`** | Comparaisons de chaînes possiblement insensibles à la casse | Toujours ordinales par défaut | Égalités de chaînes qui changent de résultat |
| **`Select Case`** | Plages et `Is >` natifs | Motifs/`when` requis | Logique de branchement à réécrire |
| **`await foreach`** | Absent | Présent | Flux async à parcourir manuellement |
| **`record`, `init`, `Span`-first** | Consommables, non déclarables | Déclarables | À isoler en bibliothèque C# ([Annexe B](../frontiere-vbnet-csharp/README.md) 🔗) |

---

### Voir aussi

- Module 2 — [Fondamentaux du langage](../../02-fondamentaux-langage/README.md) (types, opérateurs, LINQ)
- Module 3.6 — [Événements et délégués](../../03-poo/06-evenements-delegues.md) ⭐
- Module 17 — [Développer en VB.NET avec l'IA](../../17-developpement-ia/README.md) 🤖 (corriger le C# généré)
- [Annexe B — Frontière VB.NET / C#](../frontiere-vbnet-csharp/README.md) ⭐ 🔗 (ce qu'on délègue à C# et pourquoi)

---

**Juin 2026** · .NET 10 LTS · VB.NET 16.9 (stabilisé) · C# 14

⏭️ [Frontière VB.NET / C# : ce qu'on délègue à C# et pourquoi](/annexes/frontiere-vbnet-csharp/README.md)
