' ============================================================================
'  Section 2.1 : Structure d'un programme ; Option Strict / Explicit / Infer / Compare
'  Description : Exemple complet reprenant l'« anatomie d'un programme VB.NET »
'                de la section (directives Option, puis Imports, puis types
'                dans un Namespace), les procédures (Sub / Function, ByRef,
'                Optional, ParamArray — voir Procedures.vb), la continuation
'                de ligne, les conversions sous Option Strict On,
'                l'inférence Option Infer et l'effet d'Option Compare
'                (surchargé par fichier dans ComparaisonsTexte.vb).
'  Fichier source : 01-structure-options.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Option Strict On
Option Explicit On
Option Infer On
Option Compare Binary

Imports System
Imports System.Collections.Generic

Namespace MonApplication

    Module Program

        Sub Main(args As String())
            Console.OutputEncoding = System.Text.Encoding.UTF8

            ' L'anatomie de la section : tout commence ici.
            Console.WriteLine("Bonjour, VB.NET sur .NET 10 !")

            DemoProcedure()
            DemoContinuationDeLigne()
            DemoOptionStrict()
            DemoOptionInfer()
            DemoOptionCompare()
        End Sub

        ' ---- Sub, Function et paramètres (voir Procedures.vb) ---------------
        Private Sub DemoProcedure()
            Console.WriteLine()
            Console.WriteLine("== Sub, Function et paramètres ==")

            Afficher($"Carre(7) = {Carre(7)}")

            Dim x = 10
            Doubler(x)                                     ' ByRef : modifie x chez l'appelant
            Console.WriteLine($"Après Doubler(x) : x = {x}")

            Dim s = Formater("abc", majuscules:=True)      ' argument nommé (name:=)
            Console.WriteLine($"Formater(""abc"", majuscules:=True) = {s}")

            Dim total = Somme(1, 2, 3, 4)                  ' ParamArray
            Console.WriteLine($"Somme(1, 2, 3, 4) = {total}")
        End Sub

        ' ---- Continuation de ligne ------------------------------------------
        Private Sub DemoContinuationDeLigne()
            Console.WriteLine()
            Console.WriteLine("== Continuation de ligne ==")

            Dim valeurA = 1, valeurB = 2, valeurC = 3

            ' Continuation implicite (recommandée) après un opérateur
            Dim total = valeurA +
                        valeurB +
                        valeurC

            ' Continuation explicite avec '_' (héritage, généralement superflu)
            Dim message = "Ligne 1 " & _
                          "Ligne 2"

            Console.WriteLine($"total = {total}")
            Console.WriteLine($"message = {message}")
        End Sub

        ' ---- Option Strict On : conversions ----------------------------------
        Private Sub DemoOptionStrict()
            Console.WriteLine()
            Console.WriteLine("== Option Strict : conversions ==")

            Dim total As Integer = 10
            Dim moyenne As Double = total       ' OK : élargissement (Integer -> Double)
            Console.WriteLine($"Élargissement Integer -> Double : moyenne = {moyenne}")

            Dim x As Double = 3.9
            ' Dim n As Integer = x              ' ERREUR BC30512 (vérifiée) :
            '                                   ' « Option Strict On interdit les conversions
            '                                   '   implicites de 'Double' en 'Integer'. »
            Dim n2 As Integer = CInt(x)         ' OK : conversion explicite (arrondi)
            Console.WriteLine($"Conversion explicite CInt(3.9) = {n2}")

            ' Liaison tardive interdite :
            ' Dim obj As Object = "Bonjour"
            ' Console.WriteLine(obj.Length)     ' ERREUR BC30574 (vérifiée) :
            '                                   ' « Option Strict On rejette toute liaison tardive. »
            Dim texte As String = "Bonjour"     ' liaison anticipée, vérifiée à la compilation
            Console.WriteLine($"texte.Length = {texte.Length}")
        End Sub

        ' ---- Option Infer On : inférence de type ------------------------------
        Private Sub DemoOptionInfer()
            Console.WriteLine()
            Console.WriteLine("== Option Infer ==")

            Dim compteur = 0                    ' inféré : Integer
            Dim noms = New List(Of String)      ' inféré : List(Of String)

            ' Le type inféré est statique et figé — ce n'est pas du typage dynamique.
            Console.WriteLine($"Dim compteur = 0 -> type inféré : {compteur.GetType().Name}")
            Console.WriteLine($"Dim noms = New List(Of String) -> type inféré : {noms.GetType().Name}")
        End Sub

        ' ---- Option Compare : Binary vs Text ----------------------------------
        Private Sub DemoOptionCompare()
            Console.WriteLine()
            Console.WriteLine("== Option Compare ==")

            ' Ce fichier est en Option Compare Binary : sensible à la casse.
            Dim a = ("Apple" = "apple")
            Console.WriteLine($"Binary (ce fichier)  : ""Apple"" = ""apple"" -> {a}")

            ' ComparaisonsTexte.vb surcharge l'option au niveau du FICHIER
            ' (Option Compare Text) : la même comparaison y devient insensible.
            Console.WriteLine($"Text (fichier dédié) : ""Apple"" = ""apple"" -> {EgalEnModeText("Apple", "apple")}")
        End Sub

    End Module

End Namespace
