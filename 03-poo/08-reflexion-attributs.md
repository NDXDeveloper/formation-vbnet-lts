🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.8 Réflexion et attributs

Cette dernière section du chapitre explore deux mécanismes complémentaires et profondément liés. La **réflexion** (`System.Reflection`) permet d'inspecter les types, leurs membres et leurs métadonnées **à l'exécution**, voire d'instancier et d'invoquer dynamiquement. Les **attributs** sont des métadonnées déclaratives que l'on attache au code — et que l'on relit, justement, par réflexion. Ensemble, ils sous-tendent une bonne partie de l'écosystème .NET : sérialiseurs, ORM, injection de dépendances, validation, frameworks de test.

---

## La réflexion (`System.Reflection`)

### Le type `Type` : trois façons de l'obtenir

Tout part de l'objet `Type`, qui décrit un type. VB.NET offre **trois** moyens de l'obtenir, qu'il ne faut pas confondre :

```vb
Dim t1 As Type = GetType(Produit)                 ' opérateur — à partir d'un nom de type (≈ typeof en C#)
Dim t2 As Type = unObjet.GetType()                ' méthode d'instance — à partir d'un objet
Dim t3 As Type = Type.GetType("MonApp.Produit")   ' méthode statique — à partir d'une chaîne (Nothing si introuvable)
```

> ⚠️ **Distinction VB à retenir.** `GetType(Produit)` est un **opérateur** qui prend un *nom de type* (l'équivalent du `typeof` de C#). `unObjet.GetType()` est une **méthode d'instance** héritée de `Object`. `Type.GetType("…")` est une **méthode statique** qui prend une *chaîne*. Trois outils, trois usages.

### Inspecter un type

Une fois le `Type` en main, on explore sa structure et ses métadonnées :

```vb
Dim t = GetType(Produit)
Console.WriteLine($"Type : {t.FullName}")
Console.WriteLine($"Classe : {t.IsClass}, abstraite : {t.IsAbstract}")

For Each prop In t.GetProperties()
    Console.WriteLine($"  {prop.Name} : {prop.PropertyType.Name}")
Next
```

`Type` expose `GetProperties()`, `GetMethods()`, `GetFields()`, `GetConstructors()` (ou `GetMembers()` pour tout), que l'on affine au besoin avec des `BindingFlags` (`Public`, `NonPublic`, `Instance`, `Static` — le nom de l'API reste « Static », même depuis VB), ainsi que des informations comme `BaseType`, `GetInterfaces()` ou `Namespace`.

### Lire et écrire des valeurs dynamiquement

Un `PropertyInfo` permet de lire ou d'écrire une propriété **sans connaître son nom à la compilation** :

```vb
Dim p As New Produit With {.Nom = "Clavier"}
Dim propNom = GetType(Produit).GetProperty("Nom")

Dim valeur = propNom.GetValue(p)    ' lecture → « Clavier »
propNom.SetValue(p, "Souris")       ' écriture
Console.WriteLine(p.Nom)            ' « Souris »
```

### Invoquer des méthodes et créer des instances

On peut appeler une méthode via `MethodInfo.Invoke` (les arguments sont passés dans un tableau `Object()`), et instancier dynamiquement via `Activator` :

```vb
' Invoquer Produit.Recalculer(taux) dynamiquement
Dim methode = GetType(Produit).GetMethod("Recalculer")
methode.Invoke(p, New Object() {0.2D})

' Créer une instance à l'exécution
Dim instance = CType(Activator.CreateInstance(GetType(Produit)), Produit)              ' constructeur sans paramètre
Dim instance2 = CType(Activator.CreateInstance(GetType(Produit), "Écran", 199D), Produit)  ' avec arguments
```

### Assemblies

La réflexion s'étend aux **assemblies**, ce qui ouvre la porte aux architectures à plugins (charger un assembly, y découvrir les types implémentant une interface — § 3.4) :

```vb
Dim asm = System.Reflection.Assembly.GetExecutingAssembly()
Dim plugins = asm.GetTypes().
    Where(Function(ty) GetType(IPlugin).IsAssignableFrom(ty) AndAlso Not ty.IsAbstract)
```

### À quoi sert la réflexion

