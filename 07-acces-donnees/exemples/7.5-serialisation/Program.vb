' ============================================================================
'  Section 7.5 : Sérialisation
'  Description : Exemple complet reprenant les trois formats de la section :
'                  · JSON (System.Text.Json) : Serialize/Deserialize, options
'                    (WriteIndented, CamelCase), attributs (JsonPropertyName,
'                    JsonIgnore), DOM (JsonNode), et async sur un flux ;
'                  · XML : XmlSerializer (par attributs) ET les LITTÉRAUX XML
'                    natifs de VB.NET avec propriétés d'axe (.<élément>,
'                    .@attribut) — un atout du langage absent de C# ;
'                  · CSV : CsvHelper (écriture + lecture typée) et
'                    TextFieldParser (lecture intégrée VB), avec le piège de
'                    la culture (séparateur ; et point décimal invariant).
'  Fichier source : 05-serialisation.md
'  Compilation    : dotnet run   (restaure CsvHelper)
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports System.Text.Json.Serialization
Imports System.Xml.Linq
Imports System.Xml.Serialization
Imports CsvHelper
Imports Microsoft.VisualBasic.FileIO

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        DemoJson()
        Await DemoJsonAsync()
        DemoXmlSerializer()
        DemoLitterauxXml()
        DemoCsvHelper()
        DemoTextFieldParser()
    End Function

    ' ---- JSON : System.Text.Json ---------------------------------------------
    Private Sub DemoJson()
        Console.WriteLine("== JSON (System.Text.Json) ==")

        Dim client As New ClientJson With {.Id = 1, .Nom = "Martin", .MotDePasse = "secret"}

        ' Sérialisation simple
        Console.WriteLine($"Brut    : {JsonSerializer.Serialize(client)}")

        ' Avec options + attributs (<JsonPropertyName>, <JsonIgnore>)
        Dim options As New JsonSerializerOptions With {
            .WriteIndented = False,
            .PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }
        Dim json = JsonSerializer.Serialize(client, options)
        Console.WriteLine($"Options : {json}   (MotDePasse ignoré, 'identifiant' renommé)")

        ' Désérialisation (round-trip)
        Dim relu = JsonSerializer.Deserialize(Of ClientJson)(json, options)
        Console.WriteLine($"Relu    : Id={relu.Id}, Nom={relu.Nom} — round-trip OK : {relu.Nom = client.Nom}")

        ' DOM : JsonNode (sans type fixe)
        Dim noeud = JsonNode.Parse(json)
        Console.WriteLine($"DOM     : nom via JsonNode = {noeud("nom").GetValue(Of String)()}")
    End Sub

    Private Async Function DemoJsonAsync() As Task
        Console.WriteLine()
        Console.WriteLine("== JSON asynchrone sur un flux ==")
        Dim chemin = Path.Combine(Path.GetTempPath(), "serial-client.json")
        Dim client As New ClientJson With {.Id = 7, .Nom = "Durand"}

        Using flux = File.Create(chemin)
            Await JsonSerializer.SerializeAsync(flux, client)
        End Using
        Using flux = File.OpenRead(chemin)
            Dim relu = Await JsonSerializer.DeserializeAsync(Of ClientJson)(flux)
            Console.WriteLine($"Écrit puis relu depuis {Path.GetFileName(chemin)} : {relu.Nom}")
        End Using
        File.Delete(chemin)
    End Function

    ' ---- XML : XmlSerializer --------------------------------------------------
    Private Sub DemoXmlSerializer()
        Console.WriteLine()
        Console.WriteLine("== XML (XmlSerializer) ==")
        Dim client As New ClientXml With {.Id = 1, .Nom = "Martin"}
        Dim serialiseur As New XmlSerializer(GetType(ClientXml))

        Dim sb As New Text.StringBuilder()
        Using writer = New StringWriter(sb)
            serialiseur.Serialize(writer, client)
        End Using
        ' On affiche la ligne de l'élément racine (l'en-tête XML occupe la 1re ligne)
        Console.WriteLine("XML produit (extrait) : " & sb.ToString().Split(Chr(10)).First(Function(l) l.Contains("ClientXml")).Trim())

        Using reader = New StringReader(sb.ToString())
            Dim relu = CType(serialiseur.Deserialize(reader), ClientXml)
            Console.WriteLine($"Relu : Id={relu.Id}, Nom={relu.Nom} — round-trip OK : {relu.Nom = client.Nom}")
        End Using
    End Sub

    ' ---- Littéraux XML VB.NET (atout du langage) ------------------------------
    Private Sub DemoLitterauxXml()
        Console.WriteLine()
        Console.WriteLine("== Littéraux XML natifs de VB.NET (absents de C#) ==")

        ' Du XML directement dans le code
        Dim doc = <clients>
                      <client id="1">
                          <nom>Martin</nom>
                          <email>martin@exemple.fr</email>
                      </client>
                      <client id="2">
                          <nom>Durand</nom>
                          <email>durand@exemple.fr</email>
                      </client>
                  </clients>

        ' Propriétés d'axe : .<élément> et .@attribut
        Console.WriteLine($"Premier nom (.<client>.<nom>.Value) : {doc.<client>.<nom>.Value}")
        For Each c In doc.<client>
            Console.WriteLine($"  client {c.@id} : {c.<nom>.Value} <{c.<email>.Value}>")
        Next
    End Sub

    ' ---- CSV : CsvHelper ------------------------------------------------------
    Private Sub DemoCsvHelper()
        Console.WriteLine()
        Console.WriteLine("== CSV (CsvHelper) ==")
        Dim chemin = Path.Combine(Path.GetTempPath(), "serial-clients.csv")
        Dim clients As New List(Of ClientJson) From {
            New ClientJson With {.Id = 1, .Nom = "Martin"},
            New ClientJson With {.Id = 2, .Nom = "Durand"}
        }

        ' Écriture (culture invariante : point décimal, évite le piège français)
        Using writer = New StreamWriter(chemin),
              csv = New CsvWriter(writer, CultureInfo.InvariantCulture)
            csv.WriteRecords(clients)
        End Using

        ' Lecture vers objets typés
        Using reader = New StreamReader(chemin),
              csv = New CsvReader(reader, CultureInfo.InvariantCulture)
            Dim relus = csv.GetRecords(Of ClientJson)().ToList()
            Console.WriteLine($"Écrits puis relus : {relus.Count} clients ({String.Join(", ", relus.Select(Function(c) c.Nom))})")
        End Using
        File.Delete(chemin)
    End Sub

    ' ---- CSV : TextFieldParser (intégré VB) -----------------------------------
    Private Sub DemoTextFieldParser()
        Console.WriteLine()
        Console.WriteLine("== CSV « européen » avec TextFieldParser (point-virgule) ==")
        Dim chemin = Path.Combine(Path.GetTempPath(), "serial-euro.csv")
        ' CSV à séparateur ';' (Excel français), avec un champ entre guillemets contenant un ';'
        File.WriteAllText(chemin, "id;libelle;prix" & Environment.NewLine &
                                  "1;""Clavier; AZERTY"";49,90" & Environment.NewLine &
                                  "2;Écran;220,00" & Environment.NewLine)

        Using parseur As New TextFieldParser(chemin)
            parseur.TextFieldType = FieldType.Delimited
            parseur.SetDelimiters(";")
            parseur.HasFieldsEnclosedInQuotes = True
            parseur.ReadLine()   ' saute l'en-tête
            While Not parseur.EndOfData
                Dim champs = parseur.ReadFields()   ' gère les guillemets
                Console.WriteLine($"  id={champs(0)}, libelle=""{champs(1)}"", prix={champs(2)}")
            End While
        End Using
        File.Delete(chemin)
    End Sub

End Module

' ---- Types de démonstration ----
Public Class ClientJson
    <JsonPropertyName("identifiant")>
    Public Property Id As Integer
    Public Property Nom As String

    <JsonIgnore>
    Public Property MotDePasse As String   ' jamais sérialisé
End Class

Public Class ClientXml
    <XmlAttribute>
    Public Property Id As Integer
    <XmlElement>
    Public Property Nom As String
End Class
