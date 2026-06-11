🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.7 Animations et multimédia (notions)

WPF embarque un moteur d'animation et de multimédia complet — héritage de son rendu vectoriel accéléré ([6.1](01-introduction-wpf-vs-winforms.md)). Dans une application VB.NET de gestion, on n'en exploite généralement qu'une fraction : des transitions discrètes, un retour visuel, parfois la lecture d'un média. Cette section, conforme à son intitulé « notions », en présente donc le **modèle** et les **briques principales** — de quoi reconnaître ces mécanismes et les employer avec mesure, sans viser une maîtrise exhaustive.

---

## Le modèle d'animation

Une animation WPF fait **varier une propriété de dépendance dans le temps**, en interpolant entre une valeur de départ et une valeur d'arrivée. Le tout est avant tout **déclaratif** : on décrit l'animation en XAML, le moteur s'occupe du minutage et du rendu image par image.

À chaque type de propriété correspond un type d'animation : `DoubleAnimation` (opacité, largeur, échelle…), `ColorAnimation` (couleurs), `ThicknessAnimation` (marges), `PointAnimation`, etc. Pour des trajectoires plus riches (plusieurs étapes), on utilise les variantes par **images-clés**, comme `DoubleAnimationUsingKeyFrames`.

---

## Le `Storyboard` et son déclenchement

Les animations se regroupent dans un **`Storyboard`**, qui les orchestre et les cible via les propriétés attachées `Storyboard.TargetName` et `Storyboard.TargetProperty`. Reste à décider **quand** le déclencher :

- **En XAML**, via un **`EventTrigger`** (rencontré en [6.5](05-styles-templates.md)) qui réagit à un événement routé ([6.2](02-xaml-layout.md)) — `Loaded`, `MouseEnter`, `Click`… — et lance le storyboard avec `BeginStoryboard`. Les `Trigger`/`DataTrigger` de propriété, eux, démarrent une animation via leurs `EnterActions`/`ExitActions`.
- **En code**, via la méthode `BeginAnimation` d'un élément.

Un fondu d'apparition au chargement, entièrement déclaratif :

```xml
<Border Opacity="0">
    <Border.Triggers>
        <EventTrigger RoutedEvent="Border.Loaded">
            <BeginStoryboard>
                <Storyboard>
                    <DoubleAnimation Storyboard.TargetProperty="Opacity"
                                     From="0" To="1" Duration="0:0:0.4" />
                </Storyboard>
            </BeginStoryboard>
        </EventTrigger>
    </Border.Triggers>
    <!-- contenu -->
</Border>
```

Le même effet, piloté en VB.NET :

```vb
Imports System.Windows.Media.Animation

Dim fondu As New DoubleAnimation(0, 1, New Duration(TimeSpan.FromMilliseconds(400)))
panneau.BeginAnimation(UIElement.OpacityProperty, fondu)
```

---

## Les fonctions d'accélération (*easing*)

Une interpolation linéaire paraît mécanique. Les **fonctions d'accélération** rendent le mouvement naturel en modulant sa progression : `QuadraticEase`, `CubicEase`, `BackEase`, `BounceEase`, `ElasticEase`… Chacune se règle avec un `EasingMode` (`EaseIn`, `EaseOut`, `EaseInOut`).

```xml
<DoubleAnimation Storyboard.TargetProperty="Opacity"
                 From="0" To="1" Duration="0:0:0.4">
    <DoubleAnimation.EasingFunction>
        <QuadraticEase EasingMode="EaseOut" />
    </DoubleAnimation.EasingFunction>
</DoubleAnimation>
```

---

## Les états visuels : le `VisualStateManager`

Pour les **états** d'un contrôle (`Normal`, `MouseOver`, `Pressed`, `Disabled`), WPF propose une approche plus structurée que les triggers épars : le **`VisualStateManager`**. On y déclare des groupes d'états et leurs **transitions**, généralement au sein d'un `ControlTemplate` ([6.5](05-styles-templates.md)). C'est d'ailleurs le mécanisme qu'emploient les templates par défaut des contrôles modernes ; il vaut donc d'en connaître l'existence dès qu'on personnalise l'apparence d'un contrôle.

---

## Le multimédia

WPF sait afficher images, sons et vidéos :

- **`MediaElement`** — lit l'**audio et la vidéo**. On contrôle la lecture par `Source`, `LoadedBehavior` (`Manual` pour piloter soi-même), puis `Play()`, `Pause()`, `Stop()` et `Position`.
- **`Image`** — affiche une image (`Source`, `Stretch`), déjà croisé en [6.3](03-controles.md).
- **Sons simples** — `SoundPlayer` (espace `System.Media`) pour un fichier WAV, ou `SystemSounds` pour les bips système.
- **Graphiques 2D/3D** — les formes (`Rectangle`, `Ellipse`, `Path`) pour le dessin vectoriel, et `Viewport3D` pour la 3D (notions).

```xml
<MediaElement x:Name="lecteur" Source="intro.mp4"
              LoadedBehavior="Manual" Width="640" Height="360" />
```

```vb
lecteur.Play()
```

> ⚠️ Le `MediaElement` s'appuie sur les **codecs multimédias de Windows** présents sur la machine. La prise en charge d'un format dépend donc du poste : à valider sur l'environnement cible, surtout pour des formats vidéo peu courants.

---

## Performance et bon usage

Toutes les animations ne se valent pas. Animer **`Opacity`** ou une **`RenderTransform`** (translation, échelle, rotation) est composé sur le **GPU** et reste peu coûteux. Animer `Width`, `Height` ou `Margin`, en revanche, **relance le calcul de disposition** (*measure*/*arrange*, [6.2](02-xaml-layout.md)) à chaque image — coûteux et souvent saccadé. La règle pratique : pour bouger ou redimensionner un élément, **préférez `RenderTransform` aux propriétés de mise en page**. Ces points reviennent au chapitre [6.9](09-performance.md).

Dans une application de gestion, l'animation est un outil de **finition** — fluidifier une transition, signaler un changement — et non une fin en soi : une touche discrète sert l'utilisateur, l'excès le distrait.

---

## Côté VB.NET

Comme les styles et les templates ([6.5](05-styles-templates.md)), animations et multimédia sont **essentiellement déclaratifs**, donc **identiques en VB.NET et en C#**. Les seules portions de code — démarrer une animation par `BeginAnimation`, piloter un `MediaElement` — s'écrivent en VB sans la moindre particularité. C'est, une fois de plus, un domaine où la frontière entre les deux langages n'existe pas.

---

## En résumé

Une animation WPF interpole une **propriété de dépendance** dans le temps ; on la regroupe dans un **`Storyboard`** déclenché par un `EventTrigger` (ou en code via `BeginAnimation`), et on l'adoucit avec une fonction d'**accélération**. Les **états** d'un contrôle relèvent plutôt du `VisualStateManager`. Le **multimédia** s'appuie sur `MediaElement` (audio/vidéo, tributaire des codecs du poste), `Image` et les sons système. Côté performance, on privilégie `Opacity` et `RenderTransform`, composés sur le GPU. Le tout étant déclaratif, VB.NET y est à parité complète avec C#.

Reste une touche résolument moderne : le **thème Fluent et le mode sombre** apportés par .NET 10 ([6.8](08-fluent-design-net10.md)).

⏭️ [Thèmes et Fluent Design (.NET 10)](/06-wpf/08-fluent-design-net10.md)
