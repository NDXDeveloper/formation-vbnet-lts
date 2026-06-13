' ============================================================================
'  Section 3.8 : Réflexion et attributs
'  Description : Exemple complet reprenant tous les extraits de la section :
'                  · les TROIS façons d'obtenir un Type : l'opérateur
'                    GetType(Produit) (≈ typeof), obj.GetType() (instance)
'                    et Type.GetType("chaîne") ;
'                  · inspection : FullName, IsClass, GetProperties ;
'                  · PropertyInfo.GetValue / SetValue (sans connaître le nom
'                    à la compilation) ;
'                  · MethodInfo.Invoke et Activator.CreateInstance (2 formes) ;
'                  · découverte de plugins dans l'assembly (IsAssignableFrom) ;
'                  · attributs : <Obsolete> relu par réflexion, DataAnnotations
'                    exploitées par Validator (ajout), et l'attribut PERSONNALISÉ
'                    <Colonne> relu par Mapping.DecrireColonnes — la sortie
'                    attendue du cours.
'  Fichier source : 08-reflexion-attributs.md
'  Compilation    : dotnet build      Exécution : dotnet run
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Linq
Imports System.Reflection

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        DemoObtenirType()
        DemoInspecter()
        DemoLireEcrireDynamiquement()
        DemoInvoquerEtCreer()
        DemoPlugins()
        DemoAttributsIntegres()
        DemoAttributPersonnalise()
    End Sub

    ' ---- Trois façons d'obtenir un Type -----------------------------------------
    Private Sub DemoObtenirType()
        Console.WriteLine("== Obtenir un Type (3 façons) ==")

        Dim t1 As Type = GetType(Produit)                 ' opérateur (≈ typeof en C#)
        Dim unObjet As Object = New Produit()
        Dim t2 As Type = unObjet.GetType()                ' méthode d'instance
        Dim t3 As Type = Type.GetType("ReflexionAttributs.Produit")   ' chaîne (Nothing si introuvable)

        Console.WriteLine($"GetType(Produit)            -> {t1.FullName}")
        Console.WriteLine($"unObjet.GetType()           -> {t2.FullName}")
        Console.WriteLine($"Type.GetType(""…Produit"")    -> {If(t3?.FullName, "Nothing")}")
        Console.WriteLine($"Les trois désignent le même type : {t1 Is t2 AndAlso t2 Is t3}")
    End Sub

    ' ---- Inspecter un type -----------------------------------------------------------
    Private Sub DemoInspecter()
        Console.WriteLine()
        Console.WriteLine("== Inspecter un type ==")

        Dim t = GetType(Produit)
        Console.WriteLine($"Type : {t.FullName}")
        Console.WriteLine($"Classe : {t.IsClass}, abstraite : {t.IsAbstract}")

        For Each prop In t.GetProperties()
            Console.WriteLine($"  {prop.Name} : {prop.PropertyType.Name}")
        Next
    End Sub

    ' ---- Lire et écrire dynamiquement ---------------------------------------------------
    Private Sub DemoLireEcrireDynamiquement()
        Console.WriteLine()
        Console.WriteLine("== PropertyInfo : GetValue / SetValue ==")

        Dim p As New Produit With {.Nom = "Clavier"}
        Dim propNom = GetType(Produit).GetProperty("Nom")

        Dim valeur = propNom.GetValue(p)    ' lecture
        Console.WriteLine($"GetValue -> {valeur}")

        propNom.SetValue(p, "Souris")       ' écriture
        Console.WriteLine($"Après SetValue : p.Nom = {p.Nom}")
    End Sub

    ' ---- Invoquer et instancier dynamiquement ----------------------------------------------
    Private Sub DemoInvoquerEtCreer()
        Console.WriteLine()
        Console.WriteLine("== MethodInfo.Invoke et Activator.CreateInstance ==")

        Dim p As New Produit("Clavier", 100D)
        Dim methode = GetType(Produit).GetMethod("Recalculer")
        methode.Invoke(p, New Object() {0.2D})            ' Recalculer(0.2) dynamiquement
        Console.WriteLine($"Après Invoke(Recalculer, 0.2) : Prix = {p.Prix}")

        Dim instance = CType(Activator.CreateInstance(GetType(Produit)), Produit)
        Console.WriteLine($"Activator (sans paramètre)   : Nom = {If(instance.Nom, "Nothing")}, Prix = {instance.Prix}")

        Dim instance2 = CType(Activator.CreateInstance(GetType(Produit), "Écran", 199D), Produit)
        Console.WriteLine($"Activator (avec arguments)   : Nom = {instance2.Nom}, Prix = {instance2.Prix}")
    End Sub

    ' ---- Découverte de plugins dans l'assembly -----------------------------------------------
    Private Sub DemoPlugins()
        Console.WriteLine()
        Console.WriteLine("== Plugins découverts par réflexion d'assembly ==")

        Dim asm = Assembly.GetExecutingAssembly()
        Dim plugins = asm.GetTypes().
            Where(Function(ty) GetType(IPlugin).IsAssignableFrom(ty) AndAlso Not ty.IsAbstract).
            OrderBy(Function(ty) ty.Name)

        For Each typePlugin In plugins
            Dim plugin = CType(Activator.CreateInstance(typePlugin), IPlugin)
            plugin.Executer()
        Next
    End Sub

    ' ---- Attributs intégrés ------------------------------------------------------------------
    Private Sub DemoAttributsIntegres()
        Console.WriteLine()
        Console.WriteLine("== Attributs intégrés (chevrons < > en VB) ==")

        ' <Obsolete> relu par réflexion :
        Dim champ = GetType(ConfigurationLegacy).GetField("AncienChamp")
        Dim obsolete = champ.GetCustomAttribute(Of ObsoleteAttribute)()
        Console.WriteLine($"AncienChamp est marqué <Obsolete> : « {obsolete.Message} »")

        ' DataAnnotations exploitées par Validator (qui les relit par réflexion) :
        Dim invalide As New Inscription With {.Nom = "X", .Age = 150}
        Dim resultats As New List(Of ValidationResult)
        Dim estValide = Validator.TryValidateObject(
            invalide, New ValidationContext(invalide), resultats, validateAllProperties:=True)
        Console.WriteLine($"Inscription(Nom=""X"", Age=150) valide ? {estValide} — {resultats.Count} erreur(s) :")
        For Each r In resultats
            Console.WriteLine($"  - {r.ErrorMessage}")
        Next

        Dim valide As New Inscription With {.Nom = "Alice", .Age = 30}
        Console.WriteLine($"Inscription(Nom=""Alice"", Age=30) valide ? " &
            Validator.TryValidateObject(valide, New ValidationContext(valide), Nothing, True))
    End Sub

    ' ---- Attribut personnalisé relu par réflexion -----------------------------------------------
    Private Sub DemoAttributPersonnalise()
        Console.WriteLine()
        Console.WriteLine("== Attribut personnalisé <Colonne> (mini-ORM) ==")
        Mapping.DecrireColonnes(GetType(Produit))
    End Sub

End Module
