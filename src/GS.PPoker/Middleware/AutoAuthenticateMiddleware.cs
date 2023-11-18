using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GS.PPoker.Middleware;

public class AutoAuthenticateMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        await EnsureSignedInAsync(ctx);
        await next.Invoke(ctx);
    }

    private static async Task EnsureSignedInAsync(HttpContext ctx)
    {
        if (ctx.User.Identity?.IsAuthenticated == true)
            return;

        var timeProvider = ctx.RequestServices.GetRequiredService<TimeProvider>();
        var now = timeProvider.GetUtcNow();

        Claim[] claims = [new("id", Guid.NewGuid().ToString("N"))];
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
