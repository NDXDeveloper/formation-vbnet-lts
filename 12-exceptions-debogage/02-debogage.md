🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 12.2 — Débogage (points d'arrêt, espions, Hot Reload, débogage asynchrone) ; outils de VS 2026 🆕

> **Module 12 — Exceptions, débogage et journalisation**
> Passer de « gérer les erreurs » à « comprendre les bugs » : observer l'état réel du
> programme plutôt que de deviner.

---

## Diagnostiquer plutôt que deviner

La 12.1 montrait comment *réagir* aux erreurs. Reste à les *comprendre*. Et là, la pire des
méthodes — pourtant la plus répandue — consiste à saupoudrer le code de `MessageBox.Show` et de
`Console.WriteLine` jusqu'à ce que « ça finisse par faire quelque chose ». Le débogueur de
Visual Studio existe précisément pour éviter cette devinette : il permet de **suspendre**
l'exécution, d'**inspecter** l'état réel (variables, pile d'appels, threads) et d'**avancer pas
à pas** dans le déroulement du programme.

> 🟢 **Bonne nouvelle pour VB.NET :** le débogage est une fonctionnalité de l'**IDE**, pas du
> langage. Visual Studio 2026 utilise le même moteur de débogage pour VB.NET et pour C# :
> points d'arrêt, espions, Hot Reload, débogage asynchrone — **tout est disponible côté VB**.
> La seule exception concerne certains **agents IA** récents, abordés en fin de section.

---

## Démarrer une session de débogage

| Action | Raccourci |
|--------|-----------|
| Démarrer / continuer avec le débogueur | **F5** |
| Démarrer **sans** débogage | **Ctrl + F5** |
| Arrêter | **Maj + F5** |
| Redémarrer | **Ctrl + Maj + F5** |

Quelques réflexes :

- **Déboguez en configuration `Debug`, pas `Release`.** En `Release`, les optimisations du
  compilateur peuvent réordonner ou éliminer du code, et certaines variables deviennent
  inobservables. La compilation `Debug` génère les symboles (`.pdb`) nécessaires.
- **Attacher à un processus déjà lancé** (*Déboguer ▸ Attacher au processus…*) est précieux
  pour un service Windows, une application déjà en cours ou un processus hôte.
- **« Just My Code »** (activé par défaut) masque le code du framework et des bibliothèques
  tierces pour vous concentrer sur le vôtre — à désactiver ponctuellement quand vous suspectez
  un problème en dehors de votre code.

> 🆕 **VS 2026 :** le débogueur **démarre plus vite** qu'auparavant — un petit gain, mais sur
> une action répétée des dizaines de fois par jour, il compte.

---

## Les points d'arrêt

Le point d'arrêt (**F9** pour l'activer/désactiver sur la ligne courante) suspend l'exécution
*avant* d'exécuter la ligne marquée. C'est l'outil de base, mais il a plusieurs variantes
beaucoup moins connues — et souvent plus efficaces.

### Point d'arrêt conditionnel

Ne s'arrête que si une **condition** est vraie. Indispensable pour traquer un cas précis dans
une boucle de plusieurs milliers d'itérations.

```vb
For Each commande In commandes
    Traiter(commande)   ' ← point d'arrêt avec condition : commande.Id = 4271
Next
```

*(Clic droit sur le point d'arrêt ▸ Conditions ▸ « Expression conditionnelle ».)* La
condition s'écrit en **syntaxe VB** : `commande.Montant > 1000 AndAlso commande.EstUrgente`.

### Nombre d'accès (*hit count*)

S'arrêter uniquement à la *n*-ième fois où la ligne est atteinte (par exemple, « tous les
100 passages »). Utile pour les problèmes intermittents liés au volume.

### Filtre (*filter*)

Restreindre le point d'arrêt à un **thread**, un **processus** ou une **machine** donnés — utile
en multithread.

### Point de trace (*tracepoint*) — journaliser **sans** s'arrêter

Au lieu de suspendre l'exécution, un *tracepoint* **écrit un message** (avec interpolation de
variables) dans la fenêtre de sortie, puis continue. C'est un `Console.WriteLine` temporaire,
sans modifier ni recompiler le code.

> 🆕 **VS 2026 :** créer un *tracepoint* est désormais une affaire d'**un clic**, au lieu d'une
> option enfouie dans le menu contextuel.

