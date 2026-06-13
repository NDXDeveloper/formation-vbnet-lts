# 💻 Exemples du module 5 — Windows Forms (le scénario phare ⭐)

Douze projets **Windows Forms complets, compilés et exécutés** (un par section ; le README
du module n'a pas de code). Chaque projet reprend les codes de sa section, assemblés en une
application qui démarre réellement ; chaque fichier source porte un en-tête **section
concernée / description / fichier du cours**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

> 💡 **Structure** — chaque projet est créé comme le ferait Visual Studio : **cadre
> applicatif VB** (`MyType=WindowsForms`, `My Project/Application.Designer.vb`,
> `ApplicationEvents.vb` quand la section l'exige), et classes partielles
> `MainForm.vb` / `MainForm.Designer.vb`. Le `.Designer.vb` (normalement maintenu par le
> Concepteur) est écrit à la main, dans le style généré, pour que le projet soit complet
> hors de l'IDE.

## ▶️ Comment compiler et lancer

```bash
cd <dossier-de-l-exemple>
dotnet run            # ou : ouvrir le .vbproj dans VS 2026, puis F5
```

Ce sont des applications à **fenêtre** : « la sortie » est l'interface qui s'affiche, pas du
texte console. Deux mécanismes de **vérification automatique** ont été ajoutés (hors cours) :

- **`DEMO_AUTOCLOSE=1`** — variable d'environnement qui fait **s'auto-fermer** le formulaire
  après ~1,5 s. C'est le *smoke test* : l'application démarre, s'affiche, exécute sa logique
  de chargement, puis se ferme proprement (code de sortie 0) sans interaction. Le module
  partagé `AutoFermeture.vb` (présent dans chaque projet) implémente ce comportement.
  ```powershell
  $env:DEMO_AUTOCLOSE = "1"; .\bin\Debug\net10.0-windows\<App>.exe   # se ferme seul
  ```
  **Sans cette variable, l'application est pleinement interactive** : utilisez-la normalement.
- **Journal d'auto-test** — les sections à logique vérifiable (5.2, 5.7, 5.8, 5.10, 5.11,
  5.12) écrivent au chargement le **résultat de leurs assertions** dans
  `%TEMP%\<section>-autotest.log`, relu pour confirmer les valeurs attendues.

> Tous les projets ont été vérifiés ainsi : **compilation 0 erreur** + **démarrage sans
> crash (code 0)**, et, le cas échéant, **assertions du journal vraies**.

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Vérification |
|---|---|---|
| `README.md` (module) | — (aucun code) | |
| `01-introduction-designer.md` | [`5.1-architecture-designer`](#51) | smoke test |
| `02-winforms-net10.md` | [`5.2-winforms-net10`](#52) | **log** : round-trip JSON presse-papiers |
| `03-controles-fondamentaux.md` | [`5.3-controles-fondamentaux`](#53) | smoke test |
| `04-controles-avances.md` | [`5.4-controles-avances`](#54) | smoke test |
| `05-controles-personnalises.md` | [`5.5-controles-personnalises`](#55) | smoke test |
| `06-evenements.md` | [`5.6-evenements`](#56) | smoke test |
| `07-validation.md` | [`5.7-validation`](#57) | **log** : DataAnnotations + Validator |
| `08-data-binding.md` | [`5.8-data-binding`](#58) | **log** : notifications binding |
| `09-mdi.md` | [`5.9-mdi`](#59) | smoke test |
| `10-preferences.md` | [`5.10-preferences`](#510) | **log** : compteur persisté |
| `11-internationalisation.md` | [`5.11-internationalisation`](#511) | **log** : résolution des cultures |
| `12-nouveautes-net10.md` | [`5.12-nouveautes-net10`](#512) | **log** : JSON + FormScreenCaptureMode |

---

<a id="51"></a>
## 5.1-architecture-designer

- **Section** : 5.1 — Architecture et Concepteur · **Fichier** : `01-introduction-designer.md`
- **Description** : projet structuré comme le crée Visual Studio — cadre applicatif
  (`My Project/Application.Designer.vb`, `ApplicationEvents.vb`), et **classes partielles**
  `Form1.vb` / `Form1.Designer.vb` reliées par `InitializeComponent` et
  `Friend WithEvents btnValider` (qui autorise `Handles btnValider.Click`).
- **Comportement attendu** : fenêtre « Architecture et Concepteur » avec un libellé
  explicatif et un bouton **&Valider** (clic → `MessageBox`). Vérifié : démarre et se ferme
  proprement (code 0).

<a id="52"></a>
## 5.2-winforms-net10

- **Section** : 5.2 — Windows Forms sur .NET 10 · **Fichier** : `02-winforms-net10.md`
- **Description** : **mode sombre** activé via `ApplicationEvents.vb`
  (`ApplyApplicationDefaults` → `e.ColorMode = System`) ; **formulaire asynchrone**
  (`Await dlg.ShowDialogAsync(Me)`) ; **presse-papiers sécurisé**
  (`Clipboard.SetDataAsJson` / `TryGetData(Of Client)`).
- **Sortie / log attendus** (vérifiés) :
  ```text
  Clipboard round-trip JSON Client : True
    Nom relu = Dupont ; Ville relue = Rouen
  ```
- **Comportement** : le formulaire affiche `Application.ColorMode`/`SystemColorMode` et le
  texte relu du presse-papiers ; le bouton ouvre une boîte de dialogue en `ShowDialogAsync`.

<a id="53"></a>
## 5.3-controles-fondamentaux

- **Section** : 5.3 — Contrôles fondamentaux · **Fichier** : `03-controles-fondamentaux.md`
- **Description** : `TextBox` (`PlaceholderText`, `TextChanged` → filtrage d'une `ListBox`),
  `Button` (mnémonique **&Valider**), `GroupBox`+`RadioButton`, `MessageBox`
  (`DialogResult`), `OpenFileDialog`, et la **boîte de dialogue modale personnalisée**
  `ClientEditForm` (propriété `Client` marquée
  `<DesignerSerializationVisibility(Hidden)>` pour l'analyseur WFO1000,
  `AcceptButton`/`CancelButton`, `CausesValidation=False` sur Annuler).
- **Comportement attendu** : fenêtre de saisie ; filtrage en direct de la liste de fruits ;
  les boutons déclenchent message, ouverture de fichier et fiche modale. Vérifié : démarre
  et se ferme proprement.

<a id="54"></a>
## 5.4-controles-avances

- **Section** : 5.4 — Contrôles avancés · **Fichier** : `04-controles-avances.md`
- **Description** : onglets hébergeant un **`DataGridView`** lié à une
  `BindingList(Of Client)` (`DataError` géré), un **`TreeView`** (`Nodes`, `Tag` = objet
  métier, `AfterSelect`), un **`ListView`** en mode `Details` (`Columns`, `SubItems`,
  `Tag`), plus la famille **`MenuStrip`** (raccourci Ctrl+S) / **`ToolStrip`** /
  **`StatusStrip`** (`Spring`, `ToolStripProgressBar`).
- **Comportement attendu** : fenêtre à 3 onglets remplis de données ; le menu/la barre
  d'outils mettent à jour la barre d'état. Vérifié : démarre et se ferme proprement.

<a id="55"></a>
## 5.5-controles-personnalises

- **Section** : 5.5 — Contrôles personnalisés et `UserControl` · **Fichier** : `05-controles-personnalises.md`
- **Description** : les **trois approches** de la section — `UserControl` **SearchBox**
  (propriété `Placeholder` avec `<DefaultValue>` pour WFO1000, événement
  `RechercheDemandee` + `RechercheEventArgs`), héritage **NumericTextBox** (redéfinit
  `OnKeyPress`, appelle `MyBase`), dessin **LedIndicator** (hérite de `Control`, `OnPaint`
  GDI+, `SetStyle`, `Invalidate`, `SystemColors`).
- **Comportement attendu** : la SearchBox lève son événement ; la NumericTextBox refuse les
  non-chiffres ; le bouton bascule la LED dessinée. Vérifié : démarre et se ferme proprement.

<a id="56"></a>
## 5.6-evenements

- **Section** : 5.6 — Gestion des événements · **Fichier** : `06-evenements.md`
- **Description** : **souris** (`MouseDown`, `e.Button`/`e.Location`), **clavier**
  (`KeyPreview=True` + raccourci global **Ctrl+S**, `e.Handled`/`e.SuppressKeyPress`),
  composant **`Timer`** (horloge, `Tick` chaque seconde), **cycle de vie**
  (`Load`/`Shown`/`FormClosing` **annulable**), et un gestionnaire unique pour trois boutons
  (`Handles` multiple).
- **Comportement attendu** : horloge qui s'actualise, panneau réagissant aux clics, Ctrl+S
  capté, confirmation à la fermeture si modifications (désactivée en mode auto-close pour le
  test). Vérifié : démarre et se ferme proprement.

<a id="57"></a>
## 5.7-validation

- **Section** : 5.7 — Validation · **Fichier** : `07-validation.md`
- **Description** : `Client` annoté (`Required`, `Length`, `EmailAddress`, `Range`,
  `AllowedValues`) + **attribut personnalisé** `CodePostalFrAttribute`, et `Reservation`
  (**`IValidatableObject`**, validation inter-propriétés) ; le **pont
  DataAnnotations ↔ ErrorProvider** (`Validator.TryValidateObject`, report sur les
  contrôles via `NameOf`) ; `CausesValidation=False` sur Annuler.
- **Sortie / log attendus** (vérifiés) :
  ```text
  Client invalide -> valide ? False ; 4 erreur(s) :
    [Nom] Le nom doit comporter de 2 à 80 caractères.
    [Email] Adresse e-mail invalide.
    [Age] L'âge doit être compris entre 0 et 120.
    [CodePostal] Le code postal doit comporter 5 chiffres.
  Client valide -> valide ? True ; 0 erreur(s)
  Réservation (fin < début) -> valide ? False ; 1 erreur(s) : La date de fin doit suivre la date de début.
  ```

<a id="58"></a>
## 5.8-data-binding

- **Section** : 5.8 — Liaison de données · **Fichier** : `08-data-binding.md`
- **Description** : la pile **`BindingSource` → `BindingList(Of Client)` → éléments
  `INotifyPropertyChanged`** ; liaison **complexe** (`DataGridView`) et **simple**
  (`txtNom`/`txtEmail.DataBindings.Add`), **`BindingNavigator`**, et **maître-détail**
  (`bsCommandes.DataSource = bsClients`, `DataMember = NameOf(Client.Commandes)`).
- **Sortie / log attendus** (vérifiés) :
  ```text
  BindingList.Add -> ListChanged levé : True
  Client.Nom modifié -> PropertyChanged levé : True (propriété : Nom)
  Même valeur réaffectée -> aucune notification : True
  bsClients.MoveNext -> Position = 1, Current.Nom = Martin
  ```
- **Note technique** : un `BindingNavigator` exige un `IContainer` non nul à la
  construction — d'où l'initialisation `Me.components = New Container()` en tête
  d'`InitializeComponent` (sans quoi : `ArgumentNullException`, vérifié et corrigé).

<a id="59"></a>
## 5.9-mdi

- **Section** : 5.9 — Applications MDI · **Fichier** : `09-mdi.md`
- **Description** : conteneur **`IsMdiContainer=True`**, documents enfants (`MdiParent`),
  **`LayoutMdi`** (Cascade / Mosaïque), **menu Fenêtre** auto via `MdiWindowListItem`, et
  communication **découplée** avec un `FiltreForm` non modal (événement `FiltreApplique`
  + `AddHandler`).
- **Comportement attendu** : deux documents ouverts d'emblée et disposés en mosaïque ; le
  menu liste les fenêtres ; « Filtrer » ouvre une fenêtre-outil dont l'événement met à jour
  la barre d'état. Vérifié : démarre et se ferme proprement.

<a id="510"></a>
## 5.10-preferences

- **Section** : 5.10 — Préférences (`My.Settings`) · **Fichier** : `10-preferences.md`
- **Description** : paramètres de **portée User** (`My Project/Settings.settings` +
  `Settings.Designer.vb` : `DernierDossier`, `ModeSombre`, `CompteurLancements`,
  `MettreAJour`), accès **fortement typé**, **`Save()`** à la fermeture, et le motif
  **`Upgrade()`** (gardé par `MettreAJour`).
- **Sortie / log attendus** (vérifiés sur **3 lancements successifs**) :
  ```text
  Lancement -> CompteurLancements = 1
  Lancement -> CompteurLancements = 2
  Lancement -> CompteurLancements = 3
  ```
  → la persistance de `My.Settings` entre exécutions est démontrée.
- **Note** : `System.Configuration.ConfigurationManager` est fourni par le runtime Windows
  Desktop sur .NET 10 — **aucune** référence NuGet n'est nécessaire en WinForms
  (contrairement à un projet console, cf. 2.12).

<a id="511"></a>
## 5.11-internationalisation

- **Section** : 5.11 — Internationalisation · **Fichier** : `11-internationalisation.md`
- **Description** : ressources **`.resx`** par défaut + **assemblys satellites**
  (`Resources.fr.resx`, `Resources.en.resx`), accès `My.Resources` (Designer pointant la
  base `Internationalisation.Resources`), cultures fixées **tôt** dans `Startup`
  (`CurrentUICulture` / `CurrentCulture`), et formatage sensible à la culture.
- **Sortie / log attendus** (vérifiés) :
  ```text
  fr -> Bienvenue dans l'application !
  en -> Welcome to the application!
  de (aucun satellite -> repli défaut) -> Welcome (default / neutral)
  Résolution fr correcte : True
  Résolution en correcte : True
  Repli de -> défaut : True
  ```
- **Astuce** : relancez avec `DEMO_CULTURE=en` (ou `de`) pour voir l'interface basculer de
  langue — le repli en cascade vers la ressource par défaut est démontré par la culture
  `de`, sans satellite.

<a id="512"></a>
## 5.12-nouveautes-net10

- **Section** : 5.12 — Nouveautés Windows Forms .NET 10 · **Fichier** : `12-nouveautes-net10.md`
- **Description** : **presse-papiers JSON typé** avec **format inféré**
  (`SetDataAsJson(GetType(Personne).FullName, …)` puis `TryGetData(recuperee)`),
  **glisser-déposer typé** via `ITypedDataObject` (`AllowDrop`, `DragEnter`/`DragDrop` sur
  `FileDrop`), et **`Form.FormScreenCaptureMode = HideContent`** (anti-capture).
- **Sortie / log attendus** (vérifiés) :
  ```text
  Clipboard JSON Personne (format inféré) : True
  FormScreenCaptureMode appliqué : HideContent
  ```
- **Comportement** : déposez des fichiers sur le panneau → ils sont listés (récupération
  typée). L'anti-capture noircit la fenêtre dans les captures par l'API Windows.

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build`.

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

> Résidus d'exécution (supprimables) : les journaux `%TEMP%\5.*-autotest.log`, et — pour
> 5.10 — le `user.config` sous `%LOCALAPPDATA%\Preferences*\` (c'est précisément l'objet de
> la démo `My.Settings`).

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
