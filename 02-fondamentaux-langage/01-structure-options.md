🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.1 Structure d'un programme ; `Option Strict` / `Explicit` / `Infer` / `Compare`

> Avant d'écrire la moindre ligne de logique, deux choses méritent d'être maîtrisées : la façon dont
> un fichier VB.NET est organisé, et les quatre directives `Option` qui décident du niveau de rigueur
> du compilateur. C'est le point de départ de toute la formation — et le réglage de `Option Strict`
> y est, sans exagération, la décision la plus structurante.

---

## Anatomie d'un programme VB.NET

Un fichier source VB porte l'extension `.vb`. Voici sa structure type, du haut vers le bas :

```vb
Option Strict On
Option Explicit On
Option Infer On
Option Compare Binary

Imports System
Imports System.Collections.Generic

Namespace MonApplication

    Module Program

        Sub Main(args As String())
            Console.WriteLine("Bonjour, VB.NET sur .NET 10 !")
        End Sub

    End Module

End Namespace
```

On y reconnaît un ordre **imposé** :

1. les directives `Option` (si présentes) viennent **toujours en premier**, avant tout le reste ;
2. les instructions `Imports` (l'équivalent du `using` de C#) ;
3. puis les déclarations de types, regroupées le cas échéant dans un `Namespace`.

### Modules, classes et espaces de noms

VB.NET propose un type particulier, le **`Module`**, sans équivalent syntaxique direct en C# : ses
membres sont implicitement partagés (`Shared`) et accessibles sans qualification dans tout l'espace de
noms. C'est le réceptacle naturel du point d'entrée et des fonctions utilitaires. À côté, on retrouve
les `Class`, `Structure`, `Interface` et `Enum` habituels (détaillés au [module 3](../03-poo/README.md)).

Un `Namespace` organise logiquement le code ; en l'absence de déclaration explicite, les types sont
placés dans l'espace de noms racine du projet (propriété `RootNamespace`).

### Le point d'entrée

Pour une application Console, le point d'entrée est la procédure `Main`, placée dans un `Module` (ou
une classe). Plusieurs signatures sont acceptées :

```vb
Sub Main()                         ' sans arguments
Sub Main(args As String())         ' avec les arguments de ligne de commande
Function Main() As Integer         ' avec code de retour
Function Main(args As String()) As Integer
```

Pour un démarrage asynchrone, on relaie généralement vers une méthode `Async` depuis `Main` — sujet
traité au [module 4](../04-async/README.md). Pour une application Windows Forms, le démarrage peut être
géré par le *cadre applicatif* (formulaire de démarrage, voir [section 1.5](../01-introduction-vbnet/05-premier-projet.md))
ou par un `Sub Main` classique appelant `Application.Run(...)` — voir le
[module 5](../05-windows-forms/README.md).

> **Une différence honnête avec C# :** VB.NET ne dispose **pas** des *top-level statements* (instructions
> de niveau supérieur). Tout code exécutable doit vivre dans une méthode. Cette absence explique, entre
> autres, pourquoi les Minimal APIs sont peu pratiques en VB
> (voir [module 8.3](../08-services-web/03-limites-web-vbnet.md) et
> l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)).

### Procédures : `Sub`, `Function` et leurs paramètres

Tout le code exécutable vit donc dans des **procédures**. VB en distingue deux sortes : la **`Sub`**
effectue une action sans renvoyer de valeur ; la **`Function`** renvoie une valeur, fournie par
l'instruction **`Return`** :

```vb
Sub Afficher(message As String)
    Console.WriteLine(message)
End Sub

Function Carre(n As Integer) As Integer
    Return n * n
End Function
```

Les **paramètres** se déclarent avec quelques modificateurs à connaître d'emblée :

- **`ByVal`** (par défaut, donc implicite) : la procédure reçoit une **copie** de la valeur — ou, pour
  un type référence, une copie de la référence ([section 2.2](02-types-variables.md)).
- **`ByRef`** : la procédure reçoit la **variable elle-même** et peut la modifier chez l'appelant.
- **`Optional`** : le paramètre devient facultatif et **doit** porter une valeur par défaut.
- **`ParamArray`** : le dernier paramètre accepte un **nombre variable** d'arguments.

