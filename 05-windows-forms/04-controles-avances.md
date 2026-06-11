🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.4 Contrôles avancés

Les contrôles fondamentaux (section [5.3](03-controles-fondamentaux.md)) suffisent pour des écrans simples. Les applications réelles, en particulier les applications de gestion (*LOB*), réclament des contrôles plus riches : une **grille de données** pour afficher et éditer des tableaux, des vues **hiérarchiques et en liste**, et la famille **menu / barre d'outils / barre d'état** qui structure la fenêtre.

---

## La grille de données (`DataGridView`)

`DataGridView` est le cheval de bataille des applications de données : il affiche un tableau de lignes et de colonnes, éditable et liable à une source de données.

### Liaison aux données

La grille se remplit en affectant sa propriété **`DataSource`** — typiquement un `BindingSource`, une `List(Of T)`, une `BindingList(Of T)` ou un `DataTable`. Avec **`AutoGenerateColumns = True`** (défaut), les colonnes sont créées automatiquement d'après la source ; sinon, on définit ses colonnes et on relie chacune à une propriété via **`DataPropertyName`**.

```vb
Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    dgvClients.AutoGenerateColumns = True
    dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
    dgvClients.AllowUserToAddRows = False

    ' Liaison à une liste d'objets métier (le binding complet est traité en 5.8)
    dgvClients.DataSource = New BindingList(Of Client)(listeClients)
End Sub
```

Les colonnes peuvent être de plusieurs types : `DataGridViewTextBoxColumn`, `DataGridViewCheckBoxColumn`, `DataGridViewComboBoxColumn`, `DataGridViewButtonColumn`, `DataGridViewImageColumn`, `DataGridViewLinkColumn`.

### Affichage et comportement

Quelques propriétés à connaître : **`SelectionMode`** (sélection par cellule, par ligne…), **`ReadOnly`**, **`AllowUserToAddRows`**/**`AllowUserToDeleteRows`**, **`AutoSizeColumnsMode`** (ajustement automatique des largeurs), et **`Frozen`** au niveau d'une colonne pour la figer lors du défilement horizontal.

Le **formatage** passe soit par `DefaultCellStyle.Format` (chaînes de format .NET, par colonne), soit par l'événement `CellFormatting` pour une mise en forme conditionnelle.

### Événements à maîtriser

```vb
' Réagir à une cellule éditée
Private Sub dgvClients_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) _
        Handles dgvClients.CellValueChanged
    If e.RowIndex >= 0 Then
        ' ... marquer l'enregistrement comme modifié ...
    End If
End Sub

' Toujours intercepter les erreurs de données
Private Sub dgvClients_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) _
        Handles dgvClients.DataError
    MessageBox.Show($"Saisie invalide : {e.Exception.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
    e.ThrowException = False   ' évite la boîte d'erreur par défaut
End Sub
```

> ⚠️ **N'ignorez jamais l'événement `DataError`.** Sans gestionnaire, une simple erreur de conversion (texte saisi dans une colonne numérique, par exemple) déclenche une boîte d'erreur technique. Le gérer permet d'afficher un message clair et de garder la main.

> 💡 **Grandes volumétries.** Ne chargez pas des centaines de milliers de lignes en mémoire. Préférez la pagination (côté base, voir module [7](../07-acces-donnees/README.md)) ou le **mode virtuel** (`VirtualMode`), où la grille demande les données à la volée — voir les considérations du module [14](../14-performance/README.md).

---

## Les vues hiérarchiques et en liste

### L'arbre (`TreeView`)

`TreeView` affiche des données **hiérarchiques** (dossiers, catégories, organigrammes). Tout repose sur la collection **`Nodes`** de `TreeNode` ; chaque nœud a son propre `Nodes` pour ses enfants. La propriété `Tag` permet d'associer un objet métier à chaque nœud.

```vb
Private Sub ChargerArbre()
    tvDossiers.Nodes.Clear()

    Dim racine As TreeNode = tvDossiers.Nodes.Add("Projet")
    racine.Tag = projet

    For Each dossier In projet.Dossiers
        Dim noeud As TreeNode = racine.Nodes.Add(dossier.Nom)
        noeud.Tag = dossier
    Next

    racine.Expand()
End Sub

Private Sub tvDossiers_AfterSelect(sender As Object, e As TreeViewEventArgs) _
        Handles tvDossiers.AfterSelect
    AfficherDetails(e.Node.Tag)   ' l'objet métier rangé dans Tag
End Sub
```

Autres éléments utiles : la propriété **`CheckBoxes`** (cases à cocher par nœud), un **`ImageList`** pour les icônes (`ImageIndex`/`SelectedImageIndex`), et l'événement **`BeforeExpand`** pour un **chargement paresseux** des enfants d'un gros arbre (on ne peuple une branche qu'au moment où l'utilisateur l'ouvre).

### La liste (`ListView`)

`ListView` affiche une collection d'**éléments**, avec plusieurs modes d'affichage pilotés par **`View`** : `Details` (colonnes, le plus courant), `LargeIcon`, `SmallIcon`, `List`, `Tile`. En mode `Details`, on déclare des colonnes (`Columns`) puis des éléments (`ListViewItem`) dont les valeurs supplémentaires sont des **`SubItems`**.

```vb
Private Sub ChargerListe()
    lvClients.View = View.Details
    lvClients.FullRowSelect = True
    lvClients.GridLines = True

    lvClients.Columns.Clear()
    lvClients.Columns.Add("Nom", 200)
    lvClients.Columns.Add("Ville", 150)

    lvClients.Items.Clear()
    For Each c In listeClients
        Dim item As New ListViewItem(c.Nom)
        item.SubItems.Add(c.Ville)
        item.Tag = c
        lvClients.Items.Add(item)
    Next
End Sub
```

Propriétés et événements courants : **`MultiSelect`**, **`CheckBoxes`**, **`LargeImageList`**/**`SmallImageList`** ; **`SelectedIndexChanged`**, **`ItemActivate`** (double-clic / Entrée), **`ColumnClick`** (pour trier). Comme le `DataGridView`, `ListView` propose un **mode virtuel** (`VirtualMode` + `RetrieveVirtualItem`) pour les très grandes listes.

