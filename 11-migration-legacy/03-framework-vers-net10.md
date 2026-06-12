🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 11.3 — .NET Framework 4.x → .NET 10 (analyse de dépendances, APIs retirées, `appsettings`, *breaking changes* .NET 10) 🆕

> Contrairement à la migration VB6 → VB.NET ([§11.2](02-vb6-vers-vbnet.md)), ici **le langage ne change pas** : votre VB.NET reste du VB.NET. Ce qui change, c'est la **plateforme** — on quitte le .NET Framework, lié à Windows et en maintenance, pour **.NET 10 LTS**, moderne, multiplateforme et supporté jusqu'en novembre 2028. Pour du code propre, le saut peut être étonnamment fluide ; la difficulté se concentre sur les dépendances, quelques APIs retirées, le format de projet et de configuration, et une poignée de spécificités .NET 10.

---

## 1. De quoi parle-t-on : changer de plateforme, pas de langage

Le .NET Framework 4.x (dont la dernière version, 4.8.1, date de 2022) est désormais en maintenance : il est corrigé pour la sécurité mais n'évolue plus. .NET 10, lui, est la version **LTS** active, qui apporte performances, sécurité, outillage moderne et accès à tout l'écosystème NuGet récent (→ [Annexe H](../annexes/versions-reference/README.md) pour les cycles de support).

Un point mérite d'être posé d'emblée pour un développeur VB.NET : la dimension **multiplateforme** de .NET moderne est, pour la plupart des applications de bureau VB, **sans objet** — elles restent sur Windows. Le vrai bénéfice de la migration n'est donc pas « tourner sur Linux », mais : rester sur une plateforme supportée et activement développée, gagner en performances « gratuitement » (→ module 14, [§14.6](../14-performance/06-apports-net10.md)), et débloquer l'accès aux bibliothèques modernes — y compris les briques C# que l'on consomme dans une architecture hybride (→ module [10](../10-hybride-vbnet-csharp/README.md)).

Comme toujours, on commence par **évaluer l'existant** (→ [§11.1](01-evaluer-strategies.md)). Le guide de migration détaillé figure en [Annexe E](../annexes/migration-net10/README.md).

---

## 2. Analyser les dépendances (le facteur décisif)

C'est ici que se joue la réussite — ou l'enlisement — de la migration. Une dépendance se range en trois couleurs :

- 🟢 **Verte** : package NuGet disposant d'une version compatible .NET moderne, ou API du framework toujours présente → portage direct.
- 🟡 **Jaune** : dépendance à mettre à jour, remplacer ou reconfigurer (équivalent moderne existant).
- 🔴 **Rouge** : **blocker** — bibliothèque tierce abandonnée, API retirée sans équivalent direct, composant Windows-only incompatible avec la cible → à encapsuler, réécrire, ou motif pour rester sur .NET Framework.

**Outillage d'analyse :**

- **Audit des packages** : `dotnet list package --outdated` (versions obsolètes) et `dotnet list package --vulnerable` (failles connues), à compléter par `dotnet restore`.
- **Analyseurs de compatibilité de plateforme** : intégrés au SDK, ils signalent à la compilation les API non prises en charge ou réservées à Windows (avertissements de type `CA1416`), ainsi que les API obsolètes (diagnostics `SYSLIB…`).
- **Agent de modernisation GitHub Copilot** (qui remplace le défunt .NET Upgrade Assistant) : sa phase d'*assessment* analyse projets et dépendances et produit un rapport (`assessment.md`). Il prend en charge VB.NET pour certains types de projets, mais demande une **relecture critique** (risque d'hallucination) et un abonnement Copilot — détails au module [17](../17-developpement-ia/README.md) 🤖.

L'objectif de cette phase n'est pas de migrer, mais de **savoir si l'on peut migrer**, à quel coût, et où se trouvent les obstacles durs.

---

## 3. Moderniser le projet et les packages

Avant même de toucher au code, deux modernisations structurelles sont nécessaires.

### 3.1 Le format de projet : ancien → *SDK-style*

