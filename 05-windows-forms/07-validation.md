🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.7 Validation (`ErrorProvider`, `DataAnnotations`, règles personnalisées)

Valider une saisie, c'est répondre à **trois questions distinctes** — et la force d'une bonne stratégie est de ne pas les mélanger :

- **Quelles** règles ? → les **`DataAnnotations`**, déclarées sur le modèle, indépendantes de l'interface et réutilisables ;
- **Quand** valider ? → les événements **`Validating`** (pilotés par le focus, section [5.6](06-evenements.md)) ou une validation explicite avant l'enregistrement ;
- **Comment** signaler les erreurs ? → l'**`ErrorProvider`**, le composant d'affichage propre à Windows Forms.

Cette section traite les trois, puis montre comment les **combiner**.

---

## Quand valider : les événements `Validating`

Chaque contrôle déclenche **`Validating`** lorsqu'il perd le focus. Son argument est un `CancelEventArgs` : en posant `e.Cancel = True`, on **conserve le focus** sur le contrôle, empêchant l'utilisateur de le quitter tant que la saisie est invalide. L'événement `Validated` suit, une fois la validation réussie.

```vb
Imports System.ComponentModel

Private Sub txtEmail_Validating(sender As Object, e As CancelEventArgs) Handles txtEmail.Validating
    If String.IsNullOrWhiteSpace(txtEmail.Text) Then
        errProvider.SetError(txtEmail, "L'adresse e-mail est obligatoire.")
        e.Cancel = True              ' garde le focus sur le champ
    Else
        errProvider.SetError(txtEmail, "")   ' efface l'erreur
    End If
End Sub
```

Deux écueils classiques :

- **Un bouton « Annuler » ou « Fermer » doit porter `CausesValidation = False`.** Sinon, l'utilisateur reste piégé dans un champ invalide, sans pouvoir abandonner.
- **Bloquer le focus (`e.Cancel`) est parfois trop intrusif.** Beaucoup d'équipes préfèrent ne pas bloquer : on signale l'erreur, et l'on valide l'ensemble **au moment de l'enregistrement** via `ValidateChildren`, qui déclenche la validation de tous les contrôles enfants :

```vb
Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click
    If Not ValidateChildren() Then Return   ' un Validating a posé e.Cancel = True
    ' ... enregistrer ...
End Sub
```

---

## Comment signaler : l'`ErrorProvider`

`ErrorProvider` est un composant non visuel (il siège dans le bac à composants, voir section [5.5](05-controles-personnalises.md)). Sa méthode centrale, **`SetError(contrôle, message)`**, affiche une icône d'erreur clignotante à côté du contrôle, avec le message en info-bulle ; `SetError(contrôle, "")` l'efface, et `Clear()` efface toutes les erreurs d'un coup. On personnalise l'icône et le clignotement via `Icon` et `BlinkStyle`.

C'est l'outil d'affichage que les exemples ci-dessus utilisent déjà. Reste à décider **d'où viennent les règles** — et c'est là qu'interviennent les `DataAnnotations`.

---

## Quelles règles : les `DataAnnotations`

L'idée maîtresse : **placer les règles sur le modèle**, sous forme d'attributs, plutôt que de les disséminer dans le code de l'interface. Ces règles deviennent ainsi **réutilisables** partout où le modèle circule — formulaire WinForms, Web API (module [8](../08-services-web/README.md)), Entity Framework Core (module [7](../07-acces-donnees/README.md)).

Les attributs historiques (espace `System.ComponentModel.DataAnnotations`) : `Required`, `StringLength`, `Range`, `RegularExpression`, `EmailAddress`, `Phone`, `Compare`.

> 🆕 **Apports récents (.NET 8, disponibles en .NET 10).** De nouveaux attributs sont venus enrichir la palette : **`Length`** (longueur min/max d'une chaîne ou d'une collection), **`AllowedValues`** et **`DeniedValues`** (valeurs autorisées ou interdites), **`Base64String`** (chaîne Base64 valide), et l'attribut **`Range`** doté de `MinimumIsExclusive` / `MaximumIsExclusive` pour des bornes exclusives.

En VB.NET, les attributs s'écrivent entre chevrons `<...>` et les arguments nommés utilisent `:=` :

```vb
Imports System.ComponentModel.DataAnnotations

Public Class Client

    <Required(ErrorMessage:="Le nom est obligatoire.")>
    <Length(2, 80, ErrorMessage:="Le nom doit comporter de 2 à 80 caractères.")>
    Public Property Nom As String

    <Required>
    <EmailAddress(ErrorMessage:="Adresse e-mail invalide.")>
    Public Property Email As String

    <Range(0, 120, ErrorMessage:="L'âge doit être compris entre 0 et 120.")>
    Public Property Age As Integer

    <AllowedValues("Particulier", "Entreprise", ErrorMessage:="Type de client inconnu.")>
    Public Property TypeClient As String

End Class
```

Pour **valider** un objet ainsi annoté, on utilise la classe `Validator`. `TryValidateObject` renvoie un booléen et remplit une liste de `ValidationResult` :

```vb
Dim client As New Client With {.Nom = "A", .Email = "pasunemail", .Age = 200}
Dim contexte As New ValidationContext(client)
Dim resultats As New List(Of ValidationResult)

Dim estValide As Boolean =
    Validator.TryValidateObject(client, contexte, resultats, validateAllProperties:=True)

For Each r In resultats
    ' r.MemberNames = les propriétés concernées ; r.ErrorMessage = le message
    Console.WriteLine($"{String.Join(", ", r.MemberNames)} : {r.ErrorMessage}")
Next
```

