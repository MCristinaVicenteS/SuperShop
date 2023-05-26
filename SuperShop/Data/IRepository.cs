using SuperShop.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface IRepository
    {
        void AddProdutct(Product produtct);
        Product GetProduct(int id);
        IEnumerable<Product> GetProducts();
        bool ProductExists(int id);
        void RemoveProdutct(Product product);
        Task<bool> SaveAllAsync();
        void UpdateProduct(Product produtct);
    }
}