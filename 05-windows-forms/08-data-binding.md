🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.8 Liaison de données WinForms (`BindingSource`, `BindingList`, liaison à une BDD)

La **liaison de données** (*data binding*) relie automatiquement vos contrôles à vos données : les deux restent synchronisés, dans les **deux sens** (le contrôle reflète la donnée, et la saisie met la donnée à jour). C'est le mécanisme qui fait gagner le plus de temps dans une application de gestion, et il s'appuie sur les contrôles vus en [5.3](03-controles-fondamentaux.md) et [5.4](04-controles-avances.md). Toute la mécanique WinForms gravite autour d'une pièce centrale : le **`BindingSource`**.

---

## Les deux formes de liaison

- **Liaison simple** — relie **une** propriété de contrôle à **une** valeur de données. C'est le cas des contrôles scalaires (`TextBox`, `Label`, `CheckBox`), via la collection `DataBindings` :

```vb
' La propriété Text du champ suit la propriété Nom de la source ; idem pour la case à cocher
txtNom.DataBindings.Add("Text", bsClients, "Nom")
chkActif.DataBindings.Add("Checked", bsClients, "EstActif")
```

- **Liaison complexe** — relie un contrôle de **liste** à une **collection**, via `DataSource`/`DataMember` (et `DisplayMember`/`ValueMember` pour un `ComboBox` ou une `ListBox`). C'est le cas du `DataGridView`, déjà entrevu en [5.4](04-controles-avances.md).

---

## Le `BindingSource` : la pièce centrale ⭐

Le `BindingSource` s'intercale **entre les contrôles et la source réelle**. On lie les contrôles au `BindingSource`, et le `BindingSource` à la donnée (une `BindingList`, un `DataTable`, une vue EF Core…). Cette indirection apporte beaucoup :

- la **gestion de la position courante** (*currency*) : `Position`, `Current` ;
- la **synchronisation de plusieurs contrôles** sur la même source (sélectionner une ligne dans la grille met à jour les champs liés) ;
- la **navigation** (`MoveNext`, `MovePrevious`…), exploitée par le `BindingNavigator` ;
- le **tri et le filtrage** (`Sort`, `Filter`) quand la source les prend en charge ;
- des opérations **CRUD** : `AddNew`, `RemoveCurrent`, `EndEdit`, `CancelEdit` ;
- des **événements** : `CurrentChanged`, `PositionChanged`, `ListChanged`.

```vb
Private bsClients As New BindingSource()

Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    bsClients.DataSource = New BindingList(Of Client)(listeClients)

    ' Grille et champs partagent la MÊME source : ils restent synchronisés
    dgvClients.DataSource = bsClients
    txtNom.DataBindings.Add("Text", bsClients, "Nom")
    txtEmail.DataBindings.Add("Text", bsClients, "Email")

    navClients.BindingSource = bsClients   ' le BindingNavigator (barre de navigation)
End Sub
```

> 💡 Lier les contrôles au `BindingSource` plutôt qu'à la liste brute permet de **changer la source sous-jacente** sans recâbler les contrôles — un atout pour rafraîchir ou recharger les données.

---

## La bonne collection : `BindingList(Of T)` et `INotifyPropertyChanged`

Le choix de la collection conditionne le rafraîchissement automatique de l'interface.

- Une **`List(Of T)`** ne **notifie pas** les ajouts/suppressions : si vous ajoutez un élément par code, la grille **ne se met pas à jour**.
- Une **`BindingList(Of T)`** déclenche `ListChanged` à chaque ajout/suppression : l'interface **suit** automatiquement.

Reste le cas des **modifications de propriété** : pour qu'éditer un objet par code se répercute à l'écran, le type des éléments doit implémenter **`INotifyPropertyChanged`** et lever `PropertyChanged` à chaque changement.

```vb
Imports System.ComponentModel

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
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Nom)))
            End If
        End Set
    End Property

End Class
```

La pile recommandée est donc : **`BindingSource` → `BindingList(Of T)` → éléments implémentant `INotifyPropertyChanged`**.

> 🔗 C'est exactement l'interface `INotifyPropertyChanged` utilisée en WPF/MVVM (sections [6.4](../06-wpf/04-data-binding.md) et [6.6](../06-wpf/06-mvvm.md)) — une compétence transférable. Pour réduire le code répétitif, factorisez `PropertyChanged` dans une **classe de base** dotée d'une méthode d'aide (éventuellement avec `<CallerMemberName>`). Rappel (section [6.6](../06-wpf/06-mvvm.md)) : les *source generators* de `CommunityToolkit.Mvvm` étant **C# uniquement**, en VB on écrit ce socle à la main.

