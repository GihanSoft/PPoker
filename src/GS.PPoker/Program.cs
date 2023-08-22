var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseExceptionHandler("/error");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/", () => Results.LocalRedirect("/doorway"));
app.MapGet("/error", () => "something went wrong...");

app.Run();