' ============================================================================
'  Section 9.4 : WebView2 — objet hôte exposé à JavaScript
'  Description : L'objet .NET rendu appelable depuis la page web via
'                AddHostObjectToScript. Les objets hôtes TRANSITENT PAR COM :
'                la classe doit être <ComVisible(True)> — on retrouve, dans un
'                habit moderne, le mécanisme COM de la section 9.2.
'  Fichier source : 04-webview2.md
' ============================================================================

Imports System.Runtime.InteropServices

<ComVisible(True)>
Public Class Pont
    Public Function Additionner(a As Integer, b As Integer) As Integer
        Return a + b
    End Function
End Class
