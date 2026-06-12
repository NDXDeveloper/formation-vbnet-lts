🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 16.3 OWASP Top 10 pour .NET

L'**OWASP Top 10** est un document de référence qui recense, par consensus de la communauté de la sécurité, les risques les plus critiques des applications. C'est le point de départ incontournable pour qui veut écrire du code défendable.

Cette section s'appuie sur l'édition **courante**. La version officielle en vigueur est l'OWASP Top 10:2025, la huitième du nom, qui remplace celle de 2021 ; annoncée en novembre 2025, sa version finale a été publiée en janvier 2026. À la date de cette formation (juin 2026), c'est elle qui fait foi.

Comme pour le reste du module, la plupart des défenses présentées ici sont **fournies par la plateforme** .NET et s'écrivent à l'identique en VB.NET ou en C#. Deux ou trois pièges, en revanche, sont **propres à l'écosystème VB** : nous les signalons explicitement.

## Le Top 10:2025 en un coup d'œil

| # | Catégorie 2025 | Côté .NET / renvoi |
|---|----------------|--------------------|
| **A01** | Broken Access Control — *contrôle d'accès défaillant* | Autorisation → [§16.1](01-auth.md). Absorbe désormais le SSRF. |
| **A02** | Security Misconfiguration — *mauvaise configuration* | Configuration sécurisée, pas de fuite d'erreurs. Remontée du 5ᵉ au 2ᵉ rang. |
| **A03** 🆕 | Software Supply Chain Failures — *chaîne d'approvisionnement* | Dépendances NuGet → [§16.4](04-dependances-vulnerabilites.md). Élargit les « composants vulnérables et obsolètes » de 2021. |
| **A04** | Cryptographic Failures — *défaillances cryptographiques* | → [§16.2](02-cryptographie.md). |
| **A05** | **Injection** | **Détaillée ci-dessous** (SQL, XSS…). Descendue du 3ᵉ au 5ᵉ rang, mais toujours critique. |
| **A06** | Insecure Design — *conception non sécurisée* | Modélisation des menaces, défense en profondeur. |
| **A07** | Authentication Failures — *défaillances d'authentification* | → [§16.1](01-auth.md). |
| **A08** | Software or Data Integrity Failures — *intégrité logicielle/données* | Signatures, vérification des mises à jour, désérialisation sûre (`BinaryFormatter` retiré, → [module 5 (§5.2)](../05-windows-forms/02-winforms-net10.md)). |
| **A09** | Security Logging and Alerting Failures — *journalisation et alerte* | → [module 12](../12-exceptions-debogage/README.md). |
| **A10** 🆕 | Mishandling of Exceptional Conditions — *mauvaise gestion des cas exceptionnels* | Gestion des exceptions → [module 12](../12-exceptions-debogage/README.md). |

Deux évolutions méritent l'attention du développeur .NET : deux catégories entièrement nouvelles font leur entrée — les défaillances de la chaîne d'approvisionnement logicielle et la mauvaise gestion des cas exceptionnels —, et la **mauvaise configuration** grimpe à la deuxième place : signe que les failles se logent de plus en plus dans la *manière dont on configure et exploite* les applications, et pas seulement dans le code.

Le reste de cette section approfondit la catégorie **Injection**, qui correspond aux trois sujets au cœur de ce chapitre — **injection SQL et paramétrage, validation des entrées, encodage de sortie** — avant un tour d'horizon plus rapide des autres catégories.

## A05 — Injection : le fil rouge

Toutes les attaques par injection partagent un même mécanisme : **une donnée non fiable est interprétée comme du code ou une commande**. SQL, HTML/JavaScript (XSS), commandes système, LDAP, chemins de fichiers… le principe de défense est unique :

> **On ne mélange jamais une entrée utilisateur avec une instruction. On sépare les données de la commande.**

### Injection SQL et paramétrage

C'est l'illustration la plus connue — et, côté VB.NET, l'opérateur de concaténation `&` rend la faute dangereusement naturelle.

```vb
' ❌ VULNÉRABLE : l'entrée utilisateur est concaténée dans la requête.
Dim nom = txtRecherche.Text
Dim sql = "SELECT * FROM Clients WHERE Nom = '" & nom & "'"
Dim cmd As New SqlCommand(sql, connexion)
' Si l'utilisateur saisit  ' OR '1'='1  la condition devient toujours vraie.
```

La parade est la **requête paramétrée** : la valeur transite comme un paramètre, jamais comme du texte interprété.

```vb
' ✅ SÛR : commande paramétrée.
Dim sql = "SELECT * FROM Clients WHERE Nom = @Nom"
Using cmd As New SqlCommand(sql, connexion)
    cmd.Parameters.AddWithValue("@Nom", nom)
    Using lecteur = Await cmd.ExecuteReaderAsync()
        ' ...
    End Using
End Using
```

