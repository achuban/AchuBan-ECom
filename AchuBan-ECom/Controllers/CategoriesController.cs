using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AchuBan_ECom.Data;
using AchuBan_ECom.Models;
using Microsoft.Data.SqlClient;

namespace AchuBan_ECom.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,displayOrder")] Category category)
        {
            if (!ModelState.IsValid) return View(category);

            _context.Add(category);
            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Detect SQL Server duplicate-key error numbers (2627, 2601)
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    TempData["error"] = "A category with this name already exists.";
                }
                else
                {
                    var msg = baseEx?.Message ?? ex.Message;
                    if (!string.IsNullOrEmpty(msg) &&
                        (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        ModelState.AddModelError("Name", "A category with this name already exists.");
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,displayOrder")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (!ModelState.IsValid) return View(category);

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
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
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    TempData["error"] = "A category with this name already exists.";
                    return View(category);
                }

                var msg = baseEx?.Message ?? ex.Message;
                if (!string.IsNullOrEmpty(msg) &&
                    (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    TempData["error"] = "A category with this name already exists.";
                    return View(category);
                }

                TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                return View(category);
            }
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null) _context.Categories.Remove(category);

            await _context.SaveChangesAsync();
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
