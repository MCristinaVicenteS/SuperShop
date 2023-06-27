using SuperShop.Data.Entities;
using SuperShop.Models;
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

        //task q vai adicionar itens à lista temporária
        Task AddItemToOrderAsync(AddItemViewModel mode, string userName);

        //task para poder alterar a qt de um item já adicionado à lista temporária
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);

        //task para apagar -> recebe o id do detailTemp q tem de apagar e apga
        Task DeleteDetailTempAsync(int id);

        //task para confirmar se está td bem ou n com a encomenda
        Task<bool> ConfirmOrderAsync(string userName);
    }
}
