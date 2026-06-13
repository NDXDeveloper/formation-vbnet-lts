' ============================================================================
'  Section 7.2 : Entity Framework Core 10
'  Description : Exemple complet reprenant les mécanismes de la section :
'                  · EnsureCreated (la génération de migrations est C#-only) ;
'                  · LINQ to Entities (syntaxe requête ET méthode), traduit en
'                    SQL — ToQueryString affiche le SQL généré ;
'                  · relations 1-N (Include eager) et N-N (skip navigations) ;
'                  · async, pagination (Skip/Take après un tri déterministe) ;
'                  · filtrage dynamique composé sur IQueryable ;
'                  · LeftJoin natif (EF Core 10) ;
'                  · filtres globaux nommés + IgnoreQueryFilters (EF Core 10) ;
'                  · type complexe Profil mappé en JSON ;
'                  · concurrence optimiste : DbUpdateConcurrencyException
'                    capturée (Await interdit dans Catch).
'  Fichier source : 02-ef-core-10.md
'  Compilation    : dotnet run   (restaure EF Core 10 + SQLite)
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.Data.Sqlite
Imports Microsoft.EntityFrameworkCore

Module Program

    Private ReadOnly CheminBase As String = Path.Combine(Path.GetTempPath(), "efcore10-demo.db")
    Private ReadOnly ChaineConnexion As String = $"Data Source={CheminBase}"

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        If File.Exists(CheminBase) Then File.Delete(CheminBase)

        Await InitialiserAsync()
        Await DemoLinq()
        Await DemoRelations()
        Await DemoPagination()
        Await DemoFiltrageDynamique()
        Await DemoLeftJoin()
        Await DemoFiltresNommes()
        Await DemoJson()
        Await DemoConcurrence()

        SqliteConnection.ClearAllPools()
        File.Delete(CheminBase)
        Console.WriteLine()
        Console.WriteLine("Base temporaire supprimée.")
    End Function

    ' ---- EnsureCreated + données de départ -----------------------------------
    Private Async Function InitialiserAsync() As Task
        Using contexte As New BoutiqueContext(ChaineConnexion)
            Await contexte.Database.EnsureCreatedAsync()   ' crée le schéma (pas de migration C#)

            Dim clavier As New Produit With {.Libelle = "Clavier"}
            Dim ecran As New Produit With {.Libelle = "Écran"}

            Dim dupont As New Client With {
                .Nom = "Dupont", .Email = "dupont@exemple.fr", .DateInscription = New Date(2025, 2, 10),
                .Profil = New Profil With {.Bio = "Client fidèle", .SiteWeb = "https://dupont.fr"}
            }
            dupont.Commandes.Add(New Commande With {.Reference = "CMD-001", .Montant = 120D, .DateCommande = New Date(2025, 3, 1), .Produits = {clavier, ecran}})
            dupont.Commandes.Add(New Commande With {.Reference = "CMD-002", .Montant = 45D, .DateCommande = New Date(2025, 4, 2), .Produits = {clavier}})

            Dim martin As New Client With {
                .Nom = "Martin", .Email = "martin@exemple.com", .DateInscription = New Date(2025, 5, 20),
                .EstActif = False,
                .Profil = New Profil With {.Bio = "Inactif", .SiteWeb = "https://martin.com"}
            }

            Dim durand As New Client With {
                .Nom = "Durand", .DateInscription = New Date(2024, 12, 1), .EstSupprime = True,
                .Profil = New Profil With {.Bio = "Supprimé", .SiteWeb = ""}
            }

            contexte.Clients.AddRange(dupont, martin, durand)
            Await contexte.SaveChangesAsync()
        End Using
        Console.WriteLine("== Base créée (EnsureCreated) : 3 clients, 2 commandes, 2 produits ==")
    End Function

    ' ---- LINQ to Entities ----------------------------------------------------
    Private Async Function DemoLinq() As Task
        Console.WriteLine()
        Console.WriteLine("== LINQ to Entities (traduit en SQL) ==")
        Using contexte As New BoutiqueContext(ChaineConnexion)
            ' Syntaxe requête
            Dim requete = From c In contexte.Clients
                          Where c.DateInscription >= New Date(2025, 1, 1)
                          Order By c.Nom
                          Select c.Nom
            Console.WriteLine($"Inscrits depuis 2025 : {String.Join(", ", Await requete.ToListAsync())}")

            ' Syntaxe méthode + voir le SQL généré
            Dim parMethode = contexte.Clients.Where(Function(c) c.Email IsNot Nothing).OrderBy(Function(c) c.Nom)
            Console.WriteLine($"Avec e-mail : {Await parMethode.CountAsync()} client(s)")
            Console.WriteLine("SQL généré : " & parMethode.Select(Function(c) c.Nom).ToQueryString().Split(Chr(10))(0) & " …")
        End Using
    End Function

    ' ---- Relations : Include (eager) + N-N ------------------------------------
    Private Async Function DemoRelations() As Task
        Console.WriteLine()
        Console.WriteLine("== Relations : Include (eager) et N-N ==")
        Using contexte As New BoutiqueContext(ChaineConnexion)
            Dim clients = Await contexte.Clients _
                .Include(Function(c) c.Commandes) _
                    .ThenInclude(Function(cmd) cmd.Produits) _
                .OrderBy(Function(c) c.Nom) _
                .ToListAsync()

            For Each c In clients
                Dim nbProduits = c.Commandes.SelectMany(Function(cmd) cmd.Produits).Distinct().Count()
                Console.WriteLine($"  {c.Nom} : {c.Commandes.Count} commande(s), {nbProduits} produit(s) distinct(s)")
            Next
        End Using
    End Function

    ' ---- Pagination -----------------------------------------------------------
    Private Async Function DemoPagination() As Task
        Console.WriteLine()
        Console.WriteLine("== Pagination (Skip/Take après tri) ==")
        Using contexte As New BoutiqueContext(ChaineConnexion)
            Dim page1 = Await contexte.Commandes.OrderBy(Function(c) c.Reference).Skip(0).Take(1).Select(Function(c) c.Reference).ToListAsync()
            Dim page2 = Await contexte.Commandes.OrderBy(Function(c) c.Reference).Skip(1).Take(1).Select(Function(c) c.Reference).ToListAsync()
            Console.WriteLine($"Page 1 (taille 1) : {String.Join(",", page1)} ; Page 2 : {String.Join(",", page2)}")
        End Using
    End Function

    ' ---- Filtrage dynamique sur IQueryable ------------------------------------
    Private Async Function DemoFiltrageDynamique() As Task
        Console.WriteLine()
        Console.WriteLine("== Filtrage dynamique (IQueryable composé) ==")
        Console.WriteLine($"  Sans critère          : {Await RechercherAsync(Nothing, Nothing)} commande(s)")
        Console.WriteLine($"  Montant >= 100        : {Await RechercherAsync(Nothing, 100D)} commande(s)")
        Console.WriteLine($"  Client « Dup » + >=40 : {Await RechercherAsync("Dup", 40D)} commande(s)")
    End Function

    Private Async Function RechercherAsync(nomClient As String, montantMin As Decimal?) As Task(Of Integer)
        Using contexte As New BoutiqueContext(ChaineConnexion)
            Dim requete As IQueryable(Of Commande) = contexte.Commandes
            If Not String.IsNullOrWhiteSpace(nomClient) Then
                requete = requete.Where(Function(c) c.Client.Nom.Contains(nomClient))
            End If
            If montantMin.HasValue Then
                requete = requete.Where(Function(c) c.Montant >= montantMin.Value)
            End If
            Return Await requete.CountAsync()   ' le SQL n'est construit qu'ici
        End Using
    End Function

    ' ---- LeftJoin natif (EF Core 10) ------------------------------------------
    Private Async Function DemoLeftJoin() As Task
        Console.WriteLine()
        Console.WriteLine("== LeftJoin natif (EF Core 10) ==")
        Using contexte As New BoutiqueContext(ChaineConnexion)
            Dim resultat = Await contexte.Commandes _
                .LeftJoin(
                    contexte.Clients,
                    Function(cmd) cmd.ClientId,
                    Function(cl) cl.Id,
                    Function(cmd, cl) New With {
                        cmd.Reference,
                        .NomClient = If(cl IsNot Nothing, cl.Nom, "(client inconnu)")
                    }) _
                .OrderBy(Function(x) x.Reference) _
                .ToListAsync()
            For Each ligne In resultat
                Console.WriteLine($"  {ligne.Reference} -> {ligne.NomClient}")
            Next
        End Using
    End Function

    ' ---- Filtres globaux nommés (EF Core 10) ----------------------------------
    Private Async Function DemoFiltresNommes() As Task
        Console.WriteLine()
        Console.WriteLine("== Filtres globaux nommés (EF Core 10) ==")
        Using contexte As New BoutiqueContext(ChaineConnexion)
            ' Les deux filtres actifs : exclut le supprimé (Durand) ET l'inactif (Martin)
            Dim parDefaut = Await contexte.Clients.CountAsync()
            ' On ignore UNIQUEMENT « ClientActif » : Martin réapparaît, Durand reste exclu
            Dim avecInactifs = Await contexte.Clients.IgnoreQueryFilters({"ClientActif"}).CountAsync()
            ' On ignore TOUS les filtres : les 3 clients
            Dim tous = Await contexte.Clients.IgnoreQueryFilters().CountAsync()
            Console.WriteLine($"  Filtres actifs : {parDefaut} ; sans « ClientActif » : {avecInactifs} ; sans aucun filtre : {tous}")
        End Using
    End Function

    ' ---- Type complexe mappé en JSON ------------------------------------------
    Private Async Function DemoJson() As Task
        Console.WriteLine()
        Console.WriteLine("== Type complexe Profil mappé en JSON ==")
        Using contexte As New BoutiqueContext(ChaineConnexion)
            ' Requête sur une propriété imbriquée du document JSON, traduite en SQL
            Dim clientsFr = Await contexte.Clients _
                .Where(Function(c) c.Profil.SiteWeb.EndsWith(".fr")) _
                .Select(Function(c) c.Nom) _
                .ToListAsync()
            Console.WriteLine($"  Clients dont le SiteWeb finit en .fr : {String.Join(", ", clientsFr)}")

            Dim dupont = Await contexte.Clients.FirstAsync(Function(c) c.Nom = "Dupont")
            Console.WriteLine($"  Profil de Dupont : Bio = « {dupont.Profil.Bio} », Site = {dupont.Profil.SiteWeb}")
        End Using
    End Function

    ' ---- Concurrence optimiste ------------------------------------------------
    Private Async Function DemoConcurrence() As Task
        Console.WriteLine()
        Console.WriteLine("== Concurrence optimiste (jeton de version) ==")

        ' Deux contextes lisent la MÊME commande.
        Using ctxA As New BoutiqueContext(ChaineConnexion),
              ctxB As New BoutiqueContext(ChaineConnexion)

            Dim cmdA = Await ctxA.Commandes.FirstAsync(Function(c) c.Reference = "CMD-001")
            Dim cmdB = Await ctxB.Commandes.FirstAsync(Function(c) c.Reference = "CMD-001")

            ' A modifie et enregistre en premier (incrémente le jeton)
            cmdA.Montant += 10D
            cmdA.Version += 1
            Await ctxA.SaveChangesAsync()

            ' B tente d'enregistrer avec un jeton désormais périmé -> conflit
            Dim conflit As DbUpdateConcurrencyException = Nothing
            Try
                cmdB.Montant += 5D
                cmdB.Version += 1
                Await ctxB.SaveChangesAsync()
            Catch ex As DbUpdateConcurrencyException
                conflit = ex   ' capture (Await interdit dans le Catch)
            End Try

            If conflit IsNot Nothing Then
                Console.WriteLine("  Conflit détecté : un autre contexte avait déjà modifié l'enregistrement.")
                Console.WriteLine("  Stratégie « le dernier l'emporte » : on recharge puis on réapplique.")
                Await conflit.Entries.Single().ReloadAsync()
            Else
                Console.WriteLine("  (aucun conflit — inattendu)")
            End If
        End Using
    End Function

End Module
