/* ============================================================================
   Section 10.6 / 10.3 : Atelier — contrat du cœur (interface)
   Description : Exposer le contrat via une INTERFACE publique facilite les
                 tests (mocking, module 13.2) et découple l'appelant VB de
                 l'implémentation. La couche métier VB dépendra de cette
                 interface (injection par constructeur) plutôt que de la classe
                 concrète — l'ajustement « testable » prescrit en 10.6.
   Fichier source : 06-solution-mixte.md
   ============================================================================ */

namespace Importateur.Coeur;

public interface IAnalyseurCsv
{
    /// <summary>Analyse un flux CSV et renvoie les transactions. Le flux ne doit pas être null.</summary>
    IReadOnlyList<Transaction> Analyser(Stream flux);

    /// <summary>Variante asynchrone, pour une interface réactive.</summary>
    Task<IReadOnlyList<Transaction>> AnalyserAsync(Stream flux, CancellationToken ct = default);
}
