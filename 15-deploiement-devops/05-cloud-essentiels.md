🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 15.5 — Cloud (essentiels, consommés via SDK)

**Azure App Service, Blob Storage, Key Vault — le cloud comme bibliothèques .NET ; ⚠️ Azure Functions → C#**

---

## 🎯 Le cloud, terrain de la « consommation »

Le cloud illustre parfaitement le positionnement de VB.NET défendu tout au long de ce cours. Les
services cloud se pilotent depuis du code via leurs **SDK** — et un SDK Azure n'est rien d'autre
qu'une **bibliothèque .NET** (des paquets NuGet de la famille `Azure.*`). Or **consommer des
bibliothèques** est exactement ce que VB.NET fait **pleinement** : ces SDK s'utilisent en VB **à
l'identique de C#**.

Le périmètre de cette section suit le SOMMAIRE : les **trois essentiels** — héberger (**App
Service**), stocker (**Blob Storage**), protéger les secrets (**Key Vault**) — avec une **réserve
claire** : **Azure Functions n'a pas de support VB officiel** ; le *serverless* s'écrit en **C#**
(traité en fin de section).

---

## 🔑 Le fil conducteur : l'authentification sans secret

Avant les services, l'**authentification**, car elle les traverse tous. Le SDK Azure repose sur le
paquet **`Azure.Identity`** et sa classe **`DefaultAzureCredential`** : un mécanisme qui **trouve
automatiquement** les identifiants disponibles — **identité managée** une fois déployé sur Azure,
identifiants de développement en local (Visual Studio, Azure CLI) — **sans changer le code**.

L'intérêt est majeur : avec une **identité managée**, **aucun secret n'est à stocker ni à gérer**.
C'est l'approche recommandée, qui se relie à l'**authentification Entra ID** (→ **[module 16.1](../16-securite/01-auth.md)**)
et au principe « **pas de secrets dans le code, la config ou l'image** » (→ 15.4, et Key Vault
ci-dessous).

```vb
Imports Azure.Identity

' Le même code fonctionne en local (identifiants de dev) et en Azure (identité managée)
Dim credential = New DefaultAzureCredential()
```

Cette `credential` se passe ensuite à **tous** les clients Azure ci-dessous.

---

## ☁️ Azure App Service — héberger l'application

App Service est une plateforme d'hébergement gérée (PaaS) pour applications et **API web**. Il
héberge une **Web API VB.NET** (par contrôleurs, → **[module 8.2](../08-services-web/02-web-api-controllers.md)**)
comme n'importe quelle application ASP.NET Core : le **déploiement est agnostique du langage**.

Plusieurs voies de déploiement, déjà rencontrées :

- **`dotnet publish` + déploiement zip** (ou publication depuis Visual Studio) ;
- **CI/CD** (GitHub Actions, Azure DevOps, → **[15.3](03-cicd.md)**) — la voie recommandée ;
- **conteneur** (→ **[15.4](04-docker.md)**), App Service sachant exécuter une image.

```bash
dotnet publish -c Release -o ./publish
# puis déploiement (zip, Visual Studio ou pipeline CI/CD)
az webapp deploy --resource-group mon-rg --name mon-app --src-path ./publish.zip --type zip
```

Côté exploitation : **paramètres d'application** (`App Settings`) pour la configuration (qui
**surchargent** `appsettings.json`), **identité managée** activable d'un clic (pour accéder à Blob,
Key Vault… sans secret), **mise à l'échelle**, **emplacements de déploiement** (*slots*) pour des
mises en production sans interruption. Les plans App Service existent en **Linux** comme en
**Windows**.

---

## 🗄️ Azure Blob Storage — stocker des objets

Le stockage d'objets (fichiers, médias, sauvegardes) se consomme via le paquet
**`Azure.Storage.Blobs`**. La hiérarchie est simple : un **`BlobServiceClient`** (le compte) donne
des **`BlobContainerClient`** (les conteneurs), qui donnent des **`BlobClient`** (les objets).

```vb
Imports Azure.Identity
Imports Azure.Storage.Blobs

' Authentification sans secret (cf. plus haut)
Dim service = New BlobServiceClient(
    New Uri("https://moncompte.blob.core.windows.net"),
    New DefaultAzureCredential())

Dim conteneur = service.GetBlobContainerClient("documents")
Await conteneur.CreateIfNotExistsAsync()

' Téléverser
Dim blob = conteneur.GetBlobClient("rapport.pdf")
Await blob.UploadAsync("rapport-local.pdf", overwrite:=True)

' Télécharger
Await blob.DownloadToAsync("copie-locale.pdf")
```

Les API sont **asynchrones** (→ **[module 4](../04-async/README.md)**). Pour l'**authentification**,
on **préfère l'identité managée** (`DefaultAzureCredential`) à une chaîne de connexion en clair —
cette dernière étant un secret à protéger (et donc, idéalement, à ranger dans Key Vault).

---

## 🔐 Azure Key Vault — protéger les secrets

