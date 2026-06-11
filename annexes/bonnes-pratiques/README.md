🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe C — Bonnes pratiques de codage VB.NET

Conventions de nommage, organisation du code et des projets, commentaires et documentation, gestion des erreurs
et journalisation, et bonnes pratiques avec les assistants IA. 🤖

Beaucoup de ces règles sont des **conventions .NET partagées** avec C# (mêmes *Framework Design Guidelines*).
Cette annexe les rappelle, mais met surtout l'accent sur ce qui est **spécifique à VB.NET** — signalé par ⭐ —
et sur les **réflexes hérités de VB6/VBA** à abandonner (signalés par ⚠️). Principe directeur : **la cohérence
prime sur la préférence personnelle**. Une équipe gagne plus à appliquer une convention uniformément qu'à
débattre de la « meilleure ».

---

## C.1 — Le socle : les options du compilateur ⭐

C'est la **bonne pratique VB.NET la plus importante**, et celle qu'on oublie le plus souvent. Les options de
compilation changent radicalement la sécurité du code. Définissez-les **au niveau du projet** (dans le `.vbproj`),
pas fichier par fichier, pour les garantir partout.

```xml
<PropertyGroup>
  <OptionStrict>On</OptionStrict>
  <OptionExplicit>On</OptionExplicit>
  <OptionInfer>On</OptionInfer>
  <OptionCompare>Binary</OptionCompare>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

> ⚠️ Ne recopiez pas `<Nullable>enable</Nullable>` depuis un guide C# : cette propriété active les
> *nullable reference types*, une fonctionnalité **C# uniquement** (module 2.2) — dans un `.vbproj`,
> elle est **silencieusement ignorée** (vérifié sur .NET 10). Elle n'a sa place que dans les projets
> **C#** de vos solutions mixtes ([Annexe B](../frontiere-vbnet-csharp/README.md) 🔗).

- **`Option Strict On`** — *non négociable*. Interdit le *late binding* et les conversions restrictives
  implicites : la plupart des bugs de type sont alors détectés **à la compilation** au lieu de l'exécution. (La
  seule exception légitime à `Off` est l'automation COM/Office en *late binding* — voir module 9.2 — et alors
  **localement**, sur un fichier dédié, jamais sur tout le projet.)
- **`Option Explicit On`** — impose la déclaration de toute variable. Activé par défaut, mais à confirmer.
- **`Option Infer On`** — autorise `Dim x = …` (inférence) tout en gardant le typage fort. Code plus concis,
  sécurité intacte.
- **`Option Compare Binary`** — comparaisons de chaînes **ordinales** par défaut (rapides, prévisibles, alignées
  sur le comportement de C#). Réservez `Text` (insensible à la casse, dépendant de la culture) aux cas qui
  l'exigent réellement, et explicitement.

> ⚠️ `Option Compare Text` au niveau projet est un piège silencieux : il change le résultat de toutes vos égalités
> de chaînes (`=`, `Like`) sans que ce soit visible dans le code.

---

## C.2 — Conventions de nommage

VB.NET suit les conventions .NET standard. **Particularité VB ⭐ : le langage est insensible à la casse.** Vous
ne pouvez donc **pas** distinguer deux membres par la seule casse (contrairement à C#). Soyez malgré tout
rigoureux sur la casse : elle reste essentielle à la **lisibilité**, et facilite la lecture du C# environnant.

| Élément | Casse | Exemple |
|---------|-------|---------|
| Espace de noms | PascalCase | `MaSociete.Facturation` |
| Classe / Structure / Enum / Délégué | PascalCase | `ClientService`, `OrderStatus` |
| Interface | PascalCase préfixée `I` | `IRepository` |
| Méthode / Propriété / Événement | PascalCase | `CalculerTotal`, `Click` |
| Méthode asynchrone | PascalCase + suffixe `Async` | `ChargerClientsAsync` |
| Paramètre / variable locale | camelCase | `nombreClients`, `index` |
| Champ privé | `_camelCase` (préfixe `_`) | `_repository` |
| Constante (`Const`) | PascalCase | `TailleMaximale` |
| Paramètre de type générique | `T`, ou `T` + nom | `T`, `TKey`, `TValue` |
| Booléen (propriété/variable) | PascalCase avec `Is`/`Has`/`Can` | `IsActive`, `HasItems`, `CanExecute` |

**À éviter :**

- ⚠️ La **notation hongroise** héritée de VB6 (`strNom`, `intCompteur`, `objClient`) : le typage fort et l'IDE
  rendent ces préfixes inutiles et bruyants. Choisissez des noms **descriptifs**.
- Les **abréviations obscures** et les noms à une lettre (hors compteurs de boucle `i`, `j`).
- Le préfixe `m_` pour les champs (legacy) : préférez `_camelCase`.

> **Le cas débattu des contrôles WinForms.** L'usage `btnEnregistrer`, `txtNom` (préfixe de type de contrôle)
> contredit les directives Microsoft, mais reste répandu et toléré dans les équipes WinForms pour repérer les
> contrôles. C'est un choix d'équipe à **trancher une fois** puis à appliquer uniformément — pas à mélanger.

---

## C.3 — Organisation du code et des projets

**Au niveau du fichier et du type :**

- **Un type public par fichier**, le fichier portant le nom du type (`ClientService.vb`).
- Espaces de noms **alignés sur l'arborescence des dossiers**.
- Méthodes **courtes**, à responsabilité unique ; classes cohésives.
- Préférez `Using … End Using` pour tout `IDisposable` (libération déterministe).

**Au niveau de l'architecture :**

- **Séparez les responsabilités** : UI / logique métier / accès aux données. L'anti-pattern VB classique ⚠️ est
  de **noyer la logique métier dans le code-behind d'un formulaire** (`Form1.vb`). Extrayez-la dans des classes
  et bibliothèques testables.
- Privilégiez l'**injection de dépendances** (module 5.2 pour WinForms, 8.2 pour les API) plutôt que des états
  globaux ou des singletons maison.
- Réservez `Module` aux **aides réellement sans état** (fonctions utilitaires, méthodes d'extension). ⚠️ Évitez
  d'y stocker un **état mutable global** : c'est un couplage difficile à tester et à raisonner.

**L'espace `My` ⭐ ⚠️ :** très productif (`My.Settings`, `My.Computer`, `My.Resources`), mais il **couple** votre
code à l'environnement et nuit à la testabilité (module 2.12). Bon compromis : l'utiliser dans le code applicatif
de surface, mais **l'abstraire derrière une interface** dans le code que vous voulez tester. Rappel : support
**partiel** sur .NET moderne (correct en WinForms, limité en WPF, membres web supprimés).

**Au niveau de la solution :**

- Séparez en projets : **Domaine/Cœur** (VB), **Données**, **UI**. Placez les **briques modernes en C#** dans
  leurs propres projets ([Annexe B](../frontiere-vbnet-csharp/README.md) 🔗, module 10).
- Utilisez des **dossiers de solution** et une nomenclature de projets cohérente.

---

## C.4 — Commentaires et documentation

**Principe :** le code dit *ce qu'il fait*, le commentaire dit ***pourquoi***. Un bon nom supprime le besoin d'un
commentaire ; un commentaire qui paraphrase le code est du bruit.

- ✅ Commentez les **intentions, décisions et compromis** non évidents (« pourquoi ce contournement », « pourquoi
  cet ordre »).
- ⚠️ Un commentaire **périmé est pire que pas de commentaire** : maintenez-les avec le code, ou supprimez-les.
- En VB, utilisez `'` pour les commentaires de ligne. ⚠️ `REM` est un vestige VB6 — à éviter.

