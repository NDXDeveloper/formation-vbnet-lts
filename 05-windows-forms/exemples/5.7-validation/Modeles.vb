' ============================================================================
'  Section 5.7 : Validation (ErrorProvider, DataAnnotations, règles personnalisées)
'  Description : Les modèles annotés de la section :
'                  · Client avec DataAnnotations (Required, Length,
'                    EmailAddress, Range, AllowedValues) + le code postal validé
'                    par un attribut personnalisé ;
'                  · CodePostalFrAttribute : attribut de validation réutilisable
'                    (hérite de ValidationAttribute, redéfinit IsValid) ;
'                  · Reservation : validation INTER-propriétés via
'                    IValidatableObject.
'  Fichier source : 07-validation.md
' ============================================================================

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Linq
Imports System.Text.RegularExpressions

Public Class Client

    <Required(ErrorMessage:="Le nom est obligatoire.")>
    <Length(2, 80, ErrorMessage:="Le nom doit comporter de 2 à 80 caractères.")>
    Public Property Nom As String

    <Required>
    <EmailAddress(ErrorMessage:="Adresse e-mail invalide.")>
    Public Property Email As String

    <Range(0, 120, ErrorMessage:="L'âge doit être compris entre 0 et 120.")>
    Public Property Age As Integer

    <AllowedValues("Particulier", "Entreprise", ErrorMessage:="Type de client inconnu.")>
    Public Property TypeClient As String

    <CodePostalFr()>
    Public Property CodePostal As String

End Class

''' <summary>Règle réutilisable : un code postal français (5 chiffres).</summary>
Public Class CodePostalFrAttribute
    Inherits ValidationAttribute

    Protected Overrides Function IsValid(value As Object,
                                         context As ValidationContext) As ValidationResult
        Dim texte = TryCast(value, String)

        If Not String.IsNullOrEmpty(texte) AndAlso Not Regex.IsMatch(texte, "^\d{5}$") Then
            Return New ValidationResult("Le code postal doit comporter 5 chiffres.",
                                        {context.MemberName})
        End If

        Return ValidationResult.Success
    End Function
End Class

''' <summary>Validation inter-propriétés (IValidatableObject).</summary>
Public Class Reservation
    Implements IValidatableObject

    Public Property DateDebut As DateTime
    Public Property DateFin As DateTime

    Public Function Validate(context As ValidationContext) As IEnumerable(Of ValidationResult) _
            Implements IValidatableObject.Validate

        If DateFin < DateDebut Then
            Return {New ValidationResult("La date de fin doit suivre la date de début.",
                                         {NameOf(DateFin)})}
        End If

        Return Enumerable.Empty(Of ValidationResult)()
    End Function
End Class
