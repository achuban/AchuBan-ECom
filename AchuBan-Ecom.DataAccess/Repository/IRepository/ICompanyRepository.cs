using AchuBan_ECom.Models.Models;

namespace AchuBan_Ecom.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);

        // Eagerly load navigation for list pages
        //IEnumerable<Company> GetAllWithCategory();
    }
}
