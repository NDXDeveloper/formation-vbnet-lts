🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 10.2 Quand déléguer à C# (perf/`Span`, records, source generators, Minimal APIs, Native AOT, Blazor/MAUI)

> **Des critères concrets pour décider ce qui part en C# et ce qui reste en VB.NET — et sous quelle forme la délégation s'opère.**

La section 10.1 a établi *pourquoi* l'hybride est la bonne réponse. Reste la question opérationnelle : **face à un besoin donné, faut-il déléguer ?** Cette section fournit une grille de décision, puis l'applique à chacun des sujets nommés dans le titre. Le catalogue exhaustif des sujets hors VB figure dans l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)** ; ici, on se concentre sur le **jugement**.

---

## Le principe de décision

Avant tout, une distinction qui évite bien des confusions : **toutes les délégations n'ont pas la même forme**.

- **Brique consommée** — on écrit une **bibliothèque ou un composant C#** que l'application VB.NET, restée hôte, **consomme en cours d'exécution** par une simple référence. C'est le motif de l'architecture hybride au sens strict (sections 10.3 et 10.4). *Exemples : un parseur `Span`-first, un modèle à base de records, un type produit par un générateur.*
- **Surface séparée** — la fonctionnalité **n'est pas une bibliothèque que VB consomme en interne** : c'est une **interface ou un service à part entière**, écrit en C#, qui vit de son côté. *Exemples : un front-end Blazor, une application MAUI, un service Minimal API, un exécutable Native AOT.* Ici, VB ne « consomme » pas la surface en processus ; il joue un autre rôle (client, ou fournisseur d'une bibliothèque métier partagée), voire aucun.

Confondre les deux conduit à de mauvaises décisions : déléguer un *record* et déléguer un *front-end Blazor* ne sont pas la même opération. Garder cette distinction en tête, puis se poser quatre questions :

1. **Est-ce seulement non *déclarable* en VB, alors que c'est *consommable* ?** (records, `init`, types générés) → une **petite** bibliothèque C# déclare la chose, VB la consomme. Délégation peu coûteuse.
2. **Est-ce possible en VB mais *contraint* / pénible ?** (Minimal APIs, manipulation fine de `Span`) → C# est le foyer naturel ; on délègue pour l'ergonomie.
3. **Est-ce *hors périmètre* VB ?** (Blazor/MAUI/WinUI, Native AOT, écriture de générateurs, outillage gRPC/GraphQL) → territoire C# ; on délègue ou l'on s'en passe.
4. **Le bénéfice justifie-t-il le coût de la frontière ?** → on **ne délègue pas** un gain trivial ; la frontière a un prix.

> 💡 **L'heuristique unificatrice.** On délègue quand la fonctionnalité **(a)** apporte un bénéfice **réel** et **(b)** s'encapsule **proprement** derrière une frontière .NET — ou, tout simplement, quand elle est **hors de portée** de VB et qu'on en a besoin.

---

## Performance fine, `Span(Of T)` et `ref struct`

VB peut **consommer** certaines API fondées sur `Span`, mais la mécanique des `ref struct` y est **limitée** : le code zéro-allocation le plus efficace s'écrit en C# (sections 9.3 et 14.4). On délègue les **chemins chauds** : parsing sans allocation, manipulation de tampons, traitements intensifs.

> ⚠️ **Profiler avant de déléguer.** .NET 10 apporte beaucoup de performance **gratuitement** (améliorations JIT/PGO/SIMD, sans changer le code — section 14.6). Avant d'isoler quoi que ce soit pour la vitesse, **mesurez** (section 14.1) : seul un goulot d'étranglement **avéré** et sensible aux allocations justifie une bibliothèque C#. La délégation prématurée pour la performance est une erreur classique.

**Forme : brique consommée**, exposant une surface simple (tableaux, `IEnumerable`).

---

## Records, `init` et immuabilité avancée

