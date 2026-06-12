🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11. Migration et maintenance du code legacy ⭐

> *Le meilleur code est souvent celui qui tourne déjà en production depuis dix ans.*

Pour une grande partie des développeurs VB.NET en 2026, la migration et la maintenance ne sont pas un sujet annexe : **c'est l'activité principale**. Très peu de projets démarrent aujourd'hui « de zéro » en VB.NET. En revanche, des millions de lignes écrites au fil de vingt-cinq ans — applications VB6, logiciels métier (LOB) sur .NET Framework, sites ASP.NET Web Forms — continuent de faire tourner des entreprises entières. Ce code a de la valeur, des utilisateurs, et un coût de remplacement souvent prohibitif.

Ce constat s'aligne exactement avec la stratégie officielle de Microsoft (→ module 1, [§1.6](../01-introduction-vbnet/06-positionnement-2026.md)) : VB.NET est un langage **stabilisé**, pensé pour préserver et faire évoluer l'existant plutôt que pour conquérir de nouveaux territoires. Savoir évaluer, maintenir, moderniser et — quand c'est pertinent — migrer du code legacy est donc l'une des compétences les plus directement monnayables autour de VB.NET. C'est aussi pourquoi ce module porte le label ⭐ : il touche au cœur de ce que le langage fait réellement bien.

---

## 🎯 Objectifs du module

À l'issue de ce module, vous saurez :

- **évaluer** un patrimoine de code existant (inventaire, dette technique, dépendances, risques) et choisir une stratégie de migration adaptée ;
- **conduire** les deux grands chantiers de migration VB : VB6 → VB.NET et .NET Framework 4.x → .NET 10 LTS ;
- **reconnaître les impasses** — au premier rang desquelles ASP.NET Web Forms — et décider en connaissance de cause entre maintenir, isoler ou réécrire ;
- **faire coexister** ancien et nouveau code via `.NET Standard` et une architecture progressive ;
- **moderniser** du code daté (asynchronie, LINQ, EF Core, injection de dépendances, testabilité) sans tout réécrire ;
- **maîtriser le risque** d'une migration (sauvegardes, tests de non-régression, plan de *rollback*).

---

## ⚖️ Trois réalités à accepter avant de migrer

La migration legacy est autant une affaire de **décision** que de technique. Trois principes guident tout le module.

**1. Migrer n'est pas toujours la bonne décision.** Du code stable, qui répond au besoin et ne pose ni problème de sécurité ni problème de support, peut légitimement rester en place. `.NET Framework 4.8.1` reste pris en charge tant que la version de Windows qui l'héberge l'est — « ne rien faire » est parfois la stratégie la plus rationnelle, à condition d'être un choix assumé et non une dérive subie.

**2. Toutes les migrations ne se valent pas.** Faire passer du VB6 vers VB.NET et faire passer du VB.NET de .NET Framework vers .NET 10 sont deux chantiers de nature et de difficulté très différentes. Et certaines technologies n'ont tout simplement **aucun chemin de migration** vers .NET moderne : c'est le cas d'ASP.NET Web Forms (→ [§11.4](04-web-forms-legacy.md)).

**3. Le « big-bang » est rarement raisonnable.** L'approche **incrémentale**, qui maintient à chaque étape un système fonctionnel et livrable, est presque toujours préférable à une réécriture totale en une seule fois. C'est le premier sujet traité par le module (→ [§11.1](01-evaluer-strategies.md)).

---

## 🗺️ Panorama des situations de migration

| Point de départ | Cible réaliste | Difficulté | Section |
|---|---|---|---|
| **VB6** (COM, ActiveX, modèle objet propriétaire) | VB.NET sur .NET moderne | Élevée — réécriture partielle inévitable | [§11.2](02-vb6-vers-vbnet.md) |
| **VB.NET / .NET Framework 4.x** | VB.NET / **.NET 10 LTS** 🆕 | Moyenne — surtout dépendances et APIs retirées | [§11.3](03-framework-vers-net10.md) |
| **ASP.NET Web Forms** (legacy VB) | ⚠️ **Aucun chemin direct** : rester sur .NET Framework, ou réécrire | Très élevée — changement de paradigme | [§11.4](04-web-forms-legacy.md) |
| **Bibliothèques partagées** | `.NET Standard 2.0` comme pont | Faible à moyenne | [§11.5](05-coexistence.md) |
| **Code fonctionnel mais daté** | Modernisation *in situ* (async, LINQ, EF Core, DI) | Variable | [§11.6](06-moderniser.md) |

> 💡 **À garder en tête.** Une migration réussie n'est pas celle qui adopte le plus de nouveautés, mais celle qui **préserve le comportement métier** tout en réduisant la dette et le risque. La stratégie **hybride VB.NET / C#** (→ module [10](../10-hybride-vbnet-csharp/README.md)) offre souvent un compromis idéal : on garde l'existant en VB.NET et on n'introduit du C# que là où c'est strictement nécessaire.

---

## 📑 Dans ce module

