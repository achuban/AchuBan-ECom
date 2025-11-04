using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Models;
using AchuBan_ECom.Models.Models;
using AchuBan_ECom.Models.ViewModels;
using AchuBan_ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AchuBan_ECom.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompaniesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompaniesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Companies
        public async Task<IActionResult> Index()
        {
            var companies = _unitOfWork.CompanyRepository.GetAll();
            return View(companies);

        }

        // GET: Admin/Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var company = _unitOfWork.CompanyRepository.Get(c => c.Id == id.Value);
            if (company == null) return NotFound();

            return View(company);
        }

        // GET: Admin/Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,TinNumber,StreetAdress,City,Region,PostalCode,Country,Phone")] Company company)
        {
            if (!ModelState.IsValid) return View(company);

            _unitOfWork.CompanyRepository.Add(company);
            try
            {
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError(nameof(Company.Name), "A company with this name already exists.");
                    TempData["error"] = "A company with this name already exists.";
                }
                else
                {
                    var msg = baseEx?.Message ?? ex.Message;
                    if (!string.IsNullOrEmpty(msg) &&
                        (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        ModelState.AddModelError(nameof(Company.Name), "A company with this name already exists.");
                        TempData["error"] = "A company with this name already exists.";
                    }
                    else
                    {
                        TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                    }
                }
            }

            return View(company);
        }

        // GET: Admin/Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var company = _unitOfWork.CompanyRepository.Get(c => c.Id == id.Value);
            if (company == null) return NotFound();

            return View(company);
        }

        // POST: Admin/Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TinNumber,StreetAdress,City,Region,PostalCode,Country,Phone")] Company company)
        {
            if (id != company.Id) return NotFound();
            if (!ModelState.IsValid) return View(company);

            try
            {
                _unitOfWork.CompanyRepository.Update(company);
                _unitOfWork.Save();
                TempData["success"] = "Company updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(company.Id)) return NotFound();
                TempData["error"] = "Unable to save changes. Please try again.";
                throw;
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError(nameof(Company.Name), "A company with this name already exists.");
                    TempData["error"] = "A company with this name already exists.";
                    return View(company);
                }

                var msg = baseEx?.Message ?? ex.Message;
                if (!string.IsNullOrEmpty(msg) &&
                    (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    ModelState.AddModelError(nameof(Company.Name), "A company with this name already exists.");
                    TempData["error"] = "A company with this name already exists.";
                    return View(company);
                }

                TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                return View(company);
            }
        }

        // GET: Admin/Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var company = _unitOfWork.CompanyRepository.Get(c => c.Id == id.Value);
            if (company == null) return NotFound();

            return View(company);
        }

        // POST: Admin/Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
            if (company != null)
            {
                _unitOfWork.CompanyRepository.Remove(company);
                _unitOfWork.Save();
                TempData["success"] = "Company deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _unitOfWork.CompanyRepository.Get(c => c.Id == id) != null;

        }

        // GET: Admin/Companys/Upsert/5
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company companies;
            if (id == null || id == 0)
            {
                companies = new Company();
            }
            else
            {
                var company = _unitOfWork.CompanyRepository.Get(p => p.Id == id.Value);
                if (company == null) return NotFound();
                else companies = company;
            }

            return View("Upsert", companies);
        }

        // POST: Admin/Companys/Upsert
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company companies)
        {
            if (companies == null )
            {
                var vm = companies ?? new Company();
                return View("Upsert", vm);
            }

            if (!ModelState.IsValid)
            {
                return View("Upsert", companies);
            }

            try
            {
                var isNew = companies.Id == 0;
                if (isNew)
                {
                    _unitOfWork.CompanyRepository.Add(companies);
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(companies);
                }

                _unitOfWork.Save();

                TempData["success"] = isNew ? "Company created successfully." : "Company updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(companies.Id)) return NotFound();
                TempData["error"] = "Unable to save changes. Please try again.";
                throw;
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                if (baseEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    ModelState.AddModelError(nameof(Company.Name), "A company with this name already exists.");
                    TempData["error"] = "A company with this name already exists.";
                    return View("Upsert", companies);
                }

                var msg = baseEx?.Message ?? ex.Message;
                if (!string.IsNullOrEmpty(msg) &&
                    (msg.IndexOf("IX_Categories_Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    ModelState.AddModelError(nameof(Company.Name), "A company with this name already exists.");
                    TempData["error"] = "A company with this name already exists.";
                    return View("Upsert", companies);
                }

                TempData["error"] = "Unable to save changes. Try again, and if the problem persists contact the administrator.";
                return View("Upsert", companies);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companyList = _unitOfWork.CompanyRepository.GetAll().ToList();
            return Json(new { data = companyList });
        }
    }
}
