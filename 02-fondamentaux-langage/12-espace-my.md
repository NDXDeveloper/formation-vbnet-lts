🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.12 L'espace `My` — un raccourci propre à VB.NET ⭐ ⚠️

> `My` est une particularité de VB sans équivalent en C# : un point d'accès rapide et explorable aux
> ressources courantes de l'application, de la machine et de l'utilisateur. C'est un vrai gain de
> productivité (⭐), mais avec deux réserves importantes (⚠️) : un **support partiel sur .NET moderne**
> et un **coût en couplage et en testabilité** qu'il faut savoir arbitrer.

---

## Qu'est-ce que l'espace `My` ?

`My` regroupe, sous une poignée d'objets « à composition rapide », des fonctionnalités habituellement
dispersées dans plusieurs espaces de noms .NET. Là où il faudrait connaître `System.IO`,
`System.Reflection` ou la configuration applicative, `My` propose une entrée unique et découvrable via
l'IntelliSense. C'est une commodité **propre à VB** : C# n'a rien de tel et passe directement par les API
sous-jacentes.

---

## Les objets principaux

### `My.Application`

Informations et services sur l'application en cours :

```vb
Console.WriteLine(My.Application.Info.Version)      ' version de l'assembly
Console.WriteLine(My.Application.Info.DirectoryPath) ' dossier de l'exécutable
Dim args = My.Application.CommandLineArgs            ' arguments de ligne de commande
```

En Windows Forms avec le *cadre applicatif* (→ 1.5), `My.Application` expose aussi les événements de
démarrage/arrêt, l'écran de démarrage, l'instance unique et `My.Application.OpenForms`
([module 5](../05-windows-forms/README.md)).

### `My.Computer`

Accès à la machine et à ses services :

```vb
Dim contenu = My.Computer.FileSystem.ReadAllText("config.txt")
My.Computer.Clipboard.SetText("Copié !")
Dim enLigne = My.Computer.Network.IsAvailable
```

On y trouve notamment `FileSystem` (fichiers et dossiers), `Clipboard`, `Network`, `Registry` (Windows),
`Audio`, `Clock` et `Info` (système).

### `My.Settings`

Accès **fortement typé** aux paramètres de l'application et de l'utilisateur (intégré au concepteur de
paramètres) :

```vb
Dim theme = My.Settings.ThemeUtilisateur   ' propriété fortement typée
My.Settings.ThemeUtilisateur = "Sombre"
My.Settings.Save()                          ' persiste les paramètres utilisateur
```

C'est la base des préférences utilisateur en Windows Forms
([module 5.10](../05-windows-forms/10-preferences.md)).

### `My.Resources`

Accès **fortement typé** aux ressources embarquées (`.resx`) — chaînes, images, fichiers :

```vb
Dim logo = My.Resources.LogoEntreprise
Dim message = My.Resources.MessageBienvenue   ' utile pour l'internationalisation
```

C'est un pilier de l'internationalisation ([module 5.11](../05-windows-forms/11-internationalisation.md)).

### `My.User`

Informations sur l'utilisateur courant (fondées sur l'identité de sécurité) :

```vb
If My.User.IsAuthenticated Then
    Console.WriteLine(My.User.Name)
    Dim estAdmin = My.User.IsInRole("Administrateurs")
End If
```

> D'autres objets existent — `My.Forms` (instances de formulaires, Windows Forms), `My.Log` /
> `My.Application.Log` (journalisation) — mais les cinq ci-dessus couvrent l'essentiel des usages.

---

## ⚠️ Support sur .NET moderne

C'est la réserve la plus importante. Sur **.NET Framework**, `My` est complet (Windows Forms, WPF,
console, ASP.NET). Sur **.NET moderne** (.NET 10), le support est **partiel** et dépend du type de
projet :

- **Windows Forms** : `My` est **bien pris en charge** — `My.Application` (y compris le cadre
  applicatif), `My.Computer`, `My.Settings`, `My.Resources`, `My.User`, `My.Forms`. C'est le scénario
  le mieux servi.
- **Console / Bibliothèque** : support **très limité**. Sur un projet console ou bibliothèque
  .NET 10, `My.Application`, `My.Computer` et `My.User` **ne sont pas disponibles** : leurs classes
  de support (`Microsoft.VisualBasic.Devices`, `ApplicationServices`…) n'existent que dans le runtime
  *Desktop* de Windows. Restent accessibles `My.Resources` (code généré par le concepteur de
  ressources, portable) et `My.Settings` (avec le concepteur de paramètres et le paquet
  `System.Configuration.ConfigurationManager`).