VB ne **déclare** pas de `record` mais en **consomme** sans peine (sections 3.7 et 9.3). On délègue lorsqu'on veut de **vrais records** — égalité de valeur, concision des DTO immuables, expression `with` (pour les consommateurs **C#** du modèle ; côté VB, on prévoit des méthodes `WithXxx`, section 9.3) — au sein d'un **modèle ou d'un contrat partagé**.

À nuancer toutefois : VB **approche** l'immuabilité avec des propriétés `ReadOnly` et des classes (section 3.7). Pour un type immuable **purement interne** à du code VB, une bibliothèque C# n'est **pas nécessaire**. La délégation se justifie quand on veut spécifiquement la **sémantique record** ou quand le modèle est **déjà partagé** avec du code C#.

**Forme : brique consommée**, souvent la bibliothèque de **modèle / domaine** en C#.

---

## Générateurs de source (*source generators*)

Deux cas radicalement différents :

- **Consommer** une bibliothèque qui utilise des générateurs en interne → **aucune délégation** : VB consomme les **types compilés** qui en résultent, de façon transparente.
- **Écrire** un générateur → **C#-only**. Un générateur est un projet d'analyseur Roslyn, qui ne s'écrit pas en VB et **ne s'exécute pas sur du code VB**.

C'est pourquoi, par exemple, on utilise en VB les classes de base de `CommunityToolkit.Mvvm` (`ObservableObject`, `RelayCommand`) plutôt que ses attributs générateurs (section 6.6).

**Forme : un projet d'outillage C# séparé** ; sa **sortie**, elle, se consomme sans rien de particulier.

---

## Minimal APIs