**Documentation XML (`'''`)** pour toute API publique : elle alimente IntelliSense et la génération de
documentation. Activez `<GenerateDocumentationFile>true</GenerateDocumentationFile>` dans le `.vbproj`.

```vb
''' <summary>
''' Calcule le total TTC d'une commande, remises appliquées.
''' </summary>
''' <param name="commande">La commande à évaluer (ne doit pas être Nothing).</param>
''' <returns>Le montant TTC en euros.</returns>
''' <exception cref="ArgumentNullException">Si <paramref name="commande"/> est Nothing.</exception>
Public Function CalculerTotalTTC(commande As Commande) As Decimal
    ' …
End Function
```

> 💡 Une bonne documentation XML **améliore aussi les complétions de l'IA** (elle expose l'intention au modèle),
> et l'IA peut la **générer** pour vous — à relire (module 17.4 🤖).

---

## C.5 — Gestion des erreurs et journalisation

### Exceptions

- Utilisez **`Try`/`Catch`/`Finally`** structurés. ⚠️ Abandonnez **définitivement** `On Error Goto` et
  `On Error Resume Next` (héritage VB6) : ils masquent les erreurs et rendent le flux illisible.

```vb
' ⚠️ Style VB6 à proscrire
' On Error Resume Next
' …

' ✅ Style VB.NET structuré, avec filtre When (un point fort idiomatique de VB ⭐)
Try
    Await TraiterAsync(fichier)
Catch ex As IOException When ex.HResult = HR_FICHIER_VERROUILLE
    _logger.LogWarning(ex, "Fichier verrouillé : {Fichier}", fichier)
    ' nouvelle tentative…
Catch ex As IOException
    _logger.LogError(ex, "Échec d'E/S sur {Fichier}", fichier)
    Throw   ' relance en PRÉSERVANT la pile
End Try
```

