🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 13.1 — Tests unitaires (xUnit, NUnit, MSTest ; `Microsoft.Testing.Platform` dans .NET 10) 🆕

> **Module 13 — Tests et qualité du code**
> Le socle : écrire des tests rapides et isolés qui vérifient une unité de code, et le restent.

---

## Qu'est-ce qu'un test unitaire ?

Un **test unitaire** vérifie automatiquement le comportement d'une **petite unité** de code — le
plus souvent une méthode — en l'isolant de ses dépendances. Il répond à une question précise :
« *pour telles entrées, cette méthode produit-elle le bon résultat ?* ». Quand on modifie le
code, on relance la suite de tests : tout au vert signifie qu'on n'a rien cassé.

Un bon test unitaire respecte les principes **FIRST** :

- **F**ast (rapide) : des millisecondes, pour pouvoir en exécuter des milliers en continu.
- **I**solé : indépendant des autres tests et des dépendances externes (base, réseau, fichiers).
- **R**épétable : même résultat à chaque exécution, partout (pas de dépendance à l'heure, au hasard…).
- **S**elf-validating (auto-validant) : il passe ou il échoue, sans interprétation humaine.
- **T**imely (à temps) : écrit au plus près du code qu'il vérifie (idéalement avant, cf. TDD en [13.2](02-mocking-tdd.md)).

> 💡 Un test qui touche la base de données ou le réseau n'est plus un test *unitaire* mais
> d'**intégration** (cf. [13.3](03-tests-integration.md)). Les deux sont utiles, mais ne se
> confondent pas.

---

## L'anatomie d'un test : Arrange-Act-Assert

La structure de référence est **AAA** — trois temps, souvent matérialisés par des commentaires :

```vb
<Fact>
Public Sub Additionner_DeuxNombresPositifs_RetourneLaSomme()
    ' Arrange — préparer le contexte
    Dim calc As New Calculatrice()

    ' Act — exécuter l'unité testée
    Dim resultat = calc.Additionner(2, 3)

    ' Assert — vérifier le résultat
    Assert.Equal(5, resultat)
End Sub
```

Pour le **nommage**, une convention répandue est `Methode_Scenario_ResultatAttendu`. Le nom doit
décrire l'intention : à la lecture du rapport de tests, on comprend *ce qui* a échoué sans ouvrir
le code.

---

## Les trois frameworks de référence

Trois frameworks dominent l'écosystème .NET. **Tous fonctionnent en VB.NET** — MSTest et NUnit
sont d'ailleurs présentés par Microsoft comme des frameworks « pour **tous les langages .NET** ».

| | xUnit.net | NUnit | MSTest |
|---|-----------|-------|--------|
| **Origine** | communautaire (auteur de NUnit v2) | .NET Foundation, porté de JUnit | Microsoft |
| **Test simple** | `<Fact>` | `<Test>` | `<TestMethod>` (+ `<TestClass>`) |
| **Test paramétré** | `<Theory>` + `<InlineData>` | `<TestCase>` | `<DataRow>` (sur `<TestMethod>`) |
| **Initialisation** | constructeur | `<SetUp>` | `<TestInitialize>` |
| **Nettoyage** | `Dispose` (`IDisposable`) | `<TearDown>` | `<TestCleanup>` |
| **Assertions** | `Assert.Equal(...)` | `Assert.That(...)` (contraintes) | `Assert.AreEqual(...)` |

Les trois sont d'excellents choix : la différence tient surtout au style et aux habitudes
d'équipe. Voyons un même test dans chacun.

### MSTest

```vb
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class CalculatriceTests

    <TestMethod>
    Public Sub Additionner_DeuxNombres_RetourneLaSomme()
        Dim calc As New Calculatrice()
        Dim resultat = calc.Additionner(2, 3)
        Assert.AreEqual(5, resultat)   ' AreEqual(attendu, obtenu)
    End Sub
End Class
```

### NUnit — et un piège VB.NET à connaître

```vb
Imports NUnit.Framework

<TestFixture>   ' optionnel en NUnit moderne
Public Class CalculatriceTests

    <Test>
    Public Sub Additionner_DeuxNombres_RetourneLaSomme()
        Dim calc As New Calculatrice()
        Dim resultat = calc.Additionner(2, 3)
        Assert.That(resultat, [Is].EqualTo(5))
    End Sub
End Class
```

> ⚠️ **Le piège n°1 de NUnit en VB.NET : `Is` est un mot-clé du langage.** En VB.NET, `Is` est
> l'opérateur de comparaison de références — on **ne peut donc pas** écrire `Is.EqualTo(...)`
> comme en C#. Il faut **échapper** le nom de la classe de contraintes avec des crochets :
> **`[Is].EqualTo(5)`**. Sans cela, le code ne compile pas. (Les autres classes de contraintes
> — `Throws`, `Has`, `Does` — ne sont pas des mots-clés VB et s'utilisent normalement.) C'est
> exactement le genre de détail que la documentation C#, omniprésente, passe sous silence.

### xUnit

```vb
Imports Xunit

Public Class CalculatriceTests   ' aucun attribut de classe nécessaire

    <Fact>
    Public Sub Additionner_DeuxNombres_RetourneLaSomme()
        Dim calc As New Calculatrice()
        Dim resultat = calc.Additionner(2, 3)
        Assert.Equal(5, resultat)   ' Equal(attendu, obtenu)
    End Sub
End Class
```

---

## Tests paramétrés (*data-driven*)

Plutôt que de dupliquer un test pour chaque jeu de données, on le **paramètre** : un seul test,
exécuté autant de fois qu'il y a de cas.

```vb
' xUnit
<Theory>
<InlineData(2, 3, 5)>
<InlineData(-1, 1, 0)>
<InlineData(0, 0, 0)>
Public Sub Additionner_PlusieursCas_RetourneLaSomme(a As Integer, b As Integer, attendu As Integer)
    Dim calc As New Calculatrice()
    Assert.Equal(attendu, calc.Additionner(a, b))
End Sub
```

```vb
' NUnit
<TestCase(2, 3, 5)>
<TestCase(-1, 1, 0)>
Public Sub Additionner_PlusieursCas(a As Integer, b As Integer, attendu As Integer)
    Assert.That(New Calculatrice().Additionner(a, b), [Is].EqualTo(attendu))
End Sub
```

En MSTest moderne, il suffit de poser plusieurs `<DataRow(2, 3, 5)>` sur un `<TestMethod>`
ordinaire — l'ancien attribut `<DataTestMethod>` est **obsolète** (l'analyseur MSTEST0044
recommande de s'en passer).

---

## Tester du code asynchrone

Les trois frameworks gèrent nativement les méthodes de test asynchrones : la méthode devient une
`Async Function … As Task`, et on `Await` l'opération testée.

```vb
<Fact>
Public Async Function Charger_Async_RetourneLesDonnees() As Task
    Dim service As New ServiceCatalogue()
    Dim produits = Await service.ChargerAsync()
    Assert.NotEmpty(produits)
End Function
```

> 💡 Renvoyez bien une `Task` (jamais `Async Sub` dans un test) : sinon le framework ne peut pas
> *attendre* la fin du test, et un échec asynchrone passerait inaperçu.

---

## Tester qu'une exception est levée

Vérifier le « chemin d'erreur » est aussi important que le cas nominal (cf. [12.1](../12-exceptions-debogage/01-exceptions.md)).
On encapsule l'appel fautif dans une lambda `Sub() …` :

```vb
' xUnit / NUnit / MSTest exposent tous une variante de ce motif
<Fact>
Public Sub Diviser_ParZero_LeveUneException()
    Dim calc As New Calculatrice()
    Assert.Throws(Of DivideByZeroException)(Sub() calc.Diviser(10, 0))
End Sub
```

Pour le code asynchrone, utilisez la variante `Assert.ThrowsAsync(Of T)(Function() …)`.

---

## Les assertions

Chaque framework fournit ses assertions intégrées : `Assert.Equal` / `Assert.True` /
`Assert.NotNull` / `Assert.Contains`… en xUnit, `Assert.AreEqual` / `Assert.IsTrue`… en MSTest,
et le modèle à contraintes `Assert.That(valeur, [Is].EqualTo(…))` en NUnit, très expressif.

> 💡 **Bibliothèques d'assertions fluides.** Des bibliothèques tierces (Shouldly,
> AwesomeAssertions, FluentAssertions…) offrent une syntaxe plus lisible
> (`resultat.ShouldBe(5)`). Elles sont optionnelles — les assertions intégrées suffisent
> largement pour démarrer — et **certaines ont récemment changé de modèle de licence** :
> vérifiez les conditions avant d'en adopter une en contexte commercial.

---

## 🆕 Microsoft.Testing.Platform : la nouvelle plateforme de test de .NET 10

Jusqu'ici, un rouage restait invisible : c'est **VSTest** qui, depuis Visual Studio 2010,
pilotait l'exécution des tests (`dotnet test`, l'Explorateur de tests, `vstest.console`).
**Microsoft.Testing.Platform (MTP)** est le moteur appelé à le **remplacer** : plus léger, plus
portable, plus performant et nettement plus extensible.

