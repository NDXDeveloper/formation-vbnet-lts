🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.2 XAML fondamentaux et systèmes de layout (`Grid`, `StackPanel`, `DockPanel`)

Si la section [6.1](01-introduction-wpf-vs-winforms.md) a établi *pourquoi* choisir WPF, celle-ci en pose la première brique : **le XAML**, le langage déclaratif qui décrit l'interface, et les **panneaux de disposition** qui agencent les éléments à l'écran. Comprendre ces deux mécanismes, c'est tenir la grammaire de tout ce qui suivra — contrôles, *binding*, styles et MVVM.

---

## XAML : le balisage déclaratif de WPF

XAML (*eXtensible Application Markup Language*) est un dialecte XML qui décrit des objets et leurs propriétés. Sa règle fondatrice est simple : **chaque élément XAML correspond à l'instanciation d'un objet .NET, et chaque attribut à l'affectation d'une propriété**.

```xml
<Button Content="OK" Width="80" Background="LightBlue" />
```

Cette ligne équivaut, conceptuellement, à créer un `Button` et à renseigner ses propriétés `Content`, `Width` et `Background`. On *décrit* l'interface au lieu de la construire instruction par instruction — c'est tout l'écart avec le code-behind impératif de Windows Forms.

### Les espaces de noms XAML

En tête de chaque fichier, deux espaces de noms sont quasi systématiques :

```xml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:MonApp">
```

- Le namespace **par défaut** (sans préfixe) donne accès aux contrôles WPF (`Button`, `Grid`, `TextBox`…).
- Le préfixe **`x:`** expose les fonctionnalités du langage XAML lui-même : `x:Class`, `x:Name`, `x:Key`, `x:Type`, `x:Null`…
- Un préfixe **personnalisé** (ici `local`) référence vos propres classes via `clr-namespace:`.

### Syntaxe d'élément de propriété

Quand une valeur est trop riche pour tenir dans une chaîne d'attribut, on utilise la *syntaxe d'élément de propriété* : un sous-élément de la forme `<Type.Propriété>`.

```xml
<Button Content="Dégradé">
    <Button.Background>
        <LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
            <GradientStop Color="LightBlue" Offset="0" />
            <GradientStop Color="SteelBlue" Offset="1" />
        </LinearGradientBrush>
    </Button.Background>
</Button>
```

### Propriété de contenu

La plupart des contrôles désignent une propriété de **contenu** que l'on peut renseigner directement, sans la nommer. Pour un `Button`, c'est `Content` :

```xml
<Button>Cliquez-moi</Button>   <!-- équivaut à Content="Cliquez-moi" -->
```

Et comme `Content` accepte **n'importe quel objet**, on peut y placer une composition entière — ce qui serait impossible avec un bouton WinForms :

```xml
<Button Width="130">
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="💾" Margin="0,0,6,0" />
        <TextBlock Text="Enregistrer" />
    </StackPanel>
</Button>
```

### Propriétés attachées

Certaines propriétés sont **définies par un type mais affectées sur un autre** : ce sont les *propriétés attachées*, omniprésentes en disposition. Par exemple, `Grid.Row` est défini par `Grid` mais se règle sur l'enfant placé dans la grille :

```xml
<TextBox Grid.Row="1" Grid.Column="0" />
```

On les reconnaît à leur forme `Conteneur.Propriété`. `Grid.Row`, `Grid.Column`, `DockPanel.Dock` et `Canvas.Left` sont les plus courantes.

### Extensions de balisage

Les valeurs entre **accolades** sont des *extensions de balisage* : au lieu d'une valeur littérale, elles invoquent un mécanisme. Les plus fréquentes :

- `{Binding NomClient}` — liaison de données (développée en [6.4](04-data-binding.md)) ;
- `{StaticResource MaCouleur}` — référence à une ressource (voir [6.5](05-styles-templates.md)) ;
- `{x:Null}`, `{x:Static SystemColors.ControlBrush}` — valeurs spéciales du langage.

### Relier le XAML au code

Deux attributs de l'espace `x:` font le lien avec le code-behind :

