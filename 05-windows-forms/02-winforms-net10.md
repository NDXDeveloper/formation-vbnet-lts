🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.2 Windows Forms sur .NET 10 (modernisation) 🆕

Le langage VB.NET est stabilisé (module [1](../01-introduction-vbnet/06-positionnement-2026.md)), mais la **plateforme** Windows Forms, elle, continue d'évoluer. .NET 9 puis .NET 10 lui ont apporté des modernisations concrètes — et comme elles passent par des API du *framework* que l'on **consomme**, elles sont pleinement accessibles en VB.NET, sans aucune nouvelle syntaxe de langage.

Cette section en présente trois, particulièrement utiles aux applications de bureau VB.NET : le **mode sombre**, les **formulaires asynchrones** et le **presse-papiers sécurisé**.

---

## Le mode sombre intégré

C'est la nouveauté la plus visible. Le mode sombre a d'abord été introduit à titre **expérimental** dans .NET 9 (il fallait alors lever l'erreur de compilation `WFO5001` pour y accéder). Dans .NET 10, il est **finalisé** : cette fonctionnalité n'est plus protégée par cette erreur de compilation à partir de .NET 10, et l'API `Application.SetColorMode` n'est plus considérée comme expérimentale.

### L'API

Tout repose sur une méthode statique, `Application.SetColorMode`, et une énumération `SystemColorMode` à trois valeurs :

- **`Classic`** — mode clair, le comportement historique (valeur par défaut) ;
- **`System`** — suit le réglage clair/sombre défini dans Windows ;
- **`Dark`** — force le mode sombre.

Deux propriétés en lecture permettent ensuite de s'adapter dans le code : `Application.ColorMode` renvoie le mode retenu par l'application, et `Application.SystemColorMode` renvoie le réglage du système. Le mode sombre du système est pris en charge sur Windows 11 ou les versions ultérieures, et n'est pas disponible quand le mode Contraste élevé de Windows est actif.

### La façon idiomatique de l'activer en VB.NET ⭐

En C#, on appelle `SetColorMode` dans `Main`. Mais le Framework d'application VB.NET n'expose pas de `Program.cs` (section [5.1](01-introduction-designer.md)) : l'approche idiomatique consiste à fixer le mode dans l'événement **`ApplyApplicationDefaults`**, au sein de `ApplicationEvents.vb`. L'argument de cet événement expose une propriété `ColorMode` de type `SystemColorMode`, prévue exactement pour cela :

```vb
Namespace My

    Partial Friend Class MyApplication

        Private Sub MyApplication_ApplyApplicationDefaults(
                sender As Object,
                e As ApplicationServices.ApplyApplicationDefaultsEventArgs
            ) Handles Me.ApplyApplicationDefaults

            ' Suivre le thème clair/sombre choisi dans Windows
            e.ColorMode = SystemColorMode.System
        End Sub

    End Class

End Namespace
```

Sans le Framework d'application (avec un `Sub Main` manuel), on appelle directement la méthode au démarrage :

```vb
<STAThread>
Sub Main()
    Application.SetHighDpiMode(HighDpiMode.SystemAware)
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(False)
    Application.SetColorMode(SystemColorMode.Dark)   ' ou .System pour suivre Windows
    Application.Run(New MainForm())
End Sub
```

### Points de vigilance

> ⚠️ Le mode sombre est intégré, mais pas magique :
> - il ne s'applique que sous **Windows 11+** ;
> - certains contrôles demandent un ajustement : un `Button` en `FlatStyle.Standard` (le défaut) rend mal en sombre, alors que `FlatStyle.System` donne un meilleur résultat ;
> - quelques contrôles communs Win32 (les barres de défilement, par exemple) et le `TaskDialog` ne sont pas entièrement thématisés ;
> - si l'utilisateur bascule le thème de Windows *pendant* l'exécution, l'application ne se met pas à jour automatiquement — il faut le gérer soi-même si nécessaire ;
> - au moment d'écrire ces lignes, `ColorMode` n'est pas encore proposé dans le concepteur d'application (App Designer) : on le définit donc par code, comme ci-dessus.

---

## Les formulaires asynchrones

Historiquement, afficher une boîte de dialogue modale avec `ShowDialog` **bloque** la boucle de messages jusqu'à sa fermeture, et orchestrer l'ouverture de formulaires depuis du code `Async`/`Await` (module [4](../04-async/README.md)) était malcommode. .NET 10 ajoute des variantes asynchrones du côté de `Form` :

- `ShowAsync` affiche le formulaire de façon asynchrone (non modal) en passant sa propriété `Visible` à `True` ; la `Task` retournée s'achève à la fermeture ou à la libération du formulaire ;
- `ShowDialogAsync` affiche le formulaire comme boîte de dialogue modale et renvoie une `Task(Of DialogResult)` qui se termine à la fermeture du formulaire.

Les signatures côté VB.NET :

```vb
' Affichage non modal
Public Function ShowAsync(Optional owner As IWin32Window = Nothing) As Task

' Affichage modal asynchrone
Public Function ShowDialogAsync() As Task(Of DialogResult)
Public Function ShowDialogAsync(owner As IWin32Window) As Task(Of DialogResult)
```

.NET 9 avait introduit ces méthodes en mode préliminaire (il fallait alors lever l'erreur de compilation `WFO5002`). Dans **.NET 10, elles sont finalisées** : l'erreur `WFO5002` n'est plus déclenchée, et ni `ShowAsync` ni `ShowDialogAsync` ne sont désormais considérés comme expérimentaux — **aucun `NoWarn` n'est nécessaire** pour les utiliser. Bonus : la `Task` retournée détient une **référence faible** vers le formulaire, ce qui aide à garder une interface réactive lorsqu'on gère plusieurs fenêtres.

### À quoi cela sert

L'intérêt est de pouvoir **attendre** (`Await`) le résultat d'une boîte de dialogue sans figer l'interface, puis d'enchaîner avec d'autres opérations asynchrones. Avantage supplémentaire : il n'est pas nécessaire de marshaler manuellement l'appel vers le thread d'interface, même si l'appel provient d'un autre thread.

```vb
Private Async Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click
    Using dlg As New ClientEditForm()
        Dim resultat As DialogResult = Await dlg.ShowDialogAsync(Me)

        If resultat = DialogResult.OK Then
            ' Recharger les données sans avoir figé l'interface
            Await RafraichirListeAsync()
        End If
    End Using
End Sub
```

(Rappel : un gestionnaire d'événement asynchrone est un `Async Sub`, jamais un `Async Function`.) Ces méthodes nécessitent un `WindowsFormsSynchronizationContext`, présent dans une application Windows Forms standard.

### Quand l'utiliser

`ShowDialog` (synchrone) reste parfaitement adapté à une simple boîte de confirmation. `ShowDialogAsync` prend tout son sens dès que le flux environnant est asynchrone — par exemple lorsque l'ouverture du dialogue précède ou suit des appels réseau (`HttpClient`, module [8.1](../08-services-web/01-consommer-api-rest.md)) ou des accès à une base de données.

---

## Le presse-papiers sécurisé

### Le problème : la fin de `BinaryFormatter`

Pendant des années, le presse-papiers et le glisser-déposer s'appuyaient sur `BinaryFormatter` pour sérialiser des objets personnalisés. Or ce composant souffrait de vulnérabilités de sécurité connues (désérialisation non bornée de types arbitraires). À partir de .NET 9, `BinaryFormatter` a donc été retiré du runtime ; cette suppression a cassé les opérations de presse-papiers et de glisser-déposer pour les objets personnalisés, créant un manque fonctionnel pour les applications Windows Forms.

### La solution .NET 10 : des API typées fondées sur JSON 🆕

.NET 10 comble ce manque en introduisant de nouvelles API qui restaurent ces fonctionnalités tout en améliorant la sécurité, la gestion des erreurs et la compatibilité inter-processus ; elles s'appuient sur la sérialisation JSON et offrent des méthodes type-safe. Concrètement :

- `Clipboard.SetDataAsJson(Of T)` sérialise un objet personnalisé en JSON pour le placer dans le presse-papiers ;
- `Clipboard.TryGetData(Of T)` récupère la donnée de façon typée et remplace la méthode `GetData`, désormais obsolète.

```vb
' Un type métier, simplement sérialisable en JSON
Public Class Client
    Public Property Nom As String
    Public Property Ville As String
End Class
```

```vb
' Écriture : sérialisation JSON typée, sans BinaryFormatter
Dim client As New Client With {.Nom = "Dupont", .Ville = "Rouen"}
Clipboard.SetDataAsJson("MonApp.Client", client)

' Lecture : récupération typée
Dim recupere As Client = Nothing
If Clipboard.ContainsData("MonApp.Client") AndAlso
   Clipboard.TryGetData("MonApp.Client", recupere) Then
    MessageBox.Show($"{recupere.Nom} — {recupere.Ville}")
End If
```

Le type générique est ici **inféré** à partir de la variable passée par référence — pas besoin de l'indiquer explicitement. Les types .NET courants fonctionnent sans changement ; les objets personnalisés doivent simplement être sérialisables en JSON pour être déposés dans le presse-papiers.

### Et le code legacy ?

Si une application ancienne ne peut pas migrer immédiatement, il existe un pont temporaire : réactiver explicitement `BinaryFormatter` pour le presse-papiers via des commutateurs de configuration (`Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization` et `System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization`). Cette approche n'est pas recommandée et ne doit servir que de passerelle de migration pour les applications héritées : elle comporte des risques de sécurité importants. La bonne cible reste les méthodes typées ci-dessus (voir aussi le module [16](../16-securite/README.md) sur la sécurité et le module [7.5](../07-acces-donnees/05-serialisation.md) sur la sérialisation). Le glisser-déposer et l'échange JSON sont approfondis en [5.12](12-nouveautes-net10.md).

---

## Récapitulatif : le statut des nouveautés en .NET 10

| Fonctionnalité | API principale | Statut en .NET 10 | Activation |
|---|---|---|---|
| **Mode sombre** | `Application.SetColorMode` | ✅ Finalisé | Aucune action |
| **Formulaires async** | `Form.ShowAsync` / `ShowDialogAsync` | ✅ Finalisé | Aucune action |
| **Presse-papiers sécurisé** | `Clipboard.SetDataAsJson` / `TryGetData(Of T)` | ✅ Disponible (`BinaryFormatter` retiré) | Aucune (le pont legacy est déconseillé) |

> 💡 Règle de prudence : n'activez une API **expérimentale** (`WFO500x`) que si vous en avez réellement besoin. Sa signature peut encore évoluer dans une version ultérieure de .NET.

Avec ces modernisations en tête, passons à la brique de base de toute interface : les contrôles fondamentaux → [5.3 Contrôles fondamentaux](03-controles-fondamentaux.md).

⏭️ [Contrôles fondamentaux (Form, Button, TextBox, conteneurs, boîtes de dialogue)](/05-windows-forms/03-controles-fondamentaux.md)