Les anciens fichiers `.vbproj` du .NET Framework sont verbeux (liste explicite des fichiers, références détaillées, `packages.config`). .NET moderne utilise le format **SDK-style** : concis, fichiers inclus implicitement, références NuGet via `PackageReference`, et possibilité de **multi-ciblage**. Convertir le projet à ce format est un préalable technique incontournable.

### 3.2 `packages.config` → `PackageReference`

Le passage à `PackageReference` simplifie la gestion : les dépendances **transitives** sont résolues automatiquement, et le fichier projet devient nettement plus lisible.

### 3.3 Le *Target Framework Moniker* : point VB-spécifique ⚠️

Le choix du TFM n'est pas anodin pour un développeur VB.NET, dont les applications sont majoritairement Windows Forms ou WPF :

| Type de projet | TFM cible |
|---|---|
| Bibliothèque, console | `net10.0` |
| **Windows Forms / WPF** | **`net10.0-windows`** (nécessaire pour les composants Windows Desktop) |

Oublier le suffixe `-windows` pour une application WinForms/WPF empêche l'accès aux API de bureau. Pendant la transition, le **multi-ciblage** d'une bibliothèque (par ex. `net48` *et* `net10.0-windows`) permet de servir à la fois l'ancien et le nouveau monde (→ [§11.5](05-coexistence.md)).

---

## 4. Les APIs retirées

C'est la part la plus « saut de plateforme » de la migration : certaines API du .NET Framework **n'existent pas** dans .NET moderne. Les analyseurs du SDK les signalent à la compilation. Les principales :

| API / technologie .NET Framework | Statut sur .NET 10 | Remplacement / piste |
|---|---|---|
| **.NET Remoting** | ❌ Retiré | gRPC, HTTP/REST, SignalR (consommés depuis VB, → module 8) |
| **WCF côté serveur** (`System.ServiceModel` serveur) | ❌ Absent | Client WCF possible via NuGet ; côté serveur : ASP.NET Core, ou CoreWCF (communautaire) |
| **`AppDomain.CreateDomain`** (domaines applicatifs) | ❌ Non pris en charge | `AssemblyLoadContext`, ou isolation par processus |
| **Code Access Security (CAS)** | ❌ Retiré | Sécurité au niveau du système d'exploitation |
| **`BinaryFormatter`** | ❌ Bloqué (sécurité) | `System.Text.Json`, autres sérialiseurs (→ module 7, [§7.5](../07-acces-donnees/05-serialisation.md)) |
| **`System.Web`** (Web Forms, ASMX) | ❌ Absent | Cas traité en [§11.4](04-web-forms-legacy.md) |
| **Windows Workflow Foundation (WF)** | ❌ Absent | CoreWF (communautaire) ou refonte |
| **`System.Drawing.Common`** | ⚠️ Windows uniquement | OK pour le bureau Windows ; lève une exception hors Windows |
| **Enterprise Services / COM+** | ⚠️ Limité | À évaluer au cas par cas |

> Pour une application de bureau VB.NET restant sur Windows, beaucoup de ces points ne se posent pas (WinForms, WPF, `System.Drawing` sur Windows fonctionnent). Les blockers réels viennent surtout du **Remoting**, de **WCF serveur**, de **`BinaryFormatter`** et de **Web Forms**.

---

## 5. La configuration : `App.config` → `appsettings.json`

Le modèle de configuration change radicalement. Le .NET Framework s'appuie sur `App.config` / `Web.config` (XML) et l'API `ConfigurationManager`. .NET moderne s'appuie sur **`appsettings.json`** (JSON) et `Microsoft.Extensions.Configuration`.

**Avant** — `App.config` :

```xml
<configuration>
  <appSettings>
    <add key="ApiBaseUrl" value="https://api.exemple.fr" />
  </appSettings>
  <connectionStrings>
    <add name="Db" connectionString="Server=...;Database=..." />
  </connectionStrings>
</configuration>
```

**Après** — `appsettings.json` :

