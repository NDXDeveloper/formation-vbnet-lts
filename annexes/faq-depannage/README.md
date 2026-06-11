🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe G — FAQ et dépannage

Erreurs courantes et solutions, problèmes de performance, problèmes de déploiement, compatibilité des versions
.NET, et pièges fréquents avec l'IA et VB.NET. 🤖

Cette annexe rassemble les obstacles les plus fréquents, organisés par thème, avec un diagnostic rapide. Beaucoup
de pièges propres à VB tiennent à des **différences avec C#** (voir [Annexe A](../correspondance-vbnet-csharp/README.md))
ou au fait que le **langage est figé** (VB 16.9).

---

## G.1 — Erreurs courantes et solutions

**« Mon code ne compile plus après avoir activé `Option Strict On` » (BC30512, BC30574…)**
C'est attendu, et c'est le but : `Option Strict On` interdit les conversions restrictives implicites et le *late
binding*. Corrigez par des **conversions explicites** (`CInt`, `CType`, `DirectCast`) ou en typant correctement
les variables. C'est un investissement, pas un bug (Annexe C.1).

**Un tableau a « un élément de trop » ou lève `IndexOutOfRangeException`.**
En VB, `Dim a(4)` crée **5** éléments (indices 0 à 4) — la valeur est l'**indice maximal**, pas la taille
(contrairement à C#). Ajustez vos bornes en conséquence ([Annexe A § A.5](../correspondance-vbnet-csharp/README.md)).

**`Dim i As Integer = Nothing` vaut `0`, pas `null` — pourquoi ?**
En VB, `Nothing` est **aussi la valeur par défaut des types valeur**. Un `Integer` à `Nothing` vaut donc `0` — et,
plus piégeux, la comparaison `i = Nothing` est **vraie dès que `i` vaut `0`**. Le test `i Is Nothing`, lui, est
**refusé à la compilation** sur un type valeur non nullable (BC30020 : `Is` exige une référence ou un `Nullable`).
Raisonnez en « valeur par défaut » pour les types valeur, en « absence de référence » pour les types référence —
et déclarez `Integer?` si « pas de valeur » doit réellement exister ([Annexe A § A.3](../correspondance-vbnet-csharp/README.md)).

**`5 / 2` donne `2.5` au lieu de `2`.**
En VB, l'opérateur `/` renvoie **toujours un flottant**. Pour une division **entière**, utilisez `\` :
`5 \ 2` vaut `2` (piège classique à la transposition depuis C#, où `/` entre entiers est entier).

**`"Dupont" = "dupont"` est vrai alors que je ne le veux pas.**
Votre projet est probablement en `Option Compare Text` (comparaison insensible à la casse). Repassez en
`Option Compare Binary` au niveau projet, et utilisez explicitement `StringComparison` pour les cas particuliers
(Annexe C.1).

**`NullReferenceException` à l'exécution.**
Vous accédez à un membre d'un objet à `Nothing`. Ajoutez des **clauses de garde**, utilisez l'opérateur
conditionnel `?.`, et validez les arguments tôt (`ArgumentNullException`).

**Un gestionnaire d'événement ne se déclenche plus.**
Deux causes classiques, symétriques. Avec **`AddHandler`** : vous avez **réaffecté la variable** vers un nouvel
objet, mais l'abonnement manuel est resté accroché à l'**ancien** — réabonnez-vous après la réaffectation (ou
passez à `WithEvents`). Avec **`WithEvents`/`Handles`** : le câblage **suit automatiquement la variable**
(re-câblage à chaque réaffectation, module 3.6) — c'est donc toujours l'objet **actuellement référencé** qui est
écouté ; si un ancien objet, encore utilisé ailleurs, lève l'événement, celui-ci n'est plus capté.

**« Opération inter-threads non valide » en WinForms/WPF.**
Vous mettez à jour l'UI depuis un thread d'arrière-plan. Repassez sur le thread UI : `Control.Invoke`/`BeginInvoke`
en WinForms, le `Dispatcher` en WPF, ou — plus simplement — `Async`/`Await` qui restaure le contexte
(module 4, module 5.6).

**Mon application se fige sur un appel asynchrone.**
Vous bloquez sur de l'asynchrone (`.Result` / `.Wait()`) depuis le thread UI → interblocage. **Restez asynchrone
de bout en bout** ; dans les bibliothèques, ajoutez `ConfigureAwait(False)` (module 4.2).

**« Le Concepteur n'a pas pu être affiché. »**
Souvent dû à un contrôle personnalisé sans **constructeur sans paramètre**, ou à de la logique lourde dans le
constructeur. Prévoyez un constructeur vide, allégez le constructeur, puis **régénérez** la solution (module 5.5).

---

## G.2 — Problèmes de performance

**Une concaténation de chaînes dans une boucle est très lente.**
Chaque `&` alloue une nouvelle chaîne. Utilisez **`StringBuilder`** pour construire le texte, puis `.ToString()`
une seule fois (module 2.6).

**Beaucoup de requêtes SQL pour afficher une liste (problème N+1).**
Le *lazy loading* d'EF Core déclenche une requête par entité liée. Utilisez le **chargement hâtif** (`Include`),
une **projection** (`Select`), et `AsNoTracking` en lecture seule ; **inspectez le SQL généré** (module 7.2).

**Une requête LINQ semble s'exécuter plusieurs fois.**
Une requête LINQ est **différée** : chaque énumération la ré-exécute. **Matérialisez** une fois avec `ToList()` /
`ToArray()` si vous parcourez le résultat plusieurs fois (module 2.9).

**Forte consommation mémoire / pression sur le GC.**
Réduisez les allocations, **libérez** les ressources (`Using`/`IDisposable`), et envisagez le *pooling*
(`ArrayPool`, `ObjectPool` — en consommation). Évitez le *boxing* en utilisant des **génériques** (module 14).

**Un `DataGridView` rame avec beaucoup de lignes (WinForms).**
Activez le **mode virtuel**, suspendez la mise en page pendant le remplissage, et évitez l'auto-dimensionnement
sur de gros volumes (module 5.4).

**Une vue WPF est lente à l'affichage.**
Activez la **virtualisation** de l'UI, limitez le nombre de bindings, gelez (`Freeze`) les ressources
partagées et réduisez l'arbre visuel (module 6.9).

**« Mon application est lente alors que je n'ai rien changé. »**
Vous tournez peut-être encore sur un ancien runtime. **Migrer vers .NET 10** apporte des gains JIT/PGO/SIMD
**sans changer le code** (module 14.6, [Annexe E](../migration-net10/README.md)).

---

## G.3 — Problèmes de déploiement

**« You must install .NET to run this application. »**
Déploiement *framework-dependent* sur une machine sans le runtime. **Installez le runtime .NET 10** sur la cible,
ou publiez en **self-contained** (runtime embarqué — module 15.1).

**`appsettings.json` introuvable à l'exécution.**
Le fichier n'est pas copié dans la sortie. Réglez sa propriété **« Copier dans le répertoire de sortie »**
(`Copy if newer`) et vérifiez l'action de génération.

**« Could not load file or assembly… » au démarrage.**
Dépendance manquante ou conflit de version. Vérifiez la **sortie de publication** (toutes les `.dll`
présentes), les versions de paquets, et l'architecture (x86/x64/ARM64) cohérente.

**Mauvaise architecture (x86 / x64 / ARM64).**
Alignez le **RID** de publication et la cible. Attention au mélange de composants natifs de bitness différents
(fréquent avec l'interop COM/Office).

**Le *single-file* casse certaines fonctions.**
La publication en fichier unique modifie l'emplacement des assemblies ; du code reposant sur l'emplacement
physique des fichiers peut échouer. Tenez compte de ces limites (module 15.1).

**ClickOnce / MSIX : erreurs de signature ou de mise à jour.**
Vérifiez les **certificats**, le manifeste et le canal de mise à jour. Pour le *Store*, suivez les contraintes
de packaging (modules 15.1-15.2).

**L'interop COM/Office échoue sur la machine cible.**
Office doit être **installé**, dans une **version et un bitness compatibles**. Le *late binding*
(`Option Strict Off` local) tolère mieux les écarts de version, au prix de la sûreté de type (module 9.2).

**Docker : mon application WinForms ne démarre pas dans un conteneur Linux.**  
**Windows Forms est propre à Windows** : impossible à exécuter dans un conteneur Linux. Une **Web API VB par
contrôleurs**, elle, se conteneurise sans problème (module 15.4, [Annexe B § B.8](../frontiere-vbnet-csharp/README.md)).

---

## G.4 — Compatibilité des versions .NET

**« Pourquoi ne puis-je pas cibler `net10.0` dans Visual Studio 2022 ? »**
Parce qu'il faut **Visual Studio 2026 (v18)** pour cibler `net10.0`. Visual Studio 2022 (17.14) avec le SDK
.NET 10 ne permet de cibler que **.NET 9 et antérieurs**. Intégrez la montée vers VS 2026 à votre migration
([Annexe E § E.4](../migration-net10/README.md)).

**« Quelle version dois-je viser ? »**  
**.NET 10 (LTS)**, pris en charge jusqu'en **novembre 2028**. **.NET 8 et .NET 9 atteignent leur fin de support
le 10 novembre 2026** ; **.NET 6 et 7 sont déjà hors support**. Ne démarrez rien de neuf sur une version en fin
de vie ([Annexe E](../migration-net10/README.md), Annexe H).

**« Puis-je faire cohabiter .NET Framework et .NET moderne ? »**
Pas par référence directe d'un monde à l'autre. Le pont est **`.NET Standard 2.0`** : placez le code partagé dans
une bibliothèque ciblant `netstandard2.0`, consommable des deux côtés (module 11.5).

**« Un de mes paquets NuGet n'est pas compatible .NET 10. »**
Cherchez une **version mise à jour** du paquet (ou un TFM/`.NET Standard` compatible). À défaut, identifiez une
**alternative** maintenue avant de migrer (Annexe E.6).

**« `.NET Standard` est-il mort ? »**
Non : il reste utile comme **passerelle de compatibilité** (bibliothèques partagées entre Framework et moderne).
Pour du code **uniquement moderne**, ciblez directement `net10.0` (module 11.5).

**« Puis-je exécuter une application .NET 10 sur une machine n'ayant que .NET 8 ? »**
Non, en *framework-dependent* : la version majeure du runtime doit correspondre. **Installez le runtime .NET 10**
ou publiez en **self-contained**.

**« Quelle version du langage VB dois-je choisir ? »**
Aucune à choisir : le langage est **figé à VB 16.9** pour ce qui s'écrit (stabilisé, *consumption-only*).
Les rares incréments du compilateur depuis (comme VB 17.13, début 2025) n'apportent que de la **consommation**,
aucune syntaxe nouvelle — viser .NET 10 ne « met pas à jour » votre code VB (modules 1.6, 18).

---

## G.5 — Pièges fréquents avec l'IA et VB.NET 🤖

**« J'ai demandé du VB.NET et l'IA m'a répondu en C#. »**
Comportement courant : par défaut, les modèles produisent du C#. **Précisez toujours « VB.NET » et la version
.NET cible**. Si la réponse est en C#, demandez la conversion **puis validez-la** (module 17.2, Annexe C.6).

**« L'IA a généré du VB truffé de syntaxe C# qui ne compile pas. »**
Ce sont des **« C#-ismes »** : `;`, accolades `{}`, lambdas `=>` (forme C#), `record`, `init`, `await foreach`,
*switch expressions*, *top-level statements*. **Aucun n'existe en VB** : recoupez avec les
[Annexes A](../correspondance-vbnet-csharp/README.md) et [B](../frontiere-vbnet-csharp/README.md), et **compilez**
(module 17.7). ⚠️ Cas à part : les **séquences d'échappement** (`\n`, `\\`) **compilent** en VB mais y restent
**littérales** — le bug est silencieux. Transposez-les (`vbCrLf`, `\` simple — Annexe A § A.13).

**« L'IA confond VB.NET avec VB6 ou VBA. »**
Dire seulement « Visual Basic » peut produire des macros VBA ou du VB6. **Insistez sur « .NET »** dans le prompt
(module 17.2).

**« L'IA affirme que VB sait faire quelque chose… qu'il ne sait pas faire. »**
Hallucination fréquente (déclarer un `record`, utiliser `await foreach`…). **Méfiez-vous des affirmations de
fonctionnalités** et vérifiez contre l'[Annexe B](../frontiere-vbnet-csharp/README.md).

**« Les suggestions Copilot sont moins bonnes dans mon projet VB qu'en C#. »**
C'est attendu : la qualité des suggestions **dépend du volume de données d'entraînement** par langage, et VB est
bien moins représenté que C#. **Donnez plus de contexte** (`Option Strict`, exemples) et **validez davantage**
([Annexe D § D.5](../visual-studio-2026/README.md)).

**« L'IA a utilisé une API .NET obsolète. »**
Les modèles peuvent suggérer des API d'une **autre version** de .NET. **Vérifiez la disponibilité en .NET 10**
avant d'adopter (module 17.7).

**« Puis-je faire confiance au code SQL / d'authentification / de cryptographie généré ? »**
Pas à l'aveugle : risque de sécurité réel (API dépassées, schémas non sûrs). **Relecture humaine obligatoire**,
en particulier pour le code de sécurité (Annexe C.6, module 16).

**« Adaptive Paste n'a pas converti mon code C# en VB. »**
Adaptive Paste adapte le **style** (conventions, mise en forme), **pas le langage**. Pour convertir du C#,
**demandez-le explicitement** dans Copilot Chat, puis **validez** ([Annexe D § D.5](../visual-studio-2026/README.md)).

**« Quels outils IA prennent réellement en charge VB ? »**
Le **Copilot général** (complétions + chat) fonctionne avec VB, et l'agent **`@modernize-dotnet`** prend en charge
VB ✅ (migration). En revanche, les **agents et outils de langage les plus récents** (agents C#/C++, navigation
symbolique d'agent) ciblent d'abord **C#** ([Annexe D § D.5](../visual-studio-2026/README.md)).

---

### Voir aussi

- [Annexe A — Correspondance syntaxique VB.NET ↔ C#](../correspondance-vbnet-csharp/README.md) (la plupart des pièges de transposition)
- [Annexe B — Frontière VB.NET / C#](../frontiere-vbnet-csharp/README.md) 🔗 (ce que VB ne sait pas faire)
- [Annexe C — Bonnes pratiques](../bonnes-pratiques/README.md) (dont les règles d'usage de l'IA)
- [Annexe D — Visual Studio 2026](../visual-studio-2026/README.md) · [Annexe E — Migration vers .NET 10](../migration-net10/README.md) · [Annexe H — Versions et cycle de support](../versions-reference/README.md)
- Module 12 — [Exceptions, débogage et journalisation](../../12-exceptions-debogage/README.md) · Module 14 — [Performance](../../14-performance/README.md) · Module 15 — [Déploiement et DevOps](../../15-deploiement-devops/README.md) · Module 17 — [Développer avec l'IA](../../17-developpement-ia/README.md) 🤖

---

**Juin 2026** · .NET 10 LTS · VB.NET 16.9 (stabilisé) · Visual Studio 2026

⏭️ [Versions .NET et cycle de support](/annexes/versions-reference/README.md)
