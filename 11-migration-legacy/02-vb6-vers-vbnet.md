🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11.2 — VB6 → VB.NET (outils de conversion, pièges, APIs obsolètes)

> Visual Basic 6 et VB.NET partagent un nom et un air de famille syntaxique. C'est précisément le piège : cette ressemblance laisse croire à un simple portage, alors qu'il s'agit d'un **changement de plateforme**. VB6 produit du code COM natif de l'ère Win32 ; VB.NET produit du code managé sur le CLR. Migrer de l'un à l'autre, ce n'est pas traduire ligne à ligne — c'est changer de monde d'exécution.

---

## 1. VB6 et VB.NET : un nom commun, deux mondes

La parenté entre VB6 et VB.NET est réelle mais superficielle. En profondeur, tout diffère : le modèle objet (COM contre .NET), le *runtime* (le *VB6 runtime* contre le CLR), le système de types, le modèle de formulaires, l'accès aux données. Le danger principal de cette migration est donc le **faux ami** : du code qui « ressemble » au VB6 d'origine, qui compile, mais qui ne se comporte pas de la même façon.

Bonne nouvelle de calendrier : le *runtime* VB6 est toujours distribué avec Windows et pris en charge pour la durée de vie du système — une application VB6 ne cesse pas de fonctionner du jour au lendemain. Migrer relève donc de la **pérennité** (compétences qui se raréfient, impossibilité d'évoluer, écosystème figé), pas d'une urgence de panne. Ce qui laisse le temps de bien faire — à condition de commencer par **évaluer l'existant** (→ [§11.1](01-evaluer-strategies.md)) : recenser les contrôles OCX/ActiveX, l'accès aux données (DAO/RDO/ADO), les dépendances tierces, et vérifier que le **code source est bien disponible** (il est perdu dans une proportion non négligeable de parcs VB6).

---

## 2. Les pièges de la conversion (les *faux amis*)

Ce sont les constructions qui existent dans les deux langages mais n'y signifient pas la même chose. Ce sont les plus dangereuses, car elles ne déclenchent **aucune erreur de compilation** : le bug se manifeste à l'exécution, parfois bien plus tard.

### 2.1 Les types numériques changent de taille ⚠️

Le piège le plus insidieux. La taille des entiers n'est pas la même d'un langage à l'autre :

| Type VB6 | Taille VB6 | Équivalent VB.NET | Taille VB.NET |
|---|---|---|---|
| `Integer` | 16 bits | **`Short`** | 16 bits |
| `Long` | 32 bits | **`Integer`** | 32 bits |
| *(— )* | — | `Long` | 64 bits |
| `Currency` | 64 bits (monétaire) | **`Decimal`** (`Currency` supprimé) | 128 bits |
| `Variant` | variable | **`Object`** | référence |

Autrement dit : un `Integer` VB6 doit devenir un `Short` en VB.NET, et un `Long` VB6 un `Integer`. Un portage naïf qui conserve les mots-clés `Integer`/`Long` **change silencieusement la taille des variables** — avec des conséquences sur les débordements, le calcul, et surtout l'interopérabilité avec du code natif (P/Invoke) ou des fichiers binaires dont la structure dépend de la taille exacte des champs. Les outils de conversion gèrent correctement ce point ; le code repris à la main est le terrain où l'erreur s'introduit.

### 2.2 Le passage de paramètres : `ByRef` par défaut ⚠️

En VB6, un paramètre sans qualificateur est passé **par référence** (`ByRef`). En VB.NET, le défaut est **par valeur** (`ByVal`). Une procédure qui modifiait son argument en comptant sur le passage par référence cessera de le faire si la conversion ne rend pas le `ByRef` explicite — là encore sans aucune erreur visible. C'est l'un des changements de comportement les plus discrets et les plus coûteux à diagnostiquer après coup.

### 2.3 Les déclarations multiples sur une ligne

Subtilité classique : en VB6, `Dim x, y As Integer` déclare `y` en `Integer` mais **`x` en `Variant`**. En VB.NET, la même ligne déclare **les deux** en `Integer`. Le comportement change donc silencieusement à la conversion — d'où l'importance d'activer `Option Strict` et `Option Explicit` (→ module 2, [§2.1](../02-fondamentaux-langage/01-structure-options.md)) pour fiabiliser le code cible.

### 2.4 Les propriétés par défaut

VB6 autorisait les propriétés par défaut **sans argument** : écrire `Text1 = "bonjour"` signifiait implicitement `Text1.Text = "bonjour"`. VB.NET ne conserve les propriétés par défaut que pour les **indexeurs** (propriétés prenant un argument). Toutes les références implicites doivent donc être rendues **explicites** (`.Text`, `.Value`, `.Caption`…), ce que les outils savent faire mais qu'il faut vérifier.

### 2.5 Les tableaux : bornes et base

En VB6, un tableau pouvait avoir une borne inférieure arbitraire (`Dim a(1 To 10)`, ou `Option Base 1` à l'échelle du module). En VB.NET, **tout tableau commence à l'indice 0**. Les boucles `For i = 1 To UBound(a)` et les accès indexés doivent être réexaminés. La sémantique de `ReDim Preserve` diffère également.

### 2.6 Les tableaux de contrôles (*control arrays*) ⚠️

VB6 offrait les *control arrays* — plusieurs contrôles partageant un nom et un indice (`Command1(0)`, `Command1(1)`…) et un gestionnaire d'événement commun. **VB.NET n'a pas d'équivalent natif.** Il faut reconstruire le mécanisme à la main : une collection de contrôles et un câblage d'événements explicite (`AddHandler`). C'est l'un des postes de conversion les plus chronophages des interfaces VB6 chargées.

---

## 3. Les constructions et APIs obsolètes

Au-delà des faux amis, certaines constructions VB6 ont purement et simplement disparu, ou sont fortement déconseillées en VB.NET. Le tableau ci-dessous récapitule les principales et leur remplacement :

| Construction VB6 | Remplacement en VB.NET |
|---|---|
| `On Error GoTo` / `On Error Resume Next` / objet `Err` | `Try` / `Catch` / `Finally` (→ module 12, [§12.1](../12-exceptions-debogage/01-exceptions.md)) — `On Error` reste toléré mais déconseillé |
| `GoSub` / `Return` | Extraire une véritable méthode (`Sub`/`Function`) |
| `Type … End Type` (type défini par l'utilisateur) | `Structure … End Structure` |
| `Set obj = …` / `Let x = …` | Affectation simple (le mot-clé `Set`/`Let` disparaît) |
| `Property Get` / `Property Let` / `Property Set` | `Property` unifiée avec `Get` / `Set` |
| `Wend` | `End While` |
| `DefInt`, `DefStr`, `DefLng`… | Typage explicite de chaque variable |
| `Variant` | `Object` |
| `Currency` | `Decimal` |
| `Null` / `Empty` (distincts) | `Nothing` (unifié) |
| `Debug.Print` | `Debug.WriteLine` |
| `Collection` (VB6) | `List(Of T)` / `Dictionary(Of TKey, TValue)` |
| Chaînes de longueur fixe (`String * n`) | `String` (ou traitement spécifique) |
| DAO / RDO / ADO + `DataEnvironment` | ADO.NET / EF Core (→ module 7) |

> 💡 La plupart des fonctions familières (`Left$`, `Mid$`, `UCase$`, `IsNumeric`…) existent toujours via l'espace de noms `Microsoft.VisualBasic`, présent sur .NET moderne. On peut donc s'appuyer dessus dans un premier temps, puis migrer progressivement vers les API .NET natives (`String.Substring`, etc.) lors de la phase de modernisation (→ [§11.6](06-moderniser.md)).

### 3.1 Le piège de la bibliothèque de compatibilité ⚠️

Pour faciliter les migrations historiques, il avait existé un espace de noms `Microsoft.VisualBasic.Compatibility` (à ne pas confondre avec `Microsoft.VisualBasic`, toujours d'actualité). Cette **couche de compatibilité VB6 est dépréciée et absente de .NET moderne** : elle ne ciblait que .NET Framework. Concrètement, une conversion qui s'appuie sur ces béquilles produit du code qui **bloquera tout passage ultérieur à .NET 10** (→ [§11.3](03-framework-vers-net10.md)). Mieux vaut donc viser d'emblée du code qui s'en affranchit, plutôt que de reporter la dette.

---

## 4. L'interface graphique : le poste le plus coûteux

C'est généralement la partie la plus lourde de la migration, parce que le modèle de formulaires et de contrôles de VB6 ne correspond pas un-pour-un à celui de Windows Forms.

- **Contrôles OCX / ActiveX** : il faut soit trouver un équivalent .NET, soit les encapsuler via l'interop COM (hébergement `AxHost`) — solution Windows uniquement, avec ses contraintes (→ module 9, [§9.2](../09-interoperabilite/02-com-office.md)).
- **Méthodes graphiques de formulaire** (`Line`, `Circle`, `PSet`, `Print` directement sur la `Form`) : à réécrire avec GDI+ (`System.Drawing`).
- **Instances de formulaire par défaut** : VB6 permettait de référencer `Form1` directement (instance par défaut implicite). En Windows Forms, VB.NET réintroduit cette commodité via `My.Forms`, mais le comportement reste à valider.
- **Réalité du résultat** : même bien converties, les interfaces sortent « à la forme VB6 ». Une UI propre — *a fortiori* en WPF — demande souvent une **reconstruction manuelle** (→ modules [5](../05-windows-forms/README.md) WinForms et [6](../06-wpf/README.md) WPF).

---

## 5. Les outils de conversion

Premier point, essentiel : **l'assistant de mise à niveau intégré à Visual Studio a disparu**. Il était livré avec les premières versions de l'IDE (VS 2002 à 2008) et permettait d'ouvrir un projet VB6 pour le convertir ; les versions modernes de Visual Studio ne le proposent plus. Il ne faut donc pas compter sur un chemin intégré et gratuit.

Le marché s'appuie aujourd'hui sur des outils spécialisés :

| Outil | Éditeur | Ce qu'il fait |
|---|---|---|
| **VBUC** (Visual Basic Upgrade Companion) | Mobilize.Net (désormais Growth Acceleration Partners) | L'outil de référence, assisté par IA. VB6 → VB.NET **ou** C#, ciblant .NET Framework ou .NET moderne. Conversion ADO→ADO.NET, résolution des types et du *late binding*, remplacement de la gestion d'erreurs, mappage des composants tiers, projets multiples. Une licence gratuite a existé pour de petits volumes (≈ 10 000 lignes) via un partenariat Microsoft — à vérifier au moment du projet. |
| **VB Migration Partner** | Code Architects | Met l'accent sur la couverture de « toutes les fonctionnalités et contrôles VB6 » et la parité de comportement, avec moins de reprises manuelles. |
| **gmStudio** | Great Migrations | Environnement de ré-ingénierie : analyse approfondie, personnalisation et suivi des transformations ; VB6 → VB.NET ou C#. Une édition gratuite (≈ 10 000 lignes) a également existé via le même partenariat. |
| **Interop Forms Toolkit** | Microsoft | **Pas un convertisseur** : permet d'afficher des formulaires et contrôles **.NET dans une application VB6** pendant la transition (le sens inverse — un ActiveX dans .NET — passe par `AxHost`, § 4), utile pour une migration **incrémentale** (→ [§11.5](05-coexistence.md)). |

**À quoi s'attendre.** Les outils convertissent automatiquement de l'ordre de **70 à 80 %** du code (formulaires, modules, logique métier) ; le reste — contrôles tiers, interface, fonctionnalités sans équivalent — exige des reprises manuelles. Et surtout, le code produit **n'est pas du .NET idiomatique** : c'est une base fonctionnelle, pas la destination.

**IA et VB6.** Les outils modernes (VBUC en tête) sont désormais assistés par IA, et des assistants généralistes peuvent aider. Mais la prudence est de mise : les modèles sont encore moins entraînés sur VB6 que sur VB.NET, et le risque d'hallucination (API ou packages inexistants) est réel — d'où une **validation systématique**. L'usage de l'IA en migration est traité au module [17](../17-developpement-ia/README.md) (→ notamment [§17.3](../17-developpement-ia/03-migration-legacy-ia.md)) 🤖.

---

## 6. Stratégie : convertir d'abord, moderniser ensuite

La démarche éprouvée — recommandée par les spécialistes de la migration et cohérente avec l'approche incrémentale du [§11.1](01-evaluer-strategies.md) — tient en une règle : **atteindre d'abord l'équivalence fonctionnelle sur .NET, ré-architecturer ensuite**. Vouloir produire du code .NET parfaitement idiomatique en une seule passe est le meilleur moyen d'enliser le projet.

Quelques principes pratiques :

- **Nettoyer le VB6 avant de convertir** : supprimer le code mort, corriger les bugs connus, consolider les modules. La qualité de l'entrée conditionne celle de la sortie.
- **Choisir la cible** : soit convertir directement vers .NET moderne avec un outil qui le permet, soit procéder en deux temps — VB6 → VB.NET sur .NET Framework, puis .NET Framework → .NET 10 (→ [§11.3](03-framework-vers-net10.md)).
- **Sécuriser avec des tests de caractérisation** : verrouiller le comportement avant/après la conversion (→ [§11.7](07-gestion-risques.md)).
- **Coexister pendant la transition** si nécessaire (Interop Forms Toolkit, → [§11.5](05-coexistence.md)).
- **Planifier la phase de modernisation** : le code converti est un point de départ ; le rendre idiomatique (async, LINQ, EF Core, injection de dépendances) est un travail à part entière (→ [§11.6](06-moderniser.md)).

---

## 🔑 Points clés à retenir

- VB6 → VB.NET est un **changement de plateforme**, pas un portage : la ressemblance syntaxique est un piège.
- Les pièges dangereux sont **silencieux** : changement de taille des entiers (`Integer`→`Short`, `Long`→`Integer`), passage de paramètres `ByRef`→`ByVal` par défaut, déclarations multiples, propriétés par défaut, tableaux base 0, *control arrays* sans équivalent.
- Plusieurs constructions ont disparu (`GoSub`, `Type`, `Set`/`Let`, `On Error` déconseillé), et la couche **`Microsoft.VisualBasic.Compatibility` est une impasse** sur .NET moderne.
- L'**assistant intégré n'existe plus** ; on s'appuie sur des outils spécialisés (VBUC, VB Migration Partner), désormais assistés par IA — mais en prévoyant des reprises manuelles, surtout sur l'interface.
- La bonne séquence : **conversion automatisée → équivalence fonctionnelle → refactoring incrémental**. Ne jamais attendre du .NET idiomatique en sortie de convertisseur.

---

⬅️ [11.1 — Évaluer l'existant ; stratégies](01-evaluer-strategies.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [11.3 — .NET Framework 4.x → .NET 10](03-framework-vers-net10.md)

⏭️ [.NET Framework 4.x → .NET 10 (analyse de dépendances, APIs retirées, appsettings, breaking changes .NET 10)](/11-migration-legacy/03-framework-vers-net10.md)
