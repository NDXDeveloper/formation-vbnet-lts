' ============================================================================
'  Section 11.2 : VB6 → VB.NET (faux-amis et constructions obsolètes)
'  Description : Démonstration vérifiable des équivalents VB.NET corrects. Les
'                pièges VB6 sont SILENCIEUX (aucune erreur de compilation) : ce
'                programme rend leurs équivalents VB.NET explicites et observe
'                les comportements (tailles, ByRef/ByVal, base 0, etc.).
'  Fichier source : 02-vb6-vers-vbnet.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Module Program

    Private _compteurDeclenchements As Integer

    Sub Main()
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 11.2 VB6 → VB.NET : faux-amis et équivalents ===")
        Console.WriteLine()

        ' 1) Tailles des entiers : le piège le plus insidieux.
        Console.WriteLine("[Tailles] correspondance VB6 → VB.NET")
        Console.WriteLine($"  VB6 Integer (16 bits) → VB.NET Short   : {Marshal.SizeOf(Of Short)()} octets")
        Console.WriteLine($"  VB6 Long    (32 bits) → VB.NET Integer : {Marshal.SizeOf(Of Integer)()} octets")
        Console.WriteLine($"  VB.NET Long           (64 bits)        : {Marshal.SizeOf(Of Long)()} octets")
        Console.WriteLine($"  VB6 Currency          → VB.NET Decimal : {Marshal.SizeOf(Of Decimal)()} octets")
        Console.WriteLine()

        ' 2) Passage de paramètres : ByVal par défaut en VB.NET (ByRef en VB6).
        Console.WriteLine("[Paramètres] défaut = ByVal (VB6 : ByRef)")
        Dim v As Integer = 10
        DoublerByVal(v)
        Console.WriteLine($"  après DoublerByVal : v = {v}  (inchangé)")
        DoublerByRef(v)
        Console.WriteLine($"  après DoublerByRef : v = {v}  (modifié)")
        Console.WriteLine()

        ' 3) Déclaration multiple : en VB.NET, x ET y sont Integer (VB6 : x serait Variant).
        Dim x, y As Integer
        x = 40000 : y = 2
        Console.WriteLine($"[Dim x, y As Integer] x={x} ({x.GetType().Name}), y={y} ({y.GetType().Name})")
        Console.WriteLine()

        ' 4) Tableaux : base 0 obligatoire en VB.NET.
        Dim a As Integer() = {10, 20, 30}
        Console.WriteLine($"[Tableau] base 0 : a(0)={a(0)}, borne sup={a.GetUpperBound(0)}, longueur={a.Length}")
        Console.WriteLine()

        ' 5) Variant → Object.
        Dim o As Object = "texte"
        o = 42
        Console.WriteLine($"[Variant → Object] contient {o} ({o.GetType().Name})")
        Console.WriteLine()

        ' 6) Type … End Type → Structure.
        Dim p As New Point3D With {.X = 1, .Y = 2, .Z = 3}
        Console.WriteLine($"[Type → Structure] Point3D = {p}")
        Console.WriteLine()

        ' 7) Collection (VB6) → List(Of T).
        Dim noms As New List(Of String) From {"Alice", "Bob"}
        noms.Add("Charlie")
        Console.WriteLine($"[Collection → List] {noms.Count} éléments : {String.Join(", ", noms)}")
        Console.WriteLine()

        ' 8) On Error GoTo → Try / Catch.
        Console.WriteLine($"[On Error → Try/Catch] 10 \ 0 → {DiviserEnSecurite(10, 0)}")
        Console.WriteLine($"                       10 \ 2 → {DiviserEnSecurite(10, 2)}")
        Console.WriteLine()

        ' 9) « Control array » reconstruit : gestionnaire partagé via AddHandler.
        Dim capteurs As New List(Of Capteur) From {New Capteur("A"), New Capteur("B"), New Capteur("C")}
        For Each c In capteurs
            AddHandler c.Declenche, AddressOf OnCapteurDeclenche
        Next
        For Each c In capteurs
            c.Activer()
        Next
        Console.WriteLine($"[Control array → AddHandler] {_compteurDeclenchements} déclenchement(s) sur {capteurs.Count} capteurs")
        Console.WriteLine()

        Console.WriteLine("Terminé.")
    End Sub

    Private Sub DoublerByVal(n As Integer)            ' ByVal implicite
        n = n * 2
    End Sub

    Private Sub DoublerByRef(ByRef n As Integer)      ' ByRef explicite
        n = n * 2
    End Sub

    ' Remplace On Error GoTo / Resume Next par une gestion structurée.
    Private Function DiviserEnSecurite(a As Integer, b As Integer) As String
        Try
            Return (a \ b).ToString()
        Catch ex As DivideByZeroException
            Return "erreur gérée (division par zéro)"
        End Try
    End Function

    Private Sub OnCapteurDeclenche(sender As Object, e As EventArgs)
        _compteurDeclenchements += 1
    End Sub

End Module
