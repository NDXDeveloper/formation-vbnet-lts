🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 4.1 Pourquoi l'asynchronie (UI réactive, opérations d'E/S)

La plupart des programmes passent une part considérable de leur temps à **attendre** : la réponse d'un serveur web, la fin d'une lecture sur disque, le résultat d'une requête SQL. Pendant ces attentes, le processeur n'a rien à calculer — et pourtant, du code écrit naïvement reste **bloqué** sur place, immobilisant un thread qui ne fait que patienter. L'asynchronie part d'un constat simple : tant qu'on attend une ressource externe, autant rendre ce thread disponible pour faire autre chose.

Cette section explique *pourquoi* on programme de façon asynchrone, avant de voir *comment* dans la section [4.2](02-async-await.md). On y verra deux motivations complémentaires : garder une interface graphique réactive, et permettre à un service de tenir la charge.

---

## Le problème : un thread d'interface bloqué

Une application Windows Forms ou WPF repose sur **un seul thread d'interface** (le *UI thread*). Ce thread exécute en boucle une *pompe de messages* (message loop) qui traite, les uns après les autres, tous les événements de l'application : un clic, une frappe au clavier, un redimensionnement, et surtout les demandes de **redessin** de la fenêtre.

Tant que votre code occupe ce thread, la pompe de messages est à l'arrêt : la fenêtre ne se redessine plus, n'enregistre plus les clics, et au bout de quelques secondes d'inactivité Windows la marque comme **« Ne répond pas »** et la grise. C'est exactement ce qui se produit avec un appel **synchrone** long :

```vb
Private Sub BtnRechercher_Click(sender As Object, e As EventArgs) Handles BtnRechercher.Click
    ' Appel SYNCHRONE : le thread d'interface reste bloqué jusqu'au retour.
    ' Pendant ce temps, la fenêtre se fige et n'est plus repeinte.
    Dim resultats = _service.Rechercher(TxtTerme.Text)   ' peut durer plusieurs secondes
    AfficherResultats(resultats)
End Sub
```

L'utilisateur clique, et l'application se fige. Le problème n'est pas la *durée* de l'opération en soi — c'est qu'elle s'exécute **sur le thread qui doit aussi animer l'interface**.

---

## La solution : rendre la main pendant l'attente

L'asynchronie résout ce problème en **rendant le thread d'interface disponible** pendant que l'on attend. Avec `Await`, le thread retourne immédiatement à sa pompe de messages — la fenêtre reste fluide — puis l'exécution **reprend là où elle s'était arrêtée** une fois le résultat disponible :

```vb
Private Async Sub BtnRechercher_Click(sender As Object, e As EventArgs) Handles BtnRechercher.Click
    ' Await rend la main au thread d'interface pendant l'attente : la fenêtre reste réactive.
    Dim resultats = Await _service.RechercherAsync(TxtTerme.Text)

    ' Au retour, on est de nouveau sur le thread d'interface :
    ' on peut mettre à jour les contrôles directement, sans Invoke.
    AfficherResultats(resultats)
End Sub
```

Par défaut, dans une application à interface graphique, le code situé *après* un `Await` reprend bien **sur le thread d'interface**. C'est ce qui permet de manipuler les contrôles juste après — un confort propre aux applications de bureau, dont la mécanique exacte est détaillée en [4.2](02-async-await.md).

> **À noter** — Un gestionnaire d'événements est le **seul cas légitime** d'`Async Sub`. Partout ailleurs, une méthode asynchrone doit renvoyer `Task` ou `Task(Of T)`, afin que l'appelant puisse l'attendre et récupérer ses exceptions (voir [4.2](02-async-await.md) et [4.3](03-exceptions-async.md)).

---

## Les opérations d'E/S : le cœur de cible

Le terrain de prédilection de l'asynchronie, ce sont les **opérations d'entrée/sortie** (E/S, ou *I/O*) — celles qui consistent à attendre une ressource **extérieure** au processeur :

- un **appel réseau** (API REST, service web, téléchargement) ;
- une **lecture ou écriture de fichier** sur disque ;
- une **requête en base de données** ;
- l'attente d'un périphérique, d'un service système, etc.

Le point essentiel : pendant une opération d'E/S, **le processeur est inactif**. Il a transmis la requête (au système d'exploitation, à la carte réseau, au disque) et attend simplement une notification de fin. Bloquer un thread pour « surveiller » cette attente ne sert à rien : c'est précisément ce gaspillage que l'asynchronie supprime.

En VB.NET, la plupart des API d'E/S du framework proposent une variante asynchrone, reconnaissable au suffixe `Async`, qui renvoie une `Task` :

```vb
' Réseau
Dim json = Await client.GetStringAsync(url)

' Fichier
Dim contenu = Await File.ReadAllTextAsync(chemin)

' Base de données (Entity Framework Core — voir module 7.2)
Dim clients = Await db.Clients.Where(Function(c) c.Actif).ToListAsync()
```

