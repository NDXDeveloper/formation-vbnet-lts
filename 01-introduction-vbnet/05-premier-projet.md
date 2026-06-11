🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 1.5 Premier projet pas à pas (Console et Windows Forms)

Pour voir concrètement comment un projet VB.NET est **structuré** et **s'exécute**, nous
allons parcourir et commenter deux projets de départ : une **application Console**, puis une
**application Windows Forms**. Cette section est une **démonstration commentée** — vous
pouvez la suivre dans votre environnement, mais rien n'y est imposé. Les exemples utilisent
**Visual Studio 2026**, l'environnement de référence (→ 1.4), avec les équivalents en ligne
de commande lorsqu'ils sont utiles.

## Projet 1 — Une application Console

### Création

Dans Visual Studio 2026 : **Nouveau projet → « Application console » (Visual Basic) →** on
nomme le projet, on cible **.NET 10**, puis **Créer**. En ligne de commande, l'équivalent
tient en deux lignes :

```bash
dotnet new console -lang VB -o MaConsole
cd MaConsole && dotnet run
```

### Le code généré

Le projet contient un unique fichier `Program.vb` au contenu minimal :

```vb
Imports System

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")
    End Sub
End Module
```

Décortiquons-le :

- `Imports System` — rend l'espace de noms `System` accessible sans qualification
  (l'instruction `Imports` est détaillée en 2.1). Cette ligne, générée par le modèle, est
  en réalité **optionnelle** : `System` figure parmi les **imports par défaut** de tout
  projet VB — une liste d'espaces de noms importés globalement, propre à VB, visible dans
  **Propriétés du projet → Références** (et alimentée par des éléments `<Import>` du
  `.vbproj`) — et le code compilerait donc sans elle.
- `Module Program` — un **module**, conteneur de membres partagés propre à VB (→ 3.5).
- `Sub Main(args As String())` — le **point d'entrée** du programme ; `args` reçoit les
  éventuels arguments de ligne de commande.
- `Console.WriteLine(...)` — écrit une ligne dans la console.

> 💡 **VB n'a pas de *top-level statements*.** Là où C# (depuis C# 9, et par défaut dans
> les modèles de projet .NET 6+) autorise à écrire directement `Console.WriteLine("…")`
> sans `Main` explicite, VB conserve la structure `Module` / `Sub Main`. C'est une
> différence mineure mais révélatrice du caractère explicite de VB (→ Annexe B). À garder
> en tête face au code C# généré par l'IA.

### Exécution

On lance avec **F5** (avec débogage) ou **Ctrl+F5** (sans débogage) dans Visual Studio, ou
`dotnet run` en ligne de commande. La console affiche alors `Hello World!`.

## Projet 2 — Une application Windows Forms

### Création

Dans Visual Studio 2026 : **Nouveau projet → « Application Windows Forms » (Visual Basic)
→** on nomme le projet, on cible **.NET 10**, puis **Créer** (la charge de travail
« Développement .NET Desktop » doit être installée, → 1.4). On arrive **directement dans le
Concepteur (*Designer*)**, face à un formulaire `Form1` vide. En ligne de commande,
`dotnet new winforms -lang VB` crée un projet équivalent — mais **structuré différemment**
(on y revient ci-dessous) et, surtout, sans Concepteur : l'édition visuelle suppose Visual
Studio (→ 1.4).

### Les fichiers du projet

Un projet Windows Forms VB **créé dans Visual Studio** est plus riche qu'un projet
console :

- **`Form1.vb`** — votre code : gestionnaires d'événements et logique du formulaire.
- **`Form1.Designer.vb`** — généré automatiquement par le Concepteur (contrôles, position,
  propriétés). On **ne l'édite pas à la main**.
- **`Form1.resx`** — les ressources du formulaire.
- **`ApplicationEvents.vb`** (espace de noms `My`) — le **cadre applicatif** : une classe
  partielle `MyApplication` pour les événements globaux de l'application.

Côté fichier projet (`.vbproj`, format SDK-style), on retrouve `<UseWindowsForms>true</UseWindowsForms>`,
`<MyType>WindowsForms</MyType>` et une cible `net10.0-windows` (Windows Forms dépend de
Windows).

### Le démarrage : le cadre applicatif VB (≠ C#)

C'est ici que VB se distingue nettement de C#. En C#, un projet WinForms possède un fichier
`Program.cs` explicite avec `ApplicationConfiguration.Initialize()` puis
`Application.Run(new Form1())`. En **VB sous Visual Studio**, rien de tout cela : le projet
utilise le **cadre applicatif** (*Application Framework*), un démarrage **piloté par
événements** dans lequel :

