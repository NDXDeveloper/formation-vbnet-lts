🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.5 Styles, ressources, templates et triggers

La section [6.1](01-introduction-wpf-vs-winforms.md) promettait des contrôles « sans apparence » (*lookless*), entièrement re-stylisables ; [6.4](04-data-binding.md) déclarait des convertisseurs « comme ressources » sans s'expliquer. Cette section tient ces promesses. Elle décrit le **système d'apparence** de WPF, qui sépare radicalement le fond (un contrôle, sa logique) de la forme (son habillage). Quatre briques l'articulent, du plus simple au plus puissant : **ressources**, **styles**, **triggers**, **templates**.

---

## Les ressources

Une **ressource** est un objet réutilisable — un pinceau, un style, un template, un convertisseur — rangé dans un dictionnaire et identifié par une clé **`x:Key`**. Tout élément possède une propriété `Resources` ; en pratique, on regroupe les ressources sur la fenêtre ou, mieux, au niveau de l'application.

```xml
<Window.Resources>
    <SolidColorBrush x:Key="CouleurAccent" Color="#0078D4" />
    <sys:Double x:Key="MargeStandard">8</sys:Double>
</Window.Resources>
```

On les référence ensuite par leur clé, le plus souvent via l'extension `{StaticResource}` :

```xml
<Border Background="{StaticResource CouleurAccent}" />
```

### Recherche et portée

La résolution d'une ressource **remonte l'arbre visuel** depuis l'élément demandeur, puis consulte les ressources de l'application, puis celles du thème système. La ressource **la plus proche l'emporte** : on peut donc redéfinir localement une couleur déclarée globalement.

### `StaticResource` vs `DynamicResource`

- **`StaticResource`** — résolue **une fois**, au chargement. C'est le cas courant, le plus performant.
- **`DynamicResource`** — résolue à l'exécution et **réévaluée** si la ressource change. À réserver à ce qui varie en cours d'utilisation (couleurs système, changement de thème — voir [6.8](08-fluent-design-net10.md)).

### Dictionnaires fusionnés

Pour organiser un projet, on éclate les ressources en fichiers `ResourceDictionary` distincts, fusionnés via `MergedDictionaries` :

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Styles/Couleurs.xaml" />
            <ResourceDictionary Source="Styles/Boutons.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

---

## Les styles

Un **`Style`** regroupe des affectations de propriétés (`Setter`) appliquées à un type de contrôle (`TargetType`). C'est l'outil de cohérence visuelle par excellence.

```xml
<Style x:Key="BoutonPrincipal" TargetType="Button">
    <Setter Property="Background" Value="{StaticResource CouleurAccent}" />
    <Setter Property="Foreground" Value="White" />
    <Setter Property="Padding" Value="12,6" />
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

```xml
<Button Content="Enregistrer" Style="{StaticResource BoutonPrincipal}" />
```

Deux distinctions essentielles :

- **Style nommé (explicite)** — doté d'un `x:Key`, il s'applique uniquement là où on le demande (`Style="{StaticResource …}"`).
- **Style implicite** — déclaré **sans `x:Key`** mais avec un `TargetType`, il s'applique **automatiquement** à toutes les instances de ce type dans sa portée. Idéal pour habiller d'un coup tous les `TextBlock` ou tous les `Button` d'une fenêtre.

```xml
<!-- Style implicite : tous les TextBlock de la portée -->
<Style TargetType="TextBlock">
    <Setter Property="FontSize" Value="13" />
    <Setter Property="Margin" Value="0,2" />
</Style>
```

Enfin, les styles **s'héritent** via `BasedOn`, qui reprend un style existant pour le spécialiser :

```xml
<Style x:Key="Titre" TargetType="TextBlock" BasedOn="{StaticResource Corps}">
    <Setter Property="FontSize" Value="20" />
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

---

## Les triggers

Un **trigger** applique des `Setter` de façon **conditionnelle**, et — avantage décisif sur le code-behind — **rétablit automatiquement** l'état initial dès que la condition n'est plus remplie. On les déclare dans `Style.Triggers` (ou dans un template). Plusieurs variantes existent :

- **`Trigger`** (de propriété) — se déclenche quand une propriété atteint une valeur, par exemple `IsMouseOver` ou `IsEnabled`.
- **`DataTrigger`** — se déclenche selon une **valeur liée** ; très utile en MVVM pour réagir à l'état du *view-model*.
- **`MultiTrigger`** / **`MultiDataTrigger`** — combinent plusieurs conditions (ET logique).
- **`EventTrigger`** — réagit à un événement routé ([6.2](02-xaml-layout.md)), surtout pour lancer une animation (voir [6.7](07-animations-multimedia.md)).

```xml
<Style TargetType="Button">
    <Setter Property="Background" Value="LightGray" />
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="SteelBlue" />
        </Trigger>
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Opacity" Value="0.5" />
        </Trigger>
    </Style.Triggers>
</Style>
```

Un `DataTrigger` réagit, lui, à une donnée — ici, griser le texte d'un client inactif :

```xml
<Style TargetType="TextBlock">
    <Style.Triggers>
        <DataTrigger Binding="{Binding EstActif}" Value="False">
            <Setter Property="Foreground" Value="Gray" />
        </DataTrigger>
    </Style.Triggers>
</Style>
```

