🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11.4 — ASP.NET Web Forms (legacy VB) : maintenance et stratégie de sortie ⚠️

> Les sections précédentes décrivaient des migrations possibles : VB6 se convertit ([§11.2](02-vb6-vers-vbnet.md)), Windows Forms et WPF passent à .NET 10 ([§11.3](03-framework-vers-net10.md)). **Web Forms, lui, est un mur.** Il n'existe pas hors du .NET Framework et n'a **aucun chemin de migration** vers .NET moderne. La question n'est donc plus « comment le migrer ? », mais « le maintient-on sur .NET Framework, ou le réécrit-on ? ».

---

## 1. Pourquoi Web Forms est un cas à part ⚠️

ASP.NET Web Forms — ses pages `.aspx`, ses contrôles serveur, son `ViewState`, ses *postbacks*, son cycle de vie de page et son code-behind — est une technologie **exclusivement liée au .NET Framework**. Elle n'a **jamais été portée** vers .NET Core ni vers aucune version moderne de .NET, et Microsoft a indiqué clairement qu'elle ne le serait **jamais**. Web Forms est en **mode maintenance** : correctifs de sécurité uniquement, Windows uniquement, aucune feuille de route d'évolution.

La conséquence est radicale et doit être posée sans détour : **il n'existe aucun outil de mise à niveau, aucun « port »**. Les architectures de Web Forms et de .NET moderne sont trop différentes pour qu'une conversion automatique soit possible. Tout passage à .NET moderne est donc une **réécriture**, pas une migration. L'outillage de modernisation le confirme lui-même : pour Web Forms, pas de chemin direct — la couche d'interface est à réécrire (Blazor ou Razor Pages).

Ce constat tranche avec le reste du chapitre 11. Là où WinForms/WPF se migrent et où VB6 dispose de convertisseurs, Web Forms est véritablement une impasse technique — d'où le ⚠️.

---

## 2. Première option : rester sur .NET Framework

C'est une décision **légitime**, et souvent la plus rationnelle (dans la lignée du « ne rien faire » assumé du [§11.1](01-evaluer-strategies.md)). Le .NET Framework 4.8.1 est pris en charge **tant que la version de Windows qui l'héberge l'est** : l'application ne cesse pas de fonctionner.

**Quand ce choix se justifie** : application stable, sans pression de sécurité ou de conformité, peu modifiée, portée par un produit en fin de vie, ou simplement sans budget de réécriture à court terme.

**L'hébergement n'est plus un frein au cloud.** Rester sur .NET Framework ne signifie pas nécessairement rester sur site : Microsoft a introduit (Ignite 2025) un service d'hébergement *Managed Instance* pour Azure App Service, spécifiquement pensé pour les applications .NET Framework, y compris celles qui s'appuient sur des fonctionnalités propres à Windows. « Rester » peut donc aussi vouloir dire « héberger dans le cloud ».

**La contrepartie à assumer.** Le risque s'accumule silencieusement : vivier de compétences Web Forms qui se réduit, écosystème figé, et **exposition de conformité croissante** à mesure que les dépendances vieillissent. Une migration forcée finit souvent par s'imposer, dans de moins bonnes conditions. « Rester » doit donc être un **choix périodiquement réévalué**, jamais une dérive.

**Posture de maintenance** : maintenir les dépendances à jour, scanner les vulnérabilités, documenter — et, surtout, **isoler la logique métier** (cf. §3.1), ce qui prépare toute sortie future.

---

## 3. Seconde option : réécrire (et préparer la sortie)

Si l'application est active, stratégique et appelée à évoluer, la réécriture s'impose. Le mot d'ordre : **réécrire, pas porter** — et le faire intelligemment.

### 3.1 Préparer : extraire la logique métier du code-behind

Les applications Web Forms souffrent d'un mal classique : la logique métier est **enfouie dans les fichiers code-behind** (`.aspx.vb`), mêlée à la gestion de l'interface. Le tout premier geste, quelle que soit la suite, est donc d'**extraire la logique métier et l'accès aux données** vers une bibliothèque séparée — idéalement en **`.NET Standard 2.0`**, consommable à la fois par l'ancien et par le nouveau code (→ [§11.5](05-coexistence.md)).

C'est l'étape la plus rentable de toutes : elle réduit la réécriture à (essentiellement) la couche d'interface, et elle a de la valeur **même si l'on décide finalement de rester** sur .NET Framework.

### 3.2 Les cibles de réécriture — et le point VB ⚠️

Les cibles réalistes en 2026 sont au nombre de quatre. Le tableau ci-dessous les situe — et signale le point décisif pour un développeur VB :

| Cible | Profil | Langage |
|---|---|---|
| **Blazor** | Voie **principale recommandée par Microsoft**. Conceptuellement la plus proche de Web Forms : composants, état, gestionnaires d'événements, liaison de données, et un modèle serveur qui rappelle l'aller-retour du *postback*. | ⚠️ **C# uniquement** (Razor génère du C#) |
| **ASP.NET Core MVC** | Séparation explicite (contrôleurs/vues), pipeline testable. Le **plus grand écart mental** par rapport au modèle de contrôles serveur. | ⚠️ Vues **C# uniquement** |
| **Razor Pages** | Option légère, orientée page — proche de l'esprit « une page = une unité » de Web Forms. | ⚠️ **C# uniquement** |
| **Web API VB (par contrôleurs) + front séparé** | La voie **native VB** : la logique métier et l'API restent en VB, le front-end est une application distincte. | ✅ **API en VB**, front à part |

