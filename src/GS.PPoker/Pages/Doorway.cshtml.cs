using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GS.PPoker.Pages;

public class Doorway : PageModel
{
    public async Task OnGetAsync()
    {
        await HttpContext.EnsureSignedInAsync();
    }
}