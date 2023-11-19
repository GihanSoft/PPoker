using LanguageExt;

using Microsoft.AspNetCore.Components;

namespace GS.PPoker.Services;

public class BaseUrlProvider(NavigationManager navigationManager)
{
    private static string? _baseUrlCatch;
    public string BaseUrl => _baseUrlCatch ??= navigationManager.BaseUri.Apply(x => new Uri(x).AbsolutePath);
}
