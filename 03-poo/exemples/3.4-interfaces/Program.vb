' ============================================================================
'  Section 3.4 : Interfaces
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · implémentation avec clause Implements PAR MEMBRE ;
'                  · membre sous un autre nom (Rectangle.CalculerPerimetre
'                    appelé .Perimetre() via le contrat) ;
'                  · implémentation masquée : le membre Private n'est joignable
'                    que par DirectCast(r, IDisposable) — et Using l'appelle ;
'                  · un membre satisfaisant DEUX contrats (Dispose + Fermer) ;
'                  · plusieurs interfaces (Rapport), héritage d'interfaces
'                    (IModifiable inclut ILisible) ;
'                  · interfaces génériques : IComparable(Of T) (tri) et
'                    IDepot(Of T) maison ;
'                  · polymorphisme par interface et test TypeOf ... Is.
'  Fichier source : 04-interfaces.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoImplements()
        DemoNomDifferent()
        DemoImplementationMasquee()
        DemoInterfacesMultiplesEtHeritees()
        DemoInterfacesGeneriques()
        DemoPolymorphismeParInterface()
    End Sub

    ' ---- Implements par membre ---------------------------------------------------
    Private Sub DemoImplements()
        Console.WriteLine("== Implements (clause par membre) ==")
        Dim c As New Cercle With {.Rayon = 2}
        Console.WriteLine($"Cercle r=2 : Aire = {c.Aire:F2}, Périmètre = {c.Perimetre():F2}")
        c.Redimensionner(2)
        Console.WriteLine($"Après Redimensionner(2) : r = {c.Rayon}, Aire = {c.Aire:F2}")
    End Sub

    ' ---- Le membre peut porter un autre nom -----------------------------------------
    Private Sub DemoNomDifferent()
        Console.WriteLine()
        Console.WriteLine("== Membre sous un autre nom ==")
        Dim rect As New Rectangle With {.Largeur = 3, .Hauteur = 4}
        Console.WriteLine($"Sur le type     : rect.CalculerPerimetre() = {rect.CalculerPerimetre()}")
        Dim contrat As IFormeGeometrique = rect
        Console.WriteLine($"Via l'interface : contrat.Perimetre()      = {contrat.Perimetre()} (même code)")
    End Sub

    ' ---- Implémentation masquée (Private) ----------------------------------------------
    Private Sub DemoImplementationMasquee()
        Console.WriteLine()
        Console.WriteLine("== Implémentation masquée et multi-contrats ==")

        Dim r As New Ressource()
        ' r.Liberer()                          ' ❌ inaccessible : membre Private
        Console.WriteLine("Via DirectCast(r, IDisposable).Dispose() :")
        DirectCast(r, IDisposable).Dispose()   ' ✓ accessible via le contrat

        Console.WriteLine("Via DirectCast(r, IFichier).Fermer() (même implémentation) :")
        DirectCast(r, IFichier).Fermer()

        Console.WriteLine("Via Using (appel automatique de Dispose) :")
        Using res As New Ressource()
        End Using
    End Sub

    ' ---- Interfaces multiples et héritées --------------------------------------------------
    Private Sub DemoInterfacesMultiplesEtHeritees()
        Console.WriteLine()
        Console.WriteLine("== Interfaces multiples et héritage d'interfaces ==")

        Dim rapport As New Rapport()
        rapport.Imprimer()
        rapport.Enregistrer("C:\rapports\juin.pdf")

        ' IModifiable hérite d'ILisible : FichierTexte fournit Lire ET Ecrire.
        Dim fichier As New FichierTexte()
        fichier.Ecrire("Contenu de démonstration")
        Dim lecteur As ILisible = fichier        ' utilisable aussi par le contrat parent
        Console.WriteLine($"  ILisible.Lire() -> « {lecteur.Lire()} »")
    End Sub

    ' ---- Interfaces génériques -----------------------------------------------------------------
    Private Sub DemoInterfacesGeneriques()
        Console.WriteLine()
        Console.WriteLine("== Interfaces génériques ==")

        ' IComparable(Of T) rend le type triable par List.Sort
        Dim relevés As New List(Of Temperature) From {
            New Temperature(21.5), New Temperature(-3), New Temperature(35.2)
        }
        relevés.Sort()
        Console.WriteLine($"Tri IComparable : {String.Join(" ; ", relevés)}")

        ' IDepot(Of T) maison, implémenté pour Client
        Dim depot As IDepot(Of Client) = New DepotClients()
        depot.Ajouter(New Client With {.Id = 1, .Nom = "Alice"})
        depot.Ajouter(New Client With {.Id = 2, .Nom = "Bob"})
        Console.WriteLine($"IDepot(Of Client) : ObtenirParId(2).Nom = {depot.ObtenirParId(2).Nom} ; total = {depot.ObtenirTout().Count()}")
    End Sub

    ' ---- Polymorphisme par interface --------------------------------------------------------------
    Private Sub DemoPolymorphismeParInterface()
        Console.WriteLine()
        Console.WriteLine("== Polymorphisme fondé sur la capacité ==")

        Dim formes As New List(Of IFormeGeometrique) From {
            New Cercle With {.Rayon = 2},
            New Rectangle With {.Largeur = 3, .Hauteur = 4}
        }

        For Each f In formes
            Console.WriteLine($"  Aire : {f.Aire:F2}")   ' chaque forme répond selon son type réel
        Next

        ' TypeOf ... Is fonctionne aussi avec les interfaces :
        Dim obj As Object = New Ressource()
        If TypeOf obj Is IDisposable Then
            Console.Write("  TypeOf obj Is IDisposable -> ")
            DirectCast(obj, IDisposable).Dispose()
        End If
    End Sub

End Module
