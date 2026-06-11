🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.3 Contrôles et contrôles de données (`DataGrid`, `ListView`)

La section [6.2](02-xaml-layout.md) a posé la structure (XAML et panneaux) ; il s'agit maintenant de la **peupler**. WPF offre une riche bibliothèque de contrôles, organisée selon une logique bien plus puissante que la liste plate de Windows Forms. Après un tour d'horizon, nous nous concentrerons sur les deux contrôles de présentation tabulaire les plus utilisés : **`DataGrid`** et **`ListView`**.

> ℹ️ Les contrôles de données s'alimentent par liaison (`ItemsSource`). Cette section présente leur **structure et leurs capacités** ; la mécanique du *binding* est détaillée en [6.4](04-data-binding.md), et la personnalisation visuelle (templates) en [6.5](05-styles-templates.md). Les exemples ci-dessous utilisent donc `{Binding}` de façon légère, comme une promesse tenue au chapitre suivant.

---

## Le modèle de contrôles WPF

Pour comprendre les contrôles WPF, il faut saisir deux grandes familles plutôt que de les apprendre un à un.

- **`ContentControl` — un contenu unique.** Le contrôle expose une propriété `Content` qui contient **un seul objet**, lequel peut être une chaîne… ou un arbre visuel entier. En relèvent `Button`, `Label`, `CheckBox`, `RadioButton`, `GroupBox`, `ScrollViewer`, et même `Window` ou `UserControl`. C'est ce qui permettait, en [6.2](02-xaml-layout.md), de mettre une icône et un texte dans un bouton.
- **`ItemsControl` — une collection d'éléments.** Le contrôle présente **plusieurs** objets, fournis via la propriété `ItemsSource`, chacun rendu selon un modèle (`ItemTemplate`). En relèvent `ListBox`, `ComboBox`, `ListView`, `DataGrid`, `TreeView`, `TabControl` et `Menu`.

Cette taxonomie est la clé : dès qu'on sait si un contrôle porte *un contenu* ou *une collection*, on en déduit comment le remplir.

---

## Les contrôles courants

| Catégorie | Contrôles | Propriété clé |
|---|---|---|
| Texte | `TextBlock` (affichage), `TextBox` (saisie), `Label`, `PasswordBox` | `Text` / `Content` |
| Commandes | `Button`, `RepeatButton`, `ToggleButton` | `Content`, `Command` |
| Cases et options | `CheckBox`, `RadioButton` | `IsChecked` |
| Sélection | `ComboBox`, `ListBox`, `Slider` | `SelectedItem`, `Value` |
| Dates | `DatePicker`, `Calendar` | `SelectedDate` |
| Indicateurs et médias | `ProgressBar`, `Image` | `Value`, `Source` |
| Conteneurs | `GroupBox`, `Expander`, `TabControl`, `ScrollViewer`, `Border` | `Content` / `Items` |

Quelques précisions utiles :

- **`TextBlock` vs `Label`** — `TextBlock` est un élément léger d'affichage de texte ; `Label` est un véritable contrôle à contenu, qui gère les touches d'accès (`_Nom`) et la propriété `Target`. Pour de la saisie, c'est `TextBox` (avec `AcceptsReturn`, `TextWrapping`, `IsReadOnly`).
- **`CheckBox` à trois états** — sa propriété `IsChecked` est un `Boolean?` (nullable), ce qui autorise l'état indéterminé.

> ⚠️ **`PasswordBox` n'est pas *bindable* directement.** Pour des raisons de sécurité, sa propriété `Password` n'est pas une propriété de dépendance : on ne peut pas la lier comme un `TextBox.Text`. On lit sa valeur en code-behind, ou l'on recourt à un comportement (*behavior*) dédié.

---

## Présenter une collection : la famille `ItemsControl`

Tous les contrôles de liste partagent le même socle, qu'il faut connaître avant d'aborder `DataGrid` et `ListView` :

