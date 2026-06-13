' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Le contrat du service métier, injecté dans le contrôleur via
'                le conteneur d'injection de dépendances intégré (AddScoped).
'                ExisteAsync sert la démonstration Problem Details (409).
'                ListerEnrichiAsync sert la démonstration de versioning (v2).
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Threading.Tasks

Public Interface IProduitService
    Function ListerAsync() As Task(Of IReadOnlyList(Of Produit))
    Function ListerEnrichiAsync() As Task(Of IReadOnlyList(Of Produit))
    Function ObtenirAsync(id As Integer) As Task(Of Produit)
    Function ExisteAsync(nom As String) As Task(Of Boolean)
    Function CreerAsync(nouveau As CreationProduit) As Task(Of Produit)
    Function MettreAJourAsync(id As Integer, maj As MiseAJourProduit) As Task(Of Boolean)
    Function SupprimerAsync(id As Integer) As Task
End Interface
