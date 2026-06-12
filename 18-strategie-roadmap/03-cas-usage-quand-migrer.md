🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 18.3 Cas d'usage optimaux pour VB.NET : quand rester, quand migrer vers C#

Voici le cœur décisionnel du module. La section [18.1](01-strategie-microsoft.md) a établi la stratégie (stable, figée, scénarios cœur) ; la section [18.2](02-roadmap-dotnet.md) a réglé la question du support (solide, et **hors de l'équation**). Reste donc l'arbitrage qui se joue sur les **capacités et l'écosystème** : quand VB.NET est-il le bon outil, et quand faut-il migrer vers C# ? Cette section vous donne la grille pour le peser dans **votre** contexte.

> **Rappel de posture.** Ceci est une aide à la décision, pas un verdict. Le bon choix dépend de votre situation ; la section vous outille pour le faire, elle ne le fait pas à votre place.

---

## Reposer la question correctement : ce n'est pas binaire

On formule trop souvent le choix comme un « VB **ou** C# ». C'est une fausse alternative. Il existe un **spectre** :

> **Rester en VB.NET** → **Hybride (VB + C#)** → **Migrer vers C#**

La voie médiane — l'hybride ([module 10](../10-hybride-vbnet-csharp/README.md)) — n'est pas un compromis bancal : c'est **souvent la bonne réponse**, et nous y revenons plus bas.

La question centrale, elle, tient en un arbitrage — le « pari calibré » du module. Pour votre contexte précis : le **coût de la frontière** — ce que VB ne fait pas nativement ([Annexe B](../annexes/frontiere-vbnet-csharp/README.md)) plus la friction d'un écosystème C#-first — reste-t-il **gérable** (→ rester, éventuellement en hybride), ou finit-il par **dépasser le coût d'une migration** (→ migrer) ? Le support, lui, n'entre pas en ligne de compte ([18.2](02-roadmap-dotnet.md)) : tout se joue sur les capacités et l'écosystème.

---

## Quand VB.NET est le bon outil (rester)

Les situations où VB est productif et le coût de la frontière est faible :

- **Les scénarios cœur nommés par Microsoft** ([18.1](01-strategie-microsoft.md)) : Windows Forms ⭐ ([module 5](../05-windows-forms/README.md)) et bibliothèques de classes. C'est le terrain où VB est censé vivre, et où il l'est pleinement.
- **Les applications de bureau et métier (LOB) à longue durée de vie** (WinForms, WPF — [module 6](../06-wpf/README.md), outils internes). Ici, la stabilité du langage est une **fonctionnalité** : pas de *churn*, une maintenance prévisible ([18.2](02-roadmap-dotnet.md)).
- **Les bases de code VB saines et fonctionnelles.** Du code qui fait son travail n'est pas un problème à résoudre. Réécrire du code qui marche, c'est du coût et du risque pour aucun gain fonctionnel.
- **Une équipe compétente en VB.NET.** Le facteur humain compte : une équipe fluente en VB livre plus vite en VB que dans un langage qu'elle apprend.
- **L'interopérabilité COM et l'automation Office** ([module 9](../09-interoperabilite/README.md)) : Excel, Word, Outlook — une force historique de VB, idiomatique et confortable.
- **L'accès aux données** ([module 7](../07-acces-donnees/README.md)) et les **Web API par contrôleurs** ([8.2](../08-services-web/02-web-api-controllers.md) ✅) : pleinement supportés.
- **La maintenance et l'évolution incrémentale de legacy** ([module 11](../11-migration-legacy/README.md)).

Le fil commun : VB sur son terrain, une base saine, une équipe à l'aise, une frontière peu coûteuse.

---

## Quand migrer vers C# (la frontière l'emporte)

Les situations où VB n'est plus le bon outil :

- **Un besoin de *workloads* hors périmètre** ([Annexe B](../annexes/frontiere-vbnet-csharp/README.md)) : Blazor (web *front-end*), .NET MAUI / WinUI 3 (UI modernes, multiplateforme), Minimal APIs comme style d'API principal, Native AOT, gRPC / GraphQL, microservices et architectures *cloud-native*, écriture de *source generators*. Si l'**avenir** de votre projet repose sur ces briques, C# est le chemin.
- **La pression cumulée de l'écosystème.** Documentation, bibliothèques, communauté, vivier d'embauche — et désormais le **biais C# des assistants IA** ([module 17](../17-developpement-ia/README.md)). Quand la « taxe de frontière » vous coûte de façon mesurable, et plus encore si recruter des développeurs VB est difficile, la friction cumulée plaide pour C#.
- **Une direction de croissance vers le moderne** : si la base de code grandit vers le *cloud-native*, le web ou le mobile moderne, la destination est un terrain C#.
- **Un projet neuf (*greenfield*) sans raison VB spécifique** : sans legacy VB, sans interop Office, sans équipe VB, C# est le choix moderne par défaut — on démarre en C# à moins d'une raison concrète de faire autrement.

Le fil commun : le coût de la frontière dépasse celui de la migration.

---

## La voie médiane : l'hybride (souvent la meilleure réponse)

Inutile de jeter du VB qui fonctionne pour accéder à des capacités modernes. L'hybride ([module 10](../10-hybride-vbnet-csharp/README.md)) consiste à **garder VB là où il est productif** (interface, logique métier, base existante) et à **déléguer à C# uniquement ce que VB ne fait pas** (performance et `Span`, *records*, *source generators*, *workloads* modernes). Microsoft **valide** explicitement cette voie par son investissement dans l'interopérabilité ([18.1](01-strategie-microsoft.md)).

L'hybride est souvent **préférable à une migration totale** : moins de risque, moins de coût, progressif — et il sert aussi de **tremplin** si vous décidez plus tard de migrer entièrement ([18.4](04-migrer-vers-csharp.md)). Il recadre la décision : il s'agit rarement de « tout VB ou tout C# », mais de savoir **où, sur le spectre**, votre contexte se situe.

---

## Bonnes et mauvaises raisons — l'honnêteté du choix

L'erreur guette des deux côtés. On migre parfois par panique, on reste parfois par inertie — et aucun des deux n'est une stratégie. La grille des **raisons** :

| | Bonne raison | Mauvaise raison |
|---|---|---|
| **Migrer vers C#** | Un besoin concret de capacité que VB ne fournit pas, ou une friction d'écosystème qui vous coûte mesurablement | *« VB est figé/mort, donc il faut partir »* (faux — voir [18.1](01-strategie-microsoft.md) / [18.2](02-roadmap-dotnet.md)) |
| **Rester en VB.NET** | VB productif sur ses scénarios cœur, base saine, équipe compétente, frontière peu coûteuse | L'habitude, le coût irrécupérable, la peur de C# |

Le principe : **ne pas migrer pour migrer**, ne pas rester par habitude. Une migration se justifie par un **besoin concret**, jamais par le seul statut « stabilisé » du langage — que [18.1](01-strategie-microsoft.md) et [18.2](02-roadmap-dotnet.md) ont montré parfaitement viable. Symétriquement, si la frontière coûte cher et grandit, rester par confort est aussi un coût.

---

## En résumé — la grille en une phrase

Recadrez le choix en **spectre** (rester → hybride → migrer), sortez le support de l'équation, et pesez le **coût de la frontière** contre le **coût de migration** pour votre contexte. **Restez** quand VB est sur son terrain, avec une base saine et une équipe compétente ; **penchez vers l'hybride** pour gagner des capacités modernes sans jeter du VB qui marche ; **migrez** quand la frontière dépasse réellement le coût du déplacement. Le *comment* de la migration — outils, arbitrage coût/bénéfice, hybride comme tremplin — fait l'objet de la section suivante, [18.4](04-migrer-vers-csharp.md).

⏭️ [Migrer vers C# si nécessaire (outils de conversion, coût/bénéfice, code hybride)](/18-strategie-roadmap/04-migrer-vers-csharp.md)
