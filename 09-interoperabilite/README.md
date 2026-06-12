🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 9. Interopérabilité 🔗 ⭐

> **Faire dialoguer VB.NET avec le reste du monde : le Windows natif, l'univers COM/Office, les autres langages .NET et le web moderne.**

S'il existe un domaine où VB.NET n'a rien à envier à C# — et où il garde même une longueur d'avance historique — c'est bien l'**interopérabilité**. Là où d'autres chapitres de cette formation assument honnêtement les limites d'un langage stabilisé, celui-ci décrit au contraire un **point fort durable**, et l'une des principales raisons de continuer à écrire du VB.NET en 2026.

---

## Pourquoi l'interop est au cœur de la stratégie VB.NET

La stratégie officielle de Microsoft décrit VB.NET comme un langage **stabilisé**, qui adopte « généralement une approche de consommation seule » (*consumption-only*). Ce vocabulaire n'est pas neutre : **consommer**, c'est précisément ce que l'interopérabilité permet de faire. L'interop est donc la mise en œuvre technique concrète de la doctrine du langage.

Concrètement, un développeur VB.NET en 2026 ne cherche pas à tout réécrire, mais à **brancher** son code sur ce qui existe déjà :

- des **API Windows** natives, écrites en C/C++, vieilles de décennies mais toujours indispensables ;
- des **composants COM** et des applications **Office** (Excel, Word, Outlook), terrain de jeu historique de Visual Basic depuis VB6 ;
- des **bibliothèques modernes écrites en C#** (performance, *records*, fonctionnalités récentes), que l'on consomme sans friction depuis VB.NET ;
- du **contenu web moderne**, que l'on peut désormais embarquer directement dans une application de bureau.

Cette logique fait de l'interopérabilité le **pivot** de deux chapitres voisins de cette formation :

- le module **10 — Architecture hybride VB.NET / C#** 🔗, qui généralise l'idée « cœur en C#, UI et métier en VB.NET » et repose entièrement sur l'interop entre langages (section 9.3) ;
- le module **11 — Migration et maintenance du legacy** ⭐, où l'interop COM (9.2) et P/Invoke (9.1) servent à faire cohabiter l'ancien et le nouveau pendant une migration progressive.

Pour la liste des sujets que l'on **délègue** délibérément à C# tout en les consommant depuis VB.NET, on se reportera à l'**[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)**.

---

## Les quatre directions de l'interopérabilité

Ce chapitre explore quatre « directions » d'interopérabilité, chacune correspondant à un monde différent avec lequel VB.NET doit pouvoir communiquer.

| Direction | Ce que l'on fait | Section | Nature |
|-----------|------------------|---------|--------|
| **Vers le Windows natif** | Appeler des fonctions de l'API Win32 et des DLL non managées | 9.1 — P/Invoke | Force technique |
| **Vers COM et Office** | Piloter Excel / Word / Outlook, consommer des composants COM | 9.2 — COM & Office | Force **historique** ⭐ |
| **Vers les autres langages .NET** | Partager du code VB ↔ C# (et F#) dans une même solution | 9.3 — Interop langages | Stratégie **hybride 2026** 🔗 |
| **Vers le web moderne** | Embarquer une UI web (Chromium) dans une application de bureau | 9.4 — WebView2 | Modernisation de l'UI |

On peut y voir deux **époques** complémentaires de l'interop :

- une interop **historique et descendante** — vers le natif et vers COM — qui prolonge l'héritage de Visual Basic et reste précieuse pour la maintenance et le pilotage d'Office ;
- une interop **stratégique et latérale** — vers C# et vers le web moderne — qui répond directement au gel du langage en s'appuyant sur ce que produit le reste de l'écosystème.

Les deux sont pleinement dans le périmètre « consommation » de VB.NET, et toutes deux sont des compétences de premier plan en 2026.

---

## Un mot sur l'interop et le .NET moderne

L'interopérabilité existait déjà sous .NET Framework, mais quelques repères ont évolué avec **.NET moderne (.NET 10)** :

- l'**interop COM reste prise en charge, mais uniquement sous Windows** — ce qui n'est pas une contrainte pour les scénarios cœur de VB.NET, par nature centrés sur le bureau Windows ;
- certains **défauts de marshaling** et comportements par défaut diffèrent légèrement de .NET Framework : il faut en tenir compte lors d'une migration (voir module 11) ;
- côté P/Invoke, VB.NET continue de s'appuyer sur l'instruction `Declare` et sur l'attribut `<DllImport>` ; le générateur de source `LibraryImport`, plus récent, **produit du code C#** et n'est donc pas l'outil de prédilection en VB (rappel du caractère C#-only des *source generators*, cf. Annexe B) ;
- la contrainte **Native AOT** — non prise en charge en pratique pour VB — n'a pas d'incidence ici, l'interop décrite dans ce chapitre s'inscrivant dans un déploiement classique.

Autrement dit : l'interopérabilité reste l'un des terrains les plus **stables et les mieux servis** pour VB.NET sur .NET 10.

---

## Objectifs du chapitre

À l'issue de ce chapitre, vous serez en mesure de :

- **situer** chaque technique d'interopérabilité et savoir quand y recourir plutôt qu'à une solution 100 % managée ;
- **appeler des fonctions de l'API Windows** depuis VB.NET, en comprenant les notions de marshaling et de rappels (*callbacks*) ;
- **automatiser les applications Office** et consommer des composants COM, en distinguant *early binding* et *late binding* ;
- **partager du code entre projets VB et C#** au sein d'une même solution, fondation de la stratégie hybride ;
- **intégrer une interface web moderne** dans une application Windows Forms ou WPF via WebView2 ;
- **relier** ces techniques au reste de l'écosystème — architecture hybride (module 10), migration de *legacy* (module 11) et frontière VB/C# (Annexe B).

---

## Prérequis

Pour tirer le meilleur de ce chapitre, il est recommandé de maîtriser :

- la **programmation orientée objet** (module 3) — classes, interfaces, attributs ;
- les **notions d'asynchronie** (module 4) — utiles notamment pour WebView2 et certains appels d'API ;
- les **bases de Windows Forms ou WPF** (modules 5 et 6) — la plupart des scénarios d'interop sont au service d'applications de bureau.

---

## Plan du chapitre

- **9.1 — P/Invoke** : appeler des API natives Windows depuis VB.NET (marshaling, structures, *callbacks*), avec l'instruction `Declare` et l'attribut `<DllImport>`.
- **9.2 — COM et automation Office** ⭐ : piloter Excel, Word et Outlook, consommer des composants COM (RCW / CCW), et choisir entre liaison anticipée et liaison tardive (`Option Strict Off`). La force historique de Visual Basic.
- **9.3 — Interopérabilité entre langages .NET** 🔗 : faire collaborer VB, C# et F# dans une solution mixte — le socle technique de l'architecture hybride détaillée au module 10.
- **9.4 — WebView2** : embarquer du web moderne (moteur Chromium) au sein d'une application de bureau, pour combiner la richesse de l'UI web et la robustesse d'une application Windows native.

---

> 🔗 **À lire en parallèle** : module **10 — Architecture hybride VB.NET / C#** et **[Annexe B — Frontière VB.NET / C#](../annexes/frontiere-vbnet-csharp/README.md)**, qui prolongent directement ce chapitre.

⏭️ [P/Invoke (appel d'API natives Windows, marshaling, callbacks)](/09-interoperabilite/01-pinvoke.md)
