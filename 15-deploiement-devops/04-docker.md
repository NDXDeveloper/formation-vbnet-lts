🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 15.4 — Conteneurisation 🆕

**Docker pour Web API et services de fond VB.NET, avec les images conteneur .NET 10**

---

## 🎯 Conteneuriser, et pour quels scénarios

Un conteneur empaquette l'application **et son environnement d'exécution** dans une unité
**isolée, portable et reproductible** — le véhicule de déploiement privilégié des applications
**cloud** et orchestrées (Kubernetes). On y gagne en cohérence (« ça marche pareil partout »), en
isolation et en scalabilité.

Le **périmètre** est ici **côté serveur**, comme l'indique le SOMMAIRE : on conteneurise une **Web
API** ou un **service de fond** (*Worker*), **pas** une application de bureau à interface graphique
(on ne conteneurise pas un WinForms pour un usage normal). Et, comme pour la CI/CD, la
**construction de l'image est agnostique du langage** : le SDK .NET compile un projet **VB.NET
exactement comme un projet C#**, et l'image d'exécution lance la **sortie publiée** sans se soucier
du langage.

---

## 🟣 Le périmètre VB.NET, honnêtement

La conteneurisation est **pleinement réaliste** en VB.NET pour les bons scénarios, avec les réserves
déjà rencontrées dans le cours :

- **Web API** : en VB, **par contrôleurs** (→ **[module 8.2](../08-services-web/02-web-api-controllers.md)**),
  pas en Minimal APIs.
- **Service de fond** : il **n'existe pas de modèle de projet « Worker » en VB** (→ **[module 4.8](../04-async/08-background-services.md)**).
  On câble donc **à la main** une application console + Generic Host + `BackgroundService`, que l'on
  conteneurise comme n'importe quelle console.
- **Native AOT** (qui produit les images les **plus petites**) reste **hors VB** (→ **[Annexe B.4](../annexes/frontiere-vbnet-csharp/README.md)**) :
  un conteneur VB s'appuie sur le mode **framework-dependent** ou **self-contained/*trimmé*** (avec
  les précautions VB du *trimming*, → **[15.1](01-packaging-desktop.md)**), pas sur l'AOT.
- **L'écosystème microservices** (Dapr, Kubernetes outillé) **penche vers C#** (→ **[Annexe B.8](../annexes/frontiere-vbnet-csharp/README.md)**) —
  mais **le conteneur lui-même** est neutre : VB y produit des Web API et des Workers sans
  difficulté.

En somme : on conteneurise du **VB côté serveur** comme du C#, à l'identique.

---

## 🐳 Les images conteneur .NET 10 🆕

Les images officielles sont publiées sur le **Microsoft Container Registry (MCR)** :

```bash
docker pull mcr.microsoft.com/dotnet/sdk:10.0       # construire
docker pull mcr.microsoft.com/dotnet/aspnet:10.0    # exécuter une Web API / ASP.NET Core
docker pull mcr.microsoft.com/dotnet/runtime:10.0   # exécuter une console / un service de fond
```

Deux évolutions notables avec **.NET 10** :

- **Ubuntu par défaut** (et non plus Debian). Microsoft a fait le choix de **ne pas publier d'images .NET 10 basées sur Debian** ; les images par défaut reposent désormais sur **Ubuntu**, en version **Noble** (24.04).
- **Familles d'images** sélectionnables via la propriété MSBuild **`ContainerFamily`** : `noble-chiseled`, `noble-chiseled-extra`, `noble`, `alpine`.

Le format **chiseled** mérite une mention particulière : ce sont des images **ultra-minimales**, sans interpréteur de commandes ni gestionnaire de paquets, ce qui réduit drastiquement la taille et la surface d'attaque.
En contrepartie, elles n'incluent **ni ICU ni tzdata** (sauf les variantes **`-extra`**) — à
prendre en compte si l'application a besoin de la **globalisation/localisation**.

