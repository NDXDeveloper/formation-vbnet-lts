/* ============================================================================
   Section 10.5 / 10.3 : Atelier — la façade (C#)
   Description : Façade VB-friendly. Surface publique simple et stable
                 (IReadOnlyList, Stream, Task, validation explicite) ; la
                 performance Span-first (AnalyserLigne sur ReadOnlySpan<char>,
                 sans substrings) reste un DÉTAIL INTERNE, jamais exposé. Aucune
                 des fonctionnalités avancées (record, Span, async) ne « fuit »
                 dans le contrat consommé par VB.
   Fichier source : 05-atelier-core-csharp-ui-vbnet.md
   ============================================================================ */

using System.Globalization;

[assembly: CLSCompliant(true)]

namespace Importateur.Coeur;

public sealed class AnalyseurCsv : IAnalyseurCsv
{
    public IReadOnlyList<Transaction> Analyser(Stream flux)
    {
        ArgumentNullException.ThrowIfNull(flux);

        var resultats = new List<Transaction>();
        using var lecteur = new StreamReader(flux);
        bool premiere = true;
        string? ligne;
        while ((ligne = lecteur.ReadLine()) is not null)
        {
            if (premiere) { premiere = false; continue; } // en-tête
            if (ligne.Length == 0) continue;
            resultats.Add(AnalyserLigne(ligne));
        }
        return resultats;
    }

    public async Task<IReadOnlyList<Transaction>> AnalyserAsync(Stream flux, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(flux);

        var resultats = new List<Transaction>();
        using var lecteur = new StreamReader(flux);
        bool premiere = true;
        while (await lecteur.ReadLineAsync(ct) is { } ligne)
        {
            if (premiere) { premiere = false; continue; }
            if (ligne.Length == 0) continue;
            resultats.Add(AnalyserLigne(ligne));
        }
        return resultats;
    }

    // INTERNE : découpage Span-first, sans substrings intermédiaires. Jamais exposé.
    private static Transaction AnalyserLigne(ReadOnlySpan<char> ligne)
    {
        // Format attendu : Id;Date(yyyy-MM-dd);Categorie;Montant
        int p1 = ligne.IndexOf(';');
        var reste1 = ligne[(p1 + 1)..];
        int p2 = reste1.IndexOf(';');
        var reste2 = reste1[(p2 + 1)..];
        int p3 = reste2.IndexOf(';');

        var champId = ligne[..p1];
        var champDate = reste1[..p2];
        var champCategorie = reste2[..p3];
        var champMontant = reste2[(p3 + 1)..];

        int id = int.Parse(champId, CultureInfo.InvariantCulture);
        var date = DateOnly.Parse(champDate, CultureInfo.InvariantCulture);
        string categorie = champCategorie.ToString();
        decimal montant = decimal.Parse(champMontant, CultureInfo.InvariantCulture);

        return new Transaction(id, date, categorie, montant);
    }
}
