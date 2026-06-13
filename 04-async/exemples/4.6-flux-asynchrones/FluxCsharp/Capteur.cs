/* ============================================================================
   Section 4.6 : Consommer IAsyncEnumerable / flux asynchrones ; ValueTask
   Description : Producteur de flux asynchrones écrit en C# (itérateur
                 asynchrone : async + yield return) — impossible en VB.
                 Deux flux : un flux nominal (5 mesures), et un flux qui
                 ÉCHOUE au 3e élément, pour démontrer la libération sûre de
                 l'énumérateur côté VB.
   Fichier source : 06-async-streams.md
   ============================================================================ */

using System.Runtime.CompilerServices;

namespace FluxCsharp;

public static class Capteur
{
    /// <summary>Flux nominal : 5 mesures produites une à une, de façon asynchrone.</summary>
    public static async IAsyncEnumerable<int> MesurerAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 1; i <= 5; i++)
        {
            await Task.Delay(30, ct);     // produit l'élément après une « attente »
            yield return i * 10;          // 10, 20, 30, 40, 50
        }
    }

    /// <summary>Flux qui échoue au 3e élément (pour tester la libération côté VB).</summary>
    public static async IAsyncEnumerable<int> MesurerAvecEchecAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 1; i <= 5; i++)
        {
            await Task.Delay(30, ct);
            if (i == 3)
                throw new InvalidOperationException("Capteur en panne au 3e élément.");
            yield return i * 10;
        }
    }
}
