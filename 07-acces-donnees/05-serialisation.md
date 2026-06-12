🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 7.5 Sérialisation

> **Module 7 — Accès aux données** · Section 5
> Transformer des objets en formats d'échange et de stockage · JSON (`System.Text.Json`) · XML · CSV

---

## 🧭 Introduction : qu'est-ce que la sérialisation ?

**Sérialiser**, c'est transformer un objet .NET en un **format texte (ou binaire)** que l'on peut stocker dans un fichier, transmettre par le réseau ou ranger en base. **Désérialiser** est l'opération inverse : reconstruire l'objet à partir de ce format. C'est un besoin **transversal** à tout ce module : écrire des données dans un fichier (§ 7.6), échanger avec une API web (module 8), stocker un document JSON en base (§ 7.2)…

Trois formats dominent, chacun avec sa zone de pertinence :

- **JSON** — le **standard moderne** pour les API et la configuration ; léger, lisible, omniprésent.
- **XML** — historique mais toujours présent (legacy, SOAP, fichiers `.config`, formats à schéma).
- **CSV** — pour les **données tabulaires** et les échanges avec les tableurs.

---

## 🟢 1. JSON avec `System.Text.Json` ⭐

`System.Text.Json` est le sérialiseur JSON **intégré** à .NET (aucun paquet à installer), performant et économe en mémoire. C'est le choix recommandé pour tout nouveau code.

**Les bases** — `Serialize` et `Deserialize` :

```vb
Imports System.Text.Json

Public Class Client
    Public Property Id As Integer
    Public Property Nom As String
    Public Property Email As String
End Class

' Objet → JSON
Dim client As New Client With {.Id = 1, .Nom = "Martin", .Email = "martin@exemple.fr"}
Dim json As String = JsonSerializer.Serialize(client)
' → {"Id":1,"Nom":"Martin","Email":"martin@exemple.fr"}

' JSON → objet
Dim restaure As Client = JsonSerializer.Deserialize(Of Client)(json)
```

**Les options** ajustent le rendu (`JsonSerializerOptions`) :

```vb
Imports System.Text.Json.Serialization

Dim options As New JsonSerializerOptions With {
    .WriteIndented = True,                                ' JSON indenté, lisible
    .PropertyNamingPolicy = JsonNamingPolicy.CamelCase,   ' id, nom, email
    .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
}

Dim json = JsonSerializer.Serialize(client, options)
```

**L'asynchrone**, directement sur un flux (voir § 7.6) :

```vb
' Écriture asynchrone vers un fichier
Using flux = File.Create("client.json")
    Await JsonSerializer.SerializeAsync(flux, client, options)
End Using

' Lecture asynchrone depuis un fichier
Using flux = File.OpenRead("client.json")
    Dim restaure = Await JsonSerializer.DeserializeAsync(Of Client)(flux)
End Using
```

**Les attributs** personnalisent le mapping propriété ↔ JSON :

```vb
Imports System.Text.Json.Serialization

Public Class Client
    <JsonPropertyName("identifiant")>
    Public Property Id As Integer

    Public Property Nom As String

    <JsonIgnore>
    Public Property MotDePasse As String   ' jamais sérialisé
End Class
```

**Le DOM** (`JsonNode`) sert quand on n'a pas — ou ne veut pas — de type fixe :

```vb
Imports System.Text.Json.Nodes

Dim noeud = JsonNode.Parse(json)
Dim nom = noeud("nom").GetValue(Of String)()
```

> ### ⚠️ Le générateur de source JSON est réservé à C#
> `System.Text.Json` propose un **générateur de source** (`JsonSerializerContext`) qui produit du code de sérialisation à la compilation, pour les performances et le Native AOT. Comme tous les *source generators*, il **génère du C#** et n'est **pas disponible en VB.NET** (voir Annexe B). En VB, on utilise la sérialisation **par réflexion** (le mode par défaut) — qui fonctionne parfaitement. En pratique, ce n'est pas une perte : le générateur vise surtout le Native AOT et l'optimisation du démarrage, deux terrains hors du périmètre habituel de VB.NET.

