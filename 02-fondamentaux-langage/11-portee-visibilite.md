🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.11 Portée, visibilité et modificateurs d'accès

> Deux notions distinctes mais liées : la **portée** (où un nom est *visible* dans le code) et la
> **visibilité d'accès** (quelles parties du code ont le *droit* d'utiliser un membre). Sur les
> modificateurs, un point de repère essentiel : **`Friend` est l'équivalent VB de l'`internal` de C#**.

---

## La portée : où un nom est-il accessible ?

La portée d'une déclaration dépend de l'endroit où elle se trouve :

- **Portée de bloc** : déclaration à l'intérieur d'un bloc (`If`, `For`, `While`, `Using`…) — visible
  uniquement dans ce bloc.
- **Portée de procédure (locale)** : déclaration dans un `Sub`/`Function` — visible dans toute la
  procédure.
- **Portée de type (membre)** : déclaration au niveau d'une classe, structure ou module — visible dans
  tout le type (et au-delà selon le modificateur d'accès).
- **Portée d'espace de noms** : les types déclarés dans un `Namespace`.

```vb
Sub Exemple()
    Dim total As Integer = 0           ' portée : toute la procédure
    For i As Integer = 1 To 3          ' 'i' : portée limitée à la boucle
        Dim carre As Integer = i * i   ' 'carre' : portée limitée au bloc For
        total += carre
    Next
    ' Console.WriteLine(carre)   ' ERREUR : 'carre' est hors de portée ici
    Console.WriteLine(total)           ' OK
End Sub
```

> **Subtilité VB.** Une variable de bloc a une **portée** limitée à son bloc, mais sa **durée de vie**
> s'étend à toute la procédure. Concrètement, on ne peut pas y faire référence hors du bloc, mais son
> stockage n'est pas détruit en sortie de bloc.

---

## Durée de vie (rappel)

La portée décrit la *visibilité* ; la **durée de vie** décrit *quand* la variable existe :

- variables **locales** : durant l'exécution de la procédure ;
- variables locales **`Static`** : conservées entre les appels ([section 2.2](02-types-variables.md)) ;
- membres **d'instance** : aussi longtemps que l'objet ;
- membres **`Shared`** : pendant toute la durée de vie de l'application.

---

## Les modificateurs d'accès

VB.NET propose six niveaux d'accès. Le tableau ci-dessous donne, pour chacun, sa portée et son
**équivalent C#** — utile pour lire la documentation et le code généré, majoritairement en C# :

| VB | C# | Accessible depuis… |
|----|----|--------------------|
| `Public` | `public` | partout |
| `Private` | `private` | le type déclarant uniquement |
| `Protected` | `protected` | le type **et ses types dérivés** |
| `Friend` | `internal` | le **même assembly** (projet / DLL) |
| `Protected Friend` | `protected internal` | les dérivés **OU** le même assembly |
| `Private Protected` | `private protected` | les dérivés **ET** le même assembly |

Le point à mémoriser est **`Friend` ↔ `internal`** : c'est la confusion la plus fréquente pour qui vient
de C#. `Private Protected` (intersection) a été ajouté avec VB 15.5.

```vb
Public Class CompteBancaire
    Private _solde As Decimal               ' interne au type
    Protected Friend TauxInterne As Decimal ' dérivés OU même assembly

    Public ReadOnly Property Solde As Decimal   ' surface publique (le contrat)
        Get
            Return _solde
        End Get
    End Property

    Public Sub Crediter(montant As Decimal)
        _solde += montant
    End Sub

    Protected Overridable Sub Journaliser(message As String)
        ' accessible aux classes dérivées
    End Sub
End Class
```

---

## Niveaux d'accès par défaut

En l'absence de modificateur, le défaut **dépend de ce que l'on déclare** — un comportement source de
surprises :

- variable **locale** (`Dim` dans une procédure) : simplement *limitée en portée*, sans notion d'accès ;
- **champ** déclaré avec `Dim` au niveau d'une **classe** : **`Private`** par défaut ;
- **`Sub` / `Function` / `Property`** sans modificateur : **`Public`** par défaut ;
- **type** (classe, structure, interface…) déclaré directement dans un **espace de noms** : **`Friend`**
  par défaut.

> ⚠️ **Asymétrie classe / structure.** Dans une **`Structure`**, un champ déclaré avec `Dim` est
> **`Public`** par défaut (alors qu'il est `Private` dans une classe). Par ailleurs, les membres d'une
> **interface** sont **toujours `Public`** (aucun modificateur n'est admis).

Pour éviter toute ambiguïté, la bonne pratique est simple : **écrivez toujours le modificateur
explicitement**, plutôt que de vous fier à ces défauts.

---

## Bonnes pratiques : l'encapsulation

- **Partez du plus restrictif.** Déclarez en `Private` par défaut, et n'élargissez (`Friend`, `Public`)
  que lorsque c'est nécessaire. La surface publique d'un type est son **contrat** : gardez-la minimale et
  stable.
- **Champs privés, accès par propriétés.** Exposez les données via des propriétés `Public`/`Friend`
  plutôt que des champs publics ([section 3.1](../03-poo/01-classes-objets.md)).
- **`Friend` pour les API internes** au projet : utilitaires non destinés aux consommateurs externes.
- **`InternalsVisibleTo`** rend les membres `Friend` visibles à un assembly désigné — le plus souvent le
  projet de tests (en lien avec le [module 13](../13-tests-qualite/README.md)) :

```vb
' Dans un fichier du projet :
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("MaBibliotheque.Tests")>
```

---

## Et l'IA dans tout ça ? 🤖

Les modificateurs d'accès concentrent quelques traductions C# → VB faciles à manquer :

- **`internal`** (C#) → **`Friend`** — la correspondance la plus oubliée ; **`protected internal`** →
  **`Protected Friend`** ; **`private protected`** → **`Private Protected`**.
- **Casse des mots-clés** : `public` / `private` / `protected` (minuscules en C#) → **`Public` /
  `Private` / `Protected`** en VB.
- **Défauts différents** : un assistant peut supposer les défauts de C# (membres privés par défaut)
  alors qu'en VB un `Sub`/`Function` sans modificateur est **`Public`**. D'où la consigne d'**être
  explicite**.

La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- **Portée** : bloc, procédure, type, espace de noms. En VB, une variable de bloc a une portée limitée
  mais une **durée de vie** de procédure.
- **Six niveaux d'accès** : `Public`, `Private`, `Protected`, **`Friend` (= `internal`)**,
  `Protected Friend`, `Private Protected`.
- **Défauts variables** selon l'élément (champ `Dim` → `Private` en classe ; `Sub`/`Function` →
  `Public` ; type de namespace → `Friend`), avec l'asymétrie de la `Structure` → **soyez explicite**.
- **Encapsulation** : commencez au plus restrictif ; champs privés exposés par propriétés ;
  `InternalsVisibleTo` pour les tests.

---

⏭️ [L'espace My — un raccourci propre à VB.NET](/02-fondamentaux-langage/12-espace-my.md)
