using SuperShop.Data.Entities;
using System.Linq;

namespace SuperShop.Data
{
    //inserir no Startup
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IQueryable GetAllWithUsers();
    }
}