> **Et Newtonsoft.Json ?** La bibliothèque `Newtonsoft.Json` (Json.NET) reste **omniprésente dans le code existant** et plus riche dans certains cas limites. Pour du **nouveau** code, préférez `System.Text.Json` (intégré, plus rapide) ; pour de la **maintenance**, vous croiserez très souvent Json.NET.

---

## 🟠 2. XML

Moins en vogue que JSON pour les API, XML reste incontournable en **interopérabilité legacy**, pour **SOAP**, les fichiers de **configuration** et les formats régis par un **schéma (XSD)**.

**`XmlSerializer`** — piloté par attributs, le plus courant :

```vb
Imports System.Xml.Serialization
Imports System.IO

Public Class Client
    <XmlAttribute>
    Public Property Id As Integer
    <XmlElement>
    Public Property Nom As String
End Class

Dim serialiseur As New XmlSerializer(GetType(Client))

' Sérialiser
Using writer = New StreamWriter("client.xml")
    serialiseur.Serialize(writer, client)
End Using

' Désérialiser
Using reader = New StreamReader("client.xml")
    Dim restaure = CType(serialiseur.Deserialize(reader), Client)
End Using
```

`XmlSerializer` exige un **constructeur sans paramètre** sur le type et sérialise les membres publics. Les attributs `XmlRoot`, `XmlElement`, `XmlAttribute`, `XmlArray`/`XmlArrayItem`, `XmlIgnore` contrôlent finement le rendu. Pour un modèle plus explicite (héritage de WCF), il existe aussi `DataContractSerializer` (`<DataContract>` / `<DataMember>`).

> ### ⭐ Un vrai atout de VB.NET : les littéraux XML
> VB.NET possède une fonctionnalité que **C# n'a jamais eue** : les **littéraux XML natifs** et les **propriétés d'axe XML**, adossés à LINQ to XML (`System.Xml.Linq`). On écrit du XML directement dans le code, et on le navigue avec une syntaxe dédiée :

```vb
Imports System.Xml.Linq

' Littéral XML : du XML directement dans le code VB
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

' Propriétés d'axe pour naviguer le document
Dim premierNom = doc.<client>.<nom>.Value        ' "Martin"

For Each c In doc.<client>
    Console.WriteLine($"{c.@id} : {c.<nom>.Value}")   ' @id = attribut, .<nom> = élément enfant
Next
```

> `.<element>` accède aux éléments enfants, `.@attribut` à un attribut, `...<element>` aux descendants. C'est l'un des rares domaines où VB.NET offre une syntaxe **plus expressive** que C# — particulièrement agréable pour générer ou transformer du XML.

**Quand choisir XML ?** Legacy et SOAP, fichiers `.config`, formats documentaires, et tout contexte où un **schéma XSD** fait foi. Pour les API et la configuration modernes, JSON l'a largement emporté — mais XML reste très présent en entreprise et en maintenance.

---

## 🔵 3. CSV

Le CSV (*Comma-Separated Values*) est le format roi des **données tabulaires** et des échanges avec **Excel**. Point essentiel d'emblée :

> ### ⚠️ .NET n'a pas de sérialiseur CSV intégré
> Contrairement à JSON et XML, il n'existe **pas** de sérialiseur CSV dans la bibliothèque standard. Et **n'essayez pas de le faire à la main** avec un simple `Split(",")` : le CSV est plein de pièges (champs contenant des virgules, guillemets échappés, retours à la ligne dans une cellule). Utilisez un outil dédié.

**La bibliothèque de référence : `CsvHelper`** (lecture, écriture, mapping objet) :

```vb
Imports CsvHelper
Imports System.Globalization

' Écrire une collection d'objets
Using writer = New StreamWriter("clients.csv"),
      csv = New CsvWriter(writer, CultureInfo.InvariantCulture)
    csv.WriteRecords(listeClients)
End Using

' Lire vers des objets typés
Using reader = New StreamReader("clients.csv"),
      csv = New CsvReader(reader, CultureInfo.InvariantCulture)
    Dim clients = csv.GetRecords(Of Client)().ToList()
End Using
```

