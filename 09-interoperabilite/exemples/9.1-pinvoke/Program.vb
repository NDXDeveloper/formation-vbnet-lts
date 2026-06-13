' ============================================================================
'  Section 9.1 : P/Invoke (appel d'API natives Windows)
'  Description : Programme de démonstration, à sorties vérifiables. Compare un
'                appel natif à son équivalent managé (PID), mesure la taille
'                marshalée des structures, lit la géométrie du bureau
'                (GetWindowRect), énumère les fenêtres via un CALLBACK, provoque
'                et lit une ERREUR native (handle invalide -> Win32Exception),
'                et déclare MessageBox (appelé seulement avec l'argument « ui »
'                pour rester non bloquant en test automatisé).
'  Fichier source : 01-pinvoke.md
' ============================================================================

Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Runtime.InteropServices

Module Program

    Private _compteFenetres As Integer

    Function Main(args As String()) As Integer
        Console.OutputEncoding = Text.Encoding.UTF8
        Console.WriteLine("=== 9.1 P/Invoke (appel d'API natives Windows) ===")
        Console.WriteLine()

        ' 1) Appel natif vs API managée : les deux PID doivent coïncider.
        Dim pidNatif = NativeMethods.GetCurrentProcessId()
        Dim pidManage = CUInt(Environment.ProcessId)
        Console.WriteLine($"GetCurrentProcessId (natif) = {pidNatif} ; Environment.ProcessId = {pidManage} ; identiques = {pidNatif = pidManage}")

        ' 2) Tailles marshalées des structures (déterministe).
        Console.WriteLine($"Marshal.SizeOf(RECT)       = {Marshal.SizeOf(Of RECT)()} octets   (attendu 16)")
        Console.WriteLine($"Marshal.SizeOf(DeviceInfo) = {Marshal.SizeOf(Of DeviceInfo)()} octets  (attendu 140 = 4 + 64×2 + 8)")

        ' 3) GetWindowRect sur le bureau (handle toujours valide).
        Dim hBureau = NativeMethods.GetDesktopWindow()
        Dim r As RECT
        If NativeMethods.GetWindowRect(hBureau, r) Then
            Console.WriteLine($"GetWindowRect(bureau)      -> {r.Right - r.Left} × {r.Bottom - r.Top} px (left={r.Left}, top={r.Top})")
        Else
            Throw New Win32Exception(Marshal.GetLastPInvokeError())
        End If

        ' 4) Declare : GetSystemMetrics (dimensions écran principal).
        Dim cx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN)
        Dim cy = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN)
        Dim moniteurs = NativeMethods.GetSystemMetrics(NativeMethods.SM_CMONITORS)
        Console.WriteLine($"GetSystemMetrics (Declare) -> écran {cx} × {cy} px ; {moniteurs} moniteur(s)")

        ' 5) Callback synchrone : EnumWindows compte les fenêtres de premier niveau.
        _compteFenetres = 0
        Dim rappel As New NativeMethods.EnumWindowsProc(AddressOf OnWindow)
        NativeMethods.EnumWindows(rappel, IntPtr.Zero)
        GC.KeepAlive(rappel)   ' callback synchrone : la variable locale suffit
        Console.WriteLine($"EnumWindows (callback)     -> {_compteFenetres} fenêtre(s) de premier niveau")

        ' 6) Erreur native : handle invalide -> False + code 1400 + Win32Exception.
        Dim rInvalide As RECT
        If Not NativeMethods.GetWindowRect(IntPtr.Zero, rInvalide) Then
            Dim code = Marshal.GetLastPInvokeError()
            Dim ex As New Win32Exception(code)
            Console.WriteLine($"GetWindowRect(handle nul)  -> échec attendu ; code Win32 = {code} ; « {ex.Message} »")
        Else
            Console.WriteLine("GetWindowRect(handle nul)  -> succès INATTENDU")
        End If

        ' 7) GetTickCount64 : uptime de la machine.
        Console.WriteLine($"GetTickCount64             -> {NativeMethods.GetTickCount64()} ms d'uptime (> 0)")

        ' 8) MessageBox : déclaré ; appelé seulement avec l'argument « ui ».
        If args.Contains("ui") Then
            NativeMethods.MessageBox(IntPtr.Zero, "Bonjour depuis VB.NET via P/Invoke !", "9.1 P/Invoke", 0UI)
            Console.WriteLine("MessageBox affichée puis fermée.")
        Else
            Console.WriteLine("MessageBox                 -> déclarée (relancer avec l'argument « ui » pour l'afficher)")
        End If

        Console.WriteLine()
        Console.WriteLine("Terminé.")
        Return 0
    End Function

    ' Le callback : renvoyer True poursuit l'énumération.
    Private Function OnWindow(hWnd As IntPtr, lParam As IntPtr) As Boolean
        _compteFenetres += 1
        Return True
    End Function

End Module
