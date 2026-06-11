🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.3 Contrôles fondamentaux

Après l'architecture (section [5.1](01-introduction-designer.md)) et les apports de .NET 10 (section [5.2](02-winforms-net10.md)), place aux **briques de base** de toute interface : le formulaire lui-même, les deux contrôles archétypaux que sont le bouton et la zone de texte, les conteneurs qui organisent la mise en page, et les boîtes de dialogue.

---

## Le socle commun : la classe `Control`

Avant de détailler chaque contrôle, un constat qui fait gagner beaucoup de temps : **tous** les contrôles Windows Forms héritent de `System.Windows.Forms.Control`. Ils partagent donc un large socle de propriétés et d'événements. En apprendre un, c'est en apprendre une partie de tous les autres.

Propriétés communes les plus utiles :

- **`Name`** — l'identifiant du contrôle dans le code (préfixé par convention : `btn`, `txt`, `lbl`…) ;
- **`Text`** — le texte affiché (titre du bouton, contenu d'une zone de texte, libellé…) ;
- **`Location`, `Size`, `Bounds`** — position et dimensions ;
- **`Anchor`, `Dock`** — comportement à la mise en page (voir plus bas) ;
- **`Enabled`, `Visible`** — activation et visibilité ;
- **`Font`, `ForeColor`, `BackColor`** — apparence ;
- **`TabIndex`, `TabStop`** — ordre et participation à la navigation au clavier ;
- **`Margin`, `Padding`** — espacement externe et interne ;
- **`Tag`** — un emplacement libre pour ranger une donnée métier associée au contrôle.

Côté événements, on retrouve partout `Click`, `MouseEnter`/`MouseLeave`, `GotFocus`/`LostFocus`, `KeyDown`/`KeyPress`/`KeyUp`. Leur gestion fine fait l'objet de la section [5.6](06-evenements.md).

---

## Le formulaire (`Form`)

Le formulaire est lui-même un `Control` : c'est la fenêtre de premier niveau qui héberge les autres contrôles. Au-delà des propriétés communes, il en expose qui lui sont propres :

- **`Text`** — le titre affiché dans la barre de la fenêtre ;
- **`StartPosition`** — la position d'apparition (`CenterScreen`, `CenterParent`, `Manual`…) ;
- **`FormBorderStyle`** — le style de bordure (redimensionnable, fixe, outil…) ;
- **`WindowState`** — état initial (`Normal`, `Minimized`, `Maximized`) ;
- **`MinimumSize`, `MaximumSize`** — bornes de redimensionnement ;
- **`ControlBox`, `MinimizeBox`, `MaximizeBox`** — présence des boutons système ;
- **`Icon`, `TopMost`, `Opacity`** — icône, fenêtre toujours au premier plan, transparence ;
- **`AcceptButton`, `CancelButton`** — bouton déclenché par **Entrée** et par **Échap** (essentiel pour les dialogues).

Le **cycle de vie** s'observe via les événements `Load` (avant l'affichage), `Shown` (après le premier affichage), `Activated`/`Deactivate`, `FormClosing` (annulable : on peut empêcher la fermeture) et `FormClosed`. On les approfondit en [5.6](06-evenements.md).

Pour afficher ou fermer un formulaire : `Show` (non modal), `ShowDialog` (modal), `Hide`, `Close`. Les variantes asynchrones `ShowAsync`/`ShowDialogAsync` ont été présentées en [5.2](02-winforms-net10.md).

---

## Le bouton (`Button`)

Le bouton est le contrôle d'action par excellence. Ses points clés :

- **`Text`** — le libellé ; un **`&`** définit un *mnémonique* : `"&Valider"` rend le bouton activable par **Alt+V** ;
- **`Enabled`** — pour griser le bouton tant qu'une action n'est pas possible ;
- **`DialogResult`** — dans une boîte de dialogue, la valeur que prend `Form.DialogResult` quand on clique le bouton (voir plus bas) ;
- **`FlatStyle`** — l'apparence ; rappelons (section [5.2](02-winforms-net10.md)) qu'en **mode sombre**, `FlatStyle.System` rend mieux que la valeur par défaut ;
- **`Image`, `ImageAlign`, `TextImageRelation`** — pour un bouton illustré.

L'événement central est `Click` :

```vb
Private Sub btnValider_Click(sender As Object, e As EventArgs) Handles btnValider.Click
    If String.IsNullOrWhiteSpace(txtNom.Text) Then
        MessageBox.Show("Le nom est obligatoire.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Return
    End If

    ' ... traitement ...
End Sub
```

---

## Les champs de saisie (`TextBox` et ses voisins)

