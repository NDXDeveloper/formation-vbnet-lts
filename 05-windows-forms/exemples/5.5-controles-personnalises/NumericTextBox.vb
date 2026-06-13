' ============================================================================
'  Section 5.5 : Contrôles personnalisés et UserControl
'  Description : Le contrôle par HÉRITAGE de la section : une TextBox qui
'                n'accepte que des chiffres. On redéfinit la méthode protégée
'                OnKeyPress (point d'extension canonique) et l'on appelle
'                MyBase.OnKeyPress(e) — sans quoi le comportement hérité
'                serait cassé.
'  Fichier source : 05-controles-personnalises.md
' ============================================================================

Public Class NumericTextBox
    Inherits TextBox

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        ' Laisser passer les chiffres et les touches de contrôle (Retour arrière…)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If

        MyBase.OnKeyPress(e)   ' ne jamais oublier d'appeler la base
    End Sub

End Class
