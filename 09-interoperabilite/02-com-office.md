🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 9.2 COM et automation Office (Excel, Word, Outlook) — force historique de VB ⭐

> **Piloter Excel, Word ou Outlook, et consommer n'importe quel composant COM : le terrain où Visual Basic est chez lui depuis trente ans.**

Le **COM** (*Component Object Model*) est le standard binaire de composants de Microsoft, antérieur à .NET. Il sous-tend tout l'écosystème de Visual Basic 6 et de VBA, et c'est sur lui que reposent les **modèles objet d'Office**. Automatiser Office depuis VB.NET prolonge donc une filiation directe — VB6 → VBA → VB.NET — qui fait de l'interop COM l'un des points forts les plus **idiomatiques et durables** du langage, là où d'autres écosystèmes doivent fournir un effort supplémentaire pour le même résultat.

---

## COM, RCW et CCW : les fondations

Le CLR ne « parle » pas COM nativement : il s'appuie sur des **wrappers** (enveloppes) qui font le pont entre le monde managé et le monde COM, dans les deux sens.

- **RCW — *Runtime Callable Wrapper*** : lorsqu'un code managé **consomme** un objet COM, le CLR interpose un RCW, un proxy managé qui enveloppe l'objet COM. Le RCW gère pour vous le **comptage de références** COM (`AddRef`/`Release` de `IUnknown`), le marshaling des appels et le cycle de vie. Côté VB.NET, on manipule le RCW comme un objet .NET ordinaire. **C'est le cas de figure de l'automation Office** : on consomme les objets COM d'Excel, Word ou Outlook.

- **CCW — *COM Callable Wrapper*** : le sens inverse. Lorsqu'un **client COM** (une macro VBA, une vieille application VB6, un composant natif) veut utiliser un objet **.NET**, le CLR génère un CCW qui présente l'objet managé sous la forme d'un objet COM. On y revient en fin de section (exposer du .NET à COM).

```
   VB.NET  ──▶  [ RCW ]  ──▶  Objet COM        (consommer COM : Office)
   Client COM  ──▶  [ CCW ]  ──▶  Objet .NET    (exposer du .NET à COM)
```

---

## Consommer un composant COM en VB.NET

Pour utiliser un composant COM en **liaison anticipée**, on ajoute au projet une **référence COM** vers sa bibliothèque de types (par exemple *« Microsoft Excel Object Library »*). L'outillage génère alors une **assembly d'interop** qui expose les types COM sous forme de types .NET.

Deux notions clés :

- **PIA (*Primary Interop Assembly*)** — l'assembly d'interop « officielle » d'un composant, historiquement déployée à part. Les PIA d'Office (`Microsoft.Office.Interop.Excel`, etc.) sont issues du monde .NET Framework.
- **Types d'interop incorporés (*Embed Interop Types*)** — la fonctionnalité « no-PIA » : seuls les types réellement utilisés sont **incorporés** dans votre assembly, supprimant la dépendance de déploiement à la PIA. C'est l'approche **recommandée** ; sur l'outillage moderne, une référence COM active `EmbedInteropTypes` par défaut.

> ℹ️ **Sur .NET moderne (.NET 10)**, l'interop COM fonctionne **uniquement sous Windows**. Pour Office, la machine d'exécution doit en outre disposer de l'application Office installée — l'automation pilote le vrai logiciel, elle ne le réimplémente pas.

---

## Early binding vs late binding

VB.NET offre **deux modes** pour dialoguer avec un objet COM. Le choix entre les deux est un arbitrage classique — et une particularité que VB rend exceptionnellement ergonomique.

### Liaison anticipée (*early binding*)

On référence l'assembly d'interop à la compilation. Le compilateur connaît les types (`Excel.Application`, `Excel.Workbook`…), on bénéficie d'**IntelliSense**, de la **vérification de type à la compilation** et de **meilleures performances**.

