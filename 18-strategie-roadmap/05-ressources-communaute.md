🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 18.5 Communauté, documentation, livres, formation continue, outils tiers

Voici la dernière section de la formation : où continuer d'apprendre et trouver du soutien. Une réalité, déjà rencontrée tout au long du cours, en commande la lecture — les ressources sont **C#-first**. La stratégie gagnante du développeur VB.NET n'est donc **pas** de traquer un matériel VB rare : c'est d'utiliser les sources canoniques et de **s'appuyer sur les ressources C#**, en transposant via l'[Annexe A](../annexes/correspondance-vbnet-csharp/README.md). Les cinq catégories ci-dessous se lisent avec ce prisme.

> **La thèse récurrente, appliquée à l'apprentissage.** Les ressources spécifiques à VB sont rares — parce que l'écosystème est C#-first et le langage figé. Le bon réflexe est de **lire le C# couramment et de le transposer**, pas d'attendre un matériel VB qui ne viendra pas.

---

## Documentation

La référence canonique est **Microsoft Learn** (la documentation .NET et VB, `learn.microsoft.com/dotnet/visual-basic`) : la référence du langage VB, l'explorateur d'API .NET, et la page de stratégie déjà citée en [18.1](01-strategie-microsoft.md). Pour les nouveautés de la plateforme, le **blog .NET** (`devblogs.microsoft.com/dotnet`) — c'est notamment là que sont annoncés des changements comme l'extension du support STS vue en [18.2](02-roadmap-dotnet.md).

Le tempérament à garder, là encore : les **exemples** de la documentation sont C#-first. Le développeur VB les transpose mentalement, avec l'[Annexe A](../annexes/correspondance-vbnet-csharp/README.md) pour compagnon. Ce n'est pas un défaut à corriger, c'est la réalité avec laquelle on compose.

---

## Communauté

