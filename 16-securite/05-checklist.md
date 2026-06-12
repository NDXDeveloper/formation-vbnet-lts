🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 16.5 Checklist de sécurité VB.NET

Cette section rassemble, sous forme de **grille de relecture**, les pratiques vues tout au long du module. Elle se veut **actionnable** : à dérouler avant chaque mise en production, et à reprendre régulièrement — la sécurité étant un processus continu, pas un état atteint une fois pour toutes.

Chaque rubrique est rattachée à la (ou aux) catégorie(s) correspondante(s) du **Top 10:2025 de l'OWASP** ([§16.3](03-owasp.md)) et renvoie à la section du module qui la détaille. La liste n'est pas exhaustive, mais elle couvre l'essentiel d'une application VB.NET / .NET 10 bien défendue.

## ⛔ Les non-négociables

Ces points ne souffrent **aucune exception**. S'il n'en restait que six :

- [ ] **Tout le trafic est en HTTPS** — aucune donnée sensible ne circule en clair.
- [ ] **Aucun secret dans le code ni dans un fichier versionné** — coffre (Key Vault) en production, *User Secrets* en développement.
- [ ] **Toutes les requêtes SQL sont paramétrées** — jamais de concaténation d'entrée utilisateur.
- [ ] **Les mots de passe sont hachés avec une fonction lente et salée** (PBKDF2 / Argon2id), jamais avec un hachage rapide.
- [ ] **Les jetons sont validés intégralement** (signature, émetteur, audience, expiration).
- [ ] **Toute entrée est validée côté serveur** — la validation client n'est pas une sécurité.

## 1. Authentification et autorisation — *A01 / A07* → [§16.1](01-auth.md)

