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
            
            ValidAudience = x.Audience,
            ValidAlgorithms = [ "ES256", "ES384", "ES512", "PS256", "PS384", "PS512" ],
            ValidIssuer = x.Authority,
            ValidTypes = ["at+jwt"],
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

public partial class Program {}