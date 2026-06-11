🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 3.5 Modules, espaces de noms et classes partielles

Après le vocabulaire des types (classes, structures, héritage, interfaces), cette section change d'échelle pour traiter l'**organisation du code** : le **module** (un conteneur propre à VB.NET), l'**espace de noms** (pour structurer et désambiguïser), et la **classe partielle** (pour répartir un type sur plusieurs fichiers).

---

## Les modules (`Module`)

### Qu'est-ce qu'un module ?

Un module est un conteneur de membres **tous implicitement `Shared`** (§ 3.1). Il s'apparente à une classe statique, mais avec une particularité qui n'a pas d'équivalent en C# : ses membres sont accessibles **sans qualification**, directement par leur nom.

```vb
Public Module Utilitaires
    Public Const TauxTVA As Decimal = 0.2D

    Public Function AppliquerTVA(montantHT As Decimal) As Decimal
        Return montantHT * (1 + TauxTVA)
    End Function
End Module
```

Un module ne peut pas être instancié, ni hériter, ni être hérité, ni implémenter d'interface : c'est un simple regroupement de fonctions, constantes et autres membres partagés.

### Accès non qualifié

Depuis n'importe quel point du même projet (et du même espace de noms), les membres d'un module s'appellent **sans préfixe** :

```vb
Dim ttc = AppliquerTVA(100D)                ' accès direct
Dim ttc2 = Utilitaires.AppliquerTVA(100D)   ' qualification possible mais facultative
```

> ⚠️ **Risque d'ambiguïté.** Si deux modules exposent un membre de même nom, l'accès non qualifié devient ambigu : le compilateur exige alors de **préfixer** par le nom du module. C'est le revers de la commodité — un module aux membres trop génériques peut « polluer » l'espace de noms.

### Un usage clé : les méthodes d'extension

En VB.NET, les **méthodes d'extension se déclarent obligatoirement dans un module**. On les marque de l'attribut `<Extension()>` (de `System.Runtime.CompilerServices`) ; le premier paramètre désigne le type étendu :

```vb
Imports System.Runtime.CompilerServices

Public Module ExtensionsChaine
    <Extension()>
    Public Function EstVideOuBlanc(valeur As String) As Boolean
        Return String.IsNullOrWhiteSpace(valeur)
    End Function
End Module

' Utilisation, une fois l'espace de noms du module importé :
Dim vide = "   ".EstVideOuBlanc()   ' True
```

C'est l'une des rares situations où le module est **incontournable** : là où C# utilise une classe statique, VB impose un `Module`.

### Point d'entrée et utilitaires

Le point d'entrée d'une application console, `Sub Main`, est traditionnellement placé dans un module. Plus largement, le module est l'endroit idiomatique pour les **fonctions utilitaires sans état**, les **constantes globales** et les **méthodes d'extension**.

### Module ou classe `Shared` ?

En VB.NET, pour du code purement statique, **préférez le `Module`** à une classe `NotInheritable` aux membres tous `Shared` : c'est plus concis, plus idiomatique, et c'est la seule option pour les méthodes d'extension. Réservez la classe (même entièrement `Shared`) aux cas où vous avez besoin de la qualification systématique ou d'implémenter une interface — ce qu'un module ne permet pas.

---

## Les espaces de noms (Namespaces)

### Rôle

Un espace de noms regroupe logiquement des types et **évite les collisions de noms** : deux classes `Client` peuvent coexister si elles vivent dans `Ventes.Client` et `Support.Client`. C'est l'organisation à grande échelle d'une base de code.

### Déclarer un espace de noms

On utilise le bloc `Namespace … End Namespace`. Les espaces peuvent être **imbriqués**, soit par blocs, soit par notation pointée — les deux formes sont équivalentes :

```vb
Namespace Facturation
    Public Class Facture
    End Class

    Namespace Calculs              ' imbriqué → Facturation.Calculs
        Public Module Taxes
        End Module
    End Namespace
End Namespace

' Forme pointée, strictement équivalente :
Namespace Facturation.Calculs
    ' ...
End Namespace
```

### ⚠️ L'espace de noms racine (*Root Namespace*) — spécificité VB.NET

Voici un piège propre à VB.NET, surtout déroutant pour qui vient de C#. Tout projet VB définit un **espace de noms racine** (propriété *Root Namespace* du projet, par défaut le nom de l'assembly). **Chaque espace de noms déclaré dans le code est imbriqué sous cette racine.**

```vb
' Projet dont le Root Namespace vaut « Contoso.Ventes ».
Namespace Donnees                  ' espace réel : Contoso.Ventes.Donnees
    Public Class DepotFactures
    End Class
End Namespace
```

Le type complet est ici `Contoso.Ventes.Donnees.DepotFactures`, et non `Donnees.DepotFactures`. En C#, à l'inverse, le `namespace` écrit dans le fichier **est** l'espace complet (la racine n'est qu'un défaut de gabarit). Cette différence est une source classique d'erreurs de référencement, en particulier dans une **solution mixte VB/C#** (🔗, [module 10](../10-hybride-vbnet-csharp/README.md)).

> 💡 Pour faire correspondre VB au comportement de C# (espaces complets écrits en clair), il suffit de **vider** la propriété *Root Namespace* du projet et de déclarer les espaces complets dans le code.

### Importer : `Imports`, alias et `Global`

