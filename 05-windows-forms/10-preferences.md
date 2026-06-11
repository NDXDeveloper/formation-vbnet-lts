🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 5.10 Préférences et paramètres utilisateur (`My.Settings`)

Une application de bureau gagne à **se souvenir** de ses utilisateurs : taille et position de la fenêtre, thème choisi, dernier dossier ouvert, options cochées. En VB.NET, le raccourci idiomatique pour cela est **`My.Settings`** — et c'est l'un des cas où l'espace `My` est **bien pris en charge** sur .NET moderne, précisément parce qu'on est dans Windows Forms, son scénario cœur (rappel des limites de `My` en section [2.12](../02-fondamentaux-langage/12-espace-my.md)).

---

## Deux portées : `Application` et `User`

Les paramètres se déclinent en deux portées, et choisir la bonne est l'essentiel :

- **Portée `Application`** — associée à l'application, **lecture seule** à l'exécution. Elle convient à une information programme : URL d'un service, chaîne de connexion. Sa valeur réside dans le fichier de configuration de l'application.
- **Portée `User`** — propre à chaque utilisateur, **lecture/écriture** à l'exécution. C'est elle qu'on utilise pour les **préférences** : position d'une fenêtre, police, mode sombre. La valeur par défaut est dans la configuration de l'application, mais les modifications faites à l'exécution sont enregistrées dans un fichier **`user.config`** propre à l'utilisateur.

> 💡 **Règle simple :** tout ce que l'utilisateur peut modifier doit être en portée **`User`**. Un paramètre `Application` resterait figé en lecture seule.

On déclare les paramètres dans le **concepteur de paramètres** (My Project → onglet *Settings*) : un tableau où l'on saisit le **nom**, le **type**, la **portée** et la **valeur** par défaut. Visual Studio génère alors une classe `Settings` partielle (dans `Settings.vb`, sous l'espace `My`) qui expose chaque paramètre de façon fortement typée.

---

## Lire et écrire

L'accès est **fortement typé** — pas de chaîne « magique » :

```vb
' Lecture (toutes portées)
Dim dernierDossier As String = My.Settings.DernierDossier

' Écriture (portée User uniquement)
My.Settings.DernierDossier = dlg.SelectedPath
My.Settings.ModeSombre = True
```

Les types possibles vont au-delà des primitives : `String`, mais aussi `Color`, `Font`, `Point`, `Size`, et même des **types personnalisés sérialisables** (stockés sous forme XML).

---

## Persister les valeurs

Les modifications de paramètres `User` ne sont **pas écrites sur disque automatiquement** à chaque affectation : il faut appeler **`My.Settings.Save()`**, généralement à la fermeture du formulaire principal (section [5.6](06-evenements.md)) :

```vb
Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
    My.Settings.Save()   ' enregistre les paramètres User dans user.config
End Sub
```

> 💡 Le **Framework d'application** VB.NET (section [5.1](01-introduction-designer.md)) propose une case **« Enregistrer My.Settings à l'arrêt »**, cochée par défaut : la sauvegarde devient alors automatique. L'appel explicite à `Save()` reste nécessaire si vous l'avez décochée, ou pour sauvegarder à un autre moment.

Deux méthodes complètent le tableau : **`Reset()`** rétablit les valeurs par défaut, et **`Reload()`** recharge depuis le disque en annulant les modifications non enregistrées.

---

## Lier des paramètres à des contrôles (sans code)

Particularité appréciable : un paramètre peut être **lié à une propriété de contrôle dès la conception**. Dans la fenêtre Propriétés, la rubrique *(ApplicationSettings)* permet, par exemple, de lier la `Location` d'un formulaire à un paramètre : la position de la fenêtre est alors **mémorisée sans écrire une ligne de code**.

> 📌 La `Size` d'un formulaire, elle, **ne figure pas** parmi les propriétés liables par le concepteur (à cause
> des états agrandi/réduit, qui demandent un traitement particulier) : pour mémoriser la taille, on l'enregistre
> **par code** — typiquement dans `FormClosing`, en ignorant les états `Minimized`/`Maximized`.

> ⚠️ Si vous persistez la position d'une fenêtre, prévoyez le cas où la position enregistrée serait **hors écran** (écran débranché, résolution changée) ou l'état **minimisé** : sans garde-fou, la fenêtre pourrait réapparaître invisible. Une vérification au démarrage évite ce piège.

---

## Où sont stockés les paramètres

Le mécanisme par défaut (`LocalFileSettingsProvider`) écrit les données en **XML** dans des fichiers de configuration. Les paramètres `User` modifiés à l'exécution atterrissent dans un `user.config`, sous un chemin **versionné** du profil local de l'utilisateur, de la forme :

```
%LOCALAPPDATA%\<Éditeur>\<application>_<hash>\<version>\user.config
```

> ⚠️ **Le piège de la version.** Ce chemin contient le **numéro de version** de l'application. À chaque changement de version, un **nouveau dossier** est créé : les paramètres `User` de l'ancienne version semblent alors **perdus**. La parade standard est la méthode **`Upgrade()`**, appelée **une seule fois** après une mise à jour pour reprendre les valeurs précédentes — gardée par un indicateur booléen :

```vb
Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    If My.Settings.MettreAJour Then       ' paramètre booléen, vrai par défaut
        My.Settings.Upgrade()             ' reprend les valeurs de la version précédente
        My.Settings.MettreAJour = False   ' ne le refera plus
        My.Settings.Save()
    End If
End Sub
```

Pour une persistance totalement maîtrisée entre versions, on peut aussi écrire un **fournisseur de paramètres personnalisé**.

---

## `My.Settings` ou la configuration moderne ? ⚠️

Une distinction importante pour viser juste en 2026 :

- **`My.Settings` en portée `User`** reste l'approche idiomatique pour les **préférences propres à chaque utilisateur** (lecture/écriture, persistées entre les sessions). C'est pleinement pris en charge en WinForms sur .NET 10.
- Pour la **configuration applicative** (chaînes de connexion, réglages selon l'environnement), le .NET moderne privilégie désormais **`appsettings.json`** avec `Microsoft.Extensions.Configuration` / `IOptions`. Sur .NET moderne, l'ancien `ConfigurationManager` (et l'essentiel des sections de `app.config`) n'est conservé que **pour compatibilité**. Les paramètres de portée `Application` de `My.Settings` recouvrent en partie ce besoin : pour de la vraie configuration applicative, préférez `appsettings.json` (voir module [11.3](../11-migration-legacy/03-framework-vers-net10.md)).

En résumé : **`My.Settings` (User) pour les préférences**, **`appsettings.json` pour la configuration**.

---

## En résumé

`My.Settings` offre une persistance des préférences avec très peu de code : on déclare les paramètres dans le concepteur, on choisit la portée (**`User`** pour tout ce qui est modifiable), on lit/écrit de façon fortement typée, et l'on **enregistre** par `Save()` (ou automatiquement via le Framework d'application). Les paramètres se lient aussi aux contrôles dès la conception. Deux points de vigilance : le **piège de la version** (résolu par `Upgrade()`), et la frontière avec la **configuration moderne** (`appsettings.json` pour la config applicative, `My.Settings` pour les préférences utilisateur).

La section suivante traite de l'adaptation aux langues et aux cultures → [5.11 Internationalisation](11-internationalisation.md).

⏭️ [Internationalisation (i18n/l10n, ressources .resx)](/05-windows-forms/11-internationalisation.md)