### Point d'arrêt sur données (*data breakpoint*)

S'arrêter quand la **valeur d'un champ change**, quelle que soit la ligne responsable. Parfait
quand une propriété est modifiée « par on ne sait quoi » et qu'on veut prendre le coupable la
main dans le sac.

### Points d'arrêt temporaires et dépendants

- **Temporaire (à usage unique)** : se supprime après le premier déclenchement.
- **Dépendant** : ne s'active qu'*après* le déclenchement d'un autre point d'arrêt.

> 💡 La fenêtre **Points d'arrêt** (*Déboguer ▸ Fenêtres ▸ Points d'arrêt*) centralise tout :
> activer/désactiver en masse, étiqueter, grouper, exporter/importer la configuration.

---

## Inspecter l'état : espions et visualiseurs

Une fois l'exécution suspendue, plusieurs fenêtres donnent à voir l'état du programme.

| Fenêtre | Rôle |
|---------|------|
| **Variables locales** (*Locals*) | Toutes les variables du périmètre courant. |
| **Variables automatiques** (*Autos*) | Les variables utilisées autour de la ligne courante. |
| **Espion** (*Watch 1–4*) | Les expressions que **vous** choisissez de surveiller. |
| **Espion express** (*QuickWatch*) | Évaluer une expression ponctuellement (Maj + F9). |
| **Exécution** (*Immediate*) | Évaluer et **exécuter** du code à la volée (voir ci-dessous). |
| **Pile des appels** (*Call Stack*) | La chaîne des appels ayant mené ici. |

### Info-bulles de données (*DataTips*)

Survolez une variable pendant le débogage : sa valeur s'affiche, dépliable pour les objets
complexes. On peut **épingler** une info-bulle pour la garder à l'écran et la conserver d'une
session à l'autre.

### La fenêtre Exécution (*Immediate Window*) en VB.NET

Elle évalue des expressions **en syntaxe VB** et peut même *appeler des méthodes* ou modifier
des variables pendant que l'exécution est suspendue :

```vb
? client.NomComplet          ' affiche la valeur (le « ? » est optionnel)
client.Solde = 500           ' modifie une variable en direct
? CalculerRemise(client)     ' appelle une méthode et affiche son résultat
```

### Spécificateurs de format

Dans les fenêtres d'espion, des suffixes ajustent l'affichage : `,h` pour l'hexadécimal,
`,nq` pour une chaîne sans guillemets d'échappement, `,d` pour forcer le décimal… La liste
complète figure dans la documentation du débogueur.

### 🆕 Visualiseur de texte avec décodage automatique

Le **Visualiseur de texte** affiche en grand le contenu d'une chaîne. VS 2026 y ajoute un bouton
**« Détecter et formater »** : propulsé par Copilot, il identifie et **décode** automatiquement
des données complexes (Base64 compressé en GZip, par exemple) en texte lisible — sans outil
externe.

### Personnaliser l'affichage avec les attributs de débogage

VB.NET prend pleinement en charge les attributs qui pilotent l'expérience du débogueur :

```vb
<DebuggerDisplay("Commande {Id} — {Montant} ({Statut})")>
Public Class Commande
    Public Property Id As Integer
    Public Property Montant As Decimal
    Public Property Statut As StatutCommande
End Class
```

Au lieu de `WindowsApp1.Commande` dans la fenêtre d'espion, vous lirez
`Commande 4271 — 1250.00 (Expédiée)`. Entre accolades, on place une **expression** (champ,
propriété ou appel de méthode) — pas une chaîne de format .NET : pour un affichage élaboré
(montant monétaire, par exemple), le motif recommandé est une **propriété privée** qui renvoie
la chaîne voulue, affichée avec le suffixe `,nq` (*no quotes*) :
`<DebuggerDisplay("{AffichageDebogueur,nq}")>`. Autres attributs utiles : `<DebuggerStepThrough>`
(ne pas entrer dans la méthode en pas à pas) et `<DebuggerBrowsable(DebuggerBrowsableState.Never)>`
(masquer un membre).

---

## Naviguer dans l'exécution

