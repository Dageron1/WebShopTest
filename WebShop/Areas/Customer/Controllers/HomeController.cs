using WebShop.DataAccess.Repository.IRepository;
using WebShop.Models;
using WebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebShop.Models.ViewModels;
using WebShop.DataAcess.Data;
using System.Net.Mail;
using System.Net;


namespace WebShopWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Index(Product product)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.ApplicationUserId = userId;
            shoppingCart.ProductId = product.Id;


            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == product.Id);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();

            }
            else
            {
                //add cart record
                shoppingCart.Count = 1;
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }
        
        [Authorize]
        public IActionResult Help()
        {
            FeedBack message = new FeedBack();
            return View(message);
        }
        [HttpPost]
        public IActionResult Help(FeedBack text)
        {
            var feedback = text.Message.ToString();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accountUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("stryhaliouyauheni@gmail.com", "ejjqyssmdgbtqrxq");
            smtpClient.EnableSsl = true;

            MailMessage msg = new MailMessage();
            msg.To.Add("support@dagerondev.com");
            msg.Subject = $"Message from user - {accountUser.Email}";
            msg.From = new MailAddress("support@dagerondev.com");
            msg.Body = $"<html><body>{accountUser.Email}: {feedback}</body></html>";
            msg.IsBodyHtml = true;
            smtpClient.Send(msg);

            TempData["success"] = "Message sent successfully";
            return RedirectToAction("Index");
        }
        public IActionResult Photo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            return View(user);
        }
        
        public IActionResult AdvancedChat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var users = _unitOfWork.ApplicationUser.GetAll();
            ChatVM chatVm = new()
            {
                Rooms = _context.ChatRoom.ToList(),
                MaxRoomAllowed = 4,
                UserId = userId,
                //Users = users
            };
            return View(chatVm);
        }
        public IActionResult BasicChat()
        {
            return View();
        }
        public PartialViewResult ChatParial()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ChatVM chatVm = new()
            {
                Rooms = _context.ChatRoom.ToList(),
                MaxRoomAllowed = 4,
                UserId = userId,
            };
            return PartialView("ChatPartial", chatVm);
        }
        [Authorize]
        public IActionResult Chat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ChatVM chatVm = new()
            {
                Rooms = _context.ChatRoom.ToList(),
                MaxRoomAllowed = 4,
                UserId = userId,
            };
            return View(chatVm);
        }

        [HttpPost]
        public IActionResult SetTheme(string? data)
        {
            CookieOptions cookies = new CookieOptions();
            cookies.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("theme", data, cookies);
            return Ok();
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new() {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart) 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null) {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
                
            }
            else {
                //add cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";

           


            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}