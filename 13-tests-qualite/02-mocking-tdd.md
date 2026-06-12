🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 13.2 — *Mocking* (Moq, NSubstitute) et TDD

> **Module 13 — Tests et qualité du code**
> Deux outils complémentaires : **isoler** vraiment l'unité testée, et laisser les tests
> **guider la conception**.

---

Cette section réunit deux sujets liés. Le **mocking** répond à une question pratique : comment
tester une unité sans dépendre de ses voisines (base de données, service réseau…) ? Le **TDD**
répond à une question de méthode : et si on écrivait le test *avant* le code, pour qu'il en
oriente la conception ? Les deux se rejoignent, car bien tester pousse à bien concevoir.

---

# Partie A — Le *mocking* : isoler l'unité testée

## Le problème : les dépendances

Un test unitaire doit être **rapide** et **isolé** (principes FIRST, cf. [13.1](01-tests-unitaires.md)).
Or la plupart des classes collaborent avec d'autres : un `ServiceCommandes` interroge un
*repository*, qui interroge une base de données. Tester `ServiceCommandes` « tel quel »
reviendrait à tester aussi la base — lent, fragile, non déterministe.

La solution : **remplacer les dépendances par des doublures** contrôlées le temps du test. On
isole ainsi l'unité, et on décide exactement ce que ses dépendances renvoient.

## La famille des doublures (*test doubles*)

« *Mock* » est souvent employé comme terme générique, mais on distingue plusieurs sortes de
doublures :

| Doublure | Rôle |
|----------|------|
| **Dummy** | Objet passé pour satisfaire une signature, jamais réellement utilisé. |
| **Stub** | Fournit des réponses **préprogrammées** (« quand on appelle `Trouver(42)`, renvoie ceci »). |
| **Spy** | Comme un stub, mais **enregistre** comment il a été appelé, pour vérifier après coup. |
| **Mock** | Doublure dont on **vérifie les interactions** (« cette méthode a-t-elle été appelée, et comment ? »). |
| **Fake** | Implémentation **allégée mais fonctionnelle** (ex. un *repository* en mémoire). |

En pratique, deux usages dominent : **fournir des réponses** (stub) et **vérifier des appels**
(mock). Les bibliothèques comme Moq et NSubstitute couvrent les deux.

## Prérequis : concevoir pour la testabilité

C'est le point décisif, et il précède toute bibliothèque :

> 🔑 **On ne peut remplacer que ce qui est une abstraction.** Le *mocking* suppose soit une
> **interface**, soit un membre de classe déclaré **`Overridable`** (équivalent VB de `virtual`).
> Une méthode « ordinaire » d'une classe concrète **ne peut pas** être interceptée par une
> doublure.

D'où l'importance des **interfaces** (cf. [3.4](../03-poo/04-interfaces.md)) et de l'**injection
de dépendances**. Comparez :

```vb
' ✅ Testable : la dépendance est une interface injectée → remplaçable par une doublure
Public Class ServiceCommandes
    Private ReadOnly _repo As IRepositoryCommandes

    Public Sub New(repo As IRepositoryCommandes)
        _repo = repo
    End Sub

    Public Function Obtenir(id As Integer) As Commande
        Return _repo.Trouver(id)
    End Function
End Class
```

```vb
' ❌ Non testable : la dépendance est figée, impossible à remplacer
Public Class ServiceCommandes
    Private ReadOnly _repo As New RepositoryCommandesSql()   ' couplage en dur
    ' …
End Class
```

La testabilité n'est donc pas une contrainte des tests : c'est une **conséquence d'une bonne
conception orientée objet**.

## Moq

Moq construit une doublure dynamique à partir d'une interface (ou d'une classe à membres
`Overridable`). On **configure** des réponses (`Setup`/`Returns`) et on **vérifie** des appels
(`Verify`).

```vb
Imports Moq
Imports Xunit

Public Class ServiceCommandesTests

    <Fact>
    Public Sub Obtenir_CommandeExistante_RetourneLaCommande()
        ' Arrange
        Dim mockRepo As New Mock(Of IRepositoryCommandes)()
        mockRepo.Setup(Function(r) r.Trouver(42)) _
                .Returns(New Commande With {.Id = 42})

        Dim service As New ServiceCommandes(mockRepo.Object)   ' .Object = la doublure

        ' Act
        Dim commande = service.Obtenir(42)

        ' Assert
        Assert.Equal(42, commande.Id)
        mockRepo.Verify(Function(r) r.Trouver(42), Times.Once())   ' appelé exactement une fois
    End Sub
End Class
```

