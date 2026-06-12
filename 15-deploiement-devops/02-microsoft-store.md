🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 15.2 — Distribution via le Microsoft Store

**Publier une application de bureau VB.NET sur la boutique Windows : un canal pleinement réaliste**

---

## 🎯 Le Store comme canal de distribution

La **[15.1](01-packaging-desktop.md)** a présenté **MSIX**, le format de package conteneurisé.
Le Microsoft Store en est le **prolongement naturel** : un canal de distribution **public** pour
diffuser une application de bureau, avec découverte, confiance et mises à jour gérées.

C'est, pour le cœur de cible VB.NET, un **scénario pleinement réaliste** ✅ : le processus du Store
est **agnostique du langage** — on y publie la **sortie empaquetée** d'une application Windows
Forms ⭐ ou WPF écrite en VB **exactement** comme une application C#. Aucune réserve propre à VB
ici, contrairement à d'autres sujets de ce module.

---

## 💡 Pourquoi passer par le Store

Plusieurs bénéfices distinguent la distribution Store du *sideloading* (ClickOnce ou MSIX direct
de la 15.1) :

- **Signature et hébergement gratuits.** Le Store fournit la signature de code Microsoft et l'hébergement sur CDN.
  Vous n'avez donc **ni certificat à acheter et gérer, ni CDN à exploiter** — un avantage net face
  au *sideloading*, où ces deux charges vous incombent.
- **Découverte et portée** : votre application est **recherchable** par des millions d'utilisateurs
  Windows.
- **Confiance** : les applications du Store passent une **certification**, ce qui rassure les
  utilisateurs (et certaines politiques d'entreprise).
- **Mises à jour automatiques** poussées par le Store, sans mécanisme à implémenter.
- **Installation simplifiée** et **désinstallation propre** (héritées du conteneur MSIX).
- **Tarification et licences gérées** par la plateforme (gratuit, payant, achats intégrés).
- **Statistiques** détaillées (audience, usage, avis) dans **Partner Center**.

---

## 📦 Ce que le Store accepte

Le Store prend en charge plusieurs formats : MSIX est le format recommandé, mais les installeurs traditionnels EXE/MSI sont également acceptés.
C'est une **flexibilité moderne importante** : vous pouvez soumettre votre application de bureau
VB.NET soit sous forme de **package MSIX** (→ 15.1), soit directement avec son **installeur EXE/MSI
classique**, sans la réempaqueter.

> ℹ️ Le format `.xap`, hérité d'anciennes applications, n'est plus utilisé pour les nouvelles soumissions.
> À noter : pour les applications **EXE/MSI**, tous les utilisateurs du compte développeur peuvent les soumettre et les modifier, les rôles et permissions ne s'y appliquant pas encore — un point de gouvernance à garder en tête.

---

## 🛣️ Le processus, de bout en bout

### 1. Un compte développeur Partner Center

Il faut disposer d'un compte développeur actif dans Partner Center pour soumettre des applications au Store ;
l'inscription se fait depuis un simple compte Microsoft.

### 2. Réserver le nom de l'application

Toutes les applications du Store ont un nom **unique**. La réservation du nom peut se faire jusqu'à trois mois avant la publication, même avant le début du développement.
Concrètement : dans Partner Center, page *Applications et jeux* ▸ *Nouveau produit* ▸ *Application MSIX ou PWA* ▸ saisir le nom ▸ *Vérifier la disponibilité* ▸ *Réserver le nom du produit*.

### 3. Empaqueter l'application

On produit le livrable selon la 15.1 : un **MSIX** (recommandé) ou un **installeur EXE/MSI**.

### 4. Créer et compléter la soumission

Depuis la page de présentation du produit ▸ section *Mise en production* ▸ *Démarrer la soumission*, ce qui crée un brouillon.
Le brouillon décline les étapes à compléter :

- **Packages** : téléverser le ou les fichiers (MSIX ou EXE/MSI).
- **Listings du Store** : descriptions, fonctionnalités, captures d'écran, logos — ce que verront
  les clients.
- **Tarification et disponibilité** : modèle de prix, marchés, calendrier.
- **Propriétés** : configuration requise, **capacités**, coordonnées.
- **Classification par âge** (*age ratings*).

Partner Center valide les saisies et signale les éléments manquants avant la soumission.

### 5. Soumettre pour certification

Une fois les champs requis remplis, on **soumet pour certification**. Le statut passe à **« en
cours de certification »** pendant la revue.

### 6. Publication

Après une certification réussie, l'application apparaît en général dans le catalogue sous environ 15 minutes, et son statut passe à « Dans le Microsoft Store ».

---

## 🛠️ Depuis Visual Studio 2026

Visual Studio facilite la préparation du package :

- **Associer l'application au Store** : récupère l'**identité** et l'éditeur depuis Partner Center
  et les inscrit dans le projet.
- Clic droit sur le projet ▸ **Créer des packages d'application…** ▸ choisir **« Microsoft Store »** (et non « Chargement indépendant »/*Sideloading*) : Visual Studio génère alors un fichier .msixupload prêt pour la soumission.
  Ce fichier peut **contenir plusieurs packages** (différentes architectures de processeur) sous forme de *bundle*, ainsi qu'un fichier de **symboles** pour l'analyse de performance.

> ⚠️ **Changement Visual Studio 2026.** La fonctionnalité de **soumission automatique au Store**
> depuis l'IDE **n'est plus prise en charge à partir de Visual Studio 2026**. On **téléverse donc
> le package manuellement** dans Partner Center — ou, pour automatiser, on recourt à l'**API de
> soumission du Microsoft Store** (utile en CI/CD, → **[15.3](03-cicd.md)**).

> 💡 Le choix **« Microsoft Store »** est essentiel : l'option **« Sideloading »** est destinée à la
> distribution **directe** (interne/entreprise, hors Store) et **ne génère pas** le fichier
> `.msixupload` attendu par Partner Center.

---

## ⚖️ Store vs *sideloading*

Le Store ne remplace pas les mécanismes de la 15.1 — il **complète** le tableau selon le **public
visé**.

| Critère | **Microsoft Store** | ***Sideloading*** (ClickOnce / MSIX direct, → 15.1) |
|---------|---------------------|------------------------------------------------------|
| **Public** | Grand public / commercial | Interne / métier (LOB) |
| **Découverte** | ✅ recherchable | Non (lien/partage privé) |
| **Signature + CDN** | ✅ **fournis gratuitement** | À votre charge |
| **Mises à jour** | Poussées par le Store | À configurer (ClickOnce/MSIX) |
| **Certification / politiques** | Oui (revue) | Aucune |
| **Contrôle de la distribution** | Encadré par le Store | Total |
| **Cible** | Windows 10/11 | Windows |

- **Choisir le Store** pour une diffusion **publique ou commerciale** : visibilité, confiance,
  signature et hébergement gratuits, monétisation.
- **Choisir le *sideloading*** pour une application **interne** : contrôle total, pas de revue, pas
  de politique externe.
- **En entreprise**, la distribution **privée / LOB** passe désormais par **Intune / le Portail
  d'entreprise** (MSIX en *sideloading* géré), plutôt que par une boutique d'entreprise dédiée
  (→ rappel 15.1, MSIX + parcs gérés).

---

## ⚠️ Points d'attention

- **La certification peut signaler des problèmes** : Partner Center et la revue remontent les
  manques ; on **corrige et resoumet**. Mieux vaut anticiper les **politiques du Store**.
- **Signature gérée par le Store** : inutile d'acheter un certificat pour la voie Store (à la
  différence du *sideloading*).