```vb
Sub Doubler(ByRef valeur As Integer)
    valeur *= 2
End Sub

Function Formater(texte As String, Optional majuscules As Boolean = False) As String
    Return If(majuscules, texte.ToUpper(), texte)
End Function

Function Somme(ParamArray valeurs() As Integer) As Integer
    Return valeurs.Sum()
End Function

' À l'appel :
Dim x = 10
Doubler(x)                                     ' x vaut 20
Dim s = Formater("abc", majuscules:=True)      ' "ABC" — argument nommé (name:=)
Dim total = Somme(1, 2, 3, 4)                  ' 10
```

L'**argument nommé** (`nom:=valeur`) clarifie les appels et permet de ne fournir que certains
paramètres optionnels. Plusieurs procédures peuvent enfin partager le même nom avec des signatures
différentes — la **surcharge**, détaillée avec les méthodes de classe en
[section 3.1](../03-poo/01-classes-objets.md).

> ⚠️ **Héritage VB6.** En VB6, le passage par défaut était `ByRef` ; en VB.NET, c'est **`ByVal`**. Du
> code VB6 migré qui comptait sur la modification implicite d'un argument change donc de comportement —
> un piège classique de migration (→ 11.2). En VB.NET moderne, on écrit rarement `ByVal` (il est
> implicite) et l'on réserve `ByRef` aux cas qui l'exigent vraiment.

### Particularités syntaxiques utiles dès maintenant

- **Pas de point-virgule** : une instruction se termine en fin de ligne.
- **Continuation de ligne** : depuis VB 2010 (VB 10), la continuation est le plus souvent *implicite*
  après un opérateur, une virgule, une parenthèse ouvrante, etc. Le caractère de soulignement `_`
  (continuation explicite, héritée) reste valide mais devient rarement nécessaire.

  ```vb
  ' Continuation implicite (recommandée)
  Dim total = valeurA +
              valeurB +
              valeurC

  ' Continuation explicite avec '_' (héritage, généralement superflu)
  Dim message = "Ligne 1 " & _
                "Ligne 2"
  ```

- **Commentaires** : l'apostrophe `'` (ou le mot-clé hérité `REM`) pour une ligne ; trois apostrophes
  `'''` pour un commentaire de **documentation XML**, exploité par l'IDE comme par les assistants IA.

  ```vb
  ' Commentaire ordinaire
  ''' <summary>Calcule la TVA d'un montant HT.</summary>
  ```

- **Insensibilité à la casse** : `MaVariable`, `mavariable` et `MAVARIABLE` désignent la même entité.
  L'éditeur de Visual Studio normalise automatiquement la casse sur la déclaration d'origine.

---

## Les directives `Option` : pourquoi elles existent

VB.NET hérite de Visual Basic 6 une tolérance historique (variables implicites, conversions
automatiques, liaison tardive). Pour concilier compatibilité ascendante et rigueur moderne, le langage
expose quatre interrupteurs. Bien réglés, ils transforment VB.NET en un langage fortement typé et sûr ;
mal réglés, ils laissent passer à l'exécution des erreurs qu'un compilateur strict aurait interceptées.

### `Option Explicit`

Contrôle l'obligation de **déclarer** les variables avant de les utiliser. Avec `Option Explicit On`
(la valeur par défaut), toute variable doit être déclarée ; avec `Off`, une variable non déclarée est
créée implicitement (en tant qu'`Object`) — un comportement dangereux, vestige de VB6. **Laissez-le
sur `On`.**

### `Option Strict` ⭐

C'est la directive décisive. Sa valeur par défaut dans un nouveau projet est **`Off`**, pour des raisons
de compatibilité — et c'est précisément ce qu'il faut changer. Avec **`Option Strict On`**, le compilateur :

- **interdit les conversions restrictives implicites** (avec risque de perte de données) : seules les
  conversions *élargissantes* restent automatiques ;
- **interdit la liaison tardive** (*late binding*) : tout accès à un membre doit être résolu à la
  compilation ;
- impose `Option Explicit On` (qui devient alors obligatoire).

**Élargissement vs restriction.** Une conversion qui ne perd pas d'information (par ex. `Integer` →
`Double`) est *élargissante* et reste implicite ; l'inverse est *restrictif* et exige une conversion
explicite :

```vb
' Option Strict On
Dim total As Integer = 10
Dim moyenne As Double = total   ' OK : élargissement (Integer → Double)

