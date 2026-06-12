🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11.6 — Moderniser (async, LINQ, EF Core, injection de dépendances, testabilité)

> Migrer met le code sur la **bonne plateforme** ; moderniser le rend **idiomatique**. Le code sorti d'un convertisseur ([§11.2](02-vb6-vers-vbnet.md)) ou porté de .NET Framework vers .NET 10 ([§11.3](03-framework-vers-net10.md)) compile et s'exécute — mais il reste souvent « à l'ancienne ». Cette section couvre les cinq axes de modernisation, et surtout la façon de les appliquer **progressivement et sans casse**.

---

## 1. Migrer n'est pas moderniser

La migration et la modernisation sont deux choses distinctes. La première garantit que le code tourne sur une plateforme supportée ; la seconde le rend lisible, maintenable et performant. Le résultat d'une conversion est un **point de départ, pas une destination**.

Surtout, la modernisation est un effort **optionnel, priorisé et continu** — jamais un « tout ou rien ». Trois règles d'or l'encadrent :

- **Derrière un filet de tests** : on ne modernise jamais sans filet de non-régression (→ [§11.7](07-gestion-risques.md)). C'est non négociable.
- **Un sujet à la fois**, de façon incrémentale (→ [§11.1](01-evaluer-strategies.md)).
- **Là où ça paie** : concentrer l'effort sur le code activement modifié et à forte valeur (cartographie risque/valeur du [§11.1](01-evaluer-strategies.md)), selon la règle du *boy-scout* — améliorer le code **au fur et à mesure qu'on y touche**, plutôt que de lancer un grand chantier dédié.

Enfin, les cinq axes sont **liés** : l'injection de dépendances débloque la testabilité ; l'async, LINQ et EF Core sont les améliorations idiomatiques. On peut les aborder dans cet ordre.

---

## 2. Asynchronie : libérer le thread d'interface

**Avant** : E/S bloquantes, `BackgroundWorker`, threads gérés à la main, appels qui figent le thread d'interface (formulaires « gelés »).  
**Après** : `Async`/`Await`, `Task` / `Task(Of T)`, annulation par `CancellationToken` (→ module [4](../04-async/README.md)).

**Le bénéfice** est décisif pour un développeur VB.NET : une **interface réactive**, point essentiel en Windows Forms et WPF — le cœur du langage — ainsi qu'une montée en charge propre des opérations d'E/S.

**Précautions de modernisation :**

