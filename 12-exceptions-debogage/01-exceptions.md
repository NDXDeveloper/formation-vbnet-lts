🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 12.1 — `Try`/`Catch`/`Finally`, filtres `When`, hiérarchie, exceptions personnalisées

> **Module 12 — Exceptions, débogage et journalisation**
> Le socle de la fiabilité : intercepter, qualifier et réagir aux erreurs proprement,
> plutôt que de les masquer.

---

## Le modèle d'exceptions de .NET

En .NET, une erreur d'exécution se manifeste par une **exception** : un objet (dérivé de
`System.Exception`) qui décrit ce qui s'est mal passé, est *levé* (`Throw`) à l'endroit du
problème, et *remonte* la pile des appels jusqu'à ce qu'un bloc `Catch` l'intercepte — ou,
à défaut, jusqu'à faire planter l'application.

Ce mécanisme est entièrement natif en VB.NET. La gestion structurée des exceptions
(`Try` / `Catch` / `Finally`) fait partie du langage **depuis VB.NET 2002** : le gel du
langage à la version 16.9 ne change rien ici, et VB.NET dispose même d'un atout que C# n'a
obtenu que tardivement (les filtres `When`, voir plus bas).

Un principe à garder en tête dès le départ :

> ⚠️ **Une exception est faite pour les situations *exceptionnelles*, pas pour le flux normal.**
> Lever puis intercepter une exception est coûteux (capture de la pile d'appels). Ne vous en
> servez jamais comme d'un `If` déguisé. À l'inverse, **entrer dans un bloc `Try` ne coûte
> quasiment rien** s'il ne se passe aucune erreur : il n'y a donc aucune raison de « craindre »
> `Try` pour des questions de performance. Le coût est dans le *lancer*, pas dans le *try*.

---

## La hiérarchie des exceptions

Toutes les exceptions dérivent, directement ou indirectement, de `System.Exception`. Connaître
cette hiérarchie permet d'intercepter au **bon niveau de précision**.

### Les propriétés essentielles de `Exception`

| Propriété | Contenu |
|-----------|---------|
| `Message` | Message lisible décrivant l'erreur. |
| `StackTrace` | La pile d'appels au moment du lancer (chaîne de texte). |
| `InnerException` | L'exception d'origine, quand on *enveloppe* une exception dans une autre. |
| `Data` | Dictionnaire clé/valeur pour attacher du contexte supplémentaire. |
| `Source` | Nom de l'application ou de l'objet à l'origine de l'erreur. |
| `HResult` | Code d'erreur numérique (utile en interop COM / natif). |
| `TargetSite` | La méthode qui a levé l'exception. |

```vb
Try
    Dim valeur = Integer.Parse("abc")
Catch ex As FormatException
    Console.WriteLine($"Message  : {ex.Message}")
    Console.WriteLine($"Source   : {ex.Source}")
    Console.WriteLine($"Pile     : {ex.StackTrace}")
End Try
```

### Les exceptions courantes

| Exception | Quand elle survient |
|-----------|---------------------|
| `ArgumentException` | Un argument est invalide. |
| `ArgumentNullException` | Un argument attendu vaut `Nothing`. |
| `ArgumentOutOfRangeException` | Un argument est hors de la plage autorisée. |
| `InvalidOperationException` | L'objet n'est pas dans un état permettant l'opération. |
| `NullReferenceException` | Déréférencement de `Nothing` — **un bug à corriger**, pas à intercepter. |
| `IndexOutOfRangeException` | Index de tableau hors limites — un bug, lui aussi. |
| `KeyNotFoundException` | Clé absente d'un `Dictionary(Of TKey, TValue)`. |
| `FormatException` | Conversion de format impossible (`Parse`, etc.). |
| `OverflowException` | Dépassement arithmétique. |
| `NotSupportedException` / `NotImplementedException` | Opération non prise en charge / non encore écrite. |
| `IOException`, `FileNotFoundException` | Problèmes d'entrée/sortie. |
| `TimeoutException` | Délai dépassé. |
| `OperationCanceledException` / `TaskCanceledException` | Annulation coopérative (voir [module 4.4](../04-async/04-annulation-timeout.md)). |
| `HttpRequestException` | Échec d'un appel HTTP (voir [module 8.1](../08-services-web/01-consommer-api-rest.md)). |

> 💡 Distinction importante : les exceptions d'argument (`ArgumentNullException`…) sont des
> exceptions que **vous levez** pour signaler une erreur de l'*appelant*. À l'inverse,
> `NullReferenceException` ou `IndexOutOfRangeException` signalent généralement **votre propre
> bug** : on ne les « gère » pas, on corrige le code qui les provoque.

### Lever proprement les erreurs d'argument

Le .NET moderne (dont .NET 10) fournit des **gardes** concises, pleinement utilisables depuis
VB.NET — préférez-les aux `If … Then Throw New …` verbeux :

```vb
Public Sub EnregistrerClient(client As Client, nom As String, age As Integer)
    ArgumentNullException.ThrowIfNull(client)
    ArgumentException.ThrowIfNullOrWhiteSpace(nom)
    ArgumentOutOfRangeException.ThrowIfNegative(age)
    ' … logique métier …
End Sub
```

### `SystemException`, `ApplicationException` et un piège classique

Sous `Exception`, on trouve historiquement deux grandes branches : `SystemException` (les
exceptions du runtime et du framework) et `ApplicationException` (prévue à l'origine pour les
exceptions « applicatives »).

> ⚠️ **Ne dérivez PAS vos exceptions personnalisées de `ApplicationException`.** La distinction
> envisagée à l'époque ne s'est jamais révélée utile, et la recommandation officielle de
> Microsoft est d'hériter **directement de `Exception`**. `ApplicationException` n'apporte rien.

### Les exceptions qu'il ne faut (presque) jamais intercepter

Certaines exceptions signalent un état du processus si dégradé qu'il vaut mieux le laisser
s'arrêter : `StackOverflowException` (qu'on ne *peut* d'ailleurs plus intercepter dans le .NET
moderne), `OutOfMemoryException`, `AccessViolationException`. C'est l'une des raisons pour
lesquelles un `Catch ex As Exception` trop large est dangereux : il « attrape » aussi ce qu'il
ne devrait pas.

