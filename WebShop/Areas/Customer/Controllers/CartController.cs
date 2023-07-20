using WebShop.DataAccess.Repository.IRepository;
using WebShop.Models;
using WebShop.Models.ViewModels;
using WebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using WebShop.Hubs;
using System.Net.Mail;
using System.Net;
using Stripe;
using Microsoft.AspNetCore.Hosting;

namespace WebShopWeb.Areas.Customer.Controllers
{

    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<OrderHub> _orderHub;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, IHubContext<OrderHub> orderHub)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _orderHub = orderHub;
        }


        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        public IActionResult Index(ShoppingCartVM? promo = null)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {

                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new(),

            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();


            if (promo?.CartPromo.Title != null)
            {

                var findPromo = _unitOfWork.Promo.Get(p => p.Title.ToLower() == promo.CartPromo.Title.ToLower());
                if (findPromo == null)
                {
                    TempData["error"] = "Wrong promo code";

                    foreach (var cart in ShoppingCartVM.ShoppingCartList)
                    {
                        cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                        cart.Price = GetPriceBasedOnQuantity(cart);
                        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                    }
                }
                if (findPromo != null)
                {
                    TempData["success"] = "Promo code accepted";
                    TempData["Promo"] = findPromo.Title.ToString().ToLower();
                    foreach (var cart in ShoppingCartVM.ShoppingCartList)
                    {
                        cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                        cart.Price = GetPriceBasedOnQuantity(cart);
                        ShoppingCartVM.OrderHeader.OrderBeforeDiscount += (cart.Price * cart.Count);
                        ShoppingCartVM.OrderHeader.OrderTotal += ((cart.Price * cart.Count) - ((cart.Price * cart.Count) / 100 * findPromo.Discount));

                    }
                }

            }
            else
            {
                foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }
            }


            return View(ShoppingCartVM);
        }
        public IActionResult Promo()
        {
            List<PromoCode> objPromoList = _unitOfWork.Promo.GetAll().ToList();
            return View(objPromoList);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<PromoCode> objPromoList = _unitOfWork.Promo.GetAll().ToList();
            return Json(new { data = objPromoList });
        }
        public IActionResult AddPromo(int? id)
        {
            PromoVM promoVM = new()
            {
                Promo = new PromoCode()
            };
            if (id == null || id == 0)
            {
                //create
                return View(promoVM);
            }
            else
            {
                promoVM.Promo = _unitOfWork.Promo.Get(p => p.Id == id);
                return View(promoVM);
            }
        }
        [HttpPost]
        public IActionResult AddPromo(PromoVM promoCode)
        {

            if (promoCode.Promo.Id == 0)
            {
                _unitOfWork.Promo.Add(promoCode.Promo);
            }
            else
            {
                _unitOfWork.Promo.Update(promoCode.Promo);
            }
            _unitOfWork.Save();


            TempData["success"] = "Promo created/updated successfully";
            return RedirectToAction("Promo");
        }
        [HttpDelete]
        public IActionResult Deletepromo(int? id)
        {
            var promoToBeDeleted = _unitOfWork.Promo.Get(u => u.Id == id);
            if (promoToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Promo.Remove(promoToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult Summary()
        {

            var promoCode = TempData["Promo"];
            var findPromo = _unitOfWork.Promo.Get(p => p.Title.ToLower() == promoCode);
            if (findPromo != null)
            {
                TempData["AppliedPromo"] = findPromo.Title.ToString().ToLower();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            if (findPromo != null)
            {
                foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += ((cart.Price * cart.Count) - ((cart.Price * cart.Count) / 100 * findPromo.Discount));
                }
            }
            else
            {
                foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
                }
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var promoCode = TempData["AppliedPromo"];
            var findPromo = _unitOfWork.Promo.Get(p => p.Title.ToLower() == promoCode);

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now.ToString("dd MMMM yyyy HH:mm");
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;


            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            if (findPromo != null)
            {
                foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += ((cart.Price * cart.Count) - ((cart.Price * cart.Count) / 100 * findPromo.Discount));
                }
            }
            else
            {
                foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
                }
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer 
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //it is a company user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            if (findPromo != null)
            {
                if (applicationUser.CompanyId.GetValueOrDefault() == 0)
                {
                    //it is a regular customer account and we need to capture payment
                    //stripe logic
                    //получаем https через scheme 
                    //получаем домен динамично
                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                    var options = new SessionCreateOptions
                    {
                        SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                        CancelUrl = domain + "customer/cart/index",
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                    foreach (var item in ShoppingCartVM.ShoppingCartList)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100) - (long)((item.Price * 100) / 100 * findPromo.Discount), // $20.50 => 2050
                                Currency = "pln",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Title
                                }
                            },
                            Quantity = item.Count
                        };
                        options.LineItems.Add(sessionLineItem);
                    }


                    var service = new SessionService();
                    Session session = service.Create(options);
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);

                }
            }
            else
            {
                if (applicationUser.CompanyId.GetValueOrDefault() == 0)
                {
                    //it is a regular customer account and we need to capture payment
                    //stripe logic
                    //получаем https через scheme 
                    //получаем домен динамично
                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                    var options = new SessionCreateOptions
                    {
                        SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                        CancelUrl = domain + "customer/cart/index",
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                    foreach (var item in ShoppingCartVM.ShoppingCartList)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                                Currency = "pln",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Title
                                }
                            },
                            Quantity = item.Count
                        };
                        options.LineItems.Add(sessionLineItem);
                    }


                    var service = new SessionService();
                    Session session = service.Create(options);
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);

                }
            }


            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }


        public async Task<IActionResult> OrderConfirmation(int id)
        {


            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            var userEmail = orderHeader.ApplicationUser.Email;
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();

            }

            await _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - WebSHop",
                $"<html><body> Thank you for shopping in our online store, your order №{orderHeader.Id} is already being processed. " +
                $"<p></hr><b>Total price: {orderHeader.OrderTotal} zl</b></p>" +
                $"<p>In the near future you will receive an email with the tracking number of the parcel.</p></body></html>");




            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            var user = _unitOfWork.ApplicationUser.Get(u => u.Email == "admin@gmail.com");
            await _orderHub.Clients.User(user.Id).SendAsync("newOrder");

            //SmtpClient smtpClient = new SmtpClient();
            //smtpClient.Host = "smtp.gmail.com";
            //smtpClient.Port = 587;
            //smtpClient.Credentials = new NetworkCredential("stryhaliouyauheni@gmail.com", "ejjqyssmdgbtqrxq");
            //smtpClient.EnableSsl = true;

            //MailMessage message = new MailMessage();
            //message.To.Add(userEmail);
            //message.Subject = $"Order number - {orderHeader.Id}";
            //message.From = new MailAddress("support@dagerondev.com");
            //message.Body = $"<html><body> Thank you for shopping in our online store, your order №{orderHeader.Id} is already being processed. " +
            //    $"<p></hr><b>Total price: {orderHeader.OrderTotal} zl</b></p>" +
            //    $"<p>In the near future you will receive an email with the tracking number of the parcel.</p></body></html>";
            //message.IsBodyHtml = true;
            //smtpClient.Send(message);



            return View(orderHeader);
        }


        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart

                _unitOfWork.ShoppingCart.Remove(cartFromDb);
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

            _unitOfWork.ShoppingCart.Remove(cartFromDb);

            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
              .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }



        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