### La zone de texte

`TextBox` est le contrôle de saisie de référence. Ses propriétés les plus courantes :

- **`Text`** — le contenu saisi ;
- **`Multiline`** (+ **`ScrollBars`**, **`WordWrap`**) — pour un éditeur multiligne ;
- **`PasswordChar`** ou **`UseSystemPasswordChar`** — masquage des mots de passe ;
- **`MaxLength`** — longueur maximale ;
- **`ReadOnly`** — lecture seule ;
- **`CharacterCasing`** — forçage majuscules/minuscules ;
- **`PlaceholderText`** — texte d'invite affiché quand le champ est vide.

L'événement le plus utilisé est `TextChanged`, déclenché à chaque modification — pratique pour un filtrage en direct :

```vb
Private Sub txtRecherche_TextChanged(sender As Object, e As EventArgs) Handles txtRecherche.TextChanged
    FiltrerListe(txtRecherche.Text)
End Sub
```

Le filtrage des frappes (`KeyPress`) et la validation (`Validating`, `ErrorProvider`) sont traités respectivement en [5.6](06-evenements.md) et [5.7](07-validation.md).

### Le libellé et les autres contrôles de saisie courants

Le **`Label`** accompagne presque toujours une zone de saisie. Au-delà du couple `Label`/`TextBox`, la palette fondamentale comprend :

- **`CheckBox`** — case à cocher (`Checked`, événement `CheckedChanged`) ;
- **`RadioButton`** — bouton d'option ; les boutons d'un même conteneur (souvent un `GroupBox`) forment un groupe mutuellement exclusif ;
- **`ComboBox`** — liste déroulante (`Items`, `SelectedItem`/`SelectedIndex`, `DropDownStyle` pour autoriser ou non la saisie libre) ;
- **`ListBox`** — liste de sélection ;
- **`NumericUpDown`** — saisie numérique bornée (`Minimum`, `Maximum`, `Value`) ;
- **`DateTimePicker`** — sélection de date/heure ;
- **`MaskedTextBox`** — saisie guidée par un masque (téléphone, code postal…).

