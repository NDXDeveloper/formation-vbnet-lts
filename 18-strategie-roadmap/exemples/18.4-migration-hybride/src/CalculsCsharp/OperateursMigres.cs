/* ============================================================================
   Section 18.4 : Opérateurs portés en C# — le piège « ^ » en miroir
   Description : En VB, « ^ » est la PUISSANCE ; en C#, c'est le XOR binaire. Une
                 conversion LITTÉRALE de « a ^ b » de VB vers C# change donc
                 silencieusement le comportement. Cette bibliothèque expose les deux
                 faces : Puissance (l'équivalent correct, Math.Pow) et XorBinaire
                 (ce que « a ^ b » produit réellement en C#). Le filet de tests est
                 ce qui rattrape ce genre d'écart sémantique lors d'une migration.
   Fichier source : 04-migrer-vers-csharp.md
   ============================================================================ */

namespace CalculsCsharp;

/// <summary>Opérateurs portés en C#, consommés par la partie VB restante (migration hybride).</summary>
public static class OperateursMigres
{
    /// <summary>Équivalent CORRECT du « ^ » de VB : l'élévation à la puissance.</summary>
    public static double Puissance(double a, double b) => Math.Pow(a, b);

    /// <summary>Ce que « a ^ b » fait RÉELLEMENT en C# : un OU exclusif binaire (≠ puissance).</summary>
    public static int XorBinaire(int a, int b) => a ^ b;
}
