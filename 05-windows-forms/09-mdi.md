🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.9 Applications MDI et multi-formulaires

Une application réelle ne se limite presque jamais à une seule fenêtre. Deux sujets liés se posent alors : **coordonner plusieurs formulaires** (les ouvrir, les faire communiquer, gérer leurs relations) et, plus spécifiquement, le modèle **MDI** où une fenêtre parente héberge des fenêtres « document ». Cette section s'appuie sur les formulaires (section [5.3](03-controles-fondamentaux.md)), les événements et le cycle de vie (section [5.6](06-evenements.md)).

---

## Modal, non modal, propriétaire : trois relations entre formulaires

- **Modal** (`ShowDialog`) — bloque l'interaction avec le reste de l'application jusqu'à sa fermeture, et renvoie un `DialogResult` (section [5.3](03-controles-fondamentaux.md)). Sa variante asynchrone est `ShowDialogAsync` (section [5.2](02-winforms-net10.md)).
- **Non modal** (`Show`) — fenêtre **indépendante** : l'application continue de fonctionner en parallèle.
- **Propriétaire** (`Show(owner)` ou propriété `Owner`) — la fenêtre possédée reste **au-dessus** de son propriétaire, et se réduit ou se ferme **avec lui**.

```vb
' Modal : on attend la décision de l'utilisateur
Using dlg As New ClientEditForm()
    If dlg.ShowDialog(Me) = DialogResult.OK Then
        ' ...
    End If
End Using

' Non modal : fenêtre autonome
Dim fenetre As New RapportForm()
fenetre.Show()

' Propriétaire : Me devient le propriétaire de la palette
Dim palette As New OutilsForm()
palette.Show(Me)
```

---

## Faire communiquer des formulaires

Le bon couplage entre fenêtres évite bien des bugs. Trois cas, du plus simple au plus découplé.

**Transmettre des données à l'ouverture.** Par le constructeur ou par des propriétés publiques du formulaire ouvert.

**Récupérer des données d'un dialogue modal.** On lit le `DialogResult` puis les propriétés publiques du dialogue, après sa fermeture (section [5.3](03-controles-fondamentaux.md)).

**Réagir aux actions d'un formulaire non modal.** Comme le dialogue ne « rend pas la main », il **expose un événement** auquel le parent s'abonne — l'idiome `Event`/`RaiseEvent` du module [3.6](../03-poo/06-evenements-delegues.md), avec un `EventArgs` dédié (comme en section [5.5](05-controles-personnalises.md)) :

```vb
' Le formulaire enfant expose un événement, sans toucher au parent
Public Class FiltreForm
    Public Event FiltreApplique As EventHandler(Of FiltreEventArgs)

    Private Sub btnAppliquer_Click(sender As Object, e As EventArgs) Handles btnAppliquer.Click
        RaiseEvent FiltreApplique(Me, New FiltreEventArgs(txtTerme.Text))
    End Sub
End Class

' Côté parent : on s'abonne à l'événement de l'enfant
Private Sub OuvrirFiltre()
    Dim f As New FiltreForm()
    AddHandler f.FiltreApplique, AddressOf SurFiltreApplique
    f.Show(Me)
End Sub

Private Sub SurFiltreApplique(sender As Object, e As FiltreEventArgs)
    AppliquerFiltre(e.Terme)
End Sub
```

> ⚠️ **Évitez le couplage serré.** Un formulaire qui va lire ou modifier directement les contrôles d'un autre (`parent.txtRecherche.Text = …`) crée une dépendance fragile. Préférez les **propriétés**, les **événements** ou un **service partagé** injecté dans les deux formulaires (injection de dépendances via le Generic Host, module [4.8](../04-async/08-background-services.md)).

---

## Le modèle MDI (Multiple Document Interface)

Le **MDI** fait cohabiter, dans une fenêtre parente unique, plusieurs fenêtres **enfants** (les « documents »), à la manière des anciennes suites bureautiques. Il s'oppose au **SDI** (*Single Document Interface*), où chaque document a sa propre fenêtre de premier niveau.