- **`ItemsSource`** — la collection à afficher (une `List(Of T)`, une `ObservableCollection(Of T)`…).
- **`DisplayMemberPath`** — le raccourci pour n'afficher qu'une propriété sous forme de texte.
- **`ItemTemplate`** — un `DataTemplate` qui décrit le rendu complet de chaque élément (détaillé en [6.5](05-styles-templates.md)).
- **`SelectedItem` / `SelectedIndex` / `SelectedValue`** — l'élément sélectionné.

Une simple `ListBox` illustre le principe :

```xml
<ListBox ItemsSource="{Binding Villes}" DisplayMemberPath="Nom" />
```

Sur ce socle, `DataGrid` et `ListView` ajoutent la notion de **colonnes**.

---

## `DataGrid` — le tableau éditable

Le `DataGrid` affiche des données en **lignes et colonnes** et, contrairement à la plupart des contrôles WPF, il est **éditable par défaut**. C'est le choix naturel pour saisir ou modifier des données tabulaires.

Sa propriété `AutoGenerateColumns` vaut `True` par défaut : il déduit alors les colonnes des propriétés du type lié. En pratique, on la passe souvent à `False` pour maîtriser l'ordre, les en-têtes et les types de colonnes. WPF en propose plusieurs : `DataGridTextColumn`, `DataGridCheckBoxColumn`, `DataGridComboBoxColumn`, `DataGridHyperlinkColumn` et `DataGridTemplateColumn` (contenu sur mesure).

```xml
<DataGrid ItemsSource="{Binding Clients}"
          AutoGenerateColumns="False"
          SelectionMode="Single"
          CanUserAddRows="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Nom" Binding="{Binding Nom}" Width="*" />
        <DataGridTextColumn Header="Ville" Binding="{Binding Ville}" Width="150" />
        <DataGridCheckBoxColumn Header="Actif" Binding="{Binding EstActif}" />
        <DataGridTemplateColumn Header="Action" Width="100">
            <DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <Button Content="Modifier" />
                </DataTemplate>
            </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
    </DataGrid.Columns>
</DataGrid>
```

Le `DataGrid` offre, sans code, une foule de fonctionnalités contrôlables par propriété :

