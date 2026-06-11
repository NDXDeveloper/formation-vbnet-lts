🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.6 Gestion des événements (souris, clavier, cycle de vie du formulaire)

Windows Forms est un *framework* **piloté par les événements** (section [5.1](01-introduction-designer.md)) : l'utilisateur agit, le système déclenche un événement, votre code y répond. Le **mécanisme de langage** (`Event`, `RaiseEvent`, `Handles`, `WithEvents`, `AddHandler`) a été détaillé au module [3.6](../03-poo/06-evenements-delegues.md). Cette section l'applique concrètement aux trois familles qui reviennent sans cesse : la **souris**, le **clavier** et le **cycle de vie du formulaire**.

---

## Rappel : s'abonner à un événement

Deux mécanismes coexistent en VB.NET.

**`Handles` + `WithEvents`** — l'approche déclarative, celle que génère le Concepteur. Le contrôle est déclaré `WithEvents` dans le fichier `*.Designer.vb`, et le gestionnaire s'y relie par une clause `Handles`. C'est l'idiome VB par excellence ; un même gestionnaire peut d'ailleurs traiter **plusieurs** événements :

```vb
Private Sub btnValider_Click(sender As Object, e As EventArgs) Handles btnValider.Click
    ' ...
End Sub

' Un seul gestionnaire pour trois boutons — un atout de VB.NET
Private Sub Couleur_Click(sender As Object, e As EventArgs) _
        Handles btnRouge.Click, btnVert.Click, btnBleu.Click
    Dim bouton = DirectCast(sender, Button)
    AppliquerCouleur(bouton.BackColor)
End Sub
```

L'argument **`sender`** désigne le contrôle émetteur (qu'on transtype au besoin), et **`e`** transporte les données de l'événement.

**`AddHandler` / `RemoveHandler`** — l'approche dynamique, indispensable pour un contrôle **créé à l'exécution**, ou lorsqu'on veut s'abonner et se désabonner pendant la vie de l'application :

```vb
Private Sub AjouterBouton()
    Dim btn As New Button With {.Text = "Dynamique"}
    AddHandler btn.Click, AddressOf Couleur_Click   ' réutilise le même gestionnaire
    Controls.Add(btn)
End Sub
```

> 💡 En clair : `Handles` pour les contrôles posés dans le Concepteur, `AddHandler` pour ceux que vous instanciez vous-même ou que vous abonnez de façon conditionnelle.

---

## Les événements souris

### Click et MouseClick — une distinction utile

`Click` et `DoubleClick` se déclenchent pour **toute activation** du contrôle, y compris au clavier (touche **Entrée** sur un bouton) ou par programme. `MouseClick` et `MouseDoubleClick` ne se déclenchent qu'au **clic réel de la souris**. Pour un bouton, `Click` est presque toujours le bon choix ; pour réagir spécifiquement à la souris (et connaître sa position ou le bouton pressé), on passe aux événements `MouseXxx`.

### MouseDown, MouseUp, MouseMove

Ces événements fournissent un **`MouseEventArgs`** riche : `Button` (quel bouton), `Location`/`X`/`Y` (position), `Clicks` (nombre de clics), `Delta` (molette). On y détecte facilement, par exemple, un clic droit pour afficher un menu contextuel :

```vb
Private Sub panneau_MouseDown(sender As Object, e As MouseEventArgs) Handles panneau.MouseDown
    Select Case e.Button
        Case MouseButtons.Left
            DemarrerSelection(e.Location)
        Case MouseButtons.Right
            menuContextuel.Show(panneau, e.Location)
    End Select
End Sub
```

### Survol et molette

`MouseEnter`, `MouseLeave` et `MouseHover` gèrent le survol (effets visuels, info-bulles), tandis que `MouseWheel` (via `e.Delta`) gère la molette.

> 📌 Le **glisser-déposer** s'appuie sur ces événements souris, mais constitue un mécanisme distinct (`DoDragDrop`, `DragEnter`, `DragDrop`). Il est abordé, avec l'échange de données moderne, en [5.12](12-nouveautes-net10.md).

---

## Les événements clavier

Deux familles, deux usages.

**`KeyDown` / `KeyUp`** travaillent au niveau des **touches physiques** et des modificateurs. Leur `KeyEventArgs` expose `KeyCode`, `KeyData`, `Modifiers` et les raccourcis `Control`/`Shift`/`Alt`, plus `Handled` et `SuppressKeyPress`. C'est le bon outil pour les **raccourcis**, les touches de fonction et les flèches.

**`KeyPress`** travaille au niveau du **caractère** produit (`KeyPressEventArgs.KeyChar`). C'est le bon outil pour **filtrer la saisie** de texte — exactement le cas du `NumericTextBox` construit en [5.5](05-controles-personnalises.md).

### Les raccourcis au niveau du formulaire

Par défaut, seul le contrôle ayant le focus reçoit les événements clavier. En passant **`KeyPreview = True`** sur le formulaire, celui-ci les voit **avant** le contrôle focalisé — idéal pour des raccourcis valables partout dans la fenêtre :

