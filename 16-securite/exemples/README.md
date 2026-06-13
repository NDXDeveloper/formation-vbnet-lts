# 🔒 Exemples du module 16 — Sécurité

Le fil rouge du module : **en matière de sécurité, VB.NET n'a aucun désavantage**. Les défenses
reposent sur la **plateforme .NET** et les **SDK** (ASP.NET Core, `System.Security.Cryptography`,
MSAL, audit NuGet) — consommés **à l'identique** en VB et en C#. On les **utilise**, on ne les
réécrit pas. La vigilance « langage » se réduit à quelques pièges propres à VB (`&`, liaison
tardive, `My.Settings`) — et à deux limites du compilateur rencontrées ici (voir plus bas).

Chaque section porteuse de code est reconstruite en **projet complet, compilé et exécuté/testé**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 · SDK .NET **10.0.301** ·
Windows 11 (culture machine fr-FR).

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Vérifié |
|---|---|---|
| **16.1** Authentification / autorisation | [`16.1-autorisation-api`](#161-autorisation-api) · [`16.1-client-msal`](#161-client-msal) | **6/6** tests in-process (401/200/403/204/403/claims) ; client MSAL **compilé** |
| **16.2** Cryptographie | [`16.2-cryptographie`](#162-cryptographie) | RNG · SHA-256 · PBKDF2 · AES-GCM (+altération) · RSA · DPAPI — **toutes sorties vérifiées** |
| **16.3** OWASP Top 10 | [`16.3-owasp`](#163-owasp) | injection **contournée vs bloquée** · validation 0/3 · encodage HTML/URL/JS |
| **16.4** Dépendances & vulnérabilités | [`16.4-dependances`](#164-dependances) | **NU1903** (avertissement → erreur) · `list --vulnerable` |
| **16.5** Checklist de sécurité | *(pas de projet)* | grille de relecture — **synthèse documentée ci-dessous** |

---

## ▶️ Comment compiler et lancer

```powershell
# 16.1 — Web API + autorisation : tests d'intégration in-process
dotnet test 16.1-autorisation-api/tests/AutorisationApi.Tests   # Réussi! 6/6
dotnet build 16.1-client-msal -c Release                        # compile (non exécuté : Entra ID requis)

# 16.2 / 16.3 — consoles vérifiables
dotnet run --project 16.2-cryptographie -c Release
dotnet run --project 16.3-owasp          -c Release

# 16.4 — audit NuGet
dotnet build 16.4-dependances -c Release                        # observe l'avertissement NU1903
dotnet build 16.4-dependances -c Release -p:WarningsAsErrors=NU1903   # NU1903 en ERREUR (build échoue)
dotnet list  16.4-dependances package --vulnerable --include-transitive
```

Toutes les valeurs ci-dessous ont été **observées à l'exécution**.

---

## 16.1-autorisation-api

- **Section** : 16.1 · **Fichier** : `01-auth.md`
- **Description** : Web API VB protégée par **JWT Bearer** (validation intégrale : signature,
  émetteur, audience, durée de vie) et deux **politiques d'autorisation** — une politique de
  **scope** (assertion sur le claim `scp`, liste séparée par des **espaces**) et une politique de
  **rôle**. Le contrôleur expose `GET /commandes` (`<Authorize(Policy)>`),
  `DELETE /commandes/{id}` (`<Authorize(Roles)>`) et `GET /commandes/moi` (lecture des claims).
  Pour rester **vérifiable hors ligne**, la signature emploie une **clé symétrique HS256** de démo
  partagée ; le harnais de test **forge** ses propres jetons. En production, c'est un **IdP**
  (Entra ID) qui signe — le modèle d'autorisation est identique.
- **Sortie attendue** (vérifiée — `dotnet test`) :
  ```text
  Réussi!  - échec : 0, réussite : 6, total : 6   (AutorisationApi.Tests.dll, net10.0)
  ```
  | Cas | Jeton | Attendu |
  |---|---|---|
  | Sans jeton → `GET /commandes` | — | **401** Unauthorized |
  | Scope présent | `scp = "Profil.Read Commandes.Read"` | **200** + `CMD-1` |
  | Scope absent | `scp = "Profil.Read"` | **403** Forbidden |
  | Admin → `DELETE /commandes/5` | `roles = ["Administrateur"]` | **204** NoContent |
  | Non-admin → `DELETE` | `roles = ["Utilisateur"]` | **403** Forbidden |
  | `GET /commandes/moi` | `sub=alice, scp, roles` | **200** + `alice` / `Commandes.Read` / `Administrateur` |
- **Comportement vérifié** : l'autorisation est appliquée **côté serveur** ; **401** distingue
  *non authentifié* de **403** *authentifié mais non autorisé*. `MapInboundClaims = False` conserve
  les claims bruts (`scp`, `roles`, `sub`) au lieu de les remapper.
- **Note VB** : `Program` est déclarée **`Public Class`** (et non `Module`) pour que
  `WebApplicationFactory(Of Program)` puisse l'instancier depuis le projet de test.

## 16.1-client-msal

- **Section** : 16.1 · **Fichier** : `01-auth.md`
- **Description** : client public **MSAL.NET** (`Microsoft.Identity.Client`) acquérant un jeton
  d'accès : essai **silencieux** (cache) puis bascule **interactive**. **Compilé** (la sortie
  imprime son rôle) ; **non exécuté en réel** : l'acquisition exige une inscription d'application
  dans **Entra ID** et un navigateur.
- **Sortie attendue** (vérifiée) :
  ```text
  === 16.1 Client MSAL.NET (compilation seule) ===
  ServiceAuthentification acquiert un jeton : silencieux (cache) puis interactif.
  …
  ```
- **⚠️ Piège VB notable — `BC36943`** : **`Await` est interdit dans un bloc `Catch`**. On ne peut
  donc pas appeler `AcquireTokenInteractive(...)` directement dans le `Catch(MsalUiRequiredException)`.
  Contournement : on lève un **drapeau** (`interactionRequise`) dans le `Catch`, et on relance
  l'acquisition interactive **après** le `Try` (où `Await` redevient autorisé). En C#, on `await`
  directement dans le `catch` — d'où ce motif spécifique à VB.

## 16.2-cryptographie

- **Section** : 16.2 · **Fichier** : `02-cryptographie.md`
- **Description** : console exerçant les primitives de `System.Security.Cryptography` :
  **(1)** aléa sûr (`RandomNumberGenerator`), **(2)** empreinte d'intégrité **SHA-256**,
  **(3)** mot de passe **PBKDF2** salé (600 000 itérations) + vérification à **temps constant**
  (`CryptographicOperations.FixedTimeEquals`), **(4)** **AES-GCM** (round-trip puis **détection
  d'altération**), **(5)** signature **RSA**, **(6)** **DPAPI** (`ProtectedData`) pour un secret
  local de poste.
- **Sortie attendue** (vérifiée) :
  ```text
  [1 RNG]    clé de 32 octets ; dé = 4 (1<=x<=6 : True)
  [2 SHA256] 32 octets ; début = 2E5ADFA5CC3BA3FF…
  [3 PBKDF2] bon mot de passe = True ; mauvais = False
  [4 AESGCM] round-trip OK = True
  [4 AESGCM] altération détectée = True
  [5 RSA]    signature valide = True
  [6 DPAPI]  round-trip OK = True ; chiffré = 246 octets (clair = 18)
  ```
- **Comportement vérifié** : le bon mot de passe est accepté, le mauvais rejeté ; AES-GCM **refuse**
  un message falsifié d'un seul octet (`CryptographicException`) ; RSA vérifie sa propre signature ;
  DPAPI restitue le secret protégé.
- **Notes VB** :
  - **`BC30980`** — la variable RSA s'appelle **`algo`** : `rsa` entrerait en **collision**
    (insensible à la casse) avec le type `RSA`.
  - **DPAPI** exige `System.Security.Cryptography.ProtectedData` (NuGet) et est **Windows uniquement**
    → cible `net10.0-windows`.

## 16.3-owasp

- **Section** : 16.3 · **Fichier** : `03-owasp.md`
- **Description** : trois défenses du Top 10 sur une base **SQLite en mémoire** : **(1)** injection
  SQL — requête **concaténée** (vulnérable) vs **paramétrée** (sûre) ; **(2)** **validation**
  d'entrée par `DataAnnotations` (liste blanche, `Validator.TryValidateObject`) ; **(3)** **encodage**
  de sortie **contextuel** (`HtmlEncoder` / `UrlEncoder` / `JavaScriptEncoder`).
- **Sortie attendue** (vérifiée) :
  ```text
  --- (1) Injection SQL ---
    Légitime alice/S3cret!  → vulnérable=True, paramétré=True
    Attaque  alice/' OR '1'='1  → vulnérable=True (CONTOURNÉ !), paramétré=False (bloqué)
  --- (2) Validation d'entrée (liste blanche) ---
    DTO valide   → 0 erreur(s)
    DTO invalide → 3 erreur(s) :  (nom 3-20 / courriel invalide / âge 18-120)
  --- (3) Encodage de sortie contextuel ---
    HTML       → &lt;script&gt;alert(&#x27;xss&#x27;)&lt;/script&gt;
    URL        → %3Cscript%3Ealert(%27xss%27)%3C%2Fscript%3E
    JavaScript → <script>alert('xss')</script>
  ```
- **Comportement vérifié** : la requête **concaténée** est **contournée** par `' OR '1'='1`
  (authentification sans mot de passe !) ; la requête **paramétrée résiste** (l'entrée n'est jamais
  interprétée comme du SQL). La validation serveur rejette les 3 champs fautifs. L'encodage neutralise
  la charge XSS selon le contexte. *(La défense est identique sur SQL Server via
  `Microsoft.Data.SqlClient`.)*
- **Note VB** : un **littéral de chaîne ne s'imbrique pas** dans un trou d'interpolation
  (`$"… {f(""x"")} …"` ne **compile pas**) ; on **sort** l'expression dans une variable. Côté
  sécurité, ne jamais bâtir une requête avec l'opérateur **`&`** sur une entrée utilisateur.

## 16.4-dependances

- **Section** : 16.4 · **Fichier** : `04-dependances-vulnerabilites.md`
- **Description** : déclenche l'**audit NuGet**. Le projet référence **`Newtonsoft.Json 12.0.3`**,
  version à **vulnérabilité connue** (corrigée en 13.0.1). L'audit couvre le **graphe complet**
  (`NuGetAuditMode=all`). En production, on **promeut** `NU1903`/`NU1904` en **erreurs**.
- **Sortie attendue** (vérifiée) :
  ```text
  warning NU1903: Le package 'Newtonsoft.Json' 12.0.3 présente une vulnérabilité de gravité
                  élevé(e) connue, https://github.com/advisories/GHSA-5crp-9r3c-p9vr.

  # avec -p:WarningsAsErrors=NU1903  →  error NU1903 …  (build échoue, exit ≠ 0)

  # dotnet list package --vulnerable --include-transitive :
  > Newtonsoft.Json   12.0.3   12.0.3   High   https://github.com/advisories/GHSA-5crp-9r3c-p9vr
  ```
- **Comportement vérifié** : par défaut l'audit **avertit** (build réussit, le programme s'exécute) ;
  promu en erreur, le build **échoue** (effet *quality gate* en CI/CD). `dotnet list … --vulnerable`
  confirme la sévérité **High**. **Correctif réel** : monter à `13.0.1+` (ou préférer
  `System.Text.Json` du runtime).
- **Note SAST** : côté analyse statique VB, **CodeQL n'est pas disponible** ; on s'appuie sur les
  **analyseurs Roslyn** (règles `CAxxxx` de sécurité) et **SonarQube** / **Security Code Scan**.

---

## 16.5 — Checklist de sécurité *(pas de projet)*

`05-checklist.md` est une **grille de relecture** synthétique (sans code) à dérouler avant chaque
mise en production. Les **non-négociables** :

- **Tout en HTTPS** ; **aucun secret** versionné (Key Vault en prod, *User Secrets* en dev).
- **SQL toujours paramétré** ; **mots de passe** hachés lents et salés (PBKDF2/Argon2id).
- **Jetons validés intégralement** (signature, `iss`, `aud`, `exp`).
- **Toute entrée validée côté serveur** (la validation client n'est pas une sécurité).

Et quatre principes directeurs : **toute entrée est hostile**, **moindre privilège**, **défense en
profondeur**, **la sécurité est un processus continu**. Chaque rubrique du cours renvoie aux sections
16.1–16.4 (et aux modules 12 et 15 pour journalisation/erreurs et CI/CD/signature).

> Ce que les exemples de ce dépôt **n'exécutent pas en réel** (et pourquoi) : l'acquisition de jeton
> **MSAL** (inscription Entra ID + navigateur) et l'accès à **Azure Key Vault** (compte Azure +
> identité managée). Le code correspondant **compile** et suit les motifs officiels — seul l'appel
> *live* exige l'infrastructure cloud.

---

## 🧹 Nettoyage des binaires et résidus

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj | Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 · Windows 11 (fr-FR)
