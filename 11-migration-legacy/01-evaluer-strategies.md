🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11.1 — Évaluer l'existant ; stratégies (incrémentale vs *big-bang*)

> *On ne migre bien que ce que l'on comprend.* Avant toute ligne de code modifiée, deux questions doivent trouver une réponse : **qu'avons-nous exactement ?** et **comment allons-nous procéder ?** Cette section répond à la première par une démarche d'évaluation, puis à la seconde en comparant les grandes stratégies de migration.

---

## 1. Pourquoi évaluer avant de migrer

La tentation, face à du code daté, est de se lancer immédiatement : ouvrir le projet, lancer un outil de conversion, corriger les erreurs au fil de l'eau. C'est presque toujours une erreur. Une migration entreprise sans évaluation se heurte tôt ou tard à un obstacle qu'on aurait pu anticiper — une dépendance COM sans équivalent, une API retirée de .NET moderne, une bibliothèque tierce abandonnée — et le chantier s'enlise après que du temps et du budget ont déjà été engagés.

L'évaluation n'est pas une formalité administrative : elle **sert la décision**. Son but n'est pas seulement de dresser un état des lieux, mais de répondre à trois questions d'arbitrage : faut-il migrer ce code (plutôt que le maintenir ou le retirer) ? Si oui, vers quelle cible ? Et selon quelle stratégie ? Tant que ces réponses ne sont pas étayées, planifier une migration revient à naviguer sans carte.

Rappelons le cadre (→ module 1, [§1.6](../01-introduction-vbnet/06-positionnement-2026.md)) : VB.NET est un langage **stabilisé**, et la migration legacy est l'un de ses terrains les plus concrets. Bien évaluer l'existant est donc une compétence centrale, pas un préalable optionnel.

---

## 2. Inventorier le patrimoine (l'audit technique)

L'inventaire couvre quatre dimensions. Aucune ne se suffit à elle-même : c'est leur recoupement qui révèle la difficulté réelle d'une migration.