Dim x As Double = 3.9
Dim n As Integer = x            ' ERREUR de compilation avec Option Strict On
Dim n2 As Integer = CInt(x)     ' OK : conversion explicite (n2 = 4, arrondi)
```

**Liaison tardive.** Avec `Option Strict Off`, on peut appeler un membre sur un `Object` sans que le
compilateur le vérifie ; la résolution a lieu à l'exécution (et peut échouer là). Avec `On`, c'est une
erreur de compilation :

```vb
' Avec Option Strict Off : compile, résolu à l'exécution (liaison tardive)
Dim obj As Object = "Bonjour"
Console.WriteLine(obj.Length)

' Avec Option Strict On : il faut typer explicitement (liaison anticipée, vérifiée)
Dim texte As String = "Bonjour"
Console.WriteLine(texte.Length)
```

Activer `Option Strict On` est le geste le plus rentable du module : il déplace une large famille
d'erreurs de l'exécution vers la compilation. La formation l'adopte par défaut dans **tous** ses exemples.

### `Option Infer`

Active l'**inférence de type** des variables locales : le compilateur déduit le type à partir de la
valeur d'initialisation. Dans un nouveau projet, sa valeur est généralement **`On`**.

```vb
' Option Infer On
Dim compteur = 0                  ' inféré : Integer
Dim noms = New List(Of String)    ' inféré : List(Of String)
Dim requete = From x In noms Where x.Length > 3   ' inféré (types LINQ/anonymes)
```

Le point subtil tient à son **interaction avec `Option Strict`** :

- `Option Infer On` **+** `Option Strict On` : `Dim compteur = 0` est valide — l'inférence fournit un
  type réel (`Integer`), ce qui satisfait l'exigence de typage strict. C'est la combinaison idéale :
  sûreté **et** concision.
- `Option Infer Off` **+** `Option Strict On` : `Dim compteur = 0` provoque une **erreur** (« la clause
  `As` est obligatoire »). Il faut alors écrire `Dim compteur As Integer = 0`.
- `Option Infer Off` **+** `Option Strict Off` : `Dim compteur = 0` déclare un `Object` — à éviter.

Autrement dit, l'inférence n'affaiblit pas le typage : combinée à `Strict On`, elle vous fait gagner en
lisibilité sans rien concéder à la sécurité.

### `Option Compare`

Détermine la façon dont les **chaînes** sont comparées. Deux valeurs possibles :

- **`Binary`** (par défaut) : comparaison *ordinale*, fondée sur la valeur binaire des caractères, donc
  **sensible à la casse** et indépendante de la culture.
- **`Text`** : comparaison **insensible à la casse**, tenant compte de la culture.

```vb
' Option Compare Binary (défaut) — sensible à la casse
Dim a = ("Apple" = "apple")   ' False

' Option Compare Text — insensible à la casse
' (le même test renverrait True)
```

Ce réglage influence les opérateurs de comparaison (`=`, `<`, `>`…) sur les chaînes, le `Select Case`
sur des chaînes, les fonctions de chaînes héritées (`InStr`, `StrComp`…) **et l'opérateur `Like`**
(correspondance de motifs, voir [section 2.3](03-operateurs.md)). En pratique, on conserve **`Binary`**
pour un comportement prévisible et performant, et l'on exprime explicitement une intention de culture
quand elle est nécessaire — par exemple via
`String.Compare(..., StringComparison.OrdinalIgnoreCase)` (voir
[section 2.7](07-dates-nombres-culture.md)).

---

## Où définir ces options

Il existe deux niveaux de portée, le plus local l'emportant sur le plus global.

**Au niveau du projet (recommandé).** Dans un projet SDK (`.vbproj`), les directives s'appliquent
uniformément à tous les fichiers — impossible de les oublier au cas par cas :

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>MonApplication</RootNamespace>

    <OptionStrict>On</OptionStrict>
    <OptionExplicit>On</OptionExplicit>
    <OptionInfer>On</OptionInfer>
    <OptionCompare>Binary</OptionCompare>
  </PropertyGroup>

</Project>
```

