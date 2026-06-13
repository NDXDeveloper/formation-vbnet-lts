' ============================================================================
'  Section 13.1 : Tests unitaires — système sous test
'  Description : Le code à tester. Diviser utilise la division entière \ : elle
'                lève DivideByZeroException si le diviseur est 0 (cas du test
'                d'exception). ServiceCatalogue.ChargerAsync sert le test
'                asynchrone.
'  Fichier source : 01-tests-unitaires.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Threading.Tasks

Public Class Calculatrice
    Public Function Additionner(a As Integer, b As Integer) As Integer
        Return a + b
    End Function

    Public Function Diviser(a As Integer, b As Integer) As Integer
        Return a \ b   ' DivideByZeroException si b = 0
    End Function
End Class

Public Class ServiceCatalogue
    Public Async Function ChargerAsync() As Task(Of IReadOnlyList(Of String))
        Await Task.Delay(10)
        Return New List(Of String) From {"Clavier", "Écran"}
    End Function
End Class
