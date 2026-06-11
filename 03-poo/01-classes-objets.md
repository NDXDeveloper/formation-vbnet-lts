🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.1 Classes et objets

La **classe** est l'unité de base de la programmation orientée objet en VB.NET. C'est un *plan* (ou *modèle*) qui décrit l'état (les données) et le comportement (les opérations) d'une catégorie d'éléments. Un **objet** est une *instance* concrète d'une classe : à partir d'un même plan `Personne`, on peut créer autant d'objets — Alice, Bob, Camille — que nécessaire, chacun avec ses propres valeurs.

En VB.NET, une classe est un **type référence** : une variable de type classe ne contient pas l'objet lui-même, mais une *référence* vers l'emplacement mémoire où il réside. Cette distinction — par opposition aux **types valeur** comme `Structure` (§ 3.2) — a des conséquences importantes sur l'affectation, le passage en argument et l'égalité, abordées en fin de section.

---

## Déclarer une classe

Une classe se déclare avec le bloc `Class … End Class`, généralement précédé d'un modificateur d'accès :

```vb
Public Class Personne

    ' Champs, propriétés, constructeurs et méthodes prennent place ici.

End Class
```

Les modificateurs d'accès les plus courants au niveau d'une classe sont `Public` (visible partout) et `Friend` (visible dans l'assembly courant). La portée et la visibilité sont détaillées au § 2.11.

Une classe regroupe quatre grandes familles de membres :

- les **champs** (*fields*) — les variables internes qui stockent l'état ;
- les **propriétés** (*properties*) — l'interface contrôlée d'accès à l'état ;
- les **méthodes** (*methods*) — le comportement, sous forme de `Sub` ou de `Function` ;
- les **constructeurs** (`Sub New`) — l'initialisation à la création.

---

## Champs

Un champ est une variable déclarée directement dans le corps de la classe. Par convention, les champs sont `Private` et leur nom est préfixé d'un tiret bas (`_`) :

```vb
Public Class Personne
    Private _nom As String
    Private _age As Integer
    Private ReadOnly _id As Guid = Guid.NewGuid()
End Class
```

- Un champ non initialisé prend la **valeur par défaut** de son type (`Nothing` pour les références, `0` pour les numériques, `False` pour `Boolean`).
- Le modificateur `ReadOnly` rend un champ assignable uniquement à sa déclaration ou dans un constructeur ; au-delà, il est en lecture seule. C'est un premier pas vers l'immuabilité (§ 3.7).

> **Bonne pratique d'encapsulation.** On évite d'exposer des champs en `Public`. On expose plutôt des **propriétés**, qui permettent de valider, calculer ou journaliser les accès sans casser le code appelant.

---

## Propriétés

Une propriété ressemble à un champ côté appelant (`p.Nom = "Alice"`), mais c'est en réalité une paire de procédures d'accès : un *accesseur en lecture* (`Get`) et un *accesseur en écriture* (`Set`). VB.NET propose deux formes : auto-implémentée et complète.

### Propriétés auto-implémentées

Lorsqu'aucune logique particulière n'est nécessaire, la forme auto-implémentée tient en une ligne. Le compilateur génère automatiquement le champ de stockage sous-jacent :

```vb
Public Class Personne
    Public Property Nom As String
    Public Property Age As Integer
    Public Property DateInscription As Date = Date.Now   ' valeur initiale
End Class
```

> ⚠️ **Spécificité VB.NET — le champ de stockage `_Nom`.** Pour une propriété auto `Nom`, VB génère un champ privé nommé `_Nom` (préfixe `_` + nom de la propriété). Ce champ est **accessible dans la classe**, ce qui est parfois pratique, mais comme VB.NET est **insensible à la casse**, un champ que vous déclareriez vous-même sous le nom `_nom` entrerait en **collision** avec le `_Nom` généré. Choisissez donc : soit la propriété auto (et son champ `_Nom` implicite), soit une propriété complète avec votre propre champ — pas les deux pour la même donnée.

Une propriété auto peut être déclarée `ReadOnly`. Elle n'est alors assignable que dans un constructeur (ou via un initialiseur) :

```vb
Public Class Personne
    Public ReadOnly Property Id As Guid = Guid.NewGuid()
End Class
```

### Propriétés complètes

Dès qu'une **validation**, un **calcul** ou un **effet de bord** est requis, on écrit la propriété sous sa forme complète, avec un champ de stockage explicite :

```vb
Public Class CompteBancaire

    Private _solde As Decimal

    Public Property Solde As Decimal
        Get
            Return _solde
        End Get
        Set(value As Decimal)
            If value < 0D Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(value), "Le solde ne peut pas être négatif.")
            End If
            _solde = value
        End Set
    End Property

End Class
```

Deux points utiles :

- Le paramètre du `Set` est nommé `value` par convention. Vous pouvez d'ailleurs **l'omettre** : un simple `Set … End Set` met à disposition une variable implicite `value` du type de la propriété.
- Avec `Option Strict On` (recommandé, § 2.1), le type du paramètre `value` doit correspondre exactement à celui de la propriété.

### Propriétés `ReadOnly` et `WriteOnly`

Une propriété en lecture seule n'expose qu'un `Get`. C'est l'outil idéal pour les **valeurs calculées** :

```vb
Public Class Personne
    Public Property Prenom As String
    Public Property Nom As String

    Public ReadOnly Property NomComplet As String
        Get
            Return $"{Prenom} {Nom}"
        End Get
    End Property
End Class
```

Symétriquement, `WriteOnly` n'expose qu'un `Set` (cas rare, par exemple un mot de passe que l'on définit sans pouvoir le relire).

> **Propriété par défaut (indexeur).** VB.NET permet à une classe d'exposer une propriété indexée comme propriété par défaut, à l'aide du mot-clé `Default` : `Default Public ReadOnly Property Item(index As Integer) As String`. On peut alors écrire `monObjet(0)`. C'est l'équivalent de l'*indexer* `this[…]` de C#.

---

## Méthodes

Le comportement d'un objet s'exprime par des méthodes — des `Sub` (sans valeur de retour) ou des `Function` (avec retour). La mécanique des procédures (paramètres `ByVal`/`ByRef`, optionnels, `ParamArray`, surcharge) a été vue au module 2 ; ici, on les déclare simplement comme **membres** de la classe :

```vb
Public Class CompteBancaire

    Private _solde As Decimal

    Public Sub Deposer(montant As Decimal)
        If montant <= 0D Then
            Throw New ArgumentOutOfRangeException(NameOf(montant))
        End If
        _solde += montant
    End Sub

    Public Function PeutRetirer(montant As Decimal) As Boolean
        Return montant > 0D AndAlso montant <= _solde
    End Function

End Class
```

Une méthode peut accéder directement aux champs et propriétés de son objet, sans qualification particulière.

---

## Constructeurs (`Sub New`)

Un constructeur est une procédure spéciale, nommée `New`, exécutée automatiquement à la création d'un objet. Il sert à mettre l'objet dans un **état initial valide**.

### Constructeur par défaut

Si vous ne déclarez aucun constructeur, VB fournit un constructeur public sans paramètre implicite. Dès que vous en déclarez un, ce constructeur implicite **disparaît**.

### Constructeurs paramétrés et surcharge

On peut définir plusieurs constructeurs, distingués par leur signature (surcharge) :

```vb
Public Class Personne

    Public Property Nom As String
    Public Property Age As Integer

    Public Sub New()
        ' Constructeur sans paramètre, valeurs par défaut.
    End Sub

    Public Sub New(nom As String)
        Me.Nom = nom
    End Sub

End Class
```

### Chaînage de constructeurs (`Me.New`)

Pour éviter de dupliquer la logique d'initialisation, un constructeur peut **en appeler un autre** de la même classe via `Me.New(…)`. Cet appel doit être la **première instruction** du constructeur :

```vb
Public Sub New(nom As String, age As Integer)
    Me.New(nom)          ' réutilise le constructeur Sub New(nom)
    Me.Age = age
End Sub
```

L'appel au constructeur de la classe de base se fait de manière analogue avec `MyBase.New(…)` — un sujet développé avec l'héritage au § 3.3.

### Constructeur partagé (`Shared Sub New`)

Un **constructeur partagé** initialise l'état *au niveau de la classe* (membres `Shared`). Il est sans paramètre, sans modificateur d'accès, et s'exécute une seule fois, automatiquement, avant la première utilisation de la classe :

```vb
Public Class Configuration
    Public Shared ReadOnly Version As String

    Shared Sub New()
        Version = "1.0"
    End Sub
End Class
```

---

## Le mot-clé `Me`

`Me` désigne **l'instance courante** de l'objet — l'équivalent de `this` en C#. Ses deux usages principaux :

**1. Lever une ambiguïté de nom** entre un paramètre et un membre :

```vb
Public Sub New(nom As String)
    Me.Nom = nom      ' « Me.Nom » est la propriété ; « nom » est le paramètre.
End Sub
```

**2. Passer l'objet lui-même** à une autre méthode ou l'inscrire à un service :

```vb
Public Sub Enregistrer(registre As RegistreDeComptes)
    registre.Ajouter(Me)    ' transmet l'instance courante
End Sub
```

À noter, deux mots-clés apparentés sont étudiés avec l'héritage (§ 3.3) : `MyBase`, qui référence la classe de base (≈ `base` en C#), et `MyClass`, propre à VB.NET, qui force l'appel de l'implémentation définie *dans la classe courante*, comme si le membre n'était pas surchargeable.

---

## Instancier et initialiser un objet

### L'opérateur `New`

On crée une instance avec `New`, en invoquant l'un des constructeurs disponibles :

```vb
Dim alice As New Personne("Alice")
Dim bob As Personne = New Personne("Bob", 42)
Dim compte = New CompteBancaire()      ' type inféré (Option Infer On)
```

### Initialiseurs d'objet (`With`)

Lorsque les valeurs souhaitées correspondent à des propriétés accessibles, l'initialiseur d'objet `With { … }` permet de les renseigner à la construction, sans constructeur dédié :

```vb
Dim alice As New Personne With {
    .Nom = "Alice",
    .Age = 30
}
```

### `Nothing` et l'absence d'objet

Une variable de type référence non initialisée — ou explicitement affectée à `Nothing` — ne pointe vers aucun objet. Toute tentative d'accès à un membre déclenche alors une `NullReferenceException` :

```vb
Dim p As Personne = Nothing
Dim n = p.Nom            ' ⚠️ NullReferenceException à l'exécution
```

> ⚠️ Contrairement à C#, **VB.NET ne dispose pas des *nullable reference types*** (l'annotation `String?` au sens « référence pouvant être nulle »). La prévention des références nulles relève donc de la discipline de codage : tests explicites (`If p IsNot Nothing Then …`), garde-fous dans les constructeurs et les `Set`. Cette différence est rappelée au § 2.2.

---

## Membres partagés (`Shared`)

Un membre `Shared` (≈ `static` en C#) appartient à la **classe** et non à une instance. Il est commun à tous les objets et s'invoque via le nom de la classe :

```vb
Public Class Personne

    Public Shared Property NombreCrees As Integer

    Public Property Nom As String

    Public Sub New(nom As String)
        Me.Nom = nom
        NombreCrees += 1          ' incrémente le compteur partagé
    End Sub

End Class

' Utilisation :
Dim a As New Personne("Alice")
Dim b As New Personne("Bob")
Console.WriteLine(Personne.NombreCrees)   ' 2
```

Les membres partagés conviennent aux compteurs, constantes calculées, fabriques (*factory methods*) et utilitaires sans état. Lorsqu'**aucun** membre d'instance n'est nécessaire, un **`Module`** (§ 3.5) est souvent un choix plus idiomatique en VB.NET qu'une classe entièrement `Shared`.

---

## Sémantique de référence

Parce qu'une classe est un type référence, **affecter une variable d'objet à une autre copie la référence, pas l'objet**. Les deux variables désignent alors le *même* objet :

```vb
Dim a As New Personne With {.Nom = "Alice"}
Dim b = a          ' b et a référencent le MÊME objet
b.Nom = "Bob"
Console.WriteLine(a.Nom)   ' « Bob » : a et b pointent vers le même objet
```

De même, l'égalité par défaut — la méthode `Equals` héritée d'`Object`, ou l'opérateur `Is` — compare les **références**, non le contenu : deux objets `Personne` distincts portant les mêmes valeurs ne sont pas considérés comme égaux tant qu'on n'a pas redéfini cette logique. À noter qu'en VB, l'opérateur `=` n'est **pas défini** entre deux objets d'une classe (erreur de compilation BC30452, contrairement au `==` de C# qui compare les références) : il ne devient utilisable qu'en le surchargeant. Ce comportement contraste avec celui des `Structure` (§ 3.2) et des *records*, dont l'égalité est structurelle (§ 3.7).

---

## Spécificités VB.NET à retenir

- Le constructeur s'appelle toujours `Sub New` (et non un nom identique à la classe comme en C#).
- Les propriétés s'écrivent en blocs `Get … End Get` / `Set … End Set` ; le paramètre du `Set` est `value`, et peut être implicite.
- Une propriété auto `X` génère un champ `_X` — attention aux collisions dues à l'**insensibilité à la casse** de VB.
- `Me` remplace `this` ; il n'y a **pas** de *nullable reference types*.
- Le mot-clé `Shared` remplace `static`.

> 🤖 **Astuce IA.** La syntaxe des propriétés et des constructeurs est l'une des sources les plus fréquentes de confusion lorsqu'un assistant génère du C# (`public string Nom { get; set; }`, `public Personne(string nom)`) au lieu de VB. Demandez explicitement du « **Visual Basic .NET** », et vérifiez la transposition `Sub New`, blocs `Get`/`Set`, `Shared`. La grille de correspondance complète figure en **[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** ; les bonnes pratiques de prompting au **[module 17](../17-developpement-ia/README.md)**.

---

La classe étant posée, la section suivante examine son pendant en **type valeur** — la `Structure` — ainsi que les **tuples**, et précise quand préférer l'un à l'autre.

⏭️ [Structures (Structure) et tuples](/03-poo/02-structures-tuples.md)
