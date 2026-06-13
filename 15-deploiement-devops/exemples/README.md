# 💻 Exemples du module 15 — Déploiement et DevOps

Le déploiement est, pour l'essentiel, une affaire de **runtime et d'outillage**, **pas** de langage :
un pipeline `dotnet restore/build/test/publish`, un `Dockerfile` ou un SDK cloud traitent un projet
**VB.NET exactement comme un projet C#**. Les sections porteuses de code/commandes sont reconstruites
ici en exemples **complets, compilés et exécutés/publiés**.

**Environnement de validation** (juin 2026) : Visual Studio Community 2026 (18.7) ·
SDK .NET **10.0.301** · Windows 11 (culture machine fr-FR) · **Docker absent** → en 15.4, l'image
conteneur est produite en **archive (tarball) par le SDK**, sans démon Docker.

---

## 🗂️ Correspondance sections du cours → exemples

| Section | Exemple | Vérifié |
|---|---|---|
| **15.1** Packaging | [`15.1-packaging`](#151-packaging) | 3 modes publiés et exécutés (FD 5 fichiers / SC 77 Mo / single-file 1 exe) |
| **15.2** Microsoft Store | *(pas de projet)* | canal de distribution — documenté (Partner Center, MSIX, VS) |
| **15.3** CI/CD | [`15.3-cicd`](#153-cicd) | chaîne restore→build→test (3/3)→publish ; 3 workflows YAML |
| **15.4** Conteneurisation | [`15.4-docker`](#154-docker) | Worker exécuté ; **image OCI en tarball 86 Mo via le SDK** (sans Docker) |
| **15.5** Cloud | [`15.5-cloud`](#155-cloud) | SDK Azure (Identity/Blob/Key Vault) construits et lus |
| **15.6** Outils de build | [`15.6-build-tools`](#156-build-tools) | **NU1510** (élagage du paquet redondant) au build |

---

## ▶️ Comment compiler et lancer

```bash
# 15.1 — modes de publication (depuis 15.1-packaging)
dotnet publish -c Release -r win-x64 --self-contained false -o pub-fd
dotnet publish -c Release -r win-x64 --self-contained true  -o pub-sc
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o pub-sf

# 15.3 — pipeline universel (depuis 15.3-cicd)
dotnet restore && dotnet build -c Release --no-restore && dotnet test -c Release --no-build

# 15.4 — Worker + image conteneur (PowerShell pour /t:, depuis 15.4-docker)
dotnet run -c Release
dotnet publish -c Release -r linux-x64 /t:PublishContainer -p:ContainerArchiveOutputPath=image/travailleur.tar.gz

cd 15.5-cloud       && dotnet run -c Release
cd 15.6-build-tools && dotnet build -c Release      # observer l'avertissement NU1510
```

> 🐳 **Docker absent ici.** Le `Dockerfile` de 15.4 est un artefact fidèle ; `docker build` exige
> Docker. La **publication conteneur du SDK** (`/t:PublishContainer`) produit l'image **sans Docker**,
> en archive — c'est ce qui est vérifié.

Toutes les valeurs ci-dessous ont été **observées à l'exécution**.

---

## 15.1-packaging

- **Section** : 15.1 · **Fichier** : `01-packaging-desktop.md`
- **Description** : compare les **modes de publication** sur une application .NET 10 :
  **framework-dependent** (runtime requis sur la cible), **self-contained** (runtime inclus, RID
  obligatoire) et **fichier unique**. Console ici pour une vérification simple ; une appli de bureau
  ajouterait `net10.0-windows` + `UseWindowsForms` — les modes sont identiques.
- **Sortie / artefacts attendus** (vérifiés) :
  ```text
  framework-dependent : 5 fichiers, AUCUN coreclr.dll → exe exécuté (MARQUEUR + .NET 10.0.9)
  self-contained      : 192 fichiers, coreclr.dll présent, 77 Mo → exe exécuté
  single-file         : 1 fichier (Packaging.exe) → exe exécuté
  ```
- **Comportement vérifié** : les trois livrables **s'exécutent** et impriment le marqueur ; les
  artefacts confirment chaque mode (taille, présence du runtime, nombre de fichiers).
- **ClickOnce / MSIX / Microsoft Store** : mécanismes de **distribution** (Visual Studio + certificats
  de signature) — non scriptables en CLI ici ; voir le cours (15.1/15.2).

## 15.3-cicd

- **Section** : 15.3 · **Fichier** : `03-cicd.md`
- **Description** : une bibliothèque VB + tests xUnit, et **trois workflows** (`.github/workflows/ci.yml`,
  `azure-pipelines.yml`, `.gitlab-ci.yml`). Tous exécutent la **même colonne vertébrale agnostique** :
  `restore → build → test → publish`.
- **Sortie attendue** (vérifiée — chaîne locale) :
  ```text
  restore : OK   |   build -c Release : 0 erreur   |   test : Réussi! 3/3   |   publish : Calculs.dll produit
  ```
- **Comportement vérifié** : la chaîne CI/CD fonctionne **telle quelle** sur un projet VB (citoyen
  .NET de plein droit). **Seule réserve** (documentée dans les YAML) : une appli **WinForms/WPF**
  (`net10.0-windows`) exige un **runner Windows** ; une bibliothèque/Web API compile aussi sur Linux.

## 15.4-docker

- **Section** : 15.4 · **Fichier** : `04-docker.md`
- **Description** : un **service de fond VB câblé à la main** (console + Generic Host +
  `BackgroundService`, faute de modèle « Worker » en VB), un **`Dockerfile` multi-étapes** (artefact),
  et la **publication conteneur du SDK** qui produit une **image OCI en archive** sans Docker.
- **Sortie / artefacts attendus** (vérifiés) :
  ```text
  Worker          : « Cycle de travail 1..3 » puis « Travail terminé. »
  PublishContainer: image travailleur-vb:1.0.0,latest sur dotnet/runtime:10.0
                    → image/travailleur.tar.gz (~86 Mo)  [sans démon Docker]
  ```
- **Comportement vérifié** : le Worker s'exécute (3 cycles), et le SDK **construit l'image** et
  l'écrit en **tarball OCI** sans Docker. Variable d'hôte nommée `hote` (pas `host` → BC30980).
- **Réserves VB** (documentées) : pas de Minimal APIs ni de modèle Worker ; **Native AOT hors VB**
  (images les plus petites = C#) ; `docker build` exige Docker.

## 15.5-cloud

- **Section** : 15.5 · **Fichier** : `05-cloud-essentiels.md`
- **Description** : consommation des **SDK Azure** depuis VB — `Azure.Identity`
  (`DefaultAzureCredential`, auth sans secret), `Azure.Storage.Blobs` (`BlobServiceClient`),
  `Azure.Security.KeyVault.Secrets` (`SecretClient`). Les clients sont **construits** (aucun appel
  réseau) et leurs propriétés locales lues.
- **Sortie attendue** (vérifiée) :
  ```text
  [Azure.Identity] DefaultAzureCredential construit (sans secret)
  [Blob Storage] compte=moncompte ; conteneur=documents ; objet=rapport.pdf
  [Key Vault] VaultUri=https://moncoffre.vault.azure.net/
  ```
- **Comportement vérifié** : les SDK Azure se consomment **à l'identique de C#** ; la construction
  des clients ne déclenche **aucun appel réseau** (les opérations *live* exigeraient un compte Azure
  + identité). **⚠️ Azure Functions** : pas de support VB → la Function s'écrit en **C#** (hybride).

## 15.6-build-tools

- **Section** : 15.6 · **Fichier** : `06-outils-build-net10.md`
- **Description** : l'**élagage des paquets NuGet** (activé par défaut en .NET 10). Le projet
  référence **`System.Text.Json`** — déjà fourni par le runtime — ce qui rend la référence
  **redondante** : NuGet émet **NU1510**. Inclut un `global.json` (épinglage du SDK pour des builds
  reproductibles).
- **Sortie attendue** (vérifiée) :
  ```text
  warning NU1510: Le PackageReference System.Text.Json ne sera pas supprimé. Envisagez de le retirer…
  (build réussi, 0 erreur)  →  exécution : System.Text.Json (runtime) -> {"Nom":"VB.NET","Version":10}
  ```
- **Comportement vérifié** : NU1510 **signale** la référence redondante (à supprimer pour un `.vbproj`
  plus propre) ; c'est la **version du runtime** qui sert. **Distinction clé** : l'**élagage NuGet**
  (paquet, restauration, **sûr, par défaut**) n'est **pas** le ***trimming* IL** (assembly,
  publication, self-contained, **piège VB** de la réflexion — cf. 15.1). La référence est laissée
  **exprès** pour exposer l'avertissement.

---

## 🧹 Nettoyage des binaires et résidus

Outre `bin/`/`obj/`, ce module produit des **sorties de publication** (`pub-fd/`, `pub-sc/`, `pub-sf/`,
`pub/`) et une **image conteneur** (`image/`) — tous régénérables.

```powershell
# depuis le dossier exemples/
Get-ChildItem -Recurse -Directory -Include bin, obj, pub, pub-fd, pub-sc, pub-sf, image |
  Remove-Item -Recurse -Force
```

---

**Validation : juin 2026** · SDK .NET 10.0.301 · Visual Studio Community 2026 (18.7) · Windows 11 (fr-FR)
