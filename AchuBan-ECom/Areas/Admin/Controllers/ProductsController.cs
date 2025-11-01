using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Models;
using AchuBan_ECom.Models.Models;
using AchuBan_ECom.Models.ViewModels;
using AchuBan_ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AchuBan_ECom.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/Products
        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetAll().ToList();
            var categories = _unitOfWork.CategoryRepository.GetAll().ToDictionary(c => c.Id);
            foreach (var p in products)
                if (categories.TryGetValue(p.CategoryId, out var cat))
                    p.category = cat;
            return View(products);
        }

        // GET: Admin/Products/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var product = _unitOfWork.ProductRepository.Get(c => c.Id == id.Value);
            if (product == null) return NotFound();

            return View(product);
        }


        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ProductVM productVM = new()
            {
                Product = new Product
                {
                    category = new Category()
                },
                CategoryList = GetCategorySelectList()
            };
            // render the shared Upsert view for create
            return View("Upsert", productVM);
        }

        // helper to build category select list
        private IEnumerable<SelectListItem> GetCategorySelectList(int? selectedId = null)
        {
            return _unitOfWork.CategoryRepository
                .GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                    Selected = selectedId.HasValue && c.Id == selectedId.Value
                })
                .ToList();
        }

        // GET: Admin/Products/Edit/5  (use helper)
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = _unitOfWork.ProductRepository.Get(c => c.Id == id.Value);
            if (product == null) return NotFound();

            ProductVM productVM = new()
            {
                Product = product,
                CategoryList = GetCategorySelectList(product.CategoryId)
            };

            // render the shared Upsert view for edit
            return View("Upsert", productVM);
        }

        // GET: Admin/Products/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = _unitOfWork.ProductRepository.Get(c => c.Id == id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _unitOfWork.ProductRepository.Get(c => c.Id == id);
            if (product != null)
            {
                _unitOfWork.ProductRepository.Remove(product);
                _unitOfWork.Save();
                TempData["success"] = "Product deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _unitOfWork.ProductRepository.Get(c => c.Id == id) != null;
        }

        // GET: Admin/Products/Upsert/5
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM;
            if (id == null || id == 0)
            {
                productVM = new ProductVM
                {
                    Product = new Product { category = new Category() },
                    CategoryList = GetCategorySelectList()
                };
            }
            else
            {
                var product = _unitOfWork.ProductRepository.Get(p => p.Id == id.Value);
                if (product == null) return NotFound();

                productVM = new ProductVM
                {
                    Product = product,
                    CategoryList = GetCategorySelectList(product.CategoryId)
                };
            }

            return View("Upsert", productVM);
        }

        // POST: Admin/Products/Upsert
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (productVM == null || productVM.Product == null)
            {
                var vm = productVM ?? new ProductVM
                {
                    Product = new Product
                    {
                        category = new Category()
                    },
                    CategoryList = GetCategorySelectList()
                };
                return View("Upsert", vm);
            }

            if (!ModelState.IsValid)
            {
                productVM.CategoryList = GetCategorySelectList(productVM.Product.CategoryId);
                return View("Upsert", productVM);
            }

            try
            {
                // handle uploaded image
                if (productVM.ImageFile != null && productVM.ImageFile.Length > 0)
                {
                    var wwwRootPath = _hostEnvironment.WebRootPath;
                    var uploadsFolder = Path.Combine(wwwRootPath, "images", "products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // delete old image if present (only if it's inside wwwroot)
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        try
                        {
                            var oldRelative = productVM.Product.ImageUrl.TrimStart('/');
                            var oldFullPath = Path.Combine(wwwRootPath,
                                oldRelative.Replace('/', Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(oldFullPath))
                                System.IO.File.Delete(oldFullPath);
                        }
                        catch
                        {
                            // ignore deletion errors
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productVM.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        productVM.ImageFile.CopyTo(stream);
                    }

                    productVM.Product.ImageUrl = $"/images/products/{fileName}";
                }

                var isNew = productVM.Product.Id == 0;
                if (isNew)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }

                _unitOfWork.Save();

                TempData["success"] = isNew ? "Product created successfully." : "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(productVM.Product.Id)) return NotFound();
                TempData["error"] = "Unable to save changes. Please try again.";
                throw;
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError(nameof(Product.Name), "A product with this name already exists.");
                    TempData["error"] = "A product with this name already exists.";
                    productVM.CategoryList = GetCategorySelectList(productVM.Product?.CategoryId);
                    return View("Upsert", productVM);
                }

                var msg = baseEx?.Message ?? ex.Message;
                if (!string.IsNullOrEmpty(msg) &&
                    (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    ModelState.AddModelError(nameof(Product.Name), "A product with this name already exists.");
                    TempData["error"] = "A product with this name already exists.";
                    productVM.CategoryList = GetCategorySelectList(productVM.Product?.CategoryId);
                    return View("Upsert", productVM);
                }

                TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                productVM.CategoryList = GetCategorySelectList(productVM.Product?.CategoryId);
                return View("Upsert", productVM);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productList = _unitOfWork.ProductRepository.GetAllWithCategory().ToList();    
            return Json(new { data = productList });
        }

    }
}