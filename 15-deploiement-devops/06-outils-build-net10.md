🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 15.6 — Outils de build .NET 10 🆕

**Élagage des paquets NuGet et améliorations MSBuild — des livrables plus sobres, sans effort**

---

## 🎯 L'outillage qui sous-tend tout le module

Derrière chaque livrable de ce module — exécutable desktop, package MSIX, image conteneur, Web API
déployée — il y a une **chaîne de build**. .NET 10 l'améliore sur deux axes que retient le
SOMMAIRE : l'**élagage des paquets NuGet** et des **améliorations MSBuild**. Comme tout l'outillage
.NET, ces gains sont **agnostiques du langage** : un projet **VB.NET** en profite **exactement**
comme un projet C#, et le plus souvent **sans rien changer** — il suffit de **cibler `net10.0`**.

---

## ✂️ L'élagage des paquets NuGet (*Package Pruning*)

### Le principe

Le SDK peut désormais retirer du graphe de dépendances les paquets dont le framework partagé fournit déjà les fonctionnalités.
Beaucoup de bibliothèques ciblent des frameworks larges (comme `netstandard2.0`) et traînent des
dépendances — `System.Text.Json`, `System.Text.Encodings.Web`… — que le **runtime .NET récent
inclut déjà**. L'élagage les **supprime du graphe de restauration** au lieu de les télécharger,
puisque la version du runtime sera de toute façon utilisée.

### Activé par défaut en .NET 10

Introduit en option dans le SDK .NET 9, l'élagage est activé par défaut dans le SDK .NET 10 pour les projets ciblant net10.0.
Le mécanisme : le SDK embarque, par framework cible, un manifeste des paquets fournis par le runtime et de la version maximale de chacun ; à la restauration, NuGet écarte tout paquet transitif dont la version est inférieure ou égale à celle du runtime.

Point important : les **références directes ne sont pas supprimées**, mais réécrites avec `PrivateAssets="all"` et `IncludeAssets="none"` pour que la copie du runtime l'emporte.

### Le nouvel avertissement NU1510

Lorsqu'une **référence directe** chevauche une bibliothèque fournie par le framework et devient
**inutile**, NuGet émet le nouvel avertissement NU1510 (la référence est, de fait, neutralisée à
la restauration). C'est un signal à traiter : on **supprime purement et simplement** la référence
devenue redondante du fichier projet, pour un `.vbproj` plus propre.

### Les bénéfices, concrets

- **Restauration et build plus rapides**, **moins d'espace disque** et **`.deps.json` allégés** (les paquets fournis par le runtime disparaissent du fichier de dépendances).
- **Moins de faux positifs de vulnérabilités** : d'après Microsoft, les projets sur les nouveaux réglages affichent 70 % de rapports de vulnérabilités transitives en moins, et la restauration peut être jusqu'à 50 % plus rapide.
- **Restaurations plus fiables** : moins de paquets à résoudre, donc moins de conflits de versions.

### Travaille de pair avec l'audit transitif

En complément, NuGetAuditMode passe par défaut à « all » en .NET 10 : NuGet audite désormais les dépendances transitives par défaut.
Élagage et audit se **renforcent** : le premier retire le « bruit » des paquets fournis par la
plateforme, le second se concentre alors sur les **dépendances réelles** de l'application — des
rapports de sécurité plus **actionnables** (→ **[module 16.4](../16-securite/04-dependances-vulnerabilites.md)**).

### Maîtriser le comportement

