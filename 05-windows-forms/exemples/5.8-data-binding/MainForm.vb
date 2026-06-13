' ============================================================================
'  Section 5.8 : Liaison de données WinForms
'  Description : La pile recommandée de la section : BindingSource →
'                BindingList(Of Client) → éléments INotifyPropertyChanged.
'                  · liaison COMPLEXE : DataGridView.DataSource = bsClients ;
'                  · liaison SIMPLE : txtNom/txtEmail.DataBindings.Add(...) ;
'                  · BindingNavigator relié au BindingSource ;
'                  · maître-détail : bsCommandes.DataSource = bsClients,
'                    DataMember = NameOf(Client.Commandes).
'                Le journal d'auto-test vérifie que BindingList notifie les
'                ajouts (ListChanged) et que Client notifie les modifications
'                (PropertyChanged) — le fondement du rafraîchissement auto.
'  Fichier source : 08-data-binding.md
' ============================================================================

Imports System.ComponentModel
Imports System.Windows.Forms

Public Class MainForm

    Private Const JOURNAL As String = "5.8-data-binding-autotest.log"

    Private ReadOnly _clients As New BindingList(Of Client) From {
        New Client With {.Nom = "Dupont", .Email = "dupont@exemple.fr"},
        New Client With {.Nom = "Martin", .Email = "martin@exemple.fr"}
    }
    Private ReadOnly bsClients As New BindingSource()
    Private ReadOnly bsCommandes As New BindingSource()

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ChargerEtLier()
        ExecuterAutoTests()
        AutoFermeture.Activer(Me)
    End Sub

    Private Sub ChargerEtLier()
        ' Quelques commandes pour le maître-détail
        _clients(0).Commandes.Add(New Commande With {.Reference = "CMD-001", .Montant = 120D})
        _clients(0).Commandes.Add(New Commande With {.Reference = "CMD-002", .Montant = 80D})

        bsClients.DataSource = _clients

        ' Liaison complexe (grille) + liaisons simples (champs) sur la MÊME source
        dgvClients.DataSource = bsClients
        dgvClients.AutoGenerateColumns = True
        txtNom.DataBindings.Add("Text", bsClients, "Nom")
        txtEmail.DataBindings.Add("Text", bsClients, "Email")
        navClients.BindingSource = bsClients

        ' Maître-détail : les commandes du client courant
        bsCommandes.DataSource = bsClients
        bsCommandes.DataMember = NameOf(Client.Commandes)
        dgvCommandes.DataSource = bsCommandes
    End Sub

    ' ---- Vérifie le socle des notifications (journal) ----
    Private Sub ExecuterAutoTests()
        AutoFermeture.ReinitialiserJournal(JOURNAL)

        ' 1. BindingList notifie les ajouts (ListChanged)
        Dim liste As New BindingList(Of Client)
        Dim nbListChanged = 0
        AddHandler liste.ListChanged, Sub(s, e) nbListChanged += 1
        liste.Add(New Client With {.Nom = "Nouveau"})
        AutoFermeture.Journaliser(JOURNAL, $"BindingList.Add -> ListChanged levé : {nbListChanged = 1}")

        ' 2. Client notifie les modifications (PropertyChanged)
        Dim c As New Client With {.Nom = "Avant"}
        Dim nbPropChanged = 0
        Dim derniereProp As String = Nothing
        AddHandler c.PropertyChanged, Sub(s, e)
                                          nbPropChanged += 1
                                          derniereProp = e.PropertyName
                                      End Sub
        c.Nom = "Après"
        AutoFermeture.Journaliser(JOURNAL, $"Client.Nom modifié -> PropertyChanged levé : {nbPropChanged = 1} (propriété : {derniereProp})")

        ' 3. Affecter la même valeur ne notifie pas (garde If _nom <> value)
        nbPropChanged = 0
        c.Nom = "Après"
        AutoFermeture.Journaliser(JOURNAL, $"Même valeur réaffectée -> aucune notification : {nbPropChanged = 0}")

        ' 4. Navigation par le BindingSource (currency)
        bsClients.MoveNext()
        Dim courant = DirectCast(bsClients.Current, Client)
        AutoFermeture.Journaliser(JOURNAL, $"bsClients.MoveNext -> Position = {bsClients.Position}, Current.Nom = {courant.Nom}")
    End Sub

End Class
