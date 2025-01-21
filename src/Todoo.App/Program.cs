using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Todoo.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoApiService, TodoApiService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "toodoo.auth";
        options.SlidingExpiration = true;
        
        options.Events.OnSigningOut = async e =>
        {
            await e.HttpContext.RevokeRefreshTokenAsync();
        };
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        builder.Configuration.Bind("Authentication", options);

        options.SaveTokens = true;
        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        options.DisableTelemetry = true;
    });

builder.Services.AddOpenIdConnectAccessTokenManagement();

builder.Services.AddUserAccessTokenHttpClient("client",
    new UserTokenRequestParameters
    {
        ForceRenewal = true,
        Scope = "todoo"
    },
    client =>
    {
        client.BaseAddress = new Uri("https://localhost:7130");
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute().RequireAuthorization();

app.Run();