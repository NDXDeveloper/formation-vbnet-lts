' ============================================================================
'  Section 3.8 : Réflexion et attributs
'  Description : Les DataAnnotations exactes de la section (<Required>,
'                <StringLength(50, MinimumLength:=2)>, <Range(18, 120)>),
'                avec en complément (exemple ajouté) leur exploitation réelle
'                par Validator.TryValidateObject — qui relit précisément ces
'                attributs par réflexion.
'  Fichier source : 08-reflexion-attributs.md
' ============================================================================

Imports System.ComponentModel.DataAnnotations

Public Class Inscription
    <Required>
    <StringLength(50, MinimumLength:=2)>
    Public Property Nom As String

    <Range(18, 120)>
    Public Property Age As Integer
End Class
