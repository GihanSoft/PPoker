using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;

namespace GS.PPoker.Pages;

internal static class HttpContextExtensions
{
    public static async Task EnsureSignedInAsync(this HttpContext ctx)
    {
        if (ctx.User.Identity?.IsAuthenticated == true) return;

        Claim[] claims = { new("id", Guid.NewGuid().ToString("N")) };
        ClaimsIdentity identity = new(claims, "auth");
        ClaimsPrincipal principal = new(identity);

        await ctx.SignInAsync("auth", principal);
        ctx.User = principal;
    }
}