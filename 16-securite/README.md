🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 16. Sécurité des applications

> *« La sécurité n'est pas une case à cocher en fin de projet : c'est une propriété que l'on conçoit, code et vérifie du premier au dernier jour. »*

La sécurité est la préoccupation la plus **transversale** de tout le développement logiciel. Elle ne se loge pas dans un module isolé : elle traverse l'interface utilisateur, la logique métier, l'accès aux données, les services réseau, le déploiement et l'exploitation. Une application de bureau Windows Forms qui stocke des identifiants en clair, une Web API qui fait confiance aux entrées de l'utilisateur, ou une bibliothèque qui embarque une dépendance NuGet vulnérable : chacune ouvre une porte qu'un attaquant finira par pousser.

Ce module rassemble les fondations dont vous avez besoin pour bâtir des applications VB.NET **dignes de confiance** : authentifier et autoriser correctement les utilisateurs, protéger les données au repos et en transit, vous prémunir contre les attaques les plus répandues, surveiller la chaîne d'approvisionnement logicielle (vos dépendances), et disposer d'une grille de relecture systématique avant chaque mise en production.

L'approche retenue est celle de la **défense en profondeur** : aucune mesure n'est suffisante à elle seule, et la sécurité naît de l'empilement cohérent de plusieurs couches — validation des entrées, principe du moindre privilège, chiffrement, journalisation des accès, et veille continue sur les vulnérabilités connues.

## La sécurité, une affaire de plateforme avant d'être une affaire de langage ✅

Voici une bonne nouvelle, cohérente avec la philosophie de cette formation : **la sécurité est l'un des domaines où VB.NET ne souffre d'aucun désavantage**.

Contrairement à certains sujets « de pointe » que l'on délègue à C# (voir [Annexe B](../annexes/frontiere-vbnet-csharp/README.md)), la sécurité repose presque entièrement sur des **API de la plateforme** .NET et de la bibliothèque standard (BCL), consommées de façon **strictement identique** depuis VB.NET ou C# :

- la cryptographie vit dans `System.Security.Cryptography` ;
- l'authentification et l'autorisation web reposent sur les intergiciels (middlewares) d'ASP.NET Core ;
- l'intégration d'annuaire passe par les SDK Microsoft Entra ID (ex-Azure AD) ;
- la gestion des secrets s'appuie sur Azure Key Vault, le *Secret Manager* de développement, des variables d'environnement ou — sur le poste de bureau — la DPAPI ;
- la résilience et la limitation de débit (*rate limiting*) sont fournies par le framework.

Tout cela relève pleinement du **périmètre de « consommation »** dans lequel VB.NET excelle. Il n'y a, ici, presque rien à isoler dans une bibliothèque C# : le code de sécurité que vous écrivez en VB.NET est tout aussi robuste et idiomatique que son équivalent C#.

## Quelques spécificités à garder en tête côté VB.NET ⚠️

Si les API sont communes, trois habitudes propres à l'écosystème VB méritent une vigilance particulière :

- **`Option Strict Off` et la liaison tardive (*late binding*).** Pratiques pour l'automation COM/Office (voir [module 9](../09-interoperabilite/README.md)), elles désactivent une partie des vérifications de type à la compilation et peuvent masquer des erreurs exploitables. Réservez-les aux cas où elles sont indispensables, et conservez `Option Strict On` partout ailleurs — c'est aussi une mesure de sécurité.
- **La tentation de la concaténation de chaînes.** L'opérateur `&`, omniprésent en VB.NET, rend la construction de requêtes SQL « à la main » dangereusement naturelle. La règle est sans exception : on utilise des **commandes paramétrées** (voir [module 7](../07-acces-donnees/README.md)), jamais de concaténation de valeurs utilisateur dans une requête. Ce point est détaillé en 16.3 (OWASP).
- **Les réglages persistés en clair.** `My.Settings` et les fichiers `*.config` sont commodes pour les préférences, mais leurs valeurs sont écrites **en clair** dans le profil de l'utilisateur : on n'y range jamais un secret. La parade pour le poste de bureau — la DPAPI — est présentée en 16.2.

## Ce que couvre ce module

- **[16.1 — Authentification et autorisation](01-auth.md)**
  Vérifier *qui* est l'utilisateur et *ce qu'il a le droit de faire* : OAuth 2.0 / OpenID Connect, jetons JWT, et intégration avec Microsoft Entra ID. Approfondit la consommation d'authentification déjà esquissée au [module 8](../08-services-web/README.md).

