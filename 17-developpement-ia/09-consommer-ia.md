🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.9 Intégrer l'IA dans vos applications VB.NET (consommation d'API) ✅

Voici la bascule du module. Les huit sections précédentes traitaient de *développer **avec** l'IA* — utiliser l'assistant pour écrire du VB, en luttant contre le biais C#. Celle-ci traite de l'inverse : *intégrer l'IA **dans** votre application* — écrire du VB.NET qui **appelle** un modèle. Et c'est délibérément la section ✅, la plus **confortable** de tout le module.

> **Le cadrage à ne pas perdre** (rappel du [README](README.md) et de [17.1](01-pourquoi-ia-vbnet.md)). Ici, on n'écrit pas du VB *avec* l'IA ; on écrit du VB *qui appelle* l'IA. Les barrières des sections précédentes ne s'appliquent pas : intégrer un modèle, c'est **consommer des bibliothèques .NET**.

---

## Pourquoi c'est pleinement dans le périmètre de VB.NET

C'est ici que la thèse du module devient concrète. `Microsoft.Extensions.AI`, les SDK Azure OpenAI et OpenAI sont des **paquets NuGet ordinaires**. Les consommer mobilise exactement la zone de confort de VB.NET : l'asynchronie ([module 4](../04-async/README.md)), l'injection de dépendances et le Generic Host ([4.8](../04-async/08-background-services.md)), `HttpClient`, les bibliothèques tierces.

Et surtout, **aucune** des limites du module ne s'applique : pas de `record` à *déclarer*, pas de code *Span-first* à *produire*, pas de *source generator* à *écrire*. De la pure consommation — précisément ce que Microsoft désigne comme le rôle de VB.NET (*consumption-only*, [1.6](../01-introduction-vbnet/06-positionnement-2026.md)). Cette section est donc la **preuve concrète** que le statut « stabilisé / consommation seule » couvre malgré tout un domaine vaste et résolument moderne.

---

## La pile : `Microsoft.Extensions.AI` + les SDK

**`Microsoft.Extensions.AI`** est l'**abstraction recommandée** — au modèle d'IA ce que `Microsoft.Extensions.Logging` est à la journalisation. Elle expose des types **indépendants du fournisseur** (`IChatClient`, `IEmbeddingGenerator`…), s'intègre à l'injection de dépendances, et offre un pipeline de *middleware* (télémétrie, mise en cache, invocation de fonctions). L'écrire contre cette abstraction permet de **changer de fournisseur** sans réécrire son code.

**Les SDK concrets** — `Azure.AI.OpenAI` (Azure) et `OpenAI` (OpenAI) — s'utilisent soit directement, soit **derrière** l'abstraction (préférable, pour éviter le verrouillage).

**Authentification et secrets** : clés d'API via la configuration, les *user secrets* ou **Key Vault** (jamais en dur dans le code — [16.2](../16-securite/02-cryptographie.md), [15.5](../15-deploiement-devops/05-cloud-essentiels.md)) ; **Microsoft Entra ID** pour Azure ([16.1](../16-securite/01-auth.md)).

---

## Les quatre briques

### Chat — la conversation

Le cœur : un `IChatClient`, une liste de `ChatMessage` typés par rôle (`System` / `User` / `Assistant`), et un appel asynchrone (réponse complète ou en *streaming*). L'asynchronie se consomme proprement via `Async`/`Await` ([module 4](../04-async/README.md)). À quel point c'est ordinaire en VB :

```vb
Imports Microsoft.Extensions.AI

' Injection de IChatClient (abstraction Microsoft.Extensions.AI), puis appel asynchrone.
' (Les API exactes évoluent : se référer à la documentation pour les signatures à jour.)
Public Class AssistantClient
    Private ReadOnly _chat As IChatClient

    Public Sub New(chat As IChatClient)
        _chat = chat
    End Sub

    Public Async Function PoserAsync(question As String) As Task(Of String)
        Dim messages As New List(Of ChatMessage) From {
            New ChatMessage(ChatRole.System, "Tu réponds en français, de façon concise."),
            New ChatMessage(ChatRole.User, question)
        }
        Dim reponse = Await _chat.GetResponseAsync(messages)
        Return reponse.Text
    End Function
End Class
```

