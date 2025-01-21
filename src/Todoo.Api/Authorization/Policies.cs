using Microsoft.AspNetCore.Authorization;

namespace Todoo.Api.Authorization;

internal static class Policies
{
    public static Action<AuthorizationPolicyBuilder> Default => x =>
    {
        x.RequireAuthenticatedUser();
    };
}