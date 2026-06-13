' ============================================================================
'  Section 13.5 : Couverture de code — système sous test
'  Description : Doubler est testée ; Tripler ne l'est pas. La couverture de
'                lignes/méthodes sera donc partielle, signalant la zone non
'                testée (Tripler).
'  Fichier source : 05-couverture-tests-ia.md
' ============================================================================

Public Class Operations
    Public Function Doubler(x As Integer) As Integer
        Return x * 2
    End Function

    Public Function Tripler(x As Integer) As Integer   ' non couverte par les tests
        Return x * 3
    End Function
End Class
