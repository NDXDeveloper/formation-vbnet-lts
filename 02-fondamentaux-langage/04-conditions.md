🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 2.4 Structures conditionnelles

> VB.NET offre un `If…Then…Else` lisible, un opérateur ternaire `If()` à court-circuit (à ne pas
> confondre avec l'ancienne fonction `IIf`), et un `Select Case` particulièrement expressif (plages
> `To`, comparaisons `Is`). En revanche, le **filtrage de motifs** reste **plus limité qu'en C#** — un
> point que cette section assume franchement.

---

## `If…Then…Else`

La forme la plus courante est le **bloc multi-ligne**, fermé par `End If` :

```vb
If solde > 0 Then
    Console.WriteLine("Créditeur")
ElseIf solde = 0 Then
    Console.WriteLine("À zéro")
Else
    Console.WriteLine("Débiteur")
End If
```

Trois points à retenir :

- **`ElseIf` s'écrit en un seul mot** (et non `Else If`).
- La condition doit être un **`Boolean`** : sous `Option Strict On`, aucune conversion implicite d'un
  nombre en booléen n'est tolérée.
- Privilégiez **`AndAlso` / `OrElse`** dans les conditions ([section 2.3](03-operateurs.md)) : leur
  court-circuit évite d'évaluer — voire de faire échouer — une expression inutile.

Pour une instruction unique, la **forme sur une ligne** se passe de `End If` :

```vb
If estVide Then Return
If x > 0 Then Console.WriteLine("positif") Else Console.WriteLine("négatif ou nul")
```

---

## L'opérateur `If()` (ternaire et coalescence)

L'**opérateur** `If()` produit une valeur. Il existe sous deux formes :

```vb
' Ternaire : If(condition, valeurSiVrai, valeurSiFaux)
Dim statut = If(age >= 18, "majeur", "mineur")

' Coalescence : If(valeur, repli) → 'valeur' si non Nothing, sinon 'repli'
Dim nom = If(saisie, "Anonyme")
```

La forme de coalescence remplace le `??` de C#, absent de VB (déjà rencontrée pour les nullables en
[section 2.2](02-types-variables.md)).

> ⚠️ **`If()` (opérateur) vs `IIf()` (fonction héritée).** Ne confondez pas l'opérateur `If()` avec
> l'ancienne fonction `IIf()`. `IIf` est un **appel de fonction** : il évalue **les deux** branches quoi
> qu'il arrive, et renvoie un `Object`. L'opérateur `If()` (introduit avec VB 2008) **court-circuite** :
> seule la branche retenue est évaluée, et le type de retour est correctement inféré.

```vb
' À éviter : IIf évalue 'client.Solde' même si 'client' est Nothing → exception
' Dim v = IIf(client IsNot Nothing, client.Solde, 0)

' Correct : If() n'évalue 'client.Solde' que si nécessaire
Dim v = If(client IsNot Nothing, client.Solde, 0)
```

**Utilisez toujours l'opérateur `If()`, jamais `IIf()`.**

---

## `Select Case`

`Select Case` compare une expression à une série de cas. Sa syntaxe est plus riche que celle d'un
`switch` classique : on peut tester une valeur, une **liste de valeurs**, une **plage** (`To`) ou une
**comparaison relationnelle** (`Is`) :

```vb
Select Case note
    Case 90 To 100
        Console.WriteLine("Excellent")     ' plage inclusive
    Case 70 To 89
        Console.WriteLine("Bien")
    Case 50, 55, 60
        Console.WriteLine("Passable")      ' plusieurs valeurs
    Case Is < 50
        Console.WriteLine("Insuffisant")   ' comparaison relationnelle
    Case Else
        Console.WriteLine("Note invalide")
End Select
```

Quelques caractéristiques importantes :

- **Pas de *fall-through*.** Contrairement au `switch` du C, chaque `Case` est indépendant : il n'y a ni
  `break` à écrire, ni risque d'enchaînement involontaire d'un cas au suivant. C'est un avantage de
  sûreté.
- **L'expression de test est évaluée une seule fois.**
- `Select Case` fonctionne sur tout type comparable : nombres, chaînes, `Char`, énumérations. Pour les
  **chaînes**, la sensibilité à la casse suit `Option Compare` ([section 2.1](01-structure-options.md)).

> Dans `Case Is < 50`, le mot-clé `Is` n'a **pas** le sens d'égalité de référence vu en
> [section 2.3](03-operateurs.md) : ici, il introduit simplement une comparaison avec la valeur testée.

Exemple sur des chaînes :

