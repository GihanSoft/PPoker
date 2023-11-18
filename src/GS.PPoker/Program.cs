using GS.PPoker.Components;
using GS.PPoker.Middleware;
using GS.PPoker.Options;
using GS.PPoker.Services;

using Microsoft.AspNetCore.DataProtection;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((serviceProvider, logConfig) => logConfig
    .ReadFrom.Configuration(builder.Configuration));

builder.Services.AddProblemDetails();

builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var dataProtectionKeysPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "data-protection-keys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));
builder.Services.AddAuthentication()
    .AddCookie("auth", opt =>
    {
        opt.ExpireTimeSpan = TimeSpan.FromHours(6);
        opt.SlidingExpiration = true;
    });

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.Configure<RoomOptions>(builder.Configuration.GetSection(RoomOptions.ConfigSectionKey));
builder.Services.AddSingleton<RoomService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AutoAuthenticateMiddleware>();

app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapGet("/", () => Results.LocalRedirect("~/doorway"));

app.Run();
