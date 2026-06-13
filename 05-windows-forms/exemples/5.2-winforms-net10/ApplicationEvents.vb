' ============================================================================
'  Section 5.2 : Windows Forms sur .NET 10 (modernisation)
'  Description : La façon IDIOMATIQUE d'activer le mode sombre en VB.NET —
'                via l'événement ApplyApplicationDefaults du cadre applicatif
'                (pas de Program.cs en VB). e.ColorMode reçoit le mode voulu :
'                System suit le thème clair/sombre de Windows.
'  Fichier source : 02-winforms-net10.md
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices

Namespace My

    Partial Friend Class MyApplication

        Private Sub MyApplication_ApplyApplicationDefaults(
                sender As Object,
                e As ApplyApplicationDefaultsEventArgs
            ) Handles Me.ApplyApplicationDefaults

            ' Suivre le thème clair/sombre choisi dans Windows (.NET 10 : finalisé)
            e.ColorMode = SystemColorMode.System
        End Sub

    End Class

End Namespace