### Mise en place

- Le formulaire parent est le **conteneur** : `IsMdiContainer = True`.
- Chaque enfant se rattache au conteneur via `MdiParent`, puis s'affiche.

```vb
Public Class MainForm
    ' IsMdiContainer = True (réglé dans le concepteur ou par code)

    Private Sub mnuNouveau_Click(sender As Object, e As EventArgs) Handles mnuNouveau.Click
        Dim doc As New DocumentForm()
        doc.MdiParent = Me      ' rattache l'enfant au conteneur
        doc.Show()
    End Sub

    ' Disposition automatique des fenêtres enfants
    Private Sub mnuCascade_Click(sender As Object, e As EventArgs) Handles mnuCascade.Click
        LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub mnuMosaiqueH_Click(sender As Object, e As EventArgs) Handles mnuMosaiqueH.Click
        LayoutMdi(MdiLayout.TileHorizontal)
    End Sub
End Class
```

### Les outils du conteneur

- **`LayoutMdi`** dispose les enfants (`Cascade`, `TileHorizontal`, `TileVertical`, `ArrangeIcons`).
- Le conteneur expose **`ActiveMdiChild`** (la fenêtre active) et **`MdiChildren`** (toutes les fenêtres enfants), et déclenche **`MdiChildActivate`** à chaque changement.
- Le **menu Fenêtre** se peuple tout seul : sur le `MenuStrip`, affectez `MdiWindowListItem` à l'élément qui doit lister les fenêtres ouvertes.
- Les menus des enfants peuvent **fusionner** dans la barre du conteneur (`MenuStrip.AllowMerge`, propriété `MergeAction` des éléments) — la fusion de menus classique du MDI.

---

## MDI ou alternatives modernes ?

Le MDI **reste pris en charge** sur .NET 10 et convient à certaines applications de gestion, mais c'est un paradigme d'interface **daté**. Pour une application neuve, pesez-le face à des approches plus actuelles :

- une **interface à onglets** (un `TabControl` hébergeant des `UserControl`, section [5.5](05-controles-personnalises.md)) ;
- une **fenêtre unique avec navigation** ;
- un **framework d'ancrage** tiers (*docking*).

> 📌 **Note technique.** Un `TabControl` héberge des **contrôles**, pas des formulaires : les « documents » deviennent alors des `UserControl` ou des panneaux, et non des fenêtres enfants. Pour maintenir une application MDI existante, en revanche, le support est solide et il n'y a aucune urgence à réécrire.

---

## Démarrage et arrêt d'une application multi-formulaires

Dès qu'il y a plusieurs fenêtres, le **mode d'arrêt** devient décisif : l'application se ferme-t-elle quand le **formulaire de démarrage** se ferme, ou seulement quand le **dernier formulaire** ouvert se ferme ? Ce comportement se règle via le **Framework d'application** (onglet *Application*, section [5.1](01-introduction-designer.md)) — l'un des conforts propres à VB.NET.

---

## En résumé

Coordonner plusieurs formulaires repose d'abord sur trois relations bien choisies — **modal** (`ShowDialog`), **non modal** (`Show`) et **propriétaire** (`Show(owner)`) — et sur une communication **découplée** (propriétés, événements, service partagé) plutôt que par accès direct aux contrôles. Le modèle **MDI** (`IsMdiContainer`, `MdiParent`, `LayoutMdi`, menu Fenêtre via `MdiWindowListItem`) reste disponible, mais mérite d'être comparé, pour une application neuve, à une interface à onglets ou SDI. Enfin, le **mode d'arrêt** du Framework d'application détermine quand l'application se termine.

La section suivante aborde la persistance des préférences et des paramètres utilisateur → [5.10 Préférences et paramètres utilisateur (`My.Settings`)](10-preferences.md).

⏭️ [Préférences et paramètres utilisateur (My.Settings)](/05-windows-forms/10-preferences.md)
