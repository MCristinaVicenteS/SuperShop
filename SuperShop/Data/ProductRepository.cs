using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using System.Collections.Generic;
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

        public IEnumerable<SelectListItem> GetComboProducts()
        {
            var list =_context.Products.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select a product...",
                Value = "0"
            });

            return list;
        }
    }
}