- ⭐ Les **filtres `When`** (catch conditionnel) sont disponibles en VB de longue date : préférez-les à un
  `Catch` qui inspecte puis relance.
- **Attrapez le type le plus précis** possible ; évitez le `Catch ex As Exception` fourre-tout, sauf à la
  frontière de l'application (où vous journalisez et décidez quoi faire).
- ⚠️ **Ne pas relancer avec `Throw ex`** : cela **réinitialise la pile d'appels**. Utilisez `Throw` seul.
- ⚠️ **N'avalez jamais une exception** (`Catch` vide). N'utilisez pas les exceptions pour le **flux normal**.
- Validez les entrées tôt avec des **clauses de garde** (`ArgumentNullException`, `ArgumentException`).
- **Exceptions personnalisées** : héritez de `Exception`, nom suffixé `Exception`, fournissez les constructeurs
  usuels (voir module 12.1).
- Préférez **lever des exceptions** plutôt que retourner des codes d'erreur.

### Journalisation

- Utilisez l'abstraction **`Microsoft.Extensions.Logging`** (ou Serilog), **injectée** via DI — pas de logger
  statique (module 12.3). ⚠️ Bannissez `MsgBox`, `Console.WriteLine` et `Debug.Print` comme journalisation de
  production (réflexes VB6).
- **Journalisation structurée** : passez les valeurs en **paramètres nommés du modèle**, jamais par
  concaténation. Cela préserve la structure pour la recherche et l'agrégation.

```vb
' ⚠️ Perd toute structure exploitable
_logger.LogInformation("Utilisateur " & userId & " connecté depuis " & ip)

' ✅ Journalisation structurée
_logger.LogInformation("Utilisateur {UserId} connecté depuis {AdresseIp}", userId, ip)
```

- Choisissez le **niveau** adéquat : `Trace`/`Debug` (diagnostic), `Information` (événements métier),
  `Warning` (anomalie récupérable), `Error`/`Critical` (échecs).
- ⚠️ **Ne journalisez jamais de données sensibles** : mots de passe, jetons, données personnelles (voir
  module 16, sécurité).
- ⚠️ Évitez la **double journalisation** (journaliser *et* relancer à chaque couche) : journalisez **une fois**,
  à l'endroit où vous traitez réellement l'erreur.

---

## C.6 — Bonnes pratiques avec les assistants IA 🤖

L'IA est **indispensable en VB.NET** précisément parce que les modèles sont surtout entraînés sur du C# (le
*biais C#* — modules 1.6.3 et 17). Bien utilisée, elle accélère ; mal cadrée, elle injecte du C# déguisé. Les
règles ci-dessous valent pour tout le code de cette formation produit avec une IA.

**Cadrer la demande :**

- ⭐ **Précisez toujours « Visual Basic .NET / VB.NET »** *et* la version cible (« .NET 10 »). Sans cela, le
  modèle répond souvent en C#.
- ⚠️ **Levez l'ambiguïté avec VB6 / VBA** : dire seulement « Visual Basic » peut produire des macros VBA ou du
  VB6 obsolète. Insistez sur « .NET ».
- Donnez le **contexte** : `Option Strict On`, framework cible, style existant. Demandez du **VB idiomatique**
  (par ex. `Handles`/`WithEvents` pour les événements — module 3.6) et **non une traduction mécanique** du C#.

