using GS.PPoker.Options;
using GS.PPoker.Services;

using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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

app.MapRazorPages();
app.MapBlazorHub();

app.MapGet("/", () => Results.LocalRedirect("~/doorway"));

app.Run();