---

## Pourquoi pas simplement un thread par opération ?

Une réaction naturelle serait : « plutôt que de bloquer le thread d'interface, lançons l'opération sur **un autre thread** ». Cela résout effectivement le gel de l'interface, mais c'est un mauvais réflexe pour de l'E/S, pour deux raisons.

**Un thread coûte cher.** Chaque thread réserve sa propre pile mémoire (de l'ordre de 1 Mo sous Windows) et impose un coût de planification et de changement de contexte au système d'exploitation. Dédier tout un thread à une opération qui ne fait qu'**attendre** revient à payer le prix d'un travailleur pour le regarder ne rien faire.

**L'asynchronie d'E/S n'occupe, elle, aucun thread pendant l'attente.** C'est l'insight central : un `Await` sur une opération d'E/S ne « met pas un thread en pause » — il **rend le thread au système** le temps de l'attente, puis fait reprendre la suite (éventuellement sur un autre thread) quand le résultat arrive. Aucun thread n'est immobilisé entre-temps.

Cette différence est anodine pour une seule opération, mais **décisive côté serveur**. Une Web API (voir [module 8.2](../08-services-web/02-web-api-controllers.md)) qui consacrerait un thread à chaque requête en attente d'une base de données épuiserait vite le *pool de threads* : celui-ci n'ajoute de nouveaux threads qu'avec parcimonie, et la pénurie (*thread pool starvation*) se traduit par des pics de latence, voire un blocage. En libérant le thread pendant chaque attente d'E/S, le code asynchrone permet de servir bien plus de requêtes simultanées avec les mêmes ressources : c'est l'argument de **montée en charge**.

---

## E/S n'est pas calcul : asynchronie n'est pas parallélisme

Il faut distinguer deux natures de travail, car elles n'appellent pas le même outil :

| Critère | Travail d'**E/S** (*I/O-bound*) | Travail de **calcul** (*CPU-bound*) |
|--------|-------------------------------|-----------------------------------|
| Le temps est passé à… | attendre une ressource externe | exécuter des instructions sur le CPU |
| Le processeur est… | **inactif** pendant l'attente | **pleinement occupé** |
| Bon outil | `Async`/`Await` (libère le thread) | **parallélisme** / décharge sur un thread d'arrière-plan |
| Effet recherché | réactivité, montée en charge | réduction du temps de calcul |
| Exemple | télécharger un fichier, requêter une BDD | redimensionner 1 000 images, calcul numérique lourd |
| Où c'est traité | 4.2 · 4.3 · 4.4 | [4.5](05-parallelisme.md) |

Pour un calcul intensif sur le bureau, `Await` ne sert à rien : il n'accélère pas un calcul, puisqu'il n'y a aucune E/S à attendre. Pour garder l'interface réactive *et* utiliser plusieurs cœurs, on **décharge** le calcul sur un thread d'arrière-plan (typiquement via `Task.Run`) ou on le **parallélise** — c'est le sujet de la section [4.5](05-parallelisme.md).

---

## Quelques faux amis

L'asynchronie est puissante, mais quelques réflexes la trahissent :

> **Piège — Bloquer sur de l'asynchrone.** Appeler `.Result` ou `.Wait()` sur une `Task` depuis le thread d'interface annule tout le bénéfice et peut même provoquer un **interblocage** (la suite a besoin du thread d'interface… que `.Result` bloque). Règle : on attend une `Task` avec `Await`, jamais avec `.Result`/`.Wait()`. Détaillé en [4.2](02-async-await.md).

> **Piège — `Task.Run` autour d'une E/S.** Envelopper un appel réseau dans un `Task.Run` ne le rend pas « plus asynchrone » : on bloque simplement un thread d'arrière-plan au lieu du thread courant. Si une variante `…Async` existe, on l'utilise directement.

> **Piège — En abuser.** L'asynchronie a un petit coût (machine à états, allocations). Pour une opération triviale et instantanée — un calcul en mémoire, l'accès à une variable —, le code synchrone reste plus simple et plus rapide.

---

## En résumé

- Les programmes passent beaucoup de temps à **attendre** des ressources externes ; l'asynchronie évite de gaspiller un thread pendant ces attentes.
- Sur le **bureau**, le bénéfice est la **réactivité** : `Await` libère le thread d'interface, la fenêtre ne se fige plus.
- Sur le **serveur**, le bénéfice est la **montée en charge** : aucun thread n'est immobilisé pendant l'attente, d'où bien plus de requêtes servies.
- L'asynchronie vise les opérations d'**E/S** (réseau, disque, base de données). Pour le **calcul** intensif, c'est le **parallélisme** qu'il faut (section [4.5](05-parallelisme.md)).

La motivation étant posée, place à la mécanique : comment écrire et composer concrètement du code asynchrone en VB.NET.


⏭️ [Async/Await (Task, Task(Of T))](/04-async/02-async-await.md)