```json
{
  "ApiBaseUrl": "https://api.exemple.fr",
  "ConnectionStrings": {
    "Db": "Server=...;Database=..."
  }
}
```

**Lecture** — via `IConfiguration` (injecté) plutôt que `ConfigurationManager` :

```vb
Dim url As String = config("ApiBaseUrl")
Dim cs As String = config.GetConnectionString("Db")
```

Au-delà de la simple lecture, .NET moderne apporte deux atouts à exploiter lors de la modernisation (→ [§11.6](06-moderniser.md)) :

- la **configuration en couches** : `appsettings.json` + `appsettings.{Environnement}.json` + variables d'environnement + secrets utilisateur, fusionnés automatiquement ;
- le **pattern Options** : lier une section de configuration à une classe fortement typée et l'injecter via `IOptions(Of T)`.

> 💡 `ConfigurationManager` reste disponible via le package NuGet `System.Configuration.ConfigurationManager` pour une compatibilité transitoire — utile pour ne pas tout réécrire d'un coup —, mais la cible idéale est `IConfiguration`. À ne pas confondre avec **`My.Settings`** (paramètres utilisateur WinForms), mécanisme distinct qui, lui, migre correctement sur Windows Forms .NET (→ module 5, [§5.10](../05-windows-forms/10-preferences.md) ; et module 2, [§2.12](../02-fondamentaux-langage/12-espace-my.md) sur les limites de l'espace `My`).

---

## 6. Les *breaking changes* spécifiques à .NET 10

Une application qui vient du .NET Framework saute directement en .NET 10 : elle rencontre donc à la fois les retraits d'API ci-dessus **et** les ruptures propres à cette version. Microsoft les documente par domaine technologique (y compris Windows Forms), classées en incompatibilité **binaire**, **source**, ou **changement de comportement**. Les plus susceptibles de concerner du code VB.NET :

- **`System.Linq.Async` remplacé par le support intégré `System.Linq.AsyncEnumerable`** : retirer le package externe et adapter les usages (`SelectAwait`, etc.).
- **Encodage — un rappel du saut Framework → moderne plutôt qu'une rupture .NET 10** : sur .NET moderne, `Encoding.Default` vaut **UTF-8** (et non plus la page de codes ANSI de Windows comme sur .NET Framework). C'est l'écueil d'encodage le plus fréquent en arrivant du Framework : à vérifier sur la lecture de fichiers et la sérialisation texte.
- **Retraits d'API obsolètes** : des API signalées comme telles dans les versions précédentes (diagnostics `SYSLIB…`) sont désormais supprimées.
- **EF Core 10** : changements de traduction de requêtes et de comportement (par ex. sur certaines suppressions en masse) — à tester (→ module 7, [§7.2](../07-acces-donnees/02-ef-core-10.md)).
- **ASP.NET Core** (si vous exposez une Web API en VB, → module 8) : pipeline plus strict, redirection d'authentification par cookie désactivée pour les endpoints d'API, et évolutions OpenAPI.

> **Note VB-spécifique.** Le durcissement de l'enforcement des ***nullable reference types*** souvent cité parmi les ruptures .NET 10 est un sujet **C#** : VB.NET ne possède pas de types référence nullables (il a les types valeur nullables, `Nullable(Of T)` — → module 2, [§2.2](../02-fondamentaux-langage/02-types-variables.md)). Cette rupture n'affecte donc pas directement le code source VB.

La liste officielle fait foi : la page *Breaking changes in .NET 10* de Microsoft Learn, dont les points applicables à VB sont repris en **[Annexe E, §E.5](../annexes/migration-net10/README.md)**. Élément rassurant : pour du code **déjà** sur un .NET moderne, le passage à .NET 10 est généralement une mise à niveau fluide ; l'effort réel est concentré sur le saut Framework → moderne.

---

## 7. Points VB-spécifiques à surveiller

Au-delà des éléments déjà signalés, quelques particularités VB.NET :

