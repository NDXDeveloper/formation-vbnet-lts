🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.12 Nouveautés Windows Forms .NET 10 🆕

La section [5.2](02-winforms-net10.md) a déjà présenté trois modernisations majeures (mode sombre, formulaires asynchrones, sécurité du presse-papiers). Cette section complète le panorama avec les nouveautés **restantes** de .NET 10 : l'**échange de données JSON typé** (presse-papiers *et* glisser-déposer), l'API anti-capture **`Form.FormScreenCaptureMode`**, et les **éditeurs de concepteur** reportés depuis .NET Framework — auxquelles s'ajoutent quelques changements notables (analyseurs, méthodes obsolètes).

---

## Le presse-papiers et le glisser-déposer typés (JSON)

### Rappel et nouveauté

La section [5.2](02-winforms-net10.md) a expliqué le contexte : le retrait de `BinaryFormatter` (.NET 9) a cassé l'échange d'objets personnalisés, que .NET 10 restaure via des API **type-safe** fondées sur JSON. Deux compléments importants ici :

- le code du presse-papiers a été **repensé pour être partagé avec WPF** : les deux technologies de bureau utilisent désormais le même code et unifient leur façon d'interagir avec le presse-papiers ;
- le même mécanisme s'étend au **glisser-déposer**, grâce à une nouvelle interface.

.NET 10 introduit trois familles d'API : **`TryGetData` / `TryGetData(Of T)`** (récupération typée), **`SetDataAsJson(Of T)`** (stockage avec sérialisation JSON via `System.Text.Json`), et l'interface **`ITypedDataObject`** pour le glisser-déposer.

### Presse-papiers : écrire et lire

```vb
' Un type métier simple (classe, record ou structure à propriétés publiques)
Public Class Personne
    Public Property Nom As String
    Public Property Age As Integer
End Class
```

```vb
' Écriture : sérialisation JSON typée
Dim personne As New Personne With {.Nom = "Alice", .Age = 30}
Clipboard.SetDataAsJson("MonApp.Personne", personne)

' Lecture : récupération typée ; le type est inféré de la variable passée
Dim recuperee As Personne = Nothing
If Clipboard.TryGetData("MonApp.Personne", recuperee) Then
    Console.WriteLine(recuperee.Nom)
End If
```

En utilisant le **nom de type** comme format, `TryGetData(Of T)` peut même inférer le format automatiquement :

```vb
Clipboard.SetDataAsJson(GetType(Personne).FullName, personne)

Dim p As Personne = Nothing
If Clipboard.TryGetData(p) Then    ' format déduit du type
    Console.WriteLine(p.Nom)
End If
```

### Quels types nécessitent JSON ?

Beaucoup de types intégrés **n'ont pas besoin** de JSON : ils utilisent le format binaire NRBF, sûr et typé. C'est le cas des **primitives** (`Boolean`, `Integer`, `Double`, `Decimal`, `String`, `DateTime`, `TimeSpan`…), des **tableaux et listes** de primitives (mais évitez `String()` et `List(Of String)`, mal gérés par NRBF pour les valeurs nulles), et des types **`System.Drawing`** (`Point`, `Rectangle`, `Size`, `Color`, `Bitmap` — attention à la mémoire pour les grandes images). Les **types personnalisés** (classes, *records*, structures) passent, eux, par `SetDataAsJson` / `TryGetData(Of T)`, avec au besoin des attributs `System.Text.Json` (`<JsonIgnore>`, `<JsonInclude>`, `<JsonPropertyName>`) pour affiner la sérialisation (module [7.5](../07-acces-donnees/05-serialisation.md)).

### Glisser-déposer typé (`ITypedDataObject`)

C'est ici qu'aboutit le glisser-déposer annoncé en [5.6](06-evenements.md). Depuis .NET 10, la classe `DataObject` implémente `ITypedDataObject`, ce qui permet une récupération typée dans le gestionnaire `DragDrop` :

```vb
' Le contrôle cible a AllowDrop = True
Private Sub Cible_DragEnter(sender As Object, e As DragEventArgs) Handles Cible.DragEnter
    If e.Data.GetDataPresent(DataFormats.FileDrop) Then
        e.Effect = DragDropEffects.Copy
    End If
End Sub

Private Sub Cible_DragDrop(sender As Object, e As DragEventArgs) Handles Cible.DragDrop
    If TypeOf e.Data Is ITypedDataObject Then
        Dim donnees As ITypedDataObject = CType(e.Data, ITypedDataObject)

        Dim fichiers As String() = Nothing
        If donnees.TryGetData(DataFormats.FileDrop, fichiers) Then
            ChargerFichiers(fichiers)
        End If
    End If
End Sub
```

