using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GS.PPoker.Models;
using GS.PPoker.Services;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GS.PPoker.Pages;

public class JoinRoom : PageModel
{
    private readonly RoomService _roomService;

    public JoinRoom(RoomService roomService)
    {
        _roomService = roomService;
    }

    [BindProperty]
    [DisplayName("نام")]
    [Required]
    public string? Name { get; set; }

    [BindProperty]
    [DisplayName("کد اتاق")]
    [Required]
    public RoomId? RoomId { get; set; }

    public async Task OnGetAsync(string? name, RoomId? roomId)
    {
        await HttpContext.EnsureSignedInAsync();
        Name = name;
        RoomId = roomId;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Name is null || RoomId is null) return RedirectToPage();

        await HttpContext.EnsureSignedInAsync();
        UserId userId = HttpContext.User.FindFirst("id")?.Value?.Apply(Guid.Parse)
            ?? throw new InvalidOperationException("no 'id' claim found");

        _roomService.JoinRoom(RoomId.Value, userId, Name);
        return LocalRedirect("~/room/" + RoomId);
    }
}