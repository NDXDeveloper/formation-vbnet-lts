🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.1 Introduction, architecture et le Concepteur (*Designer*)

Cette première section pose les fondations du module. Avant de manipuler des contrôles, il faut comprendre **ce qu'est réellement Windows Forms**, **comment une application WinForms est structurée** en VB.NET sur .NET 10, et **comment fonctionne le Concepteur** (*Designer*) de Visual Studio — l'outil qui a fait, et fait toujours, la productivité légendaire de Visual Basic.

---

## Qu'est-ce que Windows Forms ?

Windows Forms (abrégé *WinForms*) est un *framework* d'interface graphique pour construire des applications de bureau **Windows uniquement**. Il s'agit d'une couche managée posée au-dessus du système de fenêtrage natif de Windows : la plupart des contrôles fondamentaux (`Button`, `TextBox`, `CheckBox`…) encapsulent des contrôles Win32 natifs, et le rendu s'appuie sur GDI+.

Trois caractéristiques en définissent le modèle :

- **Orienté objet et « retenu ».** L'interface est un arbre d'objets (`Control`) que le *framework* conserve en mémoire et gère pour vous : vous décrivez *quoi* afficher, pas *comment* le redessiner à chaque image.
- **Piloté par les événements.** L'utilisateur agit (clic, frappe, redimensionnement), le système traduit ces actions en **événements** .NET, et votre code y répond par des **gestionnaires** (*handlers*). C'est le paradigme idiomatique de VB.NET, vu au module [3.6](../03-poo/06-evenements-delegues.md).
- **Mono-thread d'interface.** Tous les contrôles sont créés et manipulés sur un unique **thread d'interface** (*UI thread*). En conséquence, une opération longue exécutée sur ce thread *fige* l'application — d'où l'importance de l'asynchronisme (module [4](../04-async/README.md)).

Sur .NET 10, WinForms fait partie du *runtime* **Windows Desktop** (`Microsoft.WindowsDesktop.App`), distribué avec le SDK. C'est l'un des scénarios que Microsoft a explicitement portés et continue de moderniser pour VB.NET (voir le [README du module](README.md) et la section [5.2](02-winforms-net10.md)).

> 💡 WinForms privilégie la **rapidité de développement** (RAD) et la robustesse pour les applications de gestion (*LOB*), au prix d'une personnalisation graphique moins poussée que WPF (module [6](../06-wpf/README.md)).

---

## L'architecture d'une application Windows Forms

### La boucle de messages

Au cœur de toute application WinForms se trouve une **boucle de messages** (*message loop* ou *message pump*). Lorsqu'on appelle `Application.Run`, le *framework* démarre une boucle qui :

1. récupère les **messages Windows** (souris, clavier, redimensionnement…) depuis la file d'attente du thread d'interface ;
2. les distribue au contrôle concerné, dont la méthode interne `WndProc` les traite ;
3. les convertit en **événements** .NET (`Click`, `KeyDown`, `Resize`…) auxquels votre code est abonné.

Cette boucle tourne tant que le formulaire principal est ouvert. Elle s'exécute sur un thread marqué **STA** (*Single-Threaded Apartment*), exigence du modèle COM sous-jacent (presse-papiers, glisser-déposer, boîtes de dialogue communes). Avec le Framework d'application VB.NET (voir plus bas), ce détail est géré pour vous ; sans lui, il faut l'attribut `<STAThread>` sur `Sub Main`.

### Le formulaire et la hiérarchie des contrôles

Le **formulaire** (`Form`) est l'unité centrale : c'est une fenêtre, elle-même un `Control`. Les contrôles s'organisent en **arbre parent/enfant** via la collection `Controls` : un formulaire contient des conteneurs (`Panel`, `GroupBox`…), qui contiennent eux-mêmes d'autres contrôles. Position, taille, visibilité et disposition se propagent le long de cette hiérarchie.

