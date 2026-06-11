🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.6 Architecture MVVM

Tout ce que les sections précédentes ont mis en place — la liaison de données ([6.4](04-data-binding.md)), les styles et triggers ([6.5](05-styles-templates.md)) — converge ici. **MVVM** (*Model-View-ViewModel*) est le patron d'architecture pour lequel WPF a, de fait, été conçu : il sépare l'interface de la logique, rend l'application testable, et bannit le code-behind de plomberie. C'est l'argument central pour choisir WPF plutôt que Windows Forms quand la maintenabilité prime ([6.1](01-introduction-wpf-vs-winforms.md)).

C'est aussi la section où se manifeste le **point hybride 🔗** annoncé tout au long du chapitre : l'outillage qui simplifie MVVM en C# (les *source generators*) n'est pas disponible en VB.NET. Nous verrons précisément ce que cela change — et pourquoi cela ne remet pas MVVM en cause en VB.

---

## Les principes de MVVM

MVVM répartit l'application en trois couches aux responsabilités étanches :

| Couche | Rôle | Connaît | Ignore |
|---|---|---|---|
| **Model** | Données et logique métier | — | La vue et le *view-model* |
| **View** (XAML) | Présentation visuelle | Le *view-model* (via `DataContext`) | La logique métier |
| **ViewModel** | État et comportements offerts à la vue | Le modèle | **La vue** (aucune référence aux contrôles) |

La règle d'or est la dernière colonne : **le *view-model* ne connaît pas la vue**. Il expose des **propriétés** (l'état) et des **commandes** (les actions), et la vue s'y relie par liaison. La communication est à sens unique de référence : la vue pointe vers le *view-model* via son `DataContext`, jamais l'inverse.

Trois conséquences en découlent :

- **La liaison de données est la colle.** Les contrôles affichent et modifient les propriétés du *view-model* sans code intermédiaire — d'où l'usage systématique d'`INotifyPropertyChanged` et d'`ObservableCollection` (revus en [6.4](04-data-binding.md)).
- **Les commandes remplacent les gestionnaires d'événements.** Un clic de bouton n'appelle plus un `Button_Click` dans le code-behind, mais déclenche une **commande** (`ICommand`) portée par le *view-model*.
- **Le *view-model* est testable.** N'étant qu'une classe ordinaire sans dépendance à l'UI, il se teste unitairement sans interface graphique (voir [module 13](../13-tests-qualite/README.md)).

---

## Relier la vue au *view-model*

On affecte une instance du *view-model* au `DataContext` de la vue. Deux manières simples :

```vb
' En code-behind
Public Sub New()
    InitializeComponent()
    DataContext = New ClientViewModel()
End Sub
```

```xml
<!-- Ou directement en XAML -->
<Window.DataContext>
    <local:ClientViewModel />
</Window.DataContext>
```

