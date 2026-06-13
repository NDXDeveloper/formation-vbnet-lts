# 💻 Exemples du module 2 — Fondamentaux du langage

Douze projets **complets, compilés et vérifiés** (un par section du module). Chaque projet
reprend **tous les extraits de code** de sa section, assemblés en un programme exécutable et
commenté ; chaque fichier source porte un en-tête précisant la **section concernée**, la
**description** et le **fichier du cours** dont il provient.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine **fr-FR**).

> 💡 **Compilation / exécution** — identique pour tous les projets console :
> ```bash
> cd <dossier-de-l-exemple>
> dotnet run          # ou : ouvrir le .vbproj dans VS 2026, puis F5
> ```
> Tous les projets activent `Option Strict On` (la recommandation ⭐ de la section 2.1).

> ⚠️ **Culture** — la machine de validation est en **fr-FR** : les `Double`/`Decimal`
> s'affichent avec la virgule (`3,5`), et `{x:C2}` donne `19,90 €`. L'exemple 2.7 traite
> précisément de ce sujet et force des cultures **explicites** (fr-FR, en-US, invariante)
> pour des sorties reproductibles. Dans les montants formatés, les espaces sont des espaces
> **insécables** Unicode (héritage ICU de .NET).

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Erreurs compilateur vérifiées |
|---|---|---|
| `README.md` (module) | — (aucun code) | |
| `01-structure-options.md` | [`2.1-structure-options`](#21-structure-options) | BC30512, BC30574 |
| `02-types-variables.md` | [`2.2-types-variables`](#22-types-variables) | BC33101 |
| `03-operateurs.md` | [`2.3-operateurs`](#23-operateurs) | |
| `04-conditions.md` | [`2.4-conditions`](#24-conditions) | |
| `05-boucles.md` | [`2.5-boucles`](#25-boucles) | |
| `06-chaines.md` | [`2.6-chaines`](#26-chaines) | |
| `07-dates-nombres-culture.md` | [`2.7-dates-nombres-culture`](#27-dates-nombres-culture) | |
| `08-tableaux-collections.md` | [`2.8-tableaux-collections`](#28-tableaux-collections) | |
| `09-linq.md` | [`2.9-linq`](#29-linq) | |
| `10-generiques-avances.md` | [`2.10-generiques-avances`](#210-generiques-avances) | |
| `11-portee-visibilite.md` | [`2.11-portee-visibilite`](#211-portee-visibilite) | BC30389 |
| `12-espace-my.md` | [`2.12-espace-my`](#212-espace-my) (Windows Forms) | BC30456 (en console) |

Les **erreurs vérifiées** ont été réellement provoquées à la compilation, puis recommentées
dans le code avec leur message exact — par exemple `BC30512 : « Option Strict On interdit
les conversions implicites de 'Double' en 'Integer'. »`

---

## 2.1-structure-options

- **Section concernée** : 2.1 — Structure d'un programme ; `Option Strict` / `Explicit` / `Infer` / `Compare`
- **Fichier concerné** : `01-structure-options.md`
- **Description** : l'« anatomie » exacte du cours (directives `Option` → `Imports` →
  `Namespace`/`Module`/`Sub Main`), les procédures dans `Procedures.vb` (`Sub`/`Function`,
  `ByRef`, `Optional`, `ParamArray`, argument nommé), la continuation de ligne implicite et
  explicite, les conversions sous `Option Strict On` (élargissement vs `CInt`), l'inférence
  `Option Infer` — et la **portée fichier** des directives : `ComparaisonsTexte.vb` surcharge
  le projet avec `Option Compare Text`. Le `.vbproj` reprend le `PropertyGroup` du cours.
- **Sortie attendue** (vérifiée) :
  ```text
  Bonjour, VB.NET sur .NET 10 !

  == Sub, Function et paramètres ==
  Carre(7) = 49
  Après Doubler(x) : x = 20
  Formater("abc", majuscules:=True) = ABC
  Somme(1, 2, 3, 4) = 10

  == Continuation de ligne ==
  total = 6
  message = Ligne 1 Ligne 2

  == Option Strict : conversions ==
  Élargissement Integer -> Double : moyenne = 10
  Conversion explicite CInt(3.9) = 4
  texte.Length = 7

  == Option Infer ==
  Dim compteur = 0 -> type inféré : Int32
  Dim noms = New List(Of String) -> type inféré : List`1

  == Option Compare ==
  Binary (ce fichier)  : "Apple" = "apple" -> False
  Text (fichier dédié) : "Apple" = "apple" -> True
  ```
- **Comportement attendu** : `ByRef` modifie la variable de l'appelant (x = 20) ; la même
  comparaison de chaînes change de résultat selon l'`Option Compare` du **fichier**.

## 2.2-types-variables

- **Section concernée** : 2.2 — Types de données et variables
- **Fichier concerné** : `02-types-variables.md`
- **Description** : types valeur (copie) vs référence (objet partagé), `Nothing` contextuel,
  littéraux (`1_000_000L`, `&HFF`, `&B1010`, `19.99D`, `"A"c`, `#2026-12-25#`), variable
  locale `Static`, nullables de valeur (`Integer?`, `HasValue`, `If(age, 0)`,
  `GetValueOrDefault`) — avec le rappel vérifié que `Dim nom As String?` **ne compile pas**
  (BC33101 : les *nullable reference types* sont propres à C#) —, inférence, constantes et
  les trois énumérations du cours (`Enums.vb`, dont `<Flags>`).
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  a = 10 (inchangé : la valeur a été copiée)
  liste1.Count = 4 (liste1 et liste2 pointent le même objet)
  Dim n As Integer = Nothing -> n = 0
  Dim s As String = Nothing  -> s Is Nothing = True
  &HFF -> 255 ; &B1010 -> 10 ; 19.99D -> 19,99 ; #2026-12-25# -> 2026-12-25
  Appel n° 1 / Appel n° 2 / Appel n° 3        (variable Static)
  age.HasValue = False ; age Is Nothing = True
  If(age, 0) = 30 ; If(absent, 0) = 0
  jour = Mercredi ; CInt(jour) = 2
  CodeHttp.NonTrouve = 404
  p.HasFlag(Lecture) = True ; p = Lecture, Ecriture
  ```
- **Comportement attendu** : la liste partagée passe à 4 éléments via la seconde référence ;
  le compteur `Static` survit aux appels ; l'énumération `<Flags>` s'affiche en liste de noms.

## 2.3-operateurs

- **Section concernée** : 2.3 — Opérateurs et expressions
- **Fichier concerné** : `03-operateurs.md`
- **Description** : les deux divisions (`7 / 2 = 3,5` ; `7 \ 2 = 3`), `Mod`, `^` =
  **puissance** (XOR = `Xor`), concaténation `&`, `=`/`<>`, `Is`/`IsNot`, motifs `Like`
  (`?`, `*`, `#`, `[liste]`, `[!liste]`), **court-circuit démontré par compteur
  d'évaluations** (`And` évalue la droite, `AndAlso` non) et le test sûr
  `client IsNot Nothing AndAlso client.Solde > 0` avec `client = Nothing` (`Client.vb`),
  bit-à-bit et décalages, affectations composées (pas de `++`), conversions
  (`CInt`/`CType`/`DirectCast`/`TryCast`), priorité (`And` avant `Or`).
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  7 / 2   = 3,5 (division flottante)
  7 \ 2   = 3 (division entière)
  2 ^ 10  = 1024 (puissance — le XOR s'écrit Xor)
  6 Xor 3 = 5
  "A1" Like "[A-Z]#"           -> True
  False And f()     -> False ; f() évaluée 1 fois
  False AndAlso f() -> False ; f() évaluée 0 fois (court-circuit)
  client IsNot Nothing AndAlso client.Solde > 0 -> sûr même avec client = Nothing
  &B1100 And &B1010 = 8 ; 1 << 4 = 16
  0 +=10 *=2 \=3 -> 6
  TryCast(123, String) Is Nothing -> True
  True Or False AndAlso False   -> True
  (True Or False) AndAlso False -> False
  ```
- **Comportement attendu** : aucune exception malgré `client = Nothing` (grâce à `AndAlso`) ;
  le compteur prouve le court-circuit.

## 2.4-conditions

- **Section concernée** : 2.4 — Structures conditionnelles
- **Fichier concerné** : `04-conditions.md`
- **Description** : `If…ElseIf…Else` (bloc et une ligne), opérateur `If()` ternaire et
  coalescence, **le piège `IIf()` démontré par compteurs** (la fonction héritée évalue
  TOUJOURS les deux branches ; l'opérateur `If()` court-circuite), `Select Case` complet
  (plages `To`, listes, `Case Is <`, `Case Else`, chaînes), `TypeOf…Is` + `DirectCast`
  en deux temps, et l'aiguillage par type `If/ElseIf` sur `Cercle`/`Rectangle`
  (`Formes.vb`) — la limite assumée de VB face au *pattern matching* C#.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  solde =  100 -> Créditeur ; 0 -> À zéro ; -50 -> Débiteur
  age = 20 -> majeur ; If(saisie, "Anonyme") = Anonyme
  IIf(True, ...) -> branche vraie évaluée 1 fois, branche fausse évaluée 1 fois (LES DEUX !)
  If(True, ...)  -> branche vraie évaluée 1 fois, branche fausse évaluée 0 fois (court-circuit)
  note =  95 -> Excellent ; 75 -> Bien ; 55 -> Passable ; 30 -> Insuffisant ; 150 -> Note invalide
  commande "Ouvrir" -> ouverture ; "CLOSE" -> fermeture ; "imprimer" -> Commande inconnue
  o est un String de longueur 7
  aire du Cercle : 12,57        (π × 2²)
  aire du Rectangle : 12,00     (3 × 4)
  ```
- **Comportement attendu** : les compteurs matérialisent la différence `If()`/`IIf()` ;
  aucun *fall-through* dans `Select Case`.

## 2.5-boucles

- **Section concernée** : 2.5 — Boucles et itérations
- **Fichier concerné** : `05-boucles.md`
- **Description** : `For…Next` (borne supérieure **incluse**, `Step -2`,
  `Exit For`/`Continue For` → « 3 6 9 12 »), `For Each` (liste, chaîne caractère par
  caractère), **suppression sûre** (parcours à rebours et `RemoveAll`), `Do While` /
  `Do Until` (file `Queue`), `Do…Loop Until` (la saisie `Console.ReadLine()` du cours est
  **simulée de façon déterministe** — renvoie `Nothing` à l'épuisement), `While…End While`
  (`Wend` n'existe plus) avec lecture de montants simulée.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  For i = 1 To 5          -> 1 2 3 4 5
  For i = 10 To 0 Step -2 -> 10 8 6 4 2 0
  Multiples de 3 jusqu'à 12 -> 3 6 9 12
  Parcours à rebours : 3, 4, 9
  RemoveAll          : 3, 4, 9
  Traitement de : commande A / commande B / commande C
  Traitement de : première saisie / deuxième saisie
  Fin de la saisie simulée (Nothing reçu).
  total = 30 / 75 / 125 -> Seuil de 100 atteint : total final = 125
  ```
- **Comportement attendu** : 6 itérations pour `For i = 10 To 0 Step -2` (borne incluse) ;
  les listes nettoyées des négatifs par les deux techniques donnent le même résultat.

## 2.6-chaines

- **Section concernée** : 2.6 — Chaînes et manipulation de texte
- **Fichier concerné** : `06-chaines.md`
- **Description** : immuabilité (réaffectation obligatoire), littéraux **sans antislash**
  (guillemets doublés, `Environment.NewLine`, `vbTab`, littéral multi-ligne VB 14+),
  `IsNullOrEmpty`/`IsNullOrWhiteSpace`, `s(0)` (parenthèses), `String.Join`, méthodes
  courantes (`Trim`, `Substring`+`LastIndexOf`, `Split`, `Replace`, `PadLeft`), **fonctions
  héritées VB6 en base 1** (`InStr` → 4 là où `IndexOf` → 3 ; 0 vs -1 si absent),
  `StringBuilder` (1 000 lignes assemblées), interpolation `$"…"` (format `C2`, alignement,
  `{{ }}`) et `FormattableString.Invariant`, `String.Equals` + `StringComparison`.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  après mot.ToUpper() sans réaffectation : bonjour
  après mot = mot.ToUpper()             : BONJOUR
  Il a dit "bonjour".
  InStr("Bonjour", "jour")   = 4 (base 1)
  s.IndexOf("jour")        = 3 (base 0)
  InStr absent -> 0 ; IndexOf absent -> -1
  Nombre de lignes assemblées : 1000
  Prix : 19,90 € — réf. {A-100} (culture courante de la machine)
  Alignement : [Ada       ][   36]
  Invariant : prix = 19.90
  String.Equals(a, b, OrdinalIgnoreCase) -> True
  ```
- **Comportement attendu** : le décalage base 1 / base 0 entre fonctions héritées et
  méthodes .NET est visible ; l'invariant affiche le point décimal quelle que soit la machine.

## 2.7-dates-nombres-culture

- **Section concernée** : 2.7 — Dates, nombres, formatage et culture
- **Fichier concerné** : `07-dates-nombres-culture.md`
- **Description** : le type `Date`, le **piège des littéraux** `#…#` (vérifié :
  `#6/10/2026# = #2026-06-10#` → `True`, c'est bien le 10 juin), immuabilité et `TimeSpan`
  (30 jours), `DateOnly`/`TimeOnly`/`DateTimeOffset`, `TryParse` **avec culture** (la même
  chaîne `"10/06/2026"` donne deux dates différentes en fr-FR et en-US !), `ParseExact`,
  formats standard/personnalisés (`MM` mois vs `mm` minutes), `Double` binaire
  (`0.1 + 0.2 = 0.30000000000000004`) vs `Decimal` exact, le piège du séparateur décimal
  (`TryParse("1234.56", fr-FR)` **échoue**), formats numériques par culture, et le
  « i » turc (`"i".ToUpper(tr-TR)` → `İ`) qui justifie les comparaisons ordinales.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  #6/10/2026# = #2026-06-10# -> True
  echeance = 2026-07-10 ; duree.TotalDays = 30
  "10/06/2026" lu en fr-FR (jour/mois)  -> 2026-06-10
  "10/06/2026" lu en en-US (mois/jour)  -> 2026-10-06 (autre date !)
  "D" fr-FR : mercredi 10 juin 2026
  "o"       : 2026-06-10T14:30:00.0000000 (aller-retour ISO 8601)
  ⚠️ MM vs mm : MM -> 06 (mois) ; mm -> 30 (minutes)
  0.1 + 0.2 en Double  : 0.30000000000000004
  0.1D + 0.2D en Decimal : 0.3 -> Decimal pour l'argent !
  TryParse("1234.56", fr-FR)      -> ok = False, valeur = 0 (interprétation inattendue !)
  Parse("1234.56", Invariant)    -> 1234.56
  C2 fr-FR     : 1 234,50 €
  N2 Invariant : 1,234.50
  "i".ToUpper(tr-TR) = İ (İ : I pointé majuscule !)
  ```
- **Comportement attendu** : toutes les démonstrations utilisent des **cultures explicites**
  → la sortie est identique sur toute machine (seuls `Date.Now`/`DateTimeOffset.Now`,
  affichés à titre indicatif, varient).

## 2.8-tableaux-collections

- **Section concernée** : 2.8 — Tableaux et collections
- **Fichier concerné** : `08-tableaux-collections.md`
- **Description** : le piège `Dim a(4)` = **5 éléments** (borne supérieure), initialiseurs,
  `ReDim Preserve` (contenu conservé), rectangulaires `(,)` (`GetLength`, `Rank`) vs
  dentelés `()()`, `List(Of T)` (`Add`/`Insert`/`RemoveAll`), `Dictionary` (indexeur
  ajoute-ou-met-à-jour, `TryGetValue` sans exception, parcours `KeyValuePair`),
  `HashSet`/`Queue` (FIFO)/`Stack` (LIFO), **`ObservableCollection` avec abonnement
  `CollectionChanged`** (la console « joue l'UI »), `ConcurrentDictionary.AddOrUpdate`
  atomique, `AsReadOnly` et exposition par interfaces.
- **Sortie attendue** (vérifiée, extraits clés) :
  ```text
  Dim nombres(4) -> nombres.Length = 5 (et non 4)
  après ReDim Preserve nombres(9) : Length = 10, nombres(0) = 10 (conservé)
  grille(2, 3) : GetLength(0) = 3, GetLength(1) = 4, Rank = 2
  dentelé : lignes(0)(1) = 2 ; longueurs 3 et 2
  après Add/Insert : Katherine, Ada, Alan, Grace
  après RemoveAll(StartsWith("A")) : Katherine, Grace (Count = 2)
  TryGetValue("Linus") -> False (clé absente, pas d'exception)
  Queue.Dequeue() -> premier ; Stack.Pop() -> second
    notification : Add -> Acheter du café
  compteurs("clics") après 2 AddOrUpdate = 2
  ```
- **Comportement attendu** : l'abonné `CollectionChanged` est notifié à chaque `Add` ;
  l'ordre d'énumération du `Dictionary` (ici : insertion) n'est pas contractuel.

## 2.9-linq

- **Section concernée** : 2.9 — LINQ, un point fort de VB.NET ⭐
- **Fichier concerné** : `09-linq.md`
- **Description** : la classe `Produit` et les données exactes du cours, la syntaxe de
  requête (`From…Where…Order By…Select`), le type anonyme `New With {Key …}`, les
  **mots-clés propres à VB** (`Aggregate … Into Sum/Average`, `Distinct`, `Skip`/`Take` —
  adaptés `Skip 1 Take 1` sur 3 éléments), `Group By … Into Count()/Average()`, la syntaxe
  par méthodes équivalente (lambdas `Function(p) …`), les opérateurs terminaux, et
  l'**exécution différée** : la requête voit les ajouts postérieurs ; `ToList()` fige.
- **Sortie attendue** (vérifiée) :
  ```text
  Moins de 50 € (tri par prix) : Souris, Clavier
    type anonyme : { Nom = Clavier, Prix = 45 } / { Nom = Souris, Prix = 25 }
  Aggregate ... Into Sum     : 290
  Aggregate ... Into Average : 96,67
  Catégories distinctes : Périphérique, Affichage
  Order By Nom, Skip 1 Take 1 : Écran
    Périphérique : 2 produit(s), prix moyen 35,00
    Affichage : 1 produit(s), prix moyen 220,00
  Count des produits > 50 € : 1
  ToList (tri par prix)     : Souris, Clavier, Écran
  Parcours après l'ajout de Station : Écran, Station
  Instantané ToList (après ajout de Serveur) : Écran, Station
  La requête différée, elle, voit Serveur : Écran, Station, Serveur
  ```
- **Comportement attendu** : la requête différée reflète chaque ajout à la source ;
  l'instantané `ToList` n'en voit aucun. (Tri `Order By Nom` : comparaison culturelle —
  `Écran` se classe entre `Clavier` et `Souris` en fr-FR.)

## 2.10-generiques-avances

- **Section concernée** : 2.10 — Génériques avancés
- **Fichier concerné** : `10-generiques-avances.md`
- **Description** : le type générique maison `Pile(Of T)` (`Pile.vb`), les contraintes
  `As {IEntite, New}` (débloque `New T()`) et `As IComparable(Of T)` (débloque `CompareTo`)
  dans `Contraintes.vb`, les méthodes génériques `Echanger(Of T)` (ByRef) et
  `PlusGrand(Of T…)` avec inférence, la **variance** : covariance (`IEnumerable(Of Out T)`),
  contravariance (`Action(Of In T)`) et les interfaces variantes du cours déclarées et
  implémentées (`Variance.vb`), et la valeur par défaut d'un `T` via `Nothing`
  (pas de `default(T)` en VB).
- **Sortie attendue** (vérifiée) :
  ```text
  Depiler() -> second puis premier (LIFO)
  Depot(Of Client).Creer() -> Client (Id = 0)
  Trieur(Of Integer).EstAvant(3, 7) -> True
  Trieur(Of String).EstAvant("xyz", "abc") -> False
  PlusGrand(3, 7) = 7 ; PlusGrand("abc", "xyz") = xyz
  Echanger(x, y) -> x = 2, y = 1
  Covariance IEnumerable : a, b, c
    affiché : Bonjour
  IProducteur(Of Object) <- ProducteurDeChaines : chaîne produite
    consommé : texte accepté par un consommateur d'Object
  ValeurParDefaut(Of Integer)() = 0 ; (Of Boolean) = False ; (Of String) Is Nothing = True
  ```
- **Comportement attendu** : les assignations variantes compilent dans le bon sens
  uniquement (l'invariance de `List(Of T)` est rappelée en commentaire).

## 2.11-portee-visibilite

- **Section concernée** : 2.11 — Portée, visibilité et modificateurs d'accès
- **Fichier concerné** : `11-portee-visibilite.md`
- **Description** : la portée de bloc (le `carre` du `For` n'existe plus après `Next` —
  total = 14), la classe `CompteBancaire` exacte du cours (`Private`, `Protected Friend`,
  propriété publique en lecture seule, `Protected Overridable`) plus une dérivée
  `CompteEpargne` qui consomme les membres protégés, et l'**asymétrie des défauts**
  (`Defauts.vb`) : champ `Dim` **Public** par défaut dans une `Structure`, **Private**
  dans une `Class` (erreur BC30389 vérifiée). `InternalsVisibleTo` est montré en
  commentaire (il suppose un assembly de tests, → module 13).
- **Sortie attendue** (vérifiée) :
  ```text
  total = 14 (1 + 4 + 9 — 'carre' et 'i' n'existent plus ici)
  compte.Solde = 100 (propriété publique en lecture seule)
  compte.TauxInterne = 0,03 (Protected Friend, même assembly)
  epargne.Solde après intérêts = 1050,00
  PointStruct : ps.X = 5 (champ Dim public par défaut)
  PointClasse : pc.LireY() = 0 (champ Dim privé par défaut)
  ```
- **Comportement attendu** : `TauxInterne` (Protected Friend) est accessible car nous sommes
  dans le **même assembly** ; 1 000 + 5 % = 1 050.

## 2.12-espace-my

- **Section concernée** : 2.12 — L'espace `My`, un raccourci propre à VB.NET ⭐ ⚠️
- **Fichier concerné** : `12-espace-my.md`
- **Description** : projet **Windows Forms** (le scénario où `My` est pleinement pris en
  charge sur .NET 10), avec cadre applicatif, **paramètres** (`My Project/Settings.settings`
  + `Settings.Designer.vb` écrits comme le fait le Concepteur) et **ressources**
  (`Resources.resx` + `Resources.Designer.vb`). Le formulaire affiche :
  `My.Application.Info.Version/DirectoryPath`, `CommandLineArgs`, `OpenForms`,
  `My.Computer.Name/Network.IsAvailable/FileSystem` (écriture puis relecture d'un fichier),
  `My.User.IsAuthenticated/Name`, `My.Resources.MessageBienvenue`, et **un compteur de
  lancements `My.Settings` persisté** (`Save()` → `user.config`).
  `My.Computer.Clipboard` est laissé en commentaire (pour ne pas écraser votre
  presse-papiers).
- **Compiler / exécuter** :
  ```bash
  cd 2.12-espace-my
  dotnet build       # vérification automatisée
  dotnet run         # ouvre la fenêtre (sous Windows)
  ```
- **Comportement attendu / vérifié** : compilation sans erreur ; l'application démarre et
  affiche le rapport `My` (vérifié stable sur 2 lancements) ; après deux lancements, le
  fichier `%LOCALAPPDATA%\EspaceMy\...\user.config` contient bien
  `NombreDeLancements = 2` (**persistance `My.Settings` vérifiée**). Relancez : le compteur
  affiché s'incrémente à chaque fois.
- **Vérification complémentaire de la section** ⚠️ : dans un projet **console** net10.0,
  `My.Computer`, `My.Application` et `My.User` n'existent pas — erreurs **BC30456**
  (« 'Computer' n'est pas un membre de 'My' », etc.) constatées à la compilation, conformément
  à la matrice de support du cours. Seuls `My.Resources` / `My.Settings` restent câblables
  hors Windows Forms.

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build` (aucun téléchargement NuGet nécessaire : ces projets n'utilisent que le SDK).

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

> L'exemple 2.12 écrit en outre, à l'exécution, un `user.config` sous
> `%LOCALAPPDATA%\EspaceMy\` (c'est le but de la démo `My.Settings`) et un fichier
> `espace-my-demo.txt` dans `%TEMP%` — supprimables sans risque.

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