### `DataGridView`, `ListView` ou `TreeView` : lequel choisir ?

- **`DataGridView`** — données **tabulaires éditables** et **liées** à une source : c'est le choix par défaut des écrans de gestion.
- **`ListView`** — **affichage** d'éléments (souvent en lecture seule), avec icônes et plusieurs vues. Plus léger, mais **non lié** à une source par défaut : on le peuple à la main.
- **`TreeView`** — données **hiérarchiques** où la relation parent/enfant est l'information principale.

---

## Les menus et barres (la famille `ToolStrip`)

Un point unificateur, dans l'esprit du socle `Control` vu en [5.3](03-controles-fondamentaux.md) : **`MenuStrip`**, **`ToolStrip`**, **`StatusStrip`** et **`ContextMenuStrip`** dérivent tous de `ToolStrip` et hébergent des éléments dérivés de `ToolStripItem`. Apprendre l'un éclaire les autres.

> 🆕 **Confort .NET 10.** Plusieurs éditeurs du concepteur hérités de .NET Framework — dont le `ToolStripCollectionEditor` et des éditeurs liés au `DataGridView` — ont été reportés et sont à nouveau accessibles depuis la fenêtre Propriétés et le panneau d'actions du concepteur. L'édition visuelle des menus, barres et colonnes en est facilitée.

### `MenuStrip` (et `ContextMenuStrip`)

`MenuStrip` est la barre de menus de la fenêtre ; ses éléments sont des `ToolStripMenuItem`, imbriquables pour former des sous-menus. Chaque élément expose un événement `Click`, un raccourci clavier via **`ShortcutKeys`**, et une bascule via **`Checked`**/**`CheckOnClick`**. Les **`ToolStripSeparator`** regroupent visuellement les commandes.

```vb
' Raccourci Ctrl+S (réglable aussi dans le concepteur)
mnuEnregistrer.ShortcutKeys = Keys.Control Or Keys.S

Private Sub mnuEnregistrer_Click(sender As Object, e As EventArgs) Handles mnuEnregistrer.Click
    Enregistrer()
End Sub
```

Le **`ContextMenuStrip`** est un menu contextuel (clic droit) que l'on rattache à n'importe quel contrôle via sa propriété `ContextMenuStrip`.

### `ToolStrip`

`ToolStrip` est la barre d'outils. Elle accueille des `ToolStripButton`, `ToolStripLabel`, `ToolStripDropDownButton`, `ToolStripSplitButton`, `ToolStripComboBox`, `ToolStripTextBox`… La propriété **`DisplayStyle`** d'un bouton choisit entre image seule, texte seul ou les deux. Un `ToolStripContainer` permet d'accueillir des barres déplaçables sur les quatre bords de la fenêtre.

### `StatusStrip`

`StatusStrip` est la barre d'état, en bas de la fenêtre. Elle héberge surtout des `ToolStripStatusLabel` et `ToolStripProgressBar`. La propriété **`Spring`** d'un libellé lui fait occuper tout l'espace disponible, poussant les autres éléments vers la droite.

```vb
' Mise à jour de l'état et de la progression
lblStatut.Text = "Chargement…"
pbProgression.Value = 50
```

---

## En résumé

Le **`DataGridView`** est l'outil central des écrans de données : liaison par `DataSource`, colonnes typées, formatage par `CellFormatting`, et gestion impérative de `DataError`. Le **`TreeView`** structure l'information hiérarchique (collection `Nodes`, `Tag`, chargement paresseux), tandis que le **`ListView`** affiche des éléments selon plusieurs vues (`Details`, icônes…). Enfin, la famille **`ToolStrip`** — `MenuStrip`, `ToolStrip`, `StatusStrip`, `ContextMenuStrip` — partage un même modèle d'éléments et compose l'ossature de l'application (menus, raccourcis, barre d'outils, barre d'état).

La section suivante montre comment **fabriquer ses propres contrôles** réutilisables lorsqu'aucun contrôle standard ne convient → [5.5 Contrôles personnalisés et `UserControl`](05-controles-personnalises.md).

⏭️ [Contrôles personnalisés et UserControl](/05-windows-forms/05-controles-personnalises.md)
