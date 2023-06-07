using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using System.Linq;

namespace SuperShop.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DataContext _context;

        //transfere para o "pai" - genericrepository -> para dp ser usado nos seus métodos
        public ProductRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        //trás os produtos com os users -> é um inerjoin das duas tabelas
        public IQueryable GetAllWithUsers()
        {
            return _context.Products.Include(p => p.User);
        }
    }
}
