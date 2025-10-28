using System.Diagnostics;
using System.Linq;
using AchuBan_ECom.Models;
using AchuBan_ECom.Models.Models;
using AchuBan_Ecom.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace AchuBan_ECom.Areas.Customer.Controllers
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
            // return products with category populated
            var products = _unitOfWork.ProductRepository.GetAllWithCategory().ToList();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Customer/Home/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var product = _unitOfWork.ProductRepository
                .GetAllWithCategory()
                .FirstOrDefault(p => p.Id == id.Value);

            if (product == null) return NotFound();

            return View(product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}