À plus grande échelle, on confie cette association à un *ViewModelLocator* ou à l'injection de dépendances. Quant au code-behind, l'idéal MVVM est de le réduire au minimum — sans dogmatisme : il reste légitime pour des préoccupations purement visuelles (gestion du focus, lancement d'une animation) qui n'ont pas leur place dans le *view-model*.

---

## Les commandes : `ICommand` et `RelayCommand`

L'interface **`ICommand`** (espace `System.Windows.Input`) structure une action liable :

- `Execute(parameter)` — exécute l'action ;
- `CanExecute(parameter)` — indique si elle est possible (active/désactive automatiquement le contrôle lié) ;
- l'événement `CanExecuteChanged` — signale à WPF de réévaluer `CanExecute`.

Écrire une classe par commande serait fastidieux. On utilise donc un **`RelayCommand`** (aussi appelé *DelegateCommand*) : une implémentation générique d'`ICommand` qui encapsule de simples délégués.

```vb
Imports System.Windows.Input

Public Class RelayCommand
    Implements ICommand

    Private ReadOnly _executer As Action
    Private ReadOnly _peutExecuter As Func(Of Boolean)

    Public Sub New(executer As Action, Optional peutExecuter As Func(Of Boolean) = Nothing)
        _executer = executer
        _peutExecuter = peutExecuter
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean _
            Implements ICommand.CanExecute
        Return _peutExecuter Is Nothing OrElse _peutExecuter()
    End Function

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _executer()
    End Sub

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub
End Class
```

Côté vue, on relie la commande à la propriété `Command` du bouton, qui se **désactive de lui-même** lorsque `CanExecute` renvoie `False` :

```xml
<Button Content="Enregistrer" Command="{Binding EnregistrerCommand}" />
```

> 💡 Pour gérer un **paramètre** de commande, on emploie une version générique `RelayCommand(Of T)` et l'attribut `CommandParameter` sur le contrôle.

---

## Un *view-model* « à la main »

En assemblant `INotifyPropertyChanged` (via une classe de base) et `RelayCommand`, on obtient un *view-model* complet. D'abord la base, qui implémente la notification une fois pour toutes :

```vb
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public MustInherit Class ViewModelBase
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propriete As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propriete))
    End Sub
End Class
```

Puis le *view-model* proprement dit :

```vb
Public Class ClientViewModel
    Inherits ViewModelBase

    Private _nom As String
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            If _nom <> value Then
                _nom = value
                OnPropertyChanged()
                EnregistrerCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property EnregistrerCommand As RelayCommand

    Public Sub New()
        _EnregistrerCommand = New RelayCommand(AddressOf Enregistrer, AddressOf PeutEnregistrer)
    End Sub

    Private Sub Enregistrer()
        ' logique métier de sauvegarde
    End Sub

    Private Function PeutEnregistrer() As Boolean
        Return Not String.IsNullOrWhiteSpace(Nom)
    End Function
End Class
```

Cela fonctionne parfaitement — mais on remarque la **verbosité** : la classe de base, la classe `RelayCommand`, et des propriétés complètes pour chaque champ. C'est précisément ce que la bibliothèque suivante allège.

---

## Réduire le *boilerplate* : `CommunityToolkit.Mvvm` 🔗

Le paquet NuGet **`CommunityToolkit.Mvvm`** (maintenu par Microsoft) est la solution MVVM standard aujourd'hui. Il fournit trois choses :

- des **classes de base** : `ObservableObject` (notification, avec une méthode `SetProperty`) et `ObservableValidator` (validation) ;
- des **commandes prêtes à l'emploi** : `RelayCommand`, `RelayCommand(Of T)`, `AsyncRelayCommand` ;
- des **générateurs de source** : les attributs `[ObservableProperty]` et `[RelayCommand]` qui écrivent automatiquement les propriétés et les commandes.

### Le point décisif : les générateurs sont C# uniquement

Ces *source generators* émettent du code **C#** ; ils sont donc **inaccessibles en VB.NET**. Concrètement, le *view-model* le plus concis qui soit en C# — ci-dessous — **n'a pas d'équivalent en VB** :

```csharp
// C# — non disponible en VB.NET
public partial class ClientViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnregistrerCommand))]
    private string nom = "";

    [RelayCommand(CanExecute = nameof(PeutEnregistrer))]
    private void Enregistrer() { /* ... */ }

    private bool PeutEnregistrer() => !string.IsNullOrWhiteSpace(Nom);
}
```

Les attributs génèrent ici la propriété `Nom`, la commande `EnregistrerCommand` et le rafraîchissement de `CanExecute` — sans une ligne de *plomberie*.

### En VB : les classes de base, sans les générateurs

VB.NET conserve néanmoins **tout le reste** de la bibliothèque : on hérite d'`ObservableObject` et l'on instancie `RelayCommand` directement. Le code est plus court que la version « à la main » (plus de `ViewModelBase` ni de `RelayCommand` à écrire), simplement un cran au-dessus de la concision du C# :

```vb
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ClientViewModel
    Inherits ObservableObject

    Private _nom As String
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            If SetProperty(_nom, value) Then            ' notifie l'UI si la valeur change
                EnregistrerCommand.NotifyCanExecuteChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property EnregistrerCommand As IRelayCommand

    Public Sub New()
        _EnregistrerCommand = New RelayCommand(AddressOf Enregistrer, AddressOf PeutEnregistrer)
    End Sub

    Private Sub Enregistrer()
        ' logique métier de sauvegarde
    End Sub

    Private Function PeutEnregistrer() As Boolean
        Return Not String.IsNullOrWhiteSpace(Nom)
    End Function
End Class
```

Pour la **validation** dans le *view-model* (annoncée en [6.4](04-data-binding.md)), on hérite de même d'`ObservableValidator`, qui combine `INotifyDataErrorInfo` et les attributs `DataAnnotations` (`<Required>`, `<Range>`…) — là encore, les classes de base sont pleinement utilisables en VB.

---

## Récapitulatif : trois niveaux de concision

| Approche | Notification (`INPC`) | Commandes | Verbosité |
|---|---|---|---|
| **Manuelle** | Écrire une classe `ViewModelBase` | Écrire une classe `RelayCommand` | Élevée |
| **VB + CommunityToolkit** | Hériter d'`ObservableObject`, appeler `SetProperty` | `RelayCommand` fournie | Moyenne |
| **C# + générateurs** | `[ObservableProperty]` sur un champ | `[RelayCommand]` sur une méthode | Minimale |

La lecture est claire : **VB.NET se situe au niveau intermédiaire** — privé des raccourcis par attributs, mais doté de toutes les classes de base. La perte est de la frappe, jamais de la fonctionnalité. C'est exactement l'illustration de la stratégie hybride du [module 10](../10-hybride-vbnet-csharp/README.md) : si la concision des *view-models* devenait critique, on pourrait les isoler dans une bibliothèque C# consommée depuis VB — mais pour MVVM, la version VB+toolkit suffit très largement.

> 🔗 Les exemples MVVM de la communauté sont massivement en C# et truffés d'attributs `[ObservableProperty]`/`[RelayCommand]`. L'[Annexe A](../annexes/correspondance-vbnet-csharp/README.md) aide à les retranscrire : il s'agit de remplacer chaque attribut par sa propriété complète (avec `SetProperty`) ou son instanciation de `RelayCommand`.

---

## En résumé

MVVM organise une application WPF en trois couches — **Model**, **View**, **ViewModel** — la règle cardinale étant que le *view-model* ignore la vue. La **liaison de données** relie les deux, et les **commandes** (`ICommand` via un `RelayCommand`) remplacent les gestionnaires d'événements, le tout au bénéfice d'une logique **testable**. On peut tout écrire à la main, mais `CommunityToolkit.Mvvm` allège considérablement le travail. En VB.NET, ses **classes de base** (`ObservableObject`, `RelayCommand`, `ObservableValidator`) sont pleinement disponibles ; seuls ses **générateurs de source**, propres à C#, font défaut — une verbosité supplémentaire mesurée, sans aucune limite fonctionnelle.

Avec les données, le style et l'architecture en place, il ne reste qu'à donner vie à l'interface : **animations et multimédia** ([6.7](07-animations-multimedia.md)).

⏭️ [Animations et multimédia (notions)](/06-wpf/07-animations-multimedia.md)