```vb
Imports Excel = Microsoft.Office.Interop.Excel

Dim app As New Excel.Application()
app.Visible = True
Dim book As Excel.Workbook = app.Workbooks.Add()
Dim sheet As Excel.Worksheet = CType(book.ActiveSheet, Excel.Worksheet)
sheet.Range("A1").Value = "Bonjour"
```

### Liaison tardive (*late binding*)

On manipule des `Object` et c'est le **runtime** qui résout les membres via le mécanisme COM `IDispatch`, au moment de l'appel. Aucune référence d'interop n'est nécessaire : on instancie le composant par son **ProgID** avec `CreateObject`.

```vb
' En tête de fichier : Option Strict Off
Dim app As Object = CreateObject("Excel.Application")
app.Visible = True
Dim book As Object = app.Workbooks.Add()
Dim sheet As Object = book.ActiveSheet
sheet.Range("A1").Value = "Bonjour"
```

La liaison tardive **exige `Option Strict Off`**, car elle repose sur la résolution implicite de membres sur des `Object` — précisément ce que `Option Strict On` interdit.

> 💡 **Restreindre `Option Strict Off` à un seul fichier.** Plutôt que de désactiver `Option Strict` pour tout le projet, on place la directive **`Option Strict Off` en première ligne du fichier** concerné. Le reste de la solution conserve ainsi `Option Strict On` et ses garanties.

### Tableau de décision

| Critère | Early binding | Late binding |
|---------|---------------|--------------|
| Référence d'interop | Requise (à la compilation) | Aucune (`CreateObject`) |
| Vérification de type | ✅ À la compilation | ❌ Au *runtime* seulement |
| IntelliSense | ✅ Oui | ❌ Non |
| Performance | ✅ Meilleure | Plus lente (résolution `IDispatch`) |
| `Option Strict` | Compatible `On` | Exige **`Off`** |
| Dépendance à la version d'Office | Liée à la version référencée | ✅ Indépendante (version installée) |
| Détection des erreurs | À la compilation | À l'exécution |

**En pratique :** privilégier la **liaison anticipée** pour la maintenabilité et la sûreté ; réserver la **liaison tardive** aux cas où l'on veut un code **indépendant de la version d'Office** ou éviter toute dépendance d'interop. Le fait que VB rende la liaison tardive aussi naturelle (`CreateObject`, `Option Strict Off` ciblé) est une de ses forces historiques — là où C# a dû attendre l'arrivée du type `dynamic` pour offrir un équivalent.

> 💡 `CreateObject("Excel.Application")` **lance** une nouvelle instance. Pour s'**attacher à une instance déjà ouverte**, on utilise `GetObject(, "Excel.Application")` ; pour ouvrir un fichier via son serveur COM associé, `GetObject("C:\...\classeur.xlsx")`.

---

## Automatiser Office en pratique

### Excel

Le scénario le plus courant : générer ou lire un classeur, parcourir des plages, appliquer des formats. Les exemples ci-dessus en montrent l'essentiel ; le modèle objet expose ensuite `Range`, `Cells`, `Worksheets`, `Charts`, etc.

### Word

Même logique avec le modèle objet de Word :

```vb
Dim word As Object = CreateObject("Word.Application")
word.Visible = True
Dim doc As Object = word.Documents.Add()
doc.Content.Text = "Bonjour depuis VB.NET"
doc.SaveAs2("C:\Temp\rapport.docx")
```

### Outlook

Outlook est un classique pour envoyer du courrier ou créer des rendez-vous depuis une application métier :

```vb
Dim outlook As Object = CreateObject("Outlook.Application")
Dim mail As Object = outlook.CreateItem(0)   ' 0 = olMailItem
mail.To = "destinataire@exemple.fr"
mail.Subject = "Compte rendu"
mail.Body = "Bonjour, veuillez trouver ci-joint le compte rendu."
mail.Send()
```

> ⚠️ **Garde de sécurité d'Outlook.** Selon la configuration (politique d'administration, antivirus à jour), l'accès programmatique à certaines propriétés ou l'envoi automatique peuvent **déclencher des invites de sécurité**. À tester dans l'environnement cible avant tout déploiement.

