🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.4 Interfaces

Là où l'héritage (§ 3.3) exprime une relation « est un(e) », l'**interface** exprime une **capacité** : « est capable de ». Une interface est un **contrat** — une liste de membres qu'un type s'engage à fournir — sans dire *comment* les réaliser. Elle ne porte aucun état ni code partagé : seulement des signatures.

Deux atouts par rapport à l'héritage de classe : un type peut implémenter **plusieurs** interfaces (alors qu'il n'hérite que d'une seule classe), et les interfaces relient des types **sans hiérarchie commune**. C'est le mécanisme de découplage par excellence : on programme contre un contrat, pas contre une implémentation.

---

## Qu'est-ce qu'une interface ?

Une interface définit *quoi*, jamais *comment*. Un objet `Document`, une connexion réseau et un fichier temporaire n'ont aucune parenté, mais tous trois peuvent « être libérables » : ils implémenteront `IDisposable`. Le code qui sait libérer une ressource n'a alors besoin de connaître que ce contrat, pas les types concrets.

Par convention .NET, les noms d'interface commencent par un **`I`** majuscule : `IComparable`, `IEnumerable`, `IDisposable`, `INotifyPropertyChanged`.

---

## Déclarer une interface

Une interface se déclare avec `Interface … End Interface`. Ses membres — méthodes (`Sub`/`Function`), propriétés, événements — sont déclarés **sans modificateur d'accès** (ils sont implicitement `Public`) et **sans corps** :

```vb
Public Interface IFormeGeometrique
    ReadOnly Property Aire As Double
    Function Perimetre() As Double
    Sub Redimensionner(facteur As Double)
End Interface
```

Une interface peut aussi déclarer des événements (`Event`), mais jamais de champs ni de constructeurs : elle n'a pas d'état.

---

## Implémenter une interface : `Implements`

C'est ici que VB.NET se distingue le plus nettement de C#. L'implémentation se fait en **deux temps** :

1. la classe déclare `Implements IInterface` (sur sa propre ligne, comme `Inherits`) ;
2. **chaque membre** qui satisfait le contrat porte sa propre clause `Implements IInterface.Membre`.

```vb
Public Class Cercle
    Implements IFormeGeometrique

    Public Property Rayon As Double

    Public ReadOnly Property Aire As Double Implements IFormeGeometrique.Aire
        Get
            Return Math.PI * Rayon * Rayon
        End Get
    End Property

    Public Function Perimetre() As Double Implements IFormeGeometrique.Perimetre
        Return 2 * Math.PI * Rayon
    End Function

    Public Sub Redimensionner(facteur As Double) Implements IFormeGeometrique.Redimensionner
        Rayon *= facteur
    End Sub
End Class
```

