🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 18.4 Migrer vers C# si nécessaire (outils de conversion, coût/bénéfice, code hybride)

La section [18.3](03-cas-usage-quand-migrer.md) a réglé le *si* et le *quand* ; celle-ci traite du *comment*, pour le cas où la décision penche vers C#. Trois volets : les **outils de conversion** (ce qu'ils font et ne font pas), l'**arbitrage coût/bénéfice** (quantifier la décision), et — la pièce maîtresse — le **code hybride comme chemin de migration incrémentale**.

> **Une distinction à garder.** Il s'agit ici de **VB.NET → C#** (changer de langage), à ne pas confondre avec le [module 11](../11-migration-legacy/README.md) (VB6 → VB.NET, Framework → .NET 10, qui modernise *en restant* en VB). La discipline de migration du module 11 — filet de tests, gestion des risques ([11.7](../11-migration-legacy/07-gestion-risques.md)) — s'applique néanmoins à toute migration.

---

## Les outils de conversion VB→C#

L'infrastructure Roslyn comprend les deux langages, ce qui rend la **conversion syntaxique** automatisable. On dispose en pratique du convertisseur open source de référence (ICSharpCode.CodeConverter, qui motorise des extensions Visual Studio et des convertisseurs en ligne), d'extensions d'IDE, et — en 2026 — des **assistants IA** ([module 17](../17-developpement-ia/README.md)).

Mais il faut être lucide sur ce que ces outils livrent :