### Ce que MTP change

- **Les projets de test deviennent des exécutables autonomes.** Une fois compilé, un projet de
  test peut être lancé **directement** (en exécutant le binaire produit, ou via `dotnet run`) :
  l'exécutable suffit à lancer les tests, sans hôte VSTest. C'est le même modèle que xUnit v3.
- **Support natif dans le SDK .NET 10.** À partir du **SDK .NET 10**, `dotnet test` prend en
  charge MTP **nativement** (en .NET 9 et avant, ce n'était qu'une option bâtie sur VSTest).
  La commande reconnaît les **deux** plateformes — VSTest *et* MTP — et tout framework bâti sur
  l'une ou l'autre est automatiquement pris en charge.
- **Adopté par tous les grands frameworks** : MSTest, NUnit et xUnit (à partir de **xUnit v3**),
  ainsi que d'autres comme TUnit.

### L'activer selon le framework

L'activation se fait par une propriété MSBuild, dans le `.vbproj` ou un `Directory.Build.props` :

| Framework | Propriété d'activation |
|-----------|------------------------|
| MSTest | `<EnableMSTestRunner>true</EnableMSTestRunner>` |
| NUnit | `<EnableNUnitRunner>true</EnableNUnitRunner>` |
| xUnit (**v3**) | `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` |