```
Form1
├── MenuStrip
├── Panel (haut)
│   ├── Label
│   └── TextBox
└── Button
```

### Le thread d'interface

Tout accès à un contrôle doit se faire **sur le thread d'interface**. Manipuler un contrôle depuis un autre thread provoque une exception. Pour communiquer avec l'interface depuis un travail en arrière-plan, on utilise `Invoke`/`BeginInvoke`, ou — bien mieux en 2026 — l'approche `Async`/`Await` détaillée au module [4](../04-async/README.md). On y revient pour la pratique en [5.6](06-evenements.md).

---

## Anatomie d'un projet WinForms en VB.NET (.NET 10)

### Le fichier projet (SDK-style)

Sur .NET moderne, le fichier `.vbproj` adopte le format **SDK-style**, concis et lisible :

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <RootNamespace>MonApplication</RootNamespace>
    <MyType>WindowsForms</MyType>
  </PropertyGroup>

</Project>
```

Points à retenir :

- `OutputType` à **`WinExe`** produit une application fenêtrée (sans console au démarrage).
- `TargetFramework` à **`net10.0-windows`** : le suffixe `-windows` est requis, WinForms étant spécifique à Windows.
- `UseWindowsForms` à **`true`** active la référence au *runtime* Windows Desktop et l'intégration du Concepteur.
- `MyType` à **`WindowsForms`** active le **Framework d'application VB.NET** et les membres WinForms de l'espace `My` (voir [2.12](../02-fondamentaux-langage/12-espace-my.md)). La valeur `WindowsFormsWithCustomSubMain` désactive le Framework d'application au profit d'un `Sub Main` que vous écrivez vous-même — tout en conservant l'espace `My`.

> 💡 **Nuance vérifiée sur .NET 10.** Le modèle en ligne de commande (`dotnet new winforms -lang VB`, vu en [1.5](../01-introduction-vbnet/05-premier-projet.md)) n'émet **aucun** `MyType` : sans lui, ni Framework d'application, **ni même `My.Computer` / `My.Application`** (erreur BC30456 à la compilation). C'est le modèle **Visual Studio** qui configure `MyType` et le cadre applicatif. Si vous partez du modèle CLI et voulez `My`, ajoutez vous-même la propriété au `.vbproj`.

### La structure de fichiers

Un projet WinForms VB typique comprend :

- `Form1.vb` — **votre** code du formulaire (gestionnaires d'événements, logique) ;
- `Form1.Designer.vb` — le code **généré** par le Concepteur (déclaration des contrôles, mise en page) ;
- `ApplicationEvents.vb` — les événements d'application (optionnel, propre au Framework d'application VB) ;
- les propriétés de projet (**My Project** / page *Application*), où l'on configure le démarrage.

### Les classes partielles : la séparation code / Concepteur

Un même formulaire est réparti sur deux fichiers grâce aux **classes partielles** (`Partial`, vues au module [3.5](../03-poo/05-modules-namespaces.md)). Cette séparation est essentielle : elle isole le code que vous écrivez du code que le Concepteur régénère.

Côté **votre code**, `Form1.vb` reste épuré :

```vb
Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Code exécuté au chargement du formulaire
    End Sub

    Private Sub btnValider_Click(sender As Object, e As EventArgs) Handles btnValider.Click
        MessageBox.Show("Bonjour !")
    End Sub

End Class
```

Notez que la classe **n'indique pas** `Inherits System.Windows.Forms.Form` : c'est la partie générée qui le déclare.

Côté **Concepteur**, `Form1.Designer.vb` contient la déclaration des contrôles et la méthode `InitializeComponent` :

```vb
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    ' Méthode requise par le Concepteur — ne pas modifier à la main
    Private Sub InitializeComponent()
        Me.btnValider = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        ' btnValider
        '
        Me.btnValider.Location = New System.Drawing.Point(30, 30)
        Me.btnValider.Name = "btnValider"
        Me.btnValider.Size = New System.Drawing.Size(120, 30)
        Me.btnValider.Text = "Valider"
        '
        ' Form1
        '
        Me.ClientSize = New System.Drawing.Size(400, 200)
        Me.Controls.Add(Me.btnValider)
        Me.Text = "Form1"
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents btnValider As System.Windows.Forms.Button