| Dimension | Ce que l'on recense | Pourquoi c'est déterminant |
|---|---|---|
| **Code & projets** | Langages présents (VB6, VB.NET, parfois C#), volume (lignes, projets), **format des fichiers projet** (ancien `.vbproj` vs *SDK-style*), framework(s) cible(s) | Donne la taille du chantier et révèle le point de départ technique réel |
| **Dépendances** | Packages NuGet, composants **COM / ActiveX**, code natif (**P/Invoke**), DLL tierces, bibliothèques internes partagées | C'est ici que la plupart des migrations réussissent… ou échouent |
| **Technologies structurantes** | UI (**Windows Forms**, WPF, **Web Forms**), accès aux données (ADO.NET, EF6, `DataSet`), intégrations (automation Office, services WCF, reporting) | Détermine s'il existe un chemin de migration — ou non (cas Web Forms) |
| **Qualité & testabilité** | Couverture de tests automatisés, documentation, complexité, *bus factor* (connaissance concentrée sur peu de personnes) | Conditionne le **risque** : sans filet de tests, toute migration est à l'aveugle |

### 2.1 Le code et les projets

Au-delà du simple décompte de lignes, deux indicateurs pèsent lourd. D'abord la **nature du code** : du VB6 et du VB.NET ne se migrent pas du tout de la même façon (→ [§11.2](02-vb6-vers-vbnet.md) pour VB6, [§11.3](03-framework-vers-net10.md) pour VB.NET sur .NET Framework). Ensuite le **format des fichiers projet** : un projet au format historique devra de toute façon passer au format *SDK-style* — un préalable technique souvent sous-estimé.

### 2.2 Les dépendances

C'est le facteur de risque numéro un. Une dépendance se classe en général dans l'une de ces catégories :

- **Packages NuGet** disposant d'une version compatible .NET moderne — cas le plus favorable.
- **Composants COM / ActiveX** (contrôles, automation Office) — migrables via interop, mais avec des contraintes (Windows uniquement, *late binding*…), traités au module [9](../09-interoperabilite/README.md).
- **Code natif appelé par P/Invoke** — fonctionne sur .NET moderne, mais à revérifier (signatures, *marshaling*).
- **DLL tierces sans équivalent moderne ni source** — le cas qui peut, à lui seul, bloquer une migration ou imposer un remplacement.

Recenser ces dépendances **avant** de décider évite la mauvaise surprise classique : découvrir au milieu du chantier qu'une brique essentielle n'a pas d'avenir sur la cible visée.

### 2.3 Les technologies structurantes

Certaines technologies conditionnent l'existence même d'un chemin de migration. Le cas le plus tranché est **ASP.NET Web Forms** : il n'existe pas hors .NET Framework et **n'a aucun chemin de migration direct** vers .NET moderne (→ [§11.4](04-web-forms-legacy.md)). À l'inverse, **Windows Forms** et **WPF** sont pleinement pris en charge sur .NET 10 et migrent généralement bien. Identifier la technologie d'UI dès l'inventaire oriente donc fortement la stratégie.

### 2.4 La qualité et la testabilité

Le code legacy est très souvent dépourvu de tests automatisés. Or, sans filet de non-régression, **rien ne garantit qu'on n'a pas modifié le comportement** en migrant. Mesurer (même grossièrement) la couverture de tests existante est donc essentiel : c'est elle qui dira s'il faut d'abord **investir dans des tests de caractérisation** avant de toucher au code (un sujet repris en [§11.7](07-gestion-risques.md)).

---

## 3. Repérer les obstacles (*blockers*)

Au-delà de l'inventaire, l'évaluation doit identifier explicitement les **points bloquants** — ce qui, sur la cible visée, n'existe plus ou se comporte différemment. Les plus fréquents :

- **APIs retirées de .NET moderne** : `.NET Remoting`, le service-side de **WCF**, `AppDomain.CreateDomain`, `BinaryFormatter` (désormais bloqué pour raisons de sécurité), `System.Web` (Web Forms), certains usages de `System.Drawing.Common` hors Windows… La liste exacte et son traitement sont détaillés en [§11.3](03-framework-vers-net10.md).
- **Dépendances liées exclusivement à Windows** — acceptables si la cible reste Windows (le cas de la plupart des applications VB.NET de bureau), bloquantes si l'on visait le multiplateforme.
- **Packages tiers sans version .NET moderne** — à remplacer, encapsuler ou… renoncer.
- **Spécificités VB.NET** : support partiel de l'espace `My` sur .NET moderne (correct en WinForms, limité en WPF, membres web supprimés — → module 2, [§2.12](../02-fondamentaux-langage/12-espace-my.md)) ; absence de modèle de projet « Worker » à recâbler à la main (→ module 4, [§4.8](../04-async/08-background-services.md)).

**Outillage.** L'étape d'analyse peut être assistée. L'agent **GitHub Copilot app modernization** (qui remplace le défunt .NET Upgrade Assistant, déprécié depuis fin 2025) intègre une phase d'**assessment** : il examine projets, dépendances et schémas de code, puis produit un fichier de diagnostic (`assessment.md`). Il prend en charge VB.NET pour certains types de projets, mais réclame un abonnement Copilot payant et **demande une relecture critique** : des cas d'hallucination (packages NuGet inexistants proposés) ont été rapportés. L'outillage de détection est traité au [§11.3](03-framework-vers-net10.md), et l'usage de l'IA en migration au module [17](../17-developpement-ia/README.md) 🤖.

---

## 4. Cartographier risque et valeur

Toutes les portions de code ne méritent pas le même traitement. Une fois l'inventaire posé, il est utile de positionner chaque application (ou module) sur deux axes : sa **valeur métier** (criticité, fréquence d'évolution, importance stratégique) et la **difficulté technique** de sa migration (dépendances, blockers, complexité).

| | Difficulté **faible** | Difficulté **élevée** |
|---|---|---|
| **Valeur métier forte** | ✅ **Priorité 1** — *quick wins*, à migrer en premier | 🎯 **Planifier soigneusement** — incrémental, étalé dans le temps |
| **Valeur métier faible** | 🔄 **Opportuniste** — migrer si l'occasion se présente, ou retirer | 🛑 **Laisser / encapsuler / retirer** — la migration coûte plus qu'elle ne rapporte |

