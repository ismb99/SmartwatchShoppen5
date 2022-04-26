using Data_Access_Layer.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Diagnostics;
using System.Security.Claims;

namespace SmartwatchMvc.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll();
            return View(products);
        }

        public IActionResult Info(int productId)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                ProductId = productId, 
                Product = _unitOfWork.Product.GetById(productId),
                Amount = 1,
            };
            return View(cart);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Info(ShoppingCart shoppingCart)
        {
            // Hämta inloggad användare
            var userId = (ClaimsIdentity)User.Identity;
            var claim = userId.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
     

            ShoppingCart cart = _unitOfWork.ShoppingCart.FindByCondition(u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

            if (cart == null)
            {
                _unitOfWork.ShoppingCart.Create(shoppingCart);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.
                    GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count);
            }
            else
            {
                _unitOfWork.ShoppingCart.AddAmount(cart, shoppingCart.Amount); // lägg till i varukrog utan att skapa ny order
                _unitOfWork.Save();

            }

            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}