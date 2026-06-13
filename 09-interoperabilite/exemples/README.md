# 💻 Exemples du module 9 — Interopérabilité

Quatre directions d'interopérabilité, **toutes compilées et exécutées** (le README du module
n'a pas de code). L'interop est un **point fort durable** de VB.NET : ces exemples le montrent
concrètement, du Windows natif au web moderne, en passant par COM et l'interop inter-langages.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR) · runtime **WebView2 149**.

> 🪟 Toute l'interop de ce chapitre est **Windows-only** (P/Invoke Win32, COM, WebView2) :
> les projets ciblent donc `net10.0-windows` (sauf 9.3, pur .NET, en `net10.0`).

---

## 🔧 Partis pris pour rendre les exemples exécutables et vérifiables

- **9.1** appelle des API **non interactives** (géométrie du bureau, énumération de fenêtres,
  PID, uptime, tailles de structures) pour des sorties **vérifiables** ; `MessageBox` est
  **déclaré** mais appelé seulement avec l'argument `ui` (sinon il bloquerait un test
  automatisé).
- **9.2** : Office n'étant **pas installé** sur la machine de validation, la mécanique COM
  (liaison tardive, RCW, `ReleaseComObject`) est démontrée sur un composant COM **toujours
  présent sous Windows** — `Scripting.FileSystemObject`. Le code d'automation **Excel** est
  inclus, mais **gardé** par une détection de ProgID. Le **CCW** (exposer du .NET à COM) est
  **construit** et l'on vérifie la production du `*.comhost.dll` (l'appel depuis un client COM
  exige un `regsvr32` avec élévation, hors périmètre d'un test automatisé).
- **9.3** est une **solution hybride VB + C#** : c'est le sujet même de la section (VB consomme
  C#). Entièrement exécutable.
- **9.4** pilote WebView2 en mode **auto-test** (`DEMO_AUTOCLOSE=1`) : la page web appelle
  l'objet hôte .NET, renvoie un message, l'hôte lit le titre, journalise dans
  `%TEMP%\webview2-autotest.log`, puis ferme la fenêtre.

C'est le même principe que les modules précédents (SQLite pour SQL Server au module 7,
serveurs in-process au module 8) : on substitue l'**infrastructure** absente, jamais le
**concept** enseigné.

---

## ▶️ Comment compiler et lancer

```bash
# 9.1 — console : sortie directement vérifiable
cd 9.1-pinvoke && dotnet run            # ajouter «-- ui» pour afficher la MessageBox

# 9.2 — console COM (FSO runnable ; Office gardé)
cd 9.2-com-office && dotnet run
# 9.2 — CCW : on construit et on vérifie le comhost.dll produit
cd 9.2-ccw && dotnet build              # -> bin/.../Calculatrice.comhost.dll

# 9.3 — hybride : la console VB référence et consomme la bibliothèque C#
cd 9.3-interop-langages/ClientVb && dotnet run

# 9.4 — WinForms WebView2 (F5 dans VS, ou en auto-test) :
cd 9.4-webview2
#   PowerShell :  $env:DEMO_AUTOCLOSE='1'; dotnet run
#   la fenêtre s'ouvre, exécute la séquence d'interop, journalise puis se ferme.
```

Toutes les valeurs ci-dessous ont été **observées à l'exécution** (certaines — taille d'écran,
nombre de fenêtres, PID, uptime — dépendent de la machine ; elles sont signalées).

---

## 🗂️ Correspondance fichiers du cours → exemples