Pour MSTest et NUnit, on ajoute aussi `<OutputType>Exe</OutputType>` — cohérent avec le modèle :
le projet de test **devient un exécutable**. Le point d'entrée (`Main`) et le fichier de
configuration sont **générés automatiquement**, et les extensions installées sont **détectées et
enregistrées** sans code supplémentaire.

### ⚠️ Points de vigilance à la migration depuis VSTest

- **La syntaxe de filtrage diffère.** MSTest et NUnit conservent le format `--filter` habituel
  sous MTP ; **xUnit, non** — il faut passer à ses nouvelles options (`--filter-class`,
  `--filter-method`, `--filter-trait`, `--filter-query`…).
- **Certaines fonctions reposent sur des extensions MTP** à installer : la couverture
  (`--coverage`), le rapport `.trx`, les *crash/hang dumps*…
- **« Aucun test trouvé = échec ».** MTP considère par défaut qu'une exécution sans test est un
  échec. C'est sain, mais à anticiper dans les chaînes CI qui supposaient l'inverse.

> 🟢 **Et en VB.NET ?** MTP est un modèle de **build et d'exécution**, pas une affaire de
> langage : il fonctionne à l'identique pour VB.NET. Combiné aux **modèles de projet de test
> disponibles en VB** (l'une des familles de projets VB.NET), vous profitez de la nouvelle
> plateforme sans rien de spécifique à faire.

---

## Exécuter les tests

Trois voies, complémentaires :

- **En ligne de commande :** `dotnet test` (sur la solution ou un projet), idéal pour la CI. En
  .NET 10, il pilote indifféremment les projets VSTest et MTP.
- **Exécutable direct (MTP) :** lancer le binaire de test produit, ou `dotnet run` — pratique et
  rapide.
- **Explorateur de tests** de **Visual Studio 2026** : exécution, débogage au point d'arrêt
  (cf. [12.2](../12-exceptions-debogage/02-debogage.md)), filtrage et regroupement à la souris.

> 💡 **Mise en place d'un projet de test en VB.NET :** créez un projet de test dédié (par
> exemple `MonApp.Tests`) via la boîte de dialogue de Visual Studio ou `dotnet new`
> (`mstest` / `nunit` / `xunit`, avec le langage VB). Ce projet **référence** le projet à tester
> et le paquet du framework choisi. Gardez les tests **séparés** du code de production.

---

## Quel framework choisir ?

Aucun mauvais choix, mais en repère :

- **MSTest** — intégration Microsoft de bout en bout, support MTP de première main ; valeur sûre,
  notamment en environnement 100 % Microsoft.
- **NUnit** — très riche en contraintes d'assertion et en attributs ; mature et populaire
  (pensez au `[Is]` côté VB).
- **xUnit** — minimaliste et moderne ; *de facto* standard côté C#, mais sa culture et sa
  documentation sont très centrées C# (à garder en tête en VB.NET).

Le plus important n'est pas *lequel*, mais d'en **adopter un** et d'écrire des tests.

---

## À retenir

Le test unitaire est la brique de base de la qualité : rapide, isolé, auto-validant, structuré en
**Arrange-Act-Assert**. Les trois frameworks de référence — **xUnit, NUnit, MSTest** — sont tous
pleinement utilisables en VB.NET, avec une seule subtilité notable : le `[Is]` à échapper en
NUnit. Côté infrastructure, **Microsoft.Testing.Platform** modernise l'exécution des tests
(exécutables autonomes, performances, extensibilité) et bénéficie d'un **support natif dans le
SDK .NET 10** ; il fonctionne en VB.NET sans rien de particulier, à condition de connaître les
quelques différences de migration depuis VSTest.

➡️ Section suivante : **[13.2 — Mocking et TDD](02-mocking-tdd.md)**, pour isoler vraiment l'unité
testée et laisser les tests guider la conception.

⏭️ [*Mocking* (Moq, NSubstitute) et TDD](/13-tests-qualite/02-mocking-tdd.md)
