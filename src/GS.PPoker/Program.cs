using GS.PPoker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthentication()
    .AddCookie("auth");

builder.Services.AddSingleton<RoomService>();

var app = builder.Build();

app.UseExceptionHandler("/error");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();

app.MapGet("/", () => Results.LocalRedirect("/doorway"));
app.MapGet("/error", () => "something went wrong...");

app.Run();
