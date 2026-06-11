🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.3 Héritage et polymorphisme

L'**héritage** et le **polymorphisme** sont deux des quatre piliers de la POO. L'héritage permet à une classe d'en *étendre* une autre, réutilisant son code tout en le spécialisant ; le polymorphisme permet de manipuler des objets de types différents à travers une référence commune, en laissant chacun adopter *son* comportement à l'exécution. VB.NET exprime tout cela avec un jeu de mots-clés en toutes lettres : `Inherits`, `Overridable`/`Overrides`, `MustInherit`/`MustOverride`, `NotInheritable` et `Shadows`, que cette section parcourt en détail.

---

## L'héritage avec `Inherits`

### Principe et terminologie

Une **classe dérivée** (ou *fille*) hérite d'une **classe de base** (ou *parente*) : elle reçoit automatiquement ses membres `Public`, `Protected` et `Friend`, et peut en ajouter de nouveaux ou en spécialiser certains.

L'héritage se déclare avec `Inherits`, placé sur la **première ligne** du corps de la classe :

```vb
Public Class Animal
    Public Property Nom As String
    Public Sub Manger()
        Console.WriteLine($"{Nom} mange.")
    End Sub
End Class

Public Class Chien
    Inherits Animal          ' Chien hérite de tout ce qu'expose Animal

    Public Sub Aboyer()
        Console.WriteLine("Ouaf !")
    End Sub
End Class
```

Un objet `Chien` dispose alors à la fois de `Manger` (hérité) et de `Aboyer` (propre).

> VB.NET ne pratique que l'**héritage simple** : une classe ne peut hériter que d'**une seule** classe de base. La réutilisation de plusieurs « contrats » passe par les **interfaces** (§ 3.4).

Les membres déclarés `Protected` (ou `Protected Friend`) ne sont pas visibles de l'extérieur mais le sont des classes dérivées — ils constituent l'« interface d'héritage » d'une classe (détails au § 2.11).

### Constructeurs et héritage (`MyBase.New`)

À la création d'un objet dérivé, le constructeur de la **classe de base s'exécute en premier**, puis celui de la dérivée. Si vous n'appelez pas explicitement le constructeur de base, VB insère un appel implicite à son constructeur sans paramètre. Mais si la classe de base **n'expose pas** de constructeur sans paramètre, la dérivée **doit** appeler un constructeur de base via `MyBase.New(…)`, en **première instruction** :

```vb
Public Class Animal
    Public Property Nom As String
    Public Sub New(nom As String)
        Me.Nom = nom
    End Sub
End Class

Public Class Chien
    Inherits Animal

    Public Property Race As String

    Public Sub New(nom As String, race As String)
        MyBase.New(nom)      ' obligatoire : Animal n'a pas de constructeur sans paramètre
        Me.Race = race
    End Sub
End Class
```

### `System.Object`, la racine de tout

Toute classe qui ne déclare pas de `Inherits` hérite implicitement de **`System.Object`**. C'est pourquoi tout objet possède d'office `ToString()`, `Equals()` et `GetHashCode()` — des membres `Overridable` que l'on redéfinit fréquemment, comme on le verra ci-dessous.

---

## Le polymorphisme

### Principe

Le polymorphisme permet de référencer un objet dérivé par une variable de son type de base, et d'obtenir malgré tout le comportement de l'objet **réel** à l'exécution. C'est la **résolution dynamique** (*virtual dispatch*) : l'appel est aiguillé vers la version la plus dérivée du membre.

### Membres surchargeables : `Overridable` / `Overrides`

En VB.NET, un membre n'est **pas** surchargeable par défaut. Pour permettre sa redéfinition, la classe de base le marque `Overridable` ; la classe dérivée fournit sa version avec `Overrides` :

```vb
Public Class Animal
    Public Property Nom As String

    Public Overridable Function Crier() As String
        Return "..."                 ' comportement par défaut
    End Function
End Class

Public Class Chien
    Inherits Animal
    Public Overrides Function Crier() As String
        Return "Ouaf"
    End Function
End Class

Public Class Chat
    Inherits Animal
    Public Overrides Function Crier() As String
        Return "Miaou"
    End Function
End Class
```

### Appeler la version de base : `MyBase`

Une redéfinition peut s'appuyer sur l'implémentation héritée via `MyBase`, plutôt que de la réécrire entièrement :

```vb
Public Class ChienPoli
    Inherits Chien
    Public Overrides Function Crier() As String
        Return MyBase.Crier() & " (poliment)"   ' réutilise « Ouaf »
    End Function
End Class
```

### Le polymorphisme en action

