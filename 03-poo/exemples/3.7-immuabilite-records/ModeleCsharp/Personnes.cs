/* ============================================================================
   Section 3.7 : Types immuables et records
   Description : Les records C# exacts de la section, conçus « VB-friendly » :
                 - Personne : record positionnel (propriétés init, égalité de
                   valeur, ToString générés) + méthode de copie explicite
                   WithAge — car VB n'a pas l'expression « with » ;
                 - Adresse : record positionnel complété d'une propriété
                   init-only (CodePostal), réglable depuis VB via With { }.
   Fichier source : 07-immuabilite-records.md
   ============================================================================ */

namespace ModeleCsharp;

/// <summary>
/// Record positionnel : une ligne déclare propriétés init, égalité de valeur,
/// ToString lisible, déconstruction et support de « with ».
/// </summary>
public record Personne(string Nom, int Age)
{
    /// <summary>Copie non destructive exposée comme méthode ordinaire pour VB.</summary>
    public Personne WithAge(int age) => this with { Age = age };
}

/// <summary>Record avec propriété init-only supplémentaire.</summary>
public record Adresse(string Rue, string Ville)
{
    public string? CodePostal { get; init; }
}
