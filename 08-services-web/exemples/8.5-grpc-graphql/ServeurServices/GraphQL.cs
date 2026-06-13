/* ============================================================================
   Section 8.5 : gRPC et GraphQL — serveur GraphQL HotChocolate (C#)
   Description : Le schéma GraphQL, défini par des TYPES et une classe Query C#
                 (AddQueryType<Query>()). C'est précisément ce que le cours dit
                 « non réaliste en VB » : le serveur GraphQL se délègue à C#.
                 Côté VB, on se contentera de le CONSOMMER par HttpClient + JSON.
                 HotChocolate met les champs en casse camel : Nom -> nom, etc.
   Fichier source : 05-grpc-graphql.md
   ============================================================================ */

namespace ServeurServices;

public record ProduitGql(int Id, string Nom, double Prix);

public class Query
{
    private static readonly Dictionary<int, ProduitGql> Catalogue = new()
    {
        [1] = new ProduitGql(1, "Clavier", 49.9),
        [42] = new ProduitGql(42, "Écran 27 pouces", 220.0),
    };

    public ProduitGql? GetProduit(string id) =>
        int.TryParse(id, out var n) && Catalogue.TryGetValue(n, out var p) ? p : null;
}
