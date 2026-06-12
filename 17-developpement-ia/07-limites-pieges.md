🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.7 Limites et pièges (hallucinations de syntaxe C# en VB, validation systématique)

Cette section est le **catalogue de référence** du module : elle rassemble, en une taxonomie utilisable, les pièges que les sections précédentes signalaient au fil de l'eau, puis pose la **discipline de validation** qui les rattrape. Gardez-la à portée de main.

> **La racine commune.** Tous ces pièges découlent du même fait structurel établi en [17.1](01-pourquoi-ia-vbnet.md) : les modèles sont entraînés majoritairement sur du C#, et le langage VB est figé ([16.9](../01-introduction-vbnet/06-positionnement-2026.md)). Ce n'est pas une faille à corriger en attendant un meilleur modèle, mais une contrainte permanente à encadrer. La validation n'est donc pas une précaution ponctuelle : c'est une **routine**.

---

## La taxonomie des pièges

### A. Les hallucinations de syntaxe — le C# qui fuit en VB

Le cas le plus visible. Le modèle insère des constructions C# qui n'existent pas en VB, ou les marqueurs du C# :

- **Constructions C# déclarées à tort** : `record` *déclaré*, accesseur `init`, *top-level statements*, *switch expression*, constructeur primaire, expression de collection `[…]`, *raw string literals*, espace de noms à portée de fichier, `global using`, `new` à type cible. Aucune ne se *déclare* en VB.
- **Marqueurs C# qui s'invitent** : accolades et points-virgules (évident), mais aussi attributs entre crochets (`[Fact]` au lieu de `<Fact>`), lambdas avec la flèche `=>` au lieu de `Function` / `Sub`, documentation `///` au lieu de `'''`, ou `var`.

Bonne nouvelle : ce sont les pièges les plus **faciles à attraper** — le compilateur les arrête. Le risque est davantage la rupture de flot que l'erreur de comportement (mais voir la catégorie C).

### B. Les fonctionnalités fantômes

Distinct de A : le modèle ne se contente pas d'écrire de la syntaxe C#, il **croit que VB peut faire** ce que seul le C# récent fait, et bâtit toute sa réponse sur cette parité — laquelle n'existe plus depuis le gel du langage. Cela se manifeste par des affirmations assurées et fausses (« tu peux utiliser X en VB »). À traiter comme suspect dès que l'« astuce » proposée semble trop moderne.

### C. Les pièges sémantiques — ça compile mais c'est faux

La catégorie **dangereuse**, parce que le compilateur ne les voit pas. Une traduction littérale du C# change silencieusement le comportement :

- **Opérateurs** : `^` est une **puissance** en VB (un XOR en C#) ; la division entière est `\` (le `/` renvoie toujours un flottant) ; `%` devient `Mod` ; `And` / `Or` ne court-circuitent pas (contrairement à `AndAlso` / `OrElse`) ; `&` concatène.

  ```vb
  ' Traduction littérale du C# "result = a ^ b" (XOR) :
  ' Dim result = a ^ b   ' ❌ En VB, ^ est la PUISSANCE : a élevé à la puissance b
  Dim result = a Xor b    ' ✅ XOR en VB
  ```

- **Types nullables** : valeur en VB (`Nullable(Of T)` / `T?`) vs *référence* en C# (`string?`, NRT que VB n'a pas) — le `?` n'a pas le même sens.
- **Sensibilité à la casse** : C# distingue les identifiants par la casse, VB non → collisions de noms à la traduction.
- **Conversions et liaison** : sous `Option Strict Off`, la liaison tardive masque des erreurs de type jusqu'à l'exécution.
- **`ByRef` vs `out`/`ref`**, et la **culture / le formatage** (dates, nombres) — source classique de dérive de comportement.

Ici, *« ça compile »* ne prouve rien. Seuls les **tests** rattrapent ces erreurs.

### D. La confusion VB6 / VB.NET

Le modèle mélange les deux époques de « Visual Basic » : `MsgBox`, `On Error GoTo` / `Resume Next`, `Variant`, `Set` / `Let`, tableaux de contrôles, `Option Base` peuvent surgir dans une réponse censée être en VB.NET. Aiguë en migration ([17.3](03-migration-legacy-ia.md)). Remède : ancrer explicitement « VB.NET moderne, pas VB6 ».

### E. L'assurance trompeuse — *confident wrongness*

Le piège **transversal**, et le plus pernicieux : le modèle est fluide et péremptoire même quand il se trompe. La qualité de la formulation masque l'erreur. Il frappe le plus fort là où vous êtes déjà dans l'incertitude — débogage ([17.5](05-debugger-optimiser.md)), legacy obscur, API de niche. Une réponse assurée n'est jamais une preuve.

### F. Les hallucinations de bibliothèque et d'API

Le modèle invente des méthodes, des surcharges ou des paquets NuGet qui n'existent pas ; propose des remplacements d'API erronés en migration ; ou s'appuie sur une **connaissance périmée de .NET 10** (ruptures, ce que le runtime fait désormais). Cas extrême : un **chemin de migration inexistant** (Web Forms vers .NET moderne — voir [17.3](03-migration-legacy-ia.md)). L'arbitre est la **documentation officielle**, pas la mémoire du modèle (liste des *breaking changes* : [Annexe E](../annexes/migration-net10/README.md) et [11.3](../11-migration-legacy/03-framework-vers-net10.md)).

### G. Les pièges propres aux tâches (rappel)

Consolidés depuis les sections dédiées :

- **Tests** : tester l'implémentation au lieu de l'intention (problème de l'oracle), couverture trompeuse, sur-*mocking* → [17.4](04-generer-tests-doc.md).
- **Débogage** : des **hypothèses, pas des observations** — l'IA n'exécute pas votre code → [17.5](05-debugger-optimiser.md).
- **Optimisation** : optimiser sans mesurer, micro-optimisation, suggérer ce que .NET 10 fait déjà ou ce qui est réservé au C# → [17.5](05-debugger-optimiser.md).
- **Agents** : l'autonomie **amplifie** tout ce qui précède, et le propage sur de nombreux fichiers → [17.6](06-workflow-ia-first.md).

---

## La validation systématique

Le principe fondateur tient en une phrase : **le modèle n'est jamais l'arbitre de sa propre justesse.** À chaque catégorie de piège correspond un arbitre que l'IA ne peut pas être. La défense est donc en **couches**, chacune attrapant ce que la précédente laisse passer.

1. **Le compilateur + `Option Strict On`.** Premier filtre, le moins cher. Il arrête les hallucinations de syntaxe (A), une partie des fonctionnalités fantômes (B) et des API inventées (F). `Option Strict On` est non négociable : il transforme la liaison tardive silencieuse en erreurs de compilation.
2. **Les analyseurs** (Roslyn, SonarQube — [13.4](../13-tests-qualite/04-analyse-statique.md) ; rappel : StyleCop est réservé à C#). Ils attrapent les API obsolètes, les anti-patterns et les défauts que le compilateur tolère.
3. **La relecture humaine ciblée.** Indispensable pour ce que les outils ne voient pas : les pièges sémantiques (C), la confusion VB6 (D), l'assurance trompeuse (E). On relit **pour l'intention**, avec un œil VB et la connaissance des biais du modèle ; l'[Annexe A](../annexes/correspondance-vbnet-csharp/README.md) est le compagnon de transposition.
4. **Les tests.** Le filet de sécurité comportemental : ils rattrapent la dérive sémantique (C) et les « optimisations » qui changent le comportement. Mais les tests eux-mêmes peuvent être compromis par l'IA (problème de l'oracle, [17.4](04-generer-tests-doc.md)) — c'est pourquoi **l'humain détient l'oracle**.
5. **La documentation officielle.** L'arbitre de l'existence d'une API et des spécificités de .NET 10 — jamais la mémoire du modèle.
6. **Le débogueur et le profileur.** Les arbitres du comportement à l'exécution et de la performance ([17.5](05-debugger-optimiser.md)), parce que l'IA n'observe rien.

La règle de correspondance à retenir : **syntaxe → compilateur ; sémantique → tests ; vérité d'API → documentation ; exécution → débogueur ; performance → profileur.** Ne laissez jamais le modèle trancher sur ce qu'il vient lui-même de produire.

---

## La checklist-réflexe avant d'accepter du code

Quelques questions à se poser systématiquement avant d'intégrer une réponse de l'IA :

- Est-ce **bien du VB.NET** — ni C#, ni VB6 ?
- Y a-t-il un **opérateur à double sens** ou un piège sémantique (`^`, `\`, `Mod`, casse, nullable, culture) ?
- Cette **API / cette fonctionnalité existe-t-elle vraiment** en VB et en .NET 10 ? (vérifier la doc plutôt que croire le modèle)
- Le code **compile-t-il sous `Option Strict On`** ?
- Un **test vérifie-t-il le comportement attendu** (et non l'implémentation) ?
- Pour un diagnostic ou une optimisation : ai-je **confirmé au débogueur / au profileur** au lieu de croire le modèle ?

---

## Le principe à garder en tête

Le niveau de vérification se règle sur l'enjeu : pour du code exploratoire, une relecture rapide suffit ; pour la **migration**, la **sécurité** ou un **chemin critique en performance**, la validation est intégrale et non négociable. Et parce que le biais est **structurel** ([17.1](01-pourquoi-ia-vbnet.md)) — non un défaut passager —, cette discipline est **permanente**. *Faire confiance, mais vérifier* : l'IA reste un proposant remarquable, à condition que ce ne soit jamais elle qui valide.

Les cas concrets de bout en bout, où ces pièges et ces validations se rencontrent en situation, sont déroulés en [17.8](08-cas-concrets.md) ; les erreurs fréquentes et leurs solutions, dans l'[Annexe G](../annexes/faq-depannage/README.md).

⏭️ [Cas concrets (migration VB6→VB.NET, API REST, WPF MVVM)](/17-developpement-ia/08-cas-concrets.md)
