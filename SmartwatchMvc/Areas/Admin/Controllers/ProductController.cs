using Data_Access_Layer.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;

namespace SmartwatchMvc.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll();   
            return View(productList);
        }


        public IActionResult CreateOrUpdate(int? id)
        {
            var product = new Product();
            // Dropdown
            //ProductVM productVM = new()
            //{
            //    product  = new(),
            //    categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            //    {
            //        Text = c.Name,
            //        Value = c.Id.ToString()
            //    }),
              
            //    wristbandList = _unitOfWork.WristBand.GetAll().Select(w => new SelectListItem
            //    {
            //        Text = w.Color,
            //        Value = w.Id.ToString(),
            //    })
            //};
          
            if (id == null || id == 0)
            {
                // Skapa product
                //return View(productVM);
                return View(product);   
            }
            else
            {
                //Updatera
                //productVM.product = _unitOfWork.Product.GetById(id);
                //return View(productVM);
                product = _unitOfWork.Product.GetById(id);
                return View(product);
            }
           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrUpdate(Product product, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    product.ImageUrl = @"\images\products\" + fileName + extension;

                }
                if (product.Id == 0)
                {
                    _unitOfWork.Product.Create(product);
                }
                else
                {
                    _unitOfWork.Product.Update(product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View(product);
        }


        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
                return NotFound();

            var product = _unitOfWork.Product.GetById(id);

            if (product == null)
                return NotFound();

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id) 
        {
            var product = _unitOfWork.Product.GetById(id);

            if (product == null)
                return NotFound();

            string oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\')); // Path till gammal bild
            if (System.IO.File.Exists(oldImgPath)) // om fil exsisterar radera den
            {
                System.IO.File.Delete(oldImgPath);
            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();

            return RedirectToAction("Index");
           
        }

    }
}
