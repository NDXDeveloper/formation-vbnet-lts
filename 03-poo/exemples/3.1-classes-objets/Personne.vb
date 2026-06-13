' ============================================================================
'  Section 3.1 : Classes et objets
'  Description : La classe Personne de la section, complète : champs privés
'                (convention _), propriétés auto-implémentées (dont ReadOnly
'                avec initialiseur), propriété calculée NomComplet,
'                constructeurs surchargés et chaînés (Me.New), membre Shared
'                (compteur d'instances) et indexeur (propriété Default).
'  Fichier source : 01-classes-objets.md
' ============================================================================

Public Class Personne

    ' ---- Propriétés auto-implémentées ----
    Public Property Prenom As String
    Public Property Nom As String
    Public Property Age As Integer
    Public ReadOnly Property Id As Guid = Guid.NewGuid()

    ' ⚠️ Le piège de la section, vérifié : la propriété auto « Id » génère un
    ' champ « _Id », et VB est insensible à la casse. Déclarer soi-même
    '     Private ReadOnly _id As Guid
    ' provoque l'erreur BC31061 : « variable '_id' est en conflit avec un
    ' membre déclaré implicitement pour property 'Id' ». Une donnée = soit la
    ' propriété auto (et son champ implicite), soit un champ explicite — pas les deux.

    ' ---- Propriété calculée (ReadOnly, forme complète) ----
    Public ReadOnly Property NomComplet As String
        Get
            Return $"{Prenom} {Nom}"
        End Get
    End Property

    ' ---- Membre partagé : appartient à la CLASSE, pas à une instance ----
    Public Shared Property NombreCrees As Integer

    ' ---- Constructeurs surchargés et chaînés ----
    Public Sub New()
        NombreCrees += 1
    End Sub

    Public Sub New(nom As String)
        Me.New()             ' chaînage : réutilise Sub New()
        Me.Nom = nom         ' Me lève l'ambiguïté paramètre / propriété
    End Sub

    Public Sub New(nom As String, age As Integer)
        Me.New(nom)          ' chaînage : réutilise Sub New(nom)
        Me.Age = age
    End Sub

    ' ---- Propriété par défaut (indexeur) : monObjet(i) ----
    Default Public ReadOnly Property Initiale(index As Integer) As Char
        Get
            Return NomComplet(index)
        End Get
    End Property

End Class
