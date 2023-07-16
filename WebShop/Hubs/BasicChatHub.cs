using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebShop.DataAccess;
using WebShop.DataAccess.Repository.IRepository;
using WebShop.DataAcess.Data;

namespace WebShop.Hubs
{
    public class BasicChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        public BasicChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SendMessageToAll(string user, string message)
        {
            await Clients.All.SendAsync("MessageReceived", user, message);
        }
        [Authorize]
        public async Task SendMessageToReceiver(string sender, string receiver, string message)
        {
            var receiverId = _db.Users.FirstOrDefault(u => u.Email.ToLower() == receiver.ToLower()).Id;
            var senderId = _db.Users.FirstOrDefault(u => u.Email.ToLower() == sender.ToLower()).Id;

            if (!string.IsNullOrEmpty(receiverId))
            {
                await Clients.Users(receiverId, senderId).SendAsync("MessageReceived", sender, message, receiver);
            }

        }

    } 
}
