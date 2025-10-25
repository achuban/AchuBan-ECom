using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using AchuBan_ECom.Models;
using AchuBan_Ecom.DataAccess.Repository.IRepository;

namespace AchuBan_ECom.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = _unitOfWork.CategoryRepository.GetAll();
            return View(categories);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Description,displayOrder")] Category category)
        {
            if (!ModelState.IsValid) return View(category);

            _unitOfWork.CategoryRepository.Add(category);
            try
            {
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");
                    TempData["error"] = "A category with this name already exists.";
                }
                else
                {
                    var msg = baseEx?.Message ?? ex.Message;
                    if (!string.IsNullOrEmpty(msg) &&
                        (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");
                        TempData["error"] = "A category with this name already exists.";
                    }
                    else
                    {
                        TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                    }
                }
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description,displayOrder")] Category category)
        {
            if (id != category.Id) return NotFound();
            if (!ModelState.IsValid) return View(category);

            try
            {
                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id)) return NotFound();
                TempData["error"] = "Unable to save changes. Please try again.";
                throw;
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");
                    TempData["error"] = "A category with this name already exists.";
                    return View(category);
                }

                var msg = baseEx?.Message ?? ex.Message;
                if (!string.IsNullOrEmpty(msg) &&
                    (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");
                    TempData["error"] = "A category with this name already exists.";
                    return View(category);
                }

                TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                return View(category);
            }
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (category != null)
            {
                _unitOfWork.CategoryRepository.Remove(category);
                _unitOfWork.Save();
                TempData["success"] = "Category deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _unitOfWork.CategoryRepository.Get(c => c.Id == id) != null;
        }
    }
}
