🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 13.4 — Analyse statique (analyseurs Roslyn, SonarQube, StyleCop)

> **Module 13 — Tests et qualité du code**
> Détecter des problèmes **sans même exécuter** le code.

---

## Qu'est-ce que l'analyse statique ?

Les sections précédentes vérifiaient le code **en l'exécutant** (tests unitaires et
d'intégration). L'**analyse statique** fait le contraire : elle **lit** le code source — sans le
lancer — pour y repérer bugs probables, mauvaises pratiques, problèmes de style, de maintenabilité
ou de sécurité. C'est un complément des tests, pas un substitut : là où un test prouve un
comportement, un analyseur signale un **motif suspect** (déréférencement de `Nothing` possible,
code mort, API obsolète, faille d'injection…).

Son grand atout est le **moment** où elle intervient : pendant la **frappe** (soulignements dans
l'éditeur), à la **compilation** (avertissements ou erreurs), et en **intégration continue**
(blocage du build si la qualité régresse). Plus tôt un problème est vu, moins il coûte.

---

## Les analyseurs Roslyn : le socle

La plateforme de compilation .NET (**Roslyn**) expose des **analyseurs** qui inspectent le code
en temps réel, dans l'IDE comme au build.

> 🟢 **Et c'est une excellente nouvelle pour VB.NET :** les analyseurs Roslyn intégrés inspectent
> aussi bien le code **C# que Visual Basic** (style, qualité, maintenabilité, conception…). Le
> socle de l'analyse statique .NET prend donc VB.NET en charge nativement.

On distingue deux familles, identifiables par le préfixe de leur identifiant :

- **Règles de qualité — `CAxxxx`** (ex. `CA1822`) : conception, performance, usage, sécurité,
  globalisation… Incluses dans le SDK depuis **.NET 5**, **activées par défaut**.
- **Règles de style — `IDExxxx`** (ex. `IDE0001`) : conventions de mise en forme et d'écriture,
  configurables et exécutables comme avertissements ou erreurs de build.

### Configurer la rigueur dans le projet

Quelques propriétés MSBuild (dans le `.vbproj`) pilotent le niveau d'exigence — identiques en VB
et en C# :

```xml
<PropertyGroup>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>      <!-- activé par défaut depuis .NET 5 -->
  <AnalysisLevel>latest</AnalysisLevel>
  <AnalysisMode>Recommended</AnalysisMode>           <!-- None | Default | Minimum | Recommended | All -->
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors> <!-- transforme les avertissements en erreurs -->
</PropertyGroup>
```

Chaque diagnostic a une **sévérité** : `none`, `silent`, `suggestion`, `warning` ou `error`. En
montant un avertissement clé au rang d'**erreur**, on **empêche** la compilation tant qu'il n'est
pas corrigé — l'outil le plus efficace pour faire respecter une règle.

---

## `.editorconfig` : configurer et imposer

Le fichier **`.editorconfig`** est le point central de configuration : il fixe, par dossier ou par
type de fichier, le **style** et la **sévérité** des règles. Il prend en charge **C# et VB**, avec
une distinction de préfixes utile à connaître :

```ini
[*.vb]
# Sévérité d'une règle de qualité (CA) ou de style (IDE) — préfixe dotnet_ : tous langages
dotnet_diagnostic.CA1822.severity = warning
dotnet_style_qualification_for_field = true:suggestion
```

> 💡 **Les préfixes désignent le langage concerné :** `dotnet_…` s'applique à **C# et VB** ;
> `csharp_…` est **propre à C#** (et ne concerne pas VB) ; `visual_basic_…` est **propre à VB**.
> Les options de style spécifiques à une syntaxe C# (comme `var`) n'ont donc, logiquement, pas
> d'équivalent VB.

---

## Les analyseurs externes (NuGet)

Au-delà du socle intégré, on installe des analyseurs supplémentaires — de préférence en **paquet
NuGet par projet** (ils ne s'appliquent qu'au projet concerné), ou en extension Visual Studio
(au niveau de la solution). Les exemples les plus courants sont **StyleCop**, **Roslynator**,
**Sonar Analyzer** et les analyseurs **xUnit**.

C'est ici que la **frontière VB.NET / C#** se manifeste : tous ces outils **ne prennent pas tous
VB.NET en charge**.

---

## ⚠️ StyleCop : spécifique à C#

**StyleCop Analyzers** (paquet `StyleCop.Analyzers`) impose un **style et une mise en forme C#**
(ordre des éléments, espacements, documentation, conventions de nommage…).

> ⚠️ **StyleCop ne prend en charge que C# — il ne s'applique pas à VB.NET.** Ses règles sont
> définies pour la syntaxe C# ; il n'existe pas d'équivalent StyleCop pour Visual Basic. Inutile
> donc de l'ajouter à un projet VB.

Deux précisions complètent ce constat :

- Même en C#, StyleCop est **purement stylistique** : il ne détecte ni bugs, ni problèmes de
  performance, ni vulnérabilités. Il **complète** des outils comme les analyseurs Roslyn ou
  SonarQube, il ne les remplace pas.
- **Pour le style en VB.NET**, appuyez-vous sur les **règles `IDExxxx` intégrées** et sur
  **`.editorconfig`** (qui, eux, prennent VB en charge) : conventions de nommage, qualification,
  préférences d'écriture s'y configurent très bien.

> ℹ️ Le même constat vaut pour **Roslynator**, un autre analyseur populaire : il est **dédié à
> C#**. C'est l'illustration la plus visible du **biais C# de l'écosystème d'outillage** que cette
> formation signale régulièrement (cf. [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

---

## SonarQube : qualité et sécurité, **VB.NET inclus**

**SonarQube** est une plateforme d'analyse continue de la qualité et de la sécurité du code. Elle
va plus loin que de simples règles : détection de **bugs** et de **code smells**, **points chauds
et vulnérabilités de sécurité**, mesure de la **dette technique**, **tableaux de bord** et
**portes de qualité** (*quality gates*) qui peuvent **bloquer** une livraison en CI.

Elle se décline en plusieurs formes : **SonarLint** (retour immédiat dans l'IDE), **SonarQube**
(serveur, souvent au cœur de la CI) et **SonarCloud** (version hébergée).

> ✅ **Contrairement à StyleCop, SonarQube prend en charge VB.NET.** Les analyseurs .NET de Sonar
> (*sonar-dotnet*) couvrent **C# et Visual Basic** : VB.NET y est un langage de première classe,
> avec son propre jeu de règles. Pour une équipe VB.NET soucieuse de qualité au-delà du socle
> intégré, c'est l'option de référence.

---

## Le tableau honnête pour VB.NET

Voici, sans détour, ce qui prend VB.NET en charge — et ce qui ne le prend pas :

| Outil | Prise en charge VB.NET |
|-------|------------------------|
| **Analyseurs Roslyn intégrés** (`CA` + `IDE`) | ✅ **Oui** (C# et VB) |
| **`.editorconfig`** | ✅ Oui (options `dotnet_…` et `visual_basic_…`) |
| **SonarQube / sonar-dotnet** | ✅ Oui (jeu de règles VB.NET dédié) |
| **SecurityCodeScan** (sécurité) | ✅ Oui (C# et VB.NET) |
| **StyleCop** | ❌ **Non** — C# uniquement |
| **Roslynator** | ❌ Non — C# uniquement |
| La plupart des autres analyseurs tiers | ⚠️ Majoritairement C# — **à vérifier** |

La lecture est claire : **VB.NET dispose d'une bonne couverture** grâce au **socle Roslyn
intégré** et à **SonarQube**, mais d'un **écosystème tiers nettement plus mince** que C#. Le
réflexe pratique : avant d'adopter un analyseur tiers, **vérifiez qu'il prend VB.NET en charge** —
beaucoup ne ciblent que C#.

---

## Intégration dans le flux de travail

L'analyse statique se déploie idéalement à **trois niveaux** :

- **Dans l'IDE** (Visual Studio 2026) : soulignements en temps réel et **correctifs rapides**
  (Ctrl + . pour appliquer la suggestion).
- **Au build** : les diagnostics deviennent des avertissements ou, mieux pour les règles
  critiques, des **erreurs** (`TreatWarningsAsErrors`).
- **En CI/CD** : une **porte de qualité** (avec SonarQube, par exemple) fait **échouer** le build
  si la qualité régresse — le garde-fou collectif.

L'outil `dotnet format` complète l'ensemble en **appliquant automatiquement** les corrections de
style conformes au `.editorconfig`, ce qui évite les débats de mise en forme en revue de code.

---

## Lien avec la sécurité

L'analyse statique recouvre aussi la **sécurité applicative** (analyse statique de sécurité, ou
*SAST*) : repérer, sans exécuter le code, des motifs vulnérables (injection SQL, secrets en dur,
désérialisation dangereuse…). En VB.NET, **SonarQube** et **SecurityCodeScan** assurent cette
couverture. Le sujet est approfondi au [module 16](../16-securite/README.md), en particulier
[16.3 (OWASP)](../16-securite/03-owasp.md) et
[16.4 (dépendances et vulnérabilités, SAST/DAST)](../16-securite/04-dependances-vulnerabilites.md).

---

## À retenir

L'analyse statique attrape, **sans exécuter le code**, ce que les tests ne visent pas : motifs
buggés, code mort, mauvaises pratiques, failles. Son **socle** — les analyseurs **Roslyn**
intégrés au SDK, configurés via **`.editorconfig`** — prend **pleinement VB.NET en charge**, et
**SonarQube** y ajoute qualité et sécurité **en couvrant aussi VB.NET**. La frontière à retenir
est nette : **StyleCop** (et **Roslynator**) ne visent que **C#**, et l'écosystème tiers penche
fortement vers C# — d'où la règle de prudence : **vérifier la prise en charge VB.NET avant
d'adopter un analyseur externe**. Imposez vos règles au build (avertissements en erreurs) et en
CI (portes de qualité), et laissez `dotnet format` régler la mise en forme.

➡️ Section suivante : **[13.5 — Couverture de code et génération de tests par IA](05-couverture-tests-ia.md)**,
pour mesurer ce que vos tests couvrent réellement — et accélérer leur écriture. 🤖

⏭️ [Couverture de code ; génération de tests par IA](/13-tests-qualite/05-couverture-tests-ia.md)
