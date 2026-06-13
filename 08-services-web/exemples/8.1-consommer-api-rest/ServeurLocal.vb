' ============================================================================
'  Section 8.1 : Consommer des API REST
'  Description : Un petit serveur HTTP LOCAL (System.Net.HttpListener, intégré
'                à .NET) qui sert du JSON — uniquement pour que l'exemple soit
'                exécutable hors ligne. Ce n'est PAS le sujet de la section
'                (l'exposition par contrôleurs est traitée en 8.2) : il tient
'                simplement lieu d'« API distante » à consommer.
'  Fichier source : 01-consommer-api-rest.md
' ============================================================================

Imports System.Collections.Generic
Imports System.Net
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Public Class ServeurLocal
    Implements IDisposable

    Private ReadOnly _ecouteur As New HttpListener()
    Private ReadOnly _produits As New List(Of Produit) From {
        New Produit With {.Id = 1, .Nom = "Clavier", .Prix = 49.9D, .EnStock = True},
        New Produit With {.Id = 2, .Nom = "Écran", .Prix = 220D, .EnStock = True},
        New Produit With {.Id = 3, .Nom = "Souris", .Prix = 25D, .EnStock = False}
    }
    Private _prochainId As Integer = 4

    Public ReadOnly Property AdresseBase As String

    Public Sub New(port As Integer)
        AdresseBase = $"http://localhost:{port}/"
        _ecouteur.Prefixes.Add(AdresseBase)
    End Sub

    Public Sub Demarrer()
        _ecouteur.Start()
        ' Boucle de traitement sur un thread de fond
        Dim t As New Thread(AddressOf Boucle) With {.IsBackground = True}
        t.Start()
    End Sub

    Private Sub Boucle()
        While _ecouteur.IsListening
            Dim contexte As HttpListenerContext
            Try
                contexte = _ecouteur.GetContext()
            Catch
                Return   ' écouteur arrêté
            End Try
            Traiter(contexte)
        End While
    End Sub

    Private Sub Traiter(contexte As HttpListenerContext)
        Dim chemin = contexte.Request.Url.AbsolutePath.TrimEnd("/"c)
        Dim methode = contexte.Request.HttpMethod

        Try
            If methode = "GET" AndAlso chemin = "/api/produits" Then
                Repondre(contexte, 200, JsonSerializer.Serialize(_produits))

            ElseIf methode = "GET" AndAlso chemin.StartsWith("/api/produits/") Then
                Dim id = Integer.Parse(chemin.Substring("/api/produits/".Length))
                Dim p = _produits.Find(Function(x) x.Id = id)
                If p Is Nothing Then
                    Repondre(contexte, 404, "")
                Else
                    Repondre(contexte, 200, JsonSerializer.Serialize(p))
                End If

            ElseIf methode = "POST" AndAlso chemin = "/api/produits" Then
                Dim corps As String
                Using lecteur As New IO.StreamReader(contexte.Request.InputStream)
                    corps = lecteur.ReadToEnd()
                End Using
                Dim nouveau = JsonSerializer.Deserialize(Of Produit)(corps, New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True})
                nouveau.Id = _prochainId
                _prochainId += 1
                _produits.Add(nouveau)
                Repondre(contexte, 201, JsonSerializer.Serialize(nouveau))

            Else
                Repondre(contexte, 404, "")
            End If
        Catch ex As Exception
            Repondre(contexte, 500, ex.Message)
        End Try
    End Sub

    Private Sub Repondre(contexte As HttpListenerContext, code As Integer, corps As String)
        contexte.Response.StatusCode = code
        contexte.Response.ContentType = "application/json; charset=utf-8"
        Dim octets = Encoding.UTF8.GetBytes(corps)
        contexte.Response.ContentLength64 = octets.Length
        contexte.Response.OutputStream.Write(octets, 0, octets.Length)
        contexte.Response.OutputStream.Close()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _ecouteur.IsListening Then _ecouteur.Stop()
        _ecouteur.Close()
    End Sub
End Class
