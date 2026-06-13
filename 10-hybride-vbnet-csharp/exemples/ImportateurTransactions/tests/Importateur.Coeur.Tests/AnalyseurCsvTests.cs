/* ============================================================================
   Section 10.6 : Gérer une solution mixte — tests du cœur (C#, xUnit)
   Description : Vérifie le parsing CSV du cœur sur des entrées contrôlées :
                 champs correctement analysés, en-tête et lignes vides ignorés,
                 garde sur flux null, et égalité de valeur du record Transaction.
   Fichier source : 06-solution-mixte.md
   ============================================================================ */

using System.Text;
using Importateur.Coeur;
using Xunit;

namespace Importateur.Coeur.Tests;

public class AnalyseurCsvTests
{
    private static Stream Flux(string contenu) => new MemoryStream(Encoding.UTF8.GetBytes(contenu));

    [Fact]
    public void Analyser_ParseLesChamps()
    {
        const string csv = "Id;Date;Categorie;Montant\n1;2026-01-05;Alimentation;42.50\n2;2026-01-06;Transport;15.00\n";
        var r = new AnalyseurCsv().Analyser(Flux(csv));

        Assert.Equal(2, r.Count);
        Assert.Equal(1, r[0].Id);
        Assert.Equal(new DateOnly(2026, 1, 5), r[0].Date);
        Assert.Equal("Alimentation", r[0].Categorie);
        Assert.Equal(42.50m, r[0].Montant);
    }

    [Fact]
    public void Analyser_IgnoreEnteteEtLignesVides()
    {
        const string csv = "Id;Date;Categorie;Montant\n\n1;2026-01-05;Alimentation;10.00\n";
        Assert.Single(new AnalyseurCsv().Analyser(Flux(csv)));
    }

    [Fact]
    public void Analyser_FluxNull_Leve()
        => Assert.Throws<ArgumentNullException>(() => new AnalyseurCsv().Analyser(null!));

    [Fact]
    public void Transaction_EgaliteDeValeur()
    {
        var a = new Transaction(1, new DateOnly(2026, 1, 5), "X", 1m);
        var b = new Transaction(1, new DateOnly(2026, 1, 5), "X", 1m);
        Assert.Equal(a, b);   // égalité de valeur générée par le record
    }
}
