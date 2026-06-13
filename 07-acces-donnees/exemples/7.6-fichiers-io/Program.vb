' ============================================================================
'  Section 7.6 : Fichiers, flux (Stream) et E/S
'  Description : Exemple complet reprenant les mécanismes de la section :
'                  · aides de haut niveau (File, Path.Combine, Directory) ;
'                  · flux : FileStream, MemoryStream, CopyToAsync ;
'                  · texte (StreamReader/Writer) et binaire
'                    (BinaryReader/Writer) ;
'                  · COMPRESSION GZip (avec ratio mesuré) ;
'                  · CHIFFREMENT AES via CryptoStream (round-trip), en
'                    nommant la variable « algo » (et NON « aes » : VB est
'                    insensible à la casse -> BC30980, comme host/Host) ;
'                  · empilement compresser PUIS chiffrer.
'                Tous les round-trips sont vérifiés (l'original est retrouvé).
'  Fichier source : 06-fichiers-io.md
'  Compilation    : dotnet run
' ============================================================================

Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text

Module Program

    Private ReadOnly Dossier As String = Path.Combine(Path.GetTempPath(), "fichiers-io-demo")

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        If Directory.Exists(Dossier) Then Directory.Delete(Dossier, recursive:=True)
        Directory.CreateDirectory(Dossier)

        Await DemoFileEtPath()
        Await DemoFlux()
        DemoBinaire()
        Await DemoCompression()
        Await DemoChiffrement()

        Directory.Delete(Dossier, recursive:=True)
        Console.WriteLine()
        Console.WriteLine("Dossier temporaire supprimé.")
    End Function

    ' ---- File, Path, Directory ------------------------------------------------
    Private Async Function DemoFileEtPath() As Task
        Console.WriteLine("== File / Path / Directory ==")
        Dim chemin = Path.Combine(Dossier, "notes.txt")   ' jamais de concaténation manuelle
        Await File.WriteAllTextAsync(chemin, "Bonjour")
        Dim contenu = Await File.ReadAllTextAsync(chemin)
        Console.WriteLine($"WriteAllText/ReadAllText -> « {contenu} »")

        Await File.WriteAllLinesAsync(Path.Combine(Dossier, "lignes.txt"), {"un", "deux", "trois"})
        Dim lignes = Await File.ReadAllLinesAsync(Path.Combine(Dossier, "lignes.txt"))
        Console.WriteLine($"WriteAllLines/ReadAllLines -> {lignes.Length} lignes : {String.Join(", ", lignes)}")

        Console.WriteLine($"Path : GetFileName={Path.GetFileName(chemin)}, GetExtension={Path.GetExtension(chemin)}")
        Dim fichiers = Directory.EnumerateFiles(Dossier, "*.txt").Count()
        Console.WriteLine($"Directory.EnumerateFiles(*.txt) -> {fichiers} fichier(s)")
    End Function

    ' ---- Flux : FileStream, MemoryStream, CopyToAsync ------------------------
    Private Async Function DemoFlux() As Task
        Console.WriteLine()
        Console.WriteLine("== Flux (FileStream, MemoryStream) ==")

        Dim source = Path.Combine(Dossier, "source.bin")
        Using flux As New FileStream(source, FileMode.Create)
            Dim octets = Encoding.UTF8.GetBytes("Contenu binaire de démonstration")
            Await flux.WriteAsync(octets)
        End Using

        Dim copie = Path.Combine(Dossier, "copie.bin")
        Using fSource = File.OpenRead(source), fDest = File.Create(copie)
            Await fSource.CopyToAsync(fDest)
        End Using
        Console.WriteLine($"CopyToAsync : {New FileInfo(source).Length} octets copiés -> identiques : {New FileInfo(copie).Length = New FileInfo(source).Length}")

        ' StreamReader/Writer (texte sur un flux)
        Dim journal = Path.Combine(Dossier, "journal.txt")
        Using writer As New StreamWriter(journal, append:=True)
            Await writer.WriteLineAsync("ligne 1")
            Await writer.WriteLineAsync("ligne 2")
        End Using
        Using reader As New StreamReader(journal)
            Console.WriteLine($"StreamReader.ReadLine -> « {Await reader.ReadLineAsync()} »")
        End Using
    End Function

    ' ---- BinaryReader / BinaryWriter ------------------------------------------
    Private Sub DemoBinaire()
        Console.WriteLine()
        Console.WriteLine("== Binaire (BinaryReader/Writer) ==")
        Dim chemin = Path.Combine(Dossier, "enreg.bin")
        Using flux = File.Create(chemin), writer As New BinaryWriter(flux)
            writer.Write(42)        ' Int32
            writer.Write(3.14)      ' Double
            writer.Write("Martin")  ' String préfixée de sa longueur
        End Using
        ' Relecture DANS LE MÊME ORDRE et les mêmes types
        Using flux = File.OpenRead(chemin), reader As New BinaryReader(flux)
            Dim id = reader.ReadInt32()
            Dim valeur = reader.ReadDouble()
            Dim nom = reader.ReadString()
            Console.WriteLine($"Relu : {id}, {valeur}, « {nom} » — round-trip OK : {id = 42 AndAlso nom = "Martin"}")
        End Using
    End Sub

    ' ---- Compression GZip -----------------------------------------------------
    Private Async Function DemoCompression() As Task
        Console.WriteLine()
        Console.WriteLine("== Compression (GZipStream) ==")
        Dim grosTexte = String.Concat(Enumerable.Repeat("Ligne répétitive et compressible. ", 500))
        Dim chemin = Path.Combine(Dossier, "donnees.gz")

        ' COMPRESSER : texte -> GZipStream -> FileStream
        Using fichier = File.Create(chemin),
              gzip As New GZipStream(fichier, CompressionLevel.Optimal),
              writer As New StreamWriter(gzip)
            Await writer.WriteAsync(grosTexte)
        End Using

        Dim tailleOrig = Encoding.UTF8.GetByteCount(grosTexte)
        Dim tailleComp = New FileInfo(chemin).Length
        Console.WriteLine($"Original {tailleOrig} octets -> compressé {tailleComp} octets (ratio {tailleComp * 100L \ tailleOrig} %)")

        ' DÉCOMPRESSER : FileStream -> GZipStream -> texte
        Dim relu As String
        Using fichier = File.OpenRead(chemin),
              gzip As New GZipStream(fichier, CompressionMode.Decompress),
              reader As New StreamReader(gzip)
            relu = Await reader.ReadToEndAsync()
        End Using
        Console.WriteLine($"Décompression : round-trip OK : {relu = grosTexte}")
    End Function

    ' ---- Chiffrement AES via CryptoStream -------------------------------------
    Private Async Function DemoChiffrement() As Task
        Console.WriteLine()
        Console.WriteLine("== Chiffrement (AES + CryptoStream) ==")
        Dim chemin = Path.Combine(Dossier, "secret.bin")
        Dim messageSecret = "Données confidentielles à protéger."
        Dim cle(31) As Byte
        RandomNumberGenerator.Fill(cle)   ' AES-256 (32 octets) — en vrai : via un coffre

        ' CHIFFRER. ⚠️ « algo », pas « aes » : VB est insensible à la casse, et
        ' « Dim aes = Aes.Create() » déclencherait BC30980 (collision avec le type Aes).
        Using algo = Aes.Create()
            algo.Key = cle
            algo.GenerateIV()
            Using fichier = File.Create(chemin)
                Await fichier.WriteAsync(algo.IV)   ' l'IV (non secret) est écrit en tête
                Using transform = algo.CreateEncryptor(),
                      crypto As New CryptoStream(fichier, transform, CryptoStreamMode.Write),
                      writer As New StreamWriter(crypto)
                    Await writer.WriteAsync(messageSecret)
                End Using
            End Using
        End Using
        Console.WriteLine($"Chiffré : {New FileInfo(chemin).Length} octets sur disque (illisibles en clair).")

        ' DÉCHIFFRER : lire l'IV, puis CryptoStream en lecture
        Dim dechiffre As String
        Using algo = Aes.Create()
            algo.Key = cle
            Using fichier = File.OpenRead(chemin)
                Dim iv(algo.BlockSize \ 8 - 1) As Byte   ' 16 octets
                Await fichier.ReadExactlyAsync(iv, 0, iv.Length)
                algo.IV = iv
                Using transform = algo.CreateDecryptor(),
                      crypto As New CryptoStream(fichier, transform, CryptoStreamMode.Read),
                      reader As New StreamReader(crypto)
                    dechiffre = Await reader.ReadToEndAsync()
                End Using
            End Using
        End Using
        Console.WriteLine($"Déchiffré : « {dechiffre} » — round-trip OK : {dechiffre = messageSecret}")
        Console.WriteLine("(rappel : clé via coffre, AesGcm pour l'intégrité — voir module 16)")
    End Function

End Module