- **[11.1 — Évaluer l'existant ; stratégies (incrémentale vs *big-bang*)](01-evaluer-strategies.md)**
  Inventorier le code et ses dépendances, mesurer la dette technique, cartographier le risque et la valeur métier, puis choisir une stratégie de migration cohérente.

- **[11.2 — VB6 → VB.NET](02-vb6-vers-vbnet.md)**
  Outils de conversion, pièges classiques (modèle objet, types par défaut, gestion d'erreurs `On Error`, contrôles ActiveX) et APIs obsolètes à remplacer.

- **[11.3 — .NET Framework 4.x → .NET 10](03-framework-vers-net10.md)** 🆕
  Analyse de dépendances, APIs retirées dans .NET moderne, passage de `App.config` à `appsettings.json`, et *breaking changes* spécifiques à .NET 10.

- **[11.4 — ASP.NET Web Forms (legacy VB) : maintenance et stratégie de sortie](04-web-forms-legacy.md)** ⚠️
  Le cas sans issue directe : Web Forms n'existe pas hors .NET Framework. Les options réalistes — rester sur .NET Framework (supporté avec Windows) ou réécrire (MVC/Blazor en C#, ou Web API VB + front séparé) — et comment maintenir l'existant en attendant.

- **[11.5 — Coexistence .NET Framework / .NET moderne, `.NET Standard`](05-coexistence.md)**
  Faire cohabiter ancien et nouveau code pendant la transition, en mutualisant la logique partagée dans des bibliothèques `.NET Standard 2.0`.

- **[11.6 — Moderniser (async, LINQ, EF Core, injection de dépendances, testabilité)](06-moderniser.md)**
  Améliorer du code existant sans le réécrire intégralement : introduire l'asynchronie, remplacer des boucles par du LINQ, passer à EF Core, découpler via l'injection de dépendances et rendre le code testable.

- **[11.7 — Gestion des risques (sauvegarde, tests de non-régression, *rollback*)](07-gestion-risques.md)**
  Sécuriser le chantier : sauvegardes et versionnage, filet de tests de non-régression, déploiement progressif et plan de retour arrière.

- **[11.8 — Migration assistée par IA](../17-developpement-ia/README.md)** 🤖
  La migration est l'un des terrains où l'IA apporte le plus de valeur en VB.NET. Le sujet est traité en détail au **module 17** (→ notamment [§17.3, *Migrer du legacy avec l'IA*](../17-developpement-ia/03-migration-legacy-ia.md)).

---

## 🧰 Prérequis

Avant d'aborder ce module, il est recommandé d'avoir parcouru :

- les **fondamentaux du langage** et la **POO** (modules [2](../02-fondamentaux-langage/README.md) et [3](../03-poo/README.md)), pour lire et refactoriser du code existant ;
- l'**écosystème .NET** et le **positionnement de VB.NET en 2026** (module 1, [§1.3](../01-introduction-vbnet/03-ecosysteme-dotnet.md) et [§1.6](../01-introduction-vbnet/06-positionnement-2026.md)), pour comprendre les cibles de migration ;
- l'**accès aux données** (module [7](../07-acces-donnees/README.md)), souvent au cœur des chantiers de modernisation ;
- utilement, l'**interopérabilité** et l'**architecture hybride** (modules [9](../09-interoperabilite/README.md) et [10](../10-hybride-vbnet-csharp/README.md)), qui fournissent des leviers concrets de transition.

---

## 🧭 Ce module dans les parcours

Ce module est central dans deux parcours et utile dans un troisième :

| Parcours | Place du module 11 |
|---|---|
| **Maintenance & Migration Legacy** | ⭐ Cœur du parcours (modules 1-3, 7, 9-11, 17) |
| **Architecte / Hybride VB-C#** 🔗 | Pilier de la stratégie de transition (modules 1-4, 9-11, 14-16, 18 + Annexe B) |
| **Web API / Services** | Pertinent dès qu'il s'agit de sortir d'un existant Web Forms |

---

## ➡️ Pour aller plus loin

- **Annexe E — [Guide de migration vers .NET 10 LTS](../annexes/migration-net10/README.md)** 🆕 : checklist complète, stratégies depuis .NET 6 / 8 / Framework, *breaking changes* et points spécifiques à VB.NET.
- **Annexe B — [Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)** 🔗 : pour chaque sujet hors périmètre VB (Blazor, Minimal APIs, Native AOT…), la raison et la façon de le **consommer** depuis VB.NET — indispensable pour cadrer les cibles de réécriture.
- **Module 18 — [Stratégie, feuille de route et ressources](../18-strategie-roadmap/README.md)** : quand rester en VB.NET, quand migrer vers C#, et comment arbitrer (coût/bénéfice).

---

⬅️ [10. Architecture hybride VB.NET / C#](../10-hybride-vbnet-csharp/README.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [12. Exceptions, débogage et journalisation](../12-exceptions-debogage/README.md)

⏭️ [Évaluer l'existant ; stratégies (incrémentale vs *big-bang*)](/11-migration-legacy/01-evaluer-strategies.md)