**Valider systématiquement (la règle d'or) :**

- ⚠️ **Tout code généré se compile et se relit.** C'est le mode d'échec n°1 (module 17.7) : le modèle
  **hallucine de la syntaxe C# en VB**.
- Traquez les **« C#-ismes »** qui n'existent pas en VB : `;`, accolades `{}`, lambdas `=>` (forme C#), `record`,
  accesseurs `init`, *top-level statements*, `await foreach`, *switch expressions*, `new` typé par la cible,
  littéraux de chaîne bruts. Recoupez avec les **[Annexes A](../correspondance-vbnet-csharp/README.md) et
  [B](../frontiere-vbnet-csharp/README.md)**.
- ⚠️ Le C#-isme le plus sournois **compile sans erreur** : les séquences d'échappement. `"\n"` ou `"C:\\temp"`
  collés dans du VB restent **littéraux** (VB n'échappe rien — Annexe A § A.13). Remplacez `\n` par
  `vbCrLf`/`Environment.NewLine` et `\\` par `\`.
- ⚠️ **Ne laissez pas l'IA inventer des fonctionnalités VB** : si elle prétend que VB déclare des `record` ou
  supporte `await foreach`, c'est faux — vérifiez.

**Convertir et sécuriser :**

- Quand le modèle répond en C# (ce qui arrivera), traitez-le comme un **point de départ** : transposez-le avec
  l'aide-mémoire ([Annexe A](../correspondance-vbnet-csharp/README.md)), ou demandez la conversion **puis
  vérifiez** le résultat.
- ⚠️ **Humain dans la boucle pour la correction et la sécurité** : ne validez jamais à l'aveugle du code de SQL,
  d'authentification ou de cryptographie généré (module 16) — il peut employer des **API obsolètes** ou des
  **schémas non sûrs**.
- Méfiez-vous des suggestions d'une **autre version de .NET** : vérifiez la disponibilité réelle des API en
  **.NET 10**.

**Tirer parti des forces de l'IA en VB** (module 17) : génération de *boilerplate*, de **documentation XML**,
de **tests et mocks**, explication d'erreurs et de *stack traces*, **migration assistée** (VB6 → VB.NET).

> **Modèle de prompt prêt à l'emploi :**
> *« En **VB.NET ciblant .NET 10**, avec `Option Strict On` : [demande]. Donne du **VB.NET idiomatique** (pas une
> traduction littérale du C#) et **n'utilise aucune fonctionnalité absente de VB** (records, `init`,
> `await foreach`, switch expressions, top-level statements). »*

---

## C.7 — Aide-mémoire récapitulatif

| Domaine | À faire ✅ | À éviter ⚠️ |
|---------|-----------|--------------|
| **Compilateur** | `Option Strict/Explicit/Infer On` au niveau projet | `Option Strict Off` global ; `Option Compare Text` global |
| **Nommage** | PascalCase/camelCase .NET ; noms descriptifs ; `_champ` | Notation hongroise VB6 ; `m_` ; abréviations obscures |
| **Organisation** | Séparer UI/métier/données ; DI ; un type par fichier | Métier dans le code-behind ; état global dans un `Module` |
| **`My`** | Productivité de surface ; abstrait derrière une interface si testé | Couplage direct dans le code critique |
| **Documentation** | XML (`'''`) sur l'API publique ; commenter le *pourquoi* | `REM` ; commentaires périmés ; paraphrase du code |
| **Exceptions** | `Try/Catch/Finally` ; filtres `When` ⭐ ; `Throw` seul ; gardes | `On Error Goto/Resume Next` ; `Catch` vide ; `Throw ex` |
| **Journalisation** | `ILogger` injecté ; logs **structurés** ; bon niveau | `MsgBox`/`Debug.Print` ; concaténation ; données sensibles ; double log |
| **IA** 🤖 | Préciser « VB.NET / .NET 10 » ; **valider** ; humain dans la boucle | « C#-ismes » non vérifiés ; ambiguïté VB6/VBA ; code sécurité aveugle |

---

### Voir aussi

- Module 2.1 — [Structure et options (`Option Strict`/`Explicit`/`Infer`/`Compare`)](../../02-fondamentaux-langage/01-structure-options.md)
- Module 2.12 — [L'espace `My`](../../02-fondamentaux-langage/12-espace-my.md) ⭐ ⚠️
- Module 3.6 — [Événements et délégués](../../03-poo/06-evenements-delegues.md) ⭐ (le style idiomatique VB)
- Module 12 — [Exceptions, débogage et journalisation](../../12-exceptions-debogage/README.md)
- Module 16 — [Sécurité des applications](../../16-securite/README.md) (ne pas journaliser de secrets, valider le code IA)
- Module 17 — [Développer en VB.NET avec l'IA](../../17-developpement-ia/README.md) 🤖
- [Annexe A — Correspondance syntaxique VB.NET ↔ C#](../correspondance-vbnet-csharp/README.md) · [Annexe B — Frontière VB.NET / C#](../frontiere-vbnet-csharp/README.md) 🔗

---

**Juin 2026** · .NET 10 LTS · VB.NET 16.9 (stabilisé) · Visual Studio 2026

⏭️ [Raccourcis et astuces Visual Studio 2026](/annexes/visual-studio-2026/README.md)
