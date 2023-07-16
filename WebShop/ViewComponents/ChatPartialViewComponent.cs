using WebShop.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebShop.Models.ViewModels;
using WebShop.DataAcess.Data;

namespace WebShop.ViewComponents
{
    public class ChatPartialViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public ChatPartialViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ChatVM chatVm = new()
            {
                Rooms = _context.ChatRoom.ToList(),
                MaxRoomAllowed = 4,
                UserId = userId?.Value,
            };
            return View(chatVm);
        }
    }
}