- **TFM `-windows`** obligatoire pour WinForms/WPF (cf. §3.3).
- **`Microsoft.VisualBasic`** est disponible sur .NET moderne ; **`Microsoft.VisualBasic.Compatibility`** ne l'est pas (→ [§11.2](02-vb6-vers-vbnet.md)).
- **Espace `My`** : support partiel sur .NET moderne — correct en WinForms, plus limité ailleurs (→ module 2, [§2.12](../02-fondamentaux-langage/12-espace-my.md)).
- **Pas de modèle de projet « Worker » en VB** : un service Windows écrit sur .NET Framework devra être recâblé à la main via le Generic Host et `BackgroundService` (→ module 4, [§4.8](../04-async/08-background-services.md)).
- **Framework d'application VB** (`My.Application`, écran de démarrage, instance unique) : pris en charge sur Windows Forms .NET, mais à valider.

---

## 8. Stratégie et outillage

La migration Framework → .NET 10 se prête bien à une approche **incrémentale** (→ [§11.1](01-evaluer-strategies.md)) : modernisation projet par projet, **multi-ciblage** et coexistence pendant la transition (→ [§11.5](05-coexistence.md)), le tout sécurisé par un filet de **tests de non-régression** (→ [§11.7](07-gestion-risques.md)).

> ⚠️ **Prérequis d'outillage.** Cibler `net10.0` exige **Visual Studio 2026** : Visual Studio 2022, même équipé du SDK .NET 10, ne permet de cibler que .NET 9 et antérieurs. Vérifier ce point **avant** de planifier le chantier (→ [Annexe E, §E.4](../annexes/migration-net10/README.md)).

L'**agent GitHub Copilot app modernization** structure ce chantier en trois temps — *assessment*, *planification*, *exécution* — en validant chaque changement pour permettre un retour arrière. Il couvre VB.NET pour certains types de projets, mais son résultat doit être **systématiquement relu** (→ module [17](../17-developpement-ia/README.md)). Le guide complet, la checklist et les considérations VB figurent en **[Annexe E](../annexes/migration-net10/README.md)**.

Enfin, un argument de calendrier : **.NET 8 et .NET 9 voient leur support se terminer en novembre 2026** (→ [Annexe H](../annexes/versions-reference/README.md)). Migrer vers **.NET 10 LTS** (supporté jusqu'en 2028) plutôt que vers une version intermédiaire évite de devoir recommencer dans peu de temps.

---

## 🔑 Points clés à retenir

- Cette migration **change la plateforme, pas le langage** : pour du code VB propre, elle peut être fluide ; la difficulté tient aux dépendances, aux APIs retirées et aux formats de projet/configuration.
- **Analyser les dépendances d'abord** et les classer 🟢 / 🟡 / 🔴 ; s'appuyer sur `dotnet list package`, les analyseurs du SDK et l'*assessment* de l'agent de modernisation.
- **Moderniser le projet** (format *SDK-style*, `PackageReference`) ; cibler **`net10.0-windows`** pour WinForms/WPF.
- Connaître les **APIs retirées** (Remoting, WCF serveur, AppDomains, `BinaryFormatter`, `System.Web`, CAS) et le **changement de configuration** (`App.config` → `appsettings.json` / `IConfiguration`).
- Vérifier les **ruptures propres à .NET 10** (`System.Linq.Async`, retraits d'API, EF Core 10) et le rappel d'encodage du saut Framework → moderne (`Encoding.Default` = UTF-8) — sachant que le durcissement *nullable reference types* est un sujet **C#**, pas VB.
- Viser **.NET 10 LTS** directement (support jusqu'en 2028) plutôt qu'une version intermédiaire bientôt hors support.

---

⬅️ [11.2 — VB6 → VB.NET](02-vb6-vers-vbnet.md) · 🏠 [Sommaire](../SOMMAIRE.md) · ➡️ [11.4 — ASP.NET Web Forms (legacy VB) : maintenance et stratégie de sortie](04-web-forms-legacy.md)

⏭️ [ASP.NET Web Forms (legacy VB) : maintenance et stratégie de sortie](/11-migration-legacy/04-web-forms-legacy.md)
