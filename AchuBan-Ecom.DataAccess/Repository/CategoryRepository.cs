using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Data;
using AchuBan_ECom.Models;
using Microsoft.EntityFrameworkCore;

namespace AchuBan_Ecom.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            _db.Update(category);
        }

    }
}