Quelques mécanismes utiles :

- **Correspondances d'arguments** : `It.IsAny(Of Integer)()` (n'importe quelle valeur),
  `It.Is(Of Integer)(Function(i) i > 0)` (selon un prédicat).
- **Lever une exception** : `.Throws(Of InvalidOperationException)()`.
- **Asynchrone** : `.ReturnsAsync(valeur)` pour une méthode renvoyant `Task(Of T)`.
- **Méthode `Sub` (sans retour)** : on utilise une lambda `Sub(r) …` dans `Setup`/`Verify`.

```vb
mockRepo.Setup(Function(r) r.Trouver(It.IsAny(Of Integer)())).Returns(New Commande())
mockRepo.Verify(Sub(r) r.Enregistrer(It.IsAny(Of Commande)()), Times.Once())
```

## NSubstitute

NSubstitute vise une syntaxe plus légère, sans objet « wrapper » : `Substitute.For` renvoie
**directement** la doublure.

```vb
Imports NSubstitute
Imports Xunit

Public Class ServiceCommandesTests

    <Fact>
    Public Sub Obtenir_CommandeExistante_RetourneLaCommande()
        ' Arrange
        Dim repo = Substitute.For(Of IRepositoryCommandes)()
        repo.Trouver(42).Returns(New Commande With {.Id = 42})

        Dim service As New ServiceCommandes(repo)   ' pas de .Object : repo EST la doublure

        ' Act
        Dim commande = service.Obtenir(42)

        ' Assert
        Assert.Equal(42, commande.Id)
        repo.Received(1).Trouver(42)   ' vérifie l'appel
    End Sub
End Class
```

Les équivalents des correspondances d'arguments sont `Arg.Any(Of Integer)()` et
`Arg.Is(Of Integer)(Function(i) i > 0)`. La même contrainte s'applique : interfaces ou membres
`Overridable` uniquement.

## Moq ou NSubstitute ?

Les deux font le même travail ; le choix tient au style (Moq plus explicite avec `.Setup`/`.Verify`,
NSubstitute plus fluide) et aux habitudes d'équipe.

> ⚠️ **Une note de contexte honnête (2023).** Une version de **Moq** a intégré un composant
> tiers (*SponsorLink*) qui collectait une **empreinte de l'adresse e-mail** du développeur,
> suscitant une vive controverse sur la confidentialité. Il a été **retiré peu après**, mais
> l'épisode a conduit de nombreuses équipes à privilégier **NSubstitute** (ou **FakeItEasy**).
> Les deux restent des choix viables ; tranchez selon les préférences et la politique de votre
> organisation.

## Quand *ne pas* mocker

Le *mocking* est un outil, pas un réflexe. Quelques garde-fous :

- **Ne sur-mockez pas.** Tester chaque interaction interne produit des tests **fragiles** qui
  cassent au moindre refactoring, parce qu'ils vérifient *comment* le code fait plutôt que *ce
  qu'il produit*. Préférez vérifier le **résultat** quand c'est possible.
- **Ne mockez pas ce que vous ne possédez pas.** Plutôt que de mocker directement un client HTTP
  ou une API tierce, **enveloppez-les** derrière votre propre interface, et mockez celle-ci.
- **Utilisez de vrais objets quand ils sont simples et rapides.** Inutile de mocker une classe de
  calcul pure et sans dépendance : appelez-la directement.
- **Mockez aux frontières** (accès aux données, services externes, horloge, système de fichiers),
  pas au cœur de la logique métier.

---

# Partie B — Le TDD : laisser les tests guider la conception

## Qu'est-ce que le TDD ?

Le **Test-Driven Development** (développement piloté par les tests) renverse l'ordre habituel :
on écrit le **test d'abord**, puis le code qui le satisfait. Le test n'est plus une vérification
*a posteriori* mais un **outil de conception** : il décrit le comportement voulu avant qu'il
n'existe.

## Le cycle rouge-vert-refactor

Le TDD progresse par très petites itérations :

1. 🔴 **Rouge** — écrire un test pour le **prochain** comportement attendu. Il échoue (le code
   n'existe pas encore, ou ne fait pas l'affaire).
2. 🟢 **Vert** — écrire le **minimum** de code de production pour faire passer ce test.
3. 🔵 **Refactor** — nettoyer le code (et les tests) **sans changer le comportement**, en gardant
   tous les tests au vert.

Puis on recommence pour le comportement suivant. À titre d'illustration :