---

## `Try` / `Catch` / `Finally`

### Structure de base

```vb
Try
    ' Code susceptible de lever une exception
    Dim contenu = File.ReadAllText(chemin)
    Traiter(contenu)

Catch ex As FileNotFoundException
    ' Cas précis traité en premier
    Console.WriteLine($"Fichier introuvable : {ex.FileName}")

Catch ex As IOException
    ' Cas plus général d'E/S
    Console.WriteLine($"Erreur d'E/S : {ex.Message}")

Finally
    ' S'exécute TOUJOURS : succès, exception interceptée, ou non interceptée
    Console.WriteLine("Fin de la tentative de lecture.")
End Try
```

Trois blocs, trois rôles :

- **`Try`** délimite le code surveillé.
- **`Catch`** intercepte une exception. On peut en enchaîner plusieurs.
- **`Finally`** s'exécute systématiquement, qu'il y ait eu erreur ou non. C'est l'endroit
  du **nettoyage** (fermer un fichier, libérer une ressource, remettre un état en place).

### Ordonner du plus précis au plus général

VB.NET évalue les blocs `Catch` **dans l'ordre**. Le premier dont le type correspond gagne.
Il faut donc placer les exceptions les plus **spécifiques** avant les plus **générales** :

```vb
Try
    Traiter()
Catch ex As ArgumentNullException   ' spécifique
    ' …
Catch ex As ArgumentException       ' plus général (parent du précédent)
    ' …
Catch ex As Exception               ' le plus général en dernier
    ' …
End Try
```

Si vous inversiez l'ordre (le général avant le spécifique), le bloc spécifique deviendrait
**inaccessible** — le compilateur VB.NET vous en avertira.

### `Finally` vs `Using`

Quand `Finally` ne sert qu'à **libérer une ressource** (`IDisposable`), préférez le bloc
`Using`, plus court et impossible à oublier : il génère un `Try…Finally` implicite avec appel
à `Dispose`.

