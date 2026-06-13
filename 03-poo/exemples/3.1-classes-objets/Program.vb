' ============================================================================
'  Section 3.1 : Classes et objets
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · instanciation (New, constructeurs surchargés, initialiseur
'                    d'objet With {...}) ;
'                  · propriétés auto et calculées ; propriété complète avec
'                    validation (l'exception du Set est démontrée) ;
'                  · chaînage de constructeurs et constructeur partagé ;
'                  · membre Shared : compteur d'instances (Personne.NombreCrees) ;
'                  · indexeur (propriété Default) ;
'                  · sémantique de RÉFÉRENCE : b = a partage le même objet,
'                    et l'égalité par défaut compare les références.
'  Fichier source : 01-classes-objets.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoInstanciation()
        DemoProprieteComplete()
        DemoShared()
        DemoSemantiqueReference()
    End Sub

    ' ---- Instanciation et initialiseurs -------------------------------------
    Private Sub DemoInstanciation()
        Console.WriteLine("== Instanciation et propriétés ==")

        Dim alice As New Personne("Alice")                ' constructeur (nom)
        Dim bob As Personne = New Personne("Bob", 42)     ' constructeur (nom, age)
        Dim camille As New Personne With {                ' initialiseur d'objet
            .Prenom = "Camille",
            .Nom = "Durand",
            .Age = 30
        }

        Console.WriteLine($"bob : Nom = {bob.Nom}, Age = {bob.Age}")
        Console.WriteLine($"camille.NomComplet = {camille.NomComplet} (propriété calculée)")
        Console.WriteLine($"camille(0) = {camille(0)} (indexeur : propriété Default)")
        Console.WriteLine($"Ids distincts : {alice.Id <> bob.Id}")
    End Sub

    ' ---- Propriété complète : validation dans le Set ---------------------------
    Private Sub DemoProprieteComplete()
        Console.WriteLine()
        Console.WriteLine("== Propriété complète avec validation ==")

        Dim compte As New CompteBancaire()
        compte.Deposer(150D)
        Console.WriteLine($"Après Deposer(150) : Solde = {compte.Solde}")
        Console.WriteLine($"PeutRetirer(100) = {compte.PeutRetirer(100D)} ; PeutRetirer(500) = {compte.PeutRetirer(500D)}")

        Try
            compte.Solde = -1D            ' le Set refuse les valeurs négatives
        Catch ex As ArgumentOutOfRangeException
            Console.WriteLine($"compte.Solde = -1 -> {ex.GetType().Name} : « Le solde ne peut pas être négatif. »")
        End Try
    End Sub

    ' ---- Membres partagés ----------------------------------------------------------
    Private Sub DemoShared()
        Console.WriteLine()
        Console.WriteLine("== Membres partagés (Shared) ==")
        ' 3 Personnes déjà créées dans DemoInstanciation (alice, bob, camille).
        Dim a As New Personne("Ada")
        Dim b As New Personne("Linus")
        Console.WriteLine($"Personne.NombreCrees = {Personne.NombreCrees} (compteur de classe)")
        Console.WriteLine($"Configuration.Version = {Configuration.Version} (initialisée par Shared Sub New)")
    End Sub

    ' ---- Sémantique de référence ------------------------------------------------------
    Private Sub DemoSemantiqueReference()
        Console.WriteLine()
        Console.WriteLine("== Sémantique de référence ==")

        Dim a As New Personne With {.Nom = "Alice"}
        Dim b = a          ' b et a référencent le MÊME objet
        b.Nom = "Bob"
        Console.WriteLine($"a.Nom = {a.Nom} (a et b pointent vers le même objet)")

        ' L'égalité par défaut compare les références, pas le contenu :
        Dim x As New Personne With {.Nom = "Zoé"}
        Dim y As New Personne With {.Nom = "Zoé"}
        Console.WriteLine($"x.Equals(y) = {x.Equals(y)} (mêmes valeurs, objets distincts)")
        Console.WriteLine($"x Is y      = {x Is y}")
        ' Et « x = y » ne compile pas : l'opérateur '=' n'est pas défini entre
        ' deux objets d'une classe (erreur BC30452) tant qu'on ne le surcharge pas.
    End Sub

End Module
