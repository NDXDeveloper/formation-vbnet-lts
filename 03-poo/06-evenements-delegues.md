🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.6 Événements et délégués — un point fort idiomatique de VB.NET ⭐

Les **événements** sont la colonne vertébrale de toute application pilotée par l'interaction : un clic, un message reçu, une valeur qui franchit un seuil. VB.NET y excelle, au point d'en faire l'un de ses idiomes les plus reconnus. Là où d'autres langages exigent un câblage explicite, VB propose un modèle **déclaratif** (`WithEvents` / `Handles`) et un déclenchement **sûr par construction** (`RaiseEvent`). C'est exactement ce mécanisme qui relie un bouton à son gestionnaire dans le Concepteur Windows Forms (module 5).

Les événements reposent sur les **délégués** ; nous bâtissons donc d'abord cette fondation, avant d'aborder la pièce maîtresse.

---

## Les délégués

### Qu'est-ce qu'un délégué ?

Un délégué est un **type** qui référence une méthode d'une signature donnée — l'équivalent typé et sûr d'un « pointeur de fonction ». Il permet de traiter le comportement comme une donnée : passer une méthode en argument, la stocker, l'invoquer plus tard.

On déclare un type délégué avec `Delegate Function` (méthode renvoyant une valeur) ou `Delegate Sub` (méthode sans retour) :

```vb
Public Delegate Function Operation(a As Integer, b As Integer) As Integer
Public Delegate Sub Notification(message As String)
```

### Créer et invoquer un délégué (`AddressOf`)

Pour pointer un délégué vers une méthode, VB utilise l'opérateur **`AddressOf`** :

```vb
Public Function Additionner(a As Integer, b As Integer) As Integer
    Return a + b
End Function

Dim calcul As Operation = AddressOf Additionner
Dim resultat = calcul(3, 4)        ' 7 — appel indirect via le délégué
```

`AddressOf` est l'idiome VB pour transformer une méthode en délégué (là où C# utilise simplement le nom de la méthode).

### Les délégués génériques du framework : `Action`, `Func`, `Predicate`

En pratique, on déclare rarement ses propres types délégués : le framework fournit des délégués génériques prêts à l'emploi, omniprésents (notamment en LINQ, § 2.9) :

- **`Action`**, **`Action(Of T)`**… — méthode sans valeur de retour ;
- **`Func(Of TResult)`**, **`Func(Of T, TResult)`**… — méthode renvoyant un résultat (le **dernier** paramètre de type est le retour) ;
- **`Predicate(Of T)`** — fonction renvoyant un `Boolean`.

```vb
Dim afficher As Action(Of String) = AddressOf Console.WriteLine
Dim addition As Func(Of Integer, Integer, Integer) = AddressOf Additionner  ' 2 paramètres, 1 retour
Dim estPair As Predicate(Of Integer) = AddressOf VerifierParite
```

### Délégués multicast

Un délégué peut référencer **plusieurs** méthodes à la fois (combinées via `[Delegate].Combine` / `Remove`). Cette capacité « multicast » est précisément ce qui permet à un événement de notifier plusieurs abonnés — sujet de la suite.

---

## Les lambdas

Une **lambda** est une méthode anonyme écrite en ligne. VB distingue, comme partout, la forme `Function` (renvoie une valeur) de la forme `Sub` (sans retour).

```vb
' Lambdas mono-ligne
Dim carre = Function(x As Integer) x * x
Dim saluer = Sub(nom As String) Console.WriteLine($"Bonjour {nom}")

' Lambdas multi-lignes (blocs)
Dim analyser = Function(texte As String)
                   Dim mots = texte.Split(" "c)
                   Return mots.Length
               End Function

Dim journaliser = Sub(message As String)
                      Console.WriteLine(message)
                      ' ... autres instructions
                  End Sub
```

Une lambda **capture** les variables de son contexte englobant (on parle de *fermeture* / closure) :

```vb
Dim seuil = 10
Dim depasse As Predicate(Of Integer) = Function(n) n > seuil   ' « seuil » est capturé
```