Key Vault centralise les **secrets** (chaînes de connexion, clés d'API, certificats) hors du code et
de la configuration. Il se consomme via **`Azure.Security.KeyVault.Secrets`** :

```vb
Imports Azure.Identity
Imports Azure.Security.KeyVault.Secrets

Dim client = New SecretClient(
    New Uri("https://moncoffre.vault.azure.net"),
    New DefaultAzureCredential())

Dim secret = Await client.GetSecretAsync("ChaineConnexionBdd")
Dim valeur As String = secret.Value.Value
```

Mieux encore, Key Vault s'**intègre à la configuration ASP.NET Core** : un fournisseur de
configuration charge les secrets directement dans `IConfiguration`, où ils se lisent comme n'importe
quel paramètre (via le paquet `Azure.Extensions.AspNetCore.Configuration.Secrets`) :

```vb
' Dans le code de démarrage de l'application
builder.Configuration.AddAzureKeyVault(
    New Uri("https://moncoffre.vault.azure.net"),
    New DefaultAzureCredential())
```

Le **principe** consolide tout ce qui précède : les secrets **vivent dans Key Vault**, on y accède
par **identité managée**, et ils ne figurent **jamais** dans le code source, `appsettings.json`,
l'**image conteneur** (→ 15.4) ni le **dépôt** (→ secrets de CI/CD, **[15.3](03-cicd.md)**). C'est
le pendant cloud du **module 16.2 (cryptographie et gestion des secrets)** (→ **[16.2](../16-securite/02-cryptographie.md)**).

---

## ⚠️ Azure Functions : pas de VB officiel → C#

C'est la **réserve** explicite de cette section. **Azure Functions** (le *serverless* événementiel
d'Azure) **ne prend pas en charge VB.NET** comme langage de premier rang : son outillage, ses
modèles de projet et son modèle d'exécution **.NET isolé** ciblent **C#** (aux côtés d'autres
langages comme JavaScript, Python ou Java), **pas VB**.

La conséquence est nette et conforme à la stratégie du cours :

- Si vous avez besoin de **fonctions serverless**, **écrivez-les en C#** (→ **[Annexe B](../annexes/frontiere-vbnet-csharp/README.md)**).
- C'est une application directe de la **stratégie hybride** (→ **[module 10](../10-hybride-vbnet-csharp/README.md)** 🔗) :
  une **Function C#** peut très bien **côtoyer** et **réutiliser** des bibliothèques VB.NET.
- À distinguer de la **consommation** : rien n'empêche une application VB d'**appeler** une Function
  (via HTTP) ou de **déclencher** un traitement — la réserve porte sur l'**écriture** de la Function,
  pas sur son usage.

---

## 🌐 Autres clouds (notion)

Le raisonnement vaut au-delà d'Azure : l'**AWS SDK for .NET** et les bibliothèques **Google Cloud
.NET** sont aussi des **bibliothèques .NET**, donc **consommables depuis VB.NET**. Le SOMMAIRE se
concentre sur les **essentiels Azure**, mais le principe — *consommer un SDK cloud = consommer une
bibliothèque* — reste le même quel que soit le fournisseur.

---

## 🧭 Positionnement honnête

- **Consommer le cloud** (héberger, stocker, lire des secrets, appeler des services) est **pleinement
  réaliste en VB.NET** ✅ : les SDK sont des bibliothèques, et VB excelle à les **consommer**.
- **La seule véritable réserve** est l'**écriture de fonctions serverless** (Azure Functions), qui
  passe par **C#** — une brique isolable et consommable, fidèle à l'esprit hybride.

C'est, en somme, exactement le **point fort** de VB.NET : la **consommation**.

---

## 🔁 En résumé

- Le cloud se pilote via des **SDK** = des **bibliothèques .NET**, consommées en VB **à l'identique
  de C#** — le terrain de prédilection de la « consommation ».
- **`DefaultAzureCredential` / identité managée** (`Azure.Identity`) est le fil conducteur :
  authentification **sans secret**, identique en local et en Azure.
- **App Service** héberge la **Web API VB** (déploiement agnostique, → 15.3/15.4) ; **Blob Storage**
  (`Azure.Storage.Blobs`) stocke les objets ; **Key Vault** (`Azure.Security.KeyVault.Secrets`)
  protège les secrets et s'**intègre à la configuration** ASP.NET Core.
- **Principe transverse** : les secrets **dans Key Vault**, **jamais** dans le code, la config ou
  l'image.
- **⚠️ Réserve** : **Azure Functions** n'a **pas de support VB** → **C#** (stratégie hybride) ; la
  *consommation* de services, elle, reste entièrement VB.

L'outillage de build qui sous-tend tous ces livrables — élagage NuGet, MSBuild — fait l'objet de la
**[15.6 — Outils de build .NET 10](06-outils-build-net10.md)**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Outils de build .NET 10 (élagage NuGet, améliorations MSBuild)](/15-deploiement-devops/06-outils-build-net10.md)
