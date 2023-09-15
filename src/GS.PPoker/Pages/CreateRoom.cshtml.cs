﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GS.PPoker.Models;
using GS.PPoker.Services;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GS.PPoker.Pages;

public class CreateRoom : PageModel
{
    private readonly RoomService _roomService;

    public CreateRoom(RoomService roomService)
    {
        _roomService = roomService;
    }

    [BindProperty]
    [Required]
    [DisplayName("نام")]
    public string? Name { get; set; }

    [BindProperty]
    [Required]
    [DisplayName("رای‌ها")]
    public string Votes { get; set; } = "0,1,2,3,5,8,13,21,34,55,79";

    public async Task OnGetAsync(string? name)
    {
        await HttpContext.EnsureSignedInAsync();
        Name = name;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Name is null) return RedirectToPage();
        await HttpContext.EnsureSignedInAsync();
        UserId userId = HttpContext.User.FindFirst("id")?.Value?.Apply(Guid.Parse)
            ?? throw new InvalidOperationException("no 'id' claim found");
        var votes = Arr.create(Votes.Split(','));
        var roomId = _roomService.CreateRoom(userId, Name, votes);
        return LocalRedirect("~/room/" + roomId);
    }
}