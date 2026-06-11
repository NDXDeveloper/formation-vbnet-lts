🔝 Retour au [Sommaire](/SOMMAIRE.md)

# Annexe H — Versions .NET et cycle de support

Référence des versions de .NET, de leur cycle de support, et de ce que cela implique pour la **planification d'un
projet VB.NET**. Toutes les dates sont arrêtées à **juin 2026**.

---

## H.1 — Tableau de référence

| Version | Type | Sortie | Fin de support | État (juin 2026) |
|---------|------|--------|----------------|------------------|
| **.NET 10** | **LTS** | Novembre 2025 | **14 novembre 2028** | ✅ **Recommandée** |
| .NET 9 | STS | Novembre 2024 | 10 novembre 2026 | ⚠️ Support bientôt terminé |
| .NET 8 | LTS | Novembre 2023 | 10 novembre 2026 | ⚠️ Support bientôt terminé |
| .NET 11 | STS | Prévue nov. 2026 | ~nov. 2028 | 🔮 À venir |
| .NET Framework 4.8.1 | — | 2022 | Lié au cycle de Windows | 🔧 Legacy / maintenance |

**Visual Studio 2026** : disponibilité générale le 11 novembre 2025 (.NET Conf 2025) — support complet
de .NET 10 et de C# 14.  
**Langage VB.NET** : figé à la version **16.9** (langage stabilisé, *consumption-only*).

---

## H.2 — Comprendre le cycle de support (LTS vs STS)

.NET moderne suit un **cycle de vie « moderne »** (et non le cycle « fixe » de .NET Framework). Les règles sont
simples une fois posées :

- **Une version majeure par an, en novembre.** Les numéros **pairs** sont des **LTS**, les numéros **impairs** des
  **STS**.
- **LTS (Long-Term Support)** : support et correctifs gratuits pendant **3 ans**.
- **STS (Standard-Term Support)** : support et correctifs gratuits pendant **2 ans**.
- **Mises à jour de service mensuelles** (sécurité + fiabilité), avec *roll-forward* automatique des correctifs
  par défaut.

> 💡 **Pourquoi .NET 8 (LTS) et .NET 9 (STS) expirent le même jour ?** .NET 8, sorti en novembre 2023, est
> supporté **3 ans** → novembre 2026. .NET 9, sorti en novembre 2024, est supporté **2 ans** → novembre 2026
> également. Les deux convergent au **10 novembre 2026**.

**Prochains jalons LTS** (pour planifier au-delà de .NET 10) : les LTS arrivant tous les deux ans sur les
numéros pairs, la suivante après **.NET 10 (2025)** sera **.NET 12 (novembre 2027)**, puis **.NET 14 (2029)**.
.NET 11 étant une STS, une équipe privilégiant la stabilité **reste généralement sur la LTS** en cours.

---

## H.3 — Versions déjà hors support (rappel)

À la date de cette annexe, ces versions ne reçoivent **plus de correctifs** — à ne pas viser pour un nouveau
projet, et à migrer en priorité si elles sont en production :

| Version | Fin de support |
|---------|----------------|
| .NET 7 (STS) | Mai 2024 |
| .NET 6 (LTS) | Novembre 2024 |
| .NET 5 (STS) | Mai 2022 |
| .NET Core 3.1 (LTS) | Décembre 2022 |

---

## H.4 — .NET Framework : un cycle à part

.NET **Framework** (4.x) ne suit pas le cycle « moderne » mais un **cycle fixe**, **lié au système d'exploitation
Windows** : tant que la version de Windows hôte est supportée, le Framework l'est aussi. Conséquences :

- **4.8 / 4.8.1** restent **supportés**, mais en **maintenance** : correctifs de sécurité et de fiabilité, **sans
  nouvelles fonctionnalités**.
- C'est le terrain du **legacy VB** (ASP.NET Web Forms, anciennes applications de bureau). Certains de ces
  scénarios **n'ont aucun chemin** vers le moderne (Web Forms — module 11.4 ⚠️).
- Stratégie : **rester sur Framework** (pour ce qui ne migre pas) **ou migrer** vers .NET 10 (module 11,
  [Annexe E](../migration-net10/README.md)).

---

## H.5 — Dates clés à retenir

- **11 novembre 2025** — Disponibilité générale de **.NET 10 (LTS)** et de **Visual Studio 2026** (.NET Conf 2025).
- **10 novembre 2026** — **Fin de support de .NET 8 et de .NET 9** (et sortie attendue de .NET 11).
- **Novembre 2027** — Sortie attendue de **.NET 12**, prochaine **LTS**.
- **14 novembre 2028** — **Fin de support de .NET 10 (LTS)**.

---

## H.6 — Ce que cela implique pour VB.NET

- **Cible recommandée : .NET 10 (LTS)** pour tout projet VB neuf ou migré — support jusqu'en novembre 2028 et
  gains de performance « gratuits » (module 14.6).
- **Le choix de version .NET est indépendant du langage VB.** Celui-ci est **figé à 16.9** pour ce qui
  s'écrit (*consumption-only*) : viser .NET 10 **ne met pas à jour la syntaxe VB** (les rares incréments du
  compilateur, comme VB 17.13, sont de pure consommation — module 1.6). La migration porte sur le
  runtime, le SDK et les paquets — **pas sur le code VB lui-même** ([Annexe E](../migration-net10/README.md)).
- **Sur .NET 8 ou .NET 9 ?** Planifiez la migration **avant le 10 novembre 2026**.
- **Sur .NET 6 / 7 ou .NET Framework ?** Versions hors support (6/7) ou en maintenance (Framework) : auditez et
  migrez selon la stratégie du module 11.
- **Stabilité d'abord** : comme la plupart des usages VB visent des applications de bureau et métier durables,
  rester sur la **LTS** (et sauter les STS) est la trajectoire la plus sereine.

---

## H.7 — Visual Studio et ciblage des versions

Point d'attention récurrent (voir [Annexe E § E.4](../migration-net10/README.md)) :

| Pour cibler… | Il faut… |
|--------------|----------|
| **`net10.0`** | **Visual Studio 2026 (v18)** ou ultérieur |
| `net9.0` et antérieurs | Visual Studio 2022 (17.14) avec le SDK .NET 10, **ou** Visual Studio 2026 |

Autrement dit : Visual Studio 2022, même équipé du SDK .NET 10, **ne permet pas de cibler `net10.0`**. Intégrez la
montée vers **Visual Studio 2026** au périmètre de toute migration vers .NET 10.

---

### Voir aussi

- Module 1.3 — [L'écosystème .NET](../../01-introduction-vbnet/03-ecosysteme-dotnet.md) (runtime, SDK, LTS/STS)
- Module 1.6 — [VB.NET en 2026 : positionnement honnête](../../01-introduction-vbnet/06-positionnement-2026.md) (langage figé, *consumption-only*)
- Module 11 — [Migration et maintenance du code legacy](../../11-migration-legacy/README.md) · Module 18 — [Stratégie et feuille de route](../../18-strategie-roadmap/README.md)
- [Annexe D — Visual Studio 2026](../visual-studio-2026/README.md) · [Annexe E — Guide de migration vers .NET 10](../migration-net10/README.md) · [Annexe F — Glossaire (LTS, STS, TFM…)](../glossaire/README.md)

---

**Juin 2026** · .NET 10 LTS (support jusqu'au 14 novembre 2028) · Visual Studio 2026 · VB.NET 16.9 (stabilisé)

✅ *Dernière section — fin du parcours.*
