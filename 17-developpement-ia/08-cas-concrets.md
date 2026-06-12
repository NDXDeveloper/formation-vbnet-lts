🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.8 Cas concrets (migration VB6→VB.NET, API REST, WPF MVVM)

Cette section montre le workflow des sections [17.2](02-prompting-vbnet.md) à [17.7](07-limites-pieges.md) **en action**, sur trois scénarios délibérément choisis : chacun met sous tension une facette différente du travail avec l'IA. Ce sont des **démonstrations du processus**, pas des implémentations — le fond de chaque domaine appartient à son module dédié ([11](../11-migration-legacy/README.md), [8](../08-services-web/README.md), [6](../06-wpf/README.md)). On observe ici la dimension IA se dérouler, et on voit où les pièges frappent et comment la validation les rattrape.

| Cas | Facette dominante |
|-----|-------------------|
| Migration VB6 → VB.NET | Pièges sémantiques + filet de sécurité — le cas **à haut risque** |
| API REST | Terrain **productif**, mais tenir la frontière VB/C# |
| WPF MVVM | Le **raccourci C#-only halluciné** |

---

## Cas 1 — Migration VB6 → VB.NET : le cas à haut risque

Ce scénario concentre le **double biais** ([17.3](03-migration-legacy-ia.md)) et les pièges sémantiques : il connaît mal la source (VB6, rare dans les données) et écrit mal la cible (VB, biais C#). C'est le cas où la vigilance prime.

**Le déroulé.** On commence par poser le **filet de tests de caractérisation** ([17.4](04-generer-tests-doc.md)) — non négociable avant de toucher au code, car il fixe le comportement de référence. Puis on fait **expliquer** par l'IA chaque bloc VB6 obscur avant de le traduire, et on traduit **par petits lots** (gabarits de [17.3](03-migration-legacy-ia.md)) : compiler, rejouer les tests, recommencer.

**Le piège en situation.** Un bloc `On Error Resume Next` est l'archétype de l'erreur sémantique (catégorie C/D de [17.7](07-limites-pieges.md)). Sollicitée, l'IA l'enveloppe volontiers dans un **unique** `Try/Catch` :

```vb
' VB6 d'origine : une ligne illisible est ignorée, et le calcul CONTINUE.
'   On Error Resume Next
'   total = total + CDbl(ligne(i))
'   moyenne = total / n

' ❌ Traduction qui change le comportement : un seul Try/Catch sur le bloc.
'    Une erreur sur la 1re instruction SAUTE le calcul de moyenne — alors que
'    Resume Next aurait continué à l'instruction suivante.
Try
    total = total + CDbl(ligne(i))
    moyenne = total / n
Catch
End Try
```

Le `Resume Next` reprend à l'**instruction suivante**, là où un `Try/Catch` de bloc abandonne tout le bloc : la portée n'est pas la même, et le comportement diverge silencieusement.

**La validation qui rattrape.** Le compilateur ne voit rien (le code est valide). C'est le **test de non-régression** — qui encode « une ligne illisible est ignorée, la moyenne est tout de même calculée » — qui révèle la divergence. Les **tests sont l'arbitre** de la sémantique. (Pièges associés : `Variant` → `Object`, et la bibliothèque `Microsoft.VisualBasic.Compatibility`, absente de .NET moderne — voir [17.3](03-migration-legacy-ia.md) et [11.2](../11-migration-legacy/02-vb6-vers-vbnet.md).)

**À retenir.** L'IA accélère le volume, mais ici le **filet de tests** et la **vigilance sémantique** font tout.

---

## Cas 2 — API REST en VB.NET : le cas productif (et la frontière)

Scénario plus confortable : VB.NET expose très bien une Web API **par contrôleurs** ([8.2](../08-services-web/02-web-api-controllers.md) ✅), et consommer une API REST relève de la pure **consommation**. L'IA y est réellement productive — mais le cas met sous tension la **conscience de la frontière** VB/C#.

**Le déroulé.** Génération de contrôleurs (`ControllerBase`), de routes, de la liaison de modèle, de l'injection de dépendances, de la sérialisation `System.Text.Json`, de la documentation OpenAPI : l'IA est solide, la friction est faible. Côté client, `HttpClient` / `IHttpClientFactory` et la désérialisation sont du terrain de consommation, où le modèle est fiable.

**Le piège en situation.** Entraîné sur le C# moderne, le modèle **dérive vers les Minimal APIs** : il propose un style à instructions de haut niveau (`Program.cs`, `app.MapGet(...)`) qui — rappel de [8.3](../08-services-web/03-limites-web-vbnet.md) — **n'a pas de modèle de projet en VB** et impose une syntaxe contrainte (pas de *top-level statements*). Ce n'est pas une recommandation, c'est un **signal** : le modèle vous pousse vers un idiome C#.

**La validation qui rattrape.** Le compilateur et `Option Strict On` arrêtent la syntaxe étrangère ; mais c'est surtout votre **connaissance de la frontière** ([8.3](../08-services-web/03-limites-web-vbnet.md), [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)) qui vous fait revenir aux **contrôleurs** — ou décider, en pleine conscience, de **déléguer ce bout à C#** au titre de la stratégie hybride ([module 10](../10-hybride-vbnet-csharp/README.md)).

**À retenir.** Sur ce terrain confortable, l'IA est un excellent accélérateur, à condition de **tenir la frontière** : quand elle propose les Minimal APIs, on reconnaît le penchant C# et on choisit.

---

## Cas 3 — WPF MVVM : le cas du raccourci C#-only halluciné

Démonstration manuelle de la **catégorie B** de [17.7](07-limites-pieges.md) — la *fonctionnalité fantôme* : le modèle propose avec assurance un raccourci qui n'existe pas en VB.

**Le déroulé.** L'IA aide sur le *boilerplate* MVVM : `INotifyPropertyChanged`, `ObservableCollection`, `ICommand`, la liaison de données, la structure du *ViewModel*.

**Le piège en situation.** Invitée à « réduire le *boilerplate* avec `CommunityToolkit.Mvvm` », l'IA suggère les attributs des **source generators** — `<ObservableProperty>`, `<RelayCommand>`. Or ces générateurs sont **réservés au C#** ([6.6](../06-wpf/06-mvvm.md), et [Annexe B](../annexes/frontiere-vbnet-csharp/README.md) : un *source generator* « auteur » s'écrit en C#) :

```vb
' ❌ Source generators de CommunityToolkit.Mvvm : réservés au C#.
'    En VB, ces attributs sont RECONNUS mais INERTES — aucune propriété ni commande
'    n'est générée, et la liaison XAML vers "Nom" n'a aucune cible.
<ObservableProperty>
Private _nom As String

<RelayCommand>
Private Sub Enregistrer()
End Sub
```

En VB, on utilise directement les **classes de base**, sans générateur :

```vb
' ✅ En VB : on hérite d'ObservableObject et on instancie RelayCommand à la main.
Public Class ClientViewModel
    Inherits ObservableObject

    Private _nom As String
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            SetProperty(_nom, value)          ' fourni par ObservableObject
        End Set
    End Property

    Public ReadOnly Property EnregistrerCommand As ICommand =
        New RelayCommand(AddressOf Enregistrer)

    Private Sub Enregistrer()
    End Sub
End Class
```

**La validation qui rattrape.** Le piège est sournois car le compilateur **ne hurle pas** : l'attribut est valide, simplement sans effet. Rien n'est généré, et la liaison échoue faute de cible. Ce qui rattrape ici, c'est la **connaissance de la frontière** (les générateurs sont C#-only) et le constat que la propriété attendue n'existe pas. L'arbitre n'est pas seulement le compilateur, mais la **documentation** et votre carte du périmètre.

**À retenir.** Le « raccourci moderne » de l'IA est souvent une fonctionnalité **C#-only**. En VB, on emploie les classes de base (plus verbeux, mais pleinement fonctionnel). Le réflexe : devant une magie pilotée par attributs, vérifier si elle repose sur un *source generator* (alors C#-only).

---

## Le fil commun des trois cas

Chaque scénario a mis sous tension une facette distincte : la migration VB6 stresse les **pièges sémantiques** et impose le **filet de tests** (cas à haut risque) ; l'API REST est **productive** mais exige de **tenir la frontière** ; le WPF MVVM illustre le **raccourci C#-only halluciné**.

Le workflow, lui, reste identique d'un cas à l'autre : configurer pour VB, générer, valider avec **le bon arbitre**. Ce qui change, c'est le **risque dominant** selon le domaine. Savoir lequel guette dans votre scénario permet d'y concentrer la vigilance — c'est tout l'enseignement de ces trois exemples.

Pour le fond : migration → [17.3](03-migration-legacy-ia.md) et [module 11](../11-migration-legacy/README.md) ; Web API → [8.2](../08-services-web/02-web-api-controllers.md)–[8.3](../08-services-web/03-limites-web-vbnet.md) et [Annexe B](../annexes/frontiere-vbnet-csharp/README.md) ; WPF MVVM → [6.6](../06-wpf/06-mvvm.md). Le catalogue des pièges est en [17.7](07-limites-pieges.md), les erreurs fréquentes dans l'[Annexe G](../annexes/faq-depannage/README.md). La dernière section, [17.9](09-consommer-ia.md), bascule du *« développer avec l'IA »* vers *« intégrer l'IA dans vos applications »* — le terrain de la consommation, où VB.NET est pleinement à l'aise.

⏭️ [Intégrer l'IA dans vos applications VB.NET (consommation d'API)](/17-developpement-ia/09-consommer-ia.md)
