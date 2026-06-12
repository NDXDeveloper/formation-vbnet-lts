🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11.7 — Gestion des risques (sauvegarde, tests de non-régression, *rollback*)

> Une migration modifie un logiciel **qui fonctionne déjà** et qui a des utilisateurs. Son risque central est la **régression** : casser un comportement existant. La gestion des risques est précisément ce qui permet de migrer **avec audace, mais sans imprudence**. C'est la discipline qui sous-tend, en filigrane, chaque « approche incrémentale » et chaque « filet de tests » du chapitre — cette section la rend explicite.

---

## 1. Le risque central d'une migration : la régression

Migrer, c'est changer un logiciel qui marche et dont des gens dépendent. Le risque numéro un n'est pas que le nouveau code échoue à compiler — c'est qu'il **ne fasse plus exactement ce que l'ancien faisait**.

D'où un paradoxe à résoudre : la migration **doit** avoir lieu (rester sur une plateforme figée a un coût, → [§11.1](01-evaluer-strategies.md)), mais casser la production est **inacceptable**. La gestion des risques réconcilie les deux. C'est elle qui transforme l'approche incrémentale en démarche **sûre** plutôt que téméraire.

Concrètement, elle s'organise en **trois lignes de défense**, qui correspondent aux trois volets du titre :

1. **Tests de non-régression** — détecter les régressions **avant** qu'elles n'atteignent la production.
2. **Sauvegarde** — pouvoir **revenir** à l'état précédent (code et données).
3. **Rollback** — pouvoir **battre en retraite** en production quand une étape déployée se révèle défaillante malgré les tests.

Le principe directeur en découle : chaque étape de migration doit être **petite, testée, déployable et réversible**.

---

## 2. Sauvegarde : pouvoir revenir en arrière

Une distinction fondamentale s'impose d'emblée : le **code** se récupère facilement ; les **données** sont la vraie zone de risque.

### 2.1 Le code : versionnage et build reproductible

- **Git** est le socle : **marquer (tag) l'état d'avant migration**, pour pouvoir y revenir à tout moment.
- Mener la migration **sur une branche**, avec des **commits petits et fréquents** (chaque étape redevient ainsi annulable), en gardant la branche principale héritée **toujours compilable**.
- **Conserver un build hérité reproductible** : chaîne d'outils et dépendances épinglées, afin de pouvoir reconstruire et exécuter la version .NET Framework — essentiel, puisque la coexistence ([§11.5](05-coexistence.md)) exige que l'ancien build continue de fonctionner pendant la transition.

En pratique, le code est **toujours récupérable** : il est dans le gestionnaire de versions.

### 2.2 Les données : la vraie zone de risque ⚠️

La base de données est **l'actif irremplaçable** — et la partie qui, elle, n'est **pas trivialement réversible**.

- **Sauvegardes complètes avant toute migration de schéma ou de données.**
- **Tester la restauration** : une sauvegarde non testée n'est pas une sauvegarde.

C'est ici qu'une migration peut faire des dégâts **irréversibles**. Les changements de données méritent infiniment plus de prudence que les changements de code (cf. §4.3 sur le rollback de schéma).

---

## 3. Tests de non-régression : prouver que rien n'a changé

La question centrale d'une migration est : **« fait-il toujours la même chose ? »** Les tests sont le seul moyen d'y répondre **à chaque étape**, à grande échelle. L'obstacle est connu : le code hérité est souvent **non testable** (aucune couture où insérer un test) — c'est le cercle vicieux du [§11.6](06-moderniser.md).

### 3.1 Les tests de caractérisation