Un critère complète utilement cette grille : la **fréquence de modification**. Du code qui évolue souvent tire un bénéfice élevé d'une modernisation (chaque évolution future en profite) ; du code stable et figé, beaucoup moins. Concentrer l'effort là où la valeur est forte **et** le code vivant maximise le retour sur investissement.

---

## 5. Les trois grandes stratégies

Une fois l'existant cartographié, reste à choisir **comment** procéder. Trois stratégies s'offrent à vous — et la première est trop souvent oubliée.

### 5.1 Ne rien faire (maintenir l'existant)

C'est une décision **légitime**, pas un aveu d'échec. `.NET Framework 4.8.1` reste pris en charge tant que la version de Windows qui l'héberge l'est. Maintenir se justifie quand le code est stable, sans problème de sécurité ni de support, peu modifié, ou porté par un produit en fin de vie.

La contrepartie doit toutefois être assumée : le risque s'accumule silencieusement (raréfaction des compétences, dépendances vieillissantes, écart technologique croissant), et une migration **forcée** finit souvent par s'imposer — dans de moins bonnes conditions que si elle avait été anticipée. « Ne rien faire » doit donc être un **choix** périodiquement réévalué, jamais une dérive subie.

### 5.2 Le *big-bang* (migration ou réécriture en bloc)

Le *big-bang* consiste à migrer (ou réécrire) l'ensemble en une seule fois, puis à basculer d'un coup de l'ancien système vers le nouveau.

Cette approche peut se justifier dans quelques cas précis : une base de code **petite** ; un changement de plateforme **imposé** sans demi-mesure possible ; ou une situation où la coexistence ancien/nouveau est techniquement impraticable — typiquement **Web Forms**, qui n'offre pas de migration *in situ* et contraint de fait à une réécriture (→ [§11.4](04-web-forms-legacy.md)).

Mais ses risques sont importants et bien documentés :

- **Gel de la livraison** : pendant toute la durée du chantier, rien de livrable — l'effort est invisible jusqu'au bout.
- **Cible mouvante** : l'ancien système continue d'évoluer (corrections, nouvelles demandes) ; la réécriture court derrière.
- **Piège de la parité fonctionnelle** : reproduire *à l'identique* des années de règles métier accumulées est plus long qu'estimé.
- **Effet « second système »** : la tentation d'en profiter pour « tout faire mieux » gonfle le périmètre.
- **Bascule tout ou rien** : le jour du basculement concentre tous les risques.

### 5.3 La voie incrémentale (recommandée)

La stratégie incrémentale migre le système **par morceaux**, en maintenant à chaque étape un système **fonctionnel et livrable**. C'est, dans la grande majorité des cas, l'approche à privilégier : livraison continue, risque maîtrisé par lots, apprentissage au fil de l'eau et retour arrière plus simple.

Elle s'appuie sur des patterns éprouvés :

