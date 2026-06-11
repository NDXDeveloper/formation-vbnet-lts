🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 1.1 Qu'est-ce que VB.NET et à quoi il sert réellement en 2026

La question mérite une réponse directe, car elle conditionne tout le reste de la
formation. Avant de parler syntaxe, outils ou architecture, il faut savoir **ce qu'est**
ce langage et, surtout, **à quoi il sert vraiment** aujourd'hui — sans euphémisme ni
nostalgie.

## Une définition simple

Visual Basic .NET (VB.NET) est un **langage de programmation** moderne, **orienté objet**
et **fortement typé**, développé par Microsoft pour la plateforme **.NET**. Aux côtés de
C# et de F#, c'est l'un des trois langages officiels de l'écosystème — et un **citoyen de
première classe** : son code est compilé en **code intermédiaire** (IL), s'exécute sur le
**runtime .NET** (le CLR) et accède à l'intégralité de la **bibliothèque de classes de
base** (BCL) ainsi qu'à tout l'écosystème NuGet.

Autrement dit, VB.NET et C# partagent l'essentiel :

- le même **runtime** et le même **ramasse-miettes** (GC) ;
- la **même bibliothèque standard** — tout ce que C# fait avec `System.*`, VB.NET le fait
  aussi ;
- la même **plateforme de compilation** (Roslyn), qui héberge les deux compilateurs ;
- la capacité à **consommer n'importe quel paquet NuGet**.

La différence entre les deux ne tient donc **pas** à ce que le langage peut « atteindre »
(les API et les bibliothèques sont les mêmes), mais à sa **syntaxe** — verbeuse, explicite
et lisible, héritée de la tradition Visual Basic (`Dim`, `Sub`/`End Sub`, `Imports`…) — et,
on va le voir, à son **rythme d'évolution**.

Un aperçu en quelques lignes, avant le vrai premier projet (→ 1.5) :

```vb
Dim villes As New List(Of String) From {"Paris", "Lyon", "Marseille"}

For Each ville In villes
    Console.WriteLine($"Bonjour {ville} !")
Next
```

Des mots en toutes lettres (`Dim`, `For Each`…`Next`), pas d'accolades ni de
points-virgules : le code se lit presque comme une phrase. On y croise aussi des traits
pleinement modernes, identiques à C# — génériques (`List(Of String)`), initialiseur de
collection, interpolation de chaînes `$"…"`.

> ⚠️ **VB.NET n'est pas Visual Basic 6.** Malgré un nom commun et une syntaxe de surface
> familière, VB.NET (2002) est un langage **entièrement différent** de Visual Basic 6
> (« VB classique »), bâti sur .NET et non sur l'ancien *runtime* COM. La distinction est
> capitale, en particulier face aux assistants IA, qui confondent volontiers les deux.
> Cette filiation est l'objet de la **section 1.2**.

## Un langage stabilisé — et ce que cela change

Le fait le plus important à comprendre en 2026 tient en un mot : VB.NET est un langage
**stabilisé**. Depuis le tournant stratégique de 2020, Microsoft **maintient** le langage
(correctifs de sécurité, compatibilité avec les nouveaux *runtimes*, expérience dans
Visual Studio) mais **n'y ajoute plus de nouvelle syntaxe à écrire**. La position
officielle — exposée dans la *Stratégie du langage Visual Basic* (Microsoft Learn) — tient
en deux idées : une approche de **consommation seule**, et l'absence d'extension du langage
à de **nouveaux workloads**.

Cela ne veut pas dire que plus rien ne bouge sous le capot. La dernière version du langage
documentée avec des nouveautés est **VB 16.9** (2021) — et elle n'apportait déjà plus
qu'une capacité de *consommation* (utiliser les propriétés `init-only` définies en C#).
Depuis, le compilateur n'a reçu que de rares incréments du même ordre : ainsi **VB 17.13**
(livrée avec Visual Studio 2022 17.13, début 2025) se limite à prendre en compte l'attribut
`OverloadResolutionPriority` lors de la résolution de surcharges. Aucun de ces incréments
n'introduit de **nouvelle construction à écrire** — c'est pourquoi on résume couramment la
situation par « langage **figé à VB 16.9** ». En pratique : ce que vous écrivez au
quotidien en VB.NET n'a plus évolué depuis ~2020.

