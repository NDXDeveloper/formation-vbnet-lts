🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 1.6 VB.NET en 2026 : positionnement honnête ⭐ 🔄

Voici la **section pivot** de ce module. Après avoir vu ce qu'est VB.NET (1.1), d'où il
vient (1.2), son écosystème (1.3), son outillage (1.4) et un premier projet (1.5), nous
prenons de la hauteur pour répondre à la question qui structure toute la formation :
**quelle est la place honnête de VB.NET en 2026, et qu'est-ce que cela change pour votre
façon de travailler ?**

Une précision d'emblée : cette honnêteté n'est pas du pessimisme. C'est une **carte**. Elle
indique précisément où investir votre énergie, où VB.NET excelle, et ce qu'il vaut mieux
confier à C#. Bien lue, elle est libératrice.

## La stratégie officielle Microsoft : un langage « stabilisé »

Le point de départ n'est pas une opinion, mais une **stratégie documentée**. Dans la
*Stratégie du langage Visual Basic* (Microsoft Learn), Microsoft énonce une orientation
claire : VB adopte une approche de **consommation seule** (*consumption-only*) et **ne sera
pas étendu à de nouveaux workloads**. L'entreprise s'engage en parallèle à **maintenir** le
langage — sécurité, compatibilité avec les nouveaux *runtimes*, expérience dans Visual
Studio — et à investir dans l'**interopérabilité avec C#**, tout en garantissant un *design*
**stable**.

Cette position s'inscrit dans la stratégie d'ensemble des **trois langages .NET** : C#
porte l'innovation (et l'essentiel du framework et des bibliothèques est d'ailleurs écrit en
C#), F# occupe le créneau fonctionnel, et VB joue la carte de la **stabilité** et de
l'**accessibilité**, recentré sur ses scénarios cœur.

> 📌 **À retenir.** « Stabilisé » est un choix **délibéré et assumé**, pas un abandon
> déguisé. VB.NET est maintenu et pleinement supporté sur .NET 10 LTS (→ 1.3). Ce qui
> change, c'est qu'il n'évolue plus sur le plan du **langage**.

## Un langage figé : ce que cela implique concrètement

L'intuition « langage figé » est juste — mais soyons précis. **Pour ce que vous écrivez**,
VB n'a plus reçu de syntaxe réellement nouvelle depuis ~2020. Le **numéro de version**, lui,
a légèrement avancé : on cite **VB 16.9** (2021) comme la dernière version documentée avec
des nouveautés, et le compilateur est depuis passé directement de 16.9 à **17.13** (livrée
avec Visual Studio 2022 17.13, début 2025 — toujours la version courante). La nuance
compte : **ces incréments sont purement orientés consommation** — ils permettent à VB de
*reconnaître* ou de *consommer* des éléments venus de C# ou du *runtime* (les propriétés
`init-only` en 16.9, l'attribut `OverloadResolutionPriority` en 17.13), **jamais une
nouvelle construction à écrire**.

La formule exacte est donc : *VB.NET est stabilisé — figé à 16.9 pour ce qui s'écrit, avec
de rares incréments de consommation côté compilateur ; ce que l'on écrit au quotidien n'a
pas bougé depuis ~2020.*

Concrètement, cela implique cinq choses :

- **Aucune nouvelle syntaxe d'écriture.** Les *records*, les propriétés `init`, les types à
  constructeur primaire… se **consomment** depuis VB mais ne se **déclarent** pas en VB ;
  d'autres nouveautés C# (*top-level statements*, *pattern matching* avancé, déclaration de
  membres d'interface par défaut…) restent purement côté C# (→ 3.7, Annexe B).
- **Des gains « gratuits » côté plateforme.** Performances du *runtime* (JIT, PGO, SIMD),
  nouvelles API de la BCL, outillage : tout cela profite à VB sans changer une ligne de code
  (→ 1.3 et 14.6). Le gel porte sur le **langage**, pas sur la plateforme.
- **Une documentation « C#-first ».** La doc, les exemples et les forums parlent C# par
  défaut : savoir **lire le C# et le transposer en VB** devient une compétence courante
  (→ Annexe A).
- **Une assistance IA à valider.** Les modèles sont entraînés majoritairement sur C# et
  produiront volontiers du VB « C#-isé » ou halluciné (→ 1.6.3 et module 17).