L'intérêt apparaît dès qu'on traite une collection hétérogène à travers le type de base :

```vb
Dim animaux As New List(Of Animal) From {
    New Chien With {.Nom = "Rex"},
    New Chat With {.Nom = "Félix"}
}

For Each a In animaux
    Console.WriteLine(a.Crier())   ' « Ouaf » puis « Miaou » : chaque objet répond pour lui-même
Next
```

Le code de la boucle ignore le type concret de chaque animal ; c'est l'objet réel qui décide de sa réponse. Ajouter une nouvelle espèce ne change rien à cette boucle — c'est tout le bénéfice du polymorphisme.

### `MyClass` : forcer l'implémentation de la classe courante

Propre à VB.NET (sans équivalent direct en C#), `MyClass` appelle l'implémentation définie **dans la classe courante**, comme si le membre n'était pas surchargeable — même si une classe dérivée le redéfinit :

```vb
Public Class Base
    Public Overridable Function Valeur() As Integer
        Return 1
    End Function

    Public Function ViaMe() As Integer
        Return Me.Valeur()        ' appel virtuel : version la plus dérivée
    End Function

    Public Function ViaMyClass() As Integer
        Return MyClass.Valeur()   ' toujours Base.Valeur, même depuis une dérivée
    End Function
End Class
```

### Sceller une redéfinition : `NotOverridable`

Pour empêcher qu'une classe dérivée **plus bas** dans la hiérarchie ne redéfinisse à son tour un membre, on scelle la redéfinition avec `NotOverridable` (toujours conjointement à `Overrides`) :

```vb
Public Class Chien
    Inherits Animal
    Public NotOverridable Overrides Function Crier() As String
        Return "Ouaf"             ' aucune sous-classe de Chien ne pourra redéfinir Crier
    End Function
End Class
```

---

## Classes et membres abstraits : `MustInherit` / `MustOverride`

Une classe **abstraite** définit un modèle incomplet, destiné à être hérité mais **jamais instancié directement**. On la marque `MustInherit`. Elle peut contenir des membres **abstraits** — déclarés `MustOverride`, **sans corps** — que toute classe dérivée concrète est *obligée* d'implémenter :

```vb
Public MustInherit Class Forme
    Public MustOverride Function Aire() As Double      ' abstrait : pas de corps, pas de End Function

    Public Function Decrire() As String                ' méthode concrète, héritée telle quelle
        Return $"Cette forme a une aire de {Aire():F2}."
    End Function
End Class

Public Class Cercle
    Inherits Forme
    Public Property Rayon As Double
    Public Overrides Function Aire() As Double
        Return Math.PI * Rayon * Rayon
    End Function
End Class

Public Class Rectangle
    Inherits Forme
    Public Property Largeur As Double
    Public Property Hauteur As Double
    Public Overrides Function Aire() As Double
        Return Largeur * Hauteur
    End Function
End Class
```

Deux règles à retenir :

- un membre `MustOverride` ne peut figurer que dans une classe `MustInherit` ;
- tenter d'écrire `New Forme()` provoque une **erreur de compilation** — seules les classes concrètes (`Cercle`, `Rectangle`) sont instanciables.

Le couple `MustInherit`/`MustOverride` combine ainsi abstraction et polymorphisme : `Forme` garantit que toute forme *sait calculer son aire*, sans préjuger de *comment*.

---

## Empêcher l'héritage : `NotInheritable`

À l'opposé de `MustInherit`, le modificateur `NotInheritable` **scelle** une classe entière : aucune autre classe ne pourra en hériter. C'est utile pour les types utilitaires, les types valeur logiques ou pour verrouiller un comportement de sécurité.

```vb
Public NotInheritable Class Devise
    Public ReadOnly Property Code As String
    Public Sub New(code As String)
        Me.Code = code
    End Sub
End Class

' Public Class DeviseSpeciale : Inherits Devise   ' ❌ erreur : Devise n'est pas héritable
```

---

## Masquer plutôt que redéfinir : `Shadows`

Le mot-clé `Shadows` (≈ `new` en C#) **masque** un membre de la classe de base au lieu de le redéfinir. La différence avec `Overrides` est essentielle et tient à la **règle de résolution**.

```vb
Public Class Base
    Public Sub Afficher()
        Console.WriteLine("Base")
    End Sub
End Class

Public Class Derivee
    Inherits Base
    Public Shadows Sub Afficher()       ' masque Base.Afficher (pas de polymorphisme)
        Console.WriteLine("Derivee")
    End Sub
End Class
```

Le membre appelé dépend alors du **type déclaré** de la variable, et non du type réel de l'objet :

```vb
Dim d As New Derivee()
d.Afficher()                 ' « Derivee » — type déclaré : Derivee

Dim b As Base = d
b.Afficher()                 ' « Base »  — type déclaré : Base (le masquage ne suit pas l'objet réel)
```

À comparer avec une vraie redéfinition (`Overrides`), où l'appel via une référence `Base` exécuterait quand même la version de `Derivee`. La distinction se résume ainsi :

- **`Overrides`** → résolution **dynamique** (selon l'objet réel) : c'est du polymorphisme.
- **`Shadows`** → résolution **statique** (selon le type déclaré) : c'est du masquage.

À noter : `Shadows` masque **par nom** toutes les surcharges du membre de base. Si une classe dérivée redéclare un membre de même nom sans `Overrides` ni `Shadows`, VB émet un **avertissement** et applique un masquage implicite — d'où l'intérêt d'indiquer `Shadows` explicitement pour signaler l'intention.

---

## Travailler avec des références polymorphes

Manipuler des objets via leur type de base est la norme, mais il faut parfois retrouver le type concret. VB.NET propose le test `TypeOf … Is`, puis une conversion (`DirectCast`/`CType`), ou bien la conversion prudente `TryCast` suivie d'un test de nullité :

```vb
For Each a In animaux
    If TypeOf a Is Chien Then
        Dim chien = DirectCast(a, Chien)
        chien.Aboyer()
    End If
Next

' Variante avec TryCast (renvoie Nothing en cas d'échec, sans exception) :
Dim peutEtreChat = TryCast(animal, Chat)
If peutEtreChat IsNot Nothing Then
    peutEtreChat.Crier()
End If
```

> ⚠️ **Filtrage de motifs plus limité qu'en C#.** VB.NET ne dispose pas du *type pattern* avec liaison de variable (`If a Is Chien c Then …`) ni des motifs de `switch` de C#. L'idiome VB reste donc `TypeOf … Is` + conversion, ou `TryCast` + test de nullité (rappel du § 2.4). En conception, on privilégie de toute façon le polymorphisme (`Overridable`/`Overrides`) à ces tests de type, qui trahissent souvent une hiérarchie perfectible.

---

## Récapitulatif des mots-clés

| Mot-clé VB.NET | Rôle | Équivalent C# |
|---|---|---|
| `Inherits` | Hériter d'une classe de base | `: BaseClass` |
| `Overridable` | Rendre un membre surchargeable | `virtual` |
| `Overrides` | Redéfinir un membre surchargeable | `override` |
| `NotOverridable` | Sceller une redéfinition | `sealed` (sur membre) |
| `MustInherit` | Classe abstraite (non instanciable) | `abstract` (classe) |
| `MustOverride` | Membre abstrait (sans corps) | `abstract` (membre) |
| `NotInheritable` | Classe scellée (non héritable) | `sealed` (classe) |
| `Shadows` | Masquer un membre de base | `new` (modificateur) |
| `MyBase` | Référencer la classe de base | `base` |
| `MyClass` | Forcer l'implémentation de la classe courante | *(aucun)* |

---

## Spécificités VB.NET à retenir

- L'héritage est **simple** (`Inherits`, une seule base) ; la composition de contrats passe par les interfaces (§ 3.4).
- Les membres ne sont surchargeables **que** s'ils sont marqués `Overridable` (rien de virtuel par défaut).
- `MyBase` appelle la base ; `MyClass` (propre à VB) force l'implémentation locale, non virtuelle.
- `Overrides` = polymorphisme (type réel) ; `Shadows` = masquage (type déclaré) — ne pas confondre.
- Pas de *type pattern* avec liaison : on utilise `TypeOf … Is` + conversion ou `TryCast` (§ 2.4).

> 🤖 **Astuce IA.** L'héritage est un terrain où le code généré arrive presque toujours en syntaxe C# (`: BaseClass`, `virtual`, `override`, `abstract`, `sealed`, `base`). Demandez explicitement du « **Visual Basic .NET** » et transposez à l'aide du tableau ci-dessus — en surveillant le piège `Overrides` vs `Shadows`, qui n'a pas la même sémantique que le `override` vs `new` de C#. Voir l'**[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** et le **[module 17](../17-developpement-ia/README.md)**.

---

L'héritage relie des classes par une relation « est un ». La section suivante présente l'autre grand mécanisme d'abstraction — les **interfaces** —, qui définissent un *contrat* indépendamment de toute hiérarchie de classes et permettent à un type d'en honorer plusieurs.

⏭️ [Interfaces (Implements, interfaces génériques et multiples)](/03-poo/04-interfaces.md)
