' ============================================================================
'  Section 11.2 : VB6 → VB.NET (faux-amis et constructions obsolètes)
'  Description : Types de support. Point3D illustre « Type … End Type » (UDT
'                VB6) devenu « Structure … End Structure ». Capteur illustre la
'                reconstruction d'un « control array » VB6 : plusieurs objets
'                partageant UN gestionnaire d'évènement, câblé par AddHandler
'                (VB.NET n'ayant pas d'équivalent natif des control arrays).
'  Fichier source : 02-vb6-vers-vbnet.md
' ============================================================================

' VB6 : Type Point3D ... End Type  →  VB.NET : Structure
Public Structure Point3D
    Public X As Integer
    Public Y As Integer
    Public Z As Integer

    Public Overrides Function ToString() As String
        Return $"({X}, {Y}, {Z})"
    End Function
End Structure

' Remplace un « control array » : objets distincts, gestionnaire partagé via AddHandler.
Public Class Capteur
    Public ReadOnly Property Nom As String
    Public Event Declenche(sender As Object, e As EventArgs)

    Public Sub New(nom As String)
        _Nom = nom
    End Sub

    Public Sub Activer()
        RaiseEvent Declenche(Me, EventArgs.Empty)
    End Sub
End Class