- **`x:Class="MainWindow"`** associe le fichier XAML à une classe partielle. Un fichier généré fournit la méthode `InitializeComponent`, qui analyse le XAML et construit l'arbre d'éléments au lancement de la fenêtre.
- **`x:Name="zoneNom"`** (ou simplement `Name` sur un `FrameworkElement`) crée un champ accessible depuis le code-behind, pour lire ou modifier l'élément par programme.

> ⚠️ **Piège de transposition C# → VB, vérifié sur .NET 10.** En C#, `x:Class` contient le nom **complet** de la
> classe (`x:Class="MonApp.MainWindow"`). En VB.NET, il est interprété **relativement au *Root Namespace*** du
> projet : on écrit `x:Class="MainWindow"` **sans** le préfixe. Recopier la forme C# avec le namespace produit
> une classe `MonApp.MainWindow` distincte de votre code-behind — et une erreur déroutante du type
> `BC30456 : 'Bouton_Click' n'est pas un membre de 'MainWindow'` dès qu'un gestionnaire est référencé
> dans le XAML.

```xml
<TextBox x:Name="zoneNom" />
```

```vb
' Dans le code-behind, l'élément est accessible par son nom
zoneNom.Text = "Valeur par défaut"
```

---

## L'arbre d'éléments et les événements routés

Le XAML, par son imbrication, construit un **arbre d'éléments** — la fenêtre contient un panneau, qui contient des contrôles… WPF en distingue deux vues : l'**arbre logique** (les éléments que vous avez déclarés) et l'**arbre visuel**, plus détaillé, qui inclut aussi les éléments internes issus des *templates* (notion approfondie en [6.5](05-styles-templates.md)). C'est le long de cet arbre que se résolvent les ressources, que s'hérite le `DataContext` ([6.4](04-data-binding.md))… et que **voyagent les événements**.

Car c'est ici que se concrétise le second mécanisme fondateur annoncé en [6.1](01-introduction-wpf-vs-winforms.md) : les **événements routés** (*routed events*). Contrairement à un événement WinForms, qui ne concerne que le contrôle émetteur, un événement routé **parcourt l'arbre** :

- la plupart **remontent** (*bubbling*) de l'élément source vers la racine — c'est le cas de `Click`, `MouseDown`, `KeyDown` ;
- leurs jumeaux préfixés **`Preview`** (`PreviewMouseDown`, `PreviewKeyDown`…) **descendent** d'abord de la racine vers la source (*tunneling*) — l'occasion d'intercepter une interaction *avant* que le contrôle visé ne la reçoive ;
- poser **`e.Handled = True`** arrête le voyage.

L'intérêt pratique : un **parent peut traiter les événements de tous ses enfants**. Un seul gestionnaire sur le panneau suffit pour tous les boutons — l'écho WPF du `Handles` multiple de Windows Forms ([5.6](../05-windows-forms/06-evenements.md)) :

```xml
<!-- L'événement attaché ButtonBase.Click est capté au niveau du panneau -->
<StackPanel ButtonBase.Click="Boutons_Click">
    <Button Content="Accueil" />
    <Button Content="Clients" />
    <Button Content="Rapports" />
</StackPanel>
```

```vb
Private Sub Boutons_Click(sender As Object, e As RoutedEventArgs)
    Dim bouton = DirectCast(e.Source, Button)   ' e.Source : l'émetteur réel
    Naviguer(bouton.Content.ToString())
    e.Handled = True                            ' stoppe la remontée
End Sub
```

Notez la nuance entre **`sender`** (l'élément où le gestionnaire est attaché — ici le `StackPanel`) et **`e.Source`** (l'élément qui a déclenché l'événement — le bouton cliqué). Ce mécanisme sous-tend aussi les `EventTrigger` des styles et animations ([6.5](05-styles-templates.md), [6.7](07-animations-multimedia.md)).

---

## Le système de disposition : *measure* puis *arrange*

WPF n'utilise pas de positions en pixels : il **calcule** la mise en page en deux passes. D'abord *Measure* — chaque élément indique la taille qu'il souhaite, compte tenu de l'espace disponible. Puis *Arrange* — le parent attribue à chaque enfant son rectangle définitif. C'est ce qui rend les interfaces WPF **fluides au redimensionnement** : on décrit des intentions de disposition, le framework s'occupe des dimensions réelles.

Trois propriétés gouvernent le comportement d'un élément dans son conteneur :

- **Alignement** — `HorizontalAlignment` (`Left`, `Center`, `Right`, `Stretch`) et `VerticalAlignment` (`Top`, `Center`, `Bottom`, `Stretch`). Par défaut, beaucoup d'éléments sont en `Stretch` (ils remplissent l'espace offert).
- **Marges et remplissage** — `Margin` est l'espace **autour** de l'élément, `Padding` l'espace **intérieur** (pour les contrôles à contenu). Une épaisseur accepte 1, 2 ou 4 valeurs : `Margin="10"` (partout), `Margin="10,5"` (horizontal, vertical) ou `Margin="10,5,10,5"` (gauche, haut, droite, bas).
- **Dimensionnement** — `Width`/`Height` fixent une taille ; `MinWidth`/`MaxWidth` (et équivalents en hauteur) posent des bornes. Sans ces propriétés, l'élément se dimensionne à son contenu ou s'étire.

