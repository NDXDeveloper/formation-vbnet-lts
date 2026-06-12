🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 9.4 WebView2 (intégrer du web moderne dans une application de bureau)

> **Embarquer une interface web moderne — moteur Chromium d'Edge — au cœur d'une application Windows Forms ou WPF, et la faire dialoguer avec votre code VB.NET.**

Les trois sections précédentes tournaient le regard vers le natif (Win32, COM) et vers les autres langages .NET. Celle-ci ferme le chapitre sur la dernière direction d'interopérabilité : le **web moderne**. **WebView2** est un contrôle qui affiche du contenu web (HTML, CSS, JavaScript) à l'intérieur d'une application de bureau, **propulsé par le moteur Chromium de Microsoft Edge**. Il succède à l'ancien contrôle `WebBrowser` (fondé sur le moteur d'Internet Explorer, sans support des standards web actuels) et s'intègre nativement dans **Windows Forms** ⭐ comme dans **WPF** — donc pleinement dans le terrain de jeu de VB.NET.

L'intérêt est double : **afficher** du contenu web riche (tableaux de bord, cartes, documentation, pages de connexion OAuth, éditeurs WYSIWYG), et **communiquer** dans les deux sens entre l'hôte VB.NET et la page web — ce qui en fait un sujet d'interopérabilité à part entière.

---

## Le modèle d'exécution : le runtime WebView2

WebView2 ne réimplémente pas un navigateur : il s'appuie sur un **runtime WebView2** installé sur la machine de l'utilisateur. Le contrôle lui-même provient du paquet NuGet **`Microsoft.Web.WebView2`**. Deux modes de distribution du *runtime* existent :

| Mode | Principe | Quand l'utiliser |
|------|----------|------------------|
| **Evergreen** (recommandé) | *Runtime* **partagé**, mis à jour automatiquement avec Edge | Cas général — **inclus dans Windows 11**, très largement déployé sur Windows 10 à jour |
| **Fixed Version** | Version **figée** du *runtime*, embarquée avec l'application | Contrôle strict de version (conformité, validation, environnements verrouillés) |

> ℹ️ En mode Evergreen, le *runtime* étant largement préinstallé sur les Windows récents, l'effort de déploiement est minime. Pour une robustesse maximale, on s'assure malgré tout de sa présence (programme d'amorçage *bootstrapper* ou installeur autonome fournis par Microsoft).

---

## Démarrer : le contrôle et son initialisation asynchrone

Une fois le contrôle `WebView2` placé sur le formulaire (ou la fenêtre WPF), un point est **incontournable** : l'objet sous-jacent, `CoreWebView2`, qui porte l'essentiel des fonctionnalités, doit être **initialisé de façon asynchrone** avant tout usage.

```vb
Imports Microsoft.Web.WebView2.WinForms

' "webView" est un contrôle WebView2 posé sur le formulaire
Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    Await webView.EnsureCoreWebView2Async()      ' initialisation : obligatoire
    webView.Source = New Uri("https://learn.microsoft.com")
End Sub
```

> ⚠️ **Le piège n°1 de WebView2.** Tant que `EnsureCoreWebView2Async` n'a pas rendu la main, **`webView.CoreWebView2` vaut `Nothing`** : tout accès anticipé (abonnement à un événement, objet hôte, navigation par `CoreWebView2.Navigate`) échoue. On effectue donc l'initialisation dans un gestionnaire **`Async Sub`** (ici `Form1_Load`) et l'on ne configure le reste qu'**après** l'`Await`.

Pour personnaliser l'environnement (dossier de données utilisateur, version du *runtime*, arguments de démarrage), on crée un `CoreWebView2Environment` avec `CoreWebView2Environment.CreateAsync(...)` que l'on passe à `EnsureCoreWebView2Async`. À défaut, un dossier de données par défaut est utilisé.

La navigation se pilote ensuite simplement : `webView.Source = New Uri(...)`, ou `webView.CoreWebView2.Navigate("https://...")`, et l'on suit le cycle de vie via les événements `NavigationStarting`, `NavigationCompleted`, etc.

---

## La communication .NET ↔ JavaScript — le cœur de l'interop 🔗

C'est ce qui inscrit pleinement WebView2 dans ce chapitre : la **communication bidirectionnelle** entre l'hôte VB.NET et le contenu web. Trois mécanismes la composent.

### 1. De .NET vers JavaScript : exécuter du script

`ExecuteScriptAsync` exécute du JavaScript dans la page et renvoie le résultat **sérialisé en JSON** :

```vb
Dim titreJson = Await webView.CoreWebView2.ExecuteScriptAsync("document.title")
```

### 2. De JavaScript vers .NET : la messagerie

Côté page, `window.chrome.webview.postMessage(...)` envoie un message ; côté VB.NET, il est reçu via l'événement **`WebMessageReceived`** :

```vb
' Après initialisation
AddHandler webView.CoreWebView2.WebMessageReceived, AddressOf OnWebMessage

Private Sub OnWebMessage(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)
    Dim message = e.TryGetWebMessageAsString()
    ' ... traiter le message venu de JavaScript ...
End Sub
```

```js
// Côté page web
window.chrome.webview.postMessage("bonjour depuis la page");
```

Dans l'autre sens, `PostWebMessageAsString` / `PostWebMessageAsJson` envoient un message à la page, capté en JS par `window.chrome.webview.addEventListener('message', ...)`.

### 3. Objets hôtes : appeler du .NET depuis JavaScript

Le mécanisme le plus puissant — et le plus délicat — expose un **objet .NET directement à JavaScript** via `AddHostObjectToScript`. La page peut alors appeler ses méthodes.

```vb
Imports System.Runtime.InteropServices

<ComVisible(True)>
Public Class Pont
    Public Function Additionner(a As Integer, b As Integer) As Integer
        Return a + b
    End Function
End Class

' Après initialisation
webView.CoreWebView2.AddHostObjectToScript("pont", New Pont())
```

```js
// Côté page web
const r = await window.chrome.webview.hostObjects.pont.Additionner(2, 3); // 5
```

Notez le `await` : les proxys de `hostObjects` sont **asynchrones** — chaque appel renvoie une *promise*, puisqu'il traverse la frontière de processus entre la page et l'application. Une variante synchrone existe (`window.chrome.webview.hostObjects.sync.pont`), mais elle **bloque** le script pendant l'aller-retour ; la documentation recommande la forme asynchrone.

> 🔗 **Le lien avec la section 9.2.** Les objets hôtes transitent par **COM** : l'objet exposé doit être **`<ComVisible(True)>`**. On retrouve ici, dans un habit moderne, le mécanisme COM décrit pour l'automation Office — l'interopérabilité de ce chapitre forme bien un tout cohérent.

---

## Charger du contenu local

Pour servir des **ressources web embarquées** dans l'application, la bonne pratique est le **mappage de nom d'hôte virtuel** : un dossier local est exposé sous un nom d'hôte fictif, comme s'il provenait d'un serveur.

```vb
webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
    "appassets",
    IO.Path.Combine(Application.StartupPath, "WebAssets"),
    CoreWebView2HostResourceAccessKind.Allow)

webView.Source = New Uri("https://appassets/index.html")
```

Cette approche est préférable à `file://` (elle débloque davantage de fonctionnalités web). Pour un fragment HTML en mémoire, `webView.CoreWebView2.NavigateToString(html)` convient — dans la limite de **2 Mo** prévue par l'API, et avec une page résultante d'origine `about:blank` : dès que le contenu référence d'autres ressources, le mappage d'hôte virtuel reste la meilleure voie.

---

## ⚠️ Sécurité : traiter le contenu web comme non fiable

WebView2 affiche du contenu web, avec tout ce que cela implique. Quelques principes :

- **Restreindre la navigation** : annuler dans `NavigationStarting` les navigations non désirées, pour éviter qu'un lien n'emmène l'utilisateur hors du périmètre prévu.
- **Limiter la surface des objets hôtes** : `AddHostObjectToScript` expose des capacités .NET au web. Ne l'utiliser qu'avec du **contenu de confiance** et n'exposer que le strict nécessaire — un objet hôte trop riche offert à une page distante non maîtrisée est une faille.
- **Cloisonner le contenu distant** : ne pas charger de contenu externe arbitraire avec des objets hôtes actifs ; appliquer les bonnes pratiques web (CSP, même origine) au sein du contenu.

---

## Cas d'usage typiques

- **Flux de connexion OAuth 2.0 / OpenID Connect** : afficher la page de connexion du fournisseur d'identité dans l'application — l'un des usages les plus fréquents.
- **Tableaux de bord et visualisations** : réutiliser des bibliothèques web de graphiques ou de cartographie.
- **Application hybride** : encapsuler une SPA ou un composant web existant dans une coquille de bureau Windows.
- **Documentation, rapports, contenu HTML/Markdown** rendus dans l'application.
- **Éditeurs riches (WYSIWYG)** fondés sur des composants web.

---

## ✅ WebView2 et VB.NET sur .NET 10

Bonne nouvelle dans une formation qui signale régulièrement les barrières C#-only : **WebView2 n'en est pas une**. C'est un **contrôle et une bibliothèque que l'on consomme** — exactement le périmètre de prédilection de VB.NET. Aucun générateur de source, aucune syntaxe réservée à C# : l'API s'utilise en VB sans détour.

- WebView2 est la voie **recommandée** pour embarquer du web dans une application WinForms/WPF sur .NET moderne.
- Le paquet `Microsoft.Web.WebView2` cible le .NET moderne et s'intègre sans friction à une application .NET 10.
- Migrer l'ancien contrôle `WebBrowser` (moteur IE) vers WebView2 (moteur Chromium) est une tâche de **modernisation** courante — la démarche relève du module **11** (moderniser une application *legacy*), la technique est celle de cette section.

---

## En résumé

- **WebView2** embarque du web moderne (moteur **Chromium d'Edge**) dans une application de bureau, en remplacement du `WebBrowser` hérité, avec des contrôles pour **WinForms** ⭐ et **WPF**.
- Il repose sur un **runtime** (mode **Evergreen** recommandé, déjà présent sur Windows récents ; **Fixed Version** pour un contrôle strict).
- L'objet **`CoreWebView2`** doit être **initialisé en asynchrone** (`EnsureCoreWebView2Async`) **avant** tout usage — le piège n°1.
- L'**interopérabilité .NET ↔ JavaScript** se fait par **`ExecuteScriptAsync`**, la **messagerie** (`postMessage` / `WebMessageReceived`) et les **objets hôtes** (`AddHostObjectToScript`, qui exigent **`<ComVisible(True)>`** — lien direct avec la 9.2). 🔗
- **Sécurité** : restreindre la navigation et limiter la surface des objets hôtes au contenu de confiance.
- WebView2 est **pleinement consommable en VB.NET** : pas de barrière C#-only. ✅

---

> 🏁 **Fin du chapitre 9.** L'interopérabilité — du natif (P/Invoke), du COM/Office, des autres langages .NET, jusqu'au web moderne — constitue l'un des points forts les plus **durables** de VB.NET. Le **module 10 — Architecture hybride VB.NET / C#** en fait une véritable stratégie d'architecture ; l'**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)** en dresse le catalogue de référence.

⏭️ [Architecture hybride VB.NET / C# — la stratégie 2026](/10-hybride-vbnet-csharp/README.md)
