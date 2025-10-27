using AchuBan_Ecom.DataAccess.Repository;
using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Data;
using AchuBan_ECom.Models;
using AchuBan_ECom.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchuBan_ECom.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Products
        public IActionResult Index()
        {
            var products= _unitOfWork.ProductRepository.GetAll();
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
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Description,ISBN,Author,ListPrice,Price50,Price100,CategoryId")] Product product)
        {
            if (!ModelState.IsValid) return View(product);

            _unitOfWork.ProductRepository.Add(product);
            try
            {
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError(nameof(Product.Name), "A product with this name already exists.");
                    TempData["error"] = "A product with this name already exists.";
                }
                else
                {
                    var msg = baseEx?.Message ?? ex.Message;
                    if (!string.IsNullOrEmpty(msg) &&
                        (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        ModelState.AddModelError(nameof(Product.Name), "A product with this name already exists.");
                        TempData["error"] = "A product with this name already exists.";
                    }
                    else
                    {
                        TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                    }
                }
            }

            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = _unitOfWork.ProductRepository.Get(c => c.Id == id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description,ISBN,Author,ListPrice,Price50,Price100,CategoryId")] Product product)
        {
            if (id != product.Id) return NotFound();
            if (!ModelState.IsValid) return View(product);

            try
            {
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id)) return NotFound();
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
                    return View(product);
                }

                var msg = baseEx?.Message ?? ex.Message;
                if (!string.IsNullOrEmpty(msg) &&
                    (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    ModelState.AddModelError(nameof(Product.Name), "A product with this name already exists.");
                    TempData["error"] = "A product with this name already exists.";
                    return View(product);
                }

                TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                return View(product);
            }
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
    }
}