> ⚠️ La méthode **`GetData`** (sur `Clipboard`, `DataObject` et `IDataObject`) est **obsolète** en .NET 10 : un nouvel analyseur (`WFDEV005`) vous invite à la remplacer par `TryGetData(Of T)`. Le pont `BinaryFormatter` pour le legacy reste possible mais **déconseillé** (risques de sécurité), comme rappelé en [5.2](02-winforms-net10.md) et au module [16](../16-securite/README.md).

---

## Empêcher la capture d'écran (`Form.FormScreenCaptureMode`)

Avec la multiplication des outils de capture d'écran en continu, une application de gestion manipulant des données sensibles (identifiants, mots de passe, données personnelles) court un risque de fuite. .NET 10 ajoute une nouvelle propriété, **`Form.FormScreenCaptureMode`** (de type `ScreenCaptureMode`), qui empêche les applications de capture **utilisant l'API Windows** de saisir le formulaire. Elle prend trois valeurs :

- **`Allow`** — (défaut) le formulaire peut être capturé ;
- **`HideContent`** — le formulaire apparaît **noirci** dans la capture ;
- **`HideWindow`** — le formulaire est **flouté** à la capture (nécessite Windows 10 20H1 version 2004 ou ultérieure).

```vb
' Protéger un écran sensible (connexion, données personnelles)
Me.FormScreenCaptureMode = ScreenCaptureMode.HideContent
```

> ⚠️ **Ses limites.** Cette protection s'applique au niveau de la fenêtre et ne couvre **que** les captures passant par l'API Windows. Elle **n'empêche pas** une capture matérielle (photographier l'écran), et reste contournable avec des efforts. C'est une mesure de réduction du risque, pas une garantie absolue (module [16](../16-securite/README.md)).

---

## Éditeurs de concepteur reportés depuis .NET Framework

Plusieurs éditeurs visuels (`UITypeEditor`) hérités de .NET Framework ont été **reportés** sur .NET moderne, dont le **`ToolStripCollectionEditor`** et plusieurs éditeurs liés au **`DataGridView`**. Ils sont de nouveau **accessibles depuis la fenêtre Propriétés** et le **panneau d'actions du concepteur** — l'édition visuelle des barres d'outils, menus et colonnes (les contrôles avancés de la section [5.4](04-controles-avances.md)) en bénéficie directement. Par ailleurs, les **SnapLines ont été corrigées pour les concepteurs personnalisés**, ce qui profite à vos propres contrôles (section [5.5](05-controles-personnalises.md)).

---

## Autres changements notables

- **Nouveaux analyseurs** pour guider la modernisation :
    - `WFDEV004` — les événements `Closing`/`Closed` (et `OnClosing`/`OnClosed`) sont obsolètes : utilisez **`FormClosing`/`FormClosed`** (comme dans toute cette formation) ;
    - `WFDEV005` — `GetData` est obsolète : utilisez **`TryGetData(Of T)`** ;
    - `WFDEV006` — certains contrôles sont obsolètes (conservés pour compatibilité binaire avec .NET Framework).
- **Thématisation des contrôles dessinés.** Pour qu'un contrôle personnalisé suive le mode clair/sombre, .NET 10 propose `SetStyle(ControlStyles.ApplyThemingImplicitly, ...)` — à appeler en redéfinissant `CreateParams` (et **non** dans le constructeur, lu trop tard), ce qui complète l'approche par `SystemColors` vue en [5.5](05-controles-personnalises.md).
- **Corrections de bogues** (par exemple un `DataGridView` en édition lors de la fermeture du dialogue hôte, une régression d'impression) et améliorations d'accessibilité (lecteur d'écran NVDA).

---

## En résumé

Au-delà des modernisations de la section [5.2](02-winforms-net10.md), .NET 10 apporte à Windows Forms un **échange de données JSON typé** (presse-papiers redessiné et partagé avec WPF, glisser-déposer via `ITypedDataObject`, `SetDataAsJson`/`TryGetData(Of T)`), une protection **`Form.FormScreenCaptureMode`** pour les écrans sensibles, et le **retour d'éditeurs de concepteur** facilitant l'édition visuelle des contrôles avancés et personnalisés. De nouveaux analyseurs (`WFDEV004/005/006`) accompagnent enfin la transition vers les API recommandées.

Le module se conclut sur le déploiement, traité de façon transversale au module dédié → [5.13 / Déploiement et DevOps](../15-deploiement-devops/README.md).

⏭️ [WPF (Windows Presentation Foundation)](/06-wpf/README.md)
