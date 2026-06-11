🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.8 Thèmes et Fluent Design (.NET 10) 🆕

WPF traînait depuis des années une apparence d'origine (le thème *Aero2*) figée à l'esthétique de Windows 7. La grande modernisation visuelle est venue en **deux temps** : le **thème Fluent** a été introduit en **.NET 9**, puis **étendu et corrigé en .NET 10**. C'est la touche moderne promise dès [6.1](01-introduction-wpf-vs-winforms.md) — un habillage façon Windows 11, avec modes clair/sombre intégrés, posé sur l'existant sans réécriture.

> ⚠️ **Point d'honnêteté d'emblée :** le thème Fluent est encore **en cours de finalisation et marqué expérimental**. Il modernise efficacement une application, mais comporte des lacunes et des comportements à valider (détaillés plus bas). À aborder comme une amélioration réelle mais évolutive, pas comme un système figé.

---

## Le thème Fluent : un coup de neuf pour WPF

Le thème Fluent apporte à WPF une esthétique **Windows 11** : coins arrondis, styles de contrôles rafraîchis, et trois acquis majeurs — un **mode clair et un mode sombre intégrés**, et le **suivi de la couleur d'accentuation** du système. Techniquement, ce n'est rien d'autre qu'un vaste jeu de **ressources et de styles** bâti sur les mécanismes vus en [6.5](05-styles-templates.md) : on l'active, et les contrôles adoptent automatiquement la nouvelle apparence. Il vise l'allure de Windows 11 mais fonctionne aussi sous Windows 10.

---

## Activer le thème Fluent

Deux voies mènent au même résultat ; elles restent d'ailleurs synchronisées entre elles.

### Par le dictionnaire de ressources (XAML)

On fusionne le dictionnaire Fluent dans les ressources — au niveau de l'application (tous les écrans) ou d'une seule fenêtre. C'est la voie la plus directe, dans le prolongement des `MergedDictionaries` de [6.5](05-styles-templates.md) :

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/PresentationFramework.Fluent;component/Themes/Fluent.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### Par la propriété `ThemeMode`

Plus pratique, la propriété `ThemeMode` — disponible sur `Application` et sur `Window` — applique le style Fluent sans référencer directement le dictionnaire. Elle accepte quatre valeurs :

- **`None`** — valeur **par défaut** : aucun thème Fluent, retour à l'apparence classique (*Aero2*) ;
- **`System`** — clair ou sombre, selon les réglages de Windows ;
- **`Light`** — thème Fluent clair ;
- **`Dark`** — thème Fluent sombre.

Un réglage au niveau **fenêtre prime** sur celui de l'application. En XAML :

```xml
<Window ThemeMode="System" ... >
```

Et en VB.NET :

```vb
' Au niveau de l'application
Application.Current.ThemeMode = ThemeMode.Dark
' Ou au niveau de la fenêtre courante
Me.ThemeMode = ThemeMode.System
```

> ⚠️ **`ThemeMode` est une API expérimentale.** Y accéder déclenche l'erreur de compilation `WPF0001`, qu'il faut neutraliser au niveau du projet :
> ```xml
> <PropertyGroup>
>     <NoWarn>$(NoWarn);WPF0001</NoWarn>
> </PropertyGroup>
> ```
> Ce statut expérimental implique de possibles changements cassants dans de futures versions de .NET : à garder à l'esprit, et à revérifier sur la version exacte que vous ciblez.

---

## Couleur d'accentuation et contraste élevé

Dès que le thème Fluent est actif, l'application **suit la couleur d'accentuation de Windows** et la répercute **en temps réel** si l'utilisateur la modifie. Les pinceaux d'accentuation correspondants (dans `Light.xaml`, `Dark.xaml`, `HC.xaml`) sont accessibles via `DynamicResource` ([6.5](05-styles-templates.md)) pour styliser vos propres contrôles de façon cohérente.

Le **contraste élevé** est géré automatiquement : si un thème à contraste élevé est activé dans Windows, c'est la variante haute-visibilité de Fluent qui s'applique, sans plus distinguer clair et sombre.

---

## Un état d'avancement à connaître

C'est ici que l'honnêteté de cette formation s'impose. En .NET 10, le thème Fluent **progresse mais reste incomplet** ; nombre de correctifs y ont été apportés (animation de l'`Expander`, plantages en contraste élevé, disposition droite-à-gauche de plusieurs contrôles), et de nombreux styles ont été ajoutés. Mais des écarts subsistent face au Fluent de WinUI 3 :

- des **contrôles modernes manquent** (par exemple `ToggleSwitch`, `NavigationView`, `SplitButton`, `ProgressRing`…) ;
- certaines commodités attendues ne sont pas là (le `StackPanel` n'a pas de propriété `Spacing`, le `TextBox` pas de `PlaceholderText`/*watermark*) ;
- de par son mode d'implémentation, **appliquer des styles personnalisés** par-dessus le thème peut produire des résultats inattendus.

En pratique :

- pour donner **rapidement** une allure moderne (clair/sombre, accent système) à une application VB.NET de bureau, le thème Fluent natif est un excellent choix — à condition de **valider le rendu** sur les contrôles utilisés ;
- pour disposer du **jeu complet** de contrôles Windows 11, on se tourne vers WinUI 3 ou des bibliothèques de thèmes tierces — sujets qui sortent du périmètre VB (voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

---

## Au passage : la syntaxe abrégée de `Grid` (.NET 10)

Indépendamment du thème, .NET 10 apporte une amélioration XAML qui simplifie ce que nous écrivions en [6.2](02-xaml-layout.md) : une **syntaxe abrégée** pour déclarer les lignes et colonnes d'une `Grid` directement en chaîne, avec prise en charge du *Hot Reload* XAML.

```xml
<!-- .NET 10 : forme concise -->
<Grid RowDefinitions="Auto, *, 40" ColumnDefinitions="120, *">
    <!-- ... -->
</Grid>
```

Soit l'équivalent compact des verbeux blocs `<Grid.RowDefinitions>` / `<Grid.ColumnDefinitions>`. Un petit confort, mais représentatif des raffinements que .NET 10 apporte à WPF « gratuitement », sans changer la logique du code.

---

## Côté VB.NET

Comme l'apparence en général ([6.5](05-styles-templates.md)), le thème Fluent est **déclaratif** ou se réduit à une simple propriété : son usage est **identique en VB.NET et en C#**, jusqu'à la suppression de l'avertissement `WPF0001`, qui se fait au niveau du fichier projet (donc à l'identique pour un projet VB). Encore un domaine où VB.NET est à parité complète — aucune frontière hybride ici.

---

## En résumé

Le **thème Fluent**, introduit en .NET 9 et **étendu en .NET 10**, modernise l'apparence de WPF à la façon de Windows 11 : modes **clair/sombre** intégrés et **couleur d'accentuation** système, posés sur le système de ressources et de styles. On l'active via le **dictionnaire `Fluent.xaml`** ou la propriété **`ThemeMode`** (`None`/`System`/`Light`/`Dark`), cette dernière encore **expérimentale** (`WPF0001`). Il reste **en cours de finalisation** — des contrôles et commodités manquent face à WinUI — donc parfait pour un coup de neuf rapide, à valider au cas par cas. Le tout, comme toujours en matière d'apparence, à parité totale entre VB.NET et C#.

Reste un dernier sujet transversal, déjà effleuré à propos des animations : la **performance** des applications WPF ([6.9](09-performance.md)).

⏭️ [Performance WPF (virtualisation, binding, rendu)](/06-wpf/09-performance.md)
