🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.4 Liaison de données (`OneWay`/`TwoWay`, `INotifyPropertyChanged`, `ObservableCollection`, convertisseurs, validation)

Les sections précédentes ont multiplié les `{Binding}` en promettant des explications ; les voici. La **liaison de données** (*data binding*) est le mécanisme central de WPF : elle relie une propriété d'un objet de données à une propriété d'un élément d'interface, et maintient les deux **synchronisées automatiquement**. C'est elle qui rend possible l'écriture d'interfaces sans code de plomberie, et qui fonde l'architecture MVVM ([6.6](06-mvvm.md)).

---

## Le principe de la liaison

Une liaison connecte une **source** (un objet de données) à une **cible** (un élément d'interface). La cible doit exposer une *propriété de dépendance* — c'est le cas de la quasi-totalité des propriétés des contrôles. La source, elle, peut être n'importe quel objet .NET.

```xml
<TextBlock Text="{Binding NomClient}" />
```

Une liaison se définit par quelques composantes :

- la **source** — d'où vient la donnée (par défaut, le `DataContext`) ;
- le **chemin** (`Path`) — la propriété à lire sur la source (`NomClient`, `Adresse.Ville`, `Lignes[0]`) ;
- la **cible** — la propriété d'élément reliée ;
- le **mode** — le sens de circulation de la donnée.

`{Binding NomClient}` est un raccourci pour `{Binding Path=NomClient}`.

---

## Le `DataContext` : la source par défaut

Plutôt que d'indiquer la source dans chaque liaison, on affecte un **`DataContext`** — généralement à la fenêtre ou à un panneau — et toutes les liaisons enfants résolvent leur `Path` contre cet objet. Le `DataContext` est **hérité** le long de l'arbre visuel : défini une fois en haut, il vaut pour tous les descendants.

```vb
' Code-behind : on fournit l'objet de données à la fenêtre
Public Sub New()
    DataContext = New ClientViewModel()
End Sub
```

```xml
<!-- Toutes ces liaisons visent les propriétés du ClientViewModel -->
<StackPanel>
    <TextBox Text="{Binding Nom}" />
    <TextBox Text="{Binding Ville}" />
</StackPanel>
```

Ce principe — un objet de données fourni en `DataContext`, des liaisons qui s'y rattachent — est exactement le socle de MVVM, où le `DataContext` est un *view-model*.

---

## Les modes de liaison

Le **mode** détermine le sens du flux de données.

| Mode | Sens | Usage |
|---|---|---|
| `OneWay` | source → cible | Affichage : l'UI reflète la donnée |
| `TwoWay` | source ↔ cible | Saisie : les modifications circulent dans les deux sens |
| `OneWayToSource` | cible → source | Rare : pousser une valeur d'UI vers la source |
| `OneTime` | source → cible (une seule fois) | Données statiques (gain de performance) |
| `Default` | selon la propriété cible | `TextBox.Text` → `TwoWay`, `TextBlock.Text` → `OneWay` |

```xml
<TextBox Text="{Binding Nom, Mode=TwoWay}" />
```

Chaque propriété de dépendance possède un mode par défaut sensé : un `TextBox.Text` est naturellement en `TwoWay`, un `TextBlock.Text` en `OneWay`. On précise donc rarement le mode, sauf pour le forcer.

### `UpdateSourceTrigger` : *quand* la source est mise à jour

Pour les modes `TwoWay`/`OneWayToSource`, une seconde propriété décide du **moment** où la cible répercute sa valeur vers la source :

- **`LostFocus`** — à la perte du focus (c'est le défaut de `TextBox.Text`) ;
- **`PropertyChanged`** — à chaque modification (donc à chaque frappe) ;
- **`Explicit`** — uniquement sur appel de `UpdateSource()` en code.

> ⚠️ **Surprise fréquente :** par défaut, un `TextBox` ne met *pas* à jour la source à chaque frappe, mais à la perte du focus. Pour une recherche au fil de la saisie ou une validation en temps réel, ajoutez `UpdateSourceTrigger=PropertyChanged`.

---

## Notifier les changements : `INotifyPropertyChanged`

Une liaison `OneWay`/`TwoWay` reflète les changements de la source… **à condition que la source les signale**. Sinon, modifier une propriété en code ne rafraîchira pas l'écran. C'est le rôle de l'interface **`INotifyPropertyChanged`** (espace `System.ComponentModel`) : la classe source y déclare un événement `PropertyChanged` qu'elle déclenche dans chaque accesseur `Set`.

```vb
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class Client
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private _nom As String

    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            If _nom <> value Then
                _nom = value
                NotifierChangement()   ' transmet "Nom" automatiquement
            End If
        End Set
    End Property

    Private Sub NotifierChangement(<CallerMemberName> Optional propriete As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propriete))
    End Sub
End Class
```

Deux points méritent l'attention. D'abord, l'implémentation exige des **propriétés complètes** (avec champ privé et accesseur), car une propriété auto ne peut rien déclencher dans son `Set`. Ensuite, l'attribut **`<CallerMemberName>`** dispense de répéter le nom de la propriété : le compilateur l'injecte. On retrouve ici l'idiome des événements VB vu en [3.6](../03-poo/06-evenements-delegues.md) (`Event` / `RaiseEvent`).

> 🔗 **Le verbeux côté VB.** En C#, le *source generator* `[ObservableProperty]` de `CommunityToolkit.Mvvm` écrit tout ce code à votre place. Ces générateurs étant **C# uniquement**, on écrit en VB.NET ce *boilerplate* à la main — ou l'on hérite de la classe de base `ObservableObject` de la même bibliothèque (utilisable en VB) pour appeler `SetProperty`. Cette approche est détaillée en [6.6](06-mvvm.md).

---

## Lier des collections : `ObservableCollection`

Pour les contrôles de liste ([6.3](03-controles.md)), il existe **deux niveaux de notification** distincts, dont la confusion est une cause classique de « pourquoi l'écran ne se met pas à jour ? ».

- **Ajouter ou retirer un élément** de la collection : il faut que la collection le signale. C'est le rôle d'**`ObservableCollection(Of T)`** (espace `System.Collections.ObjectModel`), qui implémente `INotifyCollectionChanged`. Une simple `List(Of T)` ne préviendrait pas l'interface.
- **Modifier une propriété d'un élément déjà présent** : il faut que la **classe de l'élément** implémente `INotifyPropertyChanged`.

En pratique, on combine donc les deux : une `ObservableCollection(Of Client)` dont la classe `Client` implémente `INotifyPropertyChanged`.

```xml
<DataGrid ItemsSource="{Binding Clients}" AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Nom" Binding="{Binding Nom}" />
    </DataGrid.Columns>
</DataGrid>
```

```vb
' L'ajout se répercute immédiatement (ObservableCollection)
Clients.Add(New Client With {.Nom = "Durand"})

' La modification d'un élément se répercute aussi (Client : INotifyPropertyChanged)
Clients(0).Nom = "Durand-Martin"
```

---

## Adapter les valeurs : les convertisseurs

Quand la source et la cible n'ont pas le même type ou la même représentation, on intercale un **convertisseur** implémentant `IValueConverter` (espace `System.Windows.Data`). Il définit `Convert` (source → cible) et `ConvertBack` (cible → source, pour le `TwoWay`).

```vb
Imports System.Globalization
Imports System.Windows.Data
Imports System.Windows.Media

Public Class BoolEnCouleurConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type,
                            parameter As Object, culture As CultureInfo) As Object _
                            Implements IValueConverter.Convert
        Return If(CBool(value), Brushes.Green, Brushes.Red)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type,
                                parameter As Object, culture As CultureInfo) As Object _
                                Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class
```

On déclare le convertisseur comme **ressource** (notion approfondie en [6.5](05-styles-templates.md)), puis on le référence dans la liaison :

```xml
<Window.Resources>
    <local:BoolEnCouleurConverter x:Key="boolEnCouleur" />
</Window.Resources>
...
<TextBlock Text="{Binding Nom}"
           Foreground="{Binding EstActif, Converter={StaticResource boolEnCouleur}}" />
```

Deux raccourcis évitent souvent d'écrire un convertisseur :

- **`BooleanToVisibilityConverter`**, fourni par WPF, transforme un booléen en `Visibility` (afficher/masquer selon une condition) ;
- **`StringFormat`** suffit pour un simple formatage, sans convertisseur : `{Binding Prix, StringFormat=C}` (monétaire) ou `{Binding Date, StringFormat=d}` (date courte), la culture courante s'appliquant comme vu en [2.7](../02-fondamentaux-langage/07-dates-nombres-culture.md).

---

## Au-delà du `DataContext` : `ElementName` et `RelativeSource`

Une liaison peut viser une autre source que le `DataContext` :

- **`ElementName`** — se lier à la propriété d'un **autre élément** nommé. Idéal pour relier deux contrôles sans code :

```xml
<Slider x:Name="curseur" Minimum="0" Maximum="100" />
<TextBlock Text="{Binding ElementName=curseur, Path=Value}" />
```

- **`RelativeSource`** — se lier relativement à soi-même (`Self`), à un parent (`FindAncestor`) ou au modèle parent (`TemplatedParent`). On l'utilise notamment dans les styles et templates ([6.5](05-styles-templates.md)).

---

## Valider les saisies

Une liaison `TwoWay` peut **valider** la valeur entrante et signaler les erreurs à l'utilisateur. Par défaut, un champ en erreur reçoit une **bordure rouge** ; les détails sont exposés par les propriétés attachées `Validation.HasError` et `Validation.Errors`.

Deux familles d'approches coexistent :

- **Les règles dans la liaison (`ValidationRules`)** — la validation vit dans la vue. Simple à mettre en place, mais peu réutilisable et mal adaptée à MVVM.
- **La validation portée par la source** — le modèle ou le *view-model* connaît ses propres règles, via `IDataErrorInfo` (ancien), **`INotifyDataErrorInfo`** (moderne : plusieurs erreurs par propriété, validation asynchrone) ou les attributs **`DataAnnotations`** (`<Required>`, `<Range>`, `<StringLength>`…, déjà rencontrés côté Windows Forms en [5.7](../05-windows-forms/07-validation.md)). Réutilisable et testable, c'est l'approche **recommandée en MVVM**.

Voici une règle de liaison autonome, qui illustre le mécanisme :

```vb
Imports System.Globalization
Imports System.Windows.Controls   ' ValidationRule, ValidationResult

Public Class ChampRequisRule
    Inherits ValidationRule

    Public Overrides Function Validate(value As Object,
                                       culture As CultureInfo) As ValidationResult
        If String.IsNullOrWhiteSpace(TryCast(value, String)) Then
            Return New ValidationResult(False, "Ce champ est obligatoire.")
        End If
        Return ValidationResult.ValidResult
    End Function
End Class
```

On l'attache via la syntaxe d'élément de liaison (vue en [6.2](02-xaml-layout.md)) :

```xml
<TextBox>
    <TextBox.Text>
        <Binding Path="Nom" UpdateSourceTrigger="PropertyChanged">
            <Binding.ValidationRules>
                <local:ChampRequisRule />
            </Binding.ValidationRules>
        </Binding>
    </TextBox.Text>
</TextBox>
```

Le retour visuel par défaut (bordure rouge) se personnalise via `Validation.ErrorTemplate` ; un motif courant consiste à afficher le premier message d'erreur dans une infobulle, en lisant `(Validation.Errors)[0].ErrorContent`. La validation **intégrée au view-model** (DataAnnotations + `INotifyDataErrorInfo`) sera reprise dans le cadre de MVVM en [6.6](06-mvvm.md).

---

## Côté VB.NET : ce qu'il faut retenir

La liaison de données est **identique en VB.NET et en C#** côté XAML : c'est le même moteur. La seule différence tient au code des sources : faute de *source generators*, on écrit en VB le code de `INotifyPropertyChanged` et de validation à la main, ou l'on s'appuie sur les **classes de base** de `CommunityToolkit.Mvvm` (`ObservableObject`, `ObservableValidator`) — parfaitement utilisables en VB, seulement privées de leurs raccourcis par attributs. Un peu plus verbeux, donc, mais sans aucune perte de fonctionnalité. L'outillage qui réduit cette verbosité est l'objet de la section MVVM.

---

## En résumé

La liaison synchronise une source (souvent le `DataContext`) et une cible d'interface ; son **mode** (`OneWay`, `TwoWay`…) fixe le sens du flux, et `UpdateSourceTrigger` le moment de mise à jour. Pour que les changements remontent à l'écran, la source implémente **`INotifyPropertyChanged`**, et les collections utilisent **`ObservableCollection`** (en gardant à l'esprit les deux niveaux de notification). Les **convertisseurs** adaptent les valeurs entre source et cible, et la **validation** — de préférence portée par le modèle — guide l'utilisateur. En VB.NET, le mécanisme est complet ; seul le code des sources demande un peu plus de frappe qu'en C#.

Reste à donner du style à tout cela : ressources, **styles, templates et triggers** ([6.5](05-styles-templates.md)).

⏭️ [Styles, ressources, templates et triggers](/06-wpf/05-styles-templates.md)