> ⚠️ N'oubliez pas **`validateAllProperties:=True`** : sans ce paramètre, seuls les `Required` (et quelques cas) sont vérifiés, pas les autres attributs.

---

## Le pont `DataAnnotations` ↔ `ErrorProvider` ⭐

Voici le motif qui réunit le meilleur des deux mondes : on valide le **modèle** avec `Validator` (règles déclaratives), puis on **reporte chaque erreur** sur le contrôle WinForms correspondant via l'`ErrorProvider` (affichage natif).

```vb
Private Function ValiderEtAfficher(client As Client) As Boolean
    errProvider.Clear()   ' efface les erreurs précédentes

    Dim contexte As New ValidationContext(client)
    Dim resultats As New List(Of ValidationResult)
    Dim estValide = Validator.TryValidateObject(client, contexte, resultats, True)

    ' Associe chaque propriété au contrôle qui la saisit
    Dim controles As New Dictionary(Of String, Control) From {
        {NameOf(Client.Nom), txtNom},
        {NameOf(Client.Email), txtEmail},
        {NameOf(Client.Age), numAge}
    }

    For Each r In resultats
        For Each membre In r.MemberNames
            Dim ctl As Control = Nothing
            If controles.TryGetValue(membre, ctl) Then
                errProvider.SetError(ctl, r.ErrorMessage)
            End If
        Next
    Next

    Return estValide
End Function
```

L'opérateur `NameOf` relie de façon sûre le nom d'une propriété au contrôle, sans chaîne « magique » qui se désynchroniserait en cas de renommage.

---

## Règles personnalisées

Quand les attributs standard ne suffisent pas, trois approches, de la plus locale à la plus réutilisable.

### 1. Impérative, dans un gestionnaire `Validating`

La plus simple pour une règle ponctuelle : on écrit la condition directement dans le `Validating` du contrôle et l'on appelle `SetError` (comme dans le premier exemple de cette section).

### 2. Un attribut de validation personnalisé

Pour une règle **réutilisable et déclarative**, on hérite de `ValidationAttribute` et l'on redéfinit `IsValid` :

```vb
Imports System.ComponentModel.DataAnnotations
Imports System.Text.RegularExpressions

' Règle réutilisable : un code postal français (5 chiffres)
Public Class CodePostalFrAttribute
    Inherits ValidationAttribute

    Protected Overrides Function IsValid(value As Object,
                                         context As ValidationContext) As ValidationResult
        Dim texte = TryCast(value, String)

        If Not String.IsNullOrEmpty(texte) AndAlso Not Regex.IsMatch(texte, "^\d{5}$") Then
            Return New ValidationResult("Le code postal doit comporter 5 chiffres.",
                                        {context.MemberName})
        End If

        Return ValidationResult.Success
    End Function
End Class
```

L'attribut s'applique alors comme les autres : `<CodePostalFr()> Public Property CodePostal As String`, et il est pris en compte par `Validator.TryValidateObject`.

### 3. Une validation au niveau de l'objet (`IValidatableObject`)

Pour une règle **inter-propriétés** (qui compare plusieurs champs), le modèle implémente `IValidatableObject` :

```vb
Imports System.ComponentModel.DataAnnotations

Public Class Reservation
    Implements IValidatableObject

    Public Property DateDebut As DateTime
    Public Property DateFin As DateTime

    Public Function Validate(context As ValidationContext) As IEnumerable(Of ValidationResult) _
            Implements IValidatableObject.Validate

        If DateFin < DateDebut Then
            Return {New ValidationResult("La date de fin doit suivre la date de début.",
                                         {NameOf(DateFin)})}
        End If

        Return Enumerable.Empty(Of ValidationResult)()
    End Function
End Class
```

La méthode `Validate` est appelée par `Validator.TryValidateObject` une fois les validations de propriétés passées — idéale pour les contraintes qui ne tiennent pas dans un seul champ.

---

## En résumé et bonnes pratiques

- **Séparez les trois préoccupations** : les règles (`DataAnnotations`), le déclenchement (`Validating` / `ValidateChildren`), l'affichage (`ErrorProvider`).
- **Placez les règles sur le modèle** : vous les réutiliserez dans l'API et la couche de données (modules [8](../08-services-web/README.md) et [7](../07-acces-donnees/README.md)).
- **`CausesValidation = False`** sur les boutons d'annulation et de fermeture.
- **Validez l'objet entier avant d'enregistrer** (`ValidateChildren` ou `Validator.TryValidateObject`), pas seulement champ par champ.
- **Guidez la saisie en amont** quand c'est possible (`MaskedTextBox`, section [5.3](03-controles-fondamentaux.md)) pour réduire les erreurs.
- **La validation côté interface ne suffit jamais** : revalidez à la frontière (base de données, service) — voir le module [16](../16-securite/README.md).

La section suivante relie ces formulaires à des sources de données et automatise l'affichage des erreurs grâce à la **liaison de données** → [5.8 Liaison de données WinForms](08-data-binding.md).

⏭️ [Liaison de données WinForms (BindingSource, BindingList, liaison à une BDD)](/05-windows-forms/08-data-binding.md)
