🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.7 Types immuables et records ⚠️ 🔗

Cette section aborde un sujet à la **frontière VB.NET / C#**. L'immuabilité — concevoir des objets dont l'état ne change plus après leur création — est pleinement réalisable en VB.NET, à la main. Les **records**, eux, sont la syntaxe concise que C# dédie à cet usage : VB.NET sait les **consommer**, mais **ne sait pas les déclarer**. Nous verrons d'abord comment construire un type immuable en VB pur, puis comment travailler avec les records écrits en C#, dans le cadre de la stratégie hybride (module 10).

---

## L'immuabilité, et pourquoi elle compte

Un type **immuable** expose son état mais interdit toute modification après construction. Cette contrainte, loin d'être un handicap, apporte des garanties précieuses :

- **Sûreté entre threads** — un objet qui ne change jamais peut être partagé et lu par plusieurs threads sans verrou (lien avec le module 4).
- **Prévisibilité** — une fois construit, l'objet ne réserve aucune surprise ; aucun code distant ne peut altérer son contenu « dans votre dos ».
- **Clés et ensembles fiables** — un objet immuable peut servir de clé de dictionnaire ou d'élément de `HashSet` sans risque que son empreinte de hachage ne change.

C'est l'outil de choix pour les **objets-valeurs** (un montant, des coordonnées, une plage de dates), les **DTO** et les données de **configuration**.

---

## Construire un type immuable en VB.NET

VB ne possède pas de `record`, mais l'immuabilité s'obtient en combinant quelques techniques bien connues.

### `ReadOnly` : champs et propriétés en lecture seule

Le pilier est le mot-clé `ReadOnly`, qui rend un membre assignable **uniquement** à sa déclaration ou dans un constructeur. Appliqué aux propriétés auto (§ 3.1), il donne un code concis :

```vb
Public Class Argent
    Public ReadOnly Property Montant As Decimal
    Public ReadOnly Property Devise As String

    Public Sub New(montant As Decimal, devise As String)
        If String.IsNullOrWhiteSpace(devise) Then
            Throw New ArgumentException("Devise requise.", NameOf(devise))
        End If
        Me.Montant = montant       ' assignation autorisée : on est dans le constructeur
        Me.Devise = devise
    End Sub
End Class
```

Après construction, ni `Montant` ni `Devise` ne peuvent plus changer : l'objet est figé.

### Aucune mutation : renvoyer de nouvelles instances

Un type immuable n'expose **aucune méthode mutante**. Les opérations qui « modifient » renvoient en réalité une **nouvelle instance** — c'est la mise à jour *non destructive*, que l'on écrit explicitement en VB :

```vb
' Dans la classe Argent :
Public Function AvecMontant(nouveauMontant As Decimal) As Argent
    Return New Argent(nouveauMontant, Devise)   ' nouvel objet, l'original est intact
End Function
```

```vb
Dim prix As New Argent(19.99D, "EUR")
Dim solde = prix.AvecMontant(14.99D)   ' « prix » reste 19,99 EUR
```

C'est l'équivalent manuel de l'expression `with` de C# (voir plus bas).

### Égalité de valeur : redéfinir `Equals` et `GetHashCode`

Par défaut, deux objets de classe distincts ne sont jamais égaux, même à contenu identique (égalité par référence, § 3.1). Or un objet-valeur doit comparer son **contenu**. On redéfinit donc `Equals` et `GetHashCode` :

```vb
' Dans la classe Argent :
Public Overrides Function Equals(obj As Object) As Boolean
    Dim autre = TryCast(obj, Argent)
    Return autre IsNot Nothing AndAlso
           Montant = autre.Montant AndAlso
           Devise = autre.Devise
End Function

Public Overrides Function GetHashCode() As Integer
    Return HashCode.Combine(Montant, Devise)
End Function

Public Overrides Function ToString() As String
    Return $"{Montant:0.00} {Devise}"
End Function
```

```vb
Dim a As New Argent(10D, "EUR")
Dim b As New Argent(10D, "EUR")
Console.WriteLine(a.Equals(b))   ' True  — égalité de valeur
Console.WriteLine(a Is b)        ' False — objets distincts (Is compare les références)
```

Pour parfaire l'ouvrage, on implémente aussi `IEquatable(Of Argent)` (§ 3.4) pour une égalité typée sans *boxing*, et éventuellement les opérateurs `=` / `<>`. Tout ce travail — égalité, hachage, `ToString` — est précisément ce qu'un record C# **génère automatiquement**.

