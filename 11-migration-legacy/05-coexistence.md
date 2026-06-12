🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11.5 — Coexistence .NET Framework / .NET moderne, `.NET Standard`

> Une migration est rarement instantanée. L'approche incrémentale — la norme recommandée ([§11.1](01-evaluer-strategies.md)) — implique au contraire une période, parfois longue, où l'ancien monde (.NET Framework) et le nouveau (.NET 10) doivent **cohabiter**. Cette section décrit les mécanismes qui rendent cette coexistence possible, avec **`.NET Standard`** comme pont central — celui auquel renvoyaient déjà les sections précédentes.

---

## 1. Pourquoi la coexistence est la norme, pas l'exception

Migrer par morceaux (pattern *Strangler Fig*, → [§11.1](01-evaluer-strategies.md)) suppose mécaniquement une phase où les deux plateformes vivent côte à côte. Il faut donc d'abord clarifier ce qui peut, ou non, se partager :

- **Un même processus ne charge pas deux runtimes.** Une application .NET Framework et une application .NET 10 sont des processus (ou des applications) **distincts** : on ne « mélange » pas les deux dans un seul exécutable.
- **En revanche, on partage du code** via une bibliothèque commune que les deux mondes consomment, **des données** via une base partagée, et — pour le web — **la session et l'authentification** via des adaptateurs.

L'ennemi à éviter est la **duplication de la logique métier** entre l'ancien et le nouveau. Toute la coexistence vise à maintenir une **source unique de vérité** le temps de la transition.

---

## 2. `.NET Standard` : le pont entre les deux mondes

### 2.1 Ce qu'est `.NET Standard`

`.NET Standard` n'est **pas un runtime**, mais une **spécification** : un ensemble d'API que toute implémentation de .NET s'engage à fournir. Une bibliothèque ciblant `.NET Standard` est donc consommable, **sans recompilation**, par .NET Framework, .NET Core / .NET 5–10, Mono, etc. C'est un contrat de compatibilité.

### 2.2 `.NET Standard 2.0` : la version pivot ⭐

Pour la coexistence avec le .NET Framework, **une seule version compte vraiment : la 2.0**. Elle est consommable à la fois par le .NET Framework et par .NET moderne :

| `.NET Standard` | .NET Framework | .NET Core / .NET 5–10 |
|---|---|---|
| **2.0** ⭐ | ✅ 4.6.1+ (**4.7.2+ recommandé** en pratique) | ✅ 2.0+ / 5 à 10 |
| **2.1** ⚠️ | ❌ **Non pris en charge** | ✅ 3.0+ / 5 à 10 |

Une bibliothèque en `.NET Standard 2.0`, c'est **un seul assembly** que votre application Framework héritée **et** votre nouvelle application .NET 10 référencent toutes les deux.

### 2.3 `.NET Standard 2.1` : le piège ⚠️

La version 2.1 **n'est pas prise en charge par le .NET Framework** (seulement .NET Core 3.0+, Mono, Xamarin). La cibler **coupe le pont** vers l'ancien monde. Tant que la coexistence avec .NET Framework est requise, il faut donc **impérativement rester en 2.0**.

### 2.4 `.NET Standard` est en maintenance — mais reste l'outil adapté à *ce* cas

Microsoft ne publie plus de nouvelles versions de `.NET Standard`. Pour du **code neuf**, la recommandation officielle est de cibler directement les TFM (`net10.0`) ou de recourir au multi-ciblage. Mais pour le scénario **précis** qui nous occupe — une bibliothèque devant servir à la fois .NET Framework et .NET moderne —, **`.NET Standard 2.0` reste exactement le bon outil**. Il ne faut simplement pas le dégainer par habitude pour du code qui n'a aucun besoin de compatibilité Framework.

---

## 3. La bibliothèque partagée : le pattern central

Le mécanisme de coexistence le plus important est simple à énoncer : **extraire la logique métier, l'accès aux données, les modèles et les interfaces dans une bibliothèque `.NET Standard 2.0`** que les deux applications référencent.

Quelques principes pour que ce pont tienne :

- **Garder la bibliothèque « pure »** : pas d'interface graphique, pas de dépendance spécifique à une plateforme. L'UI et les spécificités de plateforme restent dans **chaque hôte** (l'application Framework d'un côté, l'application .NET 10 de l'autre).
- **Abstraire les particularités via des interfaces** (injection de dépendances) : la bibliothèque définit le contrat, chaque hôte fournit son implémentation.
- C'est le **socle de la migration incrémentale** (*Strangler Fig*), évoqué tout au long des [§11.1](01-evaluer-strategies.md), [§11.2](02-vb6-vers-vbnet.md) et [§11.4](04-web-forms-legacy.md), et la fondation naturelle d'une architecture hybride VB.NET / C# (→ module [10](../10-hybride-vbnet-csharp/README.md)).

> 💡 **Note VB.** Une bibliothèque `.NET Standard 2.0` écrite en VB.NET fonctionne parfaitement. L'espace de noms `Microsoft.VisualBasic` y est disponible ; en revanche, `Microsoft.VisualBasic.Compatibility` ne l'est pas (→ [§11.2](02-vb6-vers-vbnet.md)).

---

## 4. Le multi-ciblage : un projet, plusieurs cibles

