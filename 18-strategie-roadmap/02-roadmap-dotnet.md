🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 18.2 Support long terme et feuille de route .NET (cycles LTS / STS)

La section [18.1](01-strategie-microsoft.md) a posé une distinction décisive : le **langage** VB est figé, mais la **plateforme** .NET, elle, évolue. Cette section traite du versant plateforme — combien de temps vous êtes couvert, sur quelle cadence, et ce que cela implique pour une base de code VB.NET. La bonne nouvelle, d'emblée : **le support de VB.NET est celui de .NET** ; le gel du langage ne le raccourcit pas d'un seul jour.

---

## La cadence .NET : un train annuel

.NET livre une nouvelle version majeure chaque année en novembre, pour une feuille de route régulière et prévisible. Deux régimes de support coexistent, et la qualité de toutes les versions est identique : seule la durée de support diffère.

- **LTS (Long Term Support)** — les versions **paires** (.NET 6, 8, 10). Les versions paires sont des versions LTS qui reçoivent support et correctifs gratuits pendant trois ans.
- **STS (Standard Term Support)** — les versions **impaires** (.NET 7, 9, 11). Les versions impaires sont des versions STS qui reçoivent support et correctifs gratuits pendant deux ans.

Un changement récent mérite l'attention, car il modifie les calculs que beaucoup ont en tête. Le support STS était à l'origine de 18 mois ; en septembre 2025, Microsoft l'a étendu à 24 mois, effectif à partir de .NET 9. La conséquence est frappante : .NET 8 (LTS) et .NET 9 (STS) atteignent leur fin de support le même jour, le 10 novembre 2026. Autrement dit, depuis .NET 9, STS signifie vraiment « deux ans », et non plus « 18 mois ».

Trois détails de politique comptent en pratique : les dates de fin de vie s'alignent sur le « Patch Tuesday » (le deuxième mardi du mois), ce qui explique des dates comme le 10 novembre plutôt qu'un anniversaire de sortie ; pour rester dans un état supporté, il faut exécuter le dernier correctif de sa version majeure. Enfin, les six derniers mois de toute version constituent une période de maintenance où seuls les correctifs de sécurité sont fournis.

---

## Le tableau (juin 2026)

| Version | Type | Sortie | Fin de support | État |
|---------|------|--------|----------------|------|
| **.NET 10** | **LTS** | Novembre 2025 | **14 novembre 2028** | ✅ Recommandée |
| .NET 9 | STS | Novembre 2024 | 10 novembre 2026 | ⚠️ Bientôt terminé |
| .NET 8 | LTS | Novembre 2023 | 10 novembre 2026 | ⚠️ Bientôt terminé |
| .NET 11 | STS | Prévue nov. 2026 | ~nov. 2028 | 🔮 À venir |
| .NET Framework 4.8.1 | — | 2022 | Lié au cycle de Windows | 🔧 Legacy |

Puisque .NET 10 est sorti le 11 novembre 2025, .NET 8 atteint sa fin de support douze mois plus tard, le 10 novembre 2026. La même mécanique des « douze mois après le successeur » place .NET 9 (STS, 24 mois) à cette même date. Le détail complet des versions et du support figure dans l'[Annexe H](../annexes/versions-reference/README.md). À noter : .NET Framework 4.8 a un support lié au système d'exploitation, sans date de fin prévisible — un point qui pèsera pour le legacy ([11.4](../11-migration-legacy/04-web-forms-legacy.md)).

---

## Ce que cela signifie pour VB.NET — l'essentiel

- **Le support de VB.NET est celui du runtime .NET.** VB est supporté exactement aussi longtemps que la version de .NET qu'il cible. Le gel du langage à 16.9 n'y change rien : une application VB sur .NET 10 est couverte jusqu'en novembre 2028, comme n'importe quelle application .NET.
- **Visez LTS pour la durée.** Pour les applications de bureau et métier à longue durée de vie — le terrain de VB.NET —, ciblez **.NET 10 LTS** : trois ans de support et la stabilité. Le profil LTS épouse parfaitement les usages typiques de VB.
- **La plateforme avance sous vous, sans *churn* de langage.** Chaque version adoptée apporte gratuitement des gains de runtime, de performance et de sécurité ([14.6](../14-performance/06-apports-net10.md)) — vous montez dans le train pour la performance sans subir de changement de langage. C'est la distinction *plateforme ≠ langage* de [18.1](01-strategie-microsoft.md), rendue concrète : ici, langage figé **plus** runtime qui évolue est une force.

---

## Les composants hors-bande (OOB) — à connaître pour la consommation

Tout l'écosystème ne suit pas le train annuel. Certains composants sont livrés hors-bande (OOB) avec leur propre cycle de vie indépendant — .NET Aspire, Microsoft.Extensions.AI et le C# Dev Kit en sont des exemples, qui publient régulièrement de nouvelles fonctionnalités.

Deux implications pour VB.NET. D'abord, les bibliothèques d'**intégration d'IA** que VB consomme ([17.9](../17-developpement-ia/09-consommer-ia.md)) — `Microsoft.Extensions.AI` notamment — évoluent à leur propre rythme, à planifier indépendamment de la version de .NET. Ensuite, le **C# Dev Kit** (sur lequel s'appuie VS Code, et qui ne prend pas en charge VB — [1.4](../01-introduction-vbnet/04-installation-outils.md)) est lui aussi OOB : cela fait partie de l'explication du chemin VS Code dégradé pour VB ([17.6](../17-developpement-ia/06-workflow-ia-first.md)).

---

## Solutions hybrides : un support unifié

Dans une solution mixte VB/C# ([module 10](../10-hybride-vbnet-csharp/README.md)), tous les projets ciblent la **même version de .NET** : vous choisissez un seul *Target Framework* (par exemple .NET 10 LTS) pour l'ensemble de la solution. L'histoire du support reste donc **unifiée** — l'architecture hybride ne complique en rien la planification de cycle de vie.

---

## Ce que cela change pour la décision « rester ou migrer »

Voici le point stratégique, qui prépare [18.3](03-cas-usage-quand-migrer.md). Le tableau du support et de la feuille de route est **solide et prévisible** : vous êtes couvert des années durant, sur une cadence connue, et le gel du langage ne raccourcit aucune de ces échéances. L'avenir de la **plateforme** n'est donc **pas** le risque.

La conséquence est nette : **la longévité du support n'est pas une raison de quitter VB.NET.** Il n'existe pas de « falaise de support » propre à VB — vous êtes aussi soutenu que n'importe quelle application .NET. Le gel du langage est une question de **capacités** (ce que VB ne fait pas nativement — [18.3](03-cas-usage-quand-migrer.md), [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)), pas une question de support. L'arbitrage « rester ou migrer » se joue donc sur les capacités et l'écosystème, jamais sur une échéance de fin de vie.

---

## En résumé

Train annuel en novembre ; LTS trois ans (versions paires), STS désormais deux ans (versions impaires, depuis l'extension de septembre 2025) ; **VB.NET supporté exactement comme .NET** ; viser **LTS (.NET 10 → novembre 2028)** pour les applications longues durées de vie de VB ; et rouler avec la plateforme pour la performance et la sécurité, sans *churn* de langage. Le support est la partie **réglée et confortable** du tableau VB.NET — les vraies questions stratégiques, traitées en [18.3](03-cas-usage-quand-migrer.md), se situent ailleurs.

⏭️ [Cas d'usage optimaux pour VB.NET : quand rester, quand migrer vers C#](/18-strategie-roadmap/03-cas-usage-quand-migrer.md)
