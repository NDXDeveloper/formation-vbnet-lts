# 🤖 Exemples du module 17 — Développer en VB.NET avec l'IA

Ce module est, pour l'essentiel, une **méthode de travail** (développer *avec* l'IA, en luttant
contre le biais C#), pas un catalogue d'API. Beaucoup de ses extraits de code sont **volontairement
faux** — des hallucinations C# que l'IA produit en VB. Les exemples ci-dessous **retournent la
démonstration** : ils reconstruisent, en projets VB.NET **complets, compilés et testés**, la **forme
correcte** de chaque piège, et laissent le **compilateur** et les **tests** jouer les *arbitres* que
le module ne cesse d'invoquer (« l'IA propose, le développeur valide »).

La dernière section, **17.9**, est l'exception ✅ : du vrai code de **consommation** (`Microsoft.Extensions.AI`),
le terrain de confort de VB — reconstruit et testé pour de bon.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 · SDK .NET **10.0.301** ·
Windows 11 (culture machine fr-FR).

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Vérifié |
|---|---|---|
| **17.1** Pourquoi l'IA / biais C# | [`17.1-record-immuable`](#171-record-immuable) | record C# halluciné → **classe immuable VB** (égalité par valeur) |
| **17.2** Prompting (+ **17.7** cat. C) | [`17.2-pieges-semantiques`](#172-pieges-semantiques) | `^` `\` `Mod`, court-circuit — **sorties vérifiées** |
| **17.3** Migration legacy | *(prompts ; sémantique → 17.8 Cas 1)* | conceptuel — documenté ci-dessous |
| **17.4** Tests + doc XML | [`17.4-tests-et-doc`](#174-tests-et-doc) | **10/10** tests + `Facturation.xml` généré |
| **17.5** Déboguer / optimiser | *(prompts ; arbitres = débogueur/profileur)* | conceptuel — documenté ci-dessous |
| **17.6** Workflow IA-first | [`17.6-instructions-depot`](#176-instructions-depot) | artefact `.github/copilot-instructions.md` |
| **17.7** Limites et pièges | *(catalogue ; cat. C → 17.2)* | conceptuel — documenté ci-dessous |
| **17.8** Cas concrets | [`17.8-migration-vb6`](#178-migration-vb6) · [`17.8-wpf-mvvm`](#178-wpf-mvvm) | **3/3** + **3/3** tests |
| **17.9** Intégrer l'IA ✅ | [`17.9-consommer-ia`](#179-consommer-ia) | **3/3** tests (chat + RAG, faux client) |

---

## ▶️ Comment compiler et lancer

```powershell
# Consoles (sorties vérifiables)
dotnet run  --project 17.1-record-immuable     -c Release
dotnet run  --project 17.2-pieges-semantiques  -c Release

# Bibliothèques + tests
dotnet test 17.4-tests-et-doc/tests/Facturation.Tests        # Réussi! 10/10
dotnet test 17.8-migration-vb6/tests/Migration.Tests         # Réussi!  3/3
dotnet test 17.8-wpf-mvvm/tests/DemoMvvm.Tests               # Réussi!  3/3
dotnet test 17.9-consommer-ia/tests/IntegrationIA.Tests      # Réussi!  3/3
```

Toutes les valeurs ci-dessous ont été **observées à l'exécution**.

---

## 17.1-record-immuable

- **Section** : 17.1 · **Fichier** : `01-pourquoi-ia-vbnet.md`
- **Description** : sommée de « déclarer un *record* en VB », l'IA produit souvent
  `Public Record Personne(...)` — du **C# déguisé** qui n'existe pas en VB (VB peut *consommer* un
  record C#, pas en *déclarer*). L'exemple montre l'**équivalent VB correct** : classe immuable
  écrite à la main (propriétés en lecture seule, **égalité par valeur** via `IEquatable`, hash
  cohérent, `ToString`, copie non destructive « `Avec` » = le `with` de C#).
- **Sortie attendue** (vérifiée) :
  ```text
  a = Personne { Nom = Alice, Age = 30 }
  a = b (par valeur) = True ; même hash = True
  a = c (par valeur) = False
  d = a.Avec(age:=31) -> d = Personne { Nom = Alice, Age = 31 }
  a inchangé après la copie = Personne { Nom = Alice, Age = 30 }
  ```
- **Comportement vérifié** : deux instances de mêmes valeurs sont **égales** et partagent le hash ;
  la copie « Avec » crée une **nouvelle** instance sans modifier l'originale (immuabilité).

## 17.2-pieges-semantiques

- **Section** : 17.2 (et 17.7, catégorie C) · **Fichier** : `02-prompting-vbnet.md`
- **Description** : les opérateurs « à double sens » où une **traduction littérale du C#** compile
  mais **change le résultat** — le compilateur ne voit rien, seule l'exécution/le test révèle l'écart.
- **Sortie attendue** (vérifiée) :
  ```text
  [^]    5 ^ 3   = 125 (PUISSANCE)   |   5 Xor 3 = 6 (XOR, = C# « 5 ^ 3 »)
  [\ /]  7 \ 2   = 3 (entière, = C# « int/int »)   |   7 / 2 = 3,5 (flottante)
  [Mod]  7 Mod 3 = 1
  [AndAlso] False AndAlso Sonde() : opérande droite évaluée 0 fois (court-circuit)
  [And]     False And Sonde()     : opérande droite évaluée 1 fois (PAS de court-circuit)
  [AndAlso] (x=0) x<>0 AndAlso (10\x)>1 = False (sûr, court-circuité)
  [And]     (x=0) x<>0 And (10\x)>1 -> DivideByZeroException = True
  ```
- **Comportement vérifié** : `^` est une **puissance** (XOR = `Xor`) ; `\` divise en entier, `/`
  renvoie un **flottant** (« 3,5 » — virgule décimale fr-FR) ; `Mod` = reste ; `AndAlso`/`OrElse`
  **court-circuitent** (l'opérande droite n'est pas évaluée), `And`/`Or` **non** — au point que
  `And` déclenche une `DivideByZeroException` là où `AndAlso` protège.

## 17.4-tests-et-doc

- **Section** : 17.4 · **Fichier** : `04-generer-tests-doc.md`
- **Description** : le « gradient de confiance » de la section, en VB : une bibliothèque documentée
  avec **`'''`** (le marqueur VB, **pas** `///`) et `GenerateDocumentationFile` (qui **produit**
  `Facturation.xml`), et des tests xUnit qui encodent l'**intention** (la spécification du barème de
  remise), pas l'implémentation.
- **Sortie attendue** (vérifiée) :
  ```text
  Réussi!  - échec : 0, réussite : 10, total : 10   (Facturation.Tests.dll, net10.0)
  Artefact : src/Facturation/bin/Release/net10.0/Facturation.xml  (documentation XML générée)
  ```
- **Comportement vérifié** : les 6 cas `<Theory>`/`<InlineData>` couvrent les **bornes** du barème
  (9/10/49/50), et les `<Fact>` vérifient le montant remisé et les exceptions. Points VB respectés
  (que l'IA oublie) : **attributs entre chevrons** (`<Fact>`, `<Theory>`, `<InlineData>`) et
  **lambdas `Sub`** pour `Assert.Throws` (pas la flèche `=>`).

## 17.8-migration-vb6

- **Section** : 17.8 (Cas 1) et 17.3 · **Fichier** : `08-cas-concrets.md`
- **Description** : le piège **sémantique** d'une migration `On Error Resume Next`. L'IA enveloppe
  souvent tout le bloc dans **un seul** `Try/Catch` — ce qui **abandonne** la boucle à la première
  erreur, alors que `Resume Next` **ignore** la ligne fautive et **continue**. Les deux traductions
  cohabitent ; un **test de caractérisation** fait l'arbitre.
- **Sortie attendue** (vérifiée) :
  ```text
  Réussi!  - échec : 0, réussite : 3, total : 3   (Migration.Tests.dll, net10.0)
  Entrée {"10","abc","20","30"} :  MoyenneCorrecte = 20  |  MoyenneFausse = 10 (boguée)
  ```
- **Comportement vérifié** : la traduction **correcte** (Try/Catch par itération) reproduit le
  comportement VB6 attendu (`(10+20+30)/3 = 20`) ; la **fausse** s'arrête à `"abc"` et renvoie
  `10/1 = 10`. **Le compilateur ne voit rien — seul le test révèle la divergence.**

## 17.8-wpf-mvvm

- **Section** : 17.8 (Cas 3) et 6.6 · **Fichier** : `08-cas-concrets.md`
- **Description** : la **fonctionnalité fantôme**. Invitée à réduire le *boilerplate* MVVM avec
  `CommunityToolkit.Mvvm`, l'IA propose les attributs `<ObservableProperty>` / `<RelayCommand>` —
  des **source generators réservés au C#**, qui en VB sont **reconnus mais INERTES** (rien n'est
  généré, la liaison n'a pas de cible, et le compilateur **ne dit rien**). L'exemple montre la voie
  VB correcte : `Inherits ObservableObject` + `SetProperty`, et `New RelayCommand(AddressOf …)`.
- **Sortie attendue** (vérifiée) :
  ```text
  Réussi!  - échec : 0, réussite : 3, total : 3   (DemoMvvm.Tests.dll, net10.0)
  ```
- **Comportement vérifié** (sans UI WPF) : `SetProperty` **lève `PropertyChanged`** quand la valeur
  change (et **pas** pour une valeur identique) ; `RelayCommand.Execute` **invoque** la méthode. Ce
  que les générateurs C#-only auraient dû produire, écrit à la main.

## 17.9-consommer-ia

- **Section** : 17.9 ✅ · **Fichier** : `09-consommer-ia.md`
- **Description** : la **consommation** d'IA — zone de confort de VB. `AssistantClient` consomme
  l'abstraction **`Microsoft.Extensions.AI`** (`IChatClient`) ; un **RAG basique** vectorise des
  documents en mémoire et récupère le passage pertinent par **similarité cosinus** avant de l'injecter
  dans le prompt. **Aucune clé d'API** : un **faux `IChatClient` déterministe** permet de tester la
  **plomberie** (la section rappelle qu'une sortie de modèle réelle est non déterministe).
- **Sortie attendue** (vérifiée) :
  ```text
  Réussi!  - échec : 0, réussite : 3, total : 3   (IntegrationIA.Tests.dll, net10.0)
  ```
- **Comportement vérifié** : (1) la question est **acheminée** au client et la réponse **revient** ;
  (2) le RAG **récupère** le bon document (« chat/dort » → le passage sur le chat) ; (3) le contexte
  récupéré est bien **injecté** dans le message système. Le code d'intégration serait **identique**
  avec un vrai client (Azure OpenAI / OpenAI) — seul l'appel *live* exige une clé et un service.
- **Réserve VB** : implémenter `IChatClient` exige tous ses membres ; le faux n'assure que
  `GetResponseAsync` (le *streaming* lève `NotSupportedException`, non requis ici). La vectorisation
  est volontairement simpliste — un vrai système utiliserait un `IEmbeddingGenerator` et une base
  vectorielle.

## 17.6-instructions-depot

- **Section** : 17.6 · **Fichier** : `06-workflow-ia-first.md`
- **Description** : artefact (non compilé) — un `.github/copilot-instructions.md` prêt à l'emploi qui
  ancre le **chat**, les **agents** et la **revue de code** sur VB.NET (.NET 10, `Option Strict On`,
  pièges d'opérateurs, pas de constructions C#). **Ne s'applique pas à la complétion en ligne**, qui
  ne lit pas ce fichier : pour elle, c'est le **contexte du dépôt** (du vrai code VB) qui compte.

---

## 📌 Sections sans projet (conceptuelles — pourquoi)

- **17.3 Migration legacy** : essentiellement des **gabarits de prompts** et de la méthode. Le seul
  point *exécutable* — le piège sémantique `On Error Resume Next` — est démontré et **testé** dans
  [`17.8-migration-vb6`](#178-migration-vb6). La méthodologie de fond est au [module 11](../../11-migration-legacy/README.md).
- **17.5 Déboguer / optimiser** : des prompts d'explication d'erreurs et d'optimisation. Les
  **arbitres** sont le **débogueur** et le **profileur** (qu'un projet d'exemple ne peut pas
  « exécuter » à votre place) — voir modules [12](../../12-exceptions-debogage/README.md) et
  [14](../../14-performance/README.md).
- **17.7 Limites et pièges** : c'est le **catalogue de référence**. Sa catégorie « ça compile mais
  c'est faux » (sémantique) est rendue **exécutable** dans [`17.2-pieges-semantiques`](#172-pieges-semantiques).
- **17.8 Cas 2 — API REST** : le « piège » est la **dérive vers les Minimal APIs** (sans modèle de
  projet VB). La voie correcte — une **Web API par contrôleurs** — est déjà construite et vérifiée
  aux **modules 8 et 16** (cf. `16.1-autorisation-api`). Ici, la leçon est de **tenir la frontière**
  VB/C# (voir [8.3](../../08-services-web/03-limites-web-vbnet.md)).

---

## 🧹 Nettoyage des binaires et résidus

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 · Windows 11 (fr-FR)
