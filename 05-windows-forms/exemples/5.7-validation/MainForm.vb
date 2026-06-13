' ============================================================================
'  Section 5.7 : Validation (ErrorProvider, DataAnnotations, règles personnalisées)
'  Description : Le pont DataAnnotations ↔ ErrorProvider de la section : on
'                valide le MODÈLE avec Validator.TryValidateObject (règles
'                déclaratives), puis on reporte chaque erreur sur le contrôle
'                WinForms correspondant via l'ErrorProvider (NameOf pour
'                relier propriété et contrôle, sans chaîne magique).
'                ValidateChildren / e.Cancel et CausesValidation=False sont
'                aussi illustrés. Le journal d'auto-test vérifie le nombre
'                d'erreurs et les messages sur des cas valide / invalide.
'  Fichier source : 07-validation.md
' ============================================================================

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Linq
Imports System.Windows.Forms

Public Class MainForm

    Private Const JOURNAL As String = "5.7-validation-autotest.log"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ExecuterAutoTests()
        AutoFermeture.Activer(Me)
    End Sub

    ' ---- Bouton Enregistrer : valide puis reporte les erreurs ----
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click
        Dim client = LireFormulaire()
        If ValiderEtAfficher(client) Then
            MessageBox.Show("Client valide : enregistrement possible.", "OK")
        End If
    End Sub

    Private Function LireFormulaire() As Client
        Return New Client With {
            .Nom = txtNom.Text,
            .Email = txtEmail.Text,
            .Age = CInt(numAge.Value),
            .TypeClient = "Particulier",
            .CodePostal = txtCodePostal.Text
        }
    End Function

    ' ---- Le pont DataAnnotations ↔ ErrorProvider (extrait de la section) ----
    Private Function ValiderEtAfficher(client As Client) As Boolean
        errProvider.Clear()

        Dim contexte As New ValidationContext(client)
        Dim resultats As New List(Of ValidationResult)
        Dim estValide = Validator.TryValidateObject(client, contexte, resultats, True)

        Dim controles As New Dictionary(Of String, Control) From {
            {NameOf(Client.Nom), txtNom},
            {NameOf(Client.Email), txtEmail},
            {NameOf(Client.Age), numAge},
            {NameOf(Client.CodePostal), txtCodePostal}
        }

        For Each r In resultats
            For Each membre In r.MemberNames
                Dim ctl As Control = Nothing
                If controles.TryGetValue(membre, ctl) Then
                    errProvider.SetError(ctl, r.ErrorMessage)
                End If
            Next
        Next

        Return estValide
    End Function

    ' ---- Vérification automatique des règles (journal) ----
    Private Sub ExecuterAutoTests()
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        ' Cas invalide (le cas type de la section)
        Dim invalide As New Client With {.Nom = "A", .Email = "pasunemail", .Age = 200,
                                         .TypeClient = "Particulier", .CodePostal = "ABC"}
        Dim resI As New List(Of ValidationResult)
        Dim okI = Validator.TryValidateObject(invalide, New ValidationContext(invalide), resI, True)
        AutoFermeture.Journaliser(JOURNAL, $"Client invalide -> valide ? {okI} ; {resI.Count} erreur(s) :")
        For Each r In resI
            AutoFermeture.Journaliser(JOURNAL, $"  [{String.Join(",", r.MemberNames)}] {r.ErrorMessage}")
        Next

        ' Cas valide
        Dim valide As New Client With {.Nom = "Dupont", .Email = "d@exemple.fr", .Age = 30,
                                       .TypeClient = "Entreprise", .CodePostal = "75001"}
        Dim resV As New List(Of ValidationResult)
        Dim okV = Validator.TryValidateObject(valide, New ValidationContext(valide), resV, True)
        AutoFermeture.Journaliser(JOURNAL, $"Client valide -> valide ? {okV} ; {resV.Count} erreur(s)")

        ' Validation inter-propriétés (IValidatableObject)
        Dim resa As New Reservation With {.DateDebut = New DateTime(2026, 6, 10),
                                          .DateFin = New DateTime(2026, 6, 1)}
        Dim resR As New List(Of ValidationResult)
        Dim okR = Validator.TryValidateObject(resa, New ValidationContext(resa), resR, True)
        AutoFermeture.Journaliser(JOURNAL, $"Réservation (fin < début) -> valide ? {okR} ; {resR.Count} erreur(s) : {resR.FirstOrDefault()?.ErrorMessage}")
    End Sub

End Class