- **Multi-ciblage** : l'élagage automatique ne s'applique qu'au TFM net10.0 ; les TFM plus anciens peuvent en bénéficier en le déclarant explicitement via `PrunePackageReference`.
- **Désactivation** : pour une bibliothèque qui doit réellement **embarquer sa propre** version (au
  profit de consommateurs sur d'anciens frameworks), ou un pipeline encore outillé en pré-.NET 10 :

  ```xml
  <PropertyGroup>
    <RestoreEnablePackagePruning>false</RestoreEnablePackagePruning>
  </PropertyGroup>
  ```

- **Diagnostiquer** : `dotnet nuget why <paquet>` affiche le chemin de résolution d'un paquet transitif ;
  `dotnet list package --vulnerable --include-transitive` inventorie l'exposition.

---

## ⚠️ Ne pas confondre : élagage NuGet vs *trimming* IL

C'est une confusion fréquente, et la distinction est essentielle. Les deux « élaguent », mais à des
**granularités** et des **moments** différents — d'ailleurs, l'élagage NuGet utilise la même liste que `dotnet build`/`dotnet publish`, mais au niveau du paquet plutôt que de l'assembly.

| | **Élagage NuGet** (*Package Pruning*) | ***Trimming* IL** (→ [15.1](01-packaging-desktop.md)) |
|---|---|---|
| **Niveau** | Le **paquet** (graphe de dépendances) | L'**assembly** (le code IL) |
| **Moment** | **Restauration** | **Publication** |
| **Retire** | Paquets déjà fournis par le framework | **Code inutilisé** (types, membres) |
| **Par défaut** | ✅ activé (net10.0+) | Opt-in, **self-contained uniquement** |
| **Réflexion** | **Sans risque** | **Sensible** (liaison tardive, `My`) ⚠️ |
| **Langage** | Agnostique | Agnostique (mais piège VB au *trimming*) |

En clair : l'**élagage NuGet** est **sûr, par défaut et bénéfique** pour tout projet VB ; le
***trimming* IL** demande la **prudence** détaillée en 15.1 (liaison tardive, `My`). Ce sont **deux
mécanismes distincts**.

---

## 🔧 Les améliorations MSBuild

MSBuild est le système de build qui anime dotnet build et dotnet pack, et qui renseigne les commandes sur les projets (comme dotnet list package ou dotnet run).
.NET 10 poursuit son optimisation (vitesse de build, sortie via le *terminal logger* moderne, déjà
par défaut depuis .NET 9), au bénéfice de tous les projets.

> ⚠️ **Une nuance à connaître : quel MSBuild ?** Avec la **CLI `dotnet`**, c'est le MSBuild **livré avec le SDK .NET** qui s'exécute ; avec **Visual Studio** ou en invoquant MSBuild directement, c'est celui **installé avec Visual Studio**. Cette différence d'environnement a des **conséquences** : pour garantir une build **reproductible** entre les postes et la CI (→ **[15.3](03-cicd.md)**), on **épingle la version du SDK**.

```json
// global.json — fige la version du SDK utilisée (build reproductible)
{
  "sdk": { "version": "10.0.100", "rollForward": "latestFeature" }
}
```

---

## 🧰 Améliorations de la CLI .NET (adjacentes)

.NET 10 affine aussi l'expérience en ligne de commande, utile en développement comme en pipeline :

- **Alias « nom d'abord »** : `dotnet package add` (en plus de `dotnet add package`), pour plus de
  cohérence.
- **Complétion shell** : `dotnet completions generate <shell>` (bash, zsh, PowerShell…).
- **Mode interactif par défaut** dans un terminal interactif.
- **Format d'image conteneur** explicite via `ContainerImageFormat` (Docker/OCI, → 15.4).

---

## 🟣 L'angle VB.NET et la sécurité

Rien de spécifique à configurer en VB : ces outils s'appliquent **tels quels**. En pratique :

- **Recibler `net10.0`** active **élagage** et **audit transitif** par défaut — dans le même esprit
  « gains gratuits au reciblage » que les apports runtime (→ **[14.6](../14-performance/06-apports-net10.md)**).
- Vos projets VB recevront les **avertissements NU1510** pour les références devenues redondantes :
  on **nettoie**, et le `.vbproj` s'allège.
- Les **rapports de vulnérabilités** deviennent plus **actionnables** (moins de bruit plateforme) —
  un gain de sécurité **gratuit** qui se relie au scan des dépendances (→ **[16.4](../16-securite/04-dependances-vulnerabilites.md)**).

---

## 🔁 En résumé

- **L'élagage des paquets NuGet** retire les paquets **déjà fournis par le framework** ; **activé
  par défaut** pour les cibles **net10.0+**, il accélère la restauration, allège `.deps.json` et
  **réduit les faux positifs** de vulnérabilités (couplé à l'**audit transitif** désormais par
  défaut). Le nouvel avertissement **NU1510** invite à nettoyer les références redondantes.
- **Distinction clé** : l'**élagage NuGet** (paquet, restauration, **sûr, par défaut**) n'est **pas**
  le ***trimming* IL** (assembly, publication, self-contained, **piège VB** de la réflexion, → 15.1).
- **MSBuild** progresse (vitesse, *terminal logger*) ; attention au **MSBuild du SDK vs celui de
  Visual Studio** — on **épingle le SDK** (`global.json`) pour la reproductibilité.
- Côté **CLI** : alias « nom d'abord », complétion shell, mode interactif, `ContainerImageFormat`.
- **En VB** comme en C# : **recibler `net10.0`** suffit à en bénéficier, **sans rien changer**.

---

## ✅ Conclusion du module 15

Le module a suivi le parcours complet de la **livraison** d'une application VB.NET : empaqueter le
desktop (**[15.1](01-packaging-desktop.md)**), publier sur le **Store** (**[15.2](02-microsoft-store.md)**),
**automatiser** (**[15.3](03-cicd.md)**), **conteneuriser** (**[15.4](04-docker.md)**), **consommer
le cloud** (**[15.5](05-cloud-essentiels.md)**), et **optimiser le build** (15.6).

La leçon d'ensemble est fidèle au positionnement honnête de la formation : le **déploiement et le
DevOps** sont, pour l'essentiel, une affaire de **runtime et d'outillage** — un terrain où **VB.NET
est un citoyen .NET de plein droit**. Pour le **cœur de cible** (livrer une application de bureau ⭐),
tout est complet et moderne. Les rares **exceptions** (Azure Functions, Native AOT, *trimming*
réflexif, écosystème microservices) sont **isolées, documentées**, et trouvent leur réponse dans la
**stratégie hybride VB/C#** quand il le faut. Livrer du VB en 2026, du poste de bureau au cloud, est
un terrain **solide et pleinement d'actualité**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [Sécurité des applications](/16-securite/README.md)