- **Conversion syntaxique ≠ C# idiomatique.** Les outils produisent du C# qui *compile*, mais c'est souvent du « VB écrit avec la syntaxe C# » — maladroit, non naturel. Il faut une **refactorisation humaine** pour en faire du C# idiomatique.
- **Les idiomes VB ne se convertissent pas proprement.** L'espace `My` n'a **pas d'équivalent C#** ([2.12](../02-fondamentaux-langage/12-espace-my.md)) et doit être remplacé à la main ; le couple `Handles` / `WithEvents` ([3.6](../03-poo/06-evenements-delegues.md)) devient un `+=` explicite, selon un autre modèle ; les restes de liaison tardive demandent une reprise.
- **Les pièges sémantiques — le miroir du [module 17](../17-developpement-ia/README.md).** Les mêmes différences d'opérateurs piègent dans l'autre sens. L'exemple le plus traître :

  ```vb
  ' VB : ^ est la PUISSANCE
  Dim r = a ^ b            ' a élevé à la puissance b
  ```
  ```csharp
  // ❌ Conversion littérale FAUSSE : en C#, ^ est le XOR binaire
  // var r = a ^ b;        // comportement totalement différent !
  // ✅ Équivalent correct :
  var r = Math.Pow(a, b);
  ```

  De même, la division entière `\` de VB devient `/` sur des entiers en C#, et l'insensibilité à la casse de VB cède à la sensibilité de C#. Ces écarts exigent le **filet de tests**.
- **La conversion est le début, pas la fin.** L'outil vous mène à du C# qui compile ; en faire du *bon* C# reste un travail humain.

Côté IA, une nuance intéressante : pour VB→C#, le **biais C# du modèle devient un atout** (la cible est sa langue forte) — l'inverse de la génération vers VB. Mais la lecture du source VB et la vérification de l'**équivalence sémantique** ([17.3](../17-developpement-ia/03-migration-legacy-ia.md), [17.7](../17-developpement-ia/07-limites-pieges.md)) restent indispensables, et l'IA aide aussi à l'étape de **refactorisation idiomatique**. Le filet de tests, lui, ne change pas.

---

## L'arbitrage coût/bénéfice

[18.3](03-cas-usage-quand-migrer.md) a décidé *s'il faut* migrer ; ici, on **quantifie**.

**Les coûts** — souvent sous-estimés :

- **L'effort** : conversion **plus** refactorisation idiomatique **plus** re-tests **plus** montée en compétence de l'équipe. Ce sont la refactorisation et les tests qui dominent, pas la conversion syntaxique.
- **Le risque** : dérive sémantique et régressions sur du code qui marchait — le classique de la « réécriture qui réintroduit d'anciens bugs ».
- **Le coût d'opportunité** : le temps passé à migrer n'est pas passé à livrer de la valeur.
- **La perturbation** : gel des fonctionnalités, base devenue cible mouvante, bascule de contexte pour l'équipe.

Et le point cardinal : **une migration de langage pure n'apporte aucune fonctionnalité nouvelle**. Vous payez le coût plein pour aboutir, fonctionnellement, là où vous étiez — à moins que la migration ne **débloque** une capacité.

**Les bénéfices** — réels seulement s'il existe un moteur concret (en écho aux « bonnes raisons » de [18.3](03-cas-usage-quand-migrer.md)) :

- Les **capacités de frontière** débloquées ([Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).
- L'**alignement sur l'écosystème** : documentation, bibliothèques, communauté, embauche, et l'IA dans votre langage.
- La **taxe de frontière** qui disparaît pour l'avenir.

**Le principe.** Une migration pure, sans moteur de capacité, est rarement rentable : coût plein, aucun gain fonctionnel, plus le risque (la « mauvaise raison » de [18.3](03-cas-usage-quand-migrer.md)). Elle paie quand elle **débloque** quelque chose ou **supprime** une friction mesurable et croissante. Quantifiez honnêtement (l'effort dépasse presque toujours l'estimation), et pesez le coût de **rester** (la taxe de frontière continue) contre celui de **partir**, sur un horizon réaliste.

---

## Le code hybride : la migration incrémentale (la clé)

Voici l'idée stratégique forte, qui prolonge [18.3](03-cas-usage-quand-migrer.md) et le [module 10](../10-hybride-vbnet-csharp/README.md). L'hybride n'est pas seulement un état final : c'est aussi une **stratégie de migration** — progressive et à faible risque.

**Le principe.** Au lieu d'une réécriture *big-bang*, on migre **morceau par morceau**. VB et C# coexistent dans la solution (en projets distincts : un projet est monolingue, mais une solution mêle des projets VB et C# qui se référencent). Comme ils compilent vers le même IL, l'interopérabilité est **transparente et validée par Microsoft** ([18.1](01-strategie-microsoft.md)).

**Comment.** On migre d'abord les **bibliothèques feuilles** (faiblement couplées) en remontant vers l'interface, ou l'on écrit les **nouvelles fonctionnalités en C#** ; on convertit un composant à la fois, en **validant à chaque étape** (filet de tests par incrément — [11.7](../11-migration-legacy/07-gestion-risques.md), [17.4](../17-developpement-ia/04-generer-tests-doc.md)), pendant que le reste continue de tourner.

**Pourquoi cela dérisque.** Des pas **petits et réversibles** (vs *big-bang*) ; une **validation continue** ; l'application **reste fonctionnelle** tout du long (pas de gel) ; et l'on peut **s'arrêter à tout moment** — une migration partielle est un état final valide.

**Le tremplin** (de [18.3](03-cas-usage-quand-migrer.md)). Même si l'objectif est le tout-C#, l'hybride en est le **chemin** ; et si vous vous arrêtez à mi-course, vous avez tout de même gagné les capacités dont vous aviez besoin (les parties C#), sans le risque d'une réécriture intégrale.

**La nuance.** Un hybride **permanent** a un coût continu (deux langages, deux jeux de compétences, la frontière d'interop — c'est la décision du [module 10](../10-hybride-vbnet-csharp/README.md) et de [18.3](03-cas-usage-quand-migrer.md)). Un hybride **transitoire**, lui, échange un peu de complexité contre un risque de migration bien moindre — un marché généralement avantageux.

---

## En résumé

Les outils de conversion traitent la part mécanique mais produisent du C# non idiomatique, à **refactoriser et valider** (la même discipline de filet de tests que toute migration, avec les pièges sémantiques en miroir — `^`, `\`, casse). L'arbitrage coût/bénéfice penche rarement en faveur d'un simple changement de langage sans moteur de capacité : ne migrez que si cela **débloque** quelque chose ou **supprime** une friction mesurable. Et la manière intelligente d'exécuter, c'est l'**hybride comme chemin incrémental et réversible** — avancer morceau par morceau, valider en continu, s'arrêter quand on a obtenu l'essentiel. Migrer vers C# n'a rien d'un *big-bang* terrifiant.

La dernière section, [18.5](05-ressources-communaute.md), referme le module et la formation : où continuer d'apprendre et trouver du soutien, en VB.NET comme dans la transition.

⏭️ [Communauté, documentation, livres, formation continue, outils tiers](/18-strategie-roadmap/05-ressources-communaute.md)
