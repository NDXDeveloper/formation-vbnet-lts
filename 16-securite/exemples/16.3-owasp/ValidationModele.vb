' ============================================================================
'  Section 16.3 : OWASP — validation d'entrée (DataAnnotations, liste blanche)
'  Description : Un DTO annoté (Required, StringLength, EmailAddress, Range)
'                décrit CE QUI EST AUTORISÉ (liste blanche). Validator.TryValidate-
'                Object applique ces contraintes CÔTÉ SERVEUR. Dans une Web API
'                <ApiController>, la même mécanique renvoie automatiquement un 400.
'                La validation client n'est jamais une sécurité.
'  Fichier source : 03-owasp.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Linq

''' <summary>Modèle d'inscription validé en liste blanche.</summary>
Public Class CompteDto

    <Required(ErrorMessage:="Le nom est obligatoire.")>
    <StringLength(20, MinimumLength:=3, ErrorMessage:="Le nom doit faire de 3 à 20 caractères.")>
    Public Property Nom As String = ""

    <Required(ErrorMessage:="Le courriel est obligatoire.")>
    <EmailAddress(ErrorMessage:="Le courriel est invalide.")>
    Public Property Courriel As String = ""

    <Range(18, 120, ErrorMessage:="L'âge doit être compris entre 18 et 120.")>
    Public Property Age As Integer

End Class

Public Module ValidationModele

    ''' <summary>Renvoie la liste des messages d'erreur (vide si l'objet est valide).</summary>
    Public Function Valider(modele As Object) As List(Of String)
        Dim contexte As New ValidationContext(modele)
        Dim resultats As New List(Of ValidationResult)()
        Validator.TryValidateObject(modele, contexte, resultats, validateAllProperties:=True)
        Return resultats.Select(Function(r) r.ErrorMessage).ToList()
    End Function

End Module