En production, préférez `Add` avec un **type explicite** à `AddWithValue` (qui infère le type et peut provoquer conversions implicites et contre-performances) :

```vb
cmd.Parameters.Add("@Nom", SqlDbType.NVarChar, 100).Value = nom
```

**Entity Framework Core paramètre par défaut.** Une requête LINQ est traduite en requête paramétrée sans effort de votre part :

```vb
' ✅ EF Core : paramétrage automatique.
Dim clients = Await contexte.Clients _
    .Where(Function(c) c.Nom = nom) _
    .ToListAsync()
```

Le piège réapparaît dès que l'on écrit du **SQL brut** : `FromSqlRaw` n'échappe rien, tandis que `FromSqlInterpolated` transforme les valeurs interpolées en paramètres.

```vb
' ❌ DANGER : SQL brut concaténé — injection possible.
' contexte.Clients.FromSqlRaw("SELECT * FROM Clients WHERE Nom = '" & nom & "'")

' ✅ FromSqlInterpolated : l'interpolation devient un paramètre.
Dim clients = Await contexte.Clients _
    .FromSqlInterpolated($"SELECT * FROM Clients WHERE Nom = {nom}") _
    .ToListAsync()
```

> Le même principe vaut au-delà du SQL : **ne jamais concaténer une entrée non fiable** dans une commande système, un chemin de fichier ou une requête LDAP. La séparation données/commande est universelle.

### Validation des entrées

Paramétrer protège contre l'interprétation ; **valider** garantit que la donnée est *plausible* avant même d'être traitée. En ASP.NET Core, on décrit les contraintes par annotations (`DataAnnotations`) sur le modèle :

```vb
Imports System.ComponentModel.DataAnnotations

Public Class CreerClientDto
    <Required>
    <StringLength(100, MinimumLength:=2)>
    Public Property Nom As String

    <EmailAddress>
    Public Property Email As String

    <Range(0, 150)>
    Public Property Age As Integer
End Class
```

Avec l'attribut `<ApiController>` (voir [§16.1](01-auth.md) et [module 8](../08-services-web/README.md)), la **validation du modèle est automatique** : une requête invalide reçoit une réponse `400 Bad Request` détaillant les erreurs, sans que vous ayez à tester `ModelState.IsValid` à la main.

Trois principes gouvernent une validation sûre :

- **Toujours valider côté serveur.** La validation côté client améliore l'expérience, mais elle est **contournable** : elle ne constitue jamais une mesure de sécurité. Toute donnée franchissant une frontière de confiance est revalidée côté serveur.
- **Liste blanche plutôt que liste noire.** On définit ce qui est *autorisé* (format, plage, longueur), plutôt que d'énumérer ce qui est interdit — un attaquant trouvera toujours une variante hors de votre liste noire.
- **Valider type, longueur, plage et format**, au plus près du point d'entrée.

Pour des règles complexes, **FluentValidation** (paquet NuGet, pleinement utilisable en VB.NET) offre une alternative expressive aux annotations. Côté bureau, la validation Windows Forms (`ErrorProvider`, `DataAnnotations`) a été vue au [module 5 (§5.7)](../05-windows-forms/README.md) — mais retenez que la validation de *sécurité* se joue toujours côté serveur, là où l'entrée n'est plus sous le contrôle de l'utilisateur.

### Encodage de sortie

L'injection HTML/JavaScript (**XSS**, *cross-site scripting*) se produit lorsqu'une donnée non fiable est **réaffichée sans précaution** dans une page : le navigateur l'interprète alors comme du code. La défense est l'**encodage de sortie**, adapté au **contexte** de destination.

```vb
Imports System.Text.Encodings.Web

' Encodage selon le contexte de destination.
Dim pourHtml = HtmlEncoder.Default.Encode(entreeUtilisateur)
Dim pourUrl = UrlEncoder.Default.Encode(entreeUtilisateur)
Dim pourJs = JavaScriptEncoder.Default.Encode(entreeUtilisateur)
```

Le point essentiel : **l'encodage dépend du contexte**. Un texte encodé pour du HTML reste dangereux s'il est inséré dans une URL ou dans du JavaScript. On encode *pour l'endroit où la donnée atterrit*, jamais « une fois pour toutes ».

**Une nuance de périmètre, dans l'esprit de cette formation :** les frameworks de rendu web côté serveur (Razor, Blazor) sont **du ressort de C#** (voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)) — et, bonne nouvelle, **Razor encode le HTML automatiquement** par défaut. Pour un développeur VB.NET, le sujet redevient concret dès qu'une application **produit elle-même** de la sortie interprétable :

- intégration de données dans **WebView2** (voir [module 9 (§9.4)](../09-interoperabilite/README.md)) ;
- génération manuelle de HTML (courriels, rapports) ;
- toute construction dynamique de contenu destiné à être interprété.

