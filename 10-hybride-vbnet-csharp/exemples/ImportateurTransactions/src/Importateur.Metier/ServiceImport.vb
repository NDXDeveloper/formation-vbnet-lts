' ============================================================================
'  Section 10.5 / 10.4 / 10.6 : Atelier — orchestration métier (VB)
'  Description : Consomme le cœur C# de façon TRANSPARENTE (sous Option Strict
'                On), applique les règles de gestion et agrège par catégorie en
'                LINQ — un point fort de VB. Reçoit IAnalyseurCsv par
'                CONSTRUCTEUR (injection de dépendances) : c'est l'ajustement
'                « testable » de 10.6 (le cœur se simule par un mock en test).
'  Fichier source : 05-atelier-core-csharp-ui-vbnet.md / 06-solution-mixte.md
' ============================================================================

Imports System.Linq
Imports System.Threading.Tasks
Imports Importateur.Coeur

Namespace Importateur.Metier

    Public Class ServiceImport
        Private Shared ReadOnly CategoriesValides As String() =
            {"Alimentation", "Transport", "Loisirs", "Logement"}

        Private ReadOnly _analyseur As IAnalyseurCsv

        ' Composition par défaut (utilisée par l'UI, qui ne connaît que le métier).
        Public Sub New()
            Me.New(New AnalyseurCsv())
        End Sub

        ' Le cœur est injecté (interface) plutôt qu'instancié : testable + découplé.
        Public Sub New(analyseur As IAnalyseurCsv)
            _analyseur = analyseur
        End Sub

        ' Orchestration : appelle le cœur C#, applique les règles, agrège.
        Public Async Function ImporterAsync(flux As IO.Stream) As Task(Of ResultatImport)
            Dim brutes = Await _analyseur.AnalyserAsync(flux)        ' appel transparent au cœur C#

            Dim valides = brutes.Where(AddressOf EstValide).ToList()
            Dim rejetees = brutes.Where(Function(t) Not EstValide(t)).ToList()

            ' Agrégation LINQ (idiomatique en VB)
            Dim totaux = valides _
                .GroupBy(Function(t) t.Categorie) _
                .ToDictionary(Function(g) g.Key,
                              Function(g) g.Sum(Function(t) t.Montant))

            Return New ResultatImport With {
                .Transactions = valides,
                .Rejetees = rejetees,
                .TotauxParCategorie = totaux,
                .TotalGeneral = valides.Sum(Function(t) t.Montant)
            }
        End Function

        ' Règle de gestion : montant strictement positif et catégorie connue.
        Private Shared Function EstValide(t As Transaction) As Boolean
            Return t.Montant > 0D AndAlso CategoriesValides.Contains(t.Categorie)
        End Function
    End Class

End Namespace
