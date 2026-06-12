🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 14.5 — Bonnes pratiques de performance en VB.NET

**Distiller les sections précédentes en habitudes concrètes — et savoir où s'arrête le langage**

---

## 🎯 De la mécanique aux réflexes

Les sections 14.1 à 14.4 ont posé la mécanique : mesurer, comprendre les allocations, le GC,
les vues sans copie et le *pooling*. Cette section les **condense en pratiques actionnables**,
avec une attention particulière aux points **spécifiques à VB.NET** — ceux que les exemples C#
omniprésents ne couvrent pas.

Avant tout, **la règle cardinale**, qui prime sur toutes les autres :

> 📏 **Mesurer d'abord.** On n'optimise pas une intuition (→ **[14.1](01-profilage.md)**). On
> profile en `Release`, sur une charge représentative, on établit une *baseline*, on change **une**
> chose, puis on **re-mesure**. La majorité des « optimisations » appliquées sans preuve
> n'apportent rien — ou dégradent la lisibilité pour un gain nul.

Et le rappel de hiérarchie : le **bon algorithme** et la **bonne structure de données** dépassent
de loin tout micro-réglage. C'est le premier levier, et il est entièrement à la portée de VB.NET.

---

## ⚙️ Les options de compilation VB — le levier propre au langage

C'est **le** réglage de performance spécifique à VB.NET, et il est à la fois gratuit et décisif.

### `Option Strict On`, partout

Avec **`Option Strict Off`**, VB autorise la **liaison tardive** (*late binding*) : un appel de
membre sur un `Object` est résolu **à l'exécution**, par réflexion, via le moteur de liaison
tardive du runtime VB. C'est **des ordres de grandeur plus lent** qu'un appel **anticipé**
(*early binding*), compilé directement — sans parler des erreurs de type repoussées à l'exécution.

```vb
' ❌ Option Strict Off : appel résolu à l'exécution (réflexion) — lent
Dim o As Object = service
o.Traiter(donnees)          ' liaison tardive

' ✅ Option Strict On : type connu, appel compilé directement — rapide et sûr
Dim s As IService = service
s.Traiter(donnees)          ' liaison anticipée
```

La recommandation est claire : **`Option Strict On` au niveau du projet**, et on ne le relâche
**localement** (en-tête de fichier) que là où la liaison tardive est **réellement nécessaire** —
typiquement l'**automation COM/Office** en *late binding* (→ **[module 9.2](../09-interoperabilite/02-com-office.md)**)
ou de rares scénarios dynamiques. Jamais sur un chemin chaud.

```xml
<!-- Dans le .vbproj : à l'échelle du projet -->
<PropertyGroup>
  <OptionStrict>On</OptionStrict>
  <OptionExplicit>On</OptionExplicit>
  <OptionInfer>On</OptionInfer>
</PropertyGroup>
```

`Option Explicit On` (déclaration obligatoire) et `Option Infer On` (inférence locale) complètent
de bonnes valeurs par défaut (→ **[module 2.1](../02-fondamentaux-langage/01-structure-options.md)**).

---

## 🗃️ Choisir les bonnes structures de données

- **Génériques, toujours.** `List(Of T)` et `Dictionary(Of TKey, TValue)` plutôt que `ArrayList`
  ou `Hashtable` : aucun **boxing** des types valeur (→ **[14.2](02-types-allocations.md)**,
  **[2.8](../02-fondamentaux-langage/08-tableaux-collections.md)**).
- **La collection adaptée à l'accès.** `Dictionary`/`HashSet` pour les recherches et tests
  d'appartenance en **O(1)**, plutôt qu'un parcours linéaire **O(n)** d'une `List`. `Queue`/`Stack`
  pour FIFO/LIFO.
- **Pré-dimensionner quand la taille est connue.** Indiquer la capacité initiale évite les
  réallocations et recopies internes au fil de la croissance :

```vb
' ✅ Une seule allocation interne au lieu de plusieurs doublements successifs
Dim lignes As New List(Of String)(capacity:=nombreAttendu)
Dim sb As New System.Text.StringBuilder(capacity:=4096)
```

---

## 🔤 Manipuler les chaînes efficacement

- **`StringBuilder` pour concaténer en boucle** (→ **[14.2](02-types-allocations.md)**,
  **[2.6](../02-fondamentaux-langage/06-chaines.md)**) — pré-dimensionné si la taille est connue.
