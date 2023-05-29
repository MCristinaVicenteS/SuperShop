using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using System.Linq;

namespace SuperShop.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        //transfere para o "pai" - genericrepository -> para dp ser usado nos seus métodos
        public ProductRepository(DataContext context) : base(context)
        {
        }


    }
}