- **La stabilité comme atout.** Pour des logiciels métier à longue durée de vie, l'absence
  de nouveautés de langage signifie aussi **aucune rupture de langage à rattraper**.

## Scénarios cœur vs workloads non supportés

La stratégie « consommation seule » dessine une frontière nette. Microsoft nomme
explicitement ses **scénarios cœur** — Windows Forms et les bibliothèques — et la formation
étend cette carte aux domaines réellement pertinents. À l'inverse, les **workloads
modernes** vers lesquels le langage n'est pas étendu se réalisent mieux en C#.

| ✅ Cœur VB.NET — pris en charge | ⚠️ Hors périmètre → déléguer à C# |
|------------------------------|----------------------------------|
| **Windows Forms** ⭐, **WPF** | Blazor, .NET MAUI |
| Bibliothèques de classes | Minimal APIs (sans modèle) |
| Données / LOB (ADO.NET, EF Core, LINQ) | Native AOT |
| Interopérabilité COM / Office | gRPC, GraphQL |
| Maintenance & migration de legacy | *Source generators* (en tant qu'auteur) |
| Web API par contrôleurs | Microservices / Dapr / Kubernetes |

> 💡 **« Hors périmètre » ne veut pas dire « inaccessible ».** Tout ce qui figure dans la
> colonne de droite peut être **écrit en C#** puis **consommé depuis VB.NET** — c'est la
> **stratégie hybride** (→ 1.6.2, module 10 et Annexe B). La frontière n'est pas un mur,
> c'est une **répartition des rôles**.

## Lecture du périmètre réel : une poignée de modèles de projet

Pour matérialiser cette carte, un indicateur très parlant : le catalogue des **modèles de
projet** `dotnet new`. Sur la grosse vingtaine de modèles de projet que propose le SDK
.NET 10, **douze seulement existent en VB**, regroupés en **cinq familles** : Console,
**bibliothèque de classes**, **Windows Forms** (application, bibliothèque de classes,
bibliothèque de contrôles), **WPF** (application et trois bibliothèques) et **projets de
test** (MSTest, NUnit, xUnit).

Tout le reste est **absent en VB** : pas de modèle Worker, ni Blazor, ni MAUI, ni Minimal
API, ni gRPC, ni Razor — ni même Web API (l'approche par contrôleurs du module 8 se met en
place à la main, sans modèle dédié). Ce n'est pas une limitation arbitraire — c'est la
**carte opérationnelle** des usages prévus pour VB.NET : **les modèles disponibles *sont*
les scénarios cœur**.

> 📌 **Le décompte importe moins que l'ordre de grandeur.** Le nombre exact varie selon la
> façon de compter les variantes ; ce qu'il faut retenir, c'est qu'une **fraction** du
> catalogue est ouverte à VB — et qu'elle coïncide exactement avec les forces du langage.

## En synthèse : une posture, pas un renoncement

VB.NET en 2026 est un langage .NET **stable, supporté et performant**, qui **n'évolue plus
syntaxiquement** mais reste excellent sur un **périmètre précis** : le bureau Windows, les
bibliothèques, les données, l'interopérabilité Office et la maintenance de legacy. Il délègue
à C# les *workloads* modernes, qu'il **consomme** ensuite via l'architecture hybride. Et il
suppose des **pratiques adaptées à l'IA**.

La bonne question n'est donc jamais « VB **ou** C# ? », mais : **« Qu'est-ce que je garde en
VB, qu'est-ce que je délègue à C#, et comment travailler efficacement avec l'IA ? »**

---

Les trois sous-sections qui suivent transforment cette posture en **outils concrets** :

- **1.6.1 — [VB.NET vs C# : quand choisir quoi](06.1-vbnet-vs-csharp.md)** : une grille de
  décision opérationnelle.
- **1.6.2 — [La stratégie hybride VB.NET / C# en une page](06.2-strategie-hybride.md)** 🔗 :
  comment faire cohabiter les deux langages.
- **1.6.3 — [L'ère de l'IA : pourquoi c'est décisif pour VB.NET](06.3-ere-ia.md)** 🤖 : le
  biais C# des modèles, et comment composer avec.

Commençons par la grille de décision. (→ 1.6.1)

⏭️ [VB.NET vs C# : quand choisir quoi (grille de décision)](/01-introduction-vbnet/06.1-vbnet-vs-csharp.md)
