🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 6.1 Introduction ; WPF vs Windows Forms (lequel choisir)

Avant d'entrer dans le XAML, le *binding* ou MVVM, il faut répondre à une question préalable : **WPF est-il le bon choix pour mon projet, ou Windows Forms suffira-t-il ?** Les deux frameworks sont des scénarios de premier rang en VB.NET, tous deux pris en charge sur .NET 10, tous deux limités à Windows. Mais ils reposent sur des philosophies très différentes. Cette section présente WPF, le compare honnêtement à Windows Forms (module [5](../05-windows-forms/README.md)), et propose une grille de décision concrète.

---

## Qu'est-ce que WPF ?

Windows Presentation Foundation est apparu en 2006 avec .NET Framework 3.0 (nom de code *Avalon*). Il introduit une approche de l'interface graphique qui rompt avec celle de Windows Forms sur plusieurs plans :

- **Une UI déclarative en XAML.** L'interface se décrit dans un fichier de balisage XML (`.xaml`), séparé du code de comportement (le *code-behind* ou, mieux, un *view-model* en MVVM). On *décrit* à quoi ressemble l'écran plutôt que de le *construire* instruction par instruction.
- **Un rendu vectoriel accéléré par DirectX.** WPF ne dessine pas avec GDI+ mais via Direct3D, en *mode retenu* (*retained mode*) : on décrit la scène, et le framework se charge de la redessiner quand c'est nécessaire. L'interface est exprimée en unités indépendantes du périphérique (1/96 de pouce), ce qui la rend **naturellement adaptée au HiDPI** et à la mise à l'échelle.
- **Un système de liaison de données puissant.** C'est *la* signature de WPF : un contrôle peut se synchroniser automatiquement avec une propriété d'objet, dans les deux sens, avec conversion et validation. C'est ce qui rend l'architecture MVVM si naturelle (voir [6.4](04-data-binding.md) et [6.6](06-mvvm.md)).
- **Des contrôles « sans apparence » (*lookless*).** Le comportement d'un contrôle est séparé de son apparence : via un *control template*, on peut entièrement re-styliser un `Button` ou une `ListBox` sans rien perdre de sa logique. La personnalisation visuelle est donc **totale**.
- **Une disposition dynamique.** Des panneaux comme `Grid`, `StackPanel` ou `DockPanel` agencent et redimensionnent les éléments automatiquement, au lieu de positions et de tailles figées en pixels.

Sous le capot, WPF s'appuie sur deux mécanismes propres — les **propriétés de dépendance** (*dependency properties*) et les **événements routés** (*routed events*) — qui rendent possibles le *binding*, les styles et la composition. On les rencontrera progressivement dans les sections suivantes.

### Le même écran, dans les deux mondes

Un bouton qui affiche un message illustre bien l'écart de philosophie.

En **Windows Forms**, on raisonne en code impératif (souvent généré par le concepteur), avec des positions absolues :

```vb
' Code-behind WinForms
Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim bouton As New Button() With {
            .Text = "Cliquez-moi",
            .Location = New Point(90, 80)
        }
        AddHandler bouton.Click, AddressOf Bouton_Click
        Controls.Add(bouton)
    End Sub

    Private Sub Bouton_Click(sender As Object, e As EventArgs)
        MessageBox.Show("Bonjour depuis WinForms !")
    End Sub
End Class
```

En **WPF**, on décrit l'interface en XAML, et le code-behind se réduit au comportement :

```xml
<!-- MainWindow.xaml — en VB, x:Class s'écrit SANS le namespace racine (voir 6.2) -->
<Window x:Class="MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Mon application" Height="200" Width="300">
    <Grid>
        <Button Content="Cliquez-moi"
                Width="120" Height="32"
                Click="Bouton_Click" />
    </Grid>
</Window>
```

```vb
' MainWindow.xaml.vb
Class MainWindow
    Private Sub Bouton_Click(sender As Object, e As RoutedEventArgs)
        MessageBox.Show("Bonjour depuis WPF !")
    End Sub
End Class
```

On remarque déjà plusieurs différences : le `Grid` centre et dispose le bouton sans coordonnées en pixels, l'apparence est dans le XAML, et le gestionnaire reçoit un `RoutedEventArgs` (et non un simple `EventArgs`). Et là où WinForms exigerait du code pour pré-remplir un champ, WPF peut le faire par *binding* :

```xml
<TextBox Text="{Binding NomClient}" />
```

Le `TextBox` reste alors synchronisé avec la propriété `NomClient` de l'objet de données, sans code de plomberie. C'est tout l'intérêt du modèle — développé en [6.4](04-data-binding.md).

---

## Windows Forms en bref (rappel)

Windows Forms (2002, .NET Framework 1.0) **enveloppe les contrôles natifs Win32**. Son modèle est plus simple et plus direct : on glisse-dépose des contrôles dans le concepteur, on double-clique pour écrire un gestionnaire d'événement, et le code généré positionne les contrôles en pixels. La liaison de données existe (`BindingSource`), mais reste plus modeste que celle de WPF, et la personnalisation visuelle passe par le dessin manuel (*owner-draw*).

Cette simplicité est une **force réelle** : pour une application de gestion classique, WinForms est plus rapide à mettre en œuvre, plus intuitif pour un débutant, et c'est le terrain le mieux servi par l'espace `My` en VB.NET. Le module 5 lui est entièrement consacré.

---

## Les différences fondamentales