- on **n'écrit pas de `Sub Main`** ;
- le **formulaire de démarrage** se choisit dans **Propriétés du projet → Application** ;
- le fichier `ApplicationEvents.vb` permet de gérer les **événements globaux** :
  `Startup`, `Shutdown`, `UnhandledException`, `StartupNextInstance` (application à instance
  unique), `NetworkAvailabilityChanged`.

> 💡 **Besoin d'un `Sub Main` explicite ?** C'est possible : il suffit de décocher
> « Activer le cadre d'application » et de désigner `Sub Main` comme objet de démarrage.
> Mais pour la majorité des applications de bureau, le cadre applicatif (et l'espace `My`,
> → 2.12) est un vrai gain de productivité.

À ce propos : le modèle **CLI** (`dotnet new winforms -lang VB`) fait précisément ce choix
du démarrage explicite. Il ne génère ni `ApplicationEvents.vb` ni `Form1.resx`, mais un
`Program.vb` « à la C# », déclaré comme objet de démarrage
(`<StartupObject>Sub Main</StartupObject>`) :

```vb
Friend Module Program

    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1)
    End Sub

End Module
```

Les deux structures produisent la même application ; elles illustrent simplement les
**deux modes de démarrage** possibles d'un projet Windows Forms VB. (Au passage :
`New Form1` s'écrit sans parenthèses — en VB, elles sont optionnelles quand le constructeur
ne prend pas d'argument.) Dans la suite de la formation, on privilégie le projet Visual
Studio et son cadre applicatif.

### Ajouter un contrôle et gérer un événement

Pour illustrer le modèle événementiel, voici ce qui se passe lorsqu'on dépose un **Button**
depuis la Boîte à outils sur le formulaire, puis qu'on **double-clique** dessus dans le
Concepteur : Visual Studio génère automatiquement un gestionnaire d'événement, qu'il ne
reste qu'à compléter :

```vb
Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MessageBox.Show("Bonjour depuis VB.NET !")
    End Sub
End Class
```

À noter :

- `Public Class Form1` — la classe du formulaire. Elle est **partielle** : l'autre moitié
  vit dans `Form1.Designer.vb` (→ 3.5).
- `Private Sub Button1_Click(...) Handles Button1.Click` — la **clause `Handles`** relie la
  méthode à l'événement `Click` du bouton. C'est **l'idiome VB** de gestion des événements
  (équivalent du `+=` de C#), un point fort que nous approfondirons (→ 3.6).
- `MessageBox.Show(...)` — affiche une boîte de dialogue.

### Exécution

On lance avec **F5** : le formulaire apparaît, et un clic sur le bouton affiche la boîte de
message. L'application tourne.

### Les petits plus de .NET 10 (à explorer au module 5)

Sur .NET 10, Windows Forms se modernise : **mode sombre intégré**, **formulaires
asynchrones** (`ShowAsync` / `ShowDialogAsync`), presse-papiers sécurisé (sérialisation
JSON), protection anti-capture d'écran, et divers raffinements. Ces sujets sont traités en
détail au **module 5** ; l'essentiel ici est d'avoir vu **comment un projet est structuré
et démarre**.

## Ce que ces deux projets nous apprennent

- **Console** : une structure minimale (`Module` / `Sub Main`), et le rappel que VB n'a
  **pas de *top-level statements***.
- **Windows Forms** : le **Concepteur**, la **découpe en classes partielles**
  (`Form1.vb` / `Form1.Designer.vb`) et le **cadre applicatif** (démarrage événementiel,
  espace `My`) — un modèle de productivité spécifique à VB.
- La **clause `Handles`** : la manière idiomatique de gérer les événements en VB.

Ces fondations reviendront tout au long de la formation : module 5 pour Windows Forms, 3.5
pour les modules et classes partielles, 3.6 pour les événements, 2.12 pour l'espace `My`.

---

Nous avons vu **ce qu'est** VB.NET (1.1), **d'où il vient** (1.2), son **écosystème**
(1.3), son **outillage** (1.4) et un **premier projet** (1.5). La section suivante — la
section pivot du module — prend de la hauteur pour répondre à la question stratégique : le
**positionnement honnête** de VB.NET en 2026. (→ 1.6)

⏭️ [VB.NET en 2026 : positionnement honnête](/01-introduction-vbnet/06-positionnement-2026.md)
