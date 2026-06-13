' ============================================================================
'  Section 2.1 : Structure d'un programme ; Option Strict / Explicit / Infer / Compare
'  Description : Les procédures exactes de la section — Sub (action) et
'                Function (valeur de retour), avec les modificateurs de
'                paramètres : ByRef, Optional (valeur par défaut) et
'                ParamArray (nombre variable d'arguments).
'  Fichier source : 01-structure-options.md
' ============================================================================

Imports System
Imports System.Linq

Namespace MonApplication

    Public Module Procedures

        ''' <summary>Une Sub effectue une action sans renvoyer de valeur.</summary>
        Sub Afficher(message As String)
            Console.WriteLine(message)
        End Sub

        ''' <summary>Une Function renvoie une valeur via Return.</summary>
        Function Carre(n As Integer) As Integer
            Return n * n
        End Function

        ''' <summary>ByRef : la procédure reçoit la variable elle-même.</summary>
        Sub Doubler(ByRef valeur As Integer)
            valeur *= 2
        End Sub

        ''' <summary>Optional : paramètre facultatif, valeur par défaut obligatoire.</summary>
        Function Formater(texte As String, Optional majuscules As Boolean = False) As String
            Return If(majuscules, texte.ToUpper(), texte)
        End Function

        ''' <summary>ParamArray : le dernier paramètre accepte un nombre variable d'arguments.</summary>
        Function Somme(ParamArray valeurs() As Integer) As Integer
            Return valeurs.Sum()
        End Function

    End Module

End Namespace