---

## Les panneaux de disposition

Un *panneau* est un conteneur qui sait positionner ses enfants. WPF en fournit plusieurs ; trois couvrent l'immense majorité des besoins.

### StackPanel

Le `StackPanel` **empile** ses enfants, verticalement par défaut, ou horizontalement avec `Orientation="Horizontal"`. Chaque enfant prend sa taille désirée dans le sens d'empilement et s'étire dans l'autre. C'est l'outil des barres de boutons, des petites listes et des formulaires simples.

```xml
<StackPanel Orientation="Horizontal" Margin="8">
    <Button Content="OK" Width="80" Margin="4,0" />
    <Button Content="Annuler" Width="80" Margin="4,0" />
</StackPanel>
```

> ⚠️ Un `StackPanel` ne contraint pas la taille dans le sens d'empilement : il laisse ses enfants prendre toute la place dont ils ont besoin. Pour répartir l'espace ou borner les dimensions, préférez un `Grid`.

### Grid — le panneau le plus polyvalent

La `Grid` organise ses enfants en **lignes et colonnes**, définies par `RowDefinitions` et `ColumnDefinitions`. Chaque dimension peut adopter trois modes :

- **`Auto`** — la ligne/colonne se dimensionne à son contenu ;
- **fixe** (ex. `Height="40"`) — taille exacte ;
- **étoile** (`*`, `2*`) — partage proportionnel de l'espace restant.

Les enfants se placent via les propriétés attachées `Grid.Row` et `Grid.Column`, et peuvent s'étendre avec `Grid.RowSpan` / `Grid.ColumnSpan`.

```xml
<Grid Margin="12">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="120" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <TextBlock Text="Nom :" Grid.Row="0" Grid.Column="0"
               VerticalAlignment="Center" />
    <TextBox Grid.Row="0" Grid.Column="1" Margin="0,4" />

    <TextBlock Text="Courriel :" Grid.Row="1" Grid.Column="0"
               VerticalAlignment="Center" />
    <TextBox Grid.Row="1" Grid.Column="1" Margin="0,4" />

    <TextBox Grid.Row="2" Grid.ColumnSpan="2"
             AcceptsReturn="True" TextWrapping="Wrap" Margin="0,8" />
</Grid>
```

> ⚠️ **Piège classique du débutant :** un enfant sans `Grid.Row`/`Grid.Column` est placé en cellule `(0,0)`. Si plusieurs éléments oublient ces attributs, ils se **superposent** silencieusement dans la première cellule.

### DockPanel

Le `DockPanel` **ancre** ses enfants contre les bords du conteneur grâce à la propriété attachée `DockPanel.Dock` (`Top`, `Bottom`, `Left`, `Right`). Avec `LastChildFill="True"` (la valeur par défaut), le **dernier** enfant occupe tout l'espace restant. C'est l'outil idéal pour la *coquille* d'une application : menu en haut, barre d'état en bas, contenu au centre.

```xml
<DockPanel LastChildFill="True">
    <Menu DockPanel.Dock="Top">
        <MenuItem Header="_Fichier" />
        <MenuItem Header="_Édition" />
    </Menu>
    <StatusBar DockPanel.Dock="Bottom">
        <TextBlock Text="Prêt" />
    </StatusBar>

    <!-- Dernier enfant, sans Dock : remplit l'espace restant -->
    <Grid Background="White">
        <!-- contenu principal -->
    </Grid>
</DockPanel>
```