End Class
```

Deux éléments méritent l'attention :

- **`InitializeComponent`** est appelée par le constructeur du formulaire (`Sub New`) et reconstruit l'interface à l'exécution **à partir du même code** que celui lu par le Concepteur. C'est le pivot du « va-et-vient » décrit plus loin.
- **`Friend WithEvents btnValider`** déclare le contrôle avec le mot-clé `WithEvents` : c'est précisément lui qui autorise la clause `Handles btnValider.Click` dans votre code. Cet idiome `WithEvents`/`Handles` est l'une des élégances de VB.NET (module [3.6](../03-poo/06-evenements-delegues.md)). ⭐

> ⚠️ La mention « ne pas modifier à la main » n'est pas un dogme absolu, mais le Concepteur peut réécrire et réordonner ce fichier à tout moment : toute édition manuelle risque d'être écrasée. La règle pratique : on manipule les contrôles **via le Concepteur**, on écrit la logique **dans `Form1.vb`**.

---

## Le point d'entrée et le Framework d'application VB.NET

### Le Framework d'application (la voie par défaut) ⭐

VB.NET propose une particularité absente de C# : le **Framework d'application** (*Application Framework*). Activé par défaut pour les projets WinForms, il génère pour vous le point d'entrée et expose `My.Application` comme une véritable application encadrée. Il se configure dans les propriétés du projet (onglet *Application*) et offre, sans écrire une ligne de plomberie :

- le choix du **formulaire de démarrage** ;
- le **mode d'arrêt** (à la fermeture du formulaire principal, ou du dernier formulaire) ;
- l'**instance unique** (empêcher plusieurs lancements de l'application) ;
- un **écran de démarrage** (*splash screen*) ;
- l'enregistrement automatique de `My.Settings` à la fermeture ;
- des **événements d'application** centralisés.

Ces événements se déclarent dans `ApplicationEvents.vb` :

```vb
Namespace My

    ' Framework d'application VB.NET — événements globaux
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object,
                                          e As ApplicationServices.StartupEventArgs) _
                                          Handles Me.Startup
            ' Avant l'affichage du formulaire de démarrage
        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object,
                                                     e As ApplicationServices.UnhandledExceptionEventArgs) _
                                                     Handles Me.UnhandledException
            ' Gestion centralisée des exceptions non interceptées
        End Sub

    End Class

End Namespace
```

Les événements disponibles incluent `Startup`, `Shutdown`, `StartupNextInstance` (utile avec l'instance unique), `UnhandledException` et `NetworkAvailabilityChanged`. Le Framework d'application illustre bien l'esprit de VB.NET : **moins de code d'infrastructure, plus de logique métier**.

### L'alternative : un `Sub Main` manuel

Si l'on désactive le Framework d'application (`MyType` à `WindowsFormsWithCustomSubMain`), on prend la main sur le démarrage :

```vb
Imports System.Windows.Forms

Module Program

    <STAThread>
    Sub Main()
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1())
    End Sub