> ℹ️ **Sécurité par défaut.** Depuis .NET 8 (et reconduit en .NET 10), les images ASP.NET
> s'exécutent en **utilisateur non-root** et écoutent sur le **port 8080** (un non-root ne peut pas
> se lier au port 80). Le port est d'ailleurs déduit automatiquement de `ASPNETCORE_HTTP_PORTS` / `ASPNETCORE_URLS` de l'image de base.

> 🚫 **Images `-aot`.** .NET 10 publie aussi des variantes SDK **`-aot`** pour construire des applications Native AOT. Elles ne concernent **pas** VB (AOT hors périmètre) ; on les ignore.

---

## 🛠️ Deux approches pour produire l'image

### A. Publication conteneur du SDK (sans Dockerfile) — la voie moderne

Depuis .NET 7 et **mûrie dans .NET 10**, on peut construire une image conteneur OCI **sans écrire de Dockerfile**, via `dotnet publish /t:PublishContainer`.

```bash
dotnet publish -c Release /t:PublishContainer
```

Toute la configuration passe par des **propriétés MSBuild** dans le `.vbproj` :

```xml
<PropertyGroup>
  <ContainerRepository>mon-api</ContainerRepository>
  <ContainerImageTags>1.0.0;latest</ContainerImageTags>   <!-- 'latest' par défaut depuis .NET 8 -->
  <ContainerFamily>noble-chiseled</ContainerFamily>        <!-- image Ubuntu Chiseled, minimale -->
</PropertyGroup>
```

Cette approche produit, par défaut, une image **chiseled, multi-architecture et non-root** en quelques lignes de configuration. Avantage .NET 10 supplémentaire : les **applications console** peuvent désormais produire une image via `/t:PublishContainer` **sans** activer la propriété `<EnableSdkContainerSupport>` — alignant leur comportement sur les Web API et les Workers, qui le permettaient déjà.
C'est précisément ce qu'il faut pour un **Worker VB câblé à la main** (une console).

> ⚠️ **Limite.** Le SDK **ne peut pas exécuter de commandes `RUN`** : pour installer des paquets OS,
> créer un utilisateur ou ajouter des étapes de build personnalisées, il faut un **Dockerfile**
> (approche B). .NET 10 ajoute par ailleurs la propriété **`ContainerImageFormat`** pour choisir
> explicitement le format **Docker** ou **OCI**.

### B. Dockerfile multi-étapes — contrôle total

L'approche classique, indispensable dès qu'on a besoin d'étapes de build personnalisées. Le
principe : une **étape SDK** pour compiler, une **étape runtime légère** pour exécuter — afin de
**ne pas livrer le SDK** dans l'image finale.

```dockerfile
# Étape 1 — build (image SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copier d'abord le projet et restaurer : maximise le cache de couches
COPY MonApi.vbproj .
RUN dotnet restore

# Copier le reste et publier
COPY . .
RUN dotnet publish MonApi.vbproj -c Release -o /app/publish --no-restore

# Étape 2 — exécution (image ASP.NET, légère ; 'noble-chiseled' pour le minimum)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MonApi.dll"]
```

Ce Dockerfile est **identique** à celui d'une Web API C# : seul le projet ciblé est un `.vbproj`,
et la DLL publiée se nomme `MonApi.dll`.

---

## ⚙️ Conteneuriser un service de fond (Worker)

Même structure multi-étapes, à une différence près : l'image d'exécution est **`runtime:10.0`** (et
non `aspnet`), puisqu'un Worker **n'a pas de serveur web**.

```dockerfile
# Étape d'exécution d'un service de fond : pas de serveur web
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
```

Rappel : faute de modèle « Worker » en VB, l'application est une **console** hébergeant un
`BackgroundService` via le Generic Host (→ 4.8). Sa conteneurisation ne diffère donc en rien de
celle d'une console — et la **publication conteneur du SDK** (approche A) la couvre désormais
nativement en .NET 10.

---

## ✅ Bonnes pratiques