- **Classification par âge** et **tarification** à renseigner correctement dès la première
  soumission.
- **Windows 10/11 uniquement** : comme MSIX, le canal Store ne vise pas les anciens systèmes.
- **Automatisation** : en Visual Studio 2026, la soumission n'est plus automatisable depuis l'IDE ;
  l'**API de soumission** prend le relais pour un pipeline (→ 15.3).

---

## 🔁 En résumé

- Le Microsoft Store est un canal de distribution **public** pour les applications de bureau, et un
  scénario **pleinement réaliste en VB.NET** ✅ — le processus est **agnostique du langage**.
- Il accepte **MSIX** (recommandé) **et** les **installeurs EXE/MSI** classiques, et offre
  **signature de code et hébergement CDN gratuits**, des **mises à jour automatiques** et de la
  **découverte**.
- Le parcours : **compte Partner Center → réserver le nom → empaqueter (15.1) → soumettre
  (packages, listing, prix, classification) → certification → publication**.
- **Visual Studio 2026** prépare le `.msixupload` (option **« Microsoft Store »**), mais la
  **soumission automatique depuis l'IDE a disparu** : téléversement manuel ou **API de soumission**.
- Le **Store** vise le **grand public** ; le ***sideloading*** (15.1) reste la voie de l'**interne**
  et du **métier**, l'entreprise gérée passant par **Intune**.

L'automatisation de la chaîne build → test → publication — y compris vers le Store — fait l'objet de
la **[15.3 — CI/CD](03-cicd.md)**.

---

> 🏷️ **Légende** — 🆕 nouveau (.NET 10 / VS 2026) · ⭐ cœur VB.NET ·  
> ✅ réaliste en VB.NET · ⚠️ limite VB.NET · 🔗 hybride VB.NET ↔ C#

⏭️ [CI/CD (GitHub Actions, Azure DevOps, GitLab CI)](/15-deploiement-devops/03-cicd.md)
