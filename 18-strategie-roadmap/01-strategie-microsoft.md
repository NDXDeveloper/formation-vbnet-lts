🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 18.1 La stratégie officielle Microsoft pour VB.NET (rappel et lecture)

Cette section fait deux choses, comme l'annonce son titre : un **rappel** — réénoncer précisément la stratégie officielle — et une **lecture** — l'interpréter correctement. Tout le module 18 repose sur une compréhension juste de cette stratégie, et elle le mérite : c'est probablement le fait le plus **mal lu** au sujet de VB.NET, dans deux directions opposées.

---

## Le rappel : ce que dit la stratégie

Microsoft a fixé sa position dès **2020** et la maintient sans varier depuis. La formulation officielle :

> *« Visual Basic adoptera généralement une approche de consommation seule et évitera toute nouvelle syntaxe. Visual Basic ne sera pas étendu à de nouveaux workloads. Nous continuerons à investir dans l'expérience Visual Studio et l'interopérabilité avec C#, en particulier dans les scénarios cœur de Visual Basic comme Windows Forms et les bibliothèques. »*
> — *Stratégie du langage Visual Basic*, Microsoft Learn (citée en tête du [Sommaire](/SOMMAIRE.md))

Quatre engagements s'y lisent : une approche de **consommation seule** ; l'absence de **nouvelle syntaxe** (le langage est figé) ; **pas d'extension à de nouveaux *workloads*** ; et un **investissement continu** dans l'outillage Visual Studio, l'interopérabilité avec C#, et les scénarios cœur — Windows Forms et les bibliothèques.

Ce n'est pas une annonce ponctuelle qu'on pourrait croire dépassée : c'est une politique **réaffirmée et tenue** à chaque version. VB tourne sur .NET 10, Visual Studio 2026 le prend en charge — la stratégie « stabilisée » est elle-même stable.

---

## La lecture : sortir des deux contresens

Cette stratégie est mal comprise précisément parce qu'on la tire vers l'un de deux extrêmes. La lecture juste se tient **entre les deux**.

**Le contresens catastrophiste** — *« VB est mort, déprécié, abandonné. »* C'est faux. La stratégie **s'engage explicitement** sur le support, la maintenance, l'investissement dans Visual Studio et les scénarios cœur. *« Stabilisé »* n'est pas *« déprécié »* : il n'y a ni dépréciation, ni fin de vie annoncée. Le code VB continue de tourner, d'être supporté, de recevoir l'outillage et la maintenance (sécurité, runtime).

**Le contresens du déni** — *« Rien n'a vraiment changé, VB finira par rattraper C#. »* Faux également. La politique est **explicite et durable** : pas de nouvelle syntaxe, pas de nouveaux *workloads*. L'écart avec C# se **creuse à chaque version**, par conception. Attendre une parité est l'erreur symétrique de la première.

**La lecture juste**, donc : VB.NET est un langage **stable, soutenu, mais figé** — maintenu pour ses scénarios cœur, sans évoluer. C'est une décision délibérée et de long terme, ni un abandon, ni une pause.

---

## Décortiquer les mots clés

La « lecture » au sens propre, c'est parser la formulation, terme par terme :

- **« Consommation seule ».** VB *utilise* les fonctionnalités et bibliothèques de la plateforme — y compris les plus récentes, écrites en C# — mais ne gagne pas la *syntaxe* pour les **déclarer**. On *consomme* un `record` ou un `Span` venus de C#, on ne les *déclare* pas en VB. C'est exactement le fondement de la stratégie hybride ([module 10](../10-hybride-vbnet-csharp/README.md)) et de l'intégration par consommation ([17.9](../17-developpement-ia/09-consommer-ia.md)).
- **« Éviter toute nouvelle syntaxe »** — avec le mot **« généralement »**. Le langage est figé ; le « généralement » laisse place à de menus ajustements de compatibilité, mais la **direction** est sans ambiguïté. Ne le sur-lisez pas comme « ils pourraient donc ajouter des fonctionnalités ».
- **« Pas de nouveaux *workloads* ».** C'est la source de la **frontière** ([Annexe B](../annexes/frontiere-vbnet-csharp/README.md)). Blazor, MAUI, Minimal APIs et consorts sont ciblés C# ; VB n'y sera pas étendu.
- **« Investir dans l'expérience VS et l'interop C# ».** L'engagement *positif*, souvent oublié. L'outillage VB continue de progresser (Visual Studio 2026) ; et l'interopérabilité avec C# est un investissement **explicite** — Microsoft **valide** ainsi la voie hybride. Lisez : *« utilisez C# pour ce que VB ne fait pas ; nous gardons l'interop fluide. »*
- **« Scénarios cœur : Windows Forms et les bibliothèques ».** Microsoft **désigne lui-même** le terrain de VB. Toute cette formation n'est, au fond, qu'une lecture appliquée de cette phrase.

---

## Ce que cela implique stratégiquement

De cette lecture découlent quelques conséquences qui préparent la grille de décision de [18.3](03-cas-usage-quand-migrer.md) :

- **La stabilité est une fonctionnalité, pas un défaut.** Un langage figé est un socle **prévisible** : pas de *churn* syntaxique, une surface d'apprentissage qui cesse de croître, une maintenance sans surprise. Pour des applications de bureau ou métier à longue durée de vie, c'est une vraie valeur.
- **Distinguer la plateforme du langage.** Le **runtime** évolue (VB encaisse gratuitement les gains de perf de .NET 10 — [14.6](../14-performance/06-apports-net10.md)) ; le **langage**, non. VB continue de recevoir runtime, sécurité et performance — simplement pas de nouvelles fonctionnalités de langage. Confondre les deux fausse tout le raisonnement.
- **Les capacités nouvelles passent par la consommation.** Ne les attendez pas du langage ; obtenez-les via des bibliothèques C# et l'hybride ([module 10](../10-hybride-vbnet-csharp/README.md)).
- **L'hybride est béni, pas bricolé.** L'engagement explicite sur l'interop fait de la voie hybride un chemin **de première classe, validé par Microsoft** — non un contournement.

---

## En résumé

Lue correctement, la stratégie tient en quatre mots : **stable, soutenue, figée, cœur**. VB.NET n'est ni mort, ni en train de rattraper C# : c'est un langage délibérément stabilisé, maintenu pour un terrain que Microsoft nomme explicitement. Cette lecture juste est le **socle** de toutes les décisions du reste du module — quand rester ([18.3](03-cas-usage-quand-migrer.md)), quand migrer ([18.4](04-migrer-vers-csharp.md)), et comment la feuille de route et le support vous couvrent dans le temps ([18.2](02-roadmap-dotnet.md)).

⏭️ [Support long terme et feuille de route .NET (cycles LTS / STS)](/18-strategie-roadmap/02-roadmap-dotnet.md)
