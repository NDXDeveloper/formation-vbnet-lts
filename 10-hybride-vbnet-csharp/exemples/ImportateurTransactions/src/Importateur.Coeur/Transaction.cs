/* ============================================================================
   Section 10.5 / 10.3 : Atelier — modèle immuable (C#)
   Description : Le modèle exposé par le cœur. Record POSITIONNEL : il génère un
                 constructeur (consommable directement par VB), l'égalité de
                 valeur et un ToString — autant de « fonctionnalités » C# que VB
                 consomme sans pouvoir les déclarer lui-même. Type CLS-conforme.
   Fichier source : 05-atelier-core-csharp-ui-vbnet.md
   ============================================================================ */

namespace Importateur.Coeur;

public record Transaction(int Id, DateOnly Date, string Categorie, decimal Montant);
