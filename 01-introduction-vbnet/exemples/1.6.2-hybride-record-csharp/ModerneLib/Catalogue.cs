/* ============================================================================
   Section 1.6.2 : La stratégie hybride VB.NET / C# en une page
   Description : Les briques « modernes » déclarables uniquement en C# —
                 un record positionnel et des propriétés init-only — exposées
                 via une surface publique « VB-friendly », exactement comme
                 le préconise la section (le seul vrai point d'attention).
                 Le projet VB.NET AppVB les consomme sans friction : VB et C#
                 compilent vers le même IL et partagent runtime, BCL et NuGet.
   Fichier source : 06.2-strategie-hybride.md
   ============================================================================ */

namespace ModerneLib;

/// <summary>
/// Un record C# « positionnel » : type immuable, égalité par valeur,
/// ToString() généré automatiquement. VB.NET (langage figé à 16.9) ne sait
/// pas DÉCLARER un record — mais il le consomme sans aucune difficulté.
/// </summary>
public record Produit(string Nom, decimal PrixEuros);

/// <summary>
/// Propriétés init-only : assignables uniquement à l'initialisation de
/// l'objet. Côté VB, l'initialiseur « With {...} » reste autorisé : c'est
/// précisément la capacité de consommation ajoutée par VB 16.9 (la dernière
/// évolution du langage, citée aux sections 1.1 et 1.6).
/// </summary>
public class ArticleStock
{
    public string Nom { get; init; } = "";
    public int Quantite { get; init; }
}
