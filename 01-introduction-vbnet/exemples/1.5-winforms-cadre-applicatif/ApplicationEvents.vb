' ============================================================================
'  Section 1.5 : Premier projet pas à pas (Console et Windows Forms)
'  Description : Les événements globaux du cadre applicatif, dans une classe
'                partielle MyApplication (espace de noms My) — exactement le
'                fichier ApplicationEvents.vb décrit dans la section.
'                Un gestionnaire Startup est montré en exemple ; les autres
'                événements disponibles sont listés ci-dessous.
'  Fichier source : 05-premier-projet.md
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    ' Les événements suivants sont disponibles pour MyApplication :
    '   Startup : déclenché au démarrage de l'application, avant la création
    '             du formulaire de démarrage.
    '   Shutdown : déclenché après la fermeture de tous les formulaires de
    '              l'application (pas en cas d'arrêt anormal).
    '   UnhandledException : déclenché si l'application rencontre une
    '                        exception non gérée.
    '   StartupNextInstance : déclenché au lancement d'une application à
    '                         instance unique déjà active.
    '   NetworkAvailabilityChanged : déclenché quand la connexion réseau est
    '                                connectée ou déconnectée.
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            ' Code exécuté avant l'affichage du formulaire de démarrage
            ' (initialisation de la journalisation, vérifications, etc.).
            Debug.WriteLine("Événement global Startup : l'application démarre.")
        End Sub

    End Class
End Namespace
