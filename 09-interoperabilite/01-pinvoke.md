🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 9.1 P/Invoke (appel d'API natives Windows, marshaling, callbacks)

> **Appeler du code natif non managé — l'API Win32 ou n'importe quelle DLL C/C++ — directement depuis VB.NET.**

**P/Invoke** (*Platform Invocation Services*) est le mécanisme par lequel du code managé .NET appelle des fonctions exportées par des bibliothèques **natives** (non managées), au premier rang desquelles l'**API Windows**. C'est l'une des plus anciennes formes d'interopérabilité, héritée de l'époque de VB6 et de ses `Declare`, et elle reste indispensable dès qu'aucune API .NET managée ne couvre un besoin précis.

---

## Quand utiliser P/Invoke — et quand l'éviter

Avant d'écrire la moindre déclaration native, il faut se poser la bonne question : **existe-t-il déjà une API managée ?** Au fil des versions, .NET a fini par envelopper l'immense majorité des fonctionnalités Windows courantes (fichiers, registre, réseau, processus, fenêtres via WinForms/WPF…). Recourir à P/Invoke quand une classe du BCL fait déjà le travail ajoute de la complexité et des risques pour rien.

P/Invoke est **justifié** dans les cas suivants :

- une fonction de l'API Windows **sans équivalent managé** (réglages système pointus, fonctions shell avancées, API matérielles…) ;
- une **DLL native tierce** (pilote, SDK C/C++, bibliothèque scientifique) que l'on doit piloter ;
- un besoin de **performance** où l'on veut court-circuiter une couche managée.

> ⚠️ **Un coût réel.** Chaque appel P/Invoke franchit la frontière managé ↔ non managé : marshaling des paramètres, transition du *runtime*, perte des garanties de sûreté mémoire du CLR. Un dépassement de tampon dans une structure mal déclarée provoque un crash natif, pas une exception propre. À réserver aux cas où le bénéfice est clair.

---

## Deux façons de déclarer un appel natif en VB.NET

VB.NET propose **deux syntaxes** pour déclarer une fonction native. C'est une particularité du langage : l'une lui est propre, l'autre est partagée par tout l'écosystème .NET.

### L'instruction `Declare` — l'idiome historique de VB

Directement héritée de Visual Basic, l'instruction `Declare` tient sur une seule ligne logique et ne nécessite **pas** de bloc `End Function` :

```vb
' Déclaration concise, à la mode VB
Public Declare Auto Function MessageBox Lib "user32.dll" (
    hWnd As IntPtr,
    lpText As String,
    lpCaption As String,
    uType As UInteger) As Integer
```

Le mot-clé de jeu de caractères (`Ansi`, `Unicode` ou `Auto`) se place juste après `Declare`. **Sans mot-clé, le défaut est `Ansi`** — et, particularité de `Declare` en VB, la recherche de l'export se fait alors à l'orthographe **exacte** (`ExactSpelling` vaut `True` par défaut) : `MessageBox` serait introuvable dans `user32.dll`, qui n'exporte que `MessageBoxA` et `MessageBoxW`. En pratique, on choisit donc presque toujours **`Auto`** sur Windows moderne : le *runtime* résout alors l'export Unicode (`MessageBoxW`) automatiquement.

### L'attribut `<DllImport>` — la voie commune à tout .NET

L'attribut `<DllImport>` (de `System.Runtime.InteropServices`) s'applique à une méthode **sans corps**. En VB, la signature doit malgré tout être close par `End Function` / `End Sub` — c'est une petite singularité du langage, le compilateur sachant que l'implémentation est fournie par la DLL :

```vb
Imports System.Runtime.InteropServices

Friend Module NativeMethods
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Function MessageBox(
        hWnd As IntPtr,
        lpText As String,
        lpCaption As String,
        uType As UInteger) As Integer
    End Function
End Module
```

> 💡 **Convention.** Par usage, on regroupe toutes les déclarations natives dans un type dédié nommé **`NativeMethods`** (souvent `Friend`). Ici un `Module` — où les méthodes sont implicitement `Shared` — ce qui évite d'écrire `Shared` sur chaque fonction. Dans une `Class`, il faudrait les déclarer `Public Shared` / `Friend Shared`.

### `Declare` ou `<DllImport>` : lequel choisir ?

| Critère | `Declare` | `<DllImport>` |
|---------|-----------|----------------|
| Origine | Spécifique à VB (héritage VB6) | Commune à tout .NET (C#, F#, VB) |
| Concision | ✅ Une ligne, pas de `End Function` | Bloc avec `End Function` |
| Contrôle fin | Limité | ✅ `SetLastError`, `CallingConvention`, `EntryPoint`, `ExactSpelling`, `PreserveSig`… |
| Présence dans la doc / l'IA | Rare | ✅ Omniprésent (tout est documenté en C#) |
| Recommandation 2026 | Cas très simples | **Cas non triviaux et par défaut** |

En pratique, **`<DllImport>` est à privilégier** dès qu'on a besoin de `SetLastError`, d'une convention d'appel spécifique ou d'options de marshaling — et parce que c'est la forme que l'on rencontre dans 100 % de la documentation et du code généré (voir plus bas la note sur l'IA). L'attribut accepte notamment :

- `EntryPoint` — pointer vers un export dont le nom diffère du nom VB ;
- `CallingConvention` — `Winapi` (par défaut, soit `StdCall` sur Windows) pour l'API Win32, `Cdecl` pour beaucoup de bibliothèques C ;
- `ExactSpelling:=True` — désactiver la recherche automatique des suffixes `A`/`W` ;
- `SetLastError:=True` — capturer le code d'erreur Win32 (voir la section dédiée).

---

## Le marshaling : faire passer les données à travers la frontière

Le **marshaling** est la conversion des données entre leur représentation managée et leur représentation native. C'est le cœur — et la principale source de bugs — de P/Invoke.

### Types blittables vs non blittables

Un type est dit **blittable** lorsqu'il a **la même représentation en mémoire** des deux côtés de la frontière : aucune conversion n'est nécessaire, le passage est direct et rapide.

| Catégorie | Types |
|-----------|-------|
| ✅ **Blittables** | `Byte`, `SByte`, `Short`, `UShort`, `Integer`, `UInteger`, `Long`, `ULong`, `Single`, `Double`, `IntPtr`, `UIntPtr`, ainsi que les tableaux à une dimension et les structures composés **uniquement** de types blittables |
| ⚠️ **Non blittables** | `Boolean` (taille variable selon le contexte), `Char` / `String` (dépend du jeu de caractères), `Object`, certains tableaux et structures |

Concevoir ses structures et signatures pour rester **blittable** quand c'est possible améliore nettement les performances des appels intensifs.

### Les chaînes et le jeu de caractères

Les API Windows existent souvent en deux variantes : **ANSI** (suffixe `A`) et **Unicode/UTF-16** (suffixe `W`). Le paramètre `CharSet` indique au *runtime* laquelle utiliser et comment convertir les `String` :

- `CharSet.Unicode` — variante `W`, recommandée sur Windows moderne (tout y est Unicode) ;
- `CharSet.Ansi` — variante `A`, pour les anciennes DLL ANSI ; c'est le **défaut quand on omet `CharSet`**, héritage historique source de caractères accentués mutilés ;
- `CharSet.Auto` — le *runtime* choisit selon la plateforme (Unicode sur Windows).

Pour un contrôle au cas par cas, l'attribut `<MarshalAs>` précise le format exact d'une chaîne : `UnmanagedType.LPWStr` (pointeur UTF-16), `LPStr` (ANSI), `BStr` (chaîne COM), etc.

### Les structures : `<StructLayout>`

Pour passer une structure à du code natif, il faut **fixer la disposition de ses champs** en mémoire avec `<StructLayout>` :

```vb
<StructLayout(LayoutKind.Sequential)>
Public Structure RECT
    Public Left As Integer
    Public Top As Integer
    Public Right As Integer
    Public Bottom As Integer
End Structure

<DllImport("user32.dll", SetLastError:=True)>
Friend Function GetWindowRect(hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
End Function
```

- `LayoutKind.Sequential` — les champs sont disposés dans l'ordre de déclaration (le cas courant) ;
- `LayoutKind.Explicit` — combiné à `<FieldOffset(n)>` sur chaque champ, pour reproduire des **unions** C ou un placement précis ;
- `Pack` — contrôle l'alignement des champs lorsque la DLL l'exige.

Au sein d'une structure, `<MarshalAs>` gère les cas particuliers, notamment les **chaînes et tableaux de taille fixe** :

```vb
<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
Public Structure DeviceInfo
    Public Id As Integer

    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
    Public Name As String        ' tampon de chaîne inline de 64 caractères

    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)>
    Public Flags As Byte()       ' tableau inline de 8 octets
End Structure
```

### `ByRef`, pointeurs et `IntPtr`

- En VB, **`ByRef`** se traduit naturellement par un pointeur natif (`T*`) — idéal pour les paramètres « de sortie » comme le `lpRect` ci-dessus.
- **`IntPtr`** (et `UIntPtr`) représente un pointeur ou un *handle* natif de taille dépendante de la plateforme (32 ou 64 bits).

> ⚠️ **Différence VB ↔ C# à connaître.** C# dispose des mots-clés `nint` / `nuint` ; **VB.NET ne les possède pas** et utilise directement **`IntPtr` / `UIntPtr`**. Lorsqu'on transpose un exemple C# qui emploie `nint`, on le remplace par `IntPtr` côté VB.

### Marshaling manuel : la classe `Marshal`

Pour les scénarios où le marshaling automatique ne suffit pas, la classe `Marshal` offre une boîte à outils : `Marshal.AllocHGlobal` / `FreeHGlobal` (allocation native), `Marshal.PtrToStructure(Of T)` et `Marshal.StructureToPtr` (structures ↔ mémoire), `Marshal.Copy`, `Marshal.StringToHGlobalUni`, `Marshal.SizeOf(Of T)`… On y recourt notamment pour manipuler des tampons ou des tableaux de structures de taille variable.

---

## Les callbacks : quand le code natif rappelle votre code

Certaines API Win32 fonctionnent par **rappel** (*callback*) : on leur passe une fonction que le code natif appellera lui-même, en boucle ou de façon différée. Côté VB.NET, on déclare un **délégué** dont la signature correspond exactement au callback attendu, et l'attribut `<UnmanagedFunctionPointer>` fixe sa convention d'appel :

```vb
' Signature du callback attendu par EnumWindows
<UnmanagedFunctionPointer(CallingConvention.StdCall)>
Public Delegate Function EnumWindowsProc(hWnd As IntPtr, lParam As IntPtr) As Boolean

<DllImport("user32.dll", SetLastError:=True)>
Friend Function EnumWindows(callback As EnumWindowsProc, lParam As IntPtr) As Boolean
End Function
```

Utilisation : le délégué est construit avec `AddressOf` et passé à l'API.

```vb
Private Function OnWindow(hWnd As IntPtr, lParam As IntPtr) As Boolean
    ' ... traiter la fenêtre hWnd ...
    Return True  ' renvoyer True pour poursuivre l'énumération
End Function
```

> ⚠️ **Le piège classique : garder le délégué en vie.** Le ramasse-miettes (GC) ne « voit » pas la référence détenue côté natif. Si le code natif **conserve** le callback pour l'appeler plus tard (de manière asynchrone), et que le délégué managé est entre-temps collecté, l'appel suivant **plante**. La règle :
> - **callback synchrone** (comme `EnumWindows`, qui rend la main une fois l'énumération terminée) : une variable locale suffit, le délégué survit à l'appel ;
> - **callback stocké / asynchrone** : il faut **conserver une référence vivante** — un champ de classe, ou un `GCHandle` explicite — tant que le natif peut encore l'invoquer.

---

## Gérer les erreurs natives

Les fonctions Win32 ne lèvent pas d'exception : elles renvoient un code (souvent `False` ou `0`) et déposent le détail de l'erreur dans une variable propre au thread. Pour la récupérer correctement :

1. déclarer l'import avec **`SetLastError:=True`** ;
2. lire le code **immédiatement** après l'appel, avec **`Marshal.GetLastPInvokeError()`** ;
3. le transformer en exception lisible via **`Win32Exception`** (de `System.ComponentModel`).

```vb
Imports System.ComponentModel
Imports System.Runtime.InteropServices

Dim rect As RECT
If Not NativeMethods.GetWindowRect(handle, rect) Then
    Dim code = Marshal.GetLastPInvokeError()
    Throw New Win32Exception(code)   ' message d'erreur Windows lisible
End If
```

> ⚠️ **Lire l'erreur sans délai.** Toute instruction managée intercalée entre l'appel natif et la lecture peut écraser le « dernier code d'erreur » du thread. On capture donc le code **juste après** l'appel.
>
> 💡 `Marshal.GetLastPInvokeError()` (depuis .NET 6) est le nom moderne et explicite ; `Marshal.GetLastWin32Error()` reste disponible et équivalent.

---

## Gérer les ressources natives : `SafeHandle`

Lorsqu'une API renvoie un *handle* natif (fichier, événement, descripteur système…), le représenter par un simple `IntPtr` expose à deux problèmes : les **fuites** (oubli de libération) et le **recyclage de handle** (un handle libéré puis réattribué entre-temps). La bonne pratique est de s'appuyer sur un type dérivé de **`SafeHandle`** (par exemple `SafeFileHandle`), qui :

- encapsule le handle et garantit sa **libération déterministe** ;
- protège contre le recyclage et coopère proprement avec le GC ;
- s'utilise naturellement dans un bloc `Using`.

On préférera donc déclarer les imports concernés avec un type `SafeHandle` plutôt qu'un `IntPtr` brut chaque fois qu'un cycle de vie de ressource est en jeu.

---

## P/Invoke et le .NET moderne (.NET 10)

Les fondamentaux de P/Invoke décrits ci-dessus sont **stables** et pleinement opérationnels sur .NET 10. Quelques évolutions de l'écosystème méritent toutefois d'être situées — et certaines confirment, une fois encore, la pertinence de la **stratégie hybride** de cette formation.

- **Le générateur `LibraryImport`** 🔗 — depuis .NET 7, C# dispose de l'attribut `[LibraryImport]`, qui génère le code de marshaling **à la compilation** (gain de performance, compatibilité Native AOT) au lieu de le produire au *runtime* comme `<DllImport>`. Mais c'est un **générateur de source qui émet du C#** : il n'est **pas exploitable depuis un projet VB**. En VB.NET, on s'en tient donc à `<DllImport>` / `Declare`. Si les bénéfices de `LibraryImport` (notamment l'AOT) sont réellement nécessaires, on **isole la couche P/Invoke dans une bibliothèque C#** que l'on consomme depuis VB.NET — exactement le schéma du **module 10** et de l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**.
- **Native AOT non pertinent pour VB** — VB.NET ne cible pas Native AOT en pratique (Annexe B). L'avantage AOT de `LibraryImport` n'est donc, de toute façon, pas atteignable depuis du VB pur : un argument de plus pour la délégation à C# si ce besoin se présente.
- **La classe `NativeLibrary`** — pour charger une bibliothèque et résoudre ses exports **dynamiquement au runtime** (`NativeLibrary.Load`, `NativeLibrary.GetExport`), plutôt que par déclaration statique.
- **Sécurité du chargement** — l'attribut `<DefaultDllImportSearchPaths>` restreint les répertoires dans lesquels les DLL natives sont recherchées, pour réduire la surface d'attaque (chargement de DLL malveillante).

---

## ⚠️ 🤖 P/Invoke et le code généré par IA

P/Invoke est l'un des terrains où les assistants IA se trompent le plus souvent en VB.NET, et où une **validation systématique** s'impose. Les écueils récurrents :

- la signature native proposée est **inexacte** (mauvais type, mauvaise convention d'appel) — à vérifier contre une source de référence fiable des API Windows ;
- le code mélange des **mots-clés C#** inapplicables en VB (`nint`, `[LibraryImport]`, `out`/`ref` au lieu de `ByRef`) ;
- les **dispositions de structure** (`StructLayout`, `Pack`, `MarshalAs`) sont approximatives, ce qui se traduit par des crashs natifs silencieux.

La règle d'or du module 17 s'applique pleinement ici : préciser « **Visual Basic .NET** » et la version .NET cible, puis **relire et tester** chaque déclaration native plutôt que de l'exécuter telle quelle.

---

## En résumé

- P/Invoke appelle du **code natif** (API Win32, DLL C/C++) depuis VB.NET — à réserver aux cas **sans équivalent managé**, car il franchit la frontière de sûreté du CLR.
- VB offre **deux syntaxes** : `Declare` (concise, idiome VB) et `<DllImport>` (commune à tout .NET, plus de contrôle, à privilégier).
- Le **marshaling** est central : viser des types **blittables**, fixer le `CharSet` des chaînes, et disposer les structures avec `<StructLayout>` et `<MarshalAs>`.
- Les **callbacks** passent par un **délégué** ; ne jamais oublier de **garder la référence en vie** lorsque le natif le conserve.
- Les erreurs se récupèrent avec `SetLastError:=True` + `Marshal.GetLastPInvokeError()` + `Win32Exception`, **immédiatement** après l'appel.
- Sur .NET 10, le chemin moderne `LibraryImport` est **C#-only** : besoin d'AOT ou de marshaling généré → **isoler en C#** et consommer depuis VB (modules 10 et Annexe B).

> 🔗 **Suite logique** : la section **9.2 — COM et automation Office** prolonge l'interopérabilité native vers l'univers COM, terrain historique de Visual Basic.

⏭️ [COM et automation Office (Excel, Word, Outlook) — force historique de VB](/09-interoperabilite/02-com-office.md)