### Immuabilité profonde : collections en lecture seule

Attention à un piège : un champ `ReadOnly` qui référence une `List(Of T)` n'est immuable qu'**en surface**. La référence ne peut plus changer, mais le **contenu** de la liste, lui, reste modifiable. Pour une immuabilité réelle, exposez une vue en lecture seule (`IReadOnlyList(Of T)`) ou une collection immuable (`System.Collections.Immutable`) :

```vb
Imports System.Collections.Immutable

Public Class Commande
    Private ReadOnly _lignes As ImmutableArray(Of LigneCommande)

    Public Sub New(lignes As IEnumerable(Of LigneCommande))
        _lignes = lignes.ToImmutableArray()    ' copie réellement immuable
    End Sub

    Public ReadOnly Property Lignes As ImmutableArray(Of LigneCommande)
        Get
            Return _lignes
        End Get
    End Property
End Class
```

À défaut d'`ImmutableArray`, `lignes.ToList().AsReadOnly()` fournit au moins une vue `IReadOnlyList(Of T)` non modifiable par l'appelant.

### Le cas des structures immuables

Pour une petite valeur, une **structure immuable** (tous les champs `ReadOnly`, aucune méthode mutante) est une autre voie — en gardant à l'esprit que VB ne déclare pas de `readonly struct` (§ 3.2) et que l'immuabilité y relève donc, là encore, de la discipline.

---

## Les records : ce que VB.NET ne déclare pas ⚠️

### Qu'est-ce qu'un record ?

Introduit en C# 9, le **record** est un type de référence (ou de valeur avec `record struct`, C# 10) pensé pour les données immuables. Sa syntaxe positionnelle tient en une ligne :

```csharp
// En C#
public record Personne(string Nom, int Age);
```

À partir de cette seule déclaration, C# génère : des propriétés `init` en lecture seule, l'**égalité de valeur** (`Equals`/`GetHashCode`/`==`), un `ToString` lisible, la **déconstruction**, et la prise en charge des expressions **`with`** pour la copie non destructive.

### Pourquoi VB ne les déclare pas

> ⚠️ **VB.NET ne possède pas de mot-clé `Record` et ne peut donc pas en déclarer.** Le langage étant figé en version 16.9 (*consumption-only*, § 1.6), cette syntaxe — comme `init`, les expressions `with` ou le `record struct` — restera l'apanage de C#. En VB, l'immuabilité s'écrit à la main, comme ci-dessus.

C'est exactement ce que résumait le tableau du § 3.2 : un *record* est **consommable**, non **déclarable**, en VB.

---

## Consommer un record C# depuis VB.NET 🔗

Vu de l'extérieur, un record n'est qu'une classe (ou une structure) .NET ordinaire. VB.NET peut donc l'utiliser — avec quelques limites bien identifiées. Supposons le record `Personne` ci-dessus, défini dans une bibliothèque C#.

### Ce qui fonctionne ✅

```vb
' Personne provient d'une bibliothèque C#
Dim p As New Personne("Alice", 30)      ' ✅ construction via le constructeur positionnel
Console.WriteLine(p.Nom)                 ' ✅ « Alice »
Console.WriteLine(p.Age)                 ' ✅ 30
Console.WriteLine(p.ToString())          ' ✅ « Personne { Nom = Alice, Age = 30 } »

Dim p2 As New Personne("Alice", 30)
Console.WriteLine(p.Equals(p2))          ' ✅ True — égalité de valeur générée par le record
Console.WriteLine(p Is p2)               ' False — références distinctes
```

Construction, lecture des propriétés, **égalité de valeur** et `ToString` lisible : tout cela est directement accessible depuis VB.

### Ce qui ne fonctionne pas ❌

```vb
' ❌ Expression « with » : inexistante en VB
' Dim plusVieux = p With { .Age = 31 }
Dim plusVieux = New Personne(p.Nom, p.Age + 1)   ' on reconstruit l'objet à la main

' ❌ Déconstruction : aucune syntaxe en VB (cf. § 3.2)
'    → on lit simplement les propriétés : p.Nom, p.Age
```

Deux limites à mémoriser : VB n'a **ni l'expression `with`** (la copie non destructive se fait en reconstruisant), **ni la déconstruction** (on accède aux propriétés).

Bonne nouvelle en revanche, vérifiée sur .NET 10 : les propriétés **`init`** d'un record (ou de toute classe C#) se règlent parfaitement via l'initialiseur **`With { }`** de VB — c'est précisément la capacité de consommation apportée par VB 16.9 (§ 1.6) — et restent verrouillées ensuite (toute réassignation hors initialisation est refusée, erreur BC37311) :