Là où les questions VB trouvent réponse : **Stack Overflow** (l'étiquette `vb.net`), **Microsoft Q&A**, et la communauté .NET au sens large (les espaces Reddit comme r/dotnet et r/visualbasic). Sur **GitHub**, le dépôt `dotnet/vblang` accueille les discussions sur le langage — peu actif, conséquence logique du gel — et le dépôt Roslyn concerne le compilateur.

La réalité : la communauté **spécifiquement VB** est plus petite que celle de C#, et l'essentiel du savoir communautaire .NET est exprimé en C# tout en s'appliquant à VB. Engagez la communauté .NET dans son ensemble, et transposez au besoin.

---

## Livres

Soyons francs : les livres consacrés à VB.NET sont **rares et, pour la plupart, antérieurs à l'ère moderne** — conséquence directe du langage figé et d'un marché C#-first où l'on publie peu de nouveaux ouvrages VB.

Mais le gel a un revers heureux : **les classiques sur les fondamentaux de VB.NET ne se périment pas sur le langage**, puisque celui-ci n'a pas bougé depuis la version 16.9. Un bon ouvrage ancien sur les bases de VB reste exact sur le langage lui-même.

La stratégie, dès lors : pour l'apprentissage de la **plateforme et des concepts** (EF Core, asynchronie, *patterns*, architecture), appuyez-vous sur l'abondante littérature **fondée sur C#** et sur les ouvrages .NET — ils enseignent des concepts applicables à VB, l'[Annexe A](../annexes/correspondance-vbnet-csharp/README.md) faisant le pont syntaxique. N'attendez pas un livre spécifiquement VB qui pourrait ne jamais paraître.

---

## Formation continue

Une question mérite d'être posée franchement : que signifie « continuer d'apprendre » un langage **figé** ? On n'apprend pas de **nouvelle syntaxe** VB — il n'y en a pas. On continue d'apprendre les **parties mobiles autour** du langage, et c'est là que va le budget de formation :

1. **La plateforme .NET**, qui évolue chaque année (bibliothèque de classes, EF Core, performance — [14.6](../14-performance/06-apports-net10.md), [7.2](../07-acces-donnees/02-ef-core-10.md)) et se **consomme** depuis VB. On apprend la plateforme, pas un nouveau VB.
2. **Le C#** — pour le lire et le transposer ([Annexe A](../annexes/correspondance-vbnet-csharp/README.md)), et pour la stratégie hybride ([module 10](../10-hybride-vbnet-csharp/README.md)). Pour un développeur VB, **lire** le C# couramment est sans doute la compétence au plus fort effet de levier.
3. **L'écosystème et l'outillage** : Visual Studio 2026 ([Annexe D](../annexes/visual-studio-2026/README.md)), les flux de travail assistés par IA ([module 17](../17-developpement-ia/README.md)), les *patterns* et l'architecture.

Le cadrage est utile précisément parce qu'il est contre-intuitif : votre apprentissage continu vise la **plateforme et C#**, non un langage statique.

---

## Outils tiers

La réalité : l'outillage tiers **spécifiquement VB** est clairsemé, mais la **plupart des outils .NET fonctionnent pour VB**, même documentés C#-first — les analyseurs (Roslyn, SonarQube — [13.4](../13-tests-qualite/04-analyse-statique.md)), les frameworks de test (xUnit, NUnit, MSTest — [13.1](../13-tests-qualite/01-tests-unitaires.md)), le *mocking* (Moq, NSubstitute — [13.2](../13-tests-qualite/02-mocking-tdd.md)), la conversion VB↔C# (ICSharpCode.CodeConverter — [18.4](04-migrer-vers-csharp.md)), et les assistants IA ([module 17](../17-developpement-ia/README.md)).

La vérification à faire avant d'adopter un outil : que la **prise en charge de VB ne soit pas un parent pauvre**. Certains outillages sont C#-only — **StyleCop** ([13.4](../13-tests-qualite/04-analyse-statique.md)), l'écriture de *source generators* ([Annexe B](../annexes/frontiere-vbnet-csharp/README.md)), et l'exemple cautionnaire des générateurs de `CommunityToolkit.Mvvm`, **inertes en VB** ([6.6](../06-wpf/06-mvvm.md), [17.8](../17-developpement-ia/08-cas-concrets.md)). Sans oublier la limite VS Code / C# Dev Kit ([1.4](../01-introduction-vbnet/04-installation-outils.md)). En clair : vérifiez que l'outil sert VB, pas seulement C#.

---

## Pour finir — la formation

Ce module final a transformé le **positionnement honnête** en décisions et en ressources. Et toute la formation tient en une même ligne directrice : **VB.NET en 2026 est un langage stable et soutenu, pour un ensemble de tâches bien défini** — Windows Forms, bibliothèques, données, interopérabilité, maintenance de legacy, Web API par contrôleurs — qui vit dans un monde C#-first.

Les compétences qui rendent un développeur VB efficace découlent toutes de ce constat : **connaître ce terrain**, **lire le C# couramment** pour exploiter l'écosystème, recourir à la **stratégie hybride** pour ce que VB ne fait pas, et **travailler avec l'IA** malgré son biais C#. La stratégie de ressources de cette section reflète d'ailleurs celle du cours entier : **tirer parti de l'écosystème C#-first plutôt que de lutter contre lui**, garder le jugement au-dessus des outils, et décider sur des besoins concrets.

Deux compagnons resteront ouverts bien après la dernière page : l'[Annexe A](../annexes/correspondance-vbnet-csharp/README.md) — pour lire et transposer le C# omniprésent — et l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md) — pour savoir, à chaque instant, où passe la frontière. Tout a commencé par un [positionnement lucide](../01-introduction-vbnet/README.md) ; tout y revient. Bon code.

⏭️ [Correspondance syntaxique VB.NET ↔ C# (aide-mémoire)](/annexes/correspondance-vbnet-csharp/README.md)