- **Build multi-étapes** : ne jamais expédier l'image SDK ; l'image finale ne contient que le
  runtime et l'application.
- **Images minimales** : préférer **chiseled** (ou Alpine) pour la **taille** et la **sécurité** —
  en pensant à la variante **`-extra`** si la **globalisation** est nécessaire.
- **Non-root + port 8080** : conserver l'utilisateur non privilégié par défaut.
- **`.dockerignore`** : exclure `bin/`, `obj/`, `.git/`… pour des builds plus rapides et plus sûrs.
- **Cache de couches** : copier le `.vbproj` et `dotnet restore` **avant** de copier le code source.
- **Configuration par variables d'environnement** (approche *12-factor*) : `appsettings` surchargés
  par l'environnement, jamais de valeurs sensibles en dur.
- **Secrets hors de l'image** : aucun mot de passe ni clé dans le Dockerfile ou les couches
  (→ **[module 16](../16-securite/README.md)**, **[Key Vault, 15.5](05-cloud-essentiels.md)**).
- **Sondes de santé** (*health checks*) et journalisation structurée pour l'exploitation
  (→ **[12.4](../12-exceptions-debogage/04-observabilite.md)**).
- **Étiquetage et *push*** vers un registre, intégrés au pipeline (→ **[15.3](03-cicd.md)**).

> 🧠 **Le GC en conteneur (rappel 14.3).** .NET **respecte les limites mémoire** du conteneur
> (cgroups). On peut plafonner explicitement le tas via `DOTNET_GCHeapHardLimit`, et **DATAS** rend
> le **Server GC** plus sobre par défaut — des atouts précieux dans un environnement contraint
> (→ **[14.3](../14-performance/03-gc.md)**).

---

## ☸️ Orchestration (notion)

Les images conteneur sont la **brique de base** de Kubernetes et des plateformes cloud (→ **[15.5](05-cloud-essentiels.md)**).
À ce niveau, soyons honnêtes : l'**outillage microservices** (Dapr, opérateurs, *meshes*) **penche
nettement vers C#** (→ **[Annexe B.8](../annexes/frontiere-vbnet-csharp/README.md)**). Mais cela ne
remet pas en cause la conteneurisation elle-même : un conteneur **VB** (Web API par contrôleurs,
Worker câblé) **se déploie et s'orchestre comme tout autre conteneur**, son contenu étant pour
l'orchestrateur une **boîte noire** indépendante du langage.

---

## 🔁 En résumé

- On conteneurise du **VB côté serveur** — **Web API par contrôleurs** et **services de fond câblés
  main** — **à l'identique de C#** ; la construction de l'image est **agnostique du langage**.
- **Images .NET 10** 🆕 : sur **Ubuntu (Noble)** désormais (plus de Debian), avec les variantes
  **chiseled** (minimales, sécurisées, sans ICU/tzdata hors `-extra`) ; non-root + port **8080**.
- **Deux approches** : la **publication conteneur du SDK** (`/t:PublishContainer`, sans Dockerfile,
  chiseled/multi-arch/non-root par défaut, et **console couverte en .NET 10**) — sauf besoin de
  `RUN`, où l'on garde un **Dockerfile multi-étapes**.
- **Réserves VB** : pas de Minimal APIs ni de modèle Worker, **Native AOT hors périmètre** (images
  les plus petites = C#), écosystème microservices **C#-orienté** — sans empêcher le déploiement du
  conteneur.
- **Bonnes pratiques** : multi-étapes, images minimales, non-root, secrets hors image, config par
  variables, et un **GC conscient des limites du conteneur** (14.3).

Le déploiement et la consommation des services **cloud** font l'objet de la
**[15.5 — Cloud (essentiels via SDK)](05-cloud-essentiels.md)**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Cloud (essentiels, consommés via SDK) : Azure App Service, Blob Storage, Key Vault ; ⚠️ Azure Functions sans support VB officiel → C#](/15-deploiement-devops/05-cloud-essentiels.md)
