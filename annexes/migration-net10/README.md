🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe E — Guide de migration vers .NET 10 LTS 🆕

Stratégies depuis .NET 6 / 8 / Framework, checklist complète, *breaking changes* de .NET 10, et considérations
spécifiques à VB.NET.

**Le repère qui change tout pour un développeur VB :** le langage est **figé à VB 16.9** (stabilisé,
*consumption-only*). Une migration vers .NET 10 ne porte donc **pas sur la syntaxe VB** — vous ne réécrivez pas
votre code dans une « nouvelle version de VB ». Elle porte sur le **runtime, le SDK, les bibliothèques et le
format de projet**. Mieux : une bonne partie des gains (JIT, PGO, dévirtualisation) arrive **sans toucher au
code** (module 14.6).

---

## E.1 — Pourquoi migrer maintenant : la fenêtre de temps ⏳

La date de ce guide (juin 2026) rend la question urgente, pas théorique :

- **.NET 8 (LTS)** est pris en charge jusqu'au **10 novembre 2026**, soit trois ans après sa sortie de novembre 2023.
- **.NET 9 (STS)**, sorti en novembre 2024, est pris en charge **deux ans, jusqu'en novembre 2026** également.
- Autrement dit, **.NET 8 et .NET 9 atteignent leur fin de vie le même jour, le 10 novembre 2026** — dans
  **environ cinq mois**. Après cette date : plus de correctifs de sécurité ni de bugs.
- **.NET 6 et .NET 7 sont déjà hors support** (respectivement novembre 2024 et mai 2024).
- **.NET 10 (LTS)** est pris en charge **jusqu'en novembre 2028** — c'est le successeur LTS de .NET 8 et le point d'atterrissage stratégique ; il est d'usage de sauter directement .NET 9 pour viser .NET 10.

> 💡 Si vos applications tournent sur .NET 8 ou .NET 9, vous disposez de **mois, pas d'années**. Pour une grande
> base de code, comptez **3 à 6 mois** d'évaluation, de migration et de tests : planifiez dès maintenant.

| Version de départ | État (juin 2026) | Urgence |
|-------------------|------------------|---------|
| .NET Framework 4.x | Lié au cycle de Windows (supporté) | Selon stratégie (modules 11.3-11.4) |
| .NET 6 / 7 | ❌ Hors support | Critique |
| .NET 8 (LTS) / .NET 9 (STS) | ⚠️ Fin de support le 10 nov. 2026 | Élevée |
| .NET 10 (LTS) | ✅ Jusqu'en nov. 2028 | — (cible) |

---

## E.2 — Choisir sa stratégie selon le point de départ

Deux trajectoires **radicalement différentes** :

### Cas 1 — .NET 6 / 8 / 9 → .NET 10 (moderne → moderne)

Le cas simple. Le plus souvent : **changer le *Target Framework* en `net10.0`**, **mettre à jour les paquets
NuGet**, traiter les *breaking changes* applicables, puis tester. Effort faible à modéré. C'est essentiellement
une montée de version, pas une réécriture.

### Cas 2 — .NET Framework → .NET 10 (legacy → moderne)

Un vrai projet (modules 11.3 et suivants). Il implique :

- la **conversion au format de projet SDK-style** (`.vbproj` allégé) ;
- le passage de `packages.config` à **`PackageReference`** ;
- le remplacement de `app.config`/`web.config` par **`appsettings.json`** (`Microsoft.Extensions.Configuration`) ;
- une **analyse de dépendances** (API retirées, paquets non portés) ;
- parfois une **réécriture** pour ce qui n'a aucun chemin de migration (voir ⚠️ ci-dessous).

**Incrémental vs *big-bang*** (module 11.1) : pour le legacy, l'approche **incrémentale** est presque toujours
préférable — extraire d'abord la logique dans des **bibliothèques `.NET Standard 2.0`** consommables des deux
côtés (module 11.5), puis migrer couche par couche.

