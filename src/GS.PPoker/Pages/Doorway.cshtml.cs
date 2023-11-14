using GS.PPoker.Services;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GS.PPoker.Pages;

public class Doorway : PageModel
{
    private readonly RoomService _roomService;

    public Doorway(RoomService roomService)
    {
        _roomService = roomService;
    }

    public string? Votes { get; set; }

    public async Task OnGetAsync()
    {
        await HttpContext.EnsureSignedInAsync();
        Votes = _roomService.DefaultVotes;
    }
}