> 💡 **« Stabilisé » ne veut pas dire « abandonné ».** VB.NET est **pleinement pris en
> charge** sur .NET 10 LTS (supporté jusqu'au 14 novembre 2028), reçoit les correctifs de
> sécurité et profite des gains de performance de chaque version de .NET (→ 1.3 et 14.6).
> Ce qui est figé, c'est la **syntaxe d'écriture** du langage — pas son exécution, ni son
> support, ni ses performances.

Cette stabilisation a une conséquence directe : certaines fonctionnalités modernes de C# se
**consomment** depuis VB mais ne se **déclarent** pas en VB (les *records*, les propriétés
`init`…), tandis que d'autres restent purement côté C# (les *top-level statements*, le
*pattern matching* avancé, la déclaration de membres d'interface par défaut…). Nous
y reviendrons en détail (→ 3.7 et Annexe B). Loin d'être un cul-de-sac, c'est le point de
départ de la stratégie présentée plus loin.

## À quoi VB.NET sert réellement en 2026

La stabilisation n'est pas un verdict d'inutilité : c'est une **carte**. Elle indique
précisément les domaines où VB.NET est non seulement viable, mais un **bon choix,
pleinement pris en charge**. Microsoft elle-même désigne ses « scénarios cœur » : Windows
Forms et les bibliothèques. La formation étend cette carte aux domaines réellement
pertinents :

- **Applications de bureau Windows** ⭐ — le scénario phare.
  - **Windows Forms** : le terrain le plus naturel et le mieux soutenu, modernisé sur
    .NET 10 (mode sombre et formulaires asynchrones désormais intégrés en standard,
    protection anti-capture d'écran, presse-papiers sécurisé). (→ module 5)
  - **WPF** : pour des interfaces plus riches, en XAML avec le motif MVVM. (→ module 6)
- **Bibliothèques de classes** — réutilisables et partageables (y compris avec des projets
  C#) : un scénario cœur explicitement cité par Microsoft.
- **Accès aux données et applications métier (LOB)** — ADO.NET et Entity Framework Core 10,
  avec LINQ (un vrai point fort de VB) : le terrain de prédilection des logiciels de
  gestion. (→ module 7)
- **Interopérabilité COM et automation Office** (Excel, Word, Outlook) — une **force
  historique** de VB, toujours d'actualité. (→ 9.2)
- **Maintenance et migration de code legacy** — VB6 et .NET Framework, un pan considérable
  du parc applicatif d'entreprise. (→ module 11)
- **Web API par contrôleurs** — exposer des services REST avec ASP.NET Core reste réaliste
  et pris en charge en VB.NET. (→ 8.2)

> **Repère de périmètre.** Sur la grosse vingtaine de modèles de **projet** que propose
> `dotnet new` avec le SDK .NET 10, seuls **douze** existent en VB — regroupés en **cinq
> familles** : Console, bibliothèque de classes, Windows Forms, WPF et projets de test
> (MSTest, NUnit, xUnit). Aucun modèle web, *worker* ou Blazor. Ce n'est pas une limite
> arbitraire — c'est très exactement la carte des domaines où VB.NET est pertinent. (→ 1.6)

## Ce pour quoi VB.NET n'est (plus) le bon choix

Le revers de la carte est tout aussi important. Puisque le langage n'est pas étendu à de
nouveaux *workloads*, plusieurs domaines modernes sortent de sa portée pratique et se
réalisent **mieux en C#** :

- le **web front-end moderne** : Blazor (Razor génère du C#) ;
- l'**UI multiplateforme / mobile** : .NET MAUI (pas de modèle VB, code d'interface en C#) ;
- les **Minimal APIs** : possibles, mais sans modèle de projet et à la syntaxe contrainte ;
- la **compilation Native AOT** : non prise en charge en pratique pour VB ;
- **gRPC** et **GraphQL** : outillage orienté C# ;
- l'**écriture de *source generators*** (en tant qu'auteur) : à réaliser en C# ;
- les **microservices** cloud-native typiques : écosystème majoritairement C#.

> 💡 **« Pas en VB » ne veut pas dire « inaccessible ».** Tout ce qui précède peut être
> **écrit en C#** — souvent dans une petite bibliothèque dédiée — puis **consommé depuis
> VB.NET** de façon transparente. C'est la **stratégie hybride** détaillée au module 10 et
> cartographiée en Annexe B. La bonne question n'est pas « VB **ou** C# ? », mais
> « **qu'est-ce que je garde en VB, et qu'est-ce que je délègue à C# ?** » (→ 1.6.1 et 1.6.2)

## Pourquoi VB.NET est toujours là en 2026

Si le langage n'évolue plus, pourquoi cette formation — et pourquoi tant d'entreprises
l'emploient-elles encore ? Parce que VB.NET n'est pas un langage « du passé », mais un
langage de **consolidation** :

- un **parc installé immense** : des milliers d'applications métier, souvent critiques,
  écrites en VB.NET (sans compter le VB6 en cours de migration) ;
- des **équipes** qui maintiennent et font évoluer cet existant, pour qui une réécriture
  intégrale en C# serait coûteuse et risquée ;
- une **productivité réelle** sur les scénarios cœur (bureau, données, Office), où la
  verbosité de VB se lit aussi comme de la clarté ;
- un langage **stable et durable** : pour des logiciels de gestion à longue durée de vie,
  l'absence de nouvelle syntaxe est moins un défaut qu'une **garantie de pérennité**.

## L'IA, un facteur devenu décisif

Un dernier élément achève de définir « VB.NET en 2026 » : l'**assistance par IA**. Coder
passe désormais largement par des assistants (Copilot et autres), or les modèles sont
surtout entraînés sur du **C#**. En pratique, ils produisent volontiers du code C#
« déguisé » en VB, hallucinent de la syntaxe C# inexistante en VB, ou confondent VB.NET et
VB6. La conséquence est nette : savoir **prompter, valider et corriger** le code généré est,
en VB.NET, une compétence de **première importance** — sans doute plus encore qu'en C#. La
formation y consacre un module entier (**module 17**) et y revient dès le positionnement
(→ 1.6.3).

---

**En résumé.** VB.NET en 2026 est un langage .NET à part entière, **stabilisé** : il
n'évolue plus côté syntaxe, mais reste pleinement supporté, sûr et performant. Il sert
vraiment, et bien, pour le bureau Windows, les bibliothèques, les données,
l'interopérabilité Office et la maintenance de legacy ; il cède le terrain des *workloads*
modernes à C#, que l'on **consomme** alors depuis VB. Pour comprendre **d'où vient** ce
langage — et pourquoi un tel héritage legacy existe —, la section suivante en retrace
l'histoire, de Visual Basic 6 à VB.NET. (→ 1.2)

⏭️ [De Visual Basic 6 à VB.NET (histoire et héritage legacy)](/01-introduction-vbnet/02-histoire-evolution.md)
