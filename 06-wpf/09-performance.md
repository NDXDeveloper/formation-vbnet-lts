🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.9 Performance WPF (virtualisation, binding, rendu)

Grâce à son rendu vectoriel accéléré par le GPU ([6.1](01-introduction-wpf-vs-winforms.md)), WPF est performant par défaut. Mais quelques choix d'architecture font ou défont la fluidité — particulièrement face aux **grandes listes**. Cette dernière section du chapitre se concentre sur les trois leviers décisifs : la **virtualisation**, les **liaisons** et le **rendu**, en gardant un principe de tête : **mesurer avant d'optimiser** (outils au [module 14](../14-performance/README.md)).

---

## La virtualisation de l'interface

C'est le levier le plus important. Sans précaution, un contrôle de liste pourrait créer un élément visuel **pour chaque donnée** : avec dix mille éléments, l'interface s'effondre. La **virtualisation** ne génère de conteneurs que pour les éléments **visibles** (plus une marge), et les recycle au défilement.

Les contrôles `ListBox`, `ListView` et `DataGrid` virtualisent **par défaut**, via un `VirtualizingStackPanel`. On affine ce comportement avec des propriétés attachées :

```xml
<ListBox ItemsSource="{Binding GrandeListe}"
         VirtualizingPanel.IsVirtualizing="True"
         VirtualizingPanel.VirtualizationMode="Recycling"
         VirtualizingPanel.ScrollUnit="Item" />
```

- **`VirtualizationMode="Recycling"`** — réutilise les conteneurs au lieu d'en créer et d'en jeter ; nettement plus efficace que le mode `Standard`.
- **`ScrollUnit`** — défilement par élément (`Item`) ou par pixel (`Pixel`).

Le `DataGrid` virtualise les lignes (`EnableRowVirtualization`, actif par défaut) et peut aussi virtualiser les colonnes (`EnableColumnVirtualization`).

### Les tueurs de virtualisation

Plus utile encore que les réglages : savoir ce qui **désactive silencieusement** la virtualisation. Trois pièges reviennent constamment.

- **Donner une hauteur infinie à la liste.** Placer un contrôle de liste dans un `StackPanel` (ou un `ScrollViewer`) qui le mesure sans contrainte de hauteur force la création de **tous** les éléments.

```xml
<!-- ❌ Le StackPanel mesure la liste sur une hauteur infinie : aucune virtualisation -->
<StackPanel>
    <ListBox ItemsSource="{Binding GrandeListe}" />
</StackPanel>

<!-- ✅ La liste reçoit une hauteur bornée et virtualise -->
<DockPanel>
    <ListBox ItemsSource="{Binding GrandeListe}" />
</DockPanel>
```

- **Remplacer l'`ItemsPanel` par un panneau non virtualisant** (un simple `StackPanel`, un `WrapPanel`…). Pour conserver la virtualisation, le panneau d'éléments doit être un `VirtualizingStackPanel`.
- **Mettre `ScrollViewer.CanContentScroll="False"`**, qui bascule en défilement par pixel et désactive la virtualisation par élément.

---

## Optimiser les liaisons

Les liaisons sont peu coûteuses individuellement, mais leur nombre et leurs réglages comptent.

- **Choisir le bon mode** ([6.4](04-data-binding.md)). Une donnée en lecture seule n'a pas besoin de `TwoWay` : préférez `OneWay`, voire `OneTime` pour ce qui ne change jamais — moins de suivi, moins de surcharge.
- **Éliminer les erreurs de liaison.** Chaque liaison en échec est journalisée (fenêtre *Sortie*, `System.Windows.Data Error`) et **consomme du CPU**. Corriger ces avertissements est un gain immédiat — et un code plus propre.
- **Ne pas sur-notifier.** Déclencher `PropertyChanged` à l'excès, ou pour « toutes les propriétés » (chaîne vide), alourdit inutilement l'interface.
- **Charger les collections en masse.** Une `ObservableCollection` lève **un événement par ajout** : insérer des milliers d'éléments un à un est lent. Construisez d'abord une `List`, puis **affectez-la d'un coup** à la propriété liée, ou utilisez une collection gérant les ajouts par lots.
- **Garder les convertisseurs légers** : ils s'exécutent par élément, donc autant de fois qu'il y a de lignes.

> 💡 Pour des **volumes de données** vraiment massifs, la virtualisation de l'interface ne suffit plus : on recourt à la **virtualisation des données** (charger les enregistrements à la demande), une notion qui touche aussi à l'accès aux données (Partie 3).

---

## Le rendu

WPF redessine en mode retenu sur le GPU ; quelques habitudes en réduisent encore le coût.

- **Geler les `Freezable`.** Pinceaux, géométries et transformations qui ne changent pas gagnent à être *gelés* : immuables, ils deviennent thread-safe et cessent d'émettre des notifications de changement.

