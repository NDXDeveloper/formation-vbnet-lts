# 💻 Exemples du module 6 — WPF (Windows Presentation Foundation)

Neuf projets **WPF complets, compilés et exécutés** (un par section ; le README du module
n'a pas de code). Chaque projet reprend les codes XAML et VB de sa section ; chaque fichier
source porte un en-tête **section concernée / description / fichier du cours**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

> 💡 **Structure** — chaque projet a la structure du modèle WPF VB de Visual Studio :
> `Application.xaml` (+ `.vb`), `MainWindow.xaml` (+ `.vb`), un `.vbproj` SDK-style avec
> `UseWPF=true`. ⚠️ **Spécificité VB vérifiée** : `x:Class` s'écrit **sans** le namespace
> racine (`x:Class="MainWindow"`), comme l'explique la section 6.2. Le code-behind n'a pas
> besoin d'un constructeur explicite : VB WPF appelle `InitializeComponent` automatiquement.

## ▶️ Comment compiler et lancer

```bash
cd <dossier-de-l-exemple>
dotnet run            # ou : ouvrir le .vbproj dans VS 2026, puis F5
```

Comme au module 5 (applications à fenêtre), la vérification combine **compilation 0 erreur**,
**smoke test** (l'app démarre, s'affiche, se ferme sans crash) et, pour la logique
vérifiable, un **journal d'auto-test**.

- **`DEMO_AUTOCLOSE=1`** — la fenêtre s'auto-ferme après ~1,8 s (module commun
  `AutoFermeture.vb`, présent dans chaque projet). Sans la variable, l'application est
  pleinement interactive.
- **Journal** — les sections à logique vérifiable (6.3, 6.4, 6.6, 6.8, 6.9) écrivent leurs
  assertions dans `%TEMP%\6.x-...-autotest.log`.