Les lambdas sont le carburant de **LINQ** (§ 2.9) — `Where(Function(c) c.Actif)`, `Select(Function(c) c.Nom)` — et des abonnements dynamiques aux événements, comme on le verra plus bas.

---

## Les événements

Un événement est un membre par lequel une classe **notifie** d'autres objets qu'un fait s'est produit, sans connaître ces objets à l'avance. La classe qui émet est l'**éditeur** ; celles qui réagissent sont les **abonnés**.

### Déclarer un événement (`Event`)

La forme la plus simple précise directement les paramètres transmis aux gestionnaires :

```vb
Public Class Compteur
    Public Event SeuilAtteint(valeur As Integer)
End Class
```

Mais pour une API publique, on suit la **convention .NET** : un délégué `EventHandler(Of T)`, dont les gestionnaires reçoivent `(sender As Object, e As T)`, où `T` dérive de `EventArgs`.

```vb
Public Class SeuilEventArgs
    Inherits EventArgs
    Public ReadOnly Property Valeur As Integer
    Public Sub New(valeur As Integer)
        Me.Valeur = valeur
    End Sub
End Class

Public Class Compteur
    Public Event SeuilAtteint As EventHandler(Of SeuilEventArgs)
End Class
```

### Déclencher un événement : `RaiseEvent` ⭐

VB dispose d'un mot-clé dédié, **`RaiseEvent`**, qui déclenche l'événement — et, point essentiel, est **sûr par construction** : s'il n'y a aucun abonné, il ne fait rien, sans qu'on ait à le vérifier.

```vb
Public Class Compteur
    Public Event SeuilAtteint As EventHandler(Of SeuilEventArgs)

    Private _valeur As Integer

    Public Sub Incrementer()
        _valeur += 1
        If _valeur >= 100 Then
            RaiseEvent SeuilAtteint(Me, New SeuilEventArgs(_valeur))
        End If
    End Sub
End Class
```

C'est une différence appréciable avec C#, où déclencher un événement impose de tester soi-même la présence d'abonnés (`X?.Invoke(this, e)`). En VB, `RaiseEvent` s'en charge.

> **Le motif `OnXxx`.** La convention .NET veut qu'on déclenche un événement depuis une méthode `Protected Overridable Sub OnXxx(e As …)`, afin que les classes dérivées puissent l'intercepter ou le déclencher :
> ```vb
> Protected Overridable Sub OnSeuilAtteint(e As SeuilEventArgs)
>     RaiseEvent SeuilAtteint(Me, e)
> End Sub
> ```

### S'abonner — idiome 1 : `WithEvents` + `Handles` ⭐

Voici la signature de VB.NET. On déclare un champ avec **`WithEvents`**, puis un gestionnaire muni d'une clause **`Handles`** : le câblage est **déclaratif**, établi automatiquement, sans aucune ligne d'abonnement explicite.

```vb
Public Class Surveillance
    Private WithEvents _compteur As New Compteur()

    Private Sub Compteur_SeuilAtteint(sender As Object, e As SeuilEventArgs) _
            Handles _compteur.SeuilAtteint
        Console.WriteLine($"Seuil atteint à {e.Valeur}.")
    End Sub
End Class
```

Trois propriétés rendent ce modèle particulièrement puissant :

- **Un gestionnaire, plusieurs événements.** Une même méthode peut traiter plusieurs sources, listées dans `Handles` ; `sender` permet de distinguer l'origine.
  ```vb
  Private Sub Boutons_Click(sender As Object, e As EventArgs) _
          Handles btnOui.Click, btnNon.Click
      ' un seul gestionnaire pour les deux boutons
  End Sub
  ```
- **Re-câblage automatique.** Si l'on réaffecte la variable `WithEvents` à un **nouvel** objet, VB détache les gestionnaires de l'ancien et les rattache au nouveau, sans intervention.
- **C'est le mécanisme du Concepteur.** Dans Windows Forms, chaque `Handles Button1.Click` généré repose sur ce modèle ; les contrôles sont déclarés `WithEvents` dans le fichier `.Designer.vb` (lien avec les classes partielles, § 3.5, et le module 5).

