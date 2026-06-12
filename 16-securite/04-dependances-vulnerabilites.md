🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 16.4 Dépendances et vulnérabilités

Une application .NET moderne n'est, pour l'essentiel, **pas écrite par vous** : elle assemble des dizaines, souvent des centaines de paquets NuGet, eux-mêmes tributaires d'autres paquets. Votre code applicatif ne représente qu'une fraction de ce qui s'exécute en production. Sécuriser cet **arbre de dépendances** est donc aussi important que sécuriser votre propre code — c'est précisément ce que pointe la nouvelle catégorie **A03:2025 — Software Supply Chain Failures** du Top 10 de l'OWASP (voir [§16.3](03-owasp.md)).

Les risques liés à la chaîne d'approvisionnement prennent plusieurs formes :

- **Vulnérabilités connues** (CVE, *Common Vulnerabilities and Exposures*) dans une version d'un paquet que vous utilisez — le cas le plus fréquent ;
- **Dépendances transitives** : la faille n'est pas dans le paquet que vous avez choisi, mais dans l'une de *ses* dépendances, que vous n'avez jamais vue passer ;
- **Paquets malveillants** : typosquatting (un nom proche d'un paquet légitime), confusion de dépendances (*dependency confusion*), ou paquet compromis ;
- **Paquets abandonnés** : non maintenus, donc jamais corrigés.

> **Un point de cadrage encourageant, dans l'esprit de cette formation.** L'analyse des dépendances est **indépendante du langage** : elle inspecte le graphe NuGet, que votre code soit en VB.NET ou en C#. Sur ce volet, VB.NET est donc **à parfaite égalité**. La nuance apparaîtra plus loin sur l'analyse statique (SAST), dont la couverture outillée varie selon le langage.

## Auditer ses dépendances NuGet

### Les commandes intégrées au SDK

Le SDK .NET fournit, sans rien installer, de quoi inspecter l'état de vos dépendances :

```bash
# Paquets directs présentant une vulnérabilité connue
dotnet list package --vulnerable

# Idem, en incluant les dépendances transitives (indispensable !)
dotnet list package --vulnerable --include-transitive

# Paquets dépréciés (abandonnés ou remplacés)
dotnet list package --deprecated

# Paquets pour lesquels une version plus récente existe
dotnet list package --outdated
```

La vérification des dépendances **transitives** est essentielle : c'est là que se cachent la plupart des vulnérabilités héritées.

### NuGet Audit : l'audit automatique au build

Plutôt que de lancer une commande à la main, on peut faire vérifier les dépendances **à chaque restauration**. Le SDK .NET 10 active l'**audit NuGet** par défaut et inspecte l'ensemble du graphe (dépendances directes *et* transitives). Lorsqu'une vulnérabilité connue est détectée, il émet un avertissement de la série `NU190x`, gradué par sévérité :

| Code | Sévérité |
|------|----------|
| `NU1901` | Faible |
| `NU1902` | Modérée |
| `NU1903` | Élevée |
| `NU1904` | Critique |

On configure ce comportement dans le projet (ou, mieux, dans un `Directory.Build.props` partagé par toute la solution) — et l'on peut **faire échouer le build** sur les vulnérabilités les plus graves :

```xml
<PropertyGroup>
  <!-- Audit activé (valeur par défaut), graphe complet -->
  <NuGetAudit>true</NuGetAudit>
  <NuGetAuditMode>all</NuGetAuditMode>

  <!-- On signale à partir de la sévérité « faible » -->
  <NuGetAuditLevel>low</NuGetAuditLevel>

  <!-- On TRANSFORME EN ERREURS les vulnérabilités élevées et critiques :
       le build casse, la CI/CD bloque. -->
  <WarningsAsErrors>$(WarningsAsErrors);NU1903;NU1904</WarningsAsErrors>
</PropertyGroup>
```

C'est la façon la plus simple d'imposer une **barrière de qualité** : aucune dépendance gravement vulnérable ne peut être livrée sans qu'un humain ait pris une décision explicite.

### Automatiser le suivi dans le temps

Une vérification ponctuelle ne suffit pas : de nouvelles vulnérabilités sont publiées chaque jour pour des paquets que vous utilisez déjà. On automatise donc la **veille** :

- **Dependabot** (GitHub) : ouvre automatiquement des *pull requests* pour mettre à jour les paquets vulnérables ou obsolètes, et émet des alertes de sécurité.
- **Renovate** : équivalent multiplateforme, très configurable.
- **Snyk Open Source**, **GitHub Advanced Security** : analyse continue du graphe de dépendances et priorisation des correctifs.

### Durcir la chaîne d'approvisionnement

Au-delà de la détection, plusieurs mécanismes renforcent le contrôle sur ce que vous consommez :

- **Gestion centralisée des paquets** (*Central Package Management*) via un `Directory.Packages.props` : une seule source de vérité pour les versions de toute la solution — fini les versions divergentes entre projets.
- **Fichiers de verrouillage** (`packages.lock.json`, activés par `<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>`) : restaurations reproductibles, et détection de tout changement inattendu dans le graphe.
- **Mappage des sources de paquets** (*package source mapping*) : on épingle explicitement *de quel flux* provient chaque paquet — une protection directe contre les attaques par **confusion de dépendances**.
- **Vérification des signatures** : privilégier les paquets signés et provenant de sources de confiance.
- **SBOM** (*Software Bill of Materials*) : générer un inventaire normalisé des dépendances (par ex. au format CycloneDX) pour savoir, à tout instant, ce que contient réellement une livraison.

## SAST — analyse statique

Le **SAST** (*Static Application Security Testing*) analyse le code — source ou compilé — **sans l'exécuter**, pour repérer les motifs dangereux : injection potentielle, secret codé en dur, usage cryptographique faible, etc.

### Les analyseurs Roslyn intégrés

Le compilateur .NET embarque des **analyseurs Roslyn**, qui incluent des règles de sécurité (la série `CA`, par ex. `CA2100` sur les requêtes SQL ou les règles `CA5xxx` de sécurité). On les active et on en règle la rigueur dans le projet :

```xml
<PropertyGroup>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <AnalysisLevel>latest</AnalysisLevel>
  <AnalysisMode>All</AnalysisMode>
</PropertyGroup>
```

Bonne nouvelle pour VB.NET : ces analyseurs **prennent en charge VB.NET** comme C#. Le sujet a été abordé plus largement au [module 13 (§13.4)](../13-tests-qualite/README.md) — notamment **SonarQube** et **SecurityCodeScan**, un analyseur Roslyn dédié à la sécurité qui prend en charge C# *et* VB.NET.

### Les outils tiers — et une limite réelle pour VB.NET ⚠️

| Outil SAST | Prend en charge VB.NET ? |
|------------|--------------------------|
| Analyseurs Roslyn (`CA…`) | ✅ Oui |
| SecurityCodeScan (→ [§13.4](../13-tests-qualite/04-analyse-statique.md)) | ✅ Oui (C# et VB.NET) |
| **SonarQube / SonarCloud** | ✅ Oui |
| Snyk Code | ✅ Oui |
| **GitHub CodeQL** | ❌ **Non** — VB.NET ne fait pas partie des langages pris en charge |

C'est une frontière concrète à connaître : pour une base de code **VB.NET**, l'analyse statique s'appuie sur les **analyseurs Roslyn** (dont SecurityCodeScan) et sur **SonarQube** ; **CodeQL n'est pas une option**. Là encore, l'écart se situe sur l'outillage de pointe, fortement orienté C# — exactement la logique décrite dans l'[Annexe B](../annexes/frontiere-vbnet-csharp/README.md). Rappelons que l'analyse de **dépendances**, elle, reste indépendante du langage.

## DAST — analyse dynamique

Le **DAST** (*Dynamic Application Security Testing*) procède à l'inverse : il teste l'application **en cours d'exécution**, de l'extérieur (*boîte noire*), comme le ferait un attaquant — typiquement contre une Web API déployée sur un environnement de pré-production.

- **OWASP ZAP** (gratuit, open source) et **Burp Suite** sont les outils de référence.
- Le DAST révèle des failles que le SAST ne *peut pas* voir : mauvaise configuration, en-têtes de sécurité absents, comportements d'authentification réels, injection effectivement exploitable.
- Étant agnostique au langage (il ne voit que le trafic HTTP), il s'applique aux Web API VB.NET sans particularité.

### SAST et DAST sont complémentaires

|  | SAST | DAST |
|--|------|------|
| **Ce qu'il voit** | Le code (boîte blanche) | L'application en exécution (boîte noire) |
| **Quand** | Tôt, à chaque commit | Plus tard, sur un déploiement |
| **Trouve** | Failles dans le code, secrets en dur | Failles d'exécution, configuration, en-têtes |
| **Limite** | Faux positifs ; ignore l'exécution | Ignore le code ; dépend des chemins testés |

Aucun des deux ne suffit seul : on les combine.

## Intégrer la sécurité dans la CI/CD

Le principe directeur est le *shift-left* : **déplacer les vérifications le plus tôt possible** et les exécuter **à chaque poussée de code**, plutôt qu'une seule fois avant la mise en production. Une faille détectée sur une *pull request* coûte une fraction de ce qu'elle coûte une fois déployée.

Voici un pipeline GitHub Actions minimal (les principes valent pour Azure DevOps ou GitLab CI, voir [module 15 (§15.3)](../15-deploiement-devops/README.md)) :

```yaml
name: securite
on: [push, pull_request]

jobs:
  analyse:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      # 'restore' déclenche automatiquement l'audit NuGet (NU190x) :
      # c'est CETTE étape qui échoue si le projet promeut NU1903/NU1904
      # en erreurs (propriété WarningsAsErrors vue plus haut).
      - name: Restaurer
        run: dotnet restore

      # Build avec analyseurs Roslyn ; '-warnaserror' durcit la CI :
      # tout avertissement de compilation devient une erreur.
      - name: Compiler
        run: dotnet build --no-restore -warnaserror

      # Rapport explicite des dépendances vulnérables, transitif inclus.
      - name: Auditer les dépendances
        run: dotnet list package --vulnerable --include-transitive

      - name: Tester
        run: dotnet test --no-build
```

On enrichit ensuite ce socle selon les besoins :

- une étape **SAST** (analyse SonarCloud ou Snyk Code) ;
- une étape **DAST** (par ex. l'action OWASP ZAP) exécutée contre l'environnement de pré-production après déploiement ;
- une **barrière** (*quality gate*) qui bloque la fusion en cas de détection de sévérité élevée.

## Bonnes pratiques et synthèse

- **Auditer le graphe complet**, dépendances transitives comprises (`--include-transitive`, `NuGetAuditMode=all`).
- **Faire échouer le build** sur les vulnérabilités élevées et critiques (`NU1903`, `NU1904` en erreurs).
- **Automatiser la veille** (Dependabot, Renovate) : les vulnérabilités apparaissent en continu.
- **Durcir la chaîne** : gestion centralisée des paquets, fichiers de verrouillage, mappage des sources (anti-confusion), signatures, SBOM.
- **Activer les analyseurs Roslyn** et compléter par **SonarQube** côté VB.NET (CodeQL n'étant pas disponible).
- **Combiner SAST et DAST** : le code *et* l'application en exécution.
- **Décaler à gauche** : scanner à chaque poussée, pas une fois avant la livraison.

---

Authentification, cryptographie, défenses du code, maîtrise des dépendances : il reste à **rassembler tout cela** en une grille de relecture systématique, à dérouler avant chaque mise en production. C'est l'objet de la dernière section du module.

**Suite : [16.5 — Checklist de sécurité VB.NET »](05-checklist.md)**

⏭️ [Checklist de sécurité VB.NET](/16-securite/05-checklist.md)
