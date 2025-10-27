using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Data;
using AchuBan_ECom.Models.Models;

namespace AchuBan_Ecom.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            _db.Update(product);
        }

    }
}
