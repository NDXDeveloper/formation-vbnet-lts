🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.6 Chaînes et manipulation de texte

> `String` est un type référence **immuable** : chaque « modification » crée en réalité une nouvelle
> chaîne. Pour assembler du texte en quantité, on utilise **`StringBuilder`**. Et pour composer
> lisiblement, l'**interpolation `$"…"`**. Une particularité de VB est à intégrer d'emblée : il n'y a
> **pas de séquences d'échappement** avec antislash (`\n` n'existe pas) — un guillemet s'obtient en le
> **doublant**, et les caractères spéciaux passent par des constantes.

---

## `String` : un type référence immuable

`String` (alias de `System.String`, voir [section 2.2](02-types-variables.md)) est un type **référence**,
mais **immuable** : aucune méthode ne modifie la chaîne en place ; chacune renvoie une **nouvelle**
chaîne. La chaîne d'origine reste intacte.

```vb
Dim mot = "bonjour"
mot.ToUpper()              ' renvoie "BONJOUR" mais ne modifie PAS 'mot'
Console.WriteLine(mot)     ' "bonjour"
mot = mot.ToUpper()        ' il faut réaffecter
Console.WriteLine(mot)     ' "BONJOUR"
```

Conséquence directe : concaténer de façon répétée (par exemple dans une boucle) crée quantité de chaînes
intermédiaires — d'où l'intérêt de `StringBuilder`, plus bas.

### Littéraux : pas d'antislash, guillemets doublés

VB n'utilise **pas** l'antislash comme caractère d'échappement. Pour insérer un **guillemet** dans un
littéral, on le **double** :

```vb
Dim citation = "Il a dit ""bonjour""."   ' Il a dit "bonjour".
```

Il n'existe **pas** de `\n`, `\t`, etc. Les caractères spéciaux passent par des constantes (ou
`Environment.NewLine` pour le saut de ligne portable) :

```vb
Dim deuxLignes = "Première ligne" & Environment.NewLine & "Deuxième ligne"
Dim tabule = "Nom" & vbTab & "Âge"
```

Les constantes héritées `vbCrLf`, `vbLf`, `vbCr`, `vbTab`, `vbNewLine` restent disponibles ;
`ControlChars.*` en est l'équivalent. VB n'a ni chaîne *verbatim* (`@"…"` de C#) ni *raw string*
(`"""…"""`) — il n'en a d'ailleurs pas le même besoin, faute d'échappement antislash. Depuis VB 14
(2015), un littéral `"…"` peut en revanche **s'étendre sur plusieurs lignes**, les retours à la ligne
du source étant conservés :

```vb
Dim bloc = "Première ligne
Deuxième ligne"
```

Attention toutefois : l'**indentation du code entre alors dans la chaîne** (aucun mécanisme de
dé-indentation, contrairement aux *raw strings* de C#). Pour un texte multi-ligne dont la mise en
forme compte, la concaténation avec `Environment.NewLine` reste souvent plus prévisible.

### Chaîne vide ou nulle

`String.Empty` désigne la chaîne vide (équivalente à `""`). Pour tester l'absence de contenu, préférez
les méthodes partagées plutôt qu'une comparaison manuelle :

```vb
If String.IsNullOrEmpty(saisie) Then ...        ' Nothing OU ""
If String.IsNullOrWhiteSpace(saisie) Then ...   ' Nothing, "" ou uniquement des espaces
```

---

## Opérations de base

- **Longueur** : `s.Length`.
- **Accès par indice** : `s(0)` renvoie un `Char` (VB utilise des **parenthèses**, pas des crochets) ;
  parcourir une chaîne avec `For Each c As Char In s` est possible.
- **Concaténation** : l'opérateur `&` ([section 2.3](03-operateurs.md)), ou `String.Concat`. Pour
  assembler une **séquence** avec un séparateur, `String.Join` :

```vb
Dim noms = {"Ada", "Alan", "Grace"}
Dim ligne = String.Join(", ", noms)   ' "Ada, Alan, Grace"
```

---

## Méthodes courantes

Les méthodes ci-dessous proviennent de la bibliothèque .NET et renvoient toutes une **nouvelle** chaîne
(ou une valeur) :

| Méthode | Rôle |
|---------|------|
| `ToUpper` / `ToLower` (et `…Invariant`) | Casse |
| `Trim` / `TrimStart` / `TrimEnd` | Suppression des espaces aux extrémités |
| `Contains`, `StartsWith`, `EndsWith` | Tests de présence (renvoient `Boolean`) |
| `IndexOf` / `LastIndexOf` | Position d'une sous-chaîne (**-1** si absente) |
| `Substring(début[, longueur])` | Extraction (**base 0**) |
| `Replace(ancien, nouveau)` | Remplacement |
| `Split(...)` | Découpage en tableau (`String()`) |
| `PadLeft` / `PadRight` | Alignement par remplissage |
| `Insert` / `Remove` | Insertion / suppression par position |

```vb
Dim chemin = "  rapport-2026.pdf  "
Dim propre = chemin.Trim()                  ' "rapport-2026.pdf"
Dim extension = propre.Substring(propre.LastIndexOf("."c) + 1)  ' "pdf"
Dim morceaux = "a;b;c".Split(";"c)          ' {"a", "b", "c"}
```

> **Fonctions héritées de VB6.** VB conserve `Left`, `Right`, `Mid`, `InStr`, `Len`, `UCase`/`LCase`…
> (espace `Microsoft.VisualBasic`). Elles fonctionnent, mais sont **en base 1** et `InStr` renvoie **0**
> (et non -1) en cas d'absence — un décalage propice aux erreurs si on les mélange avec les méthodes
> .NET (base 0, -1). Pour la cohérence avec le reste de l'écosystème, **préférez les méthodes .NET**
> (`Substring`, `IndexOf`, `Length`…).

---

## `StringBuilder`

Pour construire du texte par ajouts successifs — boucles, gros volumes — `System.Text.StringBuilder`
fournit un **tampon mutable** qui évite la création de chaînes intermédiaires :

```vb
Dim sb As New System.Text.StringBuilder()
For i As Integer = 1 To 1000
    sb.Append("Ligne ").Append(i).AppendLine()
Next
Dim resultat = sb.ToString()
```

Principales méthodes : `Append`, `AppendLine`, `AppendFormat`, `Insert`, `Remove`, `Replace`, `Clear`,
puis `ToString()` pour récupérer le résultat (les appels se chaînent, comme ci-dessus).

**Quand l'utiliser ?** Pour quelques concaténations, l'opérateur `&` reste parfaitement adapté. Dès qu'il
s'agit d'une boucle ou d'un assemblage volumineux, passez à `StringBuilder`. Les détails de performance
relèvent du [module 14](../14-performance/README.md).

---

## Interpolation `$"…"`

L'interpolation (disponible depuis VB 2015) insère des expressions directement dans un littéral préfixé
par `$` :

```vb
Dim nom = "Ada"
Dim age = 36
Dim message = $"Bonjour {nom}, vous avez {age} ans."
```

On y applique des **spécificateurs de format** (après `:`) et un **alignement** (après `,`), et l'on
double les accolades `{{` / `}}` pour en obtenir une littérale :

```vb
Dim prix = 19.9D
Dim total = $"Prix : {prix:C2} — réf. {{A-100}}"   ' "Prix : 19,90 € — réf. {A-100}"
Dim ligne = $"{nom,-10}{age,5}"                    ' alignement : 'nom' à gauche, 'age' à droite
```

L'interpolation est généralement préférée à `String.Format("{0} {1}", a, b)` pour sa lisibilité (les
deux produisent un résultat équivalent).