Rien d'exotique : une dépendance injectée, une fonction `Async`, une liste de messages, un `Await`. Du VB.NET comme un autre.

### Embeddings — la vectorisation

Un `IEmbeddingGenerator` transforme du texte en **vecteurs**, socle de la recherche sémantique : deux textes proches par le sens donnent des vecteurs proches. C'est la brique de base du RAG.

### Function calling — l'appel d'outils

Le modèle peut **invoquer des fonctions que vous exposez** : vous décrivez vos fonctions VB, le modèle décide quand les appeler, et `Microsoft.Extensions.AI` **automatise l'invocation** puis réinjecte le résultat dans la conversation. Les fonctions appelées sont du VB ordinaire — encore de la consommation.

### RAG basique — la récupération augmentée

Le *Retrieval-Augmented Generation* enchaîne trois briques : **vectoriser** vos documents (embeddings), **stocker** les vecteurs, puis pour chaque question **récupérer** les passages pertinents par similarité et les **injecter en contexte** dans le *prompt* de chat. Tous les composants — générateur d'embeddings, magasin de vecteurs, client de chat — se consomment depuis VB. On démarre en mémoire ; pour l'échelle, on passe à une vraie base vectorielle — souvent l'un des magasins rencontrés en [7.4](../07-acces-donnees/04-autres-stockages.md) (Azure Cosmos DB, Redis…), qui proposent aujourd'hui l'indexation vectorielle.

---

## Là où les deux usages se rencontrent

Voici la synthèse qui referme le module. L'**intégration** d'un modèle relève de la consommation confortable — mais lorsque vous **écrivez ce code d'intégration avec un assistant** (ce qui arrivera), vous retombez dans le biais C# de [17.2](02-prompting-vbnet.md) : la documentation et les exemples de ces SDK sont en C#, et l'assistant vous tendra du **code d'intégration en C#** à transposer.

Autrement dit : l'intégration elle-même est dans la zone de confort de VB.NET ; l'**écrire avec l'aide de l'IA** mobilise toujours les règles de [17.2](02-prompting-vbnet.md) (préciser « VB.NET », convertir, valider). La boucle est bouclée — les deux usages du module se retrouvent dans une même tâche.

---

## Les préoccupations de production

L'intégration est simple, mais il s'agit d'un **appel réseau vers un service payant et non déterministe**. Les préoccupations habituelles s'appliquent :

- **Résilience** : *timeouts*, nouvelles tentatives (Polly — [8.1](../08-services-web/01-consommer-api-rest.md)), annulation (`CancellationToken` — [4.4](../04-async/04-annulation-timeout.md)), gestion des limites de débit.
- **Coût et latence** : chaque appel coûte et prend du temps ; la mise en cache (via le pipeline de `Microsoft.Extensions.AI`) aide à concevoir en conséquence.
- **Sécurité** : secrets dans Key Vault / la configuration ([16.2](../16-securite/02-cryptographie.md)) ; la sortie du modèle est une donnée non fiable — validez-la et encodez-la avant tout affichage.
- **Non-déterminisme** : on ne peut pas asserter une sortie exacte dans un test. On teste la **plomberie** de l'intégration, et l'on apprécie la **qualité** par des approches d'évaluation, pas par des assertions d'égalité.

---

## En résumé — et fin du module

Cette section est le **dénouement** du cadrage honnête du module. Tandis que *développer avec l'IA* affronte en permanence le biais C#, *intégrer l'IA dans une application* est de la **consommation VB.NET pure et confortable** : `Microsoft.Extensions.AI`, les SDK OpenAI, le chat, les embeddings, le *function calling* et le RAG ne sont que des bibliothèques .NET ordinaires. Le seul endroit où le biais resurgit, c'est lorsqu'on **écrit** cette intégration avec un assistant.

C'est une conclusion à l'image du langage : VB.NET, « stabilisé » et tourné vers la consommation, intègre l'IA moderne aussi naturellement que le reste — pourvu qu'on garde, là comme ailleurs, la main sur ce que l'assistant propose.

Le module 17 s'achève ici. La réflexion de fond sur le positionnement de VB.NET, les cas où rester et ceux où migrer, se poursuit au [module 18 — Stratégie, feuille de route et ressources](../18-strategie-roadmap/README.md).

⏭️ [Stratégie, feuille de route et ressources](/18-strategie-roadmap/README.md)
