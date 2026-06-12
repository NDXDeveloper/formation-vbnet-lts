🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17. Développer en VB.NET avec l'IA (l'ère Copilot) 🤖 🆕 ⭐

> *« Pour le développeur VB.NET de 2026, l'assistant IA n'est pas un gadget : c'est la pièce qui compense le fait que tout l'écosystème — documentation, exemples, réponses en ligne — pense d'abord en C#. »*

En 2026, programmer sans assistant IA est devenu l'exception. GitHub Copilot, les agents intégrés à Visual Studio 2026 (présenté par Microsoft comme un IDE *« AI-native »*) et les modèles de conversation accessibles directement depuis l'éditeur font désormais partie du poste de travail standard du développeur .NET. La complétion ligne à ligne, la génération de méthodes entières, l'explication d'un message d'erreur ou la rédaction de tests sont devenues des gestes quotidiens.

Pour la plupart des langages, ce module se résumerait à quelques bonnes pratiques de *prompting*. **Pour VB.NET, le sujet est structurant** — au point que cette formation en fait un thème transversal (repérable au fil des modules par l'icône 🤖) et lui consacre ici un chapitre entier. La raison tient en une phrase : les grands modèles de langage ont été entraînés très majoritairement sur du **C#**, pas sur du Visual Basic.

---

## Le biais C# : le fait central de ce module

Le code C# public est infiniment plus abondant que le code VB.NET : dépôts open source, questions-réponses sur les forums, articles de blog, et surtout la documentation officielle, dont les exemples sont aujourd'hui presque toujours rédigés en C#. Or un modèle apprend ce qu'on lui montre : confronté à une demande .NET, sa pente naturelle est de produire du C#.

Concrètement, le développeur VB.NET rencontre trois symptômes récurrents :

- **La dérive silencieuse vers C#.** Demandez « écris-moi une classe qui… » sans préciser le langage, et vous obtiendrez souvent du C#, accolades et points-virgules compris.
- **Les hallucinations de syntaxe.** Même lorsqu'il produit bien du VB, le modèle peut y glisser des tournures qui n'existent pas dans le langage : un mot-clé `record` *déclaré* en VB, des *top-level statements*, une *switch expression*, un accesseur `init`… autant de constructions C# que VB.NET (figé à la version **16.9**, *consumption-only*) ne possède pas.
- **La confusion VB6 / VB.NET.** « Visual Basic » désigne, pour le modèle, aussi bien le VB6 historique que le VB.NET moderne. Sans levée d'ambiguïté explicite, les suggestions mélangent volontiers les deux époques.

Le paradoxe mérite d'être souligné : un langage perçu comme « de niche » est précisément celui pour lequel l'assistance IA demande **le plus de méthode**. Maîtriser l'IA n'est donc pas optionnel pour le vébéiste — c'est une compétence de premier plan, qui transforme un outil par défaut hostile en allié productif.

---

## Une arme à double tranchant

Bien utilisée, l'IA est un formidable accélérateur sur le terrain de jeu réel de VB.NET. Le code de bureau (Windows Forms ⭐, WPF), l'accès aux données, le *boilerplate* d'événements et de liaison, la migration de code legacy : autant de tâches répétitives et reconnaissables où un assistant fait gagner un temps considérable.

Mal utilisée — c'est-à-dire sans validation —, elle introduit des bugs subtils, du code qui ne compile pas, ou pire, du code qui compile mais trahit l'intention initiale. La ligne de conduite de ce module est donc constante : **l'IA propose, le développeur valide.** Le compilateur (avec `Option Strict On`), les tests, et l'aide-mémoire de correspondance VB.NET ↔ C# (Annexe A) sont vos garde-fous. Aucune suggestion n'entre dans le code sans avoir été relue avec un œil VB.

---

## Deux usages à ne pas confondre

Ce module traite en réalité de **deux choses distinctes**, qu'il est essentiel de garder séparées :

1. **Développer *avec* l'IA** (sections 17.1 à 17.8) — utiliser l'assistant comme copilote pour écrire, migrer, tester, déboguer et documenter du VB.NET. C'est le cœur du chapitre, et c'est là que se gère le biais C#. La question est ici une **méthode de travail**.
2. **Intégrer l'IA *dans* vos applications** (section 17.9) — appeler un modèle (Azure OpenAI, OpenAI) depuis votre propre code pour y ajouter du chat, des *embeddings*, du *function calling* ou un RAG basique. Ici, aucune barrière : ce ne sont que des bibliothèques .NET à **consommer**, exactement le scénario où VB.NET est pleinement à l'aise ✅. La question est cette fois d'**architecture applicative**.

Confondre les deux est une source classique de malentendus : on ne résout pas un problème de *prompting* avec un SDK, ni un problème d'intégration avec une meilleure formulation.

---

## Ce que vous saurez faire à l'issue du module

- Comprendre pourquoi l'IA est décisive pour VB.NET et anticiper le biais C# des modèles.
- Rédiger des *prompts* qui produisent du VB.NET correct dès le premier essai, et convertir efficacement le C# généré.
- Accélérer la migration de code legacy (VB6, .NET Framework) avec l'assistant, sans lui accorder une confiance aveugle.
- Générer tests, *mocks* et documentation XML, puis les valider.
- Déboguer et optimiser à l'aide de l'IA (lecture de messages d'erreur, analyse de *stack traces*).
- Mettre en place un flux de travail *IA-first* dans Visual Studio 2026 et VS Code.
- Reconnaître les pièges et hallucinations, et appliquer une validation systématique.
- Intégrer un service d'IA dans une application VB.NET par simple consommation d'API.

---

## Parcours du module

- **17.1** — [Coder en 2026 avec l'IA : pourquoi c'est crucial pour VB.NET (le biais C# des modèles)](01-pourquoi-ia-vbnet.md)  
  Le constat de départ et ses conséquences pratiques.
- **17.2** — [Prompting efficace pour obtenir du code VB.NET](02-prompting-vbnet.md)  
  Préciser le langage et la cible .NET, lever l'ambiguïté VB6, convertir le C#, modèles de *prompts* réutilisables.
- **17.3** — [Migrer du legacy avec l'IA (VB6, .NET Framework)](03-migration-legacy-ia.md)  
  L'assistant comme accélérateur de migration, en complément du module 11.
- **17.4** — [Générer des tests, mocks et documentation XML](04-generer-tests-doc.md)  
  Produire et fiabiliser la couverture de test et la documentation.
- **17.5** — [Déboguer et optimiser avec l'IA](05-debugger-optimiser.md)  
  Expliquer des erreurs, analyser des *stack traces*, suggérer des optimisations.
- **17.6** — [Workflow IA-first : Copilot dans Visual Studio 2026, VS Code, agents](06-workflow-ia-first.md)  
  Intégrer l'IA dans le quotidien de développement.
- **17.7** — [Limites et pièges (hallucinations de syntaxe C# en VB, validation systématique)](07-limites-pieges.md)  
  Savoir où l'IA dérape, et comment s'en prémunir.
- **17.8** — [Cas concrets (migration VB6→VB.NET, API REST, WPF MVVM)](08-cas-concrets.md)  
  Mise en application de bout en bout sur des scénarios réalistes.
- **17.9** — [Intégrer l'IA dans vos applications VB.NET (consommation d'API)](09-consommer-ia.md) ✅  
  `Microsoft.Extensions.AI`, SDK Azure OpenAI / OpenAI ; chat, *embeddings*, *function calling*, RAG basique.

---

## Place dans la formation

Ce module rend explicite un fil rouge déjà annoncé en ouverture, dans [« L'ère de l'IA : pourquoi c'est décisif pour VB.NET »](../01-introduction-vbnet/06.3-ere-ia.md). Il s'articule étroitement avec plusieurs chapitres :

- La [migration et la maintenance du legacy](../11-migration-legacy/README.md) (module 11), qui s'appuie sur l'IA pour le travail de conversion.
- Les [tests et la qualité du code](../13-tests-qualite/README.md) (module 13), dont la génération de tests assistée prolonge la section 17.4.
- La [consommation d'API REST](../08-services-web/01-consommer-api-rest.md) (section 8.1), socle technique de l'intégration d'IA traitée en 17.9.

Il s'appuie aussi sur trois annexes que vous garderez ouvertes en travaillant : la [correspondance syntaxique VB.NET ↔ C#](../annexes/correspondance-vbnet-csharp/README.md) (Annexe A, indispensable pour transposer le C# omniprésent), les [bonnes pratiques de codage](../annexes/bonnes-pratiques/README.md) (Annexe C, qui inclut les bons réflexes avec les assistants), et le [guide Visual Studio 2026](../annexes/visual-studio-2026/README.md) (Annexe D, pour les extensions IA). En cas de blocage, la [FAQ et le dépannage](../annexes/faq-depannage/README.md) (Annexe G) recensent les pièges fréquents propres au couple IA / VB.NET.

---

## À garder en tête

L'IA ne change rien à la **position stratégique** de VB.NET : le langage reste figé, *consumption-only*, et au mieux de sa forme sur ses scénarios cœur. Elle ne le rend pas « moderne » à nouveau et n'efface pas la frontière VB/C# (voir l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

Ce qu'elle change, en revanche, est considérable au quotidien : elle abaisse le coût de la maintenance de legacy, fluidifie la stratégie hybride VB.NET / C#, et rend nettement plus confortable le travail à l'intérieur d'un périmètre où la documentation et les exemples parlent une autre langue que la vôtre. Bien outillé et bien méthodique, le développeur VB.NET de 2026 n'est plus seul face au biais C# — à condition de ne jamais oublier qui, de l'humain ou du modèle, garde la main.

⏭️ [Coder en 2026 avec l'IA : pourquoi c'est crucial pour VB.NET (le biais C# des modèles)](/17-developpement-ia/01-pourquoi-ia-vbnet.md)
