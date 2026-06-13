' ============================================================================
'  Section 2.12 : L'espace My — un raccourci propre à VB.NET
'  Description : Démonstration des cinq objets principaux de la section,
'                dans LEUR scénario de prédilection (Windows Forms) :
'                  · My.Application : Info.Version, Info.DirectoryPath,
'                    CommandLineArgs, OpenForms (cadre applicatif) ;
'                  · My.Computer : Name, Network.IsAvailable et FileSystem
'                    (écriture puis relecture d'un fichier) — Clipboard est
'                    montré en commentaire pour ne pas écraser le contenu
'                    du presse-papiers ;
'                  · My.User : IsAuthenticated, Name ;
'                  · My.Settings : ThemeUtilisateur et un compteur de
'                    lancements PERSISTÉ (Save -> user.config) ;
'                  · My.Resources : chaîne fortement typée du .resx.
'  Fichier source : 12-espace-my.md
' ============================================================================

Imports System.IO

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' ---- My.Settings : accès fortement typé + persistance ----
        My.Settings.NombreDeLancements += 1
        My.Settings.Save()                          ' persiste dans user.config

        Dim sb As New System.Text.StringBuilder()

        sb.AppendLine("== My.Resources ==")
        sb.AppendLine(My.Resources.MessageBienvenue)
        sb.AppendLine()

        sb.AppendLine("== My.Application ==")
        sb.AppendLine($"Version   : {My.Application.Info.Version}")
        sb.AppendLine($"Dossier   : {My.Application.Info.DirectoryPath}")
        sb.AppendLine($"Arguments : {My.Application.CommandLineArgs.Count} argument(s) de ligne de commande")
        sb.AppendLine($"OpenForms : {My.Application.OpenForms.Count} formulaire(s) ouvert(s)")
        sb.AppendLine()

        sb.AppendLine("== My.Computer ==")
        sb.AppendLine($"Machine   : {My.Computer.Name}")
        sb.AppendLine($"Réseau    : {If(My.Computer.Network.IsAvailable, "disponible", "indisponible")}")

        ' My.Computer.FileSystem : écrire puis relire un fichier
        Dim fichier = Path.Combine(Path.GetTempPath(), "espace-my-demo.txt")
        My.Computer.FileSystem.WriteAllText(fichier, "Écrit puis relu via My.Computer.FileSystem.", append:=False)
        sb.AppendLine($"Fichier   : {My.Computer.FileSystem.ReadAllText(fichier)}")
        ' My.Computer.Clipboard.SetText("Copié !")   ' fonctionne aussi — laissé
        '                                            ' commenté pour ne pas écraser
        '                                            ' votre presse-papiers.
        sb.AppendLine()

        sb.AppendLine("== My.User ==")
        sb.AppendLine($"Authentifié : {My.User.IsAuthenticated}")
        sb.AppendLine($"Nom         : {My.User.Name}")
        sb.AppendLine()

        sb.AppendLine("== My.Settings ==")
        sb.AppendLine($"ThemeUtilisateur   : {My.Settings.ThemeUtilisateur}")
        sb.AppendLine($"NombreDeLancements : {My.Settings.NombreDeLancements} (relancez l'application : il s'incrémente)")

        txtInfos.Text = sb.ToString()
    End Sub

    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        Close()
    End Sub

End Class