La réflexion est la mécanique invisible derrière de nombreux outils : **sérialiseurs** (`System.Text.Json`, § 7.5), **mappage objet-relationnel** (EF Core, module 7), **conteneurs d'injection de dépendances**, **frameworks de validation** (DataAnnotations, § 5.7) et **frameworks de test** (découverte des méthodes de test). Dès qu'un composant doit traiter des types qu'il ne connaît pas à l'avance, il s'appuie sur la réflexion.

### ⚠️ Le coût de la réflexion

Cette puissance a un prix, qu'il faut connaître :

- **Performance** — la réflexion est nettement plus lente qu'un appel direct (recherche de membres, invocation indirecte). À proscrire sur les chemins critiques ; lorsqu'on l'emploie de façon répétée, **mettre en cache** les objets `Type`/`PropertyInfo`/`MethodInfo` (lien avec le module 14).
- **Perte de sûreté à la compilation** — les noms de membres sont des chaînes : pas d'IntelliSense, et les erreurs n'apparaissent qu'à l'exécution. Un *refactoring* peut casser silencieusement un appel par réflexion.
- **Trimming et AOT** — un recours intensif à la réflexion complique l'élagage (*trimming*, § 15.6) et la compilation Native AOT, car les membres « invisibles » au compilateur peuvent être supprimés.

> 🔗 L'alternative moderne pour éviter ce coût — les **générateurs de source** (métaprogrammation à la compilation, plus rapide et compatible AOT) — relève du **monde C#** : ils s'écrivent (et, le plus souvent, se consomment) en C# (**[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**, point B.6). Côté VB.NET, la parade pragmatique reste la **mise en cache** des objets de réflexion réutilisés.

---

## Les attributs

### Qu'est-ce qu'un attribut ?

Un attribut attache une **métadonnée déclarative** à un élément de code (type, membre, paramètre, assembly), exploitable à l'exécution par réflexion. En VB.NET, les attributs s'écrivent **entre chevrons `< >`** — et non entre crochets `[ ]` comme en C#.

```vb
<Serializable>
Public Class Configuration
    <Obsolete("Utilisez NouveauChamp à la place.")>
    Public AncienChamp As String
End Class
```

### Attributs intégrés utiles

Le framework en fournit quantité, parmi lesquels :

- **`<Obsolete(...)>`** — marque un membre déprécié (avertissement à la compilation) ;
- **`<DebuggerDisplay(...)>`** — personnalise l'affichage dans le débogueur (module 12) ;
- les **DataAnnotations** (`<Required>`, `<StringLength(...)>`, `<Range(...)>`, `<EmailAddress>`) — pour la validation (§ 5.7) ;
- **`<DllImport(...)>`** — pour l'interop P/Invoke (§ 9.1) ;
- les attributs JSON (`<JsonPropertyName(...)>`, `<JsonIgnore>`) — pour la sérialisation (§ 7.5).

```vb
Imports System.ComponentModel.DataAnnotations

Public Class Inscription
    <Required>
    <StringLength(50, MinimumLength:=2)>
    Public Property Nom As String

    <Range(18, 120)>
    Public Property Age As Integer
End Class
```

### Syntaxe : arguments, regroupement, niveau assembly

- **Arguments positionnels et nommés** : les nommés utilisent `:=`, comme `<StringLength(50, MinimumLength:=2)>`.
- **Attributs multiples** : soit empilés sur leurs propres lignes, soit séparés par des virgules dans un même bloc : `<Required, StringLength(100)>`.
- **Niveau assembly** : on préfixe par le modificateur `Assembly:` (souvent dans `AssemblyInfo.vb`, ou généré depuis le `.vbproj`) :
  ```vb
  <Assembly: AssemblyTitle("Mon Application")>
  ```

---

## Créer des attributs personnalisés

Vous pouvez définir vos propres attributs pour porter une métadonnée métier — et c'est exactement ainsi que fonctionnent, en interne, les sérialiseurs et les ORM.

### Définir l'attribut

