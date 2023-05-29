using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    //vai receber uma classe/entidade T -> neste caso é uma classe
    //fazer uma interface q se adapte a td e n só produtos
    //Nesta interface genérica n se adiciona o método para gravar na BD
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll(); //devolve todas as entidades q o T estiver a usar

        Task<T> GetByIdAsync(int id); //devolver por id -> se for do tipo IEntity, recebe smp um Id

        Task CreateAsync(T entity); //criar uma entidade do tipo T

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<bool> ExistAsync(int id); //ver se o id existe
    }
}