| Action | Raccourci | Effet |
|--------|-----------|-------|
| **Pas à pas détaillé** (*Step Into*) | **F11** | Entre dans la méthode appelée. |
| **Pas à pas principal** (*Step Over*) | **F10** | Exécute la méthode appelée d'un bloc. |
| **Pas à pas sortant** (*Step Out*) | **Maj + F11** | Termine la méthode courante et remonte. |
| **Exécuter jusqu'au curseur** (*Run to Click*) | survol ▸ bouton vert | Reprend jusqu'à la ligne visée. |

Deux gestes complémentaires :

- **Définir l'instruction suivante** : en faisant glisser la flèche jaune, on force l'exécution
  à reprendre à une autre ligne (pour rejouer un bloc ou en sauter un). À manier avec prudence :
  on contourne la logique réelle.
- **Fenêtres de contexte** : la **Pile des appels** (qui appelle qui), les **Threads** (tous les
  fils d'exécution) et les **Modules** (assemblies chargés, symboles) complètent l'inspection.

> 💡 **Retour en arrière (édition Enterprise) :** IntelliTrace et le « débogage historique »
> permettent de **revenir** sur des états d'exécution passés, sans relancer le scénario — utile
> pour les bugs difficiles à reproduire. À savoir toutefois : l'outil est **déprécié dans
> VS 2026** ; il reste disponible, mais n'en faites pas le socle d'une pratique d'équipe.

---

## Hot Reload et Edit and Continue 🆕

**Hot Reload** applique vos modifications de code à l'application **en cours d'exécution**, sans
la redémarrer ni perdre son état. Concrètement : vous corrigez la logique d'un gestionnaire de
bouton, et le correctif est actif immédiatement, sans relancer l'appli ni refaire le parcours
qui mène à l'écran concerné.

- Fonctionne pour **VB.NET** comme pour C# : Hot Reload repose sur le compilateur Roslyn et
  l'outillage .NET, pas sur une spécificité de langage. C'est particulièrement appréciable en
  **Windows Forms** et **WPF**, où relancer l'appli pour tester un détail d'UI est fastidieux.
- VS 2026 **élargit** les structures de code modifiables à chaud.
- **Limites :** certaines modifications « lourdes » (*rude edits*) imposent toujours un
  redémarrage — changer la signature d'une méthode, modifier certains types, toucher à la
  structure d'une machine à états (`Async`/itérateurs) dans des cas particuliers. Le débogueur
  vous le signale alors explicitement.

---

## Déboguer du code asynchrone

C'est l'un des points où le débogage demande un peu de méthode. Avec `Async`/`Await`, une seule
opération logique est découpée en morceaux exécutés à des moments — voire sur des threads —
différents. Une pile d'appels « naïve » ne montrerait que la mécanique interne (`MoveNext` de la
machine à états) plutôt que l'enchaînement logique que vous avez écrit.

Visual Studio 2026 sait reconstituer ce contexte :

- **Pile des appels « consciente » de l'asynchronie :** la fenêtre *Pile des appels* affiche le
  **flux logique** à travers les `Await`, et non la tuyauterie du compilateur.
- **Fenêtre Tâches** (*Déboguer ▸ Fenêtres ▸ Tâches*) : liste les `Task` actives, planifiées ou
  bloquées, avec leur état et leur emplacement.
- **Piles parallèles** (*Parallel Stacks*) : vue graphique des threads et des tâches — précieuse
  pour repérer un blocage ou une famine.

Côté exceptions, rappelez-vous (cf. [12.1](01-exceptions.md)) qu'une exception asynchrone est
relancée au moment du `Await`. La fenêtre **Paramètres d'exception** (*Déboguer ▸ Fenêtres ▸
Paramètres d'exception*) permet de **rompre dès le lancement** d'une exception (*first-chance*),
avant même qu'un `Catch` ne l'intercepte — idéal pour saisir l'état exact au point d'origine.
Les spécificités (parallélisme et `AggregateException`, annulation) sont détaillées au
[module 4.3](../04-async/03-exceptions-async.md).

---

## 🤖 Les agents IA de débogage de VS 2026 — et le point VB.NET

VS 2026 se présente comme un IDE « AI-native » : au-delà des outils classiques ci-dessus, il
introduit des **agents Copilot** spécialisés, intégrés au moteur de débogage (et non cantonnés
au panneau de discussion). On les invoque dans Copilot Chat via la syntaxe `@` :

