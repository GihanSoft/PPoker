using System.Security.Claims;

using GS.PPoker.Services;

using Microsoft.AspNetCore.Authentication;

namespace GS.PPoker.Pages;

internal static class HttpContextExtensions
{
    public static async Task EnsureSignedInAsync(this HttpContext ctx)
    {
        if (ctx.User.Identity?.IsAuthenticated == true) return;

        var timeProvider = ctx.RequestServices.GetRequiredService<TimeProvider>();
        var now = timeProvider.UtcNow;

        Claim[] claims = { new("id", Guid.NewGuid().ToString("N")) };
        ClaimsIdentity identity = new(claims, "auth");
        ClaimsPrincipal principal = new(identity);

        AuthenticationProperties authProps = new()
        {
            AllowRefresh = true,
            IsPersistent = true,
            IssuedUtc = now,
            ExpiresUtc = now + TimeSpan.FromHours(6),
        };
        await ctx.SignInAsync("auth", principal, authProps);
        ctx.User = principal;
    }
}