> **Culture.** Par défaut, l'interpolation formate selon la **culture courante** (séparateur décimal,
> symbole monétaire…). Pour forcer un format **invariant**, assignez le littéral à une
> `FormattableString` puis utilisez `FormattableString.Invariant(...)`. Le formatage selon la culture
> est traité en [section 2.7](07-dates-nombres-culture.md).

---

## Comparaison de chaînes

L'opérateur `=` sur des chaînes obéit à `Option Compare`
([sections 2.1](01-structure-options.md) et [2.3](03-operateurs.md)). Pour un contrôle explicite,
indépendant des réglages du projet, utilisez `String.Equals` ou `String.Compare` avec un
`StringComparison` :

```vb
If String.Equals(a, b, StringComparison.OrdinalIgnoreCase) Then ...
```

Le choix entre comparaison **ordinale** et **culturelle** (et ses pièges) est détaillé en
[section 2.7](07-dates-nombres-culture.md).

---

## Et l'IA dans tout ça ? 🤖

Le texte est l'un des domaines où une traduction C# → VB échoue le plus silencieusement :

- **Séquences d'échappement** : `\n`, `\t`, `\\`, `\"` (C#) **n'existent pas** en VB. Un `"\n"` y est la
  chaîne littérale *antislash-n*. Remplacez par `Environment.NewLine` / `vbTab`, et doublez les
  guillemets (`""`).
- **Chaînes `@"…"` (verbatim) et `"""…"""` (raw)** : absentes de VB (le littéral multi-ligne simple
  existe, lui, depuis VB 14). Reconstruisez l'échappement avec `""`, `&` et `Environment.NewLine`.
- **Indexation `s[0]`** (crochets C#) → **`s(0)`** (parenthèses VB).
- **Fonctions héritées** `Mid`/`Left`/`Right`/`InStr` : base 1 et `0` en cas d'absence — à ne pas confondre
  avec la base 0 des méthodes .NET.

Bon point : l'interpolation `$"…"` se traduit **à l'identique** entre C# et VB (seule la culture par
défaut peut différer). La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- `String` est **immuable** : toute opération renvoie une nouvelle chaîne (à réaffecter).
- VB n'a **pas d'échappement antislash** (doubler `""` pour un guillemet ; `Environment.NewLine`/`vbTab`
  pour les caractères spéciaux). Les littéraux **multi-lignes existent** (VB 14+) mais embarquent
  l'indentation du source ; ni *verbatim* ni *raw strings* en revanche.
- Préférez les **méthodes .NET** (base 0) aux fonctions héritées VB6 (base 1).
- **`StringBuilder`** pour les assemblages en boucle ou volumineux ; `&` suffit pour quelques
  concaténations.
- **Interpolation `$"…"`** pour composer lisiblement (avec spécificateurs de format et alignement ;
  culture courante par défaut).

---

⏭️ [Dates, nombres, formatage et culture](/02-fondamentaux-langage/07-dates-nombres-culture.md)
