using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Todoo.Api.Authorization;

internal static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
    }
}