Dans ces cas, l'encodage de sortie reste de votre responsabilité.

## Les autres catégories, côté .NET

Les catégories restantes sont, pour l'essentiel, traitées ailleurs dans la formation ou relèvent de bonnes pratiques transverses :

- **A01 — Contrôle d'accès défaillant** (toujours nº 1). L'autorisation correcte — par politique, rôle ou périmètre — est l'objet de [§16.1](01-auth.md). On y applique le principe du moindre privilège, et l'on vérifie les droits **côté serveur** à chaque action sensible (jamais en se fiant à l'interface).
- **A02 — Mauvaise configuration** (désormais nº 2). En .NET : ne **jamais exposer** de trace d'exception détaillée en production — on renvoie des `Problem Details` génériques (`AddProblemDetails` + `UseExceptionHandler`, vus au [module 8 (§8.2)](../08-services-web/02-web-api-controllers.md)) —, forcer HTTPS, poser les en-têtes de sécurité, désactiver les fonctions de débogage hors développement.
- **A03 — Chaîne d'approvisionnement logicielle** 🆕. L'audit des dépendances NuGet et la détection des vulnérabilités connues font l'objet de la **section suivante**, [§16.4](04-dependances-vulnerabilites.md).
- **A04 — Défaillances cryptographiques.** Hachage, chiffrement et gestion des secrets : voir [§16.2](02-cryptographie.md).
- **A06 — Conception non sécurisée.** Moins une API qu'une démarche : modélisation des menaces et défense en profondeur **dès la conception** — la [checklist (§16.5)](05-checklist.md) en fait une case de processus.
- **A07 — Défaillances d'authentification.** Standards OAuth 2.0 / OIDC, jetons, Entra ID : voir [§16.1](01-auth.md).
- **A08 — Intégrité logicielle et des données.** Signer les livrables et leurs mises à jour ([module 15](../15-deploiement-devops/README.md)), et bannir la désérialisation non sûre — c'est la raison du retrait de `BinaryFormatter` ([module 5 (§5.2)](../05-windows-forms/02-winforms-net10.md)).
- **A09 — Journalisation et alerte.** Tracer les événements de sécurité sans y inscrire de donnée sensible : voir le [module 12](../12-exceptions-debogage/README.md). **A10 — Mauvaise gestion des cas exceptionnels** 🆕 y est également liée : une exception mal gérée peut fuiter de l'information ou laisser le système dans un état incohérent.

## Pièges propres à VB.NET ⚠️

Deux habitudes de l'écosystème VB méritent une vigilance accrue dans le contexte de ce chapitre — le code et ses entrées (la troisième, `My.Settings` persisté en clair, relève du stockage des secrets : [§16.2](02-cryptographie.md)) :

- **La concaténation `&`.** Omniprésente et naturelle en VB.NET, elle rend tentante la construction « à la main » de requêtes SQL, de HTML ou de chemins à partir d'entrées utilisateur. C'est la porte d'entrée nº 1 des injections. La règle est sans exception : **paramétrer** (SQL) ou **encoder** (sortie), jamais concaténer une donnée non fiable.
- **`Option Strict Off` et la liaison tardive.** Pratiques pour l'automation COM/Office ([module 9](../09-interoperabilite/README.md)), elles désactivent une partie des vérifications de type et peuvent masquer des manipulations dangereuses. Réservez-les aux cas indispensables et gardez **`Option Strict On`** partout ailleurs — c'est aussi une mesure de sécurité.

## Bonnes pratiques et synthèse

- **Séparer données et commandes** : requêtes paramétrées (SQL), `FromSqlInterpolated` plutôt que `FromSqlRaw`, jamais de concaténation d'entrée non fiable.
- **Valider côté serveur**, en liste blanche, au plus près du point d'entrée — la validation client n'est pas une sécurité.
- **Encoder la sortie selon son contexte** (HTML, URL, JavaScript) partout où l'application produit du contenu interprétable.
- **Vérifier les droits côté serveur** à chaque action sensible (A01), avec le moindre privilège.
- **Ne rien laisser fuiter** : pas de trace d'exception détaillée en production (A02, A10).
- **Se reposer sur la plateforme** : les défenses .NET (paramétrage EF Core, encodage Razor, validation `<ApiController>`) sont éprouvées — on les utilise, on ne les réécrit pas.

---

La catégorie nº 3 du Top 10:2025 — les défaillances de la **chaîne d'approvisionnement logicielle** — pointe un risque que le code seul ne peut pas couvrir : celui qu'introduisent vos **dépendances**. C'est l'objet de la section suivante.

**Suite : [16.4 — Dépendances et vulnérabilités »](04-dependances-vulnerabilites.md)**

⏭️ [Dépendances et vulnérabilités (scan NuGet, SAST/DAST en CI/CD)](/16-securite/04-dependances-vulnerabilites.md)
