' ============================================================================
'  Section 6.4 : Liaison de données
'  Description : La règle de validation de liaison de la section
'                (ValidationRule) : un champ vide est rejeté avec un message.
'                Attachée via <Binding.ValidationRules> dans le XAML ; un champ
'                en erreur reçoit la bordure rouge par défaut.
'  Fichier source : 04-data-binding.md
' ============================================================================

Imports System.Globalization
Imports System.Windows.Controls   ' ValidationRule, ValidationResult

Public Class ChampRequisRule
    Inherits ValidationRule

    Public Overrides Function Validate(value As Object,
                                       culture As CultureInfo) As ValidationResult
        If String.IsNullOrWhiteSpace(TryCast(value, String)) Then
            Return New ValidationResult(False, "Ce champ est obligatoire.")
        End If
        Return ValidationResult.ValidResult
    End Function
End Class