### S'abonner — idiome 2 : `AddHandler` / `RemoveHandler`

Pour un abonnement **dynamique**, à l'exécution — objets créés à la volée, abonnement conditionnel, ou besoin de se désabonner explicitement — on emploie `AddHandler` et `RemoveHandler` :

```vb
Dim compteur As New Compteur()

AddHandler compteur.SeuilAtteint, AddressOf GererSeuil
' ...
RemoveHandler compteur.SeuilAtteint, AddressOf GererSeuil
```

On peut aussi abonner directement une **lambda** (pratique, mais non désabonnable faute de référence conservée) :

```vb
AddHandler compteur.SeuilAtteint,
    Sub(s, e) Console.WriteLine($"Atteint : {e.Valeur}")
```

### Quel idiome choisir ?

| Critère | `WithEvents` + `Handles` | `AddHandler` / `RemoveHandler` |
|---|---|---|
| Moment du câblage | Compile-time, déclaratif | Exécution, dynamique |
| Lisibilité | Maximale (intention visible) | Bonne, mais dispersée |
| Cible | Champs `WithEvents`, `Me`/`MyBase` | N'importe quel objet, même temporaire |
| Désabonnement | Implicite (lié au cycle de vie) | Explicite (`RemoveHandler`) |
| Usage type | Contrôles d'IU, collaborateurs fixes | Objets créés à la volée, abonnement conditionnel |

En règle générale : **`WithEvents`/`Handles`** pour le câblage fixe (le cas le plus fréquent en applications de bureau), **`AddHandler`/`RemoveHandler`** dès que l'abonnement est dynamique ou doit pouvoir être révoqué.

### Événements personnalisés : `Custom Event`

Pour maîtriser finement l'abonnement (liste thread-safe, événements faibles, etc.), VB autorise des **accesseurs d'événement** personnalisés via `Custom Event` — qui comporte, fait notable, **trois** blocs, dont un `RaiseEvent` absent de C# :

```vb
Public Custom Event DonneesRecues As EventHandler(Of DonneesEventArgs)
    AddHandler(value As EventHandler(Of DonneesEventArgs))
        ' enregistrer l'abonné
    End AddHandler
    RemoveHandler(value As EventHandler(Of DonneesEventArgs))
        ' retirer l'abonné
    End RemoveHandler
    RaiseEvent(sender As Object, e As DonneesEventArgs)
        ' invoquer les abonnés
    End RaiseEvent
End Event
```

C'est une fonctionnalité avancée, rarement nécessaire, mais elle illustre la profondeur du modèle événementiel de VB.

### ⚠️ Abonnement et fuites mémoire

Un abonnement crée une référence **forte** de l'éditeur vers l'abonné (via le délégué). Si un abonné s'abonne à un éditeur à longue durée de vie et ne se désabonne **jamais**, il ne pourra pas être collecté par le GC — fuite mémoire classique. Le couple `WithEvents`/`Handles` lie souvent le tout au cycle de vie de l'objet conteneur et atténue le risque ; en revanche, après un `AddHandler` vers un éditeur durable, pensez au `RemoveHandler` (lien avec le GC, § 14.3).

---

## Le pattern Observer idiomatique

Le patron **Observateur** — un *sujet* qui notifie ses *observateurs* de ses changements d'état — se traduit en VB par un simple événement. L'exemple suivant rassemble tout ce qui précède.