| Fichier du cours | Exemple | Type | Paquets NuGet (versions validées) |
|---|---|---|---|
| `README.md` (module) | — (aucun code) | | |
| `01-pinvoke.md` | [`9.1-pinvoke`](#91-pinvoke) | Console VB | — (P/Invoke intégré au runtime) |
| `02-com-office.md` | [`9.2-com-office`](#92-com-office) + [`9.2-ccw`](#92-ccw) | Console VB + Lib VB | — (COM intégré ; `EnableComHosting`) |
| `03-interop-langages.md` | [`9.3-interop-langages`](#93-interop-langages) | Hybride VB + C# | — (référence de projet) |
| `04-webview2.md` | [`9.4-webview2`](#94-webview2) | WinForms VB | Microsoft.Web.WebView2 1.0.4022.49 |

---

## 9.1-pinvoke

- **Section** : 9.1 — P/Invoke · **Fichier** : `01-pinvoke.md`
- **Description** : appel d'API Win32 par les **deux syntaxes VB** — `<DllImport>` (close par
  `End Function`) et `Declare` (concise, idiome VB). Couvre le **marshaling de structures**
  (`<StructLayout(Sequential)>` `RECT` ; `<MarshalAs(ByValTStr/ByValArray)>` dans `DeviceInfo`),
  un **callback** (`EnumWindows` + délégué `<UnmanagedFunctionPointer>`), la **gestion d'erreur
  native** (`SetLastError` + `Marshal.GetLastPInvokeError()` + `Win32Exception`), et la
  comparaison d'un appel natif à son équivalent managé (PID).
- **Sortie attendue** (vérifiée ; `*` = dépend de la machine) :
  ```text
  GetCurrentProcessId (natif) = 3160 ; Environment.ProcessId = 3160 ; identiques = True
  Marshal.SizeOf(RECT)       = 16 octets   (attendu 16)
  Marshal.SizeOf(DeviceInfo) = 140 octets  (attendu 140 = 4 + 64×2 + 8)
  GetWindowRect(bureau)      -> 1920 × 1080 px (left=0, top=0)          *
  GetSystemMetrics (Declare) -> écran 1920 × 1080 px ; 2 moniteur(s)    *
  EnumWindows (callback)     -> 204 fenêtre(s) de premier niveau        *
  GetWindowRect(handle nul)  -> échec attendu ; code Win32 = 1400 ; « Handle de fenêtre non valide. »
  GetTickCount64             -> 5857448234 ms d'uptime (> 0)            *
  MessageBox                 -> déclarée (relancer avec l'argument « ui » pour l'afficher)
  ```
- **Comportement vérifié** : PID natif == managé ; tailles marshalées **déterministes**
  (`RECT`=16, `DeviceInfo`=140) ; le callback est invoqué pour chaque fenêtre ; un handle nul
  déclenche bien l'erreur **1400** (`ERROR_INVALID_WINDOW_HANDLE`) traduite en `Win32Exception`.

## 9.2-com-office

- **Section** : 9.2 — COM et automation Office · **Fichier** : `02-com-office.md`
- **Description** : **liaison tardive** COM (`CreateObject` par ProgID, sans référence d'interop),
  avec `Option Strict Off` **limité au fichier** concerné, la règle **« un objet COM = une
  variable »** (deux points) et le nettoyage par **`Marshal.ReleaseComObject`**. Démontré sur
  `Scripting.FileSystemObject` (intégré à Windows) ; le code **Excel** (liaison tardive, `Quit`
  + libération inverse + `GC.Collect`) est présent mais **gardé** selon la présence d'Office.
- **Sortie attendue** (vérifiée) :
  ```text
  [Liaison tardive — composant COM Windows intégré (Scripting.FileSystemObject)]
    CreateObject("Scripting.FileSystemObject") OK
    Fichier écrit : C:\Users\...\Temp\demo-com-vbnet.txt
    Relu : 2 ligne(s) ; FileExists = True
    Supprimé ; FileExists = False
  [Automation Office — Excel en liaison tardive]
    Excel non installé : code montré à titre documentaire, non exécuté.
  ```
- **Comportement vérifié** : l'objet COM est créé, utilisé (écriture/relecture/suppression de
  fichier), et chaque RCW libéré ; l'automation Office se **désactive proprement** faute d'Office.

## 9.2-ccw

- **Section** : 9.2 — Exposer du .NET à COM (CCW) · **Fichier** : `02-com-office.md`
- **Description** : bibliothèque VB rendue consommable par un **client COM** via un *COM Callable
  Wrapper*. Motif recommandé : **interface explicite** `<InterfaceType(InterfaceIsDual)>` +
  `<ClassInterface(None)>` sur la classe + un `<Guid>` **fixe** par type.
  `<EnableComHosting>true</EnableComHosting>` produit le `*.comhost.dll`.
- **Sortie attendue** (vérifiée) :
  ```text
  dotnet build -> bin/Release/net10.0-windows/Calculatrice.comhost.dll  (≈ 184 Ko)
  ```
- **Comportement vérifié** : la build **réussit** et émet `Calculatrice.comhost.dll`.
  **Pour l'utiliser depuis un client COM** : `regsvr32 Calculatrice.comhost.dll` (élévation
  requise — non exécuté ici), puis `CreateObject("Calculatrice.Calculatrice")` côté client.

## 9.3-interop-langages

- **Section** : 9.3 — Interopérabilité entre langages .NET · **Fichier** : `03-interop-langages.md`
- **Description** : **solution hybride** — une bibliothèque **C#** (`BibliothequeCs`, marquée
  `<CLSCompliant(True)>`) et une console **VB** (`ClientVb`) qui la **consomme** par référence de
  projet. Couvre tous les cas de la table « C# → VB » : **record** positionnel (égalité de
  valeur), paramètre **`out` → `ByRef`**, **tuple nommé**, propriétés **`init`-only** réglées par
  `With { }`, **méthode d'extension**, **`async`/`Await`**, **`IAsyncEnumerable`** (parcouru par
  **énumérateur manuel**, sans `Await For Each`) et un membre nommé **`Stop`** (mot-clé VB échappé
  `[Stop]`).
