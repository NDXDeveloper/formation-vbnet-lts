' ============================================================================
'  Section 5.4 : Contrôles avancés
'  Description : Types métier de démonstration pour le DataGridView, le
'                TreeView (objet rangé dans Tag) et le ListView.
'  Fichier source : 04-controles-avances.md
' ============================================================================

Imports System.Collections.Generic

Public Class Client
    Public Property Nom As String
    Public Property Ville As String
    Public Property Actif As Boolean
End Class

Public Class Dossier
    Public Property Nom As String
End Class

Public Class Projet
    Public Property Nom As String
    Public Property Dossiers As New List(Of Dossier)
End Class
