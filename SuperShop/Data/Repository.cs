using SuperShop.Data.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    //vai aceder ao datacontext -> inserir no starup(injecções das dependências)
    //criar o interface -> IRepository
    public class Repository : IRepository
    
    {
        private readonly DataContext _context;

        //injectar no controlador o datacontext
        public Repository(DataContext context)
        {
            _context = context;
        }

        //aceder a todos os produtos
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.OrderBy(p => p.Name);
        }

        //Aceder apenas a um produto através do id
        public Product GetProduct(int id)
        {
            return _context.Products.Find(id);
        }

        //Adicionar um produto em memória
        public void AddProdutct(Product produtct)
        {
            _context.Products.Add(produtct);
        }

        //fazer o update
        public void UpdateProduct(Product produtct)
        {
            _context.Products.Update(produtct);
        }

        //apagar
        public void RemoveProdutct(Product product)
        {
            _context.Products.Remove(product);
        }

        //Gravar para a base de dados - tem se ser assinc
        public async Task<bool> SaveAllAsync()
        {
            //grava tudo o q está pendente para a base de dados
            return await _context.SaveChangesAsync() > 0;
        }

        //ver se o produto existe, segundo o id
        public bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.Id == id);
        }
    }
}
