' ============================================================================
'  Section 10.6 : Gérer une solution mixte — tests du métier (VB, xUnit + Moq)
'  Description : Teste ServiceImport en SIMULANT le cœur C# : un Mock(Of
'                IAnalyseurCsv) renvoie des transactions de test, et l'on vérifie
'                que les règles (valides/rejetées) et l'agrégation LINQ
'                produisent les valeurs attendues. La frontière hybride
'                (interface + injection) rend ce mocking trivial.
'  Fichier source : 06-solution-mixte.md
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports Importateur.Coeur
Imports Importateur.Metier
Imports Moq
Imports Xunit

Namespace Importateur.Metier.Tests

    Public Class ServiceImportTests

        Private Shared Function Echantillon() As IReadOnlyList(Of Transaction)
            Return New List(Of Transaction) From {
                New Transaction(1, New DateOnly(2026, 1, 5), "Alimentation", 42.5D),
                New Transaction(2, New DateOnly(2026, 1, 6), "Transport", 15D),
                New Transaction(3, New DateOnly(2026, 1, 7), "Inconnu", 99D),    ' rejetée : catégorie inconnue
                New Transaction(4, New DateOnly(2026, 1, 8), "Transport", -5D)   ' rejetée : montant <= 0
            }
        End Function

        <Fact>
        Public Async Function ImporterAsync_AppliqueReglesEtAgrege() As Task
            ' Le cœur C# est remplacé par un mock de son interface.
            Dim faux As New Mock(Of IAnalyseurCsv)()
            faux.Setup(Function(a) a.AnalyserAsync(It.IsAny(Of Stream)(), It.IsAny(Of CancellationToken)())) _
                .ReturnsAsync(Echantillon())

            Dim service As New ServiceImport(faux.Object)
            Dim r = Await service.ImporterAsync(Stream.Null)

            Assert.Equal(2, r.Transactions.Count)                       ' valides : 1 et 2
            Assert.Equal(2, r.Rejetees.Count)                           ' rejetées : 3 et 4
            Assert.Equal(57.5D, r.TotalGeneral)                         ' 42,5 + 15
            Assert.Equal(42.5D, r.TotauxParCategorie("Alimentation"))
            Assert.Equal(15D, r.TotauxParCategorie("Transport"))
            Assert.False(r.TotauxParCategorie.ContainsKey("Inconnu"))   ' la rejetée n'agrège pas
        End Function

    End Class

End Namespace