End Module
```

Cette voie offre un contrôle total (utile pour l'injection de dépendances, des scénarios d'amorçage particuliers, ou une cohérence avec un projet C#), au prix de la perte des commodités de `My.Application`. Pour la majorité des applications de bureau VB.NET, **le Framework d'application reste le choix recommandé**.

---

## Le Concepteur (*Designer*) de Visual Studio

Le Concepteur est l'atelier visuel où l'on dessine l'interface. C'est l'héritier direct de l'expérience qui a fait le succès de Visual Basic, et il demeure remarquablement productif.

### Les fenêtres clés

- **La surface de conception** : la représentation visuelle du formulaire, sur laquelle on place et dispose les contrôles.
- **La Boîte à outils** (*Toolbox*) : le catalogue des contrôles disponibles, que l'on **glisse-dépose** sur la surface.
- **La fenêtre Propriétés** : l'édition des propriétés du contrôle sélectionné (texte, taille, couleur, ancrage…). Son **onglet Événements** (icône éclair ⚡) liste tous les événements disponibles et permet d'y associer un gestionnaire.
- **L'arborescence du document** (*Document Outline*) : la vue hiérarchique de l'arbre des contrôles, pratique pour les interfaces complexes.

### Du geste visuel au code

L'enchaînement typique illustre la philosophie RAD :

1. on **glisse** un bouton depuis la Boîte à outils sur le formulaire ;
2. on **renomme et configure** le bouton dans la fenêtre Propriétés ;
3. on **double-clique** sur le bouton : le Concepteur crée automatiquement le gestionnaire de son événement par défaut (`Click`) dans `Form1.vb`, avec la clause `Handles` adéquate, et place le curseur à l'intérieur ;
4. on **écrit la logique** du gestionnaire.

Chacune de ces actions visuelles se traduit par du code dans `InitializeComponent` (côté `Form1.Designer.vb`) — création du contrôle, affectation de ses propriétés, ajout à la collection `Controls`.

### Aides à la mise en page

Le Concepteur facilite l'alignement avec les **lignes-guides** (*snap lines*) qui apparaissent au déplacement d'un contrôle, signalant alignements et marges recommandées. Les notions d'ancrage (`Anchor`) et d'accolement (`Dock`), qui assurent une mise en page adaptative, sont approfondies en [5.3](03-controles-fondamentaux.md).

### Le « va-et-vient » Concepteur ↔ code

Le Concepteur et le code entretiennent une relation **bidirectionnelle** (*round-trip*) :

- ce que vous faites dans le Concepteur est **sérialisé** en code VB dans `InitializeComponent` ;
- réciproquement, à l'ouverture, le Concepteur **lit** `InitializeComponent` pour reconstituer la surface.

Comprendre ce mécanisme démystifie le Concepteur : il n'y a pas de « magie » ni de format binaire caché — seulement du code VB.NET lisible, généré et relu. C'est aussi pourquoi le fichier `*.Designer.vb` ne doit pas être bricolé à la main.

### Le Concepteur « hors processus » sur .NET moderne 🆕

Une précision d'architecture propre à .NET moderne : alors que sous .NET Framework le Concepteur s'exécutait *dans* le processus de Visual Studio, les projets .NET (Core et suivants) utilisent un **Concepteur hors processus**. La surface de conception tourne dans un processus distinct, **sur le *runtime* cible du projet** (ici .NET 10), Visual Studio dialoguant avec lui. Ce fonctionnement est transparent au quotidien, mais il explique certains comportements (temps de chargement, prise en charge des contrôles tiers nécessitant une mise à jour compatible).

---

## En résumé

Windows Forms est un *framework* d'interface Windows, orienté objet et piloté par les événements, dont l'exécution repose sur une boucle de messages tournant sur un thread d'interface unique. En VB.NET, une application WinForms s'articule autour de **classes partielles** (votre code d'un côté, le code du Concepteur de l'autre, reliés par `InitializeComponent` et `WithEvents`), démarre le plus souvent via le **Framework d'application** propre à VB, et se construit visuellement grâce au **Concepteur**, dont chaque geste produit du code VB lisible.

Ces fondations posées, la section suivante explore ce que **.NET 10 apporte de neuf** à Windows Forms — mode sombre, formulaires asynchrones et presse-papiers sécurisé → [5.2 Windows Forms sur .NET 10](02-winforms-net10.md).

⏭️ [Windows Forms sur .NET 10 (modernisation : mode sombre intégré, formulaires async ShowAsync/ShowDialogAsync, presse-papiers sécurisé)](/05-windows-forms/02-winforms-net10.md)
