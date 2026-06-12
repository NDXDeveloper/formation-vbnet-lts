🔝 Retour au [Sommaire](/SOMMAIRE.md)

# 7.6 Fichiers, flux (`Stream`) et E/S

> **Module 7 — Accès aux données** · Section 6 *(dernière du chapitre)*
> `System.IO` · Flux et lecteurs/écrivains · Flux bufferisés, de compression et de chiffrement

---

## 🧭 Introduction : deux niveaux d'accès aux fichiers

.NET propose **deux niveaux** pour travailler avec les fichiers, et savoir lequel choisir est la clé de ce chapitre :

- les **aides de haut niveau** (`File`, `Directory`, `Path`) — pour les cas simples, en une ou deux lignes ;
- les **flux** (`Stream`) — pour le contrôle fin, les **gros volumes** (traitement par morceaux, sans tout charger en mémoire) et surtout la **composition** (compression, chiffrement…).

C'est sur les flux que repose toute la **sérialisation** vue en § 7.5 : `SerializeAsync` écrit dans un `Stream`, un `StreamReader` alimente un parseur, etc. Ce qui suit en est donc le socle.

---

## 📁 1. Les aides de haut niveau : `File`, `Directory`, `Path`

### La classe `File`

Pour lire ou écrire un fichier entier, `File` offre des méthodes directes — avec leurs **variantes asynchrones**, à privilégier dès que le fichier grossit ou que l'interface doit rester réactive (module 4) :

```vb
Imports System.IO

' Synchrone (fichiers courts)
File.WriteAllText("notes.txt", "Bonjour")
Dim contenu As String = File.ReadAllText("notes.txt")

' Asynchrone (recommandé pour gros fichiers / UI réactive)
Await File.WriteAllTextAsync("notes.txt", "Bonjour")
Dim texte = Await File.ReadAllTextAsync("notes.txt")

' Par lignes, ou par octets
Await File.WriteAllLinesAsync("lignes.txt", {"un", "deux", "trois"})
Dim lignes = Await File.ReadAllLinesAsync("lignes.txt")
Dim octets = Await File.ReadAllBytesAsync("image.png")
```

> ### Charger tout, ou ligne à ligne ?
> `ReadAllLines` charge **tout** le fichier dans un tableau en mémoire. Pour un **gros** fichier, préférez `ReadLines`, qui énumère **paresseusement**, ligne par ligne, sans tout charger :
> ```vb
> For Each ligne In File.ReadLines("gros-fichier.log")
>     ' traité au fil de l'eau ; la mémoire reste constante
> Next
> ```
> Sa variante asynchrone `ReadLinesAsync` renvoie un `IAsyncEnumerable(Of String)` — mais rappelez-vous
> ([module 4.6](../04-async/06-async-streams.md)) que VB.NET n'a **pas** d'équivalent du `await foreach` de C# :
> on consomme ce flux **manuellement** (`GetAsyncEnumerator` / `MoveNextAsync`), ou l'on s'en tient au
> `ReadLines` synchrone, paresseux lui aussi, largement suffisant ici.

`File` propose aussi `Exists`, `Delete`, `Copy`, `Move`, `AppendAllText`, et des fabriques de flux (`Create`, `OpenRead`, `OpenWrite`) que l'on retrouve plus bas.

### La classe `Path` — manipuler les chemins

`Path` opère sur des **chaînes** (elle ne touche pas le disque) :

```vb
Dim chemin = Path.Combine("donnees", "2026", "rapport.csv")   ' "donnees/2026/rapport.csv"
Dim nom = Path.GetFileName(chemin)                    ' "rapport.csv"
Dim sansExt = Path.GetFileNameWithoutExtension(chemin) ' "rapport"
Dim ext = Path.GetExtension(chemin)                   ' ".csv"
Dim dossier = Path.GetDirectoryName(chemin)           ' "donnees/2026"
```

