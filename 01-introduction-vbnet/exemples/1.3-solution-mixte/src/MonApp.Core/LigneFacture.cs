/* ============================================================================
   Section 1.3 : L'écosystème .NET — Structure d'une solution (solution mixte)
   Description : La brique C# de la solution mixte « MaSolution » montrée dans
                 la section : un record (non déclarable en VB) et une switch
                 expression — des fonctionnalités modernes du langage C#,
                 consommées de façon transparente par les projets VB.NET
                 de la même solution (stratégie hybride, module 10).
   Fichier source : 03-ecosysteme-dotnet.md
   ============================================================================ */

namespace MonApp.Core;

/// <summary>Catégories de TVA françaises (exemple pédagogique).</summary>
public enum CategorieTva
{
    /// <summary>Taux normal : 20 %.</summary>
    TauxNormal,

    /// <summary>Taux intermédiaire : 10 %.</summary>
    TauxIntermediaire,

    /// <summary>Taux réduit : 5,5 %.</summary>
    TauxReduit,
}

/// <summary>
/// Barème de TVA. La « switch expression » utilisée ici est une syntaxe
/// propre à C# : VB.NET ne sait pas l'écrire, mais appelle cette méthode
/// comme n'importe quelle méthode .NET.
/// </summary>
public static class BaremeTva
{
    public static decimal Taux(CategorieTva categorie) => categorie switch
    {
        CategorieTva.TauxNormal => 0.20m,
        CategorieTva.TauxIntermediaire => 0.10m,
        CategorieTva.TauxReduit => 0.055m,
        _ => throw new ArgumentOutOfRangeException(nameof(categorie)),
    };
}

/// <summary>
/// Un record C# : type immuable, à égalité par valeur, déclaré en une ligne.
/// VB.NET ne peut pas déclarer de record (langage figé à VB 16.9),
/// mais il le consomme sans difficulté — voir MonApp.Metier.
/// </summary>
public record LigneFacture(string Libelle, decimal PrixUnitaireHt, int Quantite, CategorieTva Categorie)
{
    /// <summary>Total hors taxes de la ligne.</summary>
    public decimal TotalHt => PrixUnitaireHt * Quantite;
}