- **`@debugger` — résolution de bug *validée à l'exécution*.** Plutôt que de deviner par analyse
  statique, l'agent valide ses hypothèses contre le **comportement réel à l'exécution** : il
  parcourt une boucle complète (comprendre et reproduire le problème, instrumenter l'application,
  isoler la cause racine, puis **vérifier le correctif par une exécution réelle**). On peut le
  lancer depuis un ticket GitHub / Azure DevOps ou en décrivant le bug en langage naturel.
- **`@profiler`** capture automatiquement des données d'exécution (temps écoulé, CPU, mémoire)
  pendant le débogage et pointe les goulets d'étranglement (→ voir aussi
  [module 14.1 — Profilage](../14-performance/01-profilage.md)).
- D'autres agents (`@test` pour la génération de tests, `@modernize` pour la migration) sont
  abordés dans le [module 17](../17-developpement-ia/README.md) et le
  [module 13](../13-tests-qualite/README.md).

> ⚠️ **Le point VB.NET, en toute honnêteté.** Ces agents sont **documentés et démontrés sur C#
> (et C++)** : les exemples officiels mettent en scène des applications C#, et l'outillage à
> conscience sémantique sur lequel s'appuient les agents prend en charge **C++, C#, Razor,
> TypeScript** et les langages disposant d'extensions LSP — **VB.NET n'y figure pas
> explicitement**. C'est une illustration de plus du **biais C# de l'outillage** que cette
> formation signale régulièrement (voir [1.4](../01-introduction-vbnet/04-installation-outils.md)
> et le [module 17](../17-developpement-ia/README.md)).
>
> En pratique : les outils **déterministes** (points d'arrêt, espions, pas à pas, Hot Reload,
> débogage asynchrone) fonctionnent à l'identique en VB.NET ; pour les **agents IA**, vérifiez
> la prise en charge de votre version avant de compter dessus, et n'hésitez pas à raisonner sur
> l'exception / la *stack trace* avec un assistant en **précisant toujours « VB.NET »** dans vos
> prompts (cf. [17.5 — Déboguer et optimiser avec l'IA](../17-developpement-ia/05-debugger-optimiser.md)).

---

## Instrumentation légère, sans débogueur

Le débogueur n'est pas toujours l'outil le plus adapté — pour un service en production, une
boucle très chaude, ou un bug qui ne se manifeste que chez le client, **journaliser** vaut mieux
que poser un point d'arrêt. À mi-chemin, l'espace `System.Diagnostics` offre une instrumentation
légère, **retirée automatiquement** des compilations `Release` :

```vb
Imports System.Diagnostics

Debug.WriteLine($"Traitement de la commande {commande.Id}")
Debug.Assert(commande.Montant >= 0, "Le montant ne devrait jamais être négatif")
```

`Debug.WriteLine` et `Debug.Assert` ne sont compilés que si le symbole `DEBUG` est défini ;
ils disparaissent en `Release`. Même logique avec la compilation conditionnelle :

```vb
#If DEBUG Then
    Console.WriteLine("Mode diagnostic actif")
#End If
```

> 💡 Quand l'instrumentation devient permanente et structurée (niveaux, destinations, format),
> on ne parle plus de débogage mais de **journalisation** : c'est l'objet de la section
> suivante.

---

> 📚 Pour l'outillage Visual Studio 2026 (raccourcis, astuces, nouveautés et références
> officielles), voir l'**[Annexe D](../annexes/visual-studio-2026/README.md)** 🆕.

---

## À retenir

Le débogueur de Visual Studio transforme la chasse au bug en observation méthodique : points
d'arrêt **conditionnels** plutôt que F9 répété, espions et visualiseurs pour lire l'état réel,
pas à pas pour suivre le flux, et un outillage **asynchrone** qui reconstitue la logique derrière
la machine à états. Toutes ces capacités sont **pleinement disponibles en VB.NET**, à l'identique
de C#. La seule réserve concerne les **agents IA** introduits dans VS 2026, pensés d'abord pour
C# et C++ : on les utilise avec discernement côté VB, en s'appuyant sur l'IA surtout pour
*expliquer* erreurs et *stack traces*, prompt en main.

➡️ Section suivante : **[12.3 — Journalisation](03-journalisation.md)**, pour savoir ce que fait
l'application *en production*, là où le débogueur ne peut plus aller.

⏭️ [Journalisation (Microsoft.Extensions.Logging, Serilog, *structured logging*)](/12-exceptions-debogage/03-journalisation.md)
