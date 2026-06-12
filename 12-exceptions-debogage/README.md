🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 12. Exceptions, débogage et journalisation

> **Partie 6 — Qualité, performance et exploitation**
> Rendre une application VB.NET **fiable**, **diagnosticable** et **exploitable** en production.

---

## Vue d'ensemble

Un programme qui « marche sur ma machine » ne suffit pas. Dès qu'une application VB.NET
quitte le poste du développeur — qu'il s'agisse d'une appli Windows Forms déployée sur des
dizaines de postes, d'une bibliothèque métier réutilisée par d'autres équipes, ou d'une Web
API exposée à des clients — trois questions deviennent vitales :

1. **Que se passe-t-il quand quelque chose tourne mal ?**
   → la *gestion des exceptions* : intercepter, qualifier, réagir, et ne jamais masquer
   silencieusement une erreur.
2. **Comment comprendre un bug sans deviner ?**
   → le *débogage* : observer l'état réel du programme, pas à pas, plutôt que d'empiler
   des `MessageBox.Show` au hasard.
3. **Comment savoir ce que fait l'application en production, sans y être ?**
   → la *journalisation* et l'*observabilité* : laisser une trace structurée et exploitable
   de ce qui se passe, hier comme en ce moment même.

Ce chapitre traite ces trois piliers comme un tout cohérent. Ils répondent à un même besoin :
passer d'un code qui *fonctionne* à un logiciel qu'on peut *maintenir, dépanner et faire vivre*
dans la durée.

---

## Pourquoi ce chapitre est entièrement « dans le périmètre » de VB.NET

C'est l'un des rares chapitres où la stratégie de « langage stabilisé » de VB.NET **ne coûte
rien** : aucun des sujets traités ici ne dépend d'une nouvelle syntaxe du langage. Tout repose
sur le **runtime**, la **bibliothèque de base (BCL)**, des **bibliothèques NuGet** et l'**IDE** —
exactement le terrain où VB.NET est un citoyen de première classe.

