🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 16.2 Cryptographie

Une fois l'identité établie ([16.1](01-auth.md)), il reste à protéger les **données elles-mêmes** : mots de passe, données personnelles, chaînes de connexion, fichiers sensibles — qu'ils transitent sur le réseau ou reposent en base. C'est le rôle de la cryptographie.

Comme pour l'authentification, ce domaine repose entièrement sur des **API de la plateforme** (`System.Security.Cryptography`) et des SDK Azure, consommés à l'identique depuis VB.NET ou C#. Il n'y a, ici encore, **rien à déléguer** : le code cryptographique que vous écrivez en VB.NET est aussi solide que son équivalent C#.

## La règle d'or : ne jamais inventer sa cryptographie

Avant toute chose, un principe non négociable :

> **On n'écrit jamais son propre algorithme de chiffrement, et on ne combine jamais des primitives « à sa façon ».**

La cryptographie est un domaine où l'intuition trompe et où la moindre erreur (un mode mal choisi, un nonce réutilisé, une comparaison sensible au temps) ruine la sécurité. La bonne pratique consiste à **utiliser les primitives éprouvées de la BCL**, telles qu'elles sont prévues, et à s'appuyer sur des recommandations à jour (OWASP, NIST). Tout le reste de cette section applique ce principe.

## Aléa cryptographique : `RandomNumberGenerator`, jamais `Random`

Clés, sels, nonces, jetons : tout ce qui doit être imprévisible se génère avec un **générateur cryptographiquement sûr**, jamais avec `System.Random` (prévisible, et donc dangereux dans un contexte de sécurité).

```vb
Imports System.Security.Cryptography

' ✅ CORRECT : aléa cryptographique
Dim cle = RandomNumberGenerator.GetBytes(32)        ' 32 octets pour une clé AES-256
Dim sel = RandomNumberGenerator.GetBytes(16)
Dim dé = RandomNumberGenerator.GetInt32(1, 7)        ' entier sûr dans [1, 6]

' ❌ À PROSCRIRE pour la sécurité
' Dim mauvais = New Random().Next()                  ' prévisible !
```

Retenez cette distinction une fois pour toutes : `System.Random` sert aux simulations et aux jeux, **jamais** à produire un secret.

## Le hachage