> ⚠️ **Sans chemin de migration vers .NET moderne :** **ASP.NET Web Forms** (module 11.4). Vos options sont
> **rester sur .NET Framework** (supporté tant que Windows l'est) ou **réécrire** (Web API VB + front séparé, ou
> MVC/Blazor en C# — voir [Annexe B](../frontiere-vbnet-csharp/README.md)). Autres absents du moderne à anticiper :
> WCF côté serveur (→ CoreWCF, gRPC ou REST), Windows Workflow Foundation, .NET Remoting, AppDomains.

---

## E.3 — Outils de migration

- **Assistant de mise à niveau .NET** (*.NET Upgrade Assistant*) : extension Visual Studio / CLI qui automatise une partie de la conversion (format de projet, retargeting, repérage des API).
- **Agent `@modernize-dotnet` (GitHub Copilot modernization)** 🤖 : l'agent prend en charge **aussi bien C# que Visual Basic**, pour les montées de version .NET et la migration vers Azure ; il est inclus dans Visual Studio 2026 (ou Visual Studio 2022 17.14.17 et ultérieure), et disponible aussi dans VS Code, la CLI Copilot et sur GitHub.com, avec plus de trente compétences chargées automatiquement selon les technologies détectées. On l'invoque via `@modernize-dotnet` dans le chat Copilot (modules 11.8 et 17.3).
- **Analyse de portabilité / d'API** pour cartographier les usages incompatibles avant de se lancer.

> ⚠️ Ces outils **accélèrent**, ils ne **remplacent ni les tests, ni le jugement**. Tout code produit par l'IA
> doit être **validé** (règles de l'[Annexe C § C.6](../bonnes-pratiques/README.md)).

---

## E.4 — Prérequis outillage : le piège Visual Studio ⚠️

Un point concret qui bloque beaucoup d'équipes :

- **Pour cibler `net10.0`, il faut Visual Studio 2026 (v18.0) ou ultérieur.** Visual Studio 2022 (17.14) avec le SDK .NET 10 ne permet de cibler que **.NET 9 et antérieurs**.
- Conséquence : si votre équipe est encore sous **VS 2022**, intégrez la **montée vers Visual Studio 2026** au
  périmètre de la migration ([Annexe D](../visual-studio-2026/README.md)). L'IDE étant désormais découplé des
  compilateurs, cette mise à jour ne casse pas votre chaîne d'outils existante.

---

## E.5 — *Breaking changes* de .NET 10 : les principaux

Microsoft maintient une **liste officielle**, classant chaque changement en *incompatibilité binaire*, *incompatibilité source* ou *changement de comportement*, et regroupée par domaine technique (runtime, SDK, ASP.NET Core, Windows Forms, EF Core…). C'est un **document vivant** : consultez-le pour la liste complète et à jour. Les points
saillants pour les scénarios VB :

- **Mot-clé `field` de C# 14** : changement à incompatibilité source en C#, déclenché si une base de code utilise déjà un membre nommé `field`.
  ✅ **Sans objet en VB** : la syntaxe de VB étant figée, ce *breaking change* **ne vous concerne pas** — un
  avantage discret de la stabilité du langage.
- **SDK / MSBuild** : l'exigence de version de Visual Studio ci-dessus ([§ E.4](#e4--prérequis-outillage--le-piège-visual-studio-)) est elle-même un *breaking change* documenté.
- **Windows Forms** (le cœur de VB ⭐) : suppressions d'API obsolètes et changements de comportement peuvent
  s'appliquer — consultez la page *breaking changes* dédiée à Windows Forms avant de migrer une application
  WinForms.
- **ASP.NET Core 10** (Web API VB par contrôleurs, module 8.2) :
  - les points de terminaison sécurisés par authentification par cookie et marqués `IApiEndpointMetadata` **ne redirigent plus** vers une page de connexion ou de refus d'accès (ils renvoient un code 401/403) — changement de comportement à tester ;
  - `IActionContextAccessor` et `ActionContextAccessor` sont marqués **obsolètes** ;
  - **OpenAPI 3.1 par défaut** : les descriptions sur les propriétés définies avec `$ref` sont désormais conservées dans le document généré.
- **EF Core 10** (module 7.2) : changements documentés sur une **page dédiée** — à vérifier si vous utilisez EF.
- **Spécifique aux plateformes** : des changements visant Linux/macOS (images Ubuntu, cryptographie) peuvent
  casser du code multiplateforme. Si vous déployez ailleurs que sous Windows, **testez sur toutes les cibles** en CI/CD.

---

## E.6 — Checklist complète de migration

### Préparation

- [ ] **Inventorier** les projets, leurs *Target Frameworks* actuels et leurs dépendances.
- [ ] **Décider de la cible** : `net10.0` (et, pour WinForms/WPF, `net10.0-windows`).
- [ ] **Auditer les paquets NuGet** : disponibilité d'une version compatible .NET 10 ; repérer les dépendances orphelines.
- [ ] **Mettre à jour l'outillage** : Visual Studio 2026 (obligatoire pour cibler `net10.0`).
- [ ] **Créer une branche** dédiée et **établir une base de tests** (filet de non-régression — module 11.7).

### Conversion

- [ ] Convertir au **format de projet SDK-style**.
- [ ] Migrer `packages.config` → **`PackageReference`**.
- [ ] Migrer `app.config`/`web.config` → **`appsettings.json`** si pertinent.
- [ ] Passer le **`TargetFramework`** à `net10.0` et **mettre à jour les paquets**.
- [ ] Lancer l'**Assistant de mise à niveau** ou l'agent **`@modernize-dotnet`**, puis relire ses modifications.

### Adaptation

- [ ] Traiter les **breaking changes** applicables ([§ E.5](#e5--breaking-changes-de-net-10--les-principaux)).
- [ ] Remplacer les **API retirées/obsolètes** signalées par le compilateur.
- [ ] **Résorber les avertissements** (idéalement, `TreatWarningsAsErrors` — Annexe C.1).

### Vérifications spécifiques VB.NET

- [ ] Auditer l'usage de l'espace **`My`** (⚠️ limité en WPF ; membres web supprimés — [§ E.7](#e7--considérations-spécifiques-à-vbnet)).
- [ ] Confirmer **`Option Strict On`** au niveau projet (Annexe C.1).
- [ ] Pour un **service/worker** : recâbler manuellement le **Generic Host** (pas de modèle *Worker* VB — module 4.8).

### Validation et déploiement

- [ ] **Build propre** + exécution complète des **tests** (non-régression).
- [ ] **Tester sur toutes les cibles de déploiement** ; vérifier les performances.
- [ ] Préparer un **plan de *rollback*** ; envisager une **exécution en parallèle** (.NET 8/9 et .NET 10) puis un déploiement **progressif**.

---

## E.7 — Considérations spécifiques à VB.NET

C'est ce qui distingue une migration VB d'une migration C#.

- **Langage figé (VB 16.9)** ⭐ : vous **n'adaptez pas de syntaxe VB**. La migration concerne le runtime, le SDK,
  les paquets et le format de projet — d'où un risque syntaxique **nul**.
- **Gains de performance « gratuits »** : .NET 10 apporte des améliorations majeures du runtime (*inlining* JIT, dévirtualisation de méthodes, allocations sur la pile), qui se traduisent directement par des applications plus rapides — sans changer le code (module 14.6). Une **raison de migrer même pour une application VB stable**.
- **Espace `My` ⚠️** : support partiel sur .NET moderne — correct en WinForms, limité en WPF, et membres web
  (`My.Request`, `My.WebServices`) **supprimés**. Auditez l'usage (module 2.12) et abstrayez derrière une
  interface ce qui doit rester testable.
- **Pas de modèle de projet *Worker*** : un service Windows / traitement de fond se câble **à la main** via le
  Generic Host (`IHostedService`/`BackgroundService` — module 4.8).
- **ASP.NET Web Forms** ⚠️ : **aucun chemin** vers le moderne (rester sur .NET Framework ou réécrire — module 11.4).
- **Azure Functions** : **pas de support VB officiel** → si vous en avez, basculez ces fonctions en C# (module 15.5).
- **Les *breaking changes* propres à C# ne vous concernent pas** (ex. le mot-clé `field`) : la stabilité du
  langage est ici un atout de migration.
- **Opportunité hybride** 🔗 : la migration est le bon moment pour **isoler les briques modernes en C#** (perf,
  *records*, *source generators*) et les consommer depuis VB (module 10 et [Annexe B](../frontiere-vbnet-csharp/README.md)).
- **Assistance IA** 🤖 : `@modernize-dotnet` couvre VB ✅, mais **validez systématiquement** sa sortie
  ([Annexe C § C.6](../bonnes-pratiques/README.md)).

---

## E.8 — En résumé

| Situation | Action recommandée |
|-----------|--------------------|
| Sur .NET 6 / 7 (hors support) | Migrer **sans délai** vers .NET 10 |
| Sur .NET 8 / 9 | Planifier la migration **avant le 10 nov. 2026** ; souvent un simple changement de TFM + mise à jour des paquets + tests |
| Sur .NET Framework (hors Web Forms) | Migration incrémentale via `.NET Standard` ; conversion SDK-style ; analyse de dépendances |
| Application **Web Forms** | Pas de migration directe : rester sur .NET Framework **ou** réécrire (Annexe B) |
| Outillage | **Visual Studio 2026 obligatoire** pour cibler `net10.0` |
| Langage VB | **Rien à migrer** côté syntaxe (figé à 16.9) |

---

### Voir aussi

- Module 1.3 — [L'écosystème .NET](../../01-introduction-vbnet/03-ecosysteme-dotnet.md) (LTS/STS, support jusqu'en nov. 2028)
- Module 11 — [Migration et maintenance du code legacy](../../11-migration-legacy/README.md) ⭐ (11.3 Framework → .NET 10, 11.4 Web Forms ⚠️, 11.5 coexistence, 11.7 risques)
- Module 14.6 — [Ce que .NET 10 apporte gratuitement](../../14-performance/06-apports-net10.md) 🆕
- Module 17.3 — [Migrer du legacy avec l'IA](../../17-developpement-ia/03-migration-legacy-ia.md) 🤖
- [Annexe B — Frontière VB.NET / C#](../frontiere-vbnet-csharp/README.md) 🔗 · [Annexe C — Bonnes pratiques](../bonnes-pratiques/README.md) · [Annexe D — Visual Studio 2026](../visual-studio-2026/README.md) · [Annexe H — Versions et cycle de support](../versions-reference/README.md)

---

**Juin 2026** · .NET 10 LTS (support jusqu'au 14 novembre 2028) · Visual Studio 2026 · VB.NET 16.9 (stabilisé)

⏭️ [Glossaire et acronymes](/annexes/glossaire/README.md)
