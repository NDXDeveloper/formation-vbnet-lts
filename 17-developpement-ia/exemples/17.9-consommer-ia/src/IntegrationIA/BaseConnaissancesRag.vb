' ============================================================================
'  Section 17.9 : RAG basique — vectoriser, stocker, récupérer
'  Description : Magasin de vecteurs EN MÉMOIRE. Chaque document est vectorisé puis,
'                pour une question, on récupère le passage le plus pertinent par
'                SIMILARITÉ COSINUS — la brique « retrieval » du RAG. La vectorisation
'                est ici volontairement simple (sac de mots haché, déterministe et
'                hors-ligne) : un vrai système utiliserait un IEmbeddingGenerator de
'                Microsoft.Extensions.AI et une base vectorielle (Cosmos DB, Redis…).
'                Le but est de montrer le PIPELINE, pas la qualité d'embedding.
'  Fichier source : 09-consommer-ia.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic

''' <summary>Petit magasin de connaissances vectorisé en mémoire pour un RAG basique.</summary>
Public NotInheritable Class BaseConnaissancesRag

    Private Const Dimension As Integer = 128
    Private Shared ReadOnly Separateurs As Char() =
        {" "c, ","c, "."c, ";"c, ":"c, "!"c, "?"c, "'"c, "("c, ")"c, ChrW(9), ChrW(10), ChrW(13)}

    Private ReadOnly _documents As New List(Of String)
    Private ReadOnly _vecteurs As New List(Of Double())

    ''' <summary>Indexe un document (le vectorise et le stocke).</summary>
    Public Sub Indexer(document As String)
        _documents.Add(document)
        _vecteurs.Add(Vectoriser(document))
    End Sub

    ''' <summary>Nombre de documents indexés.</summary>
    Public ReadOnly Property Nombre As Integer
        Get
            Return _documents.Count
        End Get
    End Property

    ''' <summary>Renvoie le document le plus proche de la question (similarité cosinus).</summary>
    Public Function Recuperer(question As String) As String
        If _documents.Count = 0 Then Return ""
        Dim vQuestion = Vectoriser(question)
        Dim meilleurIndex As Integer = 0
        Dim meilleurScore As Double = Double.NegativeInfinity
        For i As Integer = 0 To _documents.Count - 1
            Dim score = Cosinus(vQuestion, _vecteurs(i))
            If score > meilleurScore Then
                meilleurScore = score
                meilleurIndex = i
            End If
        Next
        Return _documents(meilleurIndex)
    End Function

    ' --- Vectorisation déterministe (sac de mots haché) ---

    Private Shared Function Vectoriser(texte As String) As Double()
        Dim v(Dimension - 1) As Double
        For Each mot In texte.ToLowerInvariant().Split(Separateurs, StringSplitOptions.RemoveEmptyEntries)
            v(IndiceMot(mot)) += 1.0
        Next
        Return v
    End Function

    ' Hachage stable (sans débordement : Mod à chaque étape) -> indice dans [0, Dimension[.
    Private Shared Function IndiceMot(mot As String) As Integer
        Dim h As Long = 0
        For Each c As Char In mot
            h = (h * 31 + AscW(c)) Mod Dimension
        Next
        Return CInt(h)
    End Function

    Private Shared Function Cosinus(a As Double(), b As Double()) As Double
        Dim produit As Double = 0, normeA As Double = 0, normeB As Double = 0
        For i As Integer = 0 To a.Length - 1
            produit += a(i) * b(i)
            normeA += a(i) * a(i)
            normeB += b(i) * b(i)
        Next
        If normeA = 0 OrElse normeB = 0 Then Return 0
        Return produit / (Math.Sqrt(normeA) * Math.Sqrt(normeB))
    End Function

End Class
