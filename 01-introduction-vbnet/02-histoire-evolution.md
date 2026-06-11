🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 1.2 De Visual Basic 6 à VB.NET (histoire et héritage legacy)

Pourquoi un détour par l'histoire dans une formation pratique ? Pour deux raisons très
concrètes. D'abord parce qu'une **confusion tenace** entoure ce langage — entre Visual
Basic 6 et VB.NET — et qu'elle ressurgit en permanence, notamment face aux assistants IA.
Ensuite parce que cette formation s'adresse en bonne partie à des équipes qui
**maintiennent ou migrent du legacy** : comprendre d'où vient ce code, c'est mieux savoir
quoi en faire.

## Aux origines : Visual Basic « classique » (1991-1998)

L'aventure commence en **1991** avec **Visual Basic 1.0**. À une époque où programmer une
interface Windows relevait de l'expertise, VB introduit une idée révolutionnaire : le
développement **visuel** et **piloté par les événements** (RAD, *Rapid Application
Development*). On dessine un formulaire en glissant-déposant des contrôles, on double-clique
sur un bouton, on écrit le code de l'événement — et l'application tourne. Cette simplicité
ouvre la programmation Windows à un public immense.

La lignée culmine en **1998** avec **Visual Basic 6.0**, point d'orgue du « VB classique ».
VB6 s'appuie sur le **modèle COM** (*Component Object Model*, la technologie historique de
composants logiciels de Windows) et son écosystème de contrôles **ActiveX** ; c'est aussi
la dernière version capable de produire des programmes **natifs Win32**. Son succès est
colossal : des millions de développeurs, et un nombre considérable d'applications métier
qui, pour beaucoup, **tournent encore aujourd'hui**.

> 📌 **Terminologie.** Les versions jusqu'à VB6 incluse sont rétrospectivement appelées
> **Visual Basic Classic** (ou « VB classique »), par opposition à **Visual Basic .NET**
> (VB.NET), bâti sur .NET. Garder cette distinction en tête évite bien des malentendus.

## VB6 : un succès massif, une dette qui dure

Si VB6 a dominé, c'est par sa **productivité** et son **accessibilité**. Mais, en tant que
langage, il montrait ses limites : une orientation objet **incomplète** (des classes et des
interfaces, mais **pas d'héritage d'implémentation**), un couplage étroit à COM et un modèle
d'exécution daté.

Côté support, la situation est aujourd'hui **paradoxale** — et c'est précisément ce qui
explique la persistance du legacy :

- l'**IDE et le développement VB6 ne sont plus supportés depuis le 8 avril 2008** : il
  n'existe plus de moyen officiel de *créer* ou de *maintenir* une application VB6, et
  Microsoft recommande de **remplacer** ces applications par une technologie moderne ;