Ce réglage est aussi accessible dans Visual Studio 2026 via les **propriétés du projet → Compiler**.
Les *imports* globaux du projet (l'équivalent VB des *global usings* de C#) se déclarent dans le même
fichier :

```xml
<ItemGroup>
  <Import Include="System.Net.Http" />
  <Import Include="MonApplication.Outils" />
</ItemGroup>
```

À noter : même sans la moindre déclaration, le SDK VB fournit d'office une **liste d'imports par
défaut** — `System`, `System.Collections`, `System.Collections.Generic`, `System.Linq`,
`System.Threading.Tasks`, `System.Diagnostics`, `System.Xml.Linq` et `Microsoft.VisualBasic`
(les modèles Windows Forms y ajoutent explicitement `System.Windows.Forms`, `System.Drawing` et
`System.Data` via ce même mécanisme `<Import>`). C'est pourquoi un fichier VB typique n'a
besoin que de peu d'instructions `Imports`, voire d'aucune — et pourquoi la ligne `Imports System`
générée par le modèle console est optionnelle (→ 1.5). La liste se consulte et s'édite dans
**Propriétés du projet → Références → Espaces de noms importés**.

**Au niveau du fichier.** Les directives placées en tête d'un fichier `.vb` (avant les
`Imports`) **surchargent** les réglages du projet pour ce seul fichier. C'est utile pour
isoler un cas particulier — par exemple un fichier d'interopérabilité qui doit autoriser
la liaison tardive.

## La configuration recommandée

| Directive | Défaut (nouveau projet) | Recommandé | Effet en bref |
|-----------|-------------------------|------------|---------------|
| `Option Explicit` | On | **On** | Toute variable doit être déclarée |
| `Option Strict` | **Off** | **On** ⭐ | Interdit conversions restrictives implicites et liaison tardive |
| `Option Infer` | On | **On** | Active l'inférence de type des variables locales |
| `Option Compare` | Binary | **Binary** | Comparaisons de chaînes ordinales, sensibles à la casse |

La règle tient en une phrase : **`Strict On`, `Explicit On`, `Infer On`, `Compare Binary`**, définis au
niveau du projet. C'est le socle sain sur lequel s'appuient tous les chapitres suivants.

## Cas particulier : quand désactiver `Option Strict`

Il existe une exception légitime et bien identifiée : la **liaison tardive** nécessaire à certains
scénarios d'**automation COM / Office** (piloter Excel, Word ou Outlook par *late binding*). On désactive
alors `Option Strict` localement, idéalement dans un fichier dédié plutôt que pour tout le projet. Ce
sujet est traité au [module 9.2](../09-interoperabilite/02-com-office.md). En dehors de ce type de cas,
laisser `Option Strict Off` est une dette technique qui se paie à l'exécution.

## Et l'IA dans tout ça ? 🤖

Deux pièges récurrents lorsqu'on génère ce code avec un assistant :

- les modèles, entraînés majoritairement sur du C#, produisent parfois du code qui **suppose
  `Option Strict Off`** (conversions implicites, accès tardifs). Demandez explicitement du code
  **compatible `Option Strict On`** ;
- la traduction depuis C# transforme `using` → `Imports` et `var` → `Dim` (avec `Option Infer On`) ;
  vérifiez que l'IA n'a pas laissé un mot-clé C# tel quel.

La méthode générale — toujours préciser « **Visual Basic .NET** » et la version .NET cible — est détaillée
au [module 17](../17-developpement-ia/README.md).

## En résumé

- Un programme VB.NET s'organise selon un ordre fixe : directives `Option`, puis `Imports`, puis types
  (dans des `Module`/`Class` au sein de `Namespace`).
- VB n'a pas de *top-level statements* : tout code exécutable vit dans une **procédure** — `Sub`
  (action) ou `Function` (valeur de retour), aux paramètres `ByVal` par défaut (`ByRef`, `Optional`,
  `ParamArray` au besoin).
- Les quatre directives `Option` règlent la rigueur du compilateur ; la configuration de référence est
  **`Strict On` + `Explicit On` + `Infer On` + `Compare Binary`**, posée au niveau du projet.
- `Option Strict On` est le réglage le plus important : il bascule de nombreuses erreurs de l'exécution
  vers la compilation. La seule exception courante est la liaison tardive en interop COM/Office.

---

⏭️ [Types de données et variables](/02-fondamentaux-langage/02-types-variables.md)
