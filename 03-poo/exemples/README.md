# 💻 Exemples du module 3 — Programmation orientée objet

Huit projets **complets, compilés et vérifiés** (un par section ; le README du module n'a
pas de code). Chaque projet reprend **tous les extraits** de sa section, assemblés en
programme exécutable ; chaque fichier source porte un en-tête **section concernée /
description / fichier du cours**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR).

> 💡 **Compilation / exécution** — identique pour tous les projets console :
> ```bash
> cd <dossier-de-l-exemple>
> dotnet run          # ou : ouvrir le .vbproj / .sln dans VS 2026, puis F5
> ```
> Tous les projets activent `Option Strict On`. L'exemple 3.7 est une **solution**
> (`dotnet build ImmuabiliteRecords.sln` puis `dotnet run --project AppVB`).

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Erreurs compilateur vérifiées |
|---|---|---|
| `README.md` (module) | — (aucun code) | |
| `01-classes-objets.md` | [`3.1-classes-objets`](#31-classes-objets) | BC31061 |
| `02-structures-tuples.md` | [`3.2-structures-tuples`](#32-structures-tuples) | |
| `03-heritage-polymorphisme.md` | [`3.3-heritage-polymorphisme`](#33-heritage-polymorphisme) | BC30569, BC30299 |
| `04-interfaces.md` | [`3.4-interfaces`](#34-interfaces) | |
| `05-modules-namespaces.md` | [`3.5-modules-namespaces`](#35-modules-namespaces) | |
| `06-evenements-delegues.md` ⭐ | [`3.6-evenements-delegues`](#36-evenements-delegues) | |
| `07-immuabilite-records.md` ⚠️ 🔗 | [`3.7-immuabilite-records`](#37-immuabilite-records) (solution VB + C#) | BC37311 (vérifiée au ch. 1) |
| `08-reflexion-attributs.md` | [`3.8-reflexion-attributs`](#38-reflexion-attributs) | |

---

## 3.1-classes-objets

- **Section concernée** : 3.1 — Classes et objets
- **Fichier concerné** : `01-classes-objets.md`
- **Description** : `Personne` (propriétés auto, `ReadOnly` avec initialiseur, propriété
  calculée `NomComplet`, constructeurs surchargés **chaînés** par `Me.New`, compteur
  `Shared`, **indexeur** `Default`) ; `CompteBancaire` (propriété **complète** dont le
  `Set` valide et lève `ArgumentOutOfRangeException`) ; `Configuration`
  (`Shared Sub New`) ; instanciation (`New`, initialiseur `With {…}`) ; **sémantique de
  référence** (`b = a` partage l'objet ; `Equals` par défaut compare les références).
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  camille.NomComplet = Camille Durand (propriété calculée)
  camille(0) = C (indexeur : propriété Default)
  compte.Solde = -1 -> ArgumentOutOfRangeException : « Le solde ne peut pas être négatif. »
  Personne.NombreCrees = 5 (compteur de classe)
  Configuration.Version = 1.0 (initialisée par Shared Sub New)
  a.Nom = Bob (a et b pointent vers le même objet)
  x.Equals(y) = False (mêmes valeurs, objets distincts)
  ```
- **Comportement attendu / vérifié** : le piège du cours « champ `_X` généré par la
  propriété auto `X` + insensibilité à la casse » a été **vérifié au compilateur** :
  déclarer `_id` à côté de la propriété auto `Id` provoque **BC31061** (« variable '_id'
  est en conflit avec un membre déclaré implicitement pour property 'Id' ») — documenté
  en commentaire dans `Personne.vb`.

## 3.2-structures-tuples

- **Section concernée** : 3.2 — Structures (`Structure`) et tuples
- **Fichier concerné** : `02-structures-tuples.md`
- **Description** : `Point2D` (sémantique de **copie**), `PointImmuable` (champs
  `ReadOnly` + constructeur paramétré — pas de `Sub New()` sans paramètre dans une
  structure), `Exemple` (pas d'initialiseur d'instance ; `Const`/`Shared` seulement),
  `Temperature` (structure immuable + `IComparable(Of T)` → triable), `Nothing` =
  valeur par défaut (jamais « nulle » ; absence via `Point2D?`), égalité structurelle,
  **boxing**, et les **tuples** : `Item1`/`Item2`, éléments nommés, retour multiple
  `DiviserAvecReste(17, 5)` → `3 reste 2`, **pas de déconstruction en VB**, ancien
  `System.Tuple`.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  a.X = 1 (« a » n'a pas bougé) ; b.X = 99
  = Nothing -> X = 0, Y = 0 (valeur par défaut)
  Point2D? = Nothing -> HasValue = False
  p1.Equals(p2) = True (mêmes champs -> égales)
  Triées : -3 °C ; 21,5 °C ; 35,2 °C
  DiviserAvecReste(17, 5) -> 3 reste 2
  ```

## 3.3-heritage-polymorphisme

- **Section concernée** : 3.3 — Héritage et polymorphisme
- **Fichier concerné** : `03-heritage-polymorphisme.md`
- **Description** : la hiérarchie `Animal`/`Chien`/`Chat` complète (`Inherits`,
  `MyBase.New`, `Overridable`/`Overrides`), `ChienPoli` (réutilise `MyBase.Crier()`),
  `ChienDeGarde` (`NotOverridable Overrides`), **`MyClass`** (propre à VB : `ViaMe()` = 2
  suit l'objet réel, `ViaMyClass()` = 1 force la version de `Base`), `MustInherit Forme` +
  `Cercle`/`Rectangle` (méthode concrète `Decrire`), `NotInheritable Devise`,
  **`Shadows`** (résolution selon le **type déclaré** : `d.Afficher()` → « Derivee »,
  `b.Afficher()` → « Base »), `TypeOf…Is` + `DirectCast` et `TryCast`.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  Rex     -> Ouaf / Félix -> Miaou / Médor -> Ouaf (poliment) / Brutus -> GRRR
  d.ViaMe()      = 2 (appel virtuel) ; d.ViaMyClass() = 1 (force la version de Base)
  Cercle    : Cette forme a une aire de 12,57.
  d.Afficher() -> Derivee ; b.Afficher() -> Base
  TryCast réussi : Félix dit Miaou
  ```
- **Erreurs vérifiées** : `New Forme()` → **BC30569** (« 'New' ne peut pas être utilisé
  dans une classe déclarée 'MustInherit' ») ; `Inherits Devise` → **BC30299**
  (héritage d'une classe `NotInheritable`).

## 3.4-interfaces

- **Section concernée** : 3.4 — Interfaces
- **Fichier concerné** : `04-interfaces.md`
- **Description** : `IFormeGeometrique` et la **double obligation VB** (`Implements I`
  sur la classe **et** `Implements I.Membre` sur chaque membre) ; le membre sous un
  **autre nom** (`Rectangle.CalculerPerimetre` satisfait `Perimetre`) ;
  l'**implémentation masquée** (`Ressource.Liberer` est `Private`, joignable par
  `DirectCast(r, IDisposable).Dispose()` et appelée par `Using`) qui satisfait **deux
  contrats** (`IDisposable.Dispose` + `IFichier.Fermer`) ; interfaces multiples
  (`Rapport`), héritage d'interfaces (`IModifiable Inherits ILisible`), génériques
  (`IComparable(Of T)` → tri ; `IDepot(Of T)` maison implémentée par `DepotClients`),
  polymorphisme par interface et `TypeOf … Is IDisposable`.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  Cercle r=2 : Aire = 12,57, Périmètre = 12,57
  Sur le type     : rect.CalculerPerimetre() = 14
  Via l'interface : contrat.Perimetre()      = 14 (même code)
  Via Using (appel automatique de Dispose) :
    Ressource libérée (implémentation masquée).
  IDepot(Of Client) : ObtenirParId(2).Nom = Bob ; total = 2
  ```

## 3.5-modules-namespaces

- **Section concernée** : 3.5 — Modules, espaces de noms et classes partielles
- **Fichier concerné** : `05-modules-namespaces.md`
- **Description** : module `Utilitaires` (accès **non qualifié**), méthode d'extension
  `EstVideOuBlanc` (module + `<Extension()>` — obligatoire en VB), espaces imbriqués et
  forme pointée, **le piège du Root Namespace vérifié** : le projet a
  `RootNamespace = Contoso.Ventes`, et `FullName` prouve que `Namespace Donnees` devient
  `Contoso.Ventes.Donnees` ; `Imports` avec **alias** (`IO = System.IO`) et mot-clé
  `Global` ; classes **partielles** sur deux fichiers et **méthode partielle**
  (implémentée → hook exécuté ; non implémentée → appel **supprimé** à la compilation).
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  AppliquerTVA(100) = 120,0
  "   ".EstVideOuBlanc() = True
  FullName : Contoso.Ventes.Donnees.DepotFactures
  Alias IO  : dossier\fichier.txt
  Entite.MettreAJour()
    -> hook OnModifie exécuté (implémenté dans Entite.Partie2.vb)
  EntiteSansHook.MettreAJour()          ← aucun hook : appel supprimé
  ```

## 3.6-evenements-delegues

- **Section concernée** : 3.6 — Événements et délégués ⭐ (l'idiome VB)
- **Fichier concerné** : `06-evenements-delegues.md`
- **Description** : délégués maison (`Delegate Function`/`Sub`) et `AddressOf`,
  `Action`/`Func`/`Predicate`, lambdas mono/multi-lignes et **closure** (changer `seuil`
  change le prédicat), convention .NET complète (`SeuilEventArgs`,
  `EventHandler(Of T)`, motif `Protected Overridable OnXxx`, **`RaiseEvent` sûr sans
  abonné**), l'idiome **`WithEvents` + `Handles`** (déclenchement au 100ᵉ incrément),
  « un gestionnaire, plusieurs événements » (`Handles` à sources multiples, `sender`
  distingue), **re-câblage automatique** à la réaffectation du champ `WithEvents`
  (vérifié : le remplaçant notifie encore la surveillance), `AddHandler`/`RemoveHandler`
  (y compris lambda), **`Custom Event`** à 3 blocs (liste d'abonnés tracée) et le
  **pattern Observer** complet (`Thermostat`, valeur inchangée → aucun événement).
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  calcul(3, 4) = 7 (appel indirect via le délégué)
  seuil = 100 -> depasse(11) = False (la capture suit la variable)
  [Surveillance] Seuil atteint à 100 (source : principal).
  [Double] Compteur A a atteint 2. / [Double] Compteur B a atteint 3.
  [Surveillance] Seuil atteint à 2 (source : remplaçant).   ← re-câblage automatique
  (après RemoveHandler : aucun message — RaiseEvent sans abonné est sûr)
  [Custom Event] abonné ajouté (total : 1)
  Panneau : 21,0 °C / Journal : 21,0 °C
  ```

## 3.7-immuabilite-records

- **Section concernée** : 3.7 — Types immuables et records ⚠️ 🔗
- **Fichier concerné** : `07-immuabilite-records.md`
- **Description** : **solution hybride** `ImmuabiliteRecords.sln` :
  - `AppVB` (VB) — le type immuable « maison » `Argent` (propriétés `ReadOnly` +
    constructeur validant, mise à jour **non destructive** `AvecMontant`,
    `Equals`/`GetHashCode`/`ToString` redéfinis — tout ce qu'un record C# génère) et
    l'immuabilité **profonde** (`ImmutableArray`) ;
  - `ModeleCsharp` (C#) — les records du cours : `Personne(Nom, Age)` avec la méthode
    de copie **« VB-friendly »** `WithAge` (VB n'a pas l'expression `with`), et
    `Adresse` avec propriété **`init`** `CodePostal`.
  Le tableau ✅/❌ du cours est reproduit : construction, lecture, égalité de valeur,
  `ToString` et `With {…}` sur `init` ✅ ; expression `with` et déconstruction ❌.
- **Compiler / exécuter** :
  ```bash
  cd 3.7-immuabilite-records
  dotnet build ImmuabiliteRecords.sln
  dotnet run --project AppVB
  ```
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  prix  = 19,99 EUR ; solde = prix.AvecMontant(14.99) = 14,99 EUR
  prix est resté 19,99 EUR (mise à jour non destructive)
  a.Equals(b) = True (égalité de VALEUR redéfinie)
  p.ToString() = Personne { Nom = Alice, Age = 30 } (généré par le record)
  p.Equals(p2) = True ; p Is p2 = False
  p.WithAge(31)           : Personne { Nom = Alice, Age = 31 }
  a = Adresse { Rue = 12 rue des Lilas, Ville = Paris, CodePostal = 75001 }
  ```
  (l'affectation d'une propriété `init` hors initialiseur provoque **BC37311**,
  vérifiée au chapitre 1)

## 3.8-reflexion-attributs

- **Section concernée** : 3.8 — Réflexion et attributs
- **Fichier concerné** : `08-reflexion-attributs.md`
- **Description** : les **trois** façons d'obtenir un `Type` (opérateur
  `GetType(Produit)`, `obj.GetType()`, `Type.GetType("chaîne")` — vérifiées identiques),
  inspection (`FullName`, `GetProperties`), `PropertyInfo.GetValue`/`SetValue`
  (Clavier → Souris), `MethodInfo.Invoke` (`Recalculer(0.2)` → prix 100 → 120),
  `Activator.CreateInstance` (2 formes), **découverte de plugins** dans l'assembly
  (`IsAssignableFrom` + `Not IsAbstract` — la base abstraite est exclue), attributs
  intégrés (`<Obsolete>` **relu par réflexion** ; DataAnnotations du cours exploitées
  par `Validator.TryValidateObject`, ajout pédagogique) et l'**attribut personnalisé**
  `<Colonne>` relu par `Mapping.DecrireColonnes` — la sortie du cours à l'identique.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  Les trois désignent le même type : True
  GetValue -> Clavier ; Après SetValue : p.Nom = Souris
  Après Invoke(Recalculer, 0.2) : Prix = 120,0
  Activator (avec arguments)   : Nom = Écran, Prix = 199
    exécution du plugin « Export CSV » / « Import XML »
  AncienChamp est marqué <Obsolete> : « Utilisez NouveauChamp à la place. »
  Inscription(Nom="X", Age=150) valide ? False — 2 erreur(s)
  Id → colonne « produit_id »
  Nom → colonne « libelle » (obligatoire)
  ```
  (messages de validation DataAnnotations en anglais : librairie .NET, culture neutre)

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build` (aucun téléchargement NuGet : ces projets n'utilisent que le SDK).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