> **Le point honnête, central pour VB.** Les « successeurs spirituels » de Web Forms côté interface — Blazor, vues MVC, Razor Pages — sont **tous en C#** (→ module 8, [§8.3](../08-services-web/03-limites-web-vbnet.md) et [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)). Une équipe VB qui réécrit l'**interface** Web Forms écrira donc, en pratique, ce front-end **en C#**. Pour rester dans le périmètre VB, la stratégie consiste à conserver le **back-end métier et l'API en VB** — l'exposition d'une Web API ASP.NET Core par contrôleurs étant, elle, pleinement prise en charge en VB.NET (→ module 8, [§8.2](../08-services-web/02-web-api-controllers.md) ✅) — et à traiter le front-end comme une brique séparée (Blazor/C#, SPA JavaScript, ou Razor). C'est exactement l'option « **Web API VB + front séparé** ».

### 3.3 Réécrire incrémentalement, pas en *big-bang*

Une réécriture Web Forms menée d'un seul bloc est vouée à l'échec : trop de surface, trop de règles métier accumulées, trop de risque de dégrader l'expérience pendant une longue interruption de livraison. L'approche moderne est **incrémentale**, fondée sur le pattern **Strangler Fig** (→ [§11.1](01-evaluer-strategies.md)) :

- **YARP** (reverse proxy) place l'ancienne application Web Forms et la nouvelle **côte à côte** derrière un même point d'entrée ;
- les **System.Web Adapters** (`Microsoft.AspNetCore.SystemWebAdapters`) partagent la **session et l'authentification** entre les deux mondes ;
- on migre **une route (ou une page) à la fois**, en livrant de la valeur en continu ;
- la logique métier partagée vit dans la bibliothèque `.NET Standard` commune (→ [§11.5](05-coexistence.md)).

Le nouveau code « étrangle » progressivement l'ancien, jusqu'à ce que la dernière page Web Forms puisse être retirée.

### 3.4 Les migrations qui accompagnent Web Forms

Web Forms vient rarement seul. La réécriture entraîne généralement d'autres chantiers à anticiper :

| Élément hérité | À migrer vers |
|---|---|
| **Forms Authentication / ASP.NET Membership** | ASP.NET Core Identity (→ module 16, [§16.1](../16-securite/01-auth.md)) |
| **Services web ASMX** | Web API moderne (→ module 8) |
| **WCF côté serveur (`.svc`)** | Non pris en charge — CoreWCF, ou réécriture en REST/gRPC |
| **Motifs `HttpContext.Current`** | À refactoriser (plus de contexte statique ambiant) |
| **Contrôles serveur tiers** (grilles, composants) | Pas d'équivalent direct — à remplacer |

---

## 4. Choisir : grille de décision

| Si l'application est… | …la stratégie est |
|---|---|
| Stable, en fin de vie, peu modifiée | **Rester** sur .NET Framework : maintenir, scanner, documenter, isoler le métier |
| Active, stratégique, en évolution | **Réécrire incrémentalement** (YARP + Strangler Fig) |
| À réécrire, et l'on veut rester en VB | Métier + **Web API en VB** ✅, front-end séparé (probablement C#) |
| À réécrire, sans contrainte de langage | **Blazor** (le plus proche de Web Forms) ou MVC / Razor Pages |

**Séquencement.** Quelle que soit la cible moderne, viser **.NET 10 LTS directement**, en sautant .NET 8 et .NET 9 — dont le support se termine le **10 novembre 2026** (→ [Annexe H](../annexes/versions-reference/README.md)). .NET 10 est supporté jusqu'en novembre 2028.

> 💡 L'IA peut assister la décomposition d'une application Web Forms (séparation interface/logique, cartographie des dépendances, tests de parité), mais le **résultat doit être validé** : c'est une réécriture, pas une traduction mécanique (→ module [17](../17-developpement-ia/README.md)) 🤖.

---

## 🔑 Points clés à retenir

- Web Forms est une **véritable impasse** : .NET Framework uniquement, jamais porté, aucun chemin de migration — tout passage à .NET moderne est une **réécriture**, pas une mise à niveau.
- Deux options honnêtes : **rester** sur .NET Framework (supporté avec Windows ; légitime pour une application stable ou en fin de vie ; désormais hébergeable via Azure App Service *Managed Instance*) ou **réécrire**.
- Les successeurs d'interface (Blazor, MVC, Razor Pages) sont **C# uniquement** : pour rester en VB, garder le **métier et la Web API en VB** (pris en charge, [§8.2](../08-services-web/02-web-api-controllers.md)) et traiter le **front-end comme une brique séparée** (souvent C# ou JavaScript).
- Réécrire **incrémentalement**, jamais en *big-bang* : YARP + System.Web Adapters + Strangler Fig + bibliothèque `.NET Standard` partagée (→ [§11.5](05-coexistence.md)), page par page.
- Geste préparatoire dans tous les cas : **extraire la logique métier du code-behind** vers une bibliothèque partagée.
- Quelle que soit la cible, viser **.NET 10 LTS** directement (sauter .NET 8/9, hors support en novembre 2026).

---

⬅️ [11.3 — .NET Framework 4.x → .NET 10](03-framework-vers-net10.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [11.5 — Coexistence .NET Framework / .NET moderne, `.NET Standard`](05-coexistence.md)

⏭️ [Coexistence .NET Framework / .NET moderne, .NET Standard](/11-migration-legacy/05-coexistence.md)
