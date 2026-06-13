<!-- ============================================================================
  Section 17.6 : Workflow IA-first — fichier d'instructions au niveau du dépôt
  Description : Artefact (non compilé) à placer à la racine d'un dépôt VB.NET sous
                .github/copilot-instructions.md. Reconnu par Visual Studio 2026 et
                VS Code, il oriente le CHAT, les AGENTS et la revue de code vers VB
                (mais PAS la complétion en ligne, qui ne lit pas ce fichier : pour
                elle, c'est le contexte du dépôt qui compte). À versionner et partager.
  Fichier source : 06-workflow-ia-first.md
============================================================================ -->

# Instructions du dépôt pour l'assistant IA

- Tout le code de ce dépôt est en **Visual Basic .NET** (jamais VB6, jamais C# sauf demande explicite).
- Cible : **.NET 10 (LTS)**. Langage **VB 16.9** (stabilisé, *consumption-only*). `Option Strict On` et `Option Explicit On`.
- N'utilise **aucune construction absente de VB** : pas de *record* déclaré, pas de *top-level statements*, pas de *switch expression*, pas de constructeur primaire, pas d'expression de collection `[…]`, pas de *raw string literals*.
- Idiomes attendus : `Handles`/`WithEvents` pour les événements, blocs `Using`, attributs **entre chevrons** (`<Fact>`), documentation avec **`'''`** (trois apostrophes, pas `///`), lambdas `Function`/`Sub` (pas la flèche `=>`).
- Surveille les pièges sémantiques de traduction depuis le C# : `^` est une **puissance** (XOR = `Xor`), division entière `\`, `%` → `Mod`, `&&`/`||` → `AndAlso`/`OrElse`, sensibilité à la casse, types nullables de **valeur**.
- Si une fonctionnalité demandée n'a **pas d'équivalent direct** en VB, **signale-le** au lieu de l'inventer.
- Code généré = code à **valider** : il ne passe qu'après compilation (`Option Strict On`), analyseurs et tests.