```vb
' --- Sujet (observable) ---
Public Class Thermostat
    Private _temperature As Double

    Public Event TemperatureChangee As EventHandler(Of TemperatureEventArgs)

    Public Property Temperature As Double
        Get
            Return _temperature
        End Get
        Set(value As Double)
            If value <> _temperature Then
                _temperature = value
                OnTemperatureChangee(New TemperatureEventArgs(value))
            End If
        End Set
    End Property

    Protected Overridable Sub OnTemperatureChangee(e As TemperatureEventArgs)
        RaiseEvent TemperatureChangee(Me, e)
    End Sub
End Class

Public Class TemperatureEventArgs
    Inherits EventArgs
    Public ReadOnly Property Valeur As Double
    Public Sub New(valeur As Double)
        Me.Valeur = valeur
    End Sub
End Class

' --- Observateur déclaratif (WithEvents + Handles) ---
Public Class AffichagePanneau
    Private WithEvents _thermostat As Thermostat

    Public Sub New(thermostat As Thermostat)
        _thermostat = thermostat
    End Sub

    Private Sub SurChangement(sender As Object, e As TemperatureEventArgs) _
            Handles _thermostat.TemperatureChangee
        Console.WriteLine($"Panneau : {e.Valeur:F1} °C")
    End Sub
End Class
```

Utilisation, combinant abonnement déclaratif et abonnement dynamique :

```vb
Dim thermostat As New Thermostat()
Dim panneau As New AffichagePanneau(thermostat)        ' observateur déclaratif

AddHandler thermostat.TemperatureChangee,              ' observateur dynamique
    Sub(s, e) Console.WriteLine($"Journal : {e.Valeur:F1} °C")

thermostat.Temperature = 21   ' notifie tous les observateurs en une fois
```

Pour des scénarios réactifs plus riches (composition, filtrage, asynchronie de flux d'événements), .NET propose les contrats `IObservable(Of T)` / `IObserver(Of T)` (Reactive Extensions) ; mais pour l'immense majorité des besoins, l'événement reste l'observateur idiomatique de .NET — et VB l'exprime avec une élégance rare.

---

## Correspondance VB.NET ↔ C#

| Opération | VB.NET | C# |
|---|---|---|
| Déclarer un événement | `Public Event X As EventHandler(Of T)` | `public event EventHandler<T> X;` |
| Déclencher | `RaiseEvent X(Me, e)` *(sûr)* | `X?.Invoke(this, e);` |
| S'abonner (dynamique) | `AddHandler X, AddressOf H` | `X += H;` |
| Se désabonner | `RemoveHandler X, AddressOf H` | `X -= H;` |
| S'abonner (déclaratif) | `WithEvents …` + `Handles …` | *(aucun équivalent)* |
| Méthode → délégué | `AddressOf NomMéthode` | `NomMéthode` (groupe de méthodes) |

---

## Spécificités VB.NET à retenir

- `AddressOf` est requis pour obtenir un délégué à partir d'une méthode.
- Lambdas : `Function(…) …` (avec retour) et `Sub(…) …` (sans retour), mono- ou multi-lignes.
- `RaiseEvent` déclenche un événement **sans avoir à tester** la présence d'abonnés.
- **`WithEvents` + `Handles`** offre un câblage **déclaratif** sans équivalent en C# — l'idiome roi des applications de bureau.
- `AddHandler`/`RemoveHandler` (≠ `+=`/`-=` de C#) pour l'abonnement dynamique ; `Custom Event` (trois blocs) pour le contrôle fin.

> 🤖 **Astuce IA.** L'événementiel est l'un des domaines où le code généré « part » le plus en C# : un assistant proposera volontiers `X += handler` (qui **n'existe pas** en VB — il faut `AddHandler`), oubliera les clauses `Handles`, ou ignorera `RaiseEvent` et `WithEvents`. Demandez explicitement du « **Visual Basic .NET** » et appuyez-vous sur le tableau de correspondance ci-dessus. Voir l'**[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** et le **[module 17](../17-developpement-ia/README.md)**.

---

Événements et délégués manipulent du comportement. La section suivante revient sur les **données** sous un angle différent : l'**immuabilité** et les **types `record`** — que VB.NET sait *consommer* sans pouvoir les *déclarer*, un premier contact concret avec la frontière VB/C#.

⏭️ [Types immuables et records](/03-poo/07-immuabilite-records.md)