```vb
' Au lieu de Try…Finally pour fermer un flux :
Using lecteur As New StreamReader(chemin)
    Dim ligne = lecteur.ReadLine()
    ' … Dispose() est appelé automatiquement en sortie de bloc, même en cas d'exception …
End Using
```

`Using` et `Try…Catch` se combinent naturellement : on encadre la logique métier par un
`Try…Catch` et on confie la libération des ressources à `Using`.

---

## Les filtres d'exception : `Catch … When`

C'est un **point fort idiomatique de VB.NET**. La clause `When` ajoute une **condition** à un
`Catch` : le bloc n'intercepte l'exception que si la condition est vraie.

```vb
Try
    Await client.GetAsync(url)
Catch ex As HttpRequestException When ex.StatusCode = HttpStatusCode.NotFound
    Console.WriteLine("Ressource introuvable (404).")
Catch ex As HttpRequestException When ex.StatusCode = HttpStatusCode.ServiceUnavailable
    Console.WriteLine("Service indisponible (503), nouvelle tentative plus tard.")
Catch ex As HttpRequestException
    Console.WriteLine($"Autre erreur HTTP : {ex.Message}")
End Try
```

> 🟢 **VB.NET avait les filtres d'exception bien avant C#.** La clause `Catch … When` existe
> depuis les débuts de VB.NET ; C# n'a obtenu son équivalent (`catch … when`) qu'en **C# 6 (2015)**.
> Sur ce point précis, le code VB était en avance.

### Pourquoi `When` est meilleur qu'un test à l'intérieur du `Catch`

On pourrait croire que ceci est équivalent :

```vb
' À éviter quand un filtre When suffit :
Catch ex As HttpRequestException
    If ex.StatusCode = HttpStatusCode.NotFound Then
        ' traiter
    Else
        Throw   ' relancer ce qu'on ne sait pas traiter
    End If
```

Ça ne l'est pas tout à fait. La différence est subtile mais réelle : **le filtre `When` est
évalué *avant* que la pile ne soit déroulée**. Si la condition est fausse, c'est comme si ce
`Catch` n'avait jamais existé : la pile d'appels reste **intacte** au point de lancer d'origine.
Avec un test puis un `Throw` à l'intérieur du `Catch`, la pile a déjà commencé à être déroulée,
ce qui dégrade le diagnostic au débogueur.

### Une astuce : journaliser sans intercepter

Comme un filtre peut exécuter du code, on peut s'en servir pour **logguer une exception sans la
capturer** — en renvoyant toujours `False` :

```vb
Try
    FaireLeTravail()
Catch ex As Exception When Journaliser(ex)
    ' Jamais atteint : Journaliser renvoie toujours False,
    ' donc l'exception continue de remonter, pile préservée.
End Try

Private Function Journaliser(ex As Exception) As Boolean
    _logger.LogError(ex, "Échec pendant FaireLeTravail")
    Return False   ' l'exception N'EST PAS interceptée
End Function
```

C'est une technique élégante, mais à utiliser avec discernement : elle peut surprendre un
lecteur qui ne la connaît pas.

---

## Lever et relancer : `Throw`, `Throw ex` et `ExceptionDispatchInfo`

### Lever une exception

```vb
If solde < montant Then
    Throw New InvalidOperationException("Solde insuffisant pour ce retrait.")
End If
```

### Relancer : `Throw` ≠ `Throw ex` (piège fréquent)

C'est l'une des erreurs les plus courantes, et elle gâche le diagnostic :

```vb
Catch ex As Exception
    ' … traitement partiel, logging …
    Throw       ' ✅ relance l'exception en PRÉSERVANT la pile d'origine
End Try
```

```vb
Catch ex As Exception
    Throw ex    ' ❌ relance, mais RÉINITIALISE la pile à cette ligne :
                '    on perd l'emplacement réel du problème
End Try
```

