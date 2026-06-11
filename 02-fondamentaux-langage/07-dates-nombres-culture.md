🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.7 Dates, nombres, formatage et culture

> La **culture** est le fil rouge de cette section : c'est la première cause des bugs « ça marche sur ma
> machine ». Le séparateur décimal, le symbole monétaire, l'ordre jour/mois ou la casse d'un caractère
> varient d'une locale à l'autre. La règle, répétée tout au long de cette section, est simple :
> **soyez toujours explicite sur la culture** dès que des données traversent les systèmes.

Les types et fonctions de globalisation vivent dans l'espace `System.Globalization` :

```vb
Imports System.Globalization
```

---

## Dates : le type `Date`

En VB, **`Date` est l'alias de `System.DateTime`** ([section 2.2](02-types-variables.md)). On crée une
date de plusieurs façons :

```vb
Dim maintenant = Date.Now          ' date et heure locales
Dim aujourdhui = Date.Today        ' date du jour, heure à minuit
Dim instantUtc = Date.UtcNow       ' instant en temps universel
Dim precise = New Date(2026, 6, 10)            ' via le constructeur
Dim litteral = #2026-06-10#                    ' littéral de date
```

> ⚠️ **Format des littéraux `#…#`.** À l'intérieur d'un littéral, la date est interprétée de façon
> **invariante**, au format anglo-saxon `#M/d/yyyy#` : `#6/10/2026#` désigne le **10 juin**, pas le
> 6 octobre. Pour lever toute ambiguïté, utilisez la forme **ISO `#yyyy-MM-dd#`**.

`Date` est **immuable** (comme `String`) : les opérations renvoient une **nouvelle** date. La soustraction
de deux dates produit un **`TimeSpan`** (une durée) :

```vb
Dim echeance = aujourdhui.AddDays(30)      ' nouvelle date, 30 jours plus tard
Dim duree As TimeSpan = echeance - aujourdhui
Console.WriteLine(duree.TotalDays)         ' 30
Dim composant = maintenant.Year & "-" & maintenant.Month   ' .Year, .Month, .Day, .Hour…
```

### Types complémentaires (.NET moderne)

- **`DateOnly`** et **`TimeOnly`** (depuis .NET 6) : une date sans heure, une heure sans date — plus
  justes pour une date de naissance ou un horaire d'ouverture. On les emploie par leur nom complet (le
  mot-clé `Date` reste réservé à `DateTime`).
- **`DateTimeOffset`** : une date-heure **assortie de son décalage par rapport à UTC**. C'est le type à
  privilégier pour représenter un **instant non ambigu** à travers les fuseaux. Il évite le piège de la
  propriété `Kind` de `DateTime` (`Unspecified` / `Utc` / `Local`), source classique d'erreurs. Les
  conversions de fuseau passent par `TimeZoneInfo`.

```vb
Dim naissance As New DateOnly(1990, 5, 12)           ' une date, sans heure ni fuseau
Dim ouverture As New TimeOnly(9, 30)                 ' une heure, sans date
Dim instant As DateTimeOffset = DateTimeOffset.Now   ' date + heure + décalage UTC
```

---

## Analyser (parser) une date

Quatre méthodes, selon le besoin :

- `Date.Parse` / `Date.TryParse` : analyse souple selon une culture.
- `Date.ParseExact` / `Date.TryParseExact` : analyse selon un **format imposé**.

