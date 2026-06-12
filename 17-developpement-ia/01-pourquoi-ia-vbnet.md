🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 17.1 Coder en 2026 avec l'IA : pourquoi c'est crucial pour VB.NET (le biais C# des modèles)

Cette première section pose le **fait fondateur** du module — celui qui justifie qu'on consacre à l'IA un chapitre entier plutôt qu'un encart de bonnes pratiques. Elle explique *pourquoi* l'assistance IA est, en 2026, une compétence de premier plan pour le développeur VB.NET, et *d'où vient* le biais en faveur du C# qui structure toute la pratique. Les remèdes concrets (rédaction de *prompts*, conversion du C#) sont traités en [17.2](02-prompting-vbnet.md) ; le catalogue détaillé des pièges, en [17.7](07-limites-pieges.md). Ici, on s'attache à comprendre le terrain.

---

## L'IA comme nouvelle normalité du développement

En 2026, écrire du code sans assistant est devenu l'exception. La complétion intelligente, la génération de méthodes entières, l'explication d'un message d'erreur, la rédaction de tests et la conversation directe avec un modèle depuis l'éditeur font partie du quotidien de tout développeur .NET. Visual Studio 2026 est explicitement conçu autour de cette réalité, et GitHub Copilot — sous ses différentes formes (complétion, chat, agents) — s'est imposé comme un outil par défaut.

Pour la plupart des langages populaires, le sujet s'arrêterait là : l'IA est un confort, un gain de vitesse. **Pour VB.NET, le constat est d'une autre nature**, et il tient à une asymétrie fondamentale dans la façon dont les modèles ont appris à programmer.

---

## D'où vient le biais C# : le mécanisme

Un grand modèle de langage est, pour l'essentiel, un miroir de son corpus d'entraînement : il reproduit ce qu'il a massivement vu. Or, sur l'écosystème .NET, ce corpus est très largement dominé par le **C#** :

- **Le code public.** Sur les dépôts open source, les questions-réponses des forums, les articles techniques et les *gists*, le volume de C# est d'un ordre de grandeur supérieur à celui du VB.NET. Le modèle voit donc des milliers d'exemples C# pour chaque exemple VB.
- **La documentation officielle.** Les exemples de la documentation Microsoft sont aujourd'hui presque toujours rédigés en C#. La source de référence elle-même « parle C# » par défaut.
- **L'effet du gel du langage.** VB.NET est un langage *stabilisé*, figé à la version **16.9**, en mode *consumption-only* (rappel : voir [1.6 — Positionnement 2026](../01-introduction-vbnet/06-positionnement-2026.md)). Comme il n'évolue plus et qu'il est moins utilisé pour les nouveaux projets, peu de contenu récent est produit. Le corpus VB vieillit et se raréfie *relativement* à celui du C#, qui, lui, continue de croître à chaque version.

À cela s'ajoute une **boucle qui se renforce elle-même** : moins de contenu VB produit le rend moins bien servi par les modèles ; un modèle moins à l'aise en VB décourage son usage avec l'IA ; un usage plus rare génère encore moins de contenu nouveau. Le cercle n'est pas près de s'inverser, et c'est un point sur lequel il faut être lucide plutôt qu'optimiste.

La conséquence pratique est directe : **confronté à une demande .NET non qualifiée, la pente naturelle du modèle est de produire du C#** — et, lorsqu'on l'oblige à écrire du VB, d'y laisser fuir des réflexes C#.

---

## Le paradoxe : un biais qui pèse plus lourd, justement parce que VB est minoritaire

On pourrait croire qu'un langage « de niche » a peu à attendre de l'IA. C'est l'inverse qui est vrai, et c'est le cœur de l'argument de ce module.

Pour un langage de premier plan, l'IA vient s'ajouter à un écosystème déjà riche en ressources humaines : tutoriels, réponses de forum, exemples de documentation. L'assistant est un *plus*. Pour VB.NET, ces ressources en ligne sont comparativement minces, et la documentation de référence s'exprime dans une autre langue que la vôtre. L'assistant ne vient pas s'ajouter à l'abondance : il **comble un manque structurel**. Il devient votre traducteur permanent du C# omniprésent, votre second cerveau lorsque l'exemple n'existe nulle part en VB.

Le paradoxe tient en ceci : **la rareté qui rend l'IA si nécessaire en VB.NET est exactement ce qui en dégrade la fiabilité.** L'outil est à la fois plus indispensable et moins sûr que pour un langage dominant. C'est donc le praticien du langage minoritaire qui doit être le plus aguerri à l'IA — non par confort, mais par méthode, pour franchir l'écart entre ce dont il a besoin et ce que le modèle livre spontanément.

---

## À quoi ressemble le biais, concrètement

Sans entrer dans le catalogue exhaustif (réservé à [17.7](07-limites-pieges.md)), il est utile de reconnaître dès maintenant les formes typiques que prend ce biais :

- **La dérive par défaut vers C#.** Une demande formulée sans préciser le langage — « écris-moi une classe qui… » — produit le plus souvent du C#, accolades et points-virgules compris.
- **Les hallucinations de syntaxe.** Même en répondant en VB, le modèle peut y glisser des constructions qui n'existent pas dans le langage. Par exemple, sommé de « déclarer un *record* `Personne` en VB.NET », il n'est pas rare qu'il produise quelque chose comme :

  ```vb
  ' ❌ Invalide : VB.NET ne permet pas de DÉCLARER un record,
  '    et cette syntaxe positionnelle est du C# déguisé.
  Public Record Personne(Nom As String, Age As Integer)
  ```

  alors que VB.NET (qui peut *consommer* un *record* C# mais non en déclarer — cf. [3.7](../03-poo/07-immuabilite-records.md)) attend une classe écrite à la main. On retrouve les mêmes glissements avec `init`, les *top-level statements*, les *switch expressions* ou les constructeurs primaires : autant de nouveautés C# que VB n'a pas.
- **Les fonctionnalités fantômes.** Le modèle prête parfois à VB des capacités tirées des dernières versions de C#, en supposant la parité entre les deux langages — parité qui n'existe plus depuis le gel de VB.
- **La confusion VB6 / VB.NET.** « Visual Basic » recouvre, pour le modèle, aussi bien le VB6 historique que le VB.NET moderne. Sans levée d'ambiguïté, les suggestions mélangent les deux époques.
- **L'assurance trompeuse.** Le danger n'est pas que le modèle hésite, mais qu'il se trompe *avec aplomb* : sa réponse est fluide, idiomatique en apparence, et formulée avec autorité — même quand elle est fausse en VB.

Un point mérite d'être noté, car il rejoint les forces du langage : les modèles sont nettement **plus fiables sur le code de *consommation*** (appeler la bibliothèque de classes, un paquet NuGet, un SDK) que sur les constructions idiomatiques propres à VB. Or la consommation est précisément le terrain où VB.NET excelle.

---

## L'autre face : une opportunité bien réelle

Reconnaître le biais ne revient pas à se priver de l'IA — ce serait une erreur stratégique. Bien pilotée, elle est un accélérateur considérable sur le **terrain de jeu réel** de VB.NET :

- Le *boilerplate* du code de bureau — gestionnaires d'événements, liaison de données, échafaudage de formulaires Windows Forms ⭐ et WPF — se génère vite et bien.
- La **migration de legacy** (VB6 → VB.NET, .NET Framework → .NET 10) trouve dans l'IA un puissant assistant de conversion (voir [module 11](../11-migration-legacy/README.md) et [17.3](03-migration-legacy-ia.md)).
- L'assistant fait office de **traducteur** du C# que vous croisez partout, en complément de l'aide-mémoire de correspondance ([Annexe A](../annexes/correspondance-vbnet-csharp/README.md)).
- Il abaisse le coût de la **stratégie hybride** VB.NET / C# ([module 10](../10-hybride-vbnet-csharp/README.md)) et celui de la maintenance du code existant.
- Sur les scénarios de **consommation** — y compris intégrer un service d'IA dans votre propre application ([17.9](09-consommer-ia.md)) —, là où VB est le plus à l'aise, le modèle est aussi le moins faillible.

L'IA ne ressuscite pas un langage figé, mais elle rend nettement plus confortable le travail à l'intérieur de son périmètre. C'est une transformation du quotidien, pas de la stratégie.

---

## La posture à adopter

La bonne image mentale est celle d'un **collègue de langue maternelle C#** : brillant, rapide, fluide — qui n'écrira du VB que si vous le lui imposez, et dont chaque proposition doit être relue avec un œil VB. De cette image découle une règle simple et non négociable, fil rouge de tout le module :

> **L'IA propose, le développeur valide.**

Les garde-fous sont concrets : `Option Strict On` pour que le compilateur rattrape les approximations, le compilateur lui-même comme **arbitre de vérité** (du code qui ne compile pas n'a pas voix au chapitre), et des tests pour vérifier que le code qui compile fait bien ce qu'on attend. L'autorité reste au développeur ; le modèle est un proposant, jamais un décideur.

En 2026, savoir travailler avec l'IA n'est donc pas un raffinement pour le vébéiste : c'est une **compétence de survie**, qui transforme un outil par défaut hostile en allié fiable — à condition de ne jamais oublier qui, de l'humain ou du modèle, garde la main.

---

## Ce qui ne changera pas

Il faut le dire sans détour pour éviter les faux espoirs : ce biais n'est pas un défaut passager que « le prochain modèle » corrigera. Il est **structurel**. Le corpus VB ne va pas croître ; la stratégie de Microsoft est arrêtée (*consumption-only*, voir [1.6.3 — L'ère de l'IA](../01-introduction-vbnet/06.3-ere-ia.md)). Même des modèles généralistes excellents resteront *relativement* plus faibles en VB qu'en C#, simplement parce qu'ils auront toujours vu beaucoup plus de C#.

La conclusion est donc une question de méthode, non de patience : on ne mise pas sur l'amélioration des modèles, on bâtit un flux de travail qui transforme cette contrainte en routine maîtrisée. C'est précisément l'objet des sections suivantes — à commencer par le [prompting efficace](02-prompting-vbnet.md), qui apprend à obtenir du VB.NET correct dès le premier essai.

⏭️ [Prompting efficace pour obtenir du code VB.NET](/17-developpement-ia/02-prompting-vbnet.md)