- en revanche, le **runtime VB6** bénéficie d'un engagement de compatibilité « It Just
  Works » et reste **pris en charge pour toute la durée de vie des versions de Windows
  supportées** (jusqu'à Windows 11 et Windows Server 2025 inclus — en 32 bits uniquement,
  via l'émulation WOW64 sur les systèmes 64 bits).

> ⚠️ **Le paradoxe du legacy VB6.** Les applications VB6 **continuent de fonctionner** sur
> Windows (le runtime est supporté), mais on ne peut plus les développer dans des conditions
> supportées. Résultat : un parc énorme reste en production… tout en étant « en sursis ».
> C'est tout l'enjeu de la migration (→ 11.2).

## La rupture : Visual Basic .NET (2002)

En **2002**, avec **.NET Framework 1.0** et **Visual Studio .NET 2002**, Microsoft
introduit **Visual Basic .NET**. Et c'est ici qu'il faut être précis : VB.NET **n'est pas
une nouvelle version de VB6**. C'est un **langage différent**, sur une **plateforme
différente** (.NET, le CLR), dont il ne partage que la **philosophie** et une **syntaxe de
surface** — ni le runtime, ni la sémantique.

Les changements sont profonds :

- une **vraie orientation objet** : héritage, interfaces, polymorphisme ;
- un **typage plus rigoureux** (avec `Option Strict`, `Option Explicit`) ;
- une **gestion mémoire** confiée au ramasse-miettes (GC) ;
- l'**abandon ou la transformation** de nombreuses constructions VB6.

Ces transformations rendaient la migration **tout sauf triviale**. Quelques pièges
emblématiques : le type `Integer` passe de **16 bits** (VB6) à **32 bits** (VB.NET), le
`Variant` disparaît au profit d'`Object`, les tableaux deviennent indexés à partir de zéro,
et des instructions comme `GoSub`, `Set`/`Let` ou les propriétés par défaut sans paramètre
sont supprimées. Des outils de conversion automatique (l'*Upgrade Wizard*) existaient, mais
leurs résultats étaient **imparfaits** (→ 11.2 pour le détail).

Cette rupture a provoqué une véritable **controverse** : une partie de la communauté du VB
classique s'est sentie abandonnée — d'où le fameux « VB.NET is not VB ». C'est aussi de là
que vient la **confusion durable** entre les deux langages, qu'il faut savoir dissiper —
plus que jamais en 2026 face à l'IA (→ module 17).

## L'évolution de VB.NET (2002 → aujourd'hui)

Pendant une quinzaine d'années, VB.NET et C# ont **évolué de concert**, à quasi-parité de
fonctionnalités : VB.NET a reçu LINQ (2008), `Async`/`Await` (2012) et l'essentiel des
grandes nouveautés de la plateforme. Puis la trajectoire a changé.

| Année | Jalon |
|------|-------|
| **1991** | Visual Basic 1.0 — début de l'ère VB |
| **1998** | Visual Basic 6.0 — apogée du VB « classique » (dernier natif Win32) |
| **2002** | Visual Basic .NET — rupture : nouveau langage sur la plateforme .NET |
| **2005** | VB 8.0 (.NET 2.0) : génériques et espace `My` — des marqueurs identitaires de VB.NET |
| **2008** | Fin du support de l'IDE et du développement VB6 (8 avril) |
| **2008 / 2012** | VB.NET à parité avec C# : LINQ (2008), `Async`/`Await` (2012) |
| **2017** | Nouvelle stratégie de langage : C# mène, VB se recentre |
| **2020** | Bascule « consommation seule » — le langage est **stabilisé** |
| **2025** | VB.NET sur .NET 10 LTS ; version du langage **17.13** (consumption-only) |

En **2017**, la mise à jour de la stratégie des langages .NET annonce que C# portera
désormais l'innovation, VB se concentrant sur ses scénarios cœur. En **2020**, le
basculement est acté : VB ne sera **pas étendu à de nouveaux workloads** et adopte une
approche de **consommation seule**. C'est l'état actuel, détaillé en 1.1 : un langage
**stabilisé**, dont la version courante (VB 17.13, inchangée depuis début 2025) n'évolue
plus que par de rares incréments orientés interopérabilité, sans nouvelle syntaxe à écrire.

## L'héritage legacy : deux strates à distinguer

« Legacy VB » recouvre en réalité **deux réalités bien différentes**, à ne pas confondre
lorsqu'on évalue un existant :

- **Le legacy VB6** (COM, hors .NET). Des applications encore en production, dont le runtime
  est supporté sur Windows mais sans chemin de développement officiel. La voie est la
  **migration** vers VB.NET ou C# (→ 11.2), aujourd'hui souvent **assistée par IA**
  (→ 17.3).
- **Le legacy VB.NET sur .NET Framework**. Des applications VB.NET bâties sur le
  .NET Framework (versions 1.x à 4.x), qu'il s'agit de faire **coexister** avec le
  .NET moderne ou de **migrer** vers .NET 10 (→ 11.3 et 11.5). Cas particulièrement épineux : **ASP.NET Web Forms** en VB,
  qui **n'a aucun chemin de migration** vers le .NET moderne (→ 11.4).

Dans les deux cas, la raison de cette persistance est celle déjà évoquée en 1.1 : ces
logiciels sont souvent **critiques pour l'activité**, ils **fonctionnent**, et leur
réécriture intégrale représente un **coût et un risque** considérables. Le maintien et la
migration progressive sont donc des compétences de premier plan — et une bonne part de la
raison d'être de cette formation.

## Pourquoi cette histoire compte, en 2026

Trois enseignements concrets à retenir de ce parcours :

- **Ne jamais confondre VB6 et VB.NET.** Noms proches, syntaxe de surface familière, mais
  langages et plateformes distincts. La distinction est vitale dans les recherches, la
  documentation — et surtout face aux assistants IA, qui mélangent volontiers les deux
  (→ module 17).
- **Mesurer la dette technique.** Comprendre la *nature* du legacy (VB6/COM vs VB.NET sur
  Framework) conditionne toute stratégie de maintenance ou de migration.
- **Comprendre la stabilisation.** Le statut « figé » de VB.NET en 2026 n'est pas un
  accident : c'est l'aboutissement logique de cette trajectoire de vingt-cinq ans.

---

Nous savons désormais **ce qu'est** VB.NET (1.1) et **d'où il vient** (1.2). La section
suivante cartographie le terrain sur lequel il s'exécute : l'**écosystème .NET**, du
.NET Framework legacy au .NET 10 moderne. (→ 1.3)

⏭️ [L'écosystème .NET](/01-introduction-vbnet/03-ecosysteme-dotnet.md)