```vb
' 🔴 ROUGE — le test précède l'implémentation
<Fact>
Public Sub Additionner_DeuxNombres_RetourneLaSomme()
    Assert.Equal(5, New Calculatrice().Additionner(2, 3))   ' Additionner n'existe pas encore
End Sub
```

```vb
' 🟢 VERT — le strict minimum pour passer au vert
Public Class Calculatrice
    Public Function Additionner(a As Integer, b As Integer) As Integer
        Return a + b
    End Function
End Class
```

```vb
' 🔵 REFACTOR — améliorer la structure si nécessaire, tests toujours verts
'    (ici, rien à refactorer ; on enchaîne sur le test suivant)
```

## Les trois lois du TDD

Souvent attribuées à Robert C. Martin, elles formalisent la discipline du cycle :

1. N'écrivez **aucun code de production** tant qu'un test unitaire **en échec** ne l'exige.
2. N'écrivez **pas plus de test** qu'il n'en faut pour échouer (ne pas compiler *est* un échec).
3. N'écrivez **pas plus de code de production** qu'il n'en faut pour faire passer le test courant.

## Bénéfices et limites

**Bénéfices :**

- **Pression de conception** : un code difficile à tester signale souvent un code mal couplé. Le
  TDD pousse naturellement vers des unités petites, à dépendances injectées — donc testables.
- **Filet de sécurité** dès la première ligne : on refactore en confiance.
- **Documentation vivante** : les tests décrivent le comportement attendu, exemple à l'appui.
- **Retour rapide** : on sait immédiatement si l'on a cassé quelque chose.

**Limites — à connaître pour ne pas en faire un dogme :**

- Ce n'est **pas adapté à tout** : exploration (*spike*), code jetable, UI, certains scénarios
  d'intégration s'y prêtent mal.
- Mal pratiqué, il produit des tests **fragiles** ou **tautologiques** (cf. la sur-utilisation des
  mocks).
- Il ne **remplace pas** les tests d'intégration et de bout en bout (cf. [13.3](03-tests-integration.md)).

## *Mocking* et TDD : deux écoles

Le degré de *mocking* renvoie à deux traditions du TDD :

- **École « de Londres » (*mockist*)** : on mocke tous les collaborateurs et on teste les
  **interactions**, en partant de l'extérieur vers l'intérieur (*outside-in*). Beaucoup de mocks.
- **École « de Détroit / Chicago » (*classicist*)** : on utilise de **vrais objets** autant que
  possible et on teste les **résultats**, en ne mockant qu'aux **frontières**. Peu de mocks.

Aucune n'a raison contre l'autre. En pratique, une approche **classiciste avec mocking aux
frontières** donne souvent des tests plus robustes — c'est la ligne suggérée dans la partie A.

## TDD et VB.NET — un mot sur le legacy

Le TDD est une **pratique**, indépendante du langage : il s'applique en VB.NET comme ailleurs,
avec les frameworks et bibliothèques vus ici, tous pleinement consommables.

> 🔗 **Pour du code existant**, on n'« écrit pas le test d'abord » — il est déjà là. On procède à
> l'inverse : on écrit d'abord des **tests de caractérisation** qui figent le comportement
> *actuel* du code, *avant* de le modifier ou de le migrer. C'est le filet de sécurité de la
> modernisation du legacy (cf. [11.6](../11-migration-legacy/06-moderniser.md) et
> [11.7](../11-migration-legacy/07-gestion-risques.md)), un scénario central pour VB.NET.

---

## À retenir

Le *mocking* permet d'**isoler** l'unité testée en remplaçant ses dépendances par des doublures
contrôlées — à condition d'avoir conçu ces dépendances comme des **abstractions** (interfaces ou
membres `Overridable`) : la testabilité découle d'une bonne conception OO. **Moq** et
**NSubstitute** font ce travail en VB.NET sans difficulté ; on les emploie **aux frontières**,
sans sur-mocker. Le **TDD**, lui, fait du test un outil de **conception** via le cycle
rouge → vert → refactor : puissant pour orienter du code neuf, à condition de ne pas le pratiquer
de façon dogmatique. Pour le legacy, on en garde l'esprit avec des **tests de caractérisation**.

➡️ Section suivante : **[13.3 — Tests d'intégration](03-tests-integration.md)**, pour vérifier que
les composants fonctionnent *ensemble*, contre une infrastructure réelle.

⏭️ [Tests d'intégration (WebApplicationFactory, Testcontainers, tests avec base de données)](/13-tests-qualite/03-tests-integration.md)
