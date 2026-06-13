' ============================================================================
'  Section 9.1 : P/Invoke (appel d'API natives Windows)
'  Description : Toutes les déclarations natives, regroupées par convention dans
'                un type « NativeMethods » (un Module : méthodes implicitement
'                Shared). Montre <DllImport> (la voie commune .NET) ET Declare
'                (l'idiome historique VB, concis, sans End Function), les
'                structures à disposition fixée (<StructLayout>/<MarshalAs>) et
'                la signature d'un callback (<UnmanagedFunctionPointer> + Delegate).
'  Fichier source : 01-pinvoke.md
' ============================================================================

Imports System
Imports System.Runtime.InteropServices

Friend Module NativeMethods

    ' --- <DllImport> : signature close par End Function (singularité VB) ---

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Function MessageBox(hWnd As IntPtr, lpText As String, lpCaption As String, uType As UInteger) As Integer
    End Function

    <DllImport("user32.dll")>
    Friend Function GetDesktopWindow() As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Function GetWindowRect(hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Function EnumWindows(callback As EnumWindowsProc, lParam As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll")>
    Friend Function GetCurrentProcessId() As UInteger
    End Function

    <DllImport("kernel32.dll")>
    Friend Function GetTickCount64() As ULong
    End Function

    ' --- Declare : idiome VB, une ligne logique, pas de End Function ---
    ' Auto => résolution Unicode/ANSI selon la plateforme (Unicode sur Windows).
    Public Declare Auto Function GetSystemMetrics Lib "user32.dll" (nIndex As Integer) As Integer

    ' --- Signature du callback attendu par EnumWindows ---
    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Friend Delegate Function EnumWindowsProc(hWnd As IntPtr, lParam As IntPtr) As Boolean

    ' Constantes pour GetSystemMetrics
    Friend Const SM_CXSCREEN As Integer = 0
    Friend Const SM_CYSCREEN As Integer = 1
    Friend Const SM_CMONITORS As Integer = 80

End Module

' --- Structures marshalées vers le natif ---

<StructLayout(LayoutKind.Sequential)>
Public Structure RECT
    Public Left As Integer
    Public Top As Integer
    Public Right As Integer
    Public Bottom As Integer
End Structure

' Démonstration des chaînes et tableaux de taille fixe « inline ».
<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
Public Structure DeviceInfo
    Public Id As Integer

    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
    Public Name As String        ' tampon de chaîne inline de 64 caractères

    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)>
    Public Flags As Byte()       ' tableau inline de 8 octets
End Structure