---

## Les templates

Le *template* est la customisation la plus profonde : il **redéfinit l'arbre visuel** d'un élément. C'est ici que se concrétise l'idée de contrôle *lookless* — le comportement (la classe `Button`) et l'apparence (son template) sont entièrement disjoints. WPF distingue deux templates, qu'il ne faut pas confondre.

### `ControlTemplate` — l'apparence d'un contrôle

Un **`ControlTemplate`** décrit la structure visuelle d'un contrôle. On peut donc remplacer intégralement l'aspect d'un `Button` tout en conservant sa logique. Deux mécanismes y sont centraux :

- **`TemplateBinding`** — relie une propriété d'un élément du template à une propriété du contrôle hôte (par exemple, le `Background` du `Border` reprend le `Background` du bouton) ;
- **`ContentPresenter`** — l'emplacement où s'insère le `Content` du contrôle (pour un `ItemsControl`, ce serait `ItemsPresenter`).

```xml
<Style TargetType="Button">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border x:Name="fond"
                        Background="{TemplateBinding Background}"
                        Padding="{TemplateBinding Padding}"
                        CornerRadius="6">
                    <ContentPresenter HorizontalAlignment="Center"
                                      VerticalAlignment="Center" />
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter TargetName="fond" Property="Opacity" Value="0.85" />
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

> ⚠️ **En redéfinissant le template, on perd l'apparence par défaut**, y compris les états visuels (survol, appui, focus). Il faut les recréer via les **triggers de template** — comme ci-dessus pour `IsMouseOver` (certains contrôles modernes gèrent leurs états via le `VisualStateManager`, abordé en [6.7](07-animations-multimedia.md)).

> 💡 **Astuce pratique :** plutôt que de partir de zéro, faites un clic droit sur le contrôle dans Visual Studio → *Modifier le modèle* → *Modifier une copie*. L'IDE injecte le template par défaut complet dans votre XAML, prêt à être ajusté.

### `DataTemplate` — l'apparence d'une donnée

Un **`DataTemplate`** décrit, à l'inverse, comment **un objet de données** s'affiche. C'est lui qu'utilisaient les `ItemTemplate` et `CellTemplate` de la section [6.3](03-controles.md).

```xml
<DataTemplate x:Key="ModeleClient">
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="{Binding Nom}" FontWeight="Bold" />
        <TextBlock Text="{Binding Ville}" Margin="8,0,0,0" Foreground="Gray" />
    </StackPanel>
</DataTemplate>
```

Comme les styles, un `DataTemplate` peut être **implicite** : déclaré avec `DataType="{x:Type local:Client}"` et sans clé, il s'applique automatiquement partout où un objet `Client` doit être rendu. Ce mécanisme est très commode en MVVM ([6.6](06-mvvm.md)) pour associer une vue à un type de *view-model*.

### Ne pas confondre les deux

- **`ControlTemplate`** = à quoi ressemble un **contrôle** (sa structure, son habillage).
- **`DataTemplate`** = à quoi ressemble une **donnée** (le contenu présenté).

Les deux se composent : une `ListBox` utilise son `ControlTemplate` pour le cadre et la barre de défilement, et un `DataTemplate` (via `ItemTemplate`) pour chacun de ses éléments.

---

## Côté VB.NET

Bonne nouvelle pour ce chapitre : **tout le système d'apparence est purement déclaratif (XAML)** — styles, ressources, triggers et templates sont rigoureusement **identiques en VB.NET et en C#**. C'est l'un des domaines où la frontière entre les deux langages s'efface complètement : aucun *source generator*, aucune syntaxe avancée n'entre en jeu.

Les seules portions de code restent les classes auxiliaires — convertisseurs (`IValueConverter`, [6.4](04-data-binding.md)) ou sélecteurs de template (`DataTemplateSelector`) — que l'on écrit en VB comme n'importe quelle classe. Quant au thème **Fluent** de .NET 10 ([6.8](08-fluent-design-net10.md)), il n'est rien d'autre qu'un vaste jeu de ressources et de styles bâti sur exactement ces mécanismes.

---

## En résumé

Le système d'apparence de WPF se construit en couches. Les **ressources** stockent des objets réutilisables, résolus par `StaticResource` (au chargement) ou `DynamicResource` (à l'exécution), et organisés en dictionnaires fusionnés. Les **styles** y appliquent des `Setter`, de façon explicite (par clé) ou **implicite** (par type), avec héritage via `BasedOn`. Les **triggers** rendent ces réglages **conditionnels** et réversibles, en réaction à une propriété (`Trigger`) ou à une donnée (`DataTrigger`). Enfin, les **templates** offrent la customisation totale : `ControlTemplate` pour l'habillage d'un contrôle, `DataTemplate` pour le rendu d'une donnée. Le tout en XAML pur, sans la moindre différence entre VB.NET et C#.

Ces mécanismes, combinés à la liaison de données, sont précisément ceux qui rendent l'architecture **MVVM** naturelle — c'est l'objet de la section [6.6](06-mvvm.md).

⏭️ [Architecture MVVM](/06-wpf/06-mvvm.md)