Pour des en-têtes ou des règles particulières, on définit une **carte de classe** (`ClassMap`) :

```vb
Public Class ClientMap
    Inherits ClassMap(Of Client)
    Public Sub New()
        Map(Function(c) c.Id).Name("identifiant")
        Map(Function(c) c.Nom).Name("nom_client")
        Map(Function(c) c.Email).Ignore()
    End Sub
End Class
' csv.Context.RegisterClassMap(Of ClientMap)()
```

> ### ⚠️ Le piège de la culture (crucial en français !)
> Le séparateur de colonnes du CSV est traditionnellement la **virgule**. Or, en culture **française**, la virgule est aussi le **séparateur décimal** (`3,14`). Un montant comme `49,90` risque alors d'être lu comme **deux colonnes** ! Deux parades : préciser explicitement une culture (souvent `CultureInfo.InvariantCulture`, qui emploie le **point** décimal), et/ou utiliser le **point-virgule** comme délimiteur — c'est précisément ce que fait Excel en France pour ses CSV « européens ».

**Une option intégrée, propre à VB.NET** : `TextFieldParser` (du runtime Visual Basic) sait lire des fichiers délimités en gérant correctement les guillemets — pratique pour de la **lecture** simple sans dépendance externe :

```vb
Imports Microsoft.VisualBasic.FileIO

Using parseur As New TextFieldParser("clients.csv")
    parseur.TextFieldType = FieldType.Delimited
    parseur.SetDelimiters(";")                 ' point-virgule (CSV « européen »)
    parseur.HasFieldsEnclosedInQuotes = True

    If Not parseur.EndOfData Then parseur.ReadLine()  ' sauter l'en-tête

    While Not parseur.EndOfData
        Dim champs = parseur.ReadFields()      ' gère les guillemets, renvoie un String()
        Dim id = Integer.Parse(champs(0))
        Dim nom = champs(1)
        ' …
    End While
End Using
```

`TextFieldParser` ne fait que **lire** (pas d'écriture, pas de mapping objet) ; pour un besoin complet (lecture **et** écriture, correspondance vers des classes), `CsvHelper` reste supérieur. Pour de très gros volumes où la performance prime, la bibliothèque **`Sep`** est une alternative récente et particulièrement rapide.

---

## 🧭 Synthèse : quel format pour quel besoin ?

| Format | Quand l'utiliser | Outil |
|---|---|---|
| **JSON** | API web, configuration moderne, échange général, document en base | `System.Text.Json` (intégré) |
| **XML** | Legacy, SOAP, fichiers `.config`, formats à schéma XSD | `XmlSerializer` / littéraux XML + LINQ to XML |
| **CSV** | Données tabulaires, import/export tableur | `CsvHelper` (lecture+écriture) ou `TextFieldParser` (lecture) |

---

## ✅ À retenir

- **Sérialiser** = objet → format de stockage/échange ; **désérialiser** = l'inverse. Besoin transversal à tout le module.
- **JSON** : `System.Text.Json`, **intégré** et recommandé pour le neuf. Le **générateur de source** est C#-only, mais la sérialisation par réflexion suffit en VB. Newtonsoft.Json reste partout dans le legacy.
- **XML** : `XmlSerializer` (par attributs) pour le classique ; et surtout, les **littéraux XML natifs de VB.NET** — un atout réel du langage, absent de C#.
- **CSV** : **pas** de sérialiseur intégré et **jamais** de `Split(",")` naïf. `CsvHelper` est la référence ; `TextFieldParser` est l'option VB intégrée en lecture. **Attention à la culture** (virgule décimale française vs séparateur CSV).

---

> **Prochaine étape →** [§ 7.6 — Fichiers, flux (`Stream`) et E/S](06-fichiers-io.md) : `System.IO` et les flux, sur lesquels repose toute la sérialisation vue ici — lecture/écriture de fichiers, flux de compression et de chiffrement.

⏭️ [Fichiers, flux (Stream) et E/S](/07-acces-donnees/06-fichiers-io.md)