> Tous les projets : **0 erreur de compilation** + **démarrage code 0** + (le cas échéant)
> **assertions du journal vraies**.

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Vérification |
|---|---|---|
| `README.md` (module) | — (aucun code) | |
| `01-introduction-wpf-vs-winforms.md` | [`6.1-wpf-vs-winforms`](#61) | smoke test |
| `02-xaml-layout.md` | [`6.2-xaml-layout`](#62) | smoke test |
| `03-controles.md` | [`6.3-controles`](#63) | **log** : sync ObservableCollection |
| `04-data-binding.md` | [`6.4-data-binding`](#64) | **log** : INPC, convertisseur, validation |
| `05-styles-templates.md` | [`6.5-styles-templates`](#65) | smoke test |
| `06-mvvm.md` | [`6.6-mvvm`](#66) | **log** : RelayCommand (maison + toolkit) |
| `07-animations-multimedia.md` | [`6.7-animations-multimedia`](#67) | smoke test |
| `08-fluent-design-net10.md` | [`6.8-fluent-design-net10`](#68) | **log** : ThemeMode |
| `09-performance.md` | [`6.9-performance`](#69) | **log** : Freezable, Tier |

---

<a id="61"></a>
## 6.1-wpf-vs-winforms

- **Section** : 6.1 — WPF vs Windows Forms · **Fichier** : `01-introduction-wpf-vs-winforms.md`
- **Description** : l'interface décrite **en XAML** (Grid/StackPanel centrant un bouton sans
  pixels), `Click` relié au code-behind (gestionnaire à `RoutedEventArgs`), et un
  pré-remplissage par **binding** (`{Binding NomClient}` alimenté par le `DataContext`).
  Illustre le piège VB : `x:Class="MainWindow"` **sans** le namespace racine.
- **Comportement attendu** : fenêtre avec un champ pré-rempli par binding et un bouton
  (clic → `MessageBox`). Vérifié : démarre et se ferme proprement.

<a id="62"></a>
## 6.2-xaml-layout

- **Section** : 6.2 — XAML et disposition · **Fichier** : `02-xaml-layout.md`
- **Description** : combine **DockPanel** (coquille : menu/barre d'état/contenu,
  `LastChildFill`), **Grid** (lignes/colonnes `Auto`/`*`/fixe, propriétés attachées
  `Grid.Row`/`Column`/`ColumnSpan`) et **StackPanel**. Démontre les **événements routés** :
  un seul gestionnaire au niveau du `StackPanel` (`ButtonBase.Click`) capte le clic de tous
  ses boutons ; on distingue `sender` (le panneau) de `e.Source` (le bouton réel).
- **Comportement attendu** : cliquer un bouton du volet met à jour la barre d'état avec son
  contenu. Vérifié : démarre et se ferme proprement.

<a id="63"></a>
## 6.3-controles

- **Section** : 6.3 — Contrôles de données · **Fichier** : `03-controles.md`
- **Description** : **DataGrid** éditable (colonnes typées : Text, CheckBox, Template) et
  **ListView + GridView** (affichage, `DisplayMemberBinding`, `StringFormat=C`), tous deux
  liés à une **`ObservableCollection`**. Boutons Ajouter/Supprimer.
- **Sortie / log attendus** (vérifiés) :
  ```text
  ObservableCollection liée au DataGrid : 2 -> Add -> 3 -> Remove -> 2
  Synchronisation cohérente : True
  ```

<a id="64"></a>
## 6.4-data-binding

- **Section** : 6.4 — Liaison de données · **Fichier** : `04-data-binding.md`
- **Description** : `DataContext`, liaison `TwoWay` + `UpdateSourceTrigger=PropertyChanged`,
  `INotifyPropertyChanged` (avec `<CallerMemberName>`), **convertisseur**
  `BoolEnCouleurConverter` (déclaré en ressource), `StringFormat`, liaison `ElementName`
  (un `TextBlock` suit un `Slider`), et **`ValidationRule`** (`ChampRequisRule`).
- **Sortie / log attendus** (vérifiés) :
  ```text
  INotifyPropertyChanged : Nom modifié -> PropertyChanged("Nom") -> True
  Convertisseur True  -> #FF008000 (Green attendu : True)
  Convertisseur False -> #FFFF0000 (Red attendu : True)
  ValidationRule : vide IsValid=False ; rempli IsValid=True
  ```

<a id="65"></a>
## 6.5-styles-templates

- **Section** : 6.5 — Styles, ressources, templates et triggers · **Fichier** : `05-styles-templates.md`
- **Description** : **ressources** (`SolidColorBrush`, `Thickness`, `sys:Double`) référencées
  par `{StaticResource}`, **styles** nommé / implicite / `BasedOn`, **triggers**
  (`Trigger IsMouseOver`/`IsEnabled`, `DataTrigger` sur `EstActif`), et **templates**
  (`ControlTemplate` avec `TemplateBinding`/`ContentPresenter` ; `DataTemplate` pour un
  `Client`).
- **Comportement attendu** : boutons stylés (dont un re-templaté arrondi et un désactivé),
  liste de clients dont les inactifs apparaissent en gris (DataTrigger). Vérifié : démarre
  et se ferme proprement.
- **Note technique** : un `sys:Double` (8) ne peut pas alimenter une `Margin` (de type
  `Thickness`) — une `Thickness` distincte est utilisée pour les marges (erreur
  `InvalidOperationException` sinon, vérifiée et corrigée).

<a id="66"></a>
## 6.6-mvvm

- **Section** : 6.6 — Architecture MVVM · **Fichier** : `06-mvvm.md`
- **Description** : MVVM complet en **deux variantes** :
  - **« à la main »** : `ViewModelBase` (INotifyPropertyChanged), `RelayCommand`
    (`ICommand`), `ClientViewModel` (propriété `Nom`, commande `EnregistrerCommand`
    désactivée si `Nom` vide) — c'est le `DataContext` de la fenêtre ;
  - **`CommunityToolkit.Mvvm`** : `ClientViewModelToolkit` hérite d'`ObservableObject`
    (`SetProperty`) et utilise le `RelayCommand` du toolkit. ⚠️ Rappel : les *source
    generators* (`[ObservableProperty]`/`[RelayCommand]`) sont **C# uniquement** ; en VB on
    écrit la propriété complète (un cran plus verbeux, sans perte fonctionnelle).
- **Sortie / log attendus** (vérifiés) :
  ```text
  RelayCommand maison : CanExecute (Nom vide) = False ; (Nom rempli) = True
  Execute -> DernierEnregistre = Durand (True)
  CommunityToolkit : CanExecute (vide) = False ; (rempli) = True
  ```
- **Comportement** : le bouton « Enregistrer » est grisé tant que le champ est vide
  (`CanExecute`), sans aucun gestionnaire d'événement.

<a id="67"></a>
## 6.7-animations-multimedia

- **Section** : 6.7 — Animations et multimédia · **Fichier** : `07-animations-multimedia.md`
- **Description** : un **fondu d'apparition déclaratif** (`EventTrigger` sur `Loaded` →
  `BeginStoryboard` → `DoubleAnimation` sur `Opacity` + `QuadraticEase`), et une animation
  **lancée en code** (`BeginAnimation`). On anime `Opacity`/`RenderTransform` (composés GPU),
  pas `Width`/`Height`. Le multimédia (`MediaElement`, tributaire des codecs) est documenté
  mais non exécuté.
- **Comportement attendu** : le panneau apparaît en fondu ; le bouton clignote au clic.
  Vérifié : démarre et se ferme proprement.

<a id="68"></a>
## 6.8-fluent-design-net10

- **Section** : 6.8 — Thèmes et Fluent Design (.NET 10) · **Fichier** : `08-fluent-design-net10.md`
- **Description** : active le **thème Fluent** via la propriété **`ThemeMode`**
  (`ThemeMode.System`, suit le clair/sombre de Windows). L'API étant **expérimentale**,
  l'avertissement **`WPF0001`** est neutralisé dans le `.vbproj` (`<NoWarn>`). Démontre aussi
  la **syntaxe abrégée de `Grid`** (.NET 10) : `RowDefinitions="Auto, *, 40"`.
- **Sortie / log attendus** (vérifiés) :
  ```text
  ThemeMode appliqué (fenêtre) : System
  Fluent actif (≠ None) : True
  ```

<a id="69"></a>
## 6.9-performance

- **Section** : 6.9 — Performance WPF · **Fichier** : `09-performance.md`
- **Description** : **virtualisation** d'une `ListBox` de 10 000 éléments
  (`VirtualizationMode=Recycling`, hauteur bornée par le `DockPanel`), **chargement en bloc**
  (une `List` affectée d'un coup), **`Freezable.Freeze()`** (pinceau immuable et thread-safe),
  et lecture de **`RenderCapability.Tier`** (niveau d'accélération matérielle). Le décodage
  d'image à la taille d'affichage (`DecodePixelWidth`) est montré en commentaire.
- **Sortie / log attendus** (vérifiés) :
  ```text
  Éléments chargés : 10000
  Freezable gelé : IsFrozen = True
  RenderCapability.Tier = 2
  ```
  (`Tier` vaut 0, 1 ou 2 selon le poste / l'accélération matérielle disponible)

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build` (le 6.6 retéléchargera `CommunityToolkit.Mvvm` depuis le cache NuGet).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

> Résidus d'exécution supprimables : les journaux `%TEMP%\6.*-autotest.log`.

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
