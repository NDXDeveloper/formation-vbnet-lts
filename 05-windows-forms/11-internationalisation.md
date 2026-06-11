🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.11 Internationalisation (i18n/l10n, ressources `.resx`)

Adapter une application à plusieurs langues et cultures n'est pas un raffinement de dernière minute : c'est une décision d'architecture. WinForms offre pour cela un atout remarquable — un Concepteur capable de localiser un formulaire **texte et mise en page comprises** — appuyé sur les fichiers de ressources **`.resx`**. Cette section s'appuie sur la culture et le formatage (section [2.7](../02-fondamentaux-langage/07-dates-nombres-culture.md)), l'espace `My` (section [2.12](../02-fondamentaux-langage/12-espace-my.md)) et la mise en page adaptative (section [5.3](03-controles-fondamentaux.md)).

---

## i18n et l10n : deux étapes distinctes

Deux acronymes désignent deux moments :

- **Internationalisation (i18n)** — *concevoir* l'application pour qu'elle **puisse** être adaptée : aucune chaîne en dur, formatage sensible à la culture, mises en page qui absorbent l'allongement du texte traduit.
- **Localisation (l10n)** — *réaliser* l'adaptation à une culture donnée : traduire les textes, formater dates et nombres selon les usages locaux.

L'ordre compte : on **internationalise d'abord** (l'application devient adaptable), on **localise ensuite** (on traduit). Une application mal internationalisée est très coûteuse à localiser après coup.

---

## La culture en .NET : deux notions à ne pas confondre

.NET distingue deux cultures, et c'est l'un des points les plus mal compris :

- **`CurrentCulture`** régit le **formatage** : dates, nombres, monnaie (section [2.7](../02-fondamentaux-langage/07-dates-nombres-culture.md)) ;
- **`CurrentUICulture`** régit le **choix des ressources** : quelle traduction est chargée.

Les deux sont indépendantes : on peut afficher une interface en français tout en formatant les montants selon la culture canadienne. On les fixe **tôt**, avant l'affichage du premier formulaire — dans l'événement `Startup` du Framework d'application (section [5.1](01-introduction-designer.md)) ou dans `Sub Main` :

```vb
Imports System.Globalization

Private Sub MyApplication_Startup(sender As Object,
                                  e As ApplicationServices.StartupEventArgs) _
                                  Handles Me.Startup

    Dim culture As New CultureInfo("fr-FR")
    CultureInfo.CurrentUICulture = culture   ' langue de l'interface (ressources)
    CultureInfo.CurrentCulture = culture     ' formatage (dates, nombres, monnaie)
End Sub
```

Pour fixer ces cultures comme valeurs par défaut de tous les threads, on dispose aussi de `CultureInfo.DefaultThreadCurrentCulture` et `DefaultThreadCurrentUICulture`.

---

## Les ressources `.resx` et les assemblys satellites ⭐

Un fichier **`.resx`** stocke en XML des ressources localisables : chaînes, images, icônes. Le modèle de localisation repose sur une convention de nommage :

- un fichier **par défaut** (`Resources.resx`) ;
- des fichiers **spécifiques à une culture** (`Resources.fr.resx`, `Resources.de.resx`…).

À la compilation, les ressources spécifiques sont rassemblées dans des **assemblys satellites** (par exemple `fr\MonApp.resources.dll`). À l'exécution, le **`ResourceManager`** charge automatiquement la bonne culture d'après `CurrentUICulture`, avec un **repli** (*fallback*) en cascade : `fr-FR` → `fr` → ressource par défaut. Aucune traduction manquante ne provoque d'erreur : on retombe simplement sur le niveau supérieur.

En VB.NET, l'espace `My` expose un accès **fortement typé** aux ressources par défaut via `My.Resources` (section [2.12](../02-fondamentaux-langage/12-espace-my.md)) ; la valeur localisée est résolue automatiquement selon `CurrentUICulture` :

```vb
lblBienvenue.Text = My.Resources.MessageBienvenue
Me.Icon = My.Resources.IconeApplication
```

Pour des scénarios plus dynamiques (plusieurs jeux de ressources, chargement par clé calculée), on instancie directement un `ResourceManager`.

---

## Localiser un formulaire dans le Concepteur

C'est ici que WinForms brille. La marche à suivre :

1. passez la propriété **`Localizable`** du formulaire à **`True`** ;
2. réglez sa propriété **`Language`** sur une culture (par exemple *Français*) ;
3. traduisez et ajustez chaque contrôle — son **texte**, mais aussi sa **taille** et sa **position** — pour cette langue ; le Concepteur enregistre ces valeurs dans `Form1.fr.resx` ;
4. répétez pour chaque langue cible.

À l'exécution, `InitializeComponent` applique automatiquement les ressources de la culture correspondant à `CurrentUICulture`, avec repli sur la version par défaut.

> 💡 **L'atout décisif :** cette mécanique gère non seulement le **texte** mais aussi la **mise en page** (tailles, positions). C'est essentiel, car une traduction est rarement de la même longueur que l'original — un bouton « OK » devient « Aceptar », « Annuler » devient « Abbrechen ».

---

## Sens d'écriture et allongement du texte

- Pour les langues s'écrivant de **droite à gauche** (arabe, hébreu), `RightToLeft = Yes` et `RightToLeftLayout = True` inversent l'agencement de l'interface.
- Puisque les traductions varient en longueur, concevez des mises en page **adaptatives** (`TableLayoutPanel`, `Anchor`, `AutoSize`, section [5.3](03-controles-fondamentaux.md)) plutôt que des libellés à largeur fixe qui tronqueraient le texte traduit.

---

## Formatage selon la culture

Dates, nombres et monnaie suivent **`CurrentCulture`** — sans code spécifique à chaque langue (rappel de la section [2.7](../02-fondamentaux-langage/07-dates-nombres-culture.md)) :

```vb
Dim prix As Decimal = 1234.5D
lblPrix.Text = prix.ToString("C")          ' "1 234,50 €" en fr-FR, "$1,234.50" en en-US
lblDate.Text = DateTime.Now.ToShortDateString()
```

La règle : **ne formatez jamais à la main** (en concaténant un symbole monétaire, par exemple) ; laissez le formatage sensible à la culture s'en charger.

---

## En résumé et bonnes pratiques

- **Aucune chaîne visible en dur** : tout texte d'interface va dans un `.resx`.
- **Séparez les deux cultures** : `CurrentCulture` pour le formatage, `CurrentUICulture` pour la langue.
- **Fixez la culture tôt**, avant l'affichage des formulaires.
- **Concevez pour l'allongement** du texte (mise en page adaptative) et le sens d'écriture.
- **Exploitez le Concepteur** (`Localizable` + `Language`), qui localise texte *et* disposition.
- **Testez avec de vraies traductions**, pas seulement la langue par défaut.

> 📌 En WinForms, l'approche `.resx` / `ResourceManager` / `My.Resources` est la voie idiomatique. L'abstraction `IStringLocalizer` (`Microsoft.Extensions.Localization`) existe aussi, mais elle est surtout orientée web/ASP.NET Core.

La section suivante fait le tour des **nouveautés Windows Forms apportées par .NET 10** → [5.12 Nouveautés Windows Forms .NET 10](12-nouveautes-net10.md).

⏭️ [Nouveautés Windows Forms .NET 10 (presse-papiers JSON, Form.FormScreenCaptureMode anti-capture, éditeurs de designer portés depuis .NET Framework)](/05-windows-forms/12-nouveautes-net10.md)
