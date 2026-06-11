🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.5 Contrôles personnalisés et `UserControl`

Les contrôles standard (sections [5.3](03-controles-fondamentaux.md) et [5.4](04-controles-avances.md)) couvrent la grande majorité des besoins. Mais il arrive qu'aucun ne convienne : on veut **réutiliser** un assemblage de contrôles d'un écran à l'autre, **spécialiser** le comportement d'un contrôle existant, ou **dessiner** quelque chose d'entièrement sur mesure. Windows Forms offre pour cela trois approches.

---

## Trois façons de créer un contrôle

| Approche | Classe de base | Quand l'employer |
|---|---|---|
| **UserControl** | `UserControl` | Composer plusieurs contrôles existants en une unité réutilisable (le cas le plus courant). |
| **Héritage** | un contrôle existant (`TextBox`, `Button`…) | Ajouter ou modifier un comportement précis sans repartir de zéro. |
| **Dessin sur mesure** | `Control` | Rendre un visuel que ne produit aucun contrôle existant (jauge, indicateur, graphe…). |

> 💡 Choisissez toujours **l'approche la plus simple** qui répond au besoin : un `UserControl` pour assembler, l'héritage pour un ajustement léger, et le dessin de A à Z seulement lorsque le rendu l'exige.

---

## Le `UserControl` : composer des contrôles existants ⭐

C'est l'approche reine. Un `UserControl` se conçoit **comme un mini-formulaire** : on y dépose des contrôles dans le Concepteur, et il repose sur le même mécanisme de **classes partielles** que les formulaires (le `Inherits System.Windows.Forms.UserControl` se trouve dans le fichier `*.Designer.vb` généré, voir section [5.1](01-introduction-designer.md)).

Tout l'art consiste à **exposer une interface publique propre** — des propriétés et des événements pensés pour l'utilisateur du contrôle — sans laisser fuiter les contrôles internes.

```vb
Imports System.ComponentModel

' Le Inherits UserControl est dans le fichier .Designer.vb
Public Class SearchBox

    ' Propriété exposée et documentée pour le Concepteur.
    ' DefaultValue satisfait aussi l'analyseur WFO1000 (voir 5.3) :
    ' toute propriété publique d'un contrôle doit déclarer sa sérialisation.
    <Category("Apparence")>
    <DefaultValue("")>
    <Description("Texte d'invite affiché dans le champ de recherche.")>
    Public Property Placeholder As String
        Get
            Return txtRecherche.PlaceholderText
        End Get
        Set(value As String)
            txtRecherche.PlaceholderText = value
        End Set
    End Property

    ' Événement personnalisé (idiome VB Event/RaiseEvent, voir module 3.6)
    Public Event RechercheDemandee As EventHandler(Of RechercheEventArgs)

    Private Sub btnRechercher_Click(sender As Object, e As EventArgs) Handles btnRechercher.Click
        RaiseEvent RechercheDemandee(Me, New RechercheEventArgs(txtRecherche.Text))
    End Sub

End Class
```

L'argument de l'événement est une classe d'`EventArgs` dédiée — la façon idiomatique de transporter des données d'événement (module [3.6](../03-poo/06-evenements-delegues.md)) :

```vb
Public Class RechercheEventArgs
    Inherits EventArgs

    Public ReadOnly Property Terme As String

    Public Sub New(terme As String)
        Me.Terme = terme
    End Sub
End Class
```

Une fois le projet compilé, le `UserControl` apparaît **automatiquement dans la Boîte à outils** et se manipule comme n'importe quel contrôle. L'attribut `<ToolboxBitmap(...)>` permet de lui donner une icône.

---

## Hériter d'un contrôle existant

Pour un ajustement ciblé, on dérive directement le contrôle voulu. Ici, pas de fichier `*.Designer.vb` : le contrôle hérite de tout son parent, et l'on n'ajoute que ce qui change. La bonne pratique est de **redéfinir la méthode protégée `OnXxx`** plutôt que de s'abonner à son propre événement — c'est le point d'extension canonique, plus efficace.

```vb
' Une zone de texte qui n'accepte que des chiffres
Public Class NumericTextBox
    Inherits TextBox

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        ' Laisser passer les chiffres et les touches de contrôle (Retour arrière, etc.)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If

        MyBase.OnKeyPress(e)   ' ne jamais oublier d'appeler la base
    End Sub

End Class
```

L'appel à `MyBase.OnKeyPress(e)` est essentiel : il garantit que le contrôle parent continue de déclencher son événement `KeyPress` et son comportement normal.

---

## Dessiner un contrôle de A à Z (à partir de `Control`)

Quand aucun contrôle ne produit le rendu souhaité, on hérite de `Control` et l'on **redéfinit `OnPaint`** pour tout dessiner soi-même avec GDI+ (`e.Graphics`). Trois réflexes accompagnent cette approche :

