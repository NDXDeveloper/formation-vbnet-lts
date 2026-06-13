# 🧭 Exemples du module 18 — Stratégie, feuille de route et ressources

Ce module final est **un module de décision, pas de prescription** : il prend de la hauteur au-dessus
du code (stratégie officielle, cycles de support LTS/STS, grilles « rester / hybride / migrer »,
ressources). Il ne contient donc **presque pas de code** — et c'est normal.

Le **seul** extrait exécutable du chapitre est le piège de l'opérateur `^` en **§18.4** (migration
VB→C#). Il est reconstruit ici en **solution hybride complète VB + C#**, qui démontre **deux choses à
la fois** : le **piège sémantique** (en miroir du module 17) *et* le **mécanisme hybride** que la
section présente comme le bon chemin de migration — des projets VB et C# qui coexistent, compilent
vers le même IL, et s'appellent de façon transparente.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 · SDK .NET **10.0.301** ·
Windows 11 (culture machine fr-FR).

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Vérifié |
|---|---|---|
| **18.1** Stratégie Microsoft | *(pas de code)* | rappel + lecture — documenté ci-dessous |
| **18.2** Support / feuille de route | *(pas de code)* | cycles LTS/STS, tableau — documenté ci-dessous |
| **18.3** Quand rester / migrer | *(pas de code)* | grille de décision (spectre) — documenté ci-dessous |
| **18.4** Migrer vers C# | [`18.4-migration-hybride`](#184-migration-hybride) | console VB→C# **+ 2/2 tests** (piège `^` rattrapé) |
| **18.5** Communauté / ressources | *(pas de code)* | liens et stratégie d'apprentissage — documenté ci-dessous |

---

## ▶️ Comment compiler et lancer

```powershell
# Solution hybride VB + C# (depuis 18.4-migration-hybride)
dotnet run  --project src/ApplicationVb -c Release       # la console VB appelle la lib C#
dotnet test tests/CalculsTests          -c Release       # Réussi! 2/2
# (ou ouvrir MigrationHybride.slnx dans Visual Studio 2026 : 3 projets, 2 langages)
```

Toutes les valeurs ci-dessous ont été **observées à l'exécution**.

---

## 18.4-migration-hybride

- **Section** : 18.4 · **Fichier** : `04-migrer-vers-csharp.md`
- **Description** : une **solution mixte** (`MigrationHybride.slnx`, format de solution moderne de
  .NET 10) à trois projets :
  - `src/CalculsCsharp` (**C#**) — la **bibliothèque feuille « migrée »**. Elle expose `Puissance`
    (= `Math.Pow`, l'équivalent **correct** du `^` de VB) et `XorBinaire` (= `a ^ b`, ce que `^`
    fait **réellement** en C#).
  - `src/ApplicationVb` (**VB**, console) — la **partie VB restante**, qui consomme la lib C# par une
    simple référence de projet (interop hybride transparente).
  - `tests/CalculsTests` (**VB**, xUnit) — le **filet de tests**, garde-fou sémantique.
- **Sortie attendue** (vérifiée) :
  ```text
  VB   : 5 ^ 3 (puissance)          = 125
  C#   : Puissance(5,3) = Math.Pow   = 125  (équivalent CORRECT)
  C#   : 5 ^ 3 = XOR binaire         = 6    (conversion LITTÉRALE -> FAUX)
  Équivalence correcte (VB ^  =  C# Math.Pow) : True
  Conversion littérale fausse (125 <> 6)       : True

  # tests : Réussi! - échec : 0, réussite : 2, total : 2  (CalculsTests.dll, net10.0)
  ```
- **Comportement vérifié** : le `^` de VB (**125**, puissance) **égale** `Math.Pow` en C# mais
  **diffère** d'un `^` C# (**6**, XOR). Une conversion **littérale** `a ^ b` de VB vers C# changerait
  donc silencieusement le résultat — *« ça compile »* ne prouve rien. **L'interop VB↔C# fonctionne
  telle quelle** (même IL) ; c'est exactement la migration **incrémentale et réversible** prônée par
  la section (on migre une feuille en C#, le reste de VB continue de tourner, le filet de tests valide).
- **Pièges en miroir (module 17)** : outre `^`, surveiller à la traduction VB→C# la division entière
  `\` (→ `/` sur des entiers) et la **sensibilité à la casse** de C#.

---

## 📌 Sections sans projet (module de décision — pourquoi)

Aucune de ces sections ne porte de code ; ce sont des **faits** et des **grilles de décision**. Résumé
de ce qu'elles apportent, pour mémoire :

- **18.1 — Stratégie Microsoft** (`01-strategie-microsoft.md`) : VB.NET est **stable, soutenu, figé,
  cœur**. *« Stabilisé »* ≠ *« déprécié »* (contresens catastrophiste) et ≠ *« va rattraper C# »*
  (contresens du déni). « Consommation seule », pas de nouvelle syntaxe ni de nouveaux *workloads*,
  mais investissement **explicite** dans l'interop C# → l'hybride est **validé par Microsoft**.
- **18.2 — Support / feuille de route** (`02-roadmap-dotnet.md`) : train annuel (novembre) ; **LTS 3
  ans** (versions paires), **STS 2 ans** (impaires, depuis l'extension de sept. 2025). **Le support
  de VB = celui de .NET** : viser **.NET 10 LTS → 14 nov. 2028**. La longévité n'est **pas** une
  raison de partir ; l'arbitrage se joue sur les **capacités**, pas sur une échéance de fin de vie.
- **18.3 — Quand rester / migrer** (`03-cas-usage-quand-migrer.md`) : le choix n'est **pas binaire**
  mais un **spectre** — *rester → hybride → migrer*. **Rester** quand VB est sur son terrain
  (WinForms ⭐, bibliothèques, données, COM/Office, legacy) avec une base saine ; **migrer** quand un
  *workload* hors périmètre (Blazor, MAUI, Minimal APIs, AOT…) ou la friction d'écosystème
  l'emporte ; **hybride** souvent la meilleure réponse. Migrer pour un **besoin concret**, jamais
  parce que « VB est figé ».
- **18.5 — Communauté / ressources** (`05-ressources-communaute.md`) : l'écosystème est **C#-first**.
  La bonne stratégie n'est pas de traquer un matériel VB rare, mais de **lire le C# couramment et de
  le transposer** ([Annexe A]). On « continue d'apprendre » la **plateforme .NET** et **C#**, pas une
  nouvelle syntaxe VB (il n'y en a pas). Vérifier que tout outil tiers **sert vraiment VB** (pièges :
  StyleCop, source generators, générateurs `CommunityToolkit.Mvvm` inertes — cf. module 17).

> La démonstration *exécutable* la plus proche de l'esprit du module reste l'exemple **18.4** :
> l'hybride n'est pas une théorie, c'est une solution VB+C# qui compile, tourne et se teste.

---

## 🧹 Nettoyage des binaires et résidus

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 · Windows 11 (fr-FR)
