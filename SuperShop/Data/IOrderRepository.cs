using SuperShop.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        //task q vai devolver uma tabela do tipo order - para ir buscar td as encomendas associadas a cada user
        Task<IQueryable<Order>> GetOrderAsync(string userName);

        //task q vai devolcer uma tabela do tipo orderDetailTemp - tabela temporária do user
        Task<IQueryable<OrderDetailTemp>> GetDetailTempAsync(string userName);
    }
}