> 💡 Dans un `DockPanel`, **l'ordre de déclaration compte** : chaque enfant ancré « consomme » son bord avant le suivant. Déclarer la barre d'état avant le contenu garantit qu'elle reste collée en bas.

### Les autres panneaux (en bref)

| Panneau | Disposition | Usage typique |
|---|---|---|
| `StackPanel` | Empile (vertical/horizontal) | Barres de boutons, petites listes, formulaires simples |
| `Grid` | Lignes × colonnes | Formulaires, mises en page complexes (le plus polyvalent) |
| `DockPanel` | Ancrage aux bords | Coquille d'application (menu, barre d'état, contenu) |
| `WrapPanel` | Empile avec retour à la ligne | Galeries, étiquettes, vignettes |
| `Canvas` | Positions absolues (`Canvas.Left`/`Top`) | Dessin, graphiques (rare en UI classique) |
| `UniformGrid` | Cellules de taille égale | Damiers, grilles régulières |

---

## Composer les panneaux

Une mise en page réaliste **imbrique** plusieurs panneaux : on emboîte simplement un conteneur dans un autre. Le squelette d'une fenêtre d'application combine typiquement les trois panneaux principaux.

```xml
<DockPanel LastChildFill="True">

    <Menu DockPanel.Dock="Top">
        <MenuItem Header="_Fichier" />
    </Menu>

    <StatusBar DockPanel.Dock="Bottom">
        <TextBlock Text="Prêt" />
    </StatusBar>

    <!-- Zone centrale : une grille à deux colonnes -->
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="200" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <!-- Volet latéral : une pile de boutons -->
        <StackPanel Grid.Column="0" Margin="8">
            <Button Content="Accueil" Margin="0,4" />
            <Button Content="Clients" Margin="0,4" />
            <Button Content="Rapports" Margin="0,4" />
        </StackPanel>

        <!-- Détail -->
        <Grid Grid.Column="1" Background="White">
            <!-- contenu de la page -->
        </Grid>
    </Grid>

</DockPanel>
```

Cette logique d'emboîtement — un panneau dans une cellule d'un autre — est le cœur de la conception d'interfaces WPF.

---

## Manipuler la disposition en code (VB.NET)

Tout ce qui se décrit en XAML peut aussi se faire en code, ce qui rassure les développeurs venant de Windows Forms. Les propriétés attachées s'y règlent via les méthodes statiques `SetXxx` du conteneur :

```vb
Dim champ As New TextBox()
Grid.SetRow(champ, 1)
Grid.SetColumn(champ, 0)
grilleSaisie.Children.Add(champ)
```

En pratique, on réserve le code aux cas dynamiques (construire une interface au moment de l'exécution) et l'on privilégie le XAML pour tout le reste : il est plus lisible, plus concis, et c'est la forme attendue par l'outillage (concepteur, *binding*, styles).

> 💡 La documentation et les exemples WPF sont massivement en C#. Pour transposer un extrait, gardez l'[Annexe A — Correspondance VB.NET ↔ C#](../annexes/correspondance-vbnet-csharp/README.md) sous la main : le XAML, lui, est **identique** quel que soit le langage de code-behind.

---

## En résumé

Le XAML décrit l'interface comme un arbre d'objets : un élément par objet, un attribut par propriété, enrichis par les syntaxes d'élément de propriété, les propriétés attachées et les extensions de balisage. Le long de cet arbre voyagent les **événements routés** — montants (`Click`) ou descendants (`Preview*`) — qu'un parent peut intercepter pour tous ses enfants. La disposition ne repose pas sur des pixels mais sur un calcul en deux passes (*measure*/*arrange*), piloté par l'alignement, les marges et le dimensionnement. Trois panneaux suffisent à la quasi-totalité des écrans : **`StackPanel`** pour empiler, **`Grid`** pour les grilles et les formulaires, **`DockPanel`** pour la coquille — et l'on obtient des interfaces complexes en les **imbriquant**.

Munis de cette structure, nous pouvons à présent la peupler : place aux **contrôles et contrôles de données** ([6.3](03-controles.md)).

⏭️ [Contrôles et contrôles de données (DataGrid, ListView)](/06-wpf/03-controles.md)
