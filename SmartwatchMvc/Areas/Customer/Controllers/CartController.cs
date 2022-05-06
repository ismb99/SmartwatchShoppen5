using Data_Access_Layer.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Services;
using Stripe.Checkout;
using System.Security.Claims;

namespace SmartwatchMvc.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartVm ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVm shoppingCartVm = new ShoppingCartVm()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                OrderHeader = new()
                
            };
            foreach (var cart in shoppingCartVm.ListCart)
            {
                shoppingCartVm.OrderHeader.OrderTotal += cart.Product.Price * cart.Amount;
            }
            return View(shoppingCartVm);
        }

        // GET
        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVm ShoppingCartVm = new ShoppingCartVm()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FindByCondition(x => x.Id == claim.Value);

            ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.ApplicationUser.Name;
            ShoppingCartVm.OrderHeader.StreetAdress = ShoppingCartVm.OrderHeader.ApplicationUser.StreetAdress;
            ShoppingCartVm.OrderHeader.PostTown = ShoppingCartVm.OrderHeader.ApplicationUser.PostTown;
            ShoppingCartVm.OrderHeader.City = ShoppingCartVm.OrderHeader.ApplicationUser.City;
            ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;


            foreach (var cart in ShoppingCartVm.ListCart)
            {
                ShoppingCartVm.OrderHeader.OrderTotal += cart.Product.Price * cart.Amount;
            }
            return View(ShoppingCartVm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(ShoppingCartVm ShoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.
                GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product");

          
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
           

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Product.Price * cart.Amount;
            }

            _unitOfWork.OrderHeader.Create(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Product.Price,
                    Count = cart.Amount
                };
                _unitOfWork.OrderDetail.Create(orderDetail);
                _unitOfWork.Save();
            }

            // Stripe Settings
            //var domain = "https://localhost:7049/"; 
            var domain = "https://smartwatchshoppen.azurewebsites.net/";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
       
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain+$"customer/cart/index"
            };

            foreach (var item in ShoppingCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price*100),
                        Currency = "sek",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        },

                    },
                    Quantity = item.Amount,
                };
                options.LineItems.Add(sessionLineItem);
              };

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

               
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.FindByCondition(o => o.Id == id);

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionID);

            if (session.PaymentStatus.ToLower() == "paid")
            {
               _unitOfWork.OrderHeader.UpdateStatus(id,SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            HttpContext.Session.Clear();
            _unitOfWork.ShoppingCart.DeleteMany(shoppingCarts);
            _unitOfWork.Save();
            return View(id);

          

        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetById(cartId);
            _unitOfWork.ShoppingCart.AddAmount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetById(cartId);
            if (cart.Amount <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(cart);
                var cartNum = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count-1;
                HttpContext.Session.SetInt32(SD.SessionCart, cartNum); 
            }
            else
            {
                _unitOfWork.ShoppingCart.ReduceAmount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetById(cartId);
            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Save();
            var cartNum = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.SessionCart, cartNum);
            return RedirectToAction("Index");
        }
    }
}