- **Comparer en `Ordinal`** pour les comparaisons **non linguistiques** (clés, identifiants,
  chemins) : plus rapide (pas de tables de culture) **et** plus prévisible. Éviter le
  `ToLower`/`ToUpper` pour comparer sans casse — il alloue et dépend de la culture :

```vb
' ❌ Alloue deux chaînes + sensible à la culture
If nom.ToLower() = cible.ToLower() Then ...

' ✅ Aucune allocation, comparaison explicite
If String.Equals(nom, cible, StringComparison.OrdinalIgnoreCase) Then ...
```

- **`AsSpan` plutôt que `Substring`** lorsqu'on consomme des API acceptant `ReadOnlySpan(Of Char)`
  (→ **[14.4](04-span-pooling.md)**).
- **Attention à l'interpolation en boucle chaude.** Rappel de la 14.2 : en VB, `$"..."` **boxe**
  les types valeur insérés. Négligeable en général, mesurable dans une boucle produisant beaucoup
  de chaînes — où l'on préfère `StringBuilder` ou un format explicite.

---

## 🕳️ Traquer les allocations cachées

Rappels de la 14.2, en mode réflexe :

- **Éviter le boxing** (génériques, pas d'`Object` superflu).
- **Sortir les allocations invariantes des boucles** (un `New`, un tampon, un délégué créés une
  fois hors de la boucle plutôt qu'à chaque itération).
- **Lambdas capturantes en boucle chaude** : elles allouent une fermeture. À surveiller — pas à
  bannir.
- **LINQ : clarté d'abord.** LINQ est un **point fort** de VB.NET (→ **[2.9](../02-fondamentaux-langage/09-linq.md)**)
  et reste le bon choix par défaut ; on ne le réécrit en boucle explicite **que** sur un chemin
  critique avéré par le profilage. On n'optimise pas LINQ « par principe ».

---

## 🧹 Libérer les ressources (sans en faire trop)

Rappels de la 14.3 :

- **`Using` pour tout `IDisposable`** (flux, connexions, commandes…) — libération garantie même en
  cas d'exception.
- **Pas de finaliseur** sauf détention **directe** d'une ressource non managée ; sinon, **filet
  inutile** qui prolonge la durée de vie de l'objet. Préférer **`SafeHandle`**.
- **Résilier les abonnements aux événements** (`RemoveHandler`) pour éviter les **rétentions**
  involontaires — fuite classique en code managé (→ **[3.6](../03-poo/06-evenements-delegues.md)**).

---

## ⏳ Asynchronie : ne jamais bloquer

L'asynchronie correcte est l'un des plus grands leviers de **réactivité** (interface) et de
**débit** (services). Les écueils à éviter (→ **[module 4](../04-async/README.md)**) :

- **Ne pas bloquer sur une tâche.** `.Result`, `.Wait()` ou `.GetAwaiter().GetResult()` figent le
  thread et provoquent des **interblocages** classiques dans les applications à interface
  (WinForms/WPF). On **attend** avec `Await`.

```vb
' ❌ Bloque le thread → risque de deadlock sous contexte UI
Dim donnees = ObtenirAsync().Result

' ✅ Libère le thread, reprend proprement ensuite
Dim donnees = Await ObtenirAsync()
```

- **`ConfigureAwait(False)` dans le code de *bibliothèque***, où l'on n'a pas besoin de revenir sur
  le thread d'interface — léger gain et évitement de deadlock. À l'inverse, dans le **code
  d'interface** d'une application de bureau, on **laisse** la reprise sur le thread UI (pas de
  `ConfigureAwait(False)`) pour pouvoir toucher les contrôles après l'attente.
- **`Async Sub` réservé aux gestionnaires d'événements.** Partout ailleurs, retourner une `Task`
  (`Async Function … As Task`) : un `Async Sub` ne peut être attendu et ses exceptions peuvent
  faire tomber le processus.
- Sur les chemins asynchrones **très** sollicités, `ValueTask` peut réduire les allocations
  (notion, → **[4.6](../04-async/06-async-streams.md)**).

---

## 🚀 Laisser la plateforme travailler

Souvent le meilleur rapport effort/gain :

- **Cibler .NET 10 et recompiler** suffit à accélérer le code sans le modifier (JIT, Dynamic PGO,
  SIMD, dévirtualisation) → **[14.6](06-apports-net10.md)**.
- **Utiliser les API de la BCL** plutôt que de réimplémenter : beaucoup sont déjà optimisées en
  interne autour de `Span` (→ 14.4).
- **Ne pas combattre le runtime** : pas de `GC.Collect()` manuel (→ 14.3).

---

## 🔗 Le bon langage pour le bon scénario

La frontière VB/C#, appliquée à la performance, tient en deux énoncés complémentaires :

- **Ne pas changer de langage pour une différence qui n'existe pas.** À code équivalent, VB.NET et
  C# ont des performances **identiques** (même IL, même runtime). Réécrire en C# « pour la vitesse »
  un code métier ordinaire est une perte de temps.
- **Déléguer les *hot paths* bas niveau à C#.** Pour un chemin critique avéré qui exige du code
  **intensivement** `Span`, du `stackalloc`, du SIMD écrit à la main ou un *ref struct* maison —
  toutes choses hors de portée du VB (→ 14.4) — on isole la brique dans une **bibliothèque C#** et
  on la **consomme** depuis VB.NET (→ **[module 10](../10-hybride-vbnet-csharp/README.md)** 🔗,
  **[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**). VB garde l'UI et le métier.

---

## ⚠️ Pièges spécifiques à VB.NET

Au-delà des allocations cachées déjà vues, quelques héritages du langage à surveiller :

- **La liaison tardive en chemin chaud** (`Option Strict Off`) — le piège n°1, détaillé plus haut.
- **L'interpolation `$"..."` en boucle** — boxing des valeurs (14.2).
- **`On Error` / `Resume`** (gestion d'erreurs façon VB6) plutôt que `Try`/`Catch`/`Finally`. Le
  style structuré est plus clair, mieux optimisable, et `On Error Resume Next` **avale
  silencieusement** les erreurs — à proscrire (→ **[12.1](../12-exceptions-debogage/01-exceptions.md)**).
- **Les fonctions de chaîne héritées** (`Left`, `Right`, `Mid`, `Len`, `UCase`…) passent par le
  runtime `Microsoft.VisualBasic` et traitent `Nothing` différemment des membres de `String`.
  Préférer les **membres .NET** (`Substring`, `Length`, `ToUpper`…) : comportement prévisible,
  cohérence avec l'écosystème — et meilleure adéquation avec les exemples et l'**assistance IA**
  (→ **[module 17](../17-developpement-ia/README.md)** 🤖). Le gain de performance est marginal ;
  c'est surtout un gain de **clarté et de prévisibilité**.

---

## ✅ Checklist récapitulative

- [ ] **Mesurer** avant d'optimiser (profil en `Release`, *baseline*, re-mesure).
- [ ] **Algorithme et structure de données** d'abord ; micro-optimisations en dernier.
- [ ] **`Option Strict On`** sur tout le projet ; liaison tardive isolée et justifiée.
- [ ] **Collections génériques**, **pré-dimensionnées** quand la taille est connue.
- [ ] **`StringBuilder`** en boucle ; comparaisons **`Ordinal`** ; `AsSpan` quand pertinent.
- [ ] **Pas d'allocations** invariantes dans les boucles ; vigilance boxing/fermetures.
- [ ] **LINQ pour la clarté** ; réécriture seulement sur *hot path* prouvé.
- [ ] **`Using`** pour tout `IDisposable` ; **abonnements résiliés** ; finaliseurs évités.
- [ ] **`Await`**, jamais `.Result`/`.Wait()` ; `ConfigureAwait(False)` en bibliothèque.
- [ ] **Cibler .NET 10** et s'appuyer sur la BCL ; pas de `GC.Collect()`.
- [ ] ***Hot paths* bas niveau** délégués à une **bibliothèque C#** (stratégie hybride).
- [ ] **Pas de `On Error`** ni de fonctions de chaîne héritées : style .NET structuré.

---

## 🔁 En résumé

La performance en VB.NET tient moins à des astuces qu'à des **réflexes disciplinés** : mesurer,
choisir le bon algorithme, activer `Option Strict On`, allouer moins, libérer proprement, ne pas
bloquer en asynchrone, et **laisser .NET 10 faire son travail**. Le seul véritable point où VB
cède la main, c'est le **bas niveau intensif** — que l'on confie à C# tout en gardant le reste en
VB.NET.

Reste à détailler ces gains « offerts par la plateforme » : c'est l'objet de la
**[14.6 — Ce que .NET 10 apporte gratuitement](06-apports-net10.md)**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Ce que .NET 10 apporte gratuitement (JIT/PGO/SIMD/dévirtualisation, sans changer le code)](/14-performance/06-apports-net10.md)