```vb
' Le formulaire a KeyPreview = True
Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
    If e.Control AndAlso e.KeyCode = Keys.S Then
        Enregistrer()
        e.Handled = True            ' la touche est traitée
        e.SuppressKeyPress = True   ' empêche aussi l'événement KeyPress qui suivrait
    End If
End Sub
```

`e.Handled` indique que l'événement est traité ; `e.SuppressKeyPress` va plus loin en empêchant la frappe d'atteindre le contrôle. Pour intercepter des touches de commande de façon encore plus robuste à l'échelle du formulaire, on peut redéfinir `ProcessCmdKey` (notion avancée).

---

## Réagir au temps : le composant `Timer`

Tous les événements ne viennent pas de l'utilisateur. Pour déclencher une action **à intervalle régulier** (horloge, rafraîchissement d'un état, masquage d'une notification), Windows Forms fournit le composant **`Timer`** : non visuel (il siège dans le bac à composants, section [5.5](05-controles-personnalises.md)), il lève son événement **`Tick`** toutes les `Interval` millisecondes — **sur le thread d'interface**, ce qui autorise à manipuler les contrôles directement, sans marshaling.

```vb
Private WithEvents tmrHorloge As New Timer()

Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    tmrHorloge.Interval = 1000   ' en millisecondes
    tmrHorloge.Start()           ' ou tmrHorloge.Enabled = True
End Sub

Private Sub tmrHorloge_Tick(sender As Object, e As EventArgs) Handles tmrHorloge.Tick
    lblHeure.Text = DateTime.Now.ToLongTimeString()
End Sub
```

> ⚠️ Ce `Timer` (celui de `System.Windows.Forms`) est fait pour **piloter l'interface**, pas pour la précision : son `Tick` passe par la boucle de messages, et un traitement long le retarde. Gardez le gestionnaire **court** ; pour un vrai travail périodique en arrière-plan, préférez l'asynchronisme du module [4](../04-async/README.md) — puis revenez au thread d'interface pour l'affichage.

---

## Le cycle de vie du formulaire

De sa création à sa fermeture, un formulaire traverse une séquence d'événements bien définie :

1. **`New`** (constructeur) — appelle `InitializeComponent` (section [5.1](01-introduction-designer.md)) ;
2. **`Load`** — déclenché **une fois**, avant le premier affichage : c'est ici qu'on **initialise les données** ;
3. **`Shown`** — déclenché **une fois**, après le premier affichage : pour les traitements qui exigent que la fenêtre soit déjà visible ;
4. **`Activated`** / **`Deactivate`** — à chaque fois que le formulaire **gagne** ou **perd** le focus ;
5. **`FormClosing`** — déclenché à la demande de fermeture, **annulable** (`e.Cancel = True`) ; `e.CloseReason` indique l'origine (action utilisateur, fermeture de l'application, arrêt de Windows…) ;
6. **`FormClosed`** — la fermeture est actée ;
7. **`Dispose`** — libération des ressources.

L'événement `FormClosing` est le bon endroit pour proposer un enregistrement et, le cas échéant, **empêcher** la fermeture :

```vb
Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
    If _modifieNonEnregistre Then
        Dim reponse As DialogResult =
            MessageBox.Show("Des modifications non enregistrées seront perdues. Fermer quand même ?",
                            "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If reponse = DialogResult.No Then
            e.Cancel = True   ' on annule la fermeture
        End If
    End If
End Sub
```

> 💡 **Où placer quoi ?** Les **données** dans `Load` (le formulaire existe, mais n'est pas encore peint) ; ce qui requiert une **fenêtre visible** dans `Shown` ; les **confirmations et l'annulation** dans `FormClosing` ; le **nettoyage final** dans `FormClosed`/`Dispose`.

---

## Pièges et bonnes pratiques

- **Ne bloquez pas le thread d'interface.** Un gestionnaire qui exécute une opération longue (réseau, fichier) fige l'application : basculez en `Async`/`Await` (module [4](../04-async/README.md) et section [5.2](02-winforms-net10.md)).
- **Mises à jour depuis un autre thread.** Manipuler un contrôle hors du thread d'interface lève une exception : passez par `Invoke` (ou les approches asynchrones du module [4](../04-async/README.md)).
- **Désabonnez vos `AddHandler`.** Un gestionnaire branché sur un objet à longue durée de vie maintient une référence et peut provoquer une fuite mémoire : pensez au `RemoveHandler` symétrique.
- **Gare aux événements déclenchés pendant l'initialisation.** Remplir les `Items` d'un `ComboBox`, par exemple, déclenche `SelectedIndexChanged` — souvent trop tôt. Parade : abonnez-vous **après** avoir peuplé le contrôle, ou protégez le gestionnaire par un indicateur (`_chargementEnCours`).
- **Gardez les gestionnaires fins** : déléguez la logique à des méthodes dédiées (ou à un *view-model* en MVVM, module [6.6](../06-wpf/06-mvvm.md)).

La section suivante s'appuie directement sur les événements clavier et de focus pour mettre en place la **validation** des saisies → [5.7 Validation](07-validation.md).

⏭️ [Validation (ErrorProvider, DataAnnotations, règles personnalisées)](/05-windows-forms/07-validation.md)
