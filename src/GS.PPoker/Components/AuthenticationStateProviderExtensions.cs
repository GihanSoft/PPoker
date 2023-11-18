using GS.PPoker.Models;

using LanguageExt;

using Microsoft.AspNetCore.Components.Authorization;

namespace GS.PPoker.Components;

internal static class AuthenticationStateProviderExtensions
{
    public static async Task<UserId> GetUserIdAsync(this AuthenticationStateProvider authenticationStateProvider)
    {
        UserId userId = await authenticationStateProvider.GetAuthenticationStateAsync()
            .Map(x => x.User.FindFirst("id")?.Value?.Apply(Guid.Parse))
            ?? throw new InvalidOperationException("no 'id' claim found");
        return userId;
    }
}