---

## ⚠️ Le piège majeur : libérer les objets COM

C'est **l'écueil n°1** de l'automation Office. Chaque objet COM manipulé — `Application`, `Workbook`, `Worksheet`, `Range`, mais aussi tout objet **intermédiaire** créé au passage — donne lieu à un RCW qui détient une référence COM vivante. Si ces références ne sont pas libérées, le **processus Office reste en mémoire** : ce sont les fameux processus `EXCEL.EXE` « fantômes » qui s'accumulent.

### La règle des « deux points »

Un appel chaîné comme `app.Workbooks.Add()` crée un RCW intermédiaire (`Workbooks`) dont on ne garde **aucune référence** : impossible de le libérer ensuite.

```vb
' ❌ À éviter : le RCW de "Workbooks" est perdu, donc non libérable
Dim book = app.Workbooks.Add()

' ✅ Préférable : un objet COM = une variable
Dim books = app.Workbooks
Dim book = books.Add()
' ... books et book pourront être libérés explicitement
```

### Le nettoyage explicite

On libère chaque objet avec `Marshal.ReleaseComObject` (ou `FinalReleaseComObject` pour forcer le compteur à zéro), **dans l'ordre inverse** de leur création, après avoir refermé l'application via `Quit` :

```vb
Imports System.Runtime.InteropServices

app.Quit()

Marshal.ReleaseComObject(sheet)
Marshal.ReleaseComObject(book)
Marshal.ReleaseComObject(books)
Marshal.ReleaseComObject(app)

sheet = Nothing : book = Nothing : books = Nothing : app = Nothing

GC.Collect()
GC.WaitForPendingFinalizers()
GC.Collect()
GC.WaitForPendingFinalizers()
```

> ℹ️ **Deux écoles cohabitent** : (a) libérer méticuleusement chaque objet avec `ReleaseComObject` en proscrivant les appels chaînés, ou (b) s'en remettre au GC en provoquant explicitement un cycle de collecte après avoir mis les références à `Nothing`. Dans les deux cas, le problème des **RCW intermédiaires** demeure : la discipline « un objet COM, une variable » reste la précaution de fond.

---

## ⚠️ Limite honnête : pas d'automation Office côté serveur

Aussi pratique soit-elle, l'automation Office est conçue pour un **poste client interactif**, où Office est installé et une session utilisateur présente. Microsoft **déconseille de longue date** l'automation Office **côté serveur** ou en contexte non surveillé (service Windows, application web, traitement par lot non interactif) : instabilité, blocages sur des boîtes de dialogue, problèmes de licence et de concurrence.

Pour **générer des documents côté serveur**, on privilégie des bibliothèques qui n'exigent pas Office :

- **OpenXML SDK** (`DocumentFormat.OpenXml`) — création/lecture directe des formats `.xlsx` / `.docx` / `.pptx` ;
- d'autres bibliothèques .NET dédiées au tableur ou au document.

Ces bibliothèques étant de simples paquets .NET, elles s'inscrivent pleinement dans le périmètre « consommation » de VB.NET.

---

## Exposer du .NET à COM via un CCW

Le cas inverse — rendre une classe **.NET utilisable depuis un client COM** (macro VBA, application VB6 ou native) — passe par un **CCW**. On décore la classe avec les attributs COM appropriés :

```vb
Imports System.Runtime.InteropServices

' Le contrat COM : une interface explicite, exposée en double (IDispatch + vtable)
<ComVisible(True)>
<Guid("9C2E6A14-3D7B-4F58-A1C0-5B8E2F7D9A31")>
<InterfaceType(ComInterfaceType.InterfaceIsDual)>
Public Interface ICalculatrice
    Function Additionner(a As Integer, b As Integer) As Integer
End Interface

' La classe : visible de COM, mais SANS interface de classe auto-générée
<ComVisible(True)>
<Guid("E5C9F7A2-8B4D-4E16-9F3A-7C1D5E8B2F64")>
<ClassInterface(ClassInterfaceType.None)>
Public Class Calculatrice
    Implements ICalculatrice

    Public Function Additionner(a As Integer, b As Integer) As Integer _
        Implements ICalculatrice.Additionner
        Return a + b
    End Function
End Class
```