> ### ⚠️ Toujours `Path.Combine`, jamais la concaténation
> N'assemblez **jamais** un chemin avec `"donnees\" & sousDossier`. `Path.Combine` gère le bon séparateur selon le système d'exploitation (`\` sous Windows, `/` ailleurs) et les cas limites. C'est une condition de portabilité — et une bonne habitude même en environnement 100 % Windows.

### La classe `Directory`

```vb
Directory.CreateDirectory("export")          ' crée l'arborescence si besoin (idempotent)

' Énumération paresseuse, récursive
For Each fichier In Directory.EnumerateFiles("donnees", "*.csv", SearchOption.AllDirectories)
    Console.WriteLine(fichier)
Next
```

Comme pour les lignes, `EnumerateFiles` (paresseux) est préférable à `GetFiles` (qui construit tout un tableau) sur de grandes arborescences. Pour interroger plusieurs propriétés d'un même fichier ou dossier, les versions **objet** `FileInfo` / `DirectoryInfo` évitent des appels répétés au système.

---

## 🌊 2. Les flux (`Stream`)

Un **`Stream`** est une **séquence d'octets** que l'on lit ou écrit séquentiellement. C'est une **abstraction** : les octets peuvent venir d'un fichier, du réseau, de la mémoire… et — point décisif — les flux se **composent** (on en enveloppe un dans un autre, voir § 3).

Les deux flux concrets les plus courants :

```vb
' FileStream : des octets vers/depuis un fichier
Using flux As New FileStream("donnees.bin", FileMode.Create)
    Dim octets = Text.Encoding.UTF8.GetBytes("Bonjour")
    Await flux.WriteAsync(octets)
End Using

' MemoryStream : un flux EN MÉMOIRE (composer, tester, passer entre API)
Using memoire As New MemoryStream()
    Await JsonSerializer.SerializeAsync(memoire, monObjet)
    Dim resultat = memoire.ToArray()
End Using
```

Copier d'un flux à un autre se fait en une ligne avec `CopyToAsync` :

```vb
Using source = File.OpenRead("source.dat"),
      destination = File.Create("copie.dat")
    Await source.CopyToAsync(destination)
End Using
```

### Lecteurs et écrivains de texte : `StreamReader` / `StreamWriter`

Un flux ne connaît que des octets. Pour manipuler du **texte** par-dessus (avec gestion de l'**encodage**, UTF-8 par défaut), on l'enveloppe d'un `StreamReader` ou `StreamWriter` :

```vb
' Écrire du texte (ici en mode ajout)
Using writer As New StreamWriter("journal.txt", append:=True)
    Await writer.WriteLineAsync($"{DateTime.Now:u} — démarrage")
End Using

' Lire ligne à ligne
Using reader As New StreamReader("journal.txt")
    Dim ligne = Await reader.ReadLineAsync()
    While ligne IsNot Nothing
        Console.WriteLine(ligne)
        ligne = Await reader.ReadLineAsync()
    End While
End Using
```

### Lecteurs et écrivains binaires : `BinaryReader` / `BinaryWriter`

Pour écrire des **types primitifs** sous forme binaire compacte (formats de fichiers maison) :

```vb
' Écriture
Using flux = File.Create("enreg.bin"),
      writer As New BinaryWriter(flux)
    writer.Write(42)        ' Int32
    writer.Write(3.14)      ' Double
    writer.Write("Martin")  ' String (préfixée de sa longueur)
End Using

' Lecture : DANS LE MÊME ORDRE et les mêmes types
Using flux = File.OpenRead("enreg.bin"),
      reader As New BinaryReader(flux)
    Dim id = reader.ReadInt32()
    Dim valeur = reader.ReadDouble()
    Dim nom = reader.ReadString()
End Using
```

> Le binaire est **compact et rapide**, mais **fragile** : on doit relire exactement dans l'ordre et les types d'écriture, et le moindre changement de format casse la relecture. Pour des données amenées à évoluer ou à être échangées, préférez un format sérialisé (§ 7.5).

---

## 🧅 3. Composer les flux : bufferisé, compression, chiffrement

C'est ici que la nature **composable** des flux prend tout son sens : on **empile** les flux (motif décorateur), chacun transformant les octets qui le traversent. Cette idée — un flux qui en enveloppe un autre — est la clé des sections qui suivent.

### `BufferedStream`

Il ajoute un tampon pour **réduire le nombre d'opérations d'E/S** sous-jacentes :

```vb
Using flux = File.OpenRead("gros.dat"),
      bufferise As New BufferedStream(flux)
    ' lectures…
End Using
```

> ⚠️ **À nuancer** : `FileStream` est **déjà bufferisé en interne**. `BufferedStream` n'apporte donc pas grand-chose autour d'un fichier ; il est surtout utile autour de flux **non** bufferisés (réseau, par exemple).

### Compression : `GZipStream`, `BrotliStream`

On enveloppe le flux fichier d'un flux de compression. **L'ordre d'empilement** dit le sens de l'opération :

```vb
Imports System.IO.Compression

' COMPRESSER :  texte → GZipStream(compresse) → FileStream
Using fichier = File.Create("donnees.gz"),
      gzip As New GZipStream(fichier, CompressionLevel.Optimal),
      writer As New StreamWriter(gzip)
    Await writer.WriteAsync(grosTexte)
End Using   ' la fermeture en cascade écrit bien le pied de page gzip

' DÉCOMPRESSER :  FileStream → GZipStream(décompresse) → texte
Using fichier = File.OpenRead("donnees.gz"),
      gzip As New GZipStream(fichier, CompressionMode.Decompress),
      reader As New StreamReader(gzip)
    Dim texte = Await reader.ReadToEndAsync()
End Using
```

`BrotliStream` s'utilise **exactement de la même façon**, avec un **meilleur taux de compression** (c'est le format privilégié pour le contenu web). On trouve aussi `DeflateStream` et `ZLibStream`.

> **Compression « en flux » ≠ archive `.zip`.** Les flux ci-dessus compressent **un** train d'octets. Pour de vraies **archives** contenant plusieurs fichiers, utilisez `ZipFile` / `ZipArchive` :
> ```vb
> ZipFile.CreateFromDirectory("dossier", "archive.zip")
> ZipFile.ExtractToDirectory("archive.zip", "destination")
> ```

### Chiffrement : `CryptoStream`

Même principe décorateur : `CryptoStream` applique une transformation cryptographique aux octets qui le traversent. Avec AES :

```vb
Imports System.Security.Cryptography

' CHIFFRER :  texte → CryptoStream(chiffre) → FileStream
Using algo = Aes.Create()
    algo.Key = cle           ' 32 octets = AES-256 (à fournir par un coffre, pas en dur !)
    algo.GenerateIV()        ' IV aléatoire, différent à chaque chiffrement

    Using fichier = File.Create("secret.bin")
        ' L'IV n'est pas secret : on l'écrit en tête pour pouvoir déchiffrer ensuite
        Await fichier.WriteAsync(algo.IV)

        Using transform = algo.CreateEncryptor(),
              crypto As New CryptoStream(fichier, transform, CryptoStreamMode.Write),
              writer As New StreamWriter(crypto)
            Await writer.WriteAsync(messageSecret)
        End Using
    End Using
End Using
```

> ### ⚠️ Pourquoi `algo` et non `aes` ? Encore la casse-insensible !
> L'idiome C# `using var aes = Aes.Create();` ne se transpose **pas** tel quel : VB étant **insensible à la
> casse**, la variable `aes` entre en collision avec le type `Aes`, et `Aes.Create()` est alors lu comme un
> appel sur la variable en cours de déclaration — erreur `BC30980` (vérifié sur .NET 10). C'est le même piège
> que `host` / `Host` rencontré avec le Generic Host ([module 4.8](../04-async/08-background-services.md)) :
> on renomme la variable, tout simplement.

Le déchiffrement lit d'abord l'IV, puis enveloppe le flux d'un `CryptoStream` en mode `Read` :

```vb
Using algo = Aes.Create()
    algo.Key = cle
    Using fichier = File.OpenRead("secret.bin")
        Dim iv(algo.BlockSize \ 8 - 1) As Byte    ' 16 octets pour AES
        Await fichier.ReadExactlyAsync(iv, 0, iv.Length)
        algo.IV = iv

        Using transform = algo.CreateDecryptor(),
              crypto As New CryptoStream(fichier, transform, CryptoStreamMode.Read),
              reader As New StreamReader(crypto)
            Dim message = Await reader.ReadToEndAsync()
        End Using
    End Using
End Using
```

> ### ⚠️ Le chiffrement ne s'improvise pas
> `CryptoStream` est le **mécanisme** de chiffrement en flux, mais un chiffrement **sûr** ne se réduit pas à ce code :
> - la **clé** ne doit **jamais** être codée en dur — gérez-la via un coffre (**Key Vault**, module 16) ;
> - l'**IV** doit être **aléatoire et unique** à chaque chiffrement (c'est fait ici) et stocké avec le message (il n'est pas secret) ;
> - AES-CBC assure la **confidentialité** mais **pas l'intégrité** : pour détecter une altération, préférez un **chiffrement authentifié** (`AesGcm`) ou ajoutez un HMAC.
>
> Considérez ce qui précède comme une **illustration du mécanisme**. Pour la sécurité réelle, voir le **[module 16](../16-securite/README.md)**.

> ### 💡 L'empilement complet
> On peut chaîner plusieurs décorateurs. L'ordre **recommandé** en écriture : `StreamWriter` → `GZipStream` (compresse) → `CryptoStream` (chiffre) → `FileStream`. On **compresse avant de chiffrer**, car des données chiffrées sont quasi aléatoires et ne se compressent plus. À la lecture, on dépile dans l'ordre inverse.

---

## 🧭 Synthèse : quel outil pour quel besoin ?

| Besoin | Outil |
|---|---|
| Lire/écrire un fichier court, simplement | `File.ReadAllText` / `WriteAllText` (+ `…Async`) |
| Parcourir un gros fichier ligne à ligne | `File.ReadLines` (paresseux) |
| Manipuler des chemins | `Path.Combine`, `Path.GetFileName`… |
| Contrôle fin, gros volumes, octets bruts | `FileStream` / `MemoryStream` |
| Texte sur un flux | `StreamReader` / `StreamWriter` |
| Types primitifs en binaire compact | `BinaryReader` / `BinaryWriter` |
| Compresser un flux | `GZipStream` / `BrotliStream` |
| Archive multi-fichiers `.zip` | `ZipFile` / `ZipArchive` |
| Chiffrer un flux | `CryptoStream` (+ module 16 pour la sécurité réelle) |

---

## ✅ À retenir

- **Deux niveaux** : les aides `File` / `Directory` / `Path` pour le simple ; les **flux** pour le contrôle, les gros volumes et la **composition**.
- **Toujours `Path.Combine`** ; **préférez l'asynchrone** et l'**énumération paresseuse** (`ReadLines`, `EnumerateFiles`) sur les gros volumes.
- **Les flux se composent** (motif décorateur) : `StreamReader`/`Writer` pour le texte, `BinaryReader`/`Writer` pour les primitifs, par-dessus un `FileStream` ou un `MemoryStream`.
- **Compression** : `GZipStream` / `BrotliStream` en empilant les flux ; `ZipFile` pour de vraies archives.
- **Chiffrement** : `CryptoStream` est le mécanisme, mais la **sécurité réelle** (clé, intégrité, chiffrement authentifié) relève du **module 16**.
- **Pensez l'ordre** : compresser **puis** chiffrer.

---

> ### 🏁 Fin du chapitre 7 — Accès aux données
> Vous disposez maintenant de toute la chaîne : du SQL au plus près (ADO.NET, § 7.1) à l'ORM moderne (EF Core 10, § 7.2), des fournisseurs relationnels (§ 7.3) aux stockages NoSQL et au cache (§ 7.4), de la sérialisation (§ 7.5) jusqu'aux fichiers et flux (§ 7.6). Un terrain où **VB.NET est pleinement à sa place**.
>
> **Suite →** [Module 8 — Consommer et exposer des services](../08-services-web/README.md) : mettre ces données en mouvement à travers des API REST et des services web.

⏭️ [Consommer et exposer des services](/08-services-web/README.md)
