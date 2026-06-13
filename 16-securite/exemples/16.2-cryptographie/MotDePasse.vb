' ============================================================================
'  Section 16.2 : Cryptographie — hachage de mot de passe (PBKDF2)
'  Description : Hache un mot de passe avec PBKDF2 (Rfc2898DeriveBytes.Pbkdf2),
'                salé et à coût élevé (600 000 itérations SHA-256, recommandation
'                OWASP courante). La vérification recalcule le hachage et compare
'                en TEMPS CONSTANT via CryptographicOperations.FixedTimeEquals,
'                ce qui ferme la porte aux attaques temporelles. Jamais de
'                hachage rapide (MD5/SHA brut) pour un mot de passe.
'  Fichier source : 02-cryptographie.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System
Imports System.Security.Cryptography

''' <summary>Hachage et vérification de mots de passe avec PBKDF2 salé.</summary>
Public Module MotDePasse

    Private Const Iterations As Integer = 600_000
    Private Const TailleSel As Integer = 16     ' 128 bits
    Private Const TailleHash As Integer = 32    ' 256 bits

    ''' <summary>Produit « sel.hash » (Base64) à stocker en base.</summary>
    Public Function Hacher(motDePasse As String) As String
        Dim sel As Byte() = RandomNumberGenerator.GetBytes(TailleSel)
        Dim hash As Byte() = Rfc2898DeriveBytes.Pbkdf2(
            motDePasse, sel, Iterations, HashAlgorithmName.SHA256, TailleHash)
        Return $"{Convert.ToBase64String(sel)}.{Convert.ToBase64String(hash)}"
    End Function

    ''' <summary>Vérifie un mot de passe contre la valeur stockée (temps constant).</summary>
    Public Function Verifier(motDePasse As String, valeurStockee As String) As Boolean
        Dim parties As String() = valeurStockee.Split("."c)
        If parties.Length <> 2 Then Return False

        Dim sel As Byte() = Convert.FromBase64String(parties(0))
        Dim hashAttendu As Byte() = Convert.FromBase64String(parties(1))

        Dim hashCalcule As Byte() = Rfc2898DeriveBytes.Pbkdf2(
            motDePasse, sel, Iterations, HashAlgorithmName.SHA256, hashAttendu.Length)

        ' Comparaison à temps constant : ne court-circuite pas au premier octet différent.
        Return CryptographicOperations.FixedTimeEquals(hashCalcule, hashAttendu)
    End Function

End Module