Le hachage transforme une donnée en une empreinte de taille fixe, **à sens unique** (on ne peut pas remonter à l'entrée). Mais attention : il existe **deux usages radicalement différents**, qui appellent des outils différents.

### Hachage d'intégrité (SHA-256)

Pour vérifier qu'une donnée n'a pas été altérée (intégrité d'un fichier, empreinte d'un contenu), on utilise un hachage **rapide** comme SHA-256 :

```vb
Imports System.Security.Cryptography
Imports System.Text

Dim donnees = Encoding.UTF8.GetBytes("contenu à vérifier")
Dim empreinte = SHA256.HashData(donnees)                 ' méthode « one-shot »
Dim empreinteHex = Convert.ToHexString(empreinte)
```

C'est exactement ce qu'il **ne faut pas** faire pour les mots de passe — et c'est la confusion la plus fréquente et la plus grave en sécurité applicative.

### Hachage de mot de passe — la distinction critique ⚠️

Un hachage rapide (SHA-256, MD5…) appliqué à un mot de passe est **cassable** : un attaquant peut tester des milliards de candidats par seconde. Pour les mots de passe, on utilise une **fonction de dérivation de clé lente et salée** (PBKDF2, scrypt, bcrypt, Argon2), conçue pour rendre chaque essai coûteux.

La BCL fournit **PBKDF2** via `Rfc2898DeriveBytes` :

```vb
Imports System.Security.Cryptography

Public Function HacherMotDePasse(motDePasse As String) As String
    ' Un sel aléatoire et UNIQUE par mot de passe.
    Dim sel = RandomNumberGenerator.GetBytes(16)

    Dim hash = Rfc2898DeriveBytes.Pbkdf2(
        password:=motDePasse,
        salt:=sel,
        iterations:=600_000,                       ' voir la note ci-dessous
        hashAlgorithm:=HashAlgorithmName.SHA256,
        outputLength:=32)

    ' On stocke le sel ET le hash (ici concaténés en Base64).
    Return $"{Convert.ToBase64String(sel)}.{Convert.ToBase64String(hash)}"
End Function

Public Function VerifierMotDePasse(motDePasse As String, valeurStockee As String) As Boolean
    Dim parties = valeurStockee.Split("."c)
    Dim sel = Convert.FromBase64String(parties(0))
    Dim hashAttendu = Convert.FromBase64String(parties(1))

    Dim hashCalcule = Rfc2898DeriveBytes.Pbkdf2(
        motDePasse, sel, 600_000, HashAlgorithmName.SHA256, hashAttendu.Length)

    ' Comparaison à temps constant : indispensable pour éviter
    ' les attaques par mesure du temps de réponse (timing attacks).
    Return CryptographicOperations.FixedTimeEquals(hashCalcule, hashAttendu)
End Function
```

Trois points de vigilance :

- **Le sel** doit être aléatoire et unique par mot de passe (il interdit les tables précalculées et empêche deux utilisateurs au même mot de passe d'avoir le même hash).
- **Le nombre d'itérations** doit suivre les **recommandations OWASP du moment**. Ce seuil augmente avec le temps (la valeur de 600 000 itérations correspond à une recommandation récente pour PBKDF2-HMAC-SHA256) : vérifiez la valeur courante plutôt que de figer la vôtre une fois pour toutes.
- **La comparaison** se fait avec `CryptographicOperations.FixedTimeEquals`, jamais avec `=` ou `SequenceEqual`, dont le temps d'exécution fuiterait de l'information.

En pratique, dans une application réelle, le plus sûr est de **ne pas réimplémenter cela soi-même** :

- dans une application ASP.NET Core, utilisez `PasswordHasher(Of TUser)` d'**ASP.NET Core Identity**, qui applique PBKDF2 salé avec des réglages sensés et gère le versionnage du format ;
- pour le standard le plus récent, **Argon2id** est recommandé, mais il n'est **pas dans la BCL** : il faut passer par un paquet NuGet dédié (consommation .NET classique, donc pleinement dans le périmètre VB.NET).

## Le chiffrement

Le chiffrement, contrairement au hachage, est **réversible** : on chiffre pour pouvoir déchiffrer ensuite. Deux grandes familles coexistent.

### Symétrique ou asymétrique ?

| | Clé | Usage typique |
|---|---|---|
| **Symétrique** (AES) | Une **même** clé secrète chiffre et déchiffre | Le chiffrement du gros des données — rapide. |
| **Asymétrique** (RSA, ECC) | Une **paire** : clé publique + clé privée | Échange de clé, signatures numériques — lent, sur de petites données. |

En pratique, on combine souvent les deux (**chiffrement hybride**) : l'asymétrique sert à transmettre de façon sûre une clé symétrique, puis le symétrique chiffre le volume de données. C'est, par exemple, le principe de TLS.

### Chiffrement symétrique authentifié : AES-GCM

Le piège classique du chiffrement symétrique est de garantir la **confidentialité** sans garantir l'**intégrité** : un attaquant peut alors altérer le message chiffré sans être détecté. La solution moderne est un mode de **chiffrement authentifié** (AEAD, *Authenticated Encryption with Associated Data*), et le plus simple en .NET est **AES-GCM**, qui produit une *étiquette d'authentification* (*tag*) détectant toute altération.

```vb
Imports System.Security.Cryptography

Public Function Chiffrer(clair As Byte(), cle As Byte()) As Byte()
    ' Le nonce doit être UNIQUE pour chaque chiffrement avec une même clé.
    Dim nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize)   ' 12 octets
    Dim chiffre(clair.Length - 1) As Byte
    Dim etiquette(AesGcm.TagByteSizes.MaxSize - 1) As Byte                       ' 16 octets

    Using aes As New AesGcm(cle, AesGcm.TagByteSizes.MaxSize)
        aes.Encrypt(nonce, clair, chiffre, etiquette)
    End Using

    ' On stocke ensemble : nonce + étiquette + données chiffrées.
    Return nonce.Concat(etiquette).Concat(chiffre).ToArray()
End Function

Public Function Dechiffrer(donnees As Byte(), cle As Byte()) As Byte()
    Dim tNonce = AesGcm.NonceByteSizes.MaxSize
    Dim tTag = AesGcm.TagByteSizes.MaxSize

    Dim nonce = donnees.Take(tNonce).ToArray()
    Dim etiquette = donnees.Skip(tNonce).Take(tTag).ToArray()
    Dim chiffre = donnees.Skip(tNonce + tTag).ToArray()
    Dim clair(chiffre.Length - 1) As Byte

    Using aes As New AesGcm(cle, tTag)
        ' Lève une CryptographicException si l'étiquette ne correspond pas :
        ' c'est précisément la garantie d'authenticité.
        aes.Decrypt(nonce, chiffre, etiquette, clair)
    End Using

    Return clair
End Function
```

> ⚠️ **Le nonce ne doit jamais être réutilisé** avec la même clé. Réutiliser un nonce en mode GCM compromet gravement la sécurité. On le génère donc aléatoirement (ou via un compteur garanti unique) à chaque opération, et on le stocke à côté du message chiffré (le nonce n'est pas secret).

Le mode CBC reste possible, mais il **n'authentifie pas** les données : il faut alors lui adjoindre un HMAC selon le schéma *encrypt-then-MAC*, plus délicat à mettre en œuvre correctement. **Préférez AES-GCM** quand vous le pouvez.

### Chiffrement asymétrique et signatures

Pour les signatures numériques (prouver l'origine et l'intégrité d'un message), on utilise RSA ou les courbes elliptiques (`ECDsa`) :

```vb
Imports System.Security.Cryptography
Imports System.Text

Using algo = RSA.Create(2048)
    Dim donnees = Encoding.UTF8.GetBytes("message à signer")

    Dim signature = algo.SignData(donnees,
        HashAlgorithmName.SHA256, RSASignaturePadding.Pss)

    Dim valide = algo.VerifyData(donnees, signature,
        HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
End Using
```

La variable s'appelle `algo`, et non `rsa` : VB.NET étant insensible à la casse, `Using rsa = RSA.Create(2048)` ferait entrer la variable en collision avec le type `RSA` (erreur `BC30980`) — c'est le piège déjà rencontré avec `Aes` au [module 7 (§7.6)](../07-acces-donnees/06-fichiers-io.md).

Pour le **chiffrement** asymétrique, RSA s'utilise avec un remplissage OAEP (`RSAEncryptionPadding.OaepSHA256`) — mais il ne chiffre que de **petites** données (typiquement une clé symétrique), d'où le recours au schéma hybride évoqué plus haut.

### Chiffrer de gros volumes : les flux

Pour chiffrer un fichier ou un flux volumineux, on combine un algorithme symétrique avec un `CryptoStream`, abordé au [module 7 (§7.6)](../07-acces-donnees/06-fichiers-io.md) avec les autres flux (compression, fichiers). Le principe reste le même : aléa sûr pour la clé/l'IV, et un mode authentifié dès que possible.

## La gestion des secrets

Disposer de bonnes primitives ne sert à rien si la **clé traîne dans le code source**. La gestion des secrets (chaînes de connexion, clés d'API, certificats) est aussi importante que le chiffrement lui-même.

### Le principe : aucun secret dans le code

> Un secret ne doit **jamais** figurer dans le code source, ni dans un `appsettings.json` versionné. Tout secret commité dans un dépôt doit être considéré comme **compromis** et **renouvelé** sans délai.

`appsettings.json` est réservé à la configuration **non sensible** (niveaux de journalisation, URL publiques, options fonctionnelles). Les secrets, eux, vivent ailleurs — selon l'environnement.

### En développement : les *User Secrets*

En local, .NET fournit le *Secret Manager*, qui stocke les secrets **en dehors du répertoire du projet** (dans le profil utilisateur), donc à l'abri du contrôle de version :

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:BaseDeDonnees" "Server=...;Database=...;"
```

Ces valeurs sont ensuite lues comme n'importe quelle configuration via `IConfiguration`, sans changer le code. À noter : les *User Secrets* ne sont **pas chiffrés** — ils sont simplement hors du dépôt, et destinés au **développement uniquement**.

### En production : variables d'environnement et coffre

En production, les secrets proviennent de **variables d'environnement** (injectées par l'hébergeur ou le conteneur) ou, mieux, d'un **coffre de secrets** dédié comme Azure Key Vault (section suivante).

### La hiérarchie de configuration

Le système de configuration de .NET **superpose** les sources, chacune surchargeant la précédente. Un ordre courant :

1. `appsettings.json` (valeurs par défaut, non sensibles) ;
2. `appsettings.{Environnement}.json` ;
3. *User Secrets* (en développement) ;
4. variables d'environnement ;
5. Azure Key Vault (en production).

Votre code lit toujours `IConfiguration` de la même façon : **la provenance du secret reste transparente**, ce qui permet de changer de source entre environnements sans toucher au code métier.

### Sur le poste de l'utilisateur : DPAPI pour les applications de bureau ⭐

Reste un cas que ni les *User Secrets* ni un coffre cloud ne couvrent : une application **Windows Forms ou WPF** qui doit retenir un secret **localement**, sur le poste de l'utilisateur — un jeton de rafraîchissement, une clé d'API, des identifiants de connexion mémorisés. Les écrire en clair dans un fichier (ou dans `My.Settings`, qui persiste **en clair** dans le profil utilisateur) revient à les offrir au premier venu.

La réponse idiomatique sous Windows est la **DPAPI** (*Data Protection API*) : le système chiffre les données avec une clé **dérivée de la session Windows de l'utilisateur** — il n'y a **aucune clé à gérer ni à stocker**. En .NET, elle s'expose via `ProtectedData` (paquet NuGet `System.Security.Cryptography.ProtectedData`) :

```vb
Imports System.Security.Cryptography
Imports System.Text

Public Function ProtegerSecret(secret As String) As Byte()
    Dim donnees = Encoding.UTF8.GetBytes(secret)
    Return ProtectedData.Protect(donnees, optionalEntropy:=Nothing,
                                 scope:=DataProtectionScope.CurrentUser)
End Function

Public Function LireSecret(protege As Byte()) As String
    Dim donnees = ProtectedData.Unprotect(protege, optionalEntropy:=Nothing,
                                          scope:=DataProtectionScope.CurrentUser)
    Return Encoding.UTF8.GetString(donnees)
End Function
```

Le tableau d'octets retourné par `Protect` peut être écrit sur disque sans crainte : seul **le même utilisateur, sur la même machine**, pourra le déchiffrer (portée `CurrentUser` ; la portée `LocalMachine` élargit à tous les comptes du poste, à réserver aux services).

Deux limites à connaître :

- **Windows uniquement.** La DPAPI est une API du système Windows : sur un autre système, `Protect` lève une `PlatformNotSupportedException`. Pour une application de bureau ciblant `net10.0-windows`, c'est sans conséquence.
- **Elle protège *au repos*, pas *en cours d'exécution*.** Un code malveillant s'exécutant **dans la session de l'utilisateur** peut appeler `Unprotect` comme votre application. La DPAPI protège contre le vol du fichier (autre compte, autre machine, sauvegarde exfiltrée), pas contre la compromission de la session elle-même.

## Azure Key Vault

**Azure Key Vault** est un service managé qui stocke et protège secrets, clés et certificats, avec contrôle d'accès et journalisation des accès. C'est la réponse de référence à la gestion des secrets en production dans l'écosystème Microsoft (introduit côté cloud au [module 15 (§15.5)](../15-deploiement-devops/README.md)).

### Accès depuis VB.NET

On utilise `Azure.Security.KeyVault.Secrets` pour le client, et `Azure.Identity` pour l'authentification :

```vb
Imports Azure.Identity
Imports Azure.Security.KeyVault.Secrets

Public Async Function LireSecretAsync() As Task(Of String)
    Dim uriCoffre = New Uri("https://mon-coffre.vault.azure.net/")

    ' DefaultAzureCredential choisit automatiquement la bonne identité :
    '  - en production : l'identité managée de l'application Azure ;
    '  - en local : les identifiants du développeur (Visual Studio, Azure CLI…).
    Dim client = New SecretClient(uriCoffre, New DefaultAzureCredential())

    Dim secret = Await client.GetSecretAsync("ChaineConnexionBdd")
    Return secret.Value.Value
End Function
```

### Le déclic : l'identité managée

Le coffre résout un paradoxe apparent — *« si tous mes secrets sont dans le coffre, où est le secret qui ouvre le coffre ? »*. La réponse est l'**identité managée** : une application hébergée sur Azure possède une identité gérée par la plateforme, qui s'authentifie auprès du coffre **sans aucun secret à stocker**. `DefaultAzureCredential` l'exploite de façon transparente. Vous n'avez donc plus **aucun secret** dans votre application ni dans sa configuration : c'est l'objectif à viser.

### Intégration au système de configuration

Plus élégant encore : on branche le coffre directement sur le système de configuration, et ses secrets deviennent de simples clés de configuration (paquet `Azure.Extensions.AspNetCore.Configuration.Secrets`) :

```vb
Imports Azure.Identity

builder.Configuration.AddAzureKeyVault(
    New Uri("https://mon-coffre.vault.azure.net/"),
    New DefaultAzureCredential())
```

Le reste du code lit alors ces secrets via `IConfiguration`, exactement comme les autres paramètres — la hiérarchie de configuration décrite plus haut s'applique naturellement.

## Bonnes pratiques et pièges à éviter

- **Ne jamais inventer sa cryptographie.** On consomme les primitives éprouvées de la BCL, telles qu'elles sont prévues.
- **Aléa sûr uniquement.** Clés, sels, nonces : `RandomNumberGenerator`, jamais `System.Random`.
- **Mots de passe ≠ intégrité.** Jamais de SHA-256 « nu » pour un mot de passe : PBKDF2 salé (ou Argon2id), avec un nombre d'itérations conforme aux recommandations courantes.
- **Comparaisons à temps constant** pour tout secret (`CryptographicOperations.FixedTimeEquals`).
- **Chiffrement authentifié** (AES-GCM) plutôt que CBC nu ; **un nonce unique** par opération.
- **Aucun secret dans le code ni dans un fichier versionné.** *User Secrets* en développement, coffre (Key Vault) en production, **DPAPI** (`ProtectedData`) pour les secrets locaux d'une application de bureau.
- **Viser zéro secret dans l'application** grâce à l'identité managée.
- **Renouveler tout secret exposé** : un secret commité, même brièvement, est compromis.

---

L'authentification protège l'accès, la cryptographie protège les données — mais une application reste vulnérable par la façon dont elle **traite les entrées** et **construit ses réponses**. La section suivante passe en revue les failles les plus courantes et leurs contre-mesures, à travers le Top 10 de l'OWASP.

**Suite : [16.3 — OWASP Top 10 pour .NET »](03-owasp.md)**

⏭️ [OWASP Top 10 pour .NET (injection SQL et paramétrage, validation, encodage de sortie)](/16-securite/03-owasp.md)