- **Strangler Fig** (le « figuier étrangleur », d'après Martin Fowler) : on enveloppe l'existant et on le remplace progressivement, fonctionnalité par fonctionnalité ; le nouveau code croît autour de l'ancien jusqu'à ce que ce dernier puisse être retiré. La métaphore vient du figuier qui pousse autour d'un arbre hôte avant de s'y substituer.
- **Branch by abstraction** (branchement par abstraction) : on introduit une couche d'abstraction devant le composant à remplacer, puis on substitue l'implémentation derrière cette couche, sans casser les appelants.
- **Coexistence côte à côte** : on isole la logique partagée dans une bibliothèque `.NET Standard 2.0`, consommable à la fois par l'ancien (.NET Framework) et le nouveau (.NET moderne), le temps de la transition (→ [§11.5](05-coexistence.md)).

C'est aussi le terrain naturel de la **stratégie hybride VB.NET / C#** (→ module [10](../10-hybride-vbnet-csharp/README.md)) : on conserve l'existant en VB.NET et on n'introduit du C# que là où c'est strictement nécessaire (perf, *features* récentes), brique par brique.

### Comparaison synthétique

| Critère | *Big-bang* | Incrémental |
|---|---|---|
| Livraison pendant la migration | ❌ Gel — rien de livrable avant la fin | ✅ Système livrable à chaque étape |
| Niveau de risque | Élevé (tout ou rien) | Maîtrisé (par lots) |
| Retour arrière | Difficile | Plus simple (étape par étape) |
| Délai avant le 1er bénéfice | Long | Court |
| Coexistence ancien / nouveau | Non | Oui (→ §11.5) |
| Cas pertinents | Petite base ; plateforme imposée ; coexistence impossible (Web Forms) | La majorité des situations |

---

## 6. Choisir : les critères de décision

Le choix entre ces stratégies s'appuie sur quelques critères déterminants :

- **Taille et complexité de la base** : plus elle est grande, plus l'incrémental s'impose.
- **Présence de tests automatisés** : critère le plus structurant. Sans tests, le risque de régression est tel qu'il faut généralement **commencer par construire un filet de tests** avant de migrer (→ [§11.7](07-gestion-risques.md)).
- **Exigence de continuité de service** : un système qui ne peut pas s'arrêter exclut de fait le *big-bang*.
- **Capacité et compétences de l'équipe** : disponibilité, maîtrise de .NET moderne et de C# (utile pour l'hybride).
- **Présence de blockers durs** : une technologie sans chemin de migration (Web Forms) ou une dépendance abandonnée réoriente la stratégie vers la réécriture ou l'encapsulation.
- **Pression de calendrier** : une échéance subie — par exemple une **fin de support** (→ [Annexe H](../annexes/versions-reference/README.md)) — peut imposer un rythme, mais ne doit pas faire renoncer au filet de sécurité.

---

## 7. Formaliser : du diagnostic au plan

L'évaluation débouche sur un **livrable de décision**, pas sur une simple impression. Au minimum, il rassemble : l'**inventaire** (code, dépendances, technologies, qualité), la **cartographie risque/valeur**, la **liste des blockers** identifiés, la **stratégie retenue** (maintenir / *big-bang* / incrémental) et un **plan par phases** lorsqu'il s'agit d'incrémental.

Ce document n'a pas besoin d'être lourd. Les outils modernes formalisent d'ailleurs cette logique : la phase d'*assessment* de l'agent de modernisation produit un `assessment.md` qui peut servir de point de départ — à relire et compléter, jamais à appliquer aveuglément.

Une fois cette évaluation posée, les sections suivantes traitent les chantiers concrets : **VB6 → VB.NET** ([§11.2](02-vb6-vers-vbnet.md)), **.NET Framework → .NET 10** ([§11.3](03-framework-vers-net10.md)), le cas particulier de **Web Forms** ([§11.4](04-web-forms-legacy.md)), la **coexistence** ([§11.5](05-coexistence.md)), la **modernisation** *in situ* ([§11.6](06-moderniser.md)) et la **gestion des risques** ([§11.7](07-gestion-risques.md)).

---

## 🔑 Points clés à retenir

- **Évaluer sert la décision**, pas l'inverse : l'audit existe pour répondre à « migrer ou non, vers quoi, et comment ».
- L'**inventaire** couvre quatre dimensions — code & projets, dépendances, technologies structurantes, qualité & testabilité — et ce sont surtout les **dépendances** et les **blockers** qui font le sort d'une migration.
- La **cartographie risque/valeur** concentre l'effort là où la valeur métier est forte et le code vivant.
- Trois stratégies : **maintenir** (légitime et trop souvent oubliée), **big-bang** (réservé aux petites bases, plateformes imposées et cas sans coexistence possible comme Web Forms) et **incrémental** (la voie recommandée, via *Strangler Fig*, *branch by abstraction* et coexistence `.NET Standard`).
- **Sans tests automatisés, migrer revient à travailler à l'aveugle** : le filet de non-régression est souvent le tout premier investissement.

---

⬅️ [Module 11 — Introduction](README.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [11.2 — VB6 → VB.NET](02-vb6-vers-vbnet.md)

⏭️ [VB6 → VB.NET (outils de conversion, pièges, APIs obsolètes)](/11-migration-legacy/02-vb6-vers-vbnet.md)