Ce sont des tests qui capturent le comportement **actuel** — quel qu'il soit, **bugs compris** — et non le comportement « correct ». Leur but n'est pas de juger, mais de **détecter le moindre écart** introduit par la migration. Ils sont précieux quand on ne peut pas (encore) écrire de tests unitaires fins : on les place alors **aux frontières** (API, sorties d'interface, résultats calculés).

### 3.2 Le *golden master* (test d'approbation)

On capture les **sorties** pour un large éventail d'entrées **avant** la migration, puis on les compare **après**. C'est la technique idéale pour du code hérité aux sorties complexes et dépourvu de tests existants.

### 3.3 La suite de non-régression comme contrat

La suite de tests devient la **spécification de « ce qui ne doit pas changer »** ; on l'exécute **à chaque étape** de migration. Quelques compléments :

- **Tests d'intégration** (→ module 13, [§13.3](../13-tests-qualite/03-tests-integration.md)) sur une **vraie base** (Testcontainers) : vérifier que la couche d'accès aux données migrée se comporte à l'identique.
- **Ne pas viser 100 % de couverture** : prioriser par risque × valeur ([§11.1](01-evaluer-strategies.md)) — couvrir les chemins critiques et le code le plus modifié.
- L'**IA** peut générer des tests (→ module 13, [§13.5](../13-tests-qualite/05-couverture-tests-ia.md) ; module [17](../17-developpement-ia/README.md)), mais le résultat se **valide** 🤖.

Cette suite n'est pas un coût à fonds perdu : le même filet qui sécurise la migration **débloque aussi la modernisation** ([§11.6](06-moderniser.md)). Un seul investissement, deux bénéfices (→ module [13](../13-tests-qualite/README.md)).

---

## 4. Rollback : pouvoir battre en retraite en production

Même avec des tests, la production révèle des problèmes que les tests n'avaient pas vus. Il faut donc une **retraite organisée**.

### 4.1 Le plan de repli, défini *avant* le déploiement

Qu'est-ce qui **déclenche** un rollback ? **Qui** décide ? **Comment** s'exécute-t-il, et en **combien de temps** ? Ces réponses se définissent **à froid**, en amont — pas dans l'urgence d'un incident.

### 4.2 Les stratégies de déploiement qui rendent le retour possible

| Stratégie | Principe | Retour arrière |
|---|---|---|
| **Feature flags** | Activer/désactiver le nouveau chemin **sans redéployer** | Instantané : on rebascule l'interrupteur |
| **Blue-green** | Deux environnements de production ; on bascule le trafic | On rebascule sur l'ancien environnement |
| **Canary** | Exposer la nouvelle version à un **petit pourcentage** d'utilisateurs d'abord | Élargir si sain, se replier sinon |
| **Strangler Fig / YARP** | Migration **route par route** (→ [§11.1](01-evaluer-strategies.md), [§11.4](04-web-forms-legacy.md)) | Une route défaillante est redirigée vers l'ancien |

### 4.3 Le point dur : le rollback des données ⚠️

Annuler du **code** est simple : on redéploie les anciens binaires. Annuler un changement de **schéma ou de données** ne l'est **généralement pas**. Les stratégies à connaître :

- **Migrations rétrocompatibles (*expand/contract*)** : **ajouter avant de retirer**. On ajoute la nouvelle colonne/structure, on fait coexister ancien et nouveau code le temps de la transition, et on ne supprime l'ancien **qu'une fois le nouveau prouvé**. Jamais de changement de schéma cassant en une seule étape.
- **Ne jamais supprimer prématurément** : conserver colonnes et tables anciennes tant que le nouveau chemin n'est pas validé.
- Migrations **réversibles** quand c'est possible ; les **sauvegardes** (cf. §2.2) comme dernier recours.

### 4.4 La détection conditionne le repli

On ne peut se replier que sur un problème que l'on **détecte**. La **journalisation**, les *health checks*, les **métriques** et l'observabilité (→ module 12, [§12.3](../12-exceptions-debogage/03-journalisation.md) et [§12.4](../12-exceptions-debogage/04-observabilite.md)) sont donc la condition même du rollback : sans surveillance, une régression silencieuse n'est découverte que lorsque les utilisateurs se plaignent.

---

## 5. Synthèse : de l'audace sans imprudence

La gestion des risques est ce qui réconcilie une migration **ambitieuse** avec la **sûreté** de la production. Ses trois lignes de défense recouvrent exactement les trois volets du titre :

- les **tests de non-régression** *préviennent* les régressions ;
- la **sauvegarde** permet de *récupérer* l'état (code et données) ;
- le **rollback** permet de *récupérer* la production.

Le tout converge vers une seule discipline : des étapes **petites, testées, déployables et réversibles** — c'est-à-dire la définition même de l'approche incrémentale sûre du [§11.1](01-evaluer-strategies.md). Le guide de migration et sa checklist figurent en [Annexe E](../annexes/migration-net10/README.md).

---

## 🔑 Points clés à retenir

- Le risque central d'une migration est la **régression** ; la gestion des risques est ce qui permet **l'audace sans l'imprudence**.
- **Trois lignes de défense** : tests de non-régression (*prévenir*), sauvegarde (*récupérer l'état*), rollback (*récupérer la production*).
- **Le code est récupérable** (Git + build hérité reproductible) ; **les données sont la vraie zone de risque** — sauvegarder avant tout changement de schéma/données, et **tester la restauration**.
- Les **tests de caractérisation / golden master** capturent le comportement **actuel** aux frontières, même quand les tests unitaires fins ne sont pas encore possibles ; la suite de non-régression est le **contrat** de « ce qui ne doit pas changer ».
- **Planifier le rollback avant de déployer** : feature flags, blue-green, canary, route par route (Strangler Fig).
- **Le rollback des données est le point dur** : migrations rétrocompatibles (*expand/contract*), ne jamais supprimer prématurément — et **surveiller**, car on ne se replie que sur un problème détecté.
- Chaque étape de migration : **petite, testée, déployable, réversible**.

---

⬅️ [11.6 — Moderniser (async, LINQ, EF Core, DI, testabilité)](06-moderniser.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [11.8 — Migration assistée par IA (→ module 17)](../17-developpement-ia/README.md)

⏭️ [Exceptions, débogage et journalisation](/12-exceptions-debogage/README.md)