Quand la bibliothèque a besoin de **chemins de code spécifiques** à chaque plateforme — au-delà de ce que le sous-ensemble commun de `.NET Standard` sait exprimer —, on recourt au **multi-ciblage**. Un projet *SDK-style* déclare alors plusieurs TFM à la fois :

```xml
<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>
```

À partir d'**une seule base de code**, le projet produit **plusieurs assemblys** (un par cible). Le code commun reste partagé ; les portions divergentes sont isolées par **compilation conditionnelle** — qui, en VB.NET, s'écrit avec `#If` (et non le `#if` du C#) :

```vb
#If NET48 Then
    ' Chemin spécifique au .NET Framework
#Else
    ' Chemin spécifique à .NET moderne
#End If
```

**Quand préférer le multi-ciblage à `.NET Standard` ?** Lorsqu'on a besoin d'API présentes sur chaque cible mais **absentes du sous-ensemble `.NET Standard 2.0`**, ou de comportements propres à une plateforme. `.NET Standard` reste le choix le plus simple tant que le code se contente de la surface commune ; le multi-ciblage prend le relais dès qu'il faut diverger.

---

## 5. Faire coexister les applications, pas seulement le code

Le partage de code (`.NET Standard`) est un premier axe ; faire **tourner les applications côte à côte** en est un second.

- **Cas du web (Web Forms → ASP.NET Core)** : un reverse proxy **YARP** place l'ancienne application Framework et la nouvelle application ASP.NET Core derrière un même point d'entrée, et les **System.Web Adapters** (`Microsoft.AspNetCore.SystemWebAdapters`) partagent **session et authentification** entre les deux, route par route (→ [§11.4](04-web-forms-legacy.md)).
- **Données** : les deux applications pointent vers la **même base**. Attention alors à la concurrence et à la cohérence de schéma (→ module [7](../07-acces-donnees/README.md)).

Rappel : les deux applications s'exécutent sur **leur propre runtime** (processus séparés). Elles ne partagent pas un processus — elles partagent la **bibliothèque**, les **données**, et (via les adaptateurs) la **session/authentification**.

---

## 6. Ce qui ne traverse pas le pont

`.NET Standard 2.0` est un **sous-ensemble** : les API propres au .NET Framework — `System.Web`, WCF côté serveur, `AppDomain.CreateDomain`, .NET Remoting, CAS, `BinaryFormatter` — **n'y figurent pas** (→ liste des APIs retirées en [§11.3](03-framework-vers-net10.md)). La bibliothèque partagée doit donc s'en tenir à la **surface commune**.

Pour le **côté moderne** qui aurait besoin de nombreuses API spécifiques à Windows (registre, WMI, journal d'événements…), le pack **`Microsoft.Windows.Compatibility`** ajoute des milliers d'API Windows à un projet .NET — ce qui facilite la migration, mais reste **Windows uniquement** et concerne **l'hôte moderne**, pas la bibliothèque `.NET Standard` elle-même.

La règle d'or : **ne pas chercher à faire entrer du code spécifique à une plateforme dans la bibliothèque partagée** ; l'isoler derrière des interfaces, et le réaliser dans chaque hôte.

---

## 7. Recommandations pratiques

- Pour une bibliothèque devant servir **les deux mondes** : cibler **`.NET Standard 2.0`** (jamais 2.1 si .NET Framework est requis).
- **Garder la bibliothèque pure** (logique, modèles, abstractions) ; repousser l'UI et les spécificités de plateforme vers les hôtes.
- Pour du **code réellement neuf** sans contrainte Framework : cibler **`net10.0`** directement — ne pas employer `.NET Standard` par réflexe.
- Recourir au **multi-ciblage** dès qu'il faut des chemins de code propres à chaque plateforme.
- Penser la coexistence comme un **échafaudage temporaire** : il sert la transition, puis l'ancien côté est retiré (*Strangler Fig*) et la bibliothèque `.NET Standard` peut elle-même être re-ciblée vers `net10.0` (→ [§11.6](06-moderniser.md)).

---

## 🔑 Points clés à retenir

- La migration incrémentale fait de la coexistence la **norme** : l'objectif est une **source unique de vérité**, jamais une logique métier dupliquée.
- **`.NET Standard 2.0`** est le pont : une bibliothèque consommable à la fois par .NET Framework et par .NET 10. **Jamais la 2.1** si le support Framework est nécessaire.
- Extraire la logique métier et l'accès aux données dans une bibliothèque **`.NET Standard 2.0` pure** que les deux mondes référencent ; garder UI et spécificités de plateforme dans chaque hôte, derrière des interfaces.
- Le **multi-ciblage** (`net48;net10.0-windows` dans un même projet, avec compilation conditionnelle `#If`) gère les chemins de code que le sous-ensemble `.NET Standard` ne peut exprimer.
- Les applications cohabitent sur des **runtimes séparés**, en partageant la bibliothèque, les données et — pour le web — la session/authentification via YARP + System.Web Adapters (→ [§11.4](04-web-forms-legacy.md)).
- `.NET Standard` est en maintenance : c'est l'outil adapté à **ce scénario de pont**, mais le code neuf doit cibler **`net10.0`** directement.

---

⬅️ [11.4 — ASP.NET Web Forms (legacy VB)](04-web-forms-legacy.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [11.6 — Moderniser (async, LINQ, EF Core, DI, testabilité)](06-moderniser.md)

⏭️ [Moderniser (async, LINQ, EF Core, injection de dépendances, testabilité)](/11-migration-legacy/06-moderniser.md)
