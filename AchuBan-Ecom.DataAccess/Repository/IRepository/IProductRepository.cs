using AchuBan_ECom.Models.Models;

namespace AchuBan_Ecom.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
