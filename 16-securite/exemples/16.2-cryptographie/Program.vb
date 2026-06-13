' ============================================================================
'  Section 16.2 : Cryptographie — démonstration complète
'  Description : Exerce les primitives de System.Security.Cryptography :
'                (1) aléa cryptographique (RandomNumberGenerator) ;
'                (2) empreinte d'intégrité SHA-256 ;
'                (3) mot de passe PBKDF2 salé + vérification temps constant ;
'                (4) AES-GCM : round-trip puis DÉTECTION d'altération ;
'                (5) signature RSA (variable nommée « algo » — « rsa »
'                    entrerait en collision avec le type RSA, BC30980) ;
'                (6) DPAPI (ProtectedData) pour un secret LOCAL de poste.
'                Toutes les valeurs imprimées sont vérifiables.
'  Fichier source : 02-cryptographie.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Security.Cryptography
Imports System.Text

Module Program

    Sub Main()
        Console.OutputEncoding = Encoding.UTF8
        Console.WriteLine("=== 16.2 Cryptographie (System.Security.Cryptography) ===")
        Console.WriteLine()

        ' (1) Aléa cryptographique : clés, sels, nonces, jetons. JAMAIS System.Random.
        Dim cle As Byte() = RandomNumberGenerator.GetBytes(32)
        Dim de As Integer = RandomNumberGenerator.GetInt32(1, 7)   ' borne sup. exclue → [1,6]
        Console.WriteLine($"[1 RNG]    clé de {cle.Length} octets ; dé = {de} (1<=x<=6 : {de >= 1 AndAlso de <= 6})")

        ' (2) SHA-256 : empreinte d'INTÉGRITÉ (pas pour les mots de passe — trop rapide).
        Dim empreinte As Byte() = SHA256.HashData(Encoding.UTF8.GetBytes("contenu à vérifier"))
        Console.WriteLine($"[2 SHA256] {empreinte.Length} octets ; début = {Convert.ToHexString(empreinte).Substring(0, 16)}…")

        ' (3) PBKDF2 : hachage de mot de passe salé, lent, vérifié en temps constant.
        Dim mdp As String = "M0nMotDeP@sse!"
        Dim stocke As String = MotDePasse.Hacher(mdp)
        Dim bonMdp As Boolean = MotDePasse.Verifier(mdp, stocke)
        Dim mauvaisMdp As Boolean = MotDePasse.Verifier("intrus", stocke)
        Console.WriteLine($"[3 PBKDF2] bon mot de passe = {bonMdp} ; mauvais = {mauvaisMdp}")

        ' (4) AES-GCM : chiffrement authentifié — round-trip puis altération détectée.
        Dim cleAes As Byte() = RandomNumberGenerator.GetBytes(32)
        Dim clair As String = "Données confidentielles"
        Dim chiffre As Byte() = ChiffrementAesGcm.Chiffrer(Encoding.UTF8.GetBytes(clair), cleAes)
        Dim dechiffre As String = Encoding.UTF8.GetString(ChiffrementAesGcm.Dechiffrer(chiffre, cleAes))
        Console.WriteLine($"[4 AESGCM] round-trip OK = {dechiffre = clair}")

        chiffre(chiffre.Length - 1) = chiffre(chiffre.Length - 1) Xor CByte(1)   ' on falsifie 1 octet
        Dim altereDetecte As Boolean = False
        Try
            ChiffrementAesGcm.Dechiffrer(chiffre, cleAes)
        Catch ex As CryptographicException
            altereDetecte = True
        End Try
        Console.WriteLine($"[4 AESGCM] altération détectée = {altereDetecte}")

        ' (5) RSA : signature/vérification. La variable s'appelle « algo » : « rsa »
        '     entrerait en collision (insensible à la casse) avec le type RSA → BC30980.
        Dim donnees As Byte() = Encoding.UTF8.GetBytes("message à signer")
        Dim signatureValide As Boolean
        Using algo As RSA = RSA.Create(2048)
            Dim signature As Byte() = algo.SignData(donnees, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
            signatureValide = algo.VerifyData(donnees, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
        End Using
        Console.WriteLine($"[5 RSA]    signature valide = {signatureValide}")

        ' (6) DPAPI : protège un secret LOCAL au poste/à l'utilisateur (Windows).
        '     Idéal pour un cache de jeton hors ligne — JAMAIS My.Settings ni *.config en clair.
        Dim secret As Byte() = Encoding.UTF8.GetBytes("jeton-local-secret")
        Dim protege As Byte() = ProtectedData.Protect(secret, optionalEntropy:=Nothing, scope:=DataProtectionScope.CurrentUser)
        Dim relu As String = Encoding.UTF8.GetString(
            ProtectedData.Unprotect(protege, optionalEntropy:=Nothing, scope:=DataProtectionScope.CurrentUser))
        Dim dpapiOk As Boolean = (relu = "jeton-local-secret")
        Console.WriteLine($"[6 DPAPI]  round-trip OK = {dpapiOk} ; chiffré = {protege.Length} octets (clair = {secret.Length})")

        Console.WriteLine()
        Console.WriteLine("Terminé : on UTILISE les primitives de la BCL, on n'en réécrit aucune.")
    End Sub

End Module
