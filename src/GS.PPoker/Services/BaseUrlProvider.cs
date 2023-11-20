using LanguageExt;

using Microsoft.AspNetCore.Components;

namespace GS.PPoker.Services;

public class BaseUrlProvider(NavigationManager navigationManager)
{
    public string BaseUrl { get; } = navigationManager.BaseUri.Apply(x => new Uri(x).AbsolutePath);
}
