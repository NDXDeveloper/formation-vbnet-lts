🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 14.4 — `Span(Of T)` / `Memory(Of T)` (consommation), pooling

**Réduire les allocations sur les chemins critiques — et la frontière honnête de VB.NET**

---

## 🎯 Deux outils pour allouer moins

Les sections précédentes l'ont établi : moins d'allocations, c'est moins de pression sur le GC
(→ **[14.2](02-types-allocations.md)** et **[14.3](03-gc.md)**). Sur les **chemins critiques**
identifiés par la mesure (→ **[14.1](01-profilage.md)**), deux outils complémentaires permettent
de réduire ces allocations :

- **`Span(Of T)` / `Memory(Of T)`** — manipuler des tranches de mémoire **sans les copier**
  (au lieu d'allouer une sous-chaîne ou un sous-tableau, on en crée une **vue**).
- **Le *pooling* (`ArrayPool`, `ObjectPool`)** — **réutiliser** des tampons et objets coûteux
  plutôt que de les allouer puis les jeter.

Les deux relèvent du périmètre **« consommation »** de VB.NET — pleinement utilisables — avec une
**réserve essentielle** sur `Span`, qui est au cœur de cette section et illustre parfaitement la
stratégie hybride du cours.

---

## 🔍 `Span(Of T)` et `Memory(Of T)` : des vues sans allocation

`Span(Of T)` représente une **région contiguë de mémoire** — un segment de tableau, de chaîne, de
mémoire pile ou native — **sans en posséder ni en copier** le contenu. Découper (*slice*) une
zone via un `Span` **n'alloue rien** : on obtient une fenêtre sur des données déjà présentes.

La famille comprend quatre types :

| Type | Mutable | Traverse `Await` | Usage |
|------|:---:|:---:|-------|
| `Span(Of T)` | ✅ | ❌ | Vue en lecture/écriture, code **synchrone** |
| `ReadOnlySpan(Of T)` | ❌ | ❌ | Vue en lecture seule (ex. `ReadOnlySpan(Of Char)` sur une chaîne) |
| `Memory(Of T)` | ✅ | ✅ | Équivalent **stockable** et **asynchrone** de `Span` |
| `ReadOnlyMemory(Of T)` | ❌ | ✅ | Version lecture seule de `Memory` |

L'intérêt est direct : remplacer des allocations « invisibles » par des vues. L'exemple le plus
parlant est la sous-chaîne — mais il faut poser d'emblée la **règle VB.NET**, vérifiée sur
.NET 10 :

> ⚠️ **En VB.NET, un `Span` ne peut exister que *le temps d'une expression*.** Le compilateur VB
> **refuse toute déclaration** d'une variable locale, d'un paramètre ou d'un champ de type
> `Span(Of T)` / `ReadOnlySpan(Of T)` — erreur **BC30668** : *« Types with embedded references
> are not supported in this version of your compiler »*. En revanche, un `Span` **éphémère** —
> créé et consommé dans la même expression (argument d'appel, chaînage, indexation immédiate) —
> est parfaitement accepté. Toute la consommation VB de `Span` se joue dans cette nuance.

```vb
' ❌ Alloue une NOUVELLE chaîne à chaque appel
Dim prefixe As String = texte.Substring(0, 4)

' ❌ NE COMPILE PAS en VB : déclarer une variable Span est interdit (BC30668)
' Dim vue As ReadOnlySpan(Of Char) = texte.AsSpan(0, 4)

' ✅ Vue SANS allocation, consommée DANS l'expression (ici : un appel de méthode)
Dim premierEspace As Integer = texte.AsSpan(0, 4).IndexOf(" "c)
```

---

## ✅ Ce que VB.NET consomme — en expressions éphémères

Trois scénarios couvrent l'essentiel des gains réels, tous **vérifiés à la compilation et à
l'exécution** :

**1. Analyser (*parser*) directement depuis une vue**, sans chaîne intermédiaire — de nombreuses
API de la BCL exposent des surcharges acceptant `ReadOnlySpan(Of Char)`, que l'on appelle avec la
vue **en argument direct** :

```vb
' Aucune sous-chaîne allouée : la vue ne vit que le temps de l'appel
Dim valeur As Integer = Integer.Parse(ligne.AsSpan(8, 4))
```

**2. Les entrées/sorties par tampon**, où flux et sockets acceptent `Span` — même principe,
l'argument est construit dans l'appel :

```vb
Dim tampon(4095) As Byte
Dim lus As Integer = flux.Read(tampon.AsSpan())   ' Read(Span(Of Byte))
```

**3. Chercher et découper via les méthodes d'extension de `MemoryExtensions`**, en chaînant les
appels sans jamais stocker la vue :

```vb
Dim position As Integer = "abc,def".AsSpan().IndexOf(","c)
```

À l'inverse, **une méthode VB ne peut pas *recevoir* un `Span` en paramètre** : pour exposer une
API « à tranches » depuis du code VB, on prend un **tableau** (avec indice et longueur) ou un
**`Memory(Of T)`** — et l'on garde à l'esprit qu'une bibliothèque C# `Span`-first se consomme
depuis VB **uniquement** par ces appels éphémères.

> 💡 **Le gain le plus important est souvent *invisible et gratuit*.** Depuis plusieurs versions,
> la BCL elle-même a été **réécrite en interne** autour de `Span` : opérations sur les chaînes,
> *parsing*, formatage, E/S, parties de LINQ. Quand vous appelez `Integer.Parse`,
> `String.Split` (surcharges récentes) ou la sérialisation JSON, vous **bénéficiez de ces
> optimisations sans écrire la moindre ligne de `Span`** — un bénéfice que .NET 10 prolonge
> (→ **[14.6](06-apports-net10.md)**).

---

## ⚠️ La réserve « consommation » : `Span` est un *ref struct*

`Span(Of T)` est un **`ref struct`** : une structure **vivant uniquement sur la pile**. C# encadre
ces types par des règles de durée de vie ; **VB.NET, lui, n'a jamais reçu ce support de langage**
— d'où la règle vue plus haut, plus stricte qu'en C# :

- ❌ **Aucune déclaration de type `Span`** en VB : ni **variable locale**, ni **paramètre**, ni
  **champ**, ni **argument générique** (`List(Of Span(Of Byte))`) — partout, l'erreur **BC30668**.
  Là où C# restreint (pas de champ, pas d'`async`…), VB **interdit la déclaration elle-même**.
- ✅ Seules les **expressions éphémères** passent : argument d'appel, chaînage de méthodes,
  indexation immédiate (`memoire.Span(i)`).

À cela s'ajoutent **deux contraintes propres à VB.NET**, conséquences directes du langage figé :

- **Pas de `stackalloc`.** VB **n'a pas** le mot-clé `stackalloc` : on ne peut donc pas créer en
  VB un `Span` adossé à la pile (`stackalloc Byte(255)`), idiome fréquent en C# pour de petits
  tampons temporaires sans allocation. En VB, ces scénarios passent par un tableau (idéalement
  *poolé*, voir plus bas) — ou par C#.
- **Pas de déclaration de `ref struct`.** On ne **déclare** pas de nouveaux *ref structs* en VB
  (→ **[Annexe B.7](../annexes/frontiere-vbnet-csharp/README.md)**).

---

## 🔄 `Memory(Of T)` : le pendant asynchrone

Puisqu'un `Span` ne peut pas traverser un `Await`, le code **asynchrone** utilise `Memory(Of T)`
(qui n'est **pas** un *ref struct* et se stocke donc sans restriction : variable, champ,
asynchrone…). On conserve la `Memory` à travers l'attente, puis on accède aux données via sa
propriété `.Span` — **en expression**, fidèle à la règle VB :

```vb
Dim tampon(4095) As Byte
Dim memoire As Memory(Of Byte) = tampon.AsMemory()   ' Memory : déclaration autorisée

Dim lus As Integer = Await flux.ReadAsync(memoire)   ' ReadAsync(Memory(Of Byte))

' Accès aux octets lus, par indexation immédiate (pas de variable Span) :
Dim premier As Byte = memoire.Span(0)
```

`Memory(Of T)` est **pleinement utilisable en VB.NET** — c'est lui, et non `Span`, le « porteur »
de tranches que l'on stocke et transmet en VB —, y compris dans les E/S asynchrones
(→ **[module 7.6](../07-acces-donnees/06-fichiers-io.md)**).

---

## 🔗 La leçon hybride : consommer en VB, *écrire* en C# si besoin

Ce point résume toute la philosophie de la formation appliquée à la performance :

- **Consommer** des API optimisées avec `Span`/`Memory` est **gratuit et idéal** en VB.NET : vous
  récupérez le bénéfice des internes `Span` de la BCL **sans rien écrire**, et vous appelez sans
  difficulté les surcharges `Span`/`Memory` (*slicing*, *parsing*, E/S par tampon).
- **Écrire** un chemin critique **intensivement** fondé sur `Span` — analyseur haute performance,
  manipulation fine de tampons, boucle adossée à la pile, code proche du SIMD — bute sur les
  limites ci-dessus (pas de `stackalloc`, pas de *ref struct* maison, usage restreint).

La réponse pragmatique est alors la **stratégie hybride** : isoler ce *hot path* dans une
**bibliothèque C#** et la **consommer** depuis VB.NET, qui garde l'UI et le métier
(→ **[module 10](../10-hybride-vbnet-csharp/README.md)** 🔗). L'immense majorité du code VB n'en a
toutefois **pas besoin** : la consommation directe suffit.

---

## ♻️ Le *pooling* : réutiliser plutôt que réallouer

Le *pooling* attaque le même problème sous un autre angle : au lieu d'**allouer puis jeter**, on
**emprunte** un objet à un réservoir, on s'en sert, puis on le **rend**. On supprime ainsi les
allocations répétées — particulièrement précieux pour les **grands tampons** (qui finissent sur le
**LOH** et le fragmentent, → 14.3) et pour les motifs d'allocation **à très haute fréquence**.

### `ArrayPool(Of T)` — réutiliser des tableaux

`System.Buffers.ArrayPool(Of T)` fournit un réservoir de tableaux réutilisables. On emprunte avec
`Rent`, on rend avec `Return` — impérativement dans un `Try…Finally`, pour garantir le retour
**même en cas d'exception** (la même rigueur que pour `Dispose`, → 14.3) :

```vb
Dim pool = ArrayPool(Of Byte).Shared
Dim tampon = pool.Rent(taille)     ' tampon.Length >= taille (PEUT être plus grand !)
Try
    ' N'utiliser QUE tampon(0) .. tampon(taille - 1)
    Dim lus = flux.Read(tampon, 0, taille)
    ' ... traitement ...
Finally
    pool.Return(tampon, clearArray:=True)   ' retour garanti ; effacement si données sensibles
End Try
```

Trois règles **non négociables** :

- **La taille empruntée peut être supérieure** à la taille demandée. On **suit soi-même** la
  longueur logique ; on n'utilise jamais `tampon.Length` comme s'il valait exactement `taille`.
- **Ne plus jamais utiliser le tableau après `Return`** (il peut être réattribué à un autre code).
- **`clearArray:=True`** pour effacer les données **sensibles** avant de rendre le tampon (sinon
  elles resteraient lisibles par le prochain emprunteur).

### `ObjectPool(Of T)` — réutiliser des objets coûteux

`Microsoft.Extensions.ObjectPool` (paquet NuGet du même nom) met en pool des **objets de type
référence** coûteux à construire ou à réinitialiser. Le cas d'école est le `StringBuilder`, dont
le tampon interne est ainsi recyclé :

```vb
' Imports Microsoft.Extensions.ObjectPool
Dim fournisseur As ObjectPoolProvider = New DefaultObjectPoolProvider()
Dim pool As ObjectPool(Of StringBuilder) = fournisseur.CreateStringBuilderPool()

Dim sb = pool.Get()
Try
    sb.Append("Bonjour ").Append(nom)
    Dim message = sb.ToString()
    ' ...
Finally
    pool.Return(sb)     ' l'objet est réinitialisé puis remis à disposition
End Try
```

À retenir : l'objet rendu est **réinitialisé** (par la politique du pool) pour ne pas **fuiter
d'état** entre deux usages — un objet *poolé* qui conserverait des données d'un emprunt précédent
serait une source de bugs subtils.

---

## 🧭 Quand *pooler* — et quand s'en abstenir

Le *pooling* **ajoute de la complexité et un risque de correction** (rendre un objet encore
utilisé, oublier de le rendre — une fuite —, le rendre deux fois). Il ne se justifie donc qu'**à
bon escient** :

- ✅ **Pooler** quand le profilage (→ 14.1) prouve une pression d'allocation due à des **grands
  tampons** fréquents (≥ seuil LOH) ou à une allocation **très répétée** du même type d'objet.
- ❌ **Ne pas pooler par réflexe.** Pour l'immense majorité du code, laisser la **Gen 0** faire son
  travail est plus simple, plus sûr, et amplement suffisant. L'optimisation prématurée vaut ici
  comme ailleurs.

Quelques garde-fous, si l'on poole :

- **Toujours rendre** dans un `Try…Finally`.
- **Ne plus utiliser** l'objet après l'avoir rendu.
- **Tableau emprunté potentiellement plus grand** que demandé : suivre la longueur réelle.
- **Effacer** les données sensibles au retour (`clearArray:=True`).
- Les pools partagés sont **thread-safe** pour `Rent`/`Return`, mais l'objet emprunté **n'est qu'à
  vous** jusqu'à son retour.

---

## 🔁 En résumé

- **`Span`/`Memory`** offrent des **vues sans allocation** sur de la mémoire existante. En VB.NET,
  `Span` ne vit qu'**en expression éphémère** (argument d'appel, chaînage, indexation immédiate —
  jamais en variable, paramètre ou champ : BC30668) ; **`Memory(Of T)`**, lui, se déclare et se
  transmet **sans restriction** — c'est le porteur de tranches de VB, asynchrone compris.
- Le plus grand bénéfice de `Span` en VB est souvent **gratuit** : la BCL l'utilise déjà en
  interne (→ 14.6).
- La réserve tient à la nature **`ref struct`** de `Span`, dont VB n'a jamais reçu le support de
  langage — s'y ajoutent **pas de `stackalloc`** ni de *ref struct* déclarable. Les chemins
  **intensivement** `Span` relèvent alors de la **bibliothèque C#** consommée depuis VB
  (stratégie hybride, → module 10).
- Le ***pooling*** (`ArrayPool`, `ObjectPool`) **réutilise** tampons et objets coûteux pour
  soulager le GC — à réserver, **après mesure**, aux grands tampons fréquents et aux allocations à
  haute fréquence, avec une discipline stricte de retour.

La **[14.5](05-bonnes-pratiques.md)** consolide ces principes en bonnes pratiques de performance
idiomatiques en VB.NET, et la **[14.6](06-apports-net10.md)** détaille tout ce que .NET 10 apporte
**sans changer le code**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Bonnes pratiques de performance en VB.NET](/14-performance/05-bonnes-pratiques.md)