---

## Maître-détail et `BindingNavigator`

Pour un affichage **maître-détail** (les commandes du client sélectionné, par exemple), on chaîne deux `BindingSource` : le détail prend pour `DataSource` le `BindingSource` **maître**, et pour `DataMember` le nom de la **propriété de navigation**.

```vb
bsCommandes.DataSource = bsClients
bsCommandes.DataMember = NameOf(Client.Commandes)   ' propriété de navigation

dgvCommandes.DataSource = bsCommandes   ' n'affiche que les commandes du client courant
```

Le **`BindingNavigator`** est la barre d'outils de navigation (premier/précédent/suivant/dernier, ajout, suppression, position) ; il suffit de lui affecter le `BindingSource` pour câbler ses boutons.

---

## Lier à une base de données

### ADO.NET (l'approche classique)

Un **`DataTable`** est **directement liable** : il expose l'interface attendue par le binding (via sa `DataView`), avec tri et filtrage gratuits.

```vb
dgvProduits.DataSource = monDataTable
monDataTable.DefaultView.Sort = "Nom ASC"
monDataTable.DefaultView.RowFilter = "Prix > 100"
```

C'est l'approche éprouvée des applications historiques (module [7.1](../07-acces-donnees/01-adonet.md)).

### Entity Framework Core (l'approche moderne) ⭐

Avec EF Core (module [7.2](../07-acces-donnees/02-ef-core-10.md)), la liaison passe par la **vue locale** du `DbContext`. La marche à suivre : charger les entités dans le contexte avec `Load`, puis lier `Local.ToBindingList()`. Cette méthode expose les données sous forme d'`IBindingList` que Windows Forms sait consommer, et la vue locale **reste synchronisée** avec le contexte — les entités ajoutées y figurent, les supprimées en sont exclues, ce qui reflète l'état attendu de la base après `SaveChanges`.

```vb
Imports Microsoft.EntityFrameworkCore   ' apporte Load() et ToBindingList()

Private _contexte As AppDbContext
Private bsClients As New BindingSource()

Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    _contexte = New AppDbContext()

    _contexte.Clients.Load()   ' charge les entités dans le contexte
    bsClients.DataSource = _contexte.Clients.Local.ToBindingList()
    dgvClients.DataSource = bsClients
End Sub

Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click
    bsClients.EndEdit()       ' valide l'édition en cours
    _contexte.SaveChanges()   ' persiste ajouts, suppressions et modifications
End Sub

Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
    _contexte.Dispose()       ' libère les ressources de la base (cf. cycle de vie, 5.6)
End Sub
```

Les ajouts et suppressions effectués dans la grille sont alors **suivis par le contexte**, et `SaveChanges` les écrit en base.

> ⚠️ **Ne liez pas directement `DbSet.Local`** : cette vue n'offre pas d'ordre stable. Pour Windows Forms, liez toujours le résultat de **`ToBindingList()`** (pour WPF, ce serait `ToObservableCollection()`).

---

## Liaison et validation

La liaison de données et la validation (section [5.7](07-validation.md)) se complètent naturellement : les contrôles liés affichent leurs erreurs via l'`ErrorProvider`, le `DataGridView` intercepte les siennes via `DataError` (section [5.4](04-controles-avances.md)), et l'on valide le modèle (ses `DataAnnotations`) **avant** d'appeler `SaveChanges`.

---

## En résumé

La liaison de données WinForms s'organise autour du **`BindingSource`**, intermédiaire qui apporte position courante, synchronisation, navigation et opérations CRUD. Pour que l'interface se rafraîchisse, on s'appuie sur une **`BindingList(Of T)`** (ajouts/suppressions) dont les éléments implémentent **`INotifyPropertyChanged`** (modifications de propriété). Le schéma **maître-détail** chaîne deux `BindingSource`, et le **`BindingNavigator`** offre la navigation clé en main. Côté base, un **`DataTable`** se lie directement (ADO.NET), tandis qu'EF Core se lie via **`Local.ToBindingList()`**, synchronisé avec le `DbContext`.

La section suivante élargit la perspective à l'échelle de l'application, avec les fenêtres **MDI** et la gestion multi-formulaires → [5.9 Applications MDI et multi-formulaires](09-mdi.md).

⏭️ [Applications MDI et multi-formulaires](/05-windows-forms/09-mdi.md)
