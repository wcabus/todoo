using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Todoo.Api.Authorization;
using Todoo.Api.Endpoints;
using Todoo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoService, InMemoryTodoService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

builder.Services.AddAuthentication()
    .AddJwtBearer(x =>
    {
        x.MapInboundClaims = false;
        
        builder.Configuration.GetSection("Authentication").Bind(x);

        x.TokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            
            ValidAudience = $"{x.Authority}/resources",
            ValidAlgorithms = [ "RS256" ],
            ValidIssuer = x.Authority,
            ValidTypes = ["at+jwt"],
            
            // IssuerSigningKeyResolver = AllowSpecialCases
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddDefaultPolicy(nameof(Policies.Default), Policies.Default);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapApiEndpoints();

app.Run();

return;

IEnumerable<SecurityKey> AllowSpecialCases(string token, SecurityToken securityToken, string kid, TokenValidationParameters parameters)
{
    var header = JwtHeader.Base64UrlDeserialize(token.Split('.')[0]);
    
    if (header.TryGetValue("jwk", out object? jwk))
    {
        // This code would allow specifying a JsonWebKey as part of the token's header
        return [JsonWebKey.Create(jwk.ToString())];
    }

    if (header.TryGetValue("jku", out object? jku))
    {
        // This code would allow specifying an external JsonWebKey by specifying a URL containing the keys as part of the token's header
        var client = new HttpClient();
        var jwks = client.GetStringAsync(jku.ToString()).GetAwaiter().GetResult();
        return JsonWebKeySet.Create(jwks).Keys;
    }

    if (header.TryGetValue("x5c", out object? x5c))
    {
        // This code would allow specifying a certificate as part of the token's header
        var certPem = (x5c as List<object>)![0] as string;

        SecurityKey? securityKey;
        if (certPem.Contains("RSA PUBLIC", StringComparison.Ordinal))
        {
            var rsaSecurityKey = new RsaSecurityKey(RSA.Create());
            rsaSecurityKey.Rsa.ImportFromPem(certPem);

            securityKey = rsaSecurityKey;
        }
        else
        {
            var ecdsaSecurityKey = new ECDsaSecurityKey(ECDsa.Create());
            ecdsaSecurityKey.ECDsa.ImportFromPem(certPem);

            securityKey = ecdsaSecurityKey;
        }

        return [JsonWebKeyConverter.ConvertFromSecurityKey(securityKey)];
    }

    if (header.TryGetValue("x5u", out object? x5u))
    {
        // This code would allow specifying an external certificate by specifying a URL containing the keys as part of the token's header
        var client = new HttpClient();
        var certPem = client.GetStringAsync(x5u.ToString()).GetAwaiter().GetResult();

        SecurityKey? securityKey;
        if (certPem.Contains("RSA PUBLIC", StringComparison.Ordinal))
        {
            var rsaSecurityKey = new RsaSecurityKey(RSA.Create());
            rsaSecurityKey.Rsa.ImportFromPem(certPem);

            securityKey = rsaSecurityKey;
        }
        else
        {
            var ecdsaSecurityKey = new ECDsaSecurityKey(ECDsa.Create());
            ecdsaSecurityKey.ECDsa.ImportFromPem(certPem);

            securityKey = ecdsaSecurityKey;
        }

        return [JsonWebKeyConverter.ConvertFromSecurityKey(securityKey)];
    }

    return Array.Empty<SecurityKey>();
}

public partial class Program {}