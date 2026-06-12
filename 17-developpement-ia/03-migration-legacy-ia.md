🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.3 Migrer du legacy avec l'IA (VB6, .NET Framework)

La maintenance et la migration de code existant comptent parmi les emplois les plus réels de VB.NET en 2026 — et c'est un domaine où l'IA apporte un levier considérable, à condition d'en respecter scrupuleusement les limites. Cette section traite de **l'angle IA** de la migration : ce que l'assistant fait bien, où il devient dangereux, et comment le piloter.

> **Périmètre.** La méthodologie de migration, les outils, les *breaking changes* précis et la gestion des risques appartiennent au [module 11](../11-migration-legacy/README.md) (avec l'[Annexe E — Guide de migration vers .NET 10](../annexes/migration-net10/README.md)). Les bases du *prompting* et la procédure de conversion sont en [17.2](02-prompting-vbnet.md). On ne les reprend pas ici ; on se concentre sur la conduite de la migration **avec** un assistant.

---

## Pourquoi l'IA change la donne pour la migration

La migration est par nature **répétitive, volumineuse et reconnaissable** : traduire ligne à ligne, retrouver l'équivalent moderne d'une API retirée, réécrire une configuration, moderniser un *pattern*. Autant de tâches lentes et fastidieuses que l'assistant accélère réellement. Pour la maintenance de legacy — l'un des scénarios cœur de VB.NET —, c'est un gain de productivité tangible, qui abaisse le coût d'un travail jusqu'ici ingrat.

Mais c'est aussi, paradoxalement, **le terrain où l'IA est la plus risquée**. Il faut en être conscient avant d'écrire le premier *prompt*.

---

## Le double biais : pourquoi le legacy est piégeux

Au biais C# déjà décrit en [17.1](01-pourquoi-ia-vbnet.md) — qui affaiblit la production de VB.NET — s'ajoute, en migration, un **second handicap** : la **source** est encore plus rare dans les données d'entraînement. Le VB6 est ancien et très peu représenté ; les vieilles APIs du .NET Framework s'effacent du corpus à mesure que .NET moderne progresse. Le modèle se retrouve doublement hors de sa zone de confort : il connaît mal ce qu'il lit, et tend à mal écrire ce qu'il produit.

Le danger cardinal porte un nom : la **dérive sémantique**. Un code legacy transporte des années de règles métier non documentées, de traitements de cas limites et de comportements de *runtime* subtils. L'IA traduit la *syntaxe*, pas l'*intention* : elle ne connaît ni le comportement réel de votre base, ni ses données, ni ses hypothèses implicites. Le réflexe *« ça compile »* — déjà trompeur ailleurs — devient ici franchement dangereux : du code migré peut compiler parfaitement tout en se comportant autrement que l'original.

La conséquence est non négociable et conditionne tout le reste : **le filet de tests vient en premier.** Aucune migration assistée par IA sans **tests de non-régression** établis autour du comportement actuel (cf. [11.7](../11-migration-legacy/07-gestion-risques.md), et la génération de tests par IA en [17.4](04-generer-tests-doc.md)). Ce sont ces tests qui vous autorisent à faire confiance — sous contrôle — au code produit par le modèle.

---

## Les deux chantiers et leurs dynamiques propres

### VB6 → VB.NET : une traduction qui cache des pièges sémantiques

L'assistant est correct pour traduire la **syntaxe de surface**, mais le VB6 comporte des différences de *runtime* et de sémantique qui ne se transposent pas mécaniquement. Les points à surveiller systématiquement :

- **Gestion d'erreurs** : `On Error GoTo` et surtout `On Error Resume Next` → `Try/Catch` structuré. Le `Resume Next`, qui masque silencieusement les erreurs, est particulièrement insidieux : une traduction naïve peut changer tout le flot de contrôle.
- **Propriétés par défaut sans argument**, `Set` / `Let` → accès explicite (VB.NET ne conserve la propriété par défaut que pour les indexeurs, cf. [11.2](../11-migration-legacy/02-vb6-vers-vbnet.md)).
- **`Variant`** → `Object`, avec les implications de typage que cela entraîne.
- **Chaînes de longueur fixe**, **tableaux 1-based** (`Option Base 1`), **tableaux de contrôles** (sans équivalent direct en VB.NET, à recâbler en collection) — autant de constructions sans correspondance immédiate.
- **Dépendances COM** et liaison tardive — et un piège majeur : la bibliothèque **`Microsoft.VisualBasic.Compatibility`** (béquille des anciens portages VB6) **n'existe pas en .NET moderne**. Un code qui s'y appuie ne migre pas tel quel vers .NET 10. (À ne pas confondre avec l'espace `Microsoft.VisualBasic` de base, lui bien disponible.)

À cela s'ajoute la **confusion VB6 / VB.NET** (voir 17.1), aiguë ici : sans ancrage explicite, le modèle peut « traduire » du VB6 en code resté VB6 dans l'esprit, ou halluciner. Précisez toujours : *« code source VB6 à migrer vers VB.NET moderne (.NET 10) — ce n'est pas du VB.NET »*.

L'IA est en revanche excellente comme **traducteur de premier jet** et surtout comme **explicateur** des constructions VB6 obscures (« que fait réellement ce bloc `On Error Resume Next` ? »). Comprendre précède toujours traduire. L'outillage de conversion proprement dit relève de [11.2](../11-migration-legacy/02-vb6-vers-vbnet.md).

### .NET Framework → .NET 10 : même langage, autres fondations

Ici, pas de barrière de langage : c'est toujours du VB.NET. L'assistant ne se bat plus avec la syntaxe, mais doit connaître les **APIs retirées ou modifiées** de .NET 10 et le passage au **format de projet SDK-style**.

Là où l'IA aide beaucoup :

- **Repérer les APIs obsolètes ou supprimées** et proposer le remplacement moderne — par exemple `BinaryFormatter` (retiré, et sujet de sécurité majeur), `System.Configuration.ConfigurationManager` → le modèle `Microsoft.Extensions.Configuration`, l'absence de WCF côté serveur, etc.
- **Réécrire `app.config` / `web.config`** vers `appsettings.json` et le *pattern* Options.
- **Moderniser le fichier projet** (ancien `.vbproj` verbeux → SDK-style concis) et la cible de framework.
- **Introduire les pratiques modernes** : injection de dépendances et Generic Host, async, EF Core (cf. [11.6](../11-migration-legacy/06-moderniser.md)).
- **Trier une liste de dépendances** : quels paquets NuGet ont une version compatible .NET moderne.

Là où l'IA est dangereuse :

- **Halluciner un remplacement** d'API qui n'existe pas ou dont la sémantique diffère.
- **Se tromper sur les *breaking changes* de .NET 10** : ses connaissances peuvent être périmées. La liste qui fait foi est dans l'[Annexe E](../annexes/migration-net10/README.md) et en [11.3](../11-migration-legacy/03-framework-vers-net10.md), pas dans la mémoire du modèle.
- **Sur-moderniser** : réécrire du code qui fonctionne sans nécessité, et introduire du risque gratuit.

La bonne posture : utiliser l'IA pour **trier et dégrossir**, puis valider chaque remplacement contre la documentation officielle, le compilateur et les analyseurs Roslyn.

### L'outil dédié : l'agent `@modernize-dotnet` (GitHub Copilot app modernization)

Pour ce chantier Framework → .NET 10, l'écosystème Copilot fournit un agent **spécialisé**, déjà présenté côté méthodologie en [11.3](../11-migration-legacy/03-framework-vers-net10.md) : **GitHub Copilot app modernization**, successeur du .NET Upgrade Assistant (déprécié fin 2025). Invoqué via `@modernize-dotnet` dans le chat Copilot (inclus dans Visual Studio 2026 ; abonnement Copilot requis), il structure la migration en **trois temps** — *assessment* (un rapport `assessment.md` sur les projets, dépendances et schémas de code), **planification**, puis **exécution** — en validant chaque changement pour permettre le retour arrière.

Deux réserves, dans la droite ligne de ce module : il prend en charge VB.NET **pour certains types de projets seulement**, et son résultat relève des mêmes règles que tout produit d'IA — des **hallucinations** ont été rapportées (jusqu'à des paquets NuGet inexistants proposés en remplacement), donc relecture critique systématique, compilation et tests. L'agent industrialise les 80 % mécaniques ; il ne déplace pas la responsabilité. Le guide complet et la checklist figurent en [Annexe E](../annexes/migration-net10/README.md).

### Le cas Web Forms — ce que l'IA ne peut pas faire

Un point d'honnêteté, en cohérence avec [11.4](../11-migration-legacy/04-web-forms-legacy.md) : **ASP.NET Web Forms n'a aucun chemin de migration vers .NET moderne**. Aucun assistant ne peut en inventer un. L'IA peut vous aider à **maintenir** du Web Forms sur .NET Framework, ou à **réécrire** vers une autre architecture (MVC/Blazor en C#, ou Web API en VB.NET avec un *front* séparé), mais elle ne peut pas « migrer » Web Forms vers .NET 10, faute de cible. Méfiez-vous : sollicité, le modèle peut proposer avec aplomb un chemin qui n'existe pas. Ne le laissez pas faire — la cible n'existe tout simplement pas.

---

## Comment piloter l'IA pour une migration

La démarche, étape par étape (la stratégie *incrémentale vs big-bang* et l'évaluation de l'existant relèvent de [11.1](../11-migration-legacy/01-evaluer-strategies.md)) :

1. **Cartographier d'abord.** Inventaire, dépendances, zones à risque. L'IA aide à *résumer et expliquer* un gros fichier legacy, mais la décision de stratégie reste la vôtre.
2. **Poser le filet de tests.** Construire des tests de caractérisation/non-régression autour du comportement actuel (l'IA aide à les générer — [17.4](04-generer-tests-doc.md)). C'est le préalable absolu, pas une étape parmi d'autres.
3. **Migrer par petits lots.** Traduire unité par unité, compiler, rejouer les tests de non-régression, renvoyer les erreurs au modèle (procédure de [17.2](02-prompting-vbnet.md)). Les petits lots restent traçables et réversibles.
4. **Faire expliquer le legacy obscur** avant de le traduire : décoder une construction VB6 ou une vieille API du Framework, en comprendre l'intention, *puis* la convertir.
5. **Valider chaque remplacement d'API** contre la documentation officielle, le compilateur et les analyseurs. Ne jamais accepter un remplacement sur la seule parole du modèle.
6. **Surveiller la sémantique, pas seulement la compilation.** Les pièges propres à la migration — masquage d'erreurs, `Variant`/`null`, opérateurs, division entière, culture et formatage — passent sous le radar du compilateur. Seuls les tests les rattrapent.
7. **Documenter au fil de l'eau** : l'IA rédige volontiers des notes de migration et de la documentation XML ([17.4](04-generer-tests-doc.md)).

---

## Gabarits de prompts pour la migration

Ces gabarits prolongent ceux de [17.2](02-prompting-vbnet.md) pour le cas spécifique du legacy. Remplacez les `{…}` ; ils se copient tels quels.

**1. Expliquer un bloc legacy avant de le traduire** :

```text
Explique précisément ce que fait le code {VB6 | VB.NET sur .NET Framework} suivant : effets de bord,
gestion d'erreurs et comportements implicites (propriétés par défaut, Variant, On Error Resume Next,
base des tableaux, etc.). Ne le traduis pas encore — je veux d'abord en comprendre l'intention exacte.

Code :
{colle le bloc legacy}
```

**2. Traduire VB6 → VB.NET avec vigilance sémantique** :

```text
Migre ce code VB6 vers Visual Basic .NET moderne (.NET 10, Option Strict On). Ce N'EST PAS du VB.NET.
- Convertis On Error GoTo / Resume Next en Try/Catch en préservant le comportement (attention au
  masquage d'erreurs de Resume Next).
- Remplace Variant par le type approprié ; les propriétés par défaut et Set/Let par un accès explicite ;
  les chaînes de longueur fixe et les tableaux 1-based par leurs équivalents .NET.
- N'utilise PAS Microsoft.VisualBasic.Compatibility (absent de .NET moderne).
- Signale chaque point où la sémantique VB6 n'a pas d'équivalent direct et exige une décision.

Code VB6 :
{colle le code}
```

**3. Framework → .NET 10 : repérer et remplacer les APIs** :

```text
Ce code VB.NET cible .NET Framework. Identifie les APIs retirées, obsolètes ou déconseillées sous
.NET 10 et propose pour chacune le remplacement moderne, avec l'éventuelle différence de comportement.
Ne réécris que ce qui doit l'être. N'invente aucun remplacement : si tu n'es pas certain qu'une API
existe en .NET 10, indique-le pour que je vérifie dans la documentation.

Code :
{colle le code}
```

**4. `app.config` → `appsettings.json` + Options** :

```text
Convertis cette configuration app.config (.NET Framework) en appsettings.json pour .NET 10, et propose
la classe d'options VB.NET correspondante, liée via Microsoft.Extensions.Configuration / le pattern
Options. Conserve les mêmes clés et valeurs.

app.config :
{colle le contenu}
```

---

## Les limites à garder en tête

L'IA accélère réellement les **80 % mécaniques** de la migration — traduction de surface, recherche de remplacements, réécriture de configuration. Mais les **20 % à risque** — sémantique métier, cas limites, équivalence de comportement — restent un travail humain, et c'est là que se joue la réussite ou l'échec du projet.

Le modèle peut se tromper avec assurance sur trois fronts à surveiller en priorité : les **chemins de migration inexistants** (Web Forms en tête), les **remplacements d'API hallucinés**, et une **connaissance périmée des *breaking changes*** de .NET 10. Les arbitres restent toujours les mêmes : les **tests** pour la sémantique, la **documentation officielle** (Annexe E, 11.3) pour les ruptures, le **compilateur et les analyseurs** pour la validité des APIs.

La méthodologie complète et la gestion des risques sont dans le [module 11](../11-migration-legacy/README.md) ; le catalogue détaillé des pièges de l'IA, en [17.7](07-limites-pieges.md) ; et une migration de bout en bout (VB6 → VB.NET) est déroulée en [17.8](08-cas-concrets.md).

⏭️ [Générer des tests, mocks et documentation XML](/17-developpement-ia/04-generer-tests-doc.md)
