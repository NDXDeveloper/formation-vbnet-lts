' ============================================================================
'  Section 16.1 : Authentification et autorisation — configuration de l'API
'  Description : Configure JWT Bearer (validation INTÉGRALE : signature, émetteur,
'                audience, durée de vie) puis deux POLITIQUES d'autorisation :
'                - « LectureCommandes » : assertion sur le claim « scp » (les scopes
'                  OAuth 2.0 sont une chaîne séparée par des ESPACES) ;
'                - « AdminUniquement » : exigence de rôle.
'                MapInboundClaims=False conserve les noms de claims bruts (scp, roles,
'                sub) au lieu de les remapper. Program est une CLASSE PUBLIQUE pour
'                que WebApplicationFactory(Of Program) puisse l'instancier en test.
'  Fichier source : 01-auth.md
' ============================================================================

Option Strict On
Option Explicit On

Imports System.Linq
Imports System.Text
Imports Microsoft.AspNetCore.Authentication.JwtBearer
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.IdentityModel.Tokens

Public Class Program

    Public Shared Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        ' --- Authentification : JWT Bearer, validation intégrale du jeton ---
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            Sub(options)
                options.MapInboundClaims = False   ' conserve scp / roles / sub tels quels
                options.TokenValidationParameters = New TokenValidationParameters With {
                    .ValidateIssuer = True,
                    .ValidIssuer = ClesDev.Emetteur,
                    .ValidateAudience = True,
                    .ValidAudience = ClesDev.Audience,
                    .ValidateLifetime = True,
                    .ValidateIssuerSigningKey = True,
                    .IssuerSigningKey = New SymmetricSecurityKey(Encoding.UTF8.GetBytes(ClesDev.CleSignature)),
                    .NameClaimType = "sub",
                    .RoleClaimType = "roles"
                }
            End Sub)

        ' --- Autorisation : politiques nommées (moindre privilège) ---
        Dim autorisation = builder.Services.AddAuthorizationBuilder()

        ' Politique de SCOPE : le claim « scp » est une liste séparée par des espaces.
        autorisation.AddPolicy("LectureCommandes",
            Sub(p)
                p.RequireAssertion(
                    Function(ctx)
                        Dim scp = ctx.User.FindFirst("scp")?.Value
                        Return scp IsNot Nothing AndAlso scp.Split(" "c).Contains("Commandes.Read")
                    End Function)
            End Sub)

        ' Politique de RÔLE.
        autorisation.AddPolicy("AdminUniquement", Sub(p) p.RequireRole("Administrateur"))

        builder.Services.AddControllers()

        Dim app = builder.Build()
        app.UseAuthentication()
        app.UseAuthorization()
        app.MapControllers()
        app.Run()
    End Sub

End Class