```vb
' Côté C# : public record Adresse(string Rue, string Ville) { public string? CodePostal { get; init; } }
Dim a As New Adresse("12 rue des Lilas", "Paris") With {.CodePostal = "75001"}   ' ✅ init via With { }
' a.CodePostal = "13001"   ' ❌ BC37311 : propriété init, plus modifiable après l'initialisation
```

### Récapitulatif

| Capacité du record | Depuis VB.NET |
|---|---|
| Construire (constructeur positionnel) | ✅ `New Personne("Alice", 30)` |
| Lire les propriétés | ✅ `p.Nom`, `p.Age` |
| Égalité de valeur | ✅ `p1.Equals(p2)` |
| `ToString` lisible | ✅ `p.ToString()` |
| Copie non destructive (`with`) | ❌ → reconstruire, ou méthode C# « WithXxx » |
| Déconstruction | ❌ → lire les propriétés |
| Propriétés `init` via initialiseur `With { }` | ✅ (VB 16.9) — verrouillées après l'initialisation |

---

## Stratégie hybride : où placer les records 🔗

Lorsque votre conception tire un réel bénéfice des records — un modèle de données immuable, partagé, à égalité de valeur, avec une syntaxe concise — la réponse hybride consiste à **écrire le record en C#** (bibliothèque de classes) et à le **consommer depuis VB.NET** (module 10, et **[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**, point B.7).

### Concevoir des records C# « VB-friendly »

Quelques précautions côté C# rendent le record agréable à utiliser depuis VB :

- **Privilégier la forme positionnelle**, dont le constructeur permet à VB de tout initialiser en une fois.
- VB ne disposant pas de `with`, **exposer des méthodes de copie explicites** que VB pourra appeler comme des méthodes ordinaires :

```csharp
// Côté C#
public record Personne(string Nom, int Age)
{
    public Personne WithAge(int age) => this with { Age = age };
}
```

```vb
' Depuis VB, la copie non destructive redevient ergonomique :
Dim plusVieux = p.WithAge(31)   ' ✅ une simple méthode
```

### Faut-il vraiment un record ?

Restons pragmatiques. Pour un type immuable **simple et propre à un projet VB**, la classe écrite à la main (propriétés `ReadOnly` + `Equals`/`GetHashCode`) est parfaitement suffisante — inutile d'introduire un projet C# pour un seul type. Réservez le record C# aux cas où l'on veut spécifiquement sa concision, son égalité de valeur « gratuite » et son ergonomie `with`, **ou** un modèle commun réellement partagé entre code VB et code C#.

---

## Spécificités VB.NET à retenir

- VB **ne déclare pas** de `record`, de propriétés `init`, d'expression `with` ni de `record struct` : l'immuabilité s'écrit à la main.
- Boîte à outils VB : propriétés/champs `ReadOnly`, aucune méthode mutante (renvoyer de nouvelles instances), `Equals`/`GetHashCode` redéfinis, collections en lecture seule pour l'immuabilité **profonde**.
- VB **consomme** un record C# : construction, lecture, **égalité de valeur**, `ToString` et propriétés **`init`** via `With { }` (VB 16.9) ✅ ; **expression `with` et déconstruction** ❌ (reconstruire / lire les propriétés).
- Stratégie hybride 🔗 : écrire les records en C#, prévoir des méthodes « WithXxx » pour les appelants VB.

> 🤖 **Astuce IA.** C'est un piège fréquent : sollicité en « VB », un assistant produit volontiers `Public Record Personne(...)`, une expression `with` ou des propriétés `init` — **rien de tout cela n'existe en VB.NET**. Demandez explicitement du « **Visual Basic .NET** » et, selon le besoin, soit faites-vous générer une **classe immuable manuelle** (propriétés `ReadOnly` + `Equals`/`GetHashCode`), soit déplacez le record dans une **bibliothèque C#** à consommer. Voir l'**[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)**, l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)** et le **[module 17](../17-developpement-ia/README.md)**.

---

L'immuabilité et les records concluent le tour des grandes constructions de types. La dernière section du chapitre explore l'inspection des types **à l'exécution** — la **réflexion** — et leur enrichissement par les **attributs**.

⏭️ [Réflexion et attributs (System.Reflection, attributs personnalisés)](/03-poo/08-reflexion-attributs.md)