- 🟢 **Les exceptions sont du langage VB historique.** `Try` / `Catch` / `Finally`, ainsi que
  les **filtres `When`**, existent en VB depuis longtemps (VB.NET les proposait d'ailleurs
  *avant* que C# n'introduise `when` en C# 6). Le gel du langage ne les concerne pas : rien
  à déléguer, rien à contourner.
- 🟢 **Le débogage est une fonctionnalité de l'IDE, pas du langage.** Visual Studio 2026 traite
  VB.NET et C# avec le même moteur de débogage : points d'arrêt, espions, **Hot Reload**,
  débogage asynchrone — tout est disponible côté VB. 🆕
- 🟢 **La journalisation et l'observabilité sont des bibliothèques à *consommer*.**
  `Microsoft.Extensions.Logging`, Serilog, OpenTelemetry… ce sont des paquets NuGet que l'on
  utilise via leurs API. C'est précisément le scénario *consumption-only* officiellement
  recommandé pour VB.NET — aucune limitation par rapport à C#.

> 💡 **À retenir :** sur ce chapitre, vous n'avez pas à raisonner « VB ou C# ? ». La vraie
> question est « **bonnes pratiques ou pas** ? ». Les outils, eux, sont les mêmes pour les
> deux langages.

---

## Objectifs du chapitre

À l'issue de ce chapitre, vous saurez :

- Concevoir une **stratégie de gestion des exceptions** claire (où intercepter, quoi
  intercepter, quand laisser remonter), et écrire des **exceptions personnalisées** porteuses
  de sens métier.
- Utiliser efficacement le **débogueur de Visual Studio 2026** sur du code VB.NET, y compris
  pour du code **asynchrone** (`Async`/`Await`) et avec le **Hot Reload**.
- Mettre en place une **journalisation structurée** avec `Microsoft.Extensions.Logging` et,
  au besoin, **Serilog** — au lieu d'écrire dans la console ou un fichier texte ad hoc.
- Comprendre les **notions d'observabilité** (OpenTelemetry, *health checks*, métriques) et
  savoir quand elles deviennent pertinentes pour une application VB.NET.

---

## Contenu du chapitre

| Section | Sujet | Indicateurs |
|---------|-------|-------------|
| **12.1** | [`Try` / `Catch` / `Finally`, filtres `When`, hiérarchie, exceptions personnalisées](01-exceptions.md) | |
| **12.2** | [Débogage (points d'arrêt, espions, Hot Reload, débogage asynchrone) ; outils de VS 2026](02-debogage.md) | 🆕 |
| **12.3** | [Journalisation (`Microsoft.Extensions.Logging`, Serilog, *structured logging*)](03-journalisation.md) | |
| **12.4** | [Observabilité (notions : OpenTelemetry, *health checks*, métriques)](04-observabilite.md) | |

**12.1 — Exceptions.** Le socle. Comprendre la hiérarchie des exceptions .NET, manier
`Try` / `Catch` / `Finally`, exploiter les filtres `When` propres à VB, et créer ses propres
types d'exception pour exprimer des erreurs métier plutôt que de tout noyer dans un
`Catch ex As Exception` fourre-tout.

**12.2 — Débogage.** Diagnostiquer au lieu de deviner. Maîtriser les points d'arrêt
(conditionnels, par données), les fenêtres d'espions et d'inspection, le **Hot Reload** pour
modifier le code sans redémarrer, et les spécificités du **débogage asynchrone** — le tout
avec l'outillage de Visual Studio 2026. 🆕

**12.3 — Journalisation.** Garder une trace exploitable. Adopter l'abstraction standard
`Microsoft.Extensions.Logging`, comprendre les niveaux de log et le *logging structuré*, et
voir quand un fournisseur comme **Serilog** apporte un vrai plus (enrichissement, *sinks*,
formats).

**12.4 — Observabilité (notions).** Voir l'application vivante. Une introduction à
**OpenTelemetry** (traces, métriques, logs), aux *health checks* et aux métriques — surtout
utile pour les Web API et les services, et présenté ici comme une ouverture plutôt qu'un
approfondissement.

---

## Prérequis et liens avec les autres modules

**Prérequis recommandés :**

- **[Module 4 — Programmation asynchrone et parallèle](../04-async/README.md)** : la gestion
  des exceptions et le débogage en contexte `Async`/`Await` supposent d'être à l'aise avec les
  notions du chapitre 4 (en particulier [4.3 — Gestion des exceptions asynchrones](../04-async/03-exceptions-async.md)).
- Des bases de **POO** ([Module 3](../03-poo/README.md)) pour concevoir des exceptions
  personnalisées par héritage.

**Ce chapitre prépare ou complète :**

- **[Module 8 — Services web](../08-services-web/README.md)** : journalisation, `Problem Details`
  et observabilité prennent tout leur sens côté Web API.
- **[Module 13 — Tests et qualité](../13-tests-qualite/README.md)** : un code bien instrumenté
  est un code plus facile à tester et à diagnostiquer.
- **[Module 14 — Performance](../14-performance/README.md)** : le profilage prolonge
  naturellement l'observabilité.
- **[Annexe D — Raccourcis et astuces Visual Studio 2026](../annexes/visual-studio-2026/README.md)** 🆕
  pour aller plus loin sur l'outillage de débogage.

> 🤖 **Côté IA :** déboguer et analyser des erreurs avec un assistant (expliquer une exception,
> lire une *stack trace*, suggérer un correctif) fait l'objet d'une section dédiée —
> voir **[17.5 — Déboguer et optimiser avec l'IA](../17-developpement-ia/05-debugger-optimiser.md)**.
> Pensez toutefois à toujours préciser « VB.NET » dans vos prompts : les modèles répondent
> spontanément en C#.

---

## En résumé

La fiabilité d'une application ne se décrète pas, elle s'**outille**. Exceptions bien gérées,
débogage maîtrisé et journalisation structurée forment le trépied qui permet de **comprendre
ce qui se passe** — pendant le développement comme en production. Et la bonne nouvelle, on l'a
vu, c'est que sur ce terrain VB.NET ne souffre d'**aucun handicap** face à C# : ce sont les
mêmes outils, les mêmes bibliothèques, le même débogueur.

➡️ Commencez par **[12.1 — Exceptions](01-exceptions.md)**.

⏭️ [Try/Catch/Finally, filtres When, hiérarchie, exceptions personnalisées](/12-exceptions-debogage/01-exceptions.md)