Pour des entrées **non fiables** (saisie, fichier, API), préférez la variante **`TryParse`** (pas
d'exception) et **précisez la culture** :

```vb
Dim texte = "10/06/2026"
Dim resultat As Date
If Date.TryParse(texte, New CultureInfo("fr-FR"), DateTimeStyles.None, resultat) Then
    ' resultat = 10 juin 2026 (interprété en culture française : jour/mois)
End If

' Format strictement imposé :
Dim iso = Date.ParseExact("2026-06-10", "yyyy-MM-dd", CultureInfo.InvariantCulture)
```

---

## Formater une date

`ToString` accepte un format et, idéalement, une culture explicite. On distingue les **formats standard**
(une lettre) des **formats personnalisés** :

| Format | Exemple (fr-FR) | Usage |
|--------|-----------------|-------|
| `"d"` / `"D"` | `10/06/2026` / `mercredi 10 juin 2026` | Date courte / longue |
| `"t"` / `"T"` | `14:30` / `14:30:00` | Heure courte / longue |
| `"g"` / `"G"` | `10/06/2026 14:30` | Date + heure |
| `"o"` / `"O"` | `2026-06-10T14:30:00.0000000` | **Aller-retour** ISO 8601 |
| `"s"` | `2026-06-10T14:30:00` | Triable (ISO) |
| `"yyyy-MM-dd HH:mm"` | `2026-06-10 14:30` | Personnalisé |

> ⚠️ **`MM` ≠ `mm`.** Dans un format personnalisé, `MM` désigne le **mois** et `mm` les **minutes** ;
> `HH` est l'heure sur 24, `hh` sur 12. Une confusion fréquente.

La règle d'or :

```vb
' Stockage / échange : format invariant ISO (aller-retour fiable, indépendant de la machine)
Dim horodatage = Date.UtcNow.ToString("o", CultureInfo.InvariantCulture)

' Affichage à l'utilisateur : culture courante
Dim affichage = Date.Now.ToString("D", CultureInfo.CurrentCulture)
```

---

## Nombres

Les types numériques ont été présentés en [section 2.2](02-types-variables.md). Deux rappels décisifs
ici :

- **`Decimal` pour les montants.** `Double` et `Single` sont des flottants **binaires** : ils
  n'expriment pas exactement les décimaux. `0.1 + 0.2` y vaut `0.30000000000000004`, pas `0.3`. Pour de
  l'argent et tout calcul décimal exact, utilisez **`Decimal`** (suffixe `D`).
- **L'analyse dépend de la culture.** C'est le piège numéro un.

### Le piège du séparateur décimal ⚠️

En français, le séparateur décimal est la **virgule** (`1,5`) et l'espace sépare les milliers ; en
anglais, c'est le **point** (`1.5`) et la virgule sépare les milliers. Analyser une donnée d'échange sans
préciser la culture aboutit à un résultat dépendant de la machine :

```vb
Dim brut = "1234.56"   ' donnée d'un fichier/API : point décimal (format invariant)

' ⚠️ Sur une machine en français, Double.Parse(brut) (culture courante)
'    peut échouer ou mal interpréter le point.

' ✅ Toujours préciser la culture pour des données d'échange :
Dim valeur = Double.Parse(brut, CultureInfo.InvariantCulture)   ' 1234.56

' ✅ Et préférer TryParse pour une entrée non fiable :
Dim n As Double
If Double.TryParse(brut, NumberStyles.Float, CultureInfo.InvariantCulture, n) Then
    ' n est valide
End If
```

`NumberStyles` précise ce qui est toléré (espaces, séparateurs de milliers, symbole monétaire, signe…).

### Formater des nombres

| Format | Rôle | Exemple |
|--------|------|---------|
| `"N2"` | Nombre, séparateur de milliers, 2 décimales | `1 234,50` |
| `"C2"` | Monétaire (symbole de la culture) | `1 234,50 €` |
| `"P1"` | Pourcentage | `12,5 %` |
| `"F2"` | Fixe (sans séparateur de milliers) | `1234,50` |
| `"D5"` | Entier complété à gauche par des zéros | `00042` |
| `"X"` | Hexadécimal | `FF` |
| `"#,##0.00"` | Personnalisé | `1 234,50` |

```vb
Dim montant = 1234.5D
Console.WriteLine(montant.ToString("C2"))                            ' "1 234,50 €" (fr-FR)
Console.WriteLine(montant.ToString("N2", CultureInfo.InvariantCulture)) ' "1,234.50"
```

---

## Culture : `CultureInfo`

Trois cultures structurent tout le système :

- **`CultureInfo.CurrentCulture`** : régit le **formatage et l'analyse** des nombres, dates et monnaies.
- **`CultureInfo.CurrentUICulture`** : régit la **recherche de ressources** (traductions). C'est l'objet
  de l'internationalisation, traitée au [module 5.11](../05-windows-forms/11-internationalisation.md).
- **`CultureInfo.InvariantCulture`** : une culture **stable et indépendante** de la machine. À utiliser
  pour tout ce qui doit être **reproductible** : fichiers, API, journaux, clés, sérialisation.

On instancie une culture par son nom : `New CultureInfo("fr-FR")`, `New CultureInfo("en-US")`.

### Comparaison de chaînes : ordinale vs culturelle

Rappel de [section 2.6](06-chaines.md) : le choix du `StringComparison` n'est pas anodin.

- **Ordinale** (`StringComparison.Ordinal` / `OrdinalIgnoreCase`) : comparaison binaire, rapide et
  **stable**. À utiliser pour les **identifiants, clés, noms de fichiers, chaînes de protocole**.
- **Culturelle** (`CurrentCulture` / `InvariantCulture`) : comparaison **linguistique**, pour le **tri et
  l'affichage** destinés à l'utilisateur.

> Pourquoi cette distinction ? Une comparaison culturelle peut réserver des surprises : en turc (tr-TR),
> `"i".ToUpper()` ne donne pas `"I"` mais `"İ"`. Comparer des identifiants en tenant compte de la culture
> peut donc échouer selon la locale — d'où l'usage **ordinal** pour tout ce qui n'est pas linguistique.

---

## Bonnes pratiques (récapitulatif)

- **Stockage / échange** : `CultureInfo.InvariantCulture` et format ISO 8601 (`"o"` pour les dates).
- **Affichage** : culture courante (ou culture utilisateur explicite).
- **Argent** : `Decimal`, jamais `Double`.
- **Analyse d'entrées externes** : `TryParse` + culture explicite + `NumberStyles` adapté.
- **Comparaison de clés/identifiants** : `StringComparison.Ordinal`.
- En un mot : **ne jamais s'en remettre à la culture par défaut de la machine** pour des données qui
  circulent entre systèmes.

---

## Et l'IA dans tout ça ? 🤖

Le formatage et la culture sont des angles morts fréquents du code généré :

- **Culture omise** : un assistant écrit souvent `Double.Parse(s)` ou `date.ToString("d")` **sans
  culture**, ce qui crée un bug dépendant de la machine. Exigez une **culture explicite**
  (`InvariantCulture` pour les données, culture courante pour l'affichage).
- **`Parse` au lieu de `TryParse`** pour des entrées non fiables (exception sur donnée invalide).
- **`Double` au lieu de `Decimal`** pour des montants (rappel de [section 2.2](02-types-variables.md)).
- **`DateTime` vs `Date`** : les deux fonctionnent, mais le code VB idiomatique emploie `Date`. Notez
  aussi que C# n'a pas de littéral `#…#` : une date y est construite via `new DateTime(...)`.
- **`MM` / `mm`** intervertis dans un format personnalisé.

La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- `Date` (= `DateTime`) est **immuable** ; littéraux `#…#` au format **invariant** `M/d/yyyy` (préférez
  l'ISO `#yyyy-MM-dd#`). Pour un instant non ambigu, **`DateTimeOffset`** ; pour une date ou une heure
  seule, **`DateOnly`** / **`TimeOnly`**.
- **`Decimal` pour l'argent** ; `Double`/`Single` sont des flottants binaires imprécis.
- **Toujours préciser la culture** pour analyser et formater des données d'échange : **`InvariantCulture`**
  pour le stockage, culture courante pour l'affichage.
- **`TryParse`** pour les entrées non fiables ; **`StringComparison.Ordinal`** pour les clés.

---

⏭️ [Tableaux et collections](/02-fondamentaux-langage/08-tableaux-collections.md)