- **L'asynchronie est contagieuse** (« *async all the way up* ») : rendre une méthode asynchrone n'est **pas un changement local** — cela se propage vers le haut de la pile d'appels.
- **Ne jamais bloquer sur de l'asynchrone** (`.Result`, `.Wait()`, `GetAwaiter().GetResult()`) : risque d'**interblocage**, en particulier sous le contexte de synchronisation de WinForms/WPF.
- `Async Sub` (l'équivalent du `async void`) est réservé aux **gestionnaires d'événements**.
- *Note VB* : `Async Function … As Task` / `Await` ; Windows Forms sur .NET 10 ajoute les formulaires asynchrones `ShowAsync` / `ShowDialogAsync` (→ module 5, [§5.2](../05-windows-forms/02-winforms-net10.md)).

> 💡 **Moderniser l'interface, c'est aussi remplacer les contrôles hérités.** L'exemple le plus courant : l'ancien contrôle `WebBrowser` (moteur Internet Explorer, sans support des standards web actuels) se remplace par **WebView2** (moteur Chromium) — et son initialisation, précisément **asynchrone** (`EnsureCoreWebView2Async`), illustre l'esprit de cet axe. La technique complète est au module 9, [§9.4](../09-interoperabilite/04-webview2.md).

---

## 3. LINQ : du code impératif au code déclaratif

**Avant** : boucles impératives avec filtrage et accumulation manuels, boucles imbriquées.  
**Après** : LINQ — un **point fort de VB.NET** (→ module 2, [§2.9](../02-fondamentaux-langage/09-linq.md) ⭐), dont la syntaxe requête se lit particulièrement bien :

```vb
' Avant — impératif
Dim actifs As New List(Of Client)
For Each c In clients
    If c.EstActif Then actifs.Add(c)
Next

' Après — déclaratif (LINQ, syntaxe requête)
Dim actifs = From c In clients
             Where c.EstActif
             Select c
```

**Le bénéfice** : du code déclaratif, concis et moins sujet aux erreurs.

**Précautions de modernisation :**

- **Exécution différée** : une requête LINQ est paresseuse ; l'énumérer deux fois la **ré-exécute**. Matérialiser avec `.ToList()` quand le résultat doit être réutilisé.
- **Lisibilité avant virtuosité** : une boucle claire vaut mieux qu'une chaîne LINQ obscure.
- **Chemins chauds** : LINQ alloue ; dans le code critique, une boucle peut être plus performante (→ module [14](../14-performance/README.md)).

---

## 4. EF Core : moderniser l'accès aux données

**Avant** : ADO.NET (`SqlCommand`/`DataReader`, `DataSet`/`DataTable`) ou EF6.  
**Après** : EF Core 10 (`DbContext`/`DbSet`, LINQ to Entities, migrations, requêtes asynchrones) (→ module 7, [§7.2](../07-acces-donnees/02-ef-core-10.md)).

**Le bénéfice** : requêtes fortement typées, migrations de schéma, asynchronie, moins de code répétitif, et liberté de fournisseur de base de données.

**Précautions de modernisation** — c'est le plus lourd des cinq axes :

- **EF6 → EF Core est une réécriture, pas une mise à niveau** : c'est un ORM **différent**, aux API et comportements distincts. Ne pas supposer que le code EF6 « fonctionnera tel quel ».
- **`DataSet`/`DataTable` → classes d'entités** : un changement de paradigme, pas une substitution mécanique.
- **Coexistence possible** : on peut introduire EF Core **à côté** de l'ADO.NET existant, table par table ou module par module — inutile de tout convertir d'un coup.
- **Pièges de performance** : requêtes N+1 (chargement différé), suivi de modifications contre `AsNoTracking`, sur-chargement de données.
- *Note VB* : `DbContext` et entités sont de simples classes VB ; l'intégration est propre.

> 🔗 Cet axe se prête bien à la stratégie hybride et à la bibliothèque partagée du [§11.5](05-coexistence.md) : la couche d'accès aux données modernisée peut vivre dans une bibliothèque consommée par l'ancien et le nouveau code.

---

## 5. Injection de dépendances : découpler pour évoluer

**Avant** : des `New` partout (couplage fort), singletons statiques, *service locators*, dépendances codées en dur, raccourcis de l'espace `My` (qui couplent au système).  
**Après** : injection par constructeur, `Microsoft.Extensions.DependencyInjection`, le Generic Host (→ module 4, [§4.8](../04-async/08-background-services.md)) ; services enregistrés avec la **bonne durée de vie** (transitoire / *scoped* / singleton).

**Le bénéfice** : découplage, configurabilité, et surtout **testabilité** — le pont vers l'axe suivant.

**Précautions de modernisation :**

- **Changement architectural** : la DI s'introduit au **point de composition** (la racine de l'application), puis se propage vers l'intérieur.
- **Ne pas en abuser**, et surtout **ne pas recréer un *service locator*** (un anti-pattern qui réintroduit le couplage qu'on cherchait à éliminer).
- **Les durées de vie comptent** : un mauvais choix provoque des bugs subtils (dépendances « captives »).
- *WinForms/WPF* : l'application a besoin d'un point de composition câblé via l'hôte. Les raccourcis `My` (`My.Settings`, `My.Computer`) sont commodes mais **nuisent à la testabilité** (→ module 2, [§2.12](../02-fondamentaux-langage/12-espace-my.md) ⚠️) ; la DI est l'alternative testable.

---

## 6. Testabilité : le but et le filet à la fois

**Avant** : du code non testable — dépendances statiques (`DateTime.Now`), accès aux fichiers/à la base en ligne, logique métier mêlée à l'interface, aucune **couture** (*seam*) où insérer un test.

La modernisation se heurte ici à un cercle vicieux classique : **il faut des tests pour refactoriser sans risque, mais le code hérité n'est pas testable.** La sortie se fait en trois temps :

1. **Tests de caractérisation aux frontières d'abord** (→ [§11.7](07-gestion-risques.md)) : verrouiller le comportement actuel **même sans coutures internes**.
2. **Introduire des coutures** (extraire des interfaces, injecter les dépendances — cf. §5) pour permettre des tests unitaires plus fins.
3. **Séparer la logique métier de l'interface et des E/S**, afin de la tester isolément.

**Outillage** : xUnit / NUnit / MSTest, *mocking* (Moq, NSubstitute) (→ module [13](../13-tests-qualite/README.md), notamment [§13.2](../13-tests-qualite/02-mocking-tdd.md)).

**Précautions :**

- **Ne pas viser 100 % de couverture rétroactive** : tester là où le produit risque × valeur est le plus élevé.
- La testabilité est largement une **conséquence** de la DI et de la séparation des responsabilités, **pas une tâche isolée**.

---

## 7. Une démarche, pas un grand projet

La modernisation ne réussit presque jamais sous la forme d'un « grand projet de réécriture ». Elle se **tisse dans le travail courant** (règle du *boy-scout*), priorisée par la cartographie risque/valeur du [§11.1](01-evaluer-strategies.md). Une séquence qui fonctionne bien :

1. **filet de tests** (§11.7) →
2. **introduire la DI et les coutures** (débloque la testabilité) →
3. **améliorations idiomatiques** (async, LINQ, EF Core) au fur et à mesure qu'on touche chaque zone.

Au terme de la transition, la bibliothèque partagée `.NET Standard` du [§11.5](05-coexistence.md) peut elle-même être re-ciblée vers `net10.0`, une fois l'ancien côté retiré.

> 🤖 L'IA accélère utilement ces transformations (proposer une conversion en asynchrone, réécrire une boucle en LINQ, générer tests, *mocks* et documentation) — mais **chaque changement est validé** (→ module [17](../17-developpement-ia/README.md)).

---

## 🔑 Points clés à retenir

- **Migrer ≠ moderniser** : la migration met le code sur la bonne plateforme ; la modernisation le rend idiomatique. C'est un effort **séparé, incrémental et priorisé** — jamais tout ou rien.
- Toujours moderniser **derrière un filet de tests** ([§11.7](07-gestion-risques.md)) et **un sujet à la fois** ([§11.1](01-evaluer-strategies.md)).
- **Async** : « async jusqu'en haut », ne **jamais** bloquer (`.Result` / `.Wait()`) — décisif pour la réactivité WinForms/WPF.
- **LINQ** : déclaratif et idiomatique en VB, mais attention à l'**exécution différée** et aux chemins chauds.
- **EF Core** : EF6 → EF Core est une **réécriture**, pas une mise à niveau ; l'introduire **progressivement**, à côté de l'ADO.NET existant.
- **Injection de dépendances** : découple et **débloque la testabilité** ; introduite au point de composition, sans recréer un *service locator*.
- **Testabilité** : à la fois le but et le filet ; largement une **conséquence** de la DI et de la séparation des responsabilités.

---

⬅️ [11.5 — Coexistence .NET Framework / .NET moderne](05-coexistence.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [11.7 — Gestion des risques (sauvegarde, tests de non-régression, *rollback*)](07-gestion-risques.md)

⏭️ [Gestion des risques (sauvegarde, tests de non-régression, *rollback*)](/11-migration-legacy/07-gestion-risques.md)
