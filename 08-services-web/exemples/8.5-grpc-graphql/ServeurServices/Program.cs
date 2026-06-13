/* ============================================================================
   Section 8.5 : gRPC et GraphQL — hôte du serveur (C#)
   Description : Démarre le serveur. Deux points d'écoute Kestlel en clair :
                 5185 en HTTP/2 (gRPC natif), 5186 en HTTP/1.1 (REST transcodé
                 + GraphQL). Enregistre gRPC + JSON transcoding et le serveur
                 GraphQL HotChocolate, puis mappe le service gRPC et /graphql.
   Fichier source : 05-grpc-graphql.md
   ============================================================================ */

using Microsoft.AspNetCore.Server.Kestrel.Core;
using ServeurServices;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC exige HTTP/2 ; on l'isole sur un port en clair dédié.
    options.ListenLocalhost(5185, o => o.Protocols = HttpProtocols.Http2);
    // REST (transcoding) + GraphQL : HTTP/1.1 classique.
    options.ListenLocalhost(5186, o => o.Protocols = HttpProtocols.Http1);
});

builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.Services.AddGrpc().AddJsonTranscoding();          // gRPC + REST/JSON
builder.Services.AddGraphQLServer().AddQueryType<Query>(); // GraphQL (HotChocolate)

var app = builder.Build();

app.MapGrpcService<CatalogueServiceImpl>();
app.MapGraphQL("/graphql");

app.Run();