> ⚠️ **Règle d'or :** pour relancer l'exception courante, écrivez `Throw` tout seul.
> Réservez `Throw ex` à de très rares cas où vous *voulez* réellement repartir d'ici.

### Envelopper une exception (`InnerException`)

Pour ajouter du contexte métier tout en conservant la cause technique, **enveloppez** :

```vb
Catch ex As SqlException
    Throw New DataAccessException(
        "Échec de l'enregistrement de la commande.", ex)   ' ex devient InnerException
End Try
```

Le destinataire peut alors lire le message de haut niveau *et* remonter à la cause réelle via
`InnerException` (et toute la chaîne, jusqu'à `GetBaseException()`).

### Préserver une pile à travers les frontières : `ExceptionDispatchInfo`

Lorsqu'on capture une exception pour la relancer *plus tard* ou *ailleurs* (par exemple stockée
puis rejouée), `Throw ex` perdrait la pile. La solution est `ExceptionDispatchInfo` (de
`System.Runtime.ExceptionServices`) :

```vb
Imports System.Runtime.ExceptionServices

Dim capturee As ExceptionDispatchInfo = Nothing
Try
    Risque()
Catch ex As Exception
    capturee = ExceptionDispatchInfo.Capture(ex)
End Try

' … plus tard, ailleurs …
capturee?.Throw()   ' relance en conservant la pile d'origine
```

---

## Les exceptions personnalisées

### Quand en créer une ?

Créez une exception personnalisée lorsqu'un type d'erreur **a un sens métier propre** et que
l'appelant pourrait vouloir le **traiter spécifiquement**. Exemples : `CommandeIntrouvableException`,
`SoldeInsuffisantException`, `RegleMetierException`. Si une exception standard décrit déjà
correctement la situation (`ArgumentException`, `InvalidOperationException`…), **utilisez-la**
plutôt que d'en inventer une.

### Le patron standard

Par convention, le nom se **termine par `Exception`**, et on hérite **directement de `Exception`**.
Fournissez les **trois constructeurs habituels** pour rester cohérent avec le reste de .NET :

```vb
Public Class CommandeIntrouvableException
    Inherits Exception

    ' Donnée métier portée par l'exception
    Public ReadOnly Property CommandeId As Integer

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    ' Surcharge pratique qui enrichit le contexte métier
    Public Sub New(commandeId As Integer)
        MyBase.New($"La commande {commandeId} est introuvable.")
        Me.CommandeId = commandeId
    End Sub
End Class
```

Utilisation :

```vb
Dim commande = _repo.Trouver(id)
If commande Is Nothing Then
    Throw New CommandeIntrouvableException(id)
End If
```

Et côté appelant, un `Catch` ciblé devient possible :

```vb
Try
    Traiter(id)
Catch ex As CommandeIntrouvableException
    Console.WriteLine($"Commande {ex.CommandeId} absente — affichage d'un message à l'utilisateur.")
End Try
```

### Attacher du contexte avec `Data`

Pour ajouter des informations sans créer une nouvelle propriété, le dictionnaire `Data` est
pratique :

```vb
Dim ex As New InvalidOperationException("Transition d'état invalide.")
ex.Data("EtatActuel") = etatActuel
ex.Data("EtatDemande") = etatDemande
Throw ex
```

### 🆕 Modernité .NET : plus besoin du constructeur de sérialisation

Dans du code ancien, vous verrez souvent les exceptions personnalisées décorées de
`<Serializable>` et dotées d'un constructeur de sérialisation
`Protected Sub New(info As SerializationInfo, context As StreamingContext)`.

> ⚠️ **Ce n'est plus nécessaire ni recommandé dans le .NET moderne (dont .NET 10).** Ce patron
> reposait sur la sérialisation binaire via `BinaryFormatter`, **obsolète depuis .NET 5** pour
> des raisons de sécurité et dont l'**implémentation a été retirée du runtime à partir de
> .NET 9**. Les **trois constructeurs standard suffisent** : n'ajoutez pas ce code hérité dans
> de nouvelles exceptions. (Vous le rencontrerez en revanche lors de la maintenance de legacy —
> voir [module 11](../11-migration-legacy/README.md).)

