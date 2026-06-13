' ============================================================================
'  Section 9.2 : COM et automation Office — exposer du .NET à COM (CCW)
'  Description : Le contrat COM est une INTERFACE EXPLICITE (exposée en double :
'                IDispatch + vtable via InterfaceIsDual), et la classe est
'                marquée <ClassInterface(None)> : le client COM ne voit que le
'                contrat stable de l'interface, pas une interface de classe
'                auto-générée (fragile). Chaque type porte un <Guid> FIXE pour
'                que l'identité COM ne change pas d'une compilation à l'autre.
'  Fichier source : 02-com-office.md
' ============================================================================

Imports System.Runtime.InteropServices

' Le contrat COM : une interface explicite, exposée en double (IDispatch + vtable).
<ComVisible(True)>
<Guid("9C2E6A14-3D7B-4F58-A1C0-5B8E2F7D9A31")>
<InterfaceType(ComInterfaceType.InterfaceIsDual)>
Public Interface ICalculatrice
    Function Additionner(a As Integer, b As Integer) As Integer
End Interface

' La classe : visible de COM, mais SANS interface de classe auto-générée.
<ComVisible(True)>
<Guid("E5C9F7A2-8B4D-4E16-9F3A-7C1D5E8B2F64")>
<ClassInterface(ClassInterfaceType.None)>
Public Class Calculatrice
    Implements ICalculatrice

    Public Function Additionner(a As Integer, b As Integer) As Integer _
        Implements ICalculatrice.Additionner
        Return a + b
    End Function
End Class
