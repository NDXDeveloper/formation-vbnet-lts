' ============================================================================
'  Section 3.2 : Structures (Structure) et tuples
'  Description : Les structures de la section :
'                  · Point2D — structure simple (champs publics) pour la
'                    démonstration de la sémantique de copie ;
'                  · PointImmuable — la variante ReadOnly + constructeur
'                    paramétré du cours (pas de Sub New() sans paramètre
'                    déclarable dans une Structure !) ;
'                  · Exemple — pas d'initialiseur d'instance : seuls Const
'                    et Shared sont autorisés ;
'                  · Temperature — structure immuable implémentant
'                    IComparable(Of T) (l'approche VB du readonly struct).
'  Fichier source : 02-structures-tuples.md
' ============================================================================

''' <summary>Structure simple : type VALEUR (l'affectation copie les données).</summary>
Public Structure Point2D
    Public X As Integer
    Public Y As Integer
End Structure

''' <summary>
''' Structure immuable : champs ReadOnly + constructeur paramétré.
''' « Public Sub New() » sans paramètre serait une erreur dans une Structure.
''' </summary>
Public Structure PointImmuable
    Public ReadOnly X As Integer
    Public ReadOnly Y As Integer

    Public Sub New(x As Integer, y As Integer)   ' ✓ paramétré
        Me.X = x
        Me.Y = y
    End Sub

    ' Public Sub New()   ' ❌ interdit dans une Structure
End Structure

''' <summary>Pas d'initialiseur sur les membres d'instance d'une structure.</summary>
Public Structure Exemple
    ' Public X As Integer = 0          ' ❌ initialiseur valide uniquement sur Shared/Const
    Public Const Zero As Integer = 0   ' ✓ constante
    Public Shared ReadOnly Defaut As New Exemple   ' ✓ membre partagé
End Structure

''' <summary>Structure immuable + interface : triable par List.Sort.</summary>
Public Structure Temperature
    Implements IComparable(Of Temperature)

    Public ReadOnly Property Celsius As Double

    Public Sub New(celsius As Double)
        Me.Celsius = celsius
    End Sub

    Public Function CompareTo(other As Temperature) As Integer _
            Implements IComparable(Of Temperature).CompareTo
        Return Celsius.CompareTo(other.Celsius)
    End Function

    Public Overrides Function ToString() As String
        Return $"{Celsius} °C"
    End Function
End Structure