Cette clause `Implements …` par membre est **obligatoire** en VB.NET : un membre de signature identique mais sans clause **ne satisfait pas** le contrat (le compilateur signalera l'interface non implémentée). En contrepartie, elle ouvre une flexibilité que C# n'offre pas dans son implémentation implicite.

### Conséquence 1 : le membre peut porter un autre nom

Puisque le lien est établi explicitement, le membre de la classe **n'est pas tenu de porter le même nom** que celui de l'interface :

```vb
Public Function CalculerPerimetre() As Double Implements IFormeGeometrique.Perimetre
    Return 2 * Math.PI * Rayon
End Function
```

Via une référence `IFormeGeometrique`, on appelle `.Perimetre()` ; sur le type `Cercle`, on appelle `.CalculerPerimetre()`. Les deux désignent le même code.

### Conséquence 2 : un membre peut satisfaire plusieurs contrats

Une même implémentation peut honorer plusieurs membres d'interfaces différentes, listés à la suite :

```vb
Public Sub Fermer() Implements IDisposable.Dispose, IFichier.Fermer
    ' libération unique pour les deux contrats
End Sub
```

### Implémentation « masquée » via un membre `Private`

En VB, le membre qui implémente un contrat peut avoir **n'importe quelle accessibilité**. Le déclarer `Private` le retire de la surface publique de la classe : il n'est plus accessible **que** par une référence d'interface. C'est l'équivalent VB de l'*implémentation explicite d'interface* de C# :

```vb
Public Class Ressource
    Implements IDisposable

    Private Sub Liberer() Implements IDisposable.Dispose
        ' libération des ressources
    End Sub
End Class

' Utilisation :
Dim r As New Ressource()
' r.Liberer()                          ' ❌ inaccessible : membre Private
DirectCast(r, IDisposable).Dispose()   ' ✓ accessible via le contrat

Using res As New Ressource()           ' idiomatique : Using appelle Dispose (§ 14.3)
End Using
```

---

## Implémenter plusieurs interfaces

Une classe peut honorer plusieurs contrats simultanément — c'est l'une des principales raisons d'être des interfaces :

```vb
Public Class Rapport
    Implements IImprimable, IEnregistrable

    Public Sub Imprimer() Implements IImprimable.Imprimer
        ' ...
    End Sub

    Public Sub Enregistrer(chemin As String) Implements IEnregistrable.Enregistrer
        ' ...
    End Sub
End Class
```

---

## Héritage entre interfaces

Une interface peut **hériter** d'une ou plusieurs autres interfaces, via `Inherits`. Le type qui l'implémente doit alors satisfaire l'ensemble des membres hérités :

```vb
Public Interface ILisible
    Function Lire() As String
End Interface

Public Interface IModifiable
    Inherits ILisible            ' IModifiable inclut le contrat de ILisible
    Sub Ecrire(contenu As String)
End Interface
```

Une classe `Implements IModifiable` devra fournir **à la fois** `Lire` et `Ecrire`.

---

## Interfaces génériques

Les interfaces sont fréquemment **génériques** (§ 2.10), ce qui les rend réutilisables tout en restant fortement typées.

### Implémenter une interface générique du framework

Les contrats `IComparable(Of T)`, `IEquatable(Of T)` ou `IEnumerable(Of T)` sont omniprésents. Implémenter `IComparable(Of T)`, par exemple, rend un type triable par `List(Of T).Sort` :

```vb
Public Class Temperature
    Implements IComparable(Of Temperature)

    Public ReadOnly Property Celsius As Double

    Public Sub New(celsius As Double)
        Me.Celsius = celsius
    End Sub

    Public Function CompareTo(other As Temperature) As Integer _
            Implements IComparable(Of Temperature).CompareTo
        Return Celsius.CompareTo(other.Celsius)
    End Function
End Class
```

### Définir sa propre interface générique

Le paramètre de type se déclare avec `(Of T)`, comme pour une classe générique :

```vb
Public Interface IDepot(Of T)
    Function ObtenirParId(id As Integer) As T
    Sub Ajouter(element As T)
    Function ObtenirTout() As IEnumerable(Of T)
End Interface
```

À l'implémentation, on fixe l'argument de type concret — et la clause `Implements` reprend ce type :

```vb
Public Class DepotClients
    Implements IDepot(Of Client)

    Private ReadOnly _clients As New List(Of Client)

    Public Function ObtenirParId(id As Integer) As Client _
            Implements IDepot(Of Client).ObtenirParId
        Return _clients.FirstOrDefault(Function(c) c.Id = id)
    End Function

    Public Sub Ajouter(element As Client) Implements IDepot(Of Client).Ajouter
        _clients.Add(element)
    End Sub

    Public Function ObtenirTout() As IEnumerable(Of Client) _
            Implements IDepot(Of Client).ObtenirTout
        Return _clients
    End Function
End Class
```

---

## Quelques interfaces .NET incontournables

Plusieurs interfaces structurent tout l'écosystème et reviendront au fil de la formation :

- **`IComparable(Of T)`** — ordonner des éléments (tri).
- **`IEquatable(Of T)`** — comparer par égalité de valeur.
- **`IEnumerable(Of T)`** — itérer avec `For Each` (§ 2.5, § 2.8).
- **`IDisposable`** — libération déterministe des ressources, via `Using` (§ 14.3).
- **`INotifyPropertyChanged`** — notifier l'IU lors d'un changement, pilier de la liaison de données WPF (§ 6.4).

---

## Utiliser une interface de façon polymorphe

Comme une classe de base, un type d'interface sert de référence commune à des objets hétérogènes — c'est du polymorphisme fondé sur la capacité plutôt que sur la filiation :

```vb
Dim formes As New List(Of IFormeGeometrique) From {
    New Cercle With {.Rayon = 2},
    New Rectangle With {.Largeur = 3, .Hauteur = 4}
}

For Each f In formes
    Console.WriteLine($"Aire : {f.Aire:F2}")   ' chaque forme répond selon son type réel
Next
```

Le test `TypeOf … Is` fonctionne aussi avec les interfaces, par exemple pour libérer ce qui peut l'être :

```vb
If TypeOf obj Is IDisposable Then
    DirectCast(obj, IDisposable).Dispose()
End If
```

---

## Interface ou classe abstraite ?

Les deux fournissent de l'abstraction, mais répondent à des besoins distincts :

| Critère | Interface | Classe abstraite (`MustInherit`) |
|---|---|---|
| Relation exprimée | « est capable de » | « est un(e) » |
| Implémentations multiples | ✓ (plusieurs interfaces) | ✗ (héritage simple) |
| Peut contenir de l'état (champs) | ✗ | ✓ |
| Peut fournir du code partagé | ✗ *(en VB.NET)* | ✓ (méthodes concrètes) |
| Constructeurs | ✗ | ✓ |
| Usage typique | capacité transverse (`IDisposable`…) | base commune d'une famille de types |

En pratique : **interface** pour une capacité réutilisable et orthogonale ; **classe abstraite** pour partager une base d'implémentation et de l'état au sein d'une même famille. Les deux se combinent souvent (une classe abstraite qui implémente une interface).

---

## Limite : pas de méthodes d'interface par défaut en VB.NET

> ⚠️ 🔗 Depuis C# 8, une interface peut fournir une **implémentation par défaut** de ses membres (*default interface methods*). **VB.NET ne sait pas déclarer** de tels membres : une interface VB reste purement abstraite (signatures seules). Si votre conception réclame des interfaces à comportement par défaut, écrivez-les en **C#** et consommez-les depuis VB.NET, conformément à la stratégie hybride (**[module 10](../10-hybride-vbnet-csharp/README.md)**, **[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**). C'est d'ailleurs le sens de la ligne « ✗ en VB.NET » du tableau ci-dessus.

Face à une interface C# qui contient de tels membres, le comportement de VB — vérifié sur .NET 10 — est **asymétrique** : **appeler** un membre par défaut via une référence d'interface fonctionne (l'implémentation par défaut s'exécute), mais une classe VB qui implémente l'interface doit fournir **tous** les membres, y compris ceux munis d'un défaut — le compilateur VB les exige comme s'ils étaient abstraits (erreur BC30149 sinon). Conclusion pratique pour l'hybride : une interface destinée à être *implémentée* côté VB doit rester **sans membres par défaut**. Dernier détail d'interop : si un membre d'interface C# porte un nom réservé de VB (`Resume`, `Next`, `Error`…), on l'implémente en **échappant l'identificateur entre crochets** — `Public Function [Resume]() As String Implements IHorodate.Resume`.

```vb
' En C#, autorisé :
' public interface ILogger {
'     void Log(string message);
'     void LogError(string m) => Log("ERREUR : " + m);   // implémentation par défaut
' }
'
' En VB.NET : seule la signature « Sub LogError(m As String) » est déclarable,
' sans corps — l'implémentation revient à chaque classe.
```

---

## Spécificités VB.NET à retenir

- Une interface se déclare `Interface … End Interface` ; membres sans modificateur d'accès ni corps.
- L'implémentation exige **deux** choses : `Implements I` sur la classe **et** `Implements I.Membre` sur **chaque** membre.
- Le membre implémentant peut porter un **autre nom**, en satisfaire **plusieurs**, ou être `Private` pour une implémentation **masquée** (équivalent de l'implémentation explicite de C#).
- Plusieurs interfaces possibles (séparées par des virgules) ; héritage d'interfaces via `Inherits`.
- ⚠️ Pas de **méthodes d'interface par défaut** déclarables en VB — à écrire en C# (🔗).

> 🤖 **Astuce IA.** C# implémente les interfaces **par nom** (implicitement, avec `: IInterface`) et **sans clause par membre**. Les assistants oublient donc très souvent la double obligation VB (`Implements I` *et* `Implements I.Membre` sur chaque membre), produisant un code qui ne compile pas. Demandez explicitement du « **Visual Basic .NET** » et vérifiez que **chaque** membre porte bien sa clause `Implements`. Voir l'**[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** et le **[module 17](../17-developpement-ia/README.md)**.

---

Classes, structures, héritage et interfaces forment le vocabulaire des types. La section suivante change d'échelle pour traiter l'**organisation du code** : les **modules**, les **espaces de noms** et les **classes partielles**.

⏭️ [Modules, espaces de noms et classes partielles](/03-poo/05-modules-namespaces.md)
