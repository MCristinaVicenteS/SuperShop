using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    //responsável pela ligação à BD - herda de um identity e recebe um objecto user
    public class DataContext : IdentityDbContext<User>
    {
        //Criar tabelas
        public DbSet<Product> Products { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