- **WPF** : les classes de support existent (le runtime Desktop est présent), mais les membres
  **spécifiques à Windows Forms** — cadre applicatif, `My.Forms` — **ne s'appliquent pas**
  (WPF a sa propre classe `Application`) : le support y est inégal.
- **Membres web supprimés** : `My.Request`, `My.Response`, `My.WebServices` et les parties liées au
  contexte ASP.NET ont **disparu** — ASP.NET Web Forms n'existe pas sur .NET moderne (en lien avec le
  [module 11.4](../11-migration-legacy/04-web-forms-legacy.md)).

En résumé : **`My` est pleinement à l'aise en Windows Forms**, plus inégal ailleurs.

---

## Productivité (⭐) vs limites (⚠️)

`My` illustre bien un arbitrage récurrent : confort immédiat contre qualité d'architecture.

**Ce qu'il apporte :**

- une **découvrabilité** immédiate des tâches courantes, sans mémoriser les espaces de noms ;
- un accès **fortement typé** aux paramètres et ressources, intégré aux concepteurs — réellement
  pratique ;
- un excellent rapport effort/résultat pour les **petites et moyennes applications Windows Forms**, les
  utilitaires et les prototypes.

**Ce qu'il coûte :**

- **Couplage** : `My.*` est un point d'accès **global et statique** (ambiant). Le code qui appelle
  directement `My.Computer.FileSystem` ou `My.Settings` s'y trouve fortement lié.
- **Testabilité** : ces accès globaux sont **difficiles à simuler** en test unitaire — on ne peut pas
  injecter un faux système de fichiers ou de faux paramètres facilement (en lien avec le
  [module 13](../13-tests-qualite/README.md)).
- **Dépendances cachées** : une méthode qui utilise `My.User` ou `My.Computer.Network` a des dépendances
  **invisibles dans sa signature**.
- **Portabilité** : le code est lié à VB et à la matrice de support ci-dessus.

**L'arbitrage recommandé** : réservez `My` aux **bords** de l'application — couche UI, code de démarrage,
utilitaires — là où le confort prime. Dans la **logique métier, les bibliothèques et tout code à tester
ou à garder portable**, préférez les API .NET explicites (`System.IO`, configuration via
`IConfiguration`) et l'**injection de dépendances**, en passant par des abstractions. C'est la ligne
suivie par le reste de la formation (testabilité, DI, architecture hybride).

---

## Et l'IA dans tout ça ? 🤖

`My` étant **spécifique à VB**, il appelle une vigilance particulière :

- **Aucune traduction depuis C#** : un assistant convertissant du C# vers VB ne produira jamais `My` (il
  utilisera les API explicites — ce qui est correct). Dans l'autre sens (VB → C#), chaque `My.*` doit
  être **remplacé** par l'API sous-jacente.
- **Membres inexistants sur la cible** : un modèle peut suggérer des membres `My` absents sur .NET moderne
  (membres web, ou membres Windows Forms dans un projet WPF). Recoupez avec la matrice de support
  ci-dessus.

La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- `My` est un **raccourci propre à VB** (sans équivalent C#) vers l'application (`My.Application`), la
  machine (`My.Computer`), les paramètres (`My.Settings`), les ressources (`My.Resources`) et
  l'utilisateur (`My.User`).
- **Support partiel sur .NET moderne** ⚠️ : complet en **Windows Forms**, limité en WPF/console (cadre
  applicatif et `My.Forms` absents hors WinForms), **membres web supprimés**.
- **Productif** aux bords d'une application WinForms, mais **coûteux en couplage et testabilité** : dans le
  cœur métier et les bibliothèques, préférez les **API explicites + injection de dépendances**.

---

## Fin du chapitre 2

Ce module a posé le **socle stable** du langage : structure et directives `Option`, types et variables,
opérateurs, structures de contrôle, chaînes, dates et culture, collections, LINQ, génériques, portée et
— pour finir — la commodité `My`. Autant de fondations qui, le langage étant figé, resteront valides
durablement. Le [module 3](../03-poo/README.md) bâtit dessus avec la **programmation orientée objet**.

---

⏭️ [Programmation orientée objet](/03-poo/README.md)
