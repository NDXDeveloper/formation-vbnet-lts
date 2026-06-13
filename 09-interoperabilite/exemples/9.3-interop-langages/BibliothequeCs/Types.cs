/* ============================================================================
   Section 9.3 : Interopérabilité entre langages .NET (VB <- C#)
   Description : Surface publique de la bibliothèque C#, marquée CLSCompliant
                 (garantit la consommation depuis tout langage .NET, dont VB).
                 Chaque membre illustre un point de la table « C# -> VB » de la
                 section. Tous les types employés (string, int, decimal, bool,
                 Task, IAsyncEnumerable, ValueTuple) sont CLS-conformes.
   Fichier source : 03-interop-langages.md
   ============================================================================ */

[assembly: CLSCompliant(true)]

namespace BibliothequeCs;

// Record positionnel : constructeur, égalité de valeur et ToString générés —
// tous consommables tels quels depuis VB (qui ne sait pas DÉCLARER de record).
public record Personne(string Nom, int Age);

// Record à propriétés init-only : VB les règle via le constructeur OU l'initialiseur With { }.
public record class Produit
{
    public string Nom { get; init; } = "";
    public decimal Prix { get; init; }
}

public sealed class Analyseur
{
    // Paramètre out C# -> passé en ByRef côté VB (variable déclarée au préalable).
    public bool TryParse(string s, out int value) => int.TryParse(s, out value);

    // Tuple de valeur nommé -> ValueTuple, noms d'éléments conservés côté VB.
    public (string Nom, int Age) PremierContact() => ("Alice", 30);

    // async / Task -> Await fonctionne tel quel côté VB.
    public async Task<int> CalculerAsync()
    {
        await Task.Delay(10);
        return 42;
    }

    // IAsyncEnumerable -> consommable en VB, mais SANS « Await For Each » :
    // côté VB, on parcourt l'énumérateur asynchrone manuellement.
    public async IAsyncEnumerable<int> CompterAsync(int n)
    {
        for (int i = 1; i <= n; i++)
        {
            await Task.Delay(5);
            yield return i;
        }
    }

    // Nom = mot-clé VB -> côté VB, on l'échappe avec des crochets : objet.[Stop]().
    public string Stop() => "arrêté";
}

// Méthode d'extension C# : consommable (et déclarable) depuis VB.
public static class Extensions
{
    public static string Repeter(this string s, int n) => string.Concat(Enumerable.Repeat(s, n));
}