- **Sortie attendue** (vérifiée) :
  ```text
  record Personne : Nom=Alice, Age=30 ; p1.Equals(p2) = True
    ToString() généré : Personne { Nom = Alice, Age = 30 }
  TryParse(out -> ByRef) : « 42 » -> 42
  Tuple nommé : Nom=Alice, Age=30
  record init-only via With {} : Clavier = 49,90 €
  Méthode d'extension : "ab".Repeter(3) = ababab
  Await CalculerAsync() = 42
  IAsyncEnumerable CompterAsync(5) -> somme 1..5 = 15
  Méthode nommée Stop : analyseur.[Stop]() = arrêté
  ```
- **Comportement vérifié** : chaque construction C# est consommée **sans friction** depuis VB —
  l'égalité de valeur du record, le `out` en `ByRef`, l'`init`-only via `With { }` (la
  réassignation serait **BC37311**, en commentaire), l'`IAsyncEnumerable` par énumérateur manuel,
  et l'échappement `[Stop]`.

## 9.4-webview2

- **Section** : 9.4 — WebView2 · **Fichier** : `04-webview2.md`
- **Description** : application **Windows Forms** hébergeant un contrôle **WebView2** (Chromium).
  Initialisation **asynchrone obligatoire** (`EnsureCoreWebView2Async`), contenu local servi par
  **nom d'hôte virtuel** (`SetVirtualHostNameToFolderMapping`), et les **trois voies d'interop
  .NET ↔ JavaScript** : `ExecuteScriptAsync` (.NET → JS), messagerie `postMessage` /
  `WebMessageReceived` (JS → .NET), et **objet hôte** `AddHostObjectToScript` (la page appelle
  `pont.Additionner` — l'objet exposé est **`<ComVisible(True)>`**, lien direct avec la 9.2).
- **Sortie attendue** (vérifiée — `%TEMP%\webview2-autotest.log`) :
  ```text
  EnsureCoreWebView2Async OK ; version=149.0.4022.62
  WebMessageReceived = {"titre":"Demo WebView2 VB.NET","somme":5}
  ExecuteScriptAsync(document.title) = "Demo WebView2 VB.NET"
  ```
- **Comportement vérifié** : l'objet hôte renvoie **`somme=5`** (`Additionner(2,3)`), le message
  JS → .NET est reçu, et `ExecuteScriptAsync` lit le titre (résultat **sérialisé en JSON**, d'où
  les guillemets). La fenêtre se ferme seule en mode auto-test (code de sortie 0).
- **Prérequis** : runtime **WebView2** (Evergreen, inclus dans Windows 11 ; ici v149).

---

## 🧹 Nettoyage des binaires

Les dossiers `bin/` et `obj/` ne sont pas conservés ; ils se régénèrent au premier
`dotnet build` (le paquet WebView2 est restauré depuis le cache). Les fichiers temporaires
(`demo-com-vbnet.txt`, `webview2-autotest.log`) sont créés dans `%TEMP%` et supprimés / écrasés
par les exemples eux-mêmes.

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR) · WebView2 149
