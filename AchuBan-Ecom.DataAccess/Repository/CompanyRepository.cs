using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Data;
using AchuBan_ECom.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace AchuBan_Ecom.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            _db.Update(company);
        }

        //public IEnumerable<Company> GetAllWithCategory()
        //{
        //    return _db.Companies
        //              .Include(p => p.category)
        //              .ToList();
        //}
    }
}