---

## Et le code legacy : `On Error GoTo`

Pour la compatibilité avec VB6, VB.NET accepte toujours la **gestion d'erreurs non structurée**
héritée du Basic classique : `On Error GoTo`, `On Error Resume Next`, l'objet `Err`, `Resume`…

```vb
' Style hérité (VB6) — à ne pas employer dans du code neuf
On Error GoTo Gestion
    Risque()
Exit Sub
Gestion:
    MsgBox("Erreur : " & Err.Description)
```

> ⚠️ **À proscrire dans tout nouveau code.** La gestion structurée (`Try`/`Catch`) est plus
> claire, plus sûre et plus précise. De plus, **on ne peut pas mélanger** `On Error` et
> `Try`/`Catch` dans une même méthode. Si vous maintenez du code qui en contient, traitez sa
> conversion dans le cadre de la migration — voir
> [11.2 — VB6 → VB.NET](../11-migration-legacy/02-vb6-vers-vbnet.md).

---

## Un mot sur l'asynchrone

Dans une méthode `Async`, une exception levée n'est pas propagée immédiatement : elle est
**capturée dans la `Task` renvoyée** puis **relancée au moment du `Await`**. On l'intercepte
donc avec un `Try`/`Catch` autour de l'`Await`, comme du code synchrone. Les cas particuliers
(opérations parallèles et `AggregateException`, annulation via `OperationCanceledException`)
sont traités au [module 4.3 — Gestion des exceptions asynchrones](../04-async/03-exceptions-async.md).

```vb
Try
    Dim resultat = Await TelechargerAsync(url)
Catch ex As HttpRequestException
    ' … gérée comme une exception synchrone …
End Try
```

---

## Bonnes pratiques de synthèse

- **Interceptez le plus précis possible.** Un `Catch ex As Exception` fourre-tout masque les
  vrais problèmes (et attrape même ce qu'il ne faut pas).
- **Ne « gobez » jamais une exception.** Un `Catch` vide qui ne fait rien est un bug en
  puissance : au minimum, journalisez.
- **N'utilisez pas les exceptions pour le flux normal.** Elles sont coûteuses au *lancer* ;
  réservez-les aux situations vraiment anormales.
- **Préservez la pile : `Throw`, pas `Throw ex`.**
- **Ajoutez du contexte en enveloppant** (`InnerException`) plutôt qu'en remplaçant l'erreur
  d'origine.
- **Exploitez les filtres `When`** pour discriminer sans dérouler la pile inutilement.
- **Libérez vos ressources** avec `Using` (ou `Finally`), toujours.
- **Ne capturez que ce que vous savez traiter.** Sinon, laissez remonter : mieux vaut une
  erreur visible qu'un état corrompu silencieux.
- **Journalisez à la frontière**, pas à chaque niveau, pour éviter les logs en double
  (voir [12.3 — Journalisation](03-journalisation.md)).
- **Validez les arguments tôt**, avec les gardes `ArgumentNullException.ThrowIfNull` & co.
- **Pas de `<Serializable>` ni de constructeur de sérialisation** dans les exceptions neuves
  (.NET moderne).

---

## À retenir

La gestion des exceptions n'est pas une formalité défensive : c'est ce qui distingue un code
qui *cache* ses erreurs d'un code qui les *exprime clairement*. En VB.NET, tout l'outillage est
là, natif et complet — `Try`/`Catch`/`Finally`, des filtres `When` que C# a longtemps enviés,
et des exceptions personnalisées simples à écrire. L'enjeu n'est pas technique mais d'**hygiène** :
intercepter au bon niveau, préserver l'information de diagnostic, et ne jamais masquer ce qu'on
ne sait pas traiter.

➡️ Section suivante : **[12.2 — Débogage avec Visual Studio 2026](02-debogage.md)** 🆕, pour
passer de « gérer les erreurs » à « comprendre les bugs ».

⏭️ [Débogage (points d'arrêt, espions, Hot Reload, débogage asynchrone) ; outils de VS 2026](/12-exceptions-debogage/02-debogage.md)