```vb
Dim pinceau As New SolidColorBrush(Colors.SteelBlue)
pinceau.Freeze()   ' immuable, thread-safe, moins coûteux
```

- **Alléger les templates d'éléments.** Un `DataTemplate` aux multiples niveaux imbriqués voit son coût **multiplié par le nombre d'éléments**. Pour une grande liste, gardez le modèle de ligne aussi simple que possible.
- **Décoder les images à leur taille d'affichage.** Charger une photo en pleine résolution pour l'afficher en vignette gaspille mémoire et CPU ; `DecodePixelWidth` règle le problème.

```xml
<Image>
    <Image.Source>
        <BitmapImage UriSource="photo.jpg" DecodePixelWidth="200" />
    </Image.Source>
</Image>
```

- **Animer les bonnes propriétés.** Comme vu en [6.7](07-animations-multimedia.md), animez `Opacity` et `RenderTransform` (composés sur le GPU), jamais `Width`/`Height`/`Margin`, qui relancent le calcul de disposition ([6.2](02-xaml-layout.md)) à chaque image.
- **Mettre en cache le complexe et statique** via `CacheMode="BitmapCache"`, pour éviter de redessiner un visuel coûteux qui ne bouge pas.
- **Ne jamais bloquer le thread d'interface.** Tout traitement long sur le thread UI fige le rendu : déportez-le en asynchrone ([module 4](../04-async/README.md)).

> ℹ️ Selon l'environnement (certaines VM, bureau distant), WPF peut basculer en rendu **logiciel**, plus lent. La propriété `RenderCapability.Tier` indique le niveau d'accélération matérielle disponible — à vérifier sur les postes cibles (à noter : .NET 8 a apporté l'accélération matérielle pour les applications de bureau distant).

---

## Mesurer avant d'optimiser

La règle d'or de la performance vaut ici aussi : **profiler d'abord, optimiser ensuite**. Les outils — profileur de Visual Studio, `dotnet-trace`, `dotnet-counters` — sont présentés au [module 14](../14-performance/README.md). On y rappelle aussi ce que **.NET 10 apporte « gratuitement »** : au-delà des optimisations du runtime ([14.6](../14-performance/06-apports-net10.md)), WPF a bénéficié en .NET 10 d'allocations réduites et de conversions de format de pixels accélérées — des gains obtenus **sans toucher au code**.

---

## Côté VB.NET

Les leviers de performance de WPF sont **architecturaux et déclaratifs** : réglages de virtualisation, modes de liaison, gel des `Freezable`, asynchronie. Tous sont **identiques en VB.NET et en C#** — une dernière confirmation que, sur le terrain du bureau, VB.NET reste à parité complète.

---

## En résumé

La performance d'une application WPF tient surtout à trois leviers. La **virtualisation** — active par défaut, mais qu'il faut se garder de désactiver par mégarde — est la clé des grandes listes. Les **liaisons** gagnent à employer le bon mode, à n'émettre aucune erreur et à charger les collections en bloc. Le **rendu** s'allège en gelant les `Freezable`, en simplifiant les templates, en décodant les images à la bonne taille et en réservant l'animation aux propriétés composées sur le GPU. Le tout en mesurant avant d'agir — et, comme partout dans ce chapitre, à parité entre VB.NET et C#.

---

## Conclusion du chapitre

Ce chapitre a parcouru WPF de bout en bout : le **choix** entre WPF et Windows Forms ([6.1](01-introduction-wpf-vs-winforms.md)), le **XAML** et la disposition ([6.2](02-xaml-layout.md)), les **contrôles** ([6.3](03-controles.md)), la **liaison de données** qui en est le cœur ([6.4](04-data-binding.md)), l'habillage par **styles et templates** ([6.5](05-styles-templates.md)), l'architecture **MVVM** ([6.6](06-mvvm.md)), les **animations** ([6.7](07-animations-multimedia.md)), le **thème Fluent** de .NET 10 ([6.8](08-fluent-design-net10.md)) et enfin la **performance**.

Deux enseignements traversent l'ensemble. D'abord, WPF est un **scénario pleinement viable en VB.NET** ✅ : seul l'outillage MVVM par *source generators* fait défaut ([6.6](06-mvvm.md)), une verbosité supplémentaire sans aucune perte de fonctionnalité. Ensuite, WPF brille là où Windows Forms montre ses limites — interfaces sur mesure, liaison de données riche, séparation testable — ce qui en fait le bon outil pour les applications de bureau complexes et durables.

La suite quitte l'interface pour la **persistance** : l'**accès aux données** (Partie 3, [module 7](../07-acces-donnees/README.md)), qui donnera enfin de vraies sources à toutes ces liaisons.

⏭️ [Accès aux données](/07-acces-donnees/README.md)