L'instruction `Imports` rend les types d'un espace de noms accessibles sans qualification (l'équivalent du `using` de C#). Elle accepte aussi un **alias** :

```vb
Imports System.Text
Imports System.Collections.Generic
Imports IO = System.IO            ' alias : « IO.Path.Combine(...) »
```

Le mot-clé **`Global`** permet de partir de la racine absolue de l'arborescence des espaces de noms, utile pour lever une ambiguïté (notamment avec le *Root Namespace* évoqué plus haut) :

```vb
Dim chemin = Global.System.IO.Path.Combine("dossier", "fichier.txt")
```

Enfin, VB autorise des **imports au niveau projet** (réglages du projet ou `.vbproj`), appliqués automatiquement à tous les fichiers — pratique pour les espaces de noms omniprésents, au prix d'une légère perte de lisibilité fichier par fichier.

---

## Les classes partielles (`Partial`)

### Principe

Le mot-clé `Partial` autorise la répartition d'un même type — classe, structure, interface ou module — sur **plusieurs fichiers**. À la compilation, toutes les parties sont fusionnées en un seul type. La règle VB : **au moins une** des déclarations doit porter `Partial` (les autres peuvent l'omettre ; par bonne pratique, on le met partout).

### L'usage canonique : séparer le code généré

C'est de loin l'emploi le plus courant — et il touche directement le scénario phare de VB.NET, **Windows Forms** (module 5). Le Concepteur (*Designer*) génère un fichier `Form1.Designer.vb` qui déclare la moitié « machine » du formulaire, tandis que votre code occupe `Form1.vb`. Les deux ne sont qu'une seule et même classe `Form1` :

```vb
' Form1.Designer.vb — généré par le Concepteur (ne pas éditer à la main)
Partial Class Form1
    Friend WithEvents btnValider As Button
    ' déclaration et initialisation des contrôles...
End Class

' Form1.vb — votre code
Public Class Form1
    Private Sub btnValider_Click(sender As Object, e As EventArgs) _
            Handles btnValider.Click
        ' votre logique métier
    End Sub
End Class
```

Cet exemple illustre aussi la souplesse de VB : la partie générée porte `Partial`, la vôtre peut l'omettre. Le `Handles btnValider.Click` fonctionne parce que `btnValider` est déclaré dans l'autre partie — les deux fichiers forment bien un seul type. Notez le **`WithEvents`** dans la déclaration du contrôle : la clause `Handles` l'exige (erreur BC30506 sinon) — c'est précisément pour cela que le Concepteur génère tous les contrôles en `Friend WithEvents` (le duo `WithEvents`/`Handles` est l'objet de la section suivante, § 3.6). Le même mécanisme sépare le code échafaudé par EF Core (module 7) du code que vous ajoutez.

### Organiser une grande classe

Au-delà du code généré, `Partial` peut répartir une classe volumineuse **par préoccupation** (par exemple : un fichier pour la validation, un autre pour la persistance). À utiliser avec parcimonie : une classe trop grande pour tenir dans un fichier est souvent une classe qui en fait trop, et qu'il vaudrait mieux décomposer.

### Méthodes partielles

VB prend également en charge les **méthodes partielles** : une signature déclarée dans une partie, dont l'implémentation, dans une autre partie, est **facultative**. Si personne ne l'implémente, le compilateur **supprime** la déclaration et tous ses appels — un mécanisme léger conçu pour les générateurs de code (points d'extension optionnels). Ces méthodes sont implicitement `Private` et doivent être des `Sub` :

```vb
Partial Class Entite
    Partial Private Sub OnModifie()      ' déclaration sans logique
    End Sub

    Public Sub MettreAJour()
        OnModifie()                      ' appel supprimé si non implémenté
    End Sub
End Class

' Dans une autre partie, implémentation optionnelle :
Partial Class Entite
    Private Sub OnModifie()
        ' logique personnalisée
    End Sub
End Class
```

---

## Spécificités VB.NET à retenir

- Le **`Module`** est un conteneur statique à **accès non qualifié** (sans équivalent en C#) ; il est **obligatoire** pour les méthodes d'extension (`<Extension()>`).
- Les espaces de noms déclarés sont **imbriqués sous le *Root Namespace*** du projet — un piège majeur face à C#.
- `Imports` (+ alias) joue le rôle du `using` ; `Global` part de la racine absolue.
- `Partial` répartit un type sur plusieurs fichiers ; en VB, **au moins une** déclaration porte le mot-clé. Usage roi : isoler le code généré (WinForms, EF Core).

> 🤖 **Astuce IA.** Trois écueils fréquents quand un assistant génère du « VB » à partir de réflexes C# : proposer une `static class` au lieu d'un `Module` (et oublier que l'accès non qualifié n'existe pas en C#) ; écrire des espaces de noms **complets** sans tenir compte du *Root Namespace* (d'où des types introuvables) ; et placer des méthodes d'extension dans une classe plutôt que dans un module avec `<Extension()>`. Demandez explicitement du « **Visual Basic .NET** » et vérifiez ces trois points. Voir l'**[Annexe A](../annexes/correspondance-vbnet-csharp/README.md)** et le **[module 17](../17-developpement-ia/README.md)**.

---

Le tour de la structuration des types et du code est complet. La section suivante aborde un domaine où VB.NET possède un **idiome particulièrement élégant et reconnu** — les **événements et délégués** —, au cœur des applications de bureau pilotées par l'interaction.

⏭️ [Événements et délégués — un point fort idiomatique de VB.NET](/03-poo/06-evenements-delegues.md)
