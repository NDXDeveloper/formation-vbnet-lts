/* ============================================================================
   Section 8.5 : gRPC et GraphQL — implémentation du service gRPC (C#)
   Description : L'implémentation du service gRPC. Hérite de la classe de base
                 GÉNÉRÉE (CatalogueService.CatalogueServiceBase) à partir du
                 .proto. Grâce au transcoding, la même méthode répond aussi à
                 GET /v1/produits/{id}. (Code C# : l'outillage gRPC est C#-only.)
   Fichier source : 05-grpc-graphql.md
   ============================================================================ */

using Catalogue.Grpc;
using Grpc.Core;

namespace ServeurServices;

public class CatalogueServiceImpl : CatalogueService.CatalogueServiceBase
{
    private static readonly Dictionary<int, (string Nom, double Prix)> Catalogue = new()
    {
        [1] = ("Clavier", 49.9),
        [42] = ("Écran 27 pouces", 220.0),
    };

    public override Task<ProduitReponse> ObtenirProduit(ProduitRequete request, ServerCallContext context)
    {
        if (!Catalogue.TryGetValue(request.Id, out var p))
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Produit {request.Id} introuvable."));
        }

        return Task.FromResult(new ProduitReponse
        {
            Id = request.Id,
            Nom = p.Nom,
            Prix = p.Prix
        });
    }
}
