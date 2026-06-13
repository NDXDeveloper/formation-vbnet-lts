' ============================================================================
'  Section 8.2 : Exposer une Web API (par contrôleurs)
'  Description : Point d'entrée. VB n'a pas de top-level statements ni de
'                Async Main (BC30737) : on utilise un Module + Sub Main, car
'                app.Run() est bloquant. Assemble toute la chaîne : contrôleurs,
'                injection de dépendances, middleware (ChronoMiddleware),
'                OpenAPI 3.1 + Scalar, Problem Details, versioning, rate
'                limiting et authentification JWT Bearer.
'  Fichier source : 02-web-api-controllers.md
' ============================================================================

Imports System
Imports System.Security.Claims
Imports System.Threading
Imports System.Threading.RateLimiting
Imports System.Threading.Tasks
Imports Asp.Versioning
Imports Asp.Versioning.ApiExplorer
Imports Microsoft.AspNetCore.Authentication.JwtBearer
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.OpenApi
Imports Microsoft.AspNetCore.RateLimiting
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.IdentityModel.Tokens
Imports Microsoft.OpenApi
Imports Scalar.AspNetCore

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        ' --- Services ---
        builder.Services.AddControllers()
        builder.Services.AddProblemDetails()             ' RFC 9457
        builder.Services.AddScoped(Of IProduitService, ProduitService)()

        ' OpenAPI 3.1 (natif .NET 10) — document personnalisé par transformer
        ' (les commentaires XML -> OpenAPI étant C#-only en VB).
        builder.Services.AddOpenApi(
            Sub(options As OpenApiOptions)
                options.AddDocumentTransformer(
                    Function(document As OpenApiDocument,
                             contexte As OpenApiDocumentTransformerContext,
                             jeton As CancellationToken)
                        document.Info.Title = "API Catalogue"
                        document.Info.Version = "v1"
                        document.Info.Description = "Gestion du catalogue de produits (démo VB.NET)."
                        Return Task.CompletedTask
                    End Function)
            End Sub)

        ' Versioning (Asp.Versioning, l'ancien Mvc.Versioning étant déprécié)
        builder.Services.AddApiVersioning(
            Sub(options As ApiVersioningOptions)
                options.DefaultApiVersion = New ApiVersion(1, 0)
                options.AssumeDefaultVersionWhenUnspecified = True
                options.ReportApiVersions = True
            End Sub).AddMvc().AddApiExplorer(
            Sub(options As ApiExplorerOptions)
                options.GroupNameFormat = "'v'VVV"
                options.SubstituteApiVersionInUrl = True
            End Sub)

        ' Rate limiting intégré : policy "fixe" (cours) + "stricte" (démo 429)
        builder.Services.AddRateLimiter(
            Sub(options)
                options.AddFixedWindowLimiter("fixe",
                    Sub(l As FixedWindowRateLimiterOptions)
                        l.PermitLimit = 100
                        l.Window = TimeSpan.FromMinutes(1)
                        l.QueueLimit = 0
                    End Sub)
                options.AddFixedWindowLimiter("stricte",
                    Sub(l As FixedWindowRateLimiterOptions)
                        l.PermitLimit = 3
                        l.Window = TimeSpan.FromMinutes(1)
                        l.QueueLimit = 0
                    End Sub)
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests
            End Sub)

        ' Authentification JWT Bearer : l'API VALIDE le jeton (clé locale de démo).
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) _
            .AddJwtBearer(
            Sub(options As JwtBearerOptions)
                options.MapInboundClaims = False
                options.TokenValidationParameters = New TokenValidationParameters With {
                    .ValidateIssuer = True,
                    .ValidIssuer = Securite.Emetteur,
                    .ValidateAudience = True,
                    .ValidAudience = Securite.Audience,
                    .ValidateLifetime = True,
                    .ValidateIssuerSigningKey = True,
                    .IssuerSigningKey = Securite.CleSignature,
                    .NameClaimType = ClaimTypes.Name,
                    .RoleClaimType = ClaimTypes.Role
                }
            End Sub)
        builder.Services.AddAuthorization()

        Dim app = builder.Build()

        ' --- Pipeline (l'ordre est déterminant) ---
        app.UseExceptionHandler()                ' exceptions -> Problem Details
        ' En production on forcerait HTTPS ; ici l'exemple tourne en HTTP local
        ' (test hors ligne sans certificat), donc on ne redirige qu'hors dev.
        If Not app.Environment.IsDevelopment() Then app.UseHttpsRedirection()
        app.UseMiddleware(Of ChronoMiddleware)()
        app.UseRateLimiter()
        app.UseAuthentication()                  ' qui es-tu ?
        app.UseAuthorization()                   ' as-tu le droit ?
        app.MapControllers()

        If app.Environment.IsDevelopment() Then
            app.MapOpenApi()                     ' /openapi/v1.json (OpenAPI 3.1)
            app.MapScalarApiReference()          ' interface Scalar sur /scalar
        End If

        app.Run()
    End Sub
End Module