Le motif recommandé combine ainsi une **interface explicite** (décorée `<InterfaceType>`) et `ClassInterfaceType.None` sur la classe : le client COM ne voit que le contrat de l'interface, stable et versionnable — et non une interface de classe générée automatiquement, fragile au moindre changement. Chaque type porte son propre **`<Guid>`** fixe : on en génère un par type (menu **Outils ▸ Créer un GUID** de Visual Studio, ou `Guid.NewGuid()` une fois pour toutes) plutôt que de laisser l'outillage en dériver un, afin que l'identité COM ne change pas d'une compilation à l'autre.

> ℹ️ **Sur .NET moderne**, l'hébergement COM se configure dans le projet via la propriété **`<EnableComHosting>true</EnableComHosting>`**, qui produit un fichier `*.comhost.dll` permettant l'activation COM du composant — en remplacement de l'enregistrement par `regasm` du monde .NET Framework.

---

## COM et le .NET moderne (.NET 10)

- L'interop COM, RCW comme CCW, est **prise en charge mais limitée à Windows** — sans incidence pour les scénarios cœur de VB.NET, par nature centrés sur le bureau Windows.
- L'incorporation des types d'interop (**`EmbedInteropTypes`**) est l'approche par défaut et recommandée : pas de PIA à déployer.
- L'hébergement COM des composants .NET s'appuie sur **`EnableComHosting`** plutôt que sur `regasm`.
- `Marshal.ReleaseComObject` / `FinalReleaseComObject` restent disponibles sous Windows (et lèvent une exception sur les autres plateformes — cohérent avec la restriction Windows-only).

---

## ⚠️ 🤖 COM, Office et code généré par IA

L'automation Office est un terrain où les assistants IA produisent souvent du code **fonctionnel mais fragile**. Les écueils typiques :

- des **appels chaînés** (la règle des « deux points » non respectée) qui laissent fuir les processus Office ;
- des références à d'**anciennes PIA** ou à des versions d'Office obsolètes ;
- des constantes énumérées en dur (`0` pour `olMailItem`) sans le contexte expliquant leur signification.

Comme ailleurs, on **précise « Visual Basic .NET »** et la version cible, et on **relit le code de libération des objets** avant toute mise en production.

---

## En résumé

- Le COM est le socle historique de Visual Basic ; **automatiser Office** (Excel, Word, Outlook) depuis VB.NET en est l'usage emblématique et un vrai point fort ⭐.
- Le CLR fait le pont via des **wrappers** : **RCW** pour consommer du COM (le cas Office), **CCW** pour exposer du .NET à COM.
- Deux modes : **liaison anticipée** (sûre, IntelliSense, performante — à privilégier) et **liaison tardive** (`CreateObject`, indépendante de la version, mais **exige `Option Strict Off`**, idéalement limité à un fichier).
- **Toujours libérer les objets COM** : « un objet COM, une variable » (règle des deux points), `Quit` puis `ReleaseComObject` — sous peine de processus Office fantômes.
- **Pas d'automation Office côté serveur** : pour la génération non interactive, utiliser **OpenXML SDK** ou une bibliothèque dédiée.
- Sur **.NET 10**, l'interop COM est **Windows-only**, avec `EmbedInteropTypes` et `EnableComHosting` comme réglages de référence.

> 🔗 **Suite logique** : la section **9.3 — Interopérabilité entre langages .NET** quitte le monde natif/COM pour l'interop **VB ↔ C# (et F#)**, fondation de l'architecture hybride du module 10.

⏭️ [Interopérabilité entre langages .NET (VB ↔ C#, F#)](/09-interoperabilite/03-interop-langages.md)
