🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe D — Raccourcis et astuces Visual Studio 2026 🆕

Raccourcis essentiels pour VB.NET, *snippets* personnalisés, extensions recommandées et extensions IA.

Visual Studio 2026 (v18) est disponible en version générale depuis le **11 novembre 2025** (.NET Conf 2025).
Microsoft le présente comme le **premier « Intelligent Developer Environment »** : GitHub Copilot y est intégré nativement, gratuit, et n'est plus un module de barre latérale, avec
prise en charge de **.NET 10 et C# 14**, une UI repensée et de gros gains de performance. Pour un développeur
VB.NET, l'essentiel à retenir d'emblée : la compatibilité ascendante avec les projets et extensions de Visual Studio 2022 est totale,
et l'IDE est désormais découplé des compilateurs .NET et C++ — on peut mettre à jour Visual Studio sans casser sa chaîne d'outils.

Beaucoup de raccourcis ci-dessous sont **stables depuis plusieurs versions** (profil clavier « Visual Studio »
par défaut) ; cette annexe met en avant ce qui est **spécifique à VB.NET** (⭐) et signale honnêtement
l'**outillage IA orienté C#** (⚠️), conformément au parti pris de la formation.

---

## D.1 — Repères Visual Studio 2026 utiles au quotidien

Les nouveautés VS 2026 qui changent réellement le quotidien d'un projet VB.NET :