| Critère | Windows Forms | WPF |
|---|---|---|
| Origine | 2002 (.NET Framework 1.0) | 2006 (.NET Framework 3.0) |
| Moteur de rendu | GDI+ / Win32, par pixel | DirectX, vectoriel, mode retenu |
| Définition de l'UI | Code généré par le concepteur | XAML déclaratif + code-behind / MVVM |
| Disposition | Ancrage/amarrage, positions absolues | Panneaux dynamiques (`Grid`, `StackPanel`…) |
| Personnalisation visuelle | Limitée (*owner-draw*) | Totale (styles, *control templates*) |
| Liaison de données | Basique (`BindingSource`) | Riche (bidirectionnelle, convertisseurs, validation) |
| Architecture type | Code-behind | MVVM |
| Animations / multimédia | Très limité | Intégré |
| HiDPI / mise à l'échelle | Configurable (`PerMonitorV2`) | Natif (unités indépendantes) |
| Courbe d'apprentissage | Douce | Plus raide |
| Productivité au démarrage | Élevée (glisser-déposer) | Moindre au départ |
| Plateforme | Windows uniquement | Windows uniquement |
| Multiplateforme | Non | Non |
| .NET 10 | ✅ Mode sombre intégré 🆕 | ✅ Thème Fluent 🆕 |
| En VB.NET | ✅ Scénario **phare** ⭐ | ✅ Pleinement, avec un *caveat* MVVM 🔗 |

Au-delà du tableau, l'écart se résume à un principe : **WinForms privilégie la rapidité et la simplicité ; WPF privilégie la flexibilité et la séparation des responsabilités.** WPF demande d'apprendre XAML, le *binding* et les ressources, mais cet investissement est rentabilisé sur les interfaces complexes, personnalisées et destinées à durer.

---

## Spécificités côté VB.NET

Les deux frameworks sont des modèles de projet **pleinement disponibles en VB.NET** — contrairement à Blazor, MAUI ou WinUI 3, dont le code d'interface est *C# uniquement* (voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)). Deux nuances cependant :

- **Windows Forms reste le scénario phare ⭐ du langage.** C'est l'option la plus répandue dans les bases de code VB, la plus rapide à prendre en main, et celle qui tire le meilleur parti de l'espace `My`.
- **L'outillage MVVM moderne est partiellement hors de portée en VB 🔗.** Les *source generators* de `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]`) sont C# uniquement. En VB.NET, on utilise directement les classes de base de la bibliothèque (`ObservableObject`, `RelayCommand`) : c'est fonctionnel, mais un peu plus verbeux. Détaillé en [6.6](06-mvvm.md).

> 💡 La documentation WPF et la plupart des exemples sont en C#. Gardez l'[Annexe A — Correspondance VB.NET ↔ C#](../annexes/correspondance-vbnet-csharp/README.md) à portée de main, et pensez à toujours préciser « Visual Basic .NET » à un assistant IA.

---

## Peut-on combiner WPF et Windows Forms ?

Oui — l'interopérabilité fonctionne dans les deux sens, ce qui est précieux lors d'une migration progressive :

- **Héberger un contrôle WinForms dans WPF** : via `WindowsFormsHost` (espace de noms `System.Windows.Forms.Integration`).
- **Héberger du contenu WPF dans WinForms** : via `ElementHost`.

C'est utile pour réutiliser un contrôle existant sans tout réécrire. Mais mélanger deux modèles d'interface a un coût (deux systèmes d'événements, de disposition et de rendu à faire cohabiter) : à réserver aux cas où l'on a réellement besoin de l'existant, et non comme architecture par défaut.

---

## Grille de décision : lequel choisir ?

**Choisissez Windows Forms si :**

- vous développez une application de gestion (LOB) classique : formulaires de saisie, grilles de données, rapports ;
- la rapidité de développement prime (concepteur glisser-déposer) ;
- l'équipe maîtrise déjà WinForms ;
- vous maintenez ou étendez une application WinForms existante ;
- vous comptez sur l'espace `My` (paramètres, ressources) pour aller vite ;
- l'apparence standard de Windows vous convient.

**Choisissez WPF si :**

- l'interface doit être riche, sur mesure ou « moderne » (thème personnalisé, animations) ;
- l'application comporte beaucoup de liaison de données ou de visualisation ;
- l'indépendance vis-à-vis de la résolution et le HiDPI sont critiques ;
- vous voulez une architecture testable et découplée via **MVVM** ;
- l'application est complexe et appelée à évoluer dans la durée (la maintenabilité MVVM y est rentable).

**Deux cas particuliers :**

- **Vous débutez en développement de bureau VB.NET ?** Commencez par **Windows Forms** (module 5). La boucle « concevoir → exécuter » y est plus immédiate, et c'est le terrain le mieux balisé du langage. Vous reviendrez à WPF quand vous aurez un besoin précis de ses forces.
- **Vous travaillez sur du code existant ?** Restez cohérent avec lui plutôt que de mêler deux modèles sans raison forte. L'interop dépanne, mais ne remplace pas une décision d'architecture claire.

---

## En résumé

WPF et Windows Forms ne s'opposent pas comme « ancien » et « moderne » : ce sont **deux outils Windows matures, supportés et modernisés sur .NET 10**, adaptés à des besoins différents. WinForms excelle dans la livraison rapide d'applications de gestion et demeure le choix par défaut en VB.NET ⭐. WPF apporte un rendu vectoriel, un *binding* puissant, une personnalisation totale et l'architecture MVVM — au prix d'une courbe d'apprentissage plus marquée et d'un *caveat* sur l'outillage MVVM côté VB 🔗.

Si ce chapitre vous a mené jusqu'ici, c'est sans doute que WPF correspond à votre projet. La suite commence par sa brique de base : le **XAML et les systèmes de disposition** ([6.2](02-xaml-layout.md)).

⏭️ [XAML fondamentaux et systèmes de layout (Grid, StackPanel, DockPanel)](/06-wpf/02-xaml-layout.md)
