using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    //vai aceder ao datacontext -> inserir no starup(injecções das dependências)
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly DataContext _context;

        
        //injectar no controlador o datacontext
        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        //aceder a todos os objectos do tipo IEntity
        public IQueryable<T> GetAll()
        {
            //vai à tabela T e trás toda a informação; e desliga a conexão à tabela
            return _context.Set<T>().AsNoTracking();
        }

        //Aceder apenas a um produto através do id
        public async Task<T> GetByIdAsync (int id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        //Adicionar um produto em memória e BD
        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync();
        }

        //fazer o update
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await SaveAllAsync();
        }

        //apagar
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveAllAsync();
        }

        //ver se o produto existe, segundo o id
        public async Task<bool> ExistAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id);
        }

        //este método n existe no interface - gravar na BD
        private async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
