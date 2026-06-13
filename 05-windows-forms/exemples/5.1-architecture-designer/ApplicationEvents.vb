' ============================================================================
'  Section 5.1 : Introduction, architecture et le Concepteur (Designer)
'  Description : Événements globaux du cadre applicatif VB.NET (Startup,
'                UnhandledException…), exactement comme présentés dans la
'                section. Ici, Startup ne fait que tracer un message.
'  Fichier source : 01-introduction-designer.md
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices

Namespace My

    ' Framework d'application VB.NET — événements globaux
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object,
                                          e As StartupEventArgs) _
                                          Handles Me.Startup
            ' Avant l'affichage du formulaire de démarrage
            System.Diagnostics.Debug.WriteLine("Démarrage de l'application (événement Startup).")
        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object,
                                                     e As UnhandledExceptionEventArgs) _
                                                     Handles Me.UnhandledException
            ' Gestion centralisée des exceptions non interceptées
            System.Diagnostics.Debug.WriteLine($"Exception non gérée : {e.Exception.Message}")
        End Sub

    End Class

End Namespace