Les contrôles plus riches (`DataGridView`, `TreeView`, `ListView`, menus et barres d'outils) relèvent des **contrôles avancés**, en [5.4](04-controles-avances.md).

---

## Les conteneurs et la mise en page

Les conteneurs servent à **regrouper** des contrôles et à bâtir une interface qui **s'adapte** au redimensionnement et à la résolution (DPI). Deux mécanismes de disposition sont à maîtriser.

### `Anchor` et `Dock`

- **`Anchor`** « épingle » un contrôle à un ou plusieurs bords de son conteneur : le contrôle conserve sa distance à ces bords lors du redimensionnement. Les valeurs se combinent avec `Or`.
- **`Dock`** colle un contrôle à un bord (`Top`, `Bottom`, `Left`, `Right`) ou lui fait remplir l'espace restant (`Fill`).

```vb
' Reste collé au coin inférieur droit lors du redimensionnement
btnFermer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right

' Barre d'outils accolée en haut, sur toute la largeur
toolStrip.Dock = DockStyle.Top
```

### Les conteneurs disponibles

- **`Panel`** — conteneur générique, défilable via `AutoScroll` ;
- **`GroupBox`** — cadre titré, idéal pour grouper des `RadioButton` ;
- **`TableLayoutPanel`** — disposition en grille, avec lignes/colonnes en taille absolue, en pourcentage ou automatique ; le plus robuste pour des formulaires qui se redimensionnent proprement ;
- **`FlowLayoutPanel`** — agencement « au fil de l'eau », horizontal ou vertical ;
- **`SplitContainer`** — deux zones séparées par un *splitter* déplaçable ;
- **`TabControl`** / **`TabPage`** — interface à onglets.

> 💡 **Bonne pratique.** Préférez `TableLayoutPanel`/`FlowLayoutPanel` combinés à `Anchor`/`Dock` au positionnement absolu en pixels. Vos formulaires resteront corrects après redimensionnement et sur des écrans à forte densité — un atout qui prolonge le travail sur le DPI évoqué en [5.2](02-winforms-net10.md).

---

## Les boîtes de dialogue

### `MessageBox`

La boîte de message standard affiche un texte, des boutons et une icône, et **renvoie un `DialogResult`** indiquant le choix de l'utilisateur :

```vb
Dim reponse As DialogResult =
    MessageBox.Show("Enregistrer les modifications avant de fermer ?",
                    "Confirmation",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question)

Select Case reponse
    Case DialogResult.Yes
        Enregistrer()
    Case DialogResult.No
        ' fermer sans enregistrer
    Case DialogResult.Cancel
        ' annuler la fermeture
End Select
```

### Les boîtes de dialogue communes

Windows Forms fournit des dialogues système prêts à l'emploi, tous ouverts par `ShowDialog` (qui renvoie `DialogResult`). Comme ce sont des objets jetables, on les enveloppe dans un bloc `Using` :

```vb
Using dlg As New OpenFileDialog()
    dlg.Title = "Choisir un fichier CSV"
    dlg.Filter = "Fichiers CSV (*.csv)|*.csv|Tous les fichiers (*.*)|*.*"
    dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

    If dlg.ShowDialog(Me) = DialogResult.OK Then
        Dim chemin As String = dlg.FileName
        ' ... charger le fichier ...
    End If
End Using
```

Les principaux : **`OpenFileDialog`** et **`SaveFileDialog`** (propriétés `Filter`, `FileName`/`FileNames`, `InitialDirectory`, `Multiselect` pour l'ouverture), **`FolderBrowserDialog`**, **`ColorDialog`** et **`FontDialog`**.

> 🆕 Depuis .NET 9, le `FolderBrowserDialog` accepte la **sélection multiple** : activez `Multiselect = True` et lisez la propriété `SelectedPaths`.

### Créer sa propre boîte de dialogue modale

Un dialogue personnalisé est simplement un `Form` affiché par `ShowDialog`. Le mécanisme tient en quelques règles :

1. les boutons de validation/annulation portent un **`DialogResult`** (`btnOk.DialogResult = DialogResult.OK`, `btnAnnuler.DialogResult = DialogResult.Cancel`) — réglé dans le concepteur ou en code ;
2. le formulaire définit **`AcceptButton`** et **`CancelButton`** pour activer **Entrée** et **Échap** ;
3. la donnée saisie est **exposée en propriété** publique, lue par l'appelant après fermeture ;
4. comme un formulaire modal est *masqué* et non détruit à la fermeture, on l'enveloppe dans un `Using` pour garantir sa libération (le pattern utilisé en section [5.2](02-winforms-net10.md)).

> ⚠️ **Piège .NET 9+ vérifié sur .NET 10 : l'erreur `WFO1000`.** Depuis .NET 9, un analyseur exige que toute
> propriété **publique** d'un formulaire ou d'un contrôle déclare son comportement de **sérialisation pour le
> Concepteur** — sans quoi la compilation échoue (`WFO1000`). Pour une propriété de transfert de données comme
> celle de notre dialogue, la réponse est : « ne pas sérialiser », via l'attribut dédié :
>
> ```vb
> Imports System.ComponentModel
>
> Public Class ClientEditForm
>
>     <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
>     Public Property Client As Client
>
> End Class
> ```

```vb
' Côté appelant — réutilise le formulaire ClientEditForm vu plus tôt
Using dlg As New ClientEditForm()
    dlg.Client = New Client With {.Nom = "", .Ville = ""}

    If dlg.ShowDialog(Me) = DialogResult.OK Then
        Dim saisi As Client = dlg.Client
        ' ... utiliser la donnée saisie ...
    End If
End Using
```

Pour une version asynchrone de ce schéma, on substitue `Await dlg.ShowDialogAsync(Me)` à `dlg.ShowDialog(Me)` (section [5.2](02-winforms-net10.md)). La validation des saisies à l'intérieur du dialogue est traitée en [5.7](07-validation.md).

---

## En résumé

Tous les contrôles partagent le socle de la classe `Control` (nom, position, ancrage, activation, événements communs). Le **formulaire** est la fenêtre hôte, dotée de propriétés de cycle de vie et des précieux `AcceptButton`/`CancelButton`. Le **bouton** (événement `Click`, `DialogResult`) et la **zone de texte** (`Text`, `TextChanged`, masquage, `PlaceholderText`) sont les deux contrôles à connaître en premier, entourés de leurs voisins de saisie. Les **conteneurs** (`TableLayoutPanel`, `FlowLayoutPanel`, `GroupBox`…) associés à `Anchor`/`Dock` assurent une mise en page adaptative. Enfin, les **boîtes de dialogue** — `MessageBox`, dialogues communs, ou formulaires modaux personnalisés — reposent toutes sur le couple `ShowDialog` / `DialogResult`.

La suite passe aux **contrôles avancés** : grilles de données, arbres, listes, menus et barres d'outils → [5.4 Contrôles avancés](04-controles-avances.md).

⏭️ [Contrôles avancés (DataGridView, TreeView/ListView, MenuStrip/ToolStrip/StatusStrip)](/05-windows-forms/04-controles-avances.md)
