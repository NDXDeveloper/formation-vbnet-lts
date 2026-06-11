🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 1.4 Installation et outils

Quel environnement choisir pour développer en VB.NET en 2026 ? La réponse honnête tient en
une phrase : sur Windows, **Visual Studio est l'environnement de référence** ; **VS Code
est utilisable, mais bridé pour VB** ; et, sous l'un comme sous l'autre, c'est le **SDK
.NET** qui fait tourner la machine. Cette section pose ces choix — la création des premiers
projets viendra à la section suivante.

## Le socle commun : le SDK .NET

Quel que soit l'éditeur, il vous faut le **SDK .NET**. Il réunit le *runtime*, les
**compilateurs** (dont celui de VB), la **CLI `dotnet`** et les **modèles de projet**
(→ 1.3). Installer Visual Studio l'embarque automatiquement ; sinon, on installe le SDK
seul depuis la page de téléchargements .NET. Visez le **SDK .NET 10** (LTS). Pour vérifier
qu'il est en place :

```bash
dotnet --info      # versions du SDK et des runtimes installés
dotnet --version   # version du SDK par défaut
```

## Visual Studio 2026 — l'environnement de référence ⭐ 🆕

Disponible depuis le **11 novembre 2025** (.NET Conf 2025), **Visual Studio 2026** est de
loin l'outil le plus complet et le mieux soutenu pour VB.NET : IntelliSense complet,
refactorisations, **débogage intégré**, exécuteur de tests, et surtout le **Concepteur
(*Designer*)** visuel pour Windows Forms et WPF — l'atout décisif du langage.

Côté nouveautés, VS 2026 apporte une interface repensée (**UX Fluent**), un **IDE
« AI-native »** avec une intégration poussée de Copilot, et un chargement nettement plus
rapide des **grosses solutions** multi-projets.

À l'installation, via le **Visual Studio Installer**, choisissez la charge de travail
**« Développement .NET Desktop »** : elle fournit les modèles VB.NET (Windows Forms, WPF)
et le Concepteur. Pour exposer une Web API par contrôleurs, ajoutez **« Développement
ASP.NET et web »** (→ 8.2). VB.NET fonctionne dans toutes les éditions — **Community**
(gratuite pour les particuliers, l'open source et les petites structures), **Professional**
et **Enterprise**.

> 💡 **Copilot est intégré… mais à utiliser avec méthode.** L'assistance IA de VS 2026 est
> précieuse, mais les modèles sont biaisés vers C# : validez systématiquement le code VB
> généré (→ 1.1 et module 17).

## VS Code et la CLI .NET — possible, mais limité pour VB ⚠️

**VS Code** est léger, gratuit et multiplateforme ; la **CLI `dotnet`** y fonctionne
partout. On peut tout à fait créer, compiler et exécuter un projet VB depuis le terminal,
quel que soit l'éditeur :

```bash
dotnet new console -lang VB   # nouveau projet console VB.NET
dotnet build                  # compile
dotnet run                    # exécute
dotnet test                   # lance les tests
```

(Rappel : seules cinq familles de modèles existent en VB — Console, bibliothèque,
Windows Forms, WPF, tests — → 1.6.)

En revanche, **l'expérience d'édition riche de VS Code est, elle, réservée à C#**. Elle
repose sur le **C# Dev Kit** (et les extensions C#/F#), qui apporte IntelliSense avancé,
gestion de solution, tests et débogage — **mais qui ne prend pas en charge VB.NET**. Il
n'existe pas d'équivalent de première classe pour VB. Concrètement, en VB sous VS Code,
vous disposez d'une **édition de base** (au mieux une coloration syntaxique via une
extension tierce), **sans IntelliSense riche, sans concepteur visuel et sans débogage
intégré de premier plan**.

> ⚠️ **Deux limites cumulées pour VB sous VS Code.** D'une part, le C# Dev Kit est
> **C# uniquement**. D'autre part, il **ne prend pas en charge les projets .NET Framework**,
> et le débogueur de VS Code non plus — or une large part du legacy VB.NET vit précisément
> sur .NET Framework. Pour ce code, VS Code n'est pas une option réaliste.

En pratique : VS Code convient pour de l'**édition légère**, de la **lecture**, une
**retouche rapide**, ou un travail **console/bibliothèque en ligne de commande** (y compris
sous Linux/macOS). Mais pour du développement VB.NET sérieux — et tout particulièrement le
**cœur bureau (Windows Forms, WPF)** —, c'est **Visual Studio sur Windows** qu'il faut.

## Une alternative : JetBrains Rider

Pour qui n'utilise pas Visual Studio ou travaille hors de Windows, **JetBrains Rider** est
un IDE multiplateforme qui **prend en charge VB.NET** (édition, compilation, débogage) et
qui est gratuit pour un usage non commercial. Attention toutefois à une limite de taille
pour le cœur VB : son **concepteur Windows Forms est réservé aux projets C#** (et à
Windows) — pour dessiner des formulaires VB, Visual Studio reste incontournable. Rider
n'en demeure pas moins une option crédible pour les bibliothèques, la console ou la
Web API (→ 18.5).

## Quel outil choisir ? (synthèse)

| Votre situation | Outil recommandé |
|---|---|
| Bureau Windows : Windows Forms / WPF (cœur VB) ⭐ | **Visual Studio 2026** (Windows) |
| Maintenance de gros projets / solutions mixtes VB-C# | **Visual Studio 2026** |
| Legacy VB.NET sur **.NET Framework** | **Visual Studio** (VS Code exclu) |
| Console / bibliothèques, multiplateforme (Linux/macOS) | **VS Code + CLI .NET**, ou Rider |
| Édition légère, lecture, retouche rapide | **VS Code** |
| Préférence pour l'écosystème JetBrains | **Rider** |

> 💡 **Un réglage à connaître dès le départ.** Quel que soit l'outil, activez `Option
> Strict On` (par projet, ou par défaut) pour bénéficier d'un typage rigoureux : c'est une
> bonne pratique fondamentale, détaillée en 2.1.

---

Votre environnement est prêt. La section suivante passe à la pratique et construit vos
premiers projets — **Console** puis **Windows Forms** — pas à pas. (→ 1.5)

⏭️ [Premier projet pas à pas (Console et Windows Forms)](/01-introduction-vbnet/05-premier-projet.md)