Trois règles : hériter de `System.Attribute`, suffixer le nom par `Attribute` (le suffixe est omis à l'application), et délimiter son usage avec `<AttributeUsage(...)>`. Les paramètres du constructeur deviennent des arguments **positionnels**, les propriétés des arguments **nommés**.

```vb
<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False)>
Public Class ColonneAttribute
    Inherits Attribute

    Public ReadOnly Property Nom As String
    Public Property Obligatoire As Boolean = False

    Public Sub New(nom As String)
        Me.Nom = nom
    End Sub
End Class
```

### Appliquer l'attribut

On l'utilise **sans** le suffixe `Attribute` :

```vb
Public Class Produit
    <Colonne("produit_id")>
    Public Property Id As Integer

    <Colonne("libelle", Obligatoire:=True)>
    Public Property Nom As String

    Public Property Interne As String   ' aucun attribut → ignorée par le mappage
End Class
```

### Lire l'attribut par réflexion

C'est l'aboutissement : parcourir les membres et réagir à leurs attributs. L'extension générique `GetCustomAttribute(Of T)()` renvoie l'attribut (ou `Nothing`) :

```vb
Imports System.Reflection

Public Module Mapping
    Public Sub DecrireColonnes(type As Type)
        For Each prop In type.GetProperties()
            Dim attr = prop.GetCustomAttribute(Of ColonneAttribute)()
            If attr IsNot Nothing Then
                Dim mention = If(attr.Obligatoire, " (obligatoire)", "")
                Console.WriteLine($"{prop.Name} → colonne « {attr.Nom} »{mention}")
            End If
        Next
    End Sub
End Module

' Utilisation :
Mapping.DecrireColonnes(GetType(Produit))
' Id   → colonne « produit_id »
' Nom  → colonne « libelle » (obligatoire)
```

Ce petit mappeur reproduit, en miniature, le principe d'un ORM ou d'un sérialiseur : **des attributs déclaratifs**, **relus par réflexion**, qui pilotent un traitement générique.

---

## Spécificités VB.NET à retenir

- `GetType(T)` est un **opérateur** (≈ `typeof`) ; à ne pas confondre avec la **méthode** `obj.GetType()` ni avec `Type.GetType("…")` (par chaîne).
- Les attributs s'écrivent entre **chevrons `< >`** (et non `[ ]`) ; arguments nommés en `:=` ; niveau assembly avec `Assembly:`.
- Un attribut personnalisé `Inherits Attribute`, porte le suffixe `Attribute`, et précise sa cible via `<AttributeUsage(...)>`.
- La réflexion est puissante mais **coûteuse** : mettre en cache ; l'alternative « générateurs de source » est du ressort de C# (🔗).

> 🤖 **Astuce IA.** Deux réflexes C# que les assistants transposent mal : écrire les attributs entre **crochets** `[Serializable]` au lieu de chevrons `<Serializable>`, et employer `typeof(T)` au lieu de l'opérateur `GetType(T)`. Demandez explicitement du « **Visual Basic .NET** » et vérifiez ces deux points. Voir l'**[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** et le **[module 17](../17-developpement-ia/README.md)**.

---

## Conclusion du chapitre

Ce chapitre a parcouru l'intégralité du modèle objet de VB.NET : des **classes et objets** (§ 3.1) à leur pendant en type valeur, les **structures et tuples** (§ 3.2) ; de l'**héritage et du polymorphisme** (§ 3.3) à l'abstraction par **interfaces** (§ 3.4) ; de l'**organisation du code** en modules et espaces de noms (§ 3.5) à l'idiome événementiel élégant de VB (§ 3.6 ⭐) ; enfin de l'**immuabilité** à la frontière des records (§ 3.7 🔗) jusqu'à la **réflexion et aux attributs** (§ 3.8).

Deux fils rouges l'ont traversé, fidèles à l'esprit de la formation : **VB.NET dispose d'un modèle objet complet et stable**, hérité d'avant le gel du langage et pleinement opérationnel sur .NET 10 ; et **là où VB ne déclare pas** (records, *readonly struct*, méthodes d'interface par défaut, générateurs de source), il **consomme** ce que C# produit, conformément à la stratégie hybride.

Fort de ces fondations objet, le chapitre suivant aborde une dimension transversale et essentielle des applications modernes : la **programmation asynchrone et parallèle**.

⏭️ [Programmation asynchrone et parallèle](/04-async/README.md)