Les Minimal APIs sont **possibles en VB**, mais **sans modèle de projet** et avec une **syntaxe contrainte** (pas d'instructions de niveau supérieur — section 8.3). Le chemin **réaliste et recommandé** pour une Web API en VB reste les **contrôleurs** (section 8.2) ✅.

On délègue donc à C# **uniquement** si l'on tient spécifiquement au **style Minimal API** (léger, *top-level*). Sinon, on reste en VB avec des contrôleurs — sans rien déléguer.

**Forme : surface séparée** (un service). Selon le cas, l'application VB le **consomme comme client HTTP** (chapitre 8) ou bien l'API **est** le livrable, écrit en C#.

---

## Native AOT

La compilation **Native AOT** n'est **pas prise en charge en pratique** pour VB (Annexe B). Elle produit un binaire natif, autonome, à démarrage rapide et faible empreinte — utile pour des outils en ligne de commande, des conteneurs, des scénarios *serverless*.

> ⚠️ **AOT s'applique à l'application *entière*, pas à une bibliothèque.** Native AOT est un **mode de publication** de l'exécutable. On **ne récupère pas** les bénéfices de l'AOT dans une application VB en « consommant » une bibliothèque C# AOT : l'avantage vaut pour **l'unité publiée tout entière**. Déléguer à l'AOT signifie donc « **cet exécutable / ce service est une application C# AOT à part entière** ».

À nuancer : l'AOT impose des **contraintes** (réflexion limitée, élagage) et n'a **aucune utilité** pour une application de bureau ou de gestion classique. On ne délègue à l'AOT que lorsqu'un **démarrage rapide** ou une **empreinte minimale** sont une **exigence réelle**.

**Forme : surface séparée** (un exécutable C# autonome).

---

## Blazor, .NET MAUI et WinUI 3 (front-ends modernes)

Ces interfaces n'ont **pas de modèle de projet VB** ; leur code d'UI est en C# (Razor génère du C# ; le XAML de MAUI/WinUI s'accompagne d'un *code-behind* C#) — Annexe B. On délègue lorsqu'on a besoin d'un **front-end web** (Blazor), d'une **UI multiplateforme** mobile/bureau (MAUI) ou d'une **UI Windows moderne** (WinUI 3).

C'est la délégation la plus mal comprise, car ce **n'est pas** une brique consommée : **c'est l'interface elle-même**, en C#. Deux conséquences :

- Pour le **bureau**, le foyer de VB reste **Windows Forms** ⭐ et **WPF** (modules 5 et 6). On ne va vers Blazor/MAUI/WinUI que si le **besoin** est web, multiplateforme ou UI Windows moderne.
- Le partage de code se fait alors **dans l'autre sens** : une **bibliothèque métier** (qui peut très bien rester **en VB.NET** ⭐) est **consommée par le front-end C#**, ou bien exposée derrière une API que ce front-end appelle.

**Forme : surface séparée** (l'UI, en C#).

---

## Et les autres sujets ?

D'autres *workloads* relèvent du même raisonnement et sont catalogués dans l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)** :

- **gRPC et GraphQL** — outillage orienté C# (section 8.5) ;
- **microservices / Dapr / Kubernetes** — écosystème majoritairement C# ;
- **service « Worker »** — pas de modèle de projet VB ; à câbler à la main via le Generic Host (section 4.8), ou à confier à C#.

---

## Tableau de synthèse

| Sujet | Statut en VB | Forme de délégation | Quand déléguer |
|-------|--------------|---------------------|----------------|
| Perf / `Span` / `ref struct` | Consommation limitée | **Brique consommée** | Chemin chaud **mesuré**, sensible aux allocations |
| Records / `init` | Consommable, non déclarable | **Brique consommée** (modèle partagé) | Besoin de sémantique record / contrat partagé |
| Générateurs de source (auteur) | C#-only | **Projet outillage C#** séparé | Écrire un générateur (sa sortie se consomme sans rien) |
| Minimal APIs | Possible mais contraint | **Surface séparée** (service) | Style Minimal API requis ; sinon **contrôleurs VB** ✅ |
| Native AOT | Non pris en charge | **Surface séparée** (exécutable C# entier) | Démarrage rapide / empreinte minimale **réellement** nécessaires |
| Blazor / MAUI / WinUI 3 | Pas de modèle VB | **Surface séparée** (l'UI, en C#) | Besoin web / multiplateforme / UI Windows moderne |

---

## ⚠️ Ne pas déléguer à tort

La délégation a un **coût** : une frontière à concevoir et à maintenir, une solution mixte à gérer (section 10.6), une charge cognitive. Quelques garde-fous :

- **Pas de délégation prématurée pour la performance** : mesurer d'abord ; .NET 10 offre beaucoup gratuitement.
- **Pas de délégation pour un gain marginal** : si VB fait déjà le travail correctement (immuabilité approchée, contrôleurs plutôt que Minimal API), rester en VB.
- **Vérifier la forme** : une **brique consommée** s'intègre via une référence (sections 10.3-10.4) ; une **surface séparée** est une décision d'architecture plus lourde (autre application, partage de code en sens inverse).
- **Concevoir la frontière VB-friendly** dès qu'il y a consommation (constructeurs explicites, pas de `ref struct` en surface publique, conformité CLS — section 9.3).

---

## En résumé

- Distinguer deux **formes** de délégation : la **brique consommée** (bibliothèque/composant C# appelé par VB) et la **surface séparée** (UI ou service en C#, hors processus).
- Quatre questions : est-ce **non déclarable mais consommable** ? **possible mais contraint** ? **hors périmètre** ? le **bénéfice justifie-t-il la frontière** ?
- Verdicts : `Span`/perf et records → **briques consommées** (mais **profiler** / vérifier que VB ne suffit pas déjà) ; Minimal APIs → **contrôleurs VB** par défaut ; Native AOT → **exécutable C# entier** (et l'AOT vaut pour toute l'application) ; Blazor/MAUI/WinUI → **l'UI elle-même, en C#**, avec partage de code **en sens inverse**.
- Ne pas déléguer **à tort** : ni prématurément, ni pour un gain marginal ; toujours pour un bénéfice réel et une frontière propre.

> 🔗 **Suite logique** : la section **10.3 — Isoler les fonctionnalités avancées dans des bibliothèques C#** passe de la décision à la **mise en œuvre** de la frontière.

⏭️ [Isoler les fonctionnalités avancées dans des bibliothèques C#](/10-hybride-vbnet-csharp/03-isoler-en-csharp.md)