- **[16.2 — Cryptographie](02-cryptographie.md)**
  Protéger les données : hachage (mots de passe, intégrité), chiffrement symétrique et asymétrique, et surtout **gestion des secrets** (ne jamais coder en dur une clé) avec Azure Key Vault. Prolonge les flux de chiffrement (`CryptoStream`) vus au [module 7](../07-acces-donnees/README.md).

- **[16.3 — OWASP Top 10 pour .NET](03-owasp.md)**
  Le panorama des vulnérabilités les plus courantes appliqué à .NET : injection SQL et requêtes paramétrées, validation des entrées, encodage de sortie, et les contre-mesures concrètes côté VB.NET.

- **[16.4 — Dépendances et vulnérabilités](04-dependances-vulnerabilites.md)**
  Sécuriser la chaîne d'approvisionnement logicielle : audit des paquets NuGet, détection des CVE connues, et intégration de l'analyse SAST/DAST dans la CI/CD (voir [module 15](../15-deploiement-devops/README.md)).

- **[16.5 — Checklist de sécurité VB.NET](05-checklist.md)**
  Une grille de relecture synthétique et actionnable, à dérouler avant chaque livraison pour ne rien laisser au hasard.

## Articulation avec le reste de la formation

La sécurité étant transversale, ce module **consolide et approfondit** des points abordés ailleurs :

| Sujet | Où il a été introduit | Approfondissement ici |
|-------|----------------------|------------------------|
| Authentification (JWT, OAuth/OIDC) | Web API — [module 8](../08-services-web/README.md) | 16.1 |
| Commandes paramétrées (anti-injection) | ADO.NET — [module 7](../07-acces-donnees/README.md) | 16.3 |
| Flux de chiffrement (`CryptoStream`) | E/S et flux — [module 7](../07-acces-donnees/README.md) | 16.2 |
| Gestion des secrets / Key Vault | Cloud — [module 15](../15-deploiement-devops/README.md) | 16.2 |
| Analyse statique (SAST) | Qualité du code — [module 13](../13-tests-qualite/README.md) | 16.4 |
| Pipelines CI/CD (où s'exécutent les scans) | DevOps — [module 15](../15-deploiement-devops/README.md) | 16.4 |

## Objectifs pédagogiques

À l'issue de ce module, vous serez capable de :

- mettre en place une **authentification** et une **autorisation** robustes dans une application VB.NET (bureau ou Web API), en vous appuyant sur les standards OAuth 2.0 / OIDC et sur Microsoft Entra ID ;
- choisir et utiliser correctement les **primitives cryptographiques** adaptées (hachage de mots de passe, chiffrement, signatures) sans réinventer d'algorithme ;
- **ne jamais exposer de secret** dans le code source et adopter une stratégie de gestion des secrets conforme à l'environnement (développement, CI, production) ;
- identifier et neutraliser les vulnérabilités du **Top 10 de l'OWASP** dans un contexte VB.NET / .NET ;
- **auditer vos dépendances** et automatiser la détection des vulnérabilités dans votre chaîne d'intégration continue ;
- appliquer une **démarche de relecture de sécurité** systématique avant mise en production.

## Prérequis

- Les fondamentaux du langage et de la POO ([modules 2](../02-fondamentaux-langage/README.md) et [3](../03-poo/README.md)).
- Les bases de l'accès aux données ([module 7](../07-acces-donnees/README.md)) et de la consommation/exposition de services ([module 8](../08-services-web/README.md)).
- Des notions de déploiement et de CI/CD ([module 15](../15-deploiement-devops/README.md)) sont utiles pour 16.4, sans être indispensables.

---

> **Posture à adopter pour tout le module :** partez du principe que **toute entrée est hostile** jusqu'à preuve du contraire, accordez à chaque composant le **minimum de privilèges** nécessaire, et considérez la sécurité comme un **processus continu** — pas comme un état atteint une fois pour toutes.

**Commençons par le contrôle d'accès, première ligne de défense : [16.1 — Authentification et autorisation »](01-auth.md)**

⏭️ [Authentification et autorisation (OAuth 2.0 / OIDC, JWT, Microsoft Entra ID)](/16-securite/01-auth.md)
