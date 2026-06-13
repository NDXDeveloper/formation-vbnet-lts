' ============================================================================
'  Section 11.6 : Moderniser — injection de dépendances et testabilité
'  Description : La dépendance statique non testable (DateTime.Now) est
'                remplacée par une ABSTRACTION injectée (IHorloge) : une COUTURE
'                où insérer une horloge fixe en test. ServiceSalutation reçoit
'                ses dépendances par constructeur (DI), au lieu de les créer en
'                dur, et expose une méthode ASYNCHRONE pour les E/S.
'  Fichier source : 06-moderniser.md
' ============================================================================

Imports System
Imports System.Threading.Tasks

' Abstraction : la couture testable qui remplace DateTime.Now en dur.
Public Interface IHorloge
    Function Maintenant() As DateOnly
End Interface

' Implémentation de production.
Public Class HorlogeSysteme
    Implements IHorloge
    Public Function Maintenant() As DateOnly Implements IHorloge.Maintenant
        Return DateOnly.FromDateTime(DateTime.Now)
    End Function
End Class

' Implémentation de test : date figée → sorties déterministes.
Public Class HorlogeFixe
    Implements IHorloge
    Private ReadOnly _date As DateOnly
    Public Sub New(dateFixe As DateOnly)
        _date = dateFixe
    End Sub
    Public Function Maintenant() As DateOnly Implements IHorloge.Maintenant
        Return _date
    End Function
End Class

' Service métier : dépendances injectées par constructeur (plus de New en dur).
Public Class ServiceSalutation
    Private ReadOnly _horloge As IHorloge

    Public Sub New(horloge As IHorloge)
        _horloge = horloge
    End Sub

    Public Function Saluer(nom As String) As String
        Return $"Bonjour {nom}, nous sommes le {_horloge.Maintenant():yyyy-MM-dd}"
    End Function

    ' Async : E/S simulée, sans bloquer le thread appelant.
    Public Async Function ChargerMessageAsync() As Task(Of String)
        Await Task.Delay(10)
        Return "message chargé de façon asynchrone"
    End Function
End Class