- activer les bons styles via **`SetStyle`** pour un rendu **sans scintillement** et redessiné au redimensionnement ;
- appeler **`Invalidate()`** dès qu'une propriété visuelle change, pour demander un redessin ;
- **libérer** les objets GDI+ (pinceaux, stylos) avec `Using`.

```vb
Imports System.ComponentModel
Imports System.Drawing.Drawing2D

' Le Inherits Control peut figurer ici (pas de contrôle composé, donc pas de .Designer.vb)
Public Class LedIndicator
    Inherits Control

    Private _allume As Boolean

    Public Sub New()
        ' Dessin optimisé, sans scintillement, redessiné au redimensionnement
        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw, True)
    End Sub

    <Category("Comportement")>
    <DefaultValue(False)>
    Public Property Allume As Boolean
        Get
            Return _allume
        End Get
        Set(value As Boolean)
            If _allume <> value Then
                _allume = value
                Invalidate()   ' déclenche un nouveau OnPaint
            End If
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' SystemColors s'adapte au mode clair/sombre (voir section 5.2)
        Dim couleur As Color = If(_allume, Color.LimeGreen, SystemColors.ControlDark)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Using pinceau As New SolidBrush(couleur)
            e.Graphics.FillEllipse(pinceau, 2, 2, Width - 4, Height - 4)
        End Using
    End Sub

End Class
```

> 🆕 **Mode sombre et dessin sur mesure.** Les contrôles standard suivent le mode de couleur de l'application, mais lorsque vous **dessinez vous-même**, c'est à vous de vous y conformer. La règle : utilisez `SystemColors` (qui change avec le mode clair/sombre) plutôt que des couleurs codées en dur, et lisez `Application.ColorMode` si vous devez adapter votre rendu (section [5.2](02-winforms-net10.md)).

---

## L'intégration avec le Concepteur

Quelle que soit l'approche, soigner l'intégration au Concepteur rend le contrôle agréable à utiliser. Les attributs de `System.ComponentModel` pilotent l'affichage des propriétés :

- **`<Category("…")>`** regroupe la propriété sous une rubrique de la fenêtre Propriétés ;
- **`<Description("…")>`** fournit l'aide affichée en bas de cette fenêtre ;
- **`<DefaultValue(...)>`** indique la valeur par défaut : le Concepteur met en gras les valeurs non par défaut, propose « Réinitialiser », et **n'écrit pas dans `InitializeComponent`** les valeurs restées au défaut ;
- **`<Browsable(False)>`** masque une propriété de la fenêtre Propriétés ;
- **`<DesignerSerializationVisibility(...)>`** contrôle finement la sérialisation d'une propriété.

> ⚠️ Rappel du piège `WFO1000` (vérifié sur .NET 10, présenté en [5.3](03-controles-fondamentaux.md)) : depuis
> .NET 9, déclarer la sérialisation des propriétés publiques d'un contrôle n'est **plus optionnel** — sans
> `<DefaultValue(...)>` ni `<DesignerSerializationVisibility(...)>` (ou méthodes `ShouldSerialize*`/`Reset*`),
> la compilation échoue. C'est pourquoi les propriétés `Placeholder` et `Allume` ci-dessus portent chacune
> leur attribut.

Les événements publics, eux, apparaissent automatiquement dans l'onglet **Événements** de la fenêtre Propriétés.

> 🆕 Deux points liés à .NET moderne (section [5.1](01-introduction-designer.md)) : le Concepteur s'exécute **hors processus**, et les **SnapLines ont été corrigées pour les concepteurs personnalisés** dans .NET 10 — l'alignement assisté fonctionne donc aussi pour vos contrôles.

> 📌 **Composant non visuel.** Si l'élément à réutiliser n'a pas d'interface (un service de minuterie, un fournisseur de données…), on n'hérite pas de `Control` mais de `Component` : l'objet apparaît alors dans le **bac à composants** sous le formulaire, et non sur la surface.

---

## Bonnes pratiques

- **Encapsuler.** Exposez des propriétés et événements qui expriment l'**intention** du contrôle, pas ses rouages internes.
- **Choisir la bonne base.** `UserControl` pour composer, héritage pour spécialiser, `Control` pour dessiner — du plus simple au plus coûteux.
- **Toujours appeler `MyBase`** dans les redéfinitions `OnXxx`, sous peine de casser le comportement hérité.
- **Limiter les `Invalidate()`** et libérer les ressources GDI+ : le dessin se fait sur le thread d'interface, un repaint trop fréquent nuit à la fluidité (module [14](../14-performance/README.md)).
- **Respecter le thème** via `SystemColors`, pour un rendu correct en clair comme en sombre.

La suite approfondit la mécanique des **événements** — ceux que vos contrôles consomment comme ceux qu'ils exposent → [5.6 Gestion des événements](06-evenements.md).

⏭️ [Gestion des événements (souris, clavier, cycle de vie du formulaire)](/05-windows-forms/06-evenements.md)