- [ ] L'authentification s'appuie sur des standards éprouvés (OAuth 2.0 / OIDC), jamais sur un mécanisme maison.
- [ ] Les jetons JWT sont validés intégralement : signature, `iss`, `aud`, `exp`.
- [ ] Les jetons d'accès ont une **durée de vie courte**, complétée par des jetons de rafraîchissement.
- [ ] Les droits sont vérifiés **côté serveur** à chaque action sensible (jamais en se fiant à l'interface).
- [ ] L'autorisation suit le **principe du moindre privilège** : périmètres (*scopes*) et rôles strictement nécessaires.
- [ ] Les règles d'accès complexes passent par des **politiques** nommées (*policy-based*) plutôt que par des rôles épars.
- [ ] Aucune donnée sensible n'est placée dans la charge utile d'un JWT (signé mais **non chiffré**).

## 2. Cryptographie et gestion des secrets — *A04* → [§16.2](02-cryptographie.md)

- [ ] L'aléa de sécurité (clés, sels, nonces, jetons) vient de `RandomNumberGenerator`, **jamais** de `System.Random`.
- [ ] Les mots de passe utilisent PBKDF2 (`Rfc2898DeriveBytes.Pbkdf2`) salé — ou Argon2id — avec un nombre d'itérations conforme aux recommandations OWASP **courantes**.
- [ ] Les comparaisons de secrets utilisent `CryptographicOperations.FixedTimeEquals` (temps constant).
- [ ] Le chiffrement symétrique est **authentifié** (AES-GCM) ; le **nonce est unique** à chaque opération.
- [ ] Aucun algorithme cryptographique « maison » : uniquement les primitives éprouvées de la BCL.
- [ ] Les secrets vivent dans un **coffre** (Azure Key Vault) en production, jamais dans `appsettings.json` versionné.
- [ ] L'accès au coffre se fait par **identité managée** (`DefaultAzureCredential`) — objectif : zéro secret dans l'application.
- [ ] Tout secret exposé (même brièvement commité) est considéré comme compromis et **renouvelé**.

## 3. Injection, validation et encodage — *A05* → [§16.3](03-owasp.md)

- [ ] Toutes les requêtes SQL sont **paramétrées** (`SqlCommand.Parameters`), avec types explicites (`Add` plutôt que `AddWithValue` en production).
- [ ] En EF Core, le SQL brut utilise `FromSqlInterpolated` (paramétré), **jamais** `FromSqlRaw` concaténé.
- [ ] Aucune entrée non fiable n'est concaténée dans une commande système, un chemin de fichier ou une requête LDAP.
- [ ] Toute entrée est validée **côté serveur**, en **liste blanche** (ce qui est autorisé), au plus près du point d'entrée.
- [ ] Les contraintes de modèle (`DataAnnotations`) sont posées, et la validation `<ApiController>` est effective (réponse `400` automatique).
- [ ] La sortie est **encodée selon son contexte** (HTML, URL, JavaScript) partout où l'application produit du contenu interprétable (WebView2, HTML généré, courriels).

## 4. Configuration et gestion des erreurs — *A02 / A10* → [module 12](../12-exceptions-debogage/README.md), [module 15](../15-deploiement-devops/README.md)

- [ ] **Aucune trace d'exception détaillée** n'est exposée en production : réponses `Problem Details` génériques (voir [module 8 (§8.2)](../08-services-web/02-web-api-controllers.md)).
- [ ] Les fonctions de débogage et les pages d'erreur détaillées sont **désactivées hors développement**.
- [ ] La configuration sensible est externalisée (variables d'environnement, coffre), pas codée en dur.
- [ ] Les exceptions sont gérées proprement : pas de fuite d'information, pas d'état incohérent laissé derrière.
- [ ] Les valeurs par défaut sont sûres (principe du *secure by default*).

## 5. Dépendances et chaîne d'approvisionnement — *A03* → [§16.4](04-dependances-vulnerabilites.md)

- [ ] L'**audit NuGet** est actif et couvre le graphe complet (`NuGetAuditMode=all`, `--include-transitive`).
- [ ] Le build **échoue** sur les vulnérabilités élevées et critiques (`NU1903`, `NU1904` promues en erreurs).
- [ ] Une veille automatisée est en place (Dependabot ou Renovate).
- [ ] Les paquets dépréciés ou abandonnés sont identifiés (`dotnet list package --deprecated`) et remplacés.
- [ ] La chaîne est durcie : gestion centralisée des paquets, fichiers de verrouillage, mappage des sources (anti-confusion), signatures.
- [ ] L'**analyse statique** (SAST) est activée : analyseurs Roslyn + **SonarQube** côté VB.NET (CodeQL n'étant pas disponible).
- [ ] Une analyse dynamique (DAST, par ex. OWASP ZAP) est exécutée contre un environnement déployé pour les Web API.

## 6. Journalisation et supervision — *A09* → [module 12](../12-exceptions-debogage/README.md)

- [ ] Les événements de sécurité (connexions, échecs d'autorisation, accès sensibles) sont **journalisés**.
- [ ] Les journaux **ne contiennent jamais** de donnée sensible : pas de mot de passe, de jeton ni de donnée personnelle non nécessaire (la **minimisation** est aussi une exigence du RGPD).
- [ ] Des alertes existent sur les comportements anormaux (échecs d'authentification répétés, par ex.).

## 7. Transport et réseau

- [ ] HTTPS est **forcé** (redirection HTTP → HTTPS) et HSTS (*HTTP Strict Transport Security*) est activé pour les services web.
- [ ] Une version moderne de TLS est imposée.
- [ ] Les en-têtes de sécurité pertinents sont positionnés (`Content-Security-Policy`, `X-Content-Type-Options`, etc.) pour les API/contenus web.

## 8. Spécificités VB.NET ⚠️

- [ ] **`Option Strict On`** est actif partout, **sauf** là où la liaison tardive COM/Office l'exige — et ces zones sont **isolées**.
- [ ] L'opérateur de concaténation **`&`** n'est jamais utilisé pour construire une requête SQL, du HTML ou un chemin à partir d'une entrée utilisateur.
- [ ] La liaison tardive (`Option Strict Off`) est cantonnée au strict nécessaire (interop), jamais généralisée.
- [ ] Aucun secret n'est stocké dans **`My.Settings`** ni dans un fichier `*.config` : ces valeurs sont persistées **en clair** — pour un secret local de poste, on passe par la **DPAPI** (`ProtectedData`, voir [§16.2](02-cryptographie.md)).

## 9. Processus et CI/CD — *A06 / A08* → [module 15 (§15.3)](../15-deploiement-devops/README.md)

- [ ] Les vérifications de sécurité sont **automatisées dans la CI/CD** (audit, SAST, tests), exécutées **à chaque poussée** (*shift-left*).
- [ ] Une **barrière de qualité** (*quality gate*) bloque la fusion en cas de détection de sévérité élevée.
- [ ] Les correctifs de sécurité des dépendances sont appliqués sans délai.
- [ ] Les évolutions sensibles font l'objet d'une **modélisation des menaces** en amont — la sécurité se conçoit, elle ne se rajoute pas (*A06*).
- [ ] Les livrables et leurs mises à jour sont **signés** (MSIX, ClickOnce — voir [module 15](../15-deploiement-devops/README.md)) : l'utilisateur n'installe que ce qui est vérifiable (*A08*).
- [ ] Le code **généré par l'IA** est relu avec la même exigence que le code humain : il peut introduire injections, secrets en dur ou cryptographie faible (voir [module 17](../17-developpement-ia/README.md)).
- [ ] La sécurité est revue régulièrement, pas une seule fois avant la livraison.

---

## En résumé : les principes directeurs du module

Au-delà de cette liste, quatre principes traversent tout ce qui précède et résument l'état d'esprit à conserver :

1. **Toute entrée est hostile** jusqu'à preuve du contraire — on valide, on paramètre, on encode.
2. **Le moindre privilège** partout — chaque composant n'obtient que ce dont il a strictement besoin.
3. **La défense en profondeur** — aucune mesure n'est suffisante seule ; la sécurité naît de l'empilement cohérent de plusieurs couches.
4. **La sécurité est un processus continu** — elle se vérifie en permanence, à chaque commit et à chaque dépendance mise à jour, jamais une fois pour toutes.

Et une bonne nouvelle, fil rouge de ce module : en matière de sécurité, **VB.NET n'a aucun désavantage**. Les défenses reposent sur la plateforme .NET et les SDK Azure, consommés à l'identique en VB.NET et en C#. On les **utilise**, on ne les réécrit pas — et l'on réserve la vigilance « langage » aux quelques pièges propres à VB (`&`, liaison tardive, `My.Settings`).

La sécurité ne s'arrête pas à ce module : transversale par nature, elle se prolonge notamment dans le **déploiement** ([module 15](../15-deploiement-devops/README.md)) et l'**exploitation**. Mais vous disposez désormais des fondations pour concevoir, coder et livrer des applications VB.NET **dignes de confiance**.

**↩ Retour au [sommaire du module 16](README.md)**

⏭️ [Développer en VB.NET avec l'IA (l'ère Copilot)](/17-developpement-ia/README.md)