```vb
Select Case commande.ToLower()
    Case "ouvrir", "open"
        Ouvrir()
    Case "fermer", "close"
        Fermer()
    Case Else
        Console.WriteLine("Commande inconnue")
End Select
```

---

## `TypeOf…Is` et tests de type ⚠️

L'opérateur **`TypeOf … Is`** teste si un objet est d'un type donné (en tenant compte de l'héritage et
de l'implémentation d'interfaces) ; sa négation est **`TypeOf … IsNot`** :

```vb
Dim o As Object = "Bonjour"
If TypeOf o Is String Then
    Dim s = DirectCast(o, String)   ' on transtype ensuite
    Console.WriteLine(s.Length)
End If
```

On reconnaît l'idiome VB : **tester d'abord, transtyper ensuite** (avec `DirectCast`, `CType` ou
`TryCast`, voir [section 2.3](03-operateurs.md)).

**`TypeOf…Is` vs `GetType`.** `TypeOf o Is Animal` est vrai aussi pour les types **dérivés** d'`Animal`
ou qui l'implémentent. Pour un test de type **exact**, on compare les objets `Type` :
`o.GetType() Is GetType(Animal)`.

### Une limite assumée : le filtrage de motifs

C'est le point ⚠️ signalé par le sommaire. C# dispose d'un **filtrage de motifs** (*pattern matching*)
riche : test de type **avec liaison** en une expression (`If (o Is String s)`), **expressions `switch`**,
motifs de propriété, motifs relationnels et combinateurs `and`/`or`. **VB n'en a presque rien.** Il
faut donc :

- tester puis transtyper en **deux temps** (pas de liaison intégrée comme `o Is String s` en C#) ;
- enchaîner des `If … ElseIf` de `TypeOf…Is` pour un aiguillage par type (il n'existe pas d'équivalent
  d'une expression `switch` sur un type).

En pratique, une logique d'aiguillage par type est plus **verbeuse** qu'en C# :

```vb
' VB : aiguillage par type via une chaîne If/ElseIf
Dim aire As Double
If TypeOf forme Is Cercle Then
    Dim c = DirectCast(forme, Cercle)
    aire = Math.PI * c.Rayon ^ 2
ElseIf TypeOf forme Is Rectangle Then
    Dim r = DirectCast(forme, Rectangle)
    aire = r.Largeur * r.Hauteur
End If
```

Ce code reste parfaitement lisible et idiomatique en VB. Mais lorsqu'une logique repose massivement sur
le filtrage de motifs (analyse d'arbres, dispatch complexe, déconstruction), c'est un **candidat naturel
à l'isolement dans une bibliothèque C#**, consommée ensuite depuis VB — la stratégie hybride du
[module 10](../10-hybride-vbnet-csharp/README.md) et de
l'[Annexe B.7](../annexes/frontiere-vbnet-csharp/README.md).

---

## Et l'IA dans tout ça ? 🤖

Les conditionnelles sont un terrain de divergence syntaxique avec C#. À surveiller dans le code généré :

- **`else if`** (C#) → **`ElseIf`** en un seul mot.
- **Ternaire `?:`** → **`If(condition, a, b)`** ; **`??`** → **`If(valeur, repli)`**.
- **Expressions `switch` et liaison de motif** (`o is string s`, `x switch { … }`) **n'existent pas** en
  VB : un assistant peut produire un code qui « lie » directement un type, ce qui ne compile pas. Repliez
  sur `Select Case` (pour les valeurs) ou `TypeOf…Is` + transtypage (pour les types).
- Préférez l'opérateur **`If()`** à la fonction **`IIf()`** que d'anciens exemples (ou une IA mal guidée)
  pourraient suggérer.

La méthode générale figure au [module 17](../17-developpement-ia/README.md).

## En résumé

- **`If…Then…Else`** : `ElseIf` en un mot, conditions strictement booléennes, `AndAlso`/`OrElse`
  recommandés.
- **Opérateur `If()`** : ternaire et coalescence, à court-circuit — **toujours préféré à `IIf()`**.
- **`Select Case`** : valeurs multiples, plages `To`, comparaisons `Is`, `Case Else`, **sans
  *fall-through*** (un atout de sûreté).
- **`TypeOf…Is`** : test de type (avec héritage) suivi d'un transtypage en deux temps. Le **filtrage de
  motifs de VB est volontairement limité** par rapport à C# ⚠️ — pour les cas lourds, déléguer à une
  bibliothèque C#.

---

⏭️ [Boucles et itérations (For…Next, For Each, Do…Loop, While)](/02-fondamentaux-langage/05-boucles.md)