- **Copilot intégré, gratuit** (compte GitHub), unifiant complétions et chat → voir [§ D.5](#d5--extensions-et-fonctionnalités-ia-).
- **Nouvelle UX** : onze nouveaux thèmes teintés et une marge d'éditeur basse personnalisable pour réduire la fatigue visuelle ; mode sombre soigné.
- **Format de solution SLNX** : un format de solution XML qui simplifie la gestion des grandes solutions, avec une analyse plus rapide, tout en restant interopérable avec les versions antérieures de l'IDE.
- **IDE découplé des compilateurs** : mises à jour d'UI et d'IA sans toucher au *toolchain*.
- **Assistant d'installation** : l'installation côte à côte est possible et l'assistant importe automatiquement thèmes, raccourcis, préférences et extensions depuis VS 2022.
- **Performance** : jusqu'à 30 % de temps de génération en moins sur matériel comparable (selon Microsoft).
- **Agents IA** : un Profiler Agent qui met en évidence les goulets d'étranglement et propose des optimisations, et un Debugger Agent qui analyse et corrige automatiquement les tests unitaires en échec.

---

## D.2 — Raccourcis clavier essentiels

> Profil clavier **« Visual Studio »** (par défaut). Vérifiez/personnalisez via **Outils > Options > Environnement > Clavier**. Les raccourcis en deux temps se notent `Ctrl+K, Ctrl+C` (presser la 1ʳᵉ combinaison, puis la 2ᵈᵉ).

### Édition

| Raccourci | Action |
|-----------|--------|
| `Ctrl+K, Ctrl+C` / `Ctrl+K, Ctrl+U` | Commenter / décommenter la sélection |
| `Ctrl+K, Ctrl+D` / `Ctrl+K, Ctrl+F` | Formater le document / la sélection |
| `Ctrl+D` | Dupliquer la ligne |
| `Alt+↑` / `Alt+↓` | Déplacer la ligne vers le haut / le bas |
| `Ctrl+X` (sans sélection) | Couper la ligne entière |
| `Ctrl+Espace` | Déclencher / compléter IntelliSense |
| `Ctrl+Shift+Espace` | Info-bulle des paramètres |
| `Ctrl+K, Ctrl+X` | Insérer un *snippet* |
| `Ctrl+K, Ctrl+S` | Entourer de… (*Surround With*) |

### Navigation

| Raccourci | Action |
|-----------|--------|
| `F12` | Atteindre la définition |
| `Ctrl+F12` | Atteindre l'implémentation |
| `Alt+F12` | Aperçu de la définition (*Peek*) |
| `Shift+F12` | Rechercher toutes les références |
| `Ctrl+T` | Atteindre tout (types, membres, fichiers) |
| `Ctrl+G` | Atteindre la ligne |
| `Ctrl+-` / `Ctrl+Shift+-` | Naviguer en arrière / en avant |
| `Ctrl+M, Ctrl+M` / `Ctrl+M, Ctrl+O` | Réduire-développer / tout réduire |

### Refactorisation

| Raccourci | Action |
|-----------|--------|
| `Ctrl+.` | **Actions rapides et refactorisations** (la plus utile) |
| `Ctrl+R, Ctrl+R` | Renommer |
| `Ctrl+R, Ctrl+M` | Extraire une méthode |

### Build, exécution et débogage

| Raccourci | Action |
|-----------|--------|
| `Ctrl+Shift+B` | Générer la solution |
| `F5` / `Ctrl+F5` | Démarrer avec / sans débogage |
| `Shift+F5` / `Ctrl+Shift+F5` | Arrêter / redémarrer le débogage |
| `F9` | Basculer un point d'arrêt |
| `F10` / `F11` / `Shift+F11` | Pas à pas principal / détaillé / sortant |

> 💡 **Hot Reload** (module 12.2) : appliquez les modifications de code sans redémarrer l'application, via le
> bouton dédié de la barre de débogage — précieux pour ajuster une UI WinForms/WPF en cours d'exécution.

### Recherche et IDE

| Raccourci | Action |
|-----------|--------|
| `Ctrl+F` / `Ctrl+H` | Rechercher / remplacer (fichier courant) |
| `Ctrl+Shift+F` / `Ctrl+Shift+H` | Rechercher / remplacer dans la solution |
| `Ctrl+Q` | Recherche de fonctionnalités (*Feature Search*) |
| `Ctrl+Alt+L` | Explorateur de solutions |
| `F4` | Fenêtre Propriétés |
| `Ctrl+Alt+X` | Boîte à outils (utile pour le Concepteur WinForms) |

---

## D.3 — *Snippets* de code

### Utiliser les extraits intégrés

Visual Studio fournit une bibliothèque d'extraits VB.NET organisée par catégories. On les insère via le menu
**Insérer un extrait** (`Ctrl+K, Ctrl+X`), ou — pour les extraits dotés d'un raccourci — en **tapant le raccourci
puis `Tab`**. Pour envelopper du code existant : **Entourer de…** (`Ctrl+K, Ctrl+S`).

> ⚠️ L'expérience d'**expansion en ligne** (taper un mot-clé et obtenir un bloc) est plus développée côté **C#**
> que côté VB. En VB, le **menu Insérer un extrait** reste la voie la plus fiable.

### Créer des *snippets* personnalisés

Un *snippet* est un fichier **`.snippet`** (XML). Pour VB.NET, l'attribut clé est `Language="VB"`. Les
placeholders se notent `$nom$`, `$end$` positionne le curseur final et `$selected$` sert aux extraits *Surround
With*.

```xml
<?xml version="1.0" encoding="utf-8"?>
<CodeSnippets xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
  <CodeSnippet Format="1.0.0">
    <Header>
      <Title>Propriété notifiée (MVVM)</Title>
      <Shortcut>propnotify</Shortcut>
      <Description>Propriété appelant SetProperty (INotifyPropertyChanged)</Description>
      <SnippetTypes>
        <SnippetType>Expansion</SnippetType>
      </SnippetTypes>
    </Header>
    <Snippet>
      <Declarations>
        <Literal>
          <ID>type</ID>
          <ToolTip>Type de la propriété</ToolTip>
          <Default>String</Default>
        </Literal>
        <Literal>
          <ID>name</ID>
          <ToolTip>Nom de la propriété</ToolTip>
          <Default>Valeur</Default>
        </Literal>
      </Declarations>
      <Code Language="VB"><![CDATA[Private _$name$ As $type$
Public Property $name$ As $type$
    Get
        Return _$name$
    End Get
    Set(value As $type$)
        SetProperty(_$name$, value)
    End Set
End Property$end$]]></Code>
    </Snippet>
  </CodeSnippet>
</CodeSnippets>
```

**Enregistrer l'extrait** : ouvrez le **Gestionnaire d'extraits de code** (`Ctrl+K, Ctrl+B`), choisissez le
langage **Basic**, puis **Importer** votre `.snippet` (ou ajoutez son dossier). Les extraits personnels se placent
typiquement dans `Documents\Visual Studio 2026\Code Snippets\Visual Basic\My Code Snippets\`.

> 💡 Les *snippets* personnalisés sont idéaux pour matérialiser vos **conventions d'équipe** ([Annexe C](../bonnes-pratiques/README.md)) :
> en-tête de fichier, bloc `Try/Catch` avec journalisation structurée, squelette de classe documentée…

---

## D.4 — Extensions recommandées

Le **modèle d'extensions est conservé** et la quasi-totalité des milliers d'extensions existantes fonctionne dès le premier jour
(certaines peuvent demander une mise à jour). Avant d'installer, vérifiez la **compatibilité VS 2026** sur le
*Marketplace*.

**Déjà intégré (rien à installer) :**

- **GitHub Copilot** → voir [§ D.5](#d5--extensions-et-fonctionnalités-ia-).
- **Prise en charge `.editorconfig`** : centralisez le style (indentation, fins de ligne, conventions) à la
  racine de la solution pour l'imposer à toute l'équipe — complément naturel de l'[Annexe C](../bonnes-pratiques/README.md).
- **Intégration Git / GitHub** enrichie (changement de branche, *pull requests*, revue de code dans l'IDE).

**Réellement pertinent pour VB.NET :**

- **SonarQube for IDE (ex-SonarLint)** : analyse statique avec règles **VB.NET** — branche locale de la qualité
  pilotée en CI/CD (module 13.4).
- **ReSharper** (JetBrains, payant) : assistance lourde prenant en charge VB ; à évaluer selon le budget, en
  gardant à l'esprit que son outillage reste historiquement plus riche en C#.

> ⚠️ **Note d'honnêteté.** Une partie de l'écosystème d'analyseurs/extensions est **orientée C#** : plusieurs
> analyseurs Roslyn populaires (StyleCop, Roslynator) ciblent C# et n'apportent rien — ou peu — à un projet VB.
> Privilégiez les outils qui **listent explicitement VB.NET** dans leurs langages pris en charge.

---

## D.5 — Extensions et fonctionnalités IA 🤖

C'est la partie décisive pour VB.NET — et là où la formation insiste : l'IA est **indispensable**, mais son
rapport à VB est **nuancé**. À lire avec l'[Annexe C § C.6](../bonnes-pratiques/README.md) (règles d'usage de
l'IA) et le module 17.

### GitHub Copilot, intégré nativement

VS 2026 fait de Copilot un composant de l'IDE plutôt qu'un greffon. Distinction utile : IntelliSense se concentre sur la complétion syntaxique, tandis que Copilot va plus loin en comprenant l'intention et en générant du code porteur de logique.

**Le point VB crucial ⭐ (et honnête).** Visual Studio prend en charge de nombreux langages — C#, VB.NET, C++, Python — et Copilot s'y applique.
Mais — c'est exactement la thèse de la formation — Copilot est entraîné sur tous les langages présents dans les dépôts publics, et la qualité des suggestions dépend, pour chaque langage, du volume et de la diversité des données d'entraînement.
VB.NET étant nettement moins représenté que C#, attendez-vous à des **suggestions de moindre qualité** et à des
**« C#-ismes »** glissés dans le code VB. → D'où la **règle de validation systématique** ([Annexe C § C.6](../bonnes-pratiques/README.md)),
le recoupement avec les [Annexes A](../correspondance-vbnet-csharp/README.md) et [B](../frontiere-vbnet-csharp/README.md),
et les prompts qui précisent toujours « **VB.NET / .NET 10** ».

### Fonctionnalités IA de VS 2026 utiles en VB

- **Adaptive Paste (« Paste & Fix »)** : au collage de code venu du web ou d'un autre projet, Copilot le réécrit pour respecter les conventions de nommage, le style de formatage et l'architecture du projet.
  ⚠️ C'est une **adaptation de style**, **pas** une garantie de conversion C# → VB : pour convertir du C#, demandez
  explicitement la transposition dans Copilot Chat, **puis validez**.
- **Suggestions contextuelles** : Copilot prend en compte le contexte de toute la solution plutôt que des extraits génériques.
- **Debugger Agent** : analyse et **corrige les tests unitaires en échec** (utile avec le module 13).
- **Profilage assisté** : une commande « Profile with Copilot » dans l'Explorateur de tests profile un test précis et en analyse les données CPU et d'instrumentation ; les
  *PerfTips* en débogage s'appuient sur le Profiler Agent pour suggérer des optimisations.
- **Compétences (skills) auto-découvertes** : les agents Copilot de Visual Studio découvrent et utilisent automatiquement les compétences définies dans le dépôt ou le profil utilisateur.

### `@modernize-dotnet` — un agent IA qui **supporte VB** ✅

Bonne nouvelle pour les parcours migration (modules 11 et 17.3) : l'agent **GitHub Copilot modernization**
prend en charge VB. L'agent supporte aussi bien C# que Visual Basic, pour les
montées de version .NET et la migration vers Azure, et il est inclus dans Visual Studio 2026 (ou Visual Studio 2022 version 17.14.17 et ultérieure),
également disponible dans VS Code, la CLI Copilot et sur GitHub.com. Il embarque plus de trente compétences de modernisation chargées automatiquement selon les technologies détectées dans le code, et enregistre vos décisions dans un fichier scenario-instructions.md pour les conserver entre les sessions.
On l'invoque depuis le chat Copilot via `@modernize-dotnet`.

### ⚠️ Ce qui reste orienté C#

Malgré ce qui précède, l'outillage IA **spécialisé** penche encore vers C#. Par exemple, la navigation
symbolique des agents : l'outil find_symbol du mode agent est pris en charge pour C++, C#, Razor, TypeScript et tout langage doté d'une extension LSP — VB n'y figure pas
explicitement. De même, les nouveaux agents C# et C++ introduits dans cette version n'ont
pas d'équivalent VB. À retenir : **Copilot général** et **`@modernize-dotnet`** couvrent VB ; les **agents et
outils de langage les plus récents** ciblent d'abord C#.

---

## D.6 — Aide-mémoire récapitulatif

| Besoin | Raccourci / Outil |
|--------|-------------------|
| Refactoriser / corriger sous le curseur | `Ctrl+.` (Actions rapides) |
| Renommer un symbole | `Ctrl+R, Ctrl+R` |
| Formater le document | `Ctrl+K, Ctrl+D` |
| Commenter / décommenter | `Ctrl+K, Ctrl+C` / `Ctrl+K, Ctrl+U` |
| Insérer / créer un *snippet* | `Ctrl+K, Ctrl+X` / Gestionnaire `Ctrl+K, Ctrl+B` |
| Atteindre tout | `Ctrl+T` |
| Débogage (démarrer / pas à pas) | `F5` / `F10` / `F11` |
| Recherche de fonctionnalités | `Ctrl+Q` |
| Code IA (générer / chat) | GitHub Copilot (intégré) |
| Migrer un projet (.NET / Azure) | Agent `@modernize-dotnet` ✅ |
| Style d'équipe imposé | `.editorconfig` (intégré) + *snippets* personnalisés |

---

### Voir aussi

- Module 1.4 — [Installation et outils](../../01-introduction-vbnet/04-installation-outils.md) 🆕 (⚠️ en VS Code, pas de C# Dev Kit pour VB.NET)
- Module 12.2 — [Débogage (points d'arrêt, Hot Reload, outils de VS 2026)](../../12-exceptions-debogage/02-debogage.md) 🆕
- Module 13.4 — [Analyse statique (analyseurs Roslyn, SonarQube, StyleCop)](../../13-tests-qualite/04-analyse-statique.md)
- Module 17 — [Développer en VB.NET avec l'IA](../../17-developpement-ia/README.md) 🤖
- [Annexe A — Correspondance syntaxique](../correspondance-vbnet-csharp/README.md) · [Annexe B — Frontière VB.NET / C#](../frontiere-vbnet-csharp/README.md) · [Annexe C — Bonnes pratiques](../bonnes-pratiques/README.md)

---

**Juin 2026** · Visual Studio 2026 (v18, GA 11 novembre 2025) · .NET 10 LTS · VB.NET 16.9 (stabilisé)

⏭️ [Guide de migration vers .NET 10 LTS](/annexes/migration-net10/README.md)
