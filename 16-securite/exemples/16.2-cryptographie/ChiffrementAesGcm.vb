' ============================================================================
'  Section 16.2 : Cryptographie — chiffrement authentifié (AES-GCM)
'  Description : Chiffre/déchiffre avec AES-GCM, un mode AUTHENTIFIÉ : il produit,
'                en plus du texte chiffré, une étiquette (tag) qui garantit
'                l'intégrité. Toute altération du message (ou du nonce/tag) fait
'                ÉCHOUER le déchiffrement par une CryptographicException — on ne
'                déchiffre jamais des données corrompues en silence. Le nonce est
'                RÉGÉNÉRÉ à chaque chiffrement (unicité indispensable en GCM) et
'                stocké en clair en tête du message (nonce | tag | chiffré).
'  Fichier source : 02-cryptographie.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Linq
Imports System.Security.Cryptography

''' <summary>Chiffrement symétrique authentifié AES-256-GCM.</summary>
Public Module ChiffrementAesGcm

    ' AES-GCM : nonce de 12 octets (96 bits) et tag de 16 octets (128 bits) — les tailles max.
    Private ReadOnly TailleNonce As Integer = AesGcm.NonceByteSizes.MaxSize
    Private ReadOnly TailleTag As Integer = AesGcm.TagByteSizes.MaxSize

    ''' <summary>Chiffre <paramref name="clair"/> et renvoie « nonce | tag | chiffré ».</summary>
    Public Function Chiffrer(clair As Byte(), cle As Byte()) As Byte()
        Dim nonce As Byte() = RandomNumberGenerator.GetBytes(TailleNonce)
        Dim chiffre(clair.Length - 1) As Byte
        Dim tag(TailleTag - 1) As Byte

        Using aes As New AesGcm(cle, TailleTag)
            aes.Encrypt(nonce, clair, chiffre, tag)
        End Using

        Return nonce.Concat(tag).Concat(chiffre).ToArray()
    End Function

    ''' <summary>Déchiffre « nonce | tag | chiffré » ; lève si le contenu a été altéré.</summary>
    Public Function Dechiffrer(donnees As Byte(), cle As Byte()) As Byte()
        Dim nonce As Byte() = donnees.Take(TailleNonce).ToArray()
        Dim tag As Byte() = donnees.Skip(TailleNonce).Take(TailleTag).ToArray()
        Dim chiffre As Byte() = donnees.Skip(TailleNonce + TailleTag).ToArray()
        Dim clair(chiffre.Length - 1) As Byte

        Using aes As New AesGcm(cle, TailleTag)
            ' Vérifie le tag : une altération provoque une CryptographicException.
            aes.Decrypt(nonce, chiffre, tag, clair)
        End Using

        Return clair
    End Function

End Module