- **Édition** — active par défaut ; `IsReadOnly="True"` pour la désactiver. `CanUserAddRows` et `CanUserDeleteRows` gèrent la ligne d'insertion et la suppression.
- **Tri et redimensionnement** — `CanUserSortColumns`, `CanUserResizeColumns`, `CanUserReorderColumns` (tous actifs par défaut ; clic sur l'en-tête pour trier).
- **Sélection** — `SelectionMode` (`Single` / `Extended`) et `SelectionUnit` (`FullRow` / `Cell` / `CellOrRowHeader`).
- **Présentation** — `FrozenColumnCount` (colonnes figées), `RowDetailsTemplate` (détail dépliable), lignes alternées.

---

## `ListView` et `GridView` — la liste multi-colonnes

`ListView` **hérite de `ListBox`** : sans configuration, c'est une liste de sélection. Sa force vient de sa propriété **`View`**, à laquelle on affecte un **`GridView`** pour obtenir une présentation en colonnes — l'équivalent d'une « vue détaillée ».

```xml
<ListView ItemsSource="{Binding Produits}" SelectionMode="Single">
    <ListView.View>
        <GridView>
            <GridViewColumn Header="Référence"
                            DisplayMemberBinding="{Binding Reference}" Width="120" />
            <GridViewColumn Header="Désignation"
                            DisplayMemberBinding="{Binding Designation}" Width="220" />
            <GridViewColumn Header="Prix"
                            DisplayMemberBinding="{Binding Prix, StringFormat=C}" Width="90" />
        </GridView>
    </ListView.View>
</ListView>
```

Chaque `GridViewColumn` affiche une valeur simple via `DisplayMemberBinding`, ou un contenu riche via `CellTemplate`. À la différence du `DataGrid`, la `ListView` n'édite pas ses cellules : elle excelle dans l'**affichage** soigné et facilement stylisable.

---

## `DataGrid` ou `ListView` ?

| | `DataGrid` | `ListView` + `GridView` |
|---|---|---|
| Vocation | Tableau **éditable** | Liste multi-colonnes, surtout **affichage** |
| Édition en place | Oui (par défaut) | Non (à construire) |
| Types de colonnes | Texte, case, combo, lien, template | `DisplayMemberBinding` ou `CellTemplate` |
| Ajout / suppression de lignes | Intégrés | À gérer soi-même |
| Poids et complexité | Plus lourd, très complet | Plus léger, plus souple visuellement |
| Choix typique | Saisie et édition de données | Présentation en colonnes, listes stylées |

La règle pratique : **on édite avec `DataGrid`, on affiche avec `ListView`**. Pour une liste à une seule colonne ou entièrement « gabaritée », une `ListBox` suffit souvent.

---

## Et pour les données hiérarchiques : `TreeView`

Lorsque les données forment une arborescence (dossiers, organigramme, nomenclature), le contrôle adapté est le `TreeView`, peuplé via un `HierarchicalDataTemplate` qui décrit, pour chaque nœud, ses enfants. Le mécanisme repose sur les mêmes principes de *binding* et de *template* ; il est simplement appliqué de façon récursive.

---

## Côté VB.NET

Le réflexe venu de Windows Forms — ajouter des lignes une à une à un `DataGridView` — n'a plus cours : en WPF, on **lie une collection** et l'on laisse les colonnes et templates la rendre. Le code se contente d'alimenter la source et de lire la sélection.

```vb
Imports System.Collections.ObjectModel

' ...
Private clients As New ObservableCollection(Of Client)()

Private Sub Charger()
    clients.Add(New Client With {.Nom = "Durand", .Ville = "Rouen", .EstActif = True})
    grilleClients.ItemsSource = clients
End Sub

Private Sub Supprimer()
    Dim selection = TryCast(grilleClients.SelectedItem, Client)
    If selection IsNot Nothing Then
        clients.Remove(selection)   ' la grille se rafraîchit automatiquement
    End If
End Sub
```

Le détail vaut d'être souligné : avec une **`ObservableCollection(Of T)`** (espace `System.Collections.ObjectModel`, déjà vue en [2.8](../02-fondamentaux-langage/08-tableaux-collections.md)), tout ajout ou retrait se **répercute aussitôt** à l'écran, car la collection notifie l'interface. Une simple `List(Of T)`, elle, ne signalerait pas ses modifications — d'où l'omniprésence d'`ObservableCollection` dans les applications WPF. C'est l'un des fils conducteurs de la liaison de données, qu'aborde la section suivante.

> 💡 Comparé au `DataGridView` de Windows Forms ([5.4](../05-windows-forms/04-controles-avances.md)), le `DataGrid` de WPF couvre des besoins similaires mais s'inscrit dans le modèle déclaratif : colonnes, édition et tri se déclarent en XAML et se relient à des objets, plutôt que de se piloter en code.

---

## En résumé

Les contrôles WPF se rangent en deux familles — `ContentControl` (un contenu) et `ItemsControl` (une collection) — et cette distinction guide leur utilisation. Pour les données tabulaires, deux contrôles dominent : le **`DataGrid`**, éditable et très complet, pour la **saisie** ; la **`ListView` avec `GridView`**, plus légère, pour l'**affichage** en colonnes. Tous deux s'alimentent par `ItemsSource` et tirent parti d'une `ObservableCollection` pour rester synchronisés.

Cette synchronisation automatique repose entièrement sur un mécanisme que nous avons jusqu'ici survolé : la **liaison de données**, cœur de WPF et objet de la section [6.4](04-data-binding.md).

⏭️ [Liaison de données (OneWay/TwoWay, INotifyPropertyChanged, ObservableCollection, convertisseurs, validation)](/06-wpf/04-data-binding